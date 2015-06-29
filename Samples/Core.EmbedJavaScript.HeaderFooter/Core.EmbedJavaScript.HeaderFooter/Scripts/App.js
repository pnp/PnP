Core.EmbedJavaScript.HeaderFooter.App = {
    HostWebUrl: PnPCommon.getQueryStringParameter("SPHostUrl").replace("#", ""),
    AppWebUrl: PnPCommon.getQueryStringParameter("SPAppWebUrl").replace("#", ""),
    AppUrlFile: PnPCommon.getQueryStringParameter("SPAppWebUrl").replace("#", "") + '/Scripts/PnPGlobal.js',
    DestinationFile: '_catalogs/masterpage/Display Templates',
    GlobalFile: 'PnPGlobal.js',
    waitForm: null,
    RootWeb: null,
    ServerRelativeUrl: null,
    GetInit: function () {

        PnPData.GetFileREST(PnPApp.GetFileRESTSuccess);
        PnPData.GetSiteCollectionUserCustomAction(PnPApp.GetSiteCollectionUserCustomActionValidationSuccess);
    },
    GetPropertieBagSuccess: function (data) {
        var results = JSON.parse(data.body).d;
        if (results.vti_x005f_GlobalBreadcrumbRibbon !== undefined) {
            var logo = document.getElementById('TBPropertyBag');
            logo.src = "/_layouts/15/images/check.gif";
        } else {
            var logo = document.getElementById('TBPropertyBag');
            logo.src = "/_layouts/15/images/delitem.gif";
        }
    },
    GetSiteCollectionUserCustomActionValidationSuccess: function (data) {
        var results = JSON.parse(data.body).d.results;
        if (results !== undefined && results.length > 0) {
            var logo = document.getElementById('TBUserCustomAction');
            logo.src = "/_layouts/15/images/check.gif";
        } else {
            var logo = document.getElementById('TBUserCustomAction');
            logo.src = "/_layouts/15/images/delitem.gif";
        }
    },
    GetFileRESTSuccess: function (data) {
        var Exists = JSON.parse(data.body).d.Exists;
        if (Exists) {
            var logo = document.getElementById('TBFile');
            logo.src = "/_layouts/15/images/check.gif";
        } else {
            var logo = document.getElementById('TBFile');
            logo.src = "/_layouts/15/images/delitem.gif";
        }
    },
    AddProvisionOfUserCustomAction: function () {
        if (document.getElementById('PropertyBagJSON').value !== "") {
            if (PnPCommon.IsJsonString(document.getElementById('PropertyBagJSON').value)) {
                PnPData.AddHostPropertyBag('vti_GlobalBreadcrumbRibbon', document.getElementById('PropertyBagJSON').value);
            } else {
                PnPData.AddHostPropertyBag('vti_GlobalBreadcrumbRibbon', PnPData.PropertyBag);
                PnPCommon.NotifyMessage("JSON Parse is incorrect, default value was included");
            }
        } else {
            PnPData.AddHostPropertyBag('vti_GlobalBreadcrumbRibbon', PnPData.PropertyBag);
            PnPCommon.NotifyMessage("JSON Parse is incorrect, default value was included");
        }
        PnPData.GetBinaryFile();
    },
    RemoveProvisionOfUserCustomAction: function () {
        PnPData.RemovePropertyBag();
        PnPData.RemoveHostSiteCollectionUserCustomAction();
        PnPData.DeleteGetFileUrl();
        PnPApp.GetInit();
    }
}
window.PnPApp = window.Core.EmbedJavaScript.HeaderFooter.App;
$(document).ready(function () {
    PnPApp.GetInit();
});