<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalSiteSharing.aspx.cs" Inherits="Core.ExternalSharingWeb.Pages.ExternalSiteSharing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../styles/app.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
    <title></title>
</head>
<body style="display: none">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <!-- Chrome control placeholder -->
        <div id="chrome_ctrl_placeholder"></div>
        <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
            <ProgressTemplate>
                <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                    <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                        <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                        <span class="ms-accentText" style="font-size: 36px;">&nbsp;Really working hard on it...</span>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <div id="header">
                    <p>
                        Email used as recipient for sharing:
                        <asp:TextBox ID="txtTargetEmail" runat="server" Width="204px"></asp:TextBox>
                        &nbsp;<asp:Button ID="btnValidateEmail" runat="server" Text="Validate email before sharing can be done" OnClick="btnValidateEmail_Click" />
                    </p>
                    <p>
                        Status:
                        <asp:Label ID="lblStatus" runat="server" Text="Confirm email first"></asp:Label>
                    </p>
                </div>
                <div id="footer">
                    <p>
                        Sharing option:
                        <asp:RadioButtonList ID="rblSharingOptions" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Selected="True" Value="view">View</asp:ListItem>
                            <asp:ListItem Value="edit">Edit</asp:ListItem>
                            <asp:ListItem Value="owner">Owner</asp:ListItem>
                        </asp:RadioButtonList>
                    </p>
                </div>
                <div id="actions">
                    <p>
                        <asp:LinkButton ID="btnShareSite" runat="server" Enabled="true" OnClick="btnShareSite_Click">Share site</asp:LinkButton>&nbsp|&nbsp
                        <asp:LinkButton ID="btnSharingStatus" runat="server" Enabled="true" OnClick="btnSharingStatus_Click">Get current sharing status</asp:LinkButton>&nbsp|&nbsp
                    </p>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
