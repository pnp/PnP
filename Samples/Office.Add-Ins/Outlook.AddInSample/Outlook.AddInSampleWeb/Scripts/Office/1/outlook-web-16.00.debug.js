/* Outlook web application specific API library */
/* Version: 16.0.4017.3000 Build Time: 07/15/2015 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/

var OSF=OSF || {};
var OfficeExt;
(function(OfficeExt)
{
	var SafeStorage=function()
		{
			function SafeStorage(_internalStorage)
			{
				this._internalStorage=_internalStorage
			}
			SafeStorage.prototype.getItem=function(key)
			{
				try
				{
					return this._internalStorage && this._internalStorage.getItem(key)
				}
				catch(e)
				{
					return null
				}
			};
			SafeStorage.prototype.setItem=function(key, data)
			{
				try
				{
					this._internalStorage && this._internalStorage.setItem(key,data)
				}
				catch(e){}
			};
			SafeStorage.prototype.clear=function()
			{
				try
				{
					this._internalStorage && this._internalStorage.clear()
				}
				catch(e){}
			};
			SafeStorage.prototype.removeItem=function(key)
			{
				try
				{
					this._internalStorage && this._internalStorage.removeItem(key)
				}
				catch(e){}
			};
			SafeStorage.prototype.getKeysWithPrefix=function(keyPrefix)
			{
				var keyList=[];
				try
				{
					var len=this._internalStorage && this._internalStorage.length || 0;
					for(var i=0; i < len; i++)
					{
						var key=this._internalStorage.key(i);
						if(key.indexOf(keyPrefix)===0)
							keyList.push(key)
					}
				}
				catch(e){}
				return keyList
			};
			return SafeStorage
		}();
	OfficeExt.SafeStorage=SafeStorage
})(OfficeExt || (OfficeExt={}));
OSF.OUtil=function()
{
	var _uniqueId=-1;
	var _xdmInfoKey="&_xdm_Info=";
	var _xdmSessionKeyPrefix="_xdm_";
	var _fragmentSeparator="#";
	var _loadedScripts={};
	var _defaultScriptLoadingTimeout=3e4;
	var _safeSessionStorage=null;
	var _safeLocalStorage=null;
	var _rndentropy=(new Date).getTime();
	function _random()
	{
		var nextrand=2147483647 * Math.random();
		nextrand ^=_rndentropy ^ (new Date).getMilliseconds() << Math.floor(Math.random() * (31 - 10));
		return nextrand.toString(16)
	}
	function _getSessionStorage()
	{
		if(!_safeSessionStorage)
		{
			try
			{
				var sessionStorage=window.sessionStorage
			}
			catch(ex)
			{
				sessionStorage=null
			}
			_safeSessionStorage=new OfficeExt.SafeStorage(sessionStorage)
		}
		return _safeSessionStorage
	}
	return{
			set_entropy: function OSF_OUtil$set_entropy(entropy)
			{
				if(typeof entropy=="string")
					for(var i=0; i < entropy.length; i+=4)
					{
						var temp=0;
						for(var j=0; j < 4 && i+j < entropy.length; j++)
							temp=(temp << 8)+entropy.charCodeAt(i+j);
						_rndentropy ^=temp
					}
				else if(typeof entropy=="number")
					_rndentropy ^=entropy;
				else
					_rndentropy ^=2147483647 * Math.random();
				_rndentropy &=2147483647
			},
			extend: function OSF_OUtil$extend(child, parent)
			{
				var F=function(){};
				F.prototype=parent.prototype;
				child.prototype=new F;
				child.prototype.constructor=child;
				child.uber=parent.prototype;
				if(parent.prototype.constructor===Object.prototype.constructor)
					parent.prototype.constructor=parent
			},
			setNamespace: function OSF_OUtil$setNamespace(name, parent)
			{
				if(parent && name && !parent[name])
					parent[name]={}
			},
			unsetNamespace: function OSF_OUtil$unsetNamespace(name, parent)
			{
				if(parent && name && parent[name])
					delete parent[name]
			},
			loadScript: function OSF_OUtil$loadScript(url, callback, timeoutInMs)
			{
				if(url && callback)
				{
					var doc=window.document;
					var _loadedScriptEntry=_loadedScripts[url];
					if(!_loadedScriptEntry)
					{
						var script=doc.createElement("script");
						script.type="text/javascript";
						_loadedScriptEntry={
							loaded: false,
							pendingCallbacks: [callback],
							timer: null
						};
						_loadedScripts[url]=_loadedScriptEntry;
						var onLoadCallback=function OSF_OUtil_loadScript$onLoadCallback()
							{
								if(_loadedScriptEntry.timer !=null)
								{
									clearTimeout(_loadedScriptEntry.timer);
									delete _loadedScriptEntry.timer
								}
								_loadedScriptEntry.loaded=true;
								var pendingCallbackCount=_loadedScriptEntry.pendingCallbacks.length;
								for(var i=0; i < pendingCallbackCount; i++)
								{
									var currentCallback=_loadedScriptEntry.pendingCallbacks.shift();
									currentCallback()
								}
							};
						var onLoadError=function OSF_OUtil_loadScript$onLoadError()
							{
								delete _loadedScripts[url];
								if(_loadedScriptEntry.timer !=null)
								{
									clearTimeout(_loadedScriptEntry.timer);
									delete _loadedScriptEntry.timer
								}
								var pendingCallbackCount=_loadedScriptEntry.pendingCallbacks.length;
								for(var i=0; i < pendingCallbackCount; i++)
								{
									var currentCallback=_loadedScriptEntry.pendingCallbacks.shift();
									currentCallback()
								}
							};
						if(script.readyState)
							script.onreadystatechange=function()
							{
								if(script.readyState=="loaded" || script.readyState=="complete")
								{
									script.onreadystatechange=null;
									onLoadCallback()
								}
							};
						else
							script.onload=onLoadCallback;
						script.onerror=onLoadError;
						timeoutInMs=timeoutInMs || _defaultScriptLoadingTimeout;
						_loadedScriptEntry.timer=setTimeout(onLoadError,timeoutInMs);
						script.src=url;
						doc.getElementsByTagName("head")[0].appendChild(script)
					}
					else if(_loadedScriptEntry.loaded)
						callback();
					else
						_loadedScriptEntry.pendingCallbacks.push(callback)
				}
			},
			loadCSS: function OSF_OUtil$loadCSS(url)
			{
				if(url)
				{
					var doc=window.document;
					var link=doc.createElement("link");
					link.type="text/css";
					link.rel="stylesheet";
					link.href=url;
					doc.getElementsByTagName("head")[0].appendChild(link)
				}
			},
			parseEnum: function OSF_OUtil$parseEnum(str, enumObject)
			{
				var parsed=enumObject[str.trim()];
				if(typeof parsed=="undefined")
				{
					Sys.Debug.trace("invalid enumeration string:"+str);
					throw Error.argument("str");
				}
				return parsed
			},
			delayExecutionAndCache: function OSF_OUtil$delayExecutionAndCache()
			{
				var obj={calc: arguments[0]};
				return function()
					{
						if(obj.calc)
						{
							obj.val=obj.calc.apply(this,arguments);
							delete obj.calc
						}
						return obj.val
					}
			},
			getUniqueId: function OSF_OUtil$getUniqueId()
			{
				_uniqueId=_uniqueId+1;
				return _uniqueId.toString()
			},
			formatString: function OSF_OUtil$formatString()
			{
				var args=arguments;
				var source=args[0];
				return source.replace(/{(\d+)}/gm,function(match, number)
					{
						var index=parseInt(number,10)+1;
						return args[index]===undefined ? "{"+number+"}" : args[index]
					})
			},
			generateConversationId: function OSF_OUtil$generateConversationId()
			{
				return[_random(),_random(),(new Date).getTime().toString()].join("_")
			},
			getFrameNameAndConversationId: function OSF_OUtil$getFrameNameAndConversationId(cacheKey, frame)
			{
				var frameName=_xdmSessionKeyPrefix+cacheKey+this.generateConversationId();
				frame.setAttribute("name",frameName);
				return this.generateConversationId()
			},
			addXdmInfoAsHash: function OSF_OUtil$addXdmInfoAsHash(url, xdmInfoValue)
			{
				url=url.trim() || "";
				var urlParts=url.split(_fragmentSeparator);
				var urlWithoutFragment=urlParts.shift();
				var fragment=urlParts.join(_fragmentSeparator);
				return[urlWithoutFragment,_fragmentSeparator,fragment,_xdmInfoKey,xdmInfoValue].join("")
			},
			parseXdmInfo: function OSF_OUtil$parseXdmInfo(skipSessionStorage)
			{
				return OSF.OUtil.parseXdmInfoWithGivenFragment(skipSessionStorage,window.location.hash)
			},
			parseXdmInfoWithGivenFragment: function OSF_OUtil$parseXdmInfoWithGivenFragment(skipSessionStorage, fragment)
			{
				var fragmentParts=fragment.split(_xdmInfoKey);
				var xdmInfoValue=fragmentParts.length > 1 ? fragmentParts[fragmentParts.length - 1] : null;
				var osfSessionStorage=_getSessionStorage();
				if(!skipSessionStorage && osfSessionStorage)
				{
					var sessionKeyStart=window.name.indexOf(_xdmSessionKeyPrefix);
					if(sessionKeyStart > -1)
					{
						var sessionKeyEnd=window.name.indexOf(";",sessionKeyStart);
						if(sessionKeyEnd==-1)
							sessionKeyEnd=window.name.length;
						var sessionKey=window.name.substring(sessionKeyStart,sessionKeyEnd);
						if(xdmInfoValue)
							osfSessionStorage.setItem(sessionKey,xdmInfoValue);
						else
							xdmInfoValue=osfSessionStorage.getItem(sessionKey)
					}
				}
				return xdmInfoValue
			},
			getConversationId: function OSF_OUtil$getConversationId()
			{
				var searchString=window.location.search;
				var conversationId=null;
				if(searchString)
				{
					var index=searchString.indexOf("&");
					conversationId=index > 0 ? searchString.substring(1,index) : searchString.substr(1);
					if(conversationId && conversationId.charAt(conversationId.length - 1)==="=")
					{
						conversationId=conversationId.substring(0,conversationId.length - 1);
						if(conversationId)
							conversationId=decodeURIComponent(conversationId)
					}
				}
				return conversationId
			},
			getInfoItems: function OSF_OUtil$getInfoItems(strInfo)
			{
				var items=strInfo.split("$");
				if(typeof items[1]=="undefined")
					items=strInfo.split("|");
				return items
			},
			getConversationUrl: function OSF_OUtil$getConversationUrl()
			{
				var conversationUrl="";
				var xdmInfoValue=OSF.OUtil.parseXdmInfo(true);
				if(xdmInfoValue)
				{
					var items=OSF.OUtil.getInfoItems(xdmInfoValue);
					if(items !=undefined && items.length >=3)
						conversationUrl=items[2]
				}
				return conversationUrl
			},
			validateParamObject: function OSF_OUtil$validateParamObject(params, expectedProperties, callback)
			{
				var e=Function._validateParams(arguments,[{
							name: "params",
							type: Object,
							mayBeNull: false
						},{
							name: "expectedProperties",
							type: Object,
							mayBeNull: false
						},{
							name: "callback",
							type: Function,
							mayBeNull: true
						}]);
				if(e)
					throw e;
				for(var p in expectedProperties)
				{
					e=Function._validateParameter(params[p],expectedProperties[p],p);
					if(e)
						throw e;
				}
			},
			writeProfilerMark: function OSF_OUtil$writeProfilerMark(text)
			{
				if(window.msWriteProfilerMark)
				{
					window.msWriteProfilerMark(text);
					if(typeof Sys !=="undefined" && Sys && Sys.Debug)
						Sys.Debug.trace(text)
				}
			},
			outputDebug: function OSF_OUtil$outputDebug(text)
			{
				if(typeof Sys !=="undefined" && Sys && Sys.Debug)
					Sys.Debug.trace(text)
			},
			defineNondefaultProperty: function OSF_OUtil$defineNondefaultProperty(obj, prop, descriptor, attributes)
			{
				descriptor=descriptor || {};
				for(var nd in attributes)
				{
					var attribute=attributes[nd];
					if(descriptor[attribute]==undefined)
						descriptor[attribute]=true
				}
				Object.defineProperty(obj,prop,descriptor);
				return obj
			},
			defineNondefaultProperties: function OSF_OUtil$defineNondefaultProperties(obj, descriptors, attributes)
			{
				descriptors=descriptors || {};
				for(var prop in descriptors)
					OSF.OUtil.defineNondefaultProperty(obj,prop,descriptors[prop],attributes);
				return obj
			},
			defineEnumerableProperty: function OSF_OUtil$defineEnumerableProperty(obj, prop, descriptor)
			{
				return OSF.OUtil.defineNondefaultProperty(obj,prop,descriptor,["enumerable"])
			},
			defineEnumerableProperties: function OSF_OUtil$defineEnumerableProperties(obj, descriptors)
			{
				return OSF.OUtil.defineNondefaultProperties(obj,descriptors,["enumerable"])
			},
			defineMutableProperty: function OSF_OUtil$defineMutableProperty(obj, prop, descriptor)
			{
				return OSF.OUtil.defineNondefaultProperty(obj,prop,descriptor,["writable","enumerable","configurable"])
			},
			defineMutableProperties: function OSF_OUtil$defineMutableProperties(obj, descriptors)
			{
				return OSF.OUtil.defineNondefaultProperties(obj,descriptors,["writable","enumerable","configurable"])
			},
			finalizeProperties: function OSF_OUtil$finalizeProperties(obj, descriptor)
			{
				descriptor=descriptor || {};
				var props=Object.getOwnPropertyNames(obj);
				var propsLength=props.length;
				for(var i=0; i < propsLength; i++)
				{
					var prop=props[i];
					var desc=Object.getOwnPropertyDescriptor(obj,prop);
					if(!desc.get && !desc.set)
						desc.writable=descriptor.writable || false;
					desc.configurable=descriptor.configurable || false;
					desc.enumerable=descriptor.enumerable || true;
					Object.defineProperty(obj,prop,desc)
				}
				return obj
			},
			mapList: function OSF_OUtil$MapList(list, mapFunction)
			{
				var ret=[];
				if(list)
					for(var item in list)
						ret.push(mapFunction(list[item]));
				return ret
			},
			listContainsKey: function OSF_OUtil$listContainsKey(list, key)
			{
				for(var item in list)
					if(key==item)
						return true;
				return false
			},
			listContainsValue: function OSF_OUtil$listContainsElement(list, value)
			{
				for(var item in list)
					if(value==list[item])
						return true;
				return false
			},
			augmentList: function OSF_OUtil$augmentList(list, addenda)
			{
				var add=list.push ? function(key, value)
					{
						list.push(value)
					} : function(key, value)
					{
						list[key]=value
					};
				for(var key in addenda)
					add(key,addenda[key])
			},
			redefineList: function OSF_Outil$redefineList(oldList, newList)
			{
				for(var key1 in oldList)
					delete oldList[key1];
				for(var key2 in newList)
					oldList[key2]=newList[key2]
			},
			isArray: function OSF_OUtil$isArray(obj)
			{
				return Object.prototype.toString.apply(obj)==="[object Array]"
			},
			isFunction: function OSF_OUtil$isFunction(obj)
			{
				return Object.prototype.toString.apply(obj)==="[object Function]"
			},
			isDate: function OSF_OUtil$isDate(obj)
			{
				return Object.prototype.toString.apply(obj)==="[object Date]"
			},
			addEventListener: function OSF_OUtil$addEventListener(element, eventName, listener)
			{
				if(element.addEventListener)
					element.addEventListener(eventName,listener,false);
				else if(Sys.Browser.agent===Sys.Browser.InternetExplorer && element.attachEvent)
					element.attachEvent("on"+eventName,listener);
				else
					element["on"+eventName]=listener
			},
			removeEventListener: function OSF_OUtil$removeEventListener(element, eventName, listener)
			{
				if(element.removeEventListener)
					element.removeEventListener(eventName,listener,false);
				else if(Sys.Browser.agent===Sys.Browser.InternetExplorer && element.detachEvent)
					element.detachEvent("on"+eventName,listener);
				else
					element["on"+eventName]=null
			},
			encodeBase64: function OSF_Outil$encodeBase64(input)
			{
				if(!input)
					return input;
				var codex="ABCDEFGHIJKLMNOP"+"QRSTUVWXYZabcdef"+"ghijklmnopqrstuv"+"wxyz0123456789+/=";
				var output=[];
				var temp=[];
				var index=0;
				var c1,
					c2,
					c3,
					a,
					b,
					c;
				var i;
				var length=input.length;
				do
				{
					c1=input.charCodeAt(index++);
					c2=input.charCodeAt(index++);
					c3=input.charCodeAt(index++);
					i=0;
					a=c1 & 255;
					b=c1 >> 8;
					c=c2 & 255;
					temp[i++]=a >> 2;
					temp[i++]=(a & 3) << 4 | b >> 4;
					temp[i++]=(b & 15) << 2 | c >> 6;
					temp[i++]=c & 63;
					if(!isNaN(c2))
					{
						a=c2 >> 8;
						b=c3 & 255;
						c=c3 >> 8;
						temp[i++]=a >> 2;
						temp[i++]=(a & 3) << 4 | b >> 4;
						temp[i++]=(b & 15) << 2 | c >> 6;
						temp[i++]=c & 63
					}
					if(isNaN(c2))
						temp[i - 1]=64;
					else if(isNaN(c3))
					{
						temp[i - 2]=64;
						temp[i - 1]=64
					}
					for(var t=0; t < i; t++)
						output.push(codex.charAt(temp[t]))
				} while(index < length);
				return output.join("")
			},
			getSessionStorage: function OSF_Outil$getSessionStorage()
			{
				return _getSessionStorage()
			},
			getLocalStorage: function OSF_Outil$getLocalStorage()
			{
				if(!_safeLocalStorage)
				{
					try
					{
						var localStorage=window.localStorage
					}
					catch(ex)
					{
						localStorage=null
					}
					_safeLocalStorage=new OfficeExt.SafeStorage(localStorage)
				}
				return _safeLocalStorage
			},
			convertIntToCssHexColor: function OSF_Outil$convertIntToCssHexColor(val)
			{
				var hex="#"+(Number(val)+16777216).toString(16).slice(-6);
				return hex
			},
			attachClickHandler: function OSF_Outil$attachClickHandler(element, handler)
			{
				element.onclick=function(e)
				{
					handler()
				};
				element.ontouchend=function(e)
				{
					handler();
					e.preventDefault()
				}
			},
			getQueryStringParamValue: function OSF_Outil$getQueryStringParamValue(queryString, paramName)
			{
				var e=Function._validateParams(arguments,[{
							name: "queryString",
							type: String,
							mayBeNull: false
						},{
							name: "paramName",
							type: String,
							mayBeNull: false
						}]);
				if(e)
				{
					Sys.Debug.trace("OSF_Outil_getQueryStringParamValue: Parameters cannot be null.");
					return""
				}
				var queryExp=new RegExp("[\\?&]"+paramName+"=([^&#]*)","i");
				if(!queryExp.test(queryString))
				{
					Sys.Debug.trace("OSF_Outil_getQueryStringParamValue: The parameter is not found.");
					return""
				}
				return queryExp.exec(queryString)[1]
			},
			isiOS: function OSF_Outil$isiOS()
			{
				return window.navigator.userAgent.match(/(iPad|iPhone|iPod)/g) ? true : false
			},
			shallowCopy: function OSF_Outil$shallowCopy(sourceObj)
			{
				var copyObj=sourceObj.constructor();
				for(var property in sourceObj)
					if(sourceObj.hasOwnProperty(property))
						copyObj[property]=sourceObj[property];
				return copyObj
			}
		}
}();
OSF.OUtil.Guid=function()
{
	var hexCode=["0","1","2","3","4","5","6","7","8","9","a","b","c","d","e","f"];
	return{generateNewGuid: function OSF_Outil_Guid$generateNewGuid()
			{
				var result="";
				var tick=(new Date).getTime();
				var index=0;
				for(; index < 32 && tick > 0; index++)
				{
					if(index==8 || index==12 || index==16 || index==20)
						result+="-";
					result+=hexCode[tick % 16];
					tick=Math.floor(tick / 16)
				}
				for(; index < 32; index++)
				{
					if(index==8 || index==12 || index==16 || index==20)
						result+="-";
					result+=hexCode[Math.floor(Math.random() * 16)]
				}
				return result
			}}
}();
window.OSF=OSF;
OSF.OUtil.setNamespace("OSF",window);
OSF.InternalPerfMarker={
	DataCoercionBegin: "Agave.HostCall.CoerceDataStart",
	DataCoercionEnd: "Agave.HostCall.CoerceDataEnd"
};
OSF.HostCallPerfMarker={
	IssueCall: "Agave.HostCall.IssueCall",
	ReceiveResponse: "Agave.HostCall.ReceiveResponse",
	RuntimeExceptionRaised: "Agave.HostCall.RuntimeExecptionRaised"
};
OSF.AgaveHostAction={
	Select: 0,
	UnSelect: 1,
	CancelDialog: 2,
	InsertAgave: 3,
	CtrlF6In: 4,
	CtrlF6Exit: 5,
	CtrlF6ExitShift: 6,
	SelectWithError: 7
};
OSF.SharedConstants={NotificationConversationIdSuffix: "_ntf"};
OSF.OfficeAppContext=function OSF_OfficeAppContext(id, appName, appVersion, appUILocale, dataLocale, docUrl, clientMode, settings, reason, osfControlType, eToken, correlationId, appInstanceId)
{
	this._id=id;
	this._appName=appName;
	this._appVersion=appVersion;
	this._appUILocale=appUILocale;
	this._dataLocale=dataLocale;
	this._docUrl=docUrl;
	this._clientMode=clientMode;
	this._settings=settings;
	this._reason=reason;
	this._osfControlType=osfControlType;
	this._eToken=eToken;
	this._correlationId=correlationId;
	this._appInstanceId=appInstanceId;
	this.get_id=function get_id()
	{
		return this._id
	};
	this.get_appName=function get_appName()
	{
		return this._appName
	};
	this.get_appVersion=function get_appVersion()
	{
		return this._appVersion
	};
	this.get_appUILocale=function get_appUILocale()
	{
		return this._appUILocale
	};
	this.get_dataLocale=function get_dataLocale()
	{
		return this._dataLocale
	};
	this.get_docUrl=function get_docUrl()
	{
		return this._docUrl
	};
	this.get_clientMode=function get_clientMode()
	{
		return this._clientMode
	};
	this.get_bindings=function get_bindings()
	{
		return this._bindings
	};
	this.get_settings=function get_settings()
	{
		return this._settings
	};
	this.get_reason=function get_reason()
	{
		return this._reason
	};
	this.get_osfControlType=function get_osfControlType()
	{
		return this._osfControlType
	};
	this.get_eToken=function get_eToken()
	{
		return this._eToken
	};
	this.get_correlationId=function get_correlationId()
	{
		return this._correlationId
	};
	this.get_appInstanceId=function get_appInstanceId()
	{
		return this._appInstanceId
	}
};
OSF.AppName={
	Unsupported: 0,
	Excel: 1,
	Word: 2,
	PowerPoint: 4,
	Outlook: 8,
	ExcelWebApp: 16,
	WordWebApp: 32,
	OutlookWebApp: 64,
	Project: 128,
	AccessWebApp: 256,
	PowerpointWebApp: 512,
	ExcelIOS: 1024,
	Sway: 2048,
	WordIOS: 4096,
	PowerPointIOS: 8192,
	Access: 16384,
	Lync: 32768
};
OSF.OsfControlType={
	DocumentLevel: 0,
	ContainerLevel: 1
};
OSF.ClientMode={
	ReadOnly: 0,
	ReadWrite: 1
};
OSF.OUtil.setNamespace("Microsoft",window);
OSF.OUtil.setNamespace("Office",Microsoft);
OSF.OUtil.setNamespace("Client",Microsoft.Office);
OSF.OUtil.setNamespace("WebExtension",Microsoft.Office);
Microsoft.Office.WebExtension.InitializationReason={
	Inserted: "inserted",
	DocumentOpened: "documentOpened"
};
Microsoft.Office.WebExtension.ValueFormat={
	Unformatted: "unformatted",
	Formatted: "formatted"
};
Microsoft.Office.WebExtension.FilterType={All: "all"};
Microsoft.Office.WebExtension.Parameters={
	BindingType: "bindingType",
	CoercionType: "coercionType",
	ValueFormat: "valueFormat",
	FilterType: "filterType",
	Columns: "columns",
	SampleData: "sampleData",
	GoToType: "goToType",
	SelectionMode: "selectionMode",
	Id: "id",
	PromptText: "promptText",
	ItemName: "itemName",
	FailOnCollision: "failOnCollision",
	StartRow: "startRow",
	StartColumn: "startColumn",
	RowCount: "rowCount",
	ColumnCount: "columnCount",
	Callback: "callback",
	AsyncContext: "asyncContext",
	Data: "data",
	Rows: "rows",
	OverwriteIfStale: "overwriteIfStale",
	FileType: "fileType",
	EventType: "eventType",
	Handler: "handler",
	SliceSize: "sliceSize",
	SliceIndex: "sliceIndex",
	ActiveView: "activeView",
	Status: "status",
	Xml: "xml",
	Namespace: "namespace",
	Prefix: "prefix",
	XPath: "xPath",
	TaskId: "taskId",
	FieldId: "fieldId",
	FieldValue: "fieldValue",
	ServerUrl: "serverUrl",
	ListName: "listName",
	ResourceId: "resourceId",
	ViewType: "viewType",
	ViewName: "viewName",
	GetRawValue: "getRawValue",
	CellFormat: "cellFormat",
	TableOptions: "tableOptions",
	TaskIndex: "taskIndex",
	ResourceIndex: "resourceIndex"
};
OSF.OUtil.setNamespace("DDA",OSF);
OSF.DDA.DocumentMode={
	ReadOnly: 1,
	ReadWrite: 0
};
OSF.DDA.PropertyDescriptors={AsyncResultStatus: "AsyncResultStatus"};
OSF.DDA.EventDescriptors={};
OSF.DDA.ListDescriptors={};
OSF.DDA.getXdmEventName=function OSF_DDA$GetXdmEventName(bindingId, eventType)
{
	if(eventType==Microsoft.Office.WebExtension.EventType.BindingSelectionChanged || eventType==Microsoft.Office.WebExtension.EventType.BindingDataChanged)
		return bindingId+"_"+eventType;
	else
		return eventType
};
OSF.DDA.MethodDispId={
	dispidMethodMin: 64,
	dispidGetSelectedDataMethod: 64,
	dispidSetSelectedDataMethod: 65,
	dispidAddBindingFromSelectionMethod: 66,
	dispidAddBindingFromPromptMethod: 67,
	dispidGetBindingMethod: 68,
	dispidReleaseBindingMethod: 69,
	dispidGetBindingDataMethod: 70,
	dispidSetBindingDataMethod: 71,
	dispidAddRowsMethod: 72,
	dispidClearAllRowsMethod: 73,
	dispidGetAllBindingsMethod: 74,
	dispidLoadSettingsMethod: 75,
	dispidSaveSettingsMethod: 76,
	dispidGetDocumentCopyMethod: 77,
	dispidAddBindingFromNamedItemMethod: 78,
	dispidAddColumnsMethod: 79,
	dispidGetDocumentCopyChunkMethod: 80,
	dispidReleaseDocumentCopyMethod: 81,
	dispidNavigateToMethod: 82,
	dispidGetActiveViewMethod: 83,
	dispidGetDocumentThemeMethod: 84,
	dispidGetOfficeThemeMethod: 85,
	dispidGetFilePropertiesMethod: 86,
	dispidClearFormatsMethod: 87,
	dispidSetTableOptionsMethod: 88,
	dispidSetFormatsMethod: 89,
	dispidExecuteRichApiRequestMethod: 93,
	dispidAppCommandInvocationCompletedMethod: 94,
	dispidAddDataPartMethod: 128,
	dispidGetDataPartByIdMethod: 129,
	dispidGetDataPartsByNamespaceMethod: 130,
	dispidGetDataPartXmlMethod: 131,
	dispidGetDataPartNodesMethod: 132,
	dispidDeleteDataPartMethod: 133,
	dispidGetDataNodeValueMethod: 134,
	dispidGetDataNodeXmlMethod: 135,
	dispidGetDataNodesMethod: 136,
	dispidSetDataNodeValueMethod: 137,
	dispidSetDataNodeXmlMethod: 138,
	dispidAddDataNamespaceMethod: 139,
	dispidGetDataUriByPrefixMethod: 140,
	dispidGetDataPrefixByUriMethod: 141,
	dispidMethodMax: 141,
	dispidGetSelectedTaskMethod: 110,
	dispidGetSelectedResourceMethod: 111,
	dispidGetTaskMethod: 112,
	dispidGetResourceFieldMethod: 113,
	dispidGetWSSUrlMethod: 114,
	dispidGetTaskFieldMethod: 115,
	dispidGetProjectFieldMethod: 116,
	dispidGetSelectedViewMethod: 117,
	dispidGetTaskByIndexMethod: 118,
	dispidGetResourceByIndexMethod: 119,
	dispidSetTaskFieldMethod: 120,
	dispidSetResourceFieldMethod: 121,
	dispidGetMaxTaskIndexMethod: 122,
	dispidGetMaxResourceIndexMethod: 123
};
OSF.DDA.EventDispId={
	dispidEventMin: 0,
	dispidInitializeEvent: 0,
	dispidSettingsChangedEvent: 1,
	dispidDocumentSelectionChangedEvent: 2,
	dispidBindingSelectionChangedEvent: 3,
	dispidBindingDataChangedEvent: 4,
	dispidDocumentOpenEvent: 5,
	dispidDocumentCloseEvent: 6,
	dispidActiveViewChangedEvent: 7,
	dispidDocumentThemeChangedEvent: 8,
	dispidOfficeThemeChangedEvent: 9,
	dispidActivationStatusChangedEvent: 32,
	dispidAppCommandInvokedEvent: 39,
	dispidTaskSelectionChangedEvent: 56,
	dispidResourceSelectionChangedEvent: 57,
	dispidViewSelectionChangedEvent: 58,
	dispidDataNodeAddedEvent: 60,
	dispidDataNodeReplacedEvent: 61,
	dispidDataNodeDeletedEvent: 62,
	dispidEventMax: 63
};
OSF.DDA.ErrorCodeManager=function()
{
	var _errorMappings={};
	return{
			getErrorArgs: function OSF_DDA_ErrorCodeManager$getErrorArgs(errorCode)
			{
				return _errorMappings[errorCode] || _errorMappings[this.errorCodes.ooeInternalError]
			},
			addErrorMessage: function OSF_DDA_ErrorCodeManager$addErrorMessage(errorCode, errorNameMessage)
			{
				_errorMappings[errorCode]=errorNameMessage
			},
			errorCodes: {
				ooeSuccess: 0,
				ooeCoercionTypeNotSupported: 1e3,
				ooeGetSelectionNotMatchDataType: 1001,
				ooeCoercionTypeNotMatchBinding: 1002,
				ooeInvalidGetRowColumnCounts: 1003,
				ooeSelectionNotSupportCoercionType: 1004,
				ooeInvalidGetStartRowColumn: 1005,
				ooeNonUniformPartialGetNotSupported: 1006,
				ooeGetDataIsTooLarge: 1008,
				ooeFileTypeNotSupported: 1009,
				ooeGetDataParametersConflict: 1010,
				ooeInvalidGetColumns: 1011,
				ooeInvalidGetRows: 1012,
				ooeInvalidReadForBlankRow: 1013,
				ooeUnsupportedDataObject: 2e3,
				ooeCannotWriteToSelection: 2001,
				ooeDataNotMatchSelection: 2002,
				ooeOverwriteWorksheetData: 2003,
				ooeDataNotMatchBindingSize: 2004,
				ooeInvalidSetStartRowColumn: 2005,
				ooeInvalidDataFormat: 2006,
				ooeDataNotMatchCoercionType: 2007,
				ooeDataNotMatchBindingType: 2008,
				ooeSetDataIsTooLarge: 2009,
				ooeNonUniformPartialSetNotSupported: 2010,
				ooeInvalidSetColumns: 2011,
				ooeInvalidSetRows: 2012,
				ooeSetDataParametersConflict: 2013,
				ooeCellDataAmountBeyondLimits: 2014,
				ooeSelectionCannotBound: 3e3,
				ooeBindingNotExist: 3002,
				ooeBindingToMultipleSelection: 3003,
				ooeInvalidSelectionForBindingType: 3004,
				ooeOperationNotSupportedOnThisBindingType: 3005,
				ooeNamedItemNotFound: 3006,
				ooeMultipleNamedItemFound: 3007,
				ooeInvalidNamedItemForBindingType: 3008,
				ooeUnknownBindingType: 3009,
				ooeOperationNotSupportedOnMatrixData: 3010,
				ooeInvalidColumnsForBinding: 3011,
				ooeSettingNameNotExist: 4e3,
				ooeSettingsCannotSave: 4001,
				ooeSettingsAreStale: 4002,
				ooeOperationNotSupported: 5e3,
				ooeInternalError: 5001,
				ooeDocumentReadOnly: 5002,
				ooeEventHandlerNotExist: 5003,
				ooeInvalidApiCallInContext: 5004,
				ooeShuttingDown: 5005,
				ooeUnsupportedEnumeration: 5007,
				ooeIndexOutOfRange: 5008,
				ooeBrowserAPINotSupported: 5009,
				ooeInvalidParam: 5010,
				ooeRequestTimeout: 5011,
				ooeTooManyIncompleteRequests: 5100,
				ooeRequestTokenUnavailable: 5101,
				ooeCustomXmlNodeNotFound: 6e3,
				ooeCustomXmlError: 6100,
				ooeNoCapability: 7e3,
				ooeCannotNavTo: 7001,
				ooeSpecifiedIdNotExist: 7002,
				ooeNavOutOfBound: 7004,
				ooeElementMissing: 8e3,
				ooeProtectedError: 8001,
				ooeInvalidCellsValue: 8010,
				ooeInvalidTableOptionValue: 8011,
				ooeInvalidFormatValue: 8012,
				ooeRowIndexOutOfRange: 8020,
				ooeColIndexOutOfRange: 8021,
				ooeFormatValueOutOfRange: 8022,
				ooeCellFormatAmountBeyondLimits: 8023,
				ooeMemoryFileLimit: 11e3,
				ooeNetworkProblemRetrieveFile: 11001,
				ooeInvalidSliceSize: 11002,
				ooeInvalidCallback: 11101
			},
			initializeErrorMessages: function OSF_DDA_ErrorCodeManager$initializeErrorMessages(stringNS)
			{
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCoercionTypeNotSupported]={
					name: stringNS.L_InvalidCoercion,
					message: stringNS.L_CoercionTypeNotSupported
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeGetSelectionNotMatchDataType]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_GetSelectionNotSupported
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCoercionTypeNotMatchBinding]={
					name: stringNS.L_InvalidCoercion,
					message: stringNS.L_CoercionTypeNotMatchBinding
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidGetRowColumnCounts]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_InvalidGetRowColumnCounts
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSelectionNotSupportCoercionType]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_SelectionNotSupportCoercionType
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidGetStartRowColumn]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_InvalidGetStartRowColumn
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeNonUniformPartialGetNotSupported]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_NonUniformPartialGetNotSupported
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeGetDataIsTooLarge]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_GetDataIsTooLarge
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeFileTypeNotSupported]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_FileTypeNotSupported
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeGetDataParametersConflict]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_GetDataParametersConflict
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidGetColumns]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_InvalidGetColumns
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidGetRows]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_InvalidGetRows
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidReadForBlankRow]={
					name: stringNS.L_DataReadError,
					message: stringNS.L_InvalidReadForBlankRow
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeUnsupportedDataObject]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_UnsupportedDataObject
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCannotWriteToSelection]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_CannotWriteToSelection
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeDataNotMatchSelection]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_DataNotMatchSelection
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeOverwriteWorksheetData]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_OverwriteWorksheetData
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeDataNotMatchBindingSize]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_DataNotMatchBindingSize
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidSetStartRowColumn]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_InvalidSetStartRowColumn
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidDataFormat]={
					name: stringNS.L_InvalidFormat,
					message: stringNS.L_InvalidDataFormat
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeDataNotMatchCoercionType]={
					name: stringNS.L_InvalidDataObject,
					message: stringNS.L_DataNotMatchCoercionType
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeDataNotMatchBindingType]={
					name: stringNS.L_InvalidDataObject,
					message: stringNS.L_DataNotMatchBindingType
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSetDataIsTooLarge]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_SetDataIsTooLarge
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeNonUniformPartialSetNotSupported]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_NonUniformPartialSetNotSupported
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidSetColumns]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_InvalidSetColumns
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidSetRows]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_InvalidSetRows
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSetDataParametersConflict]={
					name: stringNS.L_DataWriteError,
					message: stringNS.L_SetDataParametersConflict
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSelectionCannotBound]={
					name: stringNS.L_BindingCreationError,
					message: stringNS.L_SelectionCannotBound
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeBindingNotExist]={
					name: stringNS.L_InvalidBindingError,
					message: stringNS.L_BindingNotExist
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeBindingToMultipleSelection]={
					name: stringNS.L_BindingCreationError,
					message: stringNS.L_BindingToMultipleSelection
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidSelectionForBindingType]={
					name: stringNS.L_BindingCreationError,
					message: stringNS.L_InvalidSelectionForBindingType
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeOperationNotSupportedOnThisBindingType]={
					name: stringNS.L_InvalidBindingOperation,
					message: stringNS.L_OperationNotSupportedOnThisBindingType
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeNamedItemNotFound]={
					name: stringNS.L_BindingCreationError,
					message: stringNS.L_NamedItemNotFound
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeMultipleNamedItemFound]={
					name: stringNS.L_BindingCreationError,
					message: stringNS.L_MultipleNamedItemFound
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidNamedItemForBindingType]={
					name: stringNS.L_BindingCreationError,
					message: stringNS.L_InvalidNamedItemForBindingType
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeUnknownBindingType]={
					name: stringNS.L_InvalidBinding,
					message: stringNS.L_UnknownBindingType
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeOperationNotSupportedOnMatrixData]={
					name: stringNS.L_InvalidBindingOperation,
					message: stringNS.L_OperationNotSupportedOnMatrixData
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidColumnsForBinding]={
					name: stringNS.L_InvalidBinding,
					message: stringNS.L_InvalidColumnsForBinding
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSettingNameNotExist]={
					name: stringNS.L_ReadSettingsError,
					message: stringNS.L_SettingNameNotExist
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSettingsCannotSave]={
					name: stringNS.L_SaveSettingsError,
					message: stringNS.L_SettingsCannotSave
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSettingsAreStale]={
					name: stringNS.L_SettingsStaleError,
					message: stringNS.L_SettingsAreStale
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeOperationNotSupported]={
					name: stringNS.L_HostError,
					message: stringNS.L_OperationNotSupported
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError]={
					name: stringNS.L_InternalError,
					message: stringNS.L_InternalErrorDescription
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeDocumentReadOnly]={
					name: stringNS.L_PermissionDenied,
					message: stringNS.L_DocumentReadOnly
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeEventHandlerNotExist]={
					name: stringNS.L_EventRegistrationError,
					message: stringNS.L_EventHandlerNotExist
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidApiCallInContext]={
					name: stringNS.L_InvalidAPICall,
					message: stringNS.L_InvalidApiCallInContext
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeShuttingDown]={
					name: stringNS.L_ShuttingDown,
					message: stringNS.L_ShuttingDown
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeUnsupportedEnumeration]={
					name: stringNS.L_UnsupportedEnumeration,
					message: stringNS.L_UnsupportedEnumerationMessage
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeIndexOutOfRange]={
					name: stringNS.L_IndexOutOfRange,
					message: stringNS.L_IndexOutOfRange
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeBrowserAPINotSupported]={
					name: stringNS.L_APINotSupported,
					message: stringNS.L_BrowserAPINotSupported
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeRequestTimeout]={
					name: stringNS.L_APICallFailed,
					message: stringNS.L_RequestTimeout
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeTooManyIncompleteRequests]={
					name: stringNS.L_APICallFailed,
					message: stringNS.L_TooManyIncompleteRequests
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeRequestTokenUnavailable]={
					name: stringNS.L_APICallFailed,
					message: stringNS.L_RequestTokenUnavailable
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCustomXmlNodeNotFound]={
					name: stringNS.L_InvalidNode,
					message: stringNS.L_CustomXmlNodeNotFound
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCustomXmlError]={
					name: stringNS.L_CustomXmlError,
					message: stringNS.L_CustomXmlError
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeNoCapability]={
					name: stringNS.L_PermissionDenied,
					message: stringNS.L_NoCapability
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCannotNavTo]={
					name: stringNS.L_CannotNavigateTo,
					message: stringNS.L_CannotNavigateTo
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeSpecifiedIdNotExist]={
					name: stringNS.L_SpecifiedIdNotExist,
					message: stringNS.L_SpecifiedIdNotExist
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeNavOutOfBound]={
					name: stringNS.L_NavOutOfBound,
					message: stringNS.L_NavOutOfBound
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCellDataAmountBeyondLimits]={
					name: stringNS.L_DataWriteReminder,
					message: stringNS.L_CellDataAmountBeyondLimits
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeElementMissing]={
					name: stringNS.L_MissingParameter,
					message: stringNS.L_ElementMissing
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeProtectedError]={
					name: stringNS.L_PermissionDenied,
					message: stringNS.L_NoCapability
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidCellsValue]={
					name: stringNS.L_InvalidValue,
					message: stringNS.L_InvalidCellsValue
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidTableOptionValue]={
					name: stringNS.L_InvalidValue,
					message: stringNS.L_InvalidTableOptionValue
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidFormatValue]={
					name: stringNS.L_InvalidValue,
					message: stringNS.L_InvalidFormatValue
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeRowIndexOutOfRange]={
					name: stringNS.L_OutOfRange,
					message: stringNS.L_RowIndexOutOfRange
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeColIndexOutOfRange]={
					name: stringNS.L_OutOfRange,
					message: stringNS.L_ColIndexOutOfRange
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeFormatValueOutOfRange]={
					name: stringNS.L_OutOfRange,
					message: stringNS.L_FormatValueOutOfRange
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeCellFormatAmountBeyondLimits]={
					name: stringNS.L_FormattingReminder,
					message: stringNS.L_CellFormatAmountBeyondLimits
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeMemoryFileLimit]={
					name: stringNS.L_MemoryLimit,
					message: stringNS.L_CloseFileBeforeRetrieve
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeNetworkProblemRetrieveFile]={
					name: stringNS.L_NetworkProblem,
					message: stringNS.L_NetworkProblemRetrieveFile
				};
				_errorMappings[OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidSliceSize]={
					name: stringNS.L_InvalidValue,
					message: stringNS.L_SliceSizeNotSupported
				}
			}
		}
}();
Microsoft.Office.WebExtension.ApplicationMode={
	WebEditor: "webEditor",
	WebViewer: "webViewer",
	Client: "client"
};
Microsoft.Office.WebExtension.DocumentMode={
	ReadOnly: "readOnly",
	ReadWrite: "readWrite"
};
OSF.NamespaceManager=function OSF_NamespaceManager()
{
	var _userOffice;
	var _useShortcut=false;
	return{
			enableShortcut: function OSF_NamespaceManager$enableShortcut()
			{
				if(!_useShortcut)
				{
					if(window.Office)
						_userOffice=window.Office;
					else
						OSF.OUtil.setNamespace("Office",window);
					window.Office=Microsoft.Office.WebExtension;
					_useShortcut=true
				}
			},
			disableShortcut: function OSF_NamespaceManager$disableShortcut()
			{
				if(_useShortcut)
				{
					if(_userOffice)
						window.Office=_userOffice;
					else
						OSF.OUtil.unsetNamespace("Office",window);
					_useShortcut=false
				}
			}
		}
}();
OSF.NamespaceManager.enableShortcut();
Microsoft.Office.WebExtension.useShortNamespace=function Microsoft_Office_WebExtension_useShortNamespace(useShortcut)
{
	if(useShortcut)
		OSF.NamespaceManager.enableShortcut();
	else
		OSF.NamespaceManager.disableShortcut()
};
Microsoft.Office.WebExtension.select=function Microsoft_Office_WebExtension_select(str, errorCallback)
{
	var promise;
	if(str && typeof str=="string")
	{
		var index=str.indexOf("#");
		if(index !=-1)
		{
			var op=str.substring(0,index);
			var target=str.substring(index+1);
			switch(op)
			{
				case"binding":
				case"bindings":
					if(target)
						promise=new OSF.DDA.BindingPromise(target);
					break
			}
		}
	}
	if(!promise)
	{
		if(errorCallback)
		{
			var callbackType=typeof errorCallback;
			if(callbackType=="function")
			{
				var callArgs={};
				callArgs[Microsoft.Office.WebExtension.Parameters.Callback]=errorCallback;
				OSF.DDA.issueAsyncResult(callArgs,OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidApiCallInContext,OSF.DDA.ErrorCodeManager.getErrorArgs(OSF.DDA.ErrorCodeManager.errorCodes.ooeInvalidApiCallInContext))
			}
			else
				throw OSF.OUtil.formatString(Strings.OfficeOM.L_CallbackNotAFunction,callbackType);
		}
	}
	else
	{
		promise.onFail=errorCallback;
		return promise
	}
};
OSF.DDA.Context=function OSF_DDA_Context(officeAppContext, document, license, appOM, getOfficeTheme)
{
	OSF.OUtil.defineEnumerableProperties(this,{
		contentLanguage: {value: officeAppContext.get_dataLocale()},
		displayLanguage: {value: officeAppContext.get_appUILocale()}
	});
	if(document)
		OSF.OUtil.defineEnumerableProperty(this,"document",{value: document});
	if(license)
		OSF.OUtil.defineEnumerableProperty(this,"license",{value: license});
	if(appOM)
	{
		var displayName=appOM.displayName || "appOM";
		delete appOM.displayName;
		OSF.OUtil.defineEnumerableProperty(this,displayName,{value: appOM})
	}
	if(getOfficeTheme)
		OSF.OUtil.defineEnumerableProperty(this,"officeTheme",{get: function()
			{
				return getOfficeTheme()
			}})
};
OSF.DDA.OutlookContext=function OSF_DDA_OutlookContext(appContext, settings, license, appOM, getOfficeTheme)
{
	OSF.DDA.OutlookContext.uber.constructor.call(this,appContext,null,license,appOM,getOfficeTheme);
	if(settings)
		OSF.OUtil.defineEnumerableProperty(this,"roamingSettings",{value: settings})
};
OSF.OUtil.extend(OSF.DDA.OutlookContext,OSF.DDA.Context);
OSF.DDA.OutlookAppOm=function OSF_DDA_OutlookAppOm(appContext, window, appReady){};
OSF.DDA.Document=function OSF_DDA_Document(officeAppContext, settings)
{
	var mode;
	switch(officeAppContext.get_clientMode())
	{
		case OSF.ClientMode.ReadOnly:
			mode=Microsoft.Office.WebExtension.DocumentMode.ReadOnly;
			break;
		case OSF.ClientMode.ReadWrite:
			mode=Microsoft.Office.WebExtension.DocumentMode.ReadWrite;
			break
	}
	if(settings)
		OSF.OUtil.defineEnumerableProperty(this,"settings",{value: settings});
	OSF.OUtil.defineMutableProperties(this,{
		mode: {value: mode},
		url: {value: officeAppContext.get_docUrl()}
	})
};
OSF.DDA.JsomDocument=function OSF_DDA_JsomDocument(officeAppContext, bindingFacade, settings)
{
	OSF.DDA.JsomDocument.uber.constructor.call(this,officeAppContext,settings);
	if(bindingFacade)
		OSF.OUtil.defineEnumerableProperty(this,"bindings",{get: function OSF_DDA_Document$GetBindings()
			{
				return bindingFacade
			}});
	var am=OSF.DDA.AsyncMethodNames;
	OSF.DDA.DispIdHost.addAsyncMethods(this,[am.GetSelectedDataAsync,am.SetSelectedDataAsync]);
	OSF.DDA.DispIdHost.addEventSupport(this,new OSF.EventDispatch([Microsoft.Office.WebExtension.EventType.DocumentSelectionChanged]))
};
OSF.OUtil.extend(OSF.DDA.JsomDocument,OSF.DDA.Document);
OSF.OUtil.defineEnumerableProperty(Microsoft.Office.WebExtension,"context",{get: function Microsoft_Office_WebExtension$GetContext()
	{
		var context;
		if(OSF && OSF._OfficeAppFactory)
			context=OSF._OfficeAppFactory.getContext();
		return context
	}});
OSF.DDA.License=function OSF_DDA_License(eToken)
{
	OSF.OUtil.defineEnumerableProperty(this,"value",{value: eToken})
};
OSF.OUtil.setNamespace("AsyncResultEnum",OSF.DDA);
OSF.DDA.AsyncResultEnum.Properties={
	Context: "Context",
	Value: "Value",
	Status: "Status",
	Error: "Error"
};
Microsoft.Office.WebExtension.AsyncResultStatus={
	Succeeded: "succeeded",
	Failed: "failed"
};
OSF.DDA.AsyncResultEnum.ErrorCode={
	Success: 0,
	Failed: 1
};
OSF.DDA.AsyncResultEnum.ErrorProperties={
	Name: "Name",
	Message: "Message",
	Code: "Code"
};
OSF.DDA.AsyncMethodNames={};
OSF.DDA.AsyncMethodNames.addNames=function(methodNames)
{
	for(var entry in methodNames)
	{
		var am={};
		OSF.OUtil.defineEnumerableProperties(am,{
			id: {value: entry},
			displayName: {value: methodNames[entry]}
		});
		OSF.DDA.AsyncMethodNames[entry]=am
	}
};
OSF.DDA.AsyncMethodCall=function OSF_DDA_AsyncMethodCall(requiredParameters, supportedOptions, privateStateCallbacks, onSucceeded, onFailed, checkCallArgs, displayName)
{
	var requiredCount=requiredParameters.length;
	var getInvalidParameterString=OSF.OUtil.delayExecutionAndCache(function()
		{
			return OSF.OUtil.formatString(Strings.OfficeOM.L_InvalidParameters,displayName)
		});
	function OSF_DAA_AsyncMethodCall$VerifyArguments(params, args)
	{
		for(var name in params)
		{
			var param=params[name];
			var arg=args[name];
			if(param["enum"])
				switch(typeof arg)
				{
					case"string":
						if(OSF.OUtil.listContainsValue(param["enum"],arg))
							break;
					case"undefined":
						throw OSF.DDA.ErrorCodeManager.errorCodes.ooeUnsupportedEnumeration;
						break;
					default:
						throw getInvalidParameterString();
				}
			if(param["types"])
				if(!OSF.OUtil.listContainsValue(param["types"],typeof arg))
					throw getInvalidParameterString();
		}
	}
	function OSF_DAA_AsyncMethodCall$ExtractRequiredArguments(userArgs, caller, stateInfo)
	{
		if(userArgs.length < requiredCount)
			throw Error.parameterCount(Strings.OfficeOM.L_MissingRequiredArguments);
		var requiredArgs=[];
		var index;
		for(index=0; index < requiredCount; index++)
			requiredArgs.push(userArgs[index]);
		OSF_DAA_AsyncMethodCall$VerifyArguments(requiredParameters,requiredArgs);
		var ret={};
		for(index=0; index < requiredCount; index++)
		{
			var param=requiredParameters[index];
			var arg=requiredArgs[index];
			if(param.verify)
			{
				var isValid=param.verify(arg,caller,stateInfo);
				if(!isValid)
					throw getInvalidParameterString();
			}
			ret[param.name]=arg
		}
		return ret
	}
	function OSF_DAA_AsyncMethodCall$ExtractOptions(userArgs, requiredArgs, caller, stateInfo)
	{
		if(userArgs.length > requiredCount+2)
			throw Error.parameterCount(Strings.OfficeOM.L_TooManyArguments);
		var options,
			parameterCallback;
		for(var i=userArgs.length - 1; i >=requiredCount; i--)
		{
			var argument=userArgs[i];
			switch(typeof argument)
			{
				case"object":
					if(options)
						throw Error.parameterCount(Strings.OfficeOM.L_TooManyOptionalObjects);
					else
						options=argument;
					break;
				case"function":
					if(parameterCallback)
						throw Error.parameterCount(Strings.OfficeOM.L_TooManyOptionalFunction);
					else
						parameterCallback=argument;
					break;
				default:
					throw Error.argument(Strings.OfficeOM.L_InValidOptionalArgument);
					break
			}
		}
		options=options || {};
		for(var optionName in supportedOptions)
			if(!OSF.OUtil.listContainsKey(options,optionName))
			{
				var value=undefined;
				var option=supportedOptions[optionName];
				if(option.calculate && requiredArgs)
					value=option.calculate(requiredArgs,caller,stateInfo);
				if(!value && option.defaultValue !==undefined)
					value=option.defaultValue;
				options[optionName]=value
			}
		if(parameterCallback)
			if(options[Microsoft.Office.WebExtension.Parameters.Callback])
				throw Strings.OfficeOM.L_RedundantCallbackSpecification;
			else
				options[Microsoft.Office.WebExtension.Parameters.Callback]=parameterCallback;
		OSF_DAA_AsyncMethodCall$VerifyArguments(supportedOptions,options);
		return options
	}
	this.verifyAndExtractCall=function OSF_DAA_AsyncMethodCall$VerifyAndExtractCall(userArgs, caller, stateInfo)
	{
		var required=OSF_DAA_AsyncMethodCall$ExtractRequiredArguments(userArgs,caller,stateInfo);
		var options=OSF_DAA_AsyncMethodCall$ExtractOptions(userArgs,required,caller,stateInfo);
		var callArgs={};
		for(var r in required)
			callArgs[r]=required[r];
		for(var o in options)
			callArgs[o]=options[o];
		for(var s in privateStateCallbacks)
			callArgs[s]=privateStateCallbacks[s](caller,stateInfo);
		if(checkCallArgs)
			callArgs=checkCallArgs(callArgs,caller,stateInfo);
		return callArgs
	};
	this.processResponse=function OSF_DAA_AsyncMethodCall$ProcessResponse(status, response, caller, callArgs)
	{
		var payload;
		if(status==OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess)
			if(onSucceeded)
				payload=onSucceeded(response,caller,callArgs);
			else
				payload=response;
		else if(onFailed)
			payload=onFailed(status,response);
		else
			payload=OSF.DDA.ErrorCodeManager.getErrorArgs(status);
		return payload
	};
	this.getCallArgs=function(suppliedArgs)
	{
		var options,
			parameterCallback;
		for(var i=suppliedArgs.length - 1; i >=requiredCount; i--)
		{
			var argument=suppliedArgs[i];
			switch(typeof argument)
			{
				case"object":
					options=argument;
					break;
				case"function":
					parameterCallback=argument;
					break
			}
		}
		options=options || {};
		if(parameterCallback)
			options[Microsoft.Office.WebExtension.Parameters.Callback]=parameterCallback;
		return options
	}
};
OSF.DDA.AsyncMethodCallFactory=function()
{
	function createObject(properties)
	{
		var obj=null;
		if(properties)
		{
			obj={};
			var len=properties.length;
			for(var i=0; i < len; i++)
				obj[properties[i].name]=properties[i].value
		}
		return obj
	}
	return{manufacture: function(params)
			{
				var supportedOptions=params.supportedOptions ? createObject(params.supportedOptions) : [];
				var privateStateCallbacks=params.privateStateCallbacks ? createObject(params.privateStateCallbacks) : [];
				return new OSF.DDA.AsyncMethodCall(params.requiredArguments || [],supportedOptions,privateStateCallbacks,params.onSucceeded,params.onFailed,params.checkCallArgs,params.method.displayName)
			}}
}();
OSF.DDA.AsyncMethodCalls={};
OSF.DDA.AsyncMethodCalls.define=function(callDefinition)
{
	OSF.DDA.AsyncMethodCalls[callDefinition.method.id]=OSF.DDA.AsyncMethodCallFactory.manufacture(callDefinition)
};
OSF.DDA.Error=function OSF_DDA_Error(name, message, code)
{
	OSF.OUtil.defineEnumerableProperties(this,{
		name: {value: name},
		message: {value: message},
		code: {value: code}
	})
};
OSF.DDA.AsyncResult=function OSF_DDA_AsyncResult(initArgs, errorArgs)
{
	OSF.OUtil.defineEnumerableProperties(this,{
		value: {value: initArgs[OSF.DDA.AsyncResultEnum.Properties.Value]},
		status: {value: errorArgs ? Microsoft.Office.WebExtension.AsyncResultStatus.Failed : Microsoft.Office.WebExtension.AsyncResultStatus.Succeeded}
	});
	if(initArgs[OSF.DDA.AsyncResultEnum.Properties.Context])
		OSF.OUtil.defineEnumerableProperty(this,"asyncContext",{value: initArgs[OSF.DDA.AsyncResultEnum.Properties.Context]});
	if(errorArgs)
		OSF.OUtil.defineEnumerableProperty(this,"error",{value: new OSF.DDA.Error(errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Name],errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Message],errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Code])})
};
OSF.DDA.issueAsyncResult=function OSF_DDA$IssueAsyncResult(callArgs, status, payload)
{
	var callback=callArgs[Microsoft.Office.WebExtension.Parameters.Callback];
	if(callback)
	{
		var asyncInitArgs={};
		asyncInitArgs[OSF.DDA.AsyncResultEnum.Properties.Context]=callArgs[Microsoft.Office.WebExtension.Parameters.AsyncContext];
		var errorArgs;
		if(status==OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess)
			asyncInitArgs[OSF.DDA.AsyncResultEnum.Properties.Value]=payload;
		else
		{
			errorArgs={};
			payload=payload || OSF.DDA.ErrorCodeManager.getErrorArgs(OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError);
			errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Code]=status || OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
			errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Name]=payload.name || payload;
			errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Message]=payload.message || payload
		}
		callback(new OSF.DDA.AsyncResult(asyncInitArgs,errorArgs))
	}
};
OSF.DDA.ListType=function()
{
	var listTypes={};
	return{
			setListType: function OSF_DDA_ListType$AddListType(t, prop)
			{
				listTypes[t]=prop
			},
			isListType: function OSF_DDA_ListType$IsListType(t)
			{
				return OSF.OUtil.listContainsKey(listTypes,t)
			},
			getDescriptor: function OSF_DDA_ListType$getDescriptor(t)
			{
				return listTypes[t]
			}
		}
}();
OSF.DDA.HostParameterMap=function(specialProcessor, mappings)
{
	var toHostMap="toHost";
	var fromHostMap="fromHost";
	var self="self";
	var dynamicTypes={};
	dynamicTypes[Microsoft.Office.WebExtension.Parameters.Data]={
		toHost: function(data)
		{
			if(data !=null && data.rows !==undefined)
			{
				var tableData={};
				tableData[OSF.DDA.TableDataProperties.TableRows]=data.rows;
				tableData[OSF.DDA.TableDataProperties.TableHeaders]=data.headers;
				data=tableData
			}
			return data
		},
		fromHost: function(args)
		{
			return args
		}
	};
	dynamicTypes[Microsoft.Office.WebExtension.Parameters.SampleData]=dynamicTypes[Microsoft.Office.WebExtension.Parameters.Data];
	function mapValues(preimageSet, mapping)
	{
		var ret=preimageSet ? {} : undefined;
		for(var entry in preimageSet)
		{
			var preimage=preimageSet[entry];
			var image;
			if(OSF.DDA.ListType.isListType(entry))
			{
				image=[];
				for(var subEntry in preimage)
					image.push(mapValues(preimage[subEntry],mapping))
			}
			else if(OSF.OUtil.listContainsKey(dynamicTypes,entry))
				image=dynamicTypes[entry][mapping](preimage);
			else if(mapping==fromHostMap && specialProcessor.preserveNesting(entry))
				image=mapValues(preimage,mapping);
			else
			{
				var maps=mappings[entry];
				if(maps)
				{
					var map=maps[mapping];
					if(map)
					{
						image=map[preimage];
						if(image===undefined)
							image=preimage
					}
				}
				else
					image=preimage
			}
			ret[entry]=image
		}
		return ret
	}
	function generateArguments(imageSet, parameters)
	{
		var ret;
		for(var param in parameters)
		{
			var arg;
			if(specialProcessor.isComplexType(param))
				arg=generateArguments(imageSet,mappings[param][toHostMap]);
			else
				arg=imageSet[param];
			if(arg !=undefined)
			{
				if(!ret)
					ret={};
				var index=parameters[param];
				if(index==self)
					index=param;
				ret[index]=specialProcessor.pack(param,arg)
			}
		}
		return ret
	}
	function extractArguments(source, parameters, extracted)
	{
		if(!extracted)
			extracted={};
		for(var param in parameters)
		{
			var index=parameters[param];
			var value;
			if(index==self)
				value=source;
			else
				value=source[index];
			if(value===null || value===undefined)
				extracted[param]=undefined;
			else
			{
				value=specialProcessor.unpack(param,value);
				var map;
				if(specialProcessor.isComplexType(param))
				{
					map=mappings[param][fromHostMap];
					if(specialProcessor.preserveNesting(param))
						extracted[param]=extractArguments(value,map);
					else
						extractArguments(value,map,extracted)
				}
				else
				{
					if(OSF.DDA.ListType.isListType(param))
					{
						map={};
						var entryDescriptor=OSF.DDA.ListType.getDescriptor(param);
						map[entryDescriptor]=self;
						for(var item in value)
							value[item]=extractArguments(value[item],map)
					}
					extracted[param]=value
				}
			}
		}
		return extracted
	}
	function applyMap(mapName, preimage, mapping)
	{
		var parameters=mappings[mapName][mapping];
		var image;
		if(mapping=="toHost")
		{
			var imageSet=mapValues(preimage,mapping);
			image=generateArguments(imageSet,parameters)
		}
		else if(mapping=="fromHost")
		{
			var argumentSet=extractArguments(preimage,parameters);
			image=mapValues(argumentSet,mapping)
		}
		return image
	}
	if(!mappings)
		mappings={};
	this.addMapping=function(mapName, description)
	{
		var toHost,
			fromHost;
		if(description.map)
		{
			toHost=description.map;
			fromHost={};
			for(var preimage in toHost)
			{
				var image=toHost[preimage];
				if(image==self)
					image=preimage;
				fromHost[image]=preimage
			}
		}
		else
		{
			toHost=description.toHost;
			fromHost=description.fromHost
		}
		var pair=mappings[mapName];
		if(pair)
		{
			var currMap=pair[toHostMap];
			for(var th in currMap)
				toHost[th]=currMap[th];
			currMap=pair[fromHostMap];
			for(var fh in currMap)
				fromHost[fh]=currMap[fh]
		}
		else
			pair=mappings[mapName]={};
		pair[toHostMap]=toHost;
		pair[fromHostMap]=fromHost
	};
	this.toHost=function(mapName, preimage)
	{
		return applyMap(mapName,preimage,toHostMap)
	};
	this.fromHost=function(mapName, image)
	{
		return applyMap(mapName,image,fromHostMap)
	};
	this.self=self;
	this.addComplexType=function(ct)
	{
		specialProcessor.addComplexType(ct)
	};
	this.getDynamicType=function(dt)
	{
		return specialProcessor.getDynamicType(dt)
	};
	this.setDynamicType=function(dt, handler)
	{
		specialProcessor.setDynamicType(dt,handler)
	};
	this.dynamicTypes=dynamicTypes;
	this.doMapValues=function(preimageSet, mapping)
	{
		return mapValues(preimageSet,mapping)
	}
};
OSF.DDA.SpecialProcessor=function(complexTypes, dynamicTypes)
{
	this.addComplexType=function OSF_DDA_SpecialProcessor$addComplexType(ct)
	{
		complexTypes.push(ct)
	};
	this.getDynamicType=function OSF_DDA_SpecialProcessor$getDynamicType(dt)
	{
		return dynamicTypes[dt]
	};
	this.setDynamicType=function OSF_DDA_SpecialProcessor$setDynamicType(dt, handler)
	{
		dynamicTypes[dt]=handler
	};
	this.isComplexType=function OSF_DDA_SpecialProcessor$isComplexType(t)
	{
		return OSF.OUtil.listContainsValue(complexTypes,t)
	};
	this.isDynamicType=function OSF_DDA_SpecialProcessor$isDynamicType(p)
	{
		return OSF.OUtil.listContainsKey(dynamicTypes,p)
	};
	this.preserveNesting=function OSF_DDA_SpecialProcessor$preserveNesting(p)
	{
		var pn=[];
		if(OSF.DDA.PropertyDescriptors)
			pn.push(OSF.DDA.PropertyDescriptors.Subset);
		if(OSF.DDA.DataNodeEventProperties)
			pn=pn.concat([OSF.DDA.DataNodeEventProperties.OldNode,OSF.DDA.DataNodeEventProperties.NewNode,OSF.DDA.DataNodeEventProperties.NextSiblingNode]);
		return OSF.OUtil.listContainsValue(pn,p)
	};
	this.pack=function OSF_DDA_SpecialProcessor$pack(param, arg)
	{
		var value;
		if(this.isDynamicType(param))
			value=dynamicTypes[param].toHost(arg);
		else
			value=arg;
		return value
	};
	this.unpack=function OSF_DDA_SpecialProcessor$unpack(param, arg)
	{
		var value;
		if(this.isDynamicType(param))
			value=dynamicTypes[param].fromHost(arg);
		else
			value=arg;
		return value
	}
};
OSF.DDA.getDecoratedParameterMap=function(specialProcessor, initialDefs)
{
	var parameterMap=new OSF.DDA.HostParameterMap(specialProcessor);
	var self=parameterMap.self;
	function createObject(properties)
	{
		var obj=null;
		if(properties)
		{
			obj={};
			var len=properties.length;
			for(var i=0; i < len; i++)
				obj[properties[i].name]=properties[i].value
		}
		return obj
	}
	parameterMap.define=function define(definition)
	{
		var args={};
		var toHost=createObject(definition.toHost);
		if(definition.invertible)
			args.map=toHost;
		else if(definition.canonical)
			args.toHost=args.fromHost=toHost;
		else
		{
			args.toHost=toHost;
			args.fromHost=createObject(definition.fromHost)
		}
		parameterMap.addMapping(definition.type,args);
		if(definition.isComplexType)
			parameterMap.addComplexType(definition.type)
	};
	for(var id in initialDefs)
		parameterMap.define(initialDefs[id]);
	return parameterMap
};
OSF.OUtil.setNamespace("DispIdHost",OSF.DDA);
OSF.DDA.DispIdHost.Methods={
	InvokeMethod: "invokeMethod",
	AddEventHandler: "addEventHandler",
	RemoveEventHandler: "removeEventHandler"
};
OSF.DDA.DispIdHost.Delegates={
	ExecuteAsync: "executeAsync",
	RegisterEventAsync: "registerEventAsync",
	UnregisterEventAsync: "unregisterEventAsync",
	ParameterMap: "parameterMap"
};
OSF.DDA.DispIdHost.Facade=function OSF_DDA_DispIdHost_Facade(getDelegateMethods, parameterMap)
{
	var dispIdMap={};
	var jsom=OSF.DDA.AsyncMethodNames;
	var did=OSF.DDA.MethodDispId;
	var methodMap={
			GoToByIdAsync: did.dispidNavigateToMethod,
			GetSelectedDataAsync: did.dispidGetSelectedDataMethod,
			SetSelectedDataAsync: did.dispidSetSelectedDataMethod,
			GetDocumentCopyChunkAsync: did.dispidGetDocumentCopyChunkMethod,
			ReleaseDocumentCopyAsync: did.dispidReleaseDocumentCopyMethod,
			GetDocumentCopyAsync: did.dispidGetDocumentCopyMethod,
			AddFromSelectionAsync: did.dispidAddBindingFromSelectionMethod,
			AddFromPromptAsync: did.dispidAddBindingFromPromptMethod,
			AddFromNamedItemAsync: did.dispidAddBindingFromNamedItemMethod,
			GetAllAsync: did.dispidGetAllBindingsMethod,
			GetByIdAsync: did.dispidGetBindingMethod,
			ReleaseByIdAsync: did.dispidReleaseBindingMethod,
			GetDataAsync: did.dispidGetBindingDataMethod,
			SetDataAsync: did.dispidSetBindingDataMethod,
			AddRowsAsync: did.dispidAddRowsMethod,
			AddColumnsAsync: did.dispidAddColumnsMethod,
			DeleteAllDataValuesAsync: did.dispidClearAllRowsMethod,
			RefreshAsync: did.dispidLoadSettingsMethod,
			SaveAsync: did.dispidSaveSettingsMethod,
			GetActiveViewAsync: did.dispidGetActiveViewMethod,
			GetFilePropertiesAsync: did.dispidGetFilePropertiesMethod,
			GetOfficeThemeAsync: did.dispidGetOfficeThemeMethod,
			GetDocumentThemeAsync: did.dispidGetDocumentThemeMethod,
			ClearFormatsAsync: did.dispidClearFormatsMethod,
			SetTableOptionsAsync: did.dispidSetTableOptionsMethod,
			SetFormatsAsync: did.dispidSetFormatsMethod,
			ExecuteRichApiRequestAsync: did.dispidExecuteRichApiRequestMethod,
			AppCommandInvocationCompletedAsync: did.dispidAppCommandInvocationCompletedMethod,
			AddDataPartAsync: did.dispidAddDataPartMethod,
			GetDataPartByIdAsync: did.dispidGetDataPartByIdMethod,
			GetDataPartsByNameSpaceAsync: did.dispidGetDataPartsByNamespaceMethod,
			GetPartXmlAsync: did.dispidGetDataPartXmlMethod,
			GetPartNodesAsync: did.dispidGetDataPartNodesMethod,
			DeleteDataPartAsync: did.dispidDeleteDataPartMethod,
			GetNodeValueAsync: did.dispidGetDataNodeValueMethod,
			GetNodeXmlAsync: did.dispidGetDataNodeXmlMethod,
			GetRelativeNodesAsync: did.dispidGetDataNodesMethod,
			SetNodeValueAsync: did.dispidSetDataNodeValueMethod,
			SetNodeXmlAsync: did.dispidSetDataNodeXmlMethod,
			AddDataPartNamespaceAsync: did.dispidAddDataNamespaceMethod,
			GetDataPartNamespaceAsync: did.dispidGetDataUriByPrefixMethod,
			GetDataPartPrefixAsync: did.dispidGetDataPrefixByUriMethod,
			GetSelectedTask: did.dispidGetSelectedTaskMethod,
			GetTask: did.dispidGetTaskMethod,
			GetWSSUrl: did.dispidGetWSSUrlMethod,
			GetTaskField: did.dispidGetTaskFieldMethod,
			GetSelectedResource: did.dispidGetSelectedResourceMethod,
			GetResourceField: did.dispidGetResourceFieldMethod,
			GetProjectField: did.dispidGetProjectFieldMethod,
			GetSelectedView: did.dispidGetSelectedViewMethod,
			GetTaskByIndex: did.dispidGetTaskByIndexMethod,
			GetResourceByIndex: did.dispidGetResourceByIndexMethod,
			SetTaskField: did.dispidSetTaskFieldMethod,
			SetResourceField: did.dispidSetResourceFieldMethod,
			GetMaxTaskIndex: did.dispidGetMaxTaskIndexMethod,
			GetMaxResourceIndex: did.dispidGetMaxResourceIndexMethod
		};
	for(var method in methodMap)
		if(jsom[method])
			dispIdMap[jsom[method].id]=methodMap[method];
	jsom=Microsoft.Office.WebExtension.EventType;
	did=OSF.DDA.EventDispId;
	var eventMap={
			SettingsChanged: did.dispidSettingsChangedEvent,
			DocumentSelectionChanged: did.dispidDocumentSelectionChangedEvent,
			BindingSelectionChanged: did.dispidBindingSelectionChangedEvent,
			BindingDataChanged: did.dispidBindingDataChangedEvent,
			ActiveViewChanged: did.dispidActiveViewChangedEvent,
			OfficeThemeChanged: did.dispidOfficeThemeChangedEvent,
			DocumentThemeChanged: did.dispidDocumentThemeChangedEvent,
			AppCommandInvoked: did.dispidAppCommandInvokedEvent,
			TaskSelectionChanged: did.dispidTaskSelectionChangedEvent,
			ResourceSelectionChanged: did.dispidResourceSelectionChangedEvent,
			ViewSelectionChanged: did.dispidViewSelectionChangedEvent,
			DataNodeInserted: did.dispidDataNodeAddedEvent,
			DataNodeReplaced: did.dispidDataNodeReplacedEvent,
			DataNodeDeleted: did.dispidDataNodeDeletedEvent
		};
	for(var event in eventMap)
		if(jsom[event])
			dispIdMap[jsom[event]]=eventMap[event];
	function onException(ex, asyncMethodCall, suppliedArgs, callArgs)
	{
		if(typeof ex=="number")
		{
			if(!callArgs)
				callArgs=asyncMethodCall.getCallArgs(suppliedArgs);
			OSF.DDA.issueAsyncResult(callArgs,ex,OSF.DDA.ErrorCodeManager.getErrorArgs(ex))
		}
		else
			throw ex;
	}
	this[OSF.DDA.DispIdHost.Methods.InvokeMethod]=function OSF_DDA_DispIdHost_Facade$InvokeMethod(method, suppliedArguments, caller, privateState)
	{
		var callArgs;
		try
		{
			var methodName=method.id;
			var asyncMethodCall=OSF.DDA.AsyncMethodCalls[methodName];
			callArgs=asyncMethodCall.verifyAndExtractCall(suppliedArguments,caller,privateState);
			var dispId=dispIdMap[methodName];
			var delegate=getDelegateMethods(methodName);
			var hostCallArgs;
			if(parameterMap.toHost)
				hostCallArgs=parameterMap.toHost(dispId,callArgs);
			else
				hostCallArgs=callArgs;
			delegate[OSF.DDA.DispIdHost.Delegates.ExecuteAsync]({
				dispId: dispId,
				hostCallArgs: hostCallArgs,
				onCalling: function OSF_DDA_DispIdFacade$Execute_onCalling()
				{
					OSF.OUtil.writeProfilerMark(OSF.HostCallPerfMarker.IssueCall)
				},
				onReceiving: function OSF_DDA_DispIdFacade$Execute_onReceiving()
				{
					OSF.OUtil.writeProfilerMark(OSF.HostCallPerfMarker.ReceiveResponse)
				},
				onComplete: function(status, hostResponseArgs)
				{
					var responseArgs;
					if(status==OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess)
						if(parameterMap.fromHost)
							responseArgs=parameterMap.fromHost(dispId,hostResponseArgs);
						else
							responseArgs=hostResponseArgs;
					else
						responseArgs=hostResponseArgs;
					var payload=asyncMethodCall.processResponse(status,responseArgs,caller,callArgs);
					OSF.DDA.issueAsyncResult(callArgs,status,payload)
				}
			})
		}
		catch(ex)
		{
			onException(ex,asyncMethodCall,suppliedArguments,callArgs)
		}
	};
	this[OSF.DDA.DispIdHost.Methods.AddEventHandler]=function OSF_DDA_DispIdHost_Facade$AddEventHandler(suppliedArguments, eventDispatch, caller)
	{
		var callArgs;
		var eventType,
			handler;
		function onEnsureRegistration(status)
		{
			if(status==OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess)
			{
				var added=eventDispatch.addEventHandler(eventType,handler);
				if(!added)
					status=OSF.DDA.ErrorCodeManager.errorCodes.ooeEventHandlerAdditionFailed
			}
			var error;
			if(status !=OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess)
				error=OSF.DDA.ErrorCodeManager.getErrorArgs(status);
			OSF.DDA.issueAsyncResult(callArgs,status,error)
		}
		try
		{
			var asyncMethodCall=OSF.DDA.AsyncMethodCalls[OSF.DDA.AsyncMethodNames.AddHandlerAsync.id];
			callArgs=asyncMethodCall.verifyAndExtractCall(suppliedArguments,caller,eventDispatch);
			eventType=callArgs[Microsoft.Office.WebExtension.Parameters.EventType];
			handler=callArgs[Microsoft.Office.WebExtension.Parameters.Handler];
			if(eventDispatch.getEventHandlerCount(eventType)==0)
			{
				var dispId=dispIdMap[eventType];
				var invoker=getDelegateMethods(eventType)[OSF.DDA.DispIdHost.Delegates.RegisterEventAsync];
				invoker({
					eventType: eventType,
					dispId: dispId,
					targetId: caller.id || "",
					onCalling: function OSF_DDA_DispIdFacade$Execute_onCalling()
					{
						OSF.OUtil.writeProfilerMark(OSF.HostCallPerfMarker.IssueCall)
					},
					onReceiving: function OSF_DDA_DispIdFacade$Execute_onReceiving()
					{
						OSF.OUtil.writeProfilerMark(OSF.HostCallPerfMarker.ReceiveResponse)
					},
					onComplete: onEnsureRegistration,
					onEvent: function handleEvent(hostArgs)
					{
						var args=parameterMap.fromHost(dispId,hostArgs);
						eventDispatch.fireEvent(OSF.DDA.OMFactory.manufactureEventArgs(eventType,caller,args))
					}
				})
			}
			else
				onEnsureRegistration(OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess)
		}
		catch(ex)
		{
			onException(ex,asyncMethodCall,suppliedArguments,callArgs)
		}
	};
	this[OSF.DDA.DispIdHost.Methods.RemoveEventHandler]=function OSF_DDA_DispIdHost_Facade$RemoveEventHandler(suppliedArguments, eventDispatch, caller)
	{
		var callArgs;
		var eventType,
			handler;
		function onEnsureRegistration(status)
		{
			var error;
			if(status !=OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess)
				error=OSF.DDA.ErrorCodeManager.getErrorArgs(status);
			OSF.DDA.issueAsyncResult(callArgs,status,error)
		}
		try
		{
			var asyncMethodCall=OSF.DDA.AsyncMethodCalls[OSF.DDA.AsyncMethodNames.RemoveHandlerAsync.id];
			callArgs=asyncMethodCall.verifyAndExtractCall(suppliedArguments,caller,eventDispatch);
			eventType=callArgs[Microsoft.Office.WebExtension.Parameters.EventType];
			handler=callArgs[Microsoft.Office.WebExtension.Parameters.Handler];
			var status,
				removeSuccess;
			if(handler===null)
			{
				removeSuccess=eventDispatch.clearEventHandlers(eventType);
				status=OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess
			}
			else
			{
				removeSuccess=eventDispatch.removeEventHandler(eventType,handler);
				status=removeSuccess ? OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess : OSF.DDA.ErrorCodeManager.errorCodes.ooeEventHandlerNotExist
			}
			if(removeSuccess && eventDispatch.getEventHandlerCount(eventType)==0)
			{
				var dispId=dispIdMap[eventType];
				var invoker=getDelegateMethods(eventType)[OSF.DDA.DispIdHost.Delegates.UnregisterEventAsync];
				invoker({
					eventType: eventType,
					dispId: dispId,
					targetId: caller.id || "",
					onCalling: function OSF_DDA_DispIdFacade$Execute_onCalling()
					{
						OSF.OUtil.writeProfilerMark(OSF.HostCallPerfMarker.IssueCall)
					},
					onReceiving: function OSF_DDA_DispIdFacade$Execute_onReceiving()
					{
						OSF.OUtil.writeProfilerMark(OSF.HostCallPerfMarker.ReceiveResponse)
					},
					onComplete: onEnsureRegistration
				})
			}
			else
				onEnsureRegistration(status)
		}
		catch(ex)
		{
			onException(ex,asyncMethodCall,suppliedArguments,callArgs)
		}
	}
};
OSF.DDA.DispIdHost.addAsyncMethods=function OSF_DDA_DispIdHost$AddAsyncMethods(target, asyncMethodNames, privateState)
{
	for(var entry in asyncMethodNames)
	{
		var method=asyncMethodNames[entry];
		var name=method.displayName;
		if(!target[name])
			OSF.OUtil.defineEnumerableProperty(target,name,{value: function(asyncMethod)
				{
					return function()
						{
							var invokeMethod=OSF._OfficeAppFactory.getHostFacade()[OSF.DDA.DispIdHost.Methods.InvokeMethod];
							invokeMethod(asyncMethod,arguments,target,privateState)
						}
				}(method)})
	}
};
OSF.DDA.DispIdHost.addEventSupport=function OSF_DDA_DispIdHost$AddEventSupport(target, eventDispatch)
{
	var add=OSF.DDA.AsyncMethodNames.AddHandlerAsync.displayName;
	var remove=OSF.DDA.AsyncMethodNames.RemoveHandlerAsync.displayName;
	if(!target[add])
		OSF.OUtil.defineEnumerableProperty(target,add,{value: function()
			{
				var addEventHandler=OSF._OfficeAppFactory.getHostFacade()[OSF.DDA.DispIdHost.Methods.AddEventHandler];
				addEventHandler(arguments,eventDispatch,target)
			}});
	if(!target[remove])
		OSF.OUtil.defineEnumerableProperty(target,remove,{value: function()
			{
				var removeEventHandler=OSF._OfficeAppFactory.getHostFacade()[OSF.DDA.DispIdHost.Methods.RemoveEventHandler];
				removeEventHandler(arguments,eventDispatch,target)
			}})
};
OSF.OUtil.setNamespace("Microsoft",window);
OSF.OUtil.setNamespace("Office",Microsoft);
OSF.OUtil.setNamespace("Common",Microsoft.Office);
(function(window)
{
	"use strict";
	var stringRegEx=new RegExp('"(\\\\.|[^"\\\\])*"',"g"),
		trueFalseNullRegEx=new RegExp("\\b(true|false|null)\\b","g"),
		numbersRegEx=new RegExp("-?(0|([1-9]\\d*))(\\.\\d+)?([eE][+-]?\\d+)?","g"),
		badBracketsRegEx=new RegExp("[^{:,\\[\\s](?=\\s*\\[)"),
		badRemainderRegEx=new RegExp("[^\\s\\[\\]{}:,]"),
		jsonErrorMsg="Cannot deserialize. The data does not correspond to valid JSON.";
	function addHandler(element, eventName, handler)
	{
		if(element.addEventListener)
			element.addEventListener(eventName,handler,false);
		else if(element.attachEvent)
			element.attachEvent("on"+eventName,handler)
	}
	function getAjaxSerializer()
	{
		if(typeof Sys !=="undefined" && typeof Sys.Serialization !=="undefined" && typeof Sys.Serialization.JavaScriptSerializer !=="undefined")
			return Sys.Serialization.JavaScriptSerializer;
		return null
	}
	function deserialize(data, secure, oldDeserialize)
	{
		var transformed;
		if(!secure)
			return oldDeserialize(data);
		if(window.JSON && window.JSON.parse)
			return window.JSON.parse(data);
		transformed=data.replace(stringRegEx,"[]");
		transformed=transformed.replace(trueFalseNullRegEx,"[]");
		transformed=transformed.replace(numbersRegEx,"[]");
		if(badBracketsRegEx.test(transformed))
			throw jsonErrorMsg;
		if(badRemainderRegEx.test(transformed))
			throw jsonErrorMsg;
		try
		{
			eval("("+data+")")
		}
		catch(e)
		{
			throw jsonErrorMsg;
		}
	}
	function patchDeserializer()
	{
		var serializer=getAjaxSerializer(),
			oldDeserialize;
		if(serializer===null || typeof serializer.deserialize !=="function")
			return false;
		if(serializer.__patchVersion >=1)
			return true;
		oldDeserialize=serializer.deserialize;
		serializer.deserialize=function(data, secure)
		{
			return deserialize(data,true,oldDeserialize)
		};
		serializer.__patchVersion=1;
		return true
	}
	if(!patchDeserializer())
		addHandler(window,"load",function()
		{
			patchDeserializer()
		})
})(window);
Microsoft.Office.Common.InvokeType={
	async: 0,
	sync: 1,
	asyncRegisterEvent: 2,
	asyncUnregisterEvent: 3,
	syncRegisterEvent: 4,
	syncUnregisterEvent: 5
};
Microsoft.Office.Common.InvokeResultCode={
	noError: 0,
	errorInRequest: -1,
	errorHandlingRequest: -2,
	errorInResponse: -3,
	errorHandlingResponse: -4,
	errorHandlingRequestAccessDenied: -5,
	errorHandlingMethodCallTimedout: -6
};
Microsoft.Office.Common.MessageType={
	request: 0,
	response: 1
};
Microsoft.Office.Common.ActionType={
	invoke: 0,
	registerEvent: 1,
	unregisterEvent: 2
};
Microsoft.Office.Common.ResponseType={
	forCalling: 0,
	forEventing: 1
};
Microsoft.Office.Common.MethodObject=function Microsoft_Office_Common_MethodObject(method, invokeType, blockingOthers)
{
	this._method=method;
	this._invokeType=invokeType;
	this._blockingOthers=blockingOthers
};
Microsoft.Office.Common.MethodObject.prototype={
	getMethod: function Microsoft_Office_Common_MethodObject$getMethod()
	{
		return this._method
	},
	getInvokeType: function Microsoft_Office_Common_MethodObject$getInvokeType()
	{
		return this._invokeType
	},
	getBlockingFlag: function Microsoft_Office_Common_MethodObject$getBlockingFlag()
	{
		return this._blockingOthers
	}
};
Microsoft.Office.Common.EventMethodObject=function Microsoft_Office_Common_EventMethodObject(registerMethodObject, unregisterMethodObject)
{
	this._registerMethodObject=registerMethodObject;
	this._unregisterMethodObject=unregisterMethodObject
};
Microsoft.Office.Common.EventMethodObject.prototype={
	getRegisterMethodObject: function Microsoft_Office_Common_EventMethodObject$getRegisterMethodObject()
	{
		return this._registerMethodObject
	},
	getUnregisterMethodObject: function Microsoft_Office_Common_EventMethodObject$getUnregisterMethodObject()
	{
		return this._unregisterMethodObject
	}
};
Microsoft.Office.Common.ServiceEndPoint=function Microsoft_Office_Common_ServiceEndPoint(serviceEndPointId)
{
	var e=Function._validateParams(arguments,[{
				name: "serviceEndPointId",
				type: String,
				mayBeNull: false
			}]);
	if(e)
		throw e;
	this._methodObjectList={};
	this._eventHandlerProxyList={};
	this._Id=serviceEndPointId;
	this._conversations={};
	this._policyManager=null
};
Microsoft.Office.Common.ServiceEndPoint.prototype={
	registerMethod: function Microsoft_Office_Common_ServiceEndPoint$registerMethod(methodName, method, invokeType, blockingOthers)
	{
		var e=Function._validateParams(arguments,[{
					name: "methodName",
					type: String,
					mayBeNull: false
				},{
					name: "method",
					type: Function,
					mayBeNull: false
				},{
					name: "invokeType",
					type: Number,
					mayBeNull: false
				},{
					name: "blockingOthers",
					type: Boolean,
					mayBeNull: false
				}]);
		if(e)
			throw e;
		if(invokeType !==Microsoft.Office.Common.InvokeType.async && invokeType !==Microsoft.Office.Common.InvokeType.sync)
			throw Error.argument("invokeType");
		var methodObject=new Microsoft.Office.Common.MethodObject(method,invokeType,blockingOthers);
		this._methodObjectList[methodName]=methodObject
	},
	unregisterMethod: function Microsoft_Office_Common_ServiceEndPoint$unregisterMethod(methodName)
	{
		var e=Function._validateParams(arguments,[{
					name: "methodName",
					type: String,
					mayBeNull: false
				}]);
		if(e)
			throw e;
		delete this._methodObjectList[methodName]
	},
	registerEvent: function Microsoft_Office_Common_ServiceEndPoint$registerEvent(eventName, registerMethod, unregisterMethod)
	{
		var e=Function._validateParams(arguments,[{
					name: "eventName",
					type: String,
					mayBeNull: false
				},{
					name: "registerMethod",
					type: Function,
					mayBeNull: false
				},{
					name: "unregisterMethod",
					type: Function,
					mayBeNull: false
				}]);
		if(e)
			throw e;
		var methodObject=new Microsoft.Office.Common.EventMethodObject(new Microsoft.Office.Common.MethodObject(registerMethod,Microsoft.Office.Common.InvokeType.syncRegisterEvent,false),new Microsoft.Office.Common.MethodObject(unregisterMethod,Microsoft.Office.Common.InvokeType.syncUnregisterEvent,false));
		this._methodObjectList[eventName]=methodObject
	},
	registerEventEx: function Microsoft_Office_Common_ServiceEndPoint$registerEventEx(eventName, registerMethod, registerMethodInvokeType, unregisterMethod, unregisterMethodInvokeType)
	{
		var e=Function._validateParams(arguments,[{
					name: "eventName",
					type: String,
					mayBeNull: false
				},{
					name: "registerMethod",
					type: Function,
					mayBeNull: false
				},{
					name: "registerMethodInvokeType",
					type: Number,
					mayBeNull: false
				},{
					name: "unregisterMethod",
					type: Function,
					mayBeNull: false
				},{
					name: "unregisterMethodInvokeType",
					type: Number,
					mayBeNull: false
				}]);
		if(e)
			throw e;
		var methodObject=new Microsoft.Office.Common.EventMethodObject(new Microsoft.Office.Common.MethodObject(registerMethod,registerMethodInvokeType,false),new Microsoft.Office.Common.MethodObject(unregisterMethod,unregisterMethodInvokeType,false));
		this._methodObjectList[eventName]=methodObject
	},
	unregisterEvent: function(eventName)
	{
		var e=Function._validateParams(arguments,[{
					name: "eventName",
					type: String,
					mayBeNull: false
				}]);
		if(e)
			throw e;
		this.unregisterMethod(eventName)
	},
	registerConversation: function Microsoft_Office_Common_ServiceEndPoint$registerConversation(conversationId, conversationUrl)
	{
		var e=Function._validateParams(arguments,[{
					name: "conversationId",
					type: String,
					mayBeNull: false
				},{
					name: "conversationUrl",
					type: String,
					mayBeNull: false,
					optional: true
				}]);
		if(e)
			throw e;
		if(conversationUrl)
			this._conversations[conversationId]=conversationUrl;
		else
			this._conversations[conversationId]=true
	},
	unregisterConversation: function Microsoft_Office_Common_ServiceEndPoint$unregisterConversation(conversationId)
	{
		var e=Function._validateParams(arguments,[{
					name: "conversationId",
					type: String,
					mayBeNull: false
				}]);
		if(e)
			throw e;
		delete this._conversations[conversationId]
	},
	setPolicyManager: function Microsoft_Office_Common_ServiceEndPoint$setPolicyManager(policyManager)
	{
		var e=Function._validateParams(arguments,[{
					name: "policyManager",
					type: Object,
					mayBeNull: false
				}]);
		if(e)
			throw e;
		if(!policyManager.checkPermission)
			throw Error.argument("policyManager");
		this._policyManager=policyManager
	},
	getPolicyManager: function Microsoft_Office_Common_ServiceEndPoint$getPolicyManager()
	{
		return this._policyManager
	}
};
Microsoft.Office.Common.ClientEndPoint=function Microsoft_Office_Common_ClientEndPoint(conversationId, targetWindow, targetUrl)
{
	var e=Function._validateParams(arguments,[{
				name: "conversationId",
				type: String,
				mayBeNull: false
			},{
				name: "targetWindow",
				mayBeNull: false
			},{
				name: "targetUrl",
				type: String,
				mayBeNull: false
			}]);
	if(e)
		throw e;
	if(!targetWindow.postMessage)
		throw Error.argument("targetWindow");
	this._conversationId=conversationId;
	this._targetWindow=targetWindow;
	this._targetUrl=targetUrl;
	this._callingIndex=0;
	this._callbackList={};
	this._eventHandlerList={}
};
Microsoft.Office.Common.ClientEndPoint.prototype={
	invoke: function Microsoft_Office_Common_ClientEndPoint$invoke(targetMethodName, callback, param)
	{
		var e=Function._validateParams(arguments,[{
					name: "targetMethodName",
					type: String,
					mayBeNull: false
				},{
					name: "callback",
					type: Function,
					mayBeNull: true
				},{
					name: "param",
					mayBeNull: true
				}]);
		if(e)
			throw e;
		var correlationId=this._callingIndex++;
		var now=new Date;
		var callbackEntry={
				callback: callback,
				createdOn: now.getTime()
			};
		if(param && typeof param==="object" && typeof param.__timeout__==="number")
		{
			callbackEntry.timeout=param.__timeout__;
			delete param.__timeout__
		}
		this._callbackList[correlationId]=callbackEntry;
		try
		{
			var callRequest=new Microsoft.Office.Common.Request(targetMethodName,Microsoft.Office.Common.ActionType.invoke,this._conversationId,correlationId,param);
			var msg=Microsoft.Office.Common.MessagePackager.envelope(callRequest);
			this._targetWindow.postMessage(msg,this._targetUrl);
			Microsoft.Office.Common.XdmCommunicationManager._startMethodTimeoutTimer()
		}
		catch(ex)
		{
			try
			{
				if(callback !==null)
					callback(Microsoft.Office.Common.InvokeResultCode.errorInRequest,ex)
			}
			finally
			{
				delete this._callbackList[correlationId]
			}
		}
	},
	registerForEvent: function Microsoft_Office_Common_ClientEndPoint$registerForEvent(targetEventName, eventHandler, callback, data)
	{
		var e=Function._validateParams(arguments,[{
					name: "targetEventName",
					type: String,
					mayBeNull: false
				},{
					name: "eventHandler",
					type: Function,
					mayBeNull: false
				},{
					name: "callback",
					type: Function,
					mayBeNull: true
				},{
					name: "data",
					mayBeNull: true,
					optional: true
				}]);
		if(e)
			throw e;
		var correlationId=this._callingIndex++;
		var now=new Date;
		this._callbackList[correlationId]={
			callback: callback,
			createdOn: now.getTime()
		};
		try
		{
			var callRequest=new Microsoft.Office.Common.Request(targetEventName,Microsoft.Office.Common.ActionType.registerEvent,this._conversationId,correlationId,data);
			var msg=Microsoft.Office.Common.MessagePackager.envelope(callRequest);
			this._targetWindow.postMessage(msg,this._targetUrl);
			Microsoft.Office.Common.XdmCommunicationManager._startMethodTimeoutTimer();
			this._eventHandlerList[targetEventName]=eventHandler
		}
		catch(ex)
		{
			try
			{
				if(callback !==null)
					callback(Microsoft.Office.Common.InvokeResultCode.errorInRequest,ex)
			}
			finally
			{
				delete this._callbackList[correlationId]
			}
		}
	},
	unregisterForEvent: function Microsoft_Office_Common_ClientEndPoint$unregisterForEvent(targetEventName, callback, data)
	{
		var e=Function._validateParams(arguments,[{
					name: "targetEventName",
					type: String,
					mayBeNull: false
				},{
					name: "callback",
					type: Function,
					mayBeNull: true
				},{
					name: "data",
					mayBeNull: true,
					optional: true
				}]);
		if(e)
			throw e;
		var correlationId=this._callingIndex++;
		var now=new Date;
		this._callbackList[correlationId]={
			callback: callback,
			createdOn: now.getTime()
		};
		try
		{
			var callRequest=new Microsoft.Office.Common.Request(targetEventName,Microsoft.Office.Common.ActionType.unregisterEvent,this._conversationId,correlationId,data);
			var msg=Microsoft.Office.Common.MessagePackager.envelope(callRequest);
			this._targetWindow.postMessage(msg,this._targetUrl);
			Microsoft.Office.Common.XdmCommunicationManager._startMethodTimeoutTimer()
		}
		catch(ex)
		{
			try
			{
				if(callback !==null)
					callback(Microsoft.Office.Common.InvokeResultCode.errorInRequest,ex)
			}
			finally
			{
				delete this._callbackList[correlationId]
			}
		}
		finally
		{
			delete this._eventHandlerList[targetEventName]
		}
	}
};
Microsoft.Office.Common.XdmCommunicationManager=function()
{
	var _invokerQueue=[];
	var _messageProcessingTimer=null;
	var _processInterval=10;
	var _blockingFlag=false;
	var _methodTimeoutTimer=null;
	var _methodTimeoutProcessInterval=2e3;
	var _methodTimeoutDefault=65e3;
	var _methodTimeout=_methodTimeoutDefault;
	var _serviceEndPoints={};
	var _clientEndPoints={};
	var _initialized=false;
	function _lookupServiceEndPoint(conversationId)
	{
		for(var id in _serviceEndPoints)
			if(_serviceEndPoints[id]._conversations[conversationId])
				return _serviceEndPoints[id];
		Sys.Debug.trace("Unknown conversation Id.");
		throw Error.argument("conversationId");
	}
	function _lookupClientEndPoint(conversationId)
	{
		var clientEndPoint=_clientEndPoints[conversationId];
		if(!clientEndPoint)
		{
			Sys.Debug.trace("Unknown conversation Id.");
			throw Error.argument("conversationId");
		}
		return clientEndPoint
	}
	function _lookupMethodObject(serviceEndPoint, messageObject)
	{
		var methodOrEventMethodObject=serviceEndPoint._methodObjectList[messageObject._actionName];
		if(!methodOrEventMethodObject)
		{
			Sys.Debug.trace("The specified method is not registered on service endpoint:"+messageObject._actionName);
			throw Error.argument("messageObject");
		}
		var methodObject=null;
		if(messageObject._actionType===Microsoft.Office.Common.ActionType.invoke)
			methodObject=methodOrEventMethodObject;
		else if(messageObject._actionType===Microsoft.Office.Common.ActionType.registerEvent)
			methodObject=methodOrEventMethodObject.getRegisterMethodObject();
		else
			methodObject=methodOrEventMethodObject.getUnregisterMethodObject();
		return methodObject
	}
	function _enqueInvoker(invoker)
	{
		_invokerQueue.push(invoker)
	}
	function _dequeInvoker()
	{
		if(_messageProcessingTimer !==null)
		{
			if(!_blockingFlag)
				if(_invokerQueue.length > 0)
				{
					var invoker=_invokerQueue.shift();
					_blockingFlag=invoker.getInvokeBlockingFlag();
					invoker.invoke()
				}
				else
				{
					clearInterval(_messageProcessingTimer);
					_messageProcessingTimer=null
				}
		}
		else
			Sys.Debug.trace("channel is not ready.")
	}
	function _checkMethodTimeout()
	{
		if(_methodTimeoutTimer)
		{
			var clientEndPoint;
			var methodCallsNotTimedout=0;
			var now=new Date;
			var timeoutValue;
			for(var conversationId in _clientEndPoints)
			{
				clientEndPoint=_clientEndPoints[conversationId];
				for(var correlationId in clientEndPoint._callbackList)
				{
					var callbackEntry=clientEndPoint._callbackList[correlationId];
					timeoutValue=callbackEntry.timeout ? callbackEntry.timeout : _methodTimeout;
					if(timeoutValue >=0 && Math.abs(now.getTime() - callbackEntry.createdOn) >=timeoutValue)
						try
						{
							if(callbackEntry.callback)
								callbackEntry.callback(Microsoft.Office.Common.InvokeResultCode.errorHandlingMethodCallTimedout,null)
						}
						finally
						{
							delete clientEndPoint._callbackList[correlationId]
						}
					else
						methodCallsNotTimedout++				}
			}
			if(methodCallsNotTimedout===0)
			{
				clearInterval(_methodTimeoutTimer);
				_methodTimeoutTimer=null
			}
		}
		else
			Sys.Debug.trace("channel is not ready.")
	}
	function _postCallbackHandler()
	{
		_blockingFlag=false
	}
	function _registerListener(listener)
	{
		if(window.addEventListener)
			window.addEventListener("message",listener,false);
		else if(Sys.Browser.agent===Sys.Browser.InternetExplorer && window.attachEvent)
			window.attachEvent("onmessage",listener);
		else
		{
			Sys.Debug.trace("Browser doesn't support the required API.");
			throw Error.argument("Browser");
		}
	}
	function _checkOrigin(url, origin)
	{
		var res=false;
		if(url===true)
			return true;
		if(!url || !origin || !url.length || !origin.length)
			return res;
		var url_parser,
			org_parser;
		url_parser=document.createElement("a");
		org_parser=document.createElement("a");
		url_parser.href=url;
		org_parser.href=origin;
		res=url_parser.hostname==org_parser.hostname && url_parser.protocol==org_parser.protocol && url_parser.port==org_parser.port;
		delete url_parser,org_parser;
		return res
	}
	function _receive(e)
	{
		if(e.data !="")
		{
			var messageObject;
			try
			{
				messageObject=Microsoft.Office.Common.MessagePackager.unenvelope(e.data)
			}
			catch(ex)
			{
				return
			}
			if(typeof messageObject._messageType=="undefined")
				return;
			if(messageObject._messageType===Microsoft.Office.Common.MessageType.request)
			{
				var requesterUrl=e.origin==null || e.origin=="null" ? messageObject._origin : e.origin;
				try
				{
					var serviceEndPoint=_lookupServiceEndPoint(messageObject._conversationId);
					if(!_checkOrigin(serviceEndPoint._conversations[messageObject._conversationId],e.origin))
						throw"Failed origin check";
					var policyManager=serviceEndPoint.getPolicyManager();
					if(policyManager && !policyManager.checkPermission(messageObject._conversationId,messageObject._actionName,messageObject._data))
						throw"Access Denied";
					var methodObject=_lookupMethodObject(serviceEndPoint,messageObject);
					var invokeCompleteCallback=new Microsoft.Office.Common.InvokeCompleteCallback(e.source,requesterUrl,messageObject._actionName,messageObject._conversationId,messageObject._correlationId,_postCallbackHandler);
					var invoker=new Microsoft.Office.Common.Invoker(methodObject,messageObject._data,invokeCompleteCallback,serviceEndPoint._eventHandlerProxyList,messageObject._conversationId,messageObject._actionName);
					if(_messageProcessingTimer==null)
						_messageProcessingTimer=setInterval(_dequeInvoker,_processInterval);
					_enqueInvoker(invoker)
				}
				catch(ex)
				{
					var errorCode=Microsoft.Office.Common.InvokeResultCode.errorHandlingRequest;
					if(ex=="Access Denied")
						errorCode=Microsoft.Office.Common.InvokeResultCode.errorHandlingRequestAccessDenied;
					var callResponse=new Microsoft.Office.Common.Response(messageObject._actionName,messageObject._conversationId,messageObject._correlationId,errorCode,Microsoft.Office.Common.ResponseType.forCalling,ex);
					var envelopedResult=Microsoft.Office.Common.MessagePackager.envelope(callResponse);
					if(e.source && e.source.postMessage)
						e.source.postMessage(envelopedResult,requesterUrl)
				}
			}
			else if(messageObject._messageType===Microsoft.Office.Common.MessageType.response)
			{
				var clientEndPoint=_lookupClientEndPoint(messageObject._conversationId);
				if(!_checkOrigin(clientEndPoint._targetUrl,e.origin))
					throw"Failed orgin check";
				if(messageObject._responseType===Microsoft.Office.Common.ResponseType.forCalling)
				{
					var callbackEntry=clientEndPoint._callbackList[messageObject._correlationId];
					if(callbackEntry)
						try
						{
							if(callbackEntry.callback)
								callbackEntry.callback(messageObject._errorCode,messageObject._data)
						}
						finally
						{
							delete clientEndPoint._callbackList[messageObject._correlationId]
						}
				}
				else
				{
					var eventhandler=clientEndPoint._eventHandlerList[messageObject._actionName];
					if(eventhandler !==undefined && eventhandler !==null)
						eventhandler(messageObject._data)
				}
			}
			else
				return
		}
	}
	function _initialize()
	{
		if(!_initialized)
		{
			_registerListener(_receive);
			_initialized=true
		}
	}
	return{
			connect: function Microsoft_Office_Common_XdmCommunicationManager$connect(conversationId, targetWindow, targetUrl)
			{
				var clientEndPoint=_clientEndPoints[conversationId];
				if(!clientEndPoint)
				{
					_initialize();
					clientEndPoint=new Microsoft.Office.Common.ClientEndPoint(conversationId,targetWindow,targetUrl);
					_clientEndPoints[conversationId]=clientEndPoint
				}
				return clientEndPoint
			},
			getClientEndPoint: function Microsoft_Office_Common_XdmCommunicationManager$getClientEndPoint(conversationId)
			{
				var e=Function._validateParams(arguments,[{
							name: "conversationId",
							type: String,
							mayBeNull: false
						}]);
				if(e)
					throw e;
				return _clientEndPoints[conversationId]
			},
			createServiceEndPoint: function Microsoft_Office_Common_XdmCommunicationManager$createServiceEndPoint(serviceEndPointId)
			{
				_initialize();
				var serviceEndPoint=new Microsoft.Office.Common.ServiceEndPoint(serviceEndPointId);
				_serviceEndPoints[serviceEndPointId]=serviceEndPoint;
				return serviceEndPoint
			},
			getServiceEndPoint: function Microsoft_Office_Common_XdmCommunicationManager$getServiceEndPoint(serviceEndPointId)
			{
				var e=Function._validateParams(arguments,[{
							name: "serviceEndPointId",
							type: String,
							mayBeNull: false
						}]);
				if(e)
					throw e;
				return _serviceEndPoints[serviceEndPointId]
			},
			deleteClientEndPoint: function Microsoft_Office_Common_XdmCommunicationManager$deleteClientEndPoint(conversationId)
			{
				var e=Function._validateParams(arguments,[{
							name: "conversationId",
							type: String,
							mayBeNull: false
						}]);
				if(e)
					throw e;
				delete _clientEndPoints[conversationId]
			},
			_setMethodTimeout: function Microsoft_Office_Common_XdmCommunicationManager$_setMethodTimeout(methodTimeout)
			{
				var e=Function._validateParams(arguments,[{
							name: "methodTimeout",
							type: Number,
							mayBeNull: false
						}]);
				if(e)
					throw e;
				_methodTimeout=methodTimeout <=0 ? _methodTimeoutDefault : methodTimeout
			},
			_startMethodTimeoutTimer: function Microsoft_Office_Common_XdmCommunicationManager$_startMethodTimeoutTimer()
			{
				if(!_methodTimeoutTimer)
					_methodTimeoutTimer=setInterval(_checkMethodTimeout,_methodTimeoutProcessInterval)
			}
		}
}();
Microsoft.Office.Common.Message=function Microsoft_Office_Common_Message(messageType, actionName, conversationId, correlationId, data)
{
	var e=Function._validateParams(arguments,[{
				name: "messageType",
				type: Number,
				mayBeNull: false
			},{
				name: "actionName",
				type: String,
				mayBeNull: false
			},{
				name: "conversationId",
				type: String,
				mayBeNull: false
			},{
				name: "correlationId",
				mayBeNull: false
			},{
				name: "data",
				mayBeNull: true,
				optional: true
			}]);
	if(e)
		throw e;
	this._messageType=messageType;
	this._actionName=actionName;
	this._conversationId=conversationId;
	this._correlationId=correlationId;
	this._origin=window.location.href;
	if(typeof data=="undefined")
		this._data=null;
	else
		this._data=data
};
Microsoft.Office.Common.Message.prototype={
	getActionName: function Microsoft_Office_Common_Message$getActionName()
	{
		return this._actionName
	},
	getConversationId: function Microsoft_Office_Common_Message$getConversationId()
	{
		return this._conversationId
	},
	getCorrelationId: function Microsoft_Office_Common_Message$getCorrelationId()
	{
		return this._correlationId
	},
	getOrigin: function Microsoft_Office_Common_Message$getOrigin()
	{
		return this._origin
	},
	getData: function Microsoft_Office_Common_Message$getData()
	{
		return this._data
	},
	getMessageType: function Microsoft_Office_Common_Message$getMessageType()
	{
		return this._messageType
	}
};
Microsoft.Office.Common.Request=function Microsoft_Office_Common_Request(actionName, actionType, conversationId, correlationId, data)
{
	Microsoft.Office.Common.Request.uber.constructor.call(this,Microsoft.Office.Common.MessageType.request,actionName,conversationId,correlationId,data);
	this._actionType=actionType
};
OSF.OUtil.extend(Microsoft.Office.Common.Request,Microsoft.Office.Common.Message);
Microsoft.Office.Common.Request.prototype.getActionType=function Microsoft_Office_Common_Request$getActionType()
{
	return this._actionType
};
Microsoft.Office.Common.Response=function Microsoft_Office_Common_Response(actionName, conversationId, correlationId, errorCode, responseType, data)
{
	Microsoft.Office.Common.Response.uber.constructor.call(this,Microsoft.Office.Common.MessageType.response,actionName,conversationId,correlationId,data);
	this._errorCode=errorCode;
	this._responseType=responseType
};
OSF.OUtil.extend(Microsoft.Office.Common.Response,Microsoft.Office.Common.Message);
Microsoft.Office.Common.Response.prototype.getErrorCode=function Microsoft_Office_Common_Response$getErrorCode()
{
	return this._errorCode
};
Microsoft.Office.Common.Response.prototype.getResponseType=function Microsoft_Office_Common_Response$getResponseType()
{
	return this._responseType
};
Microsoft.Office.Common.MessagePackager={
	envelope: function Microsoft_Office_Common_MessagePackager$envelope(messageObject)
	{
		return Sys.Serialization.JavaScriptSerializer.serialize(messageObject)
	},
	unenvelope: function Microsoft_Office_Common_MessagePackager$unenvelope(messageObject)
	{
		return Sys.Serialization.JavaScriptSerializer.deserialize(messageObject,true)
	}
};
Microsoft.Office.Common.ResponseSender=function Microsoft_Office_Common_ResponseSender(requesterWindow, requesterUrl, actionName, conversationId, correlationId, responseType)
{
	var e=Function._validateParams(arguments,[{
				name: "requesterWindow",
				mayBeNull: false
			},{
				name: "requesterUrl",
				type: String,
				mayBeNull: false
			},{
				name: "actionName",
				type: String,
				mayBeNull: false
			},{
				name: "conversationId",
				type: String,
				mayBeNull: false
			},{
				name: "correlationId",
				mayBeNull: false
			},{
				name: "responsetype",
				type: Number,
				maybeNull: false
			}]);
	if(e)
		throw e;
	this._requesterWindow=requesterWindow;
	this._requesterUrl=requesterUrl;
	this._actionName=actionName;
	this._conversationId=conversationId;
	this._correlationId=correlationId;
	this._invokeResultCode=Microsoft.Office.Common.InvokeResultCode.noError;
	this._responseType=responseType;
	var me=this;
	this._send=function(result)
	{
		try
		{
			var response=new Microsoft.Office.Common.Response(me._actionName,me._conversationId,me._correlationId,me._invokeResultCode,me._responseType,result);
			var envelopedResult=Microsoft.Office.Common.MessagePackager.envelope(response);
			me._requesterWindow.postMessage(envelopedResult,me._requesterUrl)
		}
		catch(ex)
		{
			Sys.Debug.trace("ResponseSender._send error:"+ex.message)
		}
	}
};
Microsoft.Office.Common.ResponseSender.prototype={
	getRequesterWindow: function Microsoft_Office_Common_ResponseSender$getRequesterWindow()
	{
		return this._requesterWindow
	},
	getRequesterUrl: function Microsoft_Office_Common_ResponseSender$getRequesterUrl()
	{
		return this._requesterUrl
	},
	getActionName: function Microsoft_Office_Common_ResponseSender$getActionName()
	{
		return this._actionName
	},
	getConversationId: function Microsoft_Office_Common_ResponseSender$getConversationId()
	{
		return this._conversationId
	},
	getCorrelationId: function Microsoft_Office_Common_ResponseSender$getCorrelationId()
	{
		return this._correlationId
	},
	getSend: function Microsoft_Office_Common_ResponseSender$getSend()
	{
		return this._send
	},
	setResultCode: function Microsoft_Office_Common_ResponseSender$setResultCode(resultCode)
	{
		this._invokeResultCode=resultCode
	}
};
Microsoft.Office.Common.InvokeCompleteCallback=function Microsoft_Office_Common_InvokeCompleteCallback(requesterWindow, requesterUrl, actionName, conversationId, correlationId, postCallbackHandler)
{
	Microsoft.Office.Common.InvokeCompleteCallback.uber.constructor.call(this,requesterWindow,requesterUrl,actionName,conversationId,correlationId,Microsoft.Office.Common.ResponseType.forCalling);
	this._postCallbackHandler=postCallbackHandler;
	var me=this;
	this._send=function(result)
	{
		try
		{
			var response=new Microsoft.Office.Common.Response(me._actionName,me._conversationId,me._correlationId,me._invokeResultCode,me._responseType,result);
			var envelopedResult=Microsoft.Office.Common.MessagePackager.envelope(response);
			me._requesterWindow.postMessage(envelopedResult,me._requesterUrl);
			me._postCallbackHandler()
		}
		catch(ex)
		{
			Sys.Debug.trace("InvokeCompleteCallback._send error:"+ex.message)
		}
	}
};
OSF.OUtil.extend(Microsoft.Office.Common.InvokeCompleteCallback,Microsoft.Office.Common.ResponseSender);
Microsoft.Office.Common.Invoker=function Microsoft_Office_Common_Invoker(methodObject, paramValue, invokeCompleteCallback, eventHandlerProxyList, conversationId, eventName)
{
	var e=Function._validateParams(arguments,[{
				name: "methodObject",
				mayBeNull: false
			},{
				name: "paramValue",
				mayBeNull: true
			},{
				name: "invokeCompleteCallback",
				mayBeNull: false
			},{
				name: "eventHandlerProxyList",
				mayBeNull: true
			},{
				name: "conversationId",
				type: String,
				mayBeNull: false
			},{
				name: "eventName",
				type: String,
				mayBeNull: false
			}]);
	if(e)
		throw e;
	this._methodObject=methodObject;
	this._param=paramValue;
	this._invokeCompleteCallback=invokeCompleteCallback;
	this._eventHandlerProxyList=eventHandlerProxyList;
	this._conversationId=conversationId;
	this._eventName=eventName
};
Microsoft.Office.Common.Invoker.prototype={
	invoke: function Microsoft_Office_Common_Invoker$invoke()
	{
		try
		{
			var result;
			switch(this._methodObject.getInvokeType())
			{
				case Microsoft.Office.Common.InvokeType.async:
					this._methodObject.getMethod()(this._param,this._invokeCompleteCallback.getSend());
					break;
				case Microsoft.Office.Common.InvokeType.sync:
					result=this._methodObject.getMethod()(this._param);
					this._invokeCompleteCallback.getSend()(result);
					break;
				case Microsoft.Office.Common.InvokeType.syncRegisterEvent:
					var eventHandlerProxy=this._createEventHandlerProxyObject(this._invokeCompleteCallback);
					result=this._methodObject.getMethod()(eventHandlerProxy.getSend(),this._param);
					this._eventHandlerProxyList[this._conversationId+this._eventName]=eventHandlerProxy.getSend();
					this._invokeCompleteCallback.getSend()(result);
					break;
				case Microsoft.Office.Common.InvokeType.syncUnregisterEvent:
					var eventHandler=this._eventHandlerProxyList[this._conversationId+this._eventName];
					result=this._methodObject.getMethod()(eventHandler,this._param);
					delete this._eventHandlerProxyList[this._conversationId+this._eventName];
					this._invokeCompleteCallback.getSend()(result);
					break;
				case Microsoft.Office.Common.InvokeType.asyncRegisterEvent:
					var eventHandlerProxyAsync=this._createEventHandlerProxyObject(this._invokeCompleteCallback);
					this._methodObject.getMethod()(eventHandlerProxyAsync.getSend(),this._invokeCompleteCallback.getSend(),this._param);
					this._eventHandlerProxyList[this._callerId+this._eventName]=eventHandlerProxyAsync.getSend();
					break;
				case Microsoft.Office.Common.InvokeType.asyncUnregisterEvent:
					var eventHandlerAsync=this._eventHandlerProxyList[this._callerId+this._eventName];
					this._methodObject.getMethod()(eventHandlerAsync,this._invokeCompleteCallback.getSend(),this._param);
					delete this._eventHandlerProxyList[this._callerId+this._eventName];
					break;
				default:
					break
			}
		}
		catch(ex)
		{
			this._invokeCompleteCallback.setResultCode(Microsoft.Office.Common.InvokeResultCode.errorInResponse);
			this._invokeCompleteCallback.getSend()(ex)
		}
	},
	getInvokeBlockingFlag: function Microsoft_Office_Common_Invoker$getInvokeBlockingFlag()
	{
		return this._methodObject.getBlockingFlag()
	},
	_createEventHandlerProxyObject: function Microsoft_Office_Common_Invoker$_createEventHandlerProxyObject(invokeCompleteObject)
	{
		return new Microsoft.Office.Common.ResponseSender(invokeCompleteObject.getRequesterWindow(),invokeCompleteObject.getRequesterUrl(),invokeCompleteObject.getActionName(),invokeCompleteObject.getConversationId(),invokeCompleteObject.getCorrelationId(),Microsoft.Office.Common.ResponseType.forEventing)
	}
};
OSF.OUtil.setNamespace("WAC",OSF.DDA);
OSF.DDA.WAC.UniqueArguments={
	Data: "Data",
	Properties: "Properties",
	BindingRequest: "DdaBindingsMethod",
	BindingResponse: "Bindings",
	SingleBindingResponse: "singleBindingResponse",
	GetData: "DdaGetBindingData",
	AddRowsColumns: "DdaAddRowsColumns",
	SetData: "DdaSetBindingData",
	ClearFormats: "DdaClearBindingFormats",
	SetFormats: "DdaSetBindingFormats",
	SettingsRequest: "DdaSettingsMethod",
	BindingEventSource: "ddaBinding",
	ArrayData: "ArrayData"
};
OSF.OUtil.setNamespace("Delegate",OSF.DDA.WAC);
OSF.DDA.WAC.Delegate.SpecialProcessor=function OSF_DDA_WAC_Delegate_SpecialProcessor()
{
	var complexTypes=[OSF.DDA.WAC.UniqueArguments.SingleBindingResponse,OSF.DDA.WAC.UniqueArguments.BindingRequest,OSF.DDA.WAC.UniqueArguments.BindingResponse,OSF.DDA.WAC.UniqueArguments.GetData,OSF.DDA.WAC.UniqueArguments.AddRowsColumns,OSF.DDA.WAC.UniqueArguments.SetData,OSF.DDA.WAC.UniqueArguments.ClearFormats,OSF.DDA.WAC.UniqueArguments.SetFormats,OSF.DDA.WAC.UniqueArguments.SettingsRequest,OSF.DDA.WAC.UniqueArguments.BindingEventSource];
	var dynamicTypes={};
	OSF.DDA.WAC.Delegate.SpecialProcessor.uber.constructor.call(this,complexTypes,dynamicTypes)
};
OSF.OUtil.extend(OSF.DDA.WAC.Delegate.SpecialProcessor,OSF.DDA.SpecialProcessor);
OSF.DDA.WAC.Delegate.ParameterMap=OSF.DDA.getDecoratedParameterMap(new OSF.DDA.WAC.Delegate.SpecialProcessor,[]);
OSF.OUtil.setNamespace("WAC",OSF.DDA);
OSF.OUtil.setNamespace("Delegate",OSF.DDA.WAC);
OSF.DDA.WAC.getDelegateMethods=function OSF_DDA_WAC_getDelegateMethods()
{
	var delegateMethods={};
	delegateMethods[OSF.DDA.DispIdHost.Delegates.ExecuteAsync]=OSF.DDA.WAC.Delegate.executeAsync;
	delegateMethods[OSF.DDA.DispIdHost.Delegates.RegisterEventAsync]=OSF.DDA.WAC.Delegate.registerEventAsync;
	delegateMethods[OSF.DDA.DispIdHost.Delegates.UnregisterEventAsync]=OSF.DDA.WAC.Delegate.unregisterEventAsync;
	return delegateMethods
};
OSF.DDA.WAC.Delegate.version=1;
OSF.DDA.WAC.Delegate.executeAsync=function OSF_DDA_WAC_Delegate$executeAsync(args)
{
	if(!args.hostCallArgs)
		args.hostCallArgs={};
	args.hostCallArgs["DdaMethod"]={
		ControlId: OSF._OfficeAppFactory.getId(),
		Version: OSF.DDA.WAC.Delegate.version,
		DispatchId: args.dispId
	};
	args.hostCallArgs["__timeout__"]=-1;
	if(args.onCalling)
		args.onCalling();
	var startTime=(new Date).getTime();
	OSF.getClientEndPoint().invoke("executeMethod",function OSF_DDA_WAC_Delegate$OMFacade$OnResponse(xdmStatus, payload)
	{
		if(args.onReceiving)
			args.onReceiving();
		var error;
		if(xdmStatus==Microsoft.Office.Common.InvokeResultCode.noError)
		{
			OSF.DDA.WAC.Delegate.version=payload["Version"];
			error=payload["Error"]
		}
		else
			switch(xdmStatus)
			{
				case Microsoft.Office.Common.InvokeResultCode.errorHandlingRequestAccessDenied:
					error=OSF.DDA.ErrorCodeManager.errorCodes.ooeNoCapability;
					break;
				default:
					error=OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
					break
			}
		if(args.onComplete)
			args.onComplete(error,payload);
		if(OSF.AppTelemetry)
			OSF.AppTelemetry.onMethodDone(args.dispId,null,Math.abs((new Date).getTime() - startTime),error)
	},args.hostCallArgs)
};
OSF.DDA.WAC.Delegate._getOnAfterRegisterEvent=function OSF_DDA_WAC_Delegate$GetOnAfterRegisterEvent(register, args)
{
	var startTime=(new Date).getTime();
	return function OSF_DDA_WAC_Delegate$OnAfterRegisterEvent(xdmStatus, payload)
		{
			if(args.onReceiving)
				args.onReceiving();
			var status;
			if(xdmStatus !=Microsoft.Office.Common.InvokeResultCode.noError)
				switch(xdmStatus)
				{
					case Microsoft.Office.Common.InvokeResultCode.errorHandlingRequestAccessDenied:
						status=OSF.DDA.ErrorCodeManager.errorCodes.ooeNoCapability;
						break;
					default:
						status=OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
						break
				}
			else if(payload)
				if(payload["Error"])
					status=payload["Error"];
				else
					status=OSF.DDA.ErrorCodeManager.errorCodes.ooeSuccess;
			else
				status=OSF.DDA.ErrorCodeManager.errorCodes.ooeInternalError;
			if(args.onComplete)
				args.onComplete(status);
			if(OSF.AppTelemetry)
				OSF.AppTelemetry.onRegisterDone(register,args.dispId,Math.abs((new Date).getTime() - startTime),status)
		}
};
OSF.DDA.WAC.Delegate.registerEventAsync=function OSF_DDA_WAC_Delegate$RegisterEventAsync(args)
{
	if(args.onCalling)
		args.onCalling();
	OSF.getClientEndPoint().registerForEvent(OSF.DDA.getXdmEventName(args.targetId,args.eventType),function OSF_DDA_WACOMFacade$OnEvent(payload)
	{
		if(args.onEvent)
			args.onEvent(payload);
		if(OSF.AppTelemetry)
			OSF.AppTelemetry.onEventDone(args.dispId)
	},OSF.DDA.WAC.Delegate._getOnAfterRegisterEvent(true,args),{
		controlId: OSF._OfficeAppFactory.getId(),
		eventDispId: args.dispId,
		targetId: args.targetId
	})
};
OSF.DDA.WAC.Delegate.unregisterEventAsync=function OSF_DDA_WAC_Delegate$UnregisterEventAsync(args)
{
	if(args.onCalling)
		args.onCalling();
	OSF.getClientEndPoint().unregisterForEvent(OSF.DDA.getXdmEventName(args.targetId,args.eventType),OSF.DDA.WAC.Delegate._getOnAfterRegisterEvent(false,args),{
		controlId: OSF._OfficeAppFactory.getId(),
		eventDispId: args.dispId,
		targetId: args.targetId
	})
};
(function()
{
	var checkScriptOverride=function OSF$checkScriptOverride()
		{
			var postScriptOverrideCheckAction=function OSF$postScriptOverrideCheckAction(customizedScriptPath)
				{
					if(customizedScriptPath)
						OSF.OUtil.loadScript(customizedScriptPath,function()
						{
							Sys.Debug.trace("loaded customized script:"+customizedScriptPath)
						})
				};
			var conversationID,
				webAppUrl,
				items;
			var clientEndPoint=null;
			var xdmInfoValue=OSF.OUtil.parseXdmInfo();
			if(xdmInfoValue)
			{
				items=OSF.OUtil.getInfoItems(xdmInfoValue);
				if(items && items.length >=3)
				{
					conversationID=items[0];
					webAppUrl=items[2];
					clientEndPoint=Microsoft.Office.Common.XdmCommunicationManager.connect(conversationID,window.parent,webAppUrl)
				}
			}
			var customizedScriptPath=null;
			if(!clientEndPoint)
			{
				try
				{
					if(typeof window.external.getCustomizedScriptPath !=="undefined")
						customizedScriptPath=window.external.getCustomizedScriptPath()
				}
				catch(ex)
				{
					Sys.Debug.trace("no script override through window.external.")
				}
				postScriptOverrideCheckAction(customizedScriptPath)
			}
			else
				try
				{
					clientEndPoint.invoke("getCustomizedScriptPathAsync",function OSF$getCustomizedScriptPathAsyncCallback(errorCode, scriptPath)
					{
						postScriptOverrideCheckAction(errorCode===0 ? scriptPath : null)
					},{__timeout__: 1e3})
				}
				catch(ex)
				{
					Sys.Debug.trace("no script override through cross frame communication.")
				}
		};
	var isMicrosftAjaxLoaded=function OSF$isMicrosftAjaxLoaded()
		{
			if(typeof Sys !=="undefined" && typeof Type !=="undefined" && Sys.StringBuilder && typeof Sys.StringBuilder==="function" && Type.registerNamespace && typeof Type.registerNamespace==="function" && Type.registerClass && typeof Type.registerClass==="function")
				return true;
			else
				return false
		};
	if(isMicrosftAjaxLoaded())
		checkScriptOverride();
	else if(typeof Function !=="undefined")
	{
		var msAjaxCDNPath=(window.location.protocol.toLowerCase()==="https:" ? "https:" : "http:")+"//ajax.aspnetcdn.com/ajax/3.5/MicrosoftAjax.js";
		var onMicrosoftAjaxLoaded=function()
			{
				if(isMicrosftAjaxLoaded())
					checkScriptOverride();
				else if(typeof Function !=="undefined")
					throw"Not able to load MicrosoftAjax.js.";
			};
		if(!(OSF._OfficeAppFactory && OSF._OfficeAppFactory && OSF._OfficeAppFactory.getLoadScriptHelper && OSF._OfficeAppFactory.getLoadScriptHelper().isScriptLoading(OSF.ConstantNames.MicrosoftAjaxId)))
			OSF.OUtil.loadScript(msAjaxCDNPath,onMicrosoftAjaxLoaded);
		else
			OSF._OfficeAppFactory.getLoadScriptHelper().waitForScripts([OSF.ConstantNames.MicrosoftAjaxId],onMicrosoftAjaxLoaded)
	}
})();
OSF.InitializationHelper=function OSF_InitializationHelper(hostInfo, webAppState, context, settings, hostFacade)
{
	this._hostInfo=hostInfo;
	this._webAppState=webAppState;
	this._context=context;
	this._settings=settings;
	this._hostFacade=hostFacade;
	this._initializeSettings=function OSF_InitializationHelper$initializeSettings(appContext, refreshSupported)
	{
		var settings;
		var serializedSettings=appContext.get_settings();
		var osfSessionStorage=OSF.OUtil.getSessionStorage();
		if(osfSessionStorage)
		{
			var storageSettings=osfSessionStorage.getItem(OSF._OfficeAppFactory.getCachedSessionSettingsKey());
			if(storageSettings)
				serializedSettings=JSON ? JSON.parse(storageSettings) : Sys.Serialization.JavaScriptSerializer.deserialize(storageSettings,true);
			else
			{
				storageSettings=JSON ? JSON.stringify(serializedSettings) : Sys.Serialization.JavaScriptSerializer.serialize(serializedSettings);
				osfSessionStorage.setItem(OSF._OfficeAppFactory.getCachedSessionSettingsKey(),storageSettings)
			}
		}
		var deserializedSettings=OSF.DDA.SettingsManager.deserializeSettings(serializedSettings);
		if(refreshSupported)
			settings=new OSF.DDA.RefreshableSettings(deserializedSettings);
		else
			settings=new OSF.DDA.Settings(deserializedSettings);
		return settings
	};
	var windowOpen=function OSF_InitializationHelper$windowOpen(windowObj)
		{
			var proxy=window.open;
			windowObj.open=function(strUrl, strWindowName, strWindowFeatures)
			{
				var windowObject=null;
				try
				{
					windowObject=proxy(strUrl,strWindowName,strWindowFeatures)
				}
				catch(ex){}
				if(!windowObject)
				{
					var params={
							strUrl: strUrl,
							strWindowName: strWindowName,
							strWindowFeatures: strWindowFeatures
						};
					OSF._OfficeAppFactory.getClientEndPoint().invoke("ContextActivationManager_openWindowInHost",null,params)
				}
				return windowObject
			}
		};
	windowOpen(window)
};
OSF.InitializationHelper.prototype.getAppContext=function OSF_InitializationHelper$getAppContext(wnd, gotAppContext)
{
	var getInvocationCallbackWebApp=function OSF_InitializationHelper_getAppContextAsync$getInvocationCallbackWebApp(errorCode, appContext)
		{
			var settings;
			if(appContext._appName===OSF.AppName.ExcelWebApp)
			{
				var serializedSettings=appContext._settings;
				settings={};
				for(var index in serializedSettings)
				{
					var setting=serializedSettings[index];
					settings[setting[0]]=setting[1]
				}
			}
			else
				settings=appContext._settings;
			if(errorCode===0 && appContext._id !=undefined && appContext._appName !=undefined && appContext._appVersion !=undefined && appContext._appUILocale !=undefined && appContext._dataLocale !=undefined && appContext._docUrl !=undefined && appContext._clientMode !=undefined && appContext._settings !=undefined && appContext._reason !=undefined)
			{
				var returnedContext=new OSF.OfficeAppContext(appContext._id,appContext._appName,appContext._appVersion,appContext._appUILocale,appContext._dataLocale,appContext._docUrl,appContext._clientMode,settings,appContext._reason,appContext._osfControlType,appContext._eToken,appContext._correlationId,appContext._id);
				if(OSF.AppTelemetry)
					OSF.AppTelemetry.initialize(returnedContext);
				gotAppContext(returnedContext)
			}
			else
				throw"Function ContextActivationManager_getAppContextAsync call failed. ErrorCode is "+errorCode;
		};
	this._webAppState.clientEndPoint.invoke("ContextActivationManager_getAppContextAsync",getInvocationCallbackWebApp,this._webAppState.id)
};
OSF.InitializationHelper.prototype.setAgaveHostCommunication=function OSF_InitializationHelper$setAgaveHostCommunication()
{
	var me=this;
	var xdmInfoValue=OSF.OUtil.parseXdmInfoWithGivenFragment(false,OSF._OfficeAppFactory.getWindowLocationHash());
	if(xdmInfoValue)
	{
		var xdmItems=OSF.OUtil.getInfoItems(xdmInfoValue);
		if(xdmItems !=undefined && xdmItems.length===3)
		{
			me._webAppState.conversationID=xdmItems[0];
			me._webAppState.id=xdmItems[1];
			me._webAppState.webAppUrl=xdmItems[2]
		}
	}
	me._webAppState.clientEndPoint=Microsoft.Office.Common.XdmCommunicationManager.connect(me._webAppState.conversationID,me._webAppState.wnd,me._webAppState.webAppUrl);
	me._webAppState.serviceEndPoint=Microsoft.Office.Common.XdmCommunicationManager.createServiceEndPoint(me._webAppState.id);
	var notificationConversationId=me._webAppState.conversationID+OSF.SharedConstants.NotificationConversationIdSuffix;
	me._webAppState.serviceEndPoint.registerConversation(notificationConversationId,me._webAppState.webAppUrl);
	var focusFirstItem=function OSF_OfficeAppFactory_initialize$focusFirstItem()
		{
			if(!me._webAppState.focused)
			{
				me._webAppState.focused=true;
				var list=document.querySelectorAll("input,a,button");
				for(var i=0; i < list.length; i++)
				{
					var node=list[i];
					if(node instanceof HTMLElement)
					{
						var element=node;
						element.focus();
						break
					}
				}
			}
		};
	var notifyAgave=function OSF__OfficeAppFactory_initialize$notifyAgave(actionId)
		{
			switch(actionId)
			{
				case OSF.AgaveHostAction.Select:
					me._webAppState.focused=true;
					break;
				case OSF.AgaveHostAction.UnSelect:
					me._webAppState.focused=false;
					break;
				case OSF.AgaveHostAction.CtrlF6In:
					focusFirstItem();
				default:
					Sys.Debug.trace("actionId "+actionId+" notifyAgave is wrong.");
					break
			}
		};
	me._webAppState.serviceEndPoint.registerMethod("Office_notifyAgave",notifyAgave,Microsoft.Office.Common.InvokeType.async,false);
	OSF.OUtil.addEventListener(window,"focus",function()
	{
		if(!me._webAppState.focused)
			me._webAppState.focused=true;
		me._webAppState.clientEndPoint.invoke("ContextActivationManager_notifyHost",null,[me._webAppState.id,OSF.AgaveHostAction.Select])
	});
	OSF.OUtil.addEventListener(window,"blur",function()
	{
		if(me._webAppState.focused)
			me._webAppState.focused=false;
		me._webAppState.clientEndPoint.invoke("ContextActivationManager_notifyHost",null,[me._webAppState.id,OSF.AgaveHostAction.UnSelect])
	});
	OSF.OUtil.addEventListener(window,"keydown",function(e)
	{
		if(e.keyCode==117 && e.ctrlKey)
		{
			if(e.preventDefault)
				e.preventDefault();
			else
				e.returnValue=false;
			var actionId=OSF.AgaveHostAction.CtrlF6Exit;
			if(e.shiftKey)
				actionId=OSF.AgaveHostAction.CtrlF6ExitShift;
			me._webAppState.clientEndPoint.invoke("ContextActivationManager_notifyHost",null,[me._webAppState.id,actionId])
		}
	});
	OSF.OUtil.addEventListener(window,"keypress",function(e)
	{
		if(e.keyCode==117 && e.ctrlKey)
			if(e.preventDefault)
				e.preventDefault();
			else
				e.returnValue=false
	})
};
OSF.getClientEndPoint=function OSF$getClientEndPoint()
{
	var initializationHelper=OSF._OfficeAppFactory.getInitializationHelper();
	return initializationHelper._webAppState.clientEndPoint
};
OSF.InitializationHelper.prototype.prepareRightBeforeWebExtensionInitialize=function OSF_InitializationHelper$prepareRightBeforeWebExtensionInitialize(appContext)
{
	var license=new OSF.DDA.License(appContext.get_eToken());
	var isOwa=appContext.get_appName()==OSF.AppName.OutlookWebApp;
	if(isOwa)
		OSF._OfficeAppFactory.setContext(new OSF.DDA.OutlookContext(appContext,this._settings,license,appContext.appOM));
	else
		OSF._OfficeAppFactory.setContext(new OSF.DDA.OutlookContext(appContext,this._settings,license,appContext.appOM,OSF.DDA.Theming ? OSF.DDA.Theming.getOfficeTheme : null));
	var reason=appContext.get_reason();
	Microsoft.Office.WebExtension.initialize(reason);
	if(!isOwa)
		OfficeJsClient_OutlookWin32.prepareRightBeforeWebExtensionInitialize()
};
OSF.DDA.SettingsManager={
	SerializedSettings: "serializedSettings",
	DateJSONPrefix: "Date(",
	DataJSONSuffix: ")",
	serializeSettings: function OSF_DDA_SettingsManager$serializeSettings(settingsCollection)
	{
		var ret={};
		for(var key in settingsCollection)
		{
			var value=settingsCollection[key];
			try
			{
				if(JSON)
					value=JSON.stringify(value,function dateReplacer(k, v)
					{
						return OSF.OUtil.isDate(this[k]) ? OSF.DDA.SettingsManager.DateJSONPrefix+this[k].getTime()+OSF.DDA.SettingsManager.DataJSONSuffix : v
					});
				else
					value=Sys.Serialization.JavaScriptSerializer.serialize(value);
				ret[key]=value
			}
			catch(ex){}
		}
		return ret
	},
	deserializeSettings: function OSF_DDA_SettingsManager$deserializeSettings(serializedSettings)
	{
		var ret={};
		serializedSettings=serializedSettings || {};
		for(var key in serializedSettings)
		{
			var value=serializedSettings[key];
			try
			{
				if(JSON)
					value=JSON.parse(value,function dateReviver(k, v)
					{
						var d;
						if(typeof v==="string" && v && v.length > 6 && v.slice(0,5)===OSF.DDA.SettingsManager.DateJSONPrefix && v.slice(-1)===OSF.DDA.SettingsManager.DataJSONSuffix)
						{
							d=new Date(parseInt(v.slice(5,-1)));
							if(d)
								return d
						}
						return v
					});
				else
					value=Sys.Serialization.JavaScriptSerializer.deserialize(value,true);
				ret[key]=value
			}
			catch(ex){}
		}
		return ret
	}
};
OSF.InitializationHelper.prototype.loadAppSpecificScriptAndCreateOM=function OSF_InitializationHelper$loadAppSpecificScriptAndCreateOM(appContext, appReady, basePath)
{
	Type.registerNamespace("Microsoft.Office.WebExtension.MailboxEnums");
	Microsoft.Office.WebExtension.MailboxEnums.EntityType={
		MeetingSuggestion: "meetingSuggestion",
		TaskSuggestion: "taskSuggestion",
		Address: "address",
		EmailAddress: "emailAddress",
		Url: "url",
		PhoneNumber: "phoneNumber",
		Contact: "contact",
		FlightReservations: "flightReservations",
		ParcelDeliveries: "parcelDeliveries"
	};
	Microsoft.Office.WebExtension.MailboxEnums.ItemType={
		Message: "message",
		Appointment: "appointment"
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
	Microsoft.Office.WebExtension.MailboxEnums.AttachmentType={
		File: "file",
		Item: "item"
	};
	Microsoft.Office.WebExtension.MailboxEnums.BodyType={
		Text: "text",
		Html: "html"
	};
	Microsoft.Office.WebExtension.MailboxEnums.ItemNotificationMessageType={
		ProgressIndicator: "progressIndicator",
		InformationalMessage: "informationalMessage",
		ErrorMessage: "errorMessage"
	};
	Microsoft.Office.WebExtension.CoercionType={
		Text: "text",
		Html: "html"
	};
	Type.registerNamespace("OSF.DDA");
	OSF.DDA.OutlookAppOm=function(officeAppContext, targetWindow, appReadyCallback)
	{
		this.$$d__callAppReadyCallback$p$0=Function.createDelegate(this,this._callAppReadyCallback$p$0);
		this.$$d__displayNewAppointmentFormApi$p$0=Function.createDelegate(this,this._displayNewAppointmentFormApi$p$0);
		this.$$d_windowOpenOverrideHandler=Function.createDelegate(this,this.windowOpenOverrideHandler);
		this.$$d__getEwsUrl$p$0=Function.createDelegate(this,this._getEwsUrl$p$0);
		this.$$d__getDiagnostics$p$0=Function.createDelegate(this,this._getDiagnostics$p$0);
		this.$$d__getUserProfile$p$0=Function.createDelegate(this,this._getUserProfile$p$0);
		this.$$d__getItem$p$0=Function.createDelegate(this,this._getItem$p$0);
		this.$$d__getInitialDataResponseHandler$p$0=Function.createDelegate(this,this._getInitialDataResponseHandler$p$0);
		OSF.DDA.OutlookAppOm._instance$p=this;
		this._officeAppContext$p$0=officeAppContext;
		this._appReadyCallback$p$0=appReadyCallback;
		var $$t_4=this;
		var stringLoadedCallback=function()
			{
				if(appReadyCallback)
					$$t_4._invokeHostMethod$i$0(1,"GetInitialData",null,$$t_4.$$d__getInitialDataResponseHandler$p$0)
			};
		if(this._areStringsLoaded$p$0())
			stringLoadedCallback();
		else
			this._loadLocalizedScript$p$0(stringLoadedCallback)
	};
	OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i=function(currentPermissionLevel)
	{
		if(!currentPermissionLevel)
			throw Error.create(_u.ExtensibilityStrings.l_ElevatedPermissionNeeded_Text);
	};
	OSF.DDA.OutlookAppOm._throwOnOutOfRange$i=function(value, minValue, maxValue, argumentName)
	{
		if(value < minValue || value > maxValue)
			throw Error.argumentOutOfRange(argumentName);
	};
	OSF.DDA.OutlookAppOm._getHtmlBody$p=function(data)
	{
		var htmlBody="";
		if("htmlBody" in data)
		{
			OSF.DDA.OutlookAppOm._throwOnInvalidHtmlBody$p(data["htmlBody"]);
			htmlBody=data["htmlBody"]
		}
		return htmlBody
	};
	OSF.DDA.OutlookAppOm._getAttachments$p=function(data)
	{
		var attachments=[];
		if("attachments" in data)
		{
			attachments=data["attachments"];
			OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentsArray$p(attachments)
		}
		return attachments
	};
	OSF.DDA.OutlookAppOm._getOptionsAndCallback$p=function(data)
	{
		var args=[];
		if("options" in data)
			args[0]=data["options"];
		if("callback" in data)
			args[args.length]=data["callback"];
		return args
	};
	OSF.DDA.OutlookAppOm._createAttachmentsDataForHost$p=function(attachments)
	{
		var attachmentsData=new Array(0);
		if(Array.isInstanceOfType(attachments))
			for(var i=0; i < attachments.length; i++)
				if(Object.isInstanceOfType(attachments[i]))
				{
					var attachment=attachments[i];
					OSF.DDA.OutlookAppOm._throwOnInvalidAttachment$p(attachment);
					attachmentsData[i]=OSF.DDA.OutlookAppOm._createAttachmentData$p(attachment)
				}
				else
					throw Error.argument("attachments");
		return attachmentsData
	};
	OSF.DDA.OutlookAppOm._throwOnInvalidHtmlBody$p=function(htmlBody)
	{
		if(!String.isInstanceOfType(htmlBody))
			throw Error.argument("htmlBody");
		if($h.ScriptHelpers.isNullOrUndefined(htmlBody))
			throw Error.argument("htmlBody");
		OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(htmlBody.length,0,OSF.DDA.OutlookAppOm.maxBodyLength,"htmlBody")
	};
	OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentsArray$p=function(attachments)
	{
		if(!Array.isInstanceOfType(attachments))
			throw Error.argument("attachments");
	};
	OSF.DDA.OutlookAppOm._throwOnInvalidAttachment$p=function(attachment)
	{
		if(!Object.isInstanceOfType(attachment))
			throw Error.argument("attachments");
		if(!("type" in attachment) || !("name" in attachment))
			throw Error.argument("attachments");
		if(!("url" in attachment || "itemId" in attachment))
			throw Error.argument("attachments");
	};
	OSF.DDA.OutlookAppOm._createAttachmentData$p=function(attachment)
	{
		var attachmentData=null;
		if(attachment["type"]==="file")
		{
			var url=attachment["url"];
			var name=attachment["name"];
			OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentUrlOrName$p(url,name);
			attachmentData=OSF.DDA.OutlookAppOm._createFileAttachmentData$p(url,name)
		}
		else if(attachment["type"]==="item")
		{
			var itemId=attachment["itemId"];
			var name=attachment["name"];
			OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentItemIdOrName$p(itemId,name);
			attachmentData=OSF.DDA.OutlookAppOm._createItemAttachmentData$p(itemId,name)
		}
		else
			throw Error.argument("attachments");
		return attachmentData
	};
	OSF.DDA.OutlookAppOm._createFileAttachmentData$p=function(url, name)
	{
		return["file",name,url]
	};
	OSF.DDA.OutlookAppOm._createItemAttachmentData$p=function(itemId, name)
	{
		return["item",name,itemId]
	};
	OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentUrlOrName$p=function(url, name)
	{
		if(!String.isInstanceOfType(url) || !String.isInstanceOfType(name))
			throw Error.argument("attachments");
		if(url.length > 2048)
			throw Error.argumentOutOfRange("attachments",url.length,_u.ExtensibilityStrings.l_AttachmentUrlTooLong_Text);
		OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentName$p(name)
	};
	OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentItemIdOrName$p=function(itemId, name)
	{
		if(!String.isInstanceOfType(itemId) || !String.isInstanceOfType(name))
			throw Error.argument("attachments");
		if(itemId.length > 200)
			throw Error.argumentOutOfRange("attachments",itemId.length,_u.ExtensibilityStrings.l_AttachmentItemIdTooLong_Text);
		OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentName$p(name)
	};
	OSF.DDA.OutlookAppOm._throwOnInvalidAttachmentName$p=function(name)
	{
		if(name.length > 255)
			throw Error.argumentOutOfRange("attachments",name.length,_u.ExtensibilityStrings.l_AttachmentNameTooLong_Text);
	};
	OSF.DDA.OutlookAppOm._throwOnArgumentType$p=function(value, expectedType, argumentName)
	{
		if(Object.getType(value) !==expectedType)
			throw Error.argumentType(argumentName);
	};
	OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p=function(value, minLength, maxLength, name)
	{
		if($h.ScriptHelpers.isNullOrUndefined(value))
			return;
		OSF.DDA.OutlookAppOm._throwOnArgumentType$p(value,String,name);
		var stringValue=value;
		OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(stringValue.length,minLength,maxLength,name)
	};
	OSF.DDA.OutlookAppOm._convertToOutlookParameters$p=function(dispid, data)
	{
		var executeParameters=null;
		switch(dispid)
		{
			case 1:
			case 2:
			case 12:
			case 3:
			case 14:
			case 18:
			case 26:
			case 32:
			case 41:
			case 34:
				break;
			case 4:
				var jsonProperty=JSON.stringify(data["customProperties"]);
				executeParameters=[jsonProperty];
				break;
			case 5:
				executeParameters=[data["body"]];
				break;
			case 8:
			case 9:
				executeParameters=[data["itemId"]];
				break;
			case 7:
				executeParameters=[OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlookForDisplayApi$p(data["requiredAttendees"]),OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlookForDisplayApi$p(data["optionalAttendees"]),data["start"],data["end"],data["location"],OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlookForDisplayApi$p(data["resources"]),data["subject"],data["body"]];
				break;
			case 40:
				executeParameters=[data["marketplaceAssetId"],data["consentState"]];
				break;
			case 11:
			case 10:
				executeParameters=[data["htmlBody"]];
				break;
			case 31:
			case 30:
				executeParameters=[data["htmlBody"],data["attachments"]];
				break;
			case 23:
			case 13:
			case 38:
			case 29:
				executeParameters=[data["data"],data["coercionType"]];
				break;
			case 37:
			case 28:
				executeParameters=[data["coercionType"]];
				break;
			case 17:
				executeParameters=[data["subject"]];
				break;
			case 15:
				executeParameters=[data["recipientField"]];
				break;
			case 22:
			case 21:
				executeParameters=[data["recipientField"],OSF.DDA.OutlookAppOm._convertComposeEmailDictionaryParameterForSetApi$p(data["recipientArray"])];
				break;
			case 19:
				executeParameters=[data["itemId"],data["name"]];
				break;
			case 16:
				executeParameters=[data["uri"],data["name"]];
				break;
			case 20:
				executeParameters=[data["attachmentIndex"]];
				break;
			case 25:
				executeParameters=[data["TimeProperty"],data["time"]];
				break;
			case 24:
				executeParameters=[data["TimeProperty"]];
				break;
			case 27:
				executeParameters=[data["location"]];
				break;
			case 33:
			case 35:
				executeParameters=[data["key"],data["type"],data["persistent"],data["message"],data["icon"]];
				break;
			case 36:
				executeParameters=[data["key"]];
				break;
			default:
				Sys.Debug.fail("Unexpected method dispid");
				break
		}
		return executeParameters
	};
	OSF.DDA.OutlookAppOm._convertRecipientArrayParameterForOutlookForDisplayApi$p=function(array)
	{
		return array ? array.join(";") : null
	};
	OSF.DDA.OutlookAppOm._convertComposeEmailDictionaryParameterForSetApi$p=function(recipients)
	{
		if(!recipients)
			return null;
		var results=new Array(recipients.length);
		for(var i=0; i < recipients.length; i++)
			results[i]=[recipients[i]["address"],recipients[i]["name"]];
		return results
	};
	OSF.DDA.OutlookAppOm._validateAndNormalizeRecipientEmails$p=function(emailset, name)
	{
		if($h.ScriptHelpers.isNullOrUndefined(emailset))
			return null;
		OSF.DDA.OutlookAppOm._throwOnArgumentType$p(emailset,Array,name);
		var originalAttendees=emailset;
		var updatedAttendees=null;
		var normalizationNeeded=false;
		OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(originalAttendees.length,0,OSF.DDA.OutlookAppOm._maxRecipients$p,String.format("{0}.length",name));
		for(var i=0; i < originalAttendees.length; i++)
			if($h.EmailAddressDetails.isInstanceOfType(originalAttendees[i]))
			{
				normalizationNeeded=true;
				break
			}
		if(normalizationNeeded)
			updatedAttendees=[];
		for(var i=0; i < originalAttendees.length; i++)
			if(normalizationNeeded)
			{
				updatedAttendees[i]=$h.EmailAddressDetails.isInstanceOfType(originalAttendees[i]) ? originalAttendees[i].emailAddress : originalAttendees[i];
				OSF.DDA.OutlookAppOm._throwOnArgumentType$p(updatedAttendees[i],String,String.format("{0}[{1}]",name,i))
			}
			else
				OSF.DDA.OutlookAppOm._throwOnArgumentType$p(originalAttendees[i],String,String.format("{0}[{1}]",name,i));
		return updatedAttendees
	};
	OSF.DDA.OutlookAppOm.prototype={
		_initialData$p$0: null,
		_item$p$0: null,
		_userProfile$p$0: null,
		_diagnostics$p$0: null,
		_officeAppContext$p$0: null,
		_appReadyCallback$p$0: null,
		_clientEndPoint$p$0: null,
		get_clientEndPoint: function()
		{
			if(!this._clientEndPoint$p$0)
				this._clientEndPoint$p$0=OSF._OfficeAppFactory.getClientEndPoint();
			return this._clientEndPoint$p$0
		},
		set_clientEndPoint: function(value)
		{
			this._clientEndPoint$p$0=value;
			return value
		},
		get_initialData: function()
		{
			return this._initialData$p$0
		},
		get__appName$i$0: function()
		{
			return this._officeAppContext$p$0.get_appName()
		},
		initialize: function(initialData)
		{
			var ItemTypeKey="itemType";
			this._initialData$p$0=new $h.InitialData(initialData);
			if(1===initialData[ItemTypeKey])
				this._item$p$0=new $h.Message(this._initialData$p$0);
			else if(3===initialData[ItemTypeKey])
				this._item$p$0=new $h.MeetingRequest(this._initialData$p$0);
			else if(2===initialData[ItemTypeKey])
				this._item$p$0=new $h.Appointment(this._initialData$p$0);
			else if(4===initialData[ItemTypeKey])
				this._item$p$0=new $h.MessageCompose(this._initialData$p$0);
			else if(5===initialData[ItemTypeKey])
				this._item$p$0=new $h.AppointmentCompose(this._initialData$p$0);
			else
				Sys.Debug.trace("Unexpected item type was received from the host.");
			this._userProfile$p$0=new $h.UserProfile(this._initialData$p$0);
			this._diagnostics$p$0=new $h.Diagnostics(this._initialData$p$0,this._officeAppContext$p$0.get_appName());
			this._initializeMethods$p$0();
			$h.InitialData._defineReadOnlyProperty$i(this,"item",this.$$d__getItem$p$0);
			$h.InitialData._defineReadOnlyProperty$i(this,"userProfile",this.$$d__getUserProfile$p$0);
			$h.InitialData._defineReadOnlyProperty$i(this,"diagnostics",this.$$d__getDiagnostics$p$0);
			$h.InitialData._defineReadOnlyProperty$i(this,"ewsUrl",this.$$d__getEwsUrl$p$0);
			if(OSF.DDA.OutlookAppOm._instance$p.get__appName$i$0()===64)
				if(this._initialData$p$0.get__overrideWindowOpen$i$0())
					window.open=this.$$d_windowOpenOverrideHandler
		},
		windowOpenOverrideHandler: function(url, targetName, features, replace)
		{
			this._invokeHostMethod$i$0(0,"LaunchPalUrl",{launchUrl: url},null)
		},
		makeEwsRequestAsync: function(data)
		{
			var args=[];
			for(var $$pai_5=1; $$pai_5 < arguments.length;++$$pai_5)
				args[$$pai_5 - 1]=arguments[$$pai_5];
			if($h.ScriptHelpers.isNullOrUndefined(data))
				throw Error.argumentNull("data");
			if(data.length > OSF.DDA.OutlookAppOm._maxEwsRequestSize$p)
				throw Error.argument("data",_u.ExtensibilityStrings.l_EwsRequestOversized_Text);
			this._throwOnMethodCallForInsufficientPermission$i$0(3,"makeEwsRequestAsync");
			var parameters=$h.CommonParameters.parse(args,true,true);
			var ewsRequest=new $h.EwsRequest(parameters._asyncContext$p$0);
			var $$t_4=this;
			ewsRequest.onreadystatechange=function()
			{
				if(4===ewsRequest.get__requestState$i$1())
					parameters._callback$p$0(ewsRequest._asyncResult$p$0)
			};
			ewsRequest.send(data)
		},
		recordDataPoint: function(data)
		{
			if($h.ScriptHelpers.isNullOrUndefined(data))
				throw Error.argumentNull("data");
			this._invokeHostMethod$i$0(0,"RecordDataPoint",data,null)
		},
		recordTrace: function(data)
		{
			if($h.ScriptHelpers.isNullOrUndefined(data))
				throw Error.argumentNull("data");
			this._invokeHostMethod$i$0(0,"RecordTrace",data,null)
		},
		trackCtq: function(data)
		{
			if($h.ScriptHelpers.isNullOrUndefined(data))
				throw Error.argumentNull("data");
			this._invokeHostMethod$i$0(0,"TrackCtq",data,null)
		},
		convertToLocalClientTime: function(timeValue)
		{
			var date=new Date(timeValue.getTime());
			var offset=date.getTimezoneOffset() * -1;
			if(this._initialData$p$0 && this._initialData$p$0.get__timeZoneOffsets$i$0())
			{
				date.setUTCMinutes(date.getUTCMinutes() - offset);
				offset=this._findOffset$p$0(date);
				date.setUTCMinutes(date.getUTCMinutes()+offset)
			}
			var retValue=this._dateToDictionary$i$0(date);
			retValue["timezoneOffset"]=offset;
			return retValue
		},
		convertToUtcClientTime: function(input)
		{
			var retValue=this._dictionaryToDate$i$0(input);
			if(this._initialData$p$0 && this._initialData$p$0.get__timeZoneOffsets$i$0())
			{
				var offset=this._findOffset$p$0(retValue);
				retValue.setUTCMinutes(retValue.getUTCMinutes() - offset);
				offset=!input["timezoneOffset"] ? retValue.getTimezoneOffset() * -1 : input["timezoneOffset"];
				retValue.setUTCMinutes(retValue.getUTCMinutes()+offset)
			}
			return retValue
		},
		getUserIdentityTokenAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			this._throwOnMethodCallForInsufficientPermission$i$0(1,"getUserIdentityTokenAsync");
			var parameters=$h.CommonParameters.parse(args,true,true);
			this._invokeGetTokenMethodAsync$p$0(2,"GetUserIdentityToken",parameters._callback$p$0,parameters._asyncContext$p$0)
		},
		getCallbackTokenAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			this._throwOnMethodCallForInsufficientPermission$i$0(1,"getCallbackTokenAsync");
			var parameters=$h.CommonParameters.parse(args,true,true);
			this._invokeGetTokenMethodAsync$p$0(12,"GetCallbackToken",parameters._callback$p$0,parameters._asyncContext$p$0)
		},
		displayMessageForm: function(itemId)
		{
			if($h.ScriptHelpers.isNullOrUndefined(itemId))
				throw Error.argumentNull("itemId");
			this._invokeHostMethod$i$0(8,"DisplayExistingMessageForm",{itemId: itemId},null)
		},
		displayAppointmentForm: function(itemId)
		{
			if($h.ScriptHelpers.isNullOrUndefined(itemId))
				throw Error.argumentNull("itemId");
			this._invokeHostMethod$i$0(9,"DisplayExistingAppointmentForm",{itemId: itemId},null)
		},
		RegisterConsentAsync: function(consentState)
		{
			if(consentState !==2 && consentState !==1 && consentState)
				throw Error.argumentOutOfRange("consentState");
			var parameters={};
			parameters["consentState"]=consentState.toString();
			parameters["marketplaceAssetId"]=this.GetMarketplaceAssetId();
			this._invokeHostMethod$i$0(40,"RegisterConsentAsync",parameters,null)
		},
		GetIsRead: function()
		{
			return this._initialData$p$0.get__isRead$i$0()
		},
		GetConsentMetadata: function()
		{
			return this._initialData$p$0.get__consentMetadata$i$0()
		},
		GetEntryPointUrl: function()
		{
			return this._initialData$p$0.get__entryPointUrl$i$0()
		},
		GetMarketplaceContentMarket: function()
		{
			return this._initialData$p$0.get__marketplaceContentMarket$i$0()
		},
		GetMarketplaceAssetId: function()
		{
			return this._initialData$p$0.get__marketplaceAssetId$i$0()
		},
		createAsyncResult: function(value, errorCode, detailedErrorCode, userContext, errorMessage)
		{
			var initArgs={};
			var errorArgs=null;
			initArgs[OSF.DDA.AsyncResultEnum.Properties.Value]=value;
			initArgs[OSF.DDA.AsyncResultEnum.Properties.Context]=userContext;
			if(0 !==errorCode)
			{
				errorArgs={};
				var errorProperties=$h.OutlookErrorManager.getErrorArgs(detailedErrorCode);
				errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Name]=errorProperties["name"];
				errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Message]=!errorMessage ? errorProperties["message"] : errorMessage;
				errorArgs[OSF.DDA.AsyncResultEnum.ErrorProperties.Code]=detailedErrorCode
			}
			return new OSF.DDA.AsyncResult(initArgs,errorArgs)
		},
		_throwOnMethodCallForInsufficientPermission$i$0: function(requiredPermissionLevel, methodName)
		{
			if(this._initialData$p$0._permissionLevel$p$0 < requiredPermissionLevel)
				throw Error.create(String.format(_u.ExtensibilityStrings.l_ElevatedPermissionNeededForMethod_Text,methodName));
		},
		_displayReplyForm$i$0: function(obj)
		{
			this._displayReplyFormHelper$p$0(obj,false)
		},
		_displayReplyAllForm$i$0: function(obj)
		{
			this._displayReplyFormHelper$p$0(obj,true)
		},
		_displayReplyFormHelper$p$0: function(obj, isReplyAll)
		{
			if(String.isInstanceOfType(obj))
				this._doDisplayReplyForm$p$0(obj,isReplyAll);
			else if(Object.isInstanceOfType(obj) && Object.getTypeName(obj)==="Object")
			{
				var data={};
				data=$.extend(true,data,obj);
				this._doDisplayReplyFormWithAttachments$p$0(data,isReplyAll)
			}
			else
				throw Error.argumentType();
		},
		_doDisplayReplyForm$p$0: function(htmlBody, isReplyAll)
		{
			if(!$h.ScriptHelpers.isNullOrUndefined(htmlBody))
				OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(htmlBody.length,0,OSF.DDA.OutlookAppOm.maxBodyLength,"htmlBody");
			this._invokeHostMethod$i$0(isReplyAll ? 11 : 10,isReplyAll ? "DisplayReplyAllForm" : "DisplayReplyForm",{htmlBody: htmlBody},null)
		},
		_doDisplayReplyFormWithAttachments$p$0: function(data, isReplyAll)
		{
			var htmlBody=OSF.DDA.OutlookAppOm._getHtmlBody$p(data);
			var attachments=OSF.DDA.OutlookAppOm._getAttachments$p(data);
			var parameters=$h.CommonParameters.parse(OSF.DDA.OutlookAppOm._getOptionsAndCallback$p(data),false);
			var $$t_6=this;
			this._standardInvokeHostMethod$i$0(isReplyAll ? 31 : 30,isReplyAll ? "DisplayReplyAllForm" : "DisplayReplyForm",{
				htmlBody: htmlBody,
				attachments: OSF.DDA.OutlookAppOm._createAttachmentsDataForHost$p(attachments)
			},function(rawInput)
			{
				return rawInput
			},parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		_standardInvokeHostMethod$i$0: function(dispid, name, data, format, userContext, callback)
		{
			var $$t_C=this;
			this._invokeHostMethod$i$0(dispid,name,data,function(resultCode, response)
			{
				if(callback)
				{
					var asyncResult=null;
					if(Object.isInstanceOfType(response))
					{
						var responseDictionary=response;
						if("error" in responseDictionary || "data" in responseDictionary || "errorCode" in responseDictionary)
							if(!responseDictionary["error"])
							{
								var formattedData=format ? format(responseDictionary["data"]) : responseDictionary["data"];
								asyncResult=$$t_C.createAsyncResult(formattedData,0,0,userContext,null)
							}
							else
							{
								var errorCode=responseDictionary["errorCode"];
								asyncResult=$$t_C.createAsyncResult(null,1,errorCode,userContext,null)
							}
					}
					if(!asyncResult && resultCode)
						asyncResult=$$t_C.createAsyncResult(null,1,9002,userContext,null);
					callback(asyncResult)
				}
			})
		},
		_invokeHostMethod$i$0: function(dispid, name, data, responseCallback)
		{
			if(64===this._officeAppContext$p$0.get_appName())
				this.get_clientEndPoint().invoke(name,responseCallback,data);
			else if(dispid)
			{
				var executeParameters=OSF.DDA.OutlookAppOm._convertToOutlookParameters$p(dispid,data);
				var $$t_B=this;
				window.external.execute(dispid,executeParameters,function(nativeData, resultCode)
				{
					if(responseCallback)
					{
						var responseData=nativeData.toArray();
						var rawData=JSON.parse(responseData[0]);
						if(Object.isInstanceOfType(rawData))
						{
							var deserializedData=rawData;
							if(responseData.length > 1 && responseData[1])
							{
								deserializedData["error"]=true;
								deserializedData["errorCode"]=responseData[1]
							}
							else
								deserializedData["error"]=false;
							responseCallback(resultCode,deserializedData)
						}
						else if(Number.isInstanceOfType(rawData))
						{
							var returnDict={};
							returnDict["error"]=true;
							returnDict["errorCode"]=rawData;
							responseCallback(resultCode,returnDict)
						}
						else
							throw Error.notImplemented("Return data type from host must be Dictionary or int");
					}
				})
			}
			else if(responseCallback)
				responseCallback(-2,null)
		},
		_dictionaryToDate$i$0: function(input)
		{
			var retValue=new Date(input["year"],input["month"],input["date"],input["hours"],input["minutes"],input["seconds"],!input["milliseconds"] ? 0 : input["milliseconds"]);
			if(isNaN(retValue.getTime()))
				throw Error.format(_u.ExtensibilityStrings.l_InvalidDate_Text);
			return retValue
		},
		_dateToDictionary$i$0: function(input)
		{
			var retValue={};
			retValue["month"]=input.getMonth();
			retValue["date"]=input.getDate();
			retValue["year"]=input.getFullYear();
			retValue["hours"]=input.getHours();
			retValue["minutes"]=input.getMinutes();
			retValue["seconds"]=input.getSeconds();
			retValue["milliseconds"]=input.getMilliseconds();
			return retValue
		},
		_displayNewAppointmentFormApi$p$0: function(parameters)
		{
			var normalizedRequiredAttendees=OSF.DDA.OutlookAppOm._validateAndNormalizeRecipientEmails$p(parameters["requiredAttendees"],"requiredAttendees");
			var normalizedOptionalAttendees=OSF.DDA.OutlookAppOm._validateAndNormalizeRecipientEmails$p(parameters["optionalAttendees"],"optionalAttendees");
			OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p(parameters["location"],0,OSF.DDA.OutlookAppOm._maxLocationLength$p,"location");
			OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p(parameters["body"],0,OSF.DDA.OutlookAppOm.maxBodyLength,"body");
			OSF.DDA.OutlookAppOm._validateOptionalStringParameter$p(parameters["subject"],0,OSF.DDA.OutlookAppOm._maxSubjectLength$p,"subject");
			if(!$h.ScriptHelpers.isNullOrUndefined(parameters["start"]))
			{
				OSF.DDA.OutlookAppOm._throwOnArgumentType$p(parameters["start"],Date,"start");
				var startDateTime=parameters["start"];
				parameters["start"]=startDateTime.getTime();
				if(!$h.ScriptHelpers.isNullOrUndefined(parameters["end"]))
				{
					OSF.DDA.OutlookAppOm._throwOnArgumentType$p(parameters["end"],Date,"end");
					var endDateTime=parameters["end"];
					if(endDateTime < startDateTime)
						throw Error.argumentOutOfRange("end",endDateTime,_u.ExtensibilityStrings.l_InvalidEventDates_Text);
					parameters["end"]=endDateTime.getTime()
				}
			}
			var updatedParameters=null;
			if(normalizedRequiredAttendees || normalizedOptionalAttendees)
			{
				updatedParameters={};
				var $$dict_7=parameters;
				for(var $$key_8 in $$dict_7)
				{
					var entry={
							key: $$key_8,
							value: $$dict_7[$$key_8]
						};
					updatedParameters[entry.key]=entry.value
				}
				if(normalizedRequiredAttendees)
					updatedParameters["requiredAttendees"]=normalizedRequiredAttendees;
				if(normalizedOptionalAttendees)
					updatedParameters["optionalAttendees"]=normalizedOptionalAttendees
			}
			this._invokeHostMethod$i$0(7,"DisplayNewAppointmentForm",updatedParameters || parameters,null)
		},
		_initializeMethods$p$0: function()
		{
			var currentInstance=this;
			if($h.Item.isInstanceOfType(this._item$p$0))
				currentInstance.displayNewAppointmentForm=this.$$d__displayNewAppointmentFormApi$p$0
		},
		_getInitialDataResponseHandler$p$0: function(resultCode, data)
		{
			if(resultCode)
				return;
			this.initialize(data);
			this.displayName="mailbox";
			window.setTimeout(this.$$d__callAppReadyCallback$p$0,0)
		},
		_callAppReadyCallback$p$0: function()
		{
			this._appReadyCallback$p$0()
		},
		_invokeGetTokenMethodAsync$p$0: function(outlookDispid, methodName, callback, userContext)
		{
			if($h.ScriptHelpers.isNullOrUndefined(callback))
				throw Error.argumentNull("callback");
			var $$t_8=this;
			this._invokeHostMethod$i$0(outlookDispid,methodName,null,function(resultCode, response)
			{
				var asyncResult;
				if(resultCode)
					asyncResult=$$t_8.createAsyncResult(null,1,9017,userContext,String.format(_u.ExtensibilityStrings.l_InternalProtocolError_Text,resultCode));
				else
				{
					var responseDictionary=response;
					if(responseDictionary["wasSuccessful"])
						asyncResult=$$t_8.createAsyncResult(responseDictionary["token"],0,0,userContext,null);
					else
						asyncResult=$$t_8.createAsyncResult(null,1,responseDictionary["errorCode"],userContext,responseDictionary["errorMessage"])
				}
				callback(asyncResult)
			})
		},
		_getItem$p$0: function()
		{
			return this._item$p$0
		},
		_getUserProfile$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._initialData$p$0._permissionLevel$p$0);
			return this._userProfile$p$0
		},
		_getDiagnostics$p$0: function()
		{
			return this._diagnostics$p$0
		},
		_getEwsUrl$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._initialData$p$0._permissionLevel$p$0);
			return this._initialData$p$0.get__ewsUrl$i$0()
		},
		_findOffset$p$0: function(value)
		{
			var ranges=this._initialData$p$0.get__timeZoneOffsets$i$0();
			for(var r=0; r < ranges.length; r++)
			{
				var range=ranges[r];
				var start=parseInt(range["start"]);
				var end=parseInt(range["end"]);
				if(value.getTime() - start >=0 && value.getTime() - end < 0)
					return parseInt(range["offset"])
			}
			throw Error.format(_u.ExtensibilityStrings.l_InvalidDate_Text);
		},
		_areStringsLoaded$p$0: function()
		{
			var stringsLoaded=false;
			try
			{
				stringsLoaded=!$h.ScriptHelpers.isNullOrUndefined(_u.ExtensibilityStrings.l_EwsRequestOversized_Text)
			}
			catch($$e_1){}
			return stringsLoaded
		},
		_loadLocalizedScript$p$0: function(stringLoadedCallback)
		{
			var url=null;
			var baseUrl="";
			var scripts=document.getElementsByTagName("script");
			for(var i=scripts.length - 1; i >=0; i--)
			{
				var filename=null;
				var attributes=scripts[i].attributes;
				if(attributes)
				{
					var attribute=attributes.getNamedItem("src");
					if(attribute)
						filename=attribute.value;
					if(filename)
					{
						var debug=false;
						filename=filename.toLowerCase();
						var officeIndex=filename.indexOf("office_strings.js");
						if(officeIndex < 0)
						{
							officeIndex=filename.indexOf("office_strings.debug.js");
							debug=true
						}
						if(officeIndex > 0 && officeIndex < filename.length)
						{
							url=filename.replace(debug ? "office_strings.debug.js" : "office_strings.js","outlook_strings.js");
							var languageUrl=filename.substring(0,officeIndex);
							var lastIndexOfSlash=languageUrl.lastIndexOf("/",languageUrl.length - 2);
							if(lastIndexOfSlash===-1)
								lastIndexOfSlash=languageUrl.lastIndexOf("\\",languageUrl.length - 2);
							if(lastIndexOfSlash !==-1 && languageUrl.length > lastIndexOfSlash+1)
								baseUrl=languageUrl.substring(0,lastIndexOfSlash+1);
							break
						}
					}
				}
			}
			if(url)
			{
				var head=document.getElementsByTagName("head")[0];
				var scriptElement=null;
				var $$t_H=this;
				var scriptElementCallback=function()
					{
						if(stringLoadedCallback && (!scriptElement.readyState || scriptElement.readyState && (scriptElement.readyState==="loaded" || scriptElement.readyState==="complete")))
						{
							scriptElement.onload=null;
							scriptElement.onreadystatechange=null;
							stringLoadedCallback()
						}
					};
				var $$t_I=this;
				var failureCallback=function()
					{
						if(!$$t_I._areStringsLoaded$p$0())
						{
							var fallbackUrl=baseUrl+"en-us/"+"outlook_strings.js";
							scriptElement.onload=null;
							scriptElement.onreadystatechange=null;
							scriptElement=$$t_I._createScriptElement$p$0(fallbackUrl);
							scriptElement.onload=scriptElementCallback;
							scriptElement.onreadystatechange=scriptElementCallback;
							head.appendChild(scriptElement)
						}
					};
				scriptElement=this._createScriptElement$p$0(url);
				scriptElement.onload=scriptElementCallback;
				scriptElement.onreadystatechange=scriptElementCallback;
				window.setTimeout(failureCallback,2e3);
				head.appendChild(scriptElement)
			}
		},
		_createScriptElement$p$0: function(url)
		{
			var scriptElement=document.createElement("script");
			scriptElement.type="text/javascript";
			scriptElement.src=url;
			return scriptElement
		}
	};
	OSF.DDA.Settings=function(data)
	{
		this._rawData$p$0=data
	};
	OSF.DDA.Settings._convertFromRawSettings$p=function(rawSettings)
	{
		if(!rawSettings)
			return{};
		if(OSF.DDA.OutlookAppOm._instance$p.get__appName$i$0()===8)
		{
			var outlookSettings=rawSettings["SettingsKey"];
			if(outlookSettings)
				return OSF.DDA.SettingsManager.deserializeSettings(outlookSettings)
		}
		return rawSettings
	};
	OSF.DDA.Settings.prototype={
		_rawData$p$0: null,
		_settingsData$p$0: null,
		get__data$p$0: function()
		{
			if(!this._settingsData$p$0)
			{
				this._settingsData$p$0=OSF.DDA.Settings._convertFromRawSettings$p(this._rawData$p$0);
				this._rawData$p$0=null
			}
			return this._settingsData$p$0
		},
		get: function(name)
		{
			return this.get__data$p$0()[name]
		},
		set: function(name, value)
		{
			this.get__data$p$0()[name]=value
		},
		remove: function(name)
		{
			delete this.get__data$p$0()[name]
		},
		saveAsync: function()
		{
			var args=[];
			for(var $$pai_4=0; $$pai_4 < arguments.length;++$$pai_4)
				args[$$pai_4]=arguments[$$pai_4];
			var commonParameters=$h.CommonParameters.parse(args,false);
			if(JSON.stringify(OSF.DDA.SettingsManager.serializeSettings(this.get__data$p$0())).length > 32768)
			{
				var asyncResult=OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(null,1,9019,commonParameters._asyncContext$p$0,"");
				var $$t_3=this;
				window.setTimeout(function()
				{
					commonParameters._callback$p$0(asyncResult)
				},0);
				return
			}
			if(OSF.DDA.OutlookAppOm._instance$p.get__appName$i$0()===64)
				this._saveSettingsForOwa$p$0(commonParameters._callback$p$0,commonParameters._asyncContext$p$0);
			else
				this._saveSettingsForOutlook$p$0(commonParameters._callback$p$0,commonParameters._asyncContext$p$0)
		},
		_saveSettingsForOutlook$p$0: function(callback, userContext)
		{
			var storedException=null;
			try
			{
				var serializedSettings=OSF.DDA.SettingsManager.serializeSettings(this.get__data$p$0());
				var jsonSettings=JSON.stringify(serializedSettings);
				var settingsObjectToSave={SettingsKey: jsonSettings};
				OSF.DDA.ClientSettingsManager.write(settingsObjectToSave)
			}
			catch(ex)
			{
				storedException=ex
			}
			if(callback)
			{
				var asyncResult;
				if(storedException)
					asyncResult=OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(null,1,9019,userContext,storedException.message);
				else
					asyncResult=OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(null,0,0,userContext,null);
				callback(asyncResult)
			}
		},
		_saveSettingsForOwa$p$0: function(callback, userContext)
		{
			var serializedSettings=OSF.DDA.SettingsManager.serializeSettings(this.get__data$p$0());
			var $$t_7=this;
			OSF._OfficeAppFactory.getClientEndPoint().invoke("saveSettingsAsync",function(resultCode, response)
			{
				if(callback)
				{
					var asyncResult;
					if(resultCode)
						asyncResult=OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(null,1,9017,userContext,String.format(_u.ExtensibilityStrings.l_InternalProtocolError_Text,resultCode));
					else
					{
						var responseDictionary=response;
						if(!responseDictionary["error"])
							asyncResult=OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(null,0,0,userContext,null);
						else
							asyncResult=OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(null,1,9019,userContext,responseDictionary["errorMessage"])
					}
					callback(asyncResult)
				}
			},[serializedSettings])
		}
	};
	Type.registerNamespace("$h");
	Type.registerNamespace("Office.cast");
	$h.Appointment=function(dataDictionary)
	{
		this.$$d__getOrganizer$p$2=Function.createDelegate(this,this._getOrganizer$p$2);
		this.$$d__getNormalizedSubject$p$2=Function.createDelegate(this,this._getNormalizedSubject$p$2);
		this.$$d__getSubject$p$2=Function.createDelegate(this,this._getSubject$p$2);
		this.$$d__getResources$p$2=Function.createDelegate(this,this._getResources$p$2);
		this.$$d__getRequiredAttendees$p$2=Function.createDelegate(this,this._getRequiredAttendees$p$2);
		this.$$d__getOptionalAttendees$p$2=Function.createDelegate(this,this._getOptionalAttendees$p$2);
		this.$$d__getLocation$p$2=Function.createDelegate(this,this._getLocation$p$2);
		this.$$d__getEnd$p$2=Function.createDelegate(this,this._getEnd$p$2);
		this.$$d__getStart$p$2=Function.createDelegate(this,this._getStart$p$2);
		$h.Appointment.initializeBase(this,[dataDictionary]);
		$h.InitialData._defineReadOnlyProperty$i(this,"start",this.$$d__getStart$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"end",this.$$d__getEnd$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"location",this.$$d__getLocation$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"optionalAttendees",this.$$d__getOptionalAttendees$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"requiredAttendees",this.$$d__getRequiredAttendees$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"resources",this.$$d__getResources$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"subject",this.$$d__getSubject$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"normalizedSubject",this.$$d__getNormalizedSubject$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"organizer",this.$$d__getOrganizer$p$2)
	};
	$h.Appointment.prototype={
		getEntities: function()
		{
			return this._data$p$0._getEntities$i$0()
		},
		getEntitiesByType: function(entityType)
		{
			return this._data$p$0._getEntitiesByType$i$0(entityType)
		},
		getRegExMatches: function()
		{
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"getRegExMatches");
			return this._data$p$0._getRegExMatches$i$0()
		},
		getFilteredEntitiesByName: function(name)
		{
			return this._data$p$0._getFilteredEntitiesByName$i$0(name)
		},
		getRegExMatchesByName: function(name)
		{
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"getRegExMatchesByName");
			return this._data$p$0._getRegExMatchesByName$i$0(name)
		},
		displayReplyForm: function(obj)
		{
			OSF.DDA.OutlookAppOm._instance$p._displayReplyForm$i$0(obj)
		},
		displayReplyAllForm: function(obj)
		{
			OSF.DDA.OutlookAppOm._instance$p._displayReplyAllForm$i$0(obj)
		},
		getItemType: function()
		{
			return Microsoft.Office.WebExtension.MailboxEnums.ItemType.Appointment
		},
		_getStart$p$2: function()
		{
			return this._data$p$0.get__start$i$0()
		},
		_getEnd$p$2: function()
		{
			return this._data$p$0.get__end$i$0()
		},
		_getLocation$p$2: function()
		{
			return this._data$p$0.get__location$i$0()
		},
		_getOptionalAttendees$p$2: function()
		{
			return this._data$p$0.get__cc$i$0()
		},
		_getRequiredAttendees$p$2: function()
		{
			return this._data$p$0.get__to$i$0()
		},
		_getResources$p$2: function()
		{
			return this._data$p$0.get__resources$i$0()
		},
		_getSubject$p$2: function()
		{
			return this._data$p$0.get__subject$i$0()
		},
		_getNormalizedSubject$p$2: function()
		{
			return this._data$p$0.get__normalizedSubject$i$0()
		},
		_getOrganizer$p$2: function()
		{
			return this._data$p$0.get__organizer$i$0()
		}
	};
	$h.AppointmentCompose=function(data)
	{
		this.$$d__getLocation$p$2=Function.createDelegate(this,this._getLocation$p$2);
		this.$$d__getEnd$p$2=Function.createDelegate(this,this._getEnd$p$2);
		this.$$d__getStart$p$2=Function.createDelegate(this,this._getStart$p$2);
		this.$$d__getOptionalAttendees$p$2=Function.createDelegate(this,this._getOptionalAttendees$p$2);
		this.$$d__getRequiredAttendees$p$2=Function.createDelegate(this,this._getRequiredAttendees$p$2);
		$h.AppointmentCompose.initializeBase(this,[data]);
		$h.InitialData._defineReadOnlyProperty$i(this,"requiredAttendees",this.$$d__getRequiredAttendees$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"optionalAttendees",this.$$d__getOptionalAttendees$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"start",this.$$d__getStart$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"end",this.$$d__getEnd$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"location",this.$$d__getLocation$p$2)
	};
	$h.AppointmentCompose.prototype={
		_requiredAttendees$p$2: null,
		_optionalAttendees$p$2: null,
		_start$p$2: null,
		_end$p$2: null,
		_location$p$2: null,
		getItemType: function()
		{
			return Microsoft.Office.WebExtension.MailboxEnums.ItemType.Appointment
		},
		_getRequiredAttendees$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._requiredAttendees$p$2)
				this._requiredAttendees$p$2=new $h.ComposeRecipient(0,"requiredAttendees");
			return this._requiredAttendees$p$2
		},
		_getOptionalAttendees$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._optionalAttendees$p$2)
				this._optionalAttendees$p$2=new $h.ComposeRecipient(1,"optionalAttendees");
			return this._optionalAttendees$p$2
		},
		_getStart$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._start$p$2)
				this._start$p$2=new $h.ComposeTime(1);
			return this._start$p$2
		},
		_getEnd$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._end$p$2)
				this._end$p$2=new $h.ComposeTime(2);
			return this._end$p$2
		},
		_getLocation$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._location$p$2)
				this._location$p$2=new $h.ComposeLocation;
			return this._location$p$2
		}
	};
	$h.AttachmentConstants=function(){};
	$h.AttachmentDetails=function(data)
	{
		this.$$d__getIsInline$p$0=Function.createDelegate(this,this._getIsInline$p$0);
		this.$$d__getAttachmentType$p$0=Function.createDelegate(this,this._getAttachmentType$p$0);
		this.$$d__getSize$p$0=Function.createDelegate(this,this._getSize$p$0);
		this.$$d__getContentType$p$0=Function.createDelegate(this,this._getContentType$p$0);
		this.$$d__getName$p$0=Function.createDelegate(this,this._getName$p$0);
		this.$$d__getId$p$0=Function.createDelegate(this,this._getId$p$0);
		this._data$p$0=data;
		$h.InitialData._defineReadOnlyProperty$i(this,"id",this.$$d__getId$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"name",this.$$d__getName$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"contentType",this.$$d__getContentType$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"size",this.$$d__getSize$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"attachmentType",this.$$d__getAttachmentType$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"isInline",this.$$d__getIsInline$p$0)
	};
	$h.AttachmentDetails.prototype={
		_data$p$0: null,
		_getId$p$0: function()
		{
			return this._data$p$0["id"]
		},
		_getName$p$0: function()
		{
			return this._data$p$0["name"]
		},
		_getContentType$p$0: function()
		{
			return this._data$p$0["contentType"]
		},
		_getSize$p$0: function()
		{
			return this._data$p$0["size"]
		},
		_getAttachmentType$p$0: function()
		{
			var response=this._data$p$0["attachmentType"];
			return response < $h.AttachmentDetails._attachmentTypeMap$p.length ? $h.AttachmentDetails._attachmentTypeMap$p[response] : Microsoft.Office.WebExtension.MailboxEnums.AttachmentType.File
		},
		_getIsInline$p$0: function()
		{
			return this._data$p$0["isInline"]
		}
	};
	$h.Body=function(){};
	$h.Body._createParameterDictionaryToHost$i=function(data, parameters)
	{
		var dataToHost={data: data};
		if(parameters._options$p$0 && !$h.ScriptHelpers.isNull(parameters._options$p$0["coercionType"]))
		{
			var hostCoercionType;
			var $$t_4,
				$$t_5;
			if(!($$t_5=$h.Body._tryMapToHostCoercionType$i(parameters._options$p$0["coercionType"],$$t_4={val: hostCoercionType}),hostCoercionType=$$t_4.val,$$t_5))
			{
				if(parameters._callback$p$0)
					parameters._callback$p$0(OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(null,1,1e3,parameters._asyncContext$p$0,null));
				return null
			}
			dataToHost["coercionType"]=hostCoercionType
		}
		else
			dataToHost["coercionType"]=0;
		return dataToHost
	};
	$h.Body._tryMapToHostCoercionType$i=function(coercionType, hostCoercionType)
	{
		hostCoercionType.val=undefined;
		if(coercionType===Microsoft.Office.WebExtension.CoercionType.Html)
			hostCoercionType.val=3;
		else if(coercionType===Microsoft.Office.WebExtension.CoercionType.Text)
			hostCoercionType.val=0;
		else
			return false;
		return true
	};
	$h.Body.prototype={getAsync: function()
		{
			var args=[];
			for(var $$pai_3=0; $$pai_3 < arguments.length;++$$pai_3)
				args[$$pai_3]=arguments[$$pai_3];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"body.getAsync");
			var commonParameters=$h.CommonParameters.parse(args,true);
			var dataToHost=$h.Body._createParameterDictionaryToHost$i(null,commonParameters);
			if(!dataToHost)
				return;
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(37,"GetBodyAsync",dataToHost,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		}};
	$h.ComposeBody=function()
	{
		$h.ComposeBody.initializeBase(this)
	};
	$h.ComposeBody.prototype={
		getTypeAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"body.getTypeAsync");
			var parameters=$h.CommonParameters.parse(args,true);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(14,"GetBodyTypeAsync",null,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		setSelectedDataAsync: function(data)
		{
			var args=[];
			for(var $$pai_4=1; $$pai_4 < arguments.length;++$$pai_4)
				args[$$pai_4 - 1]=arguments[$$pai_4];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"body.setSelectedDataAsync");
			var parameters=$h.CommonParameters.parse(args,false);
			if(!String.isInstanceOfType(data))
				throw Error.argumentType("data",Object.getType(data),String);
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(data.length,0,1e6,"data");
			var dataToHost=$h.Body._createParameterDictionaryToHost$i(data,parameters);
			if(!dataToHost)
				return;
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(13,"BodySetSelectedDataAsync",dataToHost,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		prependAsync: function(data)
		{
			var args=[];
			for(var $$pai_4=1; $$pai_4 < arguments.length;++$$pai_4)
				args[$$pai_4 - 1]=arguments[$$pai_4];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"body.prependAsync");
			var parameters=$h.CommonParameters.parse(args,false);
			if(!String.isInstanceOfType(data))
				throw Error.argumentType("data",Object.getType(data),String);
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(data.length,0,1e6,"data");
			var dataToHost=$h.Body._createParameterDictionaryToHost$i(data,parameters);
			if(!dataToHost)
				return;
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(23,"BodyPrependAsync",dataToHost,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		setAsync: function(data)
		{
			var args=[];
			for(var $$pai_4=1; $$pai_4 < arguments.length;++$$pai_4)
				args[$$pai_4 - 1]=arguments[$$pai_4];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"body.setAsync");
			var parameters=$h.CommonParameters.parse(args,false);
			if(!String.isInstanceOfType(data))
				throw Error.argumentType("data",Object.getType(data),String);
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(data.length,0,1e6,"data");
			var dataToHost=$h.Body._createParameterDictionaryToHost$i(data,parameters);
			if(!dataToHost)
				return;
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(38,"SetBodyAsync",dataToHost,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		}
	};
	$h.ComposeItem=function(data)
	{
		this.$$d__getBody$p$1=Function.createDelegate(this,this._getBody$p$1);
		this.$$d__getSubject$p$1=Function.createDelegate(this,this._getSubject$p$1);
		$h.ComposeItem.initializeBase(this,[data]);
		$h.InitialData._defineReadOnlyProperty$i(this,"subject",this.$$d__getSubject$p$1);
		$h.InitialData._defineReadOnlyProperty$i(this,"body",this.$$d__getBody$p$1)
	};
	$h.ComposeItem.prototype={
		_subject$p$1: null,
		_body$p$1: null,
		addFileAttachmentAsync: function(uri, attachmentName)
		{
			var args=[];
			for(var $$pai_5=2; $$pai_5 < arguments.length;++$$pai_5)
				args[$$pai_5 - 2]=arguments[$$pai_5];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"addFileAttachmentAsync");
			if(!$h.ScriptHelpers.isNonEmptyString(uri))
				throw Error.argument("uri");
			if(!$h.ScriptHelpers.isNonEmptyString(attachmentName))
				throw Error.argument("attachmentName");
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(uri.length,0,2048,"uri");
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(attachmentName.length,0,255,"attachmentName");
			var commonParameters=$h.CommonParameters.parse(args,false);
			var parameters={
					uri: uri,
					name: attachmentName,
					__timeout__: 6e5
				};
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(16,"AddFileAttachmentAsync",parameters,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		},
		addItemAttachmentAsync: function(itemId, attachmentName)
		{
			var args=[];
			for(var $$pai_5=2; $$pai_5 < arguments.length;++$$pai_5)
				args[$$pai_5 - 2]=arguments[$$pai_5];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"addItemAttachmentAsync");
			if(!$h.ScriptHelpers.isNonEmptyString(itemId))
				throw Error.argument("itemId");
			if(!$h.ScriptHelpers.isNonEmptyString(attachmentName))
				throw Error.argument("attachmentName");
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(itemId.length,0,200,"itemId");
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(attachmentName.length,0,255,"attachmentName");
			var commonParameters=$h.CommonParameters.parse(args,false);
			var parameters={
					itemId: itemId,
					name: attachmentName,
					__timeout__: 6e5
				};
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(19,"AddItemAttachmentAsync",parameters,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		},
		removeAttachmentAsync: function(attachmentId)
		{
			var args=[];
			for(var $$pai_3=1; $$pai_3 < arguments.length;++$$pai_3)
				args[$$pai_3 - 1]=arguments[$$pai_3];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"removeAttachmentAsync");
			if(!$h.ScriptHelpers.isNonEmptyString(attachmentId))
				throw Error.argument("attachmentId");
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(attachmentId.length,0,200,"attachmentId");
			var commonParameters=$h.CommonParameters.parse(args,false);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(20,"RemoveAttachmentAsync",{attachmentIndex: attachmentId},null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		},
		getSelectedDataAsync: function(coercionType)
		{
			var args=[];
			for(var $$pai_7=1; $$pai_7 < arguments.length;++$$pai_7)
				args[$$pai_7 - 1]=arguments[$$pai_7];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"getSelectedDataAsync");
			var commonParameters=$h.CommonParameters.parse(args,true);
			var hostCoercionType;
			var $$t_5,
				$$t_6;
			if(coercionType !==Microsoft.Office.WebExtension.CoercionType.Html && coercionType !==Microsoft.Office.WebExtension.CoercionType.Text || !($$t_6=$h.Body._tryMapToHostCoercionType$i(coercionType,$$t_5={val: hostCoercionType}),hostCoercionType=$$t_5.val,$$t_6))
				throw Error.argument("coercionType");
			var dataToHost={coercionType: hostCoercionType};
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(28,"GetSelectedDataAsync",dataToHost,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		},
		setSelectedDataAsync: function(data)
		{
			var args=[];
			for(var $$pai_4=1; $$pai_4 < arguments.length;++$$pai_4)
				args[$$pai_4 - 1]=arguments[$$pai_4];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"setSelectedDataAsync");
			var parameters=$h.CommonParameters.parse(args,false);
			if(!String.isInstanceOfType(data))
				throw Error.argumentType("data",Object.getType(data),String);
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(data.length,0,1e6,"data");
			var dataToHost=$h.Body._createParameterDictionaryToHost$i(data,parameters);
			if(!dataToHost)
				return;
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(29,"SetSelectedDataAsync",dataToHost,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		close: function()
		{
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(41,"Close",null,null,null,null)
		},
		_getBody$p$1: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._body$p$1)
				this._body$p$1=new $h.ComposeBody;
			return this._body$p$1
		},
		_getSubject$p$1: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._subject$p$1)
				this._subject$p$1=new $h.ComposeSubject;
			return this._subject$p$1
		},
		saveAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"saveAsync");
			var parameters=$h.CommonParameters.parse(args,false);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(32,"SaveAsync",null,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		}
	};
	$h.ComposeRecipient=function(type, propertyName)
	{
		this._type$p$0=type;
		this._propertyName$p$0=propertyName
	};
	$h.ComposeRecipient._throwOnInvalidDisplayNameOrEmail$p=function(displayName, emailAddress)
	{
		if(!displayName && !emailAddress)
			throw Error.argument("recipients");
		if(displayName && displayName.length > 255)
			throw Error.argumentOutOfRange("recipients",displayName.length,_u.ExtensibilityStrings.l_DisplayNameTooLong_Text);
		if(emailAddress && emailAddress.length > 571)
			throw Error.argumentOutOfRange("recipients",emailAddress.length,_u.ExtensibilityStrings.l_EmailAddressTooLong_Text);
	};
	$h.ComposeRecipient._getAsyncFormatter$p=function(rawInput)
	{
		var input=rawInput;
		var output=[];
		for(var i=0; i < input.length; i++)
		{
			var email=new $h.EmailAddressDetails(input[i]);
			output[i]=email
		}
		return output
	};
	$h.ComposeRecipient._createEmailDictionaryForHost$p=function(address, name)
	{
		return{
				address: address,
				name: name
			}
	};
	$h.ComposeRecipient.prototype={
		_propertyName$p$0: null,
		_type$p$0: 0,
		getAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			var parameters=$h.CommonParameters.parse(args,true);
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,this._propertyName$p$0+".getAsync");
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(15,"GetRecipientsAsync",{recipientField: this._type$p$0},$h.ComposeRecipient._getAsyncFormatter$p,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		setAsync: function(recipients)
		{
			var args=[];
			for(var $$pai_2=1; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2 - 1]=arguments[$$pai_2];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,this._propertyName$p$0+".setAsync");
			this.setAddHelper(recipients,args,true)
		},
		addAsync: function(recipients)
		{
			var args=[];
			for(var $$pai_2=1; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2 - 1]=arguments[$$pai_2];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,this._propertyName$p$0+".addAsync");
			this.setAddHelper(recipients,args,false)
		},
		setAddHelper: function(recipients, args, isSet)
		{
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(recipients.length,0,100,"recipients");
			var parameters=$h.CommonParameters.parse(args,false);
			var recipientData=[];
			if(Array.isInstanceOfType(recipients))
				for(var i=0; i < recipients.length; i++)
					if(String.isInstanceOfType(recipients[i]))
					{
						$h.ComposeRecipient._throwOnInvalidDisplayNameOrEmail$p(recipients[i],recipients[i]);
						recipientData[i]=$h.ComposeRecipient._createEmailDictionaryForHost$p(recipients[i],recipients[i])
					}
					else if($h.EmailAddressDetails.isInstanceOfType(recipients[i]))
					{
						var address=recipients[i];
						$h.ComposeRecipient._throwOnInvalidDisplayNameOrEmail$p(address.displayName,address.emailAddress);
						recipientData[i]=$h.ComposeRecipient._createEmailDictionaryForHost$p(address.emailAddress,address.displayName)
					}
					else if(Object.isInstanceOfType(recipients[i]))
					{
						var input=recipients[i];
						var emailAddress=input["emailAddress"];
						var displayName=input["displayName"];
						$h.ComposeRecipient._throwOnInvalidDisplayNameOrEmail$p(displayName,emailAddress);
						recipientData[i]=$h.ComposeRecipient._createEmailDictionaryForHost$p(emailAddress,displayName)
					}
					else
						throw Error.argument("recipients");
			else
				throw Error.argument("recipients");
			var $$t_B=this;
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(isSet ? 21 : 22,isSet ? "SetRecipientsAsync" : "AddRecipientsAsync",{
				recipientField: this._type$p$0,
				recipientArray: recipientData
			},function(rawInput)
			{
				return rawInput
			},parameters._asyncContext$p$0,parameters._callback$p$0)
		}
	};
	$h.ComposeRecipient.RecipientField=function(){};
	$h.ComposeRecipient.RecipientField.prototype={
		to: 0,
		cc: 1,
		bcc: 2,
		requiredAttendees: 0,
		optionalAttendees: 1
	};
	$h.ComposeRecipient.RecipientField.registerEnum("$h.ComposeRecipient.RecipientField",false);
	$h.ComposeLocation=function(){};
	$h.ComposeLocation.prototype={
		getAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"location.getAsync");
			var parameters=$h.CommonParameters.parse(args,true);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(26,"GetLocationAsync",null,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		setAsync: function(location)
		{
			var args=[];
			for(var $$pai_3=1; $$pai_3 < arguments.length;++$$pai_3)
				args[$$pai_3 - 1]=arguments[$$pai_3];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"location.setAsync");
			var parameters=$h.CommonParameters.parse(args,false);
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(location.length,0,255,"location");
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(27,"SetLocationAsync",{location: location},null,parameters._asyncContext$p$0,parameters._callback$p$0)
		}
	};
	$h.ComposeSubject=function(){};
	$h.ComposeSubject.prototype={
		getAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			var parameters=$h.CommonParameters.parse(args,true);
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"subject.getAsync");
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(18,"GetSubjectAsync",null,null,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		setAsync: function(data)
		{
			var args=[];
			for(var $$pai_3=1; $$pai_3 < arguments.length;++$$pai_3)
				args[$$pai_3 - 1]=arguments[$$pai_3];
			var parameters=$h.CommonParameters.parse(args,false);
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,"subject.setAsync");
			if(!String.isInstanceOfType(data))
				throw Error.argument("data");
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(data.length,0,255,"data");
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(17,"SetSubjectAsync",{subject: data},null,parameters._asyncContext$p$0,parameters._callback$p$0)
		}
	};
	$h.ComposeTime=function(type)
	{
		this.$$d__ticksToDateFormatter$p$0=Function.createDelegate(this,this._ticksToDateFormatter$p$0);
		this._timeType$p$0=type
	};
	$h.ComposeTime.prototype={
		_timeType$p$0: 0,
		getAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,this._getPropertyName$p$0()+".getAsync");
			var parameters=$h.CommonParameters.parse(args,true);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(24,"GetTimeAsync",{TimeProperty: this._timeType$p$0},this.$$d__ticksToDateFormatter$p$0,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		setAsync: function(dateTime)
		{
			var args=[];
			for(var $$pai_3=1; $$pai_3 < arguments.length;++$$pai_3)
				args[$$pai_3 - 1]=arguments[$$pai_3];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(2,this._getPropertyName$p$0()+".setAsync");
			if(!Date.isInstanceOfType(dateTime))
				throw Error.argumentType("dateTime",Object.getType(dateTime),Date);
			if(isNaN(dateTime.getTime()))
				throw Error.argument("dateTime");
			if(dateTime.getTime() < -864e13 || dateTime.getTime() > 864e13)
				throw Error.argumentOutOfRange("dateTime");
			var parameters=$h.CommonParameters.parse(args,false);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(25,"SetTimeAsync",{
				TimeProperty: this._timeType$p$0,
				time: dateTime.getTime()
			},null,parameters._asyncContext$p$0,parameters._callback$p$0)
		},
		_ticksToDateFormatter$p$0: function(rawInput)
		{
			var ticks=rawInput;
			return new Date(ticks)
		},
		_getPropertyName$p$0: function()
		{
			return this._timeType$p$0===1 ? "start" : "end"
		}
	};
	$h.ComposeTime.TimeType=function(){};
	$h.ComposeTime.TimeType.prototype={
		start: 1,
		end: 2
	};
	$h.ComposeTime.TimeType.registerEnum("$h.ComposeTime.TimeType",false);
	$h.Contact=function(data)
	{
		this.$$d__getContactString$p$0=Function.createDelegate(this,this._getContactString$p$0);
		this.$$d__getAddresses$p$0=Function.createDelegate(this,this._getAddresses$p$0);
		this.$$d__getUrls$p$0=Function.createDelegate(this,this._getUrls$p$0);
		this.$$d__getEmailAddresses$p$0=Function.createDelegate(this,this._getEmailAddresses$p$0);
		this.$$d__getPhoneNumbers$p$0=Function.createDelegate(this,this._getPhoneNumbers$p$0);
		this.$$d__getBusinessName$p$0=Function.createDelegate(this,this._getBusinessName$p$0);
		this.$$d__getPersonName$p$0=Function.createDelegate(this,this._getPersonName$p$0);
		this._data$p$0=data;
		$h.InitialData._defineReadOnlyProperty$i(this,"personName",this.$$d__getPersonName$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"businessName",this.$$d__getBusinessName$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"phoneNumbers",this.$$d__getPhoneNumbers$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"emailAddresses",this.$$d__getEmailAddresses$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"urls",this.$$d__getUrls$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"addresses",this.$$d__getAddresses$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"contactString",this.$$d__getContactString$p$0)
	};
	$h.Contact.prototype={
		_data$p$0: null,
		_phoneNumbers$p$0: null,
		_getPersonName$p$0: function()
		{
			return this._data$p$0["PersonName"]
		},
		_getBusinessName$p$0: function()
		{
			return this._data$p$0["BusinessName"]
		},
		_getAddresses$p$0: function()
		{
			return $h.Entities._getExtractedStringProperty$i(this._data$p$0,"Addresses")
		},
		_getEmailAddresses$p$0: function()
		{
			return $h.Entities._getExtractedStringProperty$i(this._data$p$0,"EmailAddresses")
		},
		_getUrls$p$0: function()
		{
			return $h.Entities._getExtractedStringProperty$i(this._data$p$0,"Urls")
		},
		_getPhoneNumbers$p$0: function()
		{
			if(!this._phoneNumbers$p$0)
			{
				var $$t_1=this;
				this._phoneNumbers$p$0=$h.Entities._getExtractedObjects$i($h.PhoneNumber,this._data$p$0,"PhoneNumbers",function(data)
				{
					return new $h.PhoneNumber(data)
				})
			}
			return this._phoneNumbers$p$0
		},
		_getContactString$p$0: function()
		{
			return this._data$p$0["ContactString"]
		}
	};
	$h.CustomProperties=function(data)
	{
		if($h.ScriptHelpers.isNullOrUndefined(data))
			throw Error.argumentNull("data");
		if(Array.isInstanceOfType(data))
		{
			var customPropertiesArray=data;
			if(customPropertiesArray.length > 0)
				this._data$p$0=customPropertiesArray[0];
			else
				throw Error.argument("data");
		}
		else
			this._data$p$0=data
	};
	$h.CustomProperties.prototype={
		_data$p$0: null,
		get: function(name)
		{
			var value=this._data$p$0[name];
			if(typeof value==="string")
			{
				var valueString=value;
				if(valueString.length > 6 && valueString.startsWith("Date(") && valueString.endsWith(")"))
				{
					var ticksString=valueString.substring(5,valueString.length - 1);
					var ticks=parseInt(ticksString);
					if(!isNaN(ticks))
					{
						var dateTimeValue=new Date(ticks);
						if(dateTimeValue)
							value=dateTimeValue
					}
				}
			}
			return value
		},
		set: function(name, value)
		{
			if(OSF.OUtil.isDate(value))
				value="Date("+value.getTime()+")";
			this._data$p$0[name]=value
		},
		remove: function(name)
		{
			delete this._data$p$0[name]
		},
		saveAsync: function()
		{
			var args=[];
			for(var $$pai_4=0; $$pai_4 < arguments.length;++$$pai_4)
				args[$$pai_4]=arguments[$$pai_4];
			var MaxCustomPropertiesLength=2500;
			if(JSON.stringify(this._data$p$0).length > MaxCustomPropertiesLength)
				throw Error.argument();
			var parameters=$h.CommonParameters.parse(args,false,true);
			var saveCustomProperties=new $h.SaveDictionaryRequest(parameters._callback$p$0,parameters._asyncContext$p$0);
			saveCustomProperties._sendRequest$i$0(4,"SaveCustomProperties",{customProperties: this._data$p$0})
		}
	};
	$h.Diagnostics=function(data, appName)
	{
		this.$$d__getOwaView$p$0=Function.createDelegate(this,this._getOwaView$p$0);
		this.$$d__getHostVersion$p$0=Function.createDelegate(this,this._getHostVersion$p$0);
		this.$$d__getHostName$p$0=Function.createDelegate(this,this._getHostName$p$0);
		this._data$p$0=data;
		this._appName$p$0=appName;
		$h.InitialData._defineReadOnlyProperty$i(this,"hostName",this.$$d__getHostName$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"hostVersion",this.$$d__getHostVersion$p$0);
		if(64===this._appName$p$0)
			$h.InitialData._defineReadOnlyProperty$i(this,"OWAView",this.$$d__getOwaView$p$0)
	};
	$h.Diagnostics.prototype={
		_data$p$0: null,
		_appName$p$0: 0,
		_getHostName$p$0: function()
		{
			if(8===this._appName$p$0)
				return"Outlook";
			else if(64===this._appName$p$0)
				return"OutlookWebApp";
			return null
		},
		_getHostVersion$p$0: function()
		{
			return this._data$p$0.get__hostVersion$i$0()
		},
		_getOwaView$p$0: function()
		{
			return this._data$p$0.get__owaView$i$0()
		}
	};
	$h.EmailAddressDetails=function(data)
	{
		this.$$d__getRecipientType$p$0=Function.createDelegate(this,this._getRecipientType$p$0);
		this.$$d__getAppointmentResponse$p$0=Function.createDelegate(this,this._getAppointmentResponse$p$0);
		this.$$d__getDisplayName$p$0=Function.createDelegate(this,this._getDisplayName$p$0);
		this.$$d__getEmailAddress$p$0=Function.createDelegate(this,this._getEmailAddress$p$0);
		this._data$p$0=data;
		$h.InitialData._defineReadOnlyProperty$i(this,"emailAddress",this.$$d__getEmailAddress$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"displayName",this.$$d__getDisplayName$p$0);
		if($h.ScriptHelpers.dictionaryContainsKey(data,"appointmentResponse"))
			$h.InitialData._defineReadOnlyProperty$i(this,"appointmentResponse",this.$$d__getAppointmentResponse$p$0);
		if($h.ScriptHelpers.dictionaryContainsKey(data,"recipientType"))
			$h.InitialData._defineReadOnlyProperty$i(this,"recipientType",this.$$d__getRecipientType$p$0)
	};
	$h.EmailAddressDetails._createFromEmailUserDictionary$i=function(data)
	{
		var emailAddressDetailsDictionary={};
		var displayName=data["Name"];
		var emailAddress=data["UserId"];
		emailAddressDetailsDictionary["name"]=displayName || $h.EmailAddressDetails._emptyString$p;
		emailAddressDetailsDictionary["address"]=emailAddress || $h.EmailAddressDetails._emptyString$p;
		return new $h.EmailAddressDetails(emailAddressDetailsDictionary)
	};
	$h.EmailAddressDetails.prototype={
		_data$p$0: null,
		toJSON: function()
		{
			var result={};
			result["emailAddress"]=this._getEmailAddress$p$0();
			result["displayName"]=this._getDisplayName$p$0();
			if($h.ScriptHelpers.dictionaryContainsKey(this._data$p$0,"appointmentResponse"))
				result["appointmentResponse"]=this._getAppointmentResponse$p$0();
			if($h.ScriptHelpers.dictionaryContainsKey(this._data$p$0,"recipientType"))
				result["recipientType"]=this._getRecipientType$p$0();
			return result
		},
		_getEmailAddress$p$0: function()
		{
			return this._data$p$0["address"]
		},
		_getDisplayName$p$0: function()
		{
			return this._data$p$0["name"]
		},
		_getAppointmentResponse$p$0: function()
		{
			var response=this._data$p$0["appointmentResponse"];
			return response < $h.EmailAddressDetails._responseTypeMap$p.length ? $h.EmailAddressDetails._responseTypeMap$p[response] : Microsoft.Office.WebExtension.MailboxEnums.ResponseType.None
		},
		_getRecipientType$p$0: function()
		{
			var response=this._data$p$0["recipientType"];
			return response < $h.EmailAddressDetails._recipientTypeMap$p.length ? $h.EmailAddressDetails._recipientTypeMap$p[response] : Microsoft.Office.WebExtension.MailboxEnums.RecipientType.Other
		}
	};
	$h.Entities=function(data, filteredEntitiesData, timeSent, permissionLevel)
	{
		this.$$d__createMeetingSuggestion$p$0=Function.createDelegate(this,this._createMeetingSuggestion$p$0);
		this.$$d__getParcelDeliveries$p$0=Function.createDelegate(this,this._getParcelDeliveries$p$0);
		this.$$d__getFlightReservations$p$0=Function.createDelegate(this,this._getFlightReservations$p$0);
		this.$$d__getContacts$p$0=Function.createDelegate(this,this._getContacts$p$0);
		this.$$d__getPhoneNumbers$p$0=Function.createDelegate(this,this._getPhoneNumbers$p$0);
		this.$$d__getUrls$p$0=Function.createDelegate(this,this._getUrls$p$0);
		this.$$d__getEmailAddresses$p$0=Function.createDelegate(this,this._getEmailAddresses$p$0);
		this.$$d__getMeetingSuggestions$p$0=Function.createDelegate(this,this._getMeetingSuggestions$p$0);
		this.$$d__getTaskSuggestions$p$0=Function.createDelegate(this,this._getTaskSuggestions$p$0);
		this.$$d__getAddresses$p$0=Function.createDelegate(this,this._getAddresses$p$0);
		this._data$p$0=data || {};
		this._filteredData$p$0=filteredEntitiesData || {};
		this._dateTimeSent$p$0=timeSent;
		$h.InitialData._defineReadOnlyProperty$i(this,"addresses",this.$$d__getAddresses$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"taskSuggestions",this.$$d__getTaskSuggestions$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"meetingSuggestions",this.$$d__getMeetingSuggestions$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"emailAddresses",this.$$d__getEmailAddresses$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"urls",this.$$d__getUrls$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"phoneNumbers",this.$$d__getPhoneNumbers$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"contacts",this.$$d__getContacts$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"flightReservations",this.$$d__getFlightReservations$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"parcelDeliveries",this.$$d__getParcelDeliveries$p$0);
		this._permissionLevel$p$0=permissionLevel
	};
	$h.Entities._getExtractedObjects$i=function(T, data, name, creator, removeDuplicates, stringPropertyName)
	{
		var results=null;
		var extractedObjects=data[name];
		if(!extractedObjects)
			return new Array(0);
		if(removeDuplicates)
			extractedObjects=$h.Entities._removeDuplicate$p(Object,extractedObjects,$h.Entities._entityDictionaryEquals$p,stringPropertyName);
		results=new Array(extractedObjects.length);
		var count=0;
		for(var $$arr_9=extractedObjects, $$len_A=$$arr_9.length, $$idx_B=0; $$idx_B < $$len_A;++$$idx_B)
		{
			var extractedObject=$$arr_9[$$idx_B];
			if(creator)
				results[count++]=creator(extractedObject);
			else
				results[count++]=extractedObject
		}
		return results
	};
	$h.Entities._getExtractedStringProperty$i=function(data, name, removeDuplicate)
	{
		var extractedProperties=data[name];
		if(!extractedProperties)
			return new Array(0);
		if(removeDuplicate)
			extractedProperties=$h.Entities._removeDuplicate$p(String,extractedProperties,$h.Entities._stringEquals$p,null);
		return extractedProperties
	};
	$h.Entities._createContact$p=function(data)
	{
		return new $h.Contact(data)
	};
	$h.Entities._createTaskSuggestion$p=function(data)
	{
		return new $h.TaskSuggestion(data)
	};
	$h.Entities._createPhoneNumber$p=function(data)
	{
		return new $h.PhoneNumber(data)
	};
	$h.Entities._entityDictionaryEquals$p=function(dictionary1, dictionary2, entityPropertyIdentifier)
	{
		if(dictionary1===dictionary2)
			return true;
		if(!dictionary1 || !dictionary2)
			return false;
		if(dictionary1[entityPropertyIdentifier]===dictionary2[entityPropertyIdentifier])
			return true;
		return false
	};
	$h.Entities._stringEquals$p=function(string1, string2, entityProperty)
	{
		return string1===string2
	};
	$h.Entities._removeDuplicate$p=function(T, array, entityEquals, entityPropertyIdentifier)
	{
		for(var matchIndex1=array.length - 1; matchIndex1 >=0; matchIndex1--)
		{
			var removeMatch=false;
			for(var matchIndex2=matchIndex1 - 1; matchIndex2 >=0; matchIndex2--)
				if(entityEquals(array[matchIndex1],array[matchIndex2],entityPropertyIdentifier))
				{
					removeMatch=true;
					break
				}
			if(removeMatch)
				Array.removeAt(array,matchIndex1)
		}
		return array
	};
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
		_flightReservations$p$0: null,
		_parcelDeliveries$p$0: null,
		_getByType$i$0: function(entityType)
		{
			if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.MeetingSuggestion)
				return this._getMeetingSuggestions$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.TaskSuggestion)
				return this._getTaskSuggestions$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.Address)
				return this._getAddresses$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.PhoneNumber)
				return this._getPhoneNumbers$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.EmailAddress)
				return this._getEmailAddresses$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.Url)
				return this._getUrls$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.Contact)
				return this._getContacts$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.FlightReservations)
				return this._getFlightReservations$p$0();
			else if(entityType===Microsoft.Office.WebExtension.MailboxEnums.EntityType.ParcelDeliveries)
				return this._getParcelDeliveries$p$0();
			return null
		},
		_getFilteredEntitiesByName$i$0: function(name)
		{
			if(!this._filteredEntitiesCache$p$0)
				this._filteredEntitiesCache$p$0={};
			if(!$h.ScriptHelpers.dictionaryContainsKey(this._filteredEntitiesCache$p$0,name))
			{
				var found=false;
				for(var i=0; i < $h.Entities._allEntityKeys$p.length; i++)
				{
					var entityTypeKey=$h.Entities._allEntityKeys$p[i];
					var perEntityTypeDictionary=this._filteredData$p$0[entityTypeKey];
					if(!perEntityTypeDictionary)
						continue;
					if($h.ScriptHelpers.dictionaryContainsKey(perEntityTypeDictionary,name))
					{
						switch(entityTypeKey)
						{
							case"EmailAddresses":
							case"Urls":
								this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedStringProperty$i(perEntityTypeDictionary,name);
								break;
							case"Addresses":
								this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedStringProperty$i(perEntityTypeDictionary,name,true);
								break;
							case"PhoneNumbers":
								this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.PhoneNumber,perEntityTypeDictionary,name,$h.Entities._createPhoneNumber$p,false,null);
								break;
							case"TaskSuggestions":
								this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.TaskSuggestion,perEntityTypeDictionary,name,$h.Entities._createTaskSuggestion$p,true,"TaskString");
								break;
							case"MeetingSuggestions":
								this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.MeetingSuggestion,perEntityTypeDictionary,name,this.$$d__createMeetingSuggestion$p$0,true,"MeetingString");
								break;
							case"Contacts":
								this._filteredEntitiesCache$p$0[name]=$h.Entities._getExtractedObjects$i($h.Contact,perEntityTypeDictionary,name,$h.Entities._createContact$p,true,"ContactString");
								break
						}
						found=true;
						break
					}
				}
				if(!found)
					this._filteredEntitiesCache$p$0[name]=null
			}
			return this._filteredEntitiesCache$p$0[name]
		},
		_createMeetingSuggestion$p$0: function(data)
		{
			return new $h.MeetingSuggestion(data,this._dateTimeSent$p$0)
		},
		_getAddresses$p$0: function()
		{
			if(!this._addresses$p$0)
				this._addresses$p$0=$h.Entities._getExtractedStringProperty$i(this._data$p$0,"Addresses",true);
			return this._addresses$p$0
		},
		_getEmailAddresses$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
			if(!this._emailAddresses$p$0)
				this._emailAddresses$p$0=$h.Entities._getExtractedStringProperty$i(this._data$p$0,"EmailAddresses",false);
			return this._emailAddresses$p$0
		},
		_getUrls$p$0: function()
		{
			if(!this._urls$p$0)
				this._urls$p$0=$h.Entities._getExtractedStringProperty$i(this._data$p$0,"Urls",false);
			return this._urls$p$0
		},
		_getPhoneNumbers$p$0: function()
		{
			if(!this._phoneNumbers$p$0)
				this._phoneNumbers$p$0=$h.Entities._getExtractedObjects$i($h.PhoneNumber,this._data$p$0,"PhoneNumbers",$h.Entities._createPhoneNumber$p);
			return this._phoneNumbers$p$0
		},
		_getTaskSuggestions$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
			if(!this._taskSuggestions$p$0)
				this._taskSuggestions$p$0=$h.Entities._getExtractedObjects$i($h.TaskSuggestion,this._data$p$0,"TaskSuggestions",$h.Entities._createTaskSuggestion$p,true,"TaskString");
			return this._taskSuggestions$p$0
		},
		_getMeetingSuggestions$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
			if(!this._meetingSuggestions$p$0)
				this._meetingSuggestions$p$0=$h.Entities._getExtractedObjects$i($h.MeetingSuggestion,this._data$p$0,"MeetingSuggestions",this.$$d__createMeetingSuggestion$p$0,true,"MeetingString");
			return this._meetingSuggestions$p$0
		},
		_getContacts$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
			if(!this._contacts$p$0)
				this._contacts$p$0=$h.Entities._getExtractedObjects$i($h.Contact,this._data$p$0,"Contacts",$h.Entities._createContact$p,true,"ContactString");
			return this._contacts$p$0
		},
		_getParcelDeliveries$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
			if(!this._parcelDeliveries$p$0)
				this._parcelDeliveries$p$0=$h.Entities._getExtractedObjects$i(Object,this._data$p$0,"ParcelDeliveries",null);
			return this._parcelDeliveries$p$0
		},
		_getFlightReservations$p$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0);
			if(!this._flightReservations$p$0)
				this._flightReservations$p$0=$h.Entities._getExtractedObjects$i(Object,this._data$p$0,"FlightReservations",null);
			return this._flightReservations$p$0
		}
	};
	$h.ReplyConstants=function(){};
	$h.AsyncConstants=function(){};
	Office.cast.item=function(){};
	Office.cast.item.toItemRead=function(item)
	{
		if($h.Item.isInstanceOfType(item))
			return item;
		throw Error.argumentType();
	};
	Office.cast.item.toItemCompose=function(item)
	{
		if($h.ComposeItem.isInstanceOfType(item))
			return item;
		throw Error.argumentType();
	};
	Office.cast.item.toMessage=function(item)
	{
		return Office.cast.item.toMessageRead(item)
	};
	Office.cast.item.toMessageRead=function(item)
	{
		if($h.Message.isInstanceOfType(item))
			return item;
		throw Error.argumentType();
	};
	Office.cast.item.toMessageCompose=function(item)
	{
		if($h.MessageCompose.isInstanceOfType(item))
			return item;
		throw Error.argumentType();
	};
	Office.cast.item.toMeetingRequest=function(item)
	{
		if($h.MeetingRequest.isInstanceOfType(item))
			return item;
		throw Error.argumentType();
	};
	Office.cast.item.toAppointment=function(item)
	{
		return Office.cast.item.toAppointmentRead(item)
	};
	Office.cast.item.toAppointmentRead=function(item)
	{
		if($h.Appointment.isInstanceOfType(item))
			return item;
		throw Error.argumentType();
	};
	Office.cast.item.toAppointmentCompose=function(item)
	{
		if($h.AppointmentCompose.isInstanceOfType(item))
			return item;
		throw Error.argumentType();
	};
	$h.Item=function(data)
	{
		this.$$d__getBody$p$1=Function.createDelegate(this,this._getBody$p$1);
		this.$$d__getAttachments$p$1=Function.createDelegate(this,this._getAttachments$p$1);
		this.$$d__getItemClass$p$1=Function.createDelegate(this,this._getItemClass$p$1);
		this.$$d__getItemId$p$1=Function.createDelegate(this,this._getItemId$p$1);
		this.$$d__getDateTimeModified$p$1=Function.createDelegate(this,this._getDateTimeModified$p$1);
		this.$$d__getDateTimeCreated$p$1=Function.createDelegate(this,this._getDateTimeCreated$p$1);
		$h.Item.initializeBase(this,[data]);
		$h.InitialData._defineReadOnlyProperty$i(this,"dateTimeCreated",this.$$d__getDateTimeCreated$p$1);
		$h.InitialData._defineReadOnlyProperty$i(this,"dateTimeModified",this.$$d__getDateTimeModified$p$1);
		$h.InitialData._defineReadOnlyProperty$i(this,"itemId",this.$$d__getItemId$p$1);
		$h.InitialData._defineReadOnlyProperty$i(this,"itemClass",this.$$d__getItemClass$p$1);
		$h.InitialData._defineReadOnlyProperty$i(this,"attachments",this.$$d__getAttachments$p$1);
		$h.InitialData._defineReadOnlyProperty$i(this,"body",this.$$d__getBody$p$1)
	};
	$h.Item.prototype={
		_body$p$1: null,
		_getItemId$p$1: function()
		{
			return this._data$p$0.get__itemId$i$0()
		},
		_getItemClass$p$1: function()
		{
			return this._data$p$0.get__itemClass$i$0()
		},
		_getDateTimeCreated$p$1: function()
		{
			return this._data$p$0.get__dateTimeCreated$i$0()
		},
		_getDateTimeModified$p$1: function()
		{
			return this._data$p$0.get__dateTimeModified$i$0()
		},
		_getAttachments$p$1: function()
		{
			return this._data$p$0.get__attachments$i$0()
		},
		_getBody$p$1: function()
		{
			if(!this._body$p$1)
				this._body$p$1=new $h.Body;
			return this._body$p$1
		}
	};
	$h.ItemBase=function(data)
	{
		this.$$d__createCustomProperties$i$0=Function.createDelegate(this,this._createCustomProperties$i$0);
		this.$$d__getNotificationMessages$p$0=Function.createDelegate(this,this._getNotificationMessages$p$0);
		this.$$d_getItemType=Function.createDelegate(this,this.getItemType);
		this._data$p$0=data;
		$h.InitialData._defineReadOnlyProperty$i(this,"itemType",this.$$d_getItemType);
		$h.InitialData._defineReadOnlyProperty$i(this,"notificationMessages",this.$$d__getNotificationMessages$p$0)
	};
	$h.ItemBase.prototype={
		_data$p$0: null,
		_notificationMessages$p$0: null,
		get_data: function()
		{
			return this._data$p$0
		},
		loadCustomPropertiesAsync: function()
		{
			var args=[];
			for(var $$pai_3=0; $$pai_3 < arguments.length;++$$pai_3)
				args[$$pai_3]=arguments[$$pai_3];
			var parameters=$h.CommonParameters.parse(args,true,true);
			var loadCustomProperties=new $h._loadDictionaryRequest(this.$$d__createCustomProperties$i$0,"customProperties",parameters._callback$p$0,parameters._asyncContext$p$0);
			loadCustomProperties._sendRequest$i$0(3,"LoadCustomProperties",{})
		},
		_createCustomProperties$i$0: function(data)
		{
			return new $h.CustomProperties(data)
		},
		_getNotificationMessages$p$0: function()
		{
			if(!this._notificationMessages$p$0)
				this._notificationMessages$p$0=new $h.NotificationMessages;
			return this._notificationMessages$p$0
		}
	};
	$h.MeetingRequest=function(data)
	{
		this.$$d__getRequiredAttendees$p$3=Function.createDelegate(this,this._getRequiredAttendees$p$3);
		this.$$d__getOptionalAttendees$p$3=Function.createDelegate(this,this._getOptionalAttendees$p$3);
		this.$$d__getLocation$p$3=Function.createDelegate(this,this._getLocation$p$3);
		this.$$d__getEnd$p$3=Function.createDelegate(this,this._getEnd$p$3);
		this.$$d__getStart$p$3=Function.createDelegate(this,this._getStart$p$3);
		$h.MeetingRequest.initializeBase(this,[data]);
		$h.InitialData._defineReadOnlyProperty$i(this,"start",this.$$d__getStart$p$3);
		$h.InitialData._defineReadOnlyProperty$i(this,"end",this.$$d__getEnd$p$3);
		$h.InitialData._defineReadOnlyProperty$i(this,"location",this.$$d__getLocation$p$3);
		$h.InitialData._defineReadOnlyProperty$i(this,"optionalAttendees",this.$$d__getOptionalAttendees$p$3);
		$h.InitialData._defineReadOnlyProperty$i(this,"requiredAttendees",this.$$d__getRequiredAttendees$p$3)
	};
	$h.MeetingRequest.prototype={
		_getStart$p$3: function()
		{
			return this._data$p$0.get__start$i$0()
		},
		_getEnd$p$3: function()
		{
			return this._data$p$0.get__end$i$0()
		},
		_getLocation$p$3: function()
		{
			return this._data$p$0.get__location$i$0()
		},
		_getOptionalAttendees$p$3: function()
		{
			return this._data$p$0.get__cc$i$0()
		},
		_getRequiredAttendees$p$3: function()
		{
			return this._data$p$0.get__to$i$0()
		}
	};
	$h.MeetingSuggestion=function(data, dateTimeSent)
	{
		this.$$d__getEndTime$p$0=Function.createDelegate(this,this._getEndTime$p$0);
		this.$$d__getStartTime$p$0=Function.createDelegate(this,this._getStartTime$p$0);
		this.$$d__getSubject$p$0=Function.createDelegate(this,this._getSubject$p$0);
		this.$$d__getLocation$p$0=Function.createDelegate(this,this._getLocation$p$0);
		this.$$d__getAttendees$p$0=Function.createDelegate(this,this._getAttendees$p$0);
		this.$$d__getMeetingString$p$0=Function.createDelegate(this,this._getMeetingString$p$0);
		this._data$p$0=data;
		this._dateTimeSent$p$0=dateTimeSent;
		$h.InitialData._defineReadOnlyProperty$i(this,"meetingString",this.$$d__getMeetingString$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"attendees",this.$$d__getAttendees$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"location",this.$$d__getLocation$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"subject",this.$$d__getSubject$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"start",this.$$d__getStartTime$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"end",this.$$d__getEndTime$p$0)
	};
	$h.MeetingSuggestion.prototype={
		_dateTimeSent$p$0: null,
		_data$p$0: null,
		_attendees$p$0: null,
		_getMeetingString$p$0: function()
		{
			return this._data$p$0["MeetingString"]
		},
		_getLocation$p$0: function()
		{
			return this._data$p$0["Location"]
		},
		_getSubject$p$0: function()
		{
			return this._data$p$0["Subject"]
		},
		_getStartTime$p$0: function()
		{
			var time=this._createDateTimeFromParameter$p$0("StartTime");
			var resolvedTime=$h.MeetingSuggestionTimeDecoder.resolve(time,this._dateTimeSent$p$0);
			if(resolvedTime.getTime() !==time.getTime())
				return OSF.DDA.OutlookAppOm._instance$p.convertToUtcClientTime(OSF.DDA.OutlookAppOm._instance$p._dateToDictionary$i$0(resolvedTime));
			return time
		},
		_getEndTime$p$0: function()
		{
			var time=this._createDateTimeFromParameter$p$0("EndTime");
			var resolvedTime=$h.MeetingSuggestionTimeDecoder.resolve(time,this._dateTimeSent$p$0);
			if(resolvedTime.getTime() !==time.getTime())
				return OSF.DDA.OutlookAppOm._instance$p.convertToUtcClientTime(OSF.DDA.OutlookAppOm._instance$p._dateToDictionary$i$0(resolvedTime));
			return time
		},
		_createDateTimeFromParameter$p$0: function(keyName)
		{
			var dateTimeString=this._data$p$0[keyName];
			if(!dateTimeString)
				return null;
			return new Date(dateTimeString)
		},
		_getAttendees$p$0: function()
		{
			if(!this._attendees$p$0)
			{
				var $$t_1=this;
				this._attendees$p$0=$h.Entities._getExtractedObjects$i($h.EmailAddressDetails,this._data$p$0,"Attendees",function(data)
				{
					return $h.EmailAddressDetails._createFromEmailUserDictionary$i(data)
				})
			}
			return this._attendees$p$0
		}
	};
	$h.MeetingSuggestionTimeDecoder=function(){};
	$h.MeetingSuggestionTimeDecoder.resolve=function(inTime, sentTime)
	{
		if(!sentTime)
			return inTime;
		try
		{
			var tod;
			var outDate;
			var extractedDate;
			var sentDate=new Date(sentTime.getFullYear(),sentTime.getMonth(),sentTime.getDate(),0,0,0,0);
			var $$t_7,
				$$t_8,
				$$t_9;
			if(!($$t_9=$h.MeetingSuggestionTimeDecoder._decode$p(inTime,$$t_7={val: extractedDate},$$t_8={val: tod}),extractedDate=$$t_7.val,tod=$$t_8.val,$$t_9))
				return inTime;
			else
			{
				if($h._preciseDate.isInstanceOfType(extractedDate))
					outDate=$h.MeetingSuggestionTimeDecoder._resolvePreciseDate$p(sentDate,extractedDate);
				else if($h._relativeDate.isInstanceOfType(extractedDate))
					outDate=$h.MeetingSuggestionTimeDecoder._resolveRelativeDate$p(sentDate,extractedDate);
				else
					outDate=sentDate;
				if(isNaN(outDate.getTime()))
					return sentTime;
				outDate.setMilliseconds(outDate.getMilliseconds()+tod);
				return outDate
			}
		}
		catch($$e_6)
		{
			return sentTime
		}
	};
	$h.MeetingSuggestionTimeDecoder._isNullOrUndefined$i=function(value)
	{
		return null===value || value===undefined
	};
	$h.MeetingSuggestionTimeDecoder._resolvePreciseDate$p=function(sentDate, precise)
	{
		var year=precise._year$i$1;
		var month=!precise._month$i$1 ? sentDate.getMonth() : precise._month$i$1 - 1;
		var day=precise._day$i$1;
		if(!day)
			return sentDate;
		var candidate;
		if($h.MeetingSuggestionTimeDecoder._isNullOrUndefined$i(year))
		{
			candidate=new Date(sentDate.getFullYear(),month,day);
			if(candidate.getTime() < sentDate.getTime())
				candidate=new Date(sentDate.getFullYear()+1,month,day)
		}
		else
			candidate=new Date(year < 50 ? 2e3+year : 1900+year,month,day);
		if(candidate.getMonth() !==month)
			return sentDate;
		return candidate
	};
	$h.MeetingSuggestionTimeDecoder._resolveRelativeDate$p=function(sentDate, relative)
	{
		var date;
		switch(relative._unit$i$1)
		{
			case 0:
				date=new Date(sentDate.getFullYear(),sentDate.getMonth(),sentDate.getDate());
				date.setDate(date.getDate()+relative._offset$i$1);
				return date;
			case 5:
				return $h.MeetingSuggestionTimeDecoder._findBestDateForWeekDate$p(sentDate,relative._offset$i$1,relative._tag$i$1);
			case 2:
				var days=1;
				switch(relative._modifier$i$1)
				{
					case 1:
						break;
					case 2:
						days=16;
						break;
					default:
						if(!relative._offset$i$1)
							days=sentDate.getDate();
						break
				}
				date=new Date(sentDate.getFullYear(),sentDate.getMonth(),days);
				date.setMonth(date.getMonth()+relative._offset$i$1);
				if(date.getTime() < sentDate.getTime())
					date.setDate(date.getDate()+sentDate.getDate() - 1);
				return date;
			case 1:
				date=new Date(sentDate.getFullYear(),sentDate.getMonth(),sentDate.getDate());
				date.setDate(sentDate.getDate()+7 * relative._offset$i$1);
				if(relative._modifier$i$1===1 || !relative._modifier$i$1)
				{
					date.setDate(date.getDate()+1 - date.getDay());
					if(date.getTime() < sentDate.getTime())
						return sentDate;
					return date
				}
				else if(relative._modifier$i$1===2)
				{
					date.setDate(date.getDate()+5 - date.getDay());
					return date
				}
				break;
			case 4:
				return $h.MeetingSuggestionTimeDecoder._findBestDateForWeekOfMonthDate$p(sentDate,relative);
			case 3:
				if(relative._offset$i$1 > 0)
					return new Date(sentDate.getFullYear()+relative._offset$i$1,0,1);
				break;
			default:
				break
		}
		return sentDate
	};
	$h.MeetingSuggestionTimeDecoder._findBestDateForWeekDate$p=function(sentDate, offset, tag)
	{
		if(offset > -5 && offset < 5)
		{
			var dayOfWeek;
			var days;
			dayOfWeek=(tag+6) % 7+1;
			days=7 * offset+(dayOfWeek - sentDate.getDay());
			sentDate.setDate(sentDate.getDate()+days);
			return sentDate
		}
		else
		{
			var days=(tag - sentDate.getDay()) % 7;
			if(days < 0)
				days+=7;
			sentDate.setDate(sentDate.getDate()+days);
			return sentDate
		}
	};
	$h.MeetingSuggestionTimeDecoder._findBestDateForWeekOfMonthDate$p=function(sentDate, relative)
	{
		var date;
		var firstDay;
		var newDate;
		date=sentDate;
		if(relative._tag$i$1 <=0 || relative._tag$i$1 > 12 || relative._offset$i$1 <=0 || relative._offset$i$1 > 5)
			return sentDate;
		var monthOffset=(12+relative._tag$i$1 - date.getMonth() - 1) % 12;
		firstDay=new Date(date.getFullYear(),date.getMonth()+monthOffset,1);
		if(relative._modifier$i$1===1)
			if(relative._offset$i$1===1 && firstDay.getDay() !==6 && firstDay.getDay())
				return firstDay;
			else
			{
				newDate=new Date(firstDay.getFullYear(),firstDay.getMonth(),firstDay.getDate());
				newDate.setDate(newDate.getDate()+(7+(1 - firstDay.getDay())) % 7);
				if(firstDay.getDay() !==6 && firstDay.getDay() && firstDay.getDay() !==1)
					newDate.setDate(newDate.getDate() - 7);
				newDate.setDate(newDate.getDate()+7 * (relative._offset$i$1 - 1));
				if(newDate.getMonth()+1 !==relative._tag$i$1)
					return sentDate;
				return newDate
			}
		else
		{
			newDate=new Date(firstDay.getFullYear(),firstDay.getMonth(),$h.MeetingSuggestionTimeDecoder._daysInMonth$p(firstDay.getMonth(),firstDay.getFullYear()));
			var offset=1 - newDate.getDay();
			if(offset > 0)
				offset=offset - 7;
			newDate.setDate(newDate.getDate()+offset);
			newDate.setDate(newDate.getDate()+7 * (1 - relative._offset$i$1));
			if(newDate.getMonth()+1 !==relative._tag$i$1)
				if(firstDay.getDay() !==6 && firstDay.getDay())
					return firstDay;
				else
					return sentDate;
			else
				return newDate
		}
	};
	$h.MeetingSuggestionTimeDecoder._decode$p=function(inDate, date, time)
	{
		var DateValueMask=32767;
		date.val=null;
		time.val=0;
		if(!inDate)
			return false;
		time.val=$h.MeetingSuggestionTimeDecoder._getTimeOfDayInMillisecondsUTC$p(inDate);
		var inDateAtMidnight=inDate.getTime() - time.val;
		var value=(inDateAtMidnight - $h.MeetingSuggestionTimeDecoder._baseDate$p.getTime()) / 864e5;
		if(value < 0)
			return false;
		else if(value >=262144)
			return false;
		else
		{
			var type=value >> 15;
			value=value & DateValueMask;
			switch(type)
			{
				case 0:
					return $h.MeetingSuggestionTimeDecoder._decodePreciseDate$p(value,date);
				case 1:
					return $h.MeetingSuggestionTimeDecoder._decodeRelativeDate$p(value,date);
				default:
					return false
			}
		}
	};
	$h.MeetingSuggestionTimeDecoder._decodePreciseDate$p=function(value, date)
	{
		var c_SubTypeMask=7;
		var c_MonthMask=15;
		var c_DayMask=31;
		var c_YearMask=127;
		var year=null;
		var month=0;
		var day=0;
		date.val=null;
		var subType=value >> 12 & c_SubTypeMask;
		if((subType & 4)===4)
		{
			year=value >> 5 & c_YearMask;
			if((subType & 2)===2)
			{
				if((subType & 1)===1)
					return false;
				month=value >> 1 & c_MonthMask
			}
		}
		else
		{
			if((subType & 2)===2)
				month=value >> 8 & c_MonthMask;
			if((subType & 1)===1)
				day=value >> 3 & c_DayMask
		}
		date.val=new $h._preciseDate(day,month,year);
		return true
	};
	$h.MeetingSuggestionTimeDecoder._decodeRelativeDate$p=function(value, date)
	{
		var TagMask=15;
		var OffsetMask=63;
		var UnitMask=7;
		var ModifierMask=3;
		var tag=value & TagMask;
		value >>=4;
		var offset=$h.MeetingSuggestionTimeDecoder._fromComplement$p(value & OffsetMask,6);
		value >>=6;
		var unit=value & UnitMask;
		value >>=3;
		var modifier=value & ModifierMask;
		try
		{
			date.val=new $h._relativeDate(modifier,offset,unit,tag);
			return true
		}
		catch($$e_A)
		{
			date.val=null;
			return false
		}
	};
	$h.MeetingSuggestionTimeDecoder._fromComplement$p=function(value, n)
	{
		var signed=1 << n - 1;
		var mask=(1 << n) - 1;
		if((value & signed)===signed)
			return-((value ^ mask)+1);
		else
			return value
	};
	$h.MeetingSuggestionTimeDecoder._daysInMonth$p=function(month, year)
	{
		return 32 - new Date(year,month,32).getDate()
	};
	$h.MeetingSuggestionTimeDecoder._getTimeOfDayInMillisecondsUTC$p=function(inputTime)
	{
		var timeOfDay=0;
		timeOfDay+=inputTime.getUTCHours() * 3600;
		timeOfDay+=inputTime.getUTCMinutes() * 60;
		timeOfDay+=inputTime.getUTCSeconds();
		timeOfDay *=1e3;
		timeOfDay+=inputTime.getUTCMilliseconds();
		return timeOfDay
	};
	$h._extractedDate=function(){};
	$h._preciseDate=function(day, month, year)
	{
		$h._preciseDate.initializeBase(this);
		if(day < 0 || day > 31)
			throw Error.argumentOutOfRange("day");
		if(month < 0 || month > 12)
			throw Error.argumentOutOfRange("month");
		this._day$i$1=day;
		this._month$i$1=month;
		if(!$h.MeetingSuggestionTimeDecoder._isNullOrUndefined$i(year))
		{
			if(!month && day)
				throw Error.argument("Invalid arguments");
			if(year < 0 || year > 2099)
				throw Error.argumentOutOfRange("year");
			this._year$i$1=year % 100
		}
		else if(!this._month$i$1 && !this._day$i$1)
			throw Error.argument("Invalid datetime");
	};
	$h._preciseDate.prototype={
		_day$i$1: 0,
		_month$i$1: 0,
		_year$i$1: null
	};
	$h._relativeDate=function(modifier, offset, unit, tag)
	{
		$h._relativeDate.initializeBase(this);
		if(offset < -32 || offset > 31)
			throw Error.argumentOutOfRange("offset");
		if(tag < 0 || tag > 15)
			throw Error.argumentOutOfRange("tag");
		if(!unit && offset < 0)
			throw Error.argument("unit & offset do not form a valid date");
		this._modifier$i$1=modifier;
		this._offset$i$1=offset;
		this._unit$i$1=unit;
		this._tag$i$1=tag
	};
	$h._relativeDate.prototype={
		_modifier$i$1: 0,
		_offset$i$1: 0,
		_unit$i$1: 0,
		_tag$i$1: 0
	};
	$h.Message=function(dataDictionary)
	{
		this.$$d__getConversationId$p$2=Function.createDelegate(this,this._getConversationId$p$2);
		this.$$d__getInternetMessageId$p$2=Function.createDelegate(this,this._getInternetMessageId$p$2);
		this.$$d__getCc$p$2=Function.createDelegate(this,this._getCc$p$2);
		this.$$d__getTo$p$2=Function.createDelegate(this,this._getTo$p$2);
		this.$$d__getFrom$p$2=Function.createDelegate(this,this._getFrom$p$2);
		this.$$d__getSender$p$2=Function.createDelegate(this,this._getSender$p$2);
		this.$$d__getNormalizedSubject$p$2=Function.createDelegate(this,this._getNormalizedSubject$p$2);
		this.$$d__getSubject$p$2=Function.createDelegate(this,this._getSubject$p$2);
		$h.Message.initializeBase(this,[dataDictionary]);
		$h.InitialData._defineReadOnlyProperty$i(this,"subject",this.$$d__getSubject$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"normalizedSubject",this.$$d__getNormalizedSubject$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"sender",this.$$d__getSender$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"from",this.$$d__getFrom$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"to",this.$$d__getTo$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"cc",this.$$d__getCc$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"internetMessageId",this.$$d__getInternetMessageId$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"conversationId",this.$$d__getConversationId$p$2)
	};
	$h.Message.prototype={
		getEntities: function()
		{
			return this._data$p$0._getEntities$i$0()
		},
		getEntitiesByType: function(entityType)
		{
			return this._data$p$0._getEntitiesByType$i$0(entityType)
		},
		getFilteredEntitiesByName: function(name)
		{
			return this._data$p$0._getFilteredEntitiesByName$i$0(name)
		},
		getRegExMatches: function()
		{
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"getRegExMatches");
			return this._data$p$0._getRegExMatches$i$0()
		},
		getRegExMatchesByName: function(name)
		{
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(1,"getRegExMatchesByName");
			return this._data$p$0._getRegExMatchesByName$i$0(name)
		},
		displayReplyForm: function(obj)
		{
			OSF.DDA.OutlookAppOm._instance$p._displayReplyForm$i$0(obj)
		},
		displayReplyAllForm: function(obj)
		{
			OSF.DDA.OutlookAppOm._instance$p._displayReplyAllForm$i$0(obj)
		},
		getItemType: function()
		{
			return Microsoft.Office.WebExtension.MailboxEnums.ItemType.Message
		},
		_getSubject$p$2: function()
		{
			return this._data$p$0.get__subject$i$0()
		},
		_getNormalizedSubject$p$2: function()
		{
			return this._data$p$0.get__normalizedSubject$i$0()
		},
		_getSender$p$2: function()
		{
			return this._data$p$0.get__sender$i$0()
		},
		_getFrom$p$2: function()
		{
			return this._data$p$0.get__from$i$0()
		},
		_getTo$p$2: function()
		{
			return this._data$p$0.get__to$i$0()
		},
		_getCc$p$2: function()
		{
			return this._data$p$0.get__cc$i$0()
		},
		_getInternetMessageId$p$2: function()
		{
			return this._data$p$0.get__internetMessageId$i$0()
		},
		_getConversationId$p$2: function()
		{
			return this._data$p$0.get__conversationId$i$0()
		}
	};
	$h.MessageCompose=function(data)
	{
		this.$$d__getConversationId$p$2=Function.createDelegate(this,this._getConversationId$p$2);
		this.$$d__getBcc$p$2=Function.createDelegate(this,this._getBcc$p$2);
		this.$$d__getCc$p$2=Function.createDelegate(this,this._getCc$p$2);
		this.$$d__getTo$p$2=Function.createDelegate(this,this._getTo$p$2);
		$h.MessageCompose.initializeBase(this,[data]);
		$h.InitialData._defineReadOnlyProperty$i(this,"to",this.$$d__getTo$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"cc",this.$$d__getCc$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"bcc",this.$$d__getBcc$p$2);
		$h.InitialData._defineReadOnlyProperty$i(this,"conversationId",this.$$d__getConversationId$p$2)
	};
	$h.MessageCompose.prototype={
		_to$p$2: null,
		_cc$p$2: null,
		_bcc$p$2: null,
		getItemType: function()
		{
			return Microsoft.Office.WebExtension.MailboxEnums.ItemType.Message
		},
		_getTo$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._to$p$2)
				this._to$p$2=new $h.ComposeRecipient(0,"to");
			return this._to$p$2
		},
		_getCc$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._cc$p$2)
				this._cc$p$2=new $h.ComposeRecipient(1,"cc");
			return this._cc$p$2
		},
		_getBcc$p$2: function()
		{
			this._data$p$0._throwOnRestrictedPermissionLevel$i$0();
			if(!this._bcc$p$2)
				this._bcc$p$2=new $h.ComposeRecipient(2,"bcc");
			return this._bcc$p$2
		},
		_getConversationId$p$2: function()
		{
			return this._data$p$0.get__conversationId$i$0()
		}
	};
	$h.NotificationMessages=function(){};
	$h.NotificationMessages._mapToHostItemNotificationMessageType$p=function(dataToHost)
	{
		var notificationType;
		var hostItemNotificationMessageType;
		notificationType=dataToHost["type"];
		if(notificationType===Microsoft.Office.WebExtension.MailboxEnums.ItemNotificationMessageType.ProgressIndicator)
			hostItemNotificationMessageType=1;
		else if(notificationType===Microsoft.Office.WebExtension.MailboxEnums.ItemNotificationMessageType.InformationalMessage)
			hostItemNotificationMessageType=0;
		else if(notificationType===Microsoft.Office.WebExtension.MailboxEnums.ItemNotificationMessageType.ErrorMessage)
			hostItemNotificationMessageType=2;
		else
			throw Error.argument("type");
		dataToHost["type"]=hostItemNotificationMessageType
	};
	$h.NotificationMessages._validateKey$p=function(key)
	{
		if(!$h.ScriptHelpers.isNonEmptyString(key))
			throw Error.argument("key");
		OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(key.length,0,32,"key")
	};
	$h.NotificationMessages._validateDictionary$p=function(dictionary)
	{
		if(!$h.ScriptHelpers.isNonEmptyString(dictionary["type"]))
			throw Error.argument("type");
		if(dictionary["type"]===Microsoft.Office.WebExtension.MailboxEnums.ItemNotificationMessageType.InformationalMessage)
		{
			if(!$h.ScriptHelpers.isNonEmptyString(dictionary["icon"]))
				throw Error.argument("icon");
			OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(dictionary["icon"].length,0,32,"icon");
			if($h.ScriptHelpers.isUndefined(dictionary["persistent"]))
				throw Error.argument("persistent");
			if(!Boolean.isInstanceOfType(dictionary["persistent"]))
				throw Error.argumentType("persistent",Object.getType(dictionary["persistent"]),Boolean);
		}
		else
		{
			if(!$h.ScriptHelpers.isUndefined(dictionary["icon"]))
				throw Error.argument("icon");
			if(!$h.ScriptHelpers.isUndefined(dictionary["persistent"]))
				throw Error.argument("persistent");
		}
		if(!$h.ScriptHelpers.isNonEmptyString(dictionary["message"]))
			throw Error.argument("message");
		OSF.DDA.OutlookAppOm._throwOnOutOfRange$i(dictionary["message"].length,0,150,"message")
	};
	$h.NotificationMessages.prototype={
		addAsync: function(key, dictionary)
		{
			var args=[];
			for(var $$pai_5=2; $$pai_5 < arguments.length;++$$pai_5)
				args[$$pai_5 - 2]=arguments[$$pai_5];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(0,"NotificationMessages.addAsync");
			var commonParameters=$h.CommonParameters.parse(args,false);
			$h.NotificationMessages._validateKey$p(key);
			$h.NotificationMessages._validateDictionary$p(dictionary);
			var dataToHost={};
			dataToHost=$.extend(true,dataToHost,dictionary);
			dataToHost["key"]=key;
			$h.NotificationMessages._mapToHostItemNotificationMessageType$p(dataToHost);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(33,"AddNotificationMessageAsync",dataToHost,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		},
		getAllAsync: function()
		{
			var args=[];
			for(var $$pai_2=0; $$pai_2 < arguments.length;++$$pai_2)
				args[$$pai_2]=arguments[$$pai_2];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(0,"NotificationMessages.getAllAsync");
			var commonParameters=$h.CommonParameters.parse(args,true);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(34,"GetAllNotificationMessagesAsync",null,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		},
		replaceAsync: function(key, dictionary)
		{
			var args=[];
			for(var $$pai_5=2; $$pai_5 < arguments.length;++$$pai_5)
				args[$$pai_5 - 2]=arguments[$$pai_5];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(0,"NotificationMessages.replaceAsync");
			var commonParameters=$h.CommonParameters.parse(args,false);
			$h.NotificationMessages._validateKey$p(key);
			$h.NotificationMessages._validateDictionary$p(dictionary);
			var dataToHost={};
			dataToHost=$.extend(true,dataToHost,dictionary);
			dataToHost["key"]=key;
			$h.NotificationMessages._mapToHostItemNotificationMessageType$p(dataToHost);
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(35,"ReplaceNotificationMessageAsync",dataToHost,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		},
		removeAsync: function(key)
		{
			var args=[];
			for(var $$pai_4=1; $$pai_4 < arguments.length;++$$pai_4)
				args[$$pai_4 - 1]=arguments[$$pai_4];
			OSF.DDA.OutlookAppOm._instance$p._throwOnMethodCallForInsufficientPermission$i$0(0,"NotificationMessages.removeAsync");
			var commonParameters=$h.CommonParameters.parse(args,false);
			$h.NotificationMessages._validateKey$p(key);
			var dataToHost={key: key};
			OSF.DDA.OutlookAppOm._instance$p._standardInvokeHostMethod$i$0(36,"RemoveNotificationMessageAsync",dataToHost,null,commonParameters._asyncContext$p$0,commonParameters._callback$p$0)
		}
	};
	$h.OutlookErrorManager=function(){};
	$h.OutlookErrorManager.getErrorArgs=function(errorCode)
	{
		if(!$h.OutlookErrorManager._isInitialized$p)
			$h.OutlookErrorManager._initialize$p();
		return OSF.DDA.ErrorCodeManager.getErrorArgs(errorCode)
	};
	$h.OutlookErrorManager._initialize$p=function()
	{
		$h.OutlookErrorManager._addErrorMessage$p(9e3,"AttachmentSizeExceeded",_u.ExtensibilityStrings.l_AttachmentExceededSize_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9001,"NumberOfAttachmentsExceeded",_u.ExtensibilityStrings.l_ExceededMaxNumberOfAttachments_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9002,"InternalFormatError",_u.ExtensibilityStrings.l_InternalFormatError_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9003,"InvalidAttachmentId",_u.ExtensibilityStrings.l_InvalidAttachmentId_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9004,"InvalidAttachmentPath",_u.ExtensibilityStrings.l_InvalidAttachmentPath_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9005,"CannotAddAttachmentBeforeUpgrade",_u.ExtensibilityStrings.l_CannotAddAttachmentBeforeUpgrade_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9006,"AttachmentDeletedBeforeUploadCompletes",_u.ExtensibilityStrings.l_AttachmentDeletedBeforeUploadCompletes_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9007,"AttachmentUploadGeneralFailure",_u.ExtensibilityStrings.l_AttachmentUploadGeneralFailure_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9008,"AttachmentToDeleteDoesNotExist",_u.ExtensibilityStrings.l_DeleteAttachmentDoesNotExist_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9009,"AttachmentDeleteGeneralFailure",_u.ExtensibilityStrings.l_AttachmentDeleteGeneralFailure_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9010,"InvalidEndTime",_u.ExtensibilityStrings.l_InvalidEndTime_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9011,"HtmlSanitizationFailure",_u.ExtensibilityStrings.l_HtmlSanitizationFailure_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9012,"NumberOfRecipientsExceeded",String.format(_u.ExtensibilityStrings.l_NumberOfRecipientsExceeded_Text,500));
		$h.OutlookErrorManager._addErrorMessage$p(9013,"NoValidRecipientsProvided",_u.ExtensibilityStrings.l_NoValidRecipientsProvided_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9014,"CursorPositionChanged",_u.ExtensibilityStrings.l_CursorPositionChanged_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9016,"InvalidSelection",_u.ExtensibilityStrings.l_InvalidSelection_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9017,"AccessRestricted","");
		$h.OutlookErrorManager._addErrorMessage$p(9018,"GenericTokenError","");
		$h.OutlookErrorManager._addErrorMessage$p(9019,"GenericSettingsError","");
		$h.OutlookErrorManager._addErrorMessage$p(9020,"GenericResponseError","");
		$h.OutlookErrorManager._addErrorMessage$p(9021,"SaveError",_u.ExtensibilityStrings.l_SaveError_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9022,"MessageInDifferentStoreError",_u.ExtensibilityStrings.l_MessageInDifferentStoreError_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9023,"DuplicateNotificationKey",_u.ExtensibilityStrings.l_DuplicateNotificationKey_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9024,"NotificationKeyNotFound",_u.ExtensibilityStrings.l_NotificationKeyNotFound_Text);
		$h.OutlookErrorManager._addErrorMessage$p(9025,"NumberOfNotificationsExceeded",_u.ExtensibilityStrings.l_NumberOfNotificationsExceeded_Text);
		$h.OutlookErrorManager._isInitialized$p=true
	};
	$h.OutlookErrorManager._addErrorMessage$p=function(errorCode, errorName, errorMessage)
	{
		OSF.DDA.ErrorCodeManager.addErrorMessage(errorCode,{
			name: errorName,
			message: errorMessage
		})
	};
	$h.OutlookErrorManager.OutlookErrorCodes=function(){};
	$h.OutlookErrorManager.OsfDdaErrorCodes=function(){};
	$h.PhoneNumber=function(data)
	{
		this.$$d__getPhoneType$p$0=Function.createDelegate(this,this._getPhoneType$p$0);
		this.$$d__getOriginalPhoneString$p$0=Function.createDelegate(this,this._getOriginalPhoneString$p$0);
		this.$$d__getPhoneString$p$0=Function.createDelegate(this,this._getPhoneString$p$0);
		this._data$p$0=data;
		$h.InitialData._defineReadOnlyProperty$i(this,"phoneString",this.$$d__getPhoneString$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"originalPhoneString",this.$$d__getOriginalPhoneString$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"type",this.$$d__getPhoneType$p$0)
	};
	$h.PhoneNumber.prototype={
		_data$p$0: null,
		_getPhoneString$p$0: function()
		{
			return this._data$p$0["PhoneString"]
		},
		_getOriginalPhoneString$p$0: function()
		{
			return this._data$p$0["OriginalPhoneString"]
		},
		_getPhoneType$p$0: function()
		{
			return this._data$p$0["Type"]
		}
	};
	$h.TaskSuggestion=function(data)
	{
		this.$$d__getAssignees$p$0=Function.createDelegate(this,this._getAssignees$p$0);
		this.$$d__getTaskString$p$0=Function.createDelegate(this,this._getTaskString$p$0);
		this._data$p$0=data;
		$h.InitialData._defineReadOnlyProperty$i(this,"taskString",this.$$d__getTaskString$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"assignees",this.$$d__getAssignees$p$0)
	};
	$h.TaskSuggestion.prototype={
		_data$p$0: null,
		_assignees$p$0: null,
		_getTaskString$p$0: function()
		{
			return this._data$p$0["TaskString"]
		},
		_getAssignees$p$0: function()
		{
			if(!this._assignees$p$0)
			{
				var $$t_1=this;
				this._assignees$p$0=$h.Entities._getExtractedObjects$i($h.EmailAddressDetails,this._data$p$0,"Assignees",function(data)
				{
					return $h.EmailAddressDetails._createFromEmailUserDictionary$i(data)
				})
			}
			return this._assignees$p$0
		}
	};
	$h.UserProfile=function(data)
	{
		this.$$d__getTimeZone$p$0=Function.createDelegate(this,this._getTimeZone$p$0);
		this.$$d__getEmailAddress$p$0=Function.createDelegate(this,this._getEmailAddress$p$0);
		this.$$d__getDisplayName$p$0=Function.createDelegate(this,this._getDisplayName$p$0);
		this._data$p$0=data;
		$h.InitialData._defineReadOnlyProperty$i(this,"displayName",this.$$d__getDisplayName$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"emailAddress",this.$$d__getEmailAddress$p$0);
		$h.InitialData._defineReadOnlyProperty$i(this,"timeZone",this.$$d__getTimeZone$p$0)
	};
	$h.UserProfile.prototype={
		_data$p$0: null,
		_getDisplayName$p$0: function()
		{
			return this._data$p$0.get__userDisplayName$i$0()
		},
		_getEmailAddress$p$0: function()
		{
			return this._data$p$0.get__userEmailAddress$i$0()
		},
		_getTimeZone$p$0: function()
		{
			return this._data$p$0.get__userTimeZone$i$0()
		}
	};
	$h.RequestState=function(){};
	$h.RequestState.prototype={
		unsent: 0,
		opened: 1,
		headersReceived: 2,
		loading: 3,
		done: 4
	};
	$h.RequestState.registerEnum("$h.RequestState",false);
	$h.CommonParameters=function(options, callback, asyncContext)
	{
		this._options$p$0=options;
		this._callback$p$0=callback;
		this._asyncContext$p$0=asyncContext
	};
	$h.CommonParameters.parse=function(args, isCallbackRequired, tryLegacy)
	{
		var legacyParameters;
		var $$t_8,
			$$t_9;
		if(tryLegacy && ($$t_9=$h.CommonParameters._tryParseLegacy$p(args,$$t_8={val: legacyParameters}),legacyParameters=$$t_8.val,$$t_9))
			return legacyParameters;
		var argsLength=args.length;
		var options=null;
		var callback=null;
		var asyncContext=null;
		if(argsLength===1)
			if(Function.isInstanceOfType(args[0]))
				callback=args[0];
			else if(Object.isInstanceOfType(args[0]))
				options=args[0];
			else
				throw Error.argumentType();
		else if(argsLength===2)
		{
			if(!Object.isInstanceOfType(args[0]))
				throw Error.argument("options");
			if(!Function.isInstanceOfType(args[1]))
				throw Error.argument("callback");
			options=args[0];
			callback=args[1]
		}
		else if(argsLength)
			throw Error.parameterCount(_u.ExtensibilityStrings.l_ParametersNotAsExpected_Text);
		if(isCallbackRequired && !callback)
			throw Error.argumentNull("callback");
		if(options && !$h.ScriptHelpers.isNullOrUndefined(options["asyncContext"]))
			asyncContext=options["asyncContext"];
		return new $h.CommonParameters(options,callback,asyncContext)
	};
	$h.CommonParameters._tryParseLegacy$p=function(args, commonParameters)
	{
		commonParameters.val=null;
		var argsLength=args.length;
		var callback=null;
		var userContext=null;
		if(!argsLength || argsLength > 2)
			return false;
		if(!Function.isInstanceOfType(args[0]))
			return false;
		callback=args[0];
		if(argsLength > 1)
			userContext=args[1];
		commonParameters.val=new $h.CommonParameters(null,callback,userContext);
		return true
	};
	$h.CommonParameters.prototype={
		_options$p$0: null,
		_callback$p$0: null,
		_asyncContext$p$0: null,
		get_options: function()
		{
			return this._options$p$0
		},
		get_callback: function()
		{
			return this._callback$p$0
		},
		get_asyncContext: function()
		{
			return this._asyncContext$p$0
		}
	};
	$h.EwsRequest=function(userContext)
	{
		$h.EwsRequest.initializeBase(this,[userContext])
	};
	$h.EwsRequest.prototype={
		readyState: 1,
		status: 0,
		statusText: null,
		onreadystatechange: null,
		responseText: null,
		get__statusCode$i$1: function()
		{
			return this.status
		},
		set__statusCode$i$1: function(value)
		{
			this.status=value;
			return value
		},
		get__statusDescription$i$1: function()
		{
			return this.statusText
		},
		set__statusDescription$i$1: function(value)
		{
			this.statusText=value;
			return value
		},
		get__requestState$i$1: function()
		{
			return this.readyState
		},
		set__requestState$i$1: function(value)
		{
			this.readyState=value;
			return value
		},
		get_hasOnReadyStateChangeCallback: function()
		{
			return!$h.ScriptHelpers.isNullOrUndefined(this.onreadystatechange)
		},
		get__response$i$1: function()
		{
			return this.responseText
		},
		set__response$i$1: function(value)
		{
			this.responseText=value;
			return value
		},
		send: function(data)
		{
			this._checkSendConditions$i$1();
			if($h.ScriptHelpers.isNullOrUndefined(data))
				this._throwInvalidStateException$i$1();
			this._sendRequest$i$0(5,"EwsRequest",{body: data})
		},
		_callOnReadyStateChangeCallback$i$1: function()
		{
			if(!$h.ScriptHelpers.isNullOrUndefined(this.onreadystatechange))
				this.onreadystatechange()
		},
		_parseExtraResponseData$i$1: function(response){},
		executeExtraFailedResponseSteps: function(){}
	};
	$h.InitialData=function(data)
	{
		this._data$p$0=data;
		this._permissionLevel$p$0=this._calculatePermissionLevel$p$0()
	};
	$h.InitialData._defineReadOnlyProperty$i=function(o, methodName, getter)
	{
		var propertyDescriptor={
				get: getter,
				configurable: false
			};
		Object.defineProperty(o,methodName,propertyDescriptor)
	};
	$h.InitialData.prototype={
		_toRecipients$p$0: null,
		_ccRecipients$p$0: null,
		_attachments$p$0: null,
		_resources$p$0: null,
		_entities$p$0: null,
		_data$p$0: null,
		_permissionLevel$p$0: 0,
		get__itemId$i$0: function()
		{
			return this._data$p$0["id"]
		},
		get__itemClass$i$0: function()
		{
			return this._data$p$0["itemClass"]
		},
		get__dateTimeCreated$i$0: function()
		{
			return new Date(this._data$p$0["dateTimeCreated"])
		},
		get__dateTimeModified$i$0: function()
		{
			return new Date(this._data$p$0["dateTimeModified"])
		},
		get__dateTimeSent$i$0: function()
		{
			return new Date(this._data$p$0["dateTimeSent"])
		},
		get__subject$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			return this._data$p$0["subject"]
		},
		get__normalizedSubject$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			return this._data$p$0["normalizedSubject"]
		},
		get__internetMessageId$i$0: function()
		{
			return this._data$p$0["internetMessageId"]
		},
		get__conversationId$i$0: function()
		{
			return this._data$p$0["conversationId"]
		},
		get__sender$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			var sender=this._data$p$0["sender"];
			return $h.ScriptHelpers.isNullOrUndefined(sender) ? null : new $h.EmailAddressDetails(sender)
		},
		get__from$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			var from=this._data$p$0["from"];
			return $h.ScriptHelpers.isNullOrUndefined(from) ? null : new $h.EmailAddressDetails(from)
		},
		get__to$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			if(null===this._toRecipients$p$0)
				this._toRecipients$p$0=this._createEmailAddressDetails$p$0("to");
			return this._toRecipients$p$0
		},
		get__cc$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			if(null===this._ccRecipients$p$0)
				this._ccRecipients$p$0=this._createEmailAddressDetails$p$0("cc");
			return this._ccRecipients$p$0
		},
		get__attachments$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			if(null===this._attachments$p$0)
				this._attachments$p$0=this._createAttachmentDetails$p$0();
			return this._attachments$p$0
		},
		get__ewsUrl$i$0: function()
		{
			return this._data$p$0["ewsUrl"]
		},
		get__marketplaceAssetId$i$0: function()
		{
			return this._data$p$0["marketplaceAssetId"]
		},
		get__marketplaceContentMarket$i$0: function()
		{
			return this._data$p$0["marketplaceContentMarket"]
		},
		get__consentMetadata$i$0: function()
		{
			return this._data$p$0["consentMetadata"]
		},
		get__isRead$i$0: function()
		{
			return this._data$p$0["isRead"]
		},
		get__entryPointUrl$i$0: function()
		{
			return this._data$p$0["entryPointUrl"]
		},
		get__start$i$0: function()
		{
			return new Date(this._data$p$0["start"])
		},
		get__end$i$0: function()
		{
			return new Date(this._data$p$0["end"])
		},
		get__location$i$0: function()
		{
			return this._data$p$0["location"]
		},
		get__resources$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			if(null===this._resources$p$0)
				this._resources$p$0=this._createEmailAddressDetails$p$0("resources");
			return this._resources$p$0
		},
		get__organizer$i$0: function()
		{
			this._throwOnRestrictedPermissionLevel$i$0();
			var organizer=this._data$p$0["organizer"];
			return $h.ScriptHelpers.isNullOrUndefined(organizer) ? null : new $h.EmailAddressDetails(organizer)
		},
		get__userDisplayName$i$0: function()
		{
			return this._data$p$0["userDisplayName"]
		},
		get__userEmailAddress$i$0: function()
		{
			return this._data$p$0["userEmailAddress"]
		},
		get__userTimeZone$i$0: function()
		{
			return this._data$p$0["userTimeZone"]
		},
		get__timeZoneOffsets$i$0: function()
		{
			return this._data$p$0["timeZoneOffsets"]
		},
		get__hostVersion$i$0: function()
		{
			return this._data$p$0["hostVersion"]
		},
		get__owaView$i$0: function()
		{
			return this._data$p$0["owaView"]
		},
		get__overrideWindowOpen$i$0: function()
		{
			return this._data$p$0["overrideWindowOpen"]
		},
		_getEntities$i$0: function()
		{
			if(!this._entities$p$0)
				this._entities$p$0=new $h.Entities(this._data$p$0["entities"],this._data$p$0["filteredEntities"],this.get__dateTimeSent$i$0(),this._permissionLevel$p$0);
			return this._entities$p$0
		},
		_getEntitiesByType$i$0: function(entityType)
		{
			var entites=this._getEntities$i$0();
			return entites._getByType$i$0(entityType)
		},
		_getFilteredEntitiesByName$i$0: function(name)
		{
			var entities=this._getEntities$i$0();
			return entities._getFilteredEntitiesByName$i$0(name)
		},
		_getRegExMatches$i$0: function()
		{
			if(!this._data$p$0["regExMatches"])
				return null;
			return this._data$p$0["regExMatches"]
		},
		_getRegExMatchesByName$i$0: function(regexName)
		{
			var regexMatches=this._getRegExMatches$i$0();
			if(!regexMatches || !regexMatches[regexName])
				return null;
			return regexMatches[regexName]
		},
		_throwOnRestrictedPermissionLevel$i$0: function()
		{
			OSF.DDA.OutlookAppOm._throwOnPropertyAccessForRestrictedPermission$i(this._permissionLevel$p$0)
		},
		_createEmailAddressDetails$p$0: function(key)
		{
			var to=this._data$p$0[key];
			if($h.ScriptHelpers.isNullOrUndefined(to))
				return[];
			var recipients=[];
			for(var i=0; i < to.length; i++)
				if(!$h.ScriptHelpers.isNullOrUndefined(to[i]))
					recipients[i]=new $h.EmailAddressDetails(to[i]);
			return recipients
		},
		_createAttachmentDetails$p$0: function()
		{
			var attachments=this._data$p$0["attachments"];
			if($h.ScriptHelpers.isNullOrUndefined(attachments))
				return[];
			var attachmentDetails=[];
			for(var i=0; i < attachments.length; i++)
				if(!$h.ScriptHelpers.isNullOrUndefined(attachments[i]))
					attachmentDetails[i]=new $h.AttachmentDetails(attachments[i]);
			return attachmentDetails
		},
		_calculatePermissionLevel$p$0: function()
		{
			var HostReadItem=1;
			var HostReadWriteMailbox=2;
			var HostReadWriteItem=3;
			var permissionLevelFromHost=this._data$p$0["permissionLevel"];
			if($h.ScriptHelpers.isUndefined(this._permissionLevel$p$0))
				return 0;
			switch(permissionLevelFromHost)
			{
				case HostReadItem:
					return 1;
				case HostReadWriteItem:
					return 2;
				case HostReadWriteMailbox:
					return 3;
				default:
					return 0
			}
		}
	};
	$h._loadDictionaryRequest=function(createResultObject, dictionaryName, callback, userContext)
	{
		$h._loadDictionaryRequest.initializeBase(this,[userContext]);
		this._createResultObject$p$1=createResultObject;
		this._dictionaryName$p$1=dictionaryName;
		this._callback$p$1=callback
	};
	$h._loadDictionaryRequest.prototype={
		_dictionaryName$p$1: null,
		_createResultObject$p$1: null,
		_callback$p$1: null,
		handleResponse: function(response)
		{
			if(response["wasSuccessful"])
			{
				var value=response[this._dictionaryName$p$1];
				var responseData=JSON.parse(value);
				this.createAsyncResult(this._createResultObject$p$1(responseData),0,0,null)
			}
			else
				this.createAsyncResult(null,1,9020,response["errorMessage"]);
			this._callback$p$1(this._asyncResult$p$0)
		}
	};
	$h.ProxyRequestBase=function(userContext)
	{
		$h.ProxyRequestBase.initializeBase(this,[userContext])
	};
	$h.ProxyRequestBase.prototype={
		handleResponse: function(response)
		{
			if(!response["wasProxySuccessful"])
			{
				this.set__statusCode$i$1(500);
				this.set__statusDescription$i$1("Error");
				var errorMessage=response["errorMessage"];
				this.set__response$i$1(errorMessage);
				this.createAsyncResult(null,1,9020,errorMessage)
			}
			else
			{
				this.set__statusCode$i$1(response["statusCode"]);
				this.set__statusDescription$i$1(response["statusDescription"]);
				this.set__response$i$1(response["body"]);
				this.createAsyncResult(this.get__response$i$1(),0,0,null)
			}
			this._parseExtraResponseData$i$1(response);
			this._cycleReadyStateFromHeadersReceivedToLoadingToDone$i$1()
		},
		_throwInvalidStateException$i$1: function()
		{
			throw Error.create("DOMException",{
				code: 11,
				message: "INVALID_STATE_ERR"
			});
		},
		_cycleReadyStateFromHeadersReceivedToLoadingToDone$i$1: function()
		{
			var $$t_0=this;
			this._changeReadyState$i$1(2,function()
			{
				$$t_0._changeReadyState$i$1(3,function()
				{
					$$t_0._changeReadyState$i$1(4,null)
				})
			})
		},
		_changeReadyState$i$1: function(state, nextStep)
		{
			this.set__requestState$i$1(state);
			var $$t_2=this;
			window.setTimeout(function()
			{
				try
				{
					$$t_2._callOnReadyStateChangeCallback$i$1()
				}
				finally
				{
					if(!$h.ScriptHelpers.isNullOrUndefined(nextStep))
						nextStep()
				}
			},0)
		},
		_checkSendConditions$i$1: function()
		{
			if(this.get__requestState$i$1() !==1)
				this._throwInvalidStateException$i$1();
			if(this._isSent$p$0)
				this._throwInvalidStateException$i$1()
		}
	};
	$h.RequestBase=function(userContext)
	{
		this._userContext$p$0=userContext
	};
	$h.RequestBase.prototype={
		_isSent$p$0: false,
		_asyncResult$p$0: null,
		_userContext$p$0: null,
		get_asyncResult: function()
		{
			return this._asyncResult$p$0
		},
		_sendRequest$i$0: function(dispid, methodName, dataToSend)
		{
			this._isSent$p$0=true;
			var $$t_5=this;
			OSF.DDA.OutlookAppOm._instance$p._invokeHostMethod$i$0(dispid,methodName,dataToSend,function(resultCode, response)
			{
				if(resultCode)
					$$t_5.createAsyncResult(null,1,9017,String.format(_u.ExtensibilityStrings.l_InternalProtocolError_Text,resultCode));
				else
					$$t_5.handleResponse(response)
			})
		},
		createAsyncResult: function(value, errorCode, detailedErrorCode, errorDescription)
		{
			this._asyncResult$p$0=OSF.DDA.OutlookAppOm._instance$p.createAsyncResult(value,errorCode,detailedErrorCode,this._userContext$p$0,errorDescription)
		}
	};
	$h.SaveDictionaryRequest=function(callback, userContext)
	{
		$h.SaveDictionaryRequest.initializeBase(this,[userContext]);
		if(!$h.ScriptHelpers.isNullOrUndefined(callback))
			this._callback$p$1=callback
	};
	$h.SaveDictionaryRequest.prototype={
		_callback$p$1: null,
		handleResponse: function(response)
		{
			if(response["wasSuccessful"])
				this.createAsyncResult(null,0,0,null);
			else
				this.createAsyncResult(null,1,9020,response["errorMessage"]);
			if(!$h.ScriptHelpers.isNullOrUndefined(this._callback$p$1))
				this._callback$p$1(this._asyncResult$p$0)
		}
	};
	$h.ScriptHelpers=function(){};
	$h.ScriptHelpers.isNull=function(value)
	{
		return null===value
	};
	$h.ScriptHelpers.isNullOrUndefined=function(value)
	{
		return $h.ScriptHelpers.isNull(value) || $h.ScriptHelpers.isUndefined(value)
	};
	$h.ScriptHelpers.isUndefined=function(value)
	{
		return value===undefined
	};
	$h.ScriptHelpers.dictionaryContainsKey=function(obj, keyName)
	{
		return Object.isInstanceOfType(obj) ? keyName in obj : false
	};
	$h.ScriptHelpers.isNonEmptyString=function(value)
	{
		if(!value)
			return false;
		return String.isInstanceOfType(value)
	};
	OSF.DDA.OutlookAppOm.registerClass("OSF.DDA.OutlookAppOm");
	OSF.DDA.Settings.registerClass("OSF.DDA.Settings");
	$h.ItemBase.registerClass("$h.ItemBase");
	$h.Item.registerClass("$h.Item",$h.ItemBase);
	$h.Appointment.registerClass("$h.Appointment",$h.Item);
	$h.ComposeItem.registerClass("$h.ComposeItem",$h.ItemBase);
	$h.AppointmentCompose.registerClass("$h.AppointmentCompose",$h.ComposeItem);
	$h.AttachmentConstants.registerClass("$h.AttachmentConstants");
	$h.AttachmentDetails.registerClass("$h.AttachmentDetails");
	$h.Body.registerClass("$h.Body");
	$h.ComposeBody.registerClass("$h.ComposeBody",$h.Body);
	$h.ComposeRecipient.registerClass("$h.ComposeRecipient");
	$h.ComposeLocation.registerClass("$h.ComposeLocation");
	$h.ComposeSubject.registerClass("$h.ComposeSubject");
	$h.ComposeTime.registerClass("$h.ComposeTime");
	$h.Contact.registerClass("$h.Contact");
	$h.CustomProperties.registerClass("$h.CustomProperties");
	$h.Diagnostics.registerClass("$h.Diagnostics");
	$h.EmailAddressDetails.registerClass("$h.EmailAddressDetails");
	$h.Entities.registerClass("$h.Entities");
	$h.ReplyConstants.registerClass("$h.ReplyConstants");
	$h.AsyncConstants.registerClass("$h.AsyncConstants");
	Office.cast.item.registerClass("Office.cast.item");
	$h.Message.registerClass("$h.Message",$h.Item);
	$h.MeetingRequest.registerClass("$h.MeetingRequest",$h.Message);
	$h.MeetingSuggestion.registerClass("$h.MeetingSuggestion");
	$h.MeetingSuggestionTimeDecoder.registerClass("$h.MeetingSuggestionTimeDecoder");
	$h._extractedDate.registerClass("$h._extractedDate");
	$h._preciseDate.registerClass("$h._preciseDate",$h._extractedDate);
	$h._relativeDate.registerClass("$h._relativeDate",$h._extractedDate);
	$h.MessageCompose.registerClass("$h.MessageCompose",$h.ComposeItem);
	$h.NotificationMessages.registerClass("$h.NotificationMessages");
	$h.OutlookErrorManager.registerClass("$h.OutlookErrorManager");
	$h.OutlookErrorManager.OutlookErrorCodes.registerClass("$h.OutlookErrorManager.OutlookErrorCodes");
	$h.OutlookErrorManager.OsfDdaErrorCodes.registerClass("$h.OutlookErrorManager.OsfDdaErrorCodes");
	$h.PhoneNumber.registerClass("$h.PhoneNumber");
	$h.TaskSuggestion.registerClass("$h.TaskSuggestion");
	$h.UserProfile.registerClass("$h.UserProfile");
	$h.CommonParameters.registerClass("$h.CommonParameters");
	$h.RequestBase.registerClass("$h.RequestBase");
	$h.ProxyRequestBase.registerClass("$h.ProxyRequestBase",$h.RequestBase);
	$h.EwsRequest.registerClass("$h.EwsRequest",$h.ProxyRequestBase);
	$h.InitialData.registerClass("$h.InitialData");
	$h._loadDictionaryRequest.registerClass("$h._loadDictionaryRequest",$h.RequestBase);
	$h.SaveDictionaryRequest.registerClass("$h.SaveDictionaryRequest",$h.RequestBase);
	$h.ScriptHelpers.registerClass("$h.ScriptHelpers");
	OSF.DDA.OutlookAppOm.asyncMethodTimeoutKeyName="__timeout__";
	OSF.DDA.OutlookAppOm._maxRecipients$p=100;
	OSF.DDA.OutlookAppOm._maxSubjectLength$p=255;
	OSF.DDA.OutlookAppOm.maxBodyLength=32768;
	OSF.DDA.OutlookAppOm._maxLocationLength$p=255;
	OSF.DDA.OutlookAppOm._maxEwsRequestSize$p=1e6;
	OSF.DDA.OutlookAppOm._instance$p=null;
	$h.AttachmentConstants.maxAttachmentNameLength=255;
	$h.AttachmentConstants.maxUrlLength=2048;
	$h.AttachmentConstants.maxItemIdLength=200;
	$h.AttachmentConstants.maxRemoveIdLength=200;
	$h.AttachmentConstants.attachmentParameterName="attachments";
	$h.AttachmentConstants.attachmentTypeParameterName="type";
	$h.AttachmentConstants.attachmentUrlParameterName="url";
	$h.AttachmentConstants.attachmentItemIdParameterName="itemId";
	$h.AttachmentConstants.attachmentNameParameterName="name";
	$h.AttachmentConstants.attachmentTypeFileName="file";
	$h.AttachmentConstants.attachmentTypeItemName="item";
	$h.AttachmentDetails._attachmentTypeMap$p=[Microsoft.Office.WebExtension.MailboxEnums.AttachmentType.File,Microsoft.Office.WebExtension.MailboxEnums.AttachmentType.Item];
	$h.Body.coercionTypeParameterName="coercionType";
	$h.ComposeRecipient.displayNameLengthLimit=255;
	$h.ComposeRecipient.maxSmtpLength=571;
	$h.ComposeRecipient.recipientsLimit=100;
	$h.ComposeRecipient.totalRecipientsLimit=500;
	$h.ComposeRecipient.addressParameterName="address";
	$h.ComposeRecipient.nameParameterName="name";
	$h.ComposeLocation.locationKey="location";
	$h.ComposeLocation.maximumLocationLength=255;
	$h.ComposeSubject.maximumSubjectLength=255;
	$h.ComposeTime.timeTypeName="TimeProperty";
	$h.ComposeTime.timeDataName="time";
	$h.Diagnostics.outlookAppName="Outlook";
	$h.Diagnostics.outlookWebAppName="OutlookWebApp";
	$h.EmailAddressDetails._emptyString$p="";
	$h.EmailAddressDetails._responseTypeMap$p=[Microsoft.Office.WebExtension.MailboxEnums.ResponseType.None,Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Organizer,Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Tentative,Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Accepted,Microsoft.Office.WebExtension.MailboxEnums.ResponseType.Declined];
	$h.EmailAddressDetails._recipientTypeMap$p=[Microsoft.Office.WebExtension.MailboxEnums.RecipientType.Other,Microsoft.Office.WebExtension.MailboxEnums.RecipientType.DistributionList,Microsoft.Office.WebExtension.MailboxEnums.RecipientType.User,Microsoft.Office.WebExtension.MailboxEnums.RecipientType.ExternalUser];
	$h.Entities._allEntityKeys$p=["Addresses","EmailAddresses","Urls","PhoneNumbers","TaskSuggestions","MeetingSuggestions","Contacts","FlightReservations","ParcelDeliveries"];
	$h.ReplyConstants.htmlBodyKeyName="htmlBody";
	$h.AsyncConstants.optionsKeyName="options";
	$h.AsyncConstants.callbackKeyName="callback";
	$h.AsyncConstants.asyncResultKeyName="asyncResult";
	$h.MeetingSuggestionTimeDecoder._baseDate$p=new Date("0001-01-01T00:00:00Z");
	$h.NotificationMessages.maximumKeyLength=32;
	$h.NotificationMessages.maximumIconLength=32;
	$h.NotificationMessages.maximumMessageLength=150;
	$h.NotificationMessages.notificationsKeyParameterName="key";
	$h.NotificationMessages.notificationsTypeParameterName="type";
	$h.NotificationMessages.notificationsIconParameterName="icon";
	$h.NotificationMessages.notificationsMessageParameterName="message";
	$h.NotificationMessages.notificationsPersistentParameterName="persistent";
	$h.OutlookErrorManager.errorNameKey="name";
	$h.OutlookErrorManager.errorMessageKey="message";
	$h.OutlookErrorManager._isInitialized$p=false;
	$h.OutlookErrorManager.OutlookErrorCodes.attachmentSizeExceeded=9e3;
	$h.OutlookErrorManager.OutlookErrorCodes.numberOfAttachmentsExceeded=9001;
	$h.OutlookErrorManager.OutlookErrorCodes.internalFormatError=9002;
	$h.OutlookErrorManager.OutlookErrorCodes.invalidAttachmentId=9003;
	$h.OutlookErrorManager.OutlookErrorCodes.invalidAttachmentPath=9004;
	$h.OutlookErrorManager.OutlookErrorCodes.cannotAddAttachmentBeforeUpgrade=9005;
	$h.OutlookErrorManager.OutlookErrorCodes.attachmentDeletedBeforeUploadCompletes=9006;
	$h.OutlookErrorManager.OutlookErrorCodes.attachmentUploadGeneralFailure=9007;
	$h.OutlookErrorManager.OutlookErrorCodes.attachmentToDeleteDoesNotExist=9008;
	$h.OutlookErrorManager.OutlookErrorCodes.attachmentDeleteGeneralFailure=9009;
	$h.OutlookErrorManager.OutlookErrorCodes.invalidEndTime=9010;
	$h.OutlookErrorManager.OutlookErrorCodes.htmlSanitizationFailure=9011;
	$h.OutlookErrorManager.OutlookErrorCodes.numberOfRecipientsExceeded=9012;
	$h.OutlookErrorManager.OutlookErrorCodes.noValidRecipientsProvided=9013;
	$h.OutlookErrorManager.OutlookErrorCodes.cursorPositionChanged=9014;
	$h.OutlookErrorManager.OutlookErrorCodes.invalidSelection=9016;
	$h.OutlookErrorManager.OutlookErrorCodes.accessRestricted=9017;
	$h.OutlookErrorManager.OutlookErrorCodes.genericTokenError=9018;
	$h.OutlookErrorManager.OutlookErrorCodes.genericSettingsError=9019;
	$h.OutlookErrorManager.OutlookErrorCodes.genericResponseError=9020;
	$h.OutlookErrorManager.OutlookErrorCodes.saveError=9021;
	$h.OutlookErrorManager.OutlookErrorCodes.messageInDifferentStoreError=9022;
	$h.OutlookErrorManager.OutlookErrorCodes.duplicateNotificationKey=9023;
	$h.OutlookErrorManager.OutlookErrorCodes.notificationKeyNotFound=9024;
	$h.OutlookErrorManager.OutlookErrorCodes.numberOfNotificationsExceeded=9025;
	$h.OutlookErrorManager.OutlookErrorCodes.ooeInvalidDataFormat=2006;
	$h.OutlookErrorManager.OsfDdaErrorCodes.ooeCoercionTypeNotSupported=1e3;
	$h.CommonParameters.asyncContextKeyName="asyncContext";
	$h.ScriptHelpers.emptyString="";
	OSF.DDA.ErrorCodeManager.initializeErrorMessages(Strings.OfficeOM);
	if(appContext.get_appName()==OSF.AppName.OutlookWebApp)
		this._settings=this._initializeSettings(appContext,false);
	else
		this._settings=this._initializeSettings(false);
	appContext.appOM=new OSF.DDA.OutlookAppOm(appContext,this._webAppState.wnd,appReady)
}

