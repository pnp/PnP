<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.PeoplePickerWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../Styles/peoplepickercontrol.css" />
    <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/app.js?rev=1" type="text/javascript"></script>
    <script src="../Scripts/peoplepickercontrol.js?rev=2" type="text/javascript"></script>
    <script src="../Scripts/csompeoplepickercontrol.js?rev=2" type="text/javascript"></script>
</head>
<body style="display: none;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 50%; width: 500px; margin-left: -250px; position: absolute;">

            <h2>Peoplepicker using JSOM</h2>
            <div id="divFieldOwners">
                <h3 class="ms-core-form-line" style="margin-top: 15px !important;">Select a user:</h3>
                <div class="ms-core-form-line">
                    <div id="divAdministrators" class="cam-peoplepicker-userlookup ms-fullWidth">
                        <span id="spanAdministrators"></span>
                        <asp:TextBox ID="inputAdministrators" runat="server" CssClass="cam-peoplepicker-edit" Width="70"></asp:TextBox>
                    </div>
                    <div id="divAdministratorsSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
                    <asp:HiddenField ID="hdnAdministrators" runat="server" />
                </div>
            </div>
            <div id="divFieldTitle">
                <h3 class="ms-core-form-line" style="margin-top: 15px !important;">Give a title:</h3>
                <div class="ms-core-form-line">
                    <asp:TextBox ID="txtTitle" CssClass="ms-fullWidth" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="ms-core-form-line">
                <asp:Label ID="lblEnteredData" runat="server" CssClass="ms-fullWidth"></asp:Label>
            </div>
            <div id="divButtons" style="float: right;">
                <asp:Button ID="btnCreate" runat="server" Text="Submit" CssClass="ms-ButtonHeightWidth" OnClick="btnCreate_Click" />
            </div>
            
            <br />
            <br />
            <br />

            <h2>Peoplepicker using serverside webmethod (CSOM)</h2>
            <div id="divTestCsomPeoplePicker">
                <h3 class="ms-core-form-line" style="margin-top: 15px !important;">Select a user:</h3>
                <div class="ms-core-form-line">
                    <div id="divCsomAdministrators" class="cam-peoplepicker-userlookup ms-fullWidth">
                        <span id="spanCsomAdministrators"></span>
                        <asp:TextBox ID="inputCsomAdministrators" runat="server" CssClass="cam-peoplepicker-edit" Width="70"></asp:TextBox>
                    </div>
                    <div id="divCsomAdministratorsSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
                    <asp:HiddenField ID="hdnCsomAdministrators" runat="server" />
                </div>
            </div>

            <div class="ms-core-form-line">
                <asp:Label ID="lblCsomEnteredData" runat="server" CssClass="ms-fullWidth"></asp:Label>
            </div>
            <div id="divCsomButtons" style="float: right;">
                <asp:Button ID="btnGetValueByServer" runat="server" Text="Get values by server" CssClass="ms-ButtonHeightWidth" OnClick="btnGetValueByServer_Click" />
                <button id="GetValuesByJavascript">Get values by javascript</button>
            </div>        

        </div>
       
    </form>
</body>
</html>
