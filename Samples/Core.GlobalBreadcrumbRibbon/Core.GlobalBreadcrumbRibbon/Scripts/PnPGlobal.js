CoreGlobalBreadcrumbRibbon = {
    GetInit: function () {
        PnPGlobal.GetUrlChange();
        PnPGlobal.CreateBreadcrumb();
        PnPGlobal.CreateStyle();
        PnPGlobal.LoadSiteBreadcrumb();
        window.addEventListener("DOMContentLoaded", RibbonValidation, false);
    },
    GetUrlChange: function () {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            var element = document.createElement('script');
            var UrlChange = "window.onhashchange = locationHashChanged;function locationHashChanged(){GetUrlDocMDS();}";
            element.innerHTML = UrlChange;
            document.getElementsByTagName('body')[0].appendChild(element);
        }, "sp.js");
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
    },
    LoadSiteBreadcrumb: function () {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            var clientcontext = SP.ClientContext.get_current();
            var site = clientcontext.get_site();
            var currentWeb = clientcontext.get_web();
            clientcontext.load(currentWeb, 'ServerRelativeUrl', 'Title', 'ParentWeb', 'Url');
            clientcontext.load(site, 'ServerRelativeUrl');
            clientcontext.executeQueryAsync(
            function () {
                var element = document.createElement('div');
                var breadcrumb = '<ol id="breadcrumbSite" class="breadcrumb">';
                breadcrumb = breadcrumb + '</ol>';
                element.innerHTML = breadcrumb;
                var Custombreadcrumb = document.getElementById("s4-bodyContainer");
                Custombreadcrumb.insertBefore(element, Custombreadcrumb.childNodes[0]);
                var li = document.createElement('li');
                li.innerHTML = '<a href="' + currentWeb.get_url() + '">' + currentWeb.get_title() + '</a>';
                var Custombreadcrumb = document.getElementById("breadcrumbSite");
                Custombreadcrumb.insertBefore(li, Custombreadcrumb.childNodes[0]);
                if (site.get_serverRelativeUrl() !== currentWeb.get_serverRelativeUrl()) {
                    PnPGlobal.RecursiveWeb(currentWeb.get_parentWeb().get_serverRelativeUrl())
                }
            }, fail);
        }, "sp.js");
    },
    RecursiveWeb: function (siteUrl) {
        var clientcontext = new SP.ClientContext(siteUrl);
        var site = clientcontext.get_site();
        var currentWeb = clientcontext.get_web();
        clientcontext.load(currentWeb, 'ServerRelativeUrl', 'Title', 'ParentWeb', 'Url');
        clientcontext.load(site, 'ServerRelativeUrl');
        clientcontext.executeQueryAsync(
    function () {
        if (site.get_serverRelativeUrl() !== currentWeb.get_serverRelativeUrl()) {
            var li = document.createElement('li');
            li.innerHTML = '<a href="' + currentWeb.get_url() + '">' + currentWeb.get_title() + '</a>';
            var Custombreadcrumb = document.getElementById("breadcrumbSite");
            Custombreadcrumb.insertBefore(li, Custombreadcrumb.childNodes[0]);
            PnPGlobal.RecursiveWeb(currentWeb.get_parentWeb().get_serverRelativeUrl())
        } else {
            var li = document.createElement('li');
            li.innerHTML = '<a href="' + currentWeb.get_url() + '">' + currentWeb.get_title() + '</a>';
            var Custombreadcrumb = document.getElementById("breadcrumbSite");
            Custombreadcrumb.insertBefore(li, Custombreadcrumb.childNodes[0]);
        }

    }, fail);
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
    GetUrlDoc();
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
function getQueryStringParameter(param, serverRelativeUrl) {
    if (document.URL.split("?").length > 1) {
        var params = document.URL.split("?")[1].split("&");
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split("=");
            if (singleParam[0] == param)
                return decodeURIComponent(singleParam[1]).replace(serverRelativeUrl, "");
        }
        if (_spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[0] === "Lists") {
            return _spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[0] + "/" + _spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[1];
        } else {
            return _spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[0];
        }
    } else {
        if (_spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[0] === "Lists") {
            return _spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[0] + "/" + _spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[1];
        } else {
            return _spPageContextInfo.serverRequestPath.replace(serverRelativeUrl, "").split("/")[0];
        }
    }
}
function getQueryStringParameterMDS(param) {
    if (document.URL.split("#").length > 1) {
        if (document.URL.split("?").length > 1) {
            var params = document.URL.split("?")[1].split("&");
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split("=");
                if (singleParam[0] == param) {
                    return decodeURIComponent(singleParam[1]);
                } else if (i < params.length && singleParam[0] !== param) {
                    if (decodeURIComponent(document.URL.split("#")[1].split("/")[1]) === "Lists") {
                        return decodeURIComponent(document.URL.split("#")[1].split("/")[1]) + "/" + decodeURIComponent(document.URL.split("#")[1].split("/")[2]);
                    } else {
                        return decodeURIComponent(document.URL.split("#")[1].split("/")[1]);
                    }
                }
            }
        } else {
            if (decodeURIComponent(document.URL.split("#")[1].split("/")[1]) === "Lists") {
                return decodeURIComponent(document.URL.split("#")[1].split("/")[1]) + "/" + decodeURIComponent(document.URL.split("#")[1].split("/")[2]);
            } else {
                return decodeURIComponent(document.URL.split("#")[1].split("/")[1]);
            }
        }
    } else {
        return "";
    }

}
function GetUrlDoc() {
    var elements = document.getElementsByClassName("ListBreadcumb");
    while (elements.length > 0) {
        elements[0].parentNode.removeChild(elements[0]);
    }
    clientcontext = SP.ClientContext.get_current()
    var currentWeb = clientcontext.get_web();
    clientcontext.load(currentWeb, 'ServerRelativeUrl');
    clientcontext.executeQueryAsync(function () {
        var fullurl = currentWeb.get_serverRelativeUrl() + ((currentWeb.get_serverRelativeUrl().indexOf('/', currentWeb.get_serverRelativeUrl().length - 1) !== -1) ? '' : '/');
        var path = getQueryStringParameter("RootFolder", fullurl);
        var CustomUrl;
        if (path.split("/").length > 1) {
            var params = path.split("/");
            for (var i = 0; i < params.length; i = i + 1) {
                if (params[i].trim() !== "") {
                    fullurl = fullurl + params[i] + '/';
                    if (i === 0 && params[i].trim() === "Lists") {
                    }
                    else {
                        CustomUrl = document.createElement('li');
                        CustomUrl.className = "ListBreadcumb";
                        CustomUrl.innerHTML = '<a href="' + fullurl + '">' + params[i] + '</a>';
                        document.getElementById("breadcrumbSite").appendChild(CustomUrl);
                    }
                }
            }
        } else {
            fullurl = fullurl + path + '/';
            CustomUrl = document.createElement('li');
            CustomUrl.className = "ListBreadcumb";
            CustomUrl.innerHTML = '<a href="' + fullurl + '">' + path + '</a>';
            document.getElementById("breadcrumbSite").appendChild(CustomUrl);
        }
    }, fail);
}
function GetUrlDocMDS() {
    var elements = document.getElementsByClassName("ListBreadcumb");
    while (elements.length > 0) {
        elements[0].parentNode.removeChild(elements[0]);
    }
    clientcontext = SP.ClientContext.get_current()
    var currentWeb = clientcontext.get_web();
    clientcontext.load(currentWeb, 'ServerRelativeUrl');
    clientcontext.executeQueryAsync(function () {
        var path = getQueryStringParameterMDS("RootFolder");
        path = path.replace(currentWeb.get_serverRelativeUrl(), '');
        var CustomUrl;
        var fullurl = currentWeb.get_serverRelativeUrl() + ((currentWeb.get_serverRelativeUrl().indexOf('/', currentWeb.get_serverRelativeUrl().length - 1) !== -1) ? '' : '/');
        if (path.split("/").length > 1) {
            var params = path.split("/");
            for (var i = 0; i < params.length; i = i + 1) {
                if (params[i].trim() !== "") {
                    fullurl = fullurl + params[i] + '/';
                    if (i === 0 && params[i].trim() === "Lists") {

                    }
                    else {
                        CustomUrl = document.createElement('li');
                        CustomUrl.className = "ListBreadcumb";
                        CustomUrl.innerHTML = '<a href="' + fullurl + '">' + params[i] + '</a>';
                        document.getElementById("breadcrumbSite").appendChild(CustomUrl);
                    }
                }
            }
        } else {
            fullurl = fullurl + path + '/';
            CustomUrl = document.createElement('li');
            CustomUrl.className = "ListBreadcumb";
            CustomUrl.innerHTML = '<a href="' + fullurl + '">' + path + '</a>';
            document.getElementById("breadcrumbSite").appendChild(CustomUrl);
        }
    }, fail);
}