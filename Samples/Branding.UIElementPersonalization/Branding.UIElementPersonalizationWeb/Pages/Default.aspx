<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Branding.UIElementPersonalizationWeb.Default" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/modernizr-2.6.2.js"></script>
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
    <script type="text/javascript" src="_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="_layouts/15/sp.js"></script>      
    <script type="text/javascript" src="../Scripts/app.js"></script>
        
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
    <div id="divSPChrome"></div>
    <div style="left: 40px; position: absolute;">
        <h1>Scenario: Personalizing User Interface Elements</h1><p style="width: 75%">
        In this scenario you'll see how to personalize UI elements. This sample renders an image next to the site title that is determined by a value in your About Me section of your profile.
        The value in your profile is matched up with a value in the sample codes list. The codes in the list have an associated link to an image stored in the Site Assets library.
        The app will deploy a sample codes list and upload some sample images to the Site Assets library. It will then do the javascript injection to inject the link to the personalize.js file
        which gets executed when your page loads. The sample also uses HTML5 localstorage to store the value retrieved from your About Me section in your profile so that this user profile query does not happen each time the page loads.</p>        
        <ul style="list-style-type: square;">
            <li><b>Step 1:</b> Edit your profile's About Me section and add one of the following: "XX", "YY" or "ZZ"</li>
            <li><b>Step 2:</b> "Inject" the customization to your current site using the button in the Demo section</li>
            <li><b>Step 3:</b> Check out the changes by clicking on "Back to Site" in the top navigation.</li>
        </ul>        
        <br />
        <br />       
        Click the buttons below to "inject" or remove the customization to your current site. 
        <br />
        <br />
        <asp:Button runat="server" ID="btnSubmit" Text="Inject customization" OnClick="btnSubmit_Click"/>
        <asp:Button runat="server" ID="btnRemove" Text="Remove customization" OnClick="btnRemove_Click" />  
        <br />
        <div id="divFieldTitle" style="margin-top: 40px !important;">
                Deploy Status:
                <div class="ms-core-form-line" style="margin-left: 10px; margin-top: 15px !important;">
                    <asp:Listbox runat="server" id="status" class="form-control" Font-Size="Smaller" Width="400px" ForeColor="#0066FF" Height="100px"></asp:Listbox>
                </div>
    </div>  
    </div>
    
    </form>
    
</body>
</html>
