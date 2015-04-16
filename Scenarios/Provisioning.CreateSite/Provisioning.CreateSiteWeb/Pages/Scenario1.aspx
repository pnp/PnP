<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scenario1.aspx.cs" Inherits="Provisioning.CreateSiteWeb.Pages.Scenario1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sub site and site collection creation</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" AsyncPostBackTimeout="1800" />
        <div id="divSPChrome"></div>
        <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
            <ProgressTemplate>
                <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                    <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                        <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                        <span class="ms-accentText" style="font-size: 36px;">&nbsp;Working as fast I can on it...</span>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <div style="left: 40px; position: absolute;">
                    <h1>Scenario 1: Create a sub site</h1>
                    In this scenario you'll learn how to create a sub site to host web usign CSOM. Here's the topics which are addressed:
            <ul style="list-style-type: square;">
                <li>How to create new a new sub site using CSOM.</li>
                <li>How to check sub sites from the host web.</li>
                <li>How to define used sub site template.</li>
                <li>How to include additional customizations to the created site.</li>
            </ul>
                    <br />
                    <h2>Demo</h2>
                    Choose a used template for the sub site (this lists all the templates available, team site is good to use)
        <br />
                    <asp:DropDownList runat="server" ID="drpContentTypes" Width="400px" />
                    <br />
                    Give a name for your site.
        <br />
                    <asp:TextBox runat="server" ID="txtName" Text="Sample" Width="400px" />
                    <asp:RequiredFieldValidator runat="server" ID="validName" ControlToValidate="txtName" EnableClientScript="true" Text="Name is required" />
                    <br />
                    Give a Url for your site.</i>
        <br />
                    <asp:TextBox runat="server" ID="txtUrl" Text="Sample" Width="400px" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtUrl" EnableClientScript="true" Text="URL is required" />
                    <br />
                    <br />
                    <asp:Button runat="server" ID="btnCheckUrl" Text="Check URL availability" OnClick="btnCheckUrl_Click" />
                    <asp:Button runat="server" ID="btnCreateSite" Text="Create new sub site" OnClick="btnCreateSite_Click" />
                    <br />
                    <br />
                    <asp:Label ID="lblStatus1" runat="server" />
                    <br />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
