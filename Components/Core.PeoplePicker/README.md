# PeoplePicker for provider hosted apps #

### Summary ###
This sample shows an implementation of a SharePoint People Picker control that can be used on provider hosted SharePoint apps.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
It's important that the provider hosted add-in that's running the people picker is using the same IE security zone as the SharePoint site it's installed on. If you get "Sorry we had trouble accessing your site" errors then please check this.


### Solution ###
Solution | Author(s)
---------|----------
Core.PeoplePicker | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 15th 2013 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# HOW TO USE THE PEOPLEPICKER IN YOUR PROVIDER HOSTED SP ADD-IN? #
Using the people picker in your provider hosted add-in does not require many steps :-)

## ENSURE YOU TRIGGER THE CREATION OF AN ADD-IN WEB ##
When you build a provider hosted add-in it does not necessarily have an add-in web associated with it whereas a SharePoint hosted add-in always has an add-in web. Since the people picker control uses the CSOM object model from JavaScript it’s required to have an add-in web. To ensure you have an add-in web you can just add a dummy module to your SharePoint add-in as shown below: 

![Visual Studio project structure](http://i.imgur.com/EUDXrvo.png)

## DEFINING JAVASCRIPT GLOBAL VARIABLES ##
Your add-in should have a JavaScript file that’s being loaded by your add-in pages (app.js in the sample) and in this JavaScript file you should define a context variable for the SharePoint ClientContext and one variable for the people picker:

```JavaScript
// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the
// generated html contains JS that will call into functions of this class
var peoplePicker;
```

## CREATE THE CLIENTCONTEXT OBJECT ##
Below code shows how to load the relevant SP js files and how to create the ClientContext object. The ClientContext object is created is such a way (see the `ProxyWebRequestExecutorFactory` that's being hooked up) that it can be used in cross domain scenarios which will be the case when you’re integrating your provider hosted add-in via a dialog in SharePoint.

```JavaScript
//Wait for the page to load
$(document).ready(function () {

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Load the SP.UI.Controls.js file to render the Add-in Chrome
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //load scripts for cross site calls (needed to use the people
                    //picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);

                    });

                });
        });
});
```

## INSERT THE ‘SUPPORTING’ HTML IN YOUR ASPX PAGE ##
The people picker control is a JavaScript class that 'transforms' HTML elements on the page into a working people picker. To make this work you need to insert the correct HTML on your page:

```ASPX
<div id="divAdministrators" class="cam-peoplepicker-userlookup ms-fullWidth">
  <span id="spanAdministrators"></span>
  <asp:TextBox ID="inputAdministrators" runat="server" CssClass="cam-peoplepicker-edit" Width="70"></asp:TextBox>
</div>
<div id="divAdministratorsSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
<asp:HiddenField ID="hdnAdministrators" runat="server" />
```

## TRANSFORM THE HTML INTO A PEOPLEPICKER CONTROL ##
The final step is to transform the HTML inserted in the previous step into a people picker control. This is done by creating an instance of the `CAMControl.PeoplePicker` JavaScript class and providing it a reference to the HTML elements:

```JavaScript
//Make a people picker control
//1. context = SharePoint Client Context object
//2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
//3. $('#inputAdministrators') = INPUT that will be used to capture user input
//4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the picker
//5. $('#hdnAdministrators') = INPUT hidden control that will host a resolved users
peoplePicker = new CAMControl.PeoplePicker(context, $('#spanAdministrators'), $('#inputAdministrators'), $('#divAdministratorsSearch'), $('#hdnAdministrators'));
// required to pass the variable name here!
peoplePicker.InstanceName = "peoplePicker";
// Hookup everything
peoplePicker.Initialize();
```

### Important ###
You need to set the `InstanceName` property to the name of the used `peoplepicker` variable (case sensitive!). This is needed because the people picker control will ‘generate’ HTML and JavaScript that references the control.

# PEOPLEPICKER CONFIGURATION OPTIONS #
The people picker control does have some configuration options which are explained below.

## LANGUAGE ##
The strings displayed by the control will be loaded dynamically based on the passed language. This requires you to pass the language via taking over the `SPLanguage` URL parameter (see sample) or by hardcoding it. If no language is passed the control assumes the language is English.

```JavaScript
peoplePicker.Language = spLanguage;
```

If you would like to add additional languages you need to create the appropriate JavaScript language resource files:

![Resource JS file list](http://i.imgur.com/dxKRUNS.png)

Such a resource file is simple collection of global variables:

![JS content in English](http://i.imgur.com/YdoBzaJ.png)

![JS content in Dutch](http://i.imgur.com/qYcBb6w.png)

## MAXENTRIESSHOWN ##
This setting determines how many entries the user will see in the people picker control. If the control finds more entries than this value it will tell the user to further refine the search. Default value is 4.

## ALLOWDUPLICATES ##
Can the control allow duplicate people being picked or not? Default is false.

## PRINCIPALTYPE ##
This setting determines what kind of objects the people picker will return. Default this is set to 1 which means only users. Setting it to 15 will return all possible objects (users, groups, distribution lists,…).  
See [PrincipalType enumeration](http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx) for more details on the possible values.

## MINIMALCHARACTERSBEFORESEARCHING ##
How many characters need to be entered by the user before the control issues its first query? Default setting is 2.

## SHOWLOGINNAME AND SHOWTITLE ##
These two settings determine how the user drop down looks like. By default both are true and you’ll see the following:

![Show title and login name in the list](http://i.imgur.com/OXftpp9.png)

Putting both values on false gives you a people picker control that mimics the OOB look and feel:

![Show only name](http://i.imgur.com/ZOFYq4e.png)

# APPENDIX A: ADDING MULTIPLE PEOPLE PICKERS ON A FORM (BY KARIM KAMEKA) #
While the people picker control above is great for adding and selecting multiple users there are times when you might need more than 1 people picker on a page. This can be done with a few additions to the code in the sample. In the end you will get something similar to the following:

![Two people pickers in the UI](http://i.imgur.com/e2HR7Pz.png)

## ADDING HTML TO THE FORM & UPDATING JS INSTANTIATION ##
Following the instructions above you will need to add a second block of HTML and a second variable to the JS file used to instantiate the people picker (this is `app.js` in this sample).

**STEP 1:** At the top of the `app.js` add the following variables (1 for each people picker needed):

```JavaScript
var businessOwnerPrimaryPicker;
var businessOwnerSecondaryPicker;
```

**STEP 2:** Add the following method to the bottom of the `app.js` file:

```JavaScript
function getPeoplePickerInstance(context, spanControl, inputControl, searchDivControl, hiddenControl, variableName, spLanguage)
{
    var newPicker;

    //Make a people picker control
    //1. context = SharePoint Client Context object
    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
    newPicker = new CAMControl.PeoplePicker(context, spanControl, inputControl, searchDivControl, hiddenControl);
    // required to pass the variable name here!
    newPicker.InstanceName = variableName;
    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
    // Do not set the Language property if you do not have foreseen javascript resource file for your language
    newPicker.Language = spLanguage;
    // optionally show more/less entries in the people picker dropdown, 4 is the default
    newPicker.MaxEntriesShown = 5;
    // Can duplicate entries be selected (default = false)
    newPicker.AllowDuplicates = false;
    // Show the user loginname
    newPicker.ShowLoginName = true;
    // Show the user title
    newPicker.ShowTitle = true;
    // Set principal type to determine what is shown (default = 1, only users are resolved).
    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
    // Set ShowLoginName and ShowTitle to false if you're resolving groups
    newPicker.PrincipalType = 1;
    // start user resolving as of 2 entered characters (= default)
    newPicker.MinimalCharactersBeforeSearching = 2;    

    // Hookup everything
    newPicker.Initialize();

    return newPicker;
}
```

**STEP 3.1:** Now that we have a reusable method to instantiate the people picker we can replace the inline method with multiple calls to our new method to wire up multiple People Picker controls.  Find the code block which looks like the following (at about line 52 in `app.js`):

```JavaScript
//Make a people picker control
//1. context = SharePoint Client Context object
//2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
//3. $('#inputAdministrators') = INPUT that will be used to capture user input
//4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
//5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
peoplePicker = new CAMControl.PeoplePicker(context, $('#spanAdministrators'), $('#inputAdministrators'), $('#divAdministratorsSearch'), $('#hdnAdministrators'));
// required to pass the variable name here!
peoplePicker.InstanceName = "peoplePicker";
// Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
// Do not set the Language property if you do not have foreseen javascript resource file for your language
peoplePicker.Language = spLanguage;
// optionally show more/less entries in the people picker dropdown, 4 is the default
peoplePicker.MaxEntriesShown = 5;
// Can duplicate entries be selected (default = false)
peoplePicker.AllowDuplicates = false;
// Show the user loginname
peoplePicker.ShowLoginName = true;
// Show the user title
peoplePicker.ShowTitle = true;
// Set principal type to determine what is shown (default = 1, only users are resolved).
// See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
// Set ShowLoginName and ShowTitle to false if you're resolving groups
peoplePicker.PrincipalType = 1;
// start user resolving as of 2 entered characters (= default)
peoplePicker.MinimalCharactersBeforeSearching = 2;
// Hookup everything
peoplePicker.Initialize();
```

**STEP 3.2:** Replace with the following two lines of code(Note the values for the parameters passed **MUST** match your HTML controls in your form page:

```JavaScript
businessOwnerPrimaryPicker = getPeoplePickerInstance(context, $('#spanbusinessOwnerPrimary'), $('#inputbusinessOwnerPrimary'), $('#divbusinessOwnerPrimarySearch'), $('#hdnbusinessOwnerPrimary'), "businessOwnerPrimaryPicker", spLanguage);

businessOwnerSecondaryPicker = getPeoplePickerInstance(context, $('#spanbusinessOwnerSecondary'), $('#inputbusinessOwnerSecondary'), $('#divbusinessOwnerSecondarySearch'), $('#hdnbusinessOwnerSecondary'), "businessOwnerSecondaryPicker", spLanguage);
```

**Step 4:** Add the corresponding HTML to your page.  
**NOTE**: the `spanbusinessOwnerPrimary`, `inputbusinessOwnerPrimary`, `divbusinessOwnerPrimarySearch` and `hdnbusinessOwnerPrimary` ID's below match the names in step 3.2 (they are case-sensitive):

```ASPX
<div id="divFieldOwners">
    <h3 class="ms-core-form-line line-space">
        <asp:Literal ID="Literal4" runat="server" Text="Business Owners:" /></h3>
    <div id="divBusinessOwners" class="ms-core-form-line line-space">
        <div id="divPrimaryOwner">
            <h4 class="ms-core-form-line line-space">Primary</h4>
            <div id="divBusinessOwnerPimary" class="cam-peoplepicker-userlookup ms-fullWidth">
                <span id="spanbusinessOwnerPrimary"></span>
                <asp:TextBox ID="inputbusinessOwnerPrimary" runat="server" CssClass="cam-peoplepicker-edit" Width="35"></asp:TextBox>
            </div>
            <div id="divbusinessOwnerPrimarySearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
            <asp:HiddenField ID="hdnbusinessOwnerPrimary" runat="server" />
        </div>
        <div id="divSecondaryOwner">
            <h4 class="ms-core-form-line line-space">Secondary</h4>
            <div id="divBusinessOwnerSecondary" class="cam-peoplepicker-userlookup ms-fullWidth">
                <span id="spanbusinessOwnerSecondary"></span>
                <asp:TextBox ID="inputbusinessOwnerSecondary" runat="server" CssClass="cam-peoplepicker-edit" Width="35"></asp:TextBox>
            </div>
            <div id="divbusinessOwnerSecondarySearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
            <asp:HiddenField ID="hdnbusinessOwnerSecondary" runat="server" />
        </div>
    </div>
</div>
```

# APPENDIX B: ADDING LIMIT TO THE NUMBER OF SELECTED USERS (BY KARIM KAMEKA) #
Another scenario that might be of interest is to limit the number of users which can be entered in the people picker box.  This would mimic the out of the box behavior like that of the Site Collection Admin field(s) when creating a new Site Collection. To do this we will add a new property to the `PeoplePicker` class located in the `peoplepickercontrol.js` file.

**STEP 1:** Add the new property `MaxUsers` to the PeoplePicker class (see highlighted line below):

```JavaScript
// Constructor
function PeoplePicker(SharePointContext, PeoplePickerControl, PeoplePickerEdit, PeoplePickerDisplay, PeoplePickerData) {
    //public properties
    this.SharePointContext = SharePointContext;
    this.PeoplePickerControl = PeoplePickerControl;
    this.PeoplePickerEdit = PeoplePickerEdit;
    this.PeoplePickerDisplay = PeoplePickerDisplay;
    this.PeoplePickerData = PeoplePickerData;
    this.InstanceName = "";
    this.MaxEntriesShown = 4;
    this.ShowLoginName = true;
    this.ShowTitle = true;
    this.MinimalCharactersBeforeSearching = 2;
    this.PrincipalType = 1;
    this.AllowDuplicates = false;
    this.Language = "en-us";
    //
    this.MaxUsers = 0;
    //Private variable is not really private, just a naming convention
    this._queryID = 1;
    this._lastQueryID = 1;
    this._ResolvedUsers = [];
}
```

**STEP 2:** Add extended property to the JS Class

```JavaScript
//Property wrapped in function to allow access from event handler
PeoplePicker.prototype.MaxUsers = function () {           
    return this.MaxUsers;
}
```

**STEP 3:** Update the `PushResolvedUser` method in `peoplepickercontrol.js` with the highlighted code. Now when a user tries to add too many items to the people picker they will get an alert.

```JavaScript
// Add resolved user to array and updates the hidden field control with a JSON string
PeoplePicker.prototype.PushResolvedUser = function (resolvedUser) {
    if (this.AllowDuplicates) {
        this._ResolvedUsers.push(resolvedUser);
    } else if ((this.MaxUsers > 0) && (this._ResolvedUsers.length >= this.MaxUsers)) {
        //Send message to the user that there was an error adding he user due to too many users.
        alert("Cannot Add another user the maximum number has been reached!  Remove a user before adding another!");
    } else {
        var duplicate = false;
        for (var i = 0; i < this._ResolvedUsers.length; i++) {
            if (this._ResolvedUsers[i].Login == resolvedUser.Login) {
                duplicate = true;
            }
        }

        if (!duplicate) {
            this._ResolvedUsers.push(resolvedUser);
        }
    }

    this.PeoplePickerData.val(JSON.stringify(this._ResolvedUsers));
}
```

**STEP 4:** Set property `MaxUsers` in `app.js` when initializing the control.  
In the method (around line 52 in `app.js`) set the `MaxUsers` property to a number greater than 0.

```JavaScript
//set max users in people control to 1
newPicker.MaxUsers = 1;
```

# APPENDIX C: MODIFYING THE SIZE AND STYLE OF THE PEOPLE PICKER CONTROL (BY KARIM KAMEKA) #
The people picker control included with the sample uses a fixed height of 50px, which causes the control to display like a multi-line textbox. While modifying the sample, with the steps in Appendix B above, to restrict the people picker to select only 1 user we wanted to shrink the size of this box on the form such that the control only took up 1 line in the form. To do this we must modify the corresponding style in the `peoplepickercontrol.css` file located in the sample. However, as to not break the current styling of the control it is recommended you add a new class to do a single line.  In this case we will call this new class `cam-peoplepicker-userlookup-single`. Other aspects of the people picker control can be modified in a similar way.

**STEP 1:** Add the following new CSS class to the `peoplepickercontrol.css`

```CSS
.cam-peoplepicker-userlookup-single {    
    overflow: hidden;
    border: 1px solid #99b0c1;
    padding: 2px 5px 2px 5px;
}
```

**STEP 2:** Use the new class (`cam-peoplepicker-userlookup-single`) in your HTML:

```ASPX
<div id="divPrimaryOwner">
    <h4 class="ms-core-form-line line-space">Primary</h4>
    <div id="divBusinessOwnerPimary" class="cam-peoplepicker-userlookup-single ms-fullWidth">
        <span id="spanbusinessOwnerPrimary"></span>
        <asp:TextBox ID="inputbusinessOwnerPrimary" runat="server" CssClass="cam-peoplepicker-edit" Width="35"></asp:TextBox>
    </div>
    <div id="divbusinessOwnerPrimarySearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
    <asp:HiddenField ID="hdnbusinessOwnerPrimary" runat="server" />
</div>
```
# APPENDIX D: PEOPLEPICKER USING SERVERSIDE WEBMETHOD (CSOM) (BY STIJN NEIRINCKX) #
The regular people picker uses JavaScript to get data from SharePoint. This means that the people picker requires a working cross domain library and an Add-in Web. The CSOM people picker does not need those. The people picker will call a server-side method to get data. This server-side method will call SharePoint using C# CSOM. It is also possible to add additional filtering or other logic in this web method using C#.

**STEP 1:** Insert HTML in the aspx page

```ASPX
<div id="divCsomAdministrators" class="cam-peoplepicker-userlookup ms-fullWidth">
	<span id="spanCsomAdministrators"></span>
    <asp:TextBox ID="inputCsomAdministrators" runat="server" CssClass="cam-peoplepicker-edit" Width="70"></asp:TextBox>
</div>
<div id="divCsomAdministratorsSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
<asp:HiddenField ID="hdnCsomAdministrators" runat="server" />
```

**STEP 2:** Add `WebMethod` attribute to `GetPeoplePickerData` method on the code-behind of aspx page

```cs
[WebMethod]
public static string GetPeoplePickerData()
{
     //peoplepickerhelper will get the needed values from the query string, get data from SharePoint, and return a result in JSON format
     return PeoplePickerHelper.GetPeoplePickerSearchData();
}
```

**STEP 3:** Transform HTML into people picker control

```JavaScript
//Make a Csom people picker control
//1. data url on the server (webmethod in webforms, controller action in MVC)
//2. SpHostUrl
//3. $('#spanCsomAdministrators') = SPAN that will 'host' the people picker control
//4. $('#inputCsomAdministrators') = INPUT that will be used to capture user input
//5. $('#divCsomAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
//6. $('#hdnCsomAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
csomPeoplePicker = new CAMControl.PeoplePicker('Default.aspx/GetPeoplePickerData', spHostUrl, $('#spanCsomAdministrators'), $('#inputCsomAdministrators'), $('#divCsomAdministratorsSearch'), $('#hdnCsomAdministrators'));
// required to pass the variable name here!
csomPeoplePicker.InstanceName = "csomPeoplePicker";
// Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
// Do not set the Language property if you do not have foreseen JavaScript resource file for your language
csomPeoplePicker.Language = spLanguage;
// optionally show more/less entries in the people picker dropdown, 4 is the default
csomPeoplePicker.MaxEntriesShown = 5;
// Can duplicate entries be selected (default = false)
csomPeoplePicker.AllowDuplicates = false;
// Show the user loginname
csomPeoplePicker.ShowLoginName = true;
// Show the user title
csomPeoplePicker.ShowTitle = true;

// Set principal type to determine what is shown (default = 1, only users are resolved).
// See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
// Set ShowLoginName and ShowTitle to false if you're resolving groups
csomPeoplePicker.PrincipalType = 1;
// start user resolving as of 2 entered characters (= default)
csomPeoplePicker.MinimalCharactersBeforeSearching = 2;
// Hookup everything
csomPeoplePicker.Initialize();
```
# APPENDIX E: INCLUDING A PLACEHOLDER ATTRIBUTE TO THE PEOPLEPICKER (BY VINCENT VERBEEK) #
The regular people picker uses JavaScript and CSS to resemble the look and feel of a SharePoint OOTB people picker. There could be scenarios where you want to put a placeholder text in the people picker to better inform your users on what will be done with their entry. This sample will show the required modifications in order to achieve this.

**STEP 1:** Add the placeholder attribute and modify the width of the textbox accordingly:

```aspx
<div id="divSiteOwner" class="cam-peoplepicker-userlookup ms-fullWidth">
    <span id="spanSiteOwner"></span>
    <asp:TextBox ID="inputSiteOwner" runat="server" CssClass="cam-peoplepicker-edit" Width="155" placeholder="Who will manage this site?"></asp:TextBox>
</div>
<div id="divSiteOwnerSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
<asp:HiddenField ID="hdnSiteOwner" runat="server" />
```

**STEP 2.1:** Since the value of the selected user isn't stored inside the textbox, the placeholder text does not disappear after a user has been selected. In order to change this, we need to attach a change event to the hidden field and have that change event modify the placeholder text. Open the `app.js` file and include the following line of code in the `$(document).ready` function:
```JavaScript
//Make sure the change function is executed when the value of the hidden field changes
$('#hdnSiteOwner').change(changeSiteOwnerPlaceholder);
```

**STEP 2.2:** The function *changeSiteOwnerPlaceholder* alters the placeholder text if the hidden field has a value.
```JavaScript
/* Hide the placeholder text when a site owner is selected */
function changeSiteOwnerPlaceholder() {
    if (document.getElementById("hdnSiteOwner").value != '[]')
    {
        $('#inputSiteOwner').attr('placeholder', '');
    }
    else
    {
        $('#inputSiteOwner').attr('placeholder', 'Who will manage this site?');
    }
}
```

**STEP 3** By default, hidden fields do not fire change events. This is because the change is not done by a user, but rather through code. To make sure the change event does fire whenever a user is added or removed, we need to alter `peoplepickercontrol.js` file and specifically the methods `RemoveResolvedUser`, `RecipientsSelected` and `DeleteProcessedUser` and fire the `change()` event from there.
```JavaScript
// Remove resolved user from the array and updates the hidden field control with a JSON string
PeoplePicker.prototype.RemoveResolvedUser = function (lookupValue) {
    var newResolvedUsers = [];
    for (var i = 0; i < this._ResolvedUsers.length; i++) {
        var resolvedLookupValue = this._ResolvedUsers[i].Login ? this._ResolvedUsers[i].Login : this._ResolvedUsers[i].LookupId;
        if (resolvedLookupValue != lookupValue) {
            newResolvedUsers.push(this._ResolvedUsers[i]);
        }
    }
    this._ResolvedUsers = newResolvedUsers;
    this.PeoplePickerData.val(JSON.stringify(this._ResolvedUsers));
    this.PeoplePickerData.change();
}

// Update the people picker control to show the newly added user
PeoplePicker.prototype.RecipientSelected = function(login, name, email) {
    this.HideSelectionBox();
    // Push new resolved user to list
    this.PushResolvedUser(this.ResolvedUser(login, name, email));
    // Update the resolved user display
    this.PeoplePickerControl.html(this.ResolvedUsersToHtml());
    // Prepare the edit control for a second user selection
    this.PeoplePickerEdit.val('');
    this.PeoplePickerEdit.focus();
    this.PeoplePickerData.change();
}

// Delete a resolved user
PeoplePicker.prototype.DeleteProcessedUser = function (lookupValue) {
    this.RemoveResolvedUser(lookupValue);
    this.PeoplePickerControl.html(this.ResolvedUsersToHtml());
    this.PeoplePickerEdit.focus();
    this.PeoplePickerData.change();
}
```

<img  src="https://telemetry.sharepointpnp.com/pnp/components/Core.PeoplePicker" />