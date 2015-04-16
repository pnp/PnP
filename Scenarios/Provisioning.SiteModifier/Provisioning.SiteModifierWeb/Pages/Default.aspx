<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.SiteModifierWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Site Modifier</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none;  overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Adding app popup to host web</h1>
        In this scenario you'll learn how to add app popup to the host web. Following topics will be addressed:
        <ul style="list-style-type: square;">
            <li>How to "inject" new link to Site Actions menu in host web</li>
            <li>How to open up a app popup to show content from provider hosted app</li>
            <li>How to use CSOM to modify host web based on the actions requested in app popup</li>
            <li>How to close app popup using JavaScript after completing operations</li>
        </ul>
        <br />
        <br />
        Click the buttons below to apply changes to the host web or remove them as needed. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Apply changes" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove changes" OnClick="btnRemove_Click" />  
        <br /><br />
         <asp:Label ID="lblStatus" runat="server" />
        <br />
    </div>
    </form>
</body>
</html>
