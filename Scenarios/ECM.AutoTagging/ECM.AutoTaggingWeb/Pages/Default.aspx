<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ECM.AutoTaggingWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/common.js"></script>

</head>
<body style="display: none;  overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div>
          <asp:Button runat="server" ID="btnScenario" Text="Run scenario 1" OnClick="btnScenario1_Click" />
          <asp:Button runat="server" ID="btnRemove" Text="Run scenario 2" OnClick="btnScenario2_Click" />
    </div>
    </form>
</body>
</html>
