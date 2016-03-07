"use strict";

/// <reference path="..\schema\ifile.d.ts" />
// import { Promise } from "es6-promise";
import { replaceUrlTokens } from "../../../Util";
import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";

export class ObjectFiles extends ObjectHandlerBase {
    constructor() {
        super("Files");
    }
    public ProvisionObjects(objects: Array<IFile>) {
        super.scope_started();
        return new Promise((resolve, reject) => {
            const clientContext = SP.ClientContext.get_current();
            const web = clientContext.get_web();
            const fileInfos: Array<IFileInfo> = [];
            let promises = [];
            objects.forEach((obj, index) => {
                const filename = this.GetFilenameFromFilePath(obj.Dest);
                const webServerRelativeUrl = _spPageContextInfo.webServerRelativeUrl;
                const folder = web.getFolderByServerRelativeUrl(`${webServerRelativeUrl}/${this.GetFolderFromFilePath(obj.Dest)}`);
                promises.push(jQuery.get(replaceUrlTokens(obj.Src), (fileContents) => {
                    let f: any = {};
                    jQuery.extend(f, obj, { "Filename": filename, "Folder": folder, "Contents": fileContents });
                    fileInfos.push(f);
                }));
            });
            jQuery.when.apply(jQuery, promises).done(() => {
                fileInfos.forEach((f, index) => {
                    if (f.Filename.indexOf("Form.aspx") !== -1) {
                        return;
                    }
                    let objCreationInformation = new SP.FileCreationInformation();
                    objCreationInformation.set_overwrite(f.Overwrite !== undefined ? f.Overwrite : false);
                    objCreationInformation.set_url(f.Filename);
                    objCreationInformation.set_content(new SP.Base64EncodedByteArray());
                    for (let i = 0; i < f.Contents.length; i++) {
                        objCreationInformation.get_content().append(f.Contents.charCodeAt(i));
                    }
                    clientContext.load(f.Folder.get_files().add(objCreationInformation));
                });

                clientContext.executeQueryAsync(() => {
                    promises = [];
                    objects.forEach((obj) => {
                        if (obj.Properties && Object.keys(obj.Properties).length > 0) {
                            promises.push(this.ApplyFileProperties(obj.Dest, obj.Properties));
                        }
                        if (obj.WebParts && obj.WebParts.length > 0) {
                            promises.push(this.AddWebPartsToWebPartPage(obj.Dest, obj.Src, obj.WebParts, obj.RemoveExistingWebParts));
                        }
                    });
                    Promise.all(promises).then(() => {
                        this.ModifyHiddenViews(objects).then(() => {
                            super.scope_ended();
                            resolve();
                        }, () => {
                            super.scope_ended();
                            resolve();
                        });
                    });
                }, () => {
                    super.scope_ended();
                    resolve();
                });
            });
        });
    }
    private RemoveWebPartsFromFileIfSpecified(
        clientContext: SP.ClientContext,
        limitedWebPartManager: SP.WebParts.LimitedWebPartManager,
        shouldRemoveExisting) {
        return new Promise((resolve, reject) => {
            if (!shouldRemoveExisting) {
                resolve();
            }
            let existingWebParts = limitedWebPartManager.get_webParts();
            clientContext.load(existingWebParts);
            clientContext.executeQueryAsync(
                () => {
                    existingWebParts.get_data().forEach((wp) => {
                        wp.deleteWebPart();
                    });
                    clientContext.load(existingWebParts);
                    clientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
        });
    }
    private GetWebPartXml(webParts: Array<IWebPart>) {
        return new Promise((resolve, reject) => {
            let promises = [];
            webParts.forEach((wp, index) => {
                if (wp.Contents.FileUrl) {
                    promises.push((() => {
                        return new Promise((res, rej) => {
                            let fileUrl = replaceUrlTokens(wp.Contents.FileUrl);
                            jQuery.get(fileUrl, (xml) => {
                                webParts[index].Contents.Xml = xml;
                                res();
                            }).fail(rej);
                        });
                    })());
                }
            });

            Promise.all(promises).then(() => {
                resolve(webParts);
            });
        });
    }
    private AddWebPartsToWebPartPage(dest: string, src: string, webParts: Array<IWebPart>, shouldRemoveExisting: Boolean) {
        return new Promise((resolve, reject) => {
            const clientContext = SP.ClientContext.get_current();
            const web = clientContext.get_web();
            let fileServerRelativeUrl = `${_spPageContextInfo.webServerRelativeUrl}/${dest}`;
            let file = web.getFileByServerRelativeUrl(fileServerRelativeUrl);
            clientContext.load(file);
            clientContext.executeQueryAsync(
                () => {
                    let limitedWebPartManager = file.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);
                    this.RemoveWebPartsFromFileIfSpecified(clientContext, limitedWebPartManager, shouldRemoveExisting).then(() => {
                        this.GetWebPartXml(webParts).then((webPartsWithXml: Array<IWebPart>) => {
                            webPartsWithXml.forEach(wp => {
                                if (!wp.Contents.Xml) {
                                    return;
                                }
                                let oWebPartDefinition = limitedWebPartManager.importWebPart(replaceUrlTokens(wp.Contents.Xml));
                                let oWebPart = oWebPartDefinition.get_webPart();
                                limitedWebPartManager.addWebPart(oWebPart, wp.Zone, wp.Order);
                            });
                            clientContext.executeQueryAsync(resolve, resolve);
                        });
                    });
                }, resolve);
        });
    }
    private ApplyFileProperties(dest: string, fileProperties: Object) {
        return new Promise((resolve, reject) => {
            const clientContext = SP.ClientContext.get_current();
            const web = clientContext.get_web();
            let fileServerRelativeUrl = `${_spPageContextInfo.webServerRelativeUrl}/${dest}`;
            let file = web.getFileByServerRelativeUrl(fileServerRelativeUrl);
            let listItemAllFields = file.get_listItemAllFields();
            Object.keys(fileProperties).forEach(key => {
                listItemAllFields.set_item(key, fileProperties[key]);
            });
            listItemAllFields.update();
            clientContext.executeQueryAsync(resolve, resolve);
        });
    }
    private GetViewFromCollectionByUrl(viewCollection: SP.ViewCollection, url: string) {
        const serverRelativeUrl = `${_spPageContextInfo.webServerRelativeUrl}/${url}`;
        const viewCollectionEnumerator = viewCollection.getEnumerator();
        while (viewCollectionEnumerator.moveNext()) {
            const view = viewCollectionEnumerator.get_current();
            if (view.get_serverRelativeUrl().toString().toLowerCase() === serverRelativeUrl.toLowerCase()) {
                return view;
            }
        }
        return null;
    }
    private ModifyHiddenViews(objects: Array<IFile>) {
        return new Promise((resolve, reject) => {
            const clientContext = SP.ClientContext.get_current();
            const web = clientContext.get_web();
            let mapping = {};
            let lists: Array<SP.List> = [];
            let listViewCollections: Array<SP.ViewCollection> = [];

            objects.forEach((obj) => {
                if (!obj.Views) {
                    return;
                }
                obj.Views.forEach((v) => {
                    mapping[v.List] = mapping[v.List] || [];
                    mapping[v.List].push(jQuery.extend(v, { "Url": obj.Dest }));
                });
            });
            Object.keys(mapping).forEach((l, index) => {
                lists.push(web.get_lists().getByTitle(l));
                listViewCollections.push(web.get_lists().getByTitle(l).get_views());
                clientContext.load(lists[index]);
                clientContext.load(listViewCollections[index]);
            });

            clientContext.executeQueryAsync(
                () => {
                    Object.keys(mapping).forEach((l, index) => {
                        let views: Array<IHiddenView> = mapping[l];
                        let list = lists[index];
                        let viewCollection = listViewCollections[index];
                        views.forEach((v) => {
                            let view = this.GetViewFromCollectionByUrl(viewCollection, v.Url);
                            if (view == null) {
                                return;
                            }
                            if (v.Paged) { view.set_paged(v.Paged); }
                            if (v.Query) { view.set_viewQuery(v.Query); }
                            if (v.RowLimit) { view.set_rowLimit(v.RowLimit); }
                            if (v.ViewFields && v.ViewFields.length > 0) {
                                let columns = view.get_viewFields();
                                columns.removeAll();
                                v.ViewFields.forEach((vf) => {
                                    columns.add(vf);
                                });
                            }
                            view.update();
                        });
                        clientContext.load(viewCollection);
                        list.update();
                    });
                    clientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
        });
    }
    private GetFolderFromFilePath(filePath: string) {
        let split = filePath.split("/");
        return split.splice(0, split.length - 1).join("/");
    }
    private GetFilenameFromFilePath(filePath: string) {
        let split = filePath.split("/");
        return split[split.length - 1];
    }
}



interface IFileInfo extends IFile {
    Filename: string;
    Folder: SP.Folder;
    Contents: string;
    ServerRelativeUrl: string;
    Instance: SP.File;
};
