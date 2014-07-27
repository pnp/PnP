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
            Sample to demonstrate server side connectivity with app parts from provider hosted app using <a href="http://www.asp.net/signalr" target="_blank">SignalR</a>. <br />
            This is pretty easy implementation pattern for app parts to communicate between each other without any changes to the SharePoint or more speficifcally to the host web.
        <br />
            <ul style="list-style-type: square;">
                <li><b>Step 1:</b> Deploy this app to the Office365 tenant</li>
                <li><b>Step 2:</b> Move to host web</li>
                <li><b>Step 3:</b> Add both app parts (Connected Part One & Two) to page. Technically these could be on two different pages as well cross the site.</li>
                <li><b>Step 4:</b> Add text to one of the text boxes in app parts and click Send </li>
                <li><i>Note. You can open up second browser and put the second web part to completely different page and the connectivity would still work, since connection is done on server side.</i></li>
            </ul>
            <br />
            <br />
        </div>
    </form>
</body>
</html>
