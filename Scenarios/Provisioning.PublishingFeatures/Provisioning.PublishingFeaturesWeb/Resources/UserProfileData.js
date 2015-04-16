
// No need for MDS entry for publishing sites, since MDS is not used
SP.SOD.executeOrDelayUntilScriptLoaded(Contoso_SharePointReady, 'sp.js');

// Create an instance of the current context.
function Contoso_SharePointReady() {
    clientContext = SP.ClientContext.get_current();

    var fileref = document.createElement('script');
    fileref.setAttribute("type", "text/javascript");
    fileref.setAttribute("src", "/_layouts/15/SP.UserProfiles.js");
    document.getElementsByTagName("head")[0].appendChild(fileref)

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {

        //Get Instance of People Manager Class       
        var peopleManager = new SP.UserProfiles.PeopleManager(clientContext);

        //Get properties of the current user
        userProfileProperties = peopleManager.getMyProperties();
        clientContext.load(userProfileProperties);
        clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
            var firstname = userProfileProperties.get_userProfileProperties()['FirstName'];
            var name = userProfileProperties.get_userProfileProperties()['PreferredName'];
            var title = userProfileProperties.get_userProfileProperties()['Title']
            var aboutMe = userProfileProperties.get_userProfileProperties()['AboutMe'];
            var picture = userProfileProperties.get_userProfileProperties()['PictureURL'];

            var html = "<div><h2>Welcome " + firstname + "</h2></div><div><div style='float: left; margin-left:10px'><img style='float:left;margin-right:10px' src='" + picture + "' /><b>Name</b>: " + name + "<br /><b>Title</b>: " + title + "<br />" + aboutMe + "</div></div>";

            document.getElementById('ContosoUserProfileDataInformation').innerHTML = html;
        }), Function.createDelegate(this, function (sender, args) {
            console.log('The following error has occured while loading user profile property: ' + args.get_message());
        }));
    }, 'SP.UserProfiles.js');
}
