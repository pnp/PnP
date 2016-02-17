var additionalOwnersPicker;


(function () {
    'use strict';

    angular
        .module('app.wizard')
        .controller('SiteOwnersPeoplePickerController', SiteOwnersPeoplePickerController);
    //.value('urlparams', null);

    SiteOwnersPeoplePickerController.$inject = ['$scope', '$log', 'utilservice', 'peoplepickerfactory'];

    function SiteOwnersPeoplePickerController($scope, $log, $utilservice, $peoplepickerfactory) {
        $scope.title = 'SiteOwnersPeoplePickerController';

        $scope.AddSecondaryOwners = function (data) {
            if (data != undefined) {
                var _resolvedOwners = jQuery.parseJSON(data);
                var _ownersEmail = [];
                for (var i = 0; i < _resolvedOwners.length; i++) {
                    _ownersEmail.push(_resolvedOwners[i].email);
                }
                $scope.siteConfiguration.secondaryOwners = _ownersEmail;
                var owners = $('#ppSecondaryOwners').spSecondaryOwnersPicker('get');
                $scope.siteConfiguration._resolvedOwners = owners;
            }
        }

        var context;
        var hostweburl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
        var appweburl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
        var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

        // resources are in URLs in the form:
        // web_url/_layouts/15/resource
        var scriptbase = hostweburl + "/_layouts/15/";

        // Load the js files and continue to the successHandler
        $.getScript(scriptbase + "SP.Runtime.js",
            function () {
                $.getScript(scriptbase + "SP.js",
                    function () {
                        $.getScript(scriptbase + "SP.RequestExecutor.js",
                             function () {
                                 context = new SP.ClientContext(appweburl);
                                 var factory = new SP.ProxyWebRequestExecutorFactory(appweburl);
                                 context.set_webRequestExecutorFactory(factory);

                                 activate();

                             }
                        );
                    }
                );
            }
        );

        function activate() {
            $log.info($scope.title + ' Activated');
            initSiteOwnersPeoplePickers(context);
        }

        $scope.getEmailFromPicker = function (e) {
            var elem = angular.element(e.srcElement);
            alert(elem.val());
        }

        function initSiteOwnersPeoplePickers(context) {
            
            // setup people pickers
            $('#ppSecondaryOwners').spSecondaryOwnersPicker({
                onLoaded: function () {
                    if ($scope.siteConfiguration._resolvedOwners != undefined) {
                        var _resolvedOwners = $scope.siteConfiguration._resolvedOwners;
                        $(this).spSecondaryOwnersPicker('set', _resolvedOwners);
                    }
                }
            });
        }

        //function to get a parameter value by a specific key
        function getQueryStringParameter(urlParameterKey) {
            var params = document.URL.split('?')[1].split('&');
            var strParams = '';
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split('=');
                if (singleParam[0] == urlParameterKey)
                    return singleParam[1];
            }
        }

    }

})();