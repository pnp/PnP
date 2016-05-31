'use strict';
// ===============
// Widget Wrangler
// ===============
//
// This is a light-weight JavaScript library that loads "widgets" written in MV* style libraries such
// as AngularJS. It manages loading JS libraries efficiently and without conflict when they are used
// by multiple independent widgets on a page.
//
// Widget Wrangler does not depend on any other Javascript libraries
//

var ww = window.ww || function () {
    var ww = {

        version: "1.0.0",       // Library verison number

        // *** SCRIPT MANAGEMENT ***

        // Each script library that's needed is tracked via a script object with the following properties:
        //  index - index into the scriptLibraries[] array for this script
        //  scriptSrc - source URL for the script
        //  status - set to "Loading" or "Complete"

        scriptLibraries: [],     // Array of scripts that have been loaded or in process of loading.
        
        // loadScript(scriptSrc)
        //  This function gets called whenever a script needs to be loaded. It handles the possibility
        //  that it has already been loaded, is in the progress of loading, or has never been loaded
        //  at all, and notifies the appScriptLoaded when loading is complete.
        loadScript: function loadScript(scriptSrc) {

            var scriptLoading = {};         // Object found in - or to be added to - the scriptLibraries array

            // Check if script is already queued
            for (var i = 0; i < ww.scriptLibraries.length; i++) {
                if (ww.scriptLibraries[i].scriptSrc === scriptSrc) {
                    scriptLoading = ww.scriptLibraries[i];
                    break;
                }
            }

            // If script was not already queued, add it to the queue and start loading it
            if (Object.getOwnPropertyNames(scriptLoading).length < 1) {

                // First add it to the scriptLibraries array
                var newScript = {
                    index: ww.scriptLibraries.length,
                    scriptSrc: scriptSrc,
                    status: "Loading"
                };
                ww.scriptLibraries.push(newScript);
                scriptLoading = ww.scriptLibraries[ww.scriptLibraries.length - 1];

                // Now tell the browser to load the script
                var head = document.getElementsByTagName('head')[0];
                var script = document.createElement('script');
                script.type = 'text/javascript';
                if (script.readyState) {
                    script.onreadystatechange = function () {
                        if (this.readyState == 'complete') ww.scriptLoaded(newScript.index);
                    };
                }
                script.onload = function () {
                    ww.scriptLoaded(newScript.index);
                }
                script.onerror = function () {
                    newScript.status = "Error";
                    ww.scriptError(newScript.index);
                }
                script.src = scriptSrc;
                head.appendChild(script);
            }

            return scriptLoading;
        },

        // scriptLoaded(index)
        //  This function is called when a script has finished loading
        scriptLoaded: function scriptLoaded(index) {
            if (ww.scriptLibraries[index].status !== "Error") {
                // Mark the script as complete
                ww.scriptLibraries[index].status = "Complete";
                // Look through apps for any that are waiting for a script to load, and call
                // their appScriptLoaded with the index of the script that has completed
                for (var i = 0; i < ww.apps.length; i++) {
                    if (ww.apps[i].appStatus === "Waiting") {
                        ww.apps[i].appScriptLoaded(index);
                    }
                }
            }
        },
        // scriptError(index)
        //  This function handles script loading errors
        scriptError: function scriptError(index) {
            // Look through apps for any that are waiting for a script to load, and call
            // their appScriptError with the index of the script that has errored
            for (var i = 0; i < ww.apps.length; i++) {
                if (ww.apps[i].appStatus === "Waiting") {
                    ww.apps[i].appScriptError(index);
                }
            }
        },

        // *** APP MANAGEMENT ***

        // Each instance of WW on a page, along with its parent HTML element, is considered an "app" and
        // is tracked using an appObj object.

        apps: [],       // Array of all known apps on this page

        // appObj - prototype for an object that represents an app.
        appObj: function appObj() {
            var obj = {
                appId: "",                // Unique string identifier for this app
                appName: "",              // App name (also the Angular controller name)
                appType: "",              // App type (set to "Angular")
                appElement: {},           // DOM element to bind to
                appStatus: "Not Started", // App Status is "Not Started", "Waiting", "Complete" or "Error"
                appScripts: [],           // Array of appScriptObj objects for script this app requires
                appExecPriority: 0,       // The app's current executing priorty.
                appMaxPriority: 0,        // The app's highest priority.
                // startLoading()
                //   This function initializes the apps scripts at priority 0 to be loaded.
                startLoading: function startLoading() {
                    this.appMaxPriority = this.appScripts[this.appScripts.length-1].priority;
                    for (var i = 0; i < this.appScripts.length; i++) {
                        if (this.appScripts[i].priority === 0) {
                            this.appStatus = "Waiting";
                            var newScript = ww.loadScript(this.appScripts[i].src);
                            this.appScripts[i].status = newScript.status;
                            this.appScripts[i].index = newScript.index;
                            if (newScript.status === "Complete") {
                                //If script is already loaded, notify appScriptLoaded for this app.
                                this.appScriptLoaded(newScript.index);
                            } else if (newScript.status === "Error") {
                                //If script is already loaded, but in error, call appScriptError for this app.
                                this.appScriptError(newScript.index);
                            }
                        }
                    }
                },
                // appScriptLoaded(index)
                //  This function is called whenever an app needs to be notified that a script has loaded.
                //  index is an index into the appScripts array
                appScriptLoaded: function appScriptLoaded(index) {
                    var appLoaded = false,
                        priorityComplete = false,
                        completedScript = null;
                    // First, try to find the appScript corresponding to the script that finished loading
                    for (var i = 0; i < this.appScripts.length; i++) {
                        if (this.appScripts[i].index === index) {
                            completedScript = this.appScripts[i];
                        }
                    }
                    if (completedScript !== null) {
                        // If here, we found the appScript. Mark it complete.  Could be a script that this app hasn't loaded yet.
                        completedScript.status = "Complete";
                        if (completedScript.priority === this.appExecPriority) {
                            // Check if all the scripts we were waiting for are done. If so, we can infer that
                            // we've finished loading all scripts at this priority and should either start loading
                            // the next priority of scripts or bootstrap the app.
                            priorityComplete = true;
                            for (var i = 0; i < this.appScripts.length; i++) {
                                if (this.appScripts[i].priority === this.appExecPriority && this.appScripts[i].status !== "Complete")
                                    priorityComplete = false;
                            }
                            if (priorityComplete) {
                                appLoaded = true;
                                while (appLoaded && this.appExecPriority <= this.appMaxPriority) {
                                    appLoaded = true;
                                    // Look for additional scripts at the next priority and, if found, start them if they're not already complete.
                                    this.appExecPriority++;
                                    for (var i = 0; i < this.appScripts.length; i++) {
                                        if (this.appScripts[i].priority === this.appExecPriority && this.appScripts[i].status !== "Complete") {
                                            var newScript = ww.loadScript(this.appScripts[i].src);
                                            this.appScripts[i].status = newScript.status;
                                            this.appScripts[i].index = newScript.index;
                                            if (newScript.status !== "Complete") {
                                                appLoaded = false;
                                            }
                                            if (newScript.status === "Error") {
                                                //If script is already loaded, but in error, call appScriptError for this app.
                                                this.appScriptError(newScript.index);
                                            }
                                        }
                                    }
                                }
                            }
                            if (appLoaded) {
                                // If here, all the scripts needed by the app are loaded.
                                this.appStatus = "Complete";
                                // Bootstrap the Angular controller
                                if (this.appType === "Angular") {
                                    try {
                                        window.angular.bootstrap(this.appElement, [this.appName]);
                                        console.log(this.appName + "(" + this.appId + ")" + " loading complete.");
                                    } catch (e) {
                                        console.log("Error bootstrapping application: " + this.appName + "(" + this.appId + ")");
                                        console.log(e);
                                    }
                                }
                            }
                        }
                    }
                },
                // appScriptError(index)
                //  This script handles checking if a script the app depends on has errored.
                appScriptError: function appScriptError(index) {
                    // Try to find the appScript corresponding to the script that finished loading
                    for (var i = 0; i < this.appScripts.length; i++) {
                        if (this.appScripts[i].index === index) {
                            this.appStatus = "Error";
                            console.log(this.appName + "(" + this.appId + ")" + " failed to load script: " + this.appScripts[i].src);
                        }
                    }
                }
            };
            return obj;
        },
        // appScriptObj
        //  Prototype for an object that represents a script needed by an app
        appScriptObj: function appScriptObj() {
            var obj = {
                index: -1,                  // Index in the scriptLibraries array
                src: "",                    // Source URL
                priority: 0,                // Priority where all priority 0 scripts are loaded first, then priority 1, etc.
                status: "NotLoaded",        // Status is "NotLoaded", "Loading" or "Complete"               
            };
            return obj;
        }
    };

    return ww;
}();

