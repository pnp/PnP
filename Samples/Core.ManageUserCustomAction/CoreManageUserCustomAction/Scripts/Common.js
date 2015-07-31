Core.ManageUserCustomAction.OfficeApp.SP.Common = {

    ShowWaitMessage: function () {
        if (PnPApp.waitForm === null) {
            PnPApp.waitForm = SP.UI.ModalDialog.showWaitScreenWithNoClose("Loading", "Your request is being processed. Please wait while this process completes.");
        }
    },
	CloseWaitMessage: function () {
        if (PnPApp.waitForm !== null) {
            PnPApp.waitForm.close(0);
            PnPApp.waitForm = null;
        }
    },
	AssetPortalBrowserPageUrl: "/_layouts/15/AssetPortalBrowser.aspx",
    NotifyMessage: function (msg) {
        SP.UI.Notify.addNotification('<span>' + msg + '</span>', false);
    },
    NotifyError: function (xhr) {
        PnPCommon.CloseWaitMessage();
        SP.UI.Notify.addNotification('<span class=\'plxValidationErrors ms-status-red\'> Request failed. ' + xhr.status + '\n' + xhr.statusText + '</span>', false);
        $('.ms-trcnoti-bg').css('background-color', '#f8d4d4');
        $('.ms-trcnoti-bg').css('border-color', '#f5a6a7');
    },
    NotifyErrorSPJSOM: function (sender, args) {
        PnPCommon.CloseWaitMessage();
        SP.UI.Notify.addNotification('<span class=\'plxValidationErrors ms-status-red\'> Request failed. ' + args.get_message() + '\n' + args.get_stackTrace() + '</span>', false);
        $('.ms-trcnoti-bg').css('background-color', '#f8d4d4');
        $('.ms-trcnoti-bg').css('border-color', '#f5a6a7');
    },
	OpenAssetPortalBrowserDialog: function(context, appContext, dialogCallback) {
        var web = appContext.get_web();
        context.load(web);
        context.executeQueryAsync(function() {
            SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', { url: context.get_url() + "/.." + PnPCommon.AssetPortalBrowserPageUrl + "?&AssetType=Link&AssetUrl=&RootFolder=&MDWeb=" + web.get_id(), dialogReturnValueCallback: dialogCallback });
        }, this.ExecuteQueryFailed);
    },
	getQueryStringParameter: function(param) {
        var params = document.URL.split("?")[1].split("&");
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split("=");
            if (singleParam[0] == param)
                return decodeURIComponent(singleParam[1]);
        }
        return "";
	},
	isImage: function (filename) {
	    var ext = PnPCommon.getExtension(filename);
	    switch (ext.toLowerCase()) {
	        case 'jpg':
	        case 'gif':
	        case 'bmp':
	        case 'png':
	            return true;
	    }
	    return false;
	},
	isJS: function (filename) {
	    var ext = PnPCommon.getExtension(filename);
	    switch (ext.toLowerCase()) {
	        case 'js':
	            return true;
	    }
	    return false;
	},
	getExtension: function (filename) {
	    var parts = filename.split('.');
	    return parts[parts.length - 1];
	},
	GetContext: function (appWebUrl) {
	    var context = new SP.ClientContext(appWebUrl);
	    context.set_webRequestExecutorFactory(new SP.ProxyWebRequestExecutorFactory(appWebUrl));
	    return context;
	},
	GetAppContext: function (context, hostWebUrl) {
	    return new SP.AppContextSite(context, hostWebUrl);
	},
	LaunchTargetPicker: function () {
	    var callback = function (dest) {
	        if (dest != null && dest[0] != null) {
	            document.getElementById('dlxCustomActionRegistrationId').value = "{" + dest[0].split('?')[0].split(':')[1] + "}";
	        }
	    };
	    var iconUrl = "/_layouts/15/images/smt_icon.gif?rev=32";
	    SP.SOD.executeFunc('pickertreedialog.js', 'LaunchPickerTreeDialogSelectUrl', function () {
	        SP.UI.ModalDialog.showWaitScreenWithNoClose("Please wait...", "Please wait...", 100, 300);
	        window.LaunchPickerTreeDialogSelectUrl('CbqPickerSelectListTitle', 'CbqPickerSelectListText', 'websLists', '', PnPApp.AppWebUrl + "/../", '', '', '', iconUrl, '', callback, 'true', '');
	        SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK);
	    });
	},
	GetRightsID: function (){
	var permissions = new SP.BasePermissions();	
    var value = $('#dlxCustomActionRights').val().replace(' ','').split(',');
    var rightDescription="";
    for (var i=0;i<value.length;i++)
    {
        if (value[i].trim() !== 'undefined') { if (value[i].trim() !== '') { permissions.set(parseInt(SP.PermissionKind.prototype[value[i].trim()])); } }
    }
    return permissions;
    },
	CleanForm: function () {
	    $('#dlxUpdateUserCustomAction').prop('disabled', true);
	    $('#dlxDeleteUserCustomAction').prop('disabled', true);
	    $("#ZoneScriptBlock").css('display', 'none');
	    $("#ZoneScriptSrc").css('display', 'none');
	    $('#dlxUserCustomActionsOptions').find(':selected').prop('selected', false);
        $('#dlxUpdateUserCustomAction').val("Update");
        $('#dlxCustomActionID').val("");
        $('#dlxCustomActionName').val("");
        $('#dlxCustomActionTitle').val("");
        $('#dlxCustomActionDescription').val("");
        $('#dlxCustomActionImageUrl').val("");
        $('#dlxCustomActionLocation option[value="-1"]').prop('selected', true);
        $('#dlxCustomActionRegistrationType option[value="0"]').prop('selected', true);
        $('#dlxCustomActionRegistrationType').change();
        $('#dlxCustomActionRights').val("");
        $('#dlxCustomActionRegistrationId').val("");
        $('#dlxCustomActionScriptBlock').val("");
        $('#dlxCustomActionScriptSrc').val("");
        $('#dlxCustomActionSequence').val("0");
        $('#dlxCustomActionUrl').val("");
        $('#dlxCustomActionCommandUIExtension').val("");
        $('#dlxCustomActionVersionOfUserCustomAction').val("");
	},
	CallDataOnFocus: function (InputId,Lisbox) {
	    
	    $(InputId).focusin(function () {
	        $(Lisbox).css("display", "block");
	        $(Lisbox).css("margin-left", "120px");
	        $(Lisbox).css("margin-top", "0px");
	    }).click(function () {
	        $(Lisbox).css("display", "block");
	        $(Lisbox).css("margin-left", "120px");
	        $(Lisbox).css("margin-top", "0px");
	    });
	    $(InputId).parent().mouseleave(function () { $(Lisbox).css("display", "none"); });
	    $(Lisbox).click(function () {
	        $(Lisbox).css("display", "none");
	    });
	},
	CallDataOnFocusAfterInsert: function (InputId, Lisbox) {
	    PnPData.GetTokens(InputId, Lisbox);
	    $(InputId).focusin(function () {
	        $(Lisbox).css("display", "block");
	        $(Lisbox).css("margin-left", "120px");
	        $(Lisbox).css("margin-top", "0px");
	    }).click(function () {
	        $(Lisbox).css("display", "block");
	        $(Lisbox).css("margin-left", "120px");
	        $(Lisbox).css("margin-top", "0px");
	    });
	    $(InputId).parent().mouseleave(function () { $(Lisbox).css("display", "none"); });
	    $(Lisbox).click(function () {
	        $(Lisbox).css("display", "none");
	    });
	},
    GetRights: function(rights){
        var value = "";
        var ValidateRights = new SP.BasePermissions();
        ValidateRights.fromJson(rights);
        for (var availableRights in SP.PermissionKind.prototype)
        {
            if (ValidateRights.has(parseInt(SP.PermissionKind.prototype[availableRights])))
            {
                value += availableRights + ",";
            }
        }
        if (value.charAt(value.length - 1) ==="," )
        {
            $('#dlxCustomActionRights').val(value.slice(0, -1));
        }
        else {
            $('#dlxCustomActionRights').val(value);
        }
    }
}
window.PnPCommon = window.Core.ManageUserCustomAction.OfficeApp.SP.Common;
