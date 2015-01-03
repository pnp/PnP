'use strict';

(function ($) {

    $(document).ready(loadEverything);

    function loadEverything() {

        var isDialog = getQueryStringParameterByName('IsDlg');
        if (isDialog == '1') {
            $("#globalNavBox").hide();
        }

        var queryUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/search/query?Querytext='*'" +
					"&Properties='GraphQuery:ACTOR(ME\\, action\\:1019),GraphRankingModel:{\"features\"\\:[{\"function\"\\:\"EdgeWeight\"}]}'" +
					"&RankingModelId='0c77ded8-c3ef-466d-929d-905670ea1d72'" +
					"&SelectProperties='Title,UserName,Path'" +
					"&RowLimit=10";

        $.ajax({
            url: queryUrl,
            method: "GET",
            headers: { "Accept": "application/json; odata=nometadata" },
            success: function (data) {
                var results = [];

                $(data.PrimaryQueryResult.RelevantResults.Table.Rows).each(function (i, e) {
                    var o = {};
                    $(e.Cells).each(function (ii, ee) {

                        if (ee.Key == 'Title')
                            o.title = ee.Value;
                        else if (ee.Key == 'UserName')
                            o.username = ee.Value;
                        else if (ee.Key == 'Path')
                            o.path = ee.Value;

                    });

                    //build an image
                    o.pic = _spPageContextInfo.webAbsoluteUrl + '/_layouts/15/userphoto.aspx?size=m&accountname=' + o.username;

                    results.push(o);
                })
                RenderEverything(results);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(textStatus + " : " + errorThrown);
            }
        });

    }

    function RenderEverything(results) {

        var previewsContainer = $("#OGPreviews");

        $(results).each(function (i, e) {

            var theImg = $("<a target='_blank' href='" + e.path + "'><img src=" + e.pic + " title='" + e.title + "'></img></a>");

            previewsContainer.append(theImg);

        })
    }

    function getQueryStringParameterByName(name) {
        var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
        return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
    }

})(jQuery);