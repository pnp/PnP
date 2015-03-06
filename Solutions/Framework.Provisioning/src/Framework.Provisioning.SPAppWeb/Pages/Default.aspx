<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Framework.Provisioning.SPAppWeb.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Site Collection Creation</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" AsyncPostBackTimeout="2000" />
        <div id="divSPChrome"></div>
        <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
            <ProgressTemplate>
                <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                    <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                        <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                        <span class="ms-accentText" style="font-size: 36px;">&nbsp;Really working on it...</span>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true" >
            <ContentTemplate>
                <div style="width: 450px; margin-left:50px; ">
                    <div id="divFieldTemplate" style="display: table; width: 100%;">
                        <h3 class="ms-core-form-line">Pick a template</h3>
                        <div class="ms-core-form-line">
                            <asp:ListBox ID="listSites" runat="server" CssClass="ms-fullWidth"></asp:ListBox>
                        </div>
                    </div>
                    <div id="divFieldTitle" style="display: table;">
                        <h3 class="ms-core-form-line">Give it a name</h3>
                        <div class="ms-core-form-line">
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="ms-fullWidth"></asp:TextBox>
                        </div>
                        <h3 class="ms-core-form-line">URL name</h3>
                        <div style="float: left; white-space: nowrap; padding-bottom: 10px; width: 450px;">
                            <div style="width: 320px; font-size: 13px; float: left; padding-top: 2px;" id="divBasePath">
                                <asp:Label ID="lblBasePath" runat="server"></asp:Label>
                            </div>
                            <div style="width: 130px; float: left;">
                                <asp:TextBox ID="txtUrl" runat="server" CssClass="ms-fullWidth"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div id="divFieldDescription">
                        <h3 class="ms-core-form-line">Give it a description</h3>
                        <div class="ms-core-form-line">
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="ms-fullWidth" TextMode="MultiLine" Rows="2"></asp:TextBox>
                        </div>
                    </div>
                    <div>
                        <asp:Label ID="lblErrorMessage" ForeColor="Red" runat="server"></asp:Label>
                    </div>
                    <div id="divButtons" style="float: right;">
                        <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="ms-ButtonHeightWidth" OnClick="btnCreate_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>