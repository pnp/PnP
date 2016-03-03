// ====================
// Utility module
// ====================
define([], function () {
 
 	var utilityModule = function() {
         
         this.stringifyTreeObject = function (object) {
                   
             var cache = [];
             var stringified = JSON.stringify(object, function(key, value) {
                    if (typeof value === 'object' && value !== null) {
                        if (cache.indexOf(value) !== -1) {
                            // Circular reference found, discard key
                            return;
                        }
                        // Store value in our collection
                        cache.push(value);
                    }
                    return value;
                });
                cache = null;
                
             return stringified;
         };
         
        // The node is retrieved by its resolved display URL
        // If there are multiple nodes with the same simple link url, only the first match is returned (and you probably have some problems with your navigation consistency...)
        this.getNodeFromCurrentUrl =  function (nodes, pageUrl) {
                
            if (nodes) {
                for (var i = 0; i < nodes.length; i++) {
                                                     
                    // Does a node in the whole site map have this current page url as resolved display URL (friendly or simple link)
                    if (nodes[i].Url.localeCompare(decodeURI(pageUrl)) === 0) {    
                        return nodes[i];                        
                    }
                    
                    var found = this.getNodeFromCurrentUrl(nodes[i].ChildNodes, pageUrl);
                    if (found) return found;
                }
            }
        };
    };

    return utilityModule;  
});