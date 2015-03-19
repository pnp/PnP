<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.EmbedJavaScriptWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none;  overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Embed JavaScript</h1>
        <ul style="list-style-type: square;">
            <li><b>Step 1:</b> Embed or remove a JavaScript file on your current site using the buttons.</li>
            <li><b>Step 2:</b> Check out the changes by selecting <b>Back to Site</b> followed by:
                <ul style="list-style-type: square;">
                    <li>Verifying that every page shows a customized status message</li>
                    <li>Verifying the new subsite link has disappeared (select Site Actions > Site Contents)</li>
                </ul>
            </li>
        </ul>
        <br />        
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Embed customization" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove customization" OnClick="btnRemove_Click" />  
        <br />
    </div>
    </form>
</body>
</html>
