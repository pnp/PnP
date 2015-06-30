Core.EmbedJavaScript.HeaderFooter.Common = {
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
    getQueryStringParameter: function (param) {
        var params = document.URL.split("?")[1].split("&");
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split("=");
            if (singleParam[0] == param)
                return decodeURIComponent(singleParam[1]);
        }
        return "";
    },
    GetContext: function (appWebUrl) {
        var context = new SP.ClientContext(appWebUrl);
        context.set_webRequestExecutorFactory(new SP.ProxyWebRequestExecutorFactory(appWebUrl));
        return context;
    },
    GetAppContext: function (context, hostWebUrl) {
        return new SP.AppContextSite(context, hostWebUrl);
    },
    IsJsonString: function (str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }
}
window.PnPCommon = window.Core.EmbedJavaScript.HeaderFooter.Common;