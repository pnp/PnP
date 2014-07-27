<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.TaxonomyMenuWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Contoso Taxonomy Menu</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
 
        <div style="padding-left: 15px; padding-right: 15px; overflow-y: scroll; position: absolute; top: 132px; bottom: 0px; right: 0px; left: 0px;">
            <div style="padding-left: 10px;">
                This example shows how you can apply term store driven navigation to SharePoint using JavaScript SCOM. The example uses the built in language capabilities in term store and shows the navigation node in different languages (english, german, french, swedish) depending on the current users profile settings. The navigation script could be incorporated in master pages to work over site collections. To setup the solution on the host web follow the steps below. 
            </div>
            <div style="padding: 10px;">
                Your current display language settings: <asp:Label ID="currentLanguages" Font-Bold="true" runat="server" /> 
            </div>
            
            <div>
                <h1 style="padding-left: 10px;">Step 1: Setup term store</h1>
                <div style="padding: 10px;">
                    Click on the button below to create the required term group, term set and terms in term store. 
                </div>
                <div style="padding: 10px;">
                    <asp:Button ID="btnAddTaxonomy" Text="Setup term store" OnClick="AddTaxonomy_Click" runat="server" />             
                </div>                     
                <div style="padding: 10px;">
                    Afterwards it should look like the picture below.<br />
                    <img src="../Images/taxonomy.png" style="padding:10px;" />
                </div>           
            </div>        

            <div>
                <h1 style="padding-left: 10px;">Step 2: Add Scripts</h1>
                <div style="padding: 10px;">
                    Click on the button below to upload JQuery and taxonomy JavaScript to the Site Assets library in the host web. This step also registers script links on the web.                     
                </div>
                <div style="padding: 10px;">
                    <asp:Button ID="btnAddScripts" Text="Add scripts and links" OnClick="AddScripts_Click" runat="server" />            
                </div>                                
                <div style="padding: 10px;">
                    Afterwards the host web should look like the picture below.<br />
                    <img src="../Images/menu.png" style="padding:10px;" />
                </div>
            </div>
                
            <div style="padding: 10px;">
                <h1 style="padding-left: 10px;">Limitations</h1>
                <div>
                    <ul>
                        <li>The script does not work currently with Minimal Download Strategy.</li>
                        <li>This no production ready code so there is no caching either when accessing user profile or term store.</li>
                        <li>The code code be rewritten to do less client callbacks by removing checks like if term set exists</li>
                    </ul>
                </div>         
            </div>
            <div style="padding: 10px;">
                <h1 style="padding-left: 10px;">Removal</h1>
                <div>
                    Click on the button to remove the script links from the host web.
                </div>
                <div style="margin-top: 10px;">
                    <asp:Button ID="btnRemoveScripts" Text="Remove script links" OnClick="RemoveScripts_Click" runat="server" />
                </div>                                
            </div>
        </div>
    </form>
</body>
</html>
