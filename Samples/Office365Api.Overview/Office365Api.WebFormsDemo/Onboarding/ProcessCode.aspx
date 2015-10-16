<%@ Page Title="You successfully signed up" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProcessCode.aspx.cs" Inherits="Office365Api.WebFormsDemo.Onboarding.ProcessCode" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div id="successedOnBoarding" runat="server" visible="false">
        <h2>You successfully signed up!</h2>
        <p>Click on "Office 365 API" menu item to sign in and play with Office 365 API.</p>
    </div>
    <div id="failedOnBoarding" runat="server" visible="false">
        <h2>Failed to sign up your tenant!</h2>
        <p>
            <asp:Label ID="errorMessage" runat="server" /><br />
            <asp:Label ID="errorDescription" runat="server" />
        </p>
    </div>

</asp:Content>
