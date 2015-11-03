'use strict';

siteControllers.controller('SiteEditCtrl', ["$scope", "$routeParams", "$interval", "$http", "$window", "$sce",
    function ($scope, $routeParams, $interval, $http, $window, $sce) {
        $scope.audienceScopeDescriptions =
            {
                'Enterprise': 'Target audience 40%+ of company.',
                'Organization': 'Target audience is as large as a division, but not as small as a team.',
                'Team': 'Target audience is your workgroup or virtual team.'
            };
        $scope.expireDateTimezone = 'UTC';

        // get site URL from route
        var rawSiteUrl = $routeParams.siteUrl;
        var siteUrl = window.encodeURIComponent(rawSiteUrl);
        // load site information
        $http.get('/api/site/?siteUrl=' + siteUrl)
            .success(function (data) {
                if (data.IsSuccess == false) {
                    $scope.getResult = false;
                    alert(data.Message);
                    return;
                }
                $scope.getResult = true;
                $scope.audienceScope = data.AudienceScope == null ? 'N/A' : data.AudienceScope;
                $scope.expireDate = data.ExpireDate;
                $scope.canDecommission = data.CanDecommission;
                $scope.decommissioning = data.NeedExtend;
                $scope.extendDate = data.ExtendDate;
            })
            .error(function (data, status) {
                alert("HTTP" + status + ":" + data.ExceptionMessage);
            });

        $scope.changeAudienceScope = function (name) {
            $scope.audienceScope = name;
        };

        $scope.cancelDecommission = function () {
            $scope.decommissioning = false;
        };

        $scope.cancel = function () {
            $window.location.href = rawSiteUrl;
        };

        $scope.submit = function () {
            // get site URL from route
            var rawSiteUrl = $routeParams.siteUrl;
            var siteUrl = window.encodeURIComponent(rawSiteUrl);
            // load site information
            $http.post('/api/site/?siteUrl=' + siteUrl, {
                    AudienceScope: $scope.audienceScope,
                    NeedExtend: $scope.decommissioning,
                    IsExtend: $scope.extend,
                    ExtendDate: $scope.extendDate
                })
                .success(function (data) {
                    if (data.IsSuccess == false) {
                        alert(data.Message);
                        return;
                    }
                    $window.location.href = rawSiteUrl;
                })
                .error(function (data, status) {
                    alert("HTTP" + status + ":" + data.ExceptionMessage);
                });
        };
    }]);