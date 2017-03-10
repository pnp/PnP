# Retrieve more items than Threshold limit with CSOM #

### Summary ###
This sample shows a **ContentIterator** implementation that can be used to query large lists as it reads items in batches.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Solution ###
Solution | Author(s)
---------|----------
Core.ListViewThreshold | Anil Lakhagoudar

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 7th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
In SharePoint, when you execute query on Large List, you will receive "The attempted operation is prohibited because it exceeds the list view threshold enforced by the administrator". To avoid this exception and read list items by batch.

The new Content Iterator class is implemented in CSOM like **ContentIterator** class which is available in Server Object Model. which can use CSOM to retrieve the items. Also CamlQuery class has been extended with the Methods which can be used to set the CamlQuery properties like SPQuery for Overriding the QueryThrottleMode to avoid the QueryThrottleException.

## How to Use? ##
### Using CamlQueryExtension methods ###

```C#
CamlQuery camlQuery = new CamlQuery();
            
//CamlQuery extension Methods for setting the query properties and query option for Threshold limit

//Set View Scope for the Query
camlQuery.SetViewAttribute(QueryScope.RecursiveAll);

//Set Viewfields as String array
//camlQuery.SetViewFields(new string[] { "ID", "Title"});

//Or Set the ViewFields xml
camlQuery.SetViewFields(@"&lt;FieldRef Name='ID'/&gt;&lt;FieldRef Name='Title'/&gt;");

//Override the QueryThrottle Mode for avoiding ListViewThreshold exception
camlQuery.SetQueryThrottleMode(QueryThrottleMode.Override);

//Set Query condition
camlQuery.SetQuery("&lt;Eq&gt;&lt;FieldRef Name='IndexedField' /&gt;&lt;Value Type='Text'&gt;value&lt;/Value&gt;&lt;/Eq&gt;");


//If Query has condition Indexed column should be used  and set OrderBy with indexed column
camlQuery.SetOrderByIndexField();

//Use OrderBy ID field if Query doesn't have condition
//camlQuery.SetOrderByIDField();

//Set RowLimit
camlQuery.SetQueryRowlimit(5000);
```

### Using Implemented ContentIterator Class with CSOM ###
#### ProcessListItems method ####

```C#
using (ClientContext context = new ClientContext("SiteUrl"))
{
   ContentIterator contentIterator = new ContentIterator(context);

   try
   {
     contentIterator.ProcessListItems("ListName", camlQuery,
     ProcessItems,
     delegate(ListItemCollection items, System.Exception ex)
     {
         return true;
     });
    catch (Exception ex)
    {
    }
}

//Delegate method
private static void ProcessItems(ListItemCollection items)
{
   //Process items collection
}

```

#### ProcessListItem method ####

```C#
using (ClientContext context = new ClientContext("SiteUrl"))
{
   ContentIterator contentIterator = new ContentIterator(context);

   try
   {
     contentIterator.ProcessListItem ("ListName", camlQuery,
     ProcessItem,
     delegate(ListItem item, System.Exception ex)
     {
         return true;
     });
    catch (Exception ex)
    {
    }
}

//Delegate method
private static void ProcessItem(ListItem item)
{
 //Process each item
}
```
<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ListViewThreshold" />