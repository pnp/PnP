'use strict';

siteControllers.controller('UnlockCtrl', ["$scope", "$interval", "$http", "$location", "$sce",
    function ($scope, $interval, $http, $location, $sce) {
        $scope.loading = false;
        $scope.result = '';
        $scope.unlock = function () {
            // return if unlock request is submitted
            if ($scope.loading)
                return;
            // set status to loading
            $scope.loading = true;
            $scope.result = $sce.trustAsHtml("<span class='result-success'>Loading...</span>");
            var dot = 0;
            // keep updating prompt text till $http responses
            var promise = $interval(function () {
                if (!$scope.loading) {
                    $interval.cancel(promise);
                    return;
                }
                if (dot > 3)
                    dot = 0;
                var text = 'Loading';
                text += new Array(dot + 1).join('.');
                $scope.result = $sce.trustAsHtml(
                    "<span class='result-success'>" + text + "</span>");
                dot++;
            }, 500);
            // get site URL from route
            var siteUrl = $location.search()['SiteUrl'];
            siteUrl = window.encodeURIComponent(siteUrl);
            // send unlock site collection request
            $http.get('/api/unlock/?siteUrl=' + siteUrl)
                .success(function (data) {
                    // upadte prompt text after $http responses
                    $scope.result = data.IsSuccess == true ? 
                        $sce.trustAsHtml("<span class='result-success'>Success!</span>") : 
                        $sce.trustAsHtml("<span class='result-failed'>Failed!</span><span class='result-failed-msg'> " + data.Message + "</span>");
                    $scope.loading = false;
                })
                .error(function (data, status) {
                    $scope.result = 
                        $sce.trustAsHtml("<span class='result-failed'>Failed!</span><span class='result-failed-msg'> HTTP" + status + ":" + data.ExceptionMessage + "</span>");
                    $scope.loading = false;
                });
        };
    }]);