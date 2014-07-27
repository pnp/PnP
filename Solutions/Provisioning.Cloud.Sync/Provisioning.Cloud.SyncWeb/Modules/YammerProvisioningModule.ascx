<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YammerProvisioningModule.ascx.cs" Inherits="Contoso.Provisioning.Cloud.SyncWeb.Modules.YammerProvisioningModule" %>
<div id="divFieldTemplate" style="display: table; width: 100%;">
    <div class="ms-core-form-line yamLogin" style="display: none;">
        <h3 class="ms-core-form-line">Login to use Yammer group</h3>
        <div id="yamLogin"></div>
    </div>
    <div id="divGroupType" style="display: none;">
        <h3 class="ms-core-form-line">Create Yammer group</h3>
        <div class="ms-core-form-line">
            <asp:DropDownList ID="cboNewsfeedType" runat="server" CssClass="ms-fullWidth cboNewsfeedType" onchange="javascript:cboNewsfeedTypeChanged(this);"></asp:DropDownList>
            <asp:HiddenField ID="hdnYammerAccessToken" runat="server" />
        </div>
    </div>
    <div id="divGroupName" style="display: none;">
        <h3 class="ms-core-form-line" style="float: left;">Group Name</h3>
        <h3 class="ms-core-form-line" id="yamGroupAvailable" style="padding-top: 6px; font-size: 10px; right: 0px; position: absolute; color: green; display: none;">Name is available</h3>
        <h3 class="ms-core-form-line" id="yamGroupUnavailable" style="padding-top: 6px; font-size: 10px; right: 0px; position: absolute; color: red; display: none;">Name is unavailable</h3>
        <div class="ms-core-form-line txtGroupName">
            <asp:TextBox ID="txtGroupName" runat="server" CssClass="ms-fullWidth"></asp:TextBox>
        </div>
    </div>    
</div>