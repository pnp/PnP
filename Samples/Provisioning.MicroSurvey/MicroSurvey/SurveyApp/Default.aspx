<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>
<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>
    <meta name="WebPartPageExpansion" content="full" />
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    MicroSurvey Settings
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <%-- Web part root element and loader --%>
    <div>

        <link rel="Stylesheet" type="text/css" href="survey.css" />

        <table><tr>
            <td>
                <div style="width: 400px;" ng-controller="settings as vm" ng-include="'settings.html'"></div>
            </td>
            <td class="webPart">
                <div style="width: 400px; height: 250px;" ng-controller="main as vm" ng-include="'main.html'"></div>
            </td>
        </tr></table>

        <%-- Bootstrap with Widget Wrangler --%>
        <script type="text/javascript" src="../SurveyApp/pnp-ww.js" 
                ww-appName="microSurvey" 
                ww-appType="Angular"
                ww-appScripts='[{"src": "../SurveyApp/angular.1.3.14.min.js", "priority":0, "test": "false"},
                                {"src": "../SurveyApp/mainController.js", "priority":1, "test": "false"},
                                {"src": "../SurveyApp/settingsController.js", "priority":2, "test": "false"},
                                {"src": "../SurveyApp/listFormController.js", "priority":3, "test": "false"},
                                {"src": "../SurveyApp/surveyService.js", "priority":4, "test": "false"},
                                {"src": "../SurveyApp/spDataService.js", "priority":5, "test": "false"}
            ]'>
        </script> 

    </div>
</asp:Content>
