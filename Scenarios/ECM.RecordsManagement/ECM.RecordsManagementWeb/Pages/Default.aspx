<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ECM.RecordsManagementWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            <h1>Scenario 1: Enable In Place Records Management + site scoped settings</h1>
            <br />
            In place records management is
            <asp:Label runat="server" ID="lblIPREnabled" Font-Bold="true" ForeColor="Red"></asp:Label>
            on this site collection. Click on the button to
            <asp:Button ID="btnToggleIPRStatus" runat="server" Text="Activate" OnClick="btnToggleIPRStatus_Click" />
            in place records management.
            <br />
            <br />
            <table>
                <tr>
                    <td width="50%"><b>Record Restrictions</b></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Specify restrictions to place on a document or item once it has been declared as a record.  Changing this setting will not affect items which have already been declared records.  Note:  The information management policy settings can also specify different policies for records and non-records.</td>
                    <td>
                        <asp:RadioButtonList ID="rdRestrictions" runat="server">
                            <asp:ListItem Text="No Additional Restrictions" Selected="False" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Block Delete" Selected="False" Value="16"></asp:ListItem>
                            <asp:ListItem Text="Block Edit and Delete" Selected="True" Value="272"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td height="20px"></td>
                </tr>
                <tr>
                    <td width="50%"><b>Record Declaration Availability</b></td>
                    <td>Manual record declaration in lists and libraries should be:</td>
                </tr>
                <tr>
                    <td>Specify whether all lists and libraries in this site should make the manual declaration of records available by default.  When manual record declaration is unavailable, records can only be declared through a policy or workflow.</td>
                    <td>
                        <asp:RadioButtonList ID="rdAvailability" runat="server">
                            <asp:ListItem Text="Available in all locations by default" Selected="True" Value="True"></asp:ListItem>
                            <asp:ListItem Text="Not available in all locations by default" Selected="False" Value="False"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td height="20px"></td>
                </tr>
                <tr>
                    <td width="50%"><b>Declaration Roles</b></td>
                    <td>The declaration of records can be performed by:</td>
                </tr>
                <tr>
                    <td valign="top">Specify which user roles can declare and undeclare record status manually.</td>
                    <td>
                        <asp:RadioButtonList ID="rdDeclarationBy" runat="server">
                            <asp:ListItem Text="All list contributors and administrators" Selected="True" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Only list administrators" Selected="False" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Only policy actions" Selected="False" Value="3"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td height="20px"></td>
                </tr>
                <tr>
                    <td></td>
                    <td>Undeclaring a record can be performed by: </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:RadioButtonList ID="rdUndeclarationBy" runat="server">
                            <asp:ListItem Text="All list contributors and administrators" Selected="False" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Only list administrators" Selected="True" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Only policy actions" Selected="False" Value="3"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnSaveSiteScopedIPRSettings" runat="server" Text="Save changes" OnClick="btnSaveSiteScopedIPRSettings_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <h1>Scenario 2: List scoped settings</h1>
            <br />
            List scoped settings will be defined against a document library with name <b>IPRTest</b>. 
            <br />
            <br />
            <table>
                <tr>
                    <td width="50%">
                        <b>Manual Record Declaration Availability</b>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td>Specify whether this list should allow the manual declaration of records.  When manual record declaration is unavailable, records can only be declared through a policy or workflow.</td>
                    <td>
                        <asp:RadioButtonList ID="rdListAvailability" runat="server">
                            <asp:ListItem Text="Use the site collection default setting" Selected="True" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Always allow the manual declaration of records" Selected="False" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Never allow the manual declaration of records" Selected="False" Value="3"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td height="20px"></td>
                </tr>
                <tr>
                    <td>
                        <b>Automatic Declaration</b>
                    </td>
                    <td>
                        <asp:CheckBox ID="chbAutoDeclare" runat="server" Text="Automatically declare items as records when they are added to this list." />
                    </td>
                </tr>
                <tr>
                    <td height="20px"></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnSaveListScopedIPRSettings" runat="server" Text="Save changes" OnClick="btnSaveListScopedIPRSettings_Click" />
                    </td>
                </tr>
            </table>
            <br />
        </div>
    </form>
</body>
</html>
