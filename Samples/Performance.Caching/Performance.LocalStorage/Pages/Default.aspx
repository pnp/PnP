<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/modernizr-2.6.2.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <meta name="WebPartPageExpansion" content="full" />

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Caching Example App - Using HTML5 Local Storage
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div id="divSPChrome"></div>
        <div class="form-group" style="left: 25%; width: 500px; margin-left: -250px; position: absolute;">

            <div id="divOverview" class="form-group">
                <h3 class="ms-core-form-line" style="margin-top: 15px !important;">Overview:</h3>
                <div class="ms-core-form-line">
                    <div id="divDescription" class="ms-fullWidth">
                        <span id="spanDesciption"></span>
                        <p>This is a simple app that just reads your About Me section from your profile and caches it into HTML5 local storage. 
                           You can modify the About Me data and save it for later by storing it into local storage. The idea is to save on trips to target services.
                           Clicking the "Clear Cache" button will clear local storage and retrieve your About Me data from your profile again.
                           <br />
                           <br />
                           Note: This does NOT update your profile. The app only reads from it.                           
                        </p>
                    </div>                    
                </div>
            </div>

            <div id="divAboutMe" class="form-group">
                <h3 class="ms-core-form-line" style="margin-top: 25px !important;">About Me:</h3>
                <div class="ms-core-form-line">
                    <div id="divComposer" class="ms-fullWidth">
                        <span id="spanAboutMe"></span>
                        <textarea id="aboutMeText" class="form-control" rows="10" cols="50"></textarea>
                    </div>                    
                </div>
            </div>

            <div class="form-group" id="divButtons" style="margin-left: 175px; width: 500px; position: absolute;">
                <input class="ms-ButtonHeightWidth" type="button" id="btnSave" title="Save for later" value="Save for later" onclick="saveForLater(aboutMeText.innerText, 'aboutMeValue');" />
                <input class="ms-ButtonHeightWidth" type="button" id="btnBustCache" title="Clear the cache" value="Clear the cache" onclick="clearCache('aboutMeValue');" />
            </div>

            <div id="divFieldTitle" class="form-group">
                <h3 class="ms-core-form-line" style="margin-top: 50px !important;">Cache Status:</h3>
                <div class="ms-core-form-line">
                    <textarea  id="status" class="form-control" rows="10" cols="50"></textarea>
                </div>
            </div>         
            

            <div id="divExpiryStatus" class="form-group">
                
                <div class="ms-core-form-line">
                    <h3 class="ms-core-form-line" style="margin-top: 15px !important;">Cache Expiration Time (in seconds): (empty = no expiration)</h3>
                    <input type="text" id="expirySetting" class="form-control" style="width: 75px; height: 23px;" />
                    <input class="ms-ButtonHeightWidth" type="button" id="btnSetExpiration" title="Update the expiration configuration" value="Update" onclick="setExpiryConfiguration();" />
                </div>
            </div>          
            
        </div>

</asp:Content>
