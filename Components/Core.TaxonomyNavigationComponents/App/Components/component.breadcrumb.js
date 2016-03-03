// ====================
// Breadcrumb component
// ====================
define(['jQuery',
        'Knockout',
        'Amplify',
        'text!Templates/template.breadcrumb.html',
        'NavigationViewModel',
        'UtilityModule'], function($, ko, amplify, htmlTemplate, NavigationViewModelRef, UtilityModuleRef) {
    
    var utilityModule = new UtilityModuleRef();
        
    var getBreadcrumbNodes = function (nodes) {
  
        var breadcrumbNodes = [];   
           
        // Get the navigation node according to the current URL   
        var currentNode = utilityModule.getNodeFromCurrentUrl(nodes, window.location.pathname);

        if (currentNode !== undefined) {
         
            breadcrumbNodes.push(currentNode);                         
            
            // If there is no 'ParentUrl', this is a root term
            while (currentNode.ParentUrl !== null) {
                                        
                var parentNode = utilityModule.getNodeFromCurrentUrl(nodes, currentNode.ParentUrl);
                    
                breadcrumbNodes.push(parentNode);
                currentNode = parentNode;
            }     

            breadcrumbNodes = breadcrumbNodes.reverse();    
        }
        
        return breadcrumbNodes;
    };

    function breadcrumbComponent(params) {
                
        var self = this;
        
        // Use the existing navigation view model
        ko.utils.extend(self, new NavigationViewModelRef());
                        
        // Subscribe to the main menu nodes
        amplify.subscribe("mainMenuNodes", function(data) {
            self.initialize(getBreadcrumbNodes(data.nodes));
        });
    }                      
    // Return component definition
    return { viewModel: breadcrumbComponent, template: htmlTemplate };
});