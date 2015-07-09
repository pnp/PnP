/* Excel web application specific API library */
/* Version: 15.0.4420.1017 Build Time: 03/31/2014 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/

OSF.OUtil.setNamespace("XLS", OSF.DDA);
OSF.OUtil.augmentList(Microsoft.Office.WebExtension.FilterType, {
	OnlyVisible: "onlyVisible"
});
OSF.OUtil.augmentList(Microsoft.Office.WebExtension.EventType, {
	SettingsChanged: "settingsChanged"
});
OSF.DDA.XLS.UniqueArguments={
	Data: "Data",
	Properties : "Properties",
	BindingRequest: "DdaBindingsMethod",
	BindingResponse: "Bindings",
	SingleBindingResponse: "singleBindingResponse",
	GetData: "DdaGetBindingData",
	AddRowsColumns: "DdaAddRowsColumns",
	SetData: "DdaSetBindingData",
	SettingsRequest: "DdaSettingsMethod",
	BindingEventSource: "ddaBinding"
};
OSF.DDA.XLS.SettingsTranslator=(function () {
	var keyIndex=0;
	var valueIndex=1;
	return {
		read: function OSF_DDA_XLS_SettingsTranslator$read(payload) {
			var serializedSettings={};
			var settingsPayload=payload.Settings;
			for (var index in settingsPayload) {
				var setting=settingsPayload[index];
				serializedSettings[setting[keyIndex]]=setting[valueIndex];
			}
			return serializedSettings;
		},
		write: function OSF_DDA_XLS_SettingsTranslator$write(serializedSettings) {
			var settingsPayload=[];
			for (var key in serializedSettings) {
				var setting=[];
				setting[keyIndex]=key;
				setting[valueIndex]=serializedSettings[key];
				settingsPayload.push(setting);
			}
			return settingsPayload;
		}
	}
})();
OSF.OUtil.setNamespace("Delegate", OSF.DDA.XLS);
OSF.DDA.DispIdHost.getXLSDelegateMethods=function OSF_DDA_DispIdHost_getXLSDelegateMethods() {
	var delegateMethods={};
	delegateMethods[OSF.DDA.DispIdHost.Delegates.ExecuteAsync]=OSF.DDA.XLS.Delegate.executeAsync;
	delegateMethods[OSF.DDA.DispIdHost.Delegates.RegisterEventAsync]=OSF.DDA.XLS.Delegate.registerEventAsync;
	delegateMethods[OSF.DDA.DispIdHost.Delegates.UnregisterEventAsync]=OSF.DDA.XLS.Delegate.unregisterEventAsync;
	return delegateMethods;
};
OSF.DDA.XLS.Delegate.SpecialProcessor=function OSF_DDA_XLS_Delegate_SpecialProcessor() {
	var complexTypes=[
		OSF.DDA.PropertyDescriptors.BindingProperties,
		OSF.DDA.XLS.UniqueArguments.SingleBindingResponse,
		OSF.DDA.XLS.UniqueArguments.BindingRequest,
		OSF.DDA.XLS.UniqueArguments.BindingResponse,
		OSF.DDA.XLS.UniqueArguments.GetData,
		OSF.DDA.XLS.UniqueArguments.AddRowsColumns,
		OSF.DDA.XLS.UniqueArguments.SetData,
		OSF.DDA.XLS.UniqueArguments.SettingsRequest,
		OSF.DDA.XLS.UniqueArguments.BindingEventSource,
		OSF.DDA.EventDescriptors.BindingSelectionChangedEvent
	];
	var dynamicTypes={};
	dynamicTypes[Microsoft.Office.WebExtension.Parameters.Data]=(function () {
		var tableRows="Rows";
		var tableHeaders="Headers";
		return {
			toHost: function OSF_DDA_XLS_Delegate_SpecialProcessor_Data$toHost(data) {
				if (typeof data !="string" && data[OSF.DDA.TableDataProperties.TableRows] !==undefined) {
					var tableData={};
					tableData[tableRows]=data[OSF.DDA.TableDataProperties.TableRows];
					tableData[tableHeaders]=data[OSF.DDA.TableDataProperties.TableHeaders];
					data=tableData;
				}
				else if (OSF.DDA.DataCoercion.determineCoercionType(data)==Microsoft.Office.WebExtension.CoercionType.Text) {
					data=[[data]];
				}
				return data;
			},
			fromHost: function OSF_DDA_XLS_Delegate_SpecialProcessor_Data$fromHost(hostArgs) {
				var ret;
				if (hostArgs[tableRows] !=undefined) {
					ret={};
					ret[OSF.DDA.TableDataProperties.TableRows]=hostArgs[tableRows];
					ret[OSF.DDA.TableDataProperties.TableHeaders]=hostArgs[tableHeaders];
				}
				else {
					ret=hostArgs;
				}
				return ret;
			}
		}
	})();
	dynamicTypes[OSF.DDA.SettingsManager.SerializedSettings]={
		toHost: OSF.DDA.XLS.SettingsTranslator.write,
		fromHost: OSF.DDA.XLS.SettingsTranslator.read
	};
	OSF.DDA.XLS.Delegate.SpecialProcessor.uber.constructor.call(this, complexTypes, dynamicTypes);
	this.pack=function OSF_DDA_XLS_Delegate_SpecialProcessor$pack(param, arg) {
		var value;
		if (this.isDynamicType(param)) {
			value=dynamicTypes[param].toHost(arg);
		}
		else {
			value=arg;
		}
		return value;
	};
	this.unpack=function OSF_DDA_XLS_Delegate_SpecialProcessor$unpack(param, arg) {
		var value;
		if (this.isDynamicType(param)) {
			value=dynamicTypes[param].fromHost(arg);
		}
		else {
			value=arg;
		}
		return value;
	};
};
OSF.OUtil.extend(OSF.DDA.XLS.Delegate.SpecialProcessor, OSF.DDA.SpecialProcessor);
OSF.DDA.XLS.Delegate.ParameterMap=(function () {
	var parameterMap=new OSF.DDA.HostParameterMap(new OSF.DDA.XLS.Delegate.SpecialProcessor());
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
	ns=Microsoft.Office.WebExtension.Parameters;
	define({
		type: OSF.DDA.XLS.UniqueArguments.BindingRequest,
		toHost: [
			{ name: ns.ItemName, value: "ItemName" },
			{ name: ns.Id, value: "BindingId" },
			{ name: ns.BindingType, value: "BindingType" },
			{ name: ns.PromptText, value: "PromptText" },
			{ name: ns.FailOnCollision, value: "FailOnCollision" }
		]
	});
	define({
		type: OSF.DDA.XLS.UniqueArguments.GetData,
		toHost: [
			{ name: ns.Id, value: "BindingId" },
			{ name: ns.CoercionType, value: "CoerceType" },
			{ name: ns.ValueFormat, value: "ValueFormat" },
			{ name: ns.FilterType, value: "FilterType" },
			{ name: ns.StartRow, value: "StartRow" },
			{ name: ns.StartColumn, value: "StartCol" },
			{ name: ns.RowCount, value: "RowCount" },
			{ name: ns.ColumnCount, value: "ColCount" }
		]
	});
	define({
		type: OSF.DDA.XLS.UniqueArguments.SetData,
		toHost: [
			{ name: ns.Id, value: "BindingId" },
			{ name: ns.CoercionType, value: "CoerceType" },
			{ name: ns.Data, value: OSF.DDA.XLS.UniqueArguments.Data },
			{ name: ns.StartRow, value: "StartRow" },
			{ name: ns.StartColumn, value: "StartCol" }
		]
	});
	define({
		type: OSF.DDA.XLS.UniqueArguments.AddRowsColumns,
		toHost: [
			{ name: ns.Id, value: "BindingId" },
			{ name: ns.Data, value: OSF.DDA.XLS.UniqueArguments.Data }
		]
	});
	define({
		type: OSF.DDA.XLS.UniqueArguments.SettingsRequest,
		toHost: [
			{ name: ns.OverwriteIfStale, value: "OverwriteIfStale" },
			{ name: OSF.DDA.SettingsManager.SerializedSettings, value: OSF.DDA.XLS.UniqueArguments.Properties }
		],
		invertible: true
	});
	ns=Microsoft.Office.WebExtension.BindingType;
	define({
		type: Microsoft.Office.WebExtension.Parameters.BindingType,
		toHost: [
			{ name: ns.Text, value: 2 },
			{ name: ns.Matrix, value: 3 },
			{ name: ns.Table, value: 1 }
		],
		invertible: true
	});
	ns=OSF.DDA.BindingProperties;
	define({
		type: OSF.DDA.PropertyDescriptors.BindingProperties,
		fromHost: [
			{ name: ns.Id, value: "Name" },
			{ name: ns.Type, value: "BindingType" },
			{ name: ns.RowCount, value: "RowCount" },
			{ name: ns.ColumnCount, value: "ColCount" },
			{ name: ns.HasHeaders, value: "HasHeaders" }
		]
	});
	define({
		type: OSF.DDA.XLS.UniqueArguments.SingleBindingResponse,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: 0 }
		]
	});
	define({
		type: OSF.DDA.PropertyDescriptors.Subset,
		fromHost: [
			{ name: ns.StartRow, value: "StartRow" },
			{ name: ns.StartColumn, value: "StartCol" },
			{ name: ns.RowCount, value: "RowCount" },
			{ name: ns.ColumnCount, value: "ColCount" }
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
	define({
		type: OSF.DDA.EventDescriptors.BindingSelectionChangedEvent,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: OSF.DDA.XLS.UniqueArguments.BindingEventSource },
			{ name: OSF.DDA.PropertyDescriptors.Subset, value: OSF.DDA.PropertyDescriptors.Subset }
		]
	});
	ns=OSF.DDA.XLS.UniqueArguments;
	var cns=OSF.DDA.MethodDispId;
	define({
		type: cns.dispidGetSelectedDataMethod,
		fromHost: [
			{ name: Microsoft.Office.WebExtension.Parameters.Data, value: ns.Data }
		],
		toHost: [
			{ name: ns.GetData, value: self }
		]
	});
	define({
		type: cns.dispidSetSelectedDataMethod,
		toHost: [
			{ name: ns.SetData, value: self }
		]
	});
	define({
		type: cns.dispidAddBindingFromSelectionMethod,
		fromHost: [
			{ name: OSF.DDA.XLS.UniqueArguments.SingleBindingResponse, value: OSF.DDA.XLS.UniqueArguments.BindingResponse }
		],
		toHost: [
			{ name: ns.BindingRequest, value: self }
		]
	});
	define({
		type: cns.dispidAddBindingFromPromptMethod,
		fromHost: [
			{ name: OSF.DDA.XLS.UniqueArguments.SingleBindingResponse, value: OSF.DDA.XLS.UniqueArguments.BindingResponse }
		],
		toHost: [
			{ name: ns.BindingRequest, value: self }
		]
	});
	define({
		type: cns.dispidAddBindingFromNamedItemMethod,
		fromHost: [
			{ name: OSF.DDA.XLS.UniqueArguments.SingleBindingResponse, value: OSF.DDA.XLS.UniqueArguments.BindingResponse }
		],
		toHost: [
			{ name: ns.BindingRequest, value: self }
		]
	});
	define({
		type: cns.dispidReleaseBindingMethod,
		toHost: [
			{ name: ns.BindingRequest, value: self }
		]
	});
	define({
		type: cns.dispidGetBindingMethod,
		fromHost: [
			{ name: OSF.DDA.XLS.UniqueArguments.SingleBindingResponse, value: OSF.DDA.XLS.UniqueArguments.BindingResponse }
		],
		toHost: [
			{ name: ns.BindingRequest, value: self }
		]
	});
	define({
		type: cns.dispidGetAllBindingsMethod,
		fromHost: [
			{ name: OSF.DDA.ListDescriptors.BindingList, value: OSF.DDA.XLS.UniqueArguments.BindingResponse }
		]
	});
	define({
		type: cns.dispidGetBindingDataMethod,
		fromHost: [
			{ name: Microsoft.Office.WebExtension.Parameters.Data, value: ns.Data }
		],
		toHost: [
			{ name: ns.GetData, value: self }
		]
	});
	define({
		type: cns.dispidSetBindingDataMethod,
		toHost: [
			{ name: ns.SetData, value: self }
		]
	});
	define({
		type: cns.dispidAddRowsMethod,
		toHost: [
			{ name: ns.AddRowsColumns, value: self }
		]
	});
	define({
		type: cns.dispidAddColumnsMethod,
		toHost: [
			{ name: ns.AddRowsColumns, value: self }
		]
	});
	define({
		type: cns.dispidClearAllRowsMethod,
		toHost: [
			{ name: ns.BindingRequest, value: self }
		]
	});
	define({
		type: cns.dispidLoadSettingsMethod,
		fromHost: [
			{ name: OSF.DDA.SettingsManager.SerializedSettings, value: ns.Properties }
		]
	});
	define({
		type: cns.dispidSaveSettingsMethod,
		toHost: [
			{ name: ns.SettingsRequest, value: self }
		]
	});
	cns=OSF.DDA.EventDispId
	define({ type: cns.dispidDocumentSelectionChangedEvent });
	define({
		type: cns.dispidBindingSelectionChangedEvent,
		fromHost: [
			{ name: OSF.DDA.EventDescriptors.BindingSelectionChangedEvent, value: self }
		]
	});
	define({
		type: cns.dispidBindingDataChangedEvent,
		fromHost: [
			{ name: OSF.DDA.PropertyDescriptors.BindingProperties, value: ns.BindingEventSource }
		]
	});
	define({ type: cns.dispidSettingsChangedEvent });
	return parameterMap;
})();
OSF.DDA.XLS.Delegate.version=1;
OSF.DDA.XLS.Delegate.executeAsync=function OSF_DDA_XLS_Delegate$executeAsync(args) {
	if(!args.hostCallArgs) {
		args.hostCallArgs={};
	}
	args.hostCallArgs["DdaMethod"]={
		"ControlId": OSF._OfficeAppFactory.getId(),
		"Version": OSF.DDA.XLS.Delegate.version,
		"DispatchId": args.dispId
	};
	if(args.onCalling) {
		args.onCalling();
	}
	var startTime=(new Date()).getTime();
	OSF._OfficeAppFactory.getClientEndPoint().invoke(
		"executeMethod",
		function OSF_DDA_XLS_Delegate$OMFacade$OnResponse(xdmStatus, payload) {
			if(args.onReceiving) {
				args.onReceiving();
			}
			var error;
			if (xdmStatus==Microsoft.Office.Common.InvokeResultCode.noError) {
				OSF.DDA.XLS.Delegate.version=payload["Version"];
				error=payload["Error"];
			} else {
				switch (xdmStatus) {
					case Microsoft.Office.Common.InvokeResultCode.errorHandlingRequestAccessDenied:
						error=OSF.DDA.ErrorCodeManager.errorCodes.ooeNoCapability;
						break;
					default:
						error=OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
						break;
				}
			}
			if (args.onComplete) {
				args.onComplete(error, payload);
			}
			if (OSF.AppTelemetry) {
				OSF.AppTelemetry.onMethodDone(args.dispId, null, Math.abs((new Date()).getTime() -  startTime), error);
			}
		},
		args.hostCallArgs
	);
};
OSF.DDA.XLS.Delegate._getOnAfterRegisterEvent=function OSF_DDA_XLS_Delegate$GetOnAfterRegisterEvent(register, args) {
	var startTime=(new Date()).getTime();
	return function OSF_DDA_XLS_Delegate$OnAfterRegisterEvent(xdmStatus, succeeded) {
		if (args.onReceiving) {
				args.onReceiving();
			}
		var status;
		if (xdmStatus !=Microsoft.Office.Common.InvokeResultCode.noError) {
			switch (xdmStatus) {
				case Microsoft.Office.Common.InvokeResultCode.errorHandlingRequestAccessDenied:
					status=OSF.DDA.ErrorCodeManager.errorCodes.ooeNoCapability;
					break;
				default:
					status=OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
					break;
			}
		} else {
			status=succeeded ? OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess : OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
		}
		if (args.onComplete) {
			args.onComplete(status);
		}
		if (OSF.AppTelemetry) {
			OSF.AppTelemetry.onRegisterDone(register, args.dispId, Math.abs((new Date()).getTime() -  startTime), status);
		}
	}
};
OSF.DDA.XLS.Delegate.registerEventAsync=function OSF_DDA_XLS_Delegate$RegisterEventAsync(args) {
	if (args.onCalling) {
		args.onCalling();
	}
	OSF._OfficeAppFactory.getClientEndPoint().registerForEvent(
		OSF.DDA.getXdmEventName(args.targetId, args.eventType),
		function OSF_DDA_XLSOMFacade$OnEvent(payload) {
			if (args.onEvent) {
				args.onEvent(payload);
			}
			if (OSF.AppTelemetry) {
				OSF.AppTelemetry.onEventDone(args.dispId);
			}
		},
		OSF.DDA.XLS.Delegate._getOnAfterRegisterEvent(true, args),
		{
			"controlId": OSF._OfficeAppFactory.getId(),
			"eventDispId": args.dispId,
			"targetId": args.targetId
		}
	);
};
OSF.DDA.XLS.Delegate.unregisterEventAsync=function OSF_DDA_XLS_Delegate$UnregisterEventAsync(args) {
	if (args.onCalling) {
		args.onCalling();
	}
	OSF._OfficeAppFactory.getClientEndPoint().unregisterForEvent(
		OSF.DDA.getXdmEventName(args.targetId, args.eventType),
		OSF.DDA.XLS.Delegate._getOnAfterRegisterEvent(false, args),
		{
			"controlId": OSF._OfficeAppFactory.getId(),
			"eventDispId": args.dispId,
			"targetId": args.targetId
		}
	);
};
OSF.DDA.ExcelWebAppDocument=function Microsoft_Office_WebExtension_ExcelWebAppDocument(officeAppContext, settings) {
	var bf=new OSF.DDA.BindingFacade(this);
	OSF.DDA.DispIdHost.addAsyncMethods(bf, [OSF.DDA.AsyncMethodNames.AddFromPromptAsync]);
	OSF.DDA.ExcelWebAppDocument.uber.constructor.call(this,
		officeAppContext,
		bf,
		settings
	);
	if (this.mode==OSF.ClientMode.ReadOnly) {
		this.url=document.URL;
	}
	OSF.OUtil.finalizeProperties(this);
};
OSF.OUtil.extend(OSF.DDA.ExcelWebAppDocument, OSF.DDA.JsomDocument);

