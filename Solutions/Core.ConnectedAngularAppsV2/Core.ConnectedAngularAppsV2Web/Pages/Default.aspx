<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.ConnectedAngularAppsV2Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connected Master/Detail App Parts with SignalR and AngularJS V2</title>
        
    <script type="text/javascript" src="../Scripts/jquery-2.1.1.js"></script>
    <script type="text/javascript" src="../Scripts/modernizr-2.8.3.js"></script>
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>    
    <script type="text/javascript" src="../Scripts/app.js"></script>
    
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            Sample to demonstrate server side connectivity with app parts from provider hosted app using AngularJS and <a href="http://www.asp.net/signalr" target="_blank">SignalR</a>. <br />
            This is pretty easy implementation pattern for app parts to communicate between each other. This sample shows a migration dashboard (Master data) with site migration details (detail data) <br />
            and detail data web parts that can be placed on other sites, such as sites targeted for migration (for this sample in particular).
        <br />
            <ul style="list-style-type: square;">
                <li><b>Step 1:</b> Deploy this app to the Office365 tenant</li>
                <li><b>Step 2:</b> For first time deployment, click the Configure button below to configure the site and initialize sample data</li>
                <li><b>Step 3:</b> Click Back To Site to go to host web</li>
                <li><b>Step 4:</b> Verify lists and sample data exist</li>
                <li><b>Step 5:</b> Go to Site Pages and create a new page</li>
                <li><b>Step 6:</b> Add app parts to page(s)</li>
                <li><b>Step 7:</b> Select an event </li>
                <li><i>Note. You can open up second browser and put other web parts on completely different page and the connectivity would still work, since connection is done on server side.</i></li>
            </ul>
            <br />
            <br />
        <br />       
        Click the button below to configure and initialize the data elements. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnConfigure" Text="Configure" OnClick="btnConfigure_Click"/>
        <div id="divStatus" style="margin-top: 40px !important;">
                Deploy Status:
                <div class="ms-core-form-line" style="margin-left: 10px; margin-top: 15px !important;">
                    <asp:Listbox runat="server" id="status" class="form-control" Font-Size="Smaller" Width="400px" ForeColor="#0066FF" Height="100px"></asp:Listbox>
                </div>
        </div>
    </div>  
    </form>
</body>
</html>
