Retrieve more items than Threshold limit with CSOM
----------------------------------------------------------
**Summary**
<br><br>
In SharePoint, when you execute query on Large List, you will receive "The attempted operation is prohibited because it exceeds the list view threshold enforced by the administrator". To avoid this exception and read list items by batch.
The new Content Iterator class is implemented in CSOM like **ContentIterator** class which is available in Server Object Model. which can use CSOM to retrieve the items. Also CamlQuery class has been extended with the Methods
which can be used to set the CamlQuery properties like SPQuery for Overriding the QueryThrottleMode to avoid the QueryThrottleException.
<br><br>
**Solution**
<br>
Core.ListViewThreshold
<br>
<br>
How to Use?
-------------------------
<br>
<br>
*Using CamlQueryExtension methods*
<pre>
<code>
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
</cod>
</pre>
------------------------
*Using Implemented ContentIterator Class with CSOM*
<pre>
<code>

a)	ProcessListItems method

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



b)	ProcessListItem method

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

</code>
</pre>
