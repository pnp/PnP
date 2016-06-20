/* Version: 16.0.6216.3006 */
/*
    Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
    Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/

/// <reference path="outlook-win32.debug.js" />

Office._ExcelMask = 0x1;
Office._WordMask = 0x2;
Office._ProjectMask = 0x4;
Office._OutlookMask = 0x8;
Office._PowerPointMask = 0x10;
Office._OutlookComposeMask = 0x20;
Office._AccessWebAppMask = 0x40;

{
    Office._extractedCallback = function (originalArgs, totalArgsCount, optionalArgsCount) {
        var optionalArgs = Array.prototype.slice.call(originalArgs, totalArgsCount - optionalArgsCount);
        var callback = function(){};
        for (var i = Math.min(optionalArgs.length, optionalArgsCount) - 1; i >= 0; i--) {
            if (typeof optionalArgs[i] == "function") {
                callback = optionalArgs[i];
                break;
            }
        }
        return callback;
    }

    Office._BindingDataChangedEvents = function (eventType) {
        this.binding = new Office._Binding(bindingType);
        this.type = eventType;
        this.startColumn = {};
        this.startRow = {};
    }

    Office._DocumentEventArgs = function (eventType) {
        Office._processContents(this, {
            type: {
                annotate: {
                    /// <field type="Office.EventType"></field>
                    type: undefined
                },
                value: eventType
            }
        });
        if (eventType == "activeViewChanged") {
            Office._processItem(
                this,
                {
                    annotate: {
                        /// <field type="Office.ActiveView"></field>
                        activeView: undefined
                    }
                },
                "activeView"
            );
        }
    }

    Office._CustomXmlNodeEvents = function (eventType) {
        this.type = eventType;
        this.inUndoRedo = {};

        if (eventType == 'nodeDeleted') {
            this.oldNode = new Office._CustomXmlNode();
            this.oldNextSibling = new Office._CustomXmlNode();
        }

        if (eventType == 'nodeInserted') {
            this.newNode = new Office._CustomXmlNode();


        }
        if (eventType == 'nodeReplaced') {
            this.oldNode = new Office._CustomXmlNode();
            this.newNode = new Office._CustomXmlNode();

        }
    }

    Office._Error = function () {
        this.id = {};
        this.message = {};
        this.name = {};
    }

    Office._CustomXmlNode = function () {
        this.baseName = {};
        this.namespaceUri = {};
        this.nodeType = {};

        this.getNodesAsync = function (xPath, callback) {
            ///<summary> Gets the nodes associated with the xPath expression.  </summary>
            ///<param name="xPath" type="string">The xPath expression</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("getNodesAsync");
            callback(result);
        };
        this.getNodeValueAsync = function (callback) {
            ///<summary> Gets the node value.  </summary>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>

            var result = new Office._AsyncResult("getNodeValueAsync");
            callback(result);
        };
        this.getXmlAsync = function (callback) {
            ///<summary> Gets the node's XML.  </summary>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("getXmlAsync");
            callback(result);
        };
        this.setNodeValueAsync = function (value, callback) {
            ///<summary> Sets the node value.  </summary>
            ///<param name="value" type="string">The value to be set on the node</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("setNodeValueAsync");
            callback(result);
        };
        this.setXmlAsync = function (xml, callback) {
            ///<summary> Sets the node XML.  </summary>
            ///<param name="xml" type="string">The XML to be set on the node</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("setXmlAsync");
            callback(result);
        };
    }

    Office._context_document_customXmlParts_customXmlPrefixMappings = function () {
        this.addNamespaceAsync = function (prefix, nsUri, callback) {
            ///<summary>Adds a namespace.  </summary>
            //////<param name="prefix" type="string">The namespace prefix</param>
            //////<param name="nsUri" type="string">The namespace URI</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>

            var result = new Office._AsyncResult("addNamespaceAsync");
            callback(result);
        };
        this.getNamespaceAsync = function (prefix, callback) {
            ///<summary> Gets a namespace  with the specified prefix </summary>
            ///<param name="prefix" type="string">The namespace prefix</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("getNamespaceAsync");
            callback(result);
        };
        this.getPrefixAsync = function (nsUri, callback) {
            ///<summary> Gets a prefix  for  the specified URI </summary>
            ///<param name="nsUri" type="string">The namespace URI</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>

            var result = new Office._AsyncResult("getPrefixAsync");
            callback(result);
        };
    }

    Office._CustomXmlPart = function () {
        this.builtIn = {};
        this.id = {};
        this.namespaceManager = new Office._context_document_customXmlParts_customXmlPrefixMappings();

        this.deleteAsync = function (callback) {
            ///<summary> Deletes the Custom XML Part.  </summary>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("deleteAsync");
            callback(result);
        };
        this.getNodesAsync = function (xPath, callback) {
            ///<summary> Gets the nodes associated with the xPath expression.  </summary>
            ///<param name="xPath" type="string">The xPath expression</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>

            var result = new Office._AsyncResult("getNodesAsync");
            callback(result);
        };
        this.getXmlAsync = function (callback) {
            ///<summary> Gets the XML for the Custom XML Part.  </summary>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("getXmlAsync");
            callback(result);
        };

        this.addHandlerAsync = function (eventType, handler, callback) {
            ///<summary> Adds an event handler to the object using the specified event type.  </summary>
            ///<param name="eventType" type="Office.EventType">The event type. For CustomXmlPartNode it can be 'nodeDeleted', 'nodeInserted' or 'nodeReplaced' </param>
            ///<param name="handler" type="function">The name of the handler </param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>


            var events = new Office._CustomXmlNodeEvents(eventType);
            handler(events);

            var result = new Office._AsyncResult("addHandlerAsync");
            callback(result);
        };

        this.removeHandlerAsync = function (eventType, options, callback) {
            ///<summary> Removes an event handler from the object using the specified event type.  </summary>
            ///<param name="eventType" type="Office.EventType">The event type. For CustomXmlPartNode it can be 'nodeDeleted', 'nodeInserted' or 'nodeReplaced' </param>
            ///<param name="options" type="Object" optional="true">
            ///    Syntax example: {handler:eventHandler}
            /// &#10;     handler: Indicates a specific handler to be removed, if not specified all handlers are removed
            /// &#10;     asyncContext: Object keeping state for the callback
            ///</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            ///
            Office._extractedCallback(arguments, 3, 2)(new Office._AsyncResult("removeHandlerAsync"));
        }
    }

    Office._Binding = function (bindingType) {
        ///<field type="String" name="id">Id of the Binding</field>};
        this.id = {};

        this.type = {};
        this.document = {};

        this.setDataAsync = function (data, options, callback) {
            ///<summary> Writes the specified data into the current selection.</summary>
            ///<param name="data" type="Object">The data to be set. Either a string or value, 2d array or TableData object</param>
            ///<param name="options" type="Object" optional="true">
            ///    Syntax example: {coercionType:Office.CoercionType.Matrix} or {coercionType: 'matrix'}
            /// &#10;     coercionType: Explicitly sets the shape of the data object. Use Office.CoercionType or text value. If not supplied is inferred from the data type.
            /// &#10;     startRow: Used in partial set for table/matrix. Indicates the start row.
            /// &#10;     startColumn: Used in partial set for table/matrix. Indicates the start column.

            /// &#10;     asyncContext: Object keeping state for the callback
            ///</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            Office._extractedCallback(arguments, 3, 2)(new Office._AsyncResult("setDataAsync"));
        };

        this.getDataAsync = function (options, callback) {
            ///<summary> Returns the current selection.</summary>
            ///<param name="options" type="Object" optional="true">
            ///    Syntax example: {coercionType: 'matrix,'valueFormat: 'formatted', filterType:'all'}
            /// &#10;     coercionType: The expected shape of the selection. If not specified returns the bindingType shape. Use Office.CoercionType or text value.
            /// &#10;     valueFormat: Get data with or without format. Use Office.ValueFormat or text value.
            /// &#10;     startRow: Used in partial get for table/matrix. Indicates the start row.
            /// &#10;     startColumn: Used in partial get for table/matrix. Indicates the start column.
            /// &#10;     rowCount: Used in partial get for table/matrix. Indicates the number of rows from the start row.
            /// &#10;     columnCount: Used in partial get for table/matrix. Indicates the number of columns from the start column.
            /// &#10;     filterType: Get the visible or all the data. Useful when filtering data. Use Office.FilterType or text value.
            /// &#10;     asyncContext: Object keeping state for the callback
            ///</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>

            if (arguments.length == 1) {
                callback = options;
            }
            var result = new Office._AsyncResult("getDataAsync", options.coercionType);
            callback(result);
        };

        this.addHandlerAsync = function (eventType, handler, callback) {
            ///<summary> Adds an event handler to the object using the specified event type.  </summary>
            ///<param name="eventType" type="Office.EventType">The event type. For binding it can be 'bindingDataChanged' and 'bindingSelectionChanged' </param>
            ///<param name="handler" type="function">The name of the handler </param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>


            var events = new Office._BindingDataChangedEvents(eventType);
            handler(events);

            var result = new Office._AsyncResult("addHandlerAsync");
            callback(result);
        };

        this.removeHandlerAsync = function (eventType, options, callback) {
            ///<summary> Removes an event handler from the object using the specified event type.  </summary>
            ///<param name="eventType" type="Office.EventType">The event type. For binding can be 'bindingDataChanged' and 'bindingSelectionChanged' </param>
            ///<param name="options" type="Object" optional="true">
            ///    Syntax example: {handler:eventHandler}
            /// &#10;     handler: Indicates a specific handler to be removed, if not specified all handlers are removed
            /// &#10;     asyncContext: Object keeping state for the callback
            ///</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>


            var events = new Office._BindingDataChangedEvents(eventType);
            handler(events);

            Office._extractedCallback(arguments, 3, 2)(new Office._AsyncResult("removeHandlerAsync"));
        };


        if ((bindingType == undefined) || (bindingType == Office.BindingType.Matrix) || (bindingType == Office.BindingType.Table)) {
            this.columnCount = {};
            this.rowCount = {};
        }
        if ((bindingType == undefined) || (bindingType == Office.BindingType.Table)) {
            Office._processContents(this, {
                hasHeaders: {
                    value: {}
                },
                addRowsAsync: {
                    value: function (data, callback) {
                        ///<summary> Adds the specified rows to the table  </summary>
                        ///<param name="data" type="Object"> A 2D array with the rows to add </param>

                        ///<param name="callback" type="function" optional="true">The optional callback method</param>
                    }
                },
                addColumnsAsync: {
                    value: function (tableData, callback) {
                        ///<summary> Adds the specified columns to the table  </summary>
                        ///<param name="tableData" type="Object"> A TableData object with the headers and rows </param>

                        ///<param name="callback" type="function" optional="true">The optional callback method</param>};
                    }
                },
                deleteAllDataValuesAsync: {
                    value: function (callback) {
                        ///<summary> Clears the table</summary>
                        ///<param name="callback" type="function" optional="true">The optional callback method</param>};
                    }
                },
                formattingSpecific: {
                    metaOnly: true,
                    conditions: {
                        hosts: ["excel"],
                    },
                    contents: {
                        clearFormatsAsync: {
                            conditions: {
                                reqs: ["method TableBinding.clearFormatsAsync"]
                            },
                            value: function (options, callback) {
                                ///<summary> Clears formatting on the bound table. </summary>
                                ///<param name="options" type="Object" optional="true">
                                ///    Syntax example: {asyncContext:context}
                                /// &#10;     asyncContext: Object keeping state for the callback
                                ///</param>
                                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                                Office._extractedCallback(arguments, 2, 2)(new Office._AsyncResult("clearFormatsAsync"));
                            }
                        },
                        getFormatsAsync: {
                            conditions: {
                                reqs: ["method TableBinding.getFormatsAsync"]
                            },
                            value: function (cellReference, formats, options, callback) {
                                ///<summary> Gets the formatting on specified items in the table. </summary>
                                ///<param name="cellReference" type="Object" optional="true">An object literal containing name-value pairs that specify the range of cells to get formatting from.</param>
                                ///<param name="formats" type="Array" optional="true">An array specifying the format properties to get.</param>
                                ///<param name="options" type="Object" optional="true">
                                ///    Syntax example: {asyncContext:context}
                                /// &#10;     asyncContext: Object keeping state for the callback
                                ///</param>
                                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                                Office._extractedCallback(arguments, 4, 4)(new Office._AsyncResult("getFormatsAsync"));
                            }
                        },
                        setFormatsAsync: {
                            conditions: {
                                reqs: ["method TableBinding.setFormatsAsync"]
                            },
                            value: function (formatsInfo, options, callback) {
                                ///<summary> Sets formatting on specified items and data in the table. </summary>
                                ///<param name="formatsInfo" type="Array" elementType="Array" optional="true">
                                ///    Array elements are themselves three-element arrays:
                                ///    [target, type, formats]
                                /// &#10;     target: The identifier of the item to format. String.
                                /// &#10;     type: The kind of item to format. String.
                                /// &#10;     formats: An object literal containing a list of property name-value pairs that define the formatting to apply.
                                ///</param>
                                ///<param name="options" type="Object" optional="true">
                                ///    Syntax example: {asyncContext:context}
                                /// &#10;     asyncContext: Object keeping state for the callback
                                ///</param>
                                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                                Office._extractedCallback(arguments, 3, 3)(new Office._AsyncResult("setFormatsAsync"));
                            }
                        },
                        setTableOptionsAsync: {
                            conditions: {
                                reqs: ["method TableBinding.setTableOptionsAsync"]
                            },
                            value: function (tableOptions, options, callback) {
                                ///<summary> Updates table formatting options on the bound table. </summary>
                                ///<param name="tableOptions" type="Object">An object literal containing a list of property name-value pairs that define the table options to apply.</param>
                                ///<param name="options" type="Object" optional="true">
                                ///    Syntax example: {asyncContext:context}
                                /// &#10;     asyncContext: Object keeping state for the callback
                                ///</param>
                                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                                Office._extractedCallback(arguments, 3, 2)(new Office._AsyncResult("setTableOptionsAsync"));
                            }
                        }
                    }
                }
            });
        }
    }

    Office._TableData = function () {
        this.headers = new Array(new Array());
        this.rows = new Array(new Array());
    }

    Office._File = function () {
        this.size = {};
        this.sliceCount = {};
        this.getSliceAsync = function (sliceIndex, callback) {
            ///<summary> Gets the specified slice. </summary>
            ///<param name="sliceIndex" type="Integer">The index of the slice to be retrieved </param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("getSliceAsync");
            callback(result);
        };
        this.closeAsync = function (callback) {
            ///<summary> Closes the File. </summary>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
        };
    }

    Office._Slice = function () {
        this.data = {};
        this.index = {};
        this.size = {};
    }

    Office._AsyncResult = function (method, bindingType) {
        this.asyncContext = {};
        this.error = new Office._Error();
        this.status = {};

        if ((method == "addfromSelectionAsync") || (method == "addFromNamedItemAsync") || (method == "getByIdAsync") || (method == "addFromPromptAsync")) {
            this.value = new Office._Binding(bindingType);
        } else if ((method == "getDataAsync") || (method == "getSelectedDataAsync")) {
            if (bindingType == "table")
                this.value = new Office._TableData();
            else if (bindingType == "matrix")
                this.value = new Array(new Array);
            else
                this.value = {};
        } else if (method == "getAllAsync") {
            this.value = new Array(new Office._Binding(bindingType));
        } else if (method == "getByNamespaceAsync") {
            this.value = new Array(new Office._CustomXmlPart());
        } else if (method == "getNodesAsync") {
            this.value = new Array(new Office._CustomXmlNode());
        } else if ((method == "XMLgetByIdAsync") || (method == "addAsync")) {
            this.value = new Office._CustomXmlPart();
        } else if (method == "refreshAsync") {
            this.value = new Office._context_document_settings();
        } else if (method == "getFileAsync") {
            this.value = new Office._File();
        } else if (method == "getSliceAsync") {
            this.value = new Office._Slice();
        } else if (method == "getActiveViewAsync") {
            Office._processItem(this,
                {
                    annotate: {
                        ///<field type="String">The presentation's current view.</field>
                        value: undefined
                    }
                },
                "value"
            );
        } else if (method == "getFilePropertiesAsync") {
            this.value = new Office._FileProperties();
        } else {
            this.value = {};
        }
    }

    Office._FileProperties = function () {
        ///<field type="String" name="url">File's URL</field>
        this.url = "";
    }

    Office._context_document_settings = function () {
        this.get = function (settingName) {
            ///<summary>Retrieves the setting with the specified name.</summary>
            ///<param name="settingName" type="string">The name of the setting </param>
        };

        this.remove = function (settingName) {
            ///<summary>Removes the setting with the specified name.</summary>
            ///<param name="settingName" type="string">The name of the setting </param>
            ///
        };

        this.saveAsync = function (options, callback) {
            ///<summary>Saves all settings.</summary>
            ///<param name="options" type="Object" optional="true">
            ///    Syntax example: {overwriteIfStale:false}
            /// &#10;     overwriteIfStale: Indicates whether the setting will be replaced if stale.
            /// &#10;     asyncContext: Object keeping state for the callback
            ///</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            ///
            Office._extractedCallback(arguments, 2, 2)(new Office._AsyncResult("saveAsync", coercionType));
        };

        this.set = function (settingName, value) {
            ///<summary>Sets a value for the setting with the specified name.</summary>
            ///<param name="settingName" type="string">The name of the setting</param>
            ///<param name="value" type="object">The value for the setting</param>
        };
    };

    Office._context_document_bindings = function () {
        this.document = {};

        if (Office._AccessWebAppMask & Office._appContext) {
            this.addFromSelectionAsync = function (bindingType, options, callback) {
                ///<summary>Create a binding based on what the user's current selection.</summary>
                ///<param name="bindingType" type="Office.BindingType">The Office BindingType for the data</param>
                ///<param name="options" type="Object" optional="true">
                ///    addFromSelectionAsyncOptions- e.g. {id: "BindingID"}
                /// &#10;     id: Identifier.
                /// &#10;     asyncContext: Object keeping state for the callback
                /// &#10;     columns: The string[] of the columns involved in the binding
                /// &#10;     sampleData: A TableData that gives sample table in the Dialog.TableData.Headers is [][] of string.
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                if (arguments.length == 2) { callback = options; };
                var result = new Office._AsyncResult("addfromSelectionAsync", bindingType);
                callback(result);
            }
        } else {
            this.addFromSelectionAsync = function (bindingType, options, callback) {
                ///<summary>Create a binding based on what the user's current selection.</summary>
                ///<param name="bindingType" type="Office.BindingType">The Office BindingType for the data</param>
                ///<param name="options" type="Object" optional="true">
                ///    addFromSelectionAsyncOptions- e.g. {id: "BindingID"}
                /// &#10;     id: Identifier.
                /// &#10;     asyncContext: Object keeping state for the callback
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                if (arguments.length == 2) { callback = options; };
                var result = new Office._AsyncResult("addfromSelectionAsync", bindingType);
                callback(result);
            }
        }
        if (Office._AccessWebAppMask & Office._appContext) {
            this.addFromNamedItemAsync = function (itemName, bindingType, options, callback) {
                ///<summary>Creates a binding against a named object in the document</summary>
                ///<param name="itemName" type="String">Name of the bindable object in the document. For Example 'MyExpenses' table in Excel." </param>
                ///<param name="bindingType" type="Office.BindingType">The Office BindingType for the data</param>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {id: "BindingID"}
                /// &#10;     id: Name of the binding, autogenerated if not supplied. 
                /// &#10;     asyncContext: Object keeping state for the callback
                /// &#10;     columns: The string[] of the columns involved in the binding
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>

                if (arguments.length == 3) { callback = options; };
                var result = new Office._AsyncResult("addFromNamedItemAsync", bindingType);
                callback(result);
            }
        } else {
            this.addFromNamedItemAsync = function (itemName, bindingType, options, callback) {
                ///<summary>Creates a binding against a named object in the document</summary>
                ///<param name="itemName" type="String">Name of the bindable object in the document. For Example 'MyExpenses' table in Excel." </param>
                ///<param name="bindingType" type="Office.BindingType">The Office BindingType for the data</param>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {id: "BindingID"}
                /// &#10;     id: Name of the binding, autogenerated if not supplied. 
                /// &#10;     asyncContext: Object keeping state for the callback
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>

                if (arguments.length == 3) { callback = options; };
                var result = new Office._AsyncResult("addFromNamedItemAsync", bindingType);
                callback(result);
            }
        }
        this.getByIdAsync = function (id, callback) {
            ///<summary>Retrieves a binding based on its Name</summary>
            ///<param name="id" type="String">The binding id</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>

            var result = new Office._AsyncResult("getByIdAsync")
            callback(result);
        }
        this.getAllAsync = function (callback) {
            ///<summary>Gets an array with all the binding objects in the document.</summary>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("getAllAsync")
            callback(result);
        };

        this.releaseByIdAsync = function (id, callback) {
            ///<summary>Removes the binding from the document</summary>
            ///<param name="id" type="String">The binding id</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            var result = new Office._AsyncResult("releaseByIdAsync")
            callback(result);
        };


        if (Office._AccessWebAppMask & Office._appContext) {
            this.addFromPromptAsync = function (bindingType, options, callback) {
                ///<summary>(Access only with sample data) Create a binding by prompting the user to make a selection on the document.</summary>
                ///<param name="bindingType" type="Office.BindingType">The Office BindingType for the data</param>
                ///<param name="options" type="Object" optional="true">
                ///    addFromPromptAsyncOptions- e.g. {promptText: "Please select data", id: "mySales"}
                /// &#10;     promptText: Greet your users with a friendly word.
                /// &#10;     asyncContext: Object keeping state for the callback
                /// &#10;     id: Identifier.
                /// &#10;     sampleData: A TableData that gives sample table in the Dialog.TableData.Headers is [][] of string.
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>

                if (arguments.length == 2) { callback = options; };
                var result = new Office._AsyncResult("addFromPromptAsync", bindingType)
                callback(result);
            }
        } else if (Office._ExcelMask & Office._appContext) {
            this.addFromPromptAsync = function (bindingType, options, callback) {
                ///<summary>(Excel only) Create a binding by prompting the user to make a selection on the document.</summary>
                ///<param name="bindingType" type="Office.BindingType">The Office BindingType for the data</param>
                ///<param name="options" type="Object" optional="true">
                ///    addFromPromptAsyncOptions- e.g. {promptText: "Please select data", id: "mySales"}
                /// &#10;     promptText: Greet your users with a friendly word.
                /// &#10;     asyncContext: Object keeping state for the callback
                /// &#10;     id: Identifier.
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>

                if (arguments.length == 2) { callback = options; };
                var result = new Office._AsyncResult("addFromPromptAsync", bindingType)
                callback(result);


            }
        }

    };

    Office._context_document = {
        mode: {
            annotate: {
                //Gets the document mode
                mode: undefined
            }
        },
        url: {
            annotate: {
                //Gets the document URL
                url: undefined
            }
        },
        addHandlerAsync: {
            value: function (eventType, handler, callback) {
                ///<summary> Adds an event handler for the specified event type. </summary>
                ///<param name="eventType" type="Office.EventType">The event type. For document can be 'DocumentSelectionChanged' </param>
                ///<param name="handler" type="function">The name of the handler </param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                var result = new Office._AsyncResult("addHandlerAsync");
                callback(result);
                handler(new Office._DocumentEventArgs(eventType));
            }
        },
        removeHandlerAsync: {
            value: function (eventType, handler, callback) {
                ///<summary> Removes an event handler for the specified event type. </summary>
                ///<param name="eventType" type="Office.EventType">The event type. For document can be 'DocumentSelectionChanged' </param>
                ///<param name="handler" type="function" optional="true">The name of the handler. If not specified all handlers are removed </param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                ///
                var result = new Office._AsyncResult("removeHandlerAsync", coercionType);
                callback(result);
            }
        },
        settings: {
            conditions: {
                hosts: ["word", "excel", "ppt", "accesswebapp"],
                reqs: [
                    "set Settings GE 1.1",
                    "method Settings.get",
                    "method Settings.remove",
                    "method Settings.saveAsync",
                    "method Settings.set"
                ]
            },
            value: new Office._context_document_settings()
        },
        refreshableSettings: {
            name: "settings",
            conditions: {
                hosts: ["excel", "ppt", "accesswebapp"],
                reqs: []
            },
            partialConditions: true,
            contents: {
                addHandlerAsync: {
                    conditions: { reqs: ["method Settings.addHandlerAsync"] },
                    value: function (eventType, handler, callback) {
                        ///<summary> Adds an event handler for the object using the specified event type. </summary>
                        ///<param name="eventType" type="Office.EventType">The event type. For settings can be 'settingsChanged' </param>
                        ///<param name="handler" type="function">The name of the handler </param>
                        ///<param name="callback" type="function" optional="true">The optional callback method</param>

                        var result = new Office._AsyncResult("addHandlerAsync", coercionType);
                        callback(result);

                    }
                },
                refreshAsync: {
                    conditions: { reqs: ["method Settings.refreshAsync"] },
                    value: function (callback) {
                        ///<summary>Gets the latest version of the settings object.</summary>
                        ///<param name="callback" type="function" optional="true">The optional callback method</param>
                        var result = new Office._AsyncResult("refreshAsync", coercionType);
                        callback(result);
                    }
                },
                removeHandlerAsync: {
                    conditions: { reqs: ["method Settings.removeHandlerAsync"] },
                    value: function (eventType, handler, callback) {
                        ///<summary> Removes an event handler for the specified event type. </summary>
                        ///<param name="eventType" type="Office.EventType">The event type. For settings can be 'settingsChanged' </param>
                        ///<param name="handler" type="Object" optional="true">
                        ///    Syntax example: {handler:eventHandler}
                        /// &#10;     handler: Indicates a specific handler to be removed, if not specified all handlers are removed
                        /// &#10;     asyncContext: Object keeping state for the callback
                        ///</param>
                        ///<param name="callback" type="function" optional="true">The optional callback method</param>
                        var result = new Office._AsyncResult("removeHandlerAsync", coercionType);
                        callback(result);
                    }
                }
            }
        },
        setSelectedDataAsync: {
            conditions: {
                hosts: ["word", "excel", "ppt"],
                reqs: ["set Selection GE 1.1", "method Document.setSelectedDataAsync"]
            },
            value: function (data, options, callback) {
                ///<summary> Writes the specified data into the current selection.</summary>
                ///<param name="data" type="Object">The data to be set. Either a string or value, 2d array or TableData object</param>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {coercionType:Office.CoercionType.Matrix} or {coercionType: 'matrix'}
                /// &#10;     coercionType: Explicitly sets the shape of the data object. Use Office.CoercionType or text value. If not supplied is inferred from the data type.
                /// &#10;     imageLeft: Used for image. Sets the left position of the image.
                /// &#10;     imageTop: Used for image. Sets the top position of the image.
                /// &#10;     imageWidth: Used for image. Sets the width of the image.
                /// &#10;     imageHeight: Used for image. Sets the height of the image.
                /// &#10;     asyncContext: Object keeping state for the callback
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                ///
                if (arguments.length == 2) { callback = options; };
                var result = new Office._AsyncResult("setSelectedDataAsync");
                callback(result);
            }
        },
        bindings: {
            conditions: {
                hosts: ["word", "excel", "accesswebapp"],
                reqs: [
                    "set TextBindings GE 1.1",
                    "set TableBindings GE 1.1",
                    "set MatrixBindings GE 1.1",
                    "method Bindings.addFromPromptAsync",
                    "method Bindings.addFromNamedItemAsync",
                    "method Bindings.addFromSelectionAsync",
                    "method Bindings.getAllAsync",
                    "method Bindings.getByIdAsync",
                    "method Bindings.releaseByIdAsync",
                    "method MatrixBinding.getDataAsync",
                    "method MatrixBinding.setDataAsync",
                    "method TableBinding.clearFormatsAsync",
                    "method TableBinding.setFormatsAsync",
                    "method TableBinding.setTableOptionsAsync",
                    "method TableBinding.addColumnsAsync",
                    "method TableBinding.addRowsAsync",
                    "method TableBinding.deleteAllDataValuesAsync",
                    "method TableBinding.getDataAsync",
                    "method TableBinding.setDataAsync",
                    "method TextBinding.getDataAsync",
                    "method TextBinding.setDataAsync"
                ]
            },
            value: new Office._context_document_bindings()
        },
        getFileAsync: {
            conditions: {
                hosts: ["word", "ppt","excel"],
                reqs: ["set File GE 1.1", "method Document.getFileAsync", "method File.closeAsync", "method File.getSliceAsync"]
            },
            value: function (fileType, options, callback) {
                ///<summary>(Word and PowerPoint only) Gets the entire file in slices of up to 4MB.</summary>
                ///<param name="fileType" type="Office.FileType">The format in which the file will be returned</param>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {sliceSize:1024}
                /// &#10;     sliceSize: Specifies the desired slice size (in bytes) up to 4MB. If not specified a default slice size of 4MB will be used.
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                if (arguments.length == 2) { callback = options; };
                var result = new Office._AsyncResult("getFileAsync");
                callback(result);
            }
        },
        getSelectedDataAsync: {
            conditions: {
                hosts: ["excel", "word", "project", "ppt"],
                reqs: ["set Selection GE 1.1", "method Document.getSelectedDataAsync"]
            },
            value: function (coercionType, options, callback) {
                ///<summary> Returns the current selection.</summary>
                ///<param name="coercionType" type="Office.CoercionType">The expected shape of the selection.</param>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {valueFormat: 'formatted', filterType:'all'}
                /// &#10;     valueFormat: Get data with or without format. Use Office.ValueFormat or text value.
                /// &#10;     filterType: Get the visible or all the data. Useful when filtering data. Use Office.FilterType or text value.
                /// &#10;     asyncContext: Object keeping state for the callback
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                ///
                if (arguments.length == 2) { callback = options; };
                var result = new Office._AsyncResult("getSelectedDataAsync", coercionType);
                callback(result);
            }
        },
        customXmlParts: {
            conditions: {
                hosts: ["word"],
                reqs: [
                    "set CustomXmlParts GE 1.1",
                    "method CustomXmlNode.getNodesAsync",
                    "method CustomXmlNode.getNodeValueAsync",
                    "method CustomXmlNode.getXmlAsync",
                    "method CustomXmlNode.setNodeValueAsync",
                    "method CustomXmlNode.setXmlAsync",
                    "method CustomXmlPart.addHandlerAsync",
                    "method CustomXmlPart.deleteAsync",
                    "method CustomXmlPart.getNodesAsync",
                    "method CustomXmlPart.getXmlAsync",
                    "method CustomXmlPart.removeHandlerAsync",
                    "method CustomXmlPrefixMappings.addNamespaceAsync",
                    "method CustomXmlPrefixMappings.getNamespaceAsync",
                    "method CustomXmlPrefixMappings.getPrefixAsync"
                ]
            },
            partialConditions: true,
            contents: {
                addAsync: {
                    conditions: {
                        reqs: ["method CustomXmlParts.addAsync"]
                    },
                    value: function (xml, callback) {
                        ///<summary>(Word Only) Asynchronously adds a new custom XML part to a file.</summary>
                        ///<param name="xml" type="String">The XML to add to the newly created custom XML part.</param>
                        ///<param name="callback" type="function" optional="true">A function that is invoked when the callback returns, whose only parameter is of type AsyncResult.</param>
                        var result = new Office._AsyncResult("addAsync");
                        callback(result);
                    }
                },
                getByIdAsync: {
                    conditions: {
                        reqs: ["method CustomXmlParts.getByIdAsync"]
                    },
                    value: function (id, callback) {
                        ///<summary>(Word Only) Asynchronously gets the specified custom XML part by its id.</summary>
                        ///<param name="id" type="String">The id of the custom XML part.</param>
                        ///<param name="callback" type="function" optional="true">A function that is invoked when the callback returns, whose only parameter is of type AsyncResult.</param>
                        var result = new Office._AsyncResult("XMLgetByIdAsync");
                        callback(result);
                    }
                },
                getByNamespaceAsync: {
                    conditions: {
                        reqs: ["method CustomXmlParts.getByNamespaceAsync"]
                    },
                    value: function (ns, callback) {
                        ///<summary>(Word Only) Asynchronously gets the specified custom XML part(s) by its namespace. </summary>
                        ///<param name="ns" type="String"> The namespace to search.</param>
                        ///<param name="callback" type="function" optional="true">A function that is invoked when the callback returns, whose only parameter is of type AsyncResult.</param>
                        var result = new Office._AsyncResult("getByNamespaceAsync");
                        callback(result);
                    }
                }
            }
        },
        getActiveViewAsync: {
            conditions: {
                hosts: ["ppt"],
                reqs: ["set ActiveView GE 1.1", "method Document.getActiveViewAsync"]
            },
            value: function(options, callback) {
                ///<summary>(PowerPoint only) Returns the current view of the presentation.</summary>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {asyncContext:context}
                /// &#10;     asyncContext: Object keeping state for the callback
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                Office._extractedCallback(arguments, 2, 2)(new Office._AsyncResult("getActiveViewAsync"));
            }
        },
        getFilePropertiesAsync: {
            conditions: {
                hosts: ["word", "ppt", "excel"],
                reqs: ["method Document.getFilePropertiesAsync"]
            },
            value: function(options, callback) {
                ///<summary>Gets file properties of the current document.</summary>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {asyncContext:context}
                /// &#10;     asyncContext: Object keeping state for the callback
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                Office._extractedCallback(arguments, 2, 2)(new Office._AsyncResult("getFilePropertiesAsync"));
            }
        },
        goToByIdAsync: {
            conditions: {
                hosts: ["excel", "ppt", "word"],
                reqs: ["method Document.goToByIdAsync"]
            },
            value: function(id, goToType, options, callback) {
                ///<summary>Goes to the specified object or location in the document.</summary>
                ///<param name="id" type="String or Number">The identifier of the object or location to go to.</param>
                ///<param name="goToType" type="Office.GoToType">The type of the location to go to.</param>
                ///<param name="options" type="Object" optional="true">
                ///    Syntax example: {asyncContext:context}
                /// &#10;     selectionMode: (Word only) Use Office.SelectionMode or text value.
                /// &#10;     asyncContext: Object keeping state for the callback
                ///</param>
                ///<param name="callback" type="function" optional="true">The optional callback method</param>
                Office._extractedCallback(arguments, 4, 2)(new Office._AsyncResult("goToByIdAsync"));
            }
        }
    }
}

Office._items = {
    context: {
        contents: {
            contentLanguage: {},
            displayLanguage: {},
            license: {
                contents: {
                    value: {
                        annotate: {
                            //License summary.
                            value: undefined
                        }
                    }
                }
            },
            document: {
                conditions: {
                    hosts: ["not outlook; not outlookcompose"]
                },
                annotate: {
                    // Office Document
                    document: undefined
                },
                contents: Office._context_document
            },
            officeTheme: {
                conditions: {
                    hosts: ["excel", "outlook", "powerpoint", "project", "word"]
                },
                annotate: {
                    officeTheme: undefined
                },
                contents: {
                    "bodyBackgroundColor": {},
                    "bodyForegroundColor": {},
                    "controlBackgroundColor": {},
                    "controlForegroundColor": {}
                }
            },
            touchEnabled: {},
            commerceAllowed : {},
            requirements: {
                annotate: {
                    // Checks whether a given requirement set is supported by the current host.
                    requirements: undefined
                },
                contents: {
                    isSetSupported: {
                        value: function (name, minVersion) {
                            ///<summary> Check if the specified requirement set is supported by the host Office application </summary>
                            ///<param name="name" type="String"> Set name. e.g.: "MatrixBindings" </param>
                            ///<param name="minVersion" type="Number"> The minimum required version </param>
                        }
                    },
                }
            }
        }
    },
    initialize: {
        value: function (reason) {
            ///<summary> This method is called after the Office API was loaded.</summary>
            ///<param name="reason" type="Office.InitializationReason" optional="true"> Indicates how the app was initialized</param>
        }
    },
    useShortNamespace: {
        value: function (useShortNamespace) {
            ///<summary> Indicates if  the large namespace for objects will be used or not.</summary>
            ///<param name="useShortNamespace" type="boolean"> Indicates if 'true' that the short namespace will be used</param>
        }
    },
    select: {
        conditions: {
            hosts: ["not outlook; not outlookcompose"]
        },
        value: function (expression, callback) {
            ///<summary> Returns a promise of an object described in the expression. Callback is invoked only if method fails.</summary>
            ///<param name="expression" type="string">The object to be retrieved. Example "bindings#BindingName", retrieves a binding promise for a binding named 'BindingName'</param>
            ///<param name="callback" type="function" optional="true">The optional callback method</param>
            ///
            var result = new Office._AsyncResult("select");
            callback(result);
            return (new Office._Binding());
        }
    },
    TableData: {
        conditions: {
            hosts: ["word", "excel", "accesswebapp"],
            reqs: ["set TableBindings GE 1.1"]
        },
        value: new Office._TableData()
    }
};

/*Infrastructure***************************************************************/
Office._processItem = function (target, item, key, suppressConditionCheck) {
    var conditionsFulfilled = suppressConditionCheck || Office._filterManager._checkCondition(item.conditions);
    if (!(conditionsFulfilled || item.partialConditions)) {
        return false;
    }
    suppressConditionCheck = suppressConditionCheck || conditionsFulfilled && item.partialConditions;

    if (item.setup) {
        item.setup();
    }
    if (item.metaOnly) {
        return Office._processContents(target, item.contents, suppressConditionCheck);
    }

    key = item.name || key;
    var areItemsAdded = false;

    if (item.hasOwnProperty("value")) {
        target[key] = item.value;
        areItemsAdded = true;
    } else if (typeof item.contents == "function") {
        target[key] = item.contents();
        areItemsAdded = true;
    } else {
        target[key] = target[key] || {};
        if (Office._processContents(target[key], item.contents, suppressConditionCheck) || conditionsFulfilled) {
            areItemsAdded = true;
        } else {
            delete target[key];
        }
    }
    if (item.annotate) {
        intellisense.annotate(target, item.annotate);
        areItemsAdded = true;
    }
    return areItemsAdded;
}

