/*-----------------------------------------------------------------------------------------
Veronica Bot - makes use of Microsoft Graph in order to store the users's request in a list 
-----------------------------------------------------------------------------------------*/

var restify = require('restify');
var builder = require('botbuilder');
var botbuilder_azure = require("botbuilder-azure");
var request = require("request");
var Q = require('q');

// Setup Restify Server
var server = restify.createServer();

server.use(restify.plugins.bodyParser({
  mapParams: true
})); // To be able to get the authorization code (req.params.code)

server.listen(process.env.port || process.env.PORT || 3978, function () {
   console.log('%s listening to %s', server.name, server.url); 
});
  
// Create chat connector for communicating with the Bot Framework Service
var connector = new builder.ChatConnector({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword,
});

// Config
var config = {
    'clientId': process.env.AAD_CLIENT_ID, // The client Id retrieved from the Azure AD App
    'clientSecret': process.env.AAD_CLIENT_SECRET, // The client secret retrieved from the Azure AD App
    'tenant' : process.env.TENANT, // The tenant Id or domain name (e.g mydomain.onmicrosoft.com)
    'tokenEndpoint' : process.env.tokenEndpoint, // This URL will be used for the Azure AD Application to send the authorization code.
    'resource' : process.env.RESOURCE, // The resource endpoint we want to give access to (in this case, SharePoint Online)
}

// Graph
var graph = {};

// Listen for messages from users 
server.post('/api/messages', connector.listen());

/*----------------------------------------------------------------------------------------
* Bot Storage: This is a great spot to register the private state storage for your bot. 
* We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
* For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure
* ---------------------------------------------------------------------------------------- */

var tableName = 'botdata';
var azureTableClient = new botbuilder_azure.AzureTableClient(tableName, process.env['AzureWebJobsStorage'] || "DefaultEndpointsProtocol=https;AccountName=giulbot8e6a;AccountKey=T6i6y9pIHJKQ5qiMEyJ4lPR129Gemgjkm03HGmH49IXg9eNkBl0dh/ci8MGSk0fCWDXqF0XEbPS9plJ5dn73Pw==;");
var tableStorage = new botbuilder_azure.AzureBotStorage({ gzipData: false }, azureTableClient);

// Create your bot with a function to receive messages from the user
var bot = new builder.UniversalBot(connector);
bot.set('storage', tableStorage);
var step = '';

bot.dialog('/', function (session) {
    var usertypes = session.message.text.toLowerCase();

    if (step == '') {
      session.send("Hi I'm your SharePoint Bot to assist you to request a new SharePoint site or Teams, what do you want to request?");
      session.beginDialog('makeYourChoice');
      step = '1';
    } 
    else if(step == '1') {
      session.privateConversationData["SiteType"] = usertypes;
      step = '2';
      session.send('What is the title of your ' + usertypes + '?');
    }
    else if(step == '2') {
      session.privateConversationData["Title"] = usertypes;
      step = '3';
      session.send('Describe the reason of your request.');
    }
    else if(step == '3') {
      session.privateConversationData["Description"] = usertypes;
      step = '4';
      session.send('Please insert the email of the owner.');
    }
    else if(step == '4') {
      session.privateConversationData["Owner"] = usertypes;
      step = '5';
      session.send('Please insert an alias for your ' + session.privateConversationData["SiteType"] + ' without blank spaces or special characters.');
    }
    else if(step == '5') {
      if (session.message.text !== 'canceled' && session.message.text !== 'confirmation') {
        session.privateConversationData["Alias"] = usertypes;
      }
      session.beginDialog('confirmation');
      step = "6";
    }
    else if(step == '6') {
      if (session.message.text == "confirmed") {
        // Get an access token for the app.
        auth.getAccessToken().then(function (token) {
          // create a new list item
          var params = {
            "fields": {
              "Title": session.privateConversationData['Title'],
              "Status": "Requested",
              "Owner": session.privateConversationData['Owner'],
              "Description": session.privateConversationData['Description'],
              "SiteType": session.privateConversationData['SiteType'],
              "Alias": session.privateConversationData['Alias']
            }
          };
      
          graph.postListItem(token, params)
              .then(function (result) {
              console.log(result);
              session.send("Request submitted successfully");
              step = "";
          }, function (error) {
              console.error('>>> Error getting users: ' + error);
          });
        }, function (error) {
            console.error('>>> Error getting access token: ' + error);
        });
        
      } else {
        // reset the dialog
        step = "";
        // restart the dialog
        session.beginDialog('/');
      }
    }
    else {
      
    }

});

// Add dialog to return list of shirts available
bot.dialog('makeYourChoice', function (session) {
  var msg = new builder.Message(session);
  msg.attachmentLayout(builder.AttachmentLayout.carousel)
  msg.attachments([
      new builder.HeroCard(session)
          .title("SharePoint Site")
          .subtitle("Team Site")
          .text("Keep informed your team with news.")
          // .images([builder.CardImage.create(session, '/images/sp.png')])
          .buttons([
              builder.CardAction.imBack(session, "TeamSite", "Confirm")
          ]),
      new builder.HeroCard(session)
          .title("SharePoint Site")
          .subtitle("Communication Site")
          .text("Inform your users with corporate communications and events")
          // .images([builder.CardImage.create(session, '/images/sp.png')])
          .buttons([
              builder.CardAction.imBack(session, "CommunicationSite", "Confirm")
          ]),
      new builder.HeroCard(session)
          .title("Microsoft Teams")
          .subtitle("Teams")
          .text("Work closer with your team by using chat, meeting, conference call, documents...")
          // .images([builder.CardImage.create(session, '/images/Teams.png')])
          .buttons([
              builder.CardAction.imBack(session, "Teams", "Confirm")
          ])
  ]);
  session.send(msg).endDialog();
}).triggerAction({ matches: /^(show|list)/i });

