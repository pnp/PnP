'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

//==============================================================================================================================================
// This class stores key/value pairs into Client-Side Storage, with Expiry
//
// Provides support for two types of HTML5 Web Storage locations:
//
//  - localStorage maintains a separate storage area for each given origin domain; this storage area is durable (i.e., persistent).
//      - This storage is available for the duration of the page session (as long as the browser is open, including page reloads and restores).
//      - in addition, this storage is available (i.e., it persists) even after the browser has been closed and re-opened.
//
//  - sessionStorage maintains a separate storage area for each given origin domain; this storage area is session-based (i.e., not persistent).
//      - This storage is available for the duration of the page session (as long as the browser is open, including page reloads and restores). 
//
// TODO: Provides support for traditional session Cookie Storage:
//
//  - cookieStorage maintains a separate storage area for each given origin domain; this storage area is session-based (i.e., not persistent).
//      - This storage is available for the duration of the browser session
//
// Note that we layer Expiry on top of the storage semantic, which allows us to manage the expiry of each entry.  
//  - The default implementation of localStorage persists its objects indefinitely; however, our implementation allows us to expire an entry 
//  -  after a specified number of minutes.
//  - The default implementation of sessionStorage persists its objects for the duration of the page session; however, our implementation 
//  -  allows us to expire an entry after a specified number of minutes.
//  - The default implementation of cookieStorage persists its objects for the duration of the browser session; however, our implementation 
//  -  allows us to expire an entry after a specified number of minutes.
//
//==============================================================================================================================================

/*
    Contains functionality for Durable, Session, or Cookie(TODO) Storage
*/
ns.StorageManager = function () { };

//TODO: define a cookieStorage class that supports the same API as webStorage; map it to the cookie management API

// define constants for Storage Modes
ns.StorageManager.NoStorageMode = "none";           // item does not use a cache
ns.StorageManager.SessionStorageMode = "session";   // item uses Web Storage - sessionStorage cache
ns.StorageManager.DurableStorageMode = "durable";   // item uses Web Storage - localStorage cache
// TODO: implement cookie storage
ns.StorageManager.CookieStorageMode = "cookie";     // item uses Cookie Storage - session cache

ns.StorageManager.SetStorageMode = function (storageMode)
{
    ns.StorageManager.Storage = null;
    ns.StorageManager.StorageName = null;
    if (storageMode.toLowerCase() == ns.StorageManager.DurableStorageMode.toLowerCase())
    {
        ns.StorageManager.Storage = localStorage;
        ns.StorageManager.StorageName = "localStorage";
    }
    else if (storageMode.toLowerCase() == ns.StorageManager.SessionStorageMode.toLowerCase())
    {
        ns.StorageManager.Storage = sessionStorage;
        ns.StorageManager.StorageName = "sessionStorage";
    }
    // TODO: implement cookie storage
    else if (storageMode.toLowerCase() == ns.StorageManager.CookieStorageMode.toLowerCase())
    {
        ns.LogError('ns.StorageManager.SetStorageMode(): Not Implemented StorageMode [storageMode=' + storageMode + ']');
        //ns.StorageManager.Storage = cookieStorage;
        //ns.StorageManager.StorageName = "cookieStorage";
    }
    else
    {
        ns.LogError('ns.StorageManager.SetStorageMode(): Invalid StorageMode [storageMode=' + storageMode + ']');
    }
};

// Removes the specified custom data element from storage
ns.StorageManager.ClearItem = function (storageMode, storageKey)
{
    ns.StorageManager.SetStorageMode(storageMode);
    if (storageKey)
    {
        ns.LogMessage('ns.StorageManager.ClearItem(): Clearing item in ' + ns.StorageManager.StorageName + ' [key=' + storageKey + ']');
        ns.StorageManager.Storage.removeItem(storageKey);
    }
};
// Removes all custom data elements matching the specifed storageKey wildcard from storage
ns.StorageManager.ClearItems = function (storageMode, storageKeyWildcard)
{
    ns.StorageManager.SetStorageMode(storageMode);

    if (storageKeyWildcard)
    {
        ns.LogMessage('ns.StorageManager.ClearItems(): Clearing items in ' + ns.StorageManager.StorageName + ' that match [keyWildcard=' + storageKeyWildcard + '*]');

        var storageKeys = new Array();
        var length = ns.StorageManager.Storage.length;

        // iterate the store and save any key that matches the specified storageKeyWildcard
        // Note: we intentionally avoid the use of a callback while we traverse the store; we want to ensure a complete pass before we start removing items.
        for (var i = 0; i < length; i++)
        {
            var storageKey = ns.StorageManager.Storage.key(i);
            if (storageKey.startsWith(storageKeyWildcard))
            {
                ns.LogMessage('ns.StorageManager.ClearItems(): Found match in ' + ns.StorageManager.StorageName + ' [key=' + storageKey + ']');
                storageKeys.push(storageKey);
            }
        }

        if (storageKeys.length == 0)
        {
            ns.LogMessage('ns.StorageManager.ClearItems(): No matches found in ' + ns.StorageManager.StorageName);
        }
        else
        {
            // walk the list of saved keys and remove the corresponding custom data element from storage; OK to use a callback here if you wish...
            ns.LogMessage('ns.StorageManager.ClearItems(): Processing matches...');
            for (var i = 0; i < storageKeys.length; i++)
            {
                ns.LogMessage('ns.StorageManager.ClearItems(): Clearing item in ' + ns.StorageManager.StorageName + ' [key=' + storageKeys[i] + ']');
                ns.StorageManager.Storage.removeItem(storageKeys[i]);
            }
        }
    }
};

