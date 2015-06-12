CoreGlobalBreadcrumbRibbon = {
    GetInit: function () {
        PnPGlobal.CreateBreadcrumb()
        PnPGlobal.CreateStyle();

        window.addEventListener("DOMContentLoaded", RibbonValidation, false);
    },
    CreateStyle: function () {
        var headID = document.getElementsByTagName("head")[0];
        var cssNode = document.createElement('style');
        cssNode.innerHTML = "#CustomRibbon div{padding-top: 5px; padding-left: 10px; float: left;}.breadcrumb li {display: inline;}.breadcrumb li+li:before {content:\">> \";}";
        headID.appendChild(cssNode);
    },
    CreateBreadcrumb: function () {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            var element = document.createElement('div');
            var context = new SP.ClientContext.get_current();
            var site = context.get_site();
            var web = site.get_rootWeb().get_allProperties();
            context.load(web)
            context.executeQueryAsync(function () {
                var results = JSON.parse(web.get_item('vti_GlobalBreadcrumbRibbon'));
                var breadcrumb = '<ol class="breadcrumb">';

                for (var i = 0; i < results.Breadcrumb.length; i++) {
                    breadcrumb = breadcrumb + '<li><a href="#">' + results.Breadcrumb[i].title + '</a></li>';
                }
                breadcrumb = breadcrumb + '</ol>';
                element.innerHTML = breadcrumb;
                var Custombreadcrumb = document.getElementById("s4-bodyContainer");
                Custombreadcrumb.insertBefore(element, Custombreadcrumb.childNodes[0]);

            }, function () { });

        }, "sp.js");
    }

}
window.PnPGlobal = window.CoreGlobalBreadcrumbRibbon;

PnPGlobal.GetInit();
(function (open) {

    XMLHttpRequest.prototype.open = function (method, url, async, user, pass) {

        this.addEventListener("readystatechange", function () {
            window.addEventListener("DOMContentLoaded", RibbonValidation, false);
        }, false);

        open.call(this, method, url, async, user, pass);
    };

})(XMLHttpRequest.prototype.open);
function RibbonValidation() {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        try {
            var pm = SP.Ribbon.PageManager.get_instance();
            pm.add_ribbonInited(function () {
                CreateRibbon();
            });
            var ribbon = null;
            try {
                ribbon = pm.get_ribbon();
            }
            catch (e) { }
            if (!ribbon) {
                if (typeof (_ribbonStartInit) == "function")
                    _ribbonStartInit(_ribbon.initialTabId, false, null);
            }
            else {
                CreateRibbon();
            }
        } catch (e)
        { }
    }, "sp.ribbon.js");
}
function CreateRibbon() {

    var Ribbonhtml = document.createElement('div');
    Ribbonhtml.setAttribute("id", "CustomRibbon");
    Ribbonhtml.innerHTML = "<div><a href='#' onclick=\"alert('Custom Ribbon')\" ><img src='../_layouts/images/NoteBoard_32x32.png' /></a><br/>Ribbon Example</div><div><a href='#' onclick=\"LoadApps()\" ><img src='../_layouts/images/NoteBoard_32x32.png' /></a><br/>SP Add-in\'s</div>";
    var ribbon = SP.Ribbon.PageManager.get_instance().get_ribbon();
    if (ribbon) {
        var tab = new CUI.Tab(ribbon, 'GlobalRibbon.Tab', 'Option', 'Option', 'GlobalRibbon.Tab.Command', false, '', null);
        ribbon.addChildAtIndex(tab, 1);
        var group = new CUI.Group(ribbon, 'GlobalRibbon.Tab.Group', 'Custom Ribbon', 'Global Ribbon Example', 'GlobalRibbon.Group.Command', null);
        tab.addChild(group);
    }
    SelectRibbonTab('GlobalRibbon.Tab', false);
    document.getElementById("GlobalRibbon.Tab.Group").childNodes[0].childNodes[0].appendChild(Ribbonhtml);
    SelectRibbonTab('Ribbon.Read', true);
    window.removeEventListener("DOMContentLoaded", RibbonValidation, false);
}
function LoadApps() {
    clientcontext = SP.ClientContext.get_current()
    currentWeb = clientcontext.get_web();
    appinstancesList = SP.AppCatalog.getAppInstances(clientcontext, currentWeb);
    clientcontext.load(appinstancesList);
    clientcontext.executeQueryAsync(Success, fail);
}

function Success() {
    var stringHtml = '';
    var list = appinstancesList.getEnumerator();
    while (list.moveNext()) {
        var current = list.get_current();

        stringHtml += '<div>' + current.get_title() + '</br><a href=\'' + _spPageContextInfo.webServerRelativeUrl + ((_spPageContextInfo.webServerRelativeUrl.indexOf('/', _spPageContextInfo.webServerRelativeUrl.length - 1) !== -1) ? '' : '/') + '_layouts/15/appredirect.aspx?instance_id={' + current.get_id() + '}\'>Link</a></div>';
    }
    DialogApps(stringHtml);
}
function fail(sender, args) {
    alert(args.get_message());
}

function DialogApps(stringHtml) {
    var element = document.createElement('div');
    element.innerHTML = stringHtml;
    SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', {
        html: element,
        title: "SharePoint Add-in",
        allowMaximize: false,
        showClose: true,
        autoSize: true
    });

}