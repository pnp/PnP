<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.UploadWebPartWeb.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload Webpart Sample</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/MicrosoftAjax.js"></script>
    <script type="text/javascript">
        var context;
        //Wait for the page to load
        $(document).ready(function () {
            //get hostWebUrl and AppWebUrl and build absolute path to the layouts root with the spHostUrl
            var hostWebUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
            var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
            var layoutsRoot = hostWebUrl + '/_layouts/15/';

            //load all appropriate scripts for the page to function
            $.getScript(layoutsRoot + 'SP.Runtime.js', function () {
                $.getScript(layoutsRoot + 'SP.js', function () {
                    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);
                    });
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', function () {
                        //Get the host site logo url from the SPHostLogoUrl parameter
                        var hostlogourl = decodeURIComponent(getQueryStringParameter('SPHostLogoUrl'));

                        //Set the chrome options for launching Help, Account, and Contact pages
                        var options = {
                            'appIconUrl': hostlogourl,
                            'appTitle': document.title,
                            'settingsLinks': [],
                            'onCssLoaded': '$("#body").show()'
                        };

                        //Load the Chrome Control in the divSPChrome element of the page
                        var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                        chromeNavigation.setVisible(true);
                    })
                });
            });
        });

        //function to get a parameter value by a specific key
        function getQueryStringParameter(urlParameterKey) {
            var params = document.URL.split('?')[1].split('&');
            var strParams = '';
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split('=');
                if (singleParam[0] == urlParameterKey)
                    return singleParam[1];
            }
        }
    </script>
</head>
<body id="body" style="display: none;">
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>
        <div id="divList" style="left: 50%; width: 500px; margin-left: -250px; position: absolute; display: table;">
            <h2 class="ms-accentText">App Status</h2>
            <table>
                <tr>
                    <td><h2 class="ms-webpart-titleText">WebPart exists in WebPart Gallery:</h2></td>
                    <td><asp:Image ID="imgWPG" runat="server" ImageUrl="~/Images/No.png" /></td>
                    <td><asp:Button runat="server" ID="btnAddToGallery" Text="Add" OnClick="btnAddToGallery_Click" /></td>
                </tr>
                <tr>
                    <td><h2 class="ms-webpart-titleText">WebPart added to home page:</h2></td>
                    <td><asp:Image ID="imgWPP" runat="server" ImageUrl="~/Images/No.png" /></td>
                    <td><asp:Button runat="server" ID="btnAddToPage" Text="Add" OnClick="btnAddToPage_Click" /></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
