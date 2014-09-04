/// <reference path="../../Scripts/_references.js" />

function initiateWizard() {
    
    // instantiate the wizard
    $("#wizard").steps({
        bodyTag: 'fieldset',
        stepsOrientation: 'vertical',
        enableCancelButton: false,
        //enableFinishButton: true,
        showFinishButtonAlways: false,
        transitionEffect: 'slide',
        titleTemplate: '<span class="number">#index#</span>',

        /* Labels */
        labels: {
            finish: '',
            next: '',
            previous: ''
        },

        /* Events */
        onStepChanging: function (event, currentIndex, newIndex) {

            if($('.media.selected').length > 0) {
                
                // hide any existing errors
                $('.selection-error').hide();

                // continue the wizard
                return true;

            }
            else {

                // there are not any media elements that have been selected, show error
                $('.selection-error').show();

            }
            
        },
        onFinishing: function (event, currentIndex) {

            var form = $(this);

            // Disable validation on fields that are disabled.
            // At this point it's recommended to do an overall check (mean ignoring only disabled fields)
            form.validate().settings.ignore = ":disabled";

            // Start validation; Prevent form submission if false
            return true;//form.valid();

        },
        onFinished: function (event, currentIndex) {

            // alert('Form is valid and ready to rock!');            

            // Submit form input
            //var form = $(this);
            //form.submit();
            $('#submitForm').click();
        }
    }).validate({
        errorPlacement: function (error, element) {

            element.before(error);

        },
        rules: {

        },
        messages: {

            name: 'Please enter a name for the site',
            url: 'Please enter a URL for the site '

        },
        highlight: function (element) {

            // remove any existing bootstrap classes and add error classes
            $(element).closest('.form-group').removeClass('has-success').addClass('has-error has-feedback');

        },
        success: function (element) {

            // remove any existing error classes and add success class
            element.closest('.form-group').removeClass('has-error').addClass('has-success has-feedback');

        }
    });

}

function initiateTileSelector() {

    $('.media').click(function () {        

        // check to see if the site type is already selected
        if ($(this).hasClass('selected')) {

            // if yes, toggle it to unselected
            $(this).removeClass('selected');

        }
        else {

            // remove any preselected site types
            $('.media').removeClass('selected');

            // set the specific site to selected
            $(this).addClass('selected');
            var template = $(this).data('tmpname');//$(this).data('tmpid') + '|' + '|' + $(this).data('tmptitle');
            $('#Template').val(template);
        }

        // remove any errors from unselected tiles
        $('.selection-error').hide();

    });

}

var search_timeout;
$(document).ready(function () {

    initiateWizard();
    initiateTileSelector();

    $('#SiteUrl').bind('keyup', function () {
        if ($(this).val().length >= 3) {
            if (search_timeout != undefined) {
                clearTimeout(search_timeout);
            }
            var $this = this; // save reference to 'this' so we can use it in timeout function
            search_timeout = setTimeout(function () {
                search_timeout = undefined;
                // do stuff with $this here
                checkUrlIfExists();
            }, 1200);
        }
        else {
            $('#SiteUrlExists').text('');
        }
    }).blur(checkUrlIfExists);
});

function checkUrlIfExists() {
    var siteUrl = $('#SiteUrl').val();
    var url = __hostUrl + siteUrl;
    var requestUrl =
        __appSiteUrl + "/Provisioning/SiteExists?" +
        "url=" + encodeURIComponent(url) +
        "&SPHostUrl=" + encodeURIComponent(__hostUrl) +
        "&SPLanguage=" + encodeURIComponent(__language);

    $.get(requestUrl, "")
        .success(function (result) {
            var exists = result;
            
            if (exists) { $('#SiteUrlExists').text('Sorry, exists already.').css('color', 'DarkRed'); }
            else { $('#SiteUrlExists').text("It's yours.").css('color', 'Green'); }

        }).error(function (ex) {
            console.debug('Problem getting data from service' + ex);
        });
}