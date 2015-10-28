(function () {
    // App parts will hide chrome using IsDlg=1 query string parameter.
    // There is one HTML element that isn't hidden by this parameter - this code hides it.
    // ref: http://www.vxcompany.info/2013/01/23/removing-styling-in-a-sharepoint-2013-apppart-the-easy-way/

    'use strict';


    // $.ready() - with no script lib dependencies
    // ref: http://blog.simonwillison.net/post/57956760515/addloadevent
    function addLoadEvent(func) {
        var oldonload = window.onload;
        if (typeof window.onload !== 'function') {
            window.onload = func;
        } else {
            window.onload = function () {
                if (oldonload) {
                    oldonload();
                }
                func();
            }
        }
    }

    addLoadEvent(function hideGlobalNavBox() {

        // Query string reader with no script lib dependencies
        // ref: http://stackoverflow.com/questions/901115/how-can-i-get-query-string-values-in-javascript
        function getQueryStringParam(name) {
            var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
            return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
        }

        var isDialog = getQueryStringParam('IsDlg');
        if (isDialog === '1') {
            document.getElementById('globalNavBox').style.display = "none";
        }
    });
}());

// Angular and Script Loader
(function () {

    // loadScript() - Function to dynamically load a script with no dependencies
    // ref: http://unixpapa.com/js/dyna.html
    function loadScript(url, onload) {
        var head = document.getElementsByTagName('head')[0];
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.onreadystatechange = function () {
            if (this.readyState == 'complete') onload();
        };
        script.onload = onload;
        script.src = url;
        head.appendChild(script);
    }


    // loadApp() - Function to load the application after ensuring Angular is ready
    function loadApp(myAngular, scriptPath, elementToBind)
    {
        loadScript(scriptPath +'mainController.js', function loadMainController() {
            loadScript(scriptPath + 'surveyService.js', function loadSurveyService() {
                loadScript(scriptPath + 'settingsController.js', function loadSpDataService() {
                    loadScript(scriptPath + 'listFormController.js', function loadSpDataService() {
                        loadScript(scriptPath + 'spDataService.js', function loadSpDataService() {
                            // Bind the main controller
                            myAngular.bootstrap(elementToBind, ['microSurvey']);
                        });
                    });
                });
            });
        });
    }

    // *** In-line execution begins here ***

    // We will bind to the parent of this script block, so find it now while the DOM is still loading
    // ie. before doing any async calls to load scripts! Right now, it will be the last, deepest child in the tree.
    var element = document.documentElement;
    while (element.childNodes.length && element.lastChild.nodeType === 1) {
        element = element.lastChild;
    }
    var elementToBind = element.parentNode;
    var scriptPath = element.src;
    scriptPath = scriptPath.substring(0, scriptPath.lastIndexOf('/')+1);

    // Check for old version of Angular on the page; if found, save it awway.
    var tempAngular = null;
    if (window.angular) {
        if (angular.version.major < 1 || angular.version.minor < 3) {
            tempAngular = angular;
            angular = null;
        }
    }

    // Now check for Angular on the page. If it's here, it must be an OK version. If not, load it.
    if (!window.angular) {
        loadScript(scriptPath + "angular.1.3.14.min.js", function () {
            // Pass in the angular object because the old one will be restored before
            // the app loads (due to async calls in the loadApp function)
            loadApp(angular, scriptPath, elementToBind);

            // Restore the other version of angular, if any
            if (tempAngular) {
                angular = tempAngular;
            }
        });
    } else {
        loadApp(angular, scriptPath, elementToBind);
    }

})();
