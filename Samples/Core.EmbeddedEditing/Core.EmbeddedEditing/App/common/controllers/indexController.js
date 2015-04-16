(function ($, RSVP, SP2013, EE, window, undefined) {
    "use strict";

    EE.Controllers = EE.Controllers || {};

    EE.Controllers.Index = function ($scope) {

        function applyScopeProperties($scope, map) {
            return function (o) {
                $scope.$apply(function () {
                    Object.keys(map).forEach(function (key) {
                        if (map[key] === null) {
                            $scope[key] = o;
                        }
                        else {
                            $scope[key] = map[key];
                        }
                    });
                });

                return o;
            };
        }

        function loadHostWebList(getListFunc) {
            return function (appPartProperties) {
                var hostWebListRelativeUri = "/web/lists('" + appPartProperties.listGuid.value + "')/items?$top=" + appPartProperties.rowLimit.value;
                // TODO: Remove dependency
                return RSVP.Promise.cast(getListFunc(hostWebListRelativeUri));
            };
        }

        // TODO: Refactor dependencies
        function setSelectedListIfGuidMatchesAvailableList(guid) {
            return function (hostLists) {

                var guids = hostLists.map(function (list) { return list.id; })
                    , guidMatchIndex = guids.indexOf(guid)
                ;

                if (guidMatchIndex >= 0) {
                    $scope.$apply(function () {
                        $scope.editableAppPartProperties.selectedList = hostLists[guidMatchIndex];
                    });
                }

                return hostLists;
            };
        }

        function initiateEditControls(getHostWebResourceFunc, appPartProperties) {
            $scope.$apply(function () {
                $scope.editableAppPartProperties.title = appPartProperties.title;
                $scope.editableAppPartProperties.rowLimit = appPartProperties.rowLimit;
                $scope.editableAppPartProperties.listGuid = appPartProperties.listGuid;
                $scope.editableAppPartProperties.selectedList = null;
            });

            var hostWebListsRelativeUri = "/web/lists?$select=Id,Title,BaseTemplate&$filter=((BaseTemplate%20eq%20100)%20and%20(Hidden%20eq%20false))"
                , listPromise
            ;

            listPromise = RSVP.Promise.cast(getHostWebResourceFunc(hostWebListsRelativeUri))
                .then(EE.Promise.logResponse("Host Lists: "))
                .then(EE.Promise.map(EE.Utilities.parseList))
                .then(EE.Promise.logResponse("Parsed Host Lists: "))
                .then(applyScopeProperties($scope, { hostLists: null }))
                .then(setSelectedListIfGuidMatchesAvailableList(appPartProperties.listGuid.value))
            ;

        }

        function submitEditForm() {

            function convertToAjaxReadyObject(appPartId, metadataType) {
                return function (o) {
                    return {
                        id: o.value.id
                        , etag: o.value.etag
                        , data: {
                            '__metadata': { 'type': metadataType },
                            AppPartId: appPartId,
                            Title: "title", // only here because it's required
                            Key: o.key,
                            String: o.value.value.toString()
                        }
                    };
                };
            }

            var appWebUri = $.deparam.querystring(true).SPAppWebUrl
                // TODO: Refactor to re-use from controller scoped variables
                , configurationListTitle = 'AppPartConfiguration'
                , appPartId = $scope.appPartId
                , appPartData = {
                    title: $scope.editableAppPartProperties.title
                    , rowLimit: $scope.editableAppPartProperties.rowLimit
                    , listGuid: $scope.editableAppPartProperties.listGuid
                }
                // TODO: refactor this to grab metadata from a real list item instead of faking it, should not be a need to reconstruct this
                , metadataType = 'SP.Data.AppPartConfigurationListItem'
                , dataList
                , listEndpoint = appWebUri + "/_api/web/lists/getbytitle('" + configurationListTitle + "')/items"
                , contextInfoEndpoint = appWebUri + '/_api/contextinfo'
                , requestDigestPromise
                , mainPromise
            ;

            // Overwrite listGuid.value in case the user has changed the dropdown menu
            appPartData.listGuid.value = $scope.editableAppPartProperties.selectedList.id;

            dataList = EE.Utilities.convertDictionaryToListOfKeyValuePairs(appPartData);
            dataList = dataList.map(convertToAjaxReadyObject(appPartId, metadataType));

            // SEE: http://www.wictorwilen.se/sharepoint-2013-how-to-refresh-the-request-digest-value-in-javascript
            requestDigestPromise = RSVP.Promise.cast($.ajax({
                url: contextInfoEndpoint,
                method: "POST",
                headers: { "Accept": "application/json; odata=verbose" }
            })).then(function (odata) {
                return odata.d.GetContextWebInformation.FormDigestValue;
            });

            function generateAjaxOptions(uri, dataList) {
                return function (requestDigest) {
                    var ajaxOptions = dataList.map((function (listItemsEndpoint, requestDigest) {
                        return function (ajaxReadyObject) {
                            var ajaxOptions
                                , listItemEndpoint
                            ;

                            // If the ajaxReadyObject has an existing list id, then generate an options hash
                            // that will UPDATE the list item
                            // Otherwise, generate a CREATE options hash

                            // Reference for ETags:
                            // http://msdn.microsoft.com/en-us/library/office/dn292558.aspx
                            if ((typeof ajaxReadyObject.id === "number") && (ajaxReadyObject.id > 0)) {
                                listItemEndpoint = listItemsEndpoint + "(" + ajaxReadyObject.id + ")";
                                ajaxOptions = {
                                    type: "POST"
                                    , url: listItemEndpoint
                                    , data: JSON.stringify(ajaxReadyObject.data)
                                    , headers: {
                                        "X-Http-Method": "PATCH",
                                        "X-RequestDigest": requestDigest,
                                        "IF-MATCH": ajaxReadyObject.etag,
                                        "accept": "application/json;odata=verbose",
                                        "content-type": "application/json;odata=verbose"
                                    }
                                };
                            }
                            else {
                                ajaxOptions = {
                                    type: "POST"
                                    , url: listItemsEndpoint
                                    , data: JSON.stringify(ajaxReadyObject.data)
                                    , headers: {
                                        "X-RequestDigest": requestDigest,
                                        "accept": "application/json;odata=verbose",
                                        "content-type": "application/json;odata=verbose"
                                    }
                                };
                            }

                            return ajaxOptions;
                        };
                    })(uri, requestDigest));

                    return ajaxOptions;
                };
            }

            mainPromise = requestDigestPromise
                .then(EE.Promise.logResponse("Reuest Digest Value: "))
                .then(generateAjaxOptions(listEndpoint, dataList))
                .then(EE.Promise.logResponse("Ajax Options: "))
                .then(function (ajaxOptions) {
                    var promises = ajaxOptions.map(function (options) {
                        return RSVP.Promise.cast($.ajax(options));
                    });

                    return promises;
                })
                .then(function (promises) {
                    return RSVP.Promise.all(promises);
                })
                .then(EE.Promise.logResponse("Final Save Response: "))
            ;
        }

        $scope.isLoadingList = false;
        $scope.isEditModeEnabled = false;
        $scope.areAppPartPropertiesCalculated = false;
        $scope.errors = [];
        $scope.items = [];
        $scope.isLoadingHostList = false;
        $scope.hostListItems = [];
        $scope.hostLists = [];
        $scope.querystringProperties = [];
        $scope.appPartId = null;
        $scope.appPartDefaultProperties = {};
        $scope.appPartConfigProperties = {};
        $scope.appPartProperties = {};
        $scope.editableAppPartProperties = {};
        $scope.submitEditForm = submitEditForm;

        function init() {
            var querystringProperties = $.deparam.querystring(true)
                , querystringPropertiesList = EE.Utilities.convertDictionaryToListOfKeyValuePairs(querystringProperties)
                , configurationListTitle = 'AppPartConfiguration'
                , configurationListAppPartIdColumnName = 'AppPartId'
                , isEditModeEnabled = (querystringProperties.editMode === 1)
                , listItemsEndpoint = querystringProperties.SPAppWebUrl + "/_api/web/lists/getbytitle('" + configurationListTitle + "')/items?$filter=" + configurationListAppPartIdColumnName + "%20eq%20%27" + querystringProperties.wpId + "%27"
                , listResponse
                , appPartIframeWidth = '100%'
                , appPartIframeHeight = 800
                , appPartDefaultProperties = {
                    listGuid: {
                        id: null
                        , value: null
                    }
                    , rowLimit: {
                        id: null
                        , value: 10
                    }
                    , title: {
                        id: null
                        , value: 'Default App Part Title'
                    }
                }
                // TODO: Refactor, redundant properties naming is confusing, maybe add shortcut inside parse function, if null just return value.
                , scopeApplyMappings = {
                    rawListItems: {
                        items: null,
                        isLoadingList: false
                    }
                    , appPartConfigProperties: {
                        appPartConfigProperties: null
                    }
                    , appPartProperties: {
                        appPartProperties: null,
                        areAppPartPropertiesCalculated: true
                    }
                    , hostListItems: {
                        hostListItems: null
                    }
                    , hostLists: {
                        hostLists: null
                    }
                }
                , keyValidatorMapping = {
                    listGuid: function (x) {
                        if (typeof x !== "string") {
                            return "The type of listGuid must be a string. You passed: " + x + " You must edit the web part and select a list from the host web.";
                        }

                        if (x.length === 0) {
                            return "The listGuid is an empty string. You must edit the web part and select a list from the host web.";
                        }

                        return true;
                    }
                }
                // TODO: Possibly refactor to abstract RSVP.Promise.cast within
                , loadHostWebListFunc = SP2013.Ajax.getSpRequestExecutorPromise.bind(null, querystringProperties.SPAppWebUrl, querystringProperties.SPHostUrl)
            ;

            // Update iframe width to be 100%
            if (querystringProperties.SenderId) {
                SP2013.UX.updateIframeSize(querystringProperties.SPHostUrl, querystringProperties.SenderId, appPartIframeWidth, appPartIframeHeight);
            }

            // Assign static properties
            $scope.querystringProperties = querystringPropertiesList;
            $scope.isEditModeEnabled = isEditModeEnabled;
            $scope.appPartId = querystringProperties.wpId;
            $scope.listItemsEndpoint = listItemsEndpoint;
            $scope.appPartDefaultProperties = appPartDefaultProperties;

            $scope.isLoadingList = false;
            listResponse = RSVP.Promise.cast(EE.Utilities.getAppWebResource(listItemsEndpoint))
                //TODO: .catch(handleBadListRequest)
                .then(EE.Promise.logResponse("Raw Items: "))
                .then(EE.Promise.map(EE.Utilities.parseListItem))
                .then(applyScopeProperties($scope, scopeApplyMappings.rawListItems)) // only need to add this to scope for demo purposes
                .then(EE.Promise.logResponse("Parsed Items: "))
                .then(EE.Utilities.getRelaventPropertiesFromListItems(Object.keys(appPartDefaultProperties)))
                .then(EE.Promise.logResponse("Config Properties: "))
                .then(EE.Utilities.coercePropertyValues(appPartDefaultProperties))
                .then(applyScopeProperties($scope, scopeApplyMappings.appPartConfigProperties))
                .then(EE.Promise.logResponse("Coerced Config Properties: "))
                //.then(getAppPartPropertiesFromDefaultsAndConfig.bind(null, appPartDefaultProperties))
                .then($.extend.bind(null, true, {}, appPartDefaultProperties))
                .then(EE.Promise.logResponse("App Part Properties: "))
                .then(applyScopeProperties($scope, scopeApplyMappings.appPartProperties))
                // TODO: Look for alternative to forking the promise chain here.
                .then(function (appPartProperties) {
                    isEditModeEnabled && initiateEditControls(loadHostWebListFunc, appPartProperties);
                    return appPartProperties;
                })
                .then(EE.Validate.propertiesByMap(keyValidatorMapping))
                .catch(EE.Validate.handleValidationErrors(applyScopeProperties($scope, { errors: null })))
                .then(loadHostWebList(loadHostWebListFunc))
                .then(EE.Promise.logResponse("Host Web List Raw Items: "))
                .then(EE.Promise.map(EE.Utilities.parseHostListItem))
                .then(applyScopeProperties($scope, scopeApplyMappings.hostListItems))

            ;
        }

        init();

    };

})(jQuery, RSVP, SP2013, EE, this);