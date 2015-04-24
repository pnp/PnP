/* Excel specific API library */
/* Version: 15.0.4615.1000 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/

OSF.OUtil.augmentList(Microsoft.Office.WebExtension.FilterType, {
	OnlyVisible: "onlyVisible"
});
OSF.ClientMode={
	ReadWrite: 0,
	ReadOnly: 1
}
OSF.DDA.RichInitializationReason={
	1: Microsoft.Office.WebExtension.InitializationReason.Inserted,
	2: Microsoft.Office.WebExtension.InitializationReason.DocumentOpened
};
Microsoft.Office.WebExtension.FileType={
	Text: "text",
	Compressed: "compressed"
};
OSF.DDA.RichClientSettingsManager={
	read: function OSF_DDA_RichClientSettingsManager$Read(onCalling, onReceiving) {
		var keys=[];
		var values=[];
		if (onCalling) {
			onCalling();
		}
		window.external.GetContext().GetSettings().Read(keys, values);
		if (onReceiving) {
			onReceiving();
		}
		var serializedSettings={};
		for (var index=0; index < keys.length; index++) {
			serializedSettings[keys[index]]=values[index];
		}
		return serializedSettings;
	},
	write: function OSF_DDA_RichClientSettingsManager$Write(serializedSettings, overwriteIfStale, onCalling, onReceiving) {
		var keys=[];
		var values=[];
		for (var key in serializedSettings) {
			keys.push(key);
			values.push(serializedSettings[key]);
		}
		if (onCalling) {
			onCalling();
		}
		window.external.GetContext().GetSettings().Write(keys, values);
		if (onReceiving) {
			onReceiving();
		}
	}
};
OSF.DDA.DispIdHost.getRichClientDelegateMethods=function (actionId) {
	var delegateMethods={};
	delegateMethods[OSF.DDA.DispIdHost.Delegates.ExecuteAsync]=OSF.DDA.SafeArray.Delegate.executeAsync;
	delegateMethods[OSF.DDA.DispIdHost.Delegates.RegisterEventAsync]=OSF.DDA.SafeArray.Delegate.registerEventAsync;
	delegateMethods[OSF.DDA.DispIdHost.Delegates.UnregisterEventAsync]=OSF.DDA.SafeArray.Delegate.unregisterEventAsync;
	function getSettingsExecuteMethod(hostDelegateMethod) {
		return function (args) {
			var status, response;
			try {
				response=hostDelegateMethod(args.hostCallArgs, args.onCalling, args.onReceiving);
				status=OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess;
			} catch (ex) {
				status=OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
				response={ name : Strings.OfficeOM.L_InternalError, message : ex };
			}
			if (args.onComplete) {
				args.onComplete(status, response);
			}
		};
	}
	function readSerializedSettings(hostCallArgs, onCalling, onReceiving) {
		return OSF.DDA.RichClientSettingsManager.read(onCalling, onReceiving);
	}
	function writeSerializedSettings(hostCallArgs, onCalling, onReceiving) {
		return OSF.DDA.RichClientSettingsManager.write(
			hostCallArgs[OSF.DDA.SettingsManager.SerializedSettings],
			hostCallArgs[Microsoft.Office.WebExtension.Parameters.OverwriteIfStale],
			onCalling,
			onReceiving
		);
	}
	switch (actionId) {
		case OSF.DDA.AsyncMethodNames.RefreshAsync.id:
			delegateMethods[OSF.DDA.DispIdHost.Delegates.ExecuteAsync]=getSettingsExecuteMethod(readSerializedSettings);
			break;
		case OSF.DDA.AsyncMethodNames.SaveAsync.id:
			delegateMethods[OSF.DDA.DispIdHost.Delegates.ExecuteAsync]=getSettingsExecuteMethod(writeSerializedSettings);
			break;
		default:
			break;
	}
	return delegateMethods;
}
OSF.DDA.File=function OSF_DDA_File(handle, fileSize, sliceSize) {
	OSF.OUtil.defineEnumerableProperties(this, {
		"size": {
			value: fileSize
		},
		"sliceCount": {
			value: Math.ceil(fileSize / sliceSize)
		}
	});
	var privateState={};
	privateState[OSF.DDA.FileProperties.Handle]=handle;
	privateState[OSF.DDA.FileProperties.SliceSize]=sliceSize;
	var am=OSF.DDA.AsyncMethodNames;
	OSF.DDA.DispIdHost.addAsyncMethods(
		this, [
			am.GetDocumentCopyChunkAsync,
			am.ReleaseDocumentCopyAsync
		],
		privateState
	);
}
OSF.DDA.FileSliceOffset="fileSliceoffset";
OSF.DDA.CustomXmlParts=function OSF_DDA_CustomXmlParts() {
	this._eventDispatches=[];
	var am=OSF.DDA.AsyncMethodNames;
	OSF.DDA.DispIdHost.addAsyncMethods(this, [
		am.AddDataPartAsync,
		am.GetDataPartByIdAsync,
		am.GetDataPartsByNameSpaceAsync
	]);
};
OSF.DDA.CustomXmlPart=function OSF_DDA_CustomXmlPart(customXmlParts, id, builtIn) {
	OSF.OUtil.defineEnumerableProperties(this, {
		"builtIn": {
			value: builtIn
		},
		"id": {
			value: id
		},
		"namespaceManager": {
			value: new OSF.DDA.CustomXmlPrefixMappings(id)
		}
	});
	var am=OSF.DDA.AsyncMethodNames;
	OSF.DDA.DispIdHost.addAsyncMethods(this, [
		am.DeleteDataPartAsync,
		am.GetPartNodesAsync,
		am.GetPartXmlAsync
	]);
	var customXmlPartEventDispatches=customXmlParts._eventDispatches;
	var dispatch=customXmlPartEventDispatches[id];
	if (!dispatch) {
		var et=Microsoft.Office.WebExtension.EventType;
		dispatch=new OSF.EventDispatch([
			et.DataNodeDeleted,
			et.DataNodeInserted,
			et.DataNodeReplaced
		]);
		customXmlPartEventDispatches[id]=dispatch;
	}
	OSF.DDA.DispIdHost.addEventSupport(this, dispatch);
};
OSF.DDA.CustomXmlPrefixMappings=function OSF_DDA_CustomXmlPrefixMappings(partId) {
	var am=OSF.DDA.AsyncMethodNames;
	OSF.DDA.DispIdHost.addAsyncMethods(
		this,
		[
			am.AddDataPartNamespaceAsync,
			am.GetDataPartNamespaceAsync,
			am.GetDataPartPrefixAsync
		],
		partId
	);
};
OSF.DDA.CustomXmlNode=function OSF_DDA_CustomXmlNode(handle, nodeType, ns, baseName) {
	OSF.OUtil.defineEnumerableProperties(this, {
		"baseName": {
			value: baseName
		},
		"namespaceUri": {
			value: ns
		},
		"nodeType": {
			value: nodeType
		}
	});
	var am=OSF.DDA.AsyncMethodNames;
	OSF.DDA.DispIdHost.addAsyncMethods(
		this,
		[
			am.GetRelativeNodesAsync,
			am.GetNodeValueAsync,
			am.GetNodeXmlAsync,
			am.SetNodeValueAsync,
			am.SetNodeXmlAsync
		],
		handle
	);
};
OSF.DDA.NodeInsertedEventArgs=function OSF_DDA_NodeInsertedEventArgs(newNode, inUndoRedo) {
	OSF.OUtil.defineEnumerableProperties(this, {
		"type": {
			value: Microsoft.Office.WebExtension.EventType.DataNodeInserted
		},
		"newNode": {
			value: newNode
		},
		"inUndoRedo": {
			value: inUndoRedo
		}
	});
};
OSF.DDA.NodeReplacedEventArgs=function OSF_DDA_NodeReplacedEventArgs(oldNode, newNode, inUndoRedo) {
	OSF.OUtil.defineEnumerableProperties(this, {
		"type": {
			value: Microsoft.Office.WebExtension.EventType.DataNodeReplaced
		},
		"oldNode": {
			value: oldNode
		},
		"newNode": {
			value: newNode
		},
		"inUndoRedo": {
			value: inUndoRedo
		}
	});
};
OSF.DDA.NodeDeletedEventArgs=function OSF_DDA_NodeDeletedEventArgs(oldNode, oldNextSibling, inUndoRedo) {
	OSF.OUtil.defineEnumerableProperties(this, {
		"type": {
			value: Microsoft.Office.WebExtension.EventType.DataNodeDeleted
		},
		"oldNode": {
			value: oldNode
		},
		"oldNextSibling": {
			value: oldNextSibling
		},
		"inUndoRedo": {
			value: inUndoRedo
		}
	});
};
OSF.OUtil.setNamespace("SafeArray", OSF.DDA);
OSF.DDA.SafeArray.Response={
	Status: 0,
	Payload: 1
};
OSF.DDA.SafeArray.UniqueArguments={
	Offset: "offset",
	Run: "run",
	BindingSpecificData: "bindingSpecificData",
	MergedCellGuid: "{66e7831f-81b2-42e2-823c-89e872d541b3}"
};
OSF.OUtil.setNamespace("Delegate", OSF.DDA.SafeArray);
OSF.DDA.SafeArray.Delegate.SpecialProcessor=function OSF_DDA_SafeArray_Delegate_SpecialProcessor() {
	function _2DVBArrayToJaggedArray(vbArr) {
		var ret;
		try {
			var rows=vbArr.ubound(1);
			var cols=vbArr.ubound(2);
			vbArr=vbArr.toArray();
			if (rows==1 && cols==1) {
				ret=[vbArr];
			} else {
				ret=[];
				for (var row=0; row < rows; row++) {
					var rowArr=[];
					for (var col=0; col < cols; col++) {
						var datum=vbArr[row * cols+col];
						if (datum !=OSF.DDA.SafeArray.UniqueArguments.MergedCellGuid) {
							rowArr.push(datum);
						}
					}
					if (rowArr.length > 0) {
						ret.push(rowArr);
					}
				}
			}
		} catch (ex) {
		}
		return ret;
	}
	var complexTypes=[
		OSF.DDA.PropertyDescriptors.FileProperties,
		OSF.DDA.PropertyDescriptors.FileSliceProperties,
		OSF.DDA.PropertyDescriptors.FilePropertiesDescriptor,
		OSF.DDA.PropertyDescriptors.BindingProperties,
		OSF.DDA.SafeArray.UniqueArguments.BindingSpecificData,
		OSF.DDA.SafeArray.UniqueArguments.Offset,
		OSF.DDA.SafeArray.UniqueArguments.Run,
		OSF.DDA.PropertyDescriptors.Subset,
		OSF.DDA.PropertyDescriptors.DataPartProperties,
		OSF.DDA.PropertyDescriptors.DataNodeProperties,
		OSF.DDA.EventDescriptors.BindingSelectionChangedEvent,
		OSF.DDA.EventDescriptors.DataNodeInsertedEvent,
		OSF.DDA.EventDescriptors.DataNodeReplacedEvent,
		OSF.DDA.EventDescriptors.DataNodeDeletedEvent,
		OSF.DDA.EventDescriptors.DocumentThemeChangedEvent,
		OSF.DDA.EventDescriptors.OfficeThemeChangedEvent,
		OSF.DDA.EventDescriptors.ActiveViewChangedEvent,
		OSF.DDA.DataNodeEventProperties.OldNode,
		OSF.DDA.DataNodeEventProperties.NewNode,
		OSF.DDA.DataNodeEventProperties.NextSiblingNode,
		Microsoft.Office.Internal.Parameters.OfficeTheme,
		Microsoft.Office.Internal.Parameters.DocumentTheme
	];
	var dynamicTypes={};
	dynamicTypes[Microsoft.Office.WebExtension.Parameters.Data]=(function () {
		var tableRows=0;
		var tableHeaders=1;
		return {
			toHost: function OSF_DDA_SafeArray_Delegate_SpecialProcessor_Data$toHost(data) {
				if (typeof data !="string" && data[OSF.DDA.TableDataProperties.TableRows] !==undefined) {
					var tableData=[];
					tableData[tableRows]=data[OSF.DDA.TableDataProperties.TableRows];
					tableData[tableHeaders]=data[OSF.DDA.TableDataProperties.TableHeaders];
					data=tableData;
				}
				return data;
			},
			fromHost: function OSF_DDA_SafeArray_Delegate_SpecialProcessor_Data$fromHost(hostArgs) {
				var ret;
				if (hostArgs.toArray) {
					var dimensions=hostArgs.dimensions();
					if(dimensions===2) {
						ret=_2DVBArrayToJaggedArray(hostArgs);
					} else {
						var array=hostArgs.toArray();
						if(array.length===2 &&  ((array[0] !=null && array[0].toArray) || (array[1] !=null && array[1].toArray))) {
							ret={};
							ret[OSF.DDA.TableDataProperties.TableRows]=_2DVBArrayToJaggedArray(array[tableRows]);
							ret[OSF.DDA.TableDataProperties.TableHeaders]=_2DVBArrayToJaggedArray(array[tableHeaders]);
						} else {
							ret=array;
						}
					}
				} else {
					ret=hostArgs;
				}
				return ret;
			}
		}
	})();
	OSF.DDA.SafeArray.Delegate.SpecialProcessor.uber.constructor.call(this, complexTypes, dynamicTypes);
	this.pack=function OSF_DDA_SafeArray_Delegate_SpecialProcessor$pack(param, arg) {
		var value;
		if (this.isDynamicType(param)) {
			value=dynamicTypes[param].toHost(arg);
		} else {
			value=arg;
		}
		return value;
	};
	this.unpack=function OSF_DDA_SafeArray_Delegate_SpecialProcessor$unpack(param, arg) {
		var value;
		if (this.isComplexType(param) || OSF.DDA.ListType.isListType(param)) {
			try {
				value=arg.toArray();
			} catch (ex) {
				value=arg || {};
			}
		} else if (this.isDynamicType(param)) {
			value=dynamicTypes[param].fromHost(arg);
		} else {
			value=arg;
		}
		return value;
	};
	this.dynamicTypes=dynamicTypes;
}
OSF.OUtil.extend(OSF.DDA.SafeArray.Delegate.SpecialProcessor, OSF.DDA.SpecialProcessor);
OSF.DDA.SafeArray.Delegate.ParameterMap=(function () {
	var parameterMap=new OSF.DDA.HostParameterMap(new OSF.DDA.SafeArray.Delegate.SpecialProcessor());
	var ns;
	var self=parameterMap.self;
	function createObject(properties) {
		var obj=null;
		if (properties) {
			obj={};
			var len=properties.length;
			for (var i=0; i < len; i++) {
				obj[properties[i].name]=properties[i].value;
			}
		}
		return obj;
	}
	function define(definition) {
		var args={};
		var toHost=createObject(definition.toHost);
		if (definition.invertible) {
			args.map=toHost;
		}
		else if (definition.canonical) {
			args.toHost=args.fromHost=toHost;
		}
		else {
			args.toHost=toHost;
			args.fromHost=createObject(definition.fromHost);
		}
		parameterMap.setMapping(definition.type, args);
	}
	ns=OSF.DDA.FileProperties;
	define({
		type: OSF.DDA.PropertyDescriptors.FileProperties,
		fromHost: [
			{ name: ns.Handle, value: 0 },
			{ name: ns.FileSize, value: 1 }
		]
	});
	define({
		type: OSF.DDA.PropertyDescriptors.FileSliceProperties,
		fromHost: [
			{ name: Microsoft.Office.WebExtension.Parameters.Data, value: 0 },
			{ name: ns.SliceSize, value: 1}
		]
	});
	ns=OSF.DDA.FilePropertiesDescriptor;
	define({
		type: OSF.DDA.PropertyDescriptors.FilePropertiesDescriptor,
		fromHost: [
			{ name: ns.Url, value: 0 }
		]
	});
	ns=OSF.DDA.BindingProperties;
	define({
		type: OSF.DDA.PropertyDescriptors.BindingProperties,
		fromHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.Type, value: 1 },
			{ name: OSF.DDA.SafeArray.UniqueArguments.BindingSpecificData, value: 2 }
		]
	});
	define({
		type: OSF.DDA.SafeArray.UniqueArguments.BindingSpecificData,
		fromHost: [
			{ name: ns.RowCount, value: 0 },
			{ name: ns.ColumnCount, value: 1 },
			{ name: ns.HasHeaders, value: 2 }
		]
	});
	ns=OSF.DDA.SafeArray.UniqueArguments;
	define({
		type: OSF.DDA.PropertyDescriptors.Subset,
		toHost: [
			{ name: ns.Offset, value: 0 },
			{ name: ns.Run, value: 1 }
		],
		canonical: true
	});
	ns=Microsoft.Office.WebExtension.Parameters;
	define({
		type: OSF.DDA.SafeArray.UniqueArguments.Offset,
		toHost: [
			{ name: ns.StartRow, value: 0 },
			{ name: ns.StartColumn, value: 1 }
		],
		canonical: true
	});
	define({
		type: OSF.DDA.SafeArray.UniqueArguments.Run,
		toHost: [
			{ name: ns.RowCount, value: 0 },
			{ name: ns.ColumnCount, value: 1 }
		],
		canonical: true
	});
	ns=OSF.DDA.DataPartProperties;
	define({
		type: OSF.DDA.PropertyDescriptors.DataPartProperties,
		fromHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.BuiltIn, value: 1 }
		]
	});
	ns=OSF.DDA.DataNodeProperties;
	define({
		type: OSF.DDA.PropertyDescriptors.DataNodeProperties,
		fromHost: [
			{ name: ns.Handle, value: 0 },
			{ name: ns.BaseName, value: 1 },
			{ name: ns.NamespaceUri, value: 2 },
			{ name: ns.NodeType, value: 3 }
		]
	});
	define({
		type: OSF.DDA.EventDescriptors.BindingSelectionChangedEvent,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: 0 },
			{ name: OSF.DDA.PropertyDescriptors.Subset, value: 1 }
		]
	});
	define({
		type: OSF.DDA.EventDescriptors.DocumentThemeChangedEvent,
		fromHost: [
			{ name: Microsoft.Office.Internal.Parameters.DocumentTheme, value: self}
		]
	})
	define({
		type: OSF.DDA.EventDescriptors.OfficeThemeChangedEvent,
		fromHost: [
			{ name: Microsoft.Office.Internal.Parameters.OfficeTheme, value: self}
		]
	})
	define({
		type: OSF.DDA.EventDescriptors.ActiveViewChangedEvent,
		fromHost: [
			{ name: Microsoft.Office.WebExtension.Parameters.ActiveView, value: 0}
		]
	})
	ns=OSF.DDA.DataNodeEventProperties;
	define({
		type: OSF.DDA.EventDescriptors.DataNodeInsertedEvent,
		fromHost: [
			{ name: ns.InUndoRedo, value: 0 },
			{ name: ns.NewNode, value: 1 }
		]
	});
	define({
		type: OSF.DDA.EventDescriptors.DataNodeReplacedEvent,
		fromHost: [
			{ name: ns.InUndoRedo, value: 0 },
			{ name: ns.OldNode, value: 1 },
			{ name: ns.NewNode, value: 2 }
		]
	});
	define({
		type: OSF.DDA.EventDescriptors.DataNodeDeletedEvent,
		fromHost: [
			{ name: ns.InUndoRedo, value: 0 },
			{ name: ns.OldNode, value: 1 },
			{ name: ns.NextSiblingNode, value: 2 }
		]
	});
	define({
		type: ns.OldNode,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.DataNodeProperties, value: self }
		]
	});
	define({
		type: ns.NewNode,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.DataNodeProperties, value: self }
		]
	});
	define({
		type: ns.NextSiblingNode,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.DataNodeProperties, value: self }
		]
	});
	ns=Microsoft.Office.WebExtension.AsyncResultStatus;
	define({
		type: OSF.DDA.PropertyDescriptors.AsyncResultStatus,
		fromHost: [
			{ name: ns.Succeeded, value: 0 },
			{ name: ns.Failed, value: 1 }
		]
	});
	ns=Microsoft.Office.WebExtension.CoercionType;
	define({
		type: Microsoft.Office.WebExtension.Parameters.CoercionType,
		toHost: [
			{ name: ns.Text, value: 0 },
			{ name: ns.Matrix, value: 1 },
			{ name: ns.Table, value: 2 },
			{ name: ns.Html, value: 3 },
			{ name: ns.Ooxml, value: 4 },
			{ name: ns.SlideRange, value:7 }
		]
	});
	ns=Microsoft.Office.WebExtension.GoToType;
	define({
		type: Microsoft.Office.WebExtension.Parameters.GoToType,
		toHost: [
			{ name: ns.Binding, value: 0 },
			{ name: ns.NamedItem, value: 1 },
			{ name: ns.Slide, value: 2 },
			{ name: ns.Index, value: 3 }
		]
	});
	ns=Microsoft.Office.WebExtension.FileType;
	if (ns) {
		define({
			type: Microsoft.Office.WebExtension.Parameters.FileType,
			toHost: [
			{ name: ns.Text, value: 0 },
			{ name: ns.Compressed, value: 5 },
			{ name: ns.Pdf, value: 6 }
		]
		});
	}
	ns=Microsoft.Office.WebExtension.BindingType;
	if (ns) {
		define({
			type: Microsoft.Office.WebExtension.Parameters.BindingType,
			toHost: [
				{ name: ns.Text, value: 0 },
				{ name: ns.Matrix, value: 1 },
				{ name: ns.Table, value: 2 }
			],
			invertible: true
		});
	}
	ns=Microsoft.Office.WebExtension.ValueFormat;
	define({
		type: Microsoft.Office.WebExtension.Parameters.ValueFormat,
		toHost: [
			{ name: ns.Unformatted, value: 0 },
			{ name: ns.Formatted, value: 1 }
		]
	});
	ns=Microsoft.Office.WebExtension.FilterType;
	define({
		type: Microsoft.Office.WebExtension.Parameters.FilterType,
		toHost: [
			{ name: ns.All, value: 0 },
			{ name: ns.OnlyVisible, value: 1 }
		]
	});
	ns=Microsoft.Office.Internal.OfficeTheme;
	if (ns) {
		define({
			type:Microsoft.Office.Internal.Parameters.OfficeTheme,
			fromHost: [
						{name: ns.PrimaryFontColor, value: 0},
						{name: ns.PrimaryBackgroundColor, value: 1},
						{name: ns.SecondaryFontColor, value:2},
						{name: ns.SecondaryBackgroundColor, value:3}
			]
		})
	}
	ns=Microsoft.Office.WebExtension.ActiveView;
	if (ns) {
		define({
			type:Microsoft.Office.WebExtension.Parameters.ActiveView,
			fromHost: [
				{name: 0, value: ns.Read},
				{name: 1, value: ns.Edit}
			]
		})
	}
	ns=Microsoft.Office.Internal.DocumentTheme;
	if (ns) {
		define({
			type:Microsoft.Office.Internal.Parameters.DocumentTheme,
			fromHost: [
				{name: ns.PrimaryBackgroundColor, value: 0},
				{name: ns.PrimaryFontColor, value: 1},
				{name: ns.SecondaryBackgroundColor, value: 2},
				{name: ns.SecondaryFontColor, value: 3},
				{name: ns.Accent1, value: 4},
				{name: ns.Accent2, value: 5},
				{name: ns.Accent3, value: 6},
				{name: ns.Accent4, value: 7},
				{name: ns.Accent5, value: 8},
				{name: ns.Accent6, value: 9},
				{name: ns.Hyperlink, value: 10},
				{name: ns.FollowedHyperlink, value: 11},
				{name: ns.HeaderLatinFont, value: 12},
				{name: ns.HeaderEastAsianFont, value: 13},
				{name: ns.HeaderScriptFont, value: 14},
				{name: ns.HeaderLocalizedFont, value: 15},
				{name: ns.BodyLatinFont, value: 16},
				{name: ns.BodyEastAsianFont, value: 17},
				{name: ns.BodyScriptFont, value: 18},
				{name: ns.BodyLocalizedFont, value: 19}
			]
		})
	}
	ns=Microsoft.Office.WebExtension.SelectionMode;
	define({
		type: Microsoft.Office.WebExtension.Parameters.SelectionMode,
		toHost: [
			{ name: ns.Default, value: 0 },
			{ name: ns.Selected, value: 1 },
			{ name: ns.None, value: 2 },
		]
	});
	ns=Microsoft.Office.WebExtension.Parameters;
	var cns=OSF.DDA.MethodDispId;
	define({
		type: cns.dispidNavigateToMethod,
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.GoToType, value: 1 },
			{ name: ns.SelectionMode, value: 2 }
		]
	});
	define({
		type: cns.dispidGetSelectedDataMethod,
		fromHost: [
			{ name: ns.Data, value: self }
		],
		toHost: [
			{ name: ns.CoercionType, value: 0 },
			{ name: ns.ValueFormat, value: 1 },
			{ name: ns.FilterType, value: 2 }
		]
	});
	define({
		type: cns.dispidSetSelectedDataMethod,
		toHost: [
			{ name: ns.CoercionType, value: 0 },
			{ name: ns.Data, value: 1 }
		]
	});
	define({
		type: cns.dispidGetFilePropertiesMethod,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.FilePropertiesDescriptor, value: self }
		]
	});
	define({
		type: cns.dispidGetDocumentCopyMethod,
		toHost: [{ name: ns.FileType, value: 0}],
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.FileProperties, value: self }
		]
	});
	define({
		type: cns.dispidGetDocumentCopyChunkMethod,
		toHost: [
			{ name: OSF.DDA.FileProperties.Handle, value: 0 },
			{ name: OSF.DDA.FileSliceOffset, value: 1 },
			{ name: OSF.DDA.FileProperties.SliceSize, value: 2 }
		],
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.FileSliceProperties, value: self }
		]
	});
	define({
		type: cns.dispidReleaseDocumentCopyMethod,
		toHost: [{ name: OSF.DDA.FileProperties.Handle, value: 0}]
	});
	define({
		type: cns.dispidAddBindingFromSelectionMethod,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: self }
		],
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.BindingType, value: 1 }
		]
	});
	define({
		type: cns.dispidAddBindingFromPromptMethod,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: self }
		],
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.BindingType, value: 1 },
			{ name: ns.PromptText, value: 2 }
		]
	});
	define({
		type: cns.dispidAddBindingFromNamedItemMethod,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: self }
		],
		toHost: [
			{ name: ns.ItemName, value: 0 },
			{ name: ns.Id, value: 1 },
			{ name: ns.BindingType, value: 2 },
			{ name: ns.FailOnCollision, value: 3 }
		]
	});
	define({
		type: cns.dispidReleaseBindingMethod,
		toHost: [
			{ name: ns.Id, value: 0 }
		]
	});
	define({
		type: cns.dispidGetBindingMethod,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: self }
		],
		toHost: [
			{ name: ns.Id, value: 0 }
		]
	});
	define({
		type: cns.dispidGetAllBindingsMethod,
		fromHost: [
			{ name: OSF.DDA.ListDescriptors.BindingList, value: self }
		]
	});
	define({
		type: cns.dispidGetBindingDataMethod,
		fromHost: [
			{ name: ns.Data, value: self }
		],
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.CoercionType, value: 1 },
			{ name: ns.ValueFormat, value: 2 },
			{ name: ns.FilterType, value: 3 },
			{ name: OSF.DDA.PropertyDescriptors.Subset, value: 4 }
		]
	});
	define({
		type: cns.dispidSetBindingDataMethod,
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.CoercionType, value: 1 },
			{ name: ns.Data, value: 2 },
			{ name: OSF.DDA.SafeArray.UniqueArguments.Offset, value: 3 }
		]
	});
	define({
		type: cns.dispidAddRowsMethod,
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.Data, value: 1 }
		]
	});
	define({
		type: cns.dispidAddColumnsMethod,
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.Data, value: 1 }
		]
	});
	define({
		type: cns.dispidClearAllRowsMethod,
		toHost: [
			{ name: ns.Id, value: 0 }
		]
	});
	define({
		type: cns.dispidClearFormatsMethod,
		toHost: [
			{ name: ns.Id, value: 0 }
		]
	});
		define({
		type: cns.dispidSetTableOptionsMethod,
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.TableOptions, value: 1 },
		]
	});
	define({
		type: cns.dispidSetFormatsMethod,
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.CellFormat, value: 1 },
		]
	});
	define({
		type: cns.dispidLoadSettingsMethod,
		fromHost: [
			{ name: OSF.DDA.SettingsManager.SerializedSettings, value: self }
		]
	});
	define({
		type: cns.dispidSaveSettingsMethod,
		toHost: [
			{ name: OSF.DDA.SettingsManager.SerializedSettings, value: OSF.DDA.SettingsManager.SerializedSettings },
			{ name: Microsoft.Office.WebExtension.Parameters.OverwriteIfStale, value: Microsoft.Office.WebExtension.Parameters.OverwriteIfStale }
		]
	});
	define({
		type: OSF.DDA.MethodDispId.dispidGetOfficeThemeMethod,
		fromHost: [
			{ name: Microsoft.Office.Internal.Parameters.OfficeTheme, value: self }
		]
	});
	define({
		type: OSF.DDA.MethodDispId.dispidGetDocumentThemeMethod,
		fromHost: [
			{ name: Microsoft.Office.Internal.Parameters.DocumentTheme, value: self }
		]
	});
	define({
		type: OSF.DDA.MethodDispId.dispidGetActiveViewMethod,
		fromHost: [
			{ name: Microsoft.Office.WebExtension.Parameters.ActiveView, value: self }
		]
	});
	define({
		type: cns.dispidAddDataPartMethod,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.DataPartProperties, value: self }
		],
		toHost: [
			{ name: ns.Xml, value: 0 }
		]
	});
	define({
		type: cns.dispidGetDataPartByIdMethod,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.DataPartProperties, value: self }
		],
		toHost: [
			{ name: ns.Id, value: 0 }
		]
	});
	define({
		type: cns.dispidGetDataPartsByNamespaceMethod,
		fromHost: [
			{ name: OSF.DDA.ListDescriptors.DataPartList, value: self }
		],
		toHost: [
			{ name: ns.Namespace, value: 0 }
		]
	});
	define({
		type: cns.dispidGetDataPartXmlMethod,
		fromHost: [
			{ name: ns.Data, value: self}
		],
		toHost: [
			{ name: ns.Id, value: 0 }
		]
	});
	define({
		type: cns.dispidGetDataPartNodesMethod,
		fromHost: [
			{ name: OSF.DDA.ListDescriptors.DataNodeList, value: self }
		],
		toHost: [
			{ name: ns.Id, value: 0 },
			{ name: ns.XPath, value: 1 }
		]
	});
	define({
		type: cns.dispidDeleteDataPartMethod,
		toHost: [
			{ name: ns.Id, value: 0 }
		]
	});
	define({
		type: cns.dispidGetDataNodeValueMethod,
		fromHost: [
			{ name: ns.Data, value: self}
		],
		toHost: [
			{ name: OSF.DDA.DataNodeProperties.Handle, value: 0 }
		]
	});
	define({
		type: cns.dispidGetDataNodeXmlMethod,
		fromHost: [
			{ name: ns.Data, value: self}
		],
		toHost: [
			{ name: OSF.DDA.DataNodeProperties.Handle, value: 0 }
		]
	});
	define({
		type: cns.dispidGetDataNodesMethod,
		fromHost: [
			{ name: OSF.DDA.ListDescriptors.DataNodeList, value: self }
		],
		toHost: [
			{ name: OSF.DDA.DataNodeProperties.Handle, value: 0 },
			{ name: ns.XPath, value: 1 }
		]
	});
	define({
		type: cns.dispidSetDataNodeValueMethod,
		toHost: [
			{ name: OSF.DDA.DataNodeProperties.Handle, value: 0 },
			{ name: ns.Data, value: 1 }
		]
	});
	define({
		type: cns.dispidSetDataNodeXmlMethod,
		toHost: [
			{ name: OSF.DDA.DataNodeProperties.Handle, value: 0 },
			{ name: ns.Xml, value: 1 }
		]
	});
	define({
		type: cns.dispidAddDataNamespaceMethod,
		toHost: [
			{ name: OSF.DDA.DataPartProperties.Id, value: 0 },
			{ name: ns.Prefix, value: 1 },
			{ name: ns.Namespace, value: 2 }
		]
	});
	define({
		type: cns.dispidGetDataUriByPrefixMethod,
		fromHost: [
			{ name: ns.Data, value: self}
		],
		toHost: [
			{ name: OSF.DDA.DataPartProperties.Id, value: 0 },
			{ name: ns.Prefix, value: 1 }
		]
	});
	define({
		type: cns.dispidGetDataPrefixByUriMethod,
		fromHost: [
			{ name: ns.Data, value: self}
		],
		toHost: [
			{ name: OSF.DDA.DataPartProperties.Id, value: 0 },
			{ name: ns.Namespace, value: 1 }
		]
	});
	define({
		type: cns.dispidGetSelectedTaskMethod,
		fromHost: [
			{ name: ns.TaskId, value: self }
		]
	});
	define({
		type: cns.dispidGetTaskMethod,
		fromHost: [
			{ name: "taskName", value: 0 },
			{ name: "wssTaskId", value: 1 },
			{ name: "resourceNames", value: 2 }
		],
		toHost: [
			{ name: ns.TaskId, value: 0 }
		]
	});
	define({
		type: cns.dispidGetTaskFieldMethod,
		fromHost: [
			{ name: ns.FieldValue, value: self }
		],
		toHost: [
			{ name: ns.TaskId, value: 0 },
			{ name: ns.FieldId, value: 1 },
			{ name: ns.GetRawValue, value: 2 }
		]
	});
	define({
		type: cns.dispidGetWSSUrlMethod,
		fromHost: [
			{ name: ns.ServerUrl, value: 0 },
			{ name: ns.ListName, value: 1 }
		]
	});
	define({
		type: cns.dispidGetSelectedResourceMethod,
		fromHost: [
			{ name: ns.ResourceId, value: self }
		]
	});
	define({
		type: cns.dispidGetResourceFieldMethod,
		fromHost: [
			{ name: ns.FieldValue, value: self }
		],
		toHost: [
			{ name: ns.ResourceId, value: 0 },
			{ name: ns.FieldId, value: 1 },
			{ name: ns.GetRawValue, value: 2 }
		]
	});
	define({
		type: cns.dispidGetProjectFieldMethod,
		fromHost: [
			{ name: ns.FieldValue, value: self }
		],
		toHost: [
			{ name: ns.FieldId, value: 0 },
			{ name: ns.GetRawValue, value: 1 }
		]
	});
	define({
		type: cns.dispidGetSelectedViewMethod,
		fromHost: [
			{ name: ns.ViewType, value: 0 },
			{ name: ns.ViewName, value: 1 }
		]
	});
	cns=OSF.DDA.EventDispId
	define({ type: cns.dispidDocumentSelectionChangedEvent });
	define({
		type: cns.dispidBindingSelectionChangedEvent,
		fromHost: [
			{name: OSF.DDA.EventDescriptors.BindingSelectionChangedEvent, value: self}
		]
	});
	define({
		type: cns.dispidBindingDataChangedEvent,
		fromHost: [{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: self}]
	});
	define({ type: cns.dispidSettingsChangedEvent });
	define({
		type: cns.dispidDocumentThemeChangedEvent,
		fromHost: [
			{name: OSF.DDA.EventDescriptors.DocumentThemeChangedEvent, value: self}
		]
	});
	define({
		type: cns.dispidOfficeThemeChangedEvent,
		fromHost: [
			{name: OSF.DDA.EventDescriptors.OfficeThemeChangedEvent, value: self}
		]
	});
	define({
		type: cns.dispidActiveViewChangedEvent,
		fromHost: [{ name: OSF.DDA.EventDescriptors.ActiveViewChangedEvent, value: self}]
	});
	define({
		type: cns.dispidDataNodeAddedEvent,
		fromHost: [{ name: OSF.DDA.EventDescriptors.DataNodeInsertedEvent, value: self}]
	});
	define({
		type: cns.dispidDataNodeReplacedEvent,
		fromHost: [{ name: OSF.DDA.EventDescriptors.DataNodeReplacedEvent, value: self}]
	});
	define({
		type: cns.dispidDataNodeDeletedEvent,
		fromHost: [{ name: OSF.DDA.EventDescriptors.DataNodeDeletedEvent, value: self}]
	});
	define({ type: cns.dispidTaskSelectionChangedEvent });
	define({ type: cns.dispidResourceSelectionChangedEvent });
	define({ type: cns.dispidViewSelectionChangedEvent });
	parameterMap.define=define;
	return parameterMap;
})();
OSF.DDA.SafeArray.Delegate._onException=function OSF_DDA_SafeArray_Delegate$OnException(ex, args) {
	var status;
	var number=ex.number;
	if (number) {
		switch (number) {
			case -2146828218:
				status=OSF.DDA.ErrorCodeManager.errorCodes.ooeNoCapability;
				break;
			case -2146827850:
			default:
				status=OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
				break;
		}
	}
	if (args.onComplete) {
		args.onComplete(status || OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError);
	}
}
OSF.DDA.SafeArray.Delegate.executeAsync=function OSF_DDA_SafeArray_Delegate$ExecuteAsync(args) {
	try {
		if (args.onCalling) {
			args.onCalling();
		}
		function toArray(args) {
			var arrArgs=args;
			if (OSF.OUtil.isArray(args)) {
				var len=arrArgs.length;
				for (var i=0; i < len; i++) {
					arrArgs[i]=toArray(arrArgs[i]);
				}
			} else if (OSF.OUtil.isDate(args)) {
				arrArgs=args.getVarDate();
			} else if (typeof args==="object" && !OSF.OUtil.isArray(args)) {
				arrArgs=[];
				for (var index in args) {
					if (!OSF.OUtil.isFunction(args[index])) {
						arrArgs[index]=toArray(args[index]);
					}
				}
			}
			return arrArgs;
		}
		var startTime=(new Date()).getTime();
		window.external.Execute(
			args.dispId,
			toArray(args.hostCallArgs),
			function OSF_DDA_SafeArrayFacade$Execute_OnResponse(hostResponseArgs) {
				if (args.onReceiving) {
					args.onReceiving();
				}
				var result=hostResponseArgs.toArray();
				var status=result[OSF.DDA.SafeArray.Response.Status];
				if (args.onComplete) {
					var payload;
					if (status==OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess) {
						if (result.length > 2) {
							payload=[];
							for (var i=1; i < result.length; i++)
								payload[i - 1]=result[i];
						}
						else {
							payload=result[OSF.DDA.SafeArray.Response.Payload];
						}
					}
					else {
						payload=result[OSF.DDA.SafeArray.Response.Payload];
					}
					args.onComplete(status, payload);
				}
				if (OSF.AppTelemetry) {
					OSF.AppTelemetry.onMethodDone(args.dispId, args.hostCallArgs, Math.abs((new Date()).getTime() -  startTime), status);
				}
			}
		);
	}
	catch (ex) {
		OSF.DDA.SafeArray.Delegate._onException(ex, args);
	}
};
OSF.DDA.SafeArray.Delegate._getOnAfterRegisterEvent=function OSF_DDA_SafeArrayDelegate$GetOnAfterRegisterEvent(register, args) {
	var startTime=(new Date()).getTime();
	return function OSF_DDA_SafeArrayDelegate$OnAfterRegisterEvent(hostResponseArgs) {
		if (args.onReceiving) {
			args.onReceiving();
		}
		var status=hostResponseArgs.toArray ? hostResponseArgs.toArray()[OSF.DDA.SafeArray.Response.Status] : hostResponseArgs;
		if (args.onComplete) {
			args.onComplete(status)
		}
		if (OSF.AppTelemetry) {
			OSF.AppTelemetry.onRegisterDone(register, args.dispId, Math.abs((new Date()).getTime() - startTime), status);
		}
	}
}
OSF.DDA.SafeArray.Delegate.registerEventAsync=function OSF_DDA_SafeArray_Delegate$RegisterEventAsync(args) {
	if (args.onCalling) {
		args.onCalling();
	}
	var callback=OSF.DDA.SafeArray.Delegate._getOnAfterRegisterEvent(true, args);
	try {
		window.external.RegisterEvent(
			args.dispId,
			args.targetId,
			function OSF_DDA_SafeArrayDelegate$RegisterEventAsync_OnEvent(eventDispId, payload) {
				if (args.onEvent) {
					args.onEvent(payload);
				}
				if (OSF.AppTelemetry) {
					OSF.AppTelemetry.onEventDone(args.dispId);
				}
			},
			callback
		);
	}
	catch (ex) {
		OSF.DDA.SafeArray.Delegate._onException(ex, args);
	}
};
OSF.DDA.SafeArray.Delegate.unregisterEventAsync=function OSF_DDA_SafeArray_Delegate$UnregisterEventAsync(args) {
	if (args.onCalling) {
		args.onCalling();
	}
	var callback=OSF.DDA.SafeArray.Delegate._getOnAfterRegisterEvent(false, args);
	try {
		window.external.UnregisterEvent(
			args.dispId,
			args.targetId,
			callback
		);
	}
	catch (ex) {
		OSF.DDA.SafeArray.Delegate._onException(ex, args);
	}
};
delete Microsoft.Office.WebExtension.FileType;
OSF.DDA.ExcelDocument=function OSF_DDA_ExcelDocument(officeAppContext, settings) {
	var bf=new OSF.DDA.BindingFacade(this);
	OSF.DDA.DispIdHost.addAsyncMethods(bf, [OSF.DDA.AsyncMethodNames.AddFromPromptAsync]);
	OSF.DDA.DispIdHost.addAsyncMethods(this, [OSF.DDA.AsyncMethodNames.GetFilePropertiesAsync]);
	OSF.DDA.ExcelDocument.uber.constructor.call(this,
		officeAppContext,
		bf,
		settings
	);
	OSF.DDA.DispIdHost.addAsyncMethods(this, [
		OSF.DDA.AsyncMethodNames.GoToByIdAsync
	]);
	OSF.OUtil.finalizeProperties(this);
};
OSF.OUtil.extend(OSF.DDA.ExcelDocument, OSF.DDA.JsomDocument);
OSF.DDA.ExcelTableBinding=function OSF_DDA_ExcelTableBinding(id, docInstance, rows, cols, hasHeaders) {
	var am=OSF.DDA.AsyncMethodNames;
	OSF.DDA.DispIdHost.addAsyncMethods(this, [
		am.ClearFormatsAsync,
		am.SetTableOptionsAsync,
		am.SetFormatsAsync
	]);
	OSF.DDA.ExcelTableBinding.uber.constructor.call(this, id, docInstance, rows, cols, hasHeaders);
	OSF.OUtil.finalizeProperties(this);
};
OSF.OUtil.extend(OSF.DDA.ExcelTableBinding, OSF.DDA.TableBinding);
OSF.DDA.SafeArray.Delegate.ParameterMap.define({
	type: OSF.DDA.MethodDispId.dispidSetSelectedDataMethod,
	toHost: [
		{ name: Microsoft.Office.WebExtension.Parameters.CoercionType, value: 0 },
		{ name: Microsoft.Office.WebExtension.Parameters.Data, value: 1 },
		{ name: Microsoft.Office.WebExtension.Parameters.CellFormat, value: 2 },
		{ name: Microsoft.Office.WebExtension.Parameters.TableOptions, value: 3 }
	]
});
OSF.DDA.SafeArray.Delegate.ParameterMap.define({
	type: OSF.DDA.MethodDispId.dispidSetBindingDataMethod,
	toHost: [
		{ name: Microsoft.Office.WebExtension.Parameters.Id, value: 0 },
		{ name: Microsoft.Office.WebExtension.Parameters.CoercionType, value: 1 },
		{ name: Microsoft.Office.WebExtension.Parameters.Data, value: 2 },
		{ name: OSF.DDA.SafeArray.UniqueArguments.Offset, value: 3 },
		{ name: Microsoft.Office.WebExtension.Parameters.CellFormat, value: 4 },
		{ name: Microsoft.Office.WebExtension.Parameters.TableOptions, value: 5 }
	]
});
Microsoft.Office.WebExtension.Table={
	All : 0,
	Data : 1,
	Headers : 2
};
(function (){
	var tableOptionProperties={
		headerRow : 0,
		bandedRows : 1,
		firstColumn : 2,
		lastColumn : 3,
		bandedColumns : 4,
		filterButton : 5,
		style : 6,
		totalRow : 7
	};
	var cellProperties={
		row : 0,
		column : 1
	};
	var formatProperties={
		alignHorizontal : {text:"alignHorizontal", type:1},
		alignVertical : {text:"alignVertical", type:2},
		backgroundColor : {text:"backgroundColor", type:101},
		borderStyle : {text:"borderStyle", type:201},
		borderColor : {text:"borderColor", type:202},
		borderTopStyle : {text:"borderTopStyle", type:203},
		borderTopColor : {text:"borderTopColor", type:204},
		borderBottomStyle : {text:"borderBottomStyle", type:205},
		borderBottomColor : {text:"borderBottomColor", type:206},
		borderLeftStyle : {text:"borderLeftStyle", type:207},
		borderLeftColor : {text:"borderLeftColor", type:208},
		borderRightStyle : {text:"borderRightStyle", type:209},
		borderRightColor : {text:"borderRightColor", type:210},
		borderOutlineStyle : {text:"borderOutlineStyle", type:211},
		borderOutlineColor : {text:"borderOutlineColor", type:212},
		borderInlineStyle : {text:"borderInlineStyle", type:213},
		borderInlineColor : {text:"borderInlineColor", type:214},
		fontFamily : {text:"fontFamily", type:301},
		fontStyle : {text:"fontStyle", type:302},
		fontSize : {text:"fontSize", type:303},
		fontUnderlineStyle : {text:"fontUnderlineStyle", type:304},
		fontColor : {text:"fontColor", type:305},
		fontDirection : {text:"fontDirection", type:306},
		fontStrikethrough : {text:"fontStrikethrough", type:307},
		fontSuperscript : {text:"fontSuperscript", type:308},
		fontSubscript : {text:"fontSubscript", type:309},
		fontNormal : {text:"fontNormal", type:310},
		indentLeft : {text:"indentLeft", type:401},
		indentRight : {text:"indentRight", type:402},
		numberFormat: {text:"numberFormat", type:501},
		width : {text:"width", type:701},
		height : {text:"height", type:702},
		wrapping : {text:"wrapping", type:703}
	};
	var borderStyleSet=[
		{name:"none", value: 0},
		{name:"thin", value: 1},
		{name:"medium", value: 2},
		{name:"dashed", value: 3},
		{name:"dotted", value: 4},
		{name:"thick", value: 5},
		{name:"double", value: 6},
		{name:"hair", value: 7},
		{name:"medium dashed", value: 8},
		{name:"dash dot", value: 9},
		{name:"medium dash dot", value: 10},
		{name:"dash dot dot", value: 11},
		{name:"medium dash dot dot", value: 12},
		{name:"slant dash dot", value: 13},
	];
	var colorSet=[
		{name: "none", value: 0},
		{name: "black", value: 1},
		{name: "blue", value: 2},
		{name: "gray", value: 3},
		{name: "green", value: 4},
		{name: "orange", value: 5},
		{name: "pink", value: 6},
		{name: "purple", value: 7},
		{name: "red", value: 8},
		{name: "teal", value: 9},
		{name: "turquoise", value: 10},
		{name: "violet", value: 11},
		{name: "white", value: 12},
		{name: "yellow", value: 13},
		{name: "automatic", value: 14},
	];
	var ns=OSF.DDA.SafeArray.Delegate.ParameterMap;
	ns.define({
		type: formatProperties.alignHorizontal.text,
		toHost: [
			{name: "general", value: 0},
			{name: "left", value: 1},
			{name: "center", value: 2},
			{name: "right", value: 3},
			{name: "fill", value: 4},
			{name: "justify", value: 5},
			{name: "center across selection", value: 6},
			{name: "distributed", value: 7},
	]});
	ns.define({
		type: formatProperties.alignVertical.text,
		toHost: [
			{name: "top", value: 0},
			{name: "center", value: 1},
			{name: "bottom", value: 2},
			{name: "justify", value: 3},
			{name: "distributed", value: 4},
	]});
	ns.define({
		type: formatProperties.backgroundColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.borderStyle.text,
		toHost: borderStyleSet
	});
	ns.define({
		type: formatProperties.borderColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.borderTopStyle.text,
		toHost: borderStyleSet
	});
	ns.define({
		type: formatProperties.borderTopColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.borderBottomStyle.text,
		toHost: borderStyleSet
	});
	ns.define({
		type: formatProperties.borderBottomColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.borderLeftStyle.text,
		toHost: borderStyleSet
	});
	ns.define({
		type: formatProperties.borderLeftColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.borderRightStyle.text,
		toHost: borderStyleSet
	});
	ns.define({
		type: formatProperties.borderRightColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.borderOutlineStyle.text,
		toHost: borderStyleSet
	});
	ns.define({
		type: formatProperties.borderOutlineColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.borderInlineStyle.text,
		toHost: borderStyleSet
	});
	ns.define({
		type: formatProperties.borderInlineColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.fontStyle.text,
		toHost: [
			{name: "regular", value: 0},
			{name: "italic", value: 1},
			{name: "bold", value: 2},
			{name: "bold italic", value: 3},
	]});
	ns.define({
		type: formatProperties.fontUnderlineStyle.text,
		toHost: [
			{name: "none", value: 0},
			{name: "single", value: 1},
			{name: "double", value: 2},
			{name: "single accounting", value: 3},
			{name: "double accounting", value: 4},
	]});
	ns.define({
		type: formatProperties.fontColor.text,
		toHost: colorSet
	});
	ns.define({
		type: formatProperties.fontDirection.text,
		toHost: [
			{name: "context", value: 0},
			{name: "left-to-right", value: 1},
			{name: "right-to-left", value: 2},
	]});
	ns.define({
		type: formatProperties.width.text,
		toHost: [
			{name: "auto fit", value: -1},
	]});
	ns.define({
		type: formatProperties.height.text,
		toHost: [
			{name: "auto fit", value: -1},
	]});
	ns.define({
		type: Microsoft.Office.WebExtension.Parameters.TableOptions,
		toHost: [
			{name: "headerRow", value: 0 },
			{name: "bandedRows", value: 1 },
			{name: "firstColumn", value: 2 },
			{name: "lastColumn", value: 3 },
			{name: "bandedColumns", value: 4 },
			{name: "filterButton", value: 5 },
			{name: "style", value: 6 },
			{name: "totalRow", value: 7 }
	 ]});
	ns.dynamicTypes[Microsoft.Office.WebExtension.Parameters.CellFormat]={
		toHost: function (data) {
			for (entry in data) {
				if (data[entry].format) {
					data[entry].format=ns.mapValues(data[entry].format, "toHost");
				}
			}
			return data;
		},
		fromHost: function (args) {
			return args;
		}
	}
	ns.specialProcessorDynamicTypes[Microsoft.Office.WebExtension.Parameters.CellFormat]=(function () {
		return {
			toHost: function OSF_DDA_SafeArray_Delegate_SpecialProcessor_CellFormat$toHost(cellFormats) {
				var textCells="cells";
				var textFormat="format";
				var posCells=0;
				var posFormat=1;
				var ret=[];
				for (index in cellFormats) {
					var cfOld=cellFormats[index];
					var cfNew=[];
					if (cfOld[textCells] !=='undefined') {
						var cellsOld=cfOld[textCells];
						var cellsNew;
						if (typeof cfOld[textCells]==="object") {
							cellsNew=[];
							for (entry in cellsOld) {
								if (typeof(cellProperties[entry])!=='undefined') {
									cellsNew[cellProperties[entry]]=cellsOld[entry];
								}
							}
						} else {
							cellsNew=cellsOld;
						}
						cfNew[posCells]=cellsNew;
					}
					if (cfOld[textFormat]) {
						var formatOld=cfOld[textFormat];
						var formatNew=[];
						for (entry in formatOld) {
							if (typeof(formatProperties[entry])!=='undefined') {
								formatNew.push([formatProperties[entry].type, formatOld[entry]]);
							}
						}
						cfNew[posFormat]=formatNew;
					}
					ret[index]=cfNew;
				}
				return ret;
			},
			fromHost: function OSF_DDA_SafeArray_Delegate_SpecialProcessor_CellFormat$fromHost(hostArgs) {
				return hostArgs;
			}
		}
	})();
	ns.specialProcessorDynamicTypes[Microsoft.Office.WebExtension.Parameters.TableOptions]=(function () {
			return {
				toHost: function OSF_DDA_SafeArray_Delegate_SpecialProcessor_TableOptions$toHost(tableOptions) {
					var ret=[];
					for (entry in tableOptions) {
						if (tableOptionProperties[entry]!=undefined){
							ret[tableOptionProperties[entry]]=tableOptions[entry];
						}
					}
					return ret;
				},
				fromHost: function OSF_DDA_SafeArray_Delegate_SpecialProcessor_TableOptions$fromHost(hostArgs) {
					return hostArgs;
				}
			}
	})();
})();

