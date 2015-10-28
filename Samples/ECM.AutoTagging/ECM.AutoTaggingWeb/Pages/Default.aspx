<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ECM.AutoTaggingWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/common.js"></script>

</head>
<body style="display: none;  overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <asp:UpdateProgress ID="progress" runat="server" AssociatedUpdatePanelID="update" DynamicLayout="true">
    <ProgressTemplate>
        <div id="divWaitingPanel" style="position: absolute; z-index: 3; background: rgb(255, 255, 255); width: 100%; bottom: 0px; top: 0px;">
            <div style="top: 40%; position: absolute; left: 50%; margin-left: -150px;">
                <img alt="Working on it" src="data:image/gif;base64,R0lGODlhEAAQAIAAAFLOQv///yH/C05FVFNDQVBFMi4wAwEAAAAh+QQFCgABACwJAAIAAgACAAACAoRRACH5BAUKAAEALAwABQACAAIAAAIChFEAIfkEBQoAAQAsDAAJAAIAAgAAAgKEUQAh+QQFCgABACwJAAwAAgACAAACAoRRACH5BAUKAAEALAUADAACAAIAAAIChFEAIfkEBQoAAQAsAgAJAAIAAgAAAgKEUQAh+QQFCgABACwCAAUAAgACAAACAoRRACH5BAkKAAEALAIAAgAMAAwAAAINjAFne8kPo5y02ouzLQAh+QQJCgABACwCAAIADAAMAAACF4wBphvID1uCyNEZM7Ov4v1p0hGOZlAAACH5BAkKAAEALAIAAgAMAAwAAAIUjAGmG8gPW4qS2rscRPp1rH3H1BUAIfkECQoAAQAsAgACAAkADAAAAhGMAaaX64peiLJa6rCVFHdQAAAh+QQJCgABACwCAAIABQAMAAACDYwBFqiX3mJjUM63QAEAIfkECQoAAQAsAgACAAUACQAAAgqMARaol95iY9AUACH5BAkKAAEALAIAAgAFAAUAAAIHjAEWqJeuCgAh+QQJCgABACwFAAIAAgACAAACAoRRADs=" style="width: 32px; height: 32px;" />
                <span class="ms-accentText" style="font-size: 36px;">&nbsp;Doing the best I can on it...</span>
            </div>
        </div>
    </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="update" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div style="left: 40px; position: absolute;">
                <h1>AutoTagging Sample</h1>
                This sample solution shows how one can auto tag content when an item is added to the library. The use case for this scenario, is you 
                may have a requirement to update fields based on specific property values to assist with Search Findability. 
                The sample is dependent on using MMS 
                and a custom user profile property. 
                <br />
                <h2>Set Up</h2>
                <br />
                <h3>Step 1</h3>
                <ul>
                    <li><b>Step 1</b> Navigate to Term Store Managment </li>
                    <li><b>Step 1.1</b> Create a Term Group Called Enterprise </li>
                    <li><b>Step 1.2</b> Create a Term Set called Classification </li>
                    <li><b>Step 1.3</b> Create Term - HBI </li>
                    <li><b>Step 1.4</b> Create Term - MBI </li>
                    <li><b>Step 1.5</b> Create Term - LBI </li>
                </ul>
                <br />
                <h3>Step 2</h3>
                <ul>
                    <li><b>Step 2</b> Create a Custom user profile property for the users</li>
                    <li><b>Step 2.1</b> Navigate to User Profiles</li>
                    <li><b>Step 2.2</b> Manage User Properties</li>
                    <li><b>Setp 2.3</b> New Property </li>
                    <li><b>Setp 2.4</b> Name & Display name is Classification</li>
                    <li><b>Setp 2.4</b> Type is string (Single Value)</li>
                    <li><b>Setp 2.5</b> Check Configure a Term Set to be used for this property </li>
                    <li><b>Setp 2.6</b> In the Pick a Term Set for this property, choose the TermSet that you created in Step 1, which is Classifcation</li>
                    <li><b>Setp 2.7</b> Policy Settings - Check User can override</li>
                    <li><b>Setp 2.8</b> Edit Settings - Allow Users to edite values for this property</li>
                    <li><b>Setp 2.9</b> Display Settings - Check Show in the profile properties section of the user's profile page and Show on the Edit Details page</li>
                    <li><b>Setp 2.10</b> Click Ok</li>
                </ul>
                <br />
                <h3>Step 3</h3>
                <ul>
                    <li><b>Step 2</b> Navigate to your profile page</li>
                    <li><b>Step 2.1</b> Edit your Profile</li>
                    <li><b>Step 2.2</b> Edit Custom properties</li>
                    <li><b>Setp 2.3</b> Update the Classification Property and choose a term</li>
                    <li><b>Setp 2.10</b> Click Save all and close</li>
                </ul>
       
                <br />  
                <h3>Scenario 1</h3>
                 <ul>
                    <li>   Uses ItemAdding</li>
                </ul>
                <br />

                <h3>Scenario 2</h3>
                <ul>
                    <li>   Uses ItemAdded</li>
                </ul>
              
                <br />

                <br />
                  <asp:Button runat="server" ID="btnScenario1" Text="Run scenario 1" OnClick="btnScenario1_Click" />
                  <asp:Button runat="server" ID="btnScenario2" Text="Run scenario 2" OnClick="btnScenario2_Click" />
                  <asp:Button runat="server" ID="btnCleanUp1" Text="Remove Event scenario 1" OnClick="btnScenario3_Click" />
                  <asp:Button runat="server" ID="btnCleanUp2" Text="Remove Event scenario 2" OnClick="btnScenario4_Click" />
                  <br />
            </div>    
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
