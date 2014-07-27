<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Search.PersonalizedResults.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search API and personalization</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none;  overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Perform Search API search</h1>
        Provide search filter for tenant wide site search query: <asp:TextBox runat="server" ID="searchtext" Text="Test" />
        <br />  
        <br />
        <asp:Button runat="server" ID="btnPerformSearch" Text="Perform Simple Search" OnClick="btnPerformSearch_Click" /> 
        <br />
        <asp:Label ID="lblStatus1" runat="server" />
        <br />
        <br />
        <h1>Perform personalized search based listing of sites based on profile data</h1>
        <br />       
        <br />
        <lu>
            <li>If 'About me' does NOT contains text 'AppTest' we search only sites which are team sites (WebTemplate = STS). If 'AppTest' is present, we search all sites.</li>
        </lu>
        <br />
        <i><b>Scenario</b>: show sites or aggregrated data from specific locations based on the user profile. <br />
            Example would be to aggregate news pages whcih are only tagged with identifier mathcing current user location or city.</i>
        <br />
        <br />
        <asp:Button runat="server" ID="btnPersonalizedSearch" Text="Perform Personalized Search" OnClick="btnPersonalizedSearch_Click" /> 
        <br />
         <asp:Label ID="lblStatus2" runat="server" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>