'use strict';

var SharePointClient = SharePointClient || {};
(function () {
    SharePointClient.AddNameSpace = function (namespace) {
        var nsparts = namespace.split(".");
        var parent = SharePointClient;

        // we want to be able to include or exclude the root namespace so we strip
        // it if it's in the namespace
        if (nsparts[0] === "SharePointClient") {
            nsparts = nsparts.slice(1);
        }

        // loop through the parts and create a nested namespace if necessary
        for (var i = 0; i < nsparts.length; i++) {
            var partname = nsparts[i];
            // check if the current parent already has the namespace declared
            // if it isn't, then create it
            if (typeof parent[partname] === "undefined") {
                parent[partname] = {};
            }
            // get a reference to the deepest element in the hierarchy so far
            parent = parent[partname];
        }
        // the parent is now constructed with empty namespaces and can be used.
        // we return the outermost namespace
        return parent;
    };

    
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
            AccessToken: null,
        }
    };

    //Constants used across the namespace
    SharePointClient.AddNameSpace("Constants");
    SharePointClient.Constants = {
        CAML_CONSTANT: {
            VIEWXML: "<View></View>",
            CAML_QUERY_SCOPE: {
                filesOnly: "FilesOnly",
                recursive: "Recursive",
                recursiveAll: "RecursiveAll"
            },
            CAML_QUERY_THROTTLE_MODE: {
                setdefault: "Default",
                override: "Override",
                strict: "Strict"
            }
        },
        JSOM: {
            APP_WEB_TYPE: {
                hostWeb: "HOSTWEB",
                appWeb: "APPWEB"
            }
        },
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

    };

    //logger used to log the exception in console
    SharePointClient.AddNameSpace("Logger");
    SharePointClient.Logger = {
        LogJSOMException: function (ExceptionArgs) {
            console.log('Request failed. ' + ExceptionArgs.get_message() +
        '\n' + ExceptionArgs.get_stackTrace());
        },
        LogRESTException: function (Exception) {
            console.log('Request failed. ' + Exception);
        }
    };

    //Utilities for the JSOM
    SharePointClient.AddNameSpace("Utilities");
    SharePointClient.Utilities.Utility = function () {

        var configuration = SharePointClient.Configurations;

        //Get the value of querystring parameter from the current Url
        var queryString = function (param) {
            var params = document.URL.split("?")[1].split("&");
            var strParams = "";
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split("=");
                if (singleParam[0] == param)
                    return decodeURIComponent(singleParam[1]);
            }
        };

        //SharePoint has the object for getting weburl and other properties, which is available in SharePoint pages
        var UrlFromPageContextInfo = function () {
            var url;
            if (typeof _spPageContextInfo != "undefined") {
                url = _spPageContextInfo.webAbsoluteUrl;
            }

            return url;
        };

        //App page has the Host Url
        var hostUrl = function () {
            //Get the URI decoded URLs.
            return queryString("SPHostUrl");
        };

        //App page has the AppWebUrl
        var appWebUrl = function () {
            //Get the URI decoded URLs.
            return queryString("SPAppWebUrl");
        };

        //Construct baseurl for downloading required JS files for working with JSOM
        var baseUrl = function () {
            var scriptbase;
            if (configuration.IsApp) {
                scriptbase = hostUrl() + "/_layouts/15/";
            } else {
                if (typeof _spPageContextInfo != "undefined") {
                    scriptbase = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/"
                }
            }

            return scriptbase;
        };

        //SharePoint REST api service url
        var _api = function () {
            var restApi;
            if (configuration.IsApp) {
                if (configuration.SPUrl == null) {
                    restApi = queryString("SPAppWebUrl");
                } else {
                    restApi = configuration.SPUrl;
                }
            } else {
                if (typeof _spPageContextInfo != "undefined") {
                    restApi = _spPageContextInfo.webAbsoluteUrl;
                }
            }
            return restApi + "/" + SharePointClient.Constants.REST.API + "/";
        };

        //SharePoint Request Digest url
        var digestUrl = function () {
            return _api() + SharePointClient.Constants.REST.REQUEST_DIGEST_ENDPOINT;
        };

        //Modify the url to support cross domain access
        var CrossDomainRequestUrl = function (url) {
            var apiIndex = url.indexOf("_api/") + 5;
            var requestUrl = url.substring(0, apiIndex);
            requestUrl += "SP.AppContextSite(@target)/";
            requestUrl += url.substring(apiIndex, url.length);
            var sphostUrl = hostUrl();
            if (url.indexOf("?") > 0) {
                requestUrl = requestUrl + "@target='" + sphostUrl + "'";
            } else {
                requestUrl = requestUrl + "?@target='" + sphostUrl + "'";
            }

            return requestUrl;
        };

        //Verify whether current page has script which is going to be download
        var ScriptAlreadyLoaded = function (scriptName) {
            scriptName = scriptName.toLowerCase();
            if ($("Scripts[src$='" + scriptName + "']").length > 0) {
                return true;
            } else {
                return false;
            }
        };

        var downloadScript = function (baseUrl, scriptUrls, index, callback) {
            $.getScript(baseUrl + scriptUrls[index], function () {
                if (index + 1 <= scriptUrls.length - 1) {
                    downloadScript(baseUrl, scriptUrls, index + 1, callback);
                } else {
                    if (callback)
                        callback();
                }
            });
        };

        return {
            GetQueryStringParameter: queryString,
            GetHostUrl: hostUrl,
            GetAppWebUrl: appWebUrl,
            IsScriptExistsOnPage: ScriptAlreadyLoaded,
            GetUrlFromPageContextInfo: UrlFromPageContextInfo,
            GetScript: downloadScript,
            JSOM: {
                GetBaseUrl: baseUrl
            },
            REST: {
                GetApiUrl: _api,
                GetRequestDigestUrl: digestUrl,
                GetCrossDomainRequestUrl: CrossDomainRequestUrl,
            }
        };
    };

    //Caml Query utility for creating caml schema from JSON object
    SharePointClient.AddNameSpace("Utilities.CamlQueryUtility");
    SharePointClient.Utilities.CamlQueryUtility = {

        //Convert JSON object to Caml schema
        ConvertToCamlSchema: function (rootElement, jsonObject) {

            if (typeof jsonObject == "string" || typeof jsonObject == "number") {
                return "<" + rootElement + ">" + jsonObject + "</" + rootElement + ">";
            }

            var camlQueryXml = "<" + rootElement;
            var attrCollection = {}, elementCollection = {}, rootNodeValue;
            $.each(jsonObject, function (index, object) {
                if (SharePointClient.Utilities.CamlQueryUtility.IsAttribute(index)) {
                    attrCollection[index] = object;
                } else {
                    if (index.indexOf('_') == 0) {
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
                if (typeof v == "object") {
                    fieldXml += SharePointClient.Utilities.CamlQueryUtility.ConvertToCamlSchema(k, v);
                }
                else {
                    fieldXml += "<" + k + ">" + v + "</" + k + ">";
                }
            });

            if (fieldXml.length > 0) {
                camlQueryXml += fieldXml;
            }

            if (rootNodeValue != null) {
                camlQueryXml += rootNodeValue;
            }

            camlQueryXml += "</" + rootElement + ">";

            return camlQueryXml;
        },

        //Is there any attributes defined in the JSON object
        IsAttribute: function (input) {

            if (typeof input == "object") {
                return false;
            }

            if (input.indexOf('@') == 0) {
                return true;
            } else {
                return false;
            }
        },

        //Get the collection attributes from JSON object
        Attributes: function (JsonObject) {

            if (typeof JsonObject == "string" || typeof jsonObject == "number") {
                if (JsonObject.indexOf('@') == 0) {
                    return JsonObject;
                } else {
                    return "";
                }

            }
            var attributes = "";
            $.each(JsonObject, function (index, object) {

                if (index.indexOf('@') == 0) {
                    attributes += " " + index.replace("@", "") + "='" + object + "'";
                }
            });

            return attributes;
        },
    };

    //Extend camlquery functionalities
    SharePointClient.AddNameSpace("CamlExtension");
    SharePointClient.CamlExtension = {
        JSOM: {
            //Extended caml query methods to support for creating camlQuery viewxml
            CamlQuery: function () {
                $.extend(this, new SP.CamlQuery());
            },
        },
        REST: {
            //custom camlquery class
            CamlQuery: function () {
                var queryStatement = {};
                var viewXml = null;

                //Return viewXml property
                var get_viewXml = function () {
                    return viewXml;
                };

                //This method will return default query for Allitems
                var createAllItemsQuery = function () {
                    viewXml = "<View Scope='RecursiveAll'><Query></Query></View>";
                    return this;
                };

                //This method will return default query for AllFolders
                var createAllFoldersQuery = function () {
                    viewXml = "<View Scope='RecursiveAll'><Query><Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where></Query></View>";
                    return this;
                };

                //This method will update Scope attribute for ViewXml,parameter as scope value
                var ViewAttribute = function (scope) {

                    //Add View
                    queryStatement["View"] = {};

                    //Set scope
                    queryStatement["View"] = {
                        "@Scope": scope
                    }
                    return this;
                };

                //This method will update the query condition for filtering the result set, parameter as caml formatted condition
                var Query = function (queryCondition) {
                    //Set scope
                    queryStatement["Query"] = queryCondition;

                    return this;
                };

                //This method will be used to set the required columns in the result set,parameter as array of field names
                var ViewFields = function (viewfields) {
                    var viewFieldsXml = "";
                    for (var i = 0; i <= viewfields.length - 1; i++) {
                        viewFieldsXml += "<FieldRef "
                        viewFieldsXml += "Name='" + viewfields[i] + "'";
                        viewFieldsXml += "></FieldRef>";
                    }

                    queryStatement["ViewFields"] = viewFieldsXml;

                    return this;
                };

                //This method will be used to set the required columns in the result set, parameter as xml
                var ViewFieldsXml = function (viewfieldsXml) {
                    queryStatement["ViewFields"] = viewfieldsXml;
                    return this;
                };

                //This method will be used Override the QueryThrottle mode for applying the throttle exception for this query or not
                var QueryThrottleMode = function (mode) {
                    queryStatement["QueryOptions"] = {
                        "QueryThrottleMode": mode
                    };

                    return this;
                };

                //This method will override the order by, Use this method only when query has the condition with field as indexed
                var OrderByIndex = function () {
                    queryStatement["OrderBy"] = {
                        "@UseIndexForOrderBy": "TRUE",
                        "@Override": "TRUE"
                    };

                    return this;
                };

                //This method will override the order by with default ID field
                var OrderBy = function () {
                    queryStatement["OrderBy"] = {
                        "@UseIndexForOrderBy": "TRUE",
                        "FieldRef": {
                            "@Name": "ID"
                        }
                    };
                    return this;

                };

                //This method will override the order by with default ID field sortng order by Descending
                var OrderByDesc = function () {
                    queryStatement["OrderBy"] = {
                        "@UseIndexForOrderBy": "TRUE",
                        "FieldRef": {
                            "@Name": "ID",
                            "@Ascending": "FALSE"
                        }
                    };

                    return this;
                };

                //This method will set the row limit
                var RowLimit = function (numberOfRecords) {
                    queryStatement["RowLimit"] = {
                        "@Paged": "TRUE",
                        "_value": numberOfRecords
                    };

                    return this;
                };

                //This method will build the vewXml
                var BuildCamlQuery = function () {

                    if (queryStatement["View"] == null) { return this; }

                    var camlUtility = SharePointClient.Utilities.CamlQueryUtility;

                    //CamlQuery Elements
                    var viewRootElement = ""; //View is the root level element in camlQuery
                    var queryElement = "";//Query element which has query conditions
                    var viewFieldsElement = "";//ViewFields element for limit the fields in result while returning from list
                    var queryOptionsElement = "";//QueryOptions for camlquery
                    var orderByElement = "";//orderBy element for result set
                    var rowLimit = "";//Rowlimit for result set


                    var attributes;
                    //Root element View
                    var view = queryStatement["View"];
                    attributes = camlUtility.Attributes(view);
                    if (attributes.length > 0) {
                        viewRootElement += "<View" + attributes + ">";
                    }
                    else { viewRootElement += "<View>"; }

                    //Query element if exists
                    var query = queryStatement["Query"];
                    if (query != null) {
                        queryElement = camlUtility.ConvertToCamlSchema("Query", query);
                    }

                    //ViewFields element
                    var viewFields = queryStatement["ViewFields"];
                    if (viewFields != null) {
                        viewFieldsElement = camlUtility.ConvertToCamlSchema("ViewFields", viewFields);
                    }

                    //QueryOptions element
                    var queryOptions = queryStatement["QueryOptions"];
                    if (queryOptions != null) {
                        queryOptionsElement = camlUtility.ConvertToCamlSchema("QueryOptions", queryOptions);
                    }

                    //OrderBy element
                    var orderBy = queryStatement["OrderBy"];
                    if (orderBy != null) {
                        orderByElement = camlUtility.ConvertToCamlSchema("OrderBy", orderBy);

                        //Append to Query object
                        if (queryElement.length == 0) {
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
                    var rowlimit = queryStatement["RowLimit"];
                    if (rowlimit != null) {
                        rowLimit = camlUtility.ConvertToCamlSchema("RowLimit", rowlimit);
                    }

                    viewRootElement += queryElement + viewFieldsElement + queryOptionsElement + rowLimit + "</View>";


                    //update viewXml property
                    viewXml = viewRootElement;

                    return this;
                };

                return {
                    SetViewScopeAttribute: ViewAttribute,
                    SetQuery: Query,
                    SetViewFields: ViewFields,
                    SetViewFieldsXml: ViewFieldsXml,
                    OverrideQueryThrottleMode: QueryThrottleMode,
                    OverrideOrderByIndex: OrderByIndex,
                    OverrideOrderBy: OrderBy,
                    OverrideOrderByDesc: OrderByDesc,
                    SetRowLimit: RowLimit,
                    BuildQuery: BuildCamlQuery,
                    GetQueryViewXml: get_viewXml
                };
            },
        }
    };

    //Extend SP.CamlQuery methods
    SharePointClient.CamlExtension.JSOM.CamlQuery.prototype = {
        queryStatement: {},

        //This method will return default query for Allitems
        createAllItemsQuery: function () {
            return SP.CamlQuery.createAllItemsQuery();
        },

        //This method will return default query for AllFolders
        createAllFoldersQuery: function () {
            return SP.CamlQuery.createAllFoldersQuery()
        },

        //This method will update Scope attribute for ViewXml,parameter as scope value
        ViewAttribute: function (scope) {

            //Add View
            this.queryStatement["View"] = {};

            //Set scope
            this.queryStatement["View"] = {
                "@Scope": scope
            }
            return this;
        },

        //This method will update the query condition for filtering the result set, parameter as caml formatted condition
        Query: function (queryCondition) {
            //Set scope
            this.queryStatement["Query"] = queryCondition;

            return this;
        },

        //This method will be used to set the required columns in the result set,parameter as array of field names
        ViewFields: function (viewfields) {

            var viewFieldsXml = "";
            for (var i = 0; i <= viewfields.length - 1; i++) {
                viewFieldsXml += "<FieldRef "
                viewFieldsXml += "Name='" + viewfields[i] + "'";
                viewFieldsXml += "></FieldRef>";
            }

            this.queryStatement["ViewFields"] = viewFieldsXml;

            return this;
        },

        //This method will be used to set the required columns in the result set, parameter as xml
        ViewFieldsXml: function (viewfieldsXml) {

            //var view = this.queryStatement["View"];
            this.queryStatement["ViewFields"] = viewfieldsXml;
            return this;
        },

        //This method will be used Override the QueryThrottle mode for applying the throttle exception for this query or not
        QueryThrottleMode: function (mode) {

            //var view = this.queryStatement["View"];
            this.queryStatement["QueryOptions"] = {
                "QueryThrottleMode": mode
            };

            return this;
        },

        //This method will override the order by, Use this method only when query has the condition with field as indexed
        OrderByIndex: function () {

            //var view = this.queryStatement["View"];
            this.queryStatement["OrderBy"] = {
                "@UseIndexForOrderBy": "TRUE",
                "@Override": "TRUE"
            };

            return this;
        },

        //This method will override the order by with default ID field
        OrderBy: function () {
            //var view = this.queryStatement["View"];
            this.queryStatement["OrderBy"] = {
                "@UseIndexForOrderBy": "TRUE",
                "FieldRef": {
                    "@Name": "ID"
                }
            };
            return this;
        },

        //This method will override the order by with default ID field sortng order by Descending
        OrderByDesc: function () {

            //var view = this.queryStatement["View"];
            this.queryStatement["OrderBy"] = {
                "@UseIndexForOrderBy": "TRUE",
                "FieldRef": {
                    "@Name": "ID",
                    "@Ascending": "FALSE"
                }
            };

            return this;
        },

        //This method will set the row limit
        RowLimit: function (numberOfRecords) {

            //var view = this.queryStatement["View"];
            this.queryStatement["RowLimit"] = numberOfRecords;

            return this;
        },

        //This method will build the vewXml
        BuildQuery: function () {

            var camlUtility = SharePointClient.Utilities.CamlQueryUtility;

            //CamlQuery Elements
            var viewRootElement = ""; //View is the root level element in camlQuery
            var queryElement = "";//Query element which has query conditions
            var viewFieldsElement = "";//ViewFields element for limit the fields in result while returning from list
            var queryOptionsElement = "";//QueryOptions for camlquery
            var orderByElement = "";//orderBy element for result set
            var rowLimit = "";//Rowlimit for result set


            var attributes;
            //Root element View
            var view = this.queryStatement["View"];
            attributes = camlUtility.Attributes(view);
            if (attributes.length > 0) {
                viewRootElement += "<View" + attributes + ">";
            }
            else { viewRootElement += "<View>"; }

            //Query element if exists
            var query = this.queryStatement["Query"];
            if (query != null) {
                queryElement = camlUtility.ConvertToCamlSchema("Query", query);
            }

            //ViewFields element
            var viewFields = this.queryStatement["ViewFields"];
            if (viewFields != null) {
                viewFieldsElement = camlUtility.ConvertToCamlSchema("ViewFields", viewFields);
            }

            //QueryOptions element
            var queryOptions = this.queryStatement["QueryOptions"];
            if (queryOptions != null) {
                queryOptionsElement = camlUtility.ConvertToCamlSchema("QueryOptions", queryOptions);
            }

            //OrderBy element
            var orderBy = this.queryStatement["OrderBy"];
            if (orderBy != null) {
                orderByElement = camlUtility.ConvertToCamlSchema("OrderBy", orderBy);

                //Append to Query object
                if (queryElement.length == 0) {
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
            var rowlimit = this.queryStatement["RowLimit"];
            if (rowlimit != null) {
                rowLimit = camlUtility.ConvertToCamlSchema("RowLimit", rowlimit);
            }

            viewRootElement += queryElement + viewFieldsElement + queryOptionsElement + rowLimit + "</View>";

            this.set_viewXml(viewRootElement);

            return this;
        },
    };

    //Services  for JSOM and REST
    SharePointClient.AddNameSpace("Services");
    SharePointClient.Services = {
        JSOM: {
            //Initialize the JSOM with loading required JS files
            Initialize: function (callback) {
                var utility = new SharePointClient.Utilities.Utility();
                var config = SharePointClient.Configurations;
                var baseUrl = utility.JSOM.GetBaseUrl();

                var jsFiles = ["SP.Runtime.js", "SP.js"];

                if (config.IsCrossDomainRequest) {
                    //load SP.RequestExecutor if not mentioned in js Array
                    var jsExists = jQuery.grep(jsFiles, function (n, i) {
                        return (n !== "" && n != null && n == "SP.RequestExecutor.js");
                    });

                    if (jsExists.length == 0) {
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
                if (loadJsFiles.length == 0) {
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
            //Context class used to get the correct context from the configuration
            Context: function () {
                var utility = new SharePointClient.Utilities.Utility();

                //Properties to hold current client context and web
                var ClientContext = null;
                var Web = null;

                //This method is used while working on SharePoint pages, beacuse sharePoint default provide context
                var Current = function () {
                    if (typeof SP.ClientContext != "undefined") {
                        ClientContext = SP.ClientContext.get_current();
                        Web = ClientContext.get_web();
                    }
                };

                //Create context by SharePoint Web url
                var ContextByUrl = function (url) {
                    if (typeof SP.ClientContext != "undefined") {
                        ClientContext = new SP.ClientContext(url);
                        Web = ClientContext.get_web();
                    }
                };

                //if the Context required for cross domain request, for example access host web from SharePoint hosted app
                var CrossDomainContext = function () {

                    ContextByUrl(utility.GetAppWebUrl());

                    var factory = new SP.ProxyWebRequestExecutorFactory(utility.GetAppWebUrl());
                    ClientContext.set_webRequestExecutorFactory(factory);

                    var AppContextSite = new SP.AppContextSite(ClientContext, utility.GetHostUrl());
                    Web = AppContextSite.get_web();
                };

                //Intialize the context for SharePoint
                var Init = function () {
                    //if confiuration property App is set TRUE , set HostUrl and AppWebUrl
                    var configuration = SharePointClient.Configurations;
                    if (configuration.IsApp) {
                        configuration.SPHostUrl = utility.GetHostUrl();
                        configuration.SPAppWebUrl = utility.GetAppWebUrl();

                        //Check if the request would be cross domain call or call for current site
                        if (configuration.IsCrossDomainRequest) {
                            //Create cross domain client context
                            CrossDomainContext();
                        } else if (configuration.SPUrl == null) {
                            //Create context for App Web
                            ContextByUrl(utility.GetAppWebUrl());
                        } else {
                            //Create context SPUrl property
                            ContextByUrl(configuration.SPUrl);
                        }
                    } else {
                        //Create context for sharepoint site where required Js files are available
                        Current();
                    }
                };

                return {
                    get_clientContext: function () {
                        if (ClientContext == null || ClientContext == "undefined") {
                            //Initialize the client context
                            Init();
                        }
                        return ClientContext;
                    },
                    get_web: function () {
                        if (ClientContext == null || ClientContext == "undefined") {
                            //Initialize the client context
                            Init();
                        }
                        return Web;
                    }
                };
            },
            //JSOM list service
            ListServices: function () {
                //Get the lists collection
                var Lists = function (context, callback) {
                    var clientContext = context.get_clientContext();
                    var web = context.get_web();
                    var lists = web.get_lists();
                    clientContext.load(lists, 'Include(Title, Id)');

                    clientContext.executeQueryAsync(
                        function (sender, args) {
                            callback(lists);
                        },
                        function (sender, args) {
                            SharePointClient.Logger.LogJSOMException(args);
                        });
                };

                //Get the list by title
                var ListByTitle = function (context, listTitle, callback) {
                    var clientContext = context.get_clientContext();
                    var web = context.get_web();

                    var lists = web.get_lists();
                    list = lists.getByTitle(listTitle);

                    clientContext.load(list);

                    clientContext.executeQueryAsync(
                        function () {
                            callback(list);
                        },
                        function (sender, args) {
                            SharePointClient.Logger.LogRESTException(args);
                        });
                };

                //Get the items by listname
                var ListItemsByListName = function (context, listTitle, camlQuery, callback) {

                    var position = new SP.ListItemCollectionPosition();
                    position.set_pagingInfo("");
                    camlQuery.set_listItemCollectionPosition(position);

                    var itemsCollection = [];

                    delegateRequest(context, camlQuery, listTitle, function (d) {

                        $.each(d, function (index, value) {
                            if (Object.prototype.toString.call(value) === '[object Array]') {
                                $.each(value, function (k, v) {
                                    itemsCollection.push(v);
                                });
                            }

                        });

                        if (d.get_listItemCollectionPosition() == null) {
                            return callback(itemsCollection);
                        }
                    });
                };

                //Get the list items batch by batch 
                var ListItemsByBatch = function (context, listTitle, camlQuery, callback) {

                    var position = new SP.ListItemCollectionPosition();
                    position.set_pagingInfo("");
                    camlQuery.set_listItemCollectionPosition(position);

                    delegateRequest(context, camlQuery, listTitle, callback);
                };

                //This is the delegate request called recursively when more items to be fetched in batch
                var delegateRequest = function (context, camlQuery, listTitle, callback) {
                    var clientContext = context.get_clientContext();
                    var web = context.get_web();
                    var listItems = web.get_lists().getByTitle(listTitle).getItems(camlQuery);
                    clientContext.load(listItems);

                    clientContext.executeQueryAsync(
                            function () {
                                callback(listItems);

                                //Iterate if more items needs to be fetched
                                if (listItems.get_listItemCollectionPosition()) {
                                    camlQuery.set_listItemCollectionPosition(listItems.get_listItemCollectionPosition());

                                    delegateRequest(context, camlQuery, listTitle, callback);

                                } else {
                                    return;
                                }

                            },
                            function (sender, args) {
                                SharePointClient.Logger.LogJSOMException(args);
                            });
                };

                return {
                    GetLists: Lists,
                    GetListByTitle: ListByTitle,
                    GetLargeListItemsByListTitle: ListItemsByListName,
                    GetLargeListItemsByBatch: ListItemsByBatch
                };
            }
        },
        REST: {
            RESTService: function () {
                $.support.cors = true;
                //REST call without Request digest
                var Ajax = {
                    Call: function (url, type, data, responseType, requireStringify) {
                        var call = $.ajax({
                            url: url,
                            type: type,
                            contentType: responseType,
                            data: stringify(data, requireStringify),
                            headers: GetHeaders(responseType)
                        });
                        return (call.then(success, error));
                    }
                };
                //REST call with Request digest value
                var AjaxWithFormDigest = {
                    Call: function (url, type, data, digestValue, responseType, requireStringify) {
                        var call = $.ajax({
                            url: url,
                            type: type,
                            data: stringify(data, requireStringify),
                            headers: GetHeadersWithDigest(responseType, digestValue)
                        });
                        return (call.then(success, error));
                    }
                };
                //REST call for cross domain request
                var CrossDomainRequest = {
                    Call: function (url, type, data, responseType, requireStringify) {
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
                };
                //Convert JSON format of request data
                var stringify = function (data, requireStringify) {
                    if (requireStringify)
                        return JSON.stringify(data);
                    else
                        return data;
                };
                //Construct request headers
                var GetHeaders = function (responseType) {
                    var headers = {
                        "Accept": responseType
                    }

                    if (SharePointClient.Configurations.AccessToken != null) {
                        headers["Authorization"] = "Bearer " + SharePointClient.Configurations.AccessToken;
                    }

                    return headers;
                };
                //Construct request headers with requet digest value
                var GetHeadersWithDigest = function (responseType, digestValue) {
                    var headers = {
                        "Accept": responseType,
                        "X-RequestDigest": digestValue,
                        "Content-Type": SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON,
                    }

                    if (SharePointClient.Configurations.AccessToken != null) {
                        headers["Authorization"] = "Bearer " + SharePointClient.Configurations.AccessToken;
                    }

                    return headers;
                };
                //Event handler for Success call
                var success = function (data) {
                    return data;
                };
                //Event handler for Error call
                var error = function (xhr, errorType, exception) {
                    SharePointClient.Logger.LogRESTException("Exception : " + xhr.responseText);
                };

                return {
                    Request: Ajax.Call,
                    RequestWithDigest: AjaxWithFormDigest.Call,
                    RequestCrossDomain: CrossDomainRequest.Call
                };
            },
            //SharePoint REST ListServices
            ListServices: function () {

                var utility = new SharePointClient.Utilities.Utility();
                var constants = SharePointClient.Constants.REST;
                var Service = SharePointClient.Services.REST.RESTService();

                //Get the lists collection
                var Lists = function (callBack, responseType) {
                    var RequestUrl = utility.REST.GetApiUrl() + constants.WEB + "/" + constants.LISTS;
                    Service.Request(RequestUrl, constants.HTTP.GET, null, responseType, false).then(
                        function (data) {
                            callBack(data);
                        },
                        function (exception) {
                        });
                };

                //Get the list by title
                var ListByTitle = function (listTitle, responseType) {
                    var RequestUrl = utility.REST.GetApiUrl() + constants.WEB + "/" + constants.LISTS + "/getbytitle('" + listTitle + "')";
                    Service.Request(RequestUrl, constants.HTTP.GET, null, responseType, false).then(
                        function (data) {

                            alert(data.d.Title);
                        },
                        function (exception) {
                        });
                };

                //Get the items by listname in batch by batch
                var ListItemsByListName = function (listTitle, camlQuery, responseType, callBack) {

                    var RequestUrl = utility.REST.GetApiUrl() + constants.WEB + "/" + constants.LISTS + "/getbytitle('" + listTitle + "')/RenderListData";

                    var requestData = {
                        "viewXml": camlQuery.GetQueryViewXml()
                    };

                    if (SharePointClient.Configurations.IsCrossDomainRequest) {
                        $.getScript(utility.GetQueryStringParameter("SPHostUrl") + "/_layouts/15/SP.RequestExecutor.js", function () {
                            ExecuteCrossDomainRequest(RequestUrl, constants.HTTP.POST, requestData, responseType, callBack);
                        });
                    } else {
                        Service.Request(utility.REST.GetRequestDigestUrl(), constants.HTTP.POST, null, constants.HTTP.DATA_TYPE.JSON, false).then(function (data) {
                            var NewDigest = data.d.GetContextWebInformation.FormDigestValue;
                            delegateRequest(RequestUrl, constants.HTTP.POST, requestData, NewDigest, responseType, callBack);
                        });
                    }
                };

                //This is the delegate request for cross domain which is called recursively when more items to be fetched in batch
                var ExecuteCrossDomainRequest = function (requestUrl, requestType, requestData, responseType, callback) {

                    Service.RequestCrossDomain(requestUrl, requestType, requestData, responseType, true).then(
                        function (data) {
                            callback(data);

                            //Verify if more items are present or not
                            var convertResult;
                            if (responseType == SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON) {
                                convertResult = $.parseJSON($.parseJSON(data).d.RenderListData);
                            } else {
                                convertResult = $.parseJSON($($.parseXML(data).lastChild).text());
                            }
                            if (typeof (convertResult.NextHref) !== "undefined" && convertResult.NextHref != "") {
                                //update the Request Url for next batch
                                var Url = requestUrl;
                                var queryParam = requestUrl.split("?");
                                if (queryParam.length == 2) {
                                    Url = queryParam[0] + convertResult.NextHref
                                } else {
                                    Url = requestUrl + convertResult.NextHref;
                                }

                                ExecuteCrossDomainRequest(Url, SharePointClient.Constants.REST.HTTP.POST, requestData, responseType, callback);
                            }

                            return convertResult;
                        });
                };

                //This is the delegate request called recursively when more items to be fetched in batch
                var delegateRequest = function (requestUrl, requestType, requestData, digestValue, responseType, callback) {

                    Service.RequestWithDigest(requestUrl, requestType, requestData, digestValue, responseType, true).then(
                        function (data) {
                            callback(data);

                            //Verify if more items are present or not
                            var convertResult;
                            if (responseType == SharePointClient.Constants.REST.HTTP.DATA_TYPE.JSON) {
                                convertResult = JSON.parse(data.d.RenderListData)
                            } else {
                                convertResult = $.parseJSON(data.lastChild.lastChild.data);
                            }
                            if (typeof (convertResult.NextHref) !== "undefined" && convertResult.NextHref != "") {
                                //update the Request Url for next batch
                                var Url = requestUrl;
                                var queryParam = requestUrl.split("?");
                                if (queryParam.length == 2) {
                                    Url = queryParam[0] + convertResult.NextHref
                                } else {
                                    Url = requestUrl + convertResult.NextHref;
                                }

                                delegateRequest(Url, SharePointClient.Constants.REST.HTTP.POST, requestData, digestValue, responseType, callback);
                            }

                            return convertResult;
                        });
                };

                return {
                    GetLists: Lists,
                    GetListByTitle: ListByTitle,
                    GetListItemsByListName: ListItemsByListName
                };
            }
        }
    };
})();