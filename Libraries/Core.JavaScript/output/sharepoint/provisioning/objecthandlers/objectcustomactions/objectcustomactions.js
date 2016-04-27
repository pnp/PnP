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
        define(["require", "exports", "../ObjectHandlerBase/ObjectHandlerBase"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\schema\icustomaction.d.ts" />
    // import { Promise } from "es6-promise";
    var ObjectHandlerBase_1 = require("../ObjectHandlerBase/ObjectHandlerBase");
    var ObjectCustomActions = (function (_super) {
        __extends(ObjectCustomActions, _super);
        function ObjectCustomActions() {
            _super.call(this, "CustomActions");
        }
        ObjectCustomActions.prototype.ProvisionObjects = function (customactions) {
            var _this = this;
            _super.prototype.scope_started.call(this);
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var userCustomActions = clientContext.get_web().get_userCustomActions();
                clientContext.load(userCustomActions);
                clientContext.executeQueryAsync(function () {
                    customactions.forEach(function (obj) {
                        var objExists = jQuery.grep(userCustomActions.get_data(), function (userCustomAction) {
                            return userCustomAction.get_title() === obj.Title;
                        }).length > 0;
                        if (!objExists) {
                            var objCreationInformation = userCustomActions.add();
                            if (obj.Description) {
                                objCreationInformation.set_description(obj.Description);
                            }
                            if (obj.CommandUIExtension) {
                                objCreationInformation.set_commandUIExtension(obj.CommandUIExtension);
                            }
                            if (obj.Group) {
                                objCreationInformation.set_group(obj.Group);
                            }
                            if (obj.Title) {
                                objCreationInformation.set_title(obj.Title);
                            }
                            if (obj.Url) {
                                objCreationInformation.set_url(obj.Url);
                            }
                            if (obj.ScriptBlock) {
                                objCreationInformation.set_scriptBlock(obj.ScriptBlock);
                            }
                            if (obj.ScriptSrc) {
                                objCreationInformation.set_scriptSrc(obj.ScriptSrc);
                            }
                            if (obj.Location) {
                                objCreationInformation.set_location(obj.Location);
                            }
                            if (obj.ImageUrl) {
                                objCreationInformation.set_imageUrl(obj.ImageUrl);
                            }
                            if (obj.Name) {
                                objCreationInformation.set_name(obj.Name);
                            }
                            if (obj.RegistrationId) {
                                objCreationInformation.set_registrationId(obj.RegistrationId);
                            }
                            if (obj.RegistrationType) {
                                objCreationInformation.set_registrationType(obj.RegistrationType);
                            }
                            if (obj.Rights) {
                                objCreationInformation.set_rights(obj.Rights);
                            }
                            if (obj.Sequence) {
                                objCreationInformation.set_sequence(obj.Sequence);
                            }
                            objCreationInformation.update();
                        }
                    });
                    clientContext.executeQueryAsync(function () {
                        _super.prototype.scope_ended.call(_this);
                        resolve();
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
        return ObjectCustomActions;
    }(ObjectHandlerBase_1.ObjectHandlerBase));
    exports.ObjectCustomActions = ObjectCustomActions;
});
