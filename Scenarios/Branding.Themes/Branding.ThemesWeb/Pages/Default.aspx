<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Branding.ThemesWeb.Default" %>

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
        <h1>Scenario 1: Set theme to host site</h1>
        In this scenario you'll learn how to manipulate the host web theme using CSOM
        <ul style="list-style-type: square;">
            <li>How to apply an existing theme to your host web</li>
        </ul>
        <br />
        Choose a theme to your host web.
        <br />
        <asp:DropDownList runat="server" ID="drpThemes" Width="400px">
            <asp:ListItem Text="Orange" Value="Orange" Selected="True"></asp:ListItem>
            <asp:ListItem Text="Green" Value="Green"></asp:ListItem>
            <asp:ListItem Text="Nature" Value="Nature"></asp:ListItem>
            <asp:ListItem Text="Blossom" Value="Blossom"></asp:ListItem>
            <asp:ListItem Text="Office" Value="Office"></asp:ListItem>
            <asp:ListItem Text="Breeze" Value="Breeze"></asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <asp:Button runat="server" ID="btnScenario1" Text="Run scenario 1" OnClick="btnScenario1_Click" />  <asp:Label ID="lblStatus1" runat="server" />
        <br />
        <br />
        <h1>Scenario 2: Upload new theme and set it to be available in host site</h1>
        In this scenario you'll learn how to deploy needed theme assets to host site and how to set custom theme to be applied to the site.
        <ul style="list-style-type: square;">
            <li>How to deploy theme elements to host site</li>
            <li>How to assign custom theme to the site</li>
        </ul>
        <br />
        <br />       
        <asp:Button runat="server" ID="btnScenario2" Text="Run scenario 2" OnClick="btnScenario2_Click" />  <asp:Label ID="lblStatus2" runat="server" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
