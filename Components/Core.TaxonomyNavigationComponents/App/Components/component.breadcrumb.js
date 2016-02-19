// ====================
// Navbar component
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
           
        // Get the current node from the current URL
        var currentFriendlyUrlSegment = utilityModule.getCurrentFriendlyUrlSegment();
        var currentNode = utilityModule.getNodeByFriendlyUrlSegment(nodes, currentFriendlyUrlSegment);

        if (currentNode !== undefined) {
         
            breadcrumbNodes.push(currentNode);                         

            while (currentNode.ParentFriendlyUrlSegment !== null) {                    
                var parentNode = utilityModule.getNodeByFriendlyUrlSegment(nodes, currentNode.ParentFriendlyUrlSegment);
                breadcrumbNodes.push(parentNode);
                currentNode = parentNode;
            }     

            breadcrumbNodes = breadcrumbNodes.reverse();    
        }
        
        return breadcrumbNodes;
    };

    function breadcrumbComponent(params) {
                
        var self = this;
        
        // Use the existing navigation view model intialized with the term set id passed as parameter in the DOM element
        ko.utils.extend(self, new NavigationViewModelRef());
                        
        // Subscribe to the main menu nodes
        amplify.subscribe("mainMenuNodes", function(data) {
            self.initialize(getBreadcrumbNodes(data.nodes));
        });
    }                      
    // Return component definition
    return { viewModel: breadcrumbComponent, template: htmlTemplate };
});