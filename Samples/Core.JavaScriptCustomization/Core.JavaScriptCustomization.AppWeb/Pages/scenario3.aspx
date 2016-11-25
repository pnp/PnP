<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scenario3.aspx.cs" Inherits="Contoso.Core.JavaScriptCustomization.AppWeb.Pages.scenario3" %>

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
        <h1>Scenario3: Advanced JavaScript customizations</h1>
        Finally this scenario shows some more advanced customizations that allow to:
        <ul style="list-style-type: square;">
            <li>Apply JavaScript customizations on asynchronously loaded content (e.g. XSLT based listview rendering)</li>
            <li>Apply JavaScript customizations on content that is asynchronously loaded after a user manipulation</li>
        </ul>
        <h2>Demo steps</h2>
        <ul style="list-style-type: square;">
            <li><b>Step 1:</b> Add a new custom list "Demo" to your SharePoint site
            <br />
                <asp:Image runat="server" ImageUrl="~/Images/scenario3_customlist.jpg"  Height="200px" />
            </li>
            <li><b>Step 2: Add two columns named "column1" and "column2" to the "Demo" custom list</b> abc
            <br />
                <table>
                    <tr>
                        <td>
                            <asp:Image runat="server" ImageUrl="~/Images/scenario3_column1.jpg" Height="150px"/>
                        </td>
                        <td>
                            <asp:Image runat="server" ImageUrl="~/Images/scenario3_columns.jpg" Height="150px"/>
                        </td>
                    </tr>
                </table>                
            </li>
            <li><b>Step 3:</b> Try to logon to your site with a user that does not have access which allows you to insert a request in the "Access Requests and Invitations" queue</li>
            <li><b>Step 4:</b> "Inject" the scenario 3 JavaScript file to your current site using the button in the Demo section</li>
            <li><b>Step 5:</b> Check out the changes by clicking on "Back to Site" in the top navigation followed by:
                <ul style="list-style-type: square;">
                    <li>Going to your "Demo" list and notice that the column headers have changed</li>
                    <li>Looking at the site access requests (Site settings --> Access Requests and Invitations). When you open one you should see a limited list of roles that can be assigned compared to the default extensive list
                        <br />
                        <asp:Image runat="server" ImageUrl="~/Images/scenario3_accessrequests.jpg"  Height="300px" />
                    </li>
                </ul>
            </li>
        </ul>
        <br />
        <h2>Demo</h2>
        Click the buttons below to "inject" or remove the scenario 3 JavaScript file to your current site. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Inject customization" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove customization" OnClick="btnRemove_Click" />  
        <br />
    </div>
    </form>
</body>
</html>
