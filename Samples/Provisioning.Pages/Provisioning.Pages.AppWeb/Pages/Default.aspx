<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Provisioning.Pages.AppWeb.Default" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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
        <h1>Scenario 1: Basic wiki page manipulation</h1>
        In this scenario you'll learn how to create a wiki page and insert HTML content on that wiki page. Following topics will be addressed:
        <ul style="list-style-type: square;">
            <li>How to create a wiki page in a SharePoint Team site</li>
            <li>How to add HTML content to the created wiki page</li>
        </ul>
        <br />
        HTML text to add:
        <br />
        <asp:TextBox runat="server" ID="txtHtml" Rows="6" TextMode="MultiLine" Width="400px" Text="Hello <strong>Office 365 Dev PnP</strong>!"></asp:TextBox>
        <br />
        <br />
        <asp:Button runat="server" ID="btnScenario1" Text="Run scenario 1" OnClick="btnScenario1_Click" />
        Click <asp:HyperLink id="hplScenario1" Runat="server" Text="here" Target="_blank"></asp:HyperLink> to go to the created page.
        <br />
        <br />
        <h1>Scenario 2: Advanced wiki page manipulation</h1>
        In this scenario you'll learn some more advanced techniques. This sample builds further on scenario 1 and additionally following topics will be addressed:
        <ul style="list-style-type: square;">
            <li>How to created a promoted links list and some items to it (for demo purposes)</li>
            <li>How to create a wiki page using different layouts</li>
            <li>How to add web parts to a wiki page cell (promoted links web part on row 1 or 2, column 1)</li>
            <li>How to add html to a wiki page cell (requires a layout with at least 2 columns)</li>
            <li>How to remove a web part from a wiki page cell</li>
        </ul>
        <br />
        Choose a layout for your Wiki page:
        <br />
        <asp:DropDownList runat="server" ID="drpLayouts" Width="400px">
            <asp:ListItem Text="One column" Value="OneColumn" Selected="True"></asp:ListItem>
            <asp:ListItem Text="One column with sidebar" Value="OneColumnSideBar"></asp:ListItem>
            <asp:ListItem Text="Two columns" Value="TwoColumns"></asp:ListItem>
            <asp:ListItem Text="Two columns wih header" Value="TwoColumnsHeader"></asp:ListItem>
            <asp:ListItem Text="Two columns with header and footer" Value="TwoColumnsHeaderFooter"></asp:ListItem>
            <asp:ListItem Text="Three columns" Value="ThreeColumns"></asp:ListItem>
            <asp:ListItem Text="Thee columns with header" Value="ThreeColumnsHeader"></asp:ListItem>
            <asp:ListItem Text="Three columns with header and footer" Value="ThreeColumnsHeaderFooter"></asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <asp:Button runat="server" ID="btnScenario2" Text="Run scenario 2" OnClick="btnScenario2_Click" /> 
        <asp:Button runat="server" ID="btnScenario2Remove" Text="Remove webpart from the last page created during the scenario 2 run" OnClick="btnScenario2Remove_Click" Enabled="false" />
        Click <asp:HyperLink id="hplScenario2" Runat="server" Text="here" Target="_blank"></asp:HyperLink> to go to the created page.
        <br />
        <br />
        <h1>Scenario cleanup</h1>
        All the created scenario 1 and scenario 2 pages will be removed.
        <br />
        <br />
        <asp:Button runat="server" ID="btnCleanup" Text="Cleanup created pages" OnClick="btnCleanup_Click" />
    </div>
    </form>
</body>
</html>