Office._processContents = function (target, contents, suppressConditionCheck) {
    if (typeof contents !== "object") {
        return false;
    }

    var areItemsAdded = false;
    for (var item in contents) {
        areItemsAdded = Office._processItem(target, contents[item], item, suppressConditionCheck) || areItemsAdded;
    }
    return areItemsAdded;
}

Office._filterManager = (function () {

    var filters = [];

    return {
        _checkCondition: function (condition) {
            if (!condition)
                return true;

            var answer = true;

            for (var i = 0; i < filters.length; i++) {
                var filter = filters[i];
                var conditionForThisFilter = condition[filter.identifier];
                if (conditionForThisFilter && filter.isEnabled()) {

                    var thisFiltersAnswer = false;

                    for (var j = 0; j < conditionForThisFilter.length; j++) {
                        var productTerm = conditionForThisFilter[j].split(';');

                        var thisTermsAnswer = true;
                        for (var k = 0; k < productTerm.length; k++) {
                            var singleCondition = productTerm[k].toUpperCase().trim();
                            var invert = false;
                            if (singleCondition.indexOf("NOT") != -1) {
                                invert = true;
                                singleCondition = singleCondition.slice(singleCondition.indexOf(" ")).trim();
                            }
                            var result = filter.isConditionTrue(singleCondition, invert);
                            thisTermsAnswer = thisTermsAnswer && result;
                        }

                        thisFiltersAnswer = thisFiltersAnswer || thisTermsAnswer;
                    }

                    answer = answer && thisFiltersAnswer;
                }

                if (!answer)
                    break;
            }

            return answer;
        },

        _pushFilter: function (identifier, filteringDelegate, isEnabledDelegate) {
            filters.push({
                identifier: identifier,
                isConditionTrue: filteringDelegate,
                isEnabled: isEnabledDelegate
            });
        }
    }
})();

Office._filterManager._pushFilter(
    "hosts",
    (function () {
        var nameToMaskMapping = {
            EXCEL: Office._ExcelMask,
            WORD: Office._WordMask,
            PROJECT: Office._ProjectMask,
            OUTLOOK: Office._OutlookMask,
            PPT: Office._PowerPointMask,
            OUTLOOKCOMPOSE: Office._OutlookComposeMask,
            ACCESSWEBAPP: Office._AccessWebAppMask
        }

        return function (condition, invert) {
            var result = false;
            if (nameToMaskMapping[condition] & Office._appContext) {
                result = true;
            }
            return invert ? !result : result;
        }
    })(),
    function () {
        return typeof Office._appContext === "number";
    }
);

Office._filterManager._pushFilter(
    "reqs",
    (function () {

        function checkForMethod(methodName) {
            return Office._methodContext && Office._methodContext[methodName];
        }

        function checkForSet(setDescriptor) {
            setDescriptor = setDescriptor.split(" ");
            var setName = setDescriptor[0].trim(),
                setEntry = Office._setContext && setName in Office._setContext && (Office._setContext[setName] || "1.1");

            if (!setEntry) {
                return false;
            }

            if (setDescriptor.length === 1) {
                return true;
            } else {
                var comparisonOperator = setDescriptor[1].trim(),
                    setVersion = setDescriptor[2].split("."),
                    setEntryVersion = setEntry.split("."),
                    difference = 0,
                    maxComponentCount = Math.max(setEntryVersion.length, setVersion.length);

                for (var i = 0; i < maxComponentCount; i++) {
                    var leftInt = parseInt(setEntryVersion[i], 10) || 0,
                        rightInt = parseInt(setVersion[i], 10) || 0;
                    if (leftInt === rightInt) {
                        continue;
                    }
                    difference = leftInt - rightInt;
                    break;
                }

                switch (comparisonOperator) {
                    case "EQ":
                        return difference === 0;
                    case "GT":
                        return difference > 0;
                    case "LT":
                        return difference < 0;
                    case "GE":
                        return difference >= 0;
                    case "LE":
                        return difference <= 0;
                    default:
                        return false;
                }
            }
            return false;
        }

        return function (condition, invert) {
            var result = true;
            var typeAgnosticCond = condition.slice(condition.indexOf(" ")).trim();
            if (condition.indexOf("SET") === 0) {
                result = checkForSet(typeAgnosticCond);
            } else if (condition.indexOf("METHOD") === 0) {
                result = checkForMethod(typeAgnosticCond);
            }
            return invert ? !result : result;
        }
    })(),
    function () {
        if (Office._showAll === false) {
            return true;
        }
        return false;
    }
)

Office._addEnumOnObject = function (enumName, enumObj, targetObj, conditions) {
    Office._processItem(
        targetObj,
        {
            conditions: conditions,
            value: enumObj
        },
        enumName
    );
}
/******************************************************************************/

