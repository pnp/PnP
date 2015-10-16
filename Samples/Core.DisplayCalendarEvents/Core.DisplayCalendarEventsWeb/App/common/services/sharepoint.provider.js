(function (angular) {
    "use strict";

    angular
        .module('sharepoint.provider', [])
        .provider('Sharepoint', Sharepoint);

    function Sharepoint() {

        var siteCollectionUrl = "";
        var appWebUrl = "";

        function setAppWebUrl(s) {
            appWebUrl = s;
        }

        function setSiteCollectionUrl(s) {
            siteCollectionUrl = s;
        }

        return {
            setSiteCollectionUrl: setSiteCollectionUrl,
            setAppWebUrl: setAppWebUrl,

            $get: ['$http', '$q', function ($http, $q) {

                var proxyEndpointOptions = {
                    // add SPHostUrl to force AppOnly request
                    url: 'api/sharepoint', //?SPHostUrl='+ encodeURIComponent(siteCollectionUrl),
                    method: 'POST',
                    params: {},
                    data: {}
                };

                function getFormDigestValue() {
                    var apiContextRestUrl = siteCollectionUrl + "/_api/contextinfo";

                    var requestOptions = {
                        url: apiContextRestUrl,
                        method: 'POST',
                        headers: {
                            Accept: 'application/json',
                            'Content-Type': 'application/json'
                        }
                    };

                    return requestHostWebData(requestOptions)
                        .then(function (response) {
                            return response.FormDigestValue;
                        })
                    ;
                }

                function createListItem(listTitle, listItem) {
                    var restUrl = siteCollectionUrl + "/_api/web/lists/getbytitle('" + encodeURIComponent(listTitle) + "')/items";
                    var requestOptions = {
                        url: restUrl,
                        method: 'POST',
                        headers: {
                            Accept: 'application/json;odata=verbose',
                            "Content-Type": 'application/json',
                            "X-RequestDigest": ""
                        },
                        data: listItem
                    };

                    return getFormDigestValue()
                        .then(function (formDigestValue) {
                            requestOptions.headers['X-RequestDigest'] = formDigestValue;
                            return requestHostWebData(requestOptions);
                        })
                        .then(function (response) {
                            return response.d || response;
                        })
                    ;
                }

                function getListItem(listTitle, listItemId) {
                    var restUrl = siteCollectionUrl + "/_api/web/lists/getbytitle('" + encodeURIComponent(listTitle) + "')/items(" + listItemId + ")";
                    return getByUrl(restUrl);
                }

                function getByUrl(siteRelativeUrl) {
                    var restUrl = siteCollectionUrl + siteRelativeUrl;
                    var requestOptions = {
                        url: restUrl,
                        method: 'GET',
                        headers: {
                            Accept: 'application/json'
                        }
                    };

                    return requestHostWebData(requestOptions)
                        .then(function (response) {
                            return response.d || response;
                        })
                    ;
                }

                function getHostWebUrl() {
                    return siteCollectionUrl;
                }

                function updateListItem(listTitle, listItemId, listItemEtag, listItem) {
                    var restUrl = siteCollectionUrl + "/_api/web/lists/getbytitle('" + encodeURIComponent(listTitle) + "')/items(" + listItemId + ")";
                    var requestOptions = {
                        url: restUrl,
                        method: 'POST',
                        headers: {
                            Accept: 'application/json;odata=verbose',
                            "Content-Type": 'application/json',
                            "X-RequestDigest": "",
                            "X-HTTP-Method": "MERGE",
                            "IF-MATCH": listItemEtag
                        },
                        data: listItem
                    };

                    return getFormDigestValue()
                        .then(function (formDigestValue) {
                            requestOptions.headers['X-RequestDigest'] = formDigestValue;
                            return requestHostWebData(requestOptions);
                        }.bind(this))
                        .then(function (response) {
                            return getListItem(listTitle, listItemId);
                        })
                    ;
                }

                function deleteListItem(listTitle, listItemId, listItemEtag) {
                    var restUrl = siteCollectionUrl + "/_api/web/lists/getbytitle('" + encodeURIComponent(listTitle) + "')/items(" + listItemId + ")";
                    var requestOptions = {
                        url: restUrl,
                        method: 'DELETE',
                        headers: {
                            "Accept": 'application/json;odata=verbose',
                            "Content-Type": 'application/json',
                            "X-RequestDigest": "",
                            //"X-HTTP-Method": "DELETE",
                            "IF-MATCH": listItemEtag
                        }
                    };

                    return getFormDigestValue()
                        .then(function (formDigestValue) {
                            requestOptions.headers['X-RequestDigest'] = formDigestValue;
                            return requestHostWebData(requestOptions);
                        }.bind(this))
                    ;
                }

                // Helpers

                var requestHostWebData = R.compose(sendRequest, transformRequest);

                function transformRequest(request) {
                    var transformedRequest = $.extend(true, {}, proxyEndpointOptions);
                    transformedRequest.data = request;
                    return transformedRequest;
                }

                function sendRequest(request) {
                    return $http(request)
                        .then(getData, reformatError)
                    ;
                }

                function getData(o) {
                    return o.data;
                }

                function reformatError(e) {
                    var errorObject = {
                        friendlyMessage: "",
                        status: e.status,
                        statusText: e.statusText,
                        headers: e.headers(),
                        data: e.data
                    };

                    return $q.reject(errorObject);
                }


                return {
                    createListItem: createListItem,
                    getHostWebUrl: getHostWebUrl,
                    getListItem: getListItem,
                    getByUrl: getByUrl,
                    updateListItem: updateListItem,
                    deleteListItem: deleteListItem
                };
            }]
        }
    }

})(angular);
