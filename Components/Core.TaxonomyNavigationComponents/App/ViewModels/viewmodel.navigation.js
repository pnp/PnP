// ====================
// Navigation view model
// ====================
define(['jQuery','Knockout', 'UtilityModule'], function ($, ko, UtilityModuleRef) {

	var navigationModule = function(){
		
		var navigationMenu = this;
        var utilityModule = new UtilityModuleRef();
        		   
		navigationMenu.nodes = ko.observableArray();
        
        navigationMenu.siteServerRelativeUrl = _spPageContextInfo.siteServerRelativeUrl;
        
		navigationMenu.initialize = function (nodes) {
			populateObservableNodeArray(nodes, navigationMenu.nodes);
		};

		var populateObservableNodeArray = function (nodes, observableArray) {
			
			for (var i = 0; i < nodes.length; i++) {
				observableArray.push(new NodeViewModel(nodes[i]));
			}
		};

		var NodeViewModel = function (node) {
			var self = this;

			self.title = ko.observable(node.Title);      
            self.url = ko.pureComputed(function() { 
                
                // Empty simple link URL or header for the term
                if (node.Url.localeCompare("") === 0) {
                    return "#";
                }
                else {
                    return node.Url;
                }
            });
			self.iconCssClass = ko.observable(node.IconCssClass);
			self.hasChildren = ko.observable(node.ChildNodes.length > 0);
            self.hasParent = ko.observable(node.ParentUrl !== null);
			self.children = ko.observableArray();
            self.friendlyUrlSegment = ko.observable(node.FriendlyUrlSegment);          
            self.isCurrentNode = ko.pureComputed(function() {
               
                var isCurrent = false;
                     
                // Works for friendly and simple link URL                
                if (decodeURI(window.location.pathname).localeCompare(self.url()) === 0) {
                    isCurrent = true;
                }
                
                return isCurrent;
                
            }, this);
            
            self.excludeFromGlobalNavigation = ko.observable(node.ExcludeFromGlobalNavigation);    
            self.excludeFromCurrentNavigation = ko.observable(node.ExcludeFromCurrentNavigation);    

			populateObservableNodeArray(node.ChildNodes, self.children);
		};
	};
	
	return navigationModule;
});