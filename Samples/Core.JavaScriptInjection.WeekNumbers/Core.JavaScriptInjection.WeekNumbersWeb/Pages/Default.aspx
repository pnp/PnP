<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.JavaScriptInjection.WeekNumbersWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Calendar Week Numbers</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script> 
    <script type="text/javascript" src="../Scripts/app.js"></script> 
</head>
<body>
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>
        <div style="padding-left: 40px;">
            <h1>Add Scripts to Host Web</h1>
            <p>Press the button below to upload JQuery and week numbers script files to <a href="javascript:navigateToHostWeb();">host web</a>'s site assets library. When doing this the code also adds custom actions so that the scripts get referenced when page is loaded. Navigate to a calendars montly view and you should see the week numbers.</p>
            <asp:Button Text="Add Scripts" OnClick="AddScripts_Click" runat="server" />
            <h1>Remove Scrips from Host Web</h1>
            <p>Press the button below to remove the custom actions from <a href="javascript:navigateToHostWeb();">host web</a>.</p>
            <asp:Button Text="Remove Scripts" OnClick="RemoveScripts_Click" runat="server" />
        </div>
    </form>
</body>
</html>
