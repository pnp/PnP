/*-----------------------------------------------------------------------------------------
Veronica Bot - makes use of Microsoft Graph in order to store the users's request in a list 
Author: Giuliano De Luca (MVP Office Development) - Twitter @giuleon
Date: February 20, 2018
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
  'tenant': process.env.TENANT, // The tenant Id or domain name (e.g mydomain.onmicrosoft.com)
  'tokenEndpoint': process.env.tokenEndpoint, // This URL will be used for the Azure AD Application to send the authorization code.
  'resource': process.env.RESOURCE, // The resource endpoint we want to give access to (in this case, SharePoint Online)
  'listId': process.env.List_Id, // The list Id where the Bot will save the user's submission
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
var azureTableClient = new botbuilder_azure.AzureTableClient(tableName, process.env['AzureWebJobsStorage']);
var tableStorage = new botbuilder_azure.AzureBotStorage({ gzipData: false }, azureTableClient);

// Create your bot with a function to receive messages from the user
var bot = new builder.UniversalBot(connector);
bot.set('storage', tableStorage);

bot.dialog('/', [
  function (session) {
    if (session.privateConversationData["welcome"]) {
      session.send("Hi I'm your SharePoint Bot to assist you to request a new SharePoint site or Teams, what do you want to request?");
    }
    session.privateConversationData["welcome"] = 'true';
    session.beginDialog('makeYourChoice');
  },
  function (session, results) {
    session.privateConversationData["SiteType"] = results.response;
    session.beginDialog('askForTitle');
  },
  function (session, results) {
    session.privateConversationData["Title"] = results.response;
    session.beginDialog('askForReason');
  },
  function (session, results) {
    session.privateConversationData["Description"] = results.response;
    session.beginDialog('askForOwner');
  },
  function (session, results) {
    session.privateConversationData["Owner"] = results.response;
    session.beginDialog('askForAlias');
  },
  function (session, results) {
    session.privateConversationData["Alias"] = results.response;
    session.beginDialog('askForConfirmation');
  },
  function (session, results) {
    if (results.response === "confirmed") {
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
            session.beginDialog('askForAnotherRequest');
          }, function (error) {
            console.error('>>> Error creating a list item: ' + error);
            session.beginDialog('askForAnotherRequest');
          });
      }, function (error) {
        console.error('>>> Error getting access token: ' + error);
        session.beginDialog('askForAnotherRequest');
      });
    } else {
      session.beginDialog('askForAnotherRequest');
    }
  },
  function (session, results) {
    if (results.response === 'yes') {
      session.beginDialog('/');
    } else {
      session.endDialog();
    }
  },
]).triggerAction({ matches: /^(show|list|restart)/i });

// Add dialog to return list of choices available
bot.dialog('makeYourChoice', [
  function (session) {
    var msg = new builder.Message(session)
      .speak('what do you want to request?')
      .text('what do you want to request?');
    msg.attachmentLayout(builder.AttachmentLayout.carousel)
    msg.attachments([
      new builder.ThumbnailCard(session)
        .title("SharePoint Site")
        .subtitle("A SharePoint team site connects you and your team to the content, information, and apps you rely on every day.")
        .text("Create a SharePoint Onlineteam site to provide a location where you and your team can work on projects and share information.")
        .images([builder.CardImage.create(session, 'https://pbs.twimg.com/profile_images/920356547015749632/2in54ehS_400x400.jpg')])
        .buttons([
          builder.CardAction.imBack(session, "TeamSite", "Confirm")
        ]),
      new builder.ThumbnailCard(session)
        .title("SharePoint Site")
        .subtitle("SharePoint communication sites are a great way to share information with others in a visually compelling format.")
        .text("With a communication site, typically only a small set of members contribute content that is consumed by a much larger audience.")
        .images([builder.CardImage.create(session, 'https://pbs.twimg.com/profile_images/920356547015749632/2in54ehS_400x400.jpg')])
        .buttons([
          builder.CardAction.imBack(session, "CommunicationSite", "Confirm")
        ]),
      new builder.ThumbnailCard(session)
        .title("Microsoft Teams")
        .subtitle("Microsoft Teams is the hub for teamwork in Office 365 that integrates all the people, content, and tools your team needs to be more engaged and effective.")
        .text("Work closer with your team by using chat, meeting, conference call, documents.")
        .images([builder.CardImage.create(session, 'https://pbs.twimg.com/profile_images/873316258980036608/QXYh4F8U_400x400.jpg')])
        .buttons([
          builder.CardAction.imBack(session, "Teams", "Confirm")
        ])
    ]);
    builder.Prompts.text(session, msg);
  },
  function name(session, results) {
    session.endDialogWithResult({ response: results.response });
  }
]);

// This dialog prompts the user for a title. 
bot.dialog('askForTitle', [
  function (session, args) {
    var question = 'What is the title of your ' + session.privateConversationData["SiteType"] + '?';
    var msg = new builder.Message(session)
      .speak(question)
      .text(question);
    builder.Prompts.text(session, msg);
  },
  function (session, results) {
    session.endDialogWithResult({ response: results.response });
  }
]);

// This dialog prompts the reason why 
bot.dialog('askForReason', [
  function (session, args) {
    var question = 'Describe the reason of your request:';
    var msg = new builder.Message(session)
      .speak(question)
      .text(question);
    builder.Prompts.text(session, msg);
  },
  function (session, results) {
    session.endDialogWithResult({ response: results.response });
  }
]);

// This dialog prompts the Owner
bot.dialog('askForOwner', [
  function (session, args) {
    var question = '';
    var msg = '';
    if (args && args.reprompt) {
      question = 'The user doesn\'t exists, please insert a valid email';
      msg = new builder.Message(session)
        .speak(question)
        .text(question);
      builder.Prompts.text(session, msg);
    } else {
      question = 'Please insert the email of the owner:';
      msg = new builder.Message(session)
        .speak(question)
        .text(question);
      builder.Prompts.text(session, msg);
    }
  },
  function (session, results) {
    var userEmail = results.response;
    // Get an access token for the app.
    auth.getAccessToken().then(function (token) {
      graph.getUserByEmail(token, userEmail)
        .then(function (result) {
          console.log(result);
          session.endDialogWithResult({ response: results.response });
        }, function (error) {
          console.error('>>> Error getting the user: ' + error);
          // Repeat the dialog
          session.replaceDialog('askForOwner', { reprompt: true });
        });
    });
  }
]);

// This dialog prompts the user for a phone number. 
// It will re-prompt the user if the input does not match a pattern for phone number.
bot.dialog('askForAlias', [
  function (session, args) {
    var question = '';
    var msg = '';
    if (args && args.reprompt) {
      question = 'The alias already exists please choose another one.';
      msg = new builder.Message(session)
        .speak(question)
        .text(question);
      builder.Prompts.text(session, msg);
    } else {
      question = 'Please insert an alias for your ' + session.privateConversationData["SiteType"] + ' without blank spaces or special characters.';
      msg = new builder.Message(session)
        .speak(question)
        .text(question);
      builder.Prompts.text(session, msg);
    }
  },
  function (session, results) {
    var alias = results.response;
    // Get an access token for the app.
    auth.getAccessToken().then(function (token) {
      graph.getUnifiedGroupByAlias(token, alias)
        .then(function (result) {
          console.log(result);
          if (result.length === 0) {
            session.endDialogWithResult({ response: alias });
          } else {
            // Repeat the dialog
            session.replaceDialog('askForAlias', { reprompt: true });
          }
        }, function (error) {
          console.error('>>> Error getting the group: ' + error);
          session.endDialogWithResult({ response: alias });
        });
    });
  }
]);

// Add dialog to request a confirmation
bot.dialog('askForConfirmation', [
  function (session) {
    var msg = new builder.Message(session)
      .speak(
        "Here the summary of your request. " +
        "Title: " + session.privateConversationData['Title'] + " " +
        "Owner: " + session.privateConversationData['Owner'] + " " +
        "Description: " + session.privateConversationData['Description'] + " " +
        "SiteType: " + session.privateConversationData['SiteType'] + " " +
        "Alias: " + session.privateConversationData['Alias']);
    msg.attachmentLayout(builder.AttachmentLayout.list)
    msg.attachments([
      new builder.ThumbnailCard(session)
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
    builder.Prompts.text(session, msg);
  },
  function name(session, results) {
    session.endDialogWithResult({ response: results.response });
  }
]);

// Add dialog to request a confirmation
bot.dialog('askForAnotherRequest', [
  function (session) {
    var msg = new builder.Message(session)
      .speak("Do you want to submit another request?");
    msg.attachmentLayout(builder.AttachmentLayout.list)
    msg.attachments([
      new builder.ThumbnailCard(session)
        .title("Request")
        .subtitle("Do you want to submit another request?")
        .text("Please confirm or not.")
        // .images([builder.CardImage.create(session, '/images/sp.png')])
        .buttons([
          builder.CardAction.imBack(session, "no", "No"),
          builder.CardAction.imBack(session, "yes", "Yes"),
        ])
    ]);
    builder.Prompts.text(session, msg);
  },
  function name(session, results) {
    session.endDialogWithResult({ response: results.response });
  }
]);

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
 * Get user by email
 * @param {*} token to append in the header in order to make the request
 * @param {*} email the email of the user
 */
graph.getUserByEmail = function (token, email) {
  var deferred = Q.defer();

  // Make a request to get all users in the tenant. Use $select to only get
  // necessary values to make the app more performant.
  request.get('https://graph.microsoft.com/v1.0/users/' + email, {
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
      deferred.resolve(parsedBody.mail);
    }
  });

  return deferred.promise;
};

/**
 * Get unified group
 */
graph.getUnifiedGroupByAlias = function (token, mailNickname) {
  var deferred = Q.defer();

  // Make a request to get all users in the tenant. Use $select to only get
  // necessary values to make the app more performant.
  request.get('https://graph.microsoft.com/v1.0/groups?$filter=groupTypes/any(c:c+eq+\'Unified\') and mailNickname eq \'' + mailNickname + '\'', {
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
  var listId = config.listId;
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