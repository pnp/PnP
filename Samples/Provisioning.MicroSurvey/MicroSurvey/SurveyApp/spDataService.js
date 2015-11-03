(function () {

    'use strict';

    var module = angular
    .module('microSurvey')
    .factory('spDataService', ['$http', '$location', '$q',

    function spDataService($http, $location, $q) {

        // *** Utility Functions ****

        // getSiteUrl() - Obtains the target site URL
        function getSiteUrl() {
            var urlParts = $location.absUrl().toLowerCase().split('/');
            var result = urlParts[0] + "/";
            for (var i = 2; i < urlParts.length; i++) {
                if (urlParts[i] != 'surveyapp' &&
                    urlParts[i] != 'pages' && urlParts[i] !== 'sitepages' &&
                    urlParts[i] !== 'siteassets' && urlParts[i].indexOf('.aspx') < 0) {
                    result += '/' + urlParts[i];
                } else {
                    break;
                }
            }
            return result;
        };
        
        // getGetConfig() - Gets the $http config object for doing a GetAbsoluteUrl
        function getGetConfig() {
            return {
                headers: {
                    accept: "application/json;odata=verbose"
                }};
        }

        // getPostConfig() - Gets the $http config object for doing a POST
        function getPostConfig() {
            return {
                    'content-type': "application/json;odata=verbose",
                    headers: {
                    'accept': "application/json;odata=verbose",
                    'X-RequestDigest': document.getElementById ('__REQUESTDIGEST').value,
                    'X-Http-Method': 'POST',
                    'content-length': 0,
                    'content-type': "application/json;odata=verbose",
                    'If-Match': '*'
                }
            };
        }

        // getMergeConfig() - Gets the $http config object for doing a POST
        function getMergeConfig() {
            return {
                'content-type': "application/json;odata=verbose",
                headers: {
                    'accept': "application/json;odata=verbose",
                    'X-RequestDigest': document.getElementById('__REQUESTDIGEST').value,
                    'X-Http-Method': 'MERGE',
                    'content-length': 0,
                    'content-type': "application/json;odata=verbose",
                    'If-Match': '*'
                }
            };
        }

        // *** Public properties ***

        // Field type constants - see https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.fieldtype.aspx
        var fieldTypes = {
            Integer: 1,
            Text: 2,
            Note: 3,
            DateTime: 4,
            Counter: 5,
            Boolean: 8,
            Number: 9
        };

        // *** ensureList - Provisioning function that ensures that a list has been created ***

        // ensureList() Part 1: Check to see if the list already exists
        function ensureList(name, description) {

            var siteUrl = getSiteUrl();
            var deferred = $q.defer();

            $http.get(siteUrl + "/_api/web/lists/GetByTitle('" + name + "')", getGetConfig())
            .then(function (response) {
                // If here we got a successful response
                if (response.data.d.Title == name) {
                    deferred.resolve("Found " + name + " list");
                } else {
                    // If here something bad happened
                    deferred.reject("Internal inconsistency: List exists with wrong name")
                }
            })
            .catch(function (response) {
                if (response.status == 404) {
                    // If here the list needs to be created
                    ensureList2(name, description, siteUrl, deferred)
                } else {
                    // If here something bad happened
                    deferred.reject("Error in ensureList(): " + response.status);
                }
            });

            return deferred.promise;
        }

        // ensureList() Part 2: (Called if list doesn't exist) Create the list.
        function ensureList2(name, description, siteUrl, deferred) {
            $http.post(siteUrl + "/_api/web/lists",
            {
                // POST data
                '__metadata': { 'type': 'SP.List' },
                'AllowContentTypes': false,
                'BaseTemplate': 100,
                'ContentTypesEnabled': false,
                'Description': description,
                'Title': name
            }, getPostConfig())
            .then(function (response) {
                deferred.resolve("Created " + name + " list");
            })
            .catch(function (response) {
                deferred.reject('Error ' + response.status + ': ' + response.data.error.message.value);
            });
        }

        // *** ensureColumn - Provisioning function to ensure a column has been created on a list ***

        // ensureColumn Part 1: Check to see if the column already exists
        function ensureColumn(listName, columnName, fieldType) {

            var siteUrl = getSiteUrl();
            var deferred = $q.defer();
            var fieldTitle = "";
            var found = false;

            $http.get(siteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/Fields", getGetConfig())
                .then(function (response) {
                    
                    var fieldList = response.data.d.results;
                    for (var field in fieldList) {
                        fieldTitle = fieldList[field].Title;
                        if (fieldTitle === columnName) {
                            found = true;
                            break;
                        }
                    }
                    if (found) {
                        deferred.resolve("Found " + columnName + " column in " + listName + " list");
                    } else {
                        ensureColumn2(listName, columnName, fieldType, siteUrl, deferred);
                    }

                })
                .catch(function (response) {
                    deferred.reject("Error: Request to get field names of " + listName + " failed.");
                });

            return deferred.promise;
        }

        // ensureColumn() Part 2: If the column didn't exist, add it
        function ensureColumn2(listName, columnName, fieldType, siteUrl, deferred) {
            $http.post(siteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/Fields",
                {
                    // POST data
                    '__metadata': {
                        'type': 'SP.Field'
                    },
                    'Title': columnName,
                    'FieldTypeKind': fieldType,
                    'Required': 'true',
                    'EnforceUniqueValues': 'false',
                    'StaticName': columnName
                }, getPostConfig())
            .then(function (response) {
                deferred.resolve("Created " + columnName + " in list " + listName);
            })
            .catch(function (response) {
                deferred.reject('Error ' + response.status + ': ' + response.data.error.message.value)
            });
        }

        // *** ensureColumnInView - Provisioning function to ensure a column is in a list view ***
        function ensureColumnInView(listName, columnName, viewName)
        {

            var siteUrl = getSiteUrl();
            var deferred = $q.defer();
            var found = false;

            $http.get(siteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/Views/GetByTitle('All%20Items')/ViewFields", getGetConfig())
            .then(function (response) {
                var fieldList = response.data.d.Items.results;
                for (var field in fieldList) {
                    if (fieldList[field] === columnName) {
                        found = true;
                        break;
                    }
                }
                if (found) {
                    deferred.resolve("Found " + columnName + " in the " + viewName + " view in list " + listName);
                } else {
                    ensureColumnInView2(listName, columnName, viewName, siteUrl, deferred);
                }
            })
            .catch(function (response) {
                deferred.reject("Error: Request to get view field names for " + listName + " in " + viewName + " failed.");
            });

            return deferred.promise;
        }

        // ensureColumn() Part 2: If the column didn't exist, add it
        function ensureColumnInView2(listName, columnName, viewName, siteUrl, deferred) {
            $http.post(siteUrl + "/_api/web/lists/GetByTitle('" + listName +
                "')/Views/GetByTitle('" + viewName + "')/ViewFields/addViewField('" + columnName + "')",
                {
                    // POST data is empty for this request
                }, getPostConfig())
            .then(function (response) {
                deferred.resolve("Added column " + columnName + " to view " + viewName + " in list " + listName);
            })
            .catch(function (response) {
                deferred.reject('Error ' + response.status + ': ' + response.data.error.message.value)
            });
        }

        // *** readMostRecentListItem: Data Access function to read the most recent list item ***
        function readMostRecentListItem(listName)
        {
            var siteUrl = getSiteUrl();
            var deferred = $q.defer();

            $http.get(siteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/items?$orderby=Modified+desc&$top=1", getGetConfig())
            .then(function (response) {
                deferred.resolve(response.data.d.results[0]);
            })
            .catch(function (response) {
                if (response.status === 404) {
                    // If here the list needs to be created
                    deferred.reject("Item not found in list " + listName);
                } else {
                    // If here something bad happened
                    deferred.reject("Error " + response.status + "reading item from list " + listName);
                }
            });

            return deferred.promise;

        }
        
        // *** readListItemById: Data Access function to read a list item based on its title ***
        function readListItemById(listName, itemId) {
            var siteUrl = getSiteUrl();
            var deferred = $q.defer();

            $http.get(siteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/items('" + itemId + "')", getGetConfig())
            .then(function (response) {
                deferred.resolve(response.data.d);
            })
            .catch(function (response) {
                deferred.reject(response.status);
            });

            return deferred.promise;
        }

        // *** readListItemByTitle: Data Access function to read a list item based on its title ***
        function readListItemByTitle(listName, title) {
            var siteUrl = getSiteUrl();
            var deferred = $q.defer();

            $http.get(siteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/items/?$filter=Title eq '" + title + "'", getGetConfig())
            .then(function (response) {
                if (response.data.d.results.length <= 0) {
                    deferred.reject(404);
                } else {
                    deferred.resolve(response.data.d.results[0]);
                }
            })
            .catch(function (response) {
                deferred.reject(response.status);
            });

            return deferred.promise;
        }

        // *** addListItem: Data Access function to add a list item ***
        function addListItem(listName, title, fieldValues) {

            var siteUrl = getSiteUrl();
            var deferred = $q.defer();

            // Build the POST data including the field values passed
            var postData = {
                __metadata: {
                    type: "SP.Data." + listName + "ListItem"
                },
                Title: title
            };
            for (var val in fieldValues) {
                postData[val] = fieldValues[val];
            }

            // Post the data
            $http.post(getSiteUrl() + "/_api/web/lists/getbytitle('" + listName + "')/items",
                postData, getPostConfig())
            .then(function (response) {
                deferred.resolve(response.data.d);
            })
            .catch(function (response) {
                deferred.reject(response.status);
            });

            return deferred.promise;
        };

        // *** updateListItem: Data Access function to update a list item ***
        function updateListItemById(listName, itemId, title, fieldValues) {
            var siteUrl = getSiteUrl();
            var deferred = $q.defer();

            // Build the POST data including the field values passed
            var postData = {
                __metadata: {
                    type: "SP.Data." + listName + "ListItem"
                },
                Title: title
            };
            for (var val in fieldValues) {
                postData[val] = fieldValues[val];
            }

            // Post the data
            $http.post(getSiteUrl() + "/_api/web/lists/GetByTitle('" + listName + "')/items(" + itemId + ")",
                postData, getMergeConfig())
            .then(function (response) {
                deferred.resolve(response.data.d);
            })
            .catch(function (response) {
                deferred.reject(response.status);
            });

            return deferred.promise;
        };

        // *** setContentTypeForms: Sets the forms for a list content type
        function setContentTypeForms(listName, contentTypeName, newFormUrl, displayFormUrl, editFormUrl) {

            var ctx = {};
            ctx.listName = listName;
            ctx.contentTypeName = contentTypeName;
            ctx.newFormUrl = newFormUrl;
            ctx.displayFormUrl = displayFormUrl;
            ctx.editFormUrl = editFormUrl;

            ctx.clientContext = SP.ClientContext.get_current();
            ctx.deferred = $q.defer();

            // Get the web url
            ctx.web = ctx.clientContext.get_web();
            ctx.clientContext.load(ctx.web);

            // Get the list content types
            ctx.targetList =
                ctx.clientContext.get_web().get_lists().getByTitle(listName);
            ctx.contentTypeCollection =
                ctx.targetList.get_contentTypes();
            ctx.clientContext.load(ctx.contentTypeCollection);

            ctx.clientContext.executeQueryAsync(
                Function.createDelegate(this, function () {
                    setContentTypeForms2(ctx);
                }),
                Function.createDelegate(this, function (sender, args) {
                    ctx.deferred.reject(args.get_message());
                })
            );

            return ctx.deferred.promise;
        }

        function setContentTypeForms2(ctx) {

            // Get the URL path for this SPWeb (i.e. server relative URL)

            // Loop through the list content types
            var ctenum = ctx.contentTypeCollection.getEnumerator();
            while (ctenum.moveNext()) {

                // Get the content type being enumerated and see if it's the one we want
                var ct = ctenum.get_current();

                if (ct.get_name().toLowerCase() === ctx.contentTypeName.toLowerCase()) {

                    // Yes this is the content type to set
                    // If a new form is specified, set it
                    if (ctx.newFormUrl !== null) {
                        ct.set_newFormUrl(ctx.newFormUrl);
                        ct.update();
                    }

                    // If a display form is specified, set it
                    if (ctx.displayFormUrl !== null) {
                        ct.set_displayFormUrl(ctx.displayFormUrl);
                        ct.update();
                    }

                    // If an edit form is specified, set it
                    if (ctx.editFormUrl !== null) {
                        ct.set_editFormUrl(ctx.editFormUrl);
                        ct.update();
                    }
                }
            }

            ctx.clientContext.executeQueryAsync(
                Function.createDelegate(this, function () {
                    ctx.deferred.resolve("Set display and edit forms on " + ctx.listName + "list");
                }),
                Function.createDelegate(this, function (sender, args) {
                    ctx.deferred.reject(args.get_message());
                })
                )
        }
         

        // *** Return public members ***
        return {
            getSiteUrl: getSiteUrl,
            ensureList: ensureList,
            ensureColumn: ensureColumn,
            ensureColumnInView: ensureColumnInView,
            readMostRecentListItem: readMostRecentListItem,
            readListItemById: readListItemById,
            readListItemByTitle: readListItemByTitle,
            addListItem: addListItem,
            updateListItemById: updateListItemById,
            setContentTypeForms: setContentTypeForms,
            fieldTypes: fieldTypes
        };
    }]);
}());