// Commits the specified custom data element to storage using the specified storage options
ns.StorageManager.Set = function (storageMode, storageKey, data, useSliding, timeout)
{
    ns.StorageManager.SetStorageMode(storageMode);
    ns.LogMessage('ns.StorageManager.Set(): Storing item in ' + ns.StorageManager.StorageName + ' [key=' + storageKey + ']');
    ns.StorageManager.SetPersistedData(storageKey, ns.StorageManager.ConstructPersistedData(storageKey, data, useSliding, timeout));
};

// Retrieves the specified custom data element from storage; 
// returns null if the data element is not in storage; otherwise, response.data holds the data and response.hasExpired indicates freshness 
ns.StorageManager.Get = function (storageMode, storageKey)
{
    ns.StorageManager.SetStorageMode(storageMode);
    if (storageKey)
    {
        ns.LogMessage('ns.StorageManager.Get(): Looking for item in ' + ns.StorageManager.StorageName + ' [key=' + storageKey + ']');
        var persistedData = ns.StorageManager.Storage[storageKey];

        if (persistedData == null)
        {
            ns.LogMessage('ns.StorageManager.Get(): Item NOT found in ' + ns.StorageManager.StorageName + ' [key=' + storageKey + ']');
            return null;
        }

        ns.LogMessage('ns.StorageManager.Get(): Item found in ' + ns.StorageManager.StorageName + ' [key=' + storageKey + ']');
        persistedData = $.extend(new ns.PersistedData(), JSON.parse(persistedData));

        if (persistedData.HasExpired())
        {
            ns.LogMessage('ns.StorageManager.Get(): Item has EXPIRED [key=' + storageKey + ']');
            ns.StorageManager.Storage.removeItem(storageKey);
            return { hasExpired: true, data: persistedData.Data };
        }

        // If this item uses a sliding expiration policy, update its LastAccessOn property to reset the expiration timer
        if (persistedData.UseSliding)
        {
            ns.LogMessage('ns.StorageManager.Get(): Restarting sliding expiration timer for item [key=' + storageKey + ']');
            persistedData.AccessedOn = new Date();
            ns.StorageManager.SetPersistedData(storageKey, persistedData);
        }

        ns.LogMessage('ns.StorageManager.Get(): Returning item from ' + ns.StorageManager.StorageName + ' [key=' + storageKey + ']');
        return { hasExpired: false, data: persistedData.Data };
    }
};

ns.StorageManager.SetPersistedData = function (storageKey, persistedData)
{
    try
    {
        //TODO: ensure ns.StorageManager.Storage.remainingSpace > length of JSON.stringify(persistedData)
        // Unfortunately, there does not appear to be a standard, cross-browser implementaton of the remainingSpace property
        // Our only recourse is to catch the exception
        ns.StorageManager.Storage[storageKey] = JSON.stringify(persistedData);
    }
    catch (ex)
    {
        if (ex.name == 'QuotaExceededError')
        {
            // We could not insert the item into the cache because the cache is full.
            ns.LogMessage('ns.StorageManager.SetPersistedData(): Could not store the item [key=' + storageKey + ']; the ' + ns.StorageManager.StorageName + ' is FULL !!!');
            return;
        }
        ns.LogError('ns.StorageManager.SetPersistedData(): unexpected exception occurred storing the item [key=' + storageKey + ']; error=' + ex.message);
    }
};

ns.StorageManager.ConstructPersistedData = function (storageKey, data, useSliding, timeout)
{
    var persistedData = new ns.PersistedData();

    persistedData.Data = data;
    persistedData.CreatedOn = (new Date());
    persistedData.AccessedOn = (new Date());

    if (useSliding && timeout)
    {
        persistedData.Timeout = timeout;
        persistedData.UseSliding = useSliding;
    }
    else
    {
        persistedData.Timeout = timeout;
    }

    return persistedData;
};
