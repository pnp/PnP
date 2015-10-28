Core.EmbedJavaScript.HeaderFooter.Data = {
    PropertyBag: '{"Breadcrumb": [{"title": "Home","description": "Home","url":"https://github.com/OfficeDev"},{"title": "Product Category","description": "Product Category","url":"https://github.com/OfficeDev"},{"title": "Product","description": "Product","url":"https://github.com/OfficeDev"},{"title": "Example","description": "Example","url":"https://github.com/OfficeDev"}]}',
    AddHostPropertyBag: function (Item, property) {
        var context = PnPCommon.GetContext(PnPApp.AppWebUrl);
        var app = PnPCommon.GetAppContext(context, PnPApp.HostWebUrl);
        var site = app.get_site();
        var web = site.get_rootWeb();
        context.load(web);
        this.myPropBag = web.get_allProperties();
        this.myPropBag.set_item(Item, property);
        web.update();
        context.executeQueryAsync(function () {
            PnPCommon.CloseWaitMessage();
            PnPCommon.NotifyMessage("PropertyBag created");
        }, PnPCommon.NotifyErrorSPJSOM);
    },
    RemovePropertyBag: function () {
        var context = PnPCommon.GetContext(PnPApp.AppWebUrl);
        var app = PnPCommon.GetAppContext(context, PnPApp.HostWebUrl);
        var site = app.get_site();
        var web = site.get_rootWeb();
        context.load(web);
        this.myPropBag = web.get_allProperties();
        this.myPropBag.set_item("vti_GlobalBreadcrumbRibbon", null);
        web.update();
        context.executeQueryAsync(function () {
            PnPCommon.CloseWaitMessage();
            PnPCommon.NotifyMessage("PropertyBag remove");
        }, PnPCommon.NotifyErrorSPJSOM);
    },
    GetBinaryFile: function () {
        var req = $.ajax({
            url: PnPApp.AppUrlFile,
            type: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            cache: false
        }).done(function (fileContents) {
            if (fileContents !== undefined && fileContents.length > 0) {
                PnPData.UploadBinary(fileContents);
            }
            else {
                PnPCommon.NotifyMessage('Error accesing File');
            }
        }).fail(PnPCommon.NotifyError);

    },
    UploadBinary: function (contents) {
        var FileInfo = new SP.FileCreationInformation();
        FileInfo.set_content(new SP.Base64EncodedByteArray());
        for (var i = 0; i < contents.length; i++) {
            FileInfo.get_content().append(contents.charCodeAt(i));
        }
        FileInfo.set_overwrite(true);
        FileInfo.set_url(PnPApp.GlobalFile);
        var context = PnPCommon.GetContext(PnPApp.AppWebUrl);
        var app = PnPCommon.GetAppContext(context, PnPApp.HostWebUrl);
        var site = app.get_site();
        var files = site.get_rootWeb().getFolderByServerRelativeUrl(PnPApp.DestinationFile).get_files();
        context.load(files);
        files.add(FileInfo);

        context.executeQueryAsync(function () {
            PnPCommon.NotifyMessage("File Copy with sucess");
            PnPData.GetSiteCollectionUserCustomAction(PnPData.GetSiteCollectionUserCustomActionAddSuccess);
        }, PnPCommon.NotifyErrorSPJSOM);
    },
    GetFileREST: function (success) {
        var context = PnPCommon.GetContext(PnPCommon.getQueryStringParameter("SPAppWebUrl").replace("#", ""));
        var app = PnPCommon.GetAppContext(context, PnPCommon.getQueryStringParameter("SPHostUrl").replace("#", ""));
        var site = app.get_site();
        var web = site.get_rootWeb();
        context.load(web, 'Url', 'ServerRelativeUrl');

        context.executeQueryAsync(
             function () {
                 PnPApp.RootWeb = web.get_url() + ((web.get_url().indexOf('/', web.get_url().length - 1) !== -1) ? '' : '/');
                 PnPApp.ServerRelativeUrl = web.get_serverRelativeUrl();
                 PnPData.GetFileRESTRootWeb(success);
                 PnPData.GetPropertieBag(PnPApp.GetPropertieBagSuccess);
                 var link = document.getElementById('FileFolder');
                 link.href = PnPApp.RootWeb + PnPApp.DestinationFile;
             }, PnPCommon.NotifyErrorSPJSOM);

    },
    GetFileRESTRootWeb: function (success) {
        var executor = new SP.RequestExecutor(PnPApp.AppWebUrl);
        executor.executeAsync({
            url: PnPApp.AppWebUrl + "/_api/SP.AppContextSite(@target)/web/GetFileByServerRelativeUrl('" + PnPApp.ServerRelativeUrl + "_catalogs/masterpage/Display%20Templates/PnPGlobal.js')/Exists?@target='" + PnPApp.RootWeb + "'",
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: success,
            error: function () {
                var logo = document.getElementById('TBFile');
                logo.src = "/_layouts/15/images/delitem.gif";
            }
        });
    },
    GetSiteCollectionUserCustomAction: function (success) {
        var executor = new SP.RequestExecutor(PnPApp.AppWebUrl);
        executor.executeAsync({
            url: PnPApp.AppWebUrl + "/_api/SP.AppContextSite(@target)/site/usercustomactions?@target='" + PnPApp.HostWebUrl + "'&$select=Name,Id&$filter=Name eq 'PnPGlobalBreadcrumbRibbon'",
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: success,
            error: PnPCommon.NotifyError
        });
    },
    AddHostSiteCollectionUserCustomAction: function () {
        var context = PnPCommon.GetContext(PnPApp.AppWebUrl);
        var app = PnPCommon.GetAppContext(context, PnPApp.HostWebUrl);
        var site = app.get_site();
        UserCustomActions = site.get_userCustomActions();
        newUserCustomAction = UserCustomActions.add();
        newUserCustomAction.set_name("PnPGlobalBreadcrumbRibbon");
        newUserCustomAction.set_title("PnPGlobalBreadcrumbRibbon");
        newUserCustomAction.set_description("Global Breadcrumb and Ribbon is accessible in SP");
        newUserCustomAction.set_location('ScriptLink');
        newUserCustomAction.set_scriptSrc('~SiteCollection/_catalogs/masterpage/Display Templates/PnPGlobal.js?version=' + (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1));
        newUserCustomAction.set_sequence(58);
        newUserCustomAction.update();
        context.executeQueryAsync(
             function () {
                 PnPCommon.NotifyMessage("Custom Global Breadcrumb and Ribbon provisioned in Site Collection");
                 PnPApp.GetInit();
             }, PnPCommon.NotifyErrorSPJSOM
        );
    }
    ,
    RemoveHostSiteCollectionUserCustomAction: function () {
        PnPData.GetSiteCollectionUserCustomAction(PnPData.GetSiteCollectionUserCustomActionSuccess);
    }
    ,
    GetSiteCollectionUserCustomActionAddSuccess: function (data) {
        var results = JSON.parse(data.body).d.results;
        if (results.length === 0) {
            PnPData.AddHostSiteCollectionUserCustomAction();
        }
    },
    GetSiteCollectionUserCustomActionSuccess: function (data) {
        var results = JSON.parse(data.body).d.results;
        for (var i = 0; i < results.length; i++) {
            PnPData.DeleteUserCustomAction(results[i].Id);
            PnPCommon.NotifyMessage("UserCustomAction \"PnPGlobalBreadcrumbRibbon\" is removed from Site Collection");
        }
    },
    DeleteUserCustomAction: function (UCAId) {
        var executor = new SP.RequestExecutor(PnPApp.AppWebUrl);
        executor.executeAsync({
            url: PnPApp.AppWebUrl + "/_api/SP.AppContextSite(@target)/site/UserCustomActions('" + UCAId + "')?@target='" + PnPApp.HostWebUrl + "'",
            method: "POST",
            headers: {
                "IF-MATCH": "*",
                "X-HTTP-Method": "DELETE",
                "Accept": "application/json; odata=verbose"
            },
            success: function () { PnPApp.GetInit(); },
            error: PnPCommon.NotifyError
        });
    },
    DeleteGetFileUrl: function () {
        var executor = new SP.RequestExecutor(PnPApp.AppWebUrl);
        executor.executeAsync({
            url: PnPApp.AppWebUrl + "/_api/SP.AppContextSite(@target)/web/getfilebyserverrelativeurl('" + ((PnPApp.ServerRelativeUrl.length === 1) ? '' : PnPApp.ServerRelativeUrl) + "/_catalogs/masterpage/Display Templates/PnPGlobal.js')?@target='" + PnPApp.RootWeb + "'",
            method: "POST",
            headers: {
                "IF-MATCH": "*",
                "X-HTTP-Method": "DELETE",
                "Accept": "application/json; odata=verbose"
            },
            success: function () { PnPCommon.NotifyMessage("File \"PnPGlobal.js\" was deleted with sucess!"); PnPApp.GetInit(); },
            error: PnPCommon.NotifyError
        });
    },
    GetPropertieBag: function (success) {
        var executor = new SP.RequestExecutor(PnPApp.AppWebUrl);
        executor.executeAsync({
            url: PnPApp.AppWebUrl + "/_api/SP.AppContextSite(@target)/web/allproperties?@target='" + PnPApp.RootWeb + "'&$select=vti_x005f_GlobalBreadcrumbRibbon",
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: success,
            error: PnPCommon.NotifyError
        });
    }
}
window.PnPData = window.Core.EmbedJavaScript.HeaderFooter.Data;
