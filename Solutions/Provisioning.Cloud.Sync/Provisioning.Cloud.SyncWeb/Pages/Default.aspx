<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Provisioning.Cloud.SyncWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Start a new site</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/UXScripts.js"></script>
    <script type="text/javascript">
        var titleChangeCallbacks = [];
        function txtTitleChanged() {
            for (i = 0; i < titleChangeCallbacks.length; i++)
                titleChangeCallbacks[i]();
        }
        var urlChangeCallbacks = [];
        function txtUrlChanged() {
            for (i = 0; i < urlChangeCallbacks.length; i++)
                urlChangeCallbacks[i]();
        }
        var validationChecks = [];
        function validateMain() {
            $('.ms-fullWidth').removeClass('invalid');
            valid = true;
            if ($('#listSites').val() == null) {
                $('#listSites').addClass('invalid');
                valid = false;
            }
            if ($('#txtTitle').val().length == 0) {
                $('#txtTitle').addClass('invalid');
                valid = false;
            }
            if ($('#txtUrl').val().length == 0) {
                $('#txtUrl').addClass('invalid');
                valid = false;
            }
            return valid;
        }
        validationChecks.push(validateMain);
    </script>
    <style type="text/css">
        .invalid {
            border: 1px dashed red !important;
        }
    </style>
</head>
<body style="display: none;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="1800"></asp:ScriptManager>
    <div id="divSPChrome"></div>
    <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
        <ProgressTemplate>
            <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
                <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                    <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                    <span class="ms-accentText" style="font-size: 36px;">&nbsp;Working on it...</span>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div style="left: 50%; width: 450px; margin-left: -225px; position: absolute;">
                <div id="divFieldTemplate" style="display: table; width: 100%;">
                    <h3 class="ms-core-form-line">Pick a template</h3>
                    <div class="ms-core-form-line">
                         <asp:ListBox ID="listSites" runat="server" CssClass="ms-fullWidth" AutoPostBack="true" OnSelectedIndexChanged="listSites_SelectedIndexChanged"></asp:ListBox>
                    </div>
                </div>
                <div id="divFieldTitle" style="display: table;">
                    <h3 class="ms-core-form-line">Give it a name</h3>
                    <div class="ms-core-form-line">
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="ms-fullWidth" onkeyup="javascript:txtTitleChanged();"></asp:TextBox>
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
                <asp:Panel ID="pnlModules" runat="server">

                </asp:Panel>
                <div id="divButtons" style="float: right;">
                    <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="ms-ButtonHeightWidth" OnClick="btnCreate_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>