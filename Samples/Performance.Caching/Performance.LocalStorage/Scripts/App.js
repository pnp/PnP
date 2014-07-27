var cachingStatus = "Cache status logging started...";
var allProps;
var aboutMe;
var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();
var localStorage = window.localStorage;
var expiryKeySuffix = "ExpiryTime";
var expiryConfigKey = "ExpiryConfig";
var expired;

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //Load the UserProfiles script
    $.getScript(layoutsRoot + 'SP.UserProfiles.js');

    // Fetch user name for welcome message
    getUserName();

    if (isHtml5StorageSupported()) {
        if (Modernizr.localstorage) {

            //localStorage.clear(); // for cache testing only

            // Check for and retrieve expiration default settings
            checkForDefaultExpirySetting();

            // get profile property data from local storage cached items 
            aboutMe = localStorage.getItem("aboutMeValue");

            if (aboutMe == null) {
                cachingStatus += "\n" + "About Me data does not exist in local storage...";
                cachingStatus += "\n" + "Retrieving from user profile...";
                SP.SOD.executeOrDelayUntilScriptLoaded(getUserProperties, 'SP.UserProfiles.js');
            }
            else {
                cachingStatus += "\n" + "Retrieved profile properties from local storage...";
                $('#status').text(cachingStatus);

                var tmpKeyName = "aboutMeValue" + expiryKeySuffix;

                // Check to see if local storage expiration configuation has been set
                var expiryConfigSetting = localStorage.getItem(expiryConfigKey);

                if (expiryConfigSetting != null) {

                    // Check to see if an expiry stamp has ben set
                    var localStorageExpiry = localStorage.getItem(tmpKeyName);

                    if (localStorageExpiry != null) {
                        // Check to see if the key's value has expired
                        if (!isLocalStorageExpired("aboutMeValue", tmpKeyName)) {
                            $('#aboutMeText').val(aboutMe);
                        }
                        else {
                            cachingStatus += "\n" + "Saved data in local storage has expired";
                            cachingStatus += "\n" + "Retrieving from user profile...";
                            SP.SOD.executeOrDelayUntilScriptLoaded(getUserProperties, 'SP.UserProfiles.js');
                        }
                    }
                    else {
                        $('#aboutMeText').val(aboutMe);
                    }
                }
                else { $('#aboutMeText').val(aboutMe); }
            }
            
        }
        else {
            SP.SOD.executeOrDelayUntilScriptLoaded(getUserProperties, 'SP.UserProfiles.js');
            cachingStatus += "\n" + "Modernizr checked local storage. Retrieved profile properties from user profile...";
            $('#status').val(cachingStatus);
        }
    }
    else {
        SP.SOD.executeOrDelayUntilScriptLoaded(getUserProperties, 'SP.UserProfiles.js');
        cachingStatus += "\n" + "HTML5 storage not supported. Retrieved profile properties from user profile...";
        $('#status').val(cachingStatus);
    }

    // Set the events to track storage area changes
    window.onload = setEvents();
});

// This function sets up events to track storage area changes
function setEvents() {
    if (window.addEventListener) {
        window.addEventListener("storage", update_storage, false);
    } else {
        window.attachEvent("onstorage", update_storage);
    };
}

function update_storage(e) {
    if (!e) {
        e = window.event;

        // Log to UI status window
        cachingStatus += "\n" + "[Event Tracking] A change was made to this storage key: " + e.keyCode;
    } 
}

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

    // add to local storage
    localStorage.setItem("aboutMeValue", aboutMeValue);
    setLocalStorageKeyExpiry("aboutMeValue");

    cachingStatus += "\n" + "Populated local storage with profile properties...";
    $('#status').val(cachingStatus);
}


// Failure handler for user properties async call
onUserPropertiesFail = function (sender, args) {

    alert('Error while fetching user profile information for the current user' + args.get_message());

}

