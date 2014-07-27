<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.JavaScriptInjectionWeb.Default" %>

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
        <h1>JavaScript Injection</h1>
        <ul style="list-style-type: square;">
            <li><b>Step 1:</b> "Inject" the JavaScript file to your current site using the button in the Demo section</li>
            <li><b>Step 2:</b> Check out the changes by clicking on "Back to Site" in the top navigation followed by:
                <ul style="list-style-type: square;">
                    <li>Check that every page shows a customized status message</li>
                    <li>Check that the create a sub site link has disappeared (via Site Actions --> Site Contents)</li>
                </ul>
            </li>
        </ul>
        <br />
        Click the buttons below to "inject" or remove JavaScript file to your current site. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Inject customization" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove customization" OnClick="btnRemove_Click" />  
        <br />
    </div>
    </form>
</body>
</html>
