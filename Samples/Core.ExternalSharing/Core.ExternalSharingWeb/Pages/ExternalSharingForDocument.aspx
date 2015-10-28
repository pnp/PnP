<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalSharingForDocument.aspx.cs" Inherits="Core.ExternalSharingWeb.Pages.ExternalSharingForDocument" %>

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
                        &nbsp;<asp:Button ID="btnValidateEmail" runat="server" Text="Validate email" OnClick="btnValidateEmail_Click" />
                    </p>
                    <p>
                        Status:
            <asp:Label ID="lblStatus" runat="server" Text="Confirm email first"></asp:Label>
                    </p>
                </div>

                <div id="left">
                    <p>
                        Document libraries in host web
                <asp:ListBox ID="libraries" runat="server" Height="133px" Width="240px" AutoPostBack="True" OnSelectedIndexChanged="libraries_SelectedIndexChanged"></asp:ListBox>
                    </p>
                </div>

                <div id="right">
                    <p>
                        Documents in library
                <asp:ListBox ID="documents" runat="server" Height="133px" Width="240px" EnableViewState="true"></asp:ListBox>
                    </p>
                </div>

                <div id="footer">
                    <p>
                        Sharing option:
                <asp:RadioButtonList ID="rblSharingOptions" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True" Value="view">View</asp:ListItem>
                    <asp:ListItem Value="edit">Edit</asp:ListItem>
                </asp:RadioButtonList>
                        Expiration date for anonymous link:
                <asp:Calendar ID="expirationDate" runat="server"></asp:Calendar>
                    </p>
                </div>
                <div id="actions">
                    <p>
                        <asp:LinkButton ID="btnAnoLink" runat="server" Enabled="true" OnClick="btnAnoLink_Click">Get anonymous link</asp:LinkButton>&nbsp|&nbsp
                        <asp:LinkButton ID="btnAnoLinkExp" runat="server" Enabled="true" OnClick="btnAnoLinkExp_Click">Get anonymous link with deadline</asp:LinkButton>&nbsp|&nbsp
                        <asp:LinkButton ID="btnShareDocument" runat="server" Enabled="true" OnClick="btnShareDocument_Click">Share a document</asp:LinkButton>&nbsp|&nbsp
                        <asp:LinkButton ID="btnUnShareDoc" runat="server" Enabled="true" OnClick="btnUnShareDoc_Click">Unshare a document</asp:LinkButton>&nbsp|&nbsp
                        <asp:LinkButton ID="btnSharingStatus" runat="server" Enabled="true" OnClick="btnSharingStatus_Click">Get sharing status</asp:LinkButton>&nbsp|&nbsp
                    </p>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>