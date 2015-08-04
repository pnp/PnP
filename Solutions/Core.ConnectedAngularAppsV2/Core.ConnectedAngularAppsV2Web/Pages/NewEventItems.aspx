<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewEventItems.aspx.cs" Inherits="Core.ConnectedAngularAppsV2Web.Pages.NewEventItems" %>

<!DOCTYPE html>

<html ng-app="app">
<head>
    <title></title>
     <!--Script references. -->           
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
    <div id="eventPanel" class="panel" ng-controller="eventMgmtController as vm">

        <h3 class="panel-header">
            Create a new event           
        </h3>

        <div class="row">
            <div class="col-xs-12">
                <div class="well">
                    <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionText">Title:</label><br />
                        <input id="actionTitle" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 300px; height: 20px;" class="form-control"
                               ng-model="newEventItem.title">
                    </div>
                    <div class="form-group row"> 
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionDate">Event Date: {mm/dd/yyyy]}</label><br />
                        <input id="actionDate" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; width: 125px; height: 20px;" class="form-control"
               ng-model="newEventItem.eventdate">
                        </div>
                    <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionDescription">Event Description:</label><br />
                        <textarea id="actionDescription" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 400px; height: 75px;"   class="form-control"
                               ng-model="newEventItem.eventdesc" multiple="multiple"></textarea>
                    </div>
                    <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionLocation">Event Location:</label><br />
                        <input id="actionLocation" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 400px; height: 20px;"   class="form-control"
                               ng-model="newEventItem.eventlocation"></input>
                    </div>
                    <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionContact">Event Contact:</label><br />
                        <input id="actionContact" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 300px; height: 20px;" class="form-control"
                               ng-model="newEventItem.contact">
                    </div>
                    <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionID">Event ID:</label><br />
                        <input id="actionID" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 200px; height: 20px;" class="form-control"
                               ng-model="newEventItem.eventid">
                    </div>
                     <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionImageUrl">Event Image Url:</label><br />
                        <input id="actionImageUrl" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 400px; height: 20px;" class="form-control"
                               ng-model="newEventItem.eventimageurl">
                    </div>
                    <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionCategory">Event Category:</label><br />
                        <select id="actionCategory" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 200px; height: 20px;" class="form-control"
                                ng-model="newEventItem.category">
                            <option>General</option>
                            <option>Leadership</option>
                            <option>Technical</option>
                            <option>Featured</option>
                        </select>
                    </div>
                    <div class="form-group row">
                        <label style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px;" for="actionLocation">Event Status:</label><br />
                        <select id="actionStatus" style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:12px; font-size:12px; width: 200px; height: 20px;" class="form-control"
                                ng-model="newEventItem.status">
                            <option>Active</option>
                            <option>Cancelled</option>
                            <option>Expired</option>
                        </select>
                    </div>
                    <br />
                    <div class="form-group row">
                        <button style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size:x-small;"
                                ng-click="vm.addNew(newEventItem)">
                            Add Event
                        </button>
                    </div>                
            </div>
            <br />
           <%-- <div class="col-xs-6" >
                <table class="table">
                    <thead>
                        <tr><th>ID</th><th>Event</th></tr>
                    </thead>
                    <tr ng-repeat="item in vm.addedEvent" ng-model="vm.addedEvent">
                        <td>{{$index + 1}}</td>
                        <td>{{item.title}}</td>                                                 
                    </tr>
                </table>
            </div>--%>
        </div>
    </div>
    <br />
    <br />
    <div class="container" style="width:100%">
        <dl class="dl-horizontal" id="sigRMessages" style="width:100%">
            

        </dl>
    </div>    
    
    <!-- Bootstrapping -->
    <script type="text/javascript" src="../Scripts/app.module.js"></script>
    <script type="text/javascript" src="../Scripts/ui-bootstrap-tpls-0.13.0.min.js"></script>
    
    <!-- Core modules -->
    <script type="text/javascript" src="../Scripts/core/core.module.js"></script>
    <script type="text/javascript" src="../Scripts/core/constants.js"></script>
    <script type="text/javascript" src="../Scripts/core/config.js"></script>    
    <script type="text/javascript" src="../Scripts/core/signalRservice.js"></script>
    <script type="text/javascript" src="../Scripts/core/dataservice.js"></script>
    <script type="text/javascript" src="../Scripts/core/comms.module.js"></script>    
    <script type="text/javascript" src="../Scripts/core/comms.js"></script>
    <!-- Events and Sessions modules -->
    <script type="text/javascript" src="../Scripts/management/addevents.module.js"></script>    
    <script type="text/javascript" src="../Scripts/management/addevents.js"></script>
    <script type="text/javascript" src="../Scripts/events/events.module.js"></script>    
    <script type="text/javascript" src="../Scripts/events/events.js"></script>
    <script type="text/javascript" src="../Scripts/sessions/sessions.module.js"></script>    
    <script type="text/javascript" src="../Scripts/sessions/sessions.js"></script>
    <script type="text/javascript" src="../Scripts/speakers/speakers.module.js"></script>    
    <script type="text/javascript" src="../Scripts/speakers/speakers.js"></script>  
    
    
    <!--Reference the SignalR library. -->
    <script src="../Scripts/jquery.signalR-2.2.0.min.js"></script>
    <!--Reference the utility-generated SignalR hub script.     
    <script src="../signalr/hubs"></script>-->
    <script src="../Scripts/signalr-server.js"></script>

</body>
</html>
