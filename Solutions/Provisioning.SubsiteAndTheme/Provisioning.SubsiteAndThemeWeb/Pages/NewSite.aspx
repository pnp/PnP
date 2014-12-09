<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewSite.aspx.cs" Inherits="Provisioning.SubsiteAndThemeWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create new site</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>

<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
    
        <h1><asp:Localize runat="server" Text="Create a new site" /></h1>
        <p><asp:Localize runat="server" Text="Provision a new sub-site with a specific theme." /></p>

        <div>
            <dl>
                <dt><asp:Label runat="server" ID="ErrorMessage" ForeColor="Red" /></dt>
            </dl>

            <dl>
                <dt><asp:Label runat="server" Text="Title" Font-Bold="true" /></dt>
                <dd>
                    <asp:TextBox runat="server" ID="TitleTextBox" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TitleTextBox" Text="Title is required" />
                </dd>
            </dl>

            <dl>
                <dt><asp:Localize runat="server" Text="Description" /></dt>
                <dd><asp:TextBox runat="server" ID="DescriptionTextBox" TextMode="MultiLine" /></dd>
            </dl>
            
            <dl>
                <dt><asp:Label runat="server" Text="Path" Font-Bold="true" /></dt>
                <dd>
                    <asp:Literal runat="server" ID="ParentSiteLabel" /><asp:TextBox runat="server" ID="PathTextBox" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="PathTextBox" Text="Path is required"/>
                </dd>
            </dl>
            
            <dl>
                <dt><asp:Label runat="server" Text="Theme" Font-Bold="true"  /></dt>
                <dd>
                    <asp:DropDownList runat="server" ID="ThemeList" DataTextField="Name" DataValueField="Name" AppendDataBoundItems="true">
                        <asp:ListItem Text="Select a theme" Value="" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ThemeList" Display="Dynamic" />
                </dd>
            </dl>
            
            <dl>
            <dt><asp:Label runat="server" Text="Template" Font-Bold="true"  /></dt>
            <dd>
                <asp:DropDownList runat="server" ID="TemplateList" DataTextField="DisplayName" DataValueField="TemplateId" AppendDataBoundItems="true">
                    <asp:ListItem Text="Select a template" Value="" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TemplateList" Display="Dynamic" />
            </dd>
            </dl>
        </div>
        
        <div style="margin-top:20px">
            <asp:Button runat="server" ID="CancelButton" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
            <asp:Button runat="server" ID="SubmitButton" Text="Submit" OnClick="SubmitButton_Click" />
            <asp:HyperLink runat="server" ID="ApplyThemeButton" Text="Apply theme to all sites" NavigateUrl="~/Pages/ApplyThemeToSite.aspx"  />
        </div>
    </div>
    </form>
</body>

</html>
