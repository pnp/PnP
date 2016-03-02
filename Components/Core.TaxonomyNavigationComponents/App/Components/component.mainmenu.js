// ====================
// Main menu component
// ====================
define(['jQuery',
        'Knockout',
        'Amplify',
        'text!Templates/template.mainmenu.html', 
        'NavigationViewModel',
        'TaxonomyModule',
        'UtilityModule',  
        'OfficeUiNavBar',
        'OfficeUiContextualMenu'], function($, ko, amplify, htmlTemplate, NavigationViewModelRef, TaxonomyModuleRef, UtilityModuleRef) {
               
    var taxonomyModule = new TaxonomyModuleRef();
    var utilityModule = new UtilityModuleRef();

    function mainMenuComponent(params) {
                
        var self = this;
        var isCached = false;
        
        // Get the term set id associated to the component
        var termSetId = params.termSetId;
        
        // Use the existing navigation view model
        ko.utils.extend(self, new NavigationViewModelRef());
        
        // Initialize Office UI Fabric components logic for the main menu
        if ($.fn.NavBar && $.fn.ContextualMenu) {
            $('.ms-NavBar').NavBar();
            $('.ms-NavBar').ContextualMenu();
        }
        
        // Check is a manual refresh is needed (via a custom property "NoCache" in the term set)
        taxonomyModule.getTermSetCustomPropertyValue(termSetId, "NoCache").done(function (value){
            
            if (value === "true") {
                // Clear the value in local storage for the component
                localStorage.removeItem("mainMenuNodes");
            }
            
            if (localStorage.mainMenuNodes !== null && localStorage.mainMenuNodes !== undefined) {

                var navigationTree = JSON.parse(localStorage.mainMenuNodes);
                
                // Make sure there is a valid value in the cache (not [])
                if (navigationTree.length > 0) {  
                                
                    // Load navigation tree from the local storage browser cache
                    self.initialize(navigationTree);  
                    
                    // Publish the data to all subscribers (contextual menu and breadcrumb) 
                    amplify.publish("mainMenuNodes", { nodes: navigationTree } );
                    
                    isCached = true;            
                }          
            }
            
            if (!isCached) {
                
                // Initialize the main menu with taxonomy terms            
                taxonomyModule.getNavigationTaxonomyNodes(termSetId)
                    .done(function (navigationTree) {
                        
                        // Initialize the mainMenu view model
                        self.initialize(navigationTree);
                        
                        // Publish the data to all subscribers (contextual menu and breadcrumb) 
                        amplify.publish("mainMenuNodes", { nodes: navigationTree } );
                                                                        
                        // Set the navigation tree in the local storage of the browser
                        localStorage.mainMenuNodes = utilityModule.stringifyTreeObject(navigationTree);
                                                
                }).fail(function(sender, args) {
                    console.log('Error. ' + args.get_message() + '\n' + args.get_stackTrace());
                });
            }             
            
        }).fail(function (sedner, args) {
            console.log('Error. ' + args.get_message() + '\n' + args.get_stackTrace());
        });
                   
        // We need a custom knockout binding to ensure DOM manipulations execute after the nav bar rendering (see the template.mainmenu.html file)
        ko.bindingHandlers.loadSearchBox = {
            init: function(elem) {
                
                // Hide the OOTB search box						
                $("#searchInputBox").hide();
                
                // Deactivate all OOTB events (we just want to be able to put some keywords here and redirect to the search results page, no fancy features like scopes,  search button etc.)
                $("#searchInputBox input").prop('onfocus', null).off('focus');
                $("#searchInputBox input").prop('onblur', null).off('blur');
                $("#searchInputBox input").prop('onkeydown', null).off('keydown');
                
                // Modify the CSS to integrate nicely with the navabr 
                $("#searchInputBox input").removeClass().addClass("ms-TextField-field");
                
                // Remove the default value "Search this site..."
                $("#searchInputBox input").val("");

                // Add the input only to the nav bar
                $(elem).append($("#searchInputBox input"));					
            }
        };
    }
  
    // Return component definition
    return { viewModel: mainMenuComponent, template: htmlTemplate };
});