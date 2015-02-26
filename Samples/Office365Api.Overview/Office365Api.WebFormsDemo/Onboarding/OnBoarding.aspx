<%@ Page Title="Tenant On Boarding" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OnBoarding.aspx.cs" Inherits="Office365Api.WebFormsDemo.OnBoarding" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Sign Up your Office 365 Tenant.</h3>

    <div class="form-horizontal">
        
        <hr />

        <div class="form-group">
            <span class = "control-label col-md-2">Tenant Name (like: <i>tenant</i>.onmicrosoft.com)</span>
            <div class="col-md-10">
                <asp:TextBox ID="TenantName" runat="server" />
            </div>
        </div>



        <div class="form-group">
            <span class = "control-label col-md-2">Admin Consented</span>
            <div class="col-md-10">
                <asp:CheckBox ID="AdminConsented" runat="server" />
            </div>
        </div>

        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:button id="SignUpCommand" runat="server" Text="SignUp" CssClass="btn btn-default" OnClick="SignUpCommand_Click" />
            </div>
        </div>
    </div>
</asp:Content>