// Setup Project
Office._processItem(Office, {
    metaOnly: true,
    conditions: {
        hosts: ["project"]
    },
    contents: {
        ProjectProjectFields: {
            value: {
                ///<field type="Number">CurrencySymbol</field>
                CurrencySymbol: 1,
                ///<field type="Number">CurrencySymbolPosition</field>
                CurrencySymbolPosition: 2,
                ///<field type="Number">DurationUnits</field>
                DurationUnits: 3,
                ///<field type="Number">GUID</field>
                GUID: 4,
                ///<field type="Number">Finish</field>
                Finish: 5,
                ///<field type="Number">Start</field>
                Start: 6,
                ///<field type="Number">ReadOnly</field>
                ReadOnly: 7,
                ///<field type="Number">VERSION</field>
                VERSION: 8,
                ///<field type="Number">WorkUnits</field>
                WorkUnits: 9,
                ///<field type="Number">ProjectServerUrl</field>
                ProjectServerUrl: 10,
                ///<field type="Number">WSSUrl</field>
                WSSUrl: 11,
                ///<field type="Number">WSSList</field>
                WSSList: 12
            }
        },
        ProjectViewTypes: {
            value: {
                ///<field type="Number">Gantt</field>
                Gantt: 1,
                ///<field type="Number">NetworkDiagram</field>
                NetworkDiagram: 2,
                ///<field type="Number">TaskDiagram</field>
                TaskDiagram: 3,
                ///<field type="Number">TaskForm</field>
                TaskForm: 4,
                ///<field type="Number">TaskSheet</field>
                TaskSheet: 5,
                ///<field type="Number">ResourceForm</field>
                ResourceForm: 6,
                ///<field type="Number">ResourceSheet</field>
                ResourceSheet: 7,
                ///<field type="Number">ResourceGraph</field>
                ResourceGraph: 8,
                ///<field type="Number">TeamPlanner</field>
                TeamPlanner: 9,
                ///<field type="Number">TaskDetails</field>
                TaskDetails: 10,
                ///<field type="Number">TaskNameForm</field>
                TaskNameForm: 11,
                ///<field type="Number">ResourceNames</field>
                ResourceNames: 12,
                ///<field type="Number">Calendar</field>
                Calendar: 13,
                ///<field type="Number">TaskUsage</field>
                TaskUsage: 14,
                ///<field type="Number">ResourceUsage</field>
                ResourceUsage: 15,
                ///<field type="Number">Timeline</field>
                Timeline: 16,
                ///<field type="Number">Drawing</field>
                Drawing: 18,
                ///<field type="Number">Resource Plan</field>
                ResourcePlan: 19
            }
        },
        ProjectTaskFields: {
            value: {
                    ///<field type="Number">ActualCost</field>
                    ActualCost: 0,
                    ///<field type="Number">ActualDuration</field>
                    ActualDuration: 1,
                    ///<field type="Number">ActualFinish</field>
                    ActualFinish: 2,
                    ///<field type="Number">ActualOvertimeCost</field>
                    ActualOvertimeCost: 3,
                    ///<field type="Number">ActualOvertimeWork</field>
                    ActualOvertimeWork: 4,
                    ///<field type="Number">ActualStart</field>
                    ActualStart: 5,
                    ///<field type="Number">ActualWork</field>
                    ActualWork: 6,
                    ///<field type="Number">Text1</field>
                    Text1: 7,
                    ///<field type="Number">Text10</field>
                    Text10: 8,
                    ///<field type="Number">Finish10</field>
                    Finish10: 9,
                    ///<field type="Number">Start10</field>
                    Start10: 10,
                    ///<field type="Number">Text11</field>
                    Text11: 11,
                    ///<field type="Number">Text12</field>
                    Text12: 12,
                    ///<field type="Number">Text13</field>
                    Text13: 13,
                    ///<field type="Number">Text14</field>
                    Text14: 14,
                    ///<field type="Number">Text15</field>
                    Text15: 15,
                    ///<field type="Number">Text16</field>
                    Text16: 16,
                    ///<field type="Number">Text17</field>
                    Text17: 17,
                    ///<field type="Number">Text18</field>
                    Text18: 18,
                    ///<field type="Number">Text19</field>
                    Text19: 19,
                    ///<field type="Number">Finish1</field>
                    Finish1: 20,
                    ///<field type="Number">Start1</field>
                    Start1: 21,
                    ///<field type="Number">Text2</field>
                    Text2: 22,
                    ///<field type="Number">Text20</field>
                    Text20: 23,
                    ///<field type="Number">Text21</field>
                    Text21: 24,
                    ///<field type="Number">Text22</field>
                    Text22: 25,
                    ///<field type="Number">Text23</field>
                    Text23: 26,
                    ///<field type="Number">Text24</field>
                    Text24: 27,
                    ///<field type="Number">Text25</field>
                    Text25: 28,
                    ///<field type="Number">Text26</field>
                    Text26: 29,
                    ///<field type="Number">Text27</field>
                    Text27: 30,
                    ///<field type="Number">Text28</field>
                    Text28: 31,
                    ///<field type="Number">Text29</field>
                    Text29: 32,
                    ///<field type="Number">Finish2</field>
                    Finish2: 33,
                    ///<field type="Number">Start2</field>
                    Start2: 34,
                    ///<field type="Number">Text3</field>
                    Text3: 35,
                    ///<field type="Number">Text30</field>
                    Text30: 36,
                    ///<field type="Number">Finish3</field>
                    Finish3: 37,
                    ///<field type="Number">Start3</field>
                    Start3: 38,
                    ///<field type="Number">Text4</field>
                    Text4: 39,
                    ///<field type="Number">Finish4</field>
                    Finish4: 40,
                    ///<field type="Number">Start4</field>
                    Start4: 41,
                    ///<field type="Number">Text5</field>
                    Text5: 42,
                    ///<field type="Number">Finish5</field>
                    Finish5: 43,
                    ///<field type="Number">Start5</field>
                    Start5: 44,
                    ///<field type="Number">Text6</field>
                    Text6: 45,
                    ///<field type="Number">Finish6</field>
                    Finish6: 46,
                    ///<field type="Number">Start6</field>
                    Start6: 47,
                    ///<field type="Number">Text7</field>
                    Text7: 48,
                    ///<field type="Number">Finish7</field>
                    Finish7: 49,
                    ///<field type="Number">Start7</field>
                    Start7: 50,
                    ///<field type="Number">Text8</field>
                    Text8: 51,
                    ///<field type="Number">Finish8</field>
                    Finish8: 52,
                    ///<field type="Number">Start8</field>
                    Start8: 53,
                    ///<field type="Number">Text9</field>
                    Text9: 54,
                    ///<field type="Number">Finish9</field>
                    Finish9: 55,
                    ///<field type="Number">Start9</field>
                    Start9: 56,
                    ///<field type="Number">Baseline10BudgetCost</field>
                    Baseline10BudgetCost: 57,
                    ///<field type="Number">Baseline10BudgetWork</field>
                    Baseline10BudgetWork: 58,
                    ///<field type="Number">Baseline10Cost</field>
                    Baseline10Cost: 59,
                    ///<field type="Number">Baseline10Duration</field>
                    Baseline10Duration: 60,
                    ///<field type="Number">Baseline10Finish</field>
                    Baseline10Finish: 61,
                    ///<field type="Number">Baseline10FixedCost</field>
                    Baseline10FixedCost: 62,
                    ///<field type="Number">Baseline10FixedCostAccrual</field>
                    Baseline10FixedCostAccrual: 63,
                    ///<field type="Number">Baseline10Start</field>
                    Baseline10Start: 64,
                    ///<field type="Number">Baseline10Work</field>
                    Baseline10Work: 65,
                    ///<field type="Number">Baseline1BudgetCost</field>
                    Baseline1BudgetCost: 66,
                    ///<field type="Number">Baseline1BudgetWork</field>
                    Baseline1BudgetWork: 67,
                    ///<field type="Number">Baseline1Cost</field>
                    Baseline1Cost: 68,
                    ///<field type="Number">Baseline1Duration</field>
                    Baseline1Duration: 69,
                    ///<field type="Number">Baseline1Finish</field>
                    Baseline1Finish: 70,
                    ///<field type="Number">Baseline1FixedCost</field>
                    Baseline1FixedCost: 71,
                    ///<field type="Number">Baseline1FixedCostAccrual</field>
                    Baseline1FixedCostAccrual: 72,
                    ///<field type="Number">Baseline1Start</field>
                    Baseline1Start: 73,
                    ///<field type="Number">Baseline1Work</field>
                    Baseline1Work: 74,
                    ///<field type="Number">Baseline2BudgetCost</field>
                    Baseline2BudgetCost: 75,
                    ///<field type="Number">Baseline2BudgetWork</field>
                    Baseline2BudgetWork: 76,
                    ///<field type="Number">Baseline2Cost</field>
                    Baseline2Cost: 77,
                    ///<field type="Number">Baseline2Duration</field>
                    Baseline2Duration: 78,
                    ///<field type="Number">Baseline2Finish</field>
                    Baseline2Finish: 79,
                    ///<field type="Number">Baseline2FixedCost</field>
                    Baseline2FixedCost: 80,
                    ///<field type="Number">Baseline2FixedCostAccrual</field>
                    Baseline2FixedCostAccrual: 81,
                    ///<field type="Number">Baseline2Start</field>
                    Baseline2Start: 82,
                    ///<field type="Number">Baseline2Work</field>
                    Baseline2Work: 83,
                    ///<field type="Number">Baseline3BudgetCost</field>
                    Baseline3BudgetCost: 84,
                    ///<field type="Number">Baseline3BudgetWork</field>
                    Baseline3BudgetWork: 85,
                    ///<field type="Number">Baseline3Cost</field>
                    Baseline3Cost: 86,
                    ///<field type="Number">Baseline3Duration</field>
                    Baseline3Duration: 87,
                    ///<field type="Number">Baseline3Finish</field>
                    Baseline3Finish: 88,
                    ///<field type="Number">Baseline3FixedCost</field>
                    Baseline3FixedCost: 89,
                    ///<field type="Number">Baseline3FixedCostAccrual</field>
                    Baseline3FixedCostAccrual: 90,
                    ///<field type="Number">Basline3Start</field>
                    Basline3Start: 91,
                    ///<field type="Number">Baseline3Work</field>
                    Baseline3Work: 92,
                    ///<field type="Number">Baseline4BudgetCost</field>
                    Baseline4BudgetCost: 93,
                    ///<field type="Number">Baseline4BudgetWork</field>
                    Baseline4BudgetWork: 94,
                    ///<field type="Number">Baseline4Cost</field>
                    Baseline4Cost: 95,
                    ///<field type="Number">Baseline4Duration</field>
                    Baseline4Duration: 96,
                    ///<field type="Number">Baseline4Finish</field>
                    Baseline4Finish: 97,
                    ///<field type="Number">Baseline4FixedCost</field>
                    Baseline4FixedCost: 98,
                    ///<field type="Number">Baseline4FixedCostAccrual</field>
                    Baseline4FixedCostAccrual: 99,
                    ///<field type="Number">Baseline4Start</field>
                    Baseline4Start: 100,
                    ///<field type="Number">Baseline4Work</field>
                    Baseline4Work: 101,
                    ///<field type="Number">Baseline5BudgetCost</field>
                    Baseline5BudgetCost: 102,
                    ///<field type="Number">Baseline5BudgetWork</field>
                    Baseline5BudgetWork: 103,
                    ///<field type="Number">Baseline5Cost</field>
                    Baseline5Cost: 104,
                    ///<field type="Number">Baseline5Duration</field>
                    Baseline5Duration: 105,
                    ///<field type="Number">Baseline5Finish</field>
                    Baseline5Finish: 106,
                    ///<field type="Number">Baseline5FixedCost</field>
                    Baseline5FixedCost: 107,
                    ///<field type="Number">Baseline5FixedCostAccrual</field>
                    Baseline5FixedCostAccrual: 108,
                    ///<field type="Number">Baseline5Start</field>
                    Baseline5Start: 109,
                    ///<field type="Number">Baseline5Work</field>
                    Baseline5Work: 110,
                    ///<field type="Number">Baseline6BudgetCost</field>
                    Baseline6BudgetCost: 111,
                    ///<field type="Number">Baseline6BudgetWork</field>
                    Baseline6BudgetWork: 112,
                    ///<field type="Number">Baseline6Cost</field>
                    Baseline6Cost: 113,
                    ///<field type="Number">Baseline6Duration</field>
                    Baseline6Duration: 114,
                    ///<field type="Number">Baseline6Finish</field>
                    Baseline6Finish: 115,
                    ///<field type="Number">Baseline6FixedCost</field>
                    Baseline6FixedCost: 116,
                    ///<field type="Number">Baseline6FixedCostAccrual</field>
                    Baseline6FixedCostAccrual: 117,
                    ///<field type="Number">Baseline6Start</field>
                    Baseline6Start: 118,
                    ///<field type="Number">Baseline6Work</field>
                    Baseline6Work: 119,
                    ///<field type="Number">Baseline7BudgetCost</field>
                    Baseline7BudgetCost: 120,
                    ///<field type="Number">Baseline7BudgetWork</field>
                    Baseline7BudgetWork: 121,
                    ///<field type="Number">Baseline7Cost</field>
                    Baseline7Cost: 122,
                    ///<field type="Number">Baseline7Duration</field>
                    Baseline7Duration: 123,
                    ///<field type="Number">Baseline7Finish</field>
                    Baseline7Finish: 124,
                    ///<field type="Number">Baseline7FixedCost</field>
                    Baseline7FixedCost: 125,
                    ///<field type="Number">Baseline7FixedCostAccrual</field>
                    Baseline7FixedCostAccrual: 126,
                    ///<field type="Number">Baseline7Start</field>
                    Baseline7Start: 127,
                    ///<field type="Number">Baseline7Work</field>
                    Baseline7Work: 128,
                    ///<field type="Number">Baseline8BudgetCost</field>
                    Baseline8BudgetCost: 129,
                    ///<field type="Number">Baseline8BudgetWork</field>
                    Baseline8BudgetWork: 130,
                    ///<field type="Number">Baseline8Cost</field>
                    Baseline8Cost: 131,
                    ///<field type="Number">Baseline8Duration</field>
                    Baseline8Duration: 132,
                    ///<field type="Number">Baseline8Finish</field>
                    Baseline8Finish: 133,
                    ///<field type="Number">Baseline8FixedCost</field>
                    Baseline8FixedCost: 134,
                    ///<field type="Number">Baseline8FixedCostAccrual</field>
                    Baseline8FixedCostAccrual: 135,
                    ///<field type="Number">Baseline8Start</field>
                    Baseline8Start: 136,
                    ///<field type="Number">Baseline8Work</field>
                    Baseline8Work: 137,
                    ///<field type="Number">Baseline9BudgetCost</field>
                    Baseline9BudgetCost: 138,
                    ///<field type="Number">Baseline9BudgetWork</field>
                    Baseline9BudgetWork: 139,
                    ///<field type="Number">Baseline9Cost</field>
                    Baseline9Cost: 140,
                    ///<field type="Number">Baseline9Duration</field>
                    Baseline9Duration: 141,
                    ///<field type="Number">Baseline9Finish</field>
                    Baseline9Finish: 142,
                    ///<field type="Number">Baseline9FixedCost</field>
                    Baseline9FixedCost: 143,
                    ///<field type="Number">Baseline9FixedCostAccrual</field>
                    Baseline9FixedCostAccrual: 144,
                    ///<field type="Number">Baseline9Start</field>
                    Baseline9Start: 145,
                    ///<field type="Number">Baseline9Work</field>
                    Baseline9Work: 146,
                    ///<field type="Number">BaselineBudgetCost</field>
                    BaselineBudgetCost: 147,
                    ///<field type="Number">BaselineBudgetWork</field>
                    BaselineBudgetWork: 148,
                    ///<field type="Number">BaselineCost</field>
                    BaselineCost: 149,
                    ///<field type="Number">BaselineDuration</field>
                    BaselineDuration: 150,
                    ///<field type="Number">BaselineFinish</field>
                    BaselineFinish: 151,
                    ///<field type="Number">BaselineFixedCost</field>
                    BaselineFixedCost: 152,
                    ///<field type="Number">BaselineFixedCostAccrual</field>
                    BaselineFixedCostAccrual: 153,
                    ///<field type="Number">BaselineStart</field>
                    BaselineStart: 154,
                    ///<field type="Number">BaselineWork</field>
                    BaselineWork: 155,
                    ///<field type="Number">BudgetCost</field>
                    BudgetCost: 156,
                    ///<field type="Number">BudgetFixedCost</field>
                    BudgetFixedCost: 157,
                    ///<field type="Number">BudgetFixedWork</field>
                    BudgetFixedWork: 158,
                    ///<field type="Number">BudgetWork</field>
                    BudgetWork: 159,
                    ///<field type="Number">TaskCalendarGUID</field>
                    TaskCalendarGUID: 160,
                    ///<field type="Number">ConstraintDate</field>
                    ConstraintDate: 161,
                    ///<field type="Number">ConstraintType</field>
                    ConstraintType: 162,
                    ///<field type="Number">Cost1</field>
                    Cost1: 163,
                    ///<field type="Number">Cost10</field>
                    Cost10: 164,
                    ///<field type="Number">Cost2</field>
                    Cost2: 165,
                    ///<field type="Number">Cost3</field>
                    Cost3: 166,
                    ///<field type="Number">Cost4</field>
                    Cost4: 167,
                    ///<field type="Number">Cost5</field>
                    Cost5: 168,
                    ///<field type="Number">Cost6</field>
                    Cost6: 169,
                    ///<field type="Number">Cost7</field>
                    Cost7: 170,
                    ///<field type="Number">Cost8</field>
                    Cost8: 171,
                    ///<field type="Number">Cost9</field>
                    Cost9: 172,
                    ///<field type="Number">Date1</field>
                    Date1: 173,
                    ///<field type="Number">Date10</field>
                    Date10: 174,
                    ///<field type="Number">Date2</field>
                    Date2: 175,
                    ///<field type="Number">Date3</field>
                    Date3: 176,
                    ///<field type="Number">Date4</field>
                    Date4: 177,
                    ///<field type="Number">Date5</field>
                    Date5: 178,
                    ///<field type="Number">Date6</field>
                    Date6: 179,
                    ///<field type="Number">Date7</field>
                    Date7: 180,
                    ///<field type="Number">Date8</field>
                    Date8: 181,
                    ///<field type="Number">Date9</field>
                    Date9: 182,
                    ///<field type="Number">Deadline</field>
                    Deadline: 183,
                    ///<field type="Number">Duration1</field>
                    Duration1: 184,
                    ///<field type="Number">Duration10</field>
                    Duration10: 185,
                    ///<field type="Number">Duration2</field>
                    Duration2: 186,
                    ///<field type="Number">Duration3</field>
                    Duration3: 187,
                    ///<field type="Number">Duration4</field>
                    Duration4: 188,
                    ///<field type="Number">Duration5</field>
                    Duration5: 189,
                    ///<field type="Number">Duration6</field>
                    Duration6: 190,
                    ///<field type="Number">Duration7</field>
                    Duration7: 191,
                    ///<field type="Number">Duration8</field>
                    Duration8: 192,
                    ///<field type="Number">Duration9</field>
                    Duration9: 193,
                    ///<field type="Number">Duration</field>
                    Duration: 194,
                    ///<field type="Number">EarnedValueMethod</field>
                    EarnedValueMethod: 195,
                    ///<field type="Number">FinishSlack</field>
                    FinishSlack: 196,
                    ///<field type="Number">FixedCost</field>
                    FixedCost: 197,
                    ///<field type="Number">FixedCostAccrual</field>
                    FixedCostAccrual: 198,
                    ///<field type="Number">Flag10</field>
                    Flag10: 199,
                    ///<field type="Number">Flag1</field>
                    Flag1: 200,
                    ///<field type="Number">Flag11</field>
                    Flag11: 201,
                    ///<field type="Number">Flag12</field>
                    Flag12: 202,
                    ///<field type="Number">Flag13</field>
                    Flag13: 203,
                    ///<field type="Number">Flag14</field>
                    Flag14: 204,
                    ///<field type="Number">Flag15</field>
                    Flag15: 205,
                    ///<field type="Number">Flag16</field>
                    Flag16: 206,
                    ///<field type="Number">Flag17</field>
                    Flag17: 207,
                    ///<field type="Number">Flag18</field>
                    Flag18: 208,
                    ///<field type="Number">Flag19</field>
                    Flag19: 209,
                    ///<field type="Number">Flag2</field>
                    Flag2: 210,
                    ///<field type="Number">Flag20</field>
                    Flag20: 211,
                    ///<field type="Number">Flag3</field>
                    Flag3: 212,
                    ///<field type="Number">Flag4</field>
                    Flag4: 213,
                    ///<field type="Number">Flag5</field>
                    Flag5: 214,
                    ///<field type="Number">Flag6</field>
                    Flag6: 215,
                    ///<field type="Number">Flag7</field>
                    Flag7: 216,
                    ///<field type="Number">Flag8</field>
                    Flag8: 217,
                    ///<field type="Number">Flag9</field>
                    Flag9: 218,
                    ///<field type="Number">FreeSlack</field>
                    FreeSlack: 219,
                    ///<field type="Number">HasRollupSubTasks</field>
                    HasRollupSubTasks: 220,
                    ///<field type="Number">ID</field>
                    ID: 221,
                    ///<field type="Number">Name</field>
                    Name: 222,
                    ///<field type="Number">Notes</field>
                    Notes: 223,
                    ///<field type="Number">Number1</field>
                    Number1: 224,
                    ///<field type="Number">Number10</field>
                    Number10: 225,
                    ///<field type="Number">Number11</field>
                    Number11: 226,
                    ///<field type="Number">Number12</field>
                    Number12: 227,
                    ///<field type="Number">Number13</field>
                    Number13: 228,
                    ///<field type="Number">Number14</field>
                    Number14: 229,
                    ///<field type="Number">Number15</field>
                    Number15: 230,
                    ///<field type="Number">Number16</field>
                    Number16: 231,
                    ///<field type="Number">Number17</field>
                    Number17: 232,
                    ///<field type="Number">Number18</field>
                    Number18: 233,
                    ///<field type="Number">Number19</field>
                    Number19: 234,
                    ///<field type="Number">Number2</field>
                    Number2: 235,
                    ///<field type="Number">Number20</field>
                    Number20: 236,
                    ///<field type="Number">Number3</field>
                    Number3: 237,
                    ///<field type="Number">Number4</field>
                    Number4: 238,
                    ///<field type="Number">Number5</field>
                    Number5: 239,
                    ///<field type="Number">Number6</field>
                    Number6: 240,
                    ///<field type="Number">Number7</field>
                    Number7: 241,
                    ///<field type="Number">Number8</field>
                    Number8: 242,
                    ///<field type="Number">Number9</field>
                    Number9: 243,
                    ///<field type="Number">ScheduledDuration</field>
                    ScheduledDuration: 244,
                    ///<field type="Number">ScheduledFinish</field>
                    ScheduledFinish: 245,
                    ///<field type="Number">ScheduledStart</field>
                    ScheduledStart: 246,
                    ///<field type="Number">OutlineLevel</field>
                    OutlineLevel: 247,
                    ///<field type="Number">OvertimeCost</field>
                    OvertimeCost: 248,
                    ///<field type="Number">OvertimeWork</field>
                    OvertimeWork: 249,
                    ///<field type="Number">PercentComplete</field>
                    PercentComplete: 250,
                    ///<field type="Number">PercentWorkComplete</field>
                    PercentWorkComplete: 251,
                    ///<field type="Number">Predecessors</field>
                    Predecessors: 252,
                    ///<field type="Number">PreleveledFinish</field>
                    PreleveledFinish: 253,
                    ///<field type="Number">PreleveledStart</field>
                    PreleveledStart: 254,
                    ///<field type="Number">Priority</field>
                    Priority: 255,
                    ///<field type="Number">Active</field>
                    Active: 256,
                    ///<field type="Number">Critical</field>
                    Critical: 257,
                    ///<field type="Number">Milestone</field>
                    Milestone: 258,
                    ///<field type="Number">Overallocated</field>
                    Overallocated: 259,
                    ///<field type="Number">IsRollup</field>
                    IsRollup: 260,
                    ///<field type="Number">Summary</field>
                    Summary: 261,
                    ///<field type="Number">RegularWork</field>
                    RegularWork: 262,
                    ///<field type="Number">RemainingCost</field>
                    RemainingCost: 263,
                    ///<field type="Number">RemainingDuration</field>
                    RemainingDuration: 264,
                    ///<field type="Number">RemainingOvertimeCost</field>
                    RemainingOvertimeCost: 265,
                    ///<field type="Number">RemainingWork</field>
                    RemainingWork: 266,
                    ///<field type="Number">ResourceNames</field>
                    ResourceNames: 267,
                    ///<field type="Number">ResourceNames</field>
                    ResourceNames: 268,
                    ///<field type="Number">Cost</field>
                    Cost: 269,
                    ///<field type="Number">Finish</field>
                    Finish: 270,
                    ///<field type="Number">Start</field>
                    Start: 271,
                    ///<field type="Number">Work</field>
                    Work: 272,
                    ///<field type="Number">StartSlack</field>
                    StartSlack: 273,
                    ///<field type="Number">Status</field>
                    Status: 274,
                    ///<field type="Number">Successors</field>
                    Successors: 275,
                    ///<field type="Number">StatusManager</field>
                    StatusManager: 276,
                    ///<field type="Number">TotalSlack</field>
                    TotalSlack: 277,
                    ///<field type="Number">TaskGUID</field>
                    TaskGUID: 278,
                    ///<field type="Number">Type</field>
                    Type: 279,
                    ///<field type="Number">WBS</field>
                    WBS: 280,
                    ///<field type="Number">WBSPREDECESSORS</field>
                    WBSPREDECESSORS: 281,
                    ///<field type="Number">WBSSUCCESSORS</field>
                    WBSSUCCESSORS: 282,
                    ///<field type="Number">WSSID</field>
                    WSSID: 283
            }
        },
        ProjectResourceFields: {
            value: {
                    ///<field type="Number">Accrual</field>
                    Accrual: 0,
                    ///<field type="Number">ActualCost</field>
                    ActualCost: 1,
                    ///<field type="Number">ActualOvertimeCost</field>
                    ActualOvertimeCost: 2,
                    ///<field type="Number">ActualOvertimeWork</field>
                    ActualOvertimeWork: 3,
                    ///<field type="Number">ActualOvertimeWorkProtected</field>
                    ActualOvertimeWorkProtected: 4,
                    ///<field type="Number">ActualWork</field>
                    ActualWork: 5,
                    ///<field type="Number">ActualWorkProtected</field>
                    ActualWorkProtected: 6,
                    ///<field type="Number">BaseCalendar</field>
                    BaseCalendar: 7,
                    ///<field type="Number">Baseline10BudgetCost</field>
                    Baseline10BudgetCost: 8,
                    ///<field type="Number">Baseline10BudgetWork</field>
                    Baseline10BudgetWork: 9,
                    ///<field type="Number">Baseline10Cost</field>
                    Baseline10Cost: 10,
                    ///<field type="Number">Baseline10Work</field>
                    Baseline10Work: 11,
                    ///<field type="Number">Baseline1BudgetCost</field>
                    Baseline1BudgetCost: 12,
                    ///<field type="Number">Baseline1BudgetWork</field>
                    Baseline1BudgetWork: 13,
                    ///<field type="Number">Baseline1Cost</field>
                    Baseline1Cost: 14,
                    ///<field type="Number">Baseline1Work</field>
                    Baseline1Work: 15,
                    ///<field type="Number">Baseline2BudgetCost</field>
                    Baseline2BudgetCost: 16,
                    ///<field type="Number">Baseline2BudgetWork</field>
                    Baseline2BudgetWork: 17,
                    ///<field type="Number">Baseline2Cost</field>
                    Baseline2Cost: 18,
                    ///<field type="Number">Baseline2Work</field>
                    Baseline2Work: 19,
                    ///<field type="Number">Baseline3BudgetCost</field>
                    Baseline3BudgetCost: 20,
                    ///<field type="Number">Baseline3BudgetWork</field>
                    Baseline3BudgetWork: 21,
                    ///<field type="Number">Baseline3Cost</field>
                    Baseline3Cost: 22,
                    ///<field type="Number">Baseline3Work</field>
                    Baseline3Work: 23,
                    ///<field type="Number">Baseline4BudgetCost</field>
                    Baseline4BudgetCost: 24,
                    ///<field type="Number">Baseline4BudgetWork</field>
                    Baseline4BudgetWork: 25,
                    ///<field type="Number">Baseline4Cost</field>
                    Baseline4Cost: 26,
                    ///<field type="Number">Baseline4Work</field>
                    Baseline4Work: 27,
                    ///<field type="Number">Baseline5BudgetCost</field>
                    Baseline5BudgetCost: 28,
                    ///<field type="Number">Baseline5BudgetWork</field>
                    Baseline5BudgetWork: 29,
                    ///<field type="Number">Baseline5Cost</field>
                    Baseline5Cost: 30,
                    ///<field type="Number">Baseline5Work</field>
                    Baseline5Work: 31,
                    ///<field type="Number">Baseline6BudgetCost</field>
                    Baseline6BudgetCost: 32,
                    ///<field type="Number">Baseline6BudgetWork</field>
                    Baseline6BudgetWork: 33,
                    ///<field type="Number">Baseline6Cost</field>
                    Baseline6Cost: 34,
                    ///<field type="Number">Baseline6Work</field>
                    Baseline6Work: 35,
                    ///<field type="Number">Baseline7BudgetCost</field>
                    Baseline7BudgetCost: 36,
                    ///<field type="Number">Baseline7BudgetWork</field>
                    Baseline7BudgetWork: 37,
                    ///<field type="Number">Baseline7Cost</field>
                    Baseline7Cost: 38,
                    ///<field type="Number">Baseline7Work</field>
                    Baseline7Work: 39,
                    ///<field type="Number">Baseline8BudgetCost</field>
                    Baseline8BudgetCost: 40,
                    ///<field type="Number">Baseline8BudgetWork</field>
                    Baseline8BudgetWork: 41,
                    ///<field type="Number">Baseline8Cost</field>
                    Baseline8Cost: 42,
                    ///<field type="Number">Baseline8Work</field>
                    Baseline8Work: 43,
                    ///<field type="Number">Baseline9BudgetCost</field>
                    Baseline9BudgetCost: 44,
                    ///<field type="Number">Baseline9BudgetWork</field>
                    Baseline9BudgetWork: 45,
                    ///<field type="Number">Baseline9Cost</field>
                    Baseline9Cost: 46,
                    ///<field type="Number">Baseline9Work</field>
                    Baseline9Work: 47,
                    ///<field type="Number">BaselineBudgetCost</field>
                    BaselineBudgetCost: 48,
                    ///<field type="Number">BaselineBudgetWork</field>
                    BaselineBudgetWork: 49,
                    ///<field type="Number">BaselineCost</field>
                    BaselineCost: 50,
                    ///<field type="Number">BaselineWork</field>
                    BaselineWork: 51,
                    ///<field type="Number">BudgetCost</field>
                    BudgetCost: 52,
                    ///<field type="Number">BudgetWork</field>
                    BudgetWork: 53,
                    ///<field type="Number">ResourceCalendarGUID</field>
                    ResourceCalendarGUID: 54,
                    ///<field type="Number">Code</field>
                    Code: 55,
                    ///<field type="Number">Cost1</field>
                    Cost1: 56,
                    ///<field type="Number">Cost10</field>
                    Cost10: 57,
                    ///<field type="Number">Cost2</field>
                    Cost2: 58,
                    ///<field type="Number">Cost3</field>
                    Cost3: 59,
                    ///<field type="Number">Cost4</field>
                    Cost4: 60,
                    ///<field type="Number">Cost5</field>
                    Cost5: 61,
                    ///<field type="Number">Cost6</field>
                    Cost6: 62,
                    ///<field type="Number">Cost7</field>
                    Cost7: 63,
                    ///<field type="Number">Cost8</field>
                    Cost8: 64,
                    ///<field type="Number">Cost9</field>
                    Cost9: 65,
                    ///<field type="Number">ResourceCreationDate</field>
                    ResourceCreationDate: 66,
                    ///<field type="Number">Date1</field>
                    Date1: 67,
                    ///<field type="Number">Date10</field>
                    Date10: 68,
                    ///<field type="Number">Date2</field>
                    Date2: 69,
                    ///<field type="Number">Date3</field>
                    Date3: 70,
                    ///<field type="Number">Date4</field>
                    Date4: 71,
                    ///<field type="Number">Date5</field>
                    Date5: 72,
                    ///<field type="Number">Date6</field>
                    Date6: 73,
                    ///<field type="Number">Date7</field>
                    Date7: 74,
                    ///<field type="Number">Date8</field>
                    Date8: 75,
                    ///<field type="Number">Date9</field>
                    Date9: 76,
                    ///<field type="Number">Duration1</field>
                    Duration1: 77,
                    ///<field type="Number">Duration10</field>
                    Duration10: 78,
                    ///<field type="Number">Duration2</field>
                    Duration2: 79,
                    ///<field type="Number">Duration3</field>
                    Duration3: 80,
                    ///<field type="Number">Duration4</field>
                    Duration4: 81,
                    ///<field type="Number">Duration5</field>
                    Duration5: 82,
                    ///<field type="Number">Duration6</field>
                    Duration6: 83,
                    ///<field type="Number">Duration7</field>
                    Duration7: 84,
                    ///<field type="Number">Duration8</field>
                    Duration8: 85,
                    ///<field type="Number">Duration9</field>
                    Duration9: 86,
                    ///<field type="Number">Email</field>
                    Email: 87,
                    ///<field type="Number">End</field>
                    End: 88,
                    ///<field type="Number">Finish1</field>
                    Finish1: 89,
                    ///<field type="Number">Finish10</field>
                    Finish10: 90,
                    ///<field type="Number">Finish2</field>
                    Finish2: 91,
                    ///<field type="Number">Finish3</field>
                    Finish3: 92,
                    ///<field type="Number">Finish4</field>
                    Finish4: 93,
                    ///<field type="Number">Finish5</field>
                    Finish5: 94,
                    ///<field type="Number">Finish6</field>
                    Finish6: 95,
                    ///<field type="Number">Finish7</field>
                    Finish7: 96,
                    ///<field type="Number">Finish8</field>
                    Finish8: 97,
                    ///<field type="Number">Finish9</field>
                    Finish9: 98,
                    ///<field type="Number">Flag10</field>
                    Flag10: 99,
                    ///<field type="Number">Flag1</field>
                    Flag1: 100,
                    ///<field type="Number">Flag11</field>
                    Flag11: 101,
                    ///<field type="Number">Flag12</field>
                    Flag12: 102,
                    ///<field type="Number">Flag13</field>
                    Flag13: 103,
                    ///<field type="Number">Flag14</field>
                    Flag14: 104,
                    ///<field type="Number">Flag15</field>
                    Flag15: 105,
                    ///<field type="Number">Flag16</field>
                    Flag16: 106,
                    ///<field type="Number">Flag17</field>
                    Flag17: 107,
                    ///<field type="Number">Flag18</field>
                    Flag18: 108,
                    ///<field type="Number">Flag19</field>
                    Flag19: 109,
                    ///<field type="Number">Flag2</field>
                    Flag2: 110,
                    ///<field type="Number">Flag20</field>
                    Flag20: 111,
                    ///<field type="Number">Flag3</field>
                    Flag3: 112,
                    ///<field type="Number">Flag4</field>
                    Flag4: 113,
                    ///<field type="Number">Flag5</field>
                    Flag5: 114,
                    ///<field type="Number">Flag6</field>
                    Flag6: 115,
                    ///<field type="Number">Flag7</field>
                    Flag7: 116,
                    ///<field type="Number">Flag8</field>
                    Flag8: 117,
                    ///<field type="Number">Flag9</field>
                    Flag9: 118,
                    ///<field type="Number">Group</field>
                    Group: 119,
                    ///<field type="Number">Units</field>
                    Units: 120,
                    ///<field type="Number">Name</field>
                    Name: 121,
                    ///<field type="Number">Notes</field>
                    Notes: 122,
                    ///<field type="Number">Number1</field>
                    Number1: 123,
                    ///<field type="Number">Number10</field>
                    Number10: 124,
                    ///<field type="Number">Number11</field>
                    Number11: 125,
                    ///<field type="Number">Number12</field>
                    Number12: 126,
                    ///<field type="Number">Number13</field>
                    Number13: 127,
                    ///<field type="Number">Number14</field>
                    Number14: 128,
                    ///<field type="Number">Number15</field>
                    Number15: 129,
                    ///<field type="Number">Number16</field>
                    Number16: 130,
                    ///<field type="Number">Number17</field>
                    Number17: 131,
                    ///<field type="Number">Number18</field>
                    Number18: 132,
                    ///<field type="Number">Number19</field>
                    Number19: 133,
                    ///<field type="Number">Number2</field>
                    Number2: 134,
                    ///<field type="Number">Number20</field>
                    Number20: 135,
                    ///<field type="Number">Number3</field>
                    Number3: 136,
                    ///<field type="Number">Number4</field>
                    Number4: 137,
                    ///<field type="Number">Number5</field>
                    Number5: 138,
                    ///<field type="Number">Number6</field>
                    Number6: 139,
                    ///<field type="Number">Number7</field>
                    Number7: 140,
                    ///<field type="Number">Number8</field>
                    Number8: 141,
                    ///<field type="Number">Number9</field>
                    Number9: 142,
                    ///<field type="Number">OvertimeCost</field>
                    OvertimeCost: 143,
                    ///<field type="Number">OvertimeRate</field>
                    OvertimeRate: 144,
                    ///<field type="Number">OvertimeWork</field>
                    OvertimeWork: 145,
                    ///<field type="Number">PercentWorkComplete</field>
                    PercentWorkComplete: 146,
                    ///<field type="Number">CostPerUse</field>
                    CostPerUse: 147,
                    ///<field type="Number">Generic</field>
                    Generic: 148,
                    ///<field type="Number">OverAllocated</field>
                    OverAllocated: 149,
                    ///<field type="Number">RegularWork</field>
                    RegularWork: 150,
                    ///<field type="Number">RemainingCost</field>
                    RemainingCost: 151,
                    ///<field type="Number">RemainingOvertimeCost</field>
                    RemainingOvertimeCost: 152,
                    ///<field type="Number">RemainingOvertimeWork</field>
                    RemainingOvertimeWork: 153,
                    ///<field type="Number">RemainingWork</field>
                    RemainingWork: 154,
                    ///<field type="Number">ResourceGUID</field>
                    ResourceGUID: 155,
                    ///<field type="Number">Cost</field>
                    Cost: 156,
                    ///<field type="Number">Work</field>
                    Work: 157,
                    ///<field type="Number">Start</field>
                    Start: 158,
                    ///<field type="Number">Start1</field>
                    Start1: 159,
                    ///<field type="Number">Start10</field>
                    Start10: 160,
                    ///<field type="Number">Start2</field>
                    Start2: 161,
                    ///<field type="Number">Start3</field>
                    Start3: 162,
                    ///<field type="Number">Start4</field>
                    Start4: 163,
                    ///<field type="Number">Start5</field>
                    Start5: 164,
                    ///<field type="Number">Start6</field>
                    Start6: 165,
                    ///<field type="Number">Start7</field>
                    Start7: 166,
                    ///<field type="Number">Start8</field>
                    Start8: 167,
                    ///<field type="Number">Start9</field>
                    Start9: 168,
                    ///<field type="Number">StandardRate</field>
                    StandardRate: 169,
                    ///<field type="Number">Text1</field>
                    Text1: 170,
                    ///<field type="Number">Text10</field>
                    Text10: 171,
                    ///<field type="Number">Text11</field>
                    Text11: 172,
                    ///<field type="Number">Text12</field>
                    Text12: 173,
                    ///<field type="Number">Text13</field>
                    Text13: 174,
                    ///<field type="Number">Text14</field>
                    Text14: 175,
                    ///<field type="Number">Text15</field>
                    Text15: 176,
                    ///<field type="Number">Text16</field>
                    Text16: 177,
                    ///<field type="Number">Text17</field>
                    Text17: 178,
                    ///<field type="Number">Text18</field>
                    Text18: 179,
                    ///<field type="Number">Text19</field>
                    Text19: 180,
                    ///<field type="Number">Text2</field>
                    Text2: 181,
                    ///<field type="Number">Text20</field>
                    Text20: 182,
                    ///<field type="Number">Text21</field>
                    Text21: 183,
                    ///<field type="Number">Text22</field>
                    Text22: 184,
                    ///<field type="Number">Text23</field>
                    Text23: 185,
                    ///<field type="Number">Text24</field>
                    Text24: 186,
                    ///<field type="Number">Text25</field>
                    Text25: 187,
                    ///<field type="Number">Text26</field>
                    Text26: 188,
                    ///<field type="Number">Text27</field>
                    Text27: 189,
                    ///<field type="Number">Text28</field>
                    Text28: 190,
                    ///<field type="Number">Text29</field>
                    Text29: 191,
                    ///<field type="Number">Text3</field>
                    Text3: 192,
                    ///<field type="Number">Text30</field>
                    Text30: 193,
                    ///<field type="Number">Text4</field>
                    Text4: 194,
                    ///<field type="Number">Text5</field>
                    Text5: 195,
                    ///<field type="Number">Text6</field>
                    Text6: 196,
                    ///<field type="Number">Text7</field>
                    Text7: 197,
                    ///<field type="Number">Text8</field>
                    Text8: 198,
                    ///<field type="Number">Text9</field>
                    Text9: 199
            }
        },
        context: {
            contents: {
                document: {
                    contents: {
                        getSelectedTaskAsync: {
                            conditions: { reqs: ["method Document.getSelectedTaskAsync"] },
                            value: function (callback) {
                                    ///<summary> (Project only) Get the current selected Task's Id.</summary>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getTaskByIndexAsync: {
                            conditions: { reqs: ["method Document.getTaskByIndexAsync"] },
                            value: function (taskIndex, callback) {
                                    ///<summary> (Project only) Get the Task's Id for provided task index.</summary>
                                    ///<param name="taskIndex" type="Object">Task index in task container</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getTaskAsync: {
                            conditions: { reqs: ["method Document.getTaskAsync"] },
                            value: function (taskId, callback) {
                                    ///<summary> (Project only) Get the Task Name, WSS Task Id, and ResourceNames for given taskId .</summary>
                                    ///<param name="taskId" type="Object">Either a string or value of the Task Id.</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getTaskFieldAsync: {
                            conditions: { reqs: ["method Document.getTaskFieldAsync"] },
                            value: function (taskId, taskField, callback) {
                                    ///<summary> (Project only) Get task field for provided task Id. (Ex. StartDate).</summary>
                                    ///<param name="taskId" type="Object">Either a string or value of the Task Id.</param>
                                    ///<param name="taskField" type="Office.ProjectTaskFields">Task Fields.</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>

                            }
                        },
                        getSelectedResourceAsync: {
                            conditions: { reqs: ["method Document.getSelectedResourceAsync"] },
                            value: function (callback) {
                                    ///<summary> (Project only) Get the current selected Resource's Id.</summary>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getResourceByIndexAsync: {
                            conditions: { reqs: ["method Document.getResourceByIndexAsync"] },
                            value: function (resourceIndex, callback) {
                                    ///<summary> (Project only) Get the Resource's Id for provided resource index.</summary>
                                    ///<param name="resourceIndex" type="Object">Resource index in resource container</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getResourceFieldAsync: {
                            conditions: { reqs: ["method Document.getResourceFieldAsync"] },
                            value: function (resourceId, resourceField, callback) {
                                    ///<summary> (Project only) Get resource field for provided resource Id. (Ex.ResourceName)</summary>
                                    ///<param name="resourceId" type="Object">Either a string or value of the Resource Id.</param>
                                    ///<param name="resourceField" type="Office.ProjectResourceFields">Resource Fields.</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getProjectFieldAsync: {
                            conditions: { reqs: ["method Document.getProjectFieldAsync"] },
                            value: function (projectField, callback) {
                                    ///<summary> (Project only) Get Project field (Ex. ProjectWebAccessURL).</summary>
                                    ///<param name="projectField" type="Office.ProjectProjectFields">Project level fields.</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getSelectedViewAsync: {
                            conditions: { reqs: ["method Document.getSelectedViewAsync"] },
                            value: function (callback) {
                                    ///<summary> (Project only) Get the current selected View Type (Ex. Gantt) and View Name.</summary>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        getWSSURLAsync: {
                            conditions: { reqs: ["method Document.getWSSURLAsync"] },
                            value: function (callback) {
                                    ///<summary> (Project only) Get the WSS Url and list name for the Tasks List, the MPP is synced too.</summary>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        setTaskFieldAsync: {
                            conditions: { reqs: ["method Document.setTaskFieldAsync"] },
                            value: function (taskId, fieldId, fieldValue, callback) {
                                    ///<summary> (Project only) Set Taskfield (Ex. TaskName).</summary>
                                    ///<param name="taskId" type="Object">Either a string or value of the Task Id.</param>
                                    ///<param name="taskField" type="Office.ProjectTaskFields">Task Field.</param>
                                    ///<param name="fieldValue" type="Object">Either a string, number boolean or object for the data that you want to set.</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        },
                        setResourceFieldAsync: {
                            conditions: { reqs: ["method Document.setResourceFieldAsync"] },
                            value: function (resourceId, fieldId, fieldValue, callback) {
                                    ///<summary> (Project only) Set Resource field (Ex. ResourceName).</summary>
                                    ///<param name="resourceId" type="Object">Either a string or value of the Resource Id.</param>
                                    ///<param name="resourceField" type="Office.ProjectResourceFields">Resource Field.</param>
                                    ///<param name="fieldValue" type="Object">Either a string, number boolean or object for the data that you want to set.</param>
                                    ///<param name="callback" type="function" optional="true">The optional callback method</param>
                            }
                        }
                    }
                }
            }
        }
    }
});

// Setup outlook
Office._processItem(Office, {
    metaOnly: true,
    conditions: {
        hosts: ["outlook", "outlookcompose"],
        reqs: ["set Mailbox GE 1.1"]
    },
    contents: {
        MailboxEnums: {
            value: new Office._MailboxEnums()
        },
        context: {
            contents: {
                mailbox: {
                    value: new Office._context_mailbox()
                },
                roamingSettings: {
                    value: new Office._settings()
                }
            }
        },
        cast: {
            value: {
                item: new Office._cast_item()
            }
        }
    }
})

// Setup CustomXMLParts
Office._addEnumOnObject("CustomXmlNodeType",
    {
        ///<field type="String">Element</field>
        Element: "element",
        ///<field type="String">Attribute</field>
        Attribute: "attribute",
        ///<field type="String">String/field>
        Text: "text",
        ///<field type="String">CData</field>
        Cdata: "cdata",
        ///<field type="String">ProcessingInstruction</field>
        ProcessingInstruction: "processingInstruction",
        ///<field type="String">NodeComment</field>
        NodeComment: "nodeComment",
        ///<field type="String">NodeDocument</field>
        NodeDocument: "nodeDocument"
    },
    Office,
    {
        hosts: ["word"]
    }
);

// Other enumerations on Office
Office._addEnumOnObject("AsyncResultStatus",
    {
        ///<field type="String">Operation failed, check error object</field>
        Failed: "failed",
        ///<field type="String">Operation succeeded</field>
        Succeeded: "succeeded"

    },
    Office,
    {
        hosts: ["not outlook; not outlookcompose"]
    }
);

Office._processItem(Office,
    {
        contents: {
            Text: {
                conditions: {
                    hosts: ["excel", "word"],
                    reqs: ["set TextBindings GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Text based Binding</field>
                    Text: undefined
                },
                value: "text"
            },
            Matrix: {
                conditions: {
                    hosts: ["excel", "word"],
                    reqs: ["set MatrixBindings GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Matrix based Binding</field>
                    Matrix: undefined
                },
                value: "matrix"
            },
            Table: {
                conditions: {
                    hosts: ["excel", "word", "accesswebapp"],
                    reqs: ["set TableBindings GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Table based Binding</field>
                    Table: undefined
                },
                value: "table"
            }
        }
    },
    "BindingType"
);

Office._processItem(Office,
    {
        contents: {
            Table: {
                conditions: {
                    hosts: ["word", "excel", "accesswebapp"],
                    reqs: ["set TableCoercion GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Coerce as Table</field>
                    Table: undefined
                },
                value: "table"
            },
            Text: {
                conditions: {
                    hosts: ["excel", "ppt", "project", "word"],
                    reqs: ["set TextCoercion GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Coerce as Text</field>
                    Text: undefined
                },
                value: "text"
            },
            Matrix: {
                conditions: {
                    hosts: ["excel", "word"],
                    reqs: ["set MatrixCoercion GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Coerce as Matrix</field>
                    Matrix: undefined
                },
                value: "matrix"
            },
            Html: {
                conditions: {
                    hosts: ["word"],
                    reqs: ["set HtmlCoercion GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Coerce as HTML</field>
                    Html: undefined
                },
                value: "html"
            },
            Ooxml: {
                conditions: {
                    hosts: ["word"],
                    reqs: ["set OoxmlCoercion GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Coerce as Office Open XML</field>
                    Ooxml: undefined
                },
                value: "ooxml"
            },
            SlideRange: {
                conditions: {
                    hosts: ["ppt"]
                },
                annotate: {
                    ///<field type="String">Coerce as JSON object containing an array of the ids, titles, and indexes of the selected slides.</field>
                    SlideRange: undefined
                },
                value: "slideRange"
            }
        }
    },
    "CoercionType"
);

Office._addEnumOnObject("DocumentMode",
    {
        ///<field type="String">Document in Read Only Mode</field>
        ReadOnly: "readOnly",
        ///<field type="String">Document in Read/Write Mode</field>
        ReadWrite: "readWrite"
    },
    Office,
    {
        hosts: ["word", "excel", "accesswebapp"]
    }
);

Office._addEnumOnObject("EventType",
    {
        ///<field type="String">Triggers when a document level selection happens</field>
        DocumentSelectionChanged: "documentSelectionChanged",
        ///<field type="String"> Triggers when a binding level selection happens</field>
        BindingSelectionChanged: "bindingSelectionChanged",
        ///<field type="String">Triggers when a binding level data change happens</field>
        BindingDataChanged: "bindingDataChanged",
        ///<field type="String">Triggers when settings change in a co-Auth session.</field>
        SettingsChanged: "settingsChanged",
        ///<field type="String">Triggers when a customXmlPart node was deleted</field>
        NodeDeleted: "nodeDeleted",
        ///<field type="String">Triggers when a customXmlPart node was inserted</field>
        NodeInserted: "nodeInserted",
        ///<field type="String">Triggers when a customXmlPart node was replaced</field>
        NodeReplaced: "nodeReplaced",
        ///<field type="String">Triggers when a Task selection happens in Project.</field>
        TaskSelectionChanged: "taskSelectionChanged",
        ///<field type="String"> Triggers when a Resource selection happens in Project.</field>
        ResourceSelectionChanged: "resourceSelectionChanged",
        ///<field type="String">Triggers when a View selection happens in Project.</field>
        ViewSelectionChanged: "viewSelectionChanged"
    },
    Office,
    {
        hosts: ["not outlook; not outlookcompose"]
    }
);
// EventType augmentations
Office._processContents(Office.EventType, {
    ActiveViewChanged: {
        conditions: {
            hosts: ["ppt"]
        },
        annotate: {
            ///<field type="String">Occurs when the user changes the current view of the document.</field>
            ActiveViewChanged: undefined
        },
        value: "activeViewChanged"
    }
});

Office._processItem(Office,
    {
        conditions: { hosts: ["not outlook; not outlookcompose; not accesswebapp"] },
        contents: {
            Compressed: {
                conditions: {
                    hosts: ["ppt", "word"],
                    reqs: ["set CompressedFile GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Returns the file as a byte array </field>
                    Compressed: undefined
                },
                value: "compressed"
            },
            Pdf: {
                conditions: {
                    hosts: ["ppt", "word"],
                    reqs: ["set PdfFile GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Returns the file in PDF format as a byte array</field>
                    Pdf: undefined
                },
                value: "pdf"
            },
            Text: {
                conditions: {
                    hosts: ["word"],
                    reqs: ["set TextFile GE 1.1"]
                },
                annotate: {
                    ///<field type="String">Returns the file as plain text</field>
                    Text: undefined
                },
                value: "text"
            }
        }
    },
    "FileType"
);

Office._addEnumOnObject("FilterType",
    {
        ///<field type="String">Returns all items</field>
        All: "all",
        ///<field type="String">Returns only visible items</field>
        OnlyVisible: "onlyVisible"
    },
    Office,
    {
        hosts: ["not outlook; not outlookcompose; not accesswebapp"]
    }
);

Office._addEnumOnObject("InitializationReason",
    {
        ///<field type="String">Indicates the app was just inserted in the document /field>
        Inserted: "inserted",
        ///<field type="String">Indicated if the extension already existed in the document</field>
        DocumentOpened: "documentOpened"
    },
    Office,
    {
        hosts: ["not outlook; not outlookcompose"]
    }
);

Office._addEnumOnObject("ValueFormat",
    {
        ///<field type="String">Returns items with format</field>
        Formatted: "formatted",
        ///<field type="String">Returns items without format</field>
        Unformatted: "unformatted"
    },
    Office,
    {
        hosts: ["not outlook; not outlookcompose"]
    }
);

Office._processContents(Office, {
    GoToType: {
        contents: {
            Binding: {
                conditions: {
                    hosts: ["excel", "word"]
                },
                annotate: {
                    ///<field type="String">Goes to a binding object using the specified binding id.</field>
                    Binding: undefined
                },
                value: "binding"
            },
            NamedItem: {
                conditions: {
                    hosts: ["excel"]
                },
                annotate: {
                    /// <field type="String">
                    /// Goes to a named item using that item's name.
                    /// &#10; In Excel, you can use any structured reference for a named range or table: "Worksheet2!Table1"
                    /// </field>
                    NamedItem: undefined
                },
                value: "namedItem"
            },
            Slide: {
                conditions: {
                    hosts: ["ppt"]
                },
                annotate: {
                    ///<field type="String">Goes to a slide using the specified id.</field>
                    Slide: undefined
                },
                value: "slide"
            },
            Index: {
                conditions: {
                    hosts: ["ppt"]
                },
                annotate: {
                    ///<field type="String">Goes to the specified index by slide number or enum Office.Index</field>
                    Index: undefined
                },
                value: "index"
            }
        }
    }
});

Office._addEnumOnObject("Index",
    {
        First: "first",
        Last: "last",
        Next: "next",
        Previous: "previous"
    },
    Office,
    {
        hosts: ["ppt"]
    }
);

Office._addEnumOnObject("SelectionMode",
    {
        Default: "default",
        Selected: "selected",
        None: "none"
    },
    Office,
    {
        hosts: ["word"]
    }
);

if (!!intellisense) {
    intellisense.addEventListener('statementcompletion', function (event) {
        if (event.targetName === "this" || event.target === undefined || event.target === window) return;
        event.items = event.items.filter(function (item) {
            return !(item.name && item.name.charAt(0) === "_");
        });
    });
}

Office._processContents(Office, Office._items);

document.addEventListener("DOMContentLoaded", function () {
    Office.initialize();
});

var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};

var OfficeExtension;
(function (OfficeExtension) {
    var ClientObject = (function () {
        function ClientObject() {
            /// <summary>
            /// An abstract proxy object that represents an object in an Office document. You create proxy objects from the context (or from other proxy objects), add commands to a queue to act on the object, and then synchronize the proxy object state with the document by calling "context.sync()".
            /// </summary>
            /// <field name="context" type="OfficeExtension.ClientRequestContext"></field>
        }
        return ClientObject;
    })();
    OfficeExtension.ClientObject = ClientObject;
})(OfficeExtension || (OfficeExtension = {}));

var OfficeExtension;
(function (OfficeExtension) {
    var ClientRequestContext = (function () {
        function ClientRequestContext(url) {
            /// <summary>
            /// An abstract RequestContext object that facilitates requests to the host Office application. The "Excel.run" and "Word.run" methods provide a request context.
            /// </summary>
            /// <field name="trackedObjects" type="OfficeExtension.TrackedObjects"> Collection of objects that are tracked for automatic adjustments based on surrounding changes in the document. </field>
        }
        ClientRequestContext.prototype.load = function (object, option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="object" type="OfficeExtension.ClientObject" />
            /// <param name="option" type="string|string[]|{select?, expand?, top?, skip?}" />
        };
        ClientRequestContext.prototype.trace = function (message) {
            /// <summary>
            /// Adds a trace message to the queue. If the promise returned by "context.sync()" is rejected due to an error, this adds a ".traceMessages" array to the OfficeExtension.Error object, containing all trace messages that were executed. These messages can help you monitor the program execution sequence and detect the cause of the error.
            /// </summary>
            /// <param name="message" type="String" />
        };
        ClientRequestContext.prototype.sync = function (passThroughValue) {
            /// <summary>
            /// Synchronizes the state between JavaScript proxy objects and the Office document, by executing instructions queued on the request context and retrieving properties of loaded Office objects for use in your code.ÿThis method returns a promise, which is resolved when the synchronization is complete.
            /// </summary>
            /// <param name="passThroughValue" optional="true" />
            return new OfficeExtension.IPromise();
        };
        ClientRequestContext.prototype.__proto__ = null;
        return ClientRequestContext;
    })();
    OfficeExtension.ClientRequestContext = ClientRequestContext;
})(OfficeExtension || (OfficeExtension = {}));

var OfficeExtension;
(function (OfficeExtension) {
    var ClientResult = (function () {
        function ClientResult() {
            /// <summary>
            /// Contains the result for methods that return primitive types. The object's value property is retrieved from the document after "context.sync()" is invoked.
            /// </summary>
            /// <field name="value">
            /// The value of the result that is retrieved from the document after "context.sync()" is invoked.
            /// </field>
        }
        ClientResult.prototype.__proto__ = null;
        return ClientResult;
    })();
    OfficeExtension.ClientResult = ClientResult;
})(OfficeExtension || (OfficeExtension = {}));

var OfficeExtension;
(function (OfficeExtension) {
    var Error = (function () {
        function Error() {
            /// <summary>
            /// The error object returned by "context.sync()", if a promise is rejected due to an error while processing the request.
            /// </summary>
            /// <field name="name" type="String">
            /// Error name: "OfficeExtension.Error"
            /// </field>
            /// <field name="message" type="String">
            /// The error message passed through from the host Office application.
            /// </field>
            /// <field name="stack" type="String">
            /// Stack trace, if applicable.
            /// </field>
            /// <field name="code" type="String">
            /// Error code string, such as "InvalidArgument".
            /// </field>
            /// <field name="traceMessages" type="Array" elementType="string">
            /// Trace messages (if any) that were added via a "context.trace()" invocation before calling "context.sync()". If there was an error, this contains all trace messages that were executed before the error occurred. These messages can help you monitor the program execution sequence and detect the case of the error.
            /// </field>
            /// <field name="debugInfo">
            /// Debug info, if applicable. The ".errorLocation" property can describe the object and method or property that caused the error.
            /// </field>
            this.debugInfo = {
                __proto__: null,
                /// <field name="errorLocation" type="string" optional="true">
                /// If applicable, will return the object type and the name of the method or property that caused the error.
                /// </field>
                errorLocation: ""
            };
        }
        Error.prototype.__proto__ = null;
        return Error;
    })();
    OfficeExtension.Error = Error;
})(OfficeExtension || (OfficeExtension = {}));

var OfficeExtension;
(function (OfficeExtension) {
    var ErrorCodes = (function () {
        function ErrorCodes() {
        }
        ErrorCodes.__proto__ = null;
        ErrorCodes.accessDenied = "";
        ErrorCodes.generalException = "";
        ErrorCodes.activityLimitReached = "";
        return ErrorCodes;
    })();
})(OfficeExtension || (OfficeExtension = {}));

var OfficeExtension;
(function (OfficeExtension) {
    var IPromise = (function () {
        /// <summary>
        /// A Promise object that represents a deferred interaction with the host Office application. Promises can be chained via ".then", and errors can be caught via ".catch".  Remember to always use a ".catch" on the outer promise, and to return intermediary promises so as not to break the promise chain.
        /// </summary>
        IPromise.prototype.then = function (onFulfilled, onRejected) {
            /// <summary>
            /// This method will be called once the previous promise has been resolved.
            /// Both the onFulfilled on onRejected callbacks are optional.
            /// If either or both are omitted, the next onFulfilled/onRejected in the chain will be called called.
            /// Returns a new promise for the value or error that was returned from onFulfilled/onRejected.
            /// </summary>
            /// <param name="onFulfilled" type="Function" optional="true"></param>
            /// <param name="onRejected" type="Function" optional="true"></param>
            /// <returns type="OfficeExtension.IPromise"></returns>
            onRejected(new Error());
        }
        IPromise.prototype.catch = function (onRejected) {
            /// <summary>
            /// Catches failures or exceptions from actions within the promise, or from an unhandled exception earlier in the call stack.
            /// </summary>
            /// <param name="onRejected" type="Function" optional="true">function to be called if or when the promise rejects.</param>
            /// <returns type="OfficeExtension.IPromise"></returns>
            onRejected(new Error());
        }
        IPromise.prototype.__proto__ = null;
    })
    OfficeExtension.IPromise = IPromise;
})(OfficeExtension || (OfficeExtension = {}));

var OfficeExtension;
(function (OfficeExtension) {
    var TrackedObjects = (function () {
        function TrackedObjects() {
            /// <summary>
            /// Collection of tracked objects, contained within a request context. See "context.trackedObjects" for more information.
            /// </summary>
        }
        TrackedObjects.prototype.add = function (object) {
            /// <summary>
            /// Track a new object for automatic adjustment based on surrounding changes in the document. Only some object types require this. If you are using an object across ".sync" calls and outside the sequential execution of a ".run" batch, and get an "InvalidObjectPath" error when setting a property or invoking a method on the object, you needed to have added the object to the tracked object collection when the object was first created.
            /// </summary>
            /// <param name="object" type="OfficeExtension.ClientObject|OfficeExtension.ClientObject[]"></param>
        };
        TrackedObjects.prototype.remove = function (object) {
            /// <summary>
            /// Release the memory associated with an object that was previously added to this collection. Having many tracked objects slows down the host application, so please remember to free any objects you add, once you're done using them. You will need to call "context.sync()" before the memory release takes effect.
            /// </summary>
            /// <param name="object" type="OfficeExtension.ClientObject|OfficeExtension.ClientObject[]"></param>
        };
        TrackedObjects.prototype.__proto__ = null;
        return TrackedObjects;
    })();
    OfficeExtension.TrackedObjects = TrackedObjects;
})(OfficeExtension || (OfficeExtension = {}));

OfficeExtension.__proto__ = null;



var Excel;
(function (Excel) {
    var Application = (function(_super) {
        __extends(Application, _super);
        function Application() {
            /// <summary> Represents the Excel application that manages the workbook. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="calculationMode" type="String">Returns the calculation mode used in the workbook. See Excel.CalculationMode for details. Read-only.</field>
        }

        Application.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.Application"/>
        }
        Application.prototype.calculate = function(calculationType) {
            /// <summary>
            /// Recalculate all currently opened workbooks in Excel.
            /// </summary>
            /// <param name="calculationType" type="String">Specifies the calculation type to use. See Excel.CalculationType for details.</param>
            /// <returns ></returns>
        }
        return Application;
    })(OfficeExtension.ClientObject);
    Excel.Application = Application;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var Binding = (function(_super) {
        __extends(Binding, _super);
        function Binding() {
            /// <summary> Represents an Office.js binding that is defined in the workbook. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="id" type="String">Represents binding identifier. Read-only.</field>
            /// <field name="type" type="String">Returns the type of the binding. See Excel.BindingType for details. Read-only.</field>
        }

        Binding.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.Binding"/>
        }
        Binding.prototype.getRange = function() {
            /// <summary>
            /// Returns the range represented by the binding. Will throw an error if binding is not of the correct type.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Binding.prototype.getTable = function() {
            /// <summary>
            /// Returns the table represented by the binding. Will throw an error if binding is not of the correct type.
            /// </summary>
            /// <returns type="Excel.Table"></returns>
        }
        Binding.prototype.getText = function() {
            /// <summary>
            /// Returns the text represented by the binding. Will throw an error if binding is not of the correct type.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        return Binding;
    })(OfficeExtension.ClientObject);
    Excel.Binding = Binding;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var BindingCollection = (function(_super) {
        __extends(BindingCollection, _super);
        function BindingCollection() {
            /// <summary> Represents the collection of all the binding objects that are part of the workbook. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Returns the number of bindings in the collection. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.Binding">Gets the loaded child items in this collection.</field>
        }

        BindingCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.BindingCollection"/>
        }
        BindingCollection.prototype.getItem = function(id) {
            /// <summary>
            /// Gets a binding object by ID.
            /// </summary>
            /// <param name="id" type="String">Id of the binding object to be retrieved.</param>
            /// <returns type="Excel.Binding"></returns>
        }
        BindingCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Gets a binding object based on its position in the items array.
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.Binding"></returns>
        }
        return BindingCollection;
    })(OfficeExtension.ClientObject);
    Excel.BindingCollection = BindingCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var BindingType = {
        __proto__: null,
        "range": "range",
        "table": "table",
        "text": "text",
    }
    Excel.BindingType = BindingType;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var BorderIndex = {
        __proto__: null,
        "edgeTop": "edgeTop",
        "edgeBottom": "edgeBottom",
        "edgeLeft": "edgeLeft",
        "edgeRight": "edgeRight",
        "insideVertical": "insideVertical",
        "insideHorizontal": "insideHorizontal",
        "diagonalDown": "diagonalDown",
        "diagonalUp": "diagonalUp",
    }
    Excel.BorderIndex = BorderIndex;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var BorderLineStyle = {
        __proto__: null,
        "none": "none",
        "continuous": "continuous",
        "dash": "dash",
        "dashDot": "dashDot",
        "dashDotDot": "dashDotDot",
        "dot": "dot",
        "double": "double",
        "slantDashDot": "slantDashDot",
    }
    Excel.BorderLineStyle = BorderLineStyle;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var BorderWeight = {
        __proto__: null,
        "hairline": "hairline",
        "thin": "thin",
        "medium": "medium",
        "thick": "thick",
    }
    Excel.BorderWeight = BorderWeight;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var CalculationMode = {
        __proto__: null,
        "automatic": "automatic",
        "automaticExceptTables": "automaticExceptTables",
        "manual": "manual",
    }
    Excel.CalculationMode = CalculationMode;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var CalculationType = {
        __proto__: null,
        "recalculate": "recalculate",
        "full": "full",
        "fullRebuild": "fullRebuild",
    }
    Excel.CalculationType = CalculationType;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var Chart = (function(_super) {
        __extends(Chart, _super);
        function Chart() {
            /// <summary> Represents a chart object in a workbook. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="axes" type="Excel.ChartAxes">Represents chart axes. Read-only.</field>
            /// <field name="dataLabels" type="Excel.ChartDataLabels">Represents the datalabels on the chart. Read-only.</field>
            /// <field name="format" type="Excel.ChartAreaFormat">Encapsulates the format properties for the chart area. Read-only.</field>
            /// <field name="height" type="Number">Represents the height, in points, of the chart object.</field>
            /// <field name="left" type="Number">The distance, in points, from the left side of the chart to the worksheet origin.</field>
            /// <field name="legend" type="Excel.ChartLegend">Represents the legend for the chart. Read-only.</field>
            /// <field name="name" type="String">Represents the name of a chart object.</field>
            /// <field name="series" type="Excel.ChartSeriesCollection">Represents either a single series or collection of series in the chart. Read-only.</field>
            /// <field name="title" type="Excel.ChartTitle">Represents the title of the specified chart, including the text, visibility, position and formating of the title. Read-only.</field>
            /// <field name="top" type="Number">Represents the distance, in points, from the top edge of the object to the top of row 1 (on a worksheet) or the top of the chart area (on a chart).</field>
            /// <field name="width" type="Number">Represents the width, in points, of the chart object.</field>
        }

        Chart.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.Chart"/>
        }
        Chart.prototype.delete = function() {
            /// <summary>
            /// Deletes the chart object.
            /// </summary>
            /// <returns ></returns>
        }
        Chart.prototype.setData = function(sourceData, seriesBy) {
            /// <summary>
            /// Resets the source data for the chart.
            /// </summary>
            /// <param name="sourceData" >The Range object corresponding to the source data.</param>
            /// <param name="seriesBy" type="String" optional="true">Specifies the way columns or rows are used as data series on the chart. Can be one of the following: Auto (default), Rows, Columns. See Excel.ChartSeriesBy for details.</param>
            /// <returns ></returns>
        }
        Chart.prototype.setPosition = function(startCell, endCell) {
            /// <summary>
            /// Positions the chart relative to cells on the worksheet.
            /// </summary>
            /// <param name="startCell" >The start cell. This is where the chart will be moved to. The start cell is the top-left or top-right cell, depending on the user&apos;s right-to-left display settings.</param>
            /// <param name="endCell"  optional="true">(Optional) The end cell. If specified, the chart&apos;s width and height will be set to fully cover up this cell/range.</param>
            /// <returns ></returns>
        }
        return Chart;
    })(OfficeExtension.ClientObject);
    Excel.Chart = Chart;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartAreaFormat = (function(_super) {
        __extends(ChartAreaFormat, _super);
        function ChartAreaFormat() {
            /// <summary> Encapsulates the format properties for the overall chart area. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="fill" type="Excel.ChartFill">Represents the fill format of an object, which includes background formatting information. Read-only.</field>
            /// <field name="font" type="Excel.ChartFont">Represents the font attributes (font name, font size, color, etc.) for the current object. Read-only.</field>
        }

        ChartAreaFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartAreaFormat"/>
        }
        return ChartAreaFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartAreaFormat = ChartAreaFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartAxes = (function(_super) {
        __extends(ChartAxes, _super);
        function ChartAxes() {
            /// <summary> Represents the chart axes. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="categoryAxis" type="Excel.ChartAxis">Represents the category axis in a chart. Read-only.</field>
            /// <field name="seriesAxis" type="Excel.ChartAxis">Represents the series axis of a 3-dimensional chart. Read-only.</field>
            /// <field name="valueAxis" type="Excel.ChartAxis">Represents the value axis in an axis. Read-only.</field>
        }

        ChartAxes.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartAxes"/>
        }
        return ChartAxes;
    })(OfficeExtension.ClientObject);
    Excel.ChartAxes = ChartAxes;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartAxis = (function(_super) {
        __extends(ChartAxis, _super);
        function ChartAxis() {
            /// <summary> Represents a single axis in a chart. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartAxisFormat">Represents the formatting of a chart object, which includes line and font formatting. Read-only.</field>
            /// <field name="majorGridlines" type="Excel.ChartGridlines">Returns a gridlines object that represents the major gridlines for the specified axis. Read-only.</field>
            /// <field name="majorUnit" >Represents the interval between two major tick marks. Can be set to a numeric value or an empty string.  The returned value is always a number.</field>
            /// <field name="maximum" >Represents the maximum value on the value axis.  Can be set to a numeric value or an empty string (for automatic axis values).  The returned value is always a number.</field>
            /// <field name="minimum" >Represents the minimum value on the value axis. Can be set to a numeric value or an empty string (for automatic axis values).  The returned value is always a number.</field>
            /// <field name="minorGridlines" type="Excel.ChartGridlines">Returns a Gridlines object that represents the minor gridlines for the specified axis. Read-only.</field>
            /// <field name="minorUnit" >Represents the interval between two minor tick marks. &quot;Can be set to a numeric value or an empty string (for automatic axis values). The returned value is always a number.</field>
            /// <field name="title" type="Excel.ChartAxisTitle">Represents the axis title. Read-only.</field>
        }

        ChartAxis.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartAxis"/>
        }
        return ChartAxis;
    })(OfficeExtension.ClientObject);
    Excel.ChartAxis = ChartAxis;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartAxisFormat = (function(_super) {
        __extends(ChartAxisFormat, _super);
        function ChartAxisFormat() {
            /// <summary> Encapsulates the format properties for the chart axis. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="font" type="Excel.ChartFont">Represents the font attributes (font name, font size, color, etc.) for a chart axis element. Read-only.</field>
            /// <field name="line" type="Excel.ChartLineFormat">Represents chart line formatting. Read-only.</field>
        }

        ChartAxisFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartAxisFormat"/>
        }
        return ChartAxisFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartAxisFormat = ChartAxisFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartAxisTitle = (function(_super) {
        __extends(ChartAxisTitle, _super);
        function ChartAxisTitle() {
            /// <summary> Represents the title of a chart axis. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartAxisTitleFormat">Represents the formatting of chart axis title. Read-only.</field>
            /// <field name="text" type="String">Represents the axis title.</field>
            /// <field name="visible" type="Boolean">A boolean that specifies the visibility of an axis title.</field>
        }

        ChartAxisTitle.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartAxisTitle"/>
        }
        return ChartAxisTitle;
    })(OfficeExtension.ClientObject);
    Excel.ChartAxisTitle = ChartAxisTitle;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartAxisTitleFormat = (function(_super) {
        __extends(ChartAxisTitleFormat, _super);
        function ChartAxisTitleFormat() {
            /// <summary> Represents the chart axis title formatting. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="font" type="Excel.ChartFont">Represents the font attributes, such as font name, font size, color, etc. of chart axis title object. Read-only.</field>
        }

        ChartAxisTitleFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartAxisTitleFormat"/>
        }
        return ChartAxisTitleFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartAxisTitleFormat = ChartAxisTitleFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartCollection = (function(_super) {
        __extends(ChartCollection, _super);
        function ChartCollection() {
            /// <summary> A collection of all the chart objects on a worksheet. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Returns the number of charts in the worksheet. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.Chart">Gets the loaded child items in this collection.</field>
        }

        ChartCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartCollection"/>
        }
        ChartCollection.prototype.add = function(type, sourceData, seriesBy) {
            /// <summary>
            /// Creates a new chart.
            /// </summary>
            /// <param name="type" type="String">Represents the type of a chart. See Excel.ChartType for details.</param>
            /// <param name="sourceData" >The Range object corresponding to the source data.</param>
            /// <param name="seriesBy" type="String" optional="true">Specifies the way columns or rows are used as data series on the chart. See Excel.ChartSeriesBy for details.</param>
            /// <returns type="Excel.Chart"></returns>
        }
        ChartCollection.prototype.getItem = function(name) {
            /// <summary>
            /// Gets a chart using its name. If there are multiple charts with the same name, the first one will be returned.
            /// </summary>
            /// <param name="name" type="String">Name of the chart to be retrieved.</param>
            /// <returns type="Excel.Chart"></returns>
        }
        ChartCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Gets a chart based on its position in the collection.
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.Chart"></returns>
        }
        return ChartCollection;
    })(OfficeExtension.ClientObject);
    Excel.ChartCollection = ChartCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartDataLabelFormat = (function(_super) {
        __extends(ChartDataLabelFormat, _super);
        function ChartDataLabelFormat() {
            /// <summary> Encapsulates the format properties for the chart data labels. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="fill" type="Excel.ChartFill">Represents the fill format of the current chart data label. Read-only.</field>
            /// <field name="font" type="Excel.ChartFont">Represents the font attributes (font name, font size, color, etc.) for a chart data label. Read-only.</field>
        }

        ChartDataLabelFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartDataLabelFormat"/>
        }
        return ChartDataLabelFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartDataLabelFormat = ChartDataLabelFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartDataLabelPosition = {
        __proto__: null,
        "invalid": "invalid",
        "none": "none",
        "center": "center",
        "insideEnd": "insideEnd",
        "insideBase": "insideBase",
        "outsideEnd": "outsideEnd",
        "left": "left",
        "right": "right",
        "top": "top",
        "bottom": "bottom",
        "bestFit": "bestFit",
        "callout": "callout",
    }
    Excel.ChartDataLabelPosition = ChartDataLabelPosition;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartDataLabels = (function(_super) {
        __extends(ChartDataLabels, _super);
        function ChartDataLabels() {
            /// <summary> Represents a collection of all the data labels on a chart point. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartDataLabelFormat">Represents the format of chart data labels, which includes fill and font formatting. Read-only.</field>
            /// <field name="position" type="String">DataLabelPosition value that represents the position of the data label. See Excel.ChartDataLabelPosition for details.</field>
            /// <field name="separator" type="String">String representing the separator used for the data labels on a chart.</field>
            /// <field name="showBubbleSize" type="Boolean">Boolean value representing if the data label bubble size is visible or not.</field>
            /// <field name="showCategoryName" type="Boolean">Boolean value representing if the data label category name is visible or not.</field>
            /// <field name="showLegendKey" type="Boolean">Boolean value representing if the data label legend key is visible or not.</field>
            /// <field name="showPercentage" type="Boolean">Boolean value representing if the data label percentage is visible or not.</field>
            /// <field name="showSeriesName" type="Boolean">Boolean value representing if the data label series name is visible or not.</field>
            /// <field name="showValue" type="Boolean">Boolean value representing if the data label value is visible or not.</field>
        }

        ChartDataLabels.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartDataLabels"/>
        }
        return ChartDataLabels;
    })(OfficeExtension.ClientObject);
    Excel.ChartDataLabels = ChartDataLabels;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartFill = (function(_super) {
        __extends(ChartFill, _super);
        function ChartFill() {
            /// <summary> Represents the fill formatting for a chart element. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
        }

        ChartFill.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartFill"/>
        }
        ChartFill.prototype.clear = function() {
            /// <summary>
            /// Clear the fill color of a chart element.
            /// </summary>
            /// <returns ></returns>
        }
        ChartFill.prototype.setSolidColor = function(color) {
            /// <summary>
            /// Sets the fill formatting of a chart element to a uniform color.
            /// </summary>
            /// <param name="color" type="String">HTML color code representing the color of the border line, of the form #RRGGBB (e.g. &quot;FFA500&quot;) or as a named HTML color (e.g. &quot;orange&quot;).</param>
            /// <returns ></returns>
        }
        return ChartFill;
    })(OfficeExtension.ClientObject);
    Excel.ChartFill = ChartFill;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartFont = (function(_super) {
        __extends(ChartFont, _super);
        function ChartFont() {
            /// <summary> This object represents the font attributes (font name, font size, color, etc.) for a chart object. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="bold" type="Boolean">Represents the bold status of font.</field>
            /// <field name="color" type="String">HTML color code representation of the text color. E.g. #FF0000 represents Red.</field>
            /// <field name="italic" type="Boolean">Represents the italic status of the font.</field>
            /// <field name="name" type="String">Font name (e.g. &quot;Calibri&quot;)</field>
            /// <field name="size" type="Number">Size of the font (e.g. 11)</field>
            /// <field name="underline" type="String">Type of underline applied to the font. See Excel.ChartUnderlineStyle for details.</field>
        }

        ChartFont.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartFont"/>
        }
        return ChartFont;
    })(OfficeExtension.ClientObject);
    Excel.ChartFont = ChartFont;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartGridlines = (function(_super) {
        __extends(ChartGridlines, _super);
        function ChartGridlines() {
            /// <summary> Represents major or minor gridlines on a chart axis. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartGridlinesFormat">Represents the formatting of chart gridlines. Read-only.</field>
            /// <field name="visible" type="Boolean">Boolean value representing if the axis gridlines are visible or not.</field>
        }

        ChartGridlines.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartGridlines"/>
        }
        return ChartGridlines;
    })(OfficeExtension.ClientObject);
    Excel.ChartGridlines = ChartGridlines;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartGridlinesFormat = (function(_super) {
        __extends(ChartGridlinesFormat, _super);
        function ChartGridlinesFormat() {
            /// <summary> Encapsulates the format properties for chart gridlines. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="line" type="Excel.ChartLineFormat">Represents chart line formatting. Read-only.</field>
        }

        ChartGridlinesFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartGridlinesFormat"/>
        }
        return ChartGridlinesFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartGridlinesFormat = ChartGridlinesFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartLegend = (function(_super) {
        __extends(ChartLegend, _super);
        function ChartLegend() {
            /// <summary> Represents the legend in a chart. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartLegendFormat">Represents the formatting of a chart legend, which includes fill and font formatting. Read-only.</field>
            /// <field name="overlay" type="Boolean">Boolean value for whether the chart legend should overlap with the main body of the chart.</field>
            /// <field name="position" type="String">Represents the position of the legend on the chart. See Excel.ChartLegendPosition for details.</field>
            /// <field name="visible" type="Boolean">A boolean value the represents the visibility of a ChartLegend object.</field>
        }

        ChartLegend.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartLegend"/>
        }
        return ChartLegend;
    })(OfficeExtension.ClientObject);
    Excel.ChartLegend = ChartLegend;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartLegendFormat = (function(_super) {
        __extends(ChartLegendFormat, _super);
        function ChartLegendFormat() {
            /// <summary> Encapsulates the format properties of a chart legend. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="fill" type="Excel.ChartFill">Represents the fill format of an object, which includes background formating information. Read-only.</field>
            /// <field name="font" type="Excel.ChartFont">Represents the font attributes such as font name, font size, color, etc. of a chart legend. Read-only.</field>
        }

        ChartLegendFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartLegendFormat"/>
        }
        return ChartLegendFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartLegendFormat = ChartLegendFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartLegendPosition = {
        __proto__: null,
        "invalid": "invalid",
        "top": "top",
        "bottom": "bottom",
        "left": "left",
        "right": "right",
        "corner": "corner",
        "custom": "custom",
    }
    Excel.ChartLegendPosition = ChartLegendPosition;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartLineFormat = (function(_super) {
        __extends(ChartLineFormat, _super);
        function ChartLineFormat() {
            /// <summary> Enapsulates the formatting options for line elements. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="color" type="String">HTML color code representing the color of lines in the chart.</field>
        }

        ChartLineFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartLineFormat"/>
        }
        ChartLineFormat.prototype.clear = function() {
            /// <summary>
            /// Clear the line format of a chart element.
            /// </summary>
            /// <returns ></returns>
        }
        return ChartLineFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartLineFormat = ChartLineFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartPoint = (function(_super) {
        __extends(ChartPoint, _super);
        function ChartPoint() {
            /// <summary> Represents a point of a series in a chart. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartPointFormat">Encapsulates the format properties chart point. Read-only.</field>
            /// <field name="value" >Returns the value of a chart point. Read-only.</field>
        }

        ChartPoint.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartPoint"/>
        }
        return ChartPoint;
    })(OfficeExtension.ClientObject);
    Excel.ChartPoint = ChartPoint;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartPointFormat = (function(_super) {
        __extends(ChartPointFormat, _super);
        function ChartPointFormat() {
            /// <summary> Represents formatting object for chart points. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="fill" type="Excel.ChartFill">Represents the fill format of a chart, which includes background formating information. Read-only.</field>
        }

        ChartPointFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartPointFormat"/>
        }
        return ChartPointFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartPointFormat = ChartPointFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartPointsCollection = (function(_super) {
        __extends(ChartPointsCollection, _super);
        function ChartPointsCollection() {
            /// <summary> A collection of all the chart points within a series inside a chart. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Returns the number of chart points in the collection. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.ChartPoint">Gets the loaded child items in this collection.</field>
        }

        ChartPointsCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartPointsCollection"/>
        }
        ChartPointsCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Retrieve a point based on its position within the series.
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.ChartPoint"></returns>
        }
        return ChartPointsCollection;
    })(OfficeExtension.ClientObject);
    Excel.ChartPointsCollection = ChartPointsCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartSeries = (function(_super) {
        __extends(ChartSeries, _super);
        function ChartSeries() {
            /// <summary> Represents a series in a chart. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartSeriesFormat">Represents the formatting of a chart series, which includes fill and line formatting. Read-only.</field>
            /// <field name="name" type="String">Represents the name of a series in a chart.</field>
            /// <field name="points" type="Excel.ChartPointsCollection">Represents a collection of all points in the series. Read-only.</field>
        }

        ChartSeries.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartSeries"/>
        }
        return ChartSeries;
    })(OfficeExtension.ClientObject);
    Excel.ChartSeries = ChartSeries;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartSeriesBy = {
        __proto__: null,
        "auto": "auto",
        "columns": "columns",
        "rows": "rows",
    }
    Excel.ChartSeriesBy = ChartSeriesBy;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartSeriesCollection = (function(_super) {
        __extends(ChartSeriesCollection, _super);
        function ChartSeriesCollection() {
            /// <summary> Represents a collection of chart series. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Returns the number of series in the collection. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.ChartSeries">Gets the loaded child items in this collection.</field>
        }

        ChartSeriesCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartSeriesCollection"/>
        }
        ChartSeriesCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Retrieves a series based on its position in the collection
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.ChartSeries"></returns>
        }
        return ChartSeriesCollection;
    })(OfficeExtension.ClientObject);
    Excel.ChartSeriesCollection = ChartSeriesCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartSeriesFormat = (function(_super) {
        __extends(ChartSeriesFormat, _super);
        function ChartSeriesFormat() {
            /// <summary> encapsulates the format properties for the chart series </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="fill" type="Excel.ChartFill">Represents the fill format of a chart series, which includes background formating information. Read-only.</field>
            /// <field name="line" type="Excel.ChartLineFormat">Represents line formatting. Read-only.</field>
        }

        ChartSeriesFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartSeriesFormat"/>
        }
        return ChartSeriesFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartSeriesFormat = ChartSeriesFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartTitle = (function(_super) {
        __extends(ChartTitle, _super);
        function ChartTitle() {
            /// <summary> Represents a chart title object of a chart. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="format" type="Excel.ChartTitleFormat">Represents the formatting of a chart title, which includes fill and font formatting. Read-only.</field>
            /// <field name="overlay" type="Boolean">Boolean value representing if the chart title will overlay the chart or not.</field>
            /// <field name="text" type="String">Represents the title text of a chart.</field>
            /// <field name="visible" type="Boolean">A boolean value the represents the visibility of a chart title object.</field>
        }

        ChartTitle.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartTitle"/>
        }
        return ChartTitle;
    })(OfficeExtension.ClientObject);
    Excel.ChartTitle = ChartTitle;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartTitleFormat = (function(_super) {
        __extends(ChartTitleFormat, _super);
        function ChartTitleFormat() {
            /// <summary> Provides access to the office art formatting for chart title. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="fill" type="Excel.ChartFill">Represents the fill format of an object, which includes background formating information. Read-only.</field>
            /// <field name="font" type="Excel.ChartFont">Represents the font attributes (font name, font size, color, etc.) for an object. Read-only.</field>
        }

        ChartTitleFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.ChartTitleFormat"/>
        }
        return ChartTitleFormat;
    })(OfficeExtension.ClientObject);
    Excel.ChartTitleFormat = ChartTitleFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartType = {
        __proto__: null,
        "invalid": "invalid",
        "columnClustered": "columnClustered",
        "columnStacked": "columnStacked",
        "columnStacked100": "columnStacked100",
        "_3DColumnClustered": "_3DColumnClustered",
        "_3DColumnStacked": "_3DColumnStacked",
        "_3DColumnStacked100": "_3DColumnStacked100",
        "barClustered": "barClustered",
        "barStacked": "barStacked",
        "barStacked100": "barStacked100",
        "_3DBarClustered": "_3DBarClustered",
        "_3DBarStacked": "_3DBarStacked",
        "_3DBarStacked100": "_3DBarStacked100",
        "lineStacked": "lineStacked",
        "lineStacked100": "lineStacked100",
        "lineMarkers": "lineMarkers",
        "lineMarkersStacked": "lineMarkersStacked",
        "lineMarkersStacked100": "lineMarkersStacked100",
        "pieOfPie": "pieOfPie",
        "pieExploded": "pieExploded",
        "_3DPieExploded": "_3DPieExploded",
        "barOfPie": "barOfPie",
        "xyscatterSmooth": "xyscatterSmooth",
        "xyscatterSmoothNoMarkers": "xyscatterSmoothNoMarkers",
        "xyscatterLines": "xyscatterLines",
        "xyscatterLinesNoMarkers": "xyscatterLinesNoMarkers",
        "areaStacked": "areaStacked",
        "areaStacked100": "areaStacked100",
        "_3DAreaStacked": "_3DAreaStacked",
        "_3DAreaStacked100": "_3DAreaStacked100",
        "doughnutExploded": "doughnutExploded",
        "radarMarkers": "radarMarkers",
        "radarFilled": "radarFilled",
        "surface": "surface",
        "surfaceWireframe": "surfaceWireframe",
        "surfaceTopView": "surfaceTopView",
        "surfaceTopViewWireframe": "surfaceTopViewWireframe",
        "bubble": "bubble",
        "bubble3DEffect": "bubble3DEffect",
        "stockHLC": "stockHLC",
        "stockOHLC": "stockOHLC",
        "stockVHLC": "stockVHLC",
        "stockVOHLC": "stockVOHLC",
        "cylinderColClustered": "cylinderColClustered",
        "cylinderColStacked": "cylinderColStacked",
        "cylinderColStacked100": "cylinderColStacked100",
        "cylinderBarClustered": "cylinderBarClustered",
        "cylinderBarStacked": "cylinderBarStacked",
        "cylinderBarStacked100": "cylinderBarStacked100",
        "cylinderCol": "cylinderCol",
        "coneColClustered": "coneColClustered",
        "coneColStacked": "coneColStacked",
        "coneColStacked100": "coneColStacked100",
        "coneBarClustered": "coneBarClustered",
        "coneBarStacked": "coneBarStacked",
        "coneBarStacked100": "coneBarStacked100",
        "coneCol": "coneCol",
        "pyramidColClustered": "pyramidColClustered",
        "pyramidColStacked": "pyramidColStacked",
        "pyramidColStacked100": "pyramidColStacked100",
        "pyramidBarClustered": "pyramidBarClustered",
        "pyramidBarStacked": "pyramidBarStacked",
        "pyramidBarStacked100": "pyramidBarStacked100",
        "pyramidCol": "pyramidCol",
        "_3DColumn": "_3DColumn",
        "line": "line",
        "_3DLine": "_3DLine",
        "_3DPie": "_3DPie",
        "pie": "pie",
        "xyscatter": "xyscatter",
        "_3DArea": "_3DArea",
        "area": "area",
        "doughnut": "doughnut",
        "radar": "radar",
    }
    Excel.ChartType = ChartType;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ChartUnderlineStyle = {
        __proto__: null,
        "none": "none",
        "single": "single",
    }
    Excel.ChartUnderlineStyle = ChartUnderlineStyle;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var ClearApplyTo = {
        __proto__: null,
        "all": "all",
        "formats": "formats",
        "contents": "contents",
    }
    Excel.ClearApplyTo = ClearApplyTo;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var DeleteShiftDirection = {
        __proto__: null,
        "up": "up",
        "left": "left",
    }
    Excel.DeleteShiftDirection = DeleteShiftDirection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var HorizontalAlignment = {
        __proto__: null,
        "general": "general",
        "left": "left",
        "center": "center",
        "right": "right",
        "fill": "fill",
        "justify": "justify",
        "centerAcrossSelection": "centerAcrossSelection",
        "distributed": "distributed",
    }
    Excel.HorizontalAlignment = HorizontalAlignment;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var InsertShiftDirection = {
        __proto__: null,
        "down": "down",
        "right": "right",
    }
    Excel.InsertShiftDirection = InsertShiftDirection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var NamedItem = (function(_super) {
        __extends(NamedItem, _super);
        function NamedItem() {
            /// <summary> Represents a defined name for a range of cells or value. Names can be primitive named objects (as seen in the type below), range object, reference to a range. This object can be used to obtain range object associated with names. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="name" type="String">The name of the object. Read-only.</field>
            /// <field name="type" type="String">Indicates what type of reference is associated with the name. See Excel.NamedItemType for details. Read-only.</field>
            /// <field name="value" >Represents the formula that the name is defined to refer to. E.g. =Sheet14!$B$2:$H$12, =4.75, etc. Read-only.</field>
            /// <field name="visible" type="Boolean">Specifies whether the object is visible or not.</field>
        }

        NamedItem.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.NamedItem"/>
        }
        NamedItem.prototype.getRange = function() {
            /// <summary>
            /// Returns the range object that is associated with the name. Throws an exception if the named item&apos;s type is not a range.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        return NamedItem;
    })(OfficeExtension.ClientObject);
    Excel.NamedItem = NamedItem;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var NamedItemCollection = (function(_super) {
        __extends(NamedItemCollection, _super);
        function NamedItemCollection() {
            /// <summary> A collection of all the nameditem objects that are part of the workbook. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="items" type="Array" elementType="Excel.NamedItem">Gets the loaded child items in this collection.</field>
        }

        NamedItemCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.NamedItemCollection"/>
        }
        NamedItemCollection.prototype.getItem = function(name) {
            /// <summary>
            /// Gets a nameditem object using its name
            /// </summary>
            /// <param name="name" type="String">nameditem name.</param>
            /// <returns type="Excel.NamedItem"></returns>
        }
        return NamedItemCollection;
    })(OfficeExtension.ClientObject);
    Excel.NamedItemCollection = NamedItemCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var NamedItemType = {
        __proto__: null,
        "string": "string",
        "integer": "integer",
        "double": "double",
        "boolean": "boolean",
        "range": "range",
    }
    Excel.NamedItemType = NamedItemType;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var Range = (function(_super) {
        __extends(Range, _super);
        function Range() {
            /// <summary> Range represents a set of one or more contiguous cells such as a cell, a row, a column, block of cells, etc. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="address" type="String">Represents the range reference in A1-style. Address value will contain the Sheet reference (e.g. Sheet1!A1:B4). Read-only.</field>
            /// <field name="addressLocal" type="String">Represents range reference for the specified range in the language of the user. Read-only.</field>
            /// <field name="cellCount" type="Number">Number of cells in the range. Read-only.</field>
            /// <field name="columnCount" type="Number">Represents the total number of columns in the range. Read-only.</field>
            /// <field name="columnIndex" type="Number">Represents the column number of the first cell in the range. Zero-indexed. Read-only.</field>
            /// <field name="format" type="Excel.RangeFormat">Returns a format object, encapsulating the range&apos;s font, fill, borders, alignment, and other properties. Read-only.</field>
            /// <field name="formulas" type="Array" elementType="Array">Represents the formula in A1-style notation.</field>
            /// <field name="formulasLocal" type="Array" elementType="Array">Represents the formula in A1-style notation, in the user&apos;s language and number-formatting locale.  For example, the English &quot;=SUM(A1, 1.5)&quot; formula would become &quot;=SUMME(A1; 1,5)&quot; in German.</field>
            /// <field name="numberFormat" type="Array" elementType="Array">Represents Excel&apos;s number format code for the given cell.</field>
            /// <field name="rowCount" type="Number">Returns the total number of rows in the range. Read-only.</field>
            /// <field name="rowIndex" type="Number">Returns the row number of the first cell in the range. Zero-indexed. Read-only.</field>
            /// <field name="text" type="Array" elementType="Array">Text values of the specified range. The Text value will not depend on the cell width. The # sign substitution that happens in Excel UI will not affect the text value returned by the API. Read-only.</field>
            /// <field name="valueTypes" type="Array" elementType="Array">Represents the type of data of each cell. Read-only.</field>
            /// <field name="values" type="Array" elementType="Array">Represents the raw values of the specified range. The data returned could be of type string, number, or a boolean. Cell that contain an error will return the error string.</field>
            /// <field name="worksheet" type="Excel.Worksheet">The worksheet containing the current range. Read-only.</field>
        }

        Range.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.Range"/>
        }
        Range.prototype.clear = function(applyTo) {
            /// <summary>
            /// Clear range values, format, fill, border, etc.
            /// </summary>
            /// <param name="applyTo" type="String" optional="true">Determines the type of clear action. See Excel.ClearApplyTo for details.</param>
            /// <returns ></returns>
        }
        Range.prototype.delete = function(shift) {
            /// <summary>
            /// Deletes the cells associated with the range.
            /// </summary>
            /// <param name="shift" type="String">Specifies which way to shift the cells. See Excel.DeleteShiftDirection for details.</param>
            /// <returns ></returns>
        }
        Range.prototype.getBoundingRect = function(anotherRange) {
            /// <summary>
            /// Gets the smallest range object that encompasses the given ranges. For example, the GetBoundingRect of &quot;B2:C5&quot; and &quot;D10:E15&quot; is &quot;B2:E16&quot;.
            /// </summary>
            /// <param name="anotherRange" >The range object or address or range name.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getCell = function(row, column) {
            /// <summary>
            /// Gets the range object containing the single cell based on row and column numbers. The cell can be outside the bounds of its parent range, so long as it&apos;s stays within the worksheet grid. The returned cell is located relative to the top left cell of the range.
            /// </summary>
            /// <param name="row" type="Number">Row number of the cell to be retrieved. Zero-indexed.</param>
            /// <param name="column" type="Number">Column number of the cell to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getColumn = function(column) {
            /// <summary>
            /// Gets a column contained in the range.
            /// </summary>
            /// <param name="column" type="Number">Column number of the range to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getEntireColumn = function() {
            /// <summary>
            /// Gets an object that represents the entire column of the range.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getEntireRow = function() {
            /// <summary>
            /// Gets an object that represents the entire row of the range.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getIntersection = function(anotherRange) {
            /// <summary>
            /// Gets the range object that represents the rectangular intersection of the given ranges.
            /// </summary>
            /// <param name="anotherRange" >The range object or range address that will be used to determine the intersection of ranges.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getLastCell = function() {
            /// <summary>
            /// Gets the last cell within the range. For example, the last cell of &quot;B2:D5&quot; is &quot;D5&quot;.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getLastColumn = function() {
            /// <summary>
            /// Gets the last column within the range. For example, the last column of &quot;B2:D5&quot; is &quot;D2:D5&quot;.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getLastRow = function() {
            /// <summary>
            /// Gets the last row within the range. For example, the last row of &quot;B2:D5&quot; is &quot;B5:D5&quot;.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getOffsetRange = function(rowOffset, columnOffset) {
            /// <summary>
            /// Gets an object which represents a range that&apos;s offset from the specified range. The dimension of the returned range will match this range. If the resulting range is forced outside the bounds of the worksheet grid, an exception will be thrown.
            /// </summary>
            /// <param name="rowOffset" type="Number">The number of rows (positive, negative, or 0) by which the range is to be offset. Positive values are offset downward, and negative values are offset upward.</param>
            /// <param name="columnOffset" type="Number">The number of columns (positive, negative, or 0) by which the range is to be offset. Positive values are offset to the right, and negative values are offset to the left.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getRow = function(row) {
            /// <summary>
            /// Gets a row contained in the range.
            /// </summary>
            /// <param name="row" type="Number">Row number of the range to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.getUsedRange = function() {
            /// <summary>
            /// Returns the used range of the given range object.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.insert = function(shift) {
            /// <summary>
            /// Inserts a cell or a range of cells into the worksheet in place of this range, and shifts the other cells to make space. Returns a new Range object at the now blank space.
            /// </summary>
            /// <param name="shift" type="String">Specifies which way to shift the cells. See Excel.InsertShiftDirection for details.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Range.prototype.select = function() {
            /// <summary>
            /// Selects the specified range in the Excel UI.
            /// </summary>
            /// <returns ></returns>
        }
        return Range;
    })(OfficeExtension.ClientObject);
    Excel.Range = Range;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var RangeBorder = (function(_super) {
        __extends(RangeBorder, _super);
        function RangeBorder() {
            /// <summary> Represents the border of an object. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="color" type="String">HTML color code representing the color of the border line, of the form #RRGGBB (e.g. &quot;FFA500&quot;) or as a named HTML color (e.g. &quot;orange&quot;).</field>
            /// <field name="sideIndex" type="String">Constant value that indicates the specific side of the border. See Excel.BorderIndex for details. Read-only.</field>
            /// <field name="style" type="String">One of the constants of line style specifying the line style for the border. See Excel.BorderLineStyle for details.</field>
            /// <field name="weight" type="String">Specifies the weight of the border around a range. See Excel.BorderWeight for details.</field>
        }

        RangeBorder.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.RangeBorder"/>
        }
        return RangeBorder;
    })(OfficeExtension.ClientObject);
    Excel.RangeBorder = RangeBorder;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var RangeBorderCollection = (function(_super) {
        __extends(RangeBorderCollection, _super);
        function RangeBorderCollection() {
            /// <summary> Represents the border objects that make up range border. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Number of border objects in the collection. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.RangeBorder">Gets the loaded child items in this collection.</field>
        }

        RangeBorderCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.RangeBorderCollection"/>
        }
        RangeBorderCollection.prototype.getItem = function(index) {
            /// <summary>
            /// Gets a border object using its name
            /// </summary>
            /// <param name="index" type="String">Index value of the border object to be retrieved. See Excel.BorderIndex for details.</param>
            /// <returns type="Excel.RangeBorder"></returns>
        }
        RangeBorderCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Gets a border object using its index
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.RangeBorder"></returns>
        }
        return RangeBorderCollection;
    })(OfficeExtension.ClientObject);
    Excel.RangeBorderCollection = RangeBorderCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var RangeFill = (function(_super) {
        __extends(RangeFill, _super);
        function RangeFill() {
            /// <summary> Represents the background of a range object. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="color" type="String">HTML color code representing the color of the border line, of the form #RRGGBB (e.g. &quot;FFA500&quot;) or as a named HTML color (e.g. &quot;orange&quot;)</field>
        }

        RangeFill.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.RangeFill"/>
        }
        RangeFill.prototype.clear = function() {
            /// <summary>
            /// Resets the range background.
            /// </summary>
            /// <returns ></returns>
        }
        return RangeFill;
    })(OfficeExtension.ClientObject);
    Excel.RangeFill = RangeFill;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var RangeFont = (function(_super) {
        __extends(RangeFont, _super);
        function RangeFont() {
            /// <summary> This object represents the font attributes (font name, font size, color, etc.) for an object. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="bold" type="Boolean">Represents the bold status of font.</field>
            /// <field name="color" type="String">HTML color code representation of the text color. E.g. #FF0000 represents Red.</field>
            /// <field name="italic" type="Boolean">Represents the italic status of the font.</field>
            /// <field name="name" type="String">Font name (e.g. &quot;Calibri&quot;)</field>
            /// <field name="size" type="Number">Font size.</field>
            /// <field name="underline" type="String">Type of underline applied to the font. See Excel.RangeUnderlineStyle for details.</field>
        }

        RangeFont.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.RangeFont"/>
        }
        return RangeFont;
    })(OfficeExtension.ClientObject);
    Excel.RangeFont = RangeFont;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var RangeFormat = (function(_super) {
        __extends(RangeFormat, _super);
        function RangeFormat() {
            /// <summary> A format object encapsulating the range&apos;s font, fill, borders, alignment, and other properties. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="borders" type="Excel.RangeBorderCollection">Collection of border objects that apply to the overall range selected Read-only.</field>
            /// <field name="fill" type="Excel.RangeFill">Returns the fill object defined on the overall range. Read-only.</field>
            /// <field name="font" type="Excel.RangeFont">Returns the font object defined on the overall range selected Read-only.</field>
            /// <field name="horizontalAlignment" type="String">Represents the horizontal alignment for the specified object. See Excel.HorizontalAlignment for details.</field>
            /// <field name="verticalAlignment" type="String">Represents the vertical alignment for the specified object. See Excel.VerticalAlignment for details.</field>
            /// <field name="wrapText" type="Boolean">Indicates if Excel wraps the text in the object. A null value indicates that the entire range doesn&apos;t have uniform wrap setting</field>
        }

        RangeFormat.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.RangeFormat"/>
        }
        return RangeFormat;
    })(OfficeExtension.ClientObject);
    Excel.RangeFormat = RangeFormat;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var RangeUnderlineStyle = {
        __proto__: null,
        "none": "none",
        "single": "single",
        "double": "double",
        "singleAccountant": "singleAccountant",
        "doubleAccountant": "doubleAccountant",
    }
    Excel.RangeUnderlineStyle = RangeUnderlineStyle;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var RangeValueType = {
        __proto__: null,
        "unknown": "unknown",
        "empty": "empty",
        "string": "string",
        "integer": "integer",
        "double": "double",
        "boolean": "boolean",
        "error": "error",
    }
    Excel.RangeValueType = RangeValueType;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var SheetVisibility = {
        __proto__: null,
        "visible": "visible",
        "hidden": "hidden",
        "veryHidden": "veryHidden",
    }
    Excel.SheetVisibility = SheetVisibility;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var Table = (function(_super) {
        __extends(Table, _super);
        function Table() {
            /// <summary> Represents an Excel table. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="columns" type="Excel.TableColumnCollection">Represents a collection of all the columns in the table. Read-only.</field>
            /// <field name="id" type="Number">Returns a value that uniquely identifies the table in a given workbook. The value of the identifier remains the same even when the table is renamed. Read-only.</field>
            /// <field name="name" type="String">Name of the table.</field>
            /// <field name="rows" type="Excel.TableRowCollection">Represents a collection of all the rows in the table. Read-only.</field>
            /// <field name="showHeaders" type="Boolean">Indicates whether the header row is visible or not. This value can be set to show or remove the header row.</field>
            /// <field name="showTotals" type="Boolean">Indicates whether the total row is visible or not. This value can be set to show or remove the total row.</field>
            /// <field name="style" type="String">Constant value that represents the Table style. Possible values are: TableStyleLight1 thru TableStyleLight21, TableStyleMedium1 thru TableStyleMedium28, TableStyleStyleDark1 thru TableStyleStyleDark11. A custom user-defined style present in the workbook can also be specified.</field>
        }

        Table.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.Table"/>
        }
        Table.prototype.delete = function() {
            /// <summary>
            /// Deletes the table.
            /// </summary>
            /// <returns ></returns>
        }
        Table.prototype.getDataBodyRange = function() {
            /// <summary>
            /// Gets the range object associated with the data body of the table.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Table.prototype.getHeaderRowRange = function() {
            /// <summary>
            /// Gets the range object associated with header row of the table.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Table.prototype.getRange = function() {
            /// <summary>
            /// Gets the range object associated with the entire table.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        Table.prototype.getTotalRowRange = function() {
            /// <summary>
            /// Gets the range object associated with totals row of the table.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        return Table;
    })(OfficeExtension.ClientObject);
    Excel.Table = Table;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var TableCollection = (function(_super) {
        __extends(TableCollection, _super);
        function TableCollection() {
            /// <summary> Represents a collection of all the tables that are part of the workbook. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Returns the number of tables in the workbook. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.Table">Gets the loaded child items in this collection.</field>
        }

        TableCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.TableCollection"/>
        }
        TableCollection.prototype.add = function(address, hasHeaders) {
            /// <summary>
            /// Create a new table. The range source address determines the worksheet under which the table will be added. If the table cannot be added (e.g., because the address is invalid, or the table would overlap with another table), an error will be thrown.
            /// </summary>
            /// <param name="address" type="String">Address or name of the range object representing the data source. If the address does not contain a sheet name, the currently-active sheet is used.</param>
            /// <param name="hasHeaders" type="Boolean">Boolean value that indicates whether the data being imported has column labels. If the source does not contain headers (i.e,. when this property set to false), Excel will automatically generate header shifting the data down by one row.</param>
            /// <returns type="Excel.Table"></returns>
        }
        TableCollection.prototype.getItem = function(key) {
            /// <summary>
            /// Gets a table by Name or ID.
            /// </summary>
            /// <param name="key" >Name or ID of the table to be retrieved.</param>
            /// <returns type="Excel.Table"></returns>
        }
        TableCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Gets a table based on its position in the collection.
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.Table"></returns>
        }
        return TableCollection;
    })(OfficeExtension.ClientObject);
    Excel.TableCollection = TableCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var TableColumn = (function(_super) {
        __extends(TableColumn, _super);
        function TableColumn() {
            /// <summary> Represents a column in a table. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="id" type="Number">Returns a unique key that identifies the column within the table. Read-only.</field>
            /// <field name="index" type="Number">Returns the index number of the column within the columns collection of the table. Zero-indexed. Read-only.</field>
            /// <field name="name" type="String">Returns the name of the table column. Read-only.</field>
            /// <field name="values" type="Array" elementType="Array">Represents the raw values of the specified range. The data returned could be of type string, number, or a boolean. Cell that contain an error will return the error string.</field>
        }

        TableColumn.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.TableColumn"/>
        }
        TableColumn.prototype.delete = function() {
            /// <summary>
            /// Deletes the column from the table.
            /// </summary>
            /// <returns ></returns>
        }
        TableColumn.prototype.getDataBodyRange = function() {
            /// <summary>
            /// Gets the range object associated with the data body of the column.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        TableColumn.prototype.getHeaderRowRange = function() {
            /// <summary>
            /// Gets the range object associated with the header row of the column.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        TableColumn.prototype.getRange = function() {
            /// <summary>
            /// Gets the range object associated with the entire column.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        TableColumn.prototype.getTotalRowRange = function() {
            /// <summary>
            /// Gets the range object associated with the totals row of the column.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        return TableColumn;
    })(OfficeExtension.ClientObject);
    Excel.TableColumn = TableColumn;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var TableColumnCollection = (function(_super) {
        __extends(TableColumnCollection, _super);
        function TableColumnCollection() {
            /// <summary> Represents a collection of all the columns that are part of the table. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Returns the number of columns in the table. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.TableColumn">Gets the loaded child items in this collection.</field>
        }

        TableColumnCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.TableColumnCollection"/>
        }
        TableColumnCollection.prototype.add = function(index, values) {
            /// <summary>
            /// Adds a new column to the table.
            /// </summary>
            /// <param name="index" type="Number">Specifies the relative position of the new column. The previous column at this position is shifted to the right. The index value should be equal to or less than the last column&apos;s index value, so it cannot be used to append a column at the end of the table. Zero-indexed.</param>
            /// <param name="values"  optional="true">A 2-dimensional array of unformatted values of the table column.</param>
            /// <returns type="Excel.TableColumn"></returns>
        }
        TableColumnCollection.prototype.getItem = function(key) {
            /// <summary>
            /// Gets a column object by Name or ID.
            /// </summary>
            /// <param name="key" >Column Name or ID.</param>
            /// <returns type="Excel.TableColumn"></returns>
        }
        TableColumnCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Gets a column based on its position in the collection.
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.TableColumn"></returns>
        }
        return TableColumnCollection;
    })(OfficeExtension.ClientObject);
    Excel.TableColumnCollection = TableColumnCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var TableRow = (function(_super) {
        __extends(TableRow, _super);
        function TableRow() {
            /// <summary> Represents a row in a table. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="index" type="Number">Returns the index number of the row within the rows collection of the table. Zero-indexed. Read-only.</field>
            /// <field name="values" type="Array" elementType="Array">Represents the raw values of the specified range. The data returned could be of type string, number, or a boolean. Cell that contain an error will return the error string.</field>
        }

        TableRow.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.TableRow"/>
        }
        TableRow.prototype.delete = function() {
            /// <summary>
            /// Deletes the row from the table.
            /// </summary>
            /// <returns ></returns>
        }
        TableRow.prototype.getRange = function() {
            /// <summary>
            /// Returns the range object associated with the entire row.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        return TableRow;
    })(OfficeExtension.ClientObject);
    Excel.TableRow = TableRow;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var TableRowCollection = (function(_super) {
        __extends(TableRowCollection, _super);
        function TableRowCollection() {
            /// <summary> Represents a collection of all the rows that are part of the table. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="count" type="Number">Returns the number of rows in the table. Read-only.</field>
            /// <field name="items" type="Array" elementType="Excel.TableRow">Gets the loaded child items in this collection.</field>
        }

        TableRowCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.TableRowCollection"/>
        }
        TableRowCollection.prototype.add = function(index, values) {
            /// <summary>
            /// Adds a new row to the table.
            /// </summary>
            /// <param name="index" type="Number" optional="true">Specifies the relative position of the new row. If null, the addition happens at the end. Any rows below the inserted row are shifted downwards. Zero-indexed.</param>
            /// <param name="values"  optional="true">A 2-dimensional array of unformatted values of the table row.</param>
            /// <returns type="Excel.TableRow"></returns>
        }
        TableRowCollection.prototype.getItemAt = function(index) {
            /// <summary>
            /// Gets a row based on its position in the collection.
            /// </summary>
            /// <param name="index" type="Number">Index value of the object to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.TableRow"></returns>
        }
        return TableRowCollection;
    })(OfficeExtension.ClientObject);
    Excel.TableRowCollection = TableRowCollection;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var VerticalAlignment = {
        __proto__: null,
        "top": "top",
        "center": "center",
        "bottom": "bottom",
        "justify": "justify",
        "distributed": "distributed",
    }
    Excel.VerticalAlignment = VerticalAlignment;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var Workbook = (function(_super) {
        __extends(Workbook, _super);
        function Workbook() {
            /// <summary> Workbook is the top level object which contains related workbook objects such as worksheets, tables, ranges, etc. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="application" type="Excel.Application">Represents Excel application instance that contains this workbook. Read-only.</field>
            /// <field name="bindings" type="Excel.BindingCollection">Represents a collection of bindings that are part of the workbook. Read-only.</field>
            /// <field name="names" type="Excel.NamedItemCollection">Represents a collection of workbook scoped named items (named ranges and constants). Read-only.</field>
            /// <field name="tables" type="Excel.TableCollection">Represents a collection of tables associated with the workbook. Read-only.</field>
            /// <field name="worksheets" type="Excel.WorksheetCollection">Represents a collection of worksheets associated with the workbook. Read-only.</field>
        }

        Workbook.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.Workbook"/>
        }
        Workbook.prototype.getSelectedRange = function() {
            /// <summary>
            /// Gets the currently selected range from the workbook.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        return Workbook;
    })(OfficeExtension.ClientObject);
    Excel.Workbook = Workbook;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var Worksheet = (function(_super) {
        __extends(Worksheet, _super);
        function Worksheet() {
            /// <summary> An Excel worksheet is a grid of cells. It can contain data, tables, charts, etc. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="charts" type="Excel.ChartCollection">Returns collection of charts that are part of the worksheet. Read-only.</field>
            /// <field name="id" type="String">Returns a value that uniquely identifies the worksheet in a given workbook. The value of the identifier remains the same even when the worksheet is renamed or moved. Read-only.</field>
            /// <field name="name" type="String">The display name of the worksheet.</field>
            /// <field name="position" type="Number">The zero-based position of the worksheet within the workbook.</field>
            /// <field name="tables" type="Excel.TableCollection">Collection of tables that are part of the worksheet. Read-only.</field>
            /// <field name="visibility" type="String">The Visibility of the worksheet, Read-only.</field>
        }

        Worksheet.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.Worksheet"/>
        }
        Worksheet.prototype.activate = function() {
            /// <summary>
            /// Activate the worksheet in the Excel UI.
            /// </summary>
            /// <returns ></returns>
        }
        Worksheet.prototype.delete = function() {
            /// <summary>
            /// Deletes the worksheet from the workbook.
            /// </summary>
            /// <returns ></returns>
        }
        Worksheet.prototype.getCell = function(row, column) {
            /// <summary>
            /// Gets the range object containing the single cell based on row and column numbers. The cell can be outside the bounds of its parent range, so long as it&apos;s stays within the worksheet grid.
            /// </summary>
            /// <param name="row" type="Number">The row number of the cell to be retrieved. Zero-indexed.</param>
            /// <param name="column" type="Number">the column number of the cell to be retrieved. Zero-indexed.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Worksheet.prototype.getRange = function(address) {
            /// <summary>
            /// Gets the range object specified by the address or name.
            /// </summary>
            /// <param name="address" type="String" optional="true">The address or the name of the range. If not specified, the entire worksheet range is returned.</param>
            /// <returns type="Excel.Range"></returns>
        }
        Worksheet.prototype.getUsedRange = function() {
            /// <summary>
            /// The used range is the smallest range than encompasses any cells that have a value or formatting assigned to them. If the worksheet is blank, this function will return the top left cell.
            /// </summary>
            /// <returns type="Excel.Range"></returns>
        }
        return Worksheet;
    })(OfficeExtension.ClientObject);
    Excel.Worksheet = Worksheet;
})(Excel || (Excel = {}));

