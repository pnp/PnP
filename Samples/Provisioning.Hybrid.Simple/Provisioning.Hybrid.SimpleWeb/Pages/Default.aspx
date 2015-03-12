<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.Hybrid.SimpleWeb.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hybrid Site Collection Creation</title>
    <link rel="Stylesheet" type="text/css" href="../Styles/AppStyles.css" />
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
                <asp:MultiView ID="processViews" runat="server" ActiveViewIndex="0">
                    <asp:View ID="RequestView" runat="server">
                        <div style="left: 50%; width: 500px; margin-left: -250px; position: absolute;">
                            <div id="divFieldTitle">
                                <h3 class="ms-core-form-line line-space">
                                    <asp:Literal ID="Literal1" runat="server" Text="Title:" /></h3>
                                <div class="ms-core-form-line">
                                    <asp:TextBox ID="txtTitle" runat="server" CssClass="ms-fullWidth" TextMode="MultiLine" Rows="1" MaxLength="80"></asp:TextBox>
                                </div>
                            </div>

                            <div id="divFieldTemplate">
                                <h3 class="ms-core-form-line line-space">
                                    <asp:Literal ID="Literal2" runat="server" Text="Template:" /></h3>
                                <div class="ms-core-form-line">
                                    <asp:DropDownList ID="drlTemplate" runat="server" CssClass="ms-fullwidth">
                                        <asp:ListItem Text="Contoso collaboration site" Selected="True" Value="ContosoCollaboration" />
                                        <asp:ListItem Text="Contoso project site" Value="ContosoProject" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div id="divDataClass">
                                <h3 class="ms-core-form-line line-space">
                                    <asp:Literal ID="Literal3" runat="server" Text="Site target:" /></h3>
                                <div class="ms-core-form-line">
                                    <asp:DropDownList ID="drlEnvironment" runat="server" CssClass="ms-fullwidth">
                                        <asp:ListItem Text="Cloud" Selected="True" Value="Cloud" />
                                        <asp:ListItem Text="On-Premises" Value="Onprem" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div id="divFieldErrors">
                                <div class="ms-core-form-line">
                                    <asp:Label ID="lblErrors" runat="server" CssClass="lblError ms-fullWidth" />
                                </div>
                            </div>

                            <div id="divButtons" style="float: right;">
                                <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="ms-ButtonHeightWidth" OnClick="btnCreate_Click" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth" OnClick="btnCancel_Click" />
                            </div>

                        </div>
                    </asp:View>
                    <asp:View ID="RecordedView" runat="server">
                        <div style="width: 450px; margin-left: 50px;">
                            <div id="divFieldTemplate" style="display: table; width: 100%;">
                                <h3 class="ms-core-form-line">Your request has been recorded.
                                </h3>
                                <div class="ms-core-form-line">
                                    Title -
                                    <asp:Label runat="server" ID="lblTitle" />
                                    <br />
                                    Environment -
                                    <asp:Label runat="server" ID="lblEnvironment" />
                                    <br />
                                    Site collection administrator -
                                    <asp:Label runat="server" ID="lblSiteColAdmin" />
                                    <br />
                                    <br />
                                    You will receive email when site collection has been created with the connectivity details.
                                    <br />
                                </div>
                                <div id="divButtons" style="float: right;">
                                    <asp:Button ID="btnProceed" runat="server" Text="Proceed" CssClass="ms-ButtonHeightWidth" OnClick="btnCancel_Click" />
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


