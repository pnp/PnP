/*! Contoso.AppPartPropertyUIOverride.js
*
*  Example JavaScript library that makes it easy to change an App Part's
*  property user interface at runtime via JavaScript.
*
*/

var Contoso;
(function (Contoso) {
    "use strict";
    var AppPartPropertyUIOverride = (function () {
        function AppPartPropertyUIOverride() {
        }
        AppPartPropertyUIOverride.createNewContentAtBottom = function (settings) {
            /// <summary>
            /// Public static function that creates a new html content area in the
            /// specified App Part Property UI category bottom and returns a jQuery
            /// object that wraps the new content area created.
            /// </summary>
            /// <param name="settings" type="Object">
            /// A JavaScript object that contains the required and optional
            /// settings for this operation.  {category: "The Category"} is
            /// required.Optional object properties are: optionalName,
            /// optionalToolTip, and outputSeparator
            /// </param>
            /// <returns type="jQuery">
            /// A jQuery object that wraps the new content area created.
            /// </returns>
            var actualSettings = {
                category: "",
                optionalName: "",
                optionalToolTip: "",
                outputSeparator: true
            };

            // merge user supplied settings with default settings
            $.extend(actualSettings, settings);

            // get the category content table jQuery wrapper
            var categoryContentTable = AppPartPropertyUIOverride.zinternal.getCategoryContentTable(actualSettings.category);

            // increment the new property counter
            AppPartPropertyUIOverride.zinternal.newPropertyCounter = AppPartPropertyUIOverride.zinternal.newPropertyCounter + 1;
            var newPropertyCounter = AppPartPropertyUIOverride.zinternal.newPropertyCounter;

            // get last td
            categoryContentTable.find("td:last").append("<div class=\"UserDottedLine\" style=\"width: 100%;\"></div>");

            // build html string to inject
            var html = [];
            html.push("<tr><td>");

            if (actualSettings.optionalName !== null && actualSettings.optionalName !== "") {
                html.push("<div class=\"UserSectionHead\"><label title=\"" + actualSettings.optionalToolTip + "\">" + actualSettings.optionalName + "</label></div>");
            }

            html.push("<div class=\"UserSectionBody\" id=\"AppPartPropertyUINewContentArea" + newPropertyCounter + "\" style=\"margin-bottom: 10px;\"></div>");

            html.push("</td></tr>");

            categoryContentTable.append(html.join(""));
            return categoryContentTable.find("#AppPartPropertyUINewContentArea" + newPropertyCounter);
        };

        AppPartPropertyUIOverride.createNewContentAtTop = function (settings) {
            /// <summary>
            /// Public static function that creates a new html content area in the
            /// specified App Part Property UI category top and returns a jQuery
            /// object that wraps the new content area created.
            /// </summary>
            /// <param name="settings" type="Object">
            /// A JavaScript object that contains the required and optional
            /// settings for this operation.  {category: "The Category"} is
            /// required.Optional object properties are: optionalName,
            /// optionalToolTip, and outputSeparator
            /// </param>
            /// <returns type="jQuery">
            /// A jQuery object that wraps the new content area created.
            /// </returns>
            var actualSettings = {
                category: "",
                optionalName: "",
                optionalToolTip: "",
                outputSeparator: true
            };

            // merge user supplied settings with default settings
            $.extend(actualSettings, settings);

            // get the category content table jQuery wrapper
            var categoryContentTable = AppPartPropertyUIOverride.zinternal.getCategoryContentTable(actualSettings.category);

            // increment the new property counter
            AppPartPropertyUIOverride.zinternal.newPropertyCounter = AppPartPropertyUIOverride.zinternal.newPropertyCounter + 1;
            var newPropertyCounter = AppPartPropertyUIOverride.zinternal.newPropertyCounter;

            // build html string to inject
            var html = [];
            html.push("<tr><td>");

            if (actualSettings.optionalName !== null && actualSettings.optionalName !== "") {
                html.push("<div class=\"UserSectionHead\"><label title=\"" + actualSettings.optionalToolTip + "\">" + actualSettings.optionalName + "</label></div>");
            }

            html.push("<div class=\"UserSectionBody\" id=\"AppPartPropertyUINewContentArea" + newPropertyCounter + "\" style=\"margin-bottom: 10px;\"></div><div class=\"UserDottedLine\" style=\"width: 100%;\"></div>");

            html.push("</td></tr>");

            categoryContentTable.prepend(html.join(""));
            return categoryContentTable.find("#AppPartPropertyUINewContentArea" + newPropertyCounter);
        };

        AppPartPropertyUIOverride.expandCategory = function (category) {
            /// <summary>
            /// Public static function that expands the specified category
            /// and closes all others in the App Part property pane UI.
            /// </summary>
            /// <param name="category" type="String">
            /// The category display text of the category to expand.
            /// Example: "Custom Category 1"
            /// </param>
            function ensureCategoryOpened(opened, categoryJQueryWrapper) {
                // first determined the current state (is opened?)
                var currentStateIsOpened = false;
                var atag = categoryJQueryWrapper.find(".UserSectionTitle a:first");
                if (atag.html().indexOf("/TPMax2.gif") === -1) {
                    currentStateIsOpened = true;
                }

                // we got the current state... now compare it to what we want it to be
                if (currentStateIsOpened !== opened) {
                    // the current state is different than what we want it to be
                    // trigger a click on the UI
                    atag.trigger("click");
                }
            }

            // ensure the custom category dictionary is present
            AppPartPropertyUIOverride.zinternal.ensureCategoryDictionaryPresent();

            // now loop through all categories and ensure the correct ones are closed/opened
            var categories = AppPartPropertyUIOverride.zinternal.categoryDictionary;
            var categoryKeys = Object.keys(categories);
            var categoryKey = null;

            for (var i = categoryKeys.length - 1; i > -1; i = i - 1) {
                categoryKey = categoryKeys[i];
                if (categoryKey === category) {
                    ensureCategoryOpened(true, categories[categoryKey]);
                } else {
                    ensureCategoryOpened(false, categories[categoryKey]);
                }
            }
        };

        AppPartPropertyUIOverride.finished = function () {
            /// <summary>
            /// Tells the App Part property UI framework you are done with overriding the App Part property UI
            /// and to show the property pane again.
            /// </summary>
            AppPartPropertyUIOverride.zinternal.appPartPropertyPaneTdjQueryWrapper.show();
        };

        AppPartPropertyUIOverride.getValue = function (name, category) {
            /// <summary>
            /// Public static function that gets the current value of the
            /// declared string, number, enum, or boolean property that's
            /// already rendered in the App Part property UI.
            /// </summary>
            /// <param name="name" type="String">
            /// The display name of the property.  Example: "My Setting 1"
            /// </param>
            /// <param name="category" type="String">
            /// The display name of the category.  Example: "Custom Category 1"
            /// </param>
            /// <returns type="Any">
            /// The value of that property if it exists.
            /// </returns>
            var inputElementjQueryWrapper = AppPartPropertyUIOverride.zinternal.getInputElementJQueryWrapper(name, category);
            var dataType = AppPartPropertyUIOverride.zinternal.getInputElementDataType(name, category);
            var returnValue = null;
            switch (dataType) {
                case "CHECKBOX":
                    returnValue = inputElementjQueryWrapper.is(":checked");
                    break;
                case "SELECT":
                    returnValue = inputElementjQueryWrapper.val();
                    break;
                case "TEXT":
                    returnValue = inputElementjQueryWrapper.val();
                    if (!isNaN(returnValue)) {
                        returnValue = returnValue * 1;
                    }
                    break;
            }

            return returnValue;
        };

        AppPartPropertyUIOverride.hideProperty = function (name, category) {
            /// <summary>
            /// Public static function that hides the specified property in
            /// the  App Part property UI.
            /// </summary>
            /// <param name="name" type="String">
            /// The display name of the property.  Example: "My Property 1"
            /// </param>
            /// <param name="category" type="String">
            /// The display name of the category.  Example: "Custom Category 1"
            /// </param>
            AppPartPropertyUIOverride.zinternal.ensureCategoryDictionaryPresent();
            var labelElement = null;
            var bodyDivjQueryWrapper = null;
            var labelArray = AppPartPropertyUIOverride.zinternal.categoryDictionary[category].find("label");
            var parentElement = null;
            for (var i = 0; i < labelArray.length; i = i + 1) {
                labelElement = labelArray[i];
                if (labelElement.innerHTML === name) {
                    parentElement = $(labelElement).parent().parent();

                    // found parent
                    // now hide it
                    parentElement.hide();
                    break;
                }
            }
        };

        AppPartPropertyUIOverride.moveCategoryToTop = function (category) {
            /// <summary>
            /// Public static function that moves the specified category to
            /// the top of the App Part property pane UI.
            /// </summary>
            /// <param name="category" type="String">
            /// The display name of the category.  Example: "Custom Category 1"
            /// </param>
            // find the category and parent
            AppPartPropertyUIOverride.zinternal.ensureCategoryDictionaryPresent();
            var sourceDivToMove = AppPartPropertyUIOverride.zinternal.categoryDictionary[category];
            var parentDivToMoveUnder = AppPartPropertyUIOverride.zinternal.parentCategoryDivJQueryWrapper;

            // do the move
            sourceDivToMove.prependTo(parentDivToMoveUnder);

            // reload the dictionary
            AppPartPropertyUIOverride.zinternal.reloadCategoryDictionary();
        };

        AppPartPropertyUIOverride.setValue = function (name, value, category) {
            /// <summary>
            /// Public static function that sets the current value of the
            /// declared string, number, enum, or boolean property that's
            /// already rendered in the App Part property UI.
            /// </summary>
            /// <param name="name" type="String">
            /// The display name of the property.  Example: "My Setting 1"
            /// </param>
            /// <param name="name" type="Any">
            /// The value to set.  Example: "The Value"
            /// </param>
            /// <param name="category" type="String">
            /// The display name of the category.  Example: "Custom Category 1"
            /// </param>
            var inputElementjQueryWrapper = AppPartPropertyUIOverride.zinternal.getInputElementJQueryWrapper(name, category);
            var dataType = AppPartPropertyUIOverride.zinternal.getInputElementDataType(name, category);
            switch (dataType) {
                case "CHECKBOX":
                    inputElementjQueryWrapper.attr("checked", value);
                    break;
                case "SELECT":
                    inputElementjQueryWrapper.val(value);
                    break;
                case "TEXT":
                    inputElementjQueryWrapper.val(value + "");
                    break;
            }
        };

        AppPartPropertyUIOverride.renderToolTipsAsInstructions = function (category) {
            /// <summary>
            /// Public static function that renders tool tips as html
            /// instruction text below each property in the specified
            /// category.
            /// </summary>
            /// <param name="category" type="String">
            /// The display name of the category.  Example: "Custom Category 1"
            /// </param>
            AppPartPropertyUIOverride.zinternal.ensureCategoryDictionaryPresent();
            var labelElement = null;
            var bodyDivjQueryWrapper = null;
            var labelArray = AppPartPropertyUIOverride.zinternal.categoryDictionary[category].find("label");
            var parentElement = null;
            var toolTip = "";
            var labeljQueryWrapper = null;
            var userDottedLineDiv = null;
            for (var i = 0; i < labelArray.length; i = i + 1) {
                labelElement = $(labelArray[i]);
                if (labelElement.attr("style") !== "display: none;") {
                    parentElement = labelElement.parent().parent();
                    if (parentElement[0].tagName.toUpperCase() === "TD") {
                        // now we have a property TD
                        // see if we have a tool tip
                        toolTip = "";
                        labeljQueryWrapper = parentElement.find("div.UserSectionHead").find("label:first");
                        if (labeljQueryWrapper.length > 0) {
                            toolTip = labeljQueryWrapper.attr("title");
                            if (toolTip.length > 0) {
                                // see if there's UserDottedLine div
                                userDottedLineDiv = parentElement.find("div.UserDottedLine");
                                if (userDottedLineDiv.length > 0) {
                                    // need to add it before this node
                                    $(userDottedLineDiv[0]).before("<div style=\"margin-bottom: 10px;color: #cccccc;font-size: smaller;font-style:italic\" class=\"appPartInstruction\">" + toolTip + "</div>");
                                } else {
                                    // need to add it to end of parent content
                                    parentElement.append("<div style=\"margin-bottom: 10px;color: #cccccc;font-size: smaller;font-style:italic\" class=\"appPartInstruction\">" + toolTip + "</div>");
                                }
                            }
                        }
                    }
                }
            }
        };
        AppPartPropertyUIOverride.appWebFullUrl = "";

        AppPartPropertyUIOverride.hostWebFullUrl = "";

        AppPartPropertyUIOverride.hostWebServerRelativeUrl = "";

        AppPartPropertyUIOverride.language = 1033;

        AppPartPropertyUIOverride.remoteWebFullUrl = "";

        AppPartPropertyUIOverride.zinternal = {
            appPartPropertyPaneTdjQueryWrapper: null,
            categoryDictionary: null,
            ensureCategoryDictionaryPresent: function () {
                if (AppPartPropertyUIOverride.zinternal.categoryDictionary === null) {
                    AppPartPropertyUIOverride.zinternal.reloadCategoryDictionary();
                }
            },
            getInputElementDataType: function (name, category) {
                var key = category + "-" + name;
                if (AppPartPropertyUIOverride.zinternal.inputElementDataTypeDictionary.hasOwnProperty(key)) {
                    return AppPartPropertyUIOverride.zinternal.inputElementDataTypeDictionary[key];
                } else {
                    AppPartPropertyUIOverride.zinternal.getInputElementJQueryWrapper(name, category);
                    return AppPartPropertyUIOverride.zinternal.inputElementDataTypeDictionary[key];
                }
            },
            getInputElementJQueryWrapper: function (name, category) {
                var key = category + "-" + name;
                if (AppPartPropertyUIOverride.zinternal.inputElementJQueryDictionary.hasOwnProperty(key)) {
                    return AppPartPropertyUIOverride.zinternal.inputElementJQueryDictionary[key];
                } else {
                    AppPartPropertyUIOverride.zinternal.ensureCategoryDictionaryPresent();
                    var labelElement = null;
                    var bodyDivjQueryWrapper = null;
                    var labelArray = AppPartPropertyUIOverride.zinternal.categoryDictionary[category].find("label");
                    var parentElement = null;
                    for (var i = 0; i < labelArray.length; i = i + 1) {
                        labelElement = labelArray[i];
                        if (labelElement.innerHTML === name) {
                            parentElement = $(labelElement).parent().parent();
                            bodyDivjQueryWrapper = parentElement.find("div.UserSectionBody");

                            if (bodyDivjQueryWrapper.length === 0) {
                                returnValue = parentElement.find("div.UserSectionHead > span > input:checkbox");
                                if (returnValue.length > 0) {
                                    dataType = "CHECKBOX";
                                    returnValue = $(returnValue[0]);
                                    AppPartPropertyUIOverride.zinternal.inputElementJQueryDictionary[key] = returnValue;
                                    AppPartPropertyUIOverride.zinternal.inputElementDataTypeDictionary[key] = dataType;
                                    return returnValue;
                                } else {
                                    return null;
                                }
                            } else {
                                var inputElements = bodyDivjQueryWrapper.find(":input");
                                var inputElement = null;
                                var returnValue = null;
                                var dataType = null;
                                if (inputElements.length > 0) {
                                    for (var j = 0; j < inputElements.length; j = j + 1) {
                                        inputElement = inputElements[j];
                                        dataType = inputElement.tagName.toUpperCase();
                                        switch (dataType) {
                                            case "INPUT":
                                                dataType = inputElement.getAttribute("type").toUpperCase();
                                                if (dataType !== "HIDDEN") {
                                                    returnValue = $(inputElement);
                                                    break;
                                                }
                                            case "SELECT":
                                                returnValue = $(inputElement);
                                                break;
                                        }

                                        if (returnValue) {
                                            break;
                                        }
                                    }
                                }

                                if (returnValue) {
                                    AppPartPropertyUIOverride.zinternal.inputElementJQueryDictionary[key] = returnValue;
                                    AppPartPropertyUIOverride.zinternal.inputElementDataTypeDictionary[key] = dataType;
                                    return returnValue;
                                } else {
                                    return null;
                                }
                            }
                        }
                    }
                }
            },
            getCategoryContentTable: function (category) {
                AppPartPropertyUIOverride.zinternal.ensureCategoryDictionaryPresent();
                return $(AppPartPropertyUIOverride.zinternal.categoryDictionary[category].find("div.ms-propGridTable > table")[0]);
            },
            init: function (settings) {
                function ensureNotNullString(value) {
                    if (typeof value === "string") {
                        return value;
                    } else {
                        return "";
                    }
                }

                function spjsLoadedSoLoadJsFileToInvokeNext(jsFileToInvoke) {
                    var head = document.getElementsByTagName("head")[0];
                    var script = document.createElement("script");
                    script.src = jsFileToInvoke;
                    head.appendChild(script);
                }

                function jQueryLoadedSoLoadSPJSNext($, jsFileToInvoke, appPartPropertyPaneTdElement) {
                    $(document).ready(function () {
                        AppPartPropertyUIOverride.zinternal.appPartPropertyPaneTdjQueryWrapper = $(appPartPropertyPaneTdElement);
                        if (SP.ClientContext != null) {
                            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                                spjsLoadedSoLoadJsFileToInvokeNext(jsFileToInvoke);
                            }, 'sp.js');
                        } else {
                            SP.SOD.executeFunc('sp.js', null, function () {
                                spjsLoadedSoLoadJsFileToInvokeNext(jsFileToInvoke);
                            });
                        }
                    });
                }

                if (AppPartPropertyUIOverride.zinternal.scriptLoadedFlag) {
                    var jqueryPath = ensureNotNullString(settings.jqueryPath);
                    var jsFileToInvoke = ensureNotNullString(settings.jsFileToInvoke);
                    var appPartPropertyPaneTdElement = settings.appPartPropertyPaneTdElement;

                    if (window["jQuery"]) {
                        jQueryLoadedSoLoadSPJSNext(window["jQuery"], jsFileToInvoke, appPartPropertyPaneTdElement);
                    } else {
                        var head = document.getElementsByTagName("head")[0];
                        var script = document.createElement("script");
                        script.src = jqueryPath;
                        var done = false;
                        script.onload = script.onreadystatechange = function () {
                            if (!done && (!this.readyState || this.readyState == "loaded" || this.readyState == "complete")) {
                                done = true;
                                jQueryLoadedSoLoadSPJSNext(window["jQuery"], jsFileToInvoke, appPartPropertyPaneTdElement);
                            }
                        };
                        head.appendChild(script);
                    }
                } else {
                    setTimeout(function () {
                        AppPartPropertyUIOverride.zinternal.init(settings);
                    }, 10);
                }
            },
            inputElementDataTypeDictionary: {},
            inputElementJQueryDictionary: {},
            newPropertyCounter: 0,
            parentCategoryDivJQueryWrapper: null,
            reloadCategoryDictionary: function () {
                var categoryDictionary = {};
                var atags = AppPartPropertyUIOverride.zinternal.appPartPropertyPaneTdjQueryWrapper.find("div.UserSectionTitle a");
                var atag = null;
                var category = null;
                var categoryDivJQueryWrapper = null;
                var parentCategoryDiv = null;

                for (var i = 0; i < atags.length; i = i + 1) {
                    atag = $(atags[i]);
                    category = atag.html();
                    if (category.indexOf("<img") === -1) {
                        category = $.trim(category.replace("&nbsp;", " "));
                        categoryDivJQueryWrapper = atag.parents("div.ms-TPBody");

                        if (parentCategoryDiv === null) {
                            parentCategoryDiv = categoryDivJQueryWrapper.parent();
                        }

                        if (!categoryDictionary.hasOwnProperty(category)) {
                            categoryDictionary[category] = categoryDivJQueryWrapper;
                        }
                    }
                }

                AppPartPropertyUIOverride.zinternal.parentCategoryDivJQueryWrapper = parentCategoryDiv;
                AppPartPropertyUIOverride.zinternal.categoryDictionary = categoryDictionary;
            },
            scriptLoadedFlag: false,
            scriptLoaded: function () {
                AppPartPropertyUIOverride.zinternal.scriptLoadedFlag = true;
            }
        };
        return AppPartPropertyUIOverride;
    })();
    Contoso.AppPartPropertyUIOverride = AppPartPropertyUIOverride;
})(Contoso || (Contoso = {}));

// Register this JavaScript file for SharePoint 2013's SharePoint Minimal Download Strategy (MDS) if possible
RegisterModuleInit("Contoso.AppPartPropertyUIOverride.js", Contoso.AppPartPropertyUIOverride.zinternal.scriptLoaded); //MDS registration
Contoso.AppPartPropertyUIOverride.zinternal.scriptLoaded(); //non MDS run

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("Contoso.AppPartPropertyUIOverride.js");
}
//# sourceMappingURL=Contoso.AppPartPropertyUIOverride.js.map
