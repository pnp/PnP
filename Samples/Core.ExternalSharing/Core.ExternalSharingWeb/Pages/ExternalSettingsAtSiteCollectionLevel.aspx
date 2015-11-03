<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalSettingsAtSiteCollectionLevel.aspx.cs" Inherits="Core.ExternalSharingWeb.Pages.ExternalSettingsAtSiteCollectionLevel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../styles/app.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
    <title>Site Collection Sharing Settings</title>
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
                        Status:
                        <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
                    </p>
                </div>

                <div id="left">
                    <p>
                        Site collections in tenant:
                        <asp:ListBox ID="sitecollections" runat="server" Height="133px" Width="450px" AutoPostBack="true"
                            EnableViewState="true" OnSelectedIndexChanged="sitecollections_SelectedIndexChanged"></asp:ListBox>
                    </p>
                </div>

                <div id="footer">
                    <p>
                        External Sharing Status:
                        <asp:RadioButtonList ID="rblSharingOptions" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Selected="True" Value="Disabled">Disabled</asp:ListItem>
                            <asp:ListItem Value="ExternalUserSharingOnly">External User Sharing Only</asp:ListItem>
                            <asp:ListItem Value="ExternalUserAndGuestSharing">External User And Guest Sharing</asp:ListItem>
                        </asp:RadioButtonList>
                    </p>
                </div>
                <div id="actions">
                    <p>
                        <asp:LinkButton ID="btnUpdateSiteCollectionStatus" runat="server" Enabled="true" OnClick="btnUpdateSiteCollectionStatus_Click">Update sharing setting for site collection</asp:LinkButton>
                    </p>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
