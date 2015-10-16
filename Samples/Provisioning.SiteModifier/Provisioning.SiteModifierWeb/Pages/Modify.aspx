<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Modify.aspx.cs" Inherits="Provisioning.SiteModifierWeb.Modify" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Site Modifier</title>
</head>
<body>
    <!-- Script to load SharePoint resources
        and load the blank.html page in
        the invisible iframe
        -->
    <script type="text/javascript">
        "use strict";
        var appweburl;

        (function () {
            var ctag;

            // Get the URI decoded app web URL.
            appweburl =
                decodeURIComponent(
                    getQueryStringParameter("SPHostUrl")
            );
            // Get the ctag from the SPClientTag token.
            ctag =
                decodeURIComponent(
                    getQueryStringParameter("SPClientTag")
            );

            // The resource files are in a URL in the form:
            // web_url/_layouts/15/Resource.ashx
            var scriptbase = appweburl + "/_layouts/15/";

            // Dynamically create the invisible iframe.
            var blankiframe;
            var blankurl;
            var body;
            blankurl = appweburl + "/Pages/blank.html";
            blankiframe = document.createElement("iframe");
            blankiframe.setAttribute("src", blankurl);
            blankiframe.setAttribute("style", "display: none");
            body = document.getElementsByTagName("body");
            body[0].appendChild(blankiframe);

            // Dynamically create the link element.
            var dclink;
            var head;
            dclink = document.createElement("link");
            dclink.setAttribute("rel", "stylesheet");
            dclink.setAttribute("href", scriptbase + "defaultcss.ashx?ctag=" + ctag);
            head = document.getElementsByTagName("head");
            head[0].appendChild(dclink);
        })();

        // Function to retrieve a query string value.
        // For production purposes you may want to use
        //  a library to handle the query string.
        function getQueryStringParameter(paramToRetrieve) {
            var params;
            var strParams;

            params = document.URL.split("?")[1].split("&");
            strParams = "";
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split("=");
                if (singleParam[0] == paramToRetrieve)
                    return singleParam[1];
            }
        }
    </script>
    <form id="form1" runat="server">
        <%--<div runat="server" id="ConfigurationControls">--%>

        <%--</div>--%>
        <div>
            <fieldset id="ListsFieldSet" runat="server">
                <legend>Lists</legend>
                <asp:CheckBoxList ID="ListsCheckboxList" runat="server">
                 
                </asp:CheckBoxList>
            </fieldset>
            <fieldset>
                <legend>Various</legend>
                <asp:CheckBox ID="ApplyTheme" runat="server" Text="Apply theme" />
            </fieldset>
        </div>
        <div style="margin-top:20px;">
            <asp:Button ID="AddArtifacts" runat="server" Text="Confirm" OnClick="AddArtifacts_Click" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" />
        </div>
    </form>
</body>
</html>
