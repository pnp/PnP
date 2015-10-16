<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.QueueWebJobUsageWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asynchronous operations with Azure storage queues and WebJobs</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
            <ProgressTemplate>
                <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                    <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                        <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                        <span class="ms-accentText" style="font-size: 36px;">&nbsp;Working on it as fast as I can...</span>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:MultiView ID="processViews" runat="server" ActiveViewIndex="0">
                    <asp:View ID="RequestView" runat="server">
                        <div style="left: 40px; position: absolute;">
                            <h1>Scenario: Perform long lasting scenario towards host web</h1>
                            In this scenario you'll learn how to set take advantage of Azure storage queues and Azure web job for long lasting operations towards host web. 
                            <ul style="list-style-type: square;">
                                <li>How to take advantage of Azure storage queues and WebJobs</li>
                                <li>How to connect Azure storage queues to WebJobs with automatic execution</li>
                                <li>How to perform operations towards SharePoint using asynchronous pattern</li>
                            </ul>
                            <br />
                            <br />
                            <i>In the example code we create new library to the host web and also make intentional 20 second sleep operation to mimic long lasting operation.</i>
                            <br />
                            <br />
                            <asp:Button runat="server" ID="btnSync" Text="Synchronious operation" OnClick="btnSync_Click" />
                            <asp:Button runat="server" ID="btnAsync" Text="Asynchronious operation" OnClick="btnAsync_Click" />
                            <br />
                            <br />
                        </div>
                    </asp:View>
                    <asp:View ID="RecordedView" runat="server">
                        <div style="width: 450px; margin-left: 50px;">
                            <div id="divFieldTemplate" style="display: table; width: 100%;">
                                <h3 class="ms-core-form-line">Status of the applied operation:
                                </h3>
                                <div class="ms-core-form-line">
                                    <asp:Label ID="lblStatus" runat="server" />
                                </div>
                                <div id="divButtons" style="float: right;">
                                    <asp:Button ID="btnProceed" runat="server" Text="Proceed to host web" CssClass="ms-ButtonHeightWidth" OnClick="btnProceed_Click" />
                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
