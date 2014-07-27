// variable used for cross site CSOM calls
var context;
// variable to hold index of intialized taxPicker controls
var taxPickerIndex = {};

//Wait for the page to load
$(document).ready(function () {

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + 'SP.Runtime.js',
        function () {
            $.getScript(layoutsRoot + 'SP.js',
                function () {
                    //Load the SP.UI.Controls.js file to render the App Chrome
                    $.getScript(layoutsRoot + 'SP.UI.Controls.js', renderSPChrome);

                    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
                    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
                        context = new SP.ClientContext(appWebUrl);
                        var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                        context.set_webRequestExecutorFactory(factory);
                    });

                    //load scripts for calling taxonomy APIs
                    $.getScript(layoutsRoot + 'init.js',
                        function () {
                            $.getScript(layoutsRoot + 'sp.taxonomy.js',
                                function () {
                                    //bind the taxonomy picker to the default keywords termset
                                    //$('#taxPickerKeywords').taxpicker({ isMulti: true, allowFillIn: true, useKeywords: true }, context);

                                    $('#taxPickerContinent').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", levelToShowTerms: 1 }, context, initializeCountryTaxPicker);
                                    taxPickerIndex["#taxPickerContinent"] = 0;
                                });
                        });
                });
        });
});

function initializeCountryTaxPicker() {
    if (this._selectedTerms.length > 0) {
        $('#taxPickerCountry').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2, useTermSetasRootNode: false }, context, initializeRegionTaxPicker);
        taxPickerIndex["#taxPickerCountry"] = 4;
    }
}

function initializeRegionTaxPicker() {
    if (this._selectedTerms.length > 0) {
        $('#taxPickerRegion').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: "0cc96f04-d32c-41e7-995f-0401c1f4fda8", filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 3, useTermSetasRootNode: false }, context);
        taxPickerIndex["#taxPickerRegion"] = 5;
    }
}

function getValue(propertyName) {
    if (taxPickerIndex != null) {
        return taxPickerIndex[propertyName];
    }
};

//function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}

function chromeLoaded() {
    $('body').show();
}

//function callback to render chrome after SP.UI.Controls.js loads
function renderSPChrome() {
    var icon = decodeURIComponent(getQueryStringParameter('SPHostLogoUrl'));

    //Set the chrome options for launching Help, Account, and Contact pages
    var options = {
        'appTitle': document.title,
        'appIconUrl': icon,
        'onCssLoaded': 'chromeLoaded()'
    };

    //Load the Chrome Control in the divSPChrome element of the page
    var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
    chromeNavigation.setVisible(true);
}
