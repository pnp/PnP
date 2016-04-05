"use strict";

/// <reference path="..\schema\ilistinstance.d.ts" />
// import { Promise } from "es6-promise";
import { Sequencer } from "../../Sequencer/Sequencer";
import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";

export class ObjectLists extends ObjectHandlerBase {
    constructor() {
        super("Lists");
    }
    public ProvisionObjects(objects: Array<IListInstance>) {
        super.scope_started();
        return new Promise((resolve, reject) => {
            const clientContext = SP.ClientContext.get_current();
            let lists = clientContext.get_web().get_lists();
            let listInstances: Array<SP.List> = [];

            clientContext.load(lists);
            clientContext.executeQueryAsync(
                () => {
                    objects.forEach((obj, index) => {
                        let existingObj: SP.List = jQuery.grep(lists.get_data(), (list) => {
                            return list.get_title() === obj.Title;
                        })[0];

                        if (existingObj) {
                            if (obj.Description) { existingObj.set_description(obj.Description); }
                            if (obj.EnableVersioning !== undefined) { existingObj.set_enableVersioning(obj.EnableVersioning); }
                            if (obj.EnableMinorVersions !== undefined) { existingObj.set_enableMinorVersions(obj.EnableMinorVersions); }
                            if (obj.EnableModeration !== undefined) { existingObj.set_enableModeration(obj.EnableModeration); }
                            if (obj.EnableFolderCreation !== undefined) { existingObj.set_enableFolderCreation(obj.EnableFolderCreation); }
                            if (obj.EnableAttachments !== undefined) { existingObj.set_enableAttachments(obj.EnableAttachments); }
                            if (obj.NoCrawl !== undefined) { existingObj.set_noCrawl(obj.NoCrawl); }
                            if (obj.DefaultDisplayFormUrl) { existingObj.set_defaultDisplayFormUrl(obj.DefaultDisplayFormUrl); }
                            if (obj.DefaultEditFormUrl) { existingObj.set_defaultEditFormUrl(obj.DefaultEditFormUrl); }
                            if (obj.DefaultNewFormUrl) { existingObj.set_defaultNewFormUrl(obj.DefaultNewFormUrl); }
                            if (obj.DraftVersionVisibility) {
                                existingObj.set_draftVersionVisibility(SP.DraftVisibilityType[obj.DraftVersionVisibility]);
                            }
                            if (obj.ImageUrl) { existingObj.set_imageUrl(obj.ImageUrl); }
                            if (obj.Hidden !== undefined) { existingObj.set_hidden(obj.Hidden); }
                            if (obj.ForceCheckout !== undefined) { existingObj.set_forceCheckout(obj.ForceCheckout); }
                            existingObj.update();
                            listInstances.push(existingObj);
                            clientContext.load(listInstances[index]);
                        } else {
                            let objCreationInformation = new SP.ListCreationInformation();
                            if (obj.Description) { objCreationInformation.set_description(obj.Description); }
                            if (obj.OnQuickLaunch !== undefined) {
                                let value = obj.OnQuickLaunch ? SP.QuickLaunchOptions.on : SP.QuickLaunchOptions.off;
                                objCreationInformation.set_quickLaunchOption(value);
                            }
                            if (obj.TemplateType) { objCreationInformation.set_templateType(obj.TemplateType); }
                            if (obj.Title) { objCreationInformation.set_title(obj.Title); }
                            if (obj.Url) { objCreationInformation.set_url(obj.Url); }
                            let createdList = lists.add(objCreationInformation);
                            if (obj.EnableVersioning !== undefined) { createdList.set_enableVersioning(obj.EnableVersioning); }
                            if (obj.EnableMinorVersions !== undefined) { createdList.set_enableMinorVersions(obj.EnableMinorVersions); }
                            if (obj.EnableModeration !== undefined) { createdList.set_enableModeration(obj.EnableModeration); }
                            if (obj.EnableFolderCreation !== undefined) { createdList.set_enableFolderCreation(obj.EnableFolderCreation); }
                            if (obj.EnableAttachments !== undefined) { createdList.set_enableAttachments(obj.EnableAttachments); }
                            if (obj.NoCrawl !== undefined) { createdList.set_noCrawl(obj.NoCrawl); }
                            if (obj.DefaultDisplayFormUrl) { createdList.set_defaultDisplayFormUrl(obj.DefaultDisplayFormUrl); }
                            if (obj.DefaultEditFormUrl) { createdList.set_defaultEditFormUrl(obj.DefaultEditFormUrl); }
                            if (obj.DefaultNewFormUrl) { createdList.set_defaultNewFormUrl(obj.DefaultNewFormUrl); }
                            if (obj.DraftVersionVisibility) {
                                let value = SP.DraftVisibilityType[obj.DraftVersionVisibility.toLocaleLowerCase()];
                                createdList.set_draftVersionVisibility(value);
                            }
                            if (obj.ImageUrl) { createdList.set_imageUrl(obj.ImageUrl); }
                            if (obj.Hidden !== undefined) { createdList.set_hidden(obj.Hidden); }
                            if (obj.ForceCheckout !== undefined) { createdList.set_forceCheckout(obj.ForceCheckout); }
                            listInstances.push(createdList);
                            clientContext.load(listInstances[index]);
                        }
                    });
                    clientContext.executeQueryAsync(
                        () => {
                            let sequencer = new Sequencer([
                                this.ApplyContentTypeBindings,
                                this.ApplyListInstanceFieldRefs,
                                this.ApplyFields,
                                this.ApplyLookupFields,
                                this.ApplyListSecurity,
                                this.CreateViews,
                                this.InsertDataRows,
                                this.CreateFolders,
                            ],
                                { ClientContext: clientContext, ListInstances: listInstances, Objects: objects }, this);
                            sequencer.execute().then(() => {
                                super.scope_ended();
                                resolve();
                            });
                        }, () => {
                            super.scope_ended();
                            resolve();
                        });
                }, () => {
                    super.scope_ended();
                    resolve();
                });
        });
    }
    private EnsureLocationBasedMetadataDefaultsReceiver(clientContext: SP.ClientContext, list: SP.List) {
        let eventReceivers = list.get_eventReceivers();
        let eventRecCreationInfo = new SP.EventReceiverDefinitionCreationInformation();
        eventRecCreationInfo.set_receiverName("LocationBasedMetadataDefaultsReceiver ItemAdded");
        eventRecCreationInfo.set_synchronization(1);
        eventRecCreationInfo.set_sequenceNumber(1000);
        eventRecCreationInfo.set_receiverAssembly("Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, " +
            "PublicKeyToken=71e9bce111e9429c");
        eventRecCreationInfo.set_receiverClass("Microsoft.Office.DocumentManagement.LocationBasedMetadataDefaultsReceiver");
        eventRecCreationInfo.set_eventType(SP.EventReceiverType.itemAdded);
        eventReceivers.add(eventRecCreationInfo);
        list.update();
    }
    private CreateFolders(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            params.ListInstances.forEach((l, index) => {
                let obj = params.Objects[index];
                if (!obj.Folders) {
                    return;
                }
                let folderServerRelativeUrl = `${_spPageContextInfo.webServerRelativeUrl}/${obj.Url}`;
                let rootFolder = l.get_rootFolder();
                let metadataDefaults = "<MetadataDefaults>";
                let setMetadataDefaults = false;
                obj.Folders.forEach(f => {
                    let folderUrl = `${folderServerRelativeUrl}/${f.Name}`;
                    rootFolder.get_folders().add(folderUrl);
                    if (f.DefaultValues) {
                        let keys = Object.keys(f.DefaultValues).length;
                        if (keys > 0) {
                            metadataDefaults += `<a href='${folderUrl}'>`;
                            Object.keys(f.DefaultValues).forEach(key => {
                                metadataDefaults += `<DefaultValue FieldName="${key}">${f.DefaultValues[key]}</DefaultValue>`;
                            });
                            metadataDefaults += "</a>";
                        }
                        setMetadataDefaults = true;
                    }
                });
                metadataDefaults += "</MetadataDefaults>";

                if (setMetadataDefaults) {
                    let metadataDefaultsFileCreateInfo = new SP.FileCreationInformation();
                    metadataDefaultsFileCreateInfo.set_url(`${folderServerRelativeUrl}/Forms/client_LocationBasedDefaults.html`);
                    metadataDefaultsFileCreateInfo.set_content(new SP.Base64EncodedByteArray());
                    metadataDefaultsFileCreateInfo.set_overwrite(true);
                    for (let i = 0; i < metadataDefaults.length; i++) {
                        metadataDefaultsFileCreateInfo.get_content().append(metadataDefaults.charCodeAt(i));
                    }
                    rootFolder.get_files().add(metadataDefaultsFileCreateInfo);
                    this.EnsureLocationBasedMetadataDefaultsReceiver(params.ClientContext, l);
                }
            });
            params.ClientContext.executeQueryAsync(resolve, resolve);
        });
    }
    private ApplyContentTypeBindings(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            let webCts = params.ClientContext.get_site().get_rootWeb().get_contentTypes();
            let listCts: Array<SP.ContentTypeCollection> = [];
            params.ListInstances.forEach((l, index) => {
                listCts.push(l.get_contentTypes());
                params.ClientContext.load(listCts[index], "Include(Name,Id)");
                if (params.Objects[index].ContentTypeBindings) {
                    l.set_contentTypesEnabled(true);
                    l.update();
                }
            });
            params.ClientContext.load(webCts);
            params.ClientContext.executeQueryAsync(
                () => {
                    params.ListInstances.forEach((list, index) => {
                        let obj = params.Objects[index];
                        if (!obj.ContentTypeBindings) {
                            return;
                        }
                        let listContentTypes = listCts[index];
                        let existingContentTypes = new Array<SP.ContentType>();
                        if (obj.RemoveExistingContentTypes && obj.ContentTypeBindings.length > 0) {
                            listContentTypes.get_data().forEach(ct => {
                                existingContentTypes.push(ct);
                            });
                        }
                        obj.ContentTypeBindings.forEach(ctb => {
                            listContentTypes.addExistingContentType(webCts.getById(ctb.ContentTypeId));
                        });
                        if (obj.RemoveExistingContentTypes && obj.ContentTypeBindings.length > 0) {
                            for (let j = 0; j < existingContentTypes.length; j++) {
                                let ect = existingContentTypes[j];
                                ect.deleteObject();
                            }
                        }
                        list.update();
                    });

                    params.ClientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
        });
    }
    private ApplyListInstanceFieldRefs(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            let siteFields = params.ClientContext.get_site().get_rootWeb().get_fields();
            params.ListInstances.forEach((l, index) => {
                let obj = params.Objects[index];
                if (obj.FieldRefs) {
                    obj.FieldRefs.forEach(fr => {
                        let field = siteFields.getByInternalNameOrTitle(fr.Name);
                        l.get_fields().add(field);
                    });
                    l.update();
                }
            });
            params.ClientContext.executeQueryAsync(resolve, resolve);
        });
    }
    private ApplyFields(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            params.ListInstances.forEach((l, index) => {
                let obj = params.Objects[index];
                if (obj.Fields) {
                    obj.Fields.forEach(f => {
                        let fieldXml = this.GetFieldXml(f, params.ListInstances, l);
                        let fieldType = this.GetFieldXmlAttr(fieldXml, "Type");
                        if (fieldType !== "Lookup" && fieldType !== "LookupMulti") {
                            l.get_fields().addFieldAsXml(fieldXml, true, SP.AddFieldOptions.addToAllContentTypes);
                        }
                    });
                    l.update();
                }
            });
            params.ClientContext.executeQueryAsync(resolve, resolve);
        });
    }
    private ApplyLookupFields(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            params.ListInstances.forEach((l, index) => {
                let obj = params.Objects[index];
                if (obj.Fields) {
                    obj.Fields.forEach(f => {
                        let fieldXml = this.GetFieldXml(f, params.ListInstances, l);
                        if (!fieldXml) {
                            return;
                        }
                        let fieldType = this.GetFieldXmlAttr(fieldXml, "Type");
                        if (fieldType === "Lookup" || fieldType === "LookupMulti") {
                            l.get_fields().addFieldAsXml(fieldXml, true, SP.AddFieldOptions.addToAllContentTypes);
                        }
                    });
                    l.update();
                }
            });
            params.ClientContext.executeQueryAsync(resolve, resolve);
        });
    }
    private GetFieldXmlAttr(fieldXml: string, attr: string) {
        return $(jQuery.parseXML(fieldXml)).find("Field").attr(attr);
    }
    private GetFieldXml(field: IField, lists: Array<SP.List>, list: SP.List) {
        let fieldXml = "";
        if (!field.SchemaXml) {
            let properties = [];
            Object.keys(field).forEach(prop => {
                let value = field[prop];
                if (prop === "List") {
                    let targetList = jQuery.grep(lists, v => {
                        return v.get_title() === value;
                    });
                    if (targetList.length > 0) {
                        value = `{${targetList[0].get_id().toString()}}`;
                    } else {
                        return null;
                    }
                    properties.push(`${prop}="${value}"`);
                }
            });
            fieldXml = `<Field ${properties.join(" ")}>`;
            if (field.Type === "Calculated") {
                fieldXml += `<Formula>${field.Formula}</Formula>`;
            }
            fieldXml += "</Field>";

        }
        return fieldXml;
    }
    private ApplyListSecurity(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            params.ListInstances.forEach((l, index) => {
                let obj = params.Objects[index];
                if (!obj.Security) {
                    return;
                }
                if (obj.Security.BreakRoleInheritance) {
                    l.breakRoleInheritance(obj.Security.CopyRoleAssignments, obj.Security.ClearSubscopes);
                    l.update();
                    params.ClientContext.load(l.get_roleAssignments());
                }
            });

            let web = params.ClientContext.get_web();
            let allProperties = web.get_allProperties();
            let siteGroups = web.get_siteGroups();
            let roleDefinitions = web.get_roleDefinitions();

            params.ClientContext.load(allProperties);
            params.ClientContext.load(roleDefinitions);
            params.ClientContext.executeQueryAsync(
                () => {
                    params.ListInstances.forEach((l, index) => {
                        let obj = params.Objects[index];
                        if (!obj.Security) {
                            return;
                        }
                        obj.Security.RoleAssignments.forEach(ra => {
                            let roleDef = null;
                            if (typeof ra.RoleDefinition === "number") {
                                roleDef = roleDefinitions.getById(ra.RoleDefinition);
                            } else {
                                roleDef = roleDefinitions.getByName(ra.RoleDefinition);
                            }
                            let roleBindings = SP.RoleDefinitionBindingCollection.newObject(params.ClientContext);
                            roleBindings.add(roleDef);
                            let principal = null;
                            if (ra.Principal.match(/\{[A-Za-z]*\}+/g)) {
                                let token = ra.Principal.substring(1, ra.Principal.length - 1);
                                let groupId = allProperties.get_fieldValues()[`vti_${token}`];
                                principal = siteGroups.getById(groupId);
                            } else {
                                principal = siteGroups.getByName(principal);
                            }
                            l.get_roleAssignments().add(principal, roleBindings);
                        });
                        l.update();
                    });
                    params.ClientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
        });
    }
    private CreateViews(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            let listViewCollections: Array<SP.ViewCollection> = [];
            params.ListInstances.forEach((l, index) => {
                listViewCollections.push(l.get_views());
                params.ClientContext.load(listViewCollections[index]);
            });
            params.ClientContext.executeQueryAsync(
                () => {
                    params.ListInstances.forEach((l, index) => {
                        let obj = params.Objects[index];
                        if (!obj.Views) {
                            return;
                        }

                        listViewCollections.push(l.get_views());
                        params.ClientContext.load(listViewCollections[index]);
                        obj.Views.forEach((v) => {
                            let viewExists = jQuery.grep(listViewCollections[index].get_data(), (ev) => {
                                if (obj.RemoveExistingViews && obj.Views.length > 0) {
                                    ev.deleteObject();
                                    return false;
                                }
                                return ev.get_title() === v.Title;
                            }).length > 0;

                            if (viewExists) {
                                let view = listViewCollections[index].getByTitle(v.Title);
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
                                if (v.Scope) { view.set_scope(v.Scope); }
                                view.update();
                            } else {
                                let viewCreationInformation = new SP.ViewCreationInformation();
                                if (v.Title) { viewCreationInformation.set_title(v.Title); }
                                if (v.PersonalView) { viewCreationInformation.set_personalView(v.PersonalView); }
                                if (v.Paged) { viewCreationInformation.set_paged(v.Paged); }
                                if (v.Query) { viewCreationInformation.set_query(v.Query); }
                                if (v.RowLimit) { viewCreationInformation.set_rowLimit(v.RowLimit); }
                                if (v.SetAsDefaultView) { viewCreationInformation.set_setAsDefaultView(v.SetAsDefaultView); }
                                if (v.ViewFields) { viewCreationInformation.set_viewFields(v.ViewFields); }
                                if (v.ViewTypeKind) { viewCreationInformation.set_viewTypeKind(SP.ViewType.html); }
                                let view = l.get_views().add(viewCreationInformation);
                                if (v.Scope) {
                                    view.set_scope(v.Scope);
                                    view.update();
                                }
                                l.update();
                            }
                            params.ClientContext.load(l.get_views());
                        });
                    });
                    params.ClientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
        });
    }
    private InsertDataRows(params: IFunctionParams) {
        return new Promise((resolve, reject) => {
            params.ListInstances.forEach((l, index) => {
                let obj = params.Objects[index];
                if (obj.DataRows) {
                    obj.DataRows.forEach(r => {
                        let item = l.addItem(new SP.ListItemCreationInformation());
                        Object.keys(r).forEach(key => {
                            item.set_item(key, r[key]);
                        });
                        item.update();
                        params.ClientContext.load(item);
                    });
                }
            });
            params.ClientContext.executeQueryAsync(resolve, resolve);
        });
    }
}

interface IFunctionParams {
    ClientContext: SP.ClientContext;
    ListInstances: Array<SP.List>;
    Objects: Array<IListInstance>;
}
