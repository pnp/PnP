/* Word specific JavaScript API library */
/* Version: 15.0.4448.1000 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/

OSF.OUtil.augmentList(Microsoft.Office.WebExtension.CoercionType, {
	Html: "html",
	Ooxml: "ooxml"
});
OSF.OUtil.augmentList(Microsoft.Office.WebExtension.EventType, {
	DataNodeDeleted: "nodeDeleted",
	DataNodeInserted: "nodeInserted",
	DataNodeReplaced: "nodeReplaced"
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
		OSF.DDA.DataNodeEventProperties.OldNode,
		OSF.DDA.DataNodeEventProperties.NewNode,
		OSF.DDA.DataNodeEventProperties.NextSiblingNode
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
			{ name: ns.Ooxml, value: 4 }
		]
	});
	ns=Microsoft.Office.WebExtension.FileType;
	if (ns) {
		define({
			type: Microsoft.Office.WebExtension.Parameters.FileType,
			toHost: [
			{ name: ns.Text, value: 0 },
			{ name: ns.Compressed, value: 5 }
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
	ns=Microsoft.Office.WebExtension.Parameters;
	var cns=OSF.DDA.MethodDispId;
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
		window.external.Execute(
			args.dispId,
			toArray(args.hostCallArgs),
			function OSF_DDA_SafeArrayFacade$Execute_OnResponse(hostResponseArgs) {
				if (args.onReceiving) {
					args.onReceiving();
				}
				if (args.onComplete) {
					var result=hostResponseArgs.toArray();
					var status=result[OSF.DDA.SafeArray.Response.Status];
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
			}
		);
	}
	catch (ex) {
		OSF.DDA.SafeArray.Delegate._onException(ex, args);
	}
};
OSF.DDA.SafeArray.Delegate._getOnAfterRegisterEvent=function OSF_DDA_SafeArrayDelegate$GetOnAfterRegisterEvent(args) {
	return function OSF_DDA_SafeArrayDelegate$OnAfterRegisterEvent(hostResponseArgs) {
		if (args.onReceiving) {
			args.onReceiving();
		}
		if (args.onComplete) {
			var status=hostResponseArgs.toArray ? hostResponseArgs.toArray()[OSF.DDA.SafeArray.Response.Status] : hostResponseArgs;
			args.onComplete(status)
		}
	}
}
OSF.DDA.SafeArray.Delegate.registerEventAsync=function OSF_DDA_SafeArray_Delegate$RegisterEventAsync(args) {
	if (args.onCalling) {
		args.onCalling();
	}
	var callback=OSF.DDA.SafeArray.Delegate._getOnAfterRegisterEvent(args);
	try {
		window.external.RegisterEvent(
			args.dispId,
			args.targetId,
			function OSF_DDA_SafeArrayDelegate$RegisterEventAsync_OnEvent(eventDispId, payload) {
				if (args.onEvent) {
					args.onEvent(payload);
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
	var callback=OSF.DDA.SafeArray.Delegate._getOnAfterRegisterEvent(args);
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
OSF.DDA.WordDocument=function OSF_DDA_WordDocument(officeAppContext, settings) {
	OSF.DDA.WordDocument.uber.constructor.call(this,
		officeAppContext,
		new OSF.DDA.BindingFacade(this),
		settings
	);
	OSF.DDA.DispIdHost.addAsyncMethods(this, [
		OSF.DDA.AsyncMethodNames.GetDocumentCopyAsync
	]);
	OSF.OUtil.defineEnumerableProperty(this, "customXmlParts", {
		value: new OSF.DDA.CustomXmlParts()
	});
	OSF.OUtil.finalizeProperties(this);
};
OSF.OUtil.extend(OSF.DDA.WordDocument, OSF.DDA.JsomDocument);

