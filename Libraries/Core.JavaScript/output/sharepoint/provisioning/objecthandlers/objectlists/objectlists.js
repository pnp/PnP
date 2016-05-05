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
        define(["require", "exports", "../../Sequencer/Sequencer", "../ObjectHandlerBase/ObjectHandlerBase"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\schema\ilistinstance.d.ts" />
    // import { Promise } from "es6-promise";
    var Sequencer_1 = require("../../Sequencer/Sequencer");
    var ObjectHandlerBase_1 = require("../ObjectHandlerBase/ObjectHandlerBase");
    var ObjectLists = (function (_super) {
        __extends(ObjectLists, _super);
        function ObjectLists() {
            _super.call(this, "Lists");
        }
        ObjectLists.prototype.ProvisionObjects = function (objects) {
            var _this = this;
            _super.prototype.scope_started.call(this);
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var lists = clientContext.get_web().get_lists();
                var listInstances = [];
                clientContext.load(lists);
                clientContext.executeQueryAsync(function () {
                    objects.forEach(function (obj, index) {
                        var existingObj = jQuery.grep(lists.get_data(), function (list) {
                            return list.get_title() === obj.Title;
                        })[0];
                        if (existingObj) {
                            if (obj.Description) {
                                existingObj.set_description(obj.Description);
                            }
                            if (obj.EnableVersioning !== undefined) {
                                existingObj.set_enableVersioning(obj.EnableVersioning);
                            }
                            if (obj.EnableMinorVersions !== undefined) {
                                existingObj.set_enableMinorVersions(obj.EnableMinorVersions);
                            }
                            if (obj.EnableModeration !== undefined) {
                                existingObj.set_enableModeration(obj.EnableModeration);
                            }
                            if (obj.EnableFolderCreation !== undefined) {
                                existingObj.set_enableFolderCreation(obj.EnableFolderCreation);
                            }
                            if (obj.EnableAttachments !== undefined) {
                                existingObj.set_enableAttachments(obj.EnableAttachments);
                            }
                            if (obj.NoCrawl !== undefined) {
                                existingObj.set_noCrawl(obj.NoCrawl);
                            }
                            if (obj.DefaultDisplayFormUrl) {
                                existingObj.set_defaultDisplayFormUrl(obj.DefaultDisplayFormUrl);
                            }
                            if (obj.DefaultEditFormUrl) {
                                existingObj.set_defaultEditFormUrl(obj.DefaultEditFormUrl);
                            }
                            if (obj.DefaultNewFormUrl) {
                                existingObj.set_defaultNewFormUrl(obj.DefaultNewFormUrl);
                            }
                            if (obj.DraftVersionVisibility) {
                                existingObj.set_draftVersionVisibility(SP.DraftVisibilityType[obj.DraftVersionVisibility]);
                            }
                            if (obj.ImageUrl) {
                                existingObj.set_imageUrl(obj.ImageUrl);
                            }
                            if (obj.Hidden !== undefined) {
                                existingObj.set_hidden(obj.Hidden);
                            }
                            if (obj.ForceCheckout !== undefined) {
                                existingObj.set_forceCheckout(obj.ForceCheckout);
                            }
                            existingObj.update();
                            listInstances.push(existingObj);
                            clientContext.load(listInstances[index]);
                        }
                        else {
                            var objCreationInformation = new SP.ListCreationInformation();
                            if (obj.Description) {
                                objCreationInformation.set_description(obj.Description);
                            }
                            if (obj.OnQuickLaunch !== undefined) {
                                var value = obj.OnQuickLaunch ? SP.QuickLaunchOptions.on : SP.QuickLaunchOptions.off;
                                objCreationInformation.set_quickLaunchOption(value);
                            }
                            if (obj.TemplateType) {
                                objCreationInformation.set_templateType(obj.TemplateType);
                            }
                            if (obj.Title) {
                                objCreationInformation.set_title(obj.Title);
                            }
                            if (obj.Url) {
                                objCreationInformation.set_url(obj.Url);
                            }
                            var createdList = lists.add(objCreationInformation);
                            if (obj.EnableVersioning !== undefined) {
                                createdList.set_enableVersioning(obj.EnableVersioning);
                            }
                            if (obj.EnableMinorVersions !== undefined) {
                                createdList.set_enableMinorVersions(obj.EnableMinorVersions);
                            }
                            if (obj.EnableModeration !== undefined) {
                                createdList.set_enableModeration(obj.EnableModeration);
                            }
                            if (obj.EnableFolderCreation !== undefined) {
                                createdList.set_enableFolderCreation(obj.EnableFolderCreation);
                            }
                            if (obj.EnableAttachments !== undefined) {
                                createdList.set_enableAttachments(obj.EnableAttachments);
                            }
                            if (obj.NoCrawl !== undefined) {
                                createdList.set_noCrawl(obj.NoCrawl);
                            }
                            if (obj.DefaultDisplayFormUrl) {
                                createdList.set_defaultDisplayFormUrl(obj.DefaultDisplayFormUrl);
                            }
                            if (obj.DefaultEditFormUrl) {
                                createdList.set_defaultEditFormUrl(obj.DefaultEditFormUrl);
                            }
                            if (obj.DefaultNewFormUrl) {
                                createdList.set_defaultNewFormUrl(obj.DefaultNewFormUrl);
                            }
                            if (obj.DraftVersionVisibility) {
                                var value = SP.DraftVisibilityType[obj.DraftVersionVisibility.toLocaleLowerCase()];
                                createdList.set_draftVersionVisibility(value);
                            }
                            if (obj.ImageUrl) {
                                createdList.set_imageUrl(obj.ImageUrl);
                            }
                            if (obj.Hidden !== undefined) {
                                createdList.set_hidden(obj.Hidden);
                            }
                            if (obj.ForceCheckout !== undefined) {
                                createdList.set_forceCheckout(obj.ForceCheckout);
                            }
                            listInstances.push(createdList);
                            clientContext.load(listInstances[index]);
                        }
                    });
                    clientContext.executeQueryAsync(function () {
                        var sequencer = new Sequencer_1.Sequencer([
                            _this.ApplyContentTypeBindings,
                            _this.ApplyListInstanceFieldRefs,
                            _this.ApplyFields,
                            _this.ApplyLookupFields,
                            _this.ApplyListSecurity,
                            _this.CreateViews,
                            _this.InsertDataRows,
                            _this.CreateFolders,
                        ], { ClientContext: clientContext, ListInstances: listInstances, Objects: objects }, _this);
                        sequencer.execute().then(function () {
                            _super.prototype.scope_ended.call(_this);
                            resolve();
                        });
                    }, function () {
                        _super.prototype.scope_ended.call(_this);
                        resolve();
                    });
                }, function () {
                    _super.prototype.scope_ended.call(_this);
                    resolve();
                });
            });
        };
        ObjectLists.prototype.EnsureLocationBasedMetadataDefaultsReceiver = function (clientContext, list) {
            var eventReceivers = list.get_eventReceivers();
            var eventRecCreationInfo = new SP.EventReceiverDefinitionCreationInformation();
            eventRecCreationInfo.set_receiverName("LocationBasedMetadataDefaultsReceiver ItemAdded");
            eventRecCreationInfo.set_synchronization(1);
            eventRecCreationInfo.set_sequenceNumber(1000);
            eventRecCreationInfo.set_receiverAssembly("Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, " +
                "PublicKeyToken=71e9bce111e9429c");
            eventRecCreationInfo.set_receiverClass("Microsoft.Office.DocumentManagement.LocationBasedMetadataDefaultsReceiver");
            eventRecCreationInfo.set_eventType(SP.EventReceiverType.itemAdded);
            eventReceivers.add(eventRecCreationInfo);
            list.update();
        };
        ObjectLists.prototype.CreateFolders = function (params) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                params.ListInstances.forEach(function (l, index) {
                    var obj = params.Objects[index];
                    if (!obj.Folders) {
                        return;
                    }
                    var folderServerRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/" + obj.Url;
                    var rootFolder = l.get_rootFolder();
                    var metadataDefaults = "<MetadataDefaults>";
                    var setMetadataDefaults = false;
                    obj.Folders.forEach(function (f) {
                        var folderUrl = folderServerRelativeUrl + "/" + f.Name;
                        rootFolder.get_folders().add(folderUrl);
                        if (f.DefaultValues) {
                            var keys = Object.keys(f.DefaultValues).length;
                            if (keys > 0) {
                                metadataDefaults += "<a href='" + folderUrl + "'>";
                                Object.keys(f.DefaultValues).forEach(function (key) {
                                    metadataDefaults += "<DefaultValue FieldName=\"" + key + "\">" + f.DefaultValues[key] + "</DefaultValue>";
                                });
                                metadataDefaults += "</a>";
                            }
                            setMetadataDefaults = true;
                        }
                    });
                    metadataDefaults += "</MetadataDefaults>";
                    if (setMetadataDefaults) {
                        var metadataDefaultsFileCreateInfo = new SP.FileCreationInformation();
                        metadataDefaultsFileCreateInfo.set_url(folderServerRelativeUrl + "/Forms/client_LocationBasedDefaults.html");
                        metadataDefaultsFileCreateInfo.set_content(new SP.Base64EncodedByteArray());
                        metadataDefaultsFileCreateInfo.set_overwrite(true);
                        for (var i = 0; i < metadataDefaults.length; i++) {
                            metadataDefaultsFileCreateInfo.get_content().append(metadataDefaults.charCodeAt(i));
                        }
                        rootFolder.get_files().add(metadataDefaultsFileCreateInfo);
                        _this.EnsureLocationBasedMetadataDefaultsReceiver(params.ClientContext, l);
                    }
                });
                params.ClientContext.executeQueryAsync(resolve, resolve);
            });
        };
        ObjectLists.prototype.ApplyContentTypeBindings = function (params) {
            return new Promise(function (resolve, reject) {
                var webCts = params.ClientContext.get_site().get_rootWeb().get_contentTypes();
                var listCts = [];
                params.ListInstances.forEach(function (l, index) {
                    listCts.push(l.get_contentTypes());
                    params.ClientContext.load(listCts[index], "Include(Name,Id)");
                    if (params.Objects[index].ContentTypeBindings) {
                        l.set_contentTypesEnabled(true);
                        l.update();
                    }
                });
                params.ClientContext.load(webCts);
                params.ClientContext.executeQueryAsync(function () {
                    params.ListInstances.forEach(function (list, index) {
                        var obj = params.Objects[index];
                        if (!obj.ContentTypeBindings) {
                            return;
                        }
                        var listContentTypes = listCts[index];
                        var existingContentTypes = new Array();
                        if (obj.RemoveExistingContentTypes && obj.ContentTypeBindings.length > 0) {
                            listContentTypes.get_data().forEach(function (ct) {
                                existingContentTypes.push(ct);
                            });
                        }
                        obj.ContentTypeBindings.forEach(function (ctb) {
                            listContentTypes.addExistingContentType(webCts.getById(ctb.ContentTypeId));
                        });
                        if (obj.RemoveExistingContentTypes && obj.ContentTypeBindings.length > 0) {
                            for (var j = 0; j < existingContentTypes.length; j++) {
                                var ect = existingContentTypes[j];
                                ect.deleteObject();
                            }
                        }
                        list.update();
                    });
                    params.ClientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
            });
        };
        ObjectLists.prototype.ApplyListInstanceFieldRefs = function (params) {
            return new Promise(function (resolve, reject) {
                var siteFields = params.ClientContext.get_site().get_rootWeb().get_fields();
                params.ListInstances.forEach(function (l, index) {
                    var obj = params.Objects[index];
                    if (obj.FieldRefs) {
                        obj.FieldRefs.forEach(function (fr) {
                            var field = siteFields.getByInternalNameOrTitle(fr.Name);
                            l.get_fields().add(field);
                        });
                        l.update();
                    }
                });
                params.ClientContext.executeQueryAsync(resolve, resolve);
            });
        };
        ObjectLists.prototype.ApplyFields = function (params) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                params.ListInstances.forEach(function (l, index) {
                    var obj = params.Objects[index];
                    if (obj.Fields) {
                        obj.Fields.forEach(function (f) {
                            var fieldXml = _this.GetFieldXml(f, params.ListInstances, l);
                            var fieldType = _this.GetFieldXmlAttr(fieldXml, "Type");
                            if (fieldType !== "Lookup" && fieldType !== "LookupMulti") {
                                l.get_fields().addFieldAsXml(fieldXml, true, SP.AddFieldOptions.addToAllContentTypes);
                            }
                        });
                        l.update();
                    }
                });
                params.ClientContext.executeQueryAsync(resolve, resolve);
            });
        };
        ObjectLists.prototype.ApplyLookupFields = function (params) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                params.ListInstances.forEach(function (l, index) {
                    var obj = params.Objects[index];
                    if (obj.Fields) {
                        obj.Fields.forEach(function (f) {
                            var fieldXml = _this.GetFieldXml(f, params.ListInstances, l);
                            if (!fieldXml) {
                                return;
                            }
                            var fieldType = _this.GetFieldXmlAttr(fieldXml, "Type");
                            if (fieldType === "Lookup" || fieldType === "LookupMulti") {
                                l.get_fields().addFieldAsXml(fieldXml, true, SP.AddFieldOptions.addToAllContentTypes);
                            }
                        });
                        l.update();
                    }
                });
                params.ClientContext.executeQueryAsync(resolve, resolve);
            });
        };
        ObjectLists.prototype.GetFieldXmlAttr = function (fieldXml, attr) {
            return $(jQuery.parseXML(fieldXml)).find("Field").attr(attr);
        };
        ObjectLists.prototype.GetFieldXml = function (field, lists, list) {
            var fieldXml = "";
            if (!field.SchemaXml) {
                var properties_1 = [];
                Object.keys(field).forEach(function (prop) {
                    var value = field[prop];
                    if (prop === "List") {
                        var targetList = jQuery.grep(lists, function (v) {
                            return v.get_title() === value;
                        });
                        if (targetList.length > 0) {
                            value = "{" + targetList[0].get_id().toString() + "}";
                        }
                        else {
                            return null;
                        }
                        properties_1.push(prop + "=\"" + value + "\"");
                    }
                });
                fieldXml = "<Field " + properties_1.join(" ") + ">";
                if (field.Type === "Calculated") {
                    fieldXml += "<Formula>" + field.Formula + "</Formula>";
                }
                fieldXml += "</Field>";
            }
            return fieldXml;
        };
        ObjectLists.prototype.ApplyListSecurity = function (params) {
            return new Promise(function (resolve, reject) {
                params.ListInstances.forEach(function (l, index) {
                    var obj = params.Objects[index];
                    if (!obj.Security) {
                        return;
                    }
                    if (obj.Security.BreakRoleInheritance) {
                        l.breakRoleInheritance(obj.Security.CopyRoleAssignments, obj.Security.ClearSubscopes);
                        l.update();
                        params.ClientContext.load(l.get_roleAssignments());
                    }
                });
                var web = params.ClientContext.get_web();
                var allProperties = web.get_allProperties();
                var siteGroups = web.get_siteGroups();
                var roleDefinitions = web.get_roleDefinitions();
                params.ClientContext.load(allProperties);
                params.ClientContext.load(roleDefinitions);
                params.ClientContext.executeQueryAsync(function () {
                    params.ListInstances.forEach(function (l, index) {
                        var obj = params.Objects[index];
                        if (!obj.Security) {
                            return;
                        }
                        obj.Security.RoleAssignments.forEach(function (ra) {
                            var roleDef = null;
                            if (typeof ra.RoleDefinition === "number") {
                                roleDef = roleDefinitions.getById(ra.RoleDefinition);
                            }
                            else {
                                roleDef = roleDefinitions.getByName(ra.RoleDefinition);
                            }
                            var roleBindings = SP.RoleDefinitionBindingCollection.newObject(params.ClientContext);
                            roleBindings.add(roleDef);
                            var principal = null;
                            if (ra.Principal.match(/\{[A-Za-z]*\}+/g)) {
                                var token = ra.Principal.substring(1, ra.Principal.length - 1);
                                var groupId = allProperties.get_fieldValues()[("vti_" + token)];
                                principal = siteGroups.getById(groupId);
                            }
                            else {
                                principal = siteGroups.getByName(principal);
                            }
                            l.get_roleAssignments().add(principal, roleBindings);
                        });
                        l.update();
                    });
                    params.ClientContext.executeQueryAsync(resolve, resolve);
                }, resolve);
            });
        };
        ObjectLists.prototype.CreateViews = function (params) {
            return new Promise(function (resolve, reject) {
                var listViewCollections = [];
                params.ListInstances.forEach(function (l, index) {
                    listViewCollections.push(l.get_views());
                    params.ClientContext.load(listViewCollections[index]);
                });
                params.ClientContext.executeQueryAsync(function () {
                    params.ListInstances.forEach(function (l, index) {
                        var obj = params.Objects[index];
                        if (!obj.Views) {
                            return;
                        }
                        listViewCollections.push(l.get_views());
                        params.ClientContext.load(listViewCollections[index]);
                        obj.Views.forEach(function (v) {
                            var viewExists = jQuery.grep(listViewCollections[index].get_data(), function (ev) {
                                if (obj.RemoveExistingViews && obj.Views.length > 0) {
                                    ev.deleteObject();
                                    return false;
                                }
                                return ev.get_title() === v.Title;
                            }).length > 0;
                            if (viewExists) {
                                var view = listViewCollections[index].getByTitle(v.Title);
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
                                if (v.Scope) {
                                    view.set_scope(v.Scope);
                                }
                                view.update();
                            }
                            else {
                                var viewCreationInformation = new SP.ViewCreationInformation();
                                if (v.Title) {
                                    viewCreationInformation.set_title(v.Title);
                                }
                                if (v.PersonalView) {
                                    viewCreationInformation.set_personalView(v.PersonalView);
                                }
                                if (v.Paged) {
                                    viewCreationInformation.set_paged(v.Paged);
                                }
                                if (v.Query) {
                                    viewCreationInformation.set_query(v.Query);
                                }
                                if (v.RowLimit) {
                                    viewCreationInformation.set_rowLimit(v.RowLimit);
                                }
                                if (v.SetAsDefaultView) {
                                    viewCreationInformation.set_setAsDefaultView(v.SetAsDefaultView);
                                }
                                if (v.ViewFields) {
                                    viewCreationInformation.set_viewFields(v.ViewFields);
                                }
                                if (v.ViewTypeKind) {
                                    viewCreationInformation.set_viewTypeKind(SP.ViewType.html);
                                }
                                var view = l.get_views().add(viewCreationInformation);
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
        };
        ObjectLists.prototype.InsertDataRows = function (params) {
            return new Promise(function (resolve, reject) {
                params.ListInstances.forEach(function (l, index) {
                    var obj = params.Objects[index];
                    if (obj.DataRows) {
                        obj.DataRows.forEach(function (r) {
                            var item = l.addItem(new SP.ListItemCreationInformation());
                            Object.keys(r).forEach(function (key) {
                                item.set_item(key, r[key]);
                            });
                            item.update();
                            params.ClientContext.load(item);
                        });
                    }
                });
                params.ClientContext.executeQueryAsync(resolve, resolve);
            });
        };
        return ObjectLists;
    }(ObjectHandlerBase_1.ObjectHandlerBase));
    exports.ObjectLists = ObjectLists;
});
