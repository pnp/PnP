<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OD4B.NavLinksInjectionWeb.Default" %>

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
        <h1>Secondary OneDrive Navigation (JavaScript Injection)</h1>
        <ul style="list-style-type: square;">
            <li><b>Step 1:</b> "Inject" the JavaScript to create a secondary navigation links bar in your OneDrive site using the button in the Demo section</li>
            <li><b>Step 2:</b> Check out the changes by clicking on "Back to Site" in the top navigation followed by clicking on your suite bar OneDrive link:
                <ul style="list-style-type: square;">
                    <li>Check that the secondary navigation bar loads and displays links</li> 
                    <li>NOTE: This type of solution should only be used as a temporary solution if necessary to provide links to another location such as after a migration</li>                    
                </ul>
            </li>
        </ul>
        <br />
        Click the buttons below to "inject" or remove the JavaScript navigation links. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Inject OneDrive customization" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove OneDrive customization" OnClick="btnRemove_Click" />  
        <br />
    </div>
    </form>
</body>
</html>
