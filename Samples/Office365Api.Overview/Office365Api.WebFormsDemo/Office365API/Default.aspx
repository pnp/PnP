<%@ Page Async="true" Title="Office 365 API" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs" Inherits="Office365Api.WebFormsDemo.Office365API.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %>.</h2>
    <h3>Let's play with the Office 365 API!</h3>

    <p>
        <div class="row">
            <div class="row">
                <h2>List of TOP 10 My Files in OneDrive for Business</h2>
                <p></p>
                <p><asp:button ID="ListFilesCommand" runat="server" Text="List My Files" CssClass="btn btn-default" OnClick="ListFilesCommand_Click" /></p>
            </div>
            <div class="row">
                <h2>List of TOP 10 Contacts</h2>
                <p></p>
                <p><asp:button ID="ListContactsCommand" runat="server" Text="List Contacts" CssClass="btn btn-default" OnClick="ListContactsCommand_Click" /></p>
            </div>
            <div class="row">
                <h2>List of TOP 10 emails in Inbox</h2>
                <p></p>
                <p><asp:Button ID="ListEmailsCommand" runat="server" Text="List Messages" CssClass="btn btn-default" OnClick="ListEmailsCommand_Click" /></p>
            </div>
            <div class="row">
                <h2>Send a mail Message as Current User</h2>
                <p>To: <asp:TextBox ID="TargetEmail" runat="server" Text="[target-email]" Columns="30" /></p>
                <p><asp:Button ID="SendMailCommand" runat="server" Text="Send Mail" CssClass="btn btn-default" OnClick="SendMailCommand_Click" /></p>
            </div>
            <div class="row">
                <asp:Label ID="commandResult" runat="server" />
                <br />
                <ul>
                    <asp:ListView ID="resultsList" runat="server">
                        <ItemTemplate>
                            <li><asp:Label runat="server" Text='<%# Container.DataItem %>' /></li>
                        </ItemTemplate>
                    </asp:ListView>
                </ul>
            </div>
        </div>
    </p>

</asp:Content>
