(function () {
    'use strict';

    angular.module('office365app')
        .service('dataService', ['$q', '$http', 'graphBetaUrl', 'graphUrl', 'sharePointUrl', dataService]);

    function dataService($q, $http, graphBetaUrl, graphUrl, sharePointUrl) {

        return {
            searchForContacts: searchForContacts,
            getUserDetails: getUserDetails,
            getProfilesUrls: getProfilesUrls
        };

        /** *********************************************************** */
        
        function getValueFromResults(key, results) {
            var value = '';

            if (results !== null &&
                results.length > 0 &&
                key !== null) {
                for (var i = 0; i < results.length; i++) {
                    var resultItem = results[i];

                    if (resultItem.Key === key) {
                        value = resultItem.Value;
                        break;
                    }
                }
            }

            return value;
        }

        function searchForContacts(searchQuery, url) {
            var deferred = $q.defer();

            $http({
                url: url || graphBetaUrl + '/me/people?$search=' + encodeURIComponent(searchQuery),
                headers: {
                    'Accept': 'application/json;odata.metadata=full'
                }
            }).success(function(data) {
                var peopleInfo = {
                    nextLink: '',
                    people: []
                };
                
                peopleInfo.nextLink = data['@odata.nextLink'];
                
                data.value.forEach(function(p) {
                    peopleInfo.people.push({
                        name: p.displayName,
                        title: p.title,
                        emailAddresses: p.emailAddresses
                    });
                });
                
                deferred.resolve(peopleInfo);
            }).error(function (err) {
                deferred.reject(err);
            });

            return deferred.promise;
        }
        
        function getUserDetails(userEmail) {
            var deferred = $q.defer();

            $http({
                url: graphUrl + '/users?$filter=mail eq \'' + encodeURIComponent(userEmail) + '\'&$select=businessPhones,id,mail',
                headers: {
                    'Accept': 'application/json;odata.metadata=none'
                }
            }).success(function(data) {
                var userInfo = {
                    businessPhone: null,
                    photoUrl: null,
                    email: null,
                    profileUrl: null
                };
                
                if (data.value.length > 0) {
                    userInfo.businessPhones = data.value[0].businessPhones;
                    userInfo.email = data.value[0].mail;
                    
                    $http({
                        url: graphUrl + '/users/' + data.value[0].id + '/photo/$value',
                        responseType: 'blob'
                    }).success(function(image) {
                        var url = window.URL || window.webkitURL;
                        userInfo.photoUrl = url.createObjectURL(image);
                        
                        deferred.resolve(userInfo);
                    }).error(function (err) {
                        deferred.reject(err);
                    });
                }
                else {
                    deferred.resolve(null);
                }
            }).error(function (err) {
                deferred.reject(err);
            });

            return deferred.promise;
        }
        
        function getRequestDigest() {
            var deferred = $q.defer();
            
            $http({
                url: sharePointUrl + '/_api/contextinfo',
                method: 'POST',
                headers: {
                    'Accept': 'application/json;odata=nometadata'
                }
            }).success(function(data) {
                deferred.resolve(data.FormDigestValue);
            }).error(function(err) {
                deferred.reject(err);
            })
            
            return deferred.promise;
        }
        
        function getProfilesUrls(contacts) {
            var deferred = $q.defer();
            
            getRequestDigest().then(function(requestDigest) {
                var searchQueryParts = [];
                for (var i = 0; i < contacts.length; i++) {
                    var contact = contacts[i];
                    for (var j = 0; j < contact.emailAddresses.length; j++) {
                        searchQueryParts.push('workemail:' + contact.emailAddresses[j].address);
                    }
                }
                
                var postData = JSON.stringify({
                    'request': {
                        '__metadata': {
                            'type': 'Microsoft.Office.Server.Search.REST.SearchRequest'
                        },
                        'Querytext': searchQueryParts.join(' OR '),
                        'SelectProperties': {
                            'results': ['WorkEmail', 'Path']
                        },
                        'RowLimit': '500',
                        'StartRow': '0',
                        'SourceId': 'b09a7990-05ea-4af9-81ef-edfab16c4e31'
                    }
                });
                
                $http({
                    url: sharePointUrl + '/_api/search/postquery',
                    method: 'POST',
                    data: postData,
                    headers: {
                        'Accept': 'application/json;odata=nometadata',
                        'Content-Type': 'application/json;odata=verbose',
                        'X-RequestDigest': requestDigest
                    }
                }).success(function(data) {
                    if (data.PrimaryQueryResult &&
                        data.PrimaryQueryResult.RelevantResults &&
                        data.PrimaryQueryResult.RelevantResults.RowCount > 0) {
                        var profilesUrls = [];
                        for (var i = 0; i < data.PrimaryQueryResult.RelevantResults.Table.Rows.length; i++) {
                            var cells = data.PrimaryQueryResult.RelevantResults.Table.Rows[i].Cells;
                            profilesUrls.push({
                                email: getValueFromResults('WorkEmail', cells),
                                profileUrl: getValueFromResults('Path', cells)
                            });
                        }
                        
                        deferred.resolve(profilesUrls);
                    }
                }).error(function(err) {
                    deferred.reject(err);
                });
            }, function(err) {
                deferred.reject(err);
            });
            
            return deferred.promise;
        }
    }

})();
