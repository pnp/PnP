//Define global variables for JsHint, this will prevent '{a} not defined' warnings
/*global console, document, _spPageContextInfo, SP, arg*/
//Define option 'unused' as false for JsHint , this will prevent '{a} defined but never used' warnings
/* jshint unused:false */
var SharePointClient = SharePointClient || {};
(function () {
    "use strict";

    SharePointClient.AddNameSpace = function (namespace) {
        ///<summary>
        /// Define New Namespance or class into the root namespace.
        ///</summary>
        /// <param name="namespace" type="String">Name of the new namespace.</param>
        /// <returns type="Object">Parent namespace</returns>
        var nsparts = namespace.split("."), parent = SharePointClient, i = 0, partname;

        // we want to be able to include or exclude the root namespace so we strip
        // it if it's in the namespace
        if (nsparts[0] === "SharePointClient") {
            nsparts = nsparts.slice(1);
        }

        // loop through the parts and create a nested namespace if necessary
        for (i = 0; i < nsparts.length; i++) {
            partname = nsparts[i];
            // check if the current parent already has the namespace declared
            // if it isn't, then create it
            if (!parent[partname]) {
                parent[partname] = {};
            }
            // get a reference to the deepest element in the hierarchy so far
            parent = parent[partname];
        }
        // the parent is now constructed with empty namespaces and can be used.
        // we return the outermost namespace
        return parent;
    };

    //#region Configurations

    //Configurations for JSOM SharePoint
    SharePointClient.AddNameSpace("Configurations");
    SharePointClient.Configurations = {
        //From this property,JSOM model will create context accordingly 
        IsApp: false,
        //HostUrl from the querystring parameters
        SPHostUrl: "",
        //AppWebUrl from the querystring parameters
        SPAppWebUrl: "",
        //Cross domain request, for example app web can request data from host web.
        IsCrossDomainRequest: false,
        //This property is used to get the context by url
        SPUrl: null,
        //Configuration for REST
        REST: {
            //For authorization this token is used
            AccessToken: null
        }
    };
    //#endregion

    //#region Constants

    //Constants used across the namespace
    SharePointClient.AddNameSpace("Constants");
    SharePointClient.Constants = {

        //#region Caml Query Constants
        CAML_CONSTANT: {
            CAML_QUERY_SCOPE: {
                FILES_ONLY: "FilesOnly",
                RECURSIVE: "Recursive",
                RECURSIVE_ALL: "RecursiveAll"
            },
            CAML_QUERY_THROTTLE_MODE: {
                DEFAULT: "Default",
                OVERRIDE: "Override",
                STRICT: "Strict"
            }
        },
        //#endregion

        //#region REST Constants
        REST: {
            API: "_api",
            WEB: "web",
            LISTS: "Lists",
            REQUEST_DIGEST_ENDPOINT: "contextinfo",
            HTTP: {
                GET: "GET",
                POST: "POST",
                DATA_TYPE: {
                    JSON: "application/json;odata=verbose",
                    XML: "application/atom+xml;odata=verbose"
                }
            }
        }
        //#endregion
    };
    //#endregion

    //#region Logger
    //logger used to log the exception in console
    SharePointClient.AddNameSpace("Logger");
    SharePointClient.Logger = {
        LogJSOMException: function (ExceptionArgs) {
            ///<summary>
            /// Log JSOM service exception to Console.
            ///</summary>
            /// <param name="ExceptionArgs" type="Object">Exception Arguments</param>
            if (console) {
                console.log('Request failed. ' + ExceptionArgs.get_message() +
                    '\n' + ExceptionArgs.get_stackTrace());
            }
        },
        LogRESTException: function (Exception) {
            ///<summary>
            /// Log REST service exception to Console.
            ///</summary>
            /// <param name="Exception" type="String">Exception message</param>
            if (console) {
                console.log('Request failed. ' + Exception);
            }
        }
    };
    //#endregion

    //#region Utilities

    //Utilities for the SharePoint Client
    SharePointClient.AddNameSpace("Utilities");
    SharePointClient.Utilities.Utility = function () {

        var configuration = SharePointClient.Configurations;

        var queryString = function (param) {
            ///<summary>
            /// Get the value of querystring parameter from the current Url.
            ///</summary>
            /// <param name="param" type="String">Name of query string parameter</param>
            /// <returns type="String">Query string parameter value</returns>
            var params = document.URL.split("?")[1].split("&"), i = 0, singleParam;
            for (i = 0; i < params.length; i = i + 1) {
                singleParam = params[i].split("=");
                if (singleParam[0] === param) {
                    return decodeURIComponent(singleParam[1]);
                }
            }
        };

        var urlFromPageContextInfo = function () {
            ///<summary>
            /// SharePoint has the object for getting weburl and other properties,
            /// which is available in SharePoint pages
            ///</summary>
            /// <returns type="String">web absolute url</returns>
            var url;
            if (_spPageContextInfo) {
                url = _spPageContextInfo.webAbsoluteUrl;
            }
            return url;
        };

        var hostUrl = function () {
            ///<summary>
            /// SharePoint App page has the Host Url.
            ///</summary>
            /// <returns type="String">App Host Url</returns>
            return queryString("SPHostUrl");
        };

        var appWebUrl = function () {
            ///<summary>
            /// SharePoint App page has the AppWebUrl.
            ///</summary>
            /// <returns type="String">App Web Url</returns>
            return queryString("SPAppWebUrl");
        };

        var baseUrl = function () {
            ///<summary>
            /// Construct baseurl for downloading required JS files for working with JSOM.
            ///</summary>
            /// <returns type="String">Base url for SharePoint Site</returns>
            var scriptbase;
            if (configuration.IsApp) {
                scriptbase = hostUrl() + "/_layouts/15/";
            } else {
                if (_spPageContextInfo) {
                    scriptbase = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/";
                }
            }

            return scriptbase;
        };

        var _api = function () {
            ///<summary>
            /// SharePoint REST api service url.
            ///</summary>
            /// <returns type="String">REST api for service request</returns>
            var restApi;
            if (configuration.IsApp) {
                if (!configuration.SPUrl) {
                    restApi = queryString("SPAppWebUrl");
                } else {
                    restApi = configuration.SPUrl;
                }
            } else {
                if (_spPageContextInfo) {
                    restApi = _spPageContextInfo.webAbsoluteUrl;
                }
            }
            return restApi + "/" + SharePointClient.Constants.REST.API + "/";
        };

        var digestUrl = function () {
            ///<summary>
            /// SharePoint Request Digest url endpoint.
            ///</summary>
            /// <returns type="String">Endpoint for Digest REST service</returns>
            return _api() + SharePointClient.Constants.REST.REQUEST_DIGEST_ENDPOINT;
        };

        var crossDomainRequestUrl = function (url) {
            ///<summary>
            /// Modify the url to support cross domain access.
            ///</summary>
            /// <param name="url" type="String">url for cross domain call</param>
            /// <returns type="String">cross domain url</returns>
            var apiIndex = url.indexOf("_api/") + 5;
            var requestUrl = url.substring(0, apiIndex);
            requestUrl += "SP.AppContextSite(@target)/";
            requestUrl += url.substring(apiIndex, url.length);
            var sphostUrl = hostUrl();
            if (url.indexOf("?") > 0) {
                requestUrl = requestUrl + "&@target='" + sphostUrl + "'";
            } else {
                requestUrl = requestUrl + "?@target='" + sphostUrl + "'";
            }

            return requestUrl;
        };

        var isClientObjectAvailable = function (fileName) {
            ///<summary>
            /// Check if client objects are available on page.
            ///</summary>
            /// <param name="fileName" type="String">Name of Script to verify</param>
            /// <returns type="Boolean">TRUE if object available,FALSE if not available.</returns>
            switch (fileName) {
                case "sp.js": if (SP.ClientContext) { return true; } else { return false; }
                    break;
                case "sp.runtime.js": if (SP.ClientRuntimeContext) { return true; } else { return false; }
                    break;
                case "sp.requestexecutor.js": if (SP.RequestInfo) { return true; } else { return false; }
                    break;
                default: return false;
            }
        };

        var scriptAlreadyLoaded = function (scriptName) {
            ///<summary>
            /// Verify whether current page has script which is going to be download.
            ///</summary>
            /// <param name="scriptName" type="String">Name of Script to verify</param>
            /// <returns type="Boolean">TRUE for if already loaded,FALSE for its not loaded.</returns>
            scriptName = scriptName.toLowerCase();
            var isLoaded = false;
            var scripts = document.getElementsByTagName('script');
            for (var i = scripts.length; i--;) {
                if (scriptName === getScriptNameFromSrc(scripts[i].src)) {
                    isLoaded = true;
                }
            }

            if (!isLoaded) {
                isLoaded = isClientObjectAvailable(scriptName);
            }

            return isLoaded;

        };

        var getScriptNameFromSrc = function (src) {
            ///<summary>
            /// Get the Script file name from the src url.
            ///</summary>
            /// <param name="src" type="String">src script file url</param>
            /// <returns type="String">Script file name.</returns>

            //split if any ? rev added
            var srcArray = src.split("?");
            if (srcArray.length > 0) {
                src = srcArray[0];
            } else {
                src = src;
            }

            //read file name only
            var lastIndex = src.lastIndexOf("/");
            src = src.substring(lastIndex + 1, src.length);

            return src.toLowerCase();
        };

        var downloadScript = function (baseUrl, scriptUrls, index, callback) {
            ///<summary>
            /// Download JavaScript files Asynchronously.
            ///</summary>
            /// <param name="baseUrl" type="String">Base url for SharePoint layouts path</param>
            /// <param name="scriptUrls" type="Array">collection of Script Urls as Array</param>
            /// <param name="index" type="Number">Index in Array</param>
            /// <param name="callback" type="Function">CallBack function</param>
            $.getScript(baseUrl + scriptUrls[index], function () {
                if (index + 1 <= scriptUrls.length - 1) {
                    downloadScript(baseUrl, scriptUrls, index + 1, callback);
                } else {
                    if (callback) {
                        callback();
                    }
                }
            });
        };

        return {
            GetQueryStringParameter: queryString,
            GetHostUrl: hostUrl,
            GetAppWebUrl: appWebUrl,
            IsScriptExistsOnPage: scriptAlreadyLoaded,
            GetUrlFromPageContextInfo: urlFromPageContextInfo,
            GetScript: downloadScript,
            JSOM: {
                GetBaseUrl: baseUrl
            },
            REST: {
                GetApiUrl: _api,
                GetRequestDigestUrl: digestUrl,
                GetCrossDomainRequestUrl: crossDomainRequestUrl,
            }
        };
    };
    //#endregion

    //#region CAML Query Utility
    //Caml Query utility for creating caml schema from JSON object
    SharePointClient.AddNameSpace("Utilities.CamlQueryUtility");
    SharePointClient.Utilities.CamlQueryUtility = {
        //#region CamlQueryUtility function
        ConvertToCamlSchema: function (rootElement, jsonObject) {
            ///<summary>
            /// Convert JSON object to Caml schema.
            ///</summary>
            /// <param name="rootElement" type="String">Name of root element.</param>
            /// <param name="jsonObject" type="Object">JSON object.</param>
            /// <returns type="String">XML formatted caml query</returns>
            if (typeof jsonObject === "string" || typeof jsonObject === "number") {
                return "<" + rootElement + ">" + jsonObject + "</" + rootElement + ">";
            }

            var camlQueryXml = "<" + rootElement;
            var attrCollection = {}, elementCollection = {}, rootNodeValue;
            $.each(jsonObject, function (index, object) {
                if (SharePointClient.Utilities.CamlQueryUtility.IsAttribute(index)) {
                    attrCollection[index] = object;
                } else {
                    if (index.indexOf('_') === 0) {
                        rootNodeValue = object;
                    } else {
                        elementCollection[index] = object;
                    }
                }
            });

            //Append attributes if Any exists
            var attr = SharePointClient.Utilities.CamlQueryUtility.Attributes(attrCollection);
            if (attr.length > 0) {
                camlQueryXml += attr + ">";
            } else {
                camlQueryXml += ">";
            }

            //fields
            var fieldXml = "";
            $.each(elementCollection, function (k, v) {
                if (typeof v === "object") {
                    fieldXml += SharePointClient.Utilities.CamlQueryUtility.ConvertToCamlSchema(k, v);
                }
                else {
                    fieldXml += "<" + k + ">" + v + "</" + k + ">";
                }
            });

            if (fieldXml.length > 0) {
                camlQueryXml += fieldXml;
            }

            if (rootNodeValue) {
                camlQueryXml += rootNodeValue;
            }

            camlQueryXml += "</" + rootElement + ">";

            return camlQueryXml;
        },

        IsAttribute: function (input) {
            ///<summary>
            /// Is there any attributes defined in the JSON object.
            ///</summary>
            /// <param name="input" type="String">Name of element.</param>
            /// <returns type="Boolean">TRUE if attribute found, FALSE if attribute does not exists.</returns>
            if (typeof input === "object") {
                return false;
            }

            if (input.indexOf('@') === 0) {
                return true;
            } else {
                return false;
            }
        },

        Attributes: function (JsonObject) {
            ///<summary>
            /// Get the collection attributes from JSON object.
            ///</summary>
            /// <param name="JsonObject" type="Object">JSON Object.</param>
            /// <returns type="String">Formatted attribute collection</returns>
            if (typeof JsonObject === "string" || typeof jsonObject === "number") {
                if (JsonObject.indexOf('@') === 0) {
                    return JsonObject;
                } else {
                    return "";
                }

            }
            var attributes = "";
            $.each(JsonObject, function (index, object) {

                if (index.indexOf('@') === 0) {
                    attributes += " " + index.replace("@", "") + "='" + object + "'";
                }
            });

            return attributes;
        }
        //#endregion
    };
    //#endregion

    //#region Extend camlquery functionalities
    SharePointClient.AddNameSpace("CamlExtension");
    SharePointClient.CamlExtension = {
        //#region Extensions for JSOM/REST
        JSOM: {
            CamlQuery: function () {
                ///<summary>
                /// Extended caml query methods to support for creating camlQuery viewxml.
                ///</summary>
                $.extend(this, new SP.CamlQuery());
            }
        },
        REST: {
            CamlQuery: function () {
                ///<summary>
                /// custom camlquery class.
                ///</summary>

                //#region Private Variables
                var queryStatement = {}, viewXml = null;
                //#endregion

                var get_viewXml = function () {
                    ///<summary>
                    /// Return viewXml property.
                    ///</summary>
                    /// <returns type="String">caml query view xml</returns>
                    return viewXml;
                };

                var createAllItemsQuery = function () {
                    ///<summary>
                    /// This method will return default query for Allitems.
                    ///</summary>
                    /// <returns type="Object">Current class instance</returns>
                    viewXml = "<View Scope='RecursiveAll'><Query></Query></View>";
                    return this;
                };

                var createAllFoldersQuery = function () {
                    ///<summary>
                    /// This method will return default query for AllFolders.
                    ///</summary>
                    /// <returns type="Object">Current class instance</returns>
                    viewXml = "<View Scope='RecursiveAll'><Query><Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where></Query></View>";
                    return this;
                };

                var viewAttribute = function (scope) {
                    ///<summary>
                    /// This method will update Scope attribute for ViewXml,parameter as scope value.
                    ///</summary>
                    /// <param name="scope" type="String">Scope value.</param>
                    /// <returns type="Object">Current class instance</returns>

                    //Set scope
                    queryStatement.View = {
                        "@Scope": scope
                    };
                    return this;
                };

                var query = function (queryCondition) {
                    ///<summary>
                    /// This method will update the query condition for filtering the result set, parameter as caml formatted condition.
                    ///</summary>
                    /// <param name="queryCondition" type="String">Caml Query condition for filtering the result set.</param>
                    /// <returns type="Object">Current class instance</returns>

                    //Set scope
                    queryStatement.Query = queryCondition;

                    return this;
                };

                var viewFields = function (viewfields) {
                    ///<summary>
                    /// This method will be used to set the required columns in the result set,parameter as array of field names.
                    ///</summary>
                    /// <param name="viewfields" type="Array">Array of view fields names.</param>
                    /// <returns type="Object">Current class instance</returns>

                    var viewFieldsXml = "", i = 0;
                    for (i = 0; i <= viewfields.length - 1; i++) {
                        viewFieldsXml += "<FieldRef ";
                        viewFieldsXml += "Name='" + viewfields[i] + "'";
                        viewFieldsXml += "></FieldRef>";
                    }

                    queryStatement.ViewFields = viewFieldsXml;

                    return this;
                };

                var viewFieldsXml = function (viewfieldsXml) {
                    ///<summary>
                    /// This method will be used to set the required columns in the result set, parameter as xml.
                    ///</summary>
                    /// <param name="viewfieldsXml" type="String">XML formatted view fields collection.</param>
                    /// <returns type="Object">Current class instance</returns>

                    queryStatement.ViewFields = viewfieldsXml;
                    return this;
                };

                var queryThrottleMode = function (mode) {
                    ///<summary>
                    /// This method will be used Override the QueryThrottle mode for applying the throttle exception for this query or not.
                    ///</summary>
                    /// <param name="mode" type="String">Set Query Throttle mode.</param>
                    /// <returns type="Object">Current class instance</returns>
                    queryStatement.QueryOptions = {
                        "QueryThrottleMode": mode
                    };

                    return this;
                };

                var orderByIndex = function () {
                    ///<summary>
                    /// This method will override the order by, Use this method only when query has the condition with field as indexed.
                    ///</summary>
                    /// <returns type="Object">Current class instance</returns>
                    queryStatement.OrderBy = {
                        "@UseIndexForOrderBy": "TRUE",
                        "@Override": "TRUE"
                    };

                    return this;
                };

                var orderBy = function () {
                    ///<summary>
                    /// This method will override the order by with default ID field.
                    ///</summary>
                    /// <returns type="Object">Current class instance</returns>
                    queryStatement.OrderBy = {
                        "@UseIndexForOrderBy": "TRUE",
                        "FieldRef": {
                            "@Name": "ID"
                        }
                    };
                    return this;

                };

                var orderByDesc = function () {
                    ///<summary>
                    /// This method will override the order by with default ID field sortng order by Descending.
                    ///</summary>
                    /// <returns type="Object">Current class instance</returns>
                    queryStatement.OrderBy = {
                        "@UseIndexForOrderBy": "TRUE",
                        "FieldRef": {
                            "@Name": "ID",
                            "@Ascending": "FALSE"
                        }
                    };

                    return this;
                };

                var rowLimit = function (numberOfRecords) {
                    ///<summary>
                    /// This method will set the row limit.
                    ///</summary>
                    /// <returns type="Object">Current class instance</returns>
                    queryStatement.RowLimit = {
                        "@Paged": "TRUE",
                        "_value": numberOfRecords
                    };

                    return this;
                };

                var buildCamlQuery = function () {
                    ///<summary>
                    /// This method will build the vewXml.
                    ///</summary>
                    /// <returns type="Object">Current class instance</returns>

                    var camlUtility = SharePointClient.Utilities.CamlQueryUtility;

                    //CamlQuery Elements
                    var viewRootElement = "", //View is the root level element in camlQuery
                    queryElement = "",//Query element which has query conditions
                    viewFieldsElement = "",//ViewFields element for limit the fields in result while returning from list
                    queryOptionsElement = "",//QueryOptions for camlquery
                    orderByElement = "",//orderBy element for result set
                    rowLimit = "",//Rowlimit for result set
                    attributes;

                    //Root element View
                    var view = queryStatement.View;
                    attributes = camlUtility.Attributes(view);
                    if (attributes.length > 0) {
                        viewRootElement += "<View" + attributes + ">";
                    }
                    else { viewRootElement += "<View>"; }

                    //Query element if exists
                    var query = queryStatement.Query;
                    if (query) {
                        queryElement = camlUtility.ConvertToCamlSchema("Query", query);
                    }

                    //ViewFields element
                    var viewFields = queryStatement.ViewFields;
                    if (viewFields) {
                        viewFieldsElement = camlUtility.ConvertToCamlSchema("ViewFields", viewFields);
                    }

                    //QueryOptions element
                    var queryOptions = queryStatement.QueryOptions;
                    if (queryOptions) {
                        queryOptionsElement = camlUtility.ConvertToCamlSchema("QueryOptions", queryOptions);
                    }

                    //OrderBy element
                    var orderBy = queryStatement.OrderBy;
                    if (orderBy) {
                        orderByElement = camlUtility.ConvertToCamlSchema("OrderBy", orderBy);

                        //Append to Query object
                        if (queryElement.length === 0) {
                            queryElement += "<Query>";
                            queryElement += orderByElement;
                            queryElement += "</Query>";
                        } else {
                            //find end of Query and append order by
                            var endQueryIndex = queryElement.indexOf("</Query>");
                            var startQuery = queryElement.substring(0, endQueryIndex);
                            var endQuery = queryElement.substring(endQueryIndex, queryElement.length);
                            queryElement = startQuery + orderByElement + endQuery;
                        }

                    }

                    //RowLimit element
                    var rowlimit = queryStatement.RowLimit;
                    if (rowlimit) {
                        rowLimit = camlUtility.ConvertToCamlSchema("RowLimit", rowlimit);
                    }

                    viewRootElement += queryElement + viewFieldsElement + queryOptionsElement + rowLimit + "</View>";


                    //update viewXml property
                    viewXml = viewRootElement;

                    return this;
                };

                return {
                    SetViewScopeAttribute: viewAttribute,
                    SetQuery: query,
                    SetViewFields: viewFields,
                    SetViewFieldsXml: viewFieldsXml,
                    OverrideQueryThrottleMode: queryThrottleMode,
                    OverrideOrderByIndex: orderByIndex,
                    OverrideOrderBy: orderBy,
                    OverrideOrderByDesc: orderByDesc,
                    SetRowLimit: rowLimit,
                    BuildQuery: buildCamlQuery,
                    GetQueryViewXml: get_viewXml
                };
            }
        }
        //#endregion
    };
    //#endregion

    //#region Extend SP.CamlQuery methods
    SharePointClient.CamlExtension.JSOM.CamlQuery.prototype = {

        //#region Private Variables
        queryStatement: {},
        //#endregion

        CreateAllItemsQuery: function () {
            ///<summary>
            /// This method will return default query for Allitems.
            ///</summary>
            /// <returns type="String">View xml</returns>
            return SP.CamlQuery.createAllItemsQuery();
        },

        CreateAllFoldersQuery: function () {
            ///<summary>
            /// This method will return default query for AllFolders.
            ///</summary>
            /// <returns type="String">View xml</returns>
            return SP.CamlQuery.createAllFoldersQuery();
        },

        ViewAttribute: function (scope) {
            ///<summary>
            /// This method will update Scope attribute for ViewXml,parameter as scope value.
            ///</summary>
            /// <param name="scope" type="String">Scope value.</param>
            /// <returns type="Object">Current class instance</returns>
            //Set scope
            this.queryStatement.View = {
                "@Scope": scope
            };
            return this;
        },

        Query: function (queryCondition) {
            ///<summary>
            ///This method will update the query condition for filtering the result set, parameter as caml formatted condition.
            ///</summary>
            /// <param name="queryCondition" type="String">Caml Query condition for filtering the result set.</param>
            /// <returns type="Object">Current class instance</returns>

            this.queryStatement.Query = queryCondition;

            return this;
        },

        ViewFields: function (viewfields) {
            ///<summary>
            /// This method will be used to set the required columns in the result set,parameter as array of field names.
            ///</summary>
            /// <param name="viewfields" type="Array">Array of view fields names.</param>
            /// <returns type="Object">Current class instance</returns>

            var viewFieldsXml = "", i = 0;
            for (i = 0; i <= viewfields.length - 1; i++) {
                viewFieldsXml += "<FieldRef ";
                viewFieldsXml += "Name='" + viewfields[i] + "'";
                viewFieldsXml += "></FieldRef>";
            }

            this.queryStatement.ViewFields = viewFieldsXml;

            return this;
        },

        ViewFieldsXml: function (viewfieldsXml) {
            ///<summary>
            /// This method will be used to set the required columns in the result set, parameter as xml.
            ///</summary>
            /// <param name="viewfieldsXml" type="String">XML formatted view fields collection.</param>
            /// <returns type="Object">Current class instance</returns>
            this.queryStatement.ViewFields = viewfieldsXml;
            return this;
        },

        QueryThrottleMode: function (mode) {
            ///<summary>
            /// This method will be used Override the QueryThrottle mode for applying the throttle exception for this query or not.
            ///</summary>
            /// <param name="mode" type="String">Set Query Throttle mode.</param>
            /// <returns type="Object">Current class instance</returns>
            this.queryStatement.QueryOptions = {
                "QueryThrottleMode": mode
            };

            return this;
        },

        OrderByIndex: function () {
            ///<summary>
            /// This method will override the order by, Use this method only when query has the condition with field as indexed.
            ///</summary>
            /// <returns type="Object">Current class instance</returns>
            this.queryStatement.OrderBy = {
                "@UseIndexForOrderBy": "TRUE",
                "@Override": "TRUE"
            };

            return this;
        },

        OrderBy: function () {
            ///<summary>
            /// This method will override the order by with default ID field.
            ///</summary>
            /// <returns type="Object">Current class instance</returns>
            this.queryStatement.OrderBy = {
                "@UseIndexForOrderBy": "TRUE",
                "FieldRef": {
                    "@Name": "ID"
                }
            };
            return this;
        },

        OrderByDesc: function () {
            ///<summary>
            /// This method will override the order by with default ID field sortng order by Descending.
            ///</summary>
            /// <returns type="Object">Current class instance</returns>
            this.queryStatement.OrderBy = {
                "@UseIndexForOrderBy": "TRUE",
                "FieldRef": {
                    "@Name": "ID",
                    "@Ascending": "FALSE"
                }
            };

            return this;
        },

        RowLimit: function (numberOfRecords) {
            ///<summary>
            /// This method will set the row limit.
            ///</summary>
            /// <returns type="Object">Current class instance</returns>
            this.queryStatement.RowLimit = numberOfRecords;
            return this;
        },

        BuildQuery: function () {
            ///<summary>
            /// This method will build the vewXml.
            ///</summary>
            /// <returns type="Object">Current class instance</returns>
            var camlUtility = SharePointClient.Utilities.CamlQueryUtility;

            //CamlQuery Elements
            var viewRootElement = "",//View is the root level element in camlQuery
            queryElement = "",//Query element which has query conditions
            viewFieldsElement = "",//ViewFields element for limit the fields in result while returning from list
            queryOptionsElement = "",//QueryOptions for camlquery
            orderByElement = "",//orderBy element for result set
            rowLimit = "",//Rowlimit for result set
            attributes;
            //Root element View
            var view = this.queryStatement.View;
            attributes = camlUtility.Attributes(view);
            if (attributes.length > 0) {
                viewRootElement += "<View" + attributes + ">";
            }
            else { viewRootElement += "<View>"; }

            //Query element if exists
            var query = this.queryStatement.Query;
            if (query) {
                queryElement = camlUtility.ConvertToCamlSchema("Query", query);
            }

            //ViewFields element
            var viewFields = this.queryStatement.ViewFields;
            if (viewFields) {
                viewFieldsElement = camlUtility.ConvertToCamlSchema("ViewFields", viewFields);
            }

            //QueryOptions element
            var queryOptions = this.queryStatement.QueryOptions;
            if (queryOptions) {
                queryOptionsElement = camlUtility.ConvertToCamlSchema("QueryOptions", queryOptions);
            }

            //OrderBy element
            var orderBy = this.queryStatement.OrderBy;
            if (orderBy) {
                orderByElement = camlUtility.ConvertToCamlSchema("OrderBy", orderBy);

                //Append to Query object
                if (queryElement.length === 0) {
                    queryElement += "<Query>";
                    queryElement += orderByElement;
                    queryElement += "</Query>";
                } else {
                    //find end of Query and append order by
                    var endQueryIndex = queryElement.indexOf("</Query>");
                    var startQuery = queryElement.substring(0, endQueryIndex);
                    var endQuery = queryElement.substring(endQueryIndex, queryElement.length);
                    queryElement = startQuery + orderByElement + endQuery;
                }

            }

            //RowLimit element
            var rowlimit = this.queryStatement.RowLimit;
            if (rowlimit) {
                rowLimit = camlUtility.ConvertToCamlSchema("RowLimit", rowlimit);
            }

            viewRootElement += queryElement + viewFieldsElement + queryOptionsElement + rowLimit + "</View>";

            this.set_viewXml(viewRootElement);

            return this;
        },
    };
    //#endregion

    //#region Services  for JSOM and REST
    SharePointClient.AddNameSpace("Services");
    SharePointClient.Services = {

        //#region custom deferred object like JQuery deferred
        ClientRun: function () {
            ///<summary>
            /// ClientRun class to register callback for success and error like deferred in JQuery.
            ///</summary>

            var successFunction = function (result) {
                ///<summary>
                /// Success handler.
                ///</summary>
                /// <param name="result" type="Object">data returned from the request to success handler.</param>

                /* override in instance method*/
            };

            var errorFunction = function () {
                ///<summary>
                /// Error handler.
                ///</summary>

                /* override in instance method*/
            };

            var execute = function (success, error) {
                ///<summary>
                /// This function register Success and Error callback functions.
                ///</summary>
                /// <param name="success" type="Function">CallBack function for calling this function on Success.</param>
                /// <param name="error" type="Function">CallBack function for calling this function on Error.</param>
                if (success) {
                    successFunction = success;
                }

                if (error) {
                    errorFunction = error;
                }
            };

            return {
                Execute: execute,
                OnSuccess: function (result) { return successFunction(result); },
                OnError: function () { return errorFunction; }
            };
        },
        //#endregion

        //#region JSOM service
        JSOM: {

            Initialize: function (callback) {
                ///<summary>
                /// Initialize the JSOM with loading required JS files.
                ///</summary>
                /// <param name="callback" type="Function">CallBack function for calling this function once files downloaded.</param>

                var utility = new SharePointClient.Utilities.Utility();
                var config = SharePointClient.Configurations;
                var baseUrl = utility.JSOM.GetBaseUrl();

                var jsFiles = ["SP.Runtime.js", "SP.js"];

                if (config.IsCrossDomainRequest) {
                    //load SP.RequestExecutor if not mentioned in js Array
                    var jsExists = jQuery.grep(jsFiles, function (n, i) {
                        return (n !== "" && n !== null && n === "SP.RequestExecutor.js");
                    });

                    if (jsExists.length === 0) {
                        jsFiles.push("SP.RequestExecutor.js");
                    }
                }

                //Remove all ready loaded files from the collection
                var loadJsFiles = [];
                $.each(jsFiles, function (k, v) {
                    if (!utility.IsScriptExistsOnPage(v)) {
                        loadJsFiles.push(v);
                    }
                });

                //if all files are loaded
                if (loadJsFiles.length === 0) {
                    callback();
                    return;
                }


                //load Script files
                $.ajaxSetup({
                    cache: true
                });

                utility.GetScript(baseUrl, loadJsFiles, 0, function () {
                    callback();
                });
                $.ajaxSetup({
                    cache: false
                });
            },

            Context: function () {
                ///<summary>
                /// Context class used to get the correct context from the configuration.
                ///</summary>

                var utility = new SharePointClient.Utilities.Utility();

                //#region Private Variables
                var clientContext = null, web = null;
                //#endregion

                var current = function () {
                    ///<summary>
                    /// This method is used while working on SharePoint pages, beacuse sharePoint default provide context.
                    ///</summary>
                    if (SP.ClientContext) {
                        clientContext = SP.ClientContext.get_current();
                        web = clientContext.get_web();
                    }
                };

                var contextByUrl = function (url) {
                    ///<summary>
                    /// Create context by SharePoint Web url.
                    ///</summary>
                    /// <param name="url" type="String">Url of SharePoint site.</param>
                    if (SP.ClientContext) {
                        clientContext = new SP.ClientContext(url);
                        web = clientContext.get_web();
                    }
                };

                var crossDomainContext = function () {
                    ///<summary>
                    /// if the Context required for cross domain request, for example access host web from SharePoint hosted app.
                    ///</summary>

                    contextByUrl(utility.GetAppWebUrl());

                    var factory = new SP.ProxyWebRequestExecutorFactory(utility.GetAppWebUrl());
                    clientContext.set_webRequestExecutorFactory(factory);

                    var AppContextSite = new SP.AppContextSite(clientContext, utility.GetHostUrl());
                    web = AppContextSite.get_web();
                };

                var init = function () {
                    ///<summary>
                    /// Intialize the context for SharePoint.
                    ///</summary>

                    //if confiuration property App is set TRUE , set HostUrl and AppWebUrl
                    var configuration = SharePointClient.Configurations;
                    if (configuration.IsApp) {
                        configuration.SPHostUrl = utility.GetHostUrl();
                        configuration.SPAppWebUrl = utility.GetAppWebUrl();

                        //Check if the request would be cross domain call or call for current site
                        if (configuration.IsCrossDomainRequest) {
                            //Create cross domain client context
                            crossDomainContext();
                        } else if (!configuration.SPUrl) {
                            //Create context for App Web
                            contextByUrl(utility.GetAppWebUrl());
                        } else {
                            //Create context SPUrl property
                            contextByUrl(configuration.SPUrl);
                        }
                    } else {
                        //Create context for sharepoint site where required Js files are available
                        current();
                    }
                };

                return {
                    GetClientContext: function () {
                        if (!clientContext) {
                            //Initialize the client context
                            init();
                        }
                        return clientContext;
                    },
                    GetWeb: function () {
                        if (!clientContext) {
                            //Initialize the client context
                            init();
                        }
                        return web;
                    }
                };
            },

            ListServices: function () {
                ///<summary>
                /// JSOM list service.
                ///</summary>

                var lists = function (context) {
                    ///<summary>
                    /// Get the lists collection.
                    ///</summary>
                    /// <param name="context" type="Object">SharePoint Client Context.</param>
                    /// <returns type="Object">Lists collection</returns>

                    var clientContext = context.GetClientContext();
                    var web = context.GetWeb();
                    var lists = web.get_lists();
                    clientContext.load(lists, 'Include(Title, Id)');

                    return executeQuery(clientContext, lists);
                };

                var listByTitle = function (context, listTitle) {
                    ///<summary>
                    /// Get the list by title.
                    ///</summary>
                    /// <param name="context" type="Object">SharePoint Client Context.</param>
                    /// <param name="listTitle" type="String">List Name.</param>
                    /// <returns type="Object"> List </returns>

                    var clientContext = context.GetClientContext();
                    var web = context.GetWeb();

                    var lists = web.get_lists();
                    var list = lists.getByTitle(listTitle);

                    clientContext.load(list);

                    return executeQuery(clientContext, list);
                };

                var listItemsByListName = function (context, listTitle, camlQuery) {
                    ///<summary>
                    /// Get the items by listname.
                    ///</summary>
                    /// <param name="context" type="Object">SharePoint Client Context.</param>
                    /// <param name="listTitle" type="String">List Name.</param>
                    /// <param name="camlQuery" type="Object">CamlQuery object.</param>                    
                    /// <returns type="Object"> ListItemsCoellction </returns>

                    var run = new SharePointClient.Services.ClientRun();
                    var position = new SP.ListItemCollectionPosition();
                    position.set_pagingInfo("");
                    camlQuery.set_listItemCollectionPosition(position);

                    var itemsCollection = null;

                    var runBatch = new SharePointClient.Services.ClientRun();
                    delegateRequest(context, camlQuery, listTitle, runBatch);

                    runBatch.Execute(function (result) {
                        if (itemsCollection) {
                            $.each(result, function (index, value) {
                                if ($.isArray(value)) {
                                    //Get the previous array collection
                                    $.each(itemsCollection, function (cIndex, cValue) {
                                        if ($.isArray(cValue)) {
                                            $.each(value, function (k, v) {
                                                cValue.push(v);
                                            });
                                        }
                                    });
                                }
                            });
                        } else {
                            itemsCollection = result;
                        }

                        if (!result.get_listItemCollectionPosition()) {
                            if (!itemsCollection) {
                                itemsCollection = new SP.ListItemCollection(context.GetClientContext());
                            }
                            //Set listitemcollection position
                            var nextPageInfo = itemsCollection.get_listItemCollectionPosition();
                            nextPageInfo.set_pagingInfo(result.get_listItemCollectionPosition());

                            run.OnSuccess(itemsCollection);
                        }
                    });

                    return run;
                };

                var listItemsByBatch = function (context, listTitle, camlQuery) {
                    ///<summary>
                    /// Get the list items batch by batch.
                    ///</summary>
                    /// <param name="context" type="Object">SharePoint Client Context.</param>
                    /// <param name="listTitle" type="String">List Name.</param>
                    /// <param name="camlQuery" type="Object">CamlQuery object.</param>                    
                    /// <returns type="Object"> ListItemsCoellction </returns>

                    var run = new SharePointClient.Services.ClientRun();

                    var position = new SP.ListItemCollectionPosition();
                    position.set_pagingInfo("");
                    camlQuery.set_listItemCollectionPosition(position);

                    delegateRequest(context, camlQuery, listTitle, run);

                    return run;
                };

                var delegateRequest = function (context, camlQuery, listTitle, run) {
                    ///<summary>
                    /// This is the delegate request called recursively when more items to be fetched in batch.
                    ///</summary>
                    /// <param name="context" type="Object">SharePoint Client Context.</param>
                    /// <param name="listTitle" type="String">List Name.</param>
                    /// <param name="camlQuery" type="Object">CamlQuery object.</param>                    
                    /// <returns type="Object"> ListItemsCollection </returns>

                    var clientContext = context.GetClientContext();
                    var web = context.GetWeb();
                    var list = web.get_lists().getByTitle(listTitle);
                    var itemCollection = list.getItems(camlQuery);
                    clientContext.load(itemCollection);

                    clientContext.executeQueryAsync(
                            function () {
                                run.OnSuccess(itemCollection);

                                //Iterate if more items needs to be fetched
                                if (itemCollection.get_listItemCollectionPosition()) {
                                    camlQuery.set_listItemCollectionPosition(itemCollection.get_listItemCollectionPosition());
                                    delegateRequest(context, camlQuery, listTitle, run);
                                } else {
                                    return;
                                }
                            },
                            function (sender, args) {
                                SharePointClient.Logger.LogJSOMException(args);
                            });
                };

                var executeQuery = function (clientContext, clientObject) {
                    ///<summary>
                    /// Execute query for SharePoint client and returning promise object.
                    ///</summary>
                    /// <param name="clientContext" type="Object">SharePoint Client Context.</param>                   
                    /// <returns type="Object"> loaded object </returns>
                    var run = new SharePointClient.Services.ClientRun();

                    clientContext.executeQueryAsync(
                        function (sender, args) {
                            run.OnSuccess(clientObject);
                        },
                        function (sender, args) {
                            SharePointClient.Logger.LogJSOMException(args);
                            run.OnError(args);
                        });

                    return run;
                };

                return {
                    GetLists: lists,
                    GetListByTitle: listByTitle,
                    GetListItemsByListName: listItemsByListName,
                    GetListItemsBatchByListName: listItemsByBatch
                };
            }
        },
        //#endregion

        //#region REST service
        REST: {
            RESTService: function () {
                ///<summary>
                /// REST service class.
                ///</summary>

                $.support.cors = true;

                var ajax = {
                    //#region Ajax call without Request digest
                    call: function (url, type, data, responseType, requireStringify) {
                        ///<summary>
                        /// Ajax call.
                        ///</summary>
                        /// <param name="url" type="String">Url of service.</param>
                        /// <param name="type" type="String">Request type.</param>
                        /// <param name="data" type="Object">Request data.</param>
                        /// <param name="responseType" type="String">Response type.</param>
                        /// <param name="requireStringify" type="Boolean">Stringify request data or not.</param>
                        /// <returns type="Object">response</returns>
                        var call = $.ajax({
                            url: url,
                            type: type,
                            contentType: responseType,
                            data: stringify(data, requireStringify),
                            headers: getHeaders(responseType)
                        });
                        return (call.then(success, error));
                    }
                    //#endregion
                };

                var ajaxWithFormDigest = {
                    //#region Ajax call with Request digest value
                    call: function (url, type, data, digestValue, responseType, requireStringify) {
                        ///<summary>
                        /// Ajax call.
                        ///</summary>
                        /// <param name="url" type="String">Url of service.</param>
                        /// <param name="type" type="String">Request type.</param>
                        /// <param name="data" type="Object">Request data.</param>
                        /// <param name="digestValue" type="String">Page Digest value for SharePoint.</param>
                        /// <param name="responseType" type="String">Response type.</param>
                        /// <param name="requireStringify" type="Boolean">Stringify request data or not.</param>
                        /// <returns type="Object">response</returns>
                        var call = $.ajax({
                            url: url,
                            type: type,
                            data: stringify(data, requireStringify),
                            headers: getHeadersWithDigest(responseType, digestValue)
                        });
                        return (call.then(success, error));
                    }
                    //#endregion
                };

                var crossDomainRequest = {
                    //#region CrossDomain Request
                    call: function (url, type, data, responseType, requireStringify) {
                        ///<summary>
                        /// Ajax call.
                        ///</summary>
                        /// <param name="url" type="String">Url of service.</param>
                        /// <param name="type" type="String">Request type.</param>
                        /// <param name="data" type="Object">Request data.</param>
                        /// <param name="responseType" type="String">Response type.</param>
                        /// <param name="requireStringify" type="Boolean">Stringify request data or not.</param>
                        /// <returns type="Object">response</returns>
                        var dfd = jQuery.Deferred();
                        var utility = new SharePointClient.Utilities.Utility();
                        var executor = new SP.RequestExecutor(utility.GetQueryStringParameter("SPAppWebUrl"));
                        var requestUrl = utility.REST.GetCrossDomainRequestUrl(url);
                        var call = executor.executeAsync({
                            url: requestUrl,
                            method: type,
                            headers: {
                                "Accept": responseType,
                                "Content-Type": SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON
                            },
                            body: JSON.stringify(data),
                            success: function (data) {
                                dfd.resolve(data.body);
                            },
                            error: function (data, errorCode, errorMessage) {
                                dfd.reject(errorMessage);
                            }
                        });

                        return dfd.promise();
                    }
                    //#endregion
                };

                var stringify = function (data, requireStringify) {
                    ///<summary>
                    /// Convert JSON format of request data.
                    ///</summary>
                    /// <param name="data" type="Object">Request data.</param>
                    /// <param name="requireStringify" type="Boolean">TRUE for JSON stringify, FALSE for not converting.</param>
                    /// <returns type="Object">Request data</returns>
                    if (requireStringify) {
                        return JSON.stringify(data);
                    }
                    else {
                        return data;
                    }
                };

                var getHeaders = function (responseType) {
                    ///<summary>
                    /// Construct request headers.
                    ///</summary>
                    /// <param name="responseType" type="String">Response type.</param>
                    /// <returns type="Object">Request Headers object</returns>
                    var headers = {
                        "Accept": responseType
                    };

                    if (SharePointClient.Configurations.AccessToken) {
                        headers.Authorization = "Bearer " + SharePointClient.Configurations.AccessToken;
                    }

                    return headers;
                };

                var getHeadersWithDigest = function (responseType, digestValue) {
                    ///<summary>
                    /// Construct request headers with requet digest value.
                    ///</summary>
                    /// <param name="responseType" type="String">Response type.</param>
                    /// <param name="digestValue" type="String">Digest value for SharePoint.</param>
                    /// <returns type="Object">Request Headers object</returns>
                    var headers = {
                        "Accept": responseType,
                        "X-RequestDigest": digestValue,
                        "Content-Type": SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON,
                    };

                    if (SharePointClient.Configurations.AccessToken) {
                        headers.Authorization = "Bearer " + SharePointClient.Configurations.AccessToken;
                    }

                    return headers;
                };

                var success = function (data) {
                    ///<summary>
                    /// Success Event handler for Asynchronous calls.
                    ///</summary>
                    /// <param name="data" type="Object">Response.</param>
                    /// <returns type="Object">Response data</returns>
                    return data;
                };

                var error = function (xhr, errorType, exception) {
                    ///<summary>
                    /// Error Event handler for Asynchronous calls.
                    ///</summary>
                    /// <param name="xhr" type="Object">Response related object for Error details.</param>
                    /// <param name="errorType" type="String">Type of exception.</param>
                    /// <param name="exception" type="String">Exception message.</param>
                    SharePointClient.Logger.LogRESTException("Exception : " + xhr.responseText);
                };

                return {
                    Request: ajax.call,
                    RequestWithDigest: ajaxWithFormDigest.call,
                    RequestCrossDomain: crossDomainRequest.call
                };
            },

            ListServices: function () {
                ///<summary>
                /// SharePoint REST ListServices.
                ///</summary>

                var utility = new SharePointClient.Utilities.Utility();
                var constants = SharePointClient.Constants.REST;
                var Service = SharePointClient.Services.REST.RESTService();

                var lists = function (responseType) {
                    ///<summary>
                    /// Get the lists collection.
                    ///</summary>
                    /// <param name="responseType" type="String">Response type XML/JSON.</param>
                    /// <returns type="Object">Lists collection data</returns>
                    var run = new SharePointClient.Services.ClientRun();
                    var RequestUrl = utility.REST.GetApiUrl() + constants.WEB + "/" + constants.LISTS;

                    Service.Request(RequestUrl, constants.HTTP.GET, null, responseType, false).then(
                        function (result) {
                            run.OnSuccess(result);
                        });

                    return run;
                };

                var listByTitle = function (listTitle, responseType) {
                    ///<summary>
                    /// Get the list by title.
                    ///</summary>
                    /// <param name="listTitle" type="String">List Name.</param>
                    /// <param name="responseType" type="String">Response type XML/JSON.</param>
                    /// <returns type="Object">List</returns>
                    var run = new SharePointClient.Services.ClientRun();
                    var RequestUrl = utility.REST.GetApiUrl() + constants.WEB + "/" + constants.LISTS + "/getbytitle('" + listTitle + "')";

                    Service.Request(RequestUrl, constants.HTTP.GET, null, responseType, false).then(
                        function (result) {
                            run.OnSuccess(result);
                        });

                    return run;
                };

                var executeCrossDomainRequest = function (requestUrl, requestType, requestData, responseType, run) {
                    ///<summary>
                    /// This is the delegate request for cross domain which is called recursively when more items to be fetched in batch.
                    ///</summary>
                    /// <param name="requestUrl" type="String">Request Url.</param>
                    /// <param name="requestType" type="String">Request Type.</param>
                    /// <param name="requestData" type="Object">Request data.</param>
                    /// <param name="responseType" type="String">Response type XML/JSON.</param>
                    /// <returns type="Object">response data</returns>
                    return Service.RequestCrossDomain(requestUrl, requestType, requestData, responseType, true).then(
                        function (data) {

                            //Verify if more items are present or not
                            var convertResult;
                            if (responseType === SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON) {
                                convertResult = $.parseJSON($.parseJSON(data).d.RenderListData);
                            } else {
                                convertResult = $.parseJSON($($.parseXML(data).lastChild).text());
                            }

                            //call callback function
                            run.OnSuccess(data);

                            if (convertResult.NextHref) {
                                //update the Request Url for next batch
                                var Url = requestUrl;
                                var queryParam = requestUrl.split("?");
                                if (queryParam.length === 2) {
                                    Url = queryParam[0] + convertResult.NextHref;
                                } else {
                                    Url = requestUrl + convertResult.NextHref;
                                }

                                executeCrossDomainRequest(Url, SharePointClient.Constants.REST.HTTP.POST, requestData, responseType, run);
                            }

                            return convertResult;
                        }, function (xhr, errorType, exception) {
                            SharePointClient.Logger.LogRESTException("Exception : " + xhr.responseText);
                        });
                };

                var delegateRequest = function (requestUrl, requestType, requestData, digestValue, responseType, run) {
                    ///<summary>
                    /// This is the delegate request called recursively when more items to be fetched in batch.
                    ///</summary>
                    /// <param name="requestUrl" type="String">Request Url.</param>
                    /// <param name="requestType" type="String">Request Type.</param>
                    /// <param name="requestData" type="Object">Request data.</param>
                    /// <param name="digestValue" type="String">Request Digest value.</param>
                    /// <param name="responseType" type="String">Response type XML/JSON.</param>
                    /// <returns type="Object">response data</returns>
                    return Service.RequestWithDigest(requestUrl, requestType, requestData, digestValue, responseType, true).then(
                         function (data) {

                             //Verify if more items are present or not
                             var convertResult, Url = requestUrl, queryParam = requestUrl.split("?");
                             if (responseType === SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON) {
                                 convertResult = JSON.parse(data.d.RenderListData);
                             } else {
                                 convertResult = $.parseJSON($(data.lastChild).text());
                             }

                             //call callback function
                             run.OnSuccess(data);

                             if (convertResult.NextHref) {
                                 //update the Request Url for next batch                                
                                 if (queryParam.length === 2) {
                                     Url = queryParam[0] + convertResult.NextHref;
                                 } else {
                                     Url = requestUrl + convertResult.NextHref;
                                 }

                                 delegateRequest(Url, SharePointClient.Constants.REST.HTTP.POST, requestData, digestValue, responseType, run);
                             }

                             return convertResult;
                         }, function (xhr, errorType, exception) {
                             SharePointClient.Logger.LogRESTException("Exception : " + xhr.responseText);
                         });
                };

                var formatListCollection = function (dataCollection, responseType) {
                    ///<summary>
                    /// Format the collection object to JSON.
                    ///</summary>
                    /// <param name="dataCollection" type="String">Response data from the request.</param>
                    /// <param name="responseType" type="String">Response type XML/JSON.</param>
                    /// <returns type="Object">ListItems collection data</returns>
                    var finalFormatteData = null;
                    if (SharePointClient.Configurations.IsCrossDomainRequest) {
                        if (responseType === SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON) {
                            finalFormatteData = $.parseJSON($.parseJSON(dataCollection).d.RenderListData);
                        } else {
                            finalFormatteData = $.parseJSON($($.parseXML(dataCollection).lastChild).text());
                        }
                    } else {
                        if (responseType === SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON) {
                            finalFormatteData = JSON.parse(dataCollection.d.RenderListData);
                        } else {
                            finalFormatteData = $.parseJSON($(dataCollection.lastChild).text());
                        }
                    }

                    return finalFormatteData;
                };

                var convertRenderListDataToXml = function (jsonData) {
                    ///<summary>
                    /// Convert the RenderListData json to Xml Document.
                    ///</summary>
                    /// <param name="jsonData" type="Object">JSON object.</param>
                    /// <returns type="String">Xml document as string</returns>
                    var xmlString = "<d:RenderListData xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" " +
                        "xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\" " +
                        "xmlns:georss=\"http://www.georss.org/georss\" " +
                        "xmlns:gml=\"http://www.opengis.net/gml\">" + JSON.stringify(jsonData) +
                        "</d:RenderListData>";

                    return xmlString;
                };

                var listItemsByListName = function (listTitle, camlQuery, responseType) {
                    ///<summary>
                    /// Get the items by listname in batch by batch.
                    ///</summary>
                    /// <param name="listTitle" type="String">List Name.</param>
                    /// <param name="camlQuery" type="String">View Xml.</param>
                    /// <param name="responseType" type="String">Response type XML/JSON.</param>
                    /// <returns type="Object">ListItems collection data</returns>
                    var run = new SharePointClient.Services.ClientRun();
                    var runBatch = new SharePointClient.Services.ClientRun();

                    var RequestUrl = utility.REST.GetApiUrl() + constants.WEB + "/" + constants.LISTS + "/getbytitle('" + listTitle + "')/RenderListData";

                    var requestData = {
                        "viewXml": camlQuery.GetQueryViewXml()
                    };

                    if (SharePointClient.Configurations.IsCrossDomainRequest) {
                        $.getScript(utility.GetQueryStringParameter("SPHostUrl") + "/_layouts/15/SP.RequestExecutor.js", function () {
                            return executeCrossDomainRequest(RequestUrl, constants.HTTP.POST, requestData, responseType, runBatch);
                        });
                    } else {
                        Service.Request(utility.REST.GetRequestDigestUrl(), constants.HTTP.POST, null, constants.HTTP.DATA_TYPE.JSON, false).then(function (data) {
                            var NewDigest = data.d.GetContextWebInformation.FormDigestValue;
                            delegateRequest(RequestUrl, constants.HTTP.POST, requestData, NewDigest, responseType, runBatch);
                        });
                    }

                    var itemsCollection = null;
                    runBatch.Execute(function (result) {
                        //Convert the result to JSON
                        var modifiedResult = formatListCollection(result, responseType);
                        if (itemsCollection) {
                            $.each(modifiedResult, function (index, value) {
                                if ($.isArray(value)) {
                                    //Get the previous array collection
                                    $.each(itemsCollection, function (cIndex, cValue) {
                                        if ($.isArray(cValue)) {
                                            $.each(value, function (k, v) {
                                                cValue.push(v);
                                            });
                                        }
                                    });
                                }
                            });
                        } else {
                            itemsCollection = modifiedResult;
                        }

                        if (!modifiedResult.NextHref) {
                            //Set next item collection query string
                            itemsCollection.NextHref = modifiedResult.NextHref;

                            //Convert the final result on basis of response type
                            var finalResult = itemsCollection;
                            if (responseType === SharePointClient.Constants.REST.HTTP.DATA_TYPE.XML) {
                                finalResult = $.parseXML(convertRenderListDataToXml(finalResult));
                            } else {
                                var jsonString = {
                                    d: {
                                        RenderListData: JSON.stringify(finalResult)
                                    }
                                };

                                finalResult = jsonString;
                            }
                            run.OnSuccess(finalResult);
                        }
                    });

                    return run;
                };

                var listItemsBatchByListName = function (listTitle, camlQuery, responseType) {
                    ///<summary>
                    /// Get the items by listname in batch by batch.
                    ///</summary>
                    /// <param name="listTitle" type="String">List Name.</param>
                    /// <param name="camlQuery" type="String">View Xml.</param>
                    /// <param name="responseType" type="String">Response type XML/JSON.</param>
                    /// <returns type="Object">ListItems collection data</returns>

                    var run = new SharePointClient.Services.ClientRun();

                    var RequestUrl = utility.REST.GetApiUrl() + constants.WEB + "/" + constants.LISTS + "/getbytitle('" + listTitle + "')/RenderListData";

                    var requestData = {
                        "viewXml": camlQuery.GetQueryViewXml()
                    };

                    if (SharePointClient.Configurations.IsCrossDomainRequest) {
                        $.getScript(utility.GetQueryStringParameter("SPHostUrl") + "/_layouts/15/SP.RequestExecutor.js", function () {
                            return executeCrossDomainRequest(RequestUrl, constants.HTTP.POST, requestData, responseType, run);
                        });
                    } else {
                        Service.Request(utility.REST.GetRequestDigestUrl(), constants.HTTP.POST, null, constants.HTTP.DATA_TYPE.JSON, false).then(function (data) {
                            var NewDigest = data.d.GetContextWebInformation.FormDigestValue;
                            delegateRequest(RequestUrl, constants.HTTP.POST, requestData, NewDigest, responseType, run);
                        });
                    }

                    return run;
                };

                return {
                    GetLists: lists,
                    GetListByTitle: listByTitle,
                    GetListItemsByListName: listItemsByListName,
                    GetListItemsBatchByListName: listItemsBatchByListName
                };
            }
        }
        //#endregion
    };
    //#endregion
})();
