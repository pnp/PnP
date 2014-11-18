<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UserProfile.Manipulation.CSOMWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User profile management with CSOM</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 40px; position: absolute;">
            This sample shows how to read and update user profile properties using CSOM interfaces. Used APIs for update where originally introduced in <a href="http://aka.ms/spocsom">SPO CSOM package</a> released on September 2014. 
            <h1>Scenario 1: Get user profile properties</h1>
            In this scenario you'll learn how to get user profile properties.
            <br />
            <ul style="list-style-type: square;">
                <li>How to create new content types usign CSOM.</li>
                <li>How to add site columsn to the newly added content type using CSOM.</li>
            </ul>
            Click
            <asp:HyperLink ID="hplScenario1" runat="server" Text="here" />
            to learn more and test out scenario 1.
            <br />
            <br />
            <h1>Scenario 2: Update user profile property for user</h1>
            In this scenario you'll learn how to use CSOM to update user profile property.
            <ul style="list-style-type: square;">
                <li>How to access user profile</li>
                <li>How to use CSOM to update user profile property</li>
            </ul>
            Click
                <asp:HyperLink ID="hplScenario2" runat="server" Text="here"></asp:HyperLink>
            to learn more and test out scenario 2.
            <br />
            <br />
            <h1>Scenario 3: Update multi-value user profile property for user</h1>
            In this scenario you'll learn how to use CSOM to update multi-value user profile property.
            <ul style="list-style-type: square;">
                <li>How to access user profile</li>
                <li>How to update multi-value property (Skills)</li>
            </ul>
            Click
            <asp:HyperLink ID="hplScenario3" runat="server" Text="here"></asp:HyperLink>
            to learn more and test out scenario 3.
            <br />
        </div>
    </form>
</body>
</html>