var cachingStatus = "Cache status logging started...";
var allProps;
var aboutMe;
var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();
var localStorage = window.localStorage;
var expiryKeySuffix = "ExpiryTime";
var expiryConfigKey = "ExpiryConfig";
var expired;
var cookiesEnabled;

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {

    // Perform test to see if cookies are enabled
    setCookie("Test", "None", "", "/", "", "");

    var testCookie = getCookie("Test");
    if (testCookie == null) {
        alert("Cookies are not currently enabled");
        cookiesEnabled = false;
    }
    else {
        cookiesEnabled = true;
        deleteCookie("Test", "/", "");
        //alert("Cookies are currently enabled");
    }

    if (cookiesEnabled) {

        //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
        var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
        var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

        //Build absolute path to the layouts root with the spHostUrl
        var layoutsRoot = spHostUrl + '/_layouts/15/';

        //Load the UserProfiles script
        $.getScript(layoutsRoot + 'SP.UserProfiles.js');

        // Fetch user name for welcome message
        getUserName();

        // get profile property data from cookie
        var aboutMe = getCookie("aboutMe");

        // Cookie is expired or does not exist. get user profile properties and recreate cookie
        if (aboutMe == null) {
            cachingStatus += "\n" + "About Me data does not exist in http cookie...";
            cachingStatus += "\n" + "Retrieving from user profile...";
            SP.SOD.executeOrDelayUntilScriptLoaded(getUserProperties, 'SP.UserProfiles.js');
        }
        else {
            cachingStatus += "\n" + "Retrieved profile properties from http cookie...";
            $('#status').text(cachingStatus);
            $('#aboutMeText').val(decodeURIComponent(aboutMe));
        }
    }
});

//***** Begin Cookie functions

// Create new cookie
function setCookie(key, value, expiry, path, domain, secure) {
    var todaysDate = new Date();
    todaysDate.setTime(todaysDate.getTime());

    if (expiry == "") { expiry = "1"; }

    // line below sets for n number of days - for hours, remove * 24 - for minutes remove * 60 * 24    
    if (expiry) {
        expiry = expiry * 1000 * 60 * 60 * 24;
    }

    var newExpiry = new Date(todaysDate.getTime() + (expiry));

    document.cookie = key + "=" + escape(value) +
        ( ( expiry ) ? ";expires=" + newExpiry : "" ) +
        ( ( path ) ? ";path=" + path : "" ) +
        ( ( domain ) ? ";domain=" + domain : "" ) +
        ((secure) ? ";secure" : "");

    cachingStatus += "\n" + "Creating http cookie for AboutMe data...";
    cachingStatus += "\n" + "Cookie will expire " + newExpiry;
    $('#status').text(cachingStatus);
}

// Try to retrieve the cookie
function getCookie(key) {
    var keyval = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
    return keyval ? keyval[2] : null;
}

// Delete the cookie by expiring it
function deleteCookie(key, path, domain) {
    var today = new Date();
    var expiry = today.getTime() * 1000 * 60 * 60 * 24;
    var newExpiry = new Date(today.getTime() - (expiry));
    if (getCookie(key))
        document.cookie = key + "=" +
            ((path) ? ";path=" + path : "") +
            ((domain) ? ";domain=" + domain : "") + ";expires=" + newExpiry;

}
//***** End Cookie Functions

// This function prepares, loads, and then executes a SharePoint query to get the current users information
function getUserName() {
    context.load(user);
    context.executeQueryAsync(onGetUserNameSuccess, onGetUserNameFail);
}

// This function is executed if the above call is successful
// It replaces the contents of the 'message' element with the user name
function onGetUserNameSuccess() {
    $('#message').text('Hello ' + user.get_title());
}

// This function is executed if the above call fails
function onGetUserNameFail(sender, args) {
    alert('Failed to get user name. Error:' + args.get_message());
}


// Gets current logged in user's profile properties
function getUserProperties() {

    // Get the current client context and PeopleManager instance.
    context = SP.ClientContext.get_current();
    var peopleManager = new SP.UserProfiles.PeopleManager(context);

    // Get user properties for the current logged in user.    
    personProperties = peopleManager.getMyProperties();

    // Load the PersonProperties object and send the request.
    context.load(personProperties);

    context.executeQueryAsync(onUserPropertiesSuccess, onUserPropertiesFail);
}

// Success handler for user properties async call
onUserPropertiesSuccess = function () {

    var aboutMeValue = personProperties.get_userProfileProperties()['AboutMe'];
    $('#aboutMeText').val(aboutMeValue);

    // Create cookie
    setCookie("aboutMe", aboutMeValue, "1", "/", "", "");

    cachingStatus += "\n" + "Created http cookie with profile properties...";
    $('#status').val(cachingStatus);
}


// Failure handler for user properties async call
onUserPropertiesFail = function (sender, args) {

    alert('Error while fetching user profile information for the current user' + args.get_message());

}

//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}

// Save text area edits for later by putting them in an http cookie
function saveForLater(text, key) {
    
    // Create/update cookie
    setCookie("aboutMe", text, "", "/", "", "");

    cachingStatus += "\n" + "Updated the http cookie...";
    $('#status').val(cachingStatus);    
}

// Bust the cache
function clearCache(key) {
         
    // Delete the http cookie
    deleteCookie("aboutme", "/", "");    

    cachingStatus += "\n" + "...";
    cachingStatus += "\n" + "Cleared the cookie...";
    cachingStatus += "\n" + "Retrieving profile data from user profile...";
    $('#status').val(cachingStatus);

    // Retrieve AboutMe data from profile and repopulate text area
    getUserProperties();
}



