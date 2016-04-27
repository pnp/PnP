var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../../../Util", "../ObjectHandlerBase/ObjectHandlerBase"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\schema\ifile.d.ts" />
    // import { Promise } from "es6-promise";
    var Util_1 = require("../../../Util");
    var ObjectHandlerBase_1 = require("../ObjectHandlerBase/ObjectHandlerBase");
    var ObjectFiles = (function (_super) {
        __extends(ObjectFiles, _super);
        function ObjectFiles() {
            _super.call(this, "Files");
        }
        ObjectFiles.prototype.ProvisionObjects = function (objects) {
            var _this = this;
            _super.prototype.scope_started.call(this);
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var web = clientContext.get_web();
                var fileInfos = [];
                var promises = [];
                objects.forEach(function (obj, index) {
                    var filename = _this.GetFilenameFromFilePath(obj.Dest);
                    var webServerRelativeUrl = _spPageContextInfo.webServerRelativeUrl;
                    var folder = web.getFolderByServerRelativeUrl(webServerRelativeUrl + "/" + _this.GetFolderFromFilePath(obj.Dest));
                    promises.push(jQuery.get(Util_1.replaceUrlTokens(obj.Src), function (fileContents) {
                        var f = {};
                        jQuery.extend(f, obj, { "Filename": filename, "Folder": folder, "Contents": fileContents });
                        fileInfos.push(f);
                    }));
                });
                jQuery.when.apply(jQuery, promises).done(function () {
                    fileInfos.forEach(function (f, index) {
                        if (f.Filename.indexOf("Form.aspx") !== -1) {
                            return;
                        }
                        var objCreationInformation = new SP.FileCreationInformation();
                        objCreationInformation.set_overwrite(f.Overwrite !== undefined ? f.Overwrite : false);
                        objCreationInformation.set_url(f.Filename);
                        objCreationInformation.set_content(new SP.Base64EncodedByteArray());
                        for (var i = 0; i < f.Contents.length; i++) {
                            objCreationInformation.get_content().append(f.Contents.charCodeAt(i));
                        }
                        clientContext.load(f.Folder.get_files().add(objCreationInformation));
                    });
                    clientContext.executeQueryAsync(function () {
                        promises = [];
                        objects.forEach(function (obj) {
                            if (obj.Properties && Object.keys(obj.Properties).length > 0) {
                                promises.push(_this.ApplyFileProperties(obj.Dest, obj.Properties));
                            }
                            if (obj.WebParts && obj.WebParts.length > 0) {
                                promises.push(_this.AddWebPartsToWebPartPage(obj.Dest, obj.Src, obj.WebParts, obj.RemoveExistingWebParts));
                            }
                        });
                        Promise.all(promises).then(function () {
                            _this.ModifyHiddenViews(objects).then(function () {
                                _super.prototype.scope_ended.call(_this);
                                resolve();
                            }, function () {
                                _super.prototype.scope_ended.call(_this);
                                resolve();
                            });
                        });
                    }, function () {
                        _super.prototype.scope_ended.call(_this);
                        resolve();
                    });
                });
            });
        };
        ObjectFiles.prototype.RemoveWebPartsFromFileIfSpecified = function (clientContext, limitedWebPartManager, shouldRemoveExisting) {
            return new Promise(function (resolve, reject) {
                if (!shouldRemoveExisting) {
                    resolve();
                }
                var existingWebParts = limitedWebPartManager.get_webParts();
                clientContext.load(existingWebParts);
                clientContext.executeQueryAsync(function () {
                    existingWebParts.get_data().forEach(function (wp) {
                        wp.deleteWebPart();
                    });
                    clientContext.load(existingWebParts);
                    clientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
            });
        };
        ObjectFiles.prototype.GetWebPartXml = function (webParts) {
            return new Promise(function (resolve, reject) {
                var promises = [];
                webParts.forEach(function (wp, index) {
                    if (wp.Contents.FileUrl) {
                        promises.push((function () {
                            return new Promise(function (res, rej) {
                                var fileUrl = Util_1.replaceUrlTokens(wp.Contents.FileUrl);
                                jQuery.get(fileUrl, function (xml) {
                                    webParts[index].Contents.Xml = xml;
                                    res();
                                }).fail(rej);
                            });
                        })());
                    }
                });
                Promise.all(promises).then(function () {
                    resolve(webParts);
                });
            });
        };
        ObjectFiles.prototype.AddWebPartsToWebPartPage = function (dest, src, webParts, shouldRemoveExisting) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var web = clientContext.get_web();
                var fileServerRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/" + dest;
                var file = web.getFileByServerRelativeUrl(fileServerRelativeUrl);
                clientContext.load(file);
                clientContext.executeQueryAsync(function () {
                    var limitedWebPartManager = file.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);
                    _this.RemoveWebPartsFromFileIfSpecified(clientContext, limitedWebPartManager, shouldRemoveExisting).then(function () {
                        _this.GetWebPartXml(webParts).then(function (webPartsWithXml) {
                            webPartsWithXml.forEach(function (wp) {
                                if (!wp.Contents.Xml) {
                                    return;
                                }
                                var oWebPartDefinition = limitedWebPartManager.importWebPart(Util_1.replaceUrlTokens(wp.Contents.Xml));
                                var oWebPart = oWebPartDefinition.get_webPart();
                                limitedWebPartManager.addWebPart(oWebPart, wp.Zone, wp.Order);
                            });
                            clientContext.executeQueryAsync(resolve, resolve);
                        });
                    });
                }, resolve);
            });
        };
        ObjectFiles.prototype.ApplyFileProperties = function (dest, fileProperties) {
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var web = clientContext.get_web();
                var fileServerRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/" + dest;
                var file = web.getFileByServerRelativeUrl(fileServerRelativeUrl);
                var listItemAllFields = file.get_listItemAllFields();
                Object.keys(fileProperties).forEach(function (key) {
                    listItemAllFields.set_item(key, fileProperties[key]);
                });
                listItemAllFields.update();
                clientContext.executeQueryAsync(resolve, resolve);
            });
        };
        ObjectFiles.prototype.GetViewFromCollectionByUrl = function (viewCollection, url) {
            var serverRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/" + url;
            var viewCollectionEnumerator = viewCollection.getEnumerator();
            while (viewCollectionEnumerator.moveNext()) {
                var view = viewCollectionEnumerator.get_current();
                if (view.get_serverRelativeUrl().toString().toLowerCase() === serverRelativeUrl.toLowerCase()) {
                    return view;
                }
            }
            return null;
        };
        ObjectFiles.prototype.ModifyHiddenViews = function (objects) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var web = clientContext.get_web();
                var mapping = {};
                var lists = [];
                var listViewCollections = [];
                objects.forEach(function (obj) {
                    if (!obj.Views) {
                        return;
                    }
                    obj.Views.forEach(function (v) {
                        mapping[v.List] = mapping[v.List] || [];
                        mapping[v.List].push(jQuery.extend(v, { "Url": obj.Dest }));
                    });
                });
                Object.keys(mapping).forEach(function (l, index) {
                    lists.push(web.get_lists().getByTitle(l));
                    listViewCollections.push(web.get_lists().getByTitle(l).get_views());
                    clientContext.load(lists[index]);
                    clientContext.load(listViewCollections[index]);
                });
                clientContext.executeQueryAsync(function () {
                    Object.keys(mapping).forEach(function (l, index) {
                        var views = mapping[l];
                        var list = lists[index];
                        var viewCollection = listViewCollections[index];
                        views.forEach(function (v) {
                            var view = _this.GetViewFromCollectionByUrl(viewCollection, v.Url);
                            if (view == null) {
                                return;
                            }
                            if (v.Paged) {
                                view.set_paged(v.Paged);
                            }
                            if (v.Query) {
                                view.set_viewQuery(v.Query);
                            }
                            if (v.RowLimit) {
                                view.set_rowLimit(v.RowLimit);
                            }
                            if (v.ViewFields && v.ViewFields.length > 0) {
                                var columns_1 = view.get_viewFields();
                                columns_1.removeAll();
                                v.ViewFields.forEach(function (vf) {
                                    columns_1.add(vf);
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
        };
        ObjectFiles.prototype.GetFolderFromFilePath = function (filePath) {
            var split = filePath.split("/");
            return split.splice(0, split.length - 1).join("/");
        };
        ObjectFiles.prototype.GetFilenameFromFilePath = function (filePath) {
            var split = filePath.split("/");
            return split[split.length - 1];
        };
        return ObjectFiles;
    }(ObjectHandlerBase_1.ObjectHandlerBase));
    exports.ObjectFiles = ObjectFiles;
    ;
});
