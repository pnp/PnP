<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.CrossDomainImagesWeb.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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

            //make client-side call for tyhe third image
            $.ajax({
                url: '../Services/ImgService.svc/GetImage?accessToken=' + $('#hdnAccessToken').val() + '&site=' + encodeURIComponent(appWebUrl + '/') + '&folder=AppImages&file=O365.png',
                dataType: 'json',
                success: function (data) {
                    $('#Image3').attr('src', data.d);
                },
                error: function (err) {
                    alert('error occurred');
                }
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
    <style type="text/css">
        .img {
            width: 400px;
            height: 100px;
        }
    </style>
</head>
<body id="body" style="display: none;">
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>
        <asp:HiddenField ID="hdnAccessToken" runat="server" />
        <div id="divList" style="left: 50%; width: 500px; margin-left: -250px; position: absolute; display: table;">
            <h2 class="ms-webpart-titleText">Image with absolute URL source:</h2>
            <asp:Image ID="Image1" runat="server" CssClass="img" />
            <h2 class="ms-webpart-titleText">Image with base64 encoded source (server-side):</h2>
            <asp:Image ID="Image2" runat="server" CssClass="img" />
            <h2 class="ms-webpart-titleText">Image with base64 encoded source (client-side):</h2>
            <asp:Image ID="Image3" runat="server" CssClass="img" />
        </div>
    </form>
</body>
</html>
