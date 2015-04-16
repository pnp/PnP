// List View – Accordion Sample
// Muawiyah Shannak , @MuShannak
// Modified by Canviz LLC for inclusion in Office PnP

if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    RegisterInMDS();
}
else {
    RegisterAccordionContext();
}

function RegisterInMDS() {
    // RegisterAccordionContext-override for MDS enabled site
    RegisterModuleInit(_spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/Accordion.js", RegisterAccordionContext);
    //RegisterAccordionContext-override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
    RegisterAccordionContext();
}

function RegisterAccordionContext() {

    // jQuery library is required in this sample
    // Fallback to loading jQuery from a CDN path if the local is unavailable
    (window.jQuery || document.write('<script src="//ajax.aspnetcdn.com/ajax/jquery/jquery-1.10.0.min.js"><\/script>'));

    // Create object that has the context information about the field that we want to render differently 
    var accordionContext = {};
    accordionContext.Templates = {};

    // Be careful when adding the header for the template, because it will break the default list view render
    accordionContext.Templates.Header = "<div class='accordion'>";
    accordionContext.Templates.Footer = "</div>";

    // Add OnPostRender event handler to add accordion click events and style
    accordionContext.OnPostRender = accordionOnPostRender;

    // This line of code tells the TemplateManager that we want to change all the HTML for item row rendering
    accordionContext.Templates.Item = accordionTemplate;

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(accordionContext);

}

// This function provides the rendering logic
function accordionTemplate(ctx) {
    var title = ctx.CurrentItem["Title"];
    var description = ctx.CurrentItem["Description"];

    // Return whole item html
    return "<h2>" + title + "</h2><p>" + description + "</p><br/>";
}

function accordionOnPostRender() {

    // Register event to collapse and expand when clicking on accordion header
    $('.accordion h2').click(function () {
        $(this).next().slideToggle();
    }).next().hide();

    $('.accordion h2').css('cursor', 'pointer');
}