// Add dialog to return list of shirts available
bot.dialog('confirmation', function (session) {
  var msg = new builder.Message(session);
  msg.attachmentLayout(builder.AttachmentLayout.carousel)
  msg.attachments([
    new builder.HeroCard(session)
      .title("Summary")
      .subtitle("Here the summary of your request")
      .text(
        "Title: " + session.privateConversationData['Title'] + "\n\n" +
        "Owner: " + session.privateConversationData['Owner'] + "\n\n" +
        "Description: " + session.privateConversationData['Description'] + "\n\n" +
        "SiteType: " + session.privateConversationData['SiteType'] + "\n\n" +
        "Alias: " + session.privateConversationData['Alias'])
      // .images([builder.CardImage.create(session, '/images/sp.png')])
      .buttons([
          builder.CardAction.imBack(session, "canceled", "Cancel"),
          builder.CardAction.imBack(session, "confirmed", "Confirm"),         
      ])
  ]);
  session.send(msg).endDialog();
});

/*----------------------------------------------------------------------------------------
* GRAPH API
------------------------------------------------------------------------------------------ */

/**
 * Get all users
 * @param {*} token to append in the header in order to make the request
 */
graph.getUsers = function (token) {
    var deferred = Q.defer();
  
    // Make a request to get all users in the tenant. Use $select to only get
    // necessary values to make the app more performant.
    request.get('https://graph.microsoft.com/v1.0/users?$select=id,displayName', {
      auth: {
        bearer: token
      }
    }, function (err, response, body) {
      var parsedBody = JSON.parse(body);
  
      if (err) {
        deferred.reject(err);
      } else if (parsedBody.error) {
        deferred.reject(parsedBody.error.message);
      } else {
        // The value of the body will be an array of all users.
        deferred.resolve(parsedBody.value);
      }
    });
  
    return deferred.promise;
};

/**
 * Create Group in the SP list
 * @param {*} token to append in the header in order to make the request
 * @param {*} params the json in order to create a new o365 group
 */
graph.createGroup = (token, params) => {
  var deferred = Q.defer();
  var endpointUrl = "https://graph.microsoft.com/v1.0/groups";
  
  request.post({ 
    url: endpointUrl,
    headers: {
      "Authorization": "Bearer " + token,
      "Content-Type": "application/json"
    },
    body: JSON.stringify(params)
  }, function (err, response, body) {
    var parsedBody = JSON.parse(body);

    if (err) {
      deferred.reject(err);
    } else if (parsedBody.error) {
      deferred.reject(parsedBody.error.message);
    } else {
      // The value of the body will be an array of all users.
      deferred.resolve(parsedBody.id);
    }
  });

  return deferred.promise;
}

/**
 * 
 * @param {*} token to append in the header in order to make the request
 * @param {*} params the json in order to create a new list item
 */
graph.postListItem = (token, params) => {
  var deferred = Q.defer();
  var listId = process.env.List_Id;
  var endpointUrl = "https://graph.microsoft.com/v1.0/sites/root/lists/" + listId + "/items";
  
  request.post({ 
    url: endpointUrl,
    headers: {
      "Authorization": "Bearer " + token,
      "Content-Type": "application/json"
    },
    body: JSON.stringify(params)
  }, function (err, response, body) {
    var parsedBody = JSON.parse(body);

    if (err) {
      deferred.reject(err);
    } else if (parsedBody.error) {
      deferred.reject(parsedBody.error.message);
    } else {
      // The value of the body will be an array of all users.
      deferred.resolve(parsedBody.value);
    }
  });

  return deferred.promise;
}


/*----------------------------------------------------------------------------------------
* Authentication
------------------------------------------------------------------------------------------ */
// The auth module object.
var auth = {};

/**
 * Get access token
 */
auth.getAccessToken = function () {
  var deferred = Q.defer();

  // These are the parameters necessary for the OAuth 2.0 Client Credentials Grant Flow.
  // For more information, see Service to Service Calls Using Client Credentials (https://msdn.microsoft.com/library/azure/dn645543.aspx).
  var requestParams = {
    grant_type: 'client_credentials',
    client_id: config.clientId,
    client_secret: config.clientSecret,
    resource: config.resource
  };

  /**
   * post: Make a request to the token issuing endpoint
   */
  request.post({ url: config.tokenEndpoint, form: requestParams }, function (err, response, body) {
    var parsedBody = JSON.parse(body);

    if (err) {
      deferred.reject(err);
    } else if (parsedBody.error) {
      deferred.reject(parsedBody.error_description);
    } else {
      // If successful, return the access token.
      deferred.resolve(parsedBody.access_token);
    }
  });

  return deferred.promise;
};