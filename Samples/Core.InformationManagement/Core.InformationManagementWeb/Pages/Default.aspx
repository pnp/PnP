<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.InformationManagementWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Information Management</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body  style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario 1: Fetch site policy information</h1>
        In this scenario you'll learn how to read site policy information. Following topics will be addressed:
        <ul style="list-style-type: square;">
            <li>How to read the site expiration date</li>
            <li>How to read the site closure date</li>
            <li>How to obtain the available site policies and know which one has been applied</li>
        </ul>
        <br />
        <br />
        If you've not yet applied a site policy on this site then you can do this via <b>Site Settings -> Site Policies</b> to define the policies. Using <b>Site Settings -> Site Closure and Deletion</b> you can make a policy active.
        <br />
        <br />
        <b><asp:Label ID="lblSiteClosure" runat="server"></asp:Label></b>
        <br />
        <b><asp:Label ID="lblSiteExpiration" runat="server"></asp:Label></b>
        <br />
        <br />
        In this site the following policies are defined:
        <br />
        <b><asp:Label ID="lblSitePolicies" runat="server"></asp:Label></b>
        <br />
        <br />
        The policy that's currently applied is:
        <br />
        <b><asp:Label ID="lblAppliedPolicy" runat="server"></asp:Label></b>
        <br />
        <br />
        <h1>Scenario 2: Update site policy information</h1>
        In this scenario you'll learn how to update site policy information. Following topics will be addressed:
        <ul style="list-style-type: square;">
            <li>How to apply a site policy</li>
        </ul>
        <br />
        <br />
        Pick a policy to apply from below list (current applied policy is not listed):
        <br />
        <asp:DropDownList ID="drlPolicies" runat="server"></asp:DropDownList>
          
        <asp:Button ID="btnApplyPolicy" runat="server" Text="Apply selected policy" OnClick="btnApplyPolicy_Click"/>
        <br />
    </div>
    </form>
</body>
</html>
