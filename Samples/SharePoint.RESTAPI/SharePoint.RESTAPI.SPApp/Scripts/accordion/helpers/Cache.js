AccordionContent.CacheFunctions = function () {
    var storage = window.sessionStorage;

    var Init = function () {
        var self = this; // assign reference to current object to "self"        
    };

    var GetCachedData = function (key) {
        if (storage != null) {
            return storage[key];
        }
    };

    var SetCachedData = function (key, dataToCache) {
        if (storage != null) {
            storage[key] = dataToCache;
        }
    };

    var RemoveCachedData = function (key) {
        if (storage != null) {
            storage.removeItem(key);
        }
    };

    return {
        // declare which properties and methods are supposed to be public
        Init: Init,
        GetCachedData: GetCachedData,
        SetCachedData: SetCachedData,
        RemoveCachedData: RemoveCachedData
    }
}();