// Checks whether browser supports html storage or not 
isHtml5StorageSupported = function () {
    try {
        return 'localStorage' in window && window['localStorage'] !== null;
    } catch (e) {
        return false;
    }
    return false;
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

// Save text area edits for later by putting them in local storage
function saveForLater(text, key) {

    // add to local storage
    localStorage.setItem(key, text);

    cachingStatus += "\n" + "Updated the local storage...";
    $('#status').val(cachingStatus);

    setLocalStorageKeyExpiry(key);
}

// Bust the cache
function clearCache(key) {
         
    // Remove key/value
    localStorage.removeItem(key);
    // Remove key/value expiration
    localStorage.removeItem(key + expiryKeySuffix);

    cachingStatus += "\n" + "...";
    cachingStatus += "\n" + "Cleared the cache...";
    cachingStatus += "\n" + "Retrieving profile data from user profile...";
    $('#status').val(cachingStatus);

    // Retrieve AboutMe data from profile and repopulate text area
    getUserProperties();
}

// Set the expiration settings in seconds value (for demo purposes only)
function setExpiryConfiguration() {

    // Set the expiration (in seconds, if specified in UI) for the aboutMeValue key value
    if ($('#expirySetting').val() != "") {
        var userExpiryValue = parseInt($('#expirySetting').val());
        var currentTime = Math.floor((new Date().getTime()) / 1000);
        localStorage.setItem(expiryConfigKey, userExpiryValue);        

        // Log status to window
        cachingStatus += "\n" + "Setting default expiration config key to " + userExpiryValue + " seconds";
        $('#status').val(cachingStatus);
    }
    else {

        // Remove from local storage
        localStorage.removeItem(expiryConfigKey);
        cachingStatus += "\n" + "Clearing default expiration config key value...";
        $('#status').val(cachingStatus);
    }
}

// Set expiration in seconds specified in demo UI field
function setLocalStorageKeyExpiry(key) {

    // Check for expiration config values
    var expiryConfig = localStorage.getItem(expiryConfigKey);
    
    // Check for existing expiration stamp
    var existingStamp = localStorage.getItem(key + expiryKeySuffix);    

    // Override cached setting if a user has entered a value that is different than what is stored
    if (expiryConfig != null) {
                
        var currentTime = Math.floor((new Date().getTime()) / 1000);
        expiryConfig = parseInt(expiryConfig);
        
        var newStamp = Math.floor((currentTime + expiryConfig));
        localStorage.setItem(key + expiryKeySuffix, newStamp);
        
        // Log status to window        
        cachingStatus += "\n" + "Setting expiration for the " + key + " key...";
        $('#status').val(cachingStatus);
    }    
    else {
       
    }
}

// Check to see if the the aboutMeValue key has expired
function isLocalStorageExpired(key, keyTimeStampName) {

    // Retrieve the example setting for expiration in seconds
    var expiryConfig = localStorage.getItem(expiryConfigKey);
    
    // Retrieve the example setting for expiration in seconds
    var expiryStamp = localStorage.getItem(keyTimeStampName);

    if (expiryStamp != null && expiryConfig != null) {

        // Retrieve the expiration stamp and compare against specified settings to see if it is expired
        var currentTime = Math.floor((new Date().getTime()) / 1000);

        if (currentTime - parseInt(expiryStamp) > parseInt(expiryConfig)) {
            cachingStatus += "\n" + "The " + key + " key timestamp has expired...";
            $('#status').val(cachingStatus);
            return true;
        }
        else {
            var estimatedSeconds = parseInt(expiryStamp) - currentTime;
            cachingStatus += "\n" + "The " + key + " timestamp expires in " + estimatedSeconds + " seconds...";
            $('#status').val(cachingStatus);
            return false;
        }
    }
    else {
        //default
        return true;
    }
}

function checkForDefaultExpirySetting() {
    // Check for expiry settings
    expiryConfig = localStorage.getItem(expiryConfigKey);

    if (expiryConfig == null) {
        cachingStatus += "\n" + "Expiration configuration not set in local storage...";
        $('#status').val(cachingStatus);
    }
    else {
        cachingStatus += "\n" + "Retrieved expiration settings from local storage...";
        $('#status').val(cachingStatus);
        $('#expirySetting').val(expiryConfig);
    }
}