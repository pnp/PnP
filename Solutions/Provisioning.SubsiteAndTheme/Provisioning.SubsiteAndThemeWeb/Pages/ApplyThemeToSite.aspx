<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyThemeToSite.aspx.cs" Inherits="Provisioning.SubsiteAndThemeWeb.Pages.ApplyThemeToSite" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Apply Theme to All Sites</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>

<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
    
        <h1><asp:Localize runat="server" Text="Apply Theme to All Sites" /></h1>
        <p><asp:Localize runat="server" Text="Applies the selected theme to all subsites." /></p>
    
        <div>
            <dl>
                <dt><asp:Label runat="server" ID="ErrorMessage" ForeColor="Red" /></dt>
            </dl>

            <dl>
                <dt><asp:Label runat="server" Text="Theme" Font-Bold="true"  /></dt>
                <dd>
                    <asp:DropDownList runat="server" ID="ThemeList" DataTextField="Name" DataValueField="Name" AppendDataBoundItems="true">
                        <asp:ListItem Text="Select a theme" Value="" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ThemeList" Text="Theme selection required" />
                </dd>
            </dl>

            <dl>
                <dt><asp:Label runat="server" Text="Set site logo" Font-Bold="true"  /></dt>
                <dt><asp:CheckBox runat="server" ID="SetSiteLogoCheckbox" /></dt>
            </dl>
        </div>
        
        <div style="margin-top:20px">
            <asp:Button runat="server" ID="CancelButton" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
            <asp:Button runat="server" ID="SubmitButton" Text="Submit" OnClick="SubmitButton_Click" />
        </div>
    </div>
    </form>
</body>
</html>
