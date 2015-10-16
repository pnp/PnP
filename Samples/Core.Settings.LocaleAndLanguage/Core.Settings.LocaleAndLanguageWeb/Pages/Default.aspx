<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.Settings.LocaleAndLanguageWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Locale and language settings</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            <h1>Scenario 1: Control locale settings</h1>
            In this scenario you'll learn how to control locale settings of the host web usign CSOM.
            <ul style="list-style-type: square;">
                <li>How to set locale to the host web using CSOM</li>
            </ul>
            Select Locale
            <br />

            <asp:DropDownList ID="ddlLocales" runat="server">
            </asp:DropDownList>

            <br />
            <br />
            <asp:Button runat="server" ID="btnScenario1" Text="Run scenario" OnClick="btnScenario_Click" />
            <asp:Label ID="lblStatus" runat="server" />
            <br />
            <br />
            <h1>Scenario 2: Set language settings</h1>
            In this scenario you'll learn how to control language settings n the site and how it impacts end user experience in site.
            <br />
            <i>Notice that to be able to see site language changed, you will need to adjust your personal language preferencies from your personal user profile</i>
            <ul style="list-style-type: square;">
                <li>Access list of existing supported languages in host web</li>
                <li>Add new language as supported language in site</li>
                <li>Remove additional language from the site</li>
            </ul>
            <br />
            Currently supported languages: <i>
                <asp:Label runat="server" ID="lblCurrentlySupportedLanguages"></asp:Label></i>
            <br />
            <br />
            <asp:Button runat="server" ID="btnScenario2_Add" Text="Add Finnish Language (1035)" OnClick="btnScenario2Add_Click" />
            <asp:Button runat="server" ID="btnScenario2_Remove" Text="Remove Finnish Language (1035)" OnClick="btnScenario2Remove_Click" />
            <br />
            <asp:Label ID="lblStatus2" runat="server" />
            <br />
        </div>
        <asp:HiddenField ID="SPAppToken" ClientIDMode="Static" runat="server" />
    </form>
</body>
</html>
