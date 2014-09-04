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
            return form.valid();

        },
        onFinished: function (event, currentIndex) {

            alert('Form is valid and ready to rock!');            

            // Submit form input
            //var form = $(this);
            //form.submit();

        }
    }).validate({
        errorPlacement: function (error, element) {

            element.before(error);

        },
        rules: {
            
        },
        messages: {

            name: 'Please enter a name for the site',
            url: 'Please enter a URL for the site'

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

        }

        // remove any errors from unselected tiles
        $('.selection-error').hide();

    });

}

$(document).ready(function () {

    initiateWizard();
    initiateTileSelector();

});