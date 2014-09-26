<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.RibbonCommandsWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
            <div id="divSPChrome"></div>
        </div>
        <div style="left: 40px; position: absolute;">
            <h1>Register the ribbon</h1>
            <p>Adds a Custom Tab and ribbon buttons to all items of content type <b>Item (0x01)</b>. <br />Best viewed by looking at the <asp:HyperLink runat="server" ID="DocumentsLink" Text="Documents" Target="_blank" Font-Bold="true" /> doclib.</p>
            <asp:Button runat="server" ID="InitializeButton" OnClick="InitializeButton_Click" Text="Add Ribbon" />

            <h1>Remove the ribbon</h1>
            <p>Removes the Custom Tab and ribbon buttons.</p>
            <asp:Button runat="server" ID="RemoveButton" OnClick="RemoveButton_Click" Text="Remove Ribbon" />
        </div>
    </form>
</body>
</html>
