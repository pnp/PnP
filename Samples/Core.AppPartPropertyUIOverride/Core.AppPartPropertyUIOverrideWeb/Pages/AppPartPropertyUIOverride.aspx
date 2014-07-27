<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppPartPropertyUIOverride.aspx.cs" Inherits="Contoso.Core.AppPartPropertyUIOverrideWeb.AppPartPropertyUIOverride" %>

<!DOCTYPE html>

<html>
<head>
    <title></title>
    <script type="text/javascript">
        // Set the style of the client web part page to be consistent with the host web.
        (function () {
            'use strict';

            var hostUrl = '';
            if (document.URL.indexOf('?') != -1) {
                var params = document.URL.split('?')[1].split('&');
                for (var i = 0; i < params.length; i++) {
                    var p = decodeURIComponent(params[i]);
                    if (/^SPHostUrl=/i.test(p)) {
                        hostUrl = p.split('=')[1];
                        document.write('<link rel="stylesheet" href="' + hostUrl + '/_layouts/15/defaultcss.ashx" />');
                        break;
                    }
                }
            }
            if (hostUrl == '') {
                document.write('<link rel="stylesheet" href="/_layouts/15/1033/styles/themable/corev15.css" />');
            }
        })();
    </script>
</head>
<body>
    Now you've added this App Part to the page, from the context menu, select "Edit Web Part" 
    to see the overridden property user interface in action.<br />
    <br />
    Current query string values (this his how App Part properties are passed):<br />
    <br />
    BooleanProperty1 = <span style="font-weight: bold"><%= this.Request.QueryString["BooleanProperty1"] %></span><br />
    EnumProperty1 = <span style="font-weight: bold"><%= this.Request.QueryString["EnumProperty1"] %></span><br />
    IntegerProperty1 = <span style="font-weight: bold"><%= this.Request.QueryString["IntegerProperty1"] %></span><br />
    StringProperty1 = <span style="font-weight: bold"><%= this.Request.QueryString["StringProperty1"] %></span><br />
    HostWebListTitleHiddenTextBox = <span style="font-weight: bold"><%= this.Request.QueryString["HostWebListTitleHiddenTextBox"] %></span><br />
    BooleanProperty2 = <span style="font-weight: bold"><%= this.Request.QueryString["BooleanProperty2"] %></span><br />
</body>
</html>
