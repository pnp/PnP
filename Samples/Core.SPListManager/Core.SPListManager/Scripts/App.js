/* jshint jquery: true */
/* global SPListmanager */
/* global SP */

(function () {
    'use strict';

    SPListmanager.context = SP.ClientContext.get_current();
    SPListmanager.user = SPListmanager.context.get_web().get_currentUser();


    $(document).ready(function () {

        //set new list url params
        $("#appadd").attr("href", "newList.aspx?" + document.URL.split("?")[1]);
        $("#appadd-link").attr("href", "newList.aspx?" + document.URL.split("?")[1]);

        SPListmanager.hostweburl =
            decodeURIComponent(
                SPListmanager.GetQueryStringParameter('SPHostUrl')
        );
        SPListmanager.appweburl =
            decodeURIComponent(
                SPListmanager.GetQueryStringParameter('SPAppWebUrl')
         );

        SPListmanager.scriptbase = SPListmanager.hostweburl + '/_layouts/15/';
        SPListmanager.imagesbase = SPListmanager.hostweburl + '/_layouts/images/';

        //Translate the page to the current language
        SPListmanager.Default.Translate();

        //show working on it
        $("#message").html("<div style='margin:auto;width:500px;height:500px;margin-top:200px;'><h3>Working on it...</h3><img src='" + SPListmanager.imagesbas + "/loading.gif' alt='loading' /> This shouln't take long.<div>");

        //load RequestExecutor.js and execute our init method
        $.getScript(SPListmanager.scriptbase + 'SP.Runtime.js',
           function () {
               $.getScript(SPListmanager.scriptbase + 'SP.js',
                   function () { $.getScript(SPListmanager.scriptbase + 'SP.RequestExecutor.js', SPListmanager.Default.init); }
               );
           }
       );
    });
}());



