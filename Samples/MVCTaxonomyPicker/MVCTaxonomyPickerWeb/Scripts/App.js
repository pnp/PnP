var taxPickerIndex = {};
(function ($) {
    $(document).ready(function () {
        var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));

        //Initiate taxpickers 
        $('#DemoControl').taxpicker({ isMulti: false, allowFillIn: true, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', termSetImageUrl: '/Content/Images'});

        $('#Demo1Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', levelToShowTerms: 1, termSetImageUrl: '/Content/Images' }, function () {
            $('#Demo2Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 2, useTermSetasRootNode: false, termSetImageUrl: '/Content/Images' }, function () {
                $('#Demo3Control').taxpicker({ isMulti: false, allowFillIn: false, useKeywords: false, termSetId: 'f9a12d1b-7c94-467e-8687-70794a83211f', filterTermId: this._selectedTerms[0].Id, levelToShowTerms: 3, useTermSetasRootNode: false, termSetImageUrl: '/Content/Images' });
            });
        });
        taxPickerIndex["#Demo1Control"] = 0;
        taxPickerIndex["#Demo2Control"] = 4;
        taxPickerIndex["#Demo3Control"] = 5;


        $('#btnSubmit').on('click', function () {            
            $.ajax({
                url: '/Home/GetTaxonomyPickerHiddenValue?SPHostUrl=' + spHostUrl,
                type: 'POST',
                data: {                   
                    Demo: JSON.parse($('#Demo').val()),
                    Demo1: JSON.parse($('#Demo1').val()),
                    Demo2: JSON.parse($('#Demo2').val()),
                    Demo3: JSON.parse($('#Demo3').val())
                },
                success: function (msg) {
                    console.log(msg)
                },
                error: function (textStatus, errorThrown) {
                    console.log(textStatus)
                }
            });
        });

        $('#btnCancel').on('click', function () {
            window.location = spHostUrl;
        });
    });

}(jQuery));


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