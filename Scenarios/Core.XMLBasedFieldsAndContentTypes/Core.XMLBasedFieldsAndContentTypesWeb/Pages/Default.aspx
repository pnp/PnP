<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.XMLBasedFieldsAndContentTypesWeb.Default" %>

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
        <h1>Scenario 1: Create site columns to site collection</h1>
        This scenario will show how to create site collections scoped site column using feature framework xml definition with core component.<br />
        Deployment will be one type set to the host web of the provider hosted app. Fields will use 'Contoso Columns' group.
        <br />
        <br />       
        <asp:Button runat="server" ID="btnScenario1" Text="Run scenario 1" OnClick="btnScenario1_Click" /> <asp:Label ID="lblStatus" runat="server" />
        <br />
        <br />
        <h1>Scenario 2: Create content type to site collection</h1>
        This scenario will show how to create content types using feature framework xml definition with core component.<br />
        Deployment will be one type set to the host web of the provider hosted app. Content types will be in 'Contoso' group.
         <br />
        <br />       
        <asp:Button runat="server" ID="btnScenario2" Text="Run scenario 2" OnClick="btnScenario2_Click" /> <asp:Label ID="lblStatus2" runat="server" />
        <br />
        <br />
        <h1>Scenario 3: Create new library with custom views</h1>
        In this scenario you'll learn how to use core component to create library, associate content type in and to create custom views using xml based definition.<br />
        <i>Notice that this scenario assumes that steps 1 and 2 have been completed.</i>
        <br /><br />
        Give document library name to create with custom views: <asp:TextBox runat="server" ID="txtDocLib" Text="Sample"></asp:TextBox>
        <br /><br />     
        <asp:Button runat="server" ID="btnScenario3" Text="Run scenario 3" OnClick="btnScenario3_Click"/> <asp:Label ID="lblStatus3" runat="server" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
