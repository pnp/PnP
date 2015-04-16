<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.ConnectedAppPartsWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connected App Parts with SignalR</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            This code sample demonstrates server side connectivity using app parts and a provider hosted app using <a href="http://www.asp.net/signalr" target="_blank">SignalR</a>. <br />
            Using this pattern with your app parts allows them to communicate with each other without any changes to SharePoint, or more specifically the host web.
        <br />
            <ul style="list-style-type: square;">
                <li><b>Step 1:</b> Deploy this app to the Office365 tenant.</li>
                <li><b>Step 2:</b> Click <b>Back to Site</b>.</li>
                <li><b>Step 3:</b> Add both app parts (Connected Part One & Two) to a page. Technically these could be on two different pages or in two different sites.</li>
                <li><b>Step 4:</b> Add text to one of the text boxes in app parts and click Send. </li>
                <li><i>Note: You can open up a second browser and include a second web part on a different page and the app part will receive messages from the chat hub because there is a connection to the same server.</i></li>
            </ul>
            <br />
            <br />
        </div>
    </form>
</body>
</html>
