Core.ManageUserCustomAction.OfficeApp.SP.Data = {
	
	GetTokensUserAction: [{ value: "~SiteCollection", desc: "The URL of the parent site collection of the current website." }, { value: "~Site", desc: "The URL of the current website." }, { value: "{Site}", desc: "The URL of the current website." }, { value: "{SiteCollection}", desc: "The URL of the parent site of the current website." }, { value: "{ItemId}", desc: "ID (GUID) taken from the list view" }, { value: "{ItemUrl}", desc: "Web-relative URL of the list item (Url)" }, { value: "{RecurrenceId}", desc: "ID of a recurrent item (RecurrenceID)" }, { value: "{SiteUrl}", desc: "The fully qualified URL to the site (Url)" }, { value: "{ListId}", desc: "ID (GUID) of the list (ID)." }, { value: "{ListUrlDir}", desc: "Server-relative URL of the site plus the list's folder." }, { value: "{Source}", desc: "Fully qualified request URL." }, { value: "{SelectedListId}", desc: "ID (GUID) of the list that is currently selected from a list view." }, { value: "{SelectedItemId}", desc: "ID of the item that is currently selected from the list view." }, { value: "{Layouts}", desc: "The URL of the Layouts virtual folder for the current website." }, { value: "{ControlTemplates}", desc: "The URL of the ControlTemplates virtual folder for the current website." }],
	GetFileTypes: ["accdb", "accdt", "accdc", "accde", "accdr", "asax", "ascx", "asmx", "asp", "aspx", "bmp", "cat", "chm", "cmp", "config", "css", "db", "dib", "disc", "doc", "docm", "docx", "dot", "dotm", "dotx", "dvd", "dwp", "dwt", "eml", "est", "fwp", "gif", "hdp", "hlp", "hta", "htm", "html", "htt", "inf", "ini", "jfif", "jpe", "jpeg", "jpg", "js", "jse", "log", "master", "mht", "mhtml", "mpd", "mpp", "mps", "mpt", "mpw", "mpx", "msg", "msi", "msp", "ocx", "odc", "odp", "odt", "ods", "one", "onepkg", "onetoc2", "pdf", "png", "pot", "potm", "potx", "ppa", "ppam", "ppt", "pptm", "pptx", "pps", "ppsdc", "ppsm", "ppsx", "psp", "psd", "ptm", "ptt", "pub", "rdl", "rsapplication", "rsc", "rsd", "rsds", "rtf", "smdl", "stp", "stt", "thmx", "tif", "tiff", "txt", "vbe", "vbs", "vdw", "vdx", "vsd", "vsl", "vss", "vst", "vsu", "vsw", "vsx", "vtx", "vsdx", "vsdm", "vssm", "vssx", "vstm", "vstx", "wdp", "webpart", "wm", "wma", "wmd", "wmp", "wms", "wmv", "wmx", "wmz", "wsf", "xla", "xlam", "xls", "xlsb", "xlsm", "xlsx", "xlt", "xltb", "xltm", "xltx", "xml", "xps", "xsd", "xsl", "xsn", "xslt", "zip"],
	GetProgIds: ["Excel.Sheet", "Word.Document", "Word.Document.6", "Word.Document.12", "PowerPoint.Show.8", "PowerPoint.Show.12", "InfoPath.Document.2", "OneNote.Notebook"],
	
	GetCustomActions: function(appWebUrl, hostWebUrl, success) {
        var executor = new SP.RequestExecutor(appWebUrl);
        executor.executeAsync({
            url: this.GetCustomActionsRestUrl(appWebUrl, hostWebUrl),
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: success,
            error: PnPCommon.NotifyError
        });
    },
	GetCustomActionsRestUrl: function(appWebUrl, hostWebUrl) {
        return appWebUrl + "/_api/SP.AppContextSite(@target)/web/UserCustomActions?$orderby=title asc&$select=Id,Name,Title,Description,Location,RegistrationType,RegistrationId&@target='" + hostWebUrl + "'";
    },
	GetWebUserCustomAction: function (appWebUrl, hostWebUrl, id, success) {
	    var executor = new SP.RequestExecutor(appWebUrl);
	    executor.executeAsync({
	        url: this.GetWebUserCustomActionIdRestUrl(appWebUrl, hostWebUrl, id),
	        method: "GET",
	        headers: { "Accept": "application/json; odata=verbose" },
	        success: success,
	        error: PnPCommon.NotifyError
	    });
	},
	GetWebUserCustomActionIdRestUrl: function (appWebUrl, hostWebUrl, userCustomActionId) {
	    return appWebUrl + "/_api/SP.AppContextSite(@target)/web/UserCustomActions('" + userCustomActionId + "')?@target='" + hostWebUrl + "'";
	},
	GetListContentTypePicker: function (appWebUrl, hostWebUrl, idct, success) {
	    var executor = new SP.RequestExecutor(appWebUrl);
	    executor.executeAsync(
        {
            url: PnPData.GetListContentTypePickerRestUrl(appWebUrl, hostWebUrl, idct),
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                success([JSON.parse(data.body).d.results, idct]);
            },
            error: PnPCommon.NotifyError
        });
	},
	GetListContentTypePickerRestUrl: function (appWebUrl, hostWebUrl, idct) {
	    if (idct == 'CT') {
	        return appWebUrl + "/_api/SP.AppContextSite(@target)/web/ContentTypes?$select=Name,Id,Group&$orderby=Name&$filter=Group ne '_Hidden'&@target='" + hostWebUrl + "'";
	    } else if (idct === 'Lists') {
	        return appWebUrl + "/_api/SP.AppContextSite(@target)/web/Lists?$select=Title,Id,ImageUrl&@target='" + hostWebUrl + "'";
	    } else {
	        return appWebUrl + "/_api/SP.AppContextSite(@target)/web/Lists('" + idct + "')/ContentTypes?$select=Id,Name,Group&$filter=Group ne '_Hidden'&@target='" + hostWebUrl + "'";
	    }
	},
	AddUserCustomAction: function () {
	    var context = new SP.ClientContext(PnPApp.AppWebUrl);
	    var factory =new SP.ProxyWebRequestExecutorFactory(PnPApp.AppWebUrl);
	    context.set_webRequestExecutorFactory(factory);
	    var app = new SP.AppContextSite(context, PnPApp.HostWebUrl);
	    var web= app.get_web();
	    UserCustomActions = web.get_userCustomActions();
	
	    newUserCustomAction = UserCustomActions.add();
	    newUserCustomAction.set_name($.trim($('#dlxCustomActionName').val()));
	    newUserCustomAction.set_title($.trim($('#dlxCustomActionTitle').val()));
	    newUserCustomAction.set_description($.trim($('#dlxCustomActionDescription').val()));
	    newUserCustomAction.set_imageUrl($.trim($('#dlxCustomActionImageUrl').val()));
	    if ($('#dlxCustomActionLocation').find(':selected').val()==="")
	    {
	        newUserCustomAction.set_location($.trim($('#dlxCustomActionLocation').find(':selected').text()));
	    }
	    else{
	        newUserCustomAction.set_location($.trim($('#dlxCustomActionLocation').find(':selected').val()));
	        newUserCustomAction.set_group($.trim($('#dlxCustomActionLocation').find(':selected').text()));
	    }
	    newUserCustomAction.set_registrationType(parseInt($('#dlxCustomActionRegistrationType').find(':selected').val()));
	    newUserCustomAction.set_registrationId($.trim($('#dlxCustomActionRegistrationId').val()));
	    if ($.trim($('#dlxCustomActionLocation').find(':selected').text()).toLowerCase() === "scriptlink") {
	        if ($.trim($('#dlxCustomActionScriptSrc').val()).toLowerCase().indexOf('~site', 0) == 0) {
	            newUserCustomAction.set_scriptSrc($.trim($('#dlxCustomActionScriptSrc').val()));
	        } else {
	            newUserCustomAction.set_scriptSrc('~site' + $('#dlxCustomActionScriptSrc').val().trim());
	        }
	        newUserCustomAction.set_scriptBlock($('#dlxCustomActionScriptBlock').val());
	    }
	    newUserCustomAction.set_url($.trim($('#dlxCustomActionUrl').val()));
	    newUserCustomAction.set_sequence(parseInt($('#dlxCustomActionSequence').val()));
		
	    if($.trim($('#dlxCustomActionRights').val())!=="")
	    {
	        newUserCustomAction.set_rights(PnPCommon.GetRightsID());
	    }   
	    if($.trim($('#dlxCustomActionCommandUIExtension').val())!=="")
	    {
	        newUserCustomAction.set_commandUIExtension($('#dlxCustomActionCommandUIExtension').val());
	    }
	    newUserCustomAction.update();
    
	    context.executeQueryAsync(
             function () {
                 PnPCommon.NotifyMessage("Custom Action created with success");
                 PnPApp.GetInit();
             }, PnPCommon.NotifyErrorSPJSOM
        );
	},
	UpdateUserCustomAction: function () {
	    var context = new SP.ClientContext(PnPApp.AppWebUrl);
	    var factory = new SP.ProxyWebRequestExecutorFactory(PnPApp.AppWebUrl);
	    context.set_webRequestExecutorFactory(factory);
	    var app = new SP.AppContextSite(context, PnPApp.HostWebUrl);
	    var Web= app.get_web();
	    var collUserCustomAction = Web.get_userCustomActions();

	    context.load(Web, 'UserCustomActions');
	    context.executeQueryAsync(
            function () {
                var id = $('#dlxUserCustomActionsOptions').find(':selected').val();
                var customActionEnumerator = collUserCustomAction.getEnumerator();

                while (customActionEnumerator.moveNext()) {
                    var oUserCustomAction = customActionEnumerator.get_current();

                    if (oUserCustomAction.get_id() == id) {

                        oUserCustomAction.set_name($.trim($('#dlxCustomActionName').val()));
                        oUserCustomAction.set_title($.trim($('#dlxCustomActionTitle').val()));
                        oUserCustomAction.set_description($.trim($('#dlxCustomActionDescription').val()));
                        oUserCustomAction.set_imageUrl($.trim($('#dlxCustomActionImageUrl').val()));
                        if ($('#dlxCustomActionLocation').find(':selected').val() === "") {
                            oUserCustomAction.set_location($.trim($('#dlxCustomActionLocation').find(':selected').text()));
                        }
                        else {
                            oUserCustomAction.set_location($.trim($('#dlxCustomActionLocation').find(':selected').val()));
                            oUserCustomAction.set_group($.trim($('#dlxCustomActionLocation').find(':selected').text()));
                        }

                        oUserCustomAction.set_registrationType(parseInt($('#dlxCustomActionRegistrationType').find(':selected').val()));
                        oUserCustomAction.set_registrationId($.trim($('#dlxCustomActionRegistrationId').val()));

                        if ($.trim($('#dlxCustomActionLocation').find(':selected').text()).toLowerCase() === "scriptlink") {
                            if ($.trim($('#dlxCustomActionScriptSrc').val()).toLowerCase().indexOf('~site', 0) == 0) {
                                oUserCustomAction.set_scriptSrc($.trim($('#dlxCustomActionScriptSrc').val()));
                            } else {
                                oUserCustomAction.set_scriptSrc('~site' + $('#dlxCustomActionScriptSrc').val().trim());
                            }
                            oUserCustomAction.set_scriptBlock($('#dlxCustomActionScriptBlock').val());
                        }

                        oUserCustomAction.set_url($.trim($('#dlxCustomActionUrl').val()));
                        oUserCustomAction.set_sequence(parseInt($('#dlxCustomActionSequence').val()));

                        if ($.trim($('#dlxCustomActionRights').val()) !== "") {
                            oUserCustomAction.set_rights(PnPCommon.GetRightsID());
                        }

                        if ($.trim($('#dlxCustomActionCommandUIExtension').val()) !== "") {
                            oUserCustomAction.set_commandUIExtension($('#dlxCustomActionCommandUIExtension').val());
                        }

                        oUserCustomAction.update();
                        context.executeQueryAsync(
                         function () {
                             PnPCommon.CloseWaitMessage();
                             PnPCommon.NotifyMessage("Custom Action updated with success!");
                             PnPApp.GetInit();
                         }, PnPCommon.NotifyErrorSPJSOM);
                    }
                }
            }, PnPCommon.NotifyErrorSPJSOM);
	    
	},
	DeleteUserCustomAction: function (appWebUrl, hostWebUrl, userCustomActionId, success) {
	    var executor = new SP.RequestExecutor(appWebUrl);
	    executor.executeAsync(
        {
            url: PnPData.GetWebUserCustomActionIdRestUrl(appWebUrl, hostWebUrl, userCustomActionId),
            method: "POST",
            headers: {
                "IF-MATCH": "*",
                "X-HTTP-Method": "DELETE",
                "Accept": "application/json; odata=verbose"
            },
            success: success,
            error: PnPCommon.NotifyError
        });
	},
	GetPermissions: function () {
	    for (var prop in SP.PermissionKind.prototype) {
	        if (SP.PermissionKind.prototype.hasOwnProperty(prop)) {
	            $("#PnPRights").append('<div id="' + prop + '" onclick=\"document.getElementById(\'dlxCustomActionRights\').value=document.getElementById(\'dlxCustomActionRights\').value + this.id+\', \'\">' + prop + '</div>');
	        }
	    }
	},
	GetListTemplates: function () {
	    var listTemplates = [];
	    for (var prop in SP.ListTemplateType.prototype) {
	        if (SP.ListTemplateType.prototype.hasOwnProperty(prop)) {
	            $("#PnPListTemplates").append('<div id="' + SP.ListTemplateType.prototype[prop] + '" onclick=\"document.getElementById(\'dlxCustomActionRegistrationId\').value=this.id\">' + prop + ':' + SP.ListTemplateType.prototype[prop] + '</div>');
	        }
	    }
	},
	GetTokens: function (InputHtml, Lisbox) {
	    for (var i = 0; i < PnPData.GetTokensUserAction.length; i++) {
	        $(Lisbox).append('<div id="' + PnPData.GetTokensUserAction[i].value + '" onclick=\" $(\'' + InputHtml + '\').val($(\'' + InputHtml + '\').val() + this.id + \' \'\)"><b>' + PnPData.GetTokensUserAction[i].value + '</b>:' + PnPData.GetTokensUserAction[i].desc + '</div>');
	    }
	},
    GetProgId: function () {
        for (var i = 0; i < PnPData.GetProgIds.length; i++) {
            $("#PnPProgId").append('<div id="' + PnPData.GetProgIds[i] + '" onclick=\"document.getElementById(\'dlxCustomActionRegistrationId\').value=this.id\">' + PnPData.GetProgIds[i] + '</div>');
        }
    },
    GetFileType: function () {
        for (var i = 0; i < PnPData.GetFileTypes.length; i++) {
            $("#PnPFileTypes").append('<div id="' + PnPData.GetFileTypes[i] + '" onclick=\"document.getElementById(\'dlxCustomActionRegistrationId\').value=document.getElementById(\'dlxCustomActionRegistrationId\').value+this.id+\', \'\">' + PnPData.GetFileTypes[i] + '</div>');
        } 
    }
}
window.PnPData = window.Core.ManageUserCustomAction.OfficeApp.SP.Data;