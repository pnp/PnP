# SharePoint List Manager #

### Summary ###
This sample shows how to access list information in a SharePoint website from your add-in by using the cross domain library in SharePoint 2013.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
Core.SPListManager | Tom Van Gaever (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 5th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO 1: View the lists on the SharePoint web and view every property #
This scenario shows how to review the properties of the lists in a SharePoint website. The add-in retrieves the information of the host web by using the cross domain library and the REST API. 

It is very easy to review and learn the actual values of the available properties on the lists.

![Site Contents](http://i.imgur.com/QCOeti3.png)

Simply click on the icon representing the list or library you would like to investigate
![List Viewer Properties](http://i.imgur.com/chzEVzt.png)

![List Viewer Content Types](http://i.imgur.com/oB2rfK3.png)

## Using the cross domain library to retrieve information from the SharePoint web ##
All used code has been encapsulated by SPListmanager, which exposes all the functionality for each specific page.

```JavaScript
var executor = new SP.RequestExecutor(SPListmanager.appweburl);
executor.executeAsync(
    {
        url: SPListmanager.appweburl + "/_api/SP.AppContextSite(@target)/web/lists?@target='" + SPListmanager.hostweburl + "'",
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: SPListmanager.Default.initSuccessHandler,
        error: function (data) {
            alert(jQuery.parseJSON(data.body).error.message.value); 
        }
    }
);
initSuccessHandler: function ($result) {
        var data = jQuery.parseJSON($result.body);
        SPListmanager.Default.lists = data.d.results;
}
```

# SCENARIO 2: Create a new list in the SharePoint web via the add-in #
This scenario further extends scenario 1 by creating a new list in the host web by using the cross domain library. In this case we are providing a custom form that is 100% under our control that will create a new list based on the input of the user.

![Create new list](http://i.imgur.com/092By5r.png)

![Create new list form](http://i.imgur.com/0BLKqID.png)

Here’s how the new created list will be available on the SharePoint host web.

![Newly created list](http://i.imgur.com/fhAzOYI.png)

## Using the cross domain library to a new list on the SharePoint web ##
```JavaScript
SPListmanager.NewList = {
    init: function () {
        console.info('init started');

        //provision create list logic
        $("#btnCreateList").click(function () {

            $(".s4-bodypadding :input").attr("disabled", true);

            console.info('btnCreateList clicked');

            // Get the new name of the list from the textbox
            var newListName = $("#txtNewListTitle").val();
            var description = $("#txtNewListDescription").val();
            var listTemplateType = $("#cmbNewListTemplate").val();

            console.info('btnCreateList action with params ' + newListName + "|" + description + "|" + listTemplateType);

            // get the context from the hostweb where the add-in is installed
            var hostwebContext = new SP.AppContextSite(SPListmanager.context, SPListmanager.hostweburl);
            var web = hostwebContext.get_web();

            // create the listinstance of the new SPList
            var listCreationInfo = new SP.ListCreationInformation();
            listCreationInfo.set_title(newListName); // list name
            listCreationInfo.set_description(description); // list description
            listCreationInfo.set_templateType(listTemplateType); //list type

            // add the listinstance to the lists on the parentweb
            var list = web.get_lists().add(listCreationInfo);
            SPListmanager.context.load(list);

            // execute the action
            SPListmanager.context.executeQueryAsync(onQuerySucceeded, onQueryFailed);
        });
};

```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SPListManager" />