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
        <h1>Secondary Navigation (JavaScript Injection)</h1>
        <ul style="list-style-type: square;">
            <li><b>Step 1:</b> "Inject" the JavaScript to create a secondary navigation links bar in your OneDrive site or any SharePoint site using the button in the Demo section</li>
            <li><b>Step 2:</b> Check out the changes by clicking on "Back to Site" in the top navigation followed by clicking on your suite bar OneDrive link:
                <ul style="list-style-type: square;">
                    <li>Check that the secondary navigation bar loads and displays links</li>                   
                </ul>
            </li>
        </ul>
        <br />
        <p>
            <i>NOTE: This solution is taking dependency on the page dom structure, which could be changed between versions and 
                would then require adjustment of the JavaScript file to get it to work again. <br />
                This is however recommend solution over custom master page, since using this kind of approach sites are getting
                all the updates applied to oob master pages without any maintenance actions. 
            </i>
        </p>
        <br />
        Click the buttons below to "inject" or remove the JavaScript navigation links. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Inject second level navigation" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove second level navigation" OnClick="btnRemove_Click" />  
         <asp:Label ID="lblStatus" runat="server" />
        <br />
    </div>
    </form>
</body>
</html>
