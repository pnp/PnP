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
         
         
        this.getNodeByFriendlyUrlSegment =  function (nodes, currentFriendlyUrlSegment) {
        
            if (nodes) {
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].FriendlyUrlSegment === currentFriendlyUrlSegment) {
                        return nodes[i];
                    }
                    var found = this.getNodeByFriendlyUrlSegment(nodes[i].ChildNodes, currentFriendlyUrlSegment);
                    if (found) return found;
                }
            }
        };
        
        this.getCurrentFriendlyUrlSegment = function () {
            
            var currentFriendlyUrlSegment = window.location.href.replace(/\/$|#/g, '').split('?')[0].split('/').pop();
            
            return currentFriendlyUrlSegment;
        };  
    };

    return utilityModule;  
});