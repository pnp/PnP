Core.ManageUserCustomAction.OfficeApp.SP.App = {
	
	HostWebUrl: PnPCommon.getQueryStringParameter("SPHostUrl").replace("#", ""),
	AppWebUrl: PnPCommon.getQueryStringParameter("SPAppWebUrl").replace("#", ""),
    waitForm: null,
    GetInit: function () {
        PnPData.GetTokens();
        PnPData.GetPermissions();
        PnPData.GetListTemplates();
        PnPData.GetListContentTypePicker(PnPApp.AppWebUrl, PnPApp.HostWebUrl, 'CT', PnPApp.GetListContentTypePickerSuccess);
        PnPData.GetListContentTypePicker(PnPApp.AppWebUrl, PnPApp.HostWebUrl, 'Lists', PnPApp.GetListContentTypePickerSuccess);
        PnPData.GetProgId();
        PnPData.GetFileType();

        PnPCommon.CallDataOnFocusAfterInsert("#dlxCustomActionImageUrl", ".PnPTokensImageUrl");
        PnPCommon.CallDataOnFocusAfterInsert("#dlxCustomActionScriptBlock", ".PnPTokensScriptBlock");
        PnPCommon.CallDataOnFocusAfterInsert("#dlxCustomActionScriptSrc", ".PnPTokensScriptSrc");
        PnPCommon.CallDataOnFocusAfterInsert("#dlxCustomActionUrl", ".PnPTokensUrl");
        PnPCommon.CallDataOnFocusAfterInsert("#dlxCustomActionCommandUIExtension", ".PnPTokensUIExtension");
        PnPCommon.CallDataOnFocus("#dlxCustomActionRights", "#PnPRights");
        
        PnPCommon.CleanForm();
        PnPApp.GetCustomActions();
        PnPApp.EditCustomActions();
        $('#dlxUpdateUserCustomAction').prop('disabled', true);
        $('#dlxDeleteUserCustomAction').prop('disabled', true);
        $('#dlxCustomActionLocation').change(function () {
            if ($('#dlxCustomActionLocation').find(':selected').text().trim() === "ScriptLink")
            {
                $("#ZoneScriptBlock").css('display', 'block');
                $("#ZoneScriptSrc").css('display', 'block');
            }
            else {
                $("#ZoneScriptBlock").css('display', 'none');
                $("#ZoneScriptSrc").css('display', 'none');
            }
        });
        $('#dlxCustomActionRegistrationType').change(function () {
            if ($(this).val() === '0') {
                $(".dropdown").css("display", "none");
                $("#BtnBrowseListFolder").css('display', 'none');
                $("#PnPContentTypePicker").css('display', 'none');
                $(".dropdown").css("display", "none");
                $("#dlxCustomActionRegistrationId").off();
                
            }
            if ($(this).val() === '1') {
                $("#BtnBrowseListFolder").css('display', 'inline');
                $("#PnPContentTypePicker").css('display', 'none');
                $(".dropdown").css("display", "none");
                $("#dlxCustomActionRegistrationId").off();
                PnPCommon.CallDataOnFocus("#dlxCustomActionRegistrationId", "#PnPListTemplates");
            }
            else if ($(this).val() === '2') {
                $(".dropdown").css("display", "none");
                $("#BtnBrowseListFolder").css('display', 'none');
                $("#PnPContentTypePicker").css('display', 'inline-table');
                $("#dlxCustomActionRegistrationId").off();
                PnPCommon.CallDataOnFocus("#dlxCustomActionRegistrationId", "#PnPContentTypes");
            }
            else if ($(this).val() === '3') {
                $(".dropdown").css("display", "none");
                $("#BtnBrowseListFolder").css('display', 'none');
                $("#PnPContentTypePicker").css('display', 'none');
                $("#dlxCustomActionRegistrationId").off();
                PnPCommon.CallDataOnFocus("#dlxCustomActionRegistrationId", "#PnPProgId");
            }
            else if ($(this).val() === '4') {
                $(".dropdown").css("display", "none");
                $("#BtnBrowseListFolder").css('display', 'none');
                $("#PnPContentTypePicker").css('display', 'none');
                $("#dlxCustomActionRegistrationId").off();
                PnPCommon.CallDataOnFocus("#dlxCustomActionRegistrationId", "#PnPFileTypes");
            }
            else {
                $("#BtnBrowseListFolder").css('display', 'none');
                $("#PnPContentTypePicker").css('display', 'none');
            }
        });
    },
	GetCustomActions: function() {
		 PnPCommon.ShowWaitMessage();
		 PnPData.GetCustomActions(this.AppWebUrl, this.HostWebUrl, PnPApp.GetCustomActionsSuccess);
    },
    GetCustomActionsSuccess: function(data) {
        var results = JSON.parse(data.body).d.results;
        $("#dlxUserCustomActionsOptions").empty();
		for (var i = 0; i < results.length; i++) {
		    $("#dlxUserCustomActionsOptions").append('<option value="' + results[i].Id + '" title="' + results[i].Description + '">' + results[i].Title + '</option>');
		}
		PnPCommon.CloseWaitMessage();
    },
    GetListContentTypePickerSuccess: function (data) {
       
        var jsonObject = data[0];

        for (var i = 0; i < jsonObject.length; i++) {
            if (data[1] === 'Lists') {
                $("#PnPLists").append('<li id="' + jsonObject[i].Id + '" onclick=\"document.getElementById(\'dlxCustomActionRegistrationId\').value=\'{\'+this.id+\'}\'\" ><img  src="' + jsonObject[i].ImageUrl + '" >' + jsonObject[i].Title + '</li>');
            } else if (data[1] === 'CT') {
                $("#PnPContentType").append('<li style="padding-bottom:5px;" id="' + jsonObject[i].Id.StringValue + '" onclick=\"document.getElementById(\'dlxCustomActionRegistrationId\').value=this.id\"><img src="../_layouts/15/images/pageLayoutHS.png" >' + jsonObject[i].Name + "(" + jsonObject[i].Group + ")" + '</li>');
            }
            else {
                $("#" + data[1]).append('<ul style="padding-left:20px;padding-bottom:5px;" id="' + jsonObject[i].Id.StringValue + '" onclick=\"document.getElementById(\'dlxCustomActionRegistrationId\').value=this.id\"><img src="../_layouts/15/images/pageLayoutHS.png" >' + jsonObject[i].Name + "(" + jsonObject[i].Group + ")" + '</li></ul>');
            }
            if (data[1] == 'Lists') {
                PnPData.GetListContentTypePicker(PnPApp.AppWebUrl, PnPApp.HostWebUrl, jsonObject[i].Id, PnPApp.GetListContentTypePickerSuccess);
            }
        }
    },
    NewCustomAction: function () {
        var NewAnswer = confirm("Do you want to create the UserCustomAction:\n \"" + $('#dlxCustomActionTitle').val());
        if (NewAnswer === true) {
            PnPCommon.ShowWaitMessage();
            PnPData.AddUserCustomAction();
        }
    },
    UpdateCustomAction: function () {
        var UpdateAnswer = confirm("Do you want to update the UserCustomAction:\n \"" + $('#dlxUserCustomActionsOptions').find(':selected').text() + "\"(" + $('#dlxUserCustomActionsOptions').find(':selected').val() + ")");
        if (UpdateAnswer === true) {
            PnPCommon.ShowWaitMessage();
            PnPData.UpdateUserCustomAction();
        }
    },
    EditCustomActions : function(){
        $('#dlxUserCustomActionsOptions').change(function() {
            $('#dlxUpdateUserCustomAction').prop('disabled', false);
            $('#dlxDeleteUserCustomAction').prop('disabled', false);
            PnPCommon.ShowWaitMessage();
            PnPData.GetWebUserCustomAction(PnPApp.AppWebUrl, PnPApp.HostWebUrl, $('#dlxUserCustomActionsOptions').find(':selected').val(), PnPApp.GetCustomActionIdSuccess);
        });
    },
    GetCustomActionIdSuccess : function(data) {
        var result = JSON.parse(data.body).d;

        $('#dlxCustomActionID').val(result.Id);
        $('#dlxCustomActionName').val(result.Name);
        $('#dlxCustomActionTitle').val(result.Title);
        $('#dlxCustomActionDescription').val(result.Description);
        $('#dlxCustomActionImageUrl').val(result.ImageUrl);

        if (result.Location === "ScriptLink") {
            $("#ZoneScriptBlock").css('display', 'block');
            $("#ZoneScriptSrc").css('display', 'block');
        } else {
            $("#ZoneScriptBlock").css('display', 'none');
            $("#ZoneScriptSrc").css('display', 'none');
        }
        if ((result.Group === null) || (result.Group === "")) {
            $('#dlxCustomActionLocation optgroup option').each(function () {
                if ($(this).text().trim() === result.Location) {
                    $(this).prop('selected', true);
                }
            });
        }
        else {
            $('#dlxCustomActionLocation option:contains("' + result.Group + '")').each(function () {
                if ($(this).val().trim() === result.Location) {
                    $(this).prop('selected', true);
                }
            });
        }
        
        $('#dlxCustomActionRegistrationType option[value="' + result.RegistrationType + '"]').prop('selected', true);
        $('#dlxCustomActionRegistrationType').change();
        $('#dlxCustomActionRegistrationId').val(result.RegistrationId);
      
        PnPCommon.GetRights(result.Rights);
        $('#dlxCustomActionScriptBlock').val(result.ScriptBlock);
        $('#dlxCustomActionScriptSrc').val(result.ScriptSrc);
        $('#dlxCustomActionSequence').val(result.Sequence);
        $('#dlxCustomActionUrl').val(result.Url);
        $('#dlxCustomActionCommandUIExtension').val(result.CommandUIExtension);
        $('#dlxCustomActionVersionOfUserCustomAction').val(result.VersionOfUserCustomAction);
    
        PnPCommon.CloseWaitMessage();
    },

    DeleteUserCustomAction: function () {
        var deleteAnswer = confirm("Do you want to delete the UserCustomAction:\n \"" + $('#dlxUserCustomActionsOptions').find(':selected').text() + "\"(" + $('#dlxUserCustomActionsOptions').find(':selected').val() + ")");
        if (deleteAnswer == true) {
            PnPCommon.ShowWaitMessage();
            PnPData.DeleteUserCustomAction(PnPApp.AppWebUrl, PnPApp.HostWebUrl, $('#dlxUserCustomActionsOptions').find(':selected').val(), PnPApp.GetdeleteCustomActionIdSuccess);
        }
        
    },
    GetdeleteCustomActionIdSuccess: function (data) {
        PnPCommon.CloseWaitMessage();
        PnPCommon.NotifyMessage('UserCustomAction deleted with success!');
        PnPApp.GetInit();
    },
    OpenAssetPortalBrowserDialog: function (type, targetinput, urlToken) {
        var context = PnPApp.GetContext();
        var appContext = PnPApp.GetAppContext(context);
        var dialogCallback = function (result, target) {
            if (result == SP.UI.DialogResult.OK) {
                if ((type == "Image" && PnPCommon.isImage(target.AssetText)) || (type == "JS" && PnPCommon.isJS(target.AssetText))) {
                    var targetUrl = target.AssetUrl.replace(PnPApp.GetLocation(PnPApp.HostWebUrl).pathname, '');
                    if (targetUrl.charAt(0) == "/" && targetUrl.charAt(1) == "/") {
                        if (urlToken == 'true') { document.getElementById(targetinput).value = '~Site' + targetUrl.substring(1, targetUrl.length); }
                        else { document.getElementById(targetinput).value = target.AssetUrl; }
                    } else {
                        if (urlToken == 'true') {
                            if (target.AssetUrl.replace(PnPApp.GetLocation(PnPApp.HostWebUrl).pathname, '').charAt(0) == "/") {
                                document.getElementById(targetinput).value = '~Site' + target.AssetUrl.replace(PnPApp.GetLocation(PnPApp.HostWebUrl).pathname, '');
                            } else {
                                document.getElementById(targetinput).value = '~Site/' + target.AssetUrl.replace(PnPApp.GetLocation(PnPApp.HostWebUrl).pathname, '');
                            }
                        }
                        else { document.getElementById(targetinput).value = target.AssetUrl; }
                    }
                } else if (type === "Image") { PnPCommon.NotifyMessage("Select Image Type 'jpg,gif,bmp,png'"); }
                else if (type === "JS") { PnPCommon.NotifyMessage("Select JavaScript Type 'js'"); }
            }
        };
        PnPCommon.OpenAssetPortalBrowserDialog(context, appContext, dialogCallback);
    },
    GetContext: function () {
        return PnPCommon.GetContext(PnPApp.AppWebUrl);
    },
    GetAppContext: function (context) {
        return PnPCommon.GetAppContext(context, PnPApp.HostWebUrl);
    },
    GetLocation: function (href) {
        var l = document.createElement("a");
        l.href = href;
        return l;
    }
}
window.PnPApp = window.Core.ManageUserCustomAction.OfficeApp.SP.App;

$(document).ready(function () {
    PnPApp.GetInit();
});