var Excel;
(function (Excel) {
    var WorksheetCollection = (function(_super) {
        __extends(WorksheetCollection, _super);
        function WorksheetCollection() {
            /// <summary> Represents a collection of worksheet objects that are part of the workbook. </summary>
            /// <field name="context" type="Excel.RequestContext">The request context associated with this object</field>
            /// <field name="items" type="Array" elementType="Excel.Worksheet">Gets the loaded child items in this collection.</field>
        }

        WorksheetCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Excel.WorksheetCollection"/>
        }
        WorksheetCollection.prototype.add = function(name) {
            /// <summary>
            /// Adds a new worksheet to the workbook. The worksheet will be added at the end of existing worksheets. If you wish to activate the newly added worksheet, call &quot;.activate() on it.
            /// </summary>
            /// <param name="name" type="String" optional="true">The name of the worksheet to be added. If specified, name should be unqiue. If not specified, Excel determines the name of the new worksheet.</param>
            /// <returns type="Excel.Worksheet"></returns>
        }
        WorksheetCollection.prototype.getActiveWorksheet = function() {
            /// <summary>
            /// Gets the currently active worksheet in the workbook.
            /// </summary>
            /// <returns type="Excel.Worksheet"></returns>
        }
        WorksheetCollection.prototype.getItem = function(key) {
            /// <summary>
            /// Gets a worksheet object using its Name or ID.
            /// </summary>
            /// <param name="key" type="String">The Name or ID of the worksheet.</param>
            /// <returns type="Excel.Worksheet"></returns>
        }
        return WorksheetCollection;
    })(OfficeExtension.ClientObject);
    Excel.WorksheetCollection = WorksheetCollection;
})(Excel || (Excel = {}));
var Excel;
(function (Excel) {
    var RequestContext = (function (_super) {
        __extends(RequestContext, _super);
        function RequestContext() {
            /// <summary>
            /// The RequestContext object facilitates requests to the Excel application. Since the Office add-in and the Excel application run in two different processes, the request context is required to get access to the Excel object model from the add-in.
            /// </summary>
            /// <field name="workbook" type="Excel.Workbook">Root object for interacting with the document</field>
            _super.call(this, null);
        }
        return RequestContext;
    })(OfficeExtension.ClientRequestContext);
    Excel.RequestContext = RequestContext;

    Excel.run = function (batch) {
        /// <summary>
        /// Executes a batch script that performs actions on the Excel object model. When the promise is resolved, any tracked objects that were automatically allocated during execution will be released.
        /// </summary>
        /// <param name="batch" type="function(context) { ... }">
        /// A function that takes in a RequestContext and returns a promise (typically, just the result of "context.sync()").
        /// <br />
        /// The context parameter facilitates requests to the Excel application. Since the Office add-in and the Excel application run in two different processes, the request context is required to get access to the Excel object model from the add-in.
        /// </param>
        batch(new Excel.RequestContext());
        return new OfficeExtension.IPromise();
    }
})(Excel || (Excel = {}));
Excel.__proto__ = null;