(function () {
    // *** In-line execution begins here ***

    // We will bind to the parent of this script block, so find it now while the DOM is still loading
    // ie. before doing any async calls to load scripts! Right now, it will be the last, deepest child in the tree.
    try {
        var element = document.documentElement; // This will hold the <script> tag's DOM element
        while (element.childNodes.length && element.lastChild.nodeType === 1) {
            element = element.lastChild;
        }
        var elementToBind = element.parentNode; // This is the parent of the script tag

        // Get the path of the script source so we can handle relative script URL's
        var scriptPath = element.src;
        scriptPath = scriptPath.substring(0, scriptPath.lastIndexOf('/') + 1);

        // Get the app parameters from the <script> element
        var appName = element.getAttribute("ww-appName");
        var appType = element.getAttribute("ww-appType");
        var appScripts = null;
        try {
            appScripts = JSON.parse(element.getAttribute("ww-appScripts"));
        } catch (e) {
            console.log("Error parsing ww-appScripts tag: " + e);
        }

        if (appScripts !== null && appName.length > 0 && appType.length > 0) {
            // Create the app object
            var newApp = new ww.appObj();
            newApp.appId = appName + ww.apps.length;
            newApp.appName = appName;
            newApp.appType = appType;
            newApp.appElement = elementToBind;

            // Add an appScript object for each script the app requires 
            for (var i = 0; i < appScripts.length; i++) {
                var newScript = new ww.appScriptObj();
                if (appScripts[i].src.substring(0, 2) === "~/") {
                    appScripts[i].src = appScripts[i].src.replace("~/", scriptPath);
                }
                newScript.src = appScripts[i].src;
                newScript.priority = appScripts[i].priority || 0;
                newApp.appScripts.push(newScript);
            }

            // Add the app to the apps collection and start loading
            ww.apps.push(newApp);
            newApp.startLoading();
        } else {
            console.log("Unable to load app: " + appName + ".  Error in script tag.");
        }
    } catch (e) {
        console.log(e);
    }
})();

