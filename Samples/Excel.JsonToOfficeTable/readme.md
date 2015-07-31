# Using Apps for Excel for Custom Data Access and Reporting #

### Summary ###
Microsoft Excel is known for being the #1 reporting tool in the world. Regardless of what format insights are delivered in, users often desire the ability to export and work with the data in Excel. For that reason, Excel plug-ins are incredibly popular for custom and proprietary data access. Unfortunately, traditional plug-ins often cripple a user’s ability to upgrade Office. PowerPivot and Power Query have made it easy to connect to a variety of data sources without custom plug-ins. However, many data sources might be too complex for an end-user to understand and others aren’t supported at all. For these scenarios,  Excel-based Apps for Office can help simplify sources and get data into Excel. This sample contains patterns for using Apps for Office to accomplish complex data access and reporting tasks. Specifically, it provides patterns for generically converting JSON into an Office.TableData object that can popular Excel.

For a more thorough overview of the solution, see the blog post: [http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2014/09/08/using-apps-for-excel-for-custom-data-access-and-reporting.aspx](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2014/09/08/using-apps-for-excel-for-custom-data-access-and-reporting.aspx)

### Walkthrough Video ###

Comprehensive video of the samples in action:
[https://www.youtube.com/watch?v=kRmkdCqtwts](https://www.youtube.com/watch?v=kRmkdCqtwts)

Example of packaging Apps for Office with pre-configured Excel templates:
[https://www.youtube.com/watch?v=OlP3hd3XEqo](https://www.youtube.com/watch?v=OlP3hd3XEqo)

### Applies to ###
-  Office Client (Excel)
-  Office Online (Excel Online)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Excel.JsonToOfficeTable | Richard diZerega (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 8th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Office.TableData Extensions #
This sample adds two extension methods to the **Office.TableData** object, which is used to read/write/bind table data in Excel-based Apps for Office.

The **addHeaders** extension method initializes the table headers based on an object passed in. This object will typically be the table’s first row of data. The method will loop through and add headers/columns for each property of the object, ignoring complex data types (ex: typeof == object). Complex data types are ignored as they could represent one:many relationships between tables. This would be an interesting enhancement for a v2, but for now I’m keeping it simple with a single table.

```JavaScript
//extension to Office.TableData to add headers
Office.TableData.prototype.addHeaders = function (obj) {
    var h = new Array();
    for (var prop in obj) {
        //ignore complex types empty columns and __type from WCF
        if (typeof (obj[prop]) != 'object' &&
            prop.trim().length > 0 &&
            prop != '__type')
            h.push(prop);
    }
    this.headers = h;
}
```

The **addRange** extension method appends rows to the Office.TableData based on an array of data passed in. This method was specifically designed to support multiple appends, as would be common with throttled/paged results. The addRange method only looks at object properties that are defined as headers in the TableData object. As such, the headers should be set (manually or via addHeaders) prior to calling addRange.

```JavaScript
//extension to Office.TableData to add a range of rows
Office.TableData.prototype.addRange = function (array) {
    for (i = 0; i < array.length; i++) {
        var itemsTemp = new Array();
        $(this.headers[0]).each(function (ii, ee) {
            itemsTemp.push(array[i][ee]);
        });
        this.rows.push(itemsTemp);
    }
}
```

# Client-side Data Access #
Because the extensions methods use JSON, client-side processing from a REST call is very simple:

1. Intialize the Office.TableData object
2. Add headers/columns based on the first row of data
3. Add the rows of data to the TableData object
4. Inject the TableData into Excal workbook as a Table Coercion Type

Below is a code sample that demonstrates these four steps. The data variable is the JSON returned from a REST call:

```JavaScript
//initalize the Office.TableData and load headers/rows from data
var officeTable = new Office.TableData();
//add columns to table based on first row of data
officeTable.addHeaders(data.d[0]);
//add rows to table
officeTable.addRange(data.d);
//inject the Office.TableData in the Excel workbook
setExcelData(officeTable);
```

# Server-side Data Access #
The Web Extensibility Framework (WEF) that enables Apps for Office only provides client-side APIs for interaction with the Office document. The strategy for server-side data access will be the following:

1. Access the data using anything available to .NET
2. Serialize the data as a JSON string
3. Inject the JSON as script on the page
4. Add client-side script to check for JSON as the add-in loads
5. Process the JSON using the same client-side methods above

The code below shows a server-side button click event that retrieves data, serializes the data as a JSON string, and injects the JSON into the page using the Page's  ClientScriptManager.

```C#
protected void btnSubmit2_Click(object sender, EventArgs e)
{
    //use the stock service to get the history
    //although this samples a local service...
    //ANY data access .NET supports could be used
    Services.Stocks s = new Services.Stocks();
    var history = s.GetHistory(txtSymbol2.Text, Convert.ToInt32(cboFromYear2.SelectedValue));
    using (MemoryStream stream = new MemoryStream())
    {
        //serialize the List<StockStats> to a JSON string
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Services.StockStat>));
        ser.WriteObject(stream, history);
        stream.Position = 0;
        StreamReader sr = new StreamReader(stream);
        var json = sr.ReadToEnd();

        //output the json string of stock history as javascript on the page so script can read and process it
        Page.ClientScript.RegisterStartupScript(typeof(Default), "JSONData", String.Format("var jsonData = {0};", json), true);
    }
}
```
