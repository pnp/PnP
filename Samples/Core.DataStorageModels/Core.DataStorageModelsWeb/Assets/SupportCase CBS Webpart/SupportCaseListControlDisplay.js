/* This file is currently associated to an HTML file of the same name and is drawing content from it.  Until the files are disassociated, you will not be able to move, delete, rename, or make any other changes to this file. */

function DisplayTemplate_eb36fe9514f748a4a7f815e1889d6592(ctx) {
    var ms_outHtml = [];
    var cachePreviousTemplateData = ctx['DisplayTemplateData'];
    ctx['DisplayTemplateData'] = new Object();
    DisplayTemplate_eb36fe9514f748a4a7f815e1889d6592.DisplayTemplateData = ctx['DisplayTemplateData'];

    ctx['DisplayTemplateData']['TemplateUrl'] = '~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fSupportCaseListControlDisplay.js';
    ctx['DisplayTemplateData']['TemplateType'] = 'Control';
    ctx['DisplayTemplateData']['TargetControlType'] = ['Content Web Parts'];
    this.DisplayTemplateData = ctx['DisplayTemplateData'];

    ms_outHtml.push('', ''
    );
    if (!$isNull(ctx.ClientControl) &&
        !$isNull(ctx.ClientControl.shouldRenderControl) &&
        !ctx.ClientControl.shouldRenderControl()) {
        return "";
    }
    ctx.ListDataJSONGroupsKey = "ResultTables";
    var $noResults = Srch.ContentBySearch.getControlTemplateEncodedNoResultsMessage(ctx.ClientControl);

    var noResultsClassName = "ms-srch-result-noResults";

    var ListRenderRenderWrapper = function (itemRenderResult, inCtx, tpl) {
        var iStr = [];
        iStr.push(itemRenderResult);
        return iStr.join('');
    }
    ctx['ItemRenderWrapper'] = ListRenderRenderWrapper;

    ctx.OnPostRender = [];

    ctx.OnPostRender.push(function () {
    });

    ms_outHtml.push(''
    , '        <div class="cdsm_common_display">'
    , '            <div class="cdsm_title">Support Cases - Via Content By Search Web Part</div>'
    , '                <div id="supportcaseList">'
    , '                    <ul>'
    , '                        <li>'
    , '                            <div class="id">ID</div>'
    , '                            <div class="status">Status</div>'
    , '                            <div class="title">Title</div>'
    , '                            <div class="csr">CSR</div>'
    , '                        </li>'
    , '                        ', ctx.RenderGroups(ctx), ''
    , '                    </ul>'
    , '                </div>'
    , '            </div>'
    , '    '
    );

    ctx['DisplayTemplateData'] = cachePreviousTemplateData;
    return ms_outHtml.join('');
}
function RegisterTemplate_eb36fe9514f748a4a7f815e1889d6592() {

    if ("undefined" != typeof (Srch) && "undefined" != typeof (Srch.U) && typeof (Srch.U.registerRenderTemplateByName) == "function") {
        Srch.U.registerRenderTemplateByName("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fSupportCaseListControlDisplay.js", DisplayTemplate_eb36fe9514f748a4a7f815e1889d6592);
    }

}
RegisterTemplate_eb36fe9514f748a4a7f815e1889d6592();
if (typeof (RegisterModuleInit) == "function" && typeof (Srch.U.replaceUrlTokens) == "function") {
    RegisterModuleInit(Srch.U.replaceUrlTokens("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fSupportCaseListControlDisplay.js"), RegisterTemplate_eb36fe9514f748a4a7f815e1889d6592);
}