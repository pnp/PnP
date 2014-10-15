<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CodeSample2.aspx.cs" Inherits="Core.UserProfileManipulationWeb.Pages.CodeSample2" %>

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
            <h1>Scenario 2: Update user profile property for user</h1>
            In this scenario you'll learn how to use CSOM to update user profile property.
            <ul style="list-style-type: square;">
                <li>How to access user profile</li>
                <li>How to use CSOM to update user profile property</li>
            </ul>
            <br />
            <b>About me current value</b>:
            <br />
            <i>
                <asp:Label runat="server" ID="aboutMeValue"></asp:Label></i>
            <br />
            <hr />
            <br />
            About me new value:
            <br />
            <asp:TextBox runat="server" ID="txtAboutMe" Rows="6" TextMode="MultiLine" Width="600px" Text=""></asp:TextBox>
            <br />
            <br />
            <asp:Button runat="server" ID="btnScenario2" Text="Run scenario 2" OnClick="btnScenario2_Click" />
            <br />
        </div>
    </form>
</body>
</html>
