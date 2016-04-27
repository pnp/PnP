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
    /// <reference path="..\schema\inavigation.d.ts" />
    // import { Promise } from "es6-promise";
    var Util_1 = require("../../../Util");
    var ObjectHandlerBase_1 = require("../ObjectHandlerBase/ObjectHandlerBase");
    var ObjectNavigation = (function (_super) {
        __extends(ObjectNavigation, _super);
        function ObjectNavigation() {
            _super.call(this, "Navigation");
        }
        ObjectNavigation.prototype.ProvisionObjects = function (object) {
            var _this = this;
            _super.prototype.scope_started.call(this);
            var clientContext = SP.ClientContext.get_current();
            var navigation = clientContext.get_web().get_navigation();
            return new Promise(function (resolve, reject) {
                _this.ConfigureQuickLaunch(object.QuickLaunch, clientContext, navigation).then(function () {
                    _super.prototype.scope_ended.call(_this);
                    resolve();
                }, function () {
                    _super.prototype.scope_ended.call(_this);
                    resolve();
                });
            });
        };
        ObjectNavigation.prototype.ConfigureQuickLaunch = function (nodes, clientContext, navigation) {
            return new Promise(function (resolve, reject) {
                if (nodes.length === 0) {
                    resolve();
                }
                else {
                    var quickLaunchNodeCollection_1 = navigation.get_quickLaunch();
                    clientContext.load(quickLaunchNodeCollection_1);
                    clientContext.executeQueryAsync(function () {
                        var temporaryQuickLaunch = [];
                        var index = quickLaunchNodeCollection_1.get_count() - 1;
                        while (index >= 0) {
                            var oldNode = quickLaunchNodeCollection_1.itemAt(index);
                            temporaryQuickLaunch.push(oldNode);
                            oldNode.deleteObject();
                            index--;
                        }
                        clientContext.executeQueryAsync(function () {
                            nodes.forEach(function (n) {
                                var existingNode = Util_1.getNodeFromCollectionByTitle(temporaryQuickLaunch, n.Title);
                                var newNode = new SP.NavigationNodeCreationInformation();
                                newNode.set_title(n.Title);
                                newNode.set_url(existingNode ? existingNode.get_url() : Util_1.replaceUrlTokens(n.Url));
                                newNode.set_asLastNode(true);
                                quickLaunchNodeCollection_1.add(newNode);
                            });
                            clientContext.executeQueryAsync(function () {
                                jQuery.ajax({
                                    "url": _spPageContextInfo.webAbsoluteUrl + "/_api/web/Navigation/QuickLaunch",
                                    "type": "get",
                                    "headers": {
                                        "accept": "application/json;odata=verbose",
                                    },
                                }).done(function (data) {
                                    data = data.d.results;
                                    data.forEach(function (d) {
                                        var node = navigation.getNodeById(d.Id);
                                        var childrenNodeCollection = node.get_children();
                                        var parentNode = jQuery.grep(nodes, function (value) { return value.Title === d.Title; })[0];
                                        if (parentNode && parentNode.Children) {
                                            parentNode.Children.forEach(function (n) {
                                                var existingNode = Util_1.getNodeFromCollectionByTitle(temporaryQuickLaunch, n.Title);
                                                var newNode = new SP.NavigationNodeCreationInformation();
                                                newNode.set_title(n.Title);
                                                newNode.set_url(existingNode ? existingNode.get_url() : Util_1.replaceUrlTokens(n.Url));
                                                newNode.set_asLastNode(true);
                                                childrenNodeCollection.add(newNode);
                                            });
                                        }
                                    });
                                    clientContext.executeQueryAsync(resolve, resolve);
                                });
                            }, resolve);
                        });
                    });
                }
            });
        };
        return ObjectNavigation;
    }(ObjectHandlerBase_1.ObjectHandlerBase));
    exports.ObjectNavigation = ObjectNavigation;
});
