// ====================
// Navigation view model
// ====================
define(['jQuery','Knockout', 'UtilityModule'], function ($, ko, UtilityModuleRef) {

	var navigationModule = function(){
		
		var navigationMenu = this;
        var utilityModule = new UtilityModuleRef();
        		   
		// Public properties
		navigationMenu.nodes = ko.observableArray();
        
        navigationMenu.siteServerRelativeUrl = _spPageContextInfo.siteServerRelativeUrl;

		// Public functions
		navigationMenu.initialize = function (nodes) {
			populateObservableNodeArray(nodes, navigationMenu.nodes);
		};

		// Private functions
		var populateObservableNodeArray = function (nodes, observableArray) {
			
			for (var i = 0; i < nodes.length; i++) {
				observableArray.push(new NodeViewModel(nodes[i]));
			}
		};

		var NodeViewModel = function (node) {
			var self = this;

			self.title = ko.observable(node.Title);
			self.url = ko.observable(node.Url);
			self.iconCssClass = ko.observable(node.IconCssClass);
			self.hasChildren = ko.observable(node.ChildNodes.length > 0);
            self.hasParent = ko.observable(node.ParentFriendlyUrlSegment !== null);
			self.children = ko.observableArray();
            self.friendlyUrlSegment = ko.observable(node.FriendlyUrlSegment);          
            self.isCurrentNode = ko.pureComputed(function() {
               
                var isCurrent = false;
                
                // If the friendly URL segment matches the current URL segment, the node is the current node
                var currentFriendlyUrlSegment = utilityModule.getCurrentFriendlyUrlSegment();
                if(currentFriendlyUrlSegment.localeCompare(self.friendlyUrlSegment()) === 0) {
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