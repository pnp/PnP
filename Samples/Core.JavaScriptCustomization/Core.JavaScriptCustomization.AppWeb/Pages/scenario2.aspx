<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scenario2.aspx.cs" Inherits="Contoso.Core.JavaScriptCustomization.AppWeb.Pages.scenario2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario2: JavaScript customizations for translating site contents</h1>
        This scenario further builds up on the elements explained in scenario 1 and addresses:
        <ul style="list-style-type: square;">
            <li>A JavaScript based resource file model</li>
            <li>More advanced jQuery techniques to "select" the correct HTML dom elements on the loaded page</li>
        </ul>
        <br />
        <h2>Demo steps</h2>
        <ul style="list-style-type: square;">
            <li><b>Step 1:</b> Create a quicklaunch entry named "My quicklaunch entry" to your current site
            <br />
                <asp:Image runat="server" ImageUrl="~/Images/scenario2_quicklauch.jpg"  Height="200px" />
            </li>
            <li><b>Step 2:</b> Add a page called "Hello SharePoint" to your current site
            <br />
                <asp:Image runat="server" ImageUrl="~/Images/scenario2_page.jpg" Height="200px"/>
            </li>
            <li><b>Step 3:</b> "Inject" the scenario 2 JavaScript file to your current site using the button in the Demo section</li>
            <li><b>Step 4:</b> Check out the changes by clicking on "Back to Site" in the top navigation followed by:
                <ul style="list-style-type: square;">
                    <li>Check that the label of the quicklaunch entry "My quicklaunch entry" has changed</li>
                    <li>Check that if you open the "Hello SharePoint" page you'll see a new page title</li>
                </ul>
            </li>
        </ul>
        <br />
        <h2>Demo</h2>
        Click the buttons below to "inject" or remove the scenario 2 JavaScript file to your current site. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Inject customization" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove customization" OnClick="btnRemove_Click" />  
        <br />
    </div>
    </form>
</body>
</html>
