<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Provisioning.UX.AppWeb.Default" Async="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" data-ng-app="app">
<head>
    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title></title>

    <!-- CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.css"/>
    <link rel="stylesheet" href="../styles/app.css" /> 
    <link rel="stylesheet" href="../styles/peoplepickercontrol.css" />    
    
    <!-- AngularJS -->
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.0-beta.6/angular.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.0-beta.6/angular-messages.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.0-beta.6/angular-animate.min.js"></script>    

    <!-- Vendor JS -->
    <script src="../scripts/vendor/jquery/jquery-2.1.3.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js"></script>
    <script src="../scripts/vendor/angular-ui/ui-bootstrap-tpls-0.12.1.min.js"></script>  
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>

    <!-- People Picker from PnP -->
    <script src="../scripts/peoplepickercontrol.js?rev=2" type="text/javascript"></script>   

    
</head>
<body>    
    <div id="divSPChrome"></div>            
        
    <!-- Include the Wizard View -->
    <div data-ng-include="'wizard.html'"></div>

          
    <!-- App JS -->
             
    <script src="../scripts/app.module.js"></script>    
    <script src="../scripts/wizard/modules/wizard.module.js"></script>    
    <script src="../scripts/wizard/services/utilservice.js"></script>
    <script src="../scripts/wizard/services/peoplepickerfactory.js"></script>
    <script src="../scripts/wizard/controllers/wizard.controller.js"></script>
    <script src="../scripts/wizard/services/siteQueryService.js"></script>    
    <script src="../scripts/wizard/directives/restrict.js"></script>    
    <script src="../scripts/wizard/directives/formDirectives.js"></script>    
    <script src="../scripts/wizard/modal/wizard.modal.controller.js"></script>
    <script src="../scripts/wizard/modal/siteowners.peoplepicker.controller.js"></script>    
    <script src="../scripts/data/data.module.js"></script>
    <script src="../scripts/data/templates.factory.js"></script>
    <script src="../scripts/data/metadata.factory.js"></script>
    <script src="../scripts/data/appsettings.factory.js"></script>
    <script src="../scripts/wizard/services/provisioningServices.js"></script>

    
    <script src="../scripts/app.js"></script>    
    
</body>
</html>
