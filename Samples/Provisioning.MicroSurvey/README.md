# MicroSurvey Web Part #

This is a "microsurvey" web part, which will display a single question and
gather up the answers. It's an example of how to write a SharePoint app using AngularJS that
runs completely in the browser and can be deployed with or without the App model.

There are three ways to deploy this app!

 1. __As a SharePoint Hosted App:__ If you simply install the .app file, or let Visual Studio deploy the app to a Dev#0 site.
    Manage the app in the full page view; use the included App Part to place it on the page.

 2. __As a Drag and Drop App:__ Simply copy the contents of the SurveyApp folder to SiteAssets/SurveyApp/ within any SharePoint web.
    Manage the app by visiting the SiteAssets/SurveyApp/ folder in a web browser (click the Default.aspx file if needed.)
    Place a Content Editor Web Part on any page pointing to SiteAssets/SurveyApp/webPart.html to use the app.

 3. __As a centrally deployed app:__ Create a simple IIS site with no ASP.NET and use the web.config sample found in the SurveyAppCentralDeploy
    folder. Copy the SurveyApp folder to this site's folder under c:\wwwroot. Now copy the Default.aspx and WebPart.html files found in
    SurveyAppCentralDeploy to the web where you wish to use them.

Additional Features: 

* You can have more than one copy of the web part on a page. The trick to this is bootstrapping angular to
  from within a div that encloses the web part so no div ID or other unique attribute is required.
  See http://stackoverflow.com/questions/6932679/get-dom-element-where-script-tag-is

* The solution coexists with other Angular stuff on the page by saving away the angular object, loading
  a new one, binding the app, and then restoring the original angular object (if any)
  See http://stackoverflow.com/questions/19537960/multiple-versions-of-angularjs-in-one-page

* PowerShell deployment

* Clever URL Parsing from: https://saikiran78.wordpress.com/2014/01/17/getting-list-data-in-sharepoint-2013-using-rest-api-and-angular-js/

Notes on form URL's

DISPLAY
?List=69d0b4bc%2D5d7f%2D45b7%2D8116%2Defaa1e07eb32
&ID=1
&Source=(url)
&ContentTypeId=0x0100A8B936F1B9339C42B498D9EA7DA912A7
&RootFolder=%2FMicroSurveySPApp%2FLists%2FQuestions

EDIT
?List=69d0b4bc%2D5d7f%2D45b7%2D8116%2Defaa1e07eb32
&ID=1
&Source=(url)
&ContentTypeId=0x0100A8B936F1B9339C42B498D9EA7DA912A7

NEW
?List=69d0b4bc%2D5d7f%2D45b7%2D8116%2Defaa1e07eb32
&Source=(url)
&RootFolder=
&Web=7daf31b0%2Df3c1%2D434c%2D8b12%2D37d007da0955

Ask a Question Link (New)
?List=8fe9f4a1%2D5c25%2D4440%2Dbd34%2Dcbd3b8547943
&Source=(url)
&SPLanguage=en%2DUS
&SPClientTag=0
&SPProductNumber=15%2E0%2E4420%2E1017
&SPAppWebUrl=(url)
&Web=a77b5fb9%2Dd9e2%2D4bff%2Dba48%2D827c6438a9fc



            <td>
                <div style="width: 400px;" ng-controller="listNewForm" ng-include="'listForm.html'"></div>
            </td>



