# SharePoint List Item Change Monitor #

### Summary ###
Employ the **ChangeQuery** object to monitor list item changes.
- Accept Url, List name, User name and password arguments.
- Check every 30 seconds to see if changes have occurred.

### Applies to ###
- Office 365 Multi Tenant (MT)
- Office 365 Dedicated (D)
- SharePoint 2013 on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
Core.ListItemChangeMonitor | Daniel Budimir & Phil Cohen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 20th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
This sample shows how to use the **ChangeQuery** object to monitor a list for Add,Update and Delete to list items.  This pattern can be used as a fail-safe along with **Remote Event Receivers(RER's)** to ensure that necessary processing takes place if the RER does not fire.  An RER's firing is not guaranteed because it is typically not located on the server where SharePoint is running.  At it's core, an RER is a REST web service, upon a change or add to a list SharePoint attempts to call the RER, if for one reason or another the web service is not available the call fails and is not repeated.

## Change Log details ##
Accessing the SharePoint change log is accomplished using the GetChanges() method on the primary objects: 
- Site.GetChanges 
- Web.GetChanges 
- List.GetChanges 

The methods return a collection of objects that are descendants of the object on which the call is made. For example, calling GetChanges on a list will return changes made to the list and to items (and folders and files) in that list, but will not return changes made to other lists in the same web or to the site collection-based galleries. 

Each of these methods has a parameter of type ChangeQuery, and this type provides insight into the capabilities of the Change Log that far surpasses the MSDN reference. 

### ChangeQuery properties ###

The properties of the ChangeQuery object can be separated into two general categories - change actions and objects changed. 

Change Action     
- Add     
- DeleteObject     
- GroupMembershipAdd     
- GroupMembershipDelete     
- Move     
- Rename 
- Restore 
- RoleAssignmentAdd 
- RoleAssignmentDelete 
- RoleDefinitionAdd 
- RoleDefinitionDelete 
- RoleDefinitionUpdate 
- SystemUpdate 
- Update 

Objects Changed 
- Alert 
- ContentType 
- Field 
- File 
- Folder 
- Group 
- Item 
- List 
- Navigation 
- SecurityPolicy 
- Site 
- User 
- View 
- Web 

(Note that not all actions apply to all object types.) 

The GetChanges methods return a ChangeCollection. The items in the collection all inherit from the Change class. This inheritance hierarchy is crucial to understanding the items returned by the query. 
To process the change using its core type, cast the Change item to the appropriate inheriting type (ChangeWeb, ChangeList, etc.). This core type will provide properties that can be used to retrieve the source object.
However, be prepared for the object to no longer exist.

```cs
foreach (ChangeItem itm in coll)
{
  if (change is Microsoft.SharePoint.Client.ChangeItem)
  {
    ChangeItem ci = change as ChangeItem;
    
    // the ci variable will have the Web Id, List Id and List Item Id 
    // necessary to read the object.
    
    // Be sure to test for object existence 
  }
}
```


# To use this sample #
1. Open the .sln file for the sample in **Visual Studio**.
2. You can now run the sample with F5.
3. You are prompted to enter Url, List name, User name and Password arguments.
4. The console window will be updated every 30 seconds and will display the changes made to the list since the last check, if no changes were made a message a "No changes" message will be displayed.
5. While the monitor is running you may also press the "r" key to override the timer and force a check.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ListItemChangeMonitor" />