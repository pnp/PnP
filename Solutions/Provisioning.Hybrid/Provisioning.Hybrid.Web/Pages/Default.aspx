<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Provisioning.Hybrid.Web.Pages.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../Styles/AppStyles.css" />
    <link rel="Stylesheet" type="text/css" href="../Styles/peoplepickercontrol.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
    <script type="text/javascript" src="../Scripts/peoplepickercontrol.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
        <div style="left: 50%; width: 500px; margin-left: -250px; position: absolute;">
        <div id="divFieldTitle">
            <h3 class="ms-core-form-line line-space"><asp:Literal ID="Literal1" runat="server" Text="Title:" /></h3>
            <div class="ms-core-form-line">
                <asp:TextBox ID="txtTitle" runat="server" CssClass="ms-fullWidth" TextMode="MultiLine" Rows="1" MaxLength="80"></asp:TextBox>
            </div>
        </div>

        <div id="divFieldTemplate">
            <h3 class="ms-core-form-line line-space"><asp:Literal ID="Literal2" runat="server" Text="Template:" /></h3>
            <div class="ms-core-form-line">
                <asp:DropDownList ID="drlTemplate" runat="server" CssClass="ms-fullwidth">
                    <asp:ListItem Text="Contoso collaboration site" Selected="True" Value="ContosoCollaboration" />
                    <asp:ListItem Text="Contoso project site"  Value="ContosoProject" />
                </asp:DropDownList>
            </div>
        </div>

        <div id="divFieldOwners">
            <h3 class="ms-core-form-line line-space"><asp:Literal ID="Literal4" runat="server" Text="Owners:" /></h3>
            <div class="ms-core-form-line">
                <div id="divAdministrators" class="cam-peoplepicker-userlookup ms-fullWidth">
                    <span id="spanAdministrators"></span>
                    <asp:TextBox ID="inputAdministrators" runat="server" CssClass="cam-peoplepicker-edit" Width="70"></asp:TextBox>
                </div>
                <div id="divAdministratorsSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
                <asp:HiddenField ID="hdnAdministrators" runat="server" />
            </div>
        </div>

        <div id="divDataClass">
            <h3 class="ms-core-form-line line-space"><asp:Literal ID="Literal3" runat="server" Text="Data classification:" /></h3>
            <div class="ms-core-form-line">
                <asp:DropDownList ID="drlClassification" runat="server" CssClass="ms-fullwidth" >
                    <asp:ListItem Text="LBI (Low Business Impact)" Selected="True" Value="LBI" />
                    <asp:ListItem Text="MBI (Medium Business Impact)"  Value="MBI" />
                    <asp:ListItem Text="HBI (High Business Impact)"  Value="HBI" />
                </asp:DropDownList>
            </div>
        </div>

        <div id="divFieldErrors">
            <div class="ms-core-form-line">
                <asp:Label ID="lblErrors" runat="server" CssClass="lblError ms-fullWidth" />
            </div>
        </div>

        <div id="divLoadingDialog" title="Processing..." style="display: none;">
            <p>
            <img src="../Images/gears_anv4.gif" width='16' height='16' alt="" />                
            <asp:Label ID="lblLoadingDialog" runat="server" ForeColor="Red" Text="Please wait while your request is processed..." />
            </p>
        </div>

        <div id="divButtons" style="float: right;">
            <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="ms-ButtonHeightWidth" OnClick="btnCreate_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ms-ButtonHeightWidth" OnClick="btnCancel_Click" />
        </div>

    </div>
    </form>
</body>
</html>

