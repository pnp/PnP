'use strict'
// include utility.js

/****
    File contains utility methods
***/

function CreateNamespace(namespaceString)
{
    var parts = namespaceString.split('.'),
        parent = window,
        currentPart = '';

    for (var i = 0, length = parts.length; i < length; i++)
    {
        currentPart = parts[i];
        parent[currentPart] = parent[currentPart] || {};
        parent = parent[currentPart];
    }

    return parent;
};

var ns = CreateNamespace('PortalDataAccessLayer');

/****define the Global Settings class*********/
// any settings/constants referenced within this file should be defined here
ns.GlobalSettings = function () { };

/*******Error Logging********/
ns.LogMessage = function (msg)
{
    try
    {
        if (console)
        {
            console.log(msg);
        }
    }
    catch (err) {}
};

ns.LogWarning = function (msg)
{
    try
    {
        if (console)
        {
            console.log("WARNING: " + msg);
        }
    }
    catch (err) { }
};

ns.LogError = function (msg)
{
    try
    {
        if (console)
        {
            console.log("ERROR: " + msg);
        }
    }
    catch (err) {}
};

/****define the Persisted Data class*********/
ns.PersistedData = function ()
{
    this.Data = {};             // represents the data to persist
    this.CreatedOn = null;      // represents the time the data was persisted
    this.AccessedOn = null;     // represents the time the data was last accessed
    this.Timeout = null;        // represents the timeout value (in minutes)
    this.UseSliding = false;    // if "true", use a sliding expiration policy; otherwise, use an absolute expiration policy

    ///Computes the Expiration Time of the data entry
    /// the expiration time is calculated as follows:
    /// - if an absolute expiration policy is in play:  expiration time = CreatedOn + Timeout 
    /// - if a sliding expiration policy is in play:    expiration time = AccessedOn + Timeout
    this.ComputeExpiration = function ()
    {
        if (this.UseSliding && this.Timeout)
        {
            var newDt = new Date(this.AccessedOn);
            ns.LogMessage('ns.PersistedData.ComputeExpiration(): Item AccessedOn = ' + newDt);
            newDt = new Date(newDt.setMinutes(parseInt(newDt.getMinutes()) + parseInt(this.Timeout)));
            ns.LogMessage('ns.PersistedData.ComputeExpiration(): Item ExpiresOn = ' + newDt);
            return newDt;
        }
        else
        {
            var newDt = new Date(this.CreatedOn);
            ns.LogMessage('ns.PersistedData.ComputeExpiration(): Item CreatedOn = ' + newDt);
            newDt = new Date(newDt.setMinutes(parseInt(newDt.getMinutes()) + parseInt(this.Timeout)));
            ns.LogMessage('ns.PersistedData.ComputeExpiration(): Item ExpiresOn = ' + newDt);
            return newDt;
        }
    };

    /// Returns 'true' if the storage for the data item has expired; otherwise, 'false'
    this.HasExpired = function ()
    {
        //compute the expiration time for the data item
        var expirationTime = this.ComputeExpiration();

        //if Expiration time is less than the current date time, the storage for the data item has expired
        var currentTime = new Date();
        ns.LogMessage('ns.PersistedData.HasExpired(): CurrentTime = ' + currentTime);
        var hasExpired = expirationTime <= currentTime;
        ns.LogMessage('ns.PersistedData.HasExpired(): ItemHasExpired = ' + (hasExpired ? 'YES' : 'NO'));

        return hasExpired;
    }
};

/*******define the Utility Function Manager*********/
ns.UtilityManager = function () { };

// returns true if client browser supports HTML5 Web Storage
ns.UtilityManager.SupportsHtml5Storage = function ()
{
    try
    {
        return 'localStorage' in window && window['localStorage'] !== null;
    }
    catch (e)
    {
        return false;
    }
};

// function to get a parameter value by a specific key
ns.UtilityManager.GetQueryStringParameter = function (key)
{
    var mainParams = document.URL.split('?');

    if (mainParams.length < 2)
    {
        return "";
    }

    var params = mainParams[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1)
    {
        var singleParam = params[i].split('=');
        if (singleParam[0] == key)
        {
            return decodeURIComponent(singleParam[1]);
        }
    }
};

// Safely retrieve a property value
ns.UtilityManager.SafeGetProperty = function (props, key)
{
    try
    {
        return props.get_item(key);
    }
    catch (e)
    {
        return '';
    }
};

