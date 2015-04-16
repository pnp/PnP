<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.YammerWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Branded sites with Yammer</title>
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
                        <span class="ms-accentText" style="font-size: 36px;">&nbsp;Working on it...</span>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <div style="display:block;position:absolute;left:600px; top:250px; width:250px">
                    <i><lu><li>By default OpenGraph object is used and assocated is assocated to All Company group. </li>
                        <li>If group is selected, Feed is associated to that group. </li>
                        <li>If you create new group, a name has to be given.  </li>
                        <li>We recommend usage of OpenGraph for this scenario to avoid "group" pollution.</li>
                        </lu>
                    </i>
                </div>
                <div style="width: 500px; margin-left: 50px;">
                    <div id="divFieldTemplate" style="display: table; width: 100%;">
                        <h3 class="ms-core-form-line">Pick a template</h3>
                        <div class="ms-core-form-line">
                            <asp:ListBox ID="listSites" runat="server" CssClass="ms-fullWidth"></asp:ListBox>
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
                    <div id="divFieldYammerFeedType">
                        <h3 class="ms-core-form-line">Yammer feed type</h3>
                        <div class="ms-core-form-line">
                            <asp:RadioButtonList ID="YammerFeedType" runat="server" OnSelectedIndexChanged="YammerFeedType_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Selected="True" Text="OpenGraph" Value="OpenGraph" />
                                <asp:ListItem Text="Group" Value="Group" />
                            </asp:RadioButtonList>
                            <br />
                        </div>
                    </div>
                    <div id="divFieldYammerGroup">
                        <h3 class="ms-core-form-line">Group Association Style</h3>
                        <div class="ms-core-form-line">
                            <asp:RadioButtonList ID="YammerGroupAssociationType" runat="server" OnSelectedIndexChanged="YammerGroupAssociationType_SelectedIndexChanged" AutoPostBack="true" Enabled="false">
                                <asp:ListItem Selected="True" Text="Existing" Value="Existing" />
                                <asp:ListItem Text="New" Value="New" />
                            </asp:RadioButtonList>
                            <br />
                            <i>If you give group name which exists, that group will be used.</i>
                        </div>
                    </div>
                    <div id="divFieldYammerExistingGroup">
                        <h3 class="ms-core-form-line">Choose the group to associate feed to</h3>
                        <div class="ms-core-form-line">
                            <asp:DropDownList ID="YammerExistingGroups" runat="server" Enabled="true"></asp:DropDownList>
                        </div>
                    </div>
                    <div id="divFieldYammerGroup">
                        <h3 class="ms-core-form-line">New Yammer Group Name</h3>
                        <div class="ms-core-form-line">
                            <asp:TextBox ID="txtYammerGroup" runat="server" CssClass="ms-fullWidth" Enabled="false"></asp:TextBox>
                            <br />
                            <i>If you give group name which exists, that group will be used.</i>
                        </div>
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
