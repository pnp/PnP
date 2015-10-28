<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignalRComms.aspx.cs" Inherits="Core.ConnectedAngularAppsV2Web.Pages.SignalRComms" %>

<!DOCTYPE html>

<html ng-app="app">
<head>
    <title></title>
     <!--Script references. -->    
    <script type="text/javascript" src="../Scripts/angular.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-2.1.1.min.js" ></script>        
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js"></script>
   <%-- <script type="text/javascript" src="../Scripts/angular.min.js"></script>  --%>  
    <%--<script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>    
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>        
    <script type="text/javascript" src="/_layouts/15/sp.requestexecutor.js"></script> --%>   
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
    <div class="dl-horizontal" ng-controller="communicationsController as vm" style="width:100%">                
        This app part show messages being broadcast via SignalR.
    </div>
    <br />
    <br />
    <div class="container" style="width:100%">
        <dl class="dl-horizontal" id="sigRMessages" style="width:100%">
            

        </dl>
    </div>    
    
    <!-- Bootstrapping -->
    <script type="text/javascript" src="../Scripts/app.module.js"></script>
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
    <script type="text/javascript" src="../Scripts/core/userContextService.js"></script>    
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
    <script type="text/javascript" src="../Scripts/management/addevents.module.js"></script>    
    <script type="text/javascript" src="../Scripts/management/addevents.js"></script>
    
    <!--Reference the SignalR library. -->
    <script src="../Scripts/jquery.signalR-2.2.0.min.js"></script>
    <!--Reference the utility-generated SignalR hub script.     
    <script src="../signalr/hubs"></script>-->
    <script src="../Scripts/signalr-server.js"></script>

</body>
</html>
