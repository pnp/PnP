<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CodeSample3.aspx.cs" Inherits="Core.UserProfileManipulationWeb.Pages.CodeSample3" %>

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
            <h1>Scenario 3: Update multi-value user profile property for user</h1>
            In this scenario you'll learn how to use CSOM to update multi-value user profile property.
            <ul style="list-style-type: square;">
                <li>How to access user profile</li>
                <li>How to update multi-value property (Skills)</li>
            </ul>
            <br />
            <b>Skills current value</b>:
            <br />
            <i>
                <asp:Label runat="server" ID="lblSkills"></asp:Label></i>
            <br />
            <hr />
            <br />
            <asp:TextBox runat="server" ID="txtSkillToAdd" Rows="6" Width="172px" Text=""></asp:TextBox>
            <asp:Button ID="btnAddSkill" runat="server" Text="Add Skill" OnClick="btnAddSkill_Click" />
            <br />
            <asp:ListBox ID="lstSkills" runat="server" Height="78px" Width="254px"></asp:ListBox>
            <asp:Button ID="btnRemoveSkill" runat="server" Text="Remove Selected" OnClick="btnRemoveSkill_Click" />
            <br />
            <br />
            <asp:Button runat="server" ID="btnScenario3" Text="Run scenario 3" OnClick="btnScenario3_Click" />
            <br />

        </div>
    </form>
</body>
</html>
