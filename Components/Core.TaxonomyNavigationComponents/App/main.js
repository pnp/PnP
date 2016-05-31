require.config({
	
	paths: {
		
		// SharePoint native libraries
		'SP_RuntimeJs': window.location.origin + "/_layouts/15/sp.runtime",
		'SP_Js': window.location.origin + "/_layouts/15/sp",
		'SP_TaxonomyJs': window.location.origin + "/_layouts/15/sp.taxonomy",
		'SP_PublishingJs': window.location.origin + "/_layouts/15/sp.publishing",
		
        // All paths are relative to the main.js script inside the style library
        
		// Third party libraries
		// They can alternatively loaded by CDN (See https://github.com/requirejs/example-jquery-cdn)
        'jQuery': 'Lib/jquery-2.2.0.min',
        'Knockout': 'Lib/knockout-3.4.0',
        'Amplify': 'Lib/amplify.min',
		        
		// RequireJS Plugins
		// We use domReady RequireJS plugin instead of $(document).ready()
		// See http://requirejs.org/docs/api.html#pageload
        // We user the text RequireJS plugin to load external html files for knockout components
        // See https://github.com/requirejs/text
		'domReady' : 'Plugins/domReady',
        'text' : 'Plugins/text',
		
		// Office UI Fabric scripts for components behavior
		'OfficeUiNavBar' : 'OfficeUI/OfficeUi.NavBar',
		'OfficeUiContextualMenu' : 'OfficeUI/OfficeUi.ContextualMenu',

		// Application modules
		'TaxonomyModule' :  'Modules/module.taxonomy',
        'UtilityModule' : 'Modules/module.utility',
        
         // View Models
        'NavigationViewModel' : 'ViewModels/viewmodel.navigation',
    },
	
    shim: {
		
        'jQuery': {
            exports: '$'
        },
		
        'Knockout': {
			deps: ['jQuery'],
            exports: 'ko'
        },
        
        'Amplify': {
            exports: 'amplify'
        },
		
		'SP_Js' : {
			deps: ['SP_RuntimeJs']
        },
		
		'SP_TaxonomyJs' : {
			deps: ['SP_Js']
        },
		
		'SP_PublishingJs' : {
			deps: ['SP_Js']
        },

		'OfficeUiNavBar' : {
			deps: ['jQuery']
        },
		
		'OfficeUiContextualMenu' : {
			deps: ['jQuery']
        },
                		
		'TaxonomyModule' : {
			deps: ['SP_Js', 'SP_TaxonomyJs', 'SP_PublishingJs']
        },
    }
});

require(['domReady!',
		'jQuery', 
		'Knockout',
        'text!'],
		function (domReady, $, ko) {
            
	// At this moment, the DOM is already ready ;) (via domReady! dependency)
          
    // Register all components
    // Components files are loaded on demand via Require JS
    ko.components.register('component-mainmenu', { require: 'Components/component.mainmenu' });   
    ko.components.register('component-contextualmenu', { require: 'Components/component.contextualmenu' });  
    ko.components.register('component-breadcrumb', { require: 'Components/component.breadcrumb' });    
    
    // Add your additional component registration here  
    // ...
    
	// Insert the navbar component on the top of the "Oslo" master page
    // For this example, we use the "Site map" term set as main menu data source
	var tableRow = $(".contentwrapper").closest(".ms-tableRow");				     
    $("<div class=\"ms-NavBar\"><component-mainmenu params='termSetId: \"52d6944d-bd98-48c1-ba45-57d4efe2f941\"'></component-mainmenu></div>").insertBefore(tableRow);
        	
	// Hide the default SharePoint navigationmenu
	$("#DeltaHorizontalQuickLaunch").hide();
    
    // Apply the magic!
    ko.applyBindings();	     
});