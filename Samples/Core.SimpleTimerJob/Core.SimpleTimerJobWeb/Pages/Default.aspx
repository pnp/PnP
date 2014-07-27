<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.SimpleTimerJobWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Remote timer job pattern</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            Sample to demonstrate simple remote timer job pattern. App is only used to provide permissions to the client id and secret.<br />
            Actual business logic is located in external component which can be scheduled to be executed from Windows Azure or just as well from<br />
            on-premises. Actual scheduling of the execution can be based on whatever system you want. 
        <br />
            <ul style="list-style-type: square;">
                <li><b>Step 1:</b> Deploy this app to the Office365 tenant and approve the permissions</li>
                <li><b>Step 2:</b> Copy used client ID and secret from the provider hosted app web.config</li>
                <li><b>Step 3:</b> Update app config for the remote timer job console (note that this could be just as well Windows service or Azure role)</li>
                <li><b>Step 4:</b> Execute the "remote timer job" console to see that you can access the the site using the provided client ID and secret </li>
                <li><i>Note. Each time you deploy from VS, you will have different client ID, so ensure that you have the values correctly done to console app, which is in seperate solution to avoid start up project issues</i></li>
            </ul>
            <br />
            <br />
            Alternative for the app client and secret registration is to use the “_layouts/AppRegNew.aspx" page with specific details.
            <br />
            Check following blog posts for additional details realted on this capability 
                <br />
            <ul style="list-style-type: square;">
                <li><a href="http://blogs.msdn.com/b/shariq/archive/2013/12/09/simulate-timer-job-solution-for-sharepoint-2013-online-using-csom.aspx">Simulate Timer Job Solution for SharePoint 2013/Online using App Model & CSOM</a> - Shariq Siddiqui</li>
                <li><a href="http://blogs.msdn.com/b/kaevans/archive/2014/03/02/building-a-sharepoint-app-as-a-timer-job.aspx">Building a SharePoint App as a Timer Job</a> - Kirk Evans</li>
            </ul>
            <br />
        </div>
    </form>
</body>
</html>
