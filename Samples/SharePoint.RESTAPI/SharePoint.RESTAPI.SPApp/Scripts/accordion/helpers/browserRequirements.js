$(function () {
    var supportsMediaQuery = Modernizr.mediaqueries;
    var supportsBeforeAfter = Modernizr.generatedcontent;
    var supportsLocalStorage = Modernizr.localstorage;

    if (!(supportsMediaQuery && supportsBeforeAfter && supportsLocalStorage)) {
        var unsupportedDiv = $('#unsupported');
        unsupportedDiv.show();
        unsupportedDiv.click(function () {
            unsupportedDiv.hide();
        });
    }
});