$(function () {
    $('#dialog').dialog({
        autoOpen: false,
        modal: true,
        height: 652,
        width: 930
    });
});


function LaunchWizard() {

    $('.ui-dialog .ui-dialog-titlebar').hide();
    $('.ui-dialog .ui-dialog-content').css('padding', '0');
    $('.ui-corner-all').css('border-radius', '0');

    var dlgW = 919;
    var dlgH = 639;

    $('#dialog iframe').attr('src', '/Wizard?SPHostUrl=' + encodeURIComponent(__hostUrl))
                       .width(dlgW)
                       .height(dlgH);
    
    $('#dialog').dialog('open');
    $('.ui-dialog').css('padding', '0');
}