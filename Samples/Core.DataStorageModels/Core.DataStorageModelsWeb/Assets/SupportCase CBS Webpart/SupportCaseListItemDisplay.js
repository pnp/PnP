/* This file is currently associated to an HTML file of the same name and is drawing content from it.  Until the files are disassociated, you will not be able to move, delete, rename, or make any other changes to this file. */

function DisplayTemplate_8118b8defec94f27affc4878260b90dc(ctx) {
    var ms_outHtml = [];
    var cachePreviousTemplateData = ctx['DisplayTemplateData'];
    ctx['DisplayTemplateData'] = new Object();
    DisplayTemplate_8118b8defec94f27affc4878260b90dc.DisplayTemplateData = ctx['DisplayTemplateData'];

    ctx['DisplayTemplateData']['TemplateUrl'] = '~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fSupportCaseListItemDisplay.js';
    ctx['DisplayTemplateData']['TemplateType'] = 'Item';
    ctx['DisplayTemplateData']['TargetControlType'] = ['Content Web Parts'];
    this.DisplayTemplateData = ctx['DisplayTemplateData'];

    ctx['DisplayTemplateData']['ManagedPropertyMapping'] = { 'Title': ['Title'], 'ID': ['ListItemId'], 'Status': ['FTCAMStatusOWSTEXT'], 'CSR': ['FTCAMCSROWSTEXT'], 'Customer ID': ['FTCAMCustomerIDOWSTEXT\u0027\r\n    '] };
    var cachePreviousItemValuesFunction = ctx['ItemValues'];
    ctx['ItemValues'] = function (slotOrPropName) {
        return Srch.ValueInfo.getCachedCtxItemValue(ctx, slotOrPropName)
    };

    ms_outHtml.push('', ''
    );

    var mID = $getItemValue(ctx, "ID");
    mID.overrideValueRenderer($contentLineText);

    var mTitle = $getItemValue(ctx, "Title");
    mTitle.overrideValueRenderer($contentLineText);

    var mStatus = $getItemValue(ctx, "Status");
    mStatus.overrideValueRenderer($contentLineText);

    var mCSR = $getItemValue(ctx, "CSR");
    mCSR.overrideValueRenderer($contentLineText);

    var statusClass = "status Resolve_status";
    if (mStatus == "Open") {
        statusClass = "status Open_status";
    }
    ms_outHtml.push(''
    );
    ms_outHtml.push(''
    , '        <li>'
    , '            <div class="id">', mID, '</div>'
    , '            <div class="', statusClass, '">', mStatus, '</div>'
    , '            <div class="title">', mTitle, '</div>'
    , '            <div class="csr">', mCSR, '</div>'
    , '        </li>'
    , '    '
    );

    ctx['ItemValues'] = cachePreviousItemValuesFunction;
    ctx['DisplayTemplateData'] = cachePreviousTemplateData;
    return ms_outHtml.join('');
}
function RegisterTemplate_8118b8defec94f27affc4878260b90dc() {

    if ("undefined" != typeof (Srch) && "undefined" != typeof (Srch.U) && typeof (Srch.U.registerRenderTemplateByName) == "function") {
        Srch.U.registerRenderTemplateByName("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fSupportCaseListItemDisplay.js", DisplayTemplate_8118b8defec94f27affc4878260b90dc);
    }

}
RegisterTemplate_8118b8defec94f27affc4878260b90dc();
if (typeof (RegisterModuleInit) == "function" && typeof (Srch.U.replaceUrlTokens) == "function") {
    RegisterModuleInit(Srch.U.replaceUrlTokens("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fSupportCaseListItemDisplay.js"), RegisterTemplate_8118b8defec94f27affc4878260b90dc);
}