var Word;
(function (Word) {
    var Alignment = {
        __proto__: null,
        "unknown": "unknown",
        "left": "left",
        "centered": "centered",
        "right": "right",
        "justified": "justified",
    }
    Word.Alignment = Alignment;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var Body = (function(_super) {
        __extends(Body, _super);
        function Body() {
            /// <summary> Represents the body of a document or a section. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="contentControls" type="Word.ContentControlCollection">Gets the collection of rich text content control objects that are in the body. Read-only.</field>
            /// <field name="font" type="Word.Font">Gets the text format of the body. Use this to get and set font name, size, color, and other properties. Read-only.</field>
            /// <field name="inlinePictures" type="Word.InlinePictureCollection">Gets the collection of inlinePicture objects that are in the body. The collection does not include floating images. Read-only.</field>
            /// <field name="paragraphs" type="Word.ParagraphCollection">Gets the collection of paragraph objects that are in the body. Read-only.</field>
            /// <field name="parentContentControl" type="Word.ContentControl">Gets the content control that contains the body. Returns null if there isn&apos;t a parent content control. Read-only.</field>
            /// <field name="style" type="String">Gets or sets the style used for the body. This is the name of the pre-installed or custom style.</field>
            /// <field name="text" type="String">Gets the text of the body. Use the insertText method to insert text. Read-only.</field>
        }

        Body.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.Body"/>
        }
        Body.prototype.clear = function() {
            /// <summary>
            /// Clears the contents of the body object. The user can perform the undo operation on the cleared content.
            /// </summary>
            /// <returns ></returns>
        }
        Body.prototype.getHtml = function() {
            /// <summary>
            /// Gets the HTML representation of the body object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        Body.prototype.getOoxml = function() {
            /// <summary>
            /// Gets the OOXML (Office Open XML) representation of the body object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        Body.prototype.insertBreak = function(breakType, insertLocation) {
            /// <summary>
            /// Inserts a break at the specified location. The insertLocation value can be &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="breakType" type="String">Required. The break type to add to the body.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns ></returns>
        }
        Body.prototype.insertContentControl = function() {
            /// <summary>
            /// Wraps the body object with a Rich Text content control.
            /// </summary>
            /// <returns type="Word.ContentControl"></returns>
        }
        Body.prototype.insertFileFromBase64 = function(base64File, insertLocation) {
            /// <summary>
            /// Inserts a document into the body at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="base64File" type="String">Required. The base64 encoded file contents to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Body.prototype.insertHtml = function(html, insertLocation) {
            /// <summary>
            /// Inserts HTML at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="html" type="String">Required. The HTML to be inserted in the document.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Body.prototype.insertOoxml = function(ooxml, insertLocation) {
            /// <summary>
            /// Inserts OOXML at the specified location.  The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="ooxml" type="String">Required. The OOXML to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Body.prototype.insertParagraph = function(paragraphText, insertLocation) {
            /// <summary>
            /// Inserts a paragraph at the specified location. The insertLocation value can be &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="paragraphText" type="String">Required. The paragraph text to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Paragraph"></returns>
        }
        Body.prototype.insertText = function(text, insertLocation) {
            /// <summary>
            /// Inserts text into the body at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="text" type="String">Required. Text to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Body.prototype.search = function(searchText, searchOptions) {
            /// <summary>
            /// Performs a search with the specified searchOptions on the scope of the body object. The search results are a collection of range objects.
            /// </summary>
            /// <param name="searchText" type="String">Required. The search text.</param>
            /// <param name="searchOptions" type="Word.SearchOptions" optional="true">Optional. Options for the search.</param>
            /// <returns type="Word.SearchResultCollection"></returns>
        }
        Body.prototype.select = function() {
            /// <summary>
            /// Selects the body and navigates the Word UI to it.
            /// </summary>
            /// <returns ></returns>
        }
        return Body;
    })(OfficeExtension.ClientObject);
    Word.Body = Body;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var BreakType = {
        __proto__: null,
        "page": "page",
        "column": "column",
        "next": "next",
        "sectionContinuous": "sectionContinuous",
        "sectionEven": "sectionEven",
        "sectionOdd": "sectionOdd",
        "line": "line",
        "lineClearLeft": "lineClearLeft",
        "lineClearRight": "lineClearRight",
        "textWrapping": "textWrapping",
    }
    Word.BreakType = BreakType;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var ContentControl = (function(_super) {
        __extends(ContentControl, _super);
        function ContentControl() {
            /// <summary> Represents a content control. Content controls are bounded and potentially labeled regions in a document that serve as containers for specific types of content. Individual content controls may contain contents such as images, tables, or paragraphs of formatted text. Currently, only rich text content controls are supported. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="appearance" type="String">Gets or sets the appearance of the content control. The value can be &apos;boundingBox&apos;, &apos;tags&apos; or &apos;hidden&apos;.</field>
            /// <field name="cannotDelete" type="Boolean">Gets or sets a value that indicates whether the user can delete the content control. Mutually exclusive with removeWhenEdited.</field>
            /// <field name="cannotEdit" type="Boolean">Gets or sets a value that indicates whether the user can edit the contents of the content control.</field>
            /// <field name="color" type="String">Gets or sets the color of the content control. Color is set in &quot;#RRGGBB&quot; format or by using the color name.</field>
            /// <field name="contentControls" type="Word.ContentControlCollection">Gets the collection of content control objects in the content control. Read-only.</field>
            /// <field name="font" type="Word.Font">Gets the text format of the content control. Use this to get and set font name, size, color, and other properties. Read-only.</field>
            /// <field name="id" type="Number">Gets an integer that represents the content control identifier. Read-only.</field>
            /// <field name="inlinePictures" type="Word.InlinePictureCollection">Gets the collection of inlinePicture objects in the content control. The collection does not include floating images. Read-only.</field>
            /// <field name="paragraphs" type="Word.ParagraphCollection">Get the collection of paragraph objects in the content control. Read-only.</field>
            /// <field name="parentContentControl" type="Word.ContentControl">Gets the content control that contains the content control. Returns null if there isn&apos;t a parent content control. Read-only.</field>
            /// <field name="placeholderText" type="String">Gets or sets the placeholder text of the content control. Dimmed text will be displayed when the content control is empty.</field>
            /// <field name="removeWhenEdited" type="Boolean">Gets or sets a value that indicates whether the content control is removed after it is edited. Mutually exclusive with cannotDelete.</field>
            /// <field name="style" type="String">Gets or sets the style used for the content control. This is the name of the pre-installed or custom style.</field>
            /// <field name="tag" type="String">Gets or sets a tag to identify a content control.</field>
            /// <field name="text" type="String">Gets the text of the content control. Read-only.</field>
            /// <field name="title" type="String">Gets or sets the title for a content control.</field>
            /// <field name="type" type="String">Gets the content control type. Only rich text content controls are supported currently. Read-only.</field>
        }

        ContentControl.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.ContentControl"/>
        }
        ContentControl.prototype.clear = function() {
            /// <summary>
            /// Clears the contents of the content control. The user can perform the undo operation on the cleared content.
            /// </summary>
            /// <returns ></returns>
        }
        ContentControl.prototype.delete = function(keepContent) {
            /// <summary>
            /// Deletes the content control and its content. If keepContent is set to true, the content is not deleted.
            /// </summary>
            /// <param name="keepContent" type="Boolean">Required. Indicates whether the content should be deleted with the content control. If keepContent is set to true, the content is not deleted.</param>
            /// <returns ></returns>
        }
        ContentControl.prototype.getHtml = function() {
            /// <summary>
            /// Gets the HTML representation of the content control object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        ContentControl.prototype.getOoxml = function() {
            /// <summary>
            /// Gets the Office Open XML (OOXML) representation of the content control object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        ContentControl.prototype.insertBreak = function(breakType, insertLocation) {
            /// <summary>
            /// Inserts a break at the specified location. The insertLocation value can be &apos;Before&apos;, &apos;After&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="breakType" type="String">Required. Type of break (breakType.md)</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Before&apos;, &apos;After&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns ></returns>
        }
        ContentControl.prototype.insertFileFromBase64 = function(base64File, insertLocation) {
            /// <summary>
            /// Inserts a document into the current content control at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="base64File" type="String">Required. Base64 encoded contents of the file to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        ContentControl.prototype.insertHtml = function(html, insertLocation) {
            /// <summary>
            /// Inserts HTML into the content control at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="html" type="String">Required. The HTML to be inserted in to the content control.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        ContentControl.prototype.insertOoxml = function(ooxml, insertLocation) {
            /// <summary>
            /// Inserts OOXML into the content control at the specified location.  The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="ooxml" type="String">Required. The OOXML to be inserted in to the content control.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        ContentControl.prototype.insertParagraph = function(paragraphText, insertLocation) {
            /// <summary>
            /// Inserts a paragraph at the specified location. The insertLocation value can be &apos;Before&apos;, &apos;After&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="paragraphText" type="String">Required. The paragrph text to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Before&apos;, &apos;After&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Paragraph"></returns>
        }
        ContentControl.prototype.insertText = function(text, insertLocation) {
            /// <summary>
            /// Inserts text into the content control at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="text" type="String">Required. The text to be inserted in to the content control.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        ContentControl.prototype.search = function(searchText, searchOptions) {
            /// <summary>
            /// Performs a search with the specified searchOptions on the scope of the content control object. The search results are a collection of range objects.
            /// </summary>
            /// <param name="searchText" type="String">Required. The search text.</param>
            /// <param name="searchOptions" type="Word.SearchOptions" optional="true">Optional. Options for the search.</param>
            /// <returns type="Word.SearchResultCollection"></returns>
        }
        ContentControl.prototype.select = function() {
            /// <summary>
            /// Selects the content control. This causes Word to scroll to the selection.
            /// </summary>
            /// <returns ></returns>
        }
        return ContentControl;
    })(OfficeExtension.ClientObject);
    Word.ContentControl = ContentControl;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var ContentControlAppearance = {
        __proto__: null,
        "boundingBox": "boundingBox",
        "tags": "tags",
        "hidden": "hidden",
    }
    Word.ContentControlAppearance = ContentControlAppearance;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var ContentControlCollection = (function(_super) {
        __extends(ContentControlCollection, _super);
        function ContentControlCollection() {
            /// <summary> Contains a collection of ContentControl objects. Content controls are bounded and potentially labeled regions in a document that serve as containers for specific types of content. Individual content controls may contain contents such as images, tables, or paragraphs of formatted text. Currently, only rich text content controls are supported. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="items" type="Array" elementType="Word.ContentControl">Gets the loaded child items in this collection.</field>
        }

        ContentControlCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.ContentControlCollection"/>
        }
        ContentControlCollection.prototype.getById = function(id) {
            /// <summary>
            /// Gets a content control by its identifier.
            /// </summary>
            /// <param name="id" type="Number">Required. A content control identifier.</param>
            /// <returns type="Word.ContentControl"></returns>
        }
        ContentControlCollection.prototype.getByTag = function(tag) {
            /// <summary>
            /// Gets the content controls that have the specified tag.
            /// </summary>
            /// <param name="tag" type="String">Required. A tag set on a content control.</param>
            /// <returns type="Word.ContentControlCollection"></returns>
        }
        ContentControlCollection.prototype.getByTitle = function(title) {
            /// <summary>
            /// Gets the content controls that have the specified title.
            /// </summary>
            /// <param name="title" type="String">Required. The title of a content control.</param>
            /// <returns type="Word.ContentControlCollection"></returns>
        }
        ContentControlCollection.prototype.getItem = function(index) {
            /// <summary>
            /// Gets a content control by its index in the collection.
            /// </summary>
            /// <param name="index" >The index</param>
            /// <returns type="Word.ContentControl"></returns>
        }
        return ContentControlCollection;
    })(OfficeExtension.ClientObject);
    Word.ContentControlCollection = ContentControlCollection;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var ContentControlType = {
        __proto__: null,
        "richText": "richText",
    }
    Word.ContentControlType = ContentControlType;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var Document = (function(_super) {
        __extends(Document, _super);
        function Document() {
            /// <summary> The Document object is the top level object. A Document object contains one or more sections, content controls, and the body that contains the contents of the document. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="body" type="Word.Body">Gets the body of the document. The body is the text that excludes headers, footers, footnotes, textboxes, etc.. Read-only.</field>
            /// <field name="contentControls" type="Word.ContentControlCollection">Gets the collection of content control objects that are in the current document. This includes content controls in the body of the document, headers, footers, textboxes, etc.. Read-only.</field>
            /// <field name="saved" type="Boolean">Indicates whether the changes in the document have been saved. A value of true indicates that the document hasn&apos;t changed since it was saved. Read-only.</field>
            /// <field name="sections" type="Word.SectionCollection">Gets the collection of section objects that are in the document. Read-only.</field>
        }

        Document.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.Document"/>
        }
        Document.prototype.getSelection = function() {
            /// <summary>
            /// Gets the current selection of the document. Multiple selections are not supported.
            /// </summary>
            /// <returns type="Word.Range"></returns>
        }
        Document.prototype.save = function() {
            /// <summary>
            /// Saves the document. This will use the Word default file naming convention if the document has not been saved before.
            /// </summary>
            /// <returns ></returns>
        }
        return Document;
    })(OfficeExtension.ClientObject);
    Word.Document = Document;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var Font = (function(_super) {
        __extends(Font, _super);
        function Font() {
            /// <summary> Represents a font. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="bold" type="Boolean">Gets or sets a value that indicates whether the font is bold. True if the font is formatted as bold, otherwise, false.</field>
            /// <field name="color" type="String">Gets or sets the color for the specified font. You can provide the value in the &quot;#RRGGBB&quot; format or the color name.</field>
            /// <field name="doubleStrikeThrough" type="Boolean">Gets or sets a value that indicates whether the font has a double strike through. True if the font is formatted as double strikethrough text, otherwise, false.</field>
            /// <field name="highlightColor" type="String">Gets or sets the highlight color for the specified font. You can provide the value as either in the &quot;#RRGGBB&quot; format or the color name.</field>
            /// <field name="italic" type="Boolean">Gets or sets a value that indicates whether the font is italicized. True if the font is italicized, otherwise, false.</field>
            /// <field name="name" type="String">Gets or sets a value that represents the name of the font.</field>
            /// <field name="size" type="Number">Gets or sets a value that represents the font size in points.</field>
            /// <field name="strikeThrough" type="Boolean">Gets or sets a value that indicates whether the font has a strike through. True if the font is formatted as strikethrough text, otherwise, false.</field>
            /// <field name="subscript" type="Boolean">Gets or sets a value that indicates whether the font is a subscript. True if the font is formatted as subscript, otherwise, false.</field>
            /// <field name="superscript" type="Boolean">Gets or sets a value that indicates whether the font is a superscript. True if the font is formatted as superscript, otherwise, false.</field>
            /// <field name="underline" type="String">Gets or sets a value that indicates the font&apos;s underline type. &apos;None&apos; if the font is not underlined.</field>
        }

        Font.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.Font"/>
        }
        return Font;
    })(OfficeExtension.ClientObject);
    Word.Font = Font;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var HeaderFooterType = {
        __proto__: null,
        "primary": "primary",
        "firstPage": "firstPage",
        "evenPages": "evenPages",
    }
    Word.HeaderFooterType = HeaderFooterType;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var InlinePicture = (function(_super) {
        __extends(InlinePicture, _super);
        function InlinePicture() {
            /// <summary> Represents an inline picture. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="altTextDescription" type="String">Gets or sets a string that represents the alternative text associated with the inline image</field>
            /// <field name="altTextTitle" type="String">Gets or sets a string that contains the title for the inline image.</field>
            /// <field name="height" type="Number">Gets or sets a number that describes the height of the inline image.</field>
            /// <field name="hyperlink" type="String">Gets or sets the hyperlink associated with the inline image.</field>
            /// <field name="lockAspectRatio" type="Boolean">Gets or sets a value that indicates whether the inline image retains its original proportions when you resize it.</field>
            /// <field name="parentContentControl" type="Word.ContentControl">Gets the content control that contains the inline image. Returns null if there isn&apos;t a parent content control. Read-only.</field>
            /// <field name="width" type="Number">Gets or sets a number that describes the width of the inline image.</field>
        }

        InlinePicture.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.InlinePicture"/>
        }
        InlinePicture.prototype.getBase64ImageSrc = function() {
            /// <summary>
            /// Gets the base64 encoded string representation of the inline image.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        InlinePicture.prototype.insertContentControl = function() {
            /// <summary>
            /// Wraps the inline picture with a rich text content control.
            /// </summary>
            /// <returns type="Word.ContentControl"></returns>
        }
        return InlinePicture;
    })(OfficeExtension.ClientObject);
    Word.InlinePicture = InlinePicture;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var InlinePictureCollection = (function(_super) {
        __extends(InlinePictureCollection, _super);
        function InlinePictureCollection() {
            /// <summary> Contains a collection of [inlinePicture](inlinePicture.md) objects. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="items" type="Array" elementType="Word.InlinePicture">Gets the loaded child items in this collection.</field>
        }

        InlinePictureCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.InlinePictureCollection"/>
        }
        return InlinePictureCollection;
    })(OfficeExtension.ClientObject);
    Word.InlinePictureCollection = InlinePictureCollection;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var InsertLocation = {
        __proto__: null,
        "before": "before",
        "after": "after",
        "start": "start",
        "end": "end",
        "replace": "replace",
    }
    Word.InsertLocation = InsertLocation;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var Paragraph = (function(_super) {
        __extends(Paragraph, _super);
        function Paragraph() {
            /// <summary> Represents a single paragraph in a selection, range, content control, or document body. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="alignment" type="String">Gets or sets the alignment for a paragraph. The value can  be &quot;left&quot;, &quot;centered&quot;, &quot;right&quot;, or &quot;justified&quot;.</field>
            /// <field name="contentControls" type="Word.ContentControlCollection">Gets the collection of content control objects that are in the paragraph. Read-only.</field>
            /// <field name="firstLineIndent" type="Number">Gets or sets the value, in points, for a first line or hanging indent. Use a positive value to set a first-line indent, and use a negative value to set a hanging indent.</field>
            /// <field name="font" type="Word.Font">Gets the text format of the paragraph. Use this to get and set font name, size, color, and other properties. Read-only.</field>
            /// <field name="inlinePictures" type="Word.InlinePictureCollection">Gets the collection of inlinePicture objects that are in the paragraph. The collection does not include floating images. Read-only.</field>
            /// <field name="leftIndent" type="Number">Gets or sets the left indent value, in points, for the paragraph.</field>
            /// <field name="lineSpacing" type="Number">Gets or sets the line spacing, in points, for the specified paragraph. In the Word UI, this value is divided by 12.</field>
            /// <field name="lineUnitAfter" type="Number">Gets or sets the amount of spacing, in grid lines. after the paragraph.</field>
            /// <field name="lineUnitBefore" type="Number">Gets or sets the amount of spacing, in grid lines, before the paragraph.</field>
            /// <field name="outlineLevel" type="Number">Gets or sets the outline level for the paragraph.</field>
            /// <field name="parentContentControl" type="Word.ContentControl">Gets the content control that contains the paragraph. Returns null if there isn&apos;t a parent content control. Read-only.</field>
            /// <field name="rightIndent" type="Number">Gets or sets the right indent value, in points, for the paragraph.</field>
            /// <field name="spaceAfter" type="Number">Gets or sets the spacing, in points, after the paragraph.</field>
            /// <field name="spaceBefore" type="Number">Gets or sets the spacing, in points, before the paragraph.</field>
            /// <field name="style" type="String">Gets or sets the style used for the paragraph. This is the name of the pre-installed or custom style.</field>
            /// <field name="text" type="String">Gets the text of the paragraph. Read-only.</field>
        }

        Paragraph.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.Paragraph"/>
        }
        Paragraph.prototype.clear = function() {
            /// <summary>
            /// Clears the contents of the paragraph object. The user can perform the undo operation on the cleared content.
            /// </summary>
            /// <returns ></returns>
        }
        Paragraph.prototype.delete = function() {
            /// <summary>
            /// Deletes the paragraph and its content from the document.
            /// </summary>
            /// <returns ></returns>
        }
        Paragraph.prototype.getHtml = function() {
            /// <summary>
            /// Gets the HTML representation of the paragraph object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        Paragraph.prototype.getOoxml = function() {
            /// <summary>
            /// Gets the Office Open XML (OOXML) representation of the paragraph object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        Paragraph.prototype.insertBreak = function(breakType, insertLocation) {
            /// <summary>
            /// Inserts a break at the specified location. The insertLocation value can be &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="breakType" type="String">Required. The break type to add to the document.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Before&apos; or &apos;After&apos;.</param>
            /// <returns ></returns>
        }
        Paragraph.prototype.insertContentControl = function() {
            /// <summary>
            /// Wraps the paragraph object with a rich text content control.
            /// </summary>
            /// <returns type="Word.ContentControl"></returns>
        }
        Paragraph.prototype.insertFileFromBase64 = function(base64File, insertLocation) {
            /// <summary>
            /// Inserts a document into the current paragraph at the specified location. The insertLocation value can be &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="base64File" type="String">Required. The file base64 encoded file contents to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Paragraph.prototype.insertHtml = function(html, insertLocation) {
            /// <summary>
            /// Inserts HTML into the paragraph at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="html" type="String">Required. The HTML to be inserted in the paragraph.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Paragraph.prototype.insertInlinePictureFromBase64 = function(base64EncodedImage, insertLocation) {
            /// <summary>
            /// Inserts a picture into the paragraph at the specified location. The insertLocation value can be &apos;Before&apos;, &apos;After&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="base64EncodedImage" type="String">Required. The HTML to be inserted in the paragraph.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Before&apos;, &apos;After&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.InlinePicture"></returns>
        }
        Paragraph.prototype.insertOoxml = function(ooxml, insertLocation) {
            /// <summary>
            /// Inserts OOXML into the paragraph at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="ooxml" type="String">Required. The OOXML to be inserted in the paragraph.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Paragraph.prototype.insertParagraph = function(paragraphText, insertLocation) {
            /// <summary>
            /// Inserts a paragraph at the specified location. The insertLocation value can be &apos;Before&apos; or &apos;After&apos;.
            /// </summary>
            /// <param name="paragraphText" type="String">Required. The paragraph text to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Before&apos; or &apos;After&apos;.</param>
            /// <returns type="Word.Paragraph"></returns>
        }
        Paragraph.prototype.insertText = function(text, insertLocation) {
            /// <summary>
            /// Inserts text into the paragraph at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="text" type="String">Required. Text to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Paragraph.prototype.search = function(searchText, searchOptions) {
            /// <summary>
            /// Performs a search with the specified searchOptions on the scope of the paragraph object. The search results are a collection of range objects.
            /// </summary>
            /// <param name="searchText" type="String">Required. The search text.</param>
            /// <param name="searchOptions" type="Word.SearchOptions" optional="true">Optional. Options for the search.</param>
            /// <returns type="Word.SearchResultCollection"></returns>
        }
        Paragraph.prototype.select = function() {
            /// <summary>
            /// Selects and navigates the Word UI to the paragraph.
            /// </summary>
            /// <returns ></returns>
        }
        return Paragraph;
    })(OfficeExtension.ClientObject);
    Word.Paragraph = Paragraph;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var ParagraphCollection = (function(_super) {
        __extends(ParagraphCollection, _super);
        function ParagraphCollection() {
            /// <summary> Contains a collection of [paragraph](paragraph.md) objects. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="items" type="Array" elementType="Word.Paragraph">Gets the loaded child items in this collection.</field>
        }

        ParagraphCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.ParagraphCollection"/>
        }
        return ParagraphCollection;
    })(OfficeExtension.ClientObject);
    Word.ParagraphCollection = ParagraphCollection;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var Range = (function(_super) {
        __extends(Range, _super);
        function Range() {
            /// <summary> Represents a contiguous area in a document. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="contentControls" type="Word.ContentControlCollection">Gets the collection of content control objects that are in the range. Read-only.</field>
            /// <field name="font" type="Word.Font">Gets the text format of the range. Use this to get and set font name, size, color, and other properties. Read-only.</field>
            /// <field name="paragraphs" type="Word.ParagraphCollection">Gets the collection of paragraph objects that are in the range. Read-only.</field>
            /// <field name="parentContentControl" type="Word.ContentControl">Gets the content control that contains the range. Returns null if there isn&apos;t a parent content control. Read-only.</field>
            /// <field name="style" type="String">Gets or sets the style used for the range. This is the name of the pre-installed or custom style.</field>
            /// <field name="text" type="String">Gets the text of the range. Read-only.</field>
        }

        Range.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.Range"/>
        }
        Range.prototype.clear = function() {
            /// <summary>
            /// Clears the contents of the range object. The user can perform the undo operation on the cleared content.
            /// </summary>
            /// <returns ></returns>
        }
        Range.prototype.delete = function() {
            /// <summary>
            /// Deletes the range and its content from the document.
            /// </summary>
            /// <returns ></returns>
        }
        Range.prototype.getHtml = function() {
            /// <summary>
            /// Gets the HTML representation of the range object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        Range.prototype.getOoxml = function() {
            /// <summary>
            /// Gets the OOXML representation of the range object.
            /// </summary>
            /// <returns type="OfficeExtension.ClientResult&lt;string&gt;"></returns>
            var result = new OfficeExtension.ClientResult();
            result.__proto__ = null;
            result.value = '';
            return result;
        }
        Range.prototype.insertBreak = function(breakType, insertLocation) {
            /// <summary>
            /// Inserts a break at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Before&apos; or &apos;After&apos;.
            /// </summary>
            /// <param name="breakType" type="String">Required. The break type to add to the range.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Before&apos; or &apos;After&apos;.</param>
            /// <returns ></returns>
        }
        Range.prototype.insertContentControl = function() {
            /// <summary>
            /// Wraps the range object with a rich text content control.
            /// </summary>
            /// <returns type="Word.ContentControl"></returns>
        }
        Range.prototype.insertFileFromBase64 = function(base64File, insertLocation) {
            /// <summary>
            /// Inserts a document into the range at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="base64File" type="String">Required. The file base64 encoded file contents to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Range.prototype.insertHtml = function(html, insertLocation) {
            /// <summary>
            /// Inserts HTML into the range at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="html" type="String">Required. The HTML to be inserted in the range.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Range.prototype.insertOoxml = function(ooxml, insertLocation) {
            /// <summary>
            /// Inserts OOXML into the range at the specified location.  The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="ooxml" type="String">Required. The OOXML to be inserted in the range.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Range.prototype.insertParagraph = function(paragraphText, insertLocation) {
            /// <summary>
            /// Inserts a paragraph into the range at the specified location. The insertLocation value can be &apos;Before&apos; or &apos;After&apos;.
            /// </summary>
            /// <param name="paragraphText" type="String">Required. The paragraph text to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Before&apos; or &apos;After&apos;.</param>
            /// <returns type="Word.Paragraph"></returns>
        }
        Range.prototype.insertText = function(text, insertLocation) {
            /// <summary>
            /// Inserts text into the range at the specified location. The insertLocation value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.
            /// </summary>
            /// <param name="text" type="String">Required. Text to be inserted.</param>
            /// <param name="insertLocation" type="String">Required. The value can be &apos;Replace&apos;, &apos;Start&apos; or &apos;End&apos;.</param>
            /// <returns type="Word.Range"></returns>
        }
        Range.prototype.search = function(searchText, searchOptions) {
            /// <summary>
            /// Performs a search with the specified searchOptions on the scope of the range object. The search results are a collection of range objects.
            /// </summary>
            /// <param name="searchText" type="String">Required. The search text.</param>
            /// <param name="searchOptions" type="Word.SearchOptions" optional="true">Optional. Options for the search.</param>
            /// <returns type="Word.SearchResultCollection"></returns>
        }
        Range.prototype.select = function() {
            /// <summary>
            /// Selects and navigates the Word UI to the range.
            /// </summary>
            /// <returns ></returns>
        }
        return Range;
    })(OfficeExtension.ClientObject);
    Word.Range = Range;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var SearchOptions = (function(_super) {
        __extends(SearchOptions, _super);
        function SearchOptions() {
            /// <summary> Specifies the options to be included in a search operation. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="ignorePunct" type="Boolean">Gets or sets a value that indicates whether to ignore all punctuation characters between words. Corresponds to the Ignore punctuation check box in the Find and Replace dialog box.</field>
            /// <field name="ignoreSpace" type="Boolean">Gets or sets a value that indicates whether to ignore all white space between words. Corresponds to the Ignore white-space characters check box in the Find and Replace dialog box.</field>
            /// <field name="matchCase" type="Boolean">Gets or sets a value that indicates whether to perform a case sensitive search. Corresponds to the Match case check box in the Find and Replace dialog box (Edit menu).</field>
            /// <field name="matchPrefix" type="Boolean">Gets or sets a value that indicates whether to match words that begin with the search string. Corresponds to the Match prefix check box in the Find and Replace dialog box.</field>
            /// <field name="matchSoundsLike" type="Boolean">Gets or sets a value that indicates whether to find words that sound similar to the search string. Corresponds to the Sounds like check box in the Find and Replace dialog box</field>
            /// <field name="matchSuffix" type="Boolean">Gets or sets a value that indicates whether to match words that end with the search string. Corresponds to the Match suffix check box in the Find and Replace dialog box.</field>
            /// <field name="matchWholeWord" type="Boolean">Gets or sets a value that indicates whether to find operation only entire words, not text that is part of a larger word. Corresponds to the Find whole words only check box in the Find and Replace dialog box.</field>
            /// <field name="matchWildCards" type="Boolean">Gets or sets a value that indicates whether the search will be performed using special search operators. Corresponds to the Use wildcards check box in the Find and Replace dialog box.</field>
        }

        SearchOptions.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.SearchOptions"/>
        }
        return SearchOptions;
    })(OfficeExtension.ClientObject);
    Word.SearchOptions = SearchOptions;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var SearchResultCollection = (function(_super) {
        __extends(SearchResultCollection, _super);
        function SearchResultCollection() {
            /// <summary> Contains a collection of [range](range.md) objects as a result of a search operation. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="items" type="Array" elementType="Word.Range">Gets the loaded child items in this collection.</field>
        }

        SearchResultCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.SearchResultCollection"/>
        }
        return SearchResultCollection;
    })(OfficeExtension.ClientObject);
    Word.SearchResultCollection = SearchResultCollection;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var Section = (function(_super) {
        __extends(Section, _super);
        function Section() {
            /// <summary> Represents a section in a Word document. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="body" type="Word.Body">Gets the body of the section. This does not include the header/footer and other section metadata. Read-only.</field>
        }

        Section.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.Section"/>
        }
        Section.prototype.getFooter = function(type) {
            /// <summary>
            /// Gets one of the section&apos;s footers.
            /// </summary>
            /// <param name="type" type="String">Required. The type of footer to return. This value can be: &apos;primary&apos;, &apos;firstPage&apos; or &apos;evenPages&apos;.</param>
            /// <returns type="Word.Body"></returns>
        }
        Section.prototype.getHeader = function(type) {
            /// <summary>
            /// Gets one of the section&apos;s headers.
            /// </summary>
            /// <param name="type" type="String">Required. The type of header to return. This value can be: &apos;primary&apos;, &apos;firstPage&apos; or &apos;evenPages&apos;.</param>
            /// <returns type="Word.Body"></returns>
        }
        return Section;
    })(OfficeExtension.ClientObject);
    Word.Section = Section;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var SectionCollection = (function(_super) {
        __extends(SectionCollection, _super);
        function SectionCollection() {
            /// <summary> Contains the collection of the document&apos;s [section](section.md) objects. </summary>
            /// <field name="context" type="Word.RequestContext">The request context associated with this object</field>
            /// <field name="items" type="Array" elementType="Word.Section">Gets the loaded child items in this collection.</field>
        }

        SectionCollection.prototype.load = function(option) {
            /// <summary>
            /// Queues up a command to load the specified properties of the object. You must call "context.sync()" before reading the properties.
            /// </summary>
            /// <param name="option" type="string | string[] | OfficeExtension.LoadOption"/>
            /// <returns type="Word.SectionCollection"/>
        }
        return SectionCollection;
    })(OfficeExtension.ClientObject);
    Word.SectionCollection = SectionCollection;
})(Word || (Word = {}));

