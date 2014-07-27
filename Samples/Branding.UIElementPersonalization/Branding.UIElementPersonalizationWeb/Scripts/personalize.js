// Register script for MDS if possible
RegisterModuleInit("personalize.js", RemoteManager_Inject); //MDS registration

var personalized;
var personalizedImageLink;
var personalizationTimestamp;
var cacheTimeout = 1800;
var currentTime;
var aboutMe;
var buCode;
var buCodeTimeStamp;
var peopleManager
var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();

RemoteManager_Inject(); //non MDS run

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("personalize.js");
}

function RemoteManager_Inject() {

    var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

    // load jQuery 
    loadScript(jQuery, function () {

        personalizeIt();

    });
}

SP.SOD.executeFunc('sp.js', 'SP.ClientContext', personalizeIt);

function personalizeIt() {
    clientContext = SP.ClientContext.get_current();

    var fileref = document.createElement('script');
    fileref.setAttribute("type", "text/javascript");
    fileref.setAttribute("src", "/_layouts/15/SP.UserProfiles.js");
    document.getElementsByTagName("head")[0].appendChild(fileref);

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {        
            
        // Get localstorage values if they exist
        buCode = localStorage.getItem("bucode");
        buCodeTimeStamp = localStorage.getItem("buCodeTimeStamp");

        // Check to see if the page already has injected personalized image
        var pageTitle = $('#pageTitle')[0].innerHTML;
        if (pageTitle.indexOf("img") > -1) {
            personalized = true;
        }
        else {
            personalized = false;
        }        

        // If nothing in localstorage, get profile data, which will also populate localstorage
        if (buCode == "" || buCode == null) {
            getProfileData(clientContext);
            personalized = false;
        }
        else {
            // Check for expiration            
            if (isKeyExpired("buCodeTimeStamp")) {                
                getProfileData(clientContext);

                if (buCode != "" || buCode != null) {
                    // Set timestamp for expiration
                    currentTime = Math.floor((new Date().getTime()) / 1000);
                    localStorage.setItem("buCodeTimeStamp", currentTime);

                    // Set personalized to false so that the code can check for a new image in case buCode was updated
                    personalized = false;
                }
            }            
        }

        // Load image or make sure it is current based on value in AboutMe
        if (!personalized) {
            loadPersonalizedImage(buCode);
        }


    }, 'SP.UserProfiles.js');
}

// Check to see if the key has expired
function isKeyExpired(TimeStampKey) {

    // Retrieve the example setting for expiration in seconds
    var expiryStamp = localStorage.getItem(TimeStampKey);

    if (expiryStamp != null && cacheTimeout != null) {

        // Retrieve the timestamp and compare against specified cache timeout settings to see if it is expired
        var currentTime = Math.floor((new Date().getTime()) / 1000);

        if (currentTime - parseInt(expiryStamp) > parseInt(cacheTimeout)) {
            return true; //Expired
        }
        else {
            return false;
        }
    }
    else {
        //default 
        return true;
    }
}

// Get About Me profile data and check for code (a.k.a - Business Unit Code)
function getProfileData(clientContext) {
    // Get Instance of People Manager Class       
    peopleManager = new SP.UserProfiles.PeopleManager(clientContext);

    // Get properties of the current user
    userProfileProperties = peopleManager.getMyProperties();
    clientContext.load(userProfileProperties);
    clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
        aboutMe = userProfileProperties.get_userProfileProperties()['AboutMe'];

        if (aboutMe.indexOf("XX") > -1) { buCode = "XX"; }
        else if (aboutMe.indexOf("YY") > -1) { buCode = "YY"; }
        else if (aboutMe.indexOf("ZZ") > -1) { buCode = "ZZ"; }
        else { buCode = ""; }

        // add sample business unit code to local storage
        localStorage.setItem("bucode", buCode);
        
    }))
}

// Load personalized image for demo purposes
function loadPersonalizedImage(buCode) {
    if (!personalized) {
        
        var query = "<View><Query><Where><Contains><FieldRef Name='Title'/><Value Type='Text'>" + buCode + "</Value></Contains></Where></Query><ViewFields><FieldRef Name='ID'/><FieldRef Name='Title'/><FieldRef Name='CodesImageUrl'/></ViewFields></View>";
        
        var list = clientContext.get_web().get_lists().getByTitle("CodesList");
        clientContext.load(list);

        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml(query);
        var listItems = list.getItems(camlQuery);
        clientContext.load(listItems);

        clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
            var listItemInfo;
            var listItemEnumerator = listItems.getEnumerator();

            while (listItemEnumerator.moveNext()) {
                if (!personalized) {
                    listItemInfo = listItemEnumerator.get_current().get_item('CodesImageUrl');

                    // Create IMG element to prepend
                    var title = listItemEnumerator.get_current().get_item('Title');
                    var img = $('<img id=' + title + '>');
                    img.attr('src', listItemInfo.$2_1);

                    $('#pageTitle').prepend(img);
                    personalized = true;
                }
            }            

        }));        
    }
}

function loadScript(url, callback) {
    var head = document.getElementsByTagName("head")[0];
    var script = document.createElement("script");
    script.src = url;

    // Attach handlers for all browsers
    var done = false;
    script.onload = script.onreadystatechange = function () {
        if (!done && (!this.readyState
					|| this.readyState == "loaded"
					|| this.readyState == "complete")) {
            done = true;

            // Continue your code

            callback();

            // Handle memory leak in IE
            script.onload = script.onreadystatechange = null;
            head.removeChild(script);
        }
    };

    head.appendChild(script);

}

