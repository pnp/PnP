<%@ Page language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<WebPartPages:AllowFraming ID="AllowFraming" runat="server" />

<html>
<head>
    <title>AppPart</title>

    <link href="../Content/bootstrap.css" rel="stylesheet" />
    <link href="../Content/App.css" rel="stylesheet" />

    <script src="../Scripts/jquery-1.9.1.min.js"></script>
    <script src="../Scripts/jquery.ba-bbq.min.js"></script>
    <script src="../Scripts/rsvp-latest.js"></script>
    <script src="../Scripts/angular.js"></script>
    <script src="/_layouts/15/MicrosoftAjax.js"></script>
    <script src="/_layouts/15/sp.runtime.js"></script>
    <script src="/_layouts/15/sp.js"></script>
    <script src="/_layouts/15/SP.RequestExecutor.js"></script>
    <script src="../Scripts/SP2013.js"></script>
    <script src="../App/common/EE.js"></script>
    <script src="../App/common/controllers/indexController.js"></script>
    <script src="../App/app.js"></script>

</head>
<body data-ng-app="EmbeddedEditingApp">

    <div class="container" data-ng-controller="IndexController">

        <div class="page-header">
            <h1>Embedded Editing Sample</h1>
            <p>This app part demonstrates how to implement an advanced editing features using inline controls.</p>
        </div>

        <div class="page-header">
            <h3>Querystring Properties</h3>
            <p>These are the properties/tokens passed into the iFrame by SharePoint</p>
        </div>
        <dl class="dl-horizontal">
            <dt data-ng-repeat-start="param in querystringProperties">{{param.key}}</dt>
            <dd data-ng-repeat-end>{{param.value}}</dd>
        </dl>

        <div class="page-header">
            <h3>Raw Configuration List Data for this App Part Instance</h3>
        </div>
        <pre>GET: {{listItemsEndpoint}}</pre>
        <p data-ng-show="isLoadingList">Loading List Data...</p>

        <div data-ng-hide="isLoadingList">
            <p>This is a table view of the data we have to work with about the existing configuration of the app part.</p>
            <table class="table table-striped table-hover" >
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>AppPartId</th>
                        <th>Type</th>
                        <th>Key</th>
                        <th>Value</th>
                    </tr>
                </thead>
                <tbody data-ng-hide="items.length == 0">
                    <tr data-ng-repeat="item in items">
                        <td>{{item.id}}</td>
                        <td>{{item.appPartId}}</td>
                        <td>{{item.type}}</td>
                        <td>{{item.key}}</td>
                        <td>{{item.value}}</td>
                    </tr>
                </tbody>
                <tbody data-ng-show="items.length == 0">
                    <tr>
                        <td colspan="5" class="text-center">No Existing Records for App Part Id: <strong>{{appPartId}}</strong></td>
                    </tr>
                </tbody>
            </table>

            <div class="page-header">
                <h3>App Part Instance settings based on defaults and data found in configuration list.</h3>
            </div>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Property</th>
                        <th>Default Value</th>
                        <th>Config Value</th>
                        <th>Observed Value</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="text-right">title</td>
                        <td>{{appPartDefaultProperties.title.value}}</td>
                        <td>{{appPartConfigProperties.title.value}}</td>
                        <td>{{appPartProperties.title.value}}</td>
                    </tr>
                    <tr>
                        <td class="text-right">rowLimit</td>
                        <td>{{appPartDefaultProperties.rowLimit.value}}</td>
                        <td>{{appPartConfigProperties.rowLimit.value}}</td>
                        <td>{{appPartProperties.rowLimit.value}}</td>
                    </tr>
                    <tr>
                        <td class="text-right">listGuid</td>
                        <td>{{appPartDefaultProperties.listGuid.value}}</td>
                        <td>{{appPartConfigProperties.listGuid.value}}</td>
                        <td>{{appPartProperties.listGuid.value}}</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="errors" data-ng-hide="errors.length == 0">
            <div class="page-header text-danger">
                <h2>Errors</h2>
            </div>
            <p class="error bg-danger" data-ng-repeat="error in errors">{{error.key}}: {{error.value}}</p>
        </div>
        <div data-ng-show="errors.length == 0">
            <div class="page-header">
                <h2>Host List Items</h2>
            </div>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Title</th>
                    </tr>
                </thead>
                <tbody data-ng-hide="hostListItems.length == 0">
                    <tr data-ng-repeat="item in hostListItems">
                        <td>{{item.id}}</td>
                        <td>{{item.title}}</td>
                    </tr>
                </tbody>
                <tbody data-ng-show="hostListItems.length == 0">
                    <tr>
                        <td colspan="2" class="text-center">No Data in the Host List: <strong>{{appPartProperties.listGuid}}</strong></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div data-ng-show="isEditModeEnabled && areAppPartPropertiesCalculated">
            <div class="page-header">
                <h1>Edit Form</h1>
                <p>Users can edit this form and save configuration data as if they were editing properties in the OOTB App Part editing control; however, we have full control over the form.</p>
            </div>
            <div style="display: none;">
                <p>Title: {{editableAppPartProperties.title.value}}</p>
                <p>RowLimit: {{editableAppPartProperties.rowLimit.value}}</p>
                <p>Selected List: {{editableAppPartProperties.selectedList.title.value}}</p>
                <ul>
                    <li data-ng-repeat="list in hostLists">{{list.title}}</li>
                </ul>
            </div>
            <form class="form-horizontal" data-ng-hide="isLoadingList" role="form" data-ng-submit="submitEditForm()">
                <div class="form-group">
                    <label for="appPartTitle" class="col-sm-2 control-label">Title</label>
                    <div class="col-sm-10">
                        <input type="text" required class="form-control" id="appPartTitle" data-ng-model="editableAppPartProperties.title.value" placeholder="Enter custom app part title.">
                    </div>
                </div>
                <div class="form-group">
                    <label for="appPartRowLimit" class="col-sm-2 control-label">Row Limit</label>
                    <div class="col-sm-10">
                        <input type="number" required min="0" max="100" step="1" class="form-control" id="appPartRowLimit"  data-ng-model="editableAppPartProperties.rowLimit.value">
                    </div>
                </div>
                <div class="form-group">
                    <label for="appPartListGuid" class="col-sm-2 control-label">Host Lists Compatible With App Part</label>
                    <div class="col-sm-10">
                        <select class="form-control" required data-ng-model="editableAppPartProperties.selectedList" data-ng-options="list.title for list in hostLists">
                            <option value="">Select a host list</option>
                        </select>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-2 col-sm-10">
                        <button type="submit" class="btn btn-primary">Save</button>
                    </div>
                </div>
            </form>
        </div>

    </div>

</body>
</html>
