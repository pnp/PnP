// ====================
// Taxonomy module
// ====================
define([], function () {
	
	var taxonomyModule = function() {
                
        this.getTermSetCustomPropertyValue = function (termSetId, customPropertyName) {
            
            var deferred = new $.Deferred();
            var context = SP.ClientContext.get_current();

			var taxSession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
			var termStore = taxSession.getDefaultSiteCollectionTermStore();
			var termSet = termStore.getTermSet(termSetId);
            
            context.load(termSet, "CustomProperties");       
            
            context.executeQueryAsync(function () {

				var propertyValue = termSet.get_objectData().get_properties()["CustomProperties"][customPropertyName] !== undefined ? termSet.get_objectData().get_properties()["CustomProperties"][customPropertyName] : "";
			
				deferred.resolve(propertyValue);

			}, function (sender, args) {

				deferred.reject(sender, args);
			});

			return deferred.promise();      
        };
	
		this.getNavigationTaxonomyNodes = function (termSetId) {

			var deferred = new $.Deferred();

			var context = SP.ClientContext.get_current();
			var currentWeb = SP.ClientContext.get_current().get_web();

			var taxSession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
			var termStore = taxSession.getDefaultSiteCollectionTermStore();
			var termSet = termStore.getTermSet(termSetId);

			// The method 'getTermSetForWeb' gets the cached read only version of the term set
			// https://msdn.microsoft.com/EN-US/library/office/microsoft.sharepoint.publishing.navigation.taxonomynavigation.gettermsetforweb.aspx
			// Ex: var webNavigationTermSet = SP.Publishing.Navigation.TaxonomyNavigation.getTermSetForWeb(context, currentWeb, 'GlobalNavigationTaxonomyProvider', true);
			// In our case, we use 'getAsResolvedByWeb' method instead to retrieve a taxonomy term set as a navigation term set regardless if it is bound to the current web.
            // The downside of this approach is that the results are not retrieved from the navigation cache that can cause performance issues during the initial load
			var webNavigationTermSet = SP.Publishing.Navigation.NavigationTermSet.getAsResolvedByWeb(context, termSet, currentWeb, 'GlobalNavigationTaxonomyProvider');
            
            // Get the existing view from the navigation term set
            var termSetView = webNavigationTermSet.get_view().getCopy();
            
            // Return global and current navigation terms (the subsequent filtering will occur in the Knockout html view)
            termSetView.set_excludeTermsByProvider(false);
            
            // Sets a value that indicates whether NavigationTerm objects are trimmed if the current user does not have permissions to view the target page (the aspx physical page) for the friendly URL
            // If you don't see anything in the menu, check the node type (term driven page or simple link). In the case of term driven page, the target page must be accessible for the current user 
            termSetView.set_excludeTermsByPermissions(true);
            
            // Apply the new view filters
            webNavigationTermSet = webNavigationTermSet.getWithNewView(termSetView);
            
            var firstLevelNavigationTerms = webNavigationTermSet.get_terms();
            var allNavigationterms = webNavigationTermSet.getAllTerms();
            
            context.load(allNavigationterms, 'Include(Id, Terms, Title, FriendlyUrlSegment, ExcludeFromCurrentNavigation, ExcludeFromGlobalNavigation)');
            context.load(firstLevelNavigationTerms, 'Include(Id, Terms, Title, FriendlyUrlSegment, ExcludeFromCurrentNavigation, ExcludeFromGlobalNavigation)');

            context.executeQueryAsync(function () {

                getTermNodesAsFlat(context, allNavigationterms).then(function (nodes) {
                        
                    var navigationTree = getTermNodesAsTree(context, nodes, firstLevelNavigationTerms, null);

                    deferred.resolve(navigationTree);

                }, onError);

            }, function (sender, args) {
                deferred.reject(sender, args);
            });
				
			return deferred.promise();
		};
		
		// Get the navigation hierarchy as a flat list
		// This list will be used to easily find a node without dealing too much with asynchronous calls and recursion 
		var getTermNodesAsFlat = function (context, allTerms) {

			function getSingleTermNodeInfo(fn){

				if (i < termCount)
				{
					var currentTerm = termsEnumerator.get_current();
					var termNode = {
						"Id": currentTerm.get_id().toString(),
						"Title": currentTerm.get_title().get_value(),
						"Url": "",
						"TaxonomyTerm": currentTerm,
						"FriendlyUrlSegment": currentTerm.get_friendlyUrlSegment().get_value(),
						"ChildNodes": [],
                        "ParentUrl" : "",
						"IconCssClass" : "",
                        "ExcludeFromGlobalNavigation" : currentTerm.get_excludeFromGlobalNavigation(),
                        "ExcludeFromCurrentNavigation" : currentTerm.get_excludeFromCurrentNavigation()
					};
									
					getNavigationTermUrlInfo(context, currentTerm).then(function (termUrlInfo) {

						termNode.Url = termUrlInfo.ResolvedDisplayUrl;
                        
						getTermCustomPropertyValue(context, currentTerm.getTaxonomyTerm(), "IconCssClass").then(function (iconCssClass) {
								
							termNode.IconCssClass = iconCssClass;
							termNodes.push(termNode);
							i++;
							termsEnumerator.moveNext();
							getSingleTermNodeInfo(fn);
						
						}, onError); 
					}, onError);           
				}
				else
				{
					fn(termNodes);
				}       
			}

			var deferred = new $.Deferred();

			var termsEnumerator = allTerms.getEnumerator();
			var termCount = allTerms.get_count();
			var i = 0;
			var termNodes = [];

			termsEnumerator.moveNext();
			getSingleTermNodeInfo(function (navNodes) {

				deferred.resolve(navNodes);

			});

			return deferred.promise();
		};
	 
		// Find a specific navigation term in the flat list of all navigation terms
		var findTermNode = function (allTerms, termId) {
	 
			for (var i = 0; i < allTerms.length; i++) {

				if (allTerms[i].Id.localeCompare(termId.toString()) === 0)
				{
					return allTerms[i];
				}
			}
			return null;
		};

		var getTermNodesAsTree = function (context, allTerms, currentNodeTerms, parentNode) {

			// Special thanks to this blog post
            // https://social.msdn.microsoft.com/Forums/office/en-US/ede1aa39-4c47-4308-9aef-3b036ec9b318/get-navigation-taxonomy-term-tree-in-sharepoint-app?forum=appsforsharepoint
			var termsEnumerator = currentNodeTerms.getEnumerator();
			var termNodes = [];

			while (termsEnumerator.moveNext()) {

				// Get the corresponding navigation node in the flat tree
				var currentNode = findTermNode(allTerms, termsEnumerator.get_current().get_id().toString());
		          
				var subTerms = currentNode.TaxonomyTerm.get_terms();
				if (subTerms.get_count() > 0) {

					currentNode.ChildNodes = getTermNodesAsTree(context, allTerms, subTerms, currentNode);
				}
                    
                // Clear TaxonomyTerm property to simplify JSON string (property not useful anymore after this step)
                currentNode.TaxonomyTerm = null;
                
                if (parentNode !== null) {
                    
                    // Set the parent infos for the current node (used by the contextual menu and the breadcrumb components)            
                    currentNode.ParentUrl = parentNode.Url;
                    
                } else {
                    
                    currentNode.ParentUrl = null;
                }

				termNodes.push(currentNode);
			}

			return termNodes;
		};

		var getNavigationTermUrlInfo = function (context, navigationTerm) {

            var termUrlInfo = {
                "ResolvedDisplayUrl": "",
            };

			var deferred = new $.Deferred();

			// This method gets the resolved URL whatever if it is a simple link or a friendly URL
			var resolvedDisplayUrl = navigationTerm.getResolvedDisplayUrl();

			context.load(navigationTerm);

			context.executeQueryAsync(function () {

                termUrlInfo.ResolvedDisplayUrl = resolvedDisplayUrl.get_value();
                
                deferred.resolve(termUrlInfo);

			}, function (sender, args) {

				deferred.reject(sender, args);
			});

			return deferred.promise();
		};
        
        var getTermCustomPropertyValue = function (context, taxonomyTerm, customPropertyName) {
			
			var deferred = new $.Deferred();
			
			context.load(taxonomyTerm, 'CustomProperties');

			context.executeQueryAsync(function () {

				var propertyValue = taxonomyTerm.get_objectData().get_properties()["CustomProperties"][customPropertyName] !== undefined ? taxonomyTerm.get_objectData().get_properties()["CustomProperties"][customPropertyName] : "";
			
				deferred.resolve(propertyValue);

			}, function (sender, args) {

				deferred.reject(sender, args);
			});

			return deferred.promise();			
		};
		          
        var onError = function (sender, args) {
            console.log('Error. ' + args.get_message() + '\n' + args.get_stackTrace());
        };
	};
	
	return taxonomyModule;	
});

