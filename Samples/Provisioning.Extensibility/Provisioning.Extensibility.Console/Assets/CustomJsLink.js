// anonymous self-executing function to setup JSLink templates on page load..
( function ( ) {
	var overrideCtx = { };
	overrideCtx.Templates = { };

	overrideCtx.Templates.Header = '<ul id="custom_links">';

	overrideCtx.Templates.Item = function (ctx) {
		return '<li>' + ctx.CurrentItem.URL + '</li>';
	};

	overrideCtx.Templates.Footer = '</ul>';

	//Using PnP Provisioning we can't set the BaseViewID in the JSLink
	//as there is no way to set the specific BaseViewID when the List/View is provisioned (it always set the value to 1)
	//you can get more info the AddListViewWebpart function in PageHelper class
	//The inconvenient here is that you can't have 2 List View WP pointing to the same List in the same page
	//because both WPs will use the jsLink (not sure why, as if you check the WPs in the page using Client browser tool
	//you will see how each WP is pointing to a different View in the List, and the Views in the list have the right JSLink value)
	//overrideCtx.BaseViewID = 1;

	SPClientTemplates.TemplateManager.RegisterTemplateOverrides ( overrideCtx );
} ) ( );