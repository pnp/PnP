<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessImpactProvisioningModule.ascx.cs" Inherits="Contoso.Provisioning.Cloud.SyncWeb.Modules.BusinessImpactProvisioningModule" %>
<div id="divFieldTemplate" style="display: table; width: 100%;">
    <h3 class="ms-core-form-line">How sensitive is your data?</h3>
    <div class="ms-core-form-line">
        <asp:DropDownList ID="cboSensitivity" runat="server" CssClass="ms-fullWidth"></asp:DropDownList>
    </div>
</div>