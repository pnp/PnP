<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.FileUploadWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>File Upload Scenarios</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
            <ProgressTemplate>
                <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                    <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                        <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                        <span class="ms-accentText" style="font-size: 36px;">&nbsp;Uploading the file... this might take a while...</span>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <div style="left: 40px; position: absolute;">
                    <h1>Scenario 1: Upload file to library</h1>
                    In this scenario you'll learn how to upload file to library located in the host web regardless of the file size.
                    <ul style="list-style-type: square;">
                        <li>How to deploy files to host web</li>
                        <li>Which APIs to use to avoid issues with the file size</li>
                    </ul>
                    <br />
                    <asp:Button runat="server" ID="btnScenario1" Text="Run scenario 1" OnClick="btnScenario1_Click" />
                    <asp:Label ID="lblStatus1" runat="server" />
                    <br />
                    <br />
                    <h1>Scenario 2: Upload file to folder</h1>
                    In this scenario you'll learn how to upload file to folder located in the host web.
                    <ul style="list-style-type: square;">
                        <li>How to deploy files to host web</li>
                        <li>Which APIs to use to avoid issues with the file size</li>
                    </ul>
                    <br />
                    <asp:Button runat="server" ID="btnScenario2" Text="Run scenario 2" OnClick="btnScenario2_Click" />
                    <asp:Label ID="lblStatus2" runat="server" />
                    <br />
                    <br />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
