# Customization via JavaScript #

### Summary ###
This sample shows how one can use JavaScript to update SharePoint sites. This technique is a valuable model to do small UI customizations like removing UI elements, translating content or updating complex UI elements.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.JavaScriptCustomization | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.0  | November 7th 2013 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# Important: #
The below type of simple to complex JavaScript customizations are very depending on the actual implementation of SharePoint. If Microsoft decides to change the implementation (e.g. other control names) then your code might break. Only use this technique when there's no CSOM equivalent and when there's a clear business need.

# SCENARIO 1: BASIC JAVASCRIPT CUSTOMIZATION #
This sample shows how one can use JavaScript to perform basic updates to a SharePoint site. The later scenarios built further on the basis created in this sample. 

## LOADING YOUR SCRIPTS ##
SharePoint team sites by default make use of the Minimal Download Strategy (MDS) technique to improve performance. If we want to load custom JavaScript files we have to take this in account by loading the scripts via the below pattern:

```JavaScript
// Register script for MDS if possible
RegisterModuleInit("scenario1.js", RemoteManager_Inject); //MDS registration
RemoteManager_Inject(); //non MDS run

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("scenario1.js");
}
```

When the page that contains your script is loaded either the MDS engine (when MDS is enabled) launches your main function (RemoteManager_Inject) or your function is launched directly for non MDS invocations. The function that’s called is your entry point to load other scripts and to perform the required customizations. Loading other scripts often is needed: the sample shows how you can load the popular jQuery library. When loading other scripts it’s important that the script parts that depend on the loaded script are only executed after the other script was loaded and this is guaranteed via the below construct:

```JavaScript
var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

// load jQuery and if complete load the js resource file
loadScript(jQuery, function () {
    // your customizations go here
});
```

## INSERTING YOUR JAVASCRIPT HOOK INTO SHAREPOINT SITES ##
The above descript JavaScript file needs to be loaded by SharePoint. A typical approach would be customizing the master page, but we’ve opted to show a less intrusive approach based on SharePoint custom actions. In the samples you’ll find the below in the code behind of the scenario web pages:

```C#
//insert a hook
cc.Web.AddJsLink(Utilities.Scenario1Key, Utilities.BuildScenarioJavaScriptUrl(Utilities.Scenario1Key, this.Request));

//remove the hook
cc.Web.DeleteJsLink(Utilities.Scenario1Key);
```

If we look at the above in more detail (in OfficeDevPnP Core) you’ll see that in method public bool AddJsLink(string key, string scriptLinks) a block of JavaScript is constructed that will load out JavaScript customization file. Finally this block of JavaScript is used as ScriptBlock in a custom SharePoint action that’s defined via public bool AddCustomAction(CustomActionEntity customAction). 

# SCENARIO 2: TRANSLATING VIA JAVASCRIPT #
Scenario 2 introduces the concept of JavaScript based resource files. Today’s there’s no possibility to use custom server resource files in the Cloud Application Model, so JavaScript customization is a possible way to mitigate this. The shown technique is applicable for basic translations and we would not advise to use this concept to translate huge amounts of UI elements because the translation will happen at runtime which means the users might see the translation “happen” if there’s a lot of content to translate. To perform some limited translations in navigation elements or page titles this is however a good approach.

## LOADING THE JAVASCRIPT BASED RESOURCE FILES ##
The same load script approach as described in scenario 1 is also used to load the resource files and wait with using them until they’re loaded. Additionally there's some code that makes it possible to dynamically load the correct resource file: the whole system depends on having only the correct resource file loaded as the resource files are just a collection of global variables holding translated strings. Below code shows how to extract the used URL of the customization JavaScript file and the potential URL arguments (e.g. rev=8989) to use them for loading the resource file. This assumes the resources files are stored in the same location as your JavaScript customization file. The language to be used is fetched from the _spPageContextInfo object.

```JavaScript
var scriptUrl = "";
var scriptRevision = "";
// iterate the loaded scripts to find the scenario2 script. 
// We use the script URL to dynamically build the url for the resource file 
// to be loaded.
$('script').each(function (i, el) {
  if (el.src.toLowerCase().indexOf('scenario2.js') > -1) {
    scriptUrl = el.src;
    scriptRevision = scriptUrl.substring(scriptUrl.indexOf('.js') + 3);
    scriptUrl = scriptUrl.substring(0, scriptUrl.indexOf('.js'));
  }
})

var resourcesFile = scriptUrl + "." + _spPageContextInfo.currentUICultureName.toLowerCase() + ".js" + scriptRevision;
// load the JS resource file based on the user's language
loadScript(resourcesFile, function () {
    // your customizations go here
});
```

## APPLYING THE TRANSLATIONS ##
To apply a translation is actually nothing more than replacing the text (and potentially also the title attribute) of the element you want to translate by the appropriate variable coming from the JavaScript resource file. Below sample shows this:

