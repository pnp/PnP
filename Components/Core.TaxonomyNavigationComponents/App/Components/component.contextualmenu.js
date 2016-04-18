// ====================
// Contextual menu component
// ====================
define(['jQuery',
        'Knockout',
        'Amplify',
        'text!Templates/template.contextualmenu.html', 
        'NavigationViewModel',
        'UtilityModule',
        'OfficeUiContextualMenu'], function($, ko, amplify, htmlTemplate, NavigationViewModelRef, UtilityModuleRef) {
            
    var utilityModule = new UtilityModuleRef();
           
    function contextualMenuComponent(params) {
                
        var self = this;
                
        // Use the existing navigation view model
        ko.utils.extend(self, new NavigationViewModelRef());      
                
        // Apply Office UI Fabric logic to the contextual menu
        if ($.fn.ContextualMenu) {
            $("component-contextualmenu").ContextualMenu();    
        }  
        
        // Subscribe to the main menu nodes
        amplify.subscribe("mainMenuNodes", function(data) {
            
            var navigationTree = data.nodes;
            
            // Get the navigation node according to the current URL   
            var currentNode = utilityModule.getNodeFromCurrentUrl(navigationTree, window.location.pathname);
            
            if (currentNode !== undefined) {
            
                // If there is no 'ParentUrl', this is a root term
                if (currentNode.ParentUrl !== null) {
                    
                    navigationTree = utilityModule.getNodeFromCurrentUrl(data.nodes, currentNode.ParentUrl);
                    
                    if (navigationTree.ChildNodes.length > 0) {
                        
                        // Display all siblings and child nodes from the current node (just like the CSOM results)
                        // Siblings = children of my own parent ;)
                        navigationTree = navigationTree.ChildNodes;
                    }
                }
            }
            
            self.initialize(navigationTree);
        });
    }
  
    // Return component definition
    return { viewModel: contextualMenuComponent, template: htmlTemplate };
});