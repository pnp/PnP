(function (window) {
    // create our namespace and define the global settings
    $.extend(window, {
        officepnp: {
            settings: {
                // REST URL used to load the configuration stored in SharePoint. This is expecting a list with title "config" having two columns "Title" and "Value"
                configLoadUrl: "https://318studios.sharepoint.com/sites/dev/_api/lists/getbytitle('config')/items?$select=Title,Value",
                // 0 = Verbose, 1 = Info, 2 = Warning, 3 = Error (messages below this level will not be logged)
                activeLoggingLevel: 0,
                // if you want to use the azure telemetry logging module enter your instrumentationKey below
                azureInsightsInstrumentationKey: '',
                //default timeout in minutes for the local storage
                localStorageDefaultTimeoutMinutes: 5
            }
        }
    });
})(window);

