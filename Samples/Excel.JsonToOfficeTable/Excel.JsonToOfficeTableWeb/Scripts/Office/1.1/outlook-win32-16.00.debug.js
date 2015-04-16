/* Outlook specific API library */
/* Version: 16.0.2420.1000 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/

Type.registerNamespace('Microsoft.Office.WebExtension.MailboxEnums');
Microsoft.Office.WebExtension.MailboxEnums.EntityType={
	MeetingSuggestion: "meetingSuggestion",	
	TaskSuggestion: "taskSuggestion",
	Address: "address",
	EmailAddress: "emailAddress",
	Url: "url",
	PhoneNumber: "phoneNumber",
	Contact: "contact"
};
Microsoft.Office.WebExtension.MailboxEnums.ItemType={
	Message: 'message',
	Appointment: 'appointment'
};
Microsoft.Office.WebExtension.MailboxEnums.ResponseType={
	None: "none",
	Organizer: "organizer",
	Tentative: "tentative",
	Accepted: "accepted",
	Declined: "declined"
};
Microsoft.Office.WebExtension.MailboxEnums.RecipientType={
	Other: "other",
	DistributionList: "distributionList",
	User: "user",
	ExternalUser: "externalUser"
};
Type.registerNamespace('OSF.DDA');
OSF.DDA.OutlookAppOm=function OSF_DDA_OutlookAppOm(officeAppContext, targetWindow, appReadyCallback) {
	this.$$d__callAppReadyCallback$p$0=Function.createDelegate(this, this._callAppReadyCallback$p$0);
	this.$$d__getDiagnostics$p$0=Function.createDelegate(this, this._getDiagnostics$p$0);
	this.$$d__getUserProfile$p$0=Function.createDelegate(this, this._getUserProfile$p$0);
	this.$$d__getItem$p$0=Function.createDelegate(this, this._getItem$p$0);
	this.$$d__getInitialDataResponseHandler$p$0=Function.createDelegate(this, this._getInitialDataResponseHandler$p$0);
	OSF.DDA.OutlookAppOm._instance$p=this;
	this._officeAppContext$p$0=officeAppContext;
	this._appReadyCallback$p$0=appReadyCallback;
	var $$t_4=this;
	var stringLoadedCallback=function() {
		if (appReadyCallback) {
			$$t_4._invokeHostMethod$i$0(1, 'GetInitialData', null, $$t_4.$$d__getInitialDataResponseHandler$p$0);
		}
	};
	if (this._areStringsLoaded$p$0()) {
		stringLoadedCallback();
	}
	else {
		this._loadLocalizedScript$p$0(stringLoadedCallback);
	}
}
OSF.DDA.OutlookAppOm._createAsyncResult$i=function OSF_DDA_OutlookAppOm$_createAsyncResult$i(value, errorCode, errorDescription, userContext) {
	var initArgs={};
	initArgs[OSF.DDA.AsyncResultEnum.Properties.Value]=value;
	initArgs[OSF.DDA.AsyncResultEnum.Properties.Context]=userContext;
	var errorArgs=null;
	if (0 !==errorCode) {
		errorArgs={};
		errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Name]=errorCode;
		errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Message]=errorDescription;
	}
	return new OSF.DDA.AsyncResult(initArgs, errorArgs);
}
OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i=function OSF_DDA_OutlookAppOm$_throwOnPropertyAccessForRestrictedPermission$i(currentPermissionLevel) {
	if (!currentPermissionLevel) {
		throw Error.create(_u.ExtensibilityStrings.l_ElevatedPermissionNeeded_Text);
	}
}
OSF.DDA.OutlookAppOm._throwOnMethodCallForInsufficientPermission$i=function OSF_DDA_OutlookAppOm$_throwOnMethodCallForInsufficientPermission$i(currentPermissionLevel, requiredPermissionLevel, methodName) {
	if (currentPermissionLevel < requiredPermissionLevel) {
		throw Error.create(String.format(_u.ExtensibilityStrings.l_ElevatedPermissionNeededForMethod_Text, methodName));
	}
}
OSF.DDA.OutlookAppOm._throwOnArgumentType$p=function OSF_DDA_OutlookAppOm$_throwOnArgumentType$p(value, expectedType, argumentName) {
	if (Object.getType(value) !==expectedType) {
		throw Error.argumentType(argumentName);
	}
}
OSF.DDA.OutlookAppOm._throwOnOutOfRange$p=function OSF_DDA_OutlookAppOm$_throwOnOutOfRange$p(value, minValue, maxValue, argumentName) {
	if (value < minValue || value > maxValue) {
		throw Error.argumentOutOfRange(argumentName);
	}
}
OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p=function OSF_DDA_OutlookAppOm$_validateOptionalStringParameter$p(value, minLength, maxLength, name) {
	if ($h.ScriptHelpers.isNullOrUndefined(value)) {
		return;
	}
	OSF.DDA.OutlookAppOm._throwOnArgumentType$p(value, String, name);
	var stringValue=value;
	OSF.DDA.OutlookAppOm._throwOnOutOfRange$p(stringValue.length, minLength, maxLength, name);
}
OSF.DDA.OutlookAppOm._convertToOutlookParameters$p=function OSF_DDA_OutlookAppOm$_convertToOutlookParameters$p(dispid, data) {
	var executeParameters=null;
	switch (dispid) {
		case 1:
		case 2:
		case 3:
			break;
		case 4:
			var jsonProperty=JSON.stringify(data['customProperties']);
			executeParameters=[ jsonProperty ];
			break;
		case 5:
			executeParameters=[ data['body'] ];
			break;
		case 8:
		case 9:
			executeParameters=[ data['itemId'] ];
			break;
		case 7:
			executeParameters=[ OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlook$p(data['requiredAttendees']), OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlook$p(data['optionalAttendees']), data['start'], data['end'], data['location'], OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlook$p(data['resources']), data['subject'], data['body'] ];
			break;
		case 11:
		case 10:
			executeParameters=[ data['htmlBody'] ];
			break;
		default:
			Sys.Debug.fail('Unexpected method dispid');
			break;
	}
	return executeParameters;
}
OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlook$p=function OSF_DDA_OutlookAppOm$_convertRecipientArrayParameterForOutlook$p(array) {
	return (array) ? array.join(';') : null;
}
OSF.DDA.OutlookAppOm._validateAndNormalizeRecipientEmails$p=function OSF_DDA_OutlookAppOm$_validateAndNormalizeRecipientEmails$p(emailset, name) {
	if ($h.ScriptHelpers.isNullOrUndefined(emailset)) {
		return null;
	}
	OSF.DDA.OutlookAppOm._throwOnArgumentType$p(emailset, Array, name);
	var originalAttendees=emailset;
	var updatedAttendees=null;
	var normalizationNeeded=false;
	OSF.DDA.OutlookAppOm._throwOnOutOfRange$p(originalAttendees.length, 0, OSF.DDA.OutlookAppOm._maxRecipients$p, String.format('{0}.length', name));
	for (var i=0; i < originalAttendees.length; i++) {
		if ($h.EmailAddressDetails.isInstanceOfType(originalAttendees[i])) {
			normalizationNeeded=true;
			break;
		}
	}
	if (normalizationNeeded) {
		updatedAttendees=[];
	}
	for (var i=0; i < originalAttendees.length; i++) {
		if (normalizationNeeded) {
			updatedAttendees[i]=($h.EmailAddressDetails.isInstanceOfType(originalAttendees[i])) ? (originalAttendees[i]).emailAddress : originalAttendees[i];
			OSF.DDA.OutlookAppOm._throwOnArgumentType$p(updatedAttendees[i], String, String.format('{0}[{1}]', name, i));
		}
		else {
			OSF.DDA.OutlookAppOm._throwOnArgumentType$p(originalAttendees[i], String, String.format('{0}[{1}]', name, i));
		}
	}
	return updatedAttendees;
}
OSF.DDA.OutlookAppOm.prototype={
	_initialData$p$0: null,
	_item$p$0: null,
	_userProfile$p$0: null,
	_diagnostics$p$0: null,
	_officeAppContext$p$0: null,
	_appReadyCallback$p$0: null,
	initialize: function OSF_DDA_OutlookAppOm$initialize(initialData) {
		var ItemTypeKey='itemType';
		this._initialData$p$0=new $h.InitialData(initialData);
		if (1===initialData[ItemTypeKey]) {
			this._item$p$0=new $h.Message(this._initialData$p$0);
		}
		else if (3===initialData[ItemTypeKey]) {
			this._item$p$0=new $h.MeetingRequest(this._initialData$p$0);
		}
		else if (2===initialData[ItemTypeKey]) {
			this._item$p$0=new $h.Appointment(this._initialData$p$0);
		}
		else {
			Sys.Debug.trace('Unexpected item type was received from the host.');
		}
		this._userProfile$p$0=new $h.UserProfile(this._initialData$p$0);
		this._diagnostics$p$0=new $h.Diagnostics(this._initialData$p$0, this._officeAppContext$p$0.get_appName());
		$h.InitialData._defineReadOnlyProperty$i(this, 'item', this.$$d__getItem$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this, 'userProfile', this.$$d__getUserProfile$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this, 'diagnostics', this.$$d__getDiagnostics$p$0);
	},
	makeEwsRequestAsync: function OSF_DDA_OutlookAppOm$makeEwsRequestAsync(data, callback, userContext) {
		if ($h.ScriptHelpers.isNullOrUndefined(data)) {
			throw Error.argumentNull('data');
		}
		if (data.length > OSF.DDA.OutlookAppOm._maxEwsRequestSize$p) {
			throw Error.argument('data', _u.ExtensibilityStrings.l_EwsRequestOversized_Text);
		}
		OSF.DDA.OutlookAppOm._throwOnMethodCallForInsufficientPermission$i(this._initialData$p$0.get__permissionLevel$i$0(), 2, 'makeEwsRequestAsync');
		var ewsRequest=new $h.EwsRequest(userContext);
		var $$t_4=this;
		ewsRequest.onreadystatechange=function() {
			if (4===ewsRequest.get__requestState$i$1()) {
				callback(ewsRequest._asyncResult$p$0);
			}
		};
		ewsRequest.send(data);
	},
	recordDataPoint: function OSF_DDA_OutlookAppOm$recordDataPoint(data) {
		if ($h.ScriptHelpers.isNullOrUndefined(data)) {
			throw Error.argumentNull('data');
		}
		this._invokeHostMethod$i$0(0, 'RecordDataPoint', data, null);
	},
	recordTrace: function OSF_DDA_OutlookAppOm$recordTrace(data) {
		if ($h.ScriptHelpers.isNullOrUndefined(data)) {
			throw Error.argumentNull('data');
		}
		this._invokeHostMethod$i$0(0, 'RecordTrace', data, null);
	},
	trackCtq: function OSF_DDA_OutlookAppOm$trackCtq(data) {
		if ($h.ScriptHelpers.isNullOrUndefined(data)) {
			throw Error.argumentNull('data');
		}
		this._invokeHostMethod$i$0(0, 'TrackCtq', data, null);
	},
	convertToLocalClientTime: function OSF_DDA_OutlookAppOm$convertToLocalClientTime(timeValue) {
		var date=new Date(timeValue.getTime());
		var offset=date.getTimezoneOffset() * -1;
		if (this._initialData$p$0 && this._initialData$p$0.get__timeZoneOffsets$i$0()) {
			date.setUTCMinutes(date.getUTCMinutes() - offset);
			offset=this._findOffset$p$0(date);
			date.setUTCMinutes(date.getUTCMinutes()+offset);
		}
		var retValue=this._dateToDictionary$i$0(date);
		retValue['timezoneOffset']=offset;
		return retValue;
	},
	convertToUtcClientTime: function OSF_DDA_OutlookAppOm$convertToUtcClientTime(input) {
		var retValue=this._dictionaryToDate$i$0(input);
		if (this._initialData$p$0 && this._initialData$p$0.get__timeZoneOffsets$i$0()) {
			var offset=this._findOffset$p$0(retValue);
			retValue.setUTCMinutes(retValue.getUTCMinutes() - offset);
			offset=(!input['timezoneOffset']) ? retValue.getTimezoneOffset() * -1 : input['timezoneOffset'];
			retValue.setUTCMinutes(retValue.getUTCMinutes()+offset);
		}
		return retValue;
	},
	getUserIdentityTokenAsync: function OSF_DDA_OutlookAppOm$getUserIdentityTokenAsync(callback, userContext) {
		OSF.DDA.OutlookAppOm._throwOnMethodCallForInsufficientPermission$i(this._initialData$p$0.get__permissionLevel$i$0(), 1, 'getUserIdentityTokenAsync');
		if ($h.ScriptHelpers.isNullOrUndefined(callback)) {
			throw Error.argumentNull('callback');
		}
		var $$t_6=this;
		this._invokeHostMethod$i$0(2, 'GetUserIdentityToken', null, function(resultCode, response) {
			if (resultCode) {
				OSF.DDA.OutlookAppOm._createAsyncResult$i(null, 1, String.format(_u.ExtensibilityStrings.l_InternalProtocolError_Text, resultCode), userContext);
			}
			else {
				var responseDictionary=response;
				var asyncResult;
				if (responseDictionary['wasSuccessful']) {
					asyncResult=OSF.DDA.OutlookAppOm._createAsyncResult$i(responseDictionary['token'], 0, null, userContext);
				}
				else {
					asyncResult=OSF.DDA.OutlookAppOm._createAsyncResult$i(null, 1, responseDictionary['errorMessage'], userContext);
				}
				callback(asyncResult);
			}
		});
	},
	displayMessageForm: function OSF_DDA_OutlookAppOm$displayMessageForm(itemId) {
		if ($h.ScriptHelpers.isNullOrUndefined(itemId)) {
			throw Error.argumentNull('itemId');
		}
		this._invokeHostMethod$i$0(8, 'DisplayExistingMessageForm', { itemId: itemId }, null);
	},
	displayAppointmentForm: function OSF_DDA_OutlookAppOm$displayAppointmentForm(itemId) {
		if ($h.ScriptHelpers.isNullOrUndefined(itemId)) {
			throw Error.argumentNull('itemId');
		}
		this._invokeHostMethod$i$0(9, 'DisplayExistingAppointmentForm', { itemId: itemId }, null);
	},
	displayNewAppointmentForm: function OSF_DDA_OutlookAppOm$displayNewAppointmentForm(parameters) {
		var normalizedRequiredAttendees=OSF.DDA.OutlookAppOm._validateAndNormalizeRecipientEmails$p(parameters['requiredAttendees'], 'requiredAttendees');
		var normalizedOptionalAttendees=OSF.DDA.OutlookAppOm._validateAndNormalizeRecipientEmails$p(parameters['optionalAttendees'], 'optionalAttendees');
		OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p(parameters['location'], 0, OSF.DDA.OutlookAppOm._maxLocationLength$p, 'location');
		OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p(parameters['body'], 0, OSF.DDA.OutlookAppOm._maxBodyLength$p, 'body');
		OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p(parameters['subject'], 0, OSF.DDA.OutlookAppOm._maxSubjectLength$p, 'subject');
		if (!$h.ScriptHelpers.isNullOrUndefined(parameters['start'])) {
			OSF.DDA.OutlookAppOm._throwOnArgumentType$p(parameters['start'], Date, 'start');
			var startDateTime=parameters['start'];
			parameters['start']=startDateTime.getTime();
			if (!$h.ScriptHelpers.isNullOrUndefined(parameters['end'])) {
				OSF.DDA.OutlookAppOm._throwOnArgumentType$p(parameters['end'], Date, 'end');
				var endDateTime=parameters['end'];
				if (endDateTime < startDateTime) {
					throw Error.argumentOutOfRange('end', endDateTime, _u.ExtensibilityStrings.l_InvalidEventDates_Text);
				}
				parameters['end']=endDateTime.getTime();
			}
		}
		var updatedParameters=null;
		if (normalizedRequiredAttendees || normalizedOptionalAttendees) {
			updatedParameters={};
			var $$dict_6=parameters;
			for (var $$key_7 in $$dict_6) {
				var entry={ key: $$key_7, value: $$dict_6[$$key_7] };
				updatedParameters[entry.key]=entry.value;
			}
			if (normalizedRequiredAttendees) {
				updatedParameters['requiredAttendees']=normalizedRequiredAttendees;
			}
			if (normalizedOptionalAttendees) {
				updatedParameters['optionalAttendees']=normalizedOptionalAttendees;
			}
		}
		this._invokeHostMethod$i$0(7, 'DisplayNewAppointmentForm', updatedParameters || parameters, null);
	},
	_displayReplyForm$i$0: function OSF_DDA_OutlookAppOm$_displayReplyForm$i$0(htmlBody) {
		if (!$h.ScriptHelpers.isNullOrUndefined(htmlBody)) {
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$p(htmlBody.length, 0, OSF.DDA.OutlookAppOm._maxBodyLength$p, 'htmlBody');
		}
		this._invokeHostMethod$i$0(10, 'DisplayReplyForm', { htmlBody: htmlBody }, null);
	},
	_displayReplyAllForm$i$0: function OSF_DDA_OutlookAppOm$_displayReplyAllForm$i$0(htmlBody) {
		if (!$h.ScriptHelpers.isNullOrUndefined(htmlBody)) {
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$p(htmlBody.length, 0, OSF.DDA.OutlookAppOm._maxBodyLength$p, 'htmlBody');
		}
		this._invokeHostMethod$i$0(11, 'DisplayReplyAllForm', { htmlBody: htmlBody }, null);
	},
	_invokeHostMethod$i$0: function OSF_DDA_OutlookAppOm$_invokeHostMethod$i$0(dispid, name, data, responseCallback) {
		if (64===this._officeAppContext$p$0.get_appName()) {
			OSF._OfficeAppFactory.getClientEndPoint().invoke(name, responseCallback, data);
		}
		else if (dispid) {
			var executeParameters=OSF.DDA.OutlookAppOm._convertToOutlookParameters$p(dispid, data);
			var $$t_9=this;
			window.external.Execute(dispid, executeParameters, function(nativeData, resultCode) {
				if (responseCallback) {
					var serializedData=nativeData.getItem(0);
					var deserializedData=JSON.parse(serializedData);
					responseCallback(resultCode, deserializedData);
				}
			});
		}
		else if (responseCallback) {
			responseCallback(-2, null);
		}
	},
	_dictionaryToDate$i$0: function OSF_DDA_OutlookAppOm$_dictionaryToDate$i$0(input) {
		var retValue=new Date(input['year'], input['month'], input['date'], input['hours'], input['minutes'], input['seconds'], (!input['milliseconds']) ? 0 : input['milliseconds']);
		if (isNaN(retValue.getTime())) {
			throw Error.format(_u.ExtensibilityStrings.l_InvalidDate_Text);
		}
		return retValue;
	},
	_dateToDictionary$i$0: function OSF_DDA_OutlookAppOm$_dateToDictionary$i$0(input) {
		var retValue={};
		retValue['month']=input.getMonth();
		retValue['date']=input.getDate();
		retValue['year']=input.getFullYear();
		retValue['hours']=input.getHours();
		retValue['minutes']=input.getMinutes();
		retValue['seconds']=input.getSeconds();
		retValue['milliseconds']=input.getMilliseconds();
		return retValue;
	},
	_getInitialDataResponseHandler$p$0: function OSF_DDA_OutlookAppOm$_getInitialDataResponseHandler$p$0(resultCode, data) {
		if (resultCode) {
			return;
		}
		this.initialize(data);
		(this).displayName='mailbox';
		window.setTimeout(this.$$d__callAppReadyCallback$p$0, 0);
	},
	_callAppReadyCallback$p$0: function OSF_DDA_OutlookAppOm$_callAppReadyCallback$p$0() {
		this._appReadyCallback$p$0();
	},
	_getItem$p$0: function OSF_DDA_OutlookAppOm$_getItem$p$0() {
		return this._item$p$0;
	},
	_getUserProfile$p$0: function OSF_DDA_OutlookAppOm$_getUserProfile$p$0() {
		OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._initialData$p$0.get__permissionLevel$i$0());
		return this._userProfile$p$0;
	},
	_getDiagnostics$p$0: function OSF_DDA_OutlookAppOm$_getDiagnostics$p$0() {
		return this._diagnostics$p$0;
	},
	_findOffset$p$0: function OSF_DDA_OutlookAppOm$_findOffset$p$0(value) {
		var ranges=this._initialData$p$0.get__timeZoneOffsets$i$0();
		for (var r=0; r < ranges.length; r++) {
			var range=ranges[r];
			var start=parseInt(range['start']);
			var end=parseInt(range['end']);
			if (value.getTime() - start >=0 && value.getTime() - end < 0) {
				return parseInt(range['offset']);
			}
		}
		throw Error.format(_u.ExtensibilityStrings.l_InvalidDate_Text);
	},
	_areStringsLoaded$p$0: function OSF_DDA_OutlookAppOm$_areStringsLoaded$p$0() {
		var stringsLoaded=false;
		try {
			stringsLoaded=!$h.ScriptHelpers.isNullOrUndefined(_u.ExtensibilityStrings.l_EwsRequestOversized_Text);
		}
		catch ($$e_1) {
		}
		return stringsLoaded;
	},
	_loadLocalizedScript$p$0: function OSF_DDA_OutlookAppOm$_loadLocalizedScript$p$0(stringLoadedCallback) {
		var url=null;
		var baseUrl='';
		var scripts=document.getElementsByTagName('script');
		for (var i=scripts.length - 1; i >=0; i--) {
			var filename=null;
			var attributes=scripts[i].attributes;
			if (attributes) {
				var attribute=attributes.getNamedItem('src');
				if (attribute) {
					filename=attribute.value;
				}
				if (filename) {
					var debug=false;
					filename=filename.toLowerCase();
					var officeIndex=filename.indexOf('office_strings.js');
					if (officeIndex < 0) {
						officeIndex=filename.indexOf('office_strings.debug.js');
						debug=true;
					}
					if ((officeIndex > 0) && (officeIndex < filename.length)) {
						url=filename.replace((debug) ? 'office_strings.debug.js' : 'office_strings.js', 'outlook_strings.js');
						var languageUrl=filename.substring(0, officeIndex);
						var lastIndexOfSlash=languageUrl.lastIndexOf('/', languageUrl.length - 2);
						if (lastIndexOfSlash===-1) {
							lastIndexOfSlash=languageUrl.lastIndexOf('\\', languageUrl.length - 2);
						}
						if (lastIndexOfSlash !==-1 && languageUrl.length > lastIndexOfSlash+1) {
							baseUrl=languageUrl.substring(0, lastIndexOfSlash+1);
						}
						break;
					}
				}
			}
		}
		if (url) {
			var head=document.getElementsByTagName('head')[0];
			var scriptElement=null;
			var $$t_H=this;
			var scriptElementCallback=function() {
				if (stringLoadedCallback && (!scriptElement.readyState || (scriptElement.readyState && (scriptElement.readyState==='loaded' || scriptElement.readyState==='complete')))) {
					scriptElement.onload=null;
					scriptElement.onreadystatechange=null;
					stringLoadedCallback();
				}
			};
			var $$t_I=this;
			var failureCallback=function() {
				if (!$$t_I._areStringsLoaded$p$0()) {
					var fallbackUrl=baseUrl+'en-us/'+'outlook_strings.js';
					scriptElement.onload=null;
					scriptElement.onreadystatechange=null;
					scriptElement=$$t_I._createScriptElement$p$0(fallbackUrl);
					scriptElement.onload=scriptElementCallback;
					scriptElement.onreadystatechange=scriptElementCallback;
					head.appendChild(scriptElement);
				}
			};
			scriptElement=this._createScriptElement$p$0(url);
			scriptElement.onload=scriptElementCallback;
			scriptElement.onreadystatechange=scriptElementCallback;
			window.setTimeout(failureCallback, 2000);
			head.appendChild(scriptElement);
		}
	},
	_createScriptElement$p$0: function OSF_DDA_OutlookAppOm$_createScriptElement$p$0(url) {
		var scriptElement=document.createElement('script');
		scriptElement.type='text/javascript';
		scriptElement.src=url;
		return scriptElement;
	}
}
Type.registerNamespace('$h');
$h.Appointment=function $h_Appointment(dataDictionary) {
	this.$$d__getOrganizer$p$1=Function.createDelegate(this, this._getOrganizer$p$1);
	this.$$d__getNormalizedSubject$p$1=Function.createDelegate(this, this._getNormalizedSubject$p$1);
	this.$$d__getSubject$p$1=Function.createDelegate(this, this._getSubject$p$1);
	this.$$d__getResources$p$1=Function.createDelegate(this, this._getResources$p$1);
	this.$$d__getRequiredAttendees$p$1=Function.createDelegate(this, this._getRequiredAttendees$p$1);
	this.$$d__getOptionalAttendees$p$1=Function.createDelegate(this, this._getOptionalAttendees$p$1);
	this.$$d__getLocation$p$1=Function.createDelegate(this, this._getLocation$p$1);
	this.$$d__getEnd$p$1=Function.createDelegate(this, this._getEnd$p$1);
	this.$$d__getStart$p$1=Function.createDelegate(this, this._getStart$p$1);
	$h.Appointment.initializeBase(this, [ dataDictionary ]);
	$h.InitialData._defineReadOnlyProperty$i(this, 'start', this.$$d__getStart$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'end', this.$$d__getEnd$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'location', this.$$d__getLocation$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'optionalAttendees', this.$$d__getOptionalAttendees$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'requiredAttendees', this.$$d__getRequiredAttendees$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'resources', this.$$d__getResources$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'subject', this.$$d__getSubject$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'normalizedSubject', this.$$d__getNormalizedSubject$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'organizer', this.$$d__getOrganizer$p$1);
}
$h.Appointment.prototype={
	getEntities: function $h_Appointment$getEntities() {
		return this._data$p$0._getEntities$i$0();
	},
	getEntitiesByType: function $h_Appointment$getEntitiesByType(entityType) {
		return this._data$p$0._getEntitiesByType$i$0(entityType);
	},
	getRegExMatches: function $h_Appointment$getRegExMatches() {
		OSF.DDA.OutlookAppOm._throwOnMethodCallForInsufficientPermission$i(this._data$p$0.get__permissionLevel$i$0(), 1, 'getRegExMatches');
		return this._data$p$0._getRegExMatches$i$0();
	},
	getFilteredEntitiesByName: function $h_Appointment$getFilteredEntitiesByName(name) {
		return this._data$p$0._getFilteredEntitiesByName$i$0(name);
	},
	getRegExMatchesByName: function $h_Appointment$getRegExMatchesByName(name) {
		OSF.DDA.OutlookAppOm._throwOnMethodCallForInsufficientPermission$i(this._data$p$0.get__permissionLevel$i$0(), 1, 'getRegExMatchesByName');
		return this._data$p$0._getRegExMatchesByName$i$0(name);
	},
	displayReplyForm: function $h_Appointment$displayReplyForm(htmlBody) {
		OSF.DDA.OutlookAppOm._instance$p._displayReplyForm$i$0(htmlBody);
	},
	displayReplyAllForm: function $h_Appointment$displayReplyAllForm(htmlBody) {
		OSF.DDA.OutlookAppOm._instance$p._displayReplyAllForm$i$0(htmlBody);
	},
	getItemType: function $h_Appointment$getItemType() {
		return Microsoft.Office.WebExtension.MailboxEnums.ItemType.Appointment;
	},
	_getStart$p$1: function $h_Appointment$_getStart$p$1() {
		return this._data$p$0.get__start$i$0();
	},
	_getEnd$p$1: function $h_Appointment$_getEnd$p$1() {
		return this._data$p$0.get__end$i$0();
	},
	_getLocation$p$1: function $h_Appointment$_getLocation$p$1() {
		return this._data$p$0.get__location$i$0();
	},
	_getOptionalAttendees$p$1: function $h_Appointment$_getOptionalAttendees$p$1() {
		return this._data$p$0.get__cc$i$0();
	},
	_getRequiredAttendees$p$1: function $h_Appointment$_getRequiredAttendees$p$1() {
		return this._data$p$0.get__to$i$0();
	},
	_getResources$p$1: function $h_Appointment$_getResources$p$1() {
		return this._data$p$0.get__resources$i$0();
	},
	_getSubject$p$1: function $h_Appointment$_getSubject$p$1() {
		return this._data$p$0.get__subject$i$0();
	},
	_getNormalizedSubject$p$1: function $h_Appointment$_getNormalizedSubject$p$1() {
		return this._data$p$0.get__normalizedSubject$i$0();
	},
	_getOrganizer$p$1: function $h_Appointment$_getOrganizer$p$1() {
		return this._data$p$0.get__organizer$i$0();
	}
}
$h.Contact=function $h_Contact(data) {
	this.$$d__getContactString$p$0=Function.createDelegate(this, this._getContactString$p$0);
	this.$$d__getAddresses$p$0=Function.createDelegate(this, this._getAddresses$p$0);
	this.$$d__getUrls$p$0=Function.createDelegate(this, this._getUrls$p$0);
	this.$$d__getEmailAddresses$p$0=Function.createDelegate(this, this._getEmailAddresses$p$0);
	this.$$d__getPhoneNumbers$p$0=Function.createDelegate(this, this._getPhoneNumbers$p$0);
	this.$$d__getBusinessName$p$0=Function.createDelegate(this, this._getBusinessName$p$0);
	this.$$d__getPersonName$p$0=Function.createDelegate(this, this._getPersonName$p$0);
	this._data$p$0=data;
	$h.InitialData._defineReadOnlyProperty$i(this, 'personName', this.$$d__getPersonName$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'businessName', this.$$d__getBusinessName$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'phoneNumbers', this.$$d__getPhoneNumbers$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'emailAddresses', this.$$d__getEmailAddresses$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'urls', this.$$d__getUrls$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'addresses', this.$$d__getAddresses$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'contactString', this.$$d__getContactString$p$0);
}
$h.Contact.prototype={
	_data$p$0: null,
	_phoneNumbers$p$0: null,
	_getPersonName$p$0: function $h_Contact$_getPersonName$p$0() {
		return this._data$p$0['PersonName'];
	},
	_getBusinessName$p$0: function $h_Contact$_getBusinessName$p$0() {
		return this._data$p$0['BusinessName'];
	},
	_getAddresses$p$0: function $h_Contact$_getAddresses$p$0() {
		return $h.Entities._getExtractedStringProperty$i(this._data$p$0, 'Addresses');
	},
	_getEmailAddresses$p$0: function $h_Contact$_getEmailAddresses$p$0() {
		return $h.Entities._getExtractedStringProperty$i(this._data$p$0, 'EmailAddresses');
	},
	_getUrls$p$0: function $h_Contact$_getUrls$p$0() {
		return $h.Entities._getExtractedStringProperty$i(this._data$p$0, 'Urls');
	},
	_getPhoneNumbers$p$0: function $h_Contact$_getPhoneNumbers$p$0() {
		if (!this._phoneNumbers$p$0) {
			var $$t_1=this;
			this._phoneNumbers$p$0=$h.Entities._getExtractedObjects$i($h.PhoneNumber, this._data$p$0, 'PhoneNumbers', function(data) {
				return new $h.PhoneNumber(data);
			});
		}
		return this._phoneNumbers$p$0;
	},
	_getContactString$p$0: function $h_Contact$_getContactString$p$0() {
		return this._data$p$0['ContactString'];
	}
}
$h.CustomProperties=function $h_CustomProperties(data) {
	if ($h.ScriptHelpers.isNullOrUndefined(data)) {
		throw Error.argumentNull('data');
	}
	this._data$p$0=data;
}
$h.CustomProperties.prototype={
	_data$p$0: null,
	get: function $h_CustomProperties$get(name) {
		var value=this._data$p$0[name];
		if (typeof(value)==='string') {
			var valueString=value;
			if (valueString.length > 6 && valueString.startsWith('Date(') && valueString.endsWith(')')) {
				var ticksString=valueString.substring(5, valueString.length - 1);
				var ticks=parseInt(ticksString);
				if (!isNaN(ticks)) {
					var dateTimeValue=new Date(ticks);
					if (dateTimeValue) {
						value=dateTimeValue;
					}
				}
			}
		}
		return value;
	},
	set: function $h_CustomProperties$set(name, value) {
		if (OSF.OUtil.isDate(value)) {
			value='Date('+(value).getTime()+')';
		}
		this._data$p$0[name]=value;
	},
	remove: function $h_CustomProperties$remove(name) {
		delete this._data$p$0[name];
	},
	saveAsync: function $h_CustomProperties$saveAsync(callback, userContext) {
		var MaxCustomPropertiesLength=2500;
		if (JSON.stringify(this._data$p$0).length > MaxCustomPropertiesLength) {
			throw Error.argument();
		}
		var saveCustomProperties=new $h._saveDictionaryRequest(callback, userContext);
		saveCustomProperties._sendRequest$i$0(4, 'SaveCustomProperties', { customProperties: this._data$p$0 });
	}
}
$h.Diagnostics=function $h_Diagnostics(data, appName) {
	this.$$d__getOwaView$p$0=Function.createDelegate(this, this._getOwaView$p$0);
	this.$$d__getHostVersion$p$0=Function.createDelegate(this, this._getHostVersion$p$0);
	this.$$d__getHostName$p$0=Function.createDelegate(this, this._getHostName$p$0);
	this._data$p$0=data;
	this._appName$p$0=appName;
	$h.InitialData._defineReadOnlyProperty$i(this, 'hostName', this.$$d__getHostName$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'hostVersion', this.$$d__getHostVersion$p$0);
	if (64===this._appName$p$0) {
		$h.InitialData._defineReadOnlyProperty$i(this, 'OWAView', this.$$d__getOwaView$p$0);
	}
}
$h.Diagnostics.prototype={
	_data$p$0: null,
	_appName$p$0: 0,
	_getHostName$p$0: function $h_Diagnostics$_getHostName$p$0() {
		if (8===this._appName$p$0) {
			return 'Outlook';
		}
		else if (64===this._appName$p$0) {
			return 'OutlookWebApp';
		}
		return null;
	},
	_getHostVersion$p$0: function $h_Diagnostics$_getHostVersion$p$0() {
		return this._data$p$0.get__hostVersion$i$0();
	},
	_getOwaView$p$0: function $h_Diagnostics$_getOwaView$p$0() {
		return this._data$p$0.get__owaView$i$0();
	}
}
$h.EmailAddressDetails=function $h_EmailAddressDetails(data) {
	this.$$d__getRecipientType$p$0=Function.createDelegate(this, this._getRecipientType$p$0);
	this.$$d__getAppointmentResponse$p$0=Function.createDelegate(this, this._getAppointmentResponse$p$0);
	this.$$d__getDisplayName$p$0=Function.createDelegate(this, this._getDisplayName$p$0);
	this.$$d__getEmailAddress$p$0=Function.createDelegate(this, this._getEmailAddress$p$0);
	this._data$p$0=data;
	$h.InitialData._defineReadOnlyProperty$i(this, 'emailAddress', this.$$d__getEmailAddress$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'displayName', this.$$d__getDisplayName$p$0);
	if ($h.ScriptHelpers.dictionaryContainsKey(data, 'appointmentResponse')) {
		$h.InitialData._defineReadOnlyProperty$i(this, 'appointmentResponse', this.$$d__getAppointmentResponse$p$0);
	}
	if ($h.ScriptHelpers.dictionaryContainsKey(data, 'recipientType')) {
		$h.InitialData._defineReadOnlyProperty$i(this, 'recipientType', this.$$d__getRecipientType$p$0);
	}
}
$h.EmailAddressDetails._createFromEmailUserDictionary$i=function $h_EmailAddressDetails$_createFromEmailUserDictionary$i(data) {
	var emailAddressDetailsDictionary={};
	var displayName=data['Name'];
	var emailAddress=data['UserId'];
	emailAddressDetailsDictionary['name']=displayName || $h.EmailAddressDetails._emptyString$p;
	emailAddressDetailsDictionary['address']=emailAddress || $h.EmailAddressDetails._emptyString$p;
	return new $h.EmailAddressDetails(emailAddressDetailsDictionary);
}
$h.EmailAddressDetails.prototype={
	_data$p$0: null,
	_getEmailAddress$p$0: function $h_EmailAddressDetails$_getEmailAddress$p$0() {
		return this._data$p$0['address'];
	},
	_getDisplayName$p$0: function $h_EmailAddressDetails$_getDisplayName$p$0() {
		return this._data$p$0['name'];
	},
	_getAppointmentResponse$p$0: function $h_EmailAddressDetails$_getAppointmentResponse$p$0() {
		var response=this._data$p$0['appointmentResponse'];
		return (response < $h.EmailAddressDetails._responseTypeMap$p.length) ? $h.EmailAddressDetails._responseTypeMap$p[response] : Microsoft.Office.WebExtension.MailboxEnums.ResponseType.None;
	},
	_getRecipientType$p$0: function $h_EmailAddressDetails$_getRecipientType$p$0() {
		var response=this._data$p$0['recipientType'];
		return (response < $h.EmailAddressDetails._recipientTypeMap$p.length) ? $h.EmailAddressDetails._recipientTypeMap$p[response] : Microsoft.Office.WebExtension.MailboxEnums.RecipientType.Other;
	}
}
$h.Entities=function $h_Entities(data, filteredEntitiesData, timeSent, permissionLevel) {
	this.$$d__createMeetingSuggestion$p$0=Function.createDelegate(this, this._createMeetingSuggestion$p$0);
	this.$$d__getContacts$p$0=Function.createDelegate(this, this._getContacts$p$0);
	this.$$d__getPhoneNumbers$p$0=Function.createDelegate(this, this._getPhoneNumbers$p$0);
	this.$$d__getUrls$p$0=Function.createDelegate(this, this._getUrls$p$0);
	this.$$d__getEmailAddresses$p$0=Function.createDelegate(this, this._getEmailAddresses$p$0);
	this.$$d__getMeetingSuggestions$p$0=Function.createDelegate(this, this._getMeetingSuggestions$p$0);
	this.$$d__getTaskSuggestions$p$0=Function.createDelegate(this, this._getTaskSuggestions$p$0);
	this.$$d__getAddresses$p$0=Function.createDelegate(this, this._getAddresses$p$0);
	this._data$p$0=data || {};
	this._filteredData$p$0=filteredEntitiesData || {};
	this._dateTimeSent$p$0=timeSent;
	$h.InitialData._defineReadOnlyProperty$i(this, 'addresses', this.$$d__getAddresses$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'taskSuggestions', this.$$d__getTaskSuggestions$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'meetingSuggestions', this.$$d__getMeetingSuggestions$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'emailAddresses', this.$$d__getEmailAddresses$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'urls', this.$$d__getUrls$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'phoneNumbers', this.$$d__getPhoneNumbers$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'contacts', this.$$d__getContacts$p$0);
	this._permissionLevel$p$0=permissionLevel;
}
$h.Entities._getExtractedObjects$i=function $h_Entities$_getExtractedObjects$i(T, data, name, creator, removeDuplicates, stringPropertyName) {
	var results=null;
	var extractedObjects=data[name];
	if (!extractedObjects) {
		return new Array(0);
	}
	if (removeDuplicates) {
		extractedObjects=$h.Entities._removeDuplicate$p(Object, extractedObjects, $h.Entities._entityDictionaryEquals$p, stringPropertyName);
	}
	results=new Array(extractedObjects.length);
	var count=0;
	for (var $$arr_9=extractedObjects, $$len_A=$$arr_9.length, $$idx_B=0; $$idx_B < $$len_A;++$$idx_B) {
		var extractedObject=$$arr_9[$$idx_B];
		results[count++]=creator(extractedObject);
	}
	return results;
}
$h.Entities._getExtractedStringProperty$i=function $h_Entities$_getExtractedStringProperty$i(data, name, removeDuplicate) {
	var extractedProperties=data[name];
	if (!extractedProperties) {
		return new Array(0);
	}
	if (removeDuplicate) {
		extractedProperties=$h.Entities._removeDuplicate$p(String, extractedProperties, $h.Entities._stringEquals$p, null);
	}
	return extractedProperties;
}
$h.Entities._createContact$p=function $h_Entities$_createContact$p(data) {
	return new $h.Contact(data);
}
$h.Entities._createTaskSuggestion$p=function $h_Entities$_createTaskSuggestion$p(data) {
	return new $h.TaskSuggestion(data);
}
$h.Entities._createPhoneNumber$p=function $h_Entities$_createPhoneNumber$p(data) {
	return new $h.PhoneNumber(data);
}
$h.Entities._entityDictionaryEquals$p=function $h_Entities$_entityDictionaryEquals$p(dictionary1, dictionary2, entityPropertyIdentifier) {
	if (dictionary1===dictionary2) {
		return true;
	}
	if (!dictionary1 || !dictionary2) {
		return false;
	}
	if (dictionary1[entityPropertyIdentifier]===dictionary2[entityPropertyIdentifier]) {
		return true;
	}
	return false;
}
$h.Entities._stringEquals$p=function $h_Entities$_stringEquals$p(string1, string2, entityProperty) {
	return string1===string2;
}
$h.Entities._removeDuplicate$p=function $h_Entities$_removeDuplicate$p(T, array, entityEquals, entityPropertyIdentifier) {
	for (var matchIndex1=array.length - 1; matchIndex1 >=0; matchIndex1--) {
		var removeMatch=false;
		for (var matchIndex2=matchIndex1 - 1; matchIndex2 >=0; matchIndex2--) {
			if (entityEquals(array[matchIndex1], array[matchIndex2], entityPropertyIdentifier)) {
				removeMatch=true;
				break;
			}
		}
		if (removeMatch) {
			Array.removeAt(array, matchIndex1);
		}
	}
	return array;
}
$h.Entities.prototype={
	_dateTimeSent$p$0: null,
	_data$p$0: null,
	_filteredData$p$0: null,
	_filteredEntitiesCache$p$0: null,
	_permissionLevel$p$0: 0,
	_taskSuggestions$p$0: null,
	_meetingSuggestions$p$0: null,
	_phoneNumbers$p$0: null,
	_contacts$p$0: null,
	_addresses$p$0: null,
	_emailAddresses$p$0: null,
	_urls$p$0: null,
	_getByType$i$0: function $h_Entities$_getByType$i$0(entityType) {
		if (entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.MeetingSuggestion) {
			return this._getMeetingSuggestions$p$0();
		}
		else if (entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.TaskSuggestion) {
			return this._getTaskSuggestions$p$0();
		}
		else if (entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.Address) {
			return this._getAddresses$p$0();
		}
		else if (entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.PhoneNumber) {
			return this._getPhoneNumbers$p$0();
		}
		else if (entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.EmailAddress) {
			return this._getEmailAddresses$p$0();
		}
		else if (entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.Url) {
			return this._getUrls$p$0();
		}
		else if (entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.Contact) {
			return this._getContacts$p$0();
		}
		return null;
	},
	_getFilteredEntitiesByName$i$0: function $h_Entities$_getFilteredEntitiesByName$i$0(name) {
		if (!this._filteredEntitiesCache$p$0) {
			this._filteredEntitiesCache$p$0={};
		}
		if (!$h.ScriptHelpers.dictionaryContainsKey(this._filteredEntitiesCache$p$0, name)) {
			var found=false;
			for (var i=0; i < $h.Entities._allEntityKeys$p.length; i++) {
				var entityTypeKey=$h.Entities._allEntityKeys$p[i];
				var perEntityTypeDictionary=this._filteredData$p$0[entityTypeKey];
				if (!perEntityTypeDictionary) {
					continue;
				}
				if ($h.ScriptHelpers.dictionaryContainsKey(perEntityTypeDictionary, name)) {
					switch (entityTypeKey) {
						case 'EmailAddresses':
						case 'Urls':
							this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedStringProperty$i(perEntityTypeDictionary, name);
							break;
						case 'Addresses':
							this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedStringProperty$i(perEntityTypeDictionary, name, true);
							break;
						case 'PhoneNumbers':
							this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.PhoneNumber, perEntityTypeDictionary, name, $h.Entities._createPhoneNumber$p, false, null);
							break;
						case 'TaskSuggestions':
							this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.TaskSuggestion, perEntityTypeDictionary, name, $h.Entities._createTaskSuggestion$p, true, 'TaskString');
							break;
						case 'MeetingSuggestions':
							this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.MeetingSuggestion, perEntityTypeDictionary, name, this.$$d__createMeetingSuggestion$p$0, true, 'MeetingString');
							break;
						case 'Contacts':
							this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.Contact, perEntityTypeDictionary, name, $h.Entities._createContact$p, true, 'ContactString');
							break;
					}
					found=true;
					break;
				}
			}
			if (!found) {
				this._filteredEntitiesCache$p$0[name]=null;
			}
		}
		return this._filteredEntitiesCache$p$0[name];
	},
	_createMeetingSuggestion$p$0: function $h_Entities$_createMeetingSuggestion$p$0(data) {
		return new $h.MeetingSuggestion(data, this._dateTimeSent$p$0);
	},
	_getAddresses$p$0: function $h_Entities$_getAddresses$p$0() {
		if (!this._addresses$p$0) {
			this._addresses$p$0=$h.Entities._getExtractedStringProperty$i(this._data$p$0, 'Addresses', true);
		}
		return this._addresses$p$0;
	},
	_getEmailAddresses$p$0: function $h_Entities$_getEmailAddresses$p$0() {
		OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
		if (!this._emailAddresses$p$0) {
			this._emailAddresses$p$0=$h.Entities._getExtractedStringProperty$i(this._data$p$0, 'EmailAddresses', false);
		}
		return this._emailAddresses$p$0;
	},
	_getUrls$p$0: function $h_Entities$_getUrls$p$0() {
		if (!this._urls$p$0) {
			this._urls$p$0=$h.Entities._getExtractedStringProperty$i(this._data$p$0, 'Urls', false);
		}
		return this._urls$p$0;
	},
	_getPhoneNumbers$p$0: function $h_Entities$_getPhoneNumbers$p$0() {
		if (!this._phoneNumbers$p$0) {
			this._phoneNumbers$p$0=$h.Entities._getExtractedObjects$i($h.PhoneNumber, this._data$p$0, 'PhoneNumbers', $h.Entities._createPhoneNumber$p);
		}
		return this._phoneNumbers$p$0;
	},
	_getTaskSuggestions$p$0: function $h_Entities$_getTaskSuggestions$p$0() {
		OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
		if (!this._taskSuggestions$p$0) {
			this._taskSuggestions$p$0=$h.Entities._getExtractedObjects$i($h.TaskSuggestion, this._data$p$0, 'TaskSuggestions', $h.Entities._createTaskSuggestion$p, true, 'TaskString');
		}
		return this._taskSuggestions$p$0;
	},
	_getMeetingSuggestions$p$0: function $h_Entities$_getMeetingSuggestions$p$0() {
		OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
		if (!this._meetingSuggestions$p$0) {
			this._meetingSuggestions$p$0=$h.Entities._getExtractedObjects$i($h.MeetingSuggestion, this._data$p$0, 'MeetingSuggestions', this.$$d__createMeetingSuggestion$p$0, true, 'MeetingString');
		}
		return this._meetingSuggestions$p$0;
	},
	_getContacts$p$0: function $h_Entities$_getContacts$p$0() {
		OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
		if (!this._contacts$p$0) {
			this._contacts$p$0=$h.Entities._getExtractedObjects$i($h.Contact, this._data$p$0, 'Contacts', $h.Entities._createContact$p, true, 'ContactString');
		}
		return this._contacts$p$0;
	}
}
$h.Item=function $h_Item(data) {
	this.$$d__createCustomProperties$i$0=Function.createDelegate(this, this._createCustomProperties$i$0);
	this.$$d__getItemClass$p$0=Function.createDelegate(this, this._getItemClass$p$0);
	this.$$d__getItemId$p$0=Function.createDelegate(this, this._getItemId$p$0);
	this.$$d__getDateTimeModified$p$0=Function.createDelegate(this, this._getDateTimeModified$p$0);
	this.$$d__getDateTimeCreated$p$0=Function.createDelegate(this, this._getDateTimeCreated$p$0);
	this._data$p$0=data;
	$h.InitialData._defineReadOnlyProperty$i(this, 'dateTimeCreated', this.$$d__getDateTimeCreated$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'dateTimeModified', this.$$d__getDateTimeModified$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'itemId', this.$$d__getItemId$p$0);
	var $$t_1=this;
	$h.InitialData._defineReadOnlyProperty$i(this, 'itemType', function() {
		return $$t_1.getItemType();
	});
	$h.InitialData._defineReadOnlyProperty$i(this, 'itemClass', this.$$d__getItemClass$p$0);
}
$h.Item.prototype={
	_data$p$0: null,
	loadCustomPropertiesAsync: function $h_Item$loadCustomPropertiesAsync(callback, userContext) {
		if ($h.ScriptHelpers.isNullOrUndefined(callback)) {
			throw Error.argumentNull('callback');
		}
		var loadCustomProperties=new $h._loadDictionaryRequest(this.$$d__createCustomProperties$i$0, 'customProperties', callback, userContext);
		loadCustomProperties._sendRequest$i$0(3, 'LoadCustomProperties', {});
	},
	_createCustomProperties$i$0: function $h_Item$_createCustomProperties$i$0(data) {
		return new $h.CustomProperties(data);
	},
	_getItemId$p$0: function $h_Item$_getItemId$p$0() {
		return this._data$p$0.get__itemId$i$0();
	},
	_getItemClass$p$0: function $h_Item$_getItemClass$p$0() {
		return this._data$p$0.get__itemClass$i$0();
	},
	_getDateTimeCreated$p$0: function $h_Item$_getDateTimeCreated$p$0() {
		return this._data$p$0.get__dateTimeCreated$i$0();
	},
	_getDateTimeModified$p$0: function $h_Item$_getDateTimeModified$p$0() {
		return this._data$p$0.get__dateTimeModified$i$0();
	}
}
$h.MeetingRequest=function $h_MeetingRequest(data) {
	this.$$d__getRequiredAttendees$p$2=Function.createDelegate(this, this._getRequiredAttendees$p$2);
	this.$$d__getOptionalAttendees$p$2=Function.createDelegate(this, this._getOptionalAttendees$p$2);
	this.$$d__getLocation$p$2=Function.createDelegate(this, this._getLocation$p$2);
	this.$$d__getEnd$p$2=Function.createDelegate(this, this._getEnd$p$2);
	this.$$d__getStart$p$2=Function.createDelegate(this, this._getStart$p$2);
	$h.MeetingRequest.initializeBase(this, [ data ]);
	$h.InitialData._defineReadOnlyProperty$i(this, 'start', this.$$d__getStart$p$2);
	$h.InitialData._defineReadOnlyProperty$i(this, 'end', this.$$d__getEnd$p$2);
	$h.InitialData._defineReadOnlyProperty$i(this, 'location', this.$$d__getLocation$p$2);
	$h.InitialData._defineReadOnlyProperty$i(this, 'optionalAttendees', this.$$d__getOptionalAttendees$p$2);
	$h.InitialData._defineReadOnlyProperty$i(this, 'requiredAttendees', this.$$d__getRequiredAttendees$p$2);
}
$h.MeetingRequest.prototype={
	_getStart$p$2: function $h_MeetingRequest$_getStart$p$2() {
		return this._data$p$0.get__start$i$0();
	},
	_getEnd$p$2: function $h_MeetingRequest$_getEnd$p$2() {
		return this._data$p$0.get__end$i$0();
	},
	_getLocation$p$2: function $h_MeetingRequest$_getLocation$p$2() {
		return this._data$p$0.get__location$i$0();
	},
	_getOptionalAttendees$p$2: function $h_MeetingRequest$_getOptionalAttendees$p$2() {
		return this._data$p$0.get__cc$i$0();
	},
	_getRequiredAttendees$p$2: function $h_MeetingRequest$_getRequiredAttendees$p$2() {
		return this._data$p$0.get__to$i$0();
	}
}
$h.MeetingSuggestion=function $h_MeetingSuggestion(data, dateTimeSent) {
	this.$$d__getEndTime$p$0=Function.createDelegate(this, this._getEndTime$p$0);
	this.$$d__getStartTime$p$0=Function.createDelegate(this, this._getStartTime$p$0);
	this.$$d__getSubject$p$0=Function.createDelegate(this, this._getSubject$p$0);
	this.$$d__getLocation$p$0=Function.createDelegate(this, this._getLocation$p$0);
	this.$$d__getAttendees$p$0=Function.createDelegate(this, this._getAttendees$p$0);
	this.$$d__getMeetingString$p$0=Function.createDelegate(this, this._getMeetingString$p$0);
	this._data$p$0=data;
	this._dateTimeSent$p$0=dateTimeSent;
	$h.InitialData._defineReadOnlyProperty$i(this, 'meetingString', this.$$d__getMeetingString$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'attendees', this.$$d__getAttendees$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'location', this.$$d__getLocation$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'subject', this.$$d__getSubject$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'start', this.$$d__getStartTime$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'end', this.$$d__getEndTime$p$0);
}
$h.MeetingSuggestion.prototype={
	_dateTimeSent$p$0: null,
	_data$p$0: null,
	_attendees$p$0: null,
	_getMeetingString$p$0: function $h_MeetingSuggestion$_getMeetingString$p$0() {
		return this._data$p$0['MeetingString'];
	},
	_getLocation$p$0: function $h_MeetingSuggestion$_getLocation$p$0() {
		return this._data$p$0['Location'];
	},
	_getSubject$p$0: function $h_MeetingSuggestion$_getSubject$p$0() {
		return this._data$p$0['Subject'];
	},
	_getStartTime$p$0: function $h_MeetingSuggestion$_getStartTime$p$0() {
		var time=this._createDateTimeFromParameter$p$0('StartTime');
		var resolvedTime=$h.MeetingSuggestionTimeDecoder.resolve(time, this._dateTimeSent$p$0);
		if (resolvedTime.getTime() !==time.getTime()) {
			return OSF.DDA.OutlookAppOm._instance$p.convertToUtcClientTime(OSF.DDA.OutlookAppOm._instance$p._dateToDictionary$i$0(resolvedTime));
		}
		return time;
	},
	_getEndTime$p$0: function $h_MeetingSuggestion$_getEndTime$p$0() {
		var time=this._createDateTimeFromParameter$p$0('EndTime');
		var resolvedTime=$h.MeetingSuggestionTimeDecoder.resolve(time, this._dateTimeSent$p$0);
		if (resolvedTime.getTime() !==time.getTime()) {
			return OSF.DDA.OutlookAppOm._instance$p.convertToUtcClientTime(OSF.DDA.OutlookAppOm._instance$p._dateToDictionary$i$0(resolvedTime));
		}
		return time;
	},
	_createDateTimeFromParameter$p$0: function $h_MeetingSuggestion$_createDateTimeFromParameter$p$0(keyName) {
		var dateTimeString=this._data$p$0[keyName];
		if (!dateTimeString) {
			return null;
		}
		return new Date(dateTimeString);
	},
	_getAttendees$p$0: function $h_MeetingSuggestion$_getAttendees$p$0() {
		if (!this._attendees$p$0) {
			var $$t_1=this;
			this._attendees$p$0=$h.Entities._getExtractedObjects$i($h.EmailAddressDetails, this._data$p$0, 'Attendees', function(data) {
				return $h.EmailAddressDetails._createFromEmailUserDictionary$i(data);
			});
		}
		return this._attendees$p$0;
	}
}
$h.MeetingSuggestionTimeDecoder=function $h_MeetingSuggestionTimeDecoder() {
}
$h.MeetingSuggestionTimeDecoder.resolve=function $h_MeetingSuggestionTimeDecoder$resolve(inTime, sentTime) {
	if (!sentTime) {
		return inTime;
	}
	try {
		var tod;
		var outDate;
		var extractedDate;
		var sentDate=new Date(sentTime.getFullYear(), sentTime.getMonth(), sentTime.getDate(), 0, 0, 0, 0);
		var $$t_7, $$t_8, $$t_9;
		if (!(($$t_9=$h.MeetingSuggestionTimeDecoder._decode$p(inTime, ($$t_7={'val': extractedDate}), ($$t_8={'val': tod}))), extractedDate=$$t_7.val, tod=$$t_8.val, $$t_9)) {
			return inTime;
		}
		else {
			if ($h._preciseDate.isInstanceOfType(extractedDate)) {
				outDate=$h.MeetingSuggestionTimeDecoder._resolvePreciseDate$p(sentDate, extractedDate);
			}
			else {
				if ($h._relativeDate.isInstanceOfType(extractedDate)) {
					outDate=$h.MeetingSuggestionTimeDecoder._resolveRelativeDate$p(sentDate, extractedDate);
				}
				else {
					outDate=sentDate;
				}
			}
			if (isNaN(outDate.getTime())) {
				return sentTime;
			}
			outDate.setMilliseconds(outDate.getMilliseconds()+tod);
			return outDate;
		}
	}
	catch ($$e_6) {
		return sentTime;
	}
}
$h.MeetingSuggestionTimeDecoder._isNullOrUndefined$i=function $h_MeetingSuggestionTimeDecoder$_isNullOrUndefined$i(value) {
	return null===value || value===undefined;
}
$h.MeetingSuggestionTimeDecoder._resolvePreciseDate$p=function $h_MeetingSuggestionTimeDecoder$_resolvePreciseDate$p(sentDate, precise) {
	var year=precise._year$i$1;
	var month=(!precise._month$i$1) ? sentDate.getMonth() : precise._month$i$1 - 1;
	var day=precise._day$i$1;
	if (!day) {
		return sentDate;
	}
	var candidate;
	if ($h.MeetingSuggestionTimeDecoder._isNullOrUndefined$i(year)) {
		candidate=new Date(sentDate.getFullYear(), month, day);
		if (candidate.getTime() < sentDate.getTime()) {
			candidate=new Date(sentDate.getFullYear()+1, month, day);
		}
	}
	else {
		candidate=new Date((year < 50) ? 2000+year : 1900+year, month, day);
	}
	if (candidate.getMonth() !==month) {
		return sentDate;
	}
	return candidate;
}
$h.MeetingSuggestionTimeDecoder._resolveRelativeDate$p=function $h_MeetingSuggestionTimeDecoder$_resolveRelativeDate$p(sentDate, relative) {
	var date;
	switch (relative._unit$i$1) {
		case 0:
			date=new Date(sentDate.getFullYear(), sentDate.getMonth(), sentDate.getDate());
			date.setDate(date.getDate()+relative._offset$i$1);
			return date;
		case 5:
			return $h.MeetingSuggestionTimeDecoder._findBestDateForWeekDate$p(sentDate, relative._offset$i$1, relative._tag$i$1);
		case 2:
			var days=1;
			switch (relative._modifier$i$1) {
				case 1:
					break;
				case 2:
					days=16;
					break;
				default:
					if (!relative._offset$i$1) {
						days=sentDate.getDate();
					}
					break;
			}
			date=new Date(sentDate.getFullYear(), sentDate.getMonth(), days);
			date.setMonth(date.getMonth()+relative._offset$i$1);
			if (date.getTime() < sentDate.getTime()) {
				date.setDate(date.getDate()+sentDate.getDate() - 1);
			}
			return date;
		case 1:
			date=new Date(sentDate.getFullYear(), sentDate.getMonth(), sentDate.getDate());
			date.setDate(sentDate.getDate()+(7 * relative._offset$i$1));
			if (relative._modifier$i$1===1 || !relative._modifier$i$1) {
				date.setDate(date.getDate()+1 - date.getDay());
				if (date.getTime() < sentDate.getTime()) {
					return sentDate;
				}
				return date;
			}
			else if (relative._modifier$i$1===2) {
				date.setDate(date.getDate()+5 - date.getDay());
				return date;
			}
			break;
		case 4:
			return $h.MeetingSuggestionTimeDecoder._findBestDateForWeekOfMonthDate$p(sentDate, relative);
		case 3:
			if (relative._offset$i$1 > 0) {
				return new Date(sentDate.getFullYear()+relative._offset$i$1, 0, 1);
			}
			break;
		default:
			break;
	}
	return sentDate;
}
$h.MeetingSuggestionTimeDecoder._findBestDateForWeekDate$p=function $h_MeetingSuggestionTimeDecoder$_findBestDateForWeekDate$p(sentDate, offset, tag) {
	if (offset > -5 && offset < 5) {
		var dayOfWeek;
		var days;
		dayOfWeek=((tag+6) % 7)+1;
		days=(7 * offset)+(dayOfWeek - sentDate.getDay());
		sentDate.setDate(sentDate.getDate()+days);
		return sentDate;
	}
	else {
		var days=(tag - sentDate.getDay()) % 7;
		if (days < 0) {
			days+=7;
		}
		sentDate.setDate(sentDate.getDate()+days);
		return sentDate;
	}
}
$h.MeetingSuggestionTimeDecoder._findBestDateForWeekOfMonthDate$p=function $h_MeetingSuggestionTimeDecoder$_findBestDateForWeekOfMonthDate$p(sentDate, relative) {
	var date;
	var firstDay;
	var newDate;
	date=sentDate;
	if (relative._tag$i$1 <=0 || relative._tag$i$1 > 12 || relative._offset$i$1 <=0 || relative._offset$i$1 > 5) {
		return sentDate;
	}
	var monthOffset=(12+relative._tag$i$1 - date.getMonth() - 1) % 12;
	firstDay=new Date(date.getFullYear(), date.getMonth()+monthOffset, 1);
	if (relative._modifier$i$1===1) {
		if (relative._offset$i$1===1 && firstDay.getDay() !==6 && firstDay.getDay() !==0) {
			return firstDay;
		}
		else {
			newDate=new Date(firstDay.getFullYear(), firstDay.getMonth(), firstDay.getDate());
			newDate.setDate(newDate.getDate()+((7+(1 - firstDay.getDay())) % 7));
			if (firstDay.getDay() !==6 && firstDay.getDay() !==0 && firstDay.getDay() !==1) {
				newDate.setDate(newDate.getDate() - 7);
			}
			newDate.setDate(newDate.getDate()+(7 * (relative._offset$i$1 - 1)));
			if (newDate.getMonth()+1 !==relative._tag$i$1) {
				return sentDate;
			}
			return newDate;
		}
	}
	else {
		newDate=new Date(firstDay.getFullYear(), firstDay.getMonth(), $h.MeetingSuggestionTimeDecoder._daysInMonth$p(firstDay.getMonth(), firstDay.getFullYear()));
		var offset=1 - newDate.getDay();
		if (offset > 0) {
			offset=offset - 7;
		}
		newDate.setDate(newDate.getDate()+offset);
		newDate.setDate(newDate.getDate()+(7 * (1 - relative._offset$i$1)));
		if (newDate.getMonth()+1 !==relative._tag$i$1) {
			if (firstDay.getDay() !==6 && firstDay.getDay() !==0) {
				return firstDay;
			}
			else {
				return sentDate;
			}
		}
		else {
			return newDate;
		}
	}
}
$h.MeetingSuggestionTimeDecoder._decode$p=function $h_MeetingSuggestionTimeDecoder$_decode$p(inDate, date, time) {
	var DateValueMask=32767;
	date.val=null;
	time.val=0;
	if (!inDate) {
		return false;
	}
	time.val=$h.MeetingSuggestionTimeDecoder._getTimeOfDayInMillisecondsUTC$p(inDate);
	var inDateAtMidnight=inDate.getTime() - time.val;
	var value=(inDateAtMidnight - $h.MeetingSuggestionTimeDecoder._baseDate$p.getTime()) / 86400000;
	if (value < 0) {
		return false;
	}
	else if (value >=262144) {
		return false;
	}
	else {
		var type=value >> 15;
		value=value & DateValueMask;
		switch (type) {
			case 0:
				return $h.MeetingSuggestionTimeDecoder._decodePreciseDate$p(value, date);
			case 1:
				return $h.MeetingSuggestionTimeDecoder._decodeRelativeDate$p(value, date);
			default:
				return false;
		}
	}
}
$h.MeetingSuggestionTimeDecoder._decodePreciseDate$p=function $h_MeetingSuggestionTimeDecoder$_decodePreciseDate$p(value, date) {
	var c_SubTypeMask=7;
	var c_MonthMask=15;
	var c_DayMask=31;
	var c_YearMask=127;
	var year=null;
	var month=0;
	var day=0;
	date.val=null;
	var subType=(value >> 12) & c_SubTypeMask;
	if ((subType & 4)===4) {
		year=(value >> 5) & c_YearMask;
		if ((subType & 2)===2) {
			if ((subType & 1)===1) {
				return false;
			}
			month=(value >> 1) & c_MonthMask;
		}
	}
	else {
		if ((subType & 2)===2) {
			month=(value >> 8) & c_MonthMask;
		}
		if ((subType & 1)===1) {
			day=(value >> 3) & c_DayMask;
		}
	}
	date.val=new $h._preciseDate(day, month, year);
	return true;
}
$h.MeetingSuggestionTimeDecoder._decodeRelativeDate$p=function $h_MeetingSuggestionTimeDecoder$_decodeRelativeDate$p(value, date) {
	var TagMask=15;
	var OffsetMask=63;
	var UnitMask=7;
	var ModifierMask=3;
	var tag=value & TagMask;
	value >>=4;
	var offset=$h.MeetingSuggestionTimeDecoder._fromComplement$p(value & OffsetMask, 6);
	value >>=6;
	var unit=value & UnitMask;
	value >>=3;
	var modifier=value & ModifierMask;
	try {
		date.val=new $h._relativeDate(modifier, offset, unit, tag);
		return true;
	}
	catch ($$e_A) {
		date.val=null;
		return false;
	}
}
$h.MeetingSuggestionTimeDecoder._fromComplement$p=function $h_MeetingSuggestionTimeDecoder$_fromComplement$p(value, n) {
	var signed=1 << (n - 1);
	var mask=(1 << n) - 1;
	if ((value & signed)===signed) {
		return -((value ^ mask)+1);
	}
	else {
		return value;
	}
}
$h.MeetingSuggestionTimeDecoder._daysInMonth$p=function $h_MeetingSuggestionTimeDecoder$_daysInMonth$p(month, year) {
	return 32 - new Date(year, month, 32).getDate();
}
$h.MeetingSuggestionTimeDecoder._getTimeOfDayInMillisecondsUTC$p=function $h_MeetingSuggestionTimeDecoder$_getTimeOfDayInMillisecondsUTC$p(inputTime) {
	var timeOfDay=0;
	timeOfDay+=inputTime.getUTCHours() * 3600;
	timeOfDay+=inputTime.getUTCMinutes() * 60;
	timeOfDay+=inputTime.getUTCSeconds();
	timeOfDay *=1000;
	timeOfDay+=inputTime.getUTCMilliseconds();
	return timeOfDay;
}
$h._extractedDate=function $h__extractedDate() {
}
$h._preciseDate=function $h__preciseDate(day, month, year) {
	$h._preciseDate.initializeBase(this);
	if (day < 0 || day > 31) {
		throw Error.argumentOutOfRange('day');
	}
	if (month < 0 || month > 12) {
		throw Error.argumentOutOfRange('month');
	}
	this._day$i$1=day;
	this._month$i$1=month;
	if (!$h.MeetingSuggestionTimeDecoder._isNullOrUndefined$i(year)) {
		if (!month && day) {
			throw Error.argument('Invalid arguments');
		}
		if (year < 0 || year > 2099) {
			throw Error.argumentOutOfRange('year');
		}
		this._year$i$1=year % 100;
	}
	else if (!this._month$i$1 && !this._day$i$1) {
		throw Error.argument('Invalid datetime');
	}
}
$h._preciseDate.prototype={
	_day$i$1: 0,
	_month$i$1: 0,
	_year$i$1: null
}
$h._relativeDate=function $h__relativeDate(modifier, offset, unit, tag) {
	$h._relativeDate.initializeBase(this);
	if (offset < -32 || offset > 31) {
		throw Error.argumentOutOfRange('offset');
	}
	if (tag < 0 || tag > 15) {
		throw Error.argumentOutOfRange('tag');
	}
	if (!unit && offset < 0) {
		throw Error.argument('unit & offset do not form a valid date');
	}
	this._modifier$i$1=modifier;
	this._offset$i$1=offset;
	this._unit$i$1=unit;
	this._tag$i$1=tag;
}
$h._relativeDate.prototype={
	_modifier$i$1: 0,
	_offset$i$1: 0,
	_unit$i$1: 0,
	_tag$i$1: 0
}
$h.Message=function $h_Message(dataDictionary) {
	this.$$d__getConversationId$p$1=Function.createDelegate(this, this._getConversationId$p$1);
	this.$$d__getInternetMessageId$p$1=Function.createDelegate(this, this._getInternetMessageId$p$1);
	this.$$d__getCc$p$1=Function.createDelegate(this, this._getCc$p$1);
	this.$$d__getTo$p$1=Function.createDelegate(this, this._getTo$p$1);
	this.$$d__getFrom$p$1=Function.createDelegate(this, this._getFrom$p$1);
	this.$$d__getSender$p$1=Function.createDelegate(this, this._getSender$p$1);
	this.$$d__getNormalizedSubject$p$1=Function.createDelegate(this, this._getNormalizedSubject$p$1);
	this.$$d__getSubject$p$1=Function.createDelegate(this, this._getSubject$p$1);
	$h.Message.initializeBase(this, [ dataDictionary ]);
	$h.InitialData._defineReadOnlyProperty$i(this, 'subject', this.$$d__getSubject$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'normalizedSubject', this.$$d__getNormalizedSubject$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'sender', this.$$d__getSender$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'from', this.$$d__getFrom$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'to', this.$$d__getTo$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'cc', this.$$d__getCc$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'internetMessageId', this.$$d__getInternetMessageId$p$1);
	$h.InitialData._defineReadOnlyProperty$i(this, 'conversationId', this.$$d__getConversationId$p$1);
}
$h.Message.prototype={
	getEntities: function $h_Message$getEntities() {
		return this._data$p$0._getEntities$i$0();
	},
	getEntitiesByType: function $h_Message$getEntitiesByType(entityType) {
		return this._data$p$0._getEntitiesByType$i$0(entityType);
	},
	getFilteredEntitiesByName: function $h_Message$getFilteredEntitiesByName(name) {
		return this._data$p$0._getFilteredEntitiesByName$i$0(name);
	},
	getRegExMatches: function $h_Message$getRegExMatches() {
		OSF.DDA.OutlookAppOm._throwOnMethodCallForInsufficientPermission$i(this._data$p$0.get__permissionLevel$i$0(), 1, 'getRegExMatches');
		return this._data$p$0._getRegExMatches$i$0();
	},
	getRegExMatchesByName: function $h_Message$getRegExMatchesByName(name) {
		OSF.DDA.OutlookAppOm._throwOnMethodCallForInsufficientPermission$i(this._data$p$0.get__permissionLevel$i$0(), 1, 'getRegExMatchesByName');
		return this._data$p$0._getRegExMatchesByName$i$0(name);
	},
	displayReplyForm: function $h_Message$displayReplyForm(htmlBody) {
		OSF.DDA.OutlookAppOm._instance$p._displayReplyForm$i$0(htmlBody);
	},
	displayReplyAllForm: function $h_Message$displayReplyAllForm(htmlBody) {
		OSF.DDA.OutlookAppOm._instance$p._displayReplyAllForm$i$0(htmlBody);
	},
	getItemType: function $h_Message$getItemType() {
		return Microsoft.Office.WebExtension.MailboxEnums.ItemType.Message;
	},
	_getSubject$p$1: function $h_Message$_getSubject$p$1() {
		return this._data$p$0.get__subject$i$0();
	},
	_getNormalizedSubject$p$1: function $h_Message$_getNormalizedSubject$p$1() {
		return this._data$p$0.get__normalizedSubject$i$0();
	},
	_getSender$p$1: function $h_Message$_getSender$p$1() {
		return this._data$p$0.get__sender$i$0();
	},
	_getFrom$p$1: function $h_Message$_getFrom$p$1() {
		return this._data$p$0.get__from$i$0();
	},
	_getTo$p$1: function $h_Message$_getTo$p$1() {
		return this._data$p$0.get__to$i$0();
	},
	_getCc$p$1: function $h_Message$_getCc$p$1() {
		return this._data$p$0.get__cc$i$0();
	},
	_getInternetMessageId$p$1: function $h_Message$_getInternetMessageId$p$1() {
		return this._data$p$0.get__internetMessageId$i$0();
	},
	_getConversationId$p$1: function $h_Message$_getConversationId$p$1() {
		return this._data$p$0.get__conversationId$i$0();
	}
}
$h.PhoneNumber=function $h_PhoneNumber(data) {
	this.$$d__getPhoneType$p$0=Function.createDelegate(this, this._getPhoneType$p$0);
	this.$$d__getOriginalPhoneString$p$0=Function.createDelegate(this, this._getOriginalPhoneString$p$0);
	this.$$d__getPhoneString$p$0=Function.createDelegate(this, this._getPhoneString$p$0);
	this._data$p$0=data;
	$h.InitialData._defineReadOnlyProperty$i(this, 'phoneString', this.$$d__getPhoneString$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'originalPhoneString', this.$$d__getOriginalPhoneString$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'type', this.$$d__getPhoneType$p$0);
}
$h.PhoneNumber.prototype={
	_data$p$0: null,
	_getPhoneString$p$0: function $h_PhoneNumber$_getPhoneString$p$0() {
		return this._data$p$0['PhoneString'];
	},
	_getOriginalPhoneString$p$0: function $h_PhoneNumber$_getOriginalPhoneString$p$0() {
		return this._data$p$0['OriginalPhoneString'];
	},
	_getPhoneType$p$0: function $h_PhoneNumber$_getPhoneType$p$0() {
		return this._data$p$0['Type'];
	}
}
$h.TaskSuggestion=function $h_TaskSuggestion(data) {
	this.$$d__getAssignees$p$0=Function.createDelegate(this, this._getAssignees$p$0);
	this.$$d__getTaskString$p$0=Function.createDelegate(this, this._getTaskString$p$0);
	this._data$p$0=data;
	$h.InitialData._defineReadOnlyProperty$i(this, 'taskString', this.$$d__getTaskString$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'assignees', this.$$d__getAssignees$p$0);
}
$h.TaskSuggestion.prototype={
	_data$p$0: null,
	_assignees$p$0: null,
	_getTaskString$p$0: function $h_TaskSuggestion$_getTaskString$p$0() {
		return this._data$p$0['TaskString'];
	},
	_getAssignees$p$0: function $h_TaskSuggestion$_getAssignees$p$0() {
		if (!this._assignees$p$0) {
			var $$t_1=this;
			this._assignees$p$0=$h.Entities._getExtractedObjects$i($h.EmailAddressDetails, this._data$p$0, 'Assignees', function(data) {
				return $h.EmailAddressDetails._createFromEmailUserDictionary$i(data);
			});
		}
		return this._assignees$p$0;
	}
}
$h.UserProfile=function $h_UserProfile(data) {
	this.$$d__getTimeZone$p$0=Function.createDelegate(this, this._getTimeZone$p$0);
	this.$$d__getEmailAddress$p$0=Function.createDelegate(this, this._getEmailAddress$p$0);
	this.$$d__getDisplayName$p$0=Function.createDelegate(this, this._getDisplayName$p$0);
	this._data$p$0=data;
	$h.InitialData._defineReadOnlyProperty$i(this, 'displayName', this.$$d__getDisplayName$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'emailAddress', this.$$d__getEmailAddress$p$0);
	$h.InitialData._defineReadOnlyProperty$i(this, 'timeZone', this.$$d__getTimeZone$p$0);
}
$h.UserProfile.prototype={
	_data$p$0: null,
	_getDisplayName$p$0: function $h_UserProfile$_getDisplayName$p$0() {
		return this._data$p$0.get__userDisplayName$i$0();
	},
	_getEmailAddress$p$0: function $h_UserProfile$_getEmailAddress$p$0() {
		return this._data$p$0.get__userEmailAddress$i$0();
	},
	_getTimeZone$p$0: function $h_UserProfile$_getTimeZone$p$0() {
		return this._data$p$0.get__userTimeZone$i$0();
	}
}
$h.RequestState=function() {}
$h.RequestState.prototype={
	unsent: 0,
	opened: 1,
	headersReceived: 2,
	loading: 3,
	done: 4
}
$h.RequestState.registerEnum('$h.RequestState', false);
$h.EwsRequest=function $h_EwsRequest(userContext) {
	this.readyState=1;
	$h.EwsRequest.initializeBase(this, [ userContext ]);
}
$h.EwsRequest.prototype={
	status: 0,
	statusText: null,
	onreadystatechange: null,
	responseText: null,
	get__statusCode$i$1: function $h_EwsRequest$get__statusCode$i$1() {
		return this.status;
	},
	set__statusCode$i$1: function $h_EwsRequest$set__statusCode$i$1(value) {
		this.status=value;
		return value;
	},
	get__statusDescription$i$1: function $h_EwsRequest$get__statusDescription$i$1() {
		return this.statusText;
	},
	set__statusDescription$i$1: function $h_EwsRequest$set__statusDescription$i$1(value) {
		this.statusText=value;
		return value;
	},
	get__requestState$i$1: function $h_EwsRequest$get__requestState$i$1() {
		return this.readyState;
	},
	set__requestState$i$1: function $h_EwsRequest$set__requestState$i$1(value) {
		this.readyState=value;
		return value;
	},
	get__response$i$1: function $h_EwsRequest$get__response$i$1() {
		return this.responseText;
	},
	set__response$i$1: function $h_EwsRequest$set__response$i$1(value) {
		this.responseText=value;
		return value;
	},
	send: function $h_EwsRequest$send(data) {
		this._checkSendConditions$i$1();
		if ($h.ScriptHelpers.isNullOrUndefined(data)) {
			this._throwInvalidStateException$i$1();
		}
		this._sendRequest$i$0(5, 'EwsRequest', { body: data });
	},
	_callOnReadyStateChangeCallback$i$1: function $h_EwsRequest$_callOnReadyStateChangeCallback$i$1() {
		if (!$h.ScriptHelpers.isNullOrUndefined(this.onreadystatechange)) {
			this.onreadystatechange();
		}
	},
	_parseExtraResponseData$i$1: function $h_EwsRequest$_parseExtraResponseData$i$1(response) {
	}
}
$h.InitialData=function $h_InitialData(data) {
	this._data$p$0=data;
}
$h.InitialData._defineReadOnlyProperty$i=function $h_InitialData$_defineReadOnlyProperty$i(o, methodName, getter) {
	var propertyDescriptor={ get: getter, configurable: false };
	Object.defineProperty(o, methodName, propertyDescriptor);
}
$h.InitialData.prototype={
	_toRecipients$p$0: null,
	_ccRecipients$p$0: null,
	_resources$p$0: null,
	_entities$p$0: null,
	_data$p$0: null,
	get__permissionLevel$i$0: function $h_InitialData$get__permissionLevel$i$0() {
		var permissionLevel=this._data$p$0['permissionLevel'];
		return (!$h.ScriptHelpers.isUndefined(permissionLevel)) ? permissionLevel : 0;
	},
	get__itemId$i$0: function $h_InitialData$get__itemId$i$0() {
		return this._data$p$0['id'];
	},
	get__itemClass$i$0: function $h_InitialData$get__itemClass$i$0() {
		return this._data$p$0['itemClass'];
	},
	get__dateTimeCreated$i$0: function $h_InitialData$get__dateTimeCreated$i$0() {
		return new Date(this._data$p$0['dateTimeCreated']);
	},
	get__dateTimeModified$i$0: function $h_InitialData$get__dateTimeModified$i$0() {
		return new Date(this._data$p$0['dateTimeModified']);
	},
	get__dateTimeSent$i$0: function $h_InitialData$get__dateTimeSent$i$0() {
		return new Date(this._data$p$0['dateTimeSent']);
	},
	get__subject$i$0: function $h_InitialData$get__subject$i$0() {
		return this._data$p$0['subject'];
	},
	get__normalizedSubject$i$0: function $h_InitialData$get__normalizedSubject$i$0() {
		return this._data$p$0['normalizedSubject'];
	},
	get__internetMessageId$i$0: function $h_InitialData$get__internetMessageId$i$0() {
		return this._data$p$0['internetMessageId'];
	},
	get__conversationId$i$0: function $h_InitialData$get__conversationId$i$0() {
		return this._data$p$0['conversationId'];
	},
	get__sender$i$0: function $h_InitialData$get__sender$i$0() {
		this._throwOnRestrictedPermissionLevel$p$0();
		var sender=this._data$p$0['sender'];
		return ($h.ScriptHelpers.isNullOrUndefined(sender)) ? null : new $h.EmailAddressDetails(sender);
	},
	get__from$i$0: function $h_InitialData$get__from$i$0() {
		this._throwOnRestrictedPermissionLevel$p$0();
		var from=this._data$p$0['from'];
		return ($h.ScriptHelpers.isNullOrUndefined(from)) ? null : new $h.EmailAddressDetails(from);
	},
	get__to$i$0: function $h_InitialData$get__to$i$0() {
		this._throwOnRestrictedPermissionLevel$p$0();
		if (null===this._toRecipients$p$0) {
			this._toRecipients$p$0=this._createEmailAddressDetails$p$0('to');
		}
		return this._toRecipients$p$0;
	},
	get__cc$i$0: function $h_InitialData$get__cc$i$0() {
		this._throwOnRestrictedPermissionLevel$p$0();
		if (null===this._ccRecipients$p$0) {
			this._ccRecipients$p$0=this._createEmailAddressDetails$p$0('cc');
		}
		return this._ccRecipients$p$0;
	},
	get__start$i$0: function $h_InitialData$get__start$i$0() {
		return new Date(this._data$p$0['start']);
	},
	get__end$i$0: function $h_InitialData$get__end$i$0() {
		return new Date(this._data$p$0['end']);
	},
	get__location$i$0: function $h_InitialData$get__location$i$0() {
		return this._data$p$0['location'];
	},
	get__resources$i$0: function $h_InitialData$get__resources$i$0() {
		this._throwOnRestrictedPermissionLevel$p$0();
		if (null===this._resources$p$0) {
			this._resources$p$0=this._createEmailAddressDetails$p$0('resources');
		}
		return this._resources$p$0;
	},
	get__organizer$i$0: function $h_InitialData$get__organizer$i$0() {
		this._throwOnRestrictedPermissionLevel$p$0();
		var organizer=this._data$p$0['organizer'];
		return ($h.ScriptHelpers.isNullOrUndefined(organizer)) ? null : new $h.EmailAddressDetails(organizer);
	},
	get__userDisplayName$i$0: function $h_InitialData$get__userDisplayName$i$0() {
		return this._data$p$0['userDisplayName'];
	},
	get__userEmailAddress$i$0: function $h_InitialData$get__userEmailAddress$i$0() {
		return this._data$p$0['userEmailAddress'];
	},
	get__userTimeZone$i$0: function $h_InitialData$get__userTimeZone$i$0() {
		return this._data$p$0['userTimeZone'];
	},
	get__timeZoneOffsets$i$0: function $h_InitialData$get__timeZoneOffsets$i$0() {
		return this._data$p$0['timeZoneOffsets'];
	},
	get__hostVersion$i$0: function $h_InitialData$get__hostVersion$i$0() {
		return this._data$p$0['hostVersion'];
	},
	get__owaView$i$0: function $h_InitialData$get__owaView$i$0() {
		return this._data$p$0['owaView'];
	},
	_getEntities$i$0: function $h_InitialData$_getEntities$i$0() {
		if (!this._entities$p$0) {
			this._entities$p$0=new $h.Entities(this._data$p$0['entities'], this._data$p$0['filteredEntities'], this.get__dateTimeSent$i$0(), this.get__permissionLevel$i$0());
		}
		return this._entities$p$0;
	},
	_getEntitiesByType$i$0: function $h_InitialData$_getEntitiesByType$i$0(entityType) {
		var entites=this._getEntities$i$0();
		return entites._getByType$i$0(entityType);
	},
	_getFilteredEntitiesByName$i$0: function $h_InitialData$_getFilteredEntitiesByName$i$0(name) {
		var entities=this._getEntities$i$0();
		return entities._getFilteredEntitiesByName$i$0(name);
	},
	_getRegExMatches$i$0: function $h_InitialData$_getRegExMatches$i$0() {
		if (!this._data$p$0['regExMatches']) {
			return null;
		}
		return this._data$p$0['regExMatches'];
	},
	_getRegExMatchesByName$i$0: function $h_InitialData$_getRegExMatchesByName$i$0(regexName) {
		var regexMatches=this._getRegExMatches$i$0();
		if (!regexMatches || !regexMatches[regexName]) {
			return null;
		}
		return regexMatches[regexName];
	},
	_createEmailAddressDetails$p$0: function $h_InitialData$_createEmailAddressDetails$p$0(key) {
		var to=this._data$p$0[key];
		if ($h.ScriptHelpers.isNullOrUndefined(to)) {
			return [];
		}
		var recipients=[];
		for (var i=0; i < to.length; i++) {
			if (!$h.ScriptHelpers.isNullOrUndefined(to[i])) {
				recipients[i]=new $h.EmailAddressDetails(to[i]);
			}
		}
		return recipients;
	},
	_throwOnRestrictedPermissionLevel$p$0: function $h_InitialData$_throwOnRestrictedPermissionLevel$p$0() {
		OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this.get__permissionLevel$i$0());
	}
}
$h._loadDictionaryRequest=function $h__loadDictionaryRequest(createResultObject, dictionaryName, callback, userContext) {
	$h._loadDictionaryRequest.initializeBase(this, [ userContext ]);
	this._createResultObject$p$1=createResultObject;
	this._dictionaryName$p$1=dictionaryName;
	this._callback$p$1=callback;
}
$h._loadDictionaryRequest.prototype={
	_dictionaryName$p$1: null,
	_createResultObject$p$1: null,
	_callback$p$1: null,
	handleResponse: function $h__loadDictionaryRequest$handleResponse(response) {
		if (response['wasSuccessful']) {
			var value=response[this._dictionaryName$p$1];
			var responseData=JSON.parse(value);
			this.createAsyncResult(this._createResultObject$p$1(responseData), 0, null);
		}
		else {
			this.createAsyncResult(null, 1, response['errorMessage']);
		}
		this._callback$p$1(this._asyncResult$p$0);
	}
}
$h.ProxyRequestBase=function $h_ProxyRequestBase(userContext) {
	$h.ProxyRequestBase.initializeBase(this, [ userContext ]);
}
$h.ProxyRequestBase.prototype={
	handleResponse: function $h_ProxyRequestBase$handleResponse(response) {
		if (!(response['wasProxySuccessful'])) {
			this.set__statusCode$i$1(500);
			this.set__statusDescription$i$1('Error');
			var errorMessage=response['errorMessage'];
			this.set__response$i$1(errorMessage);
			this.createAsyncResult(null, 1, errorMessage);
		}
		else {
			this.set__statusCode$i$1(response['statusCode']);
			this.set__statusDescription$i$1(response['statusDescription']);
			this.set__response$i$1(response['body']);
			this.createAsyncResult(this.get__response$i$1(), 0, null);
		}
		this._parseExtraResponseData$i$1(response);
		this._cycleReadyStateFromHeadersReceivedToLoadingToDone$i$1();
	},
	_throwInvalidStateException$i$1: function $h_ProxyRequestBase$_throwInvalidStateException$i$1() {
		throw Error.create('DOMException', { code: 11, message: 'INVALID_STATE_ERR' });
	},
	_cycleReadyStateFromHeadersReceivedToLoadingToDone$i$1: function $h_ProxyRequestBase$_cycleReadyStateFromHeadersReceivedToLoadingToDone$i$1() {
		var $$t_0=this;
		this._changeReadyState$i$1(2, function() {
			$$t_0._changeReadyState$i$1(3, function() {
				$$t_0._changeReadyState$i$1(4, null);
			});
		});
	},
	_changeReadyState$i$1: function $h_ProxyRequestBase$_changeReadyState$i$1(state, nextStep) {
		this.set__requestState$i$1(state);
		var $$t_2=this;
		window.setTimeout(function() {
			try {
				$$t_2._callOnReadyStateChangeCallback$i$1();
			}
			finally {
				if (!$h.ScriptHelpers.isNullOrUndefined(nextStep)) {
					nextStep();
				}
			}
		}, 0);
	},
	_checkSendConditions$i$1: function $h_ProxyRequestBase$_checkSendConditions$i$1() {
		if (this.get__requestState$i$1() !==1) {
			this._throwInvalidStateException$i$1();
		}
		if (this._isSent$p$0) {
			this._throwInvalidStateException$i$1();
		}
	}
}
$h.RequestBase=function $h_RequestBase(userContext) {
	this._userContext$p$0=userContext;
}
$h.RequestBase.prototype={
	_isSent$p$0: false,
	_asyncResult$p$0: null,
	_userContext$p$0: null,
	_sendRequest$i$0: function $h_RequestBase$_sendRequest$i$0(dispid, methodName, dataToSend) {
		this._isSent$p$0=true;
		var $$t_5=this;
		OSF.DDA.OutlookAppOm._instance$p._invokeHostMethod$i$0(dispid, methodName, dataToSend, function(resultCode, response) {
			if (resultCode) {
				$$t_5.createAsyncResult(null, 1, String.format(_u.ExtensibilityStrings.l_InternalProtocolError_Text, resultCode));
			}
			else {
				$$t_5.handleResponse(response);
			}
		});
	},
	createAsyncResult: function $h_RequestBase$createAsyncResult(value, errorCode, errorDescription) {
		this._asyncResult$p$0=OSF.DDA.OutlookAppOm._createAsyncResult$i(value, errorCode, errorDescription, this._userContext$p$0);
	}
}
$h._saveDictionaryRequest=function $h__saveDictionaryRequest(callback, userContext) {
	$h._saveDictionaryRequest.initializeBase(this, [ userContext ]);
	if (!$h.ScriptHelpers.isNullOrUndefined(callback)) {
		this._callback$p$1=callback;
	}
}
$h._saveDictionaryRequest.prototype={
	_callback$p$1: null,
	handleResponse: function $h__saveDictionaryRequest$handleResponse(response) {
		if (response['wasSuccessful']) {
			this.createAsyncResult(null, 0, null);
		}
		else {
			this.createAsyncResult(null, 1, response['errorMessage']);
		}
		if (!$h.ScriptHelpers.isNullOrUndefined(this._callback$p$1)) {
			this._callback$p$1(this._asyncResult$p$0);
		}
	}
}
$h.ScriptHelpers=function $h_ScriptHelpers() {
}
$h.ScriptHelpers.isNull=function $h_ScriptHelpers$isNull(value) {
	return null===value;
}
$h.ScriptHelpers.isNullOrUndefined=function $h_ScriptHelpers$isNullOrUndefined(value) {
	return $h.ScriptHelpers.isNull(value) || $h.ScriptHelpers.isUndefined(value);
}
$h.ScriptHelpers.isUndefined=function $h_ScriptHelpers$isUndefined(value) {
	return value===undefined;
}
$h.ScriptHelpers.dictionaryContainsKey=function $h_ScriptHelpers$dictionaryContainsKey(obj, keyName) {
	return (Object.isInstanceOfType(obj)) ? keyName in obj : false;
}
OSF.DDA.OutlookAppOm.registerClass('OSF.DDA.OutlookAppOm');
$h.Item.registerClass('$h.Item');
$h.Appointment.registerClass('$h.Appointment', $h.Item);
$h.Contact.registerClass('$h.Contact');
$h.CustomProperties.registerClass('$h.CustomProperties');
$h.Diagnostics.registerClass('$h.Diagnostics');
$h.EmailAddressDetails.registerClass('$h.EmailAddressDetails');
$h.Entities.registerClass('$h.Entities');
$h.Message.registerClass('$h.Message', $h.Item);
$h.MeetingRequest.registerClass('$h.MeetingRequest', $h.Message);
$h.MeetingSuggestion.registerClass('$h.MeetingSuggestion');
$h.MeetingSuggestionTimeDecoder.registerClass('$h.MeetingSuggestionTimeDecoder');
$h._extractedDate.registerClass('$h._extractedDate');
$h._preciseDate.registerClass('$h._preciseDate', $h._extractedDate);
$h._relativeDate.registerClass('$h._relativeDate', $h._extractedDate);
$h.PhoneNumber.registerClass('$h.PhoneNumber');
$h.TaskSuggestion.registerClass('$h.TaskSuggestion');
$h.UserProfile.registerClass('$h.UserProfile');
$h.RequestBase.registerClass('$h.RequestBase');
$h.ProxyRequestBase.registerClass('$h.ProxyRequestBase', $h.RequestBase);
$h.EwsRequest.registerClass('$h.EwsRequest', $h.ProxyRequestBase);
$h.InitialData.registerClass('$h.InitialData');
$h._loadDictionaryRequest.registerClass('$h._loadDictionaryRequest', $h.RequestBase);
$h._saveDictionaryRequest.registerClass('$h._saveDictionaryRequest', $h.RequestBase);
$h.ScriptHelpers.registerClass('$h.ScriptHelpers');
OSF.DDA.OutlookAppOm._maxRecipients$p=100;
OSF.DDA.OutlookAppOm._maxSubjectLength$p=255;
OSF.DDA.OutlookAppOm._maxBodyLength$p=32768;
OSF.DDA.OutlookAppOm._maxLocationLength$p=255;
OSF.DDA.OutlookAppOm._maxEwsRequestSize$p=1000000;
OSF.DDA.OutlookAppOm._instance$p=null;
$h.Diagnostics.outlookAppName='Outlook';
$h.Diagnostics.outlookWebAppName='OutlookWebApp';
$h.EmailAddressDetails._emptyString$p='';
$h.EmailAddressDetails._responseTypeMap$p=[ Microsoft.Office.WebExtension.MailboxEnums.ResponseType.None, Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Organizer, Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Tentative, Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Accepted, Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Declined ];
$h.EmailAddressDetails._recipientTypeMap$p=[ Microsoft.Office.WebExtension.MailboxEnums.RecipientType.Other, Microsoft.Office.WebExtension.MailboxEnums.RecipientType.DistributionList, Microsoft.Office.WebExtension.MailboxEnums.RecipientType.User, Microsoft.Office.WebExtension.MailboxEnums.RecipientType.ExternalUser ];
$h.Entities._allEntityKeys$p=[ 'Addresses', 'EmailAddresses', 'Urls', 'PhoneNumbers', 'TaskSuggestions', 'MeetingSuggestions', 'Contacts' ];
$h.MeetingSuggestionTimeDecoder._baseDate$p=new Date('0001-01-01T00:00:00Z');
$h.ScriptHelpers.emptyString='';
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
		var outlookSettingValues=serializedSettings['SettingsKey'];
		if (outlookSettingValues) {
			if(JSON)
				serializedSettings=JSON.parse(outlookSettingValues);
			else
				serializedSettings=Sys.Serialization.JavaScriptSerializer.deserialize(outlookSettingValues, true);
		}
		return serializedSettings;
	},
	write: function OSF_DDA_RichClientSettingsManager$Write(serializedSettings, overwriteIfStale, onCalling, onReceiving) {
		var keys=[];
		var values=[];
		var outlookSerializedSettings;
		if(JSON)
			outlookSerializedSettings=JSON.stringify(serializedSettings);
		else
			outlookSerializedSettings=Sys.Serialization.JavaScriptSerializer.serialize(serializedSettings);
		keys.push('SettingsKey');
		values.push(outlookSerializedSettings);
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
OSF.OUtil.getTrailingItem=function OSF_OUtil$getTrailingFunction(list, type) {
	if (list.length > 0) {
		var candidate=list[list.length - 1];
		if (typeof candidate==type)
			return candidate;
	}
	return null;
}
OSF.OUtil.checkParamsAndGetCallback=function OSF_OUtil$checkParamsAndGetCallback(suppliedArguments, expectedArguments) {
	var callback=OSF.OUtil.getTrailingItem(suppliedArguments, "function");
	var options=OSF.OUtil.getTrailingItem(suppliedArguments, "object");
	if (options) {
		if (options[Microsoft.Office.WebExtension.Parameters.Callback]) {
			if (callback) {
				throw OSF.OUtil.formatString(Strings.OfficeOM.L_RedundantCallbackSpecification);
			} else {
				callback=options[Microsoft.Office.WebExtension.Parameters.Callback];
				var callbackType=typeof callback;
				if (callbackType !="function") {
					throw OSF.OUtil.formatString(Strings.OfficeOM.L_CallbackNotAFunction, callbackType);
				}
			}
		}
	}
	expectedArguments.push({ name: "options", type: Object, optional: true });
	var e=Function._validateParams(suppliedArguments, expectedArguments, false );
	if (e) throw e;
	return callback;
}
OSF.DDA.Settings=function OSF_DDA_Settings(settings) {
	settings=settings || {};
	Object.defineProperties(this, {
		"get": {
			value: function OSF_DDA_Settings$get(name) {
				var e=Function._validateParams(arguments, [
					{ name: "name", type: String, mayBeNull: false }
				]);
				if (e) throw e;
				var setting=settings[name];
				return setting || null;
			}
		},
		"set": {
			value: function OSF_DDA_Settings$set(name, value) {
				var e=Function._validateParams(arguments, [
					{ name: "name", type: String, mayBeNull: false },
					{ name: "value", mayBeNull: true }
				]);
				if (e) throw e;
				settings[name]=value;
			}
		},
		"remove": {
			value: function OSF_DDA_Settings$remove(name) {
				var e=Function._validateParams(arguments, [
					{ name: "name", type: String, mayBeNull: false }
				]);
				if (e) throw e;
				delete settings[name];
			}
		},
		"saveAsync": {
			value: function OSF_DDA_Settings$saveAsync(options) {
				var callback=OSF.OUtil.checkParamsAndGetCallback(arguments, []);
				options=options || {};
				var errorArgs;
				try {
					var serializedSettings=OSF.DDA.SettingsManager.serializeSettings(settings);
					OSF.DDA.RichClientSettingsManager.write(serializedSettings);
				}
				catch (ex) {
					errorArgs={};
					errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Name]=OSF.DDA.AsyncResultEnum.ErrorCode.Failed;
					errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Message]=ex.message;
				}
				if(callback) {
					var initArgs={};
					initArgs[OSF.DDA.AsyncResultEnum.Properties.Context]=options[Microsoft.Office.WebExtension.Parameters.AsyncContext];
					initArgs[OSF.DDA.AsyncResultEnum.Properties.Value]=this;
					var asyncResult=new OSF.DDA.AsyncResult(initArgs, errorArgs);
					callback(asyncResult);
				}
			}
		}
	});
};