```JavaScript
// Note that you can use the jQuery each function to iterate all elements 
// that match your jQuery selector.
$("span.ms-navedit-flyoutArrow").each(function () {
  if (this.innerText.toLowerCase().indexOf('my quicklaunch entry') > -1) {
    //update the label
    $(this).find('.menu-item-text').text(quickLauch_Scenario2);
    //update the tooltip
    $(this).parent().attr("title", quickLauch_Scenario2);
  } 
});

// Change the title of the "Hello SharePoint" page
if (IsOnPage("Hello%20SharePoint.aspx")) {
  $("#DeltaPlaceHolderPageTitleInTitleArea").find("A").each(function () {
    if ($(this).text().toLowerCase().indexOf("hello sharepoint") > -1) {
      //update the label
      $(this).text(pageTitle_HelloSharePoint);
      //update the tooltip
      $(this).attr("title", pageTitle_HelloSharePoint);
    }
  });
}
```

# SCENARIO 3: ADVANCED CUSTOMIZATIONS #
This scenario shows some more advanced techniques like how to deal with asynchronously loaded page content or page content which is loaded dynamically inside the page during a user event like an onClick event. 

### Note: ###
This sample shows how to translate column labels. Back when this sample was created there was no CSOM API for inserting column translations, but currently there is (see the SetLocalizationForField method in https://github.com/OfficeDev/PnP/blob/7911fe8da7a8c4c15a9de0cbbab565721e13292c/OfficeDevPnP.Core/OfficeDevPnP.Core/AppModelExtensions/FieldAndContentTypeExtensions.cs for a sample use of the new TitleResource and DescriptionResource properties). The concept of asynchrounsly loaded content however stays, so the this pattern will be stay valid.

## DEALING WITH ASYNCHRONOULSY LOADED PAGE CONTENT ##
The solution to both patterns described above depends on timed execution of your JavaScript customization code. We do this by: 
1. Using a time that fires our code
2. when our code runs it might be that the content we want to update has not yet (fully) loaded and thus our code might not work…alternatively the content was loaded and our code worked fine
3. When the code worked fine we’re all good, when the code did not work we actually use a timer to have the same code run again within x milliseconds
4. Eventually our code will run and the content which it should update has loaded, thus the code will work fine

Below code snippets show the base elements of this pattern.

### DEFINE THE VARIABLES ###

```JavaScript
//Variables used to control the asynchronous requests
var asyncReqExecutedTime = 350; //Milliseconds
var columnReqExecuted = false;
var columnReqCount = 1
```

### CALL OUR CODE VIA A TIMER ###

```JavaScript
setTimeout(ColumnReq, asyncReqExecutedTime);
```

### THE CODE THAT MAKES THE CHANGE ###

```JavaScript
function ColumnReq() {
    // If we managed to execute this code then we're done
    if (columnReqExecuted) {
        return false;
    }

    //Place your actual "customization logic" here and don't forget the 
    //flag the request to be true when you're code did succeed
    //when the selector returns data this means that the XSLT based listview 
    //has finished loading and thus the request can be flagged as done
    $(".ms-vh-div").each(function () {
        if (this.innerText.toLowerCase().indexOf("column1") > -1) {
            $(this).text(Column1_Title);
        } if (this.innerText.toLowerCase().indexOf("column2") > -1) {
            $(this).text(Column2_Title);
        }
        columnReqExecuted = true;
    });    

    // we apparently did not manage to do the change since the element where 
    // not yet loaded. Schedule another retry
    if (!columnReqExecuted) {
        // We've tried too many times...something must be wrong here
        if (columnReqCount > 15) {
            columnReqExecuted = true;
        }
        else {
            columnReqCount = columnReqCount + 1;
            // setup the next attempt
            setTimeout(ColumnReq, asyncReqExecutedTime);
        }
    }
}
```

## DEALING WITH DYNMICALLY CREATED PAGE CONTENT ##
There are places in SharePoint where page content is dynamically loaded via JavaScript triggered by a user action. A typical example here is the site access request system: the popup showing the roles you can assign to a user only is generated after you’ve opened it by a click. This means that our JavaScript code needs to “inject” itself so that it’s executed after the users click event and this is what below sample shows:

```JavaScript
//Place your actual "customization logic" here and don't forget the flag 
// the request to be true when you're code did succeed
$("a.ms-ellipsis-a").each(function (i, v) {
  if ($(this).attr('onclick')) {
    pendingReqExecuted = true;
    // bind our click event after the original click event, meaning SharePoint 
    // will first show the content on click and then our code 
    // will run and update the content loaded by SharePoint
    $(this).bind('click', function (e) {
      $(".ms-accRqCllOt-PrmCmbBx").find('option').each(function () {
        if ($(this).html().toLowerCase().indexOf("[edit]") > -1 ||
            $(this).html().toLowerCase().indexOf("[read]") > -1 ) {
          // do nothing as we want to leave this entries in the list
        } else {
          // all entries, except the "placeholder" do have an # in their 
          // value and thus need to be deleted
          if ($(this).attr("value").indexOf("#") > -1) {
            $(this).remove();
          }
        }
      });
    });
  }
});
```

# Important: #
The above type of complex JavaScript customizations are very depending on the actual implementation of SharePoint. If Microsoft decides to change the implementation (e.g. other control names) then the code might break. Only use this technique when there's no CSOM equivalent and when there's a clear business need.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.JavaScriptCustomization" />