<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Speakers.aspx.cs" Inherits="Core.ConnectedAngularAppsV2Web.Pages.Speakers" %>

<!DOCTYPE html>

<html ng-app="app">
<head>
    <title></title>
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap-theme.min.css">
    <script src="//ajax.googleapis.com/ajax/libs/angularjs/1.3.13/angular.js"></script>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
    <%--<script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>    
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>        
    <script type="text/javascript" src="/_layouts/15/sp.requestexecutor.js"></script>--%>

    <script type="text/javascript">
        // Set the style of the client web part page to be consistent with the host web.
        (function () {
            'use strict';

            var hostUrl = '';
            if (document.URL.indexOf('?') != -1) {
                var params = document.URL.split('?')[1].split('&');
                for (var i = 0; i < params.length; i++) {
                    var p = decodeURIComponent(params[i]);
                    if (/^SPHostUrl=/i.test(p)) {
                        hostUrl = p.split('=')[1];
                        document.write('<link rel="stylesheet" href="' + hostUrl + '/_layouts/15/defaultcss.ashx" />');
                        break;
                    }
                }
            }
            if (hostUrl == '') {
                document.write('<link rel="stylesheet" href="/_layouts/15/1033/styles/themable/corev15.css" />');
            }
        })();
    </script>

     <style type="text/css">
        .featured-event-item { width:200px; float:left; padding:10px; }
        .dl-horizontal dt {
            font-weight: normal;
            font-family: 'Segoe UI';
            font-size: 10px;
            float: left;
            width: 500px;
            overflow: hidden;
            clear: left;
            text-align: left;
            text-overflow: ellipsis;
            white-space: nowrap;
          }
          .dl-horizontal dd {
            margin-left: 180px;
          }
    </style>

</head>
<body>
    <div class="container" ng-controller="speakersController as vm" style="width:100%">
        <div class="table-responsive">            
            <table class="table table-striped">
              <thead>          
              </thead>
              <tbody>
              <tr ng-repeat="e in vm.speakers" >                
                <td style="font-size: small; font-weight: normal; font-style: normal; vertical-align: middle; text-align: center;">{{e.speakerfirstname}}</td>
                <td style="font-size: small; font-weight: normal; font-style: normal; vertical-align: middle; text-align: center;">{{e.speakerlastname}}</td>
                <td style="font-size: small; font-weight: normal; font-style: normal; vertical-align: middle; text-align: center;">{{e.speakeremail}}</td>                                                
              </tr>
              </tbody>
           </table>
       </div> 
    
        
    </div>
     
   
    <br />
    <br />
      

    <!-- Bootstrapping -->
    <script type="text/javascript" src="../Scripts/app.module.js"></script>
    <script type="text/javascript" src="../Scripts/ui-bootstrap-tpls-0.13.0.min.js"></script>

    <!-- Reusable blocks/modules -->
    <!--<script type="text/javascript" src="../Scripts/blocks/exception/exception.module.js"></script>
    <script type="text/javascript" src="../Scripts/blocks/exception/exception-handler.provider.js"></script>
    <script type="text/javascript" src="../Scripts/blocks/exception/exception.js"></script>
    <script type="text/javascript" src="../Scripts/blocks/logger/logger.module.js"></script>
    <script type="text/javascript" src="../Scripts/blocks/logger/logger.js"></script>-->

    <!-- Core modules -->
    <script type="text/javascript" src="../Scripts/core/core.module.js"></script>
    <script type="text/javascript" src="../Scripts/core/constants.js"></script>
    <script type="text/javascript" src="../Scripts/core/config.js"></script> 
    <script type="text/javascript" src="../Scripts/core/usercontextservice.js"></script>   
    <script type="text/javascript" src="../Scripts/core/signalRservice.js"></script>
    <script type="text/javascript" src="../Scripts/core/dataservice.js"></script>         
    <script type="text/javascript" src="../Scripts/core/comms.module.js"></script>    
    <script type="text/javascript" src="../Scripts/core/comms.js"></script>

    <!-- Events and Sessions modules -->
    <script type="text/javascript" src="../Scripts/events/events.module.js"></script>    
    <script type="text/javascript" src="../Scripts/events/events.js"></script>
    <script type="text/javascript" src="../Scripts/sessions/sessions.module.js"></script>    
    <script type="text/javascript" src="../Scripts/sessions/sessions.js"></script>
    <script type="text/javascript" src="../Scripts/speakers/speakers.module.js"></script>    
    <script type="text/javascript" src="../Scripts/speakers/speakers.js"></script>
    <%--<script type="text/javascript" src="../Scripts/management/addevents.module.js"></script>    
    <script type="text/javascript" src="../Scripts/management/addevents.js"></script>--%>
    
    
    <!--Reference the SignalR library. -->
    <script src="../Scripts/jquery.signalR-2.2.0.min.js"></script>
    <!--Reference the utility-generated SignalR hub script.     
    <script src="../signalr/hubs"></script>-->
    <script src="../Scripts/signalr-server.js"></script>    

</body>
</html>
