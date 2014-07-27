<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Branding.DisplayTemplatesWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/MicrosoftAjax.js"></script>    
    <script type="text/javascript" src="../Scripts/app.js"></script>
    <script type="text/javascript">
        //function callback to render chrome after SP.UI.Controls.js loads
        function renderSPChrome() {
            //Set the chrome options
            var options = {
                'appTitle': "Branding - Display Templates",
                'onCssLoaded': 'chromeLoaded()'
            };

            //Load the Chrome Control in the chrome_ctrl_placeholder element of the page
            var chromeNavigation = new SP.UI.Controls.Navigation('chrome_ctrl_placeholder', options);
            chromeNavigation.setVisible(true);
        }

        function chromeLoaded() {
            $('body').show();
        }
    </script>
</head>
<body style="display:none">
    <form id="form1" runat="server">
        <div id="chrome_ctrl_placeholder"></div>        
        <div style="padding-left: 20px; padding-right: 20px;">
            <h2>Instructions</h2>
            <br />
            <h3>Deploy all the artifacts</h3>
            <p>
                Click the Deploy button to create folders, upload Master Pages, CSS, image and Display Template JavaScript files, create pages, create Site Columns, create a Content Type, create the Home Hero list and initialize the list with data.<br />
            </p>
            <asp:Button runat="server" ID="btnIniSiteContent" Text="Deploy" OnClick="btnIniSiteContent_Click" />
            <br />
            <br />
            <h3>Delete all the artifacts (Optional)</h3>
            <p>
                To run the deployment process again, click the Delete Artifacts button below to remove all the artifacts created by the app. Then, click the Deploy button to create all the artifacts again.
            </p>
            <asp:Button runat="server" ID="btnDelete" Text="Delete Artifacts" OnClick="btnDelete_Click" />
            <br />
            <br />
            <asp:Label ID="lblInfo" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