var Word;
(function (Word) {
    var UnderlineType = {
        __proto__: null,
        "none": "none",
        "single": "single",
        "word": "word",
        "double": "double",
        "dotted": "dotted",
        "hidden": "hidden",
        "thick": "thick",
        "dashLine": "dashLine",
        "dotLine": "dotLine",
        "dotDashLine": "dotDashLine",
        "twoDotDashLine": "twoDotDashLine",
        "wave": "wave",
    }
    Word.UnderlineType = UnderlineType;
})(Word || (Word = {}));
var Word;
(function (Word) {
    var RequestContext = (function (_super) {
        __extends(RequestContext, _super);
        function RequestContext() {
            /// <summary>
            /// The RequestContext object facilitates requests to the Word application. Since the Office add-in and the Word application run in two different processes, the request context is required to get access to the Word object model from the add-in.
            /// </summary>
            /// <field name="document" type="Word.Document">Root object for interacting with the document</field>
            _super.call(this, null);
        }
        return RequestContext;
    })(OfficeExtension.ClientRequestContext);
    Word.RequestContext = RequestContext;

    Word.run = function (batch) {
        /// <summary>
        /// Executes a batch script that performs actions on the Word object model. When the promise is resolved, any tracked objects that were automatically allocated during execution will be released.
        /// </summary>
        /// <param name="batch" type="function(context) { ... }">
        /// A function that takes in a RequestContext and returns a promise (typically, just the result of "context.sync()").
        /// <br />
        /// The context parameter facilitates requests to the Word application. Since the Office add-in and the Word application run in two different processes, the request context is required to get access to the Word object model from the add-in.
        /// </param>
        batch(new Word.RequestContext());
        return new OfficeExtension.IPromise();
    }
})(Word || (Word = {}));
Word.__proto__ = null;

