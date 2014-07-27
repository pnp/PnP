/* jshint jquery: true */
/* global SPListmanager */
/* global SP */

(function () {
    'use strict';

    SPListmanager.context = SP.ClientContext.get_current();
    SPListmanager.user = SPListmanager.context.get_web().get_currentUser();

    $(document).ready(function () {

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

        SPListmanager.NewList.Translate();

        $.getScript(SPListmanager.scriptbase + 'SP.Runtime.js',
           function () {
               $.getScript(SPListmanager.scriptbase + 'SP.js',
                   function () { $.getScript(SPListmanager.scriptbase + 'SP.RequestExecutor.js', SPListmanager.NewList.init); }
               );
           }
       );
    });
}());