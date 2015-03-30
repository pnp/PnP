(function () {
    // Intialize the variables for overrides objects
    var overrideCtx = {};
    overrideCtx.Templates = {};

    // Override field data
    overrideCtx.Templates.Fields = {
        // PercentComplate = internal name of the % Complete
        // View = you want to change the field rendering of a view
        // <dev ... = here we define what the output of the field will be.
        'PercentComplete': { 'View': '<div class="progress" data-toggle="tooltip" data-placement="right" title="<#=ctx.CurrentItem.PercentComplete.replace(" %", "")#>%"><div class="progress-bar" role="progressbar" aria-valuenow="<#=ctx.CurrentItem.PercentComplete.replace(" %", "")#>" aria-valuemin="0" aria-valuemax="100" style="width: <#=ctx.CurrentItem.PercentComplete.replace(" %", "")#>%;"></div></div>' }
    };

    // Register the override of the field
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
})();



(function () {
    "use strict";

    if (typeof (_spBodyOnLoadCalled) === 'undefined' || _spBodyOnLoadCalled) {
        load();
    }
    else {
        _spBodyOnLoadFunctions.push(load);
    }

    function load () {
        CDNManager.getScript(['jquery-1.11.2.min.js','bootstrap.min.js'], ready);
    };

    function ready () {
        jQuery('[data-toggle="tooltip"]').tooltip();
    };


}());








