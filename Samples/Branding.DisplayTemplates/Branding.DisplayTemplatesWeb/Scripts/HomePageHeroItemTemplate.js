/* This file is currently associated to an HTML file of the same name and is drawing content from it.  Until the files are disassociated, you will not be able to move, delete, rename, or make any other changes to this file. */

function DisplayTemplate_0af02e06fc1642f4835552a92b566a9f(ctx) {
    var ms_outHtml = [];
    var cachePreviousTemplateData = ctx['DisplayTemplateData'];
    ctx['DisplayTemplateData'] = new Object();
    DisplayTemplate_0af02e06fc1642f4835552a92b566a9f.DisplayTemplateData = ctx['DisplayTemplateData'];

    ctx['DisplayTemplateData']['TemplateUrl'] = '~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroItemTemplate.js';
    ctx['DisplayTemplateData']['TemplateType'] = 'Item';
    ctx['DisplayTemplateData']['TargetControlType'] = ['Content Web Parts'];
    this.DisplayTemplateData = ctx['DisplayTemplateData'];

    ctx['DisplayTemplateData']['ManagedPropertyMapping'] = { 'Title': ['Title'], 'Tag Line': ['brandingTagLineOWSTEXT'], 'Left Caption Background Color': ['brandingLeftCaptionBGColorOWSTEXT'], 'Left Caption Background Opacity': ['brandingLeftCaptionBGOpacityOWSTEXT'], 'HeroImage': ['brandingHeroImageOWSIMGE'], 'Hero URL Link': ['brandingLinkURLOWSTEXT'], 'Right Caption Title': ['brandingRightCaptionTitleOWSTEXT'], 'Right Caption Description': ['brandingRightCaptionDescriptionOWSMTXT'], 'Sort Order': ['brandingSortOrderOWSNMBR'], 'Display Start Date': ['BrandingDisplayStartDateOWSDATE'], 'Display End Date': ['BrandingDisplayEndDateOWSDATE\u0027\n'] };
    var cachePreviousItemValuesFunction = ctx['ItemValues'];
    ctx['ItemValues'] = function (slotOrPropName) {
        return Srch.ValueInfo.getCachedCtxItemValue(ctx, slotOrPropName)
    };

    ms_outHtml.push('', ''
    );
    var encodedId = $htmlEncode(ctx.ClientControl.get_nextUniqueId() + "_homePageItem_");

    var title = $getItemValue(ctx, "Title");
    var tagLine = $getItemValue(ctx, "Tag Line");
    var leftCaptionBGColor = $getItemValue(ctx, "Left Caption Background Color");
    var leftCaptionBGOpacity = $getItemValue(ctx, "Left Caption Background Opacity");
    var heroImage = $getItemValue(ctx, "HeroImage");
    var rightCaptionTitle = $getItemValue(ctx, "Right Caption Title");
    var rightCaptionDescription = $getItemValue(ctx, "Right Caption Description");
    var sortOrder = $getItemValue(ctx, "Sort Order");
    var displayStartDate = $getItemValue(ctx, "Display Start Date");
    var displayEndDate = $getItemValue(ctx, "Display End Date");

    var containerId = encodedId + "container";
    var leftContainerId = encodedId + "leftContainer";
    var rightContainerId = encodedId + "rightContainer";
    var homelink = $getItemValue(ctx, "Hero URL Link");
    var cursor = homelink.isEmpty ? "Default" : "Pointer";
    var onclick = homelink.isEmpty ? "return false;" : "location.href=\'" + homelink + "\'; return false;";
    ms_outHtml.push(''
   , '        <div class="homePageHeroItem-ImageContainer" title="', homelink, '" style="cursor:', cursor, '" onclick="', onclick, '">'
   , '            <img class="homePageHeroItem-Image" src="', heroImage, '" />'
   , '        </div>'
   , ''
   , '        <div class="homePageHeroItem-CaptionContainer" title="', homelink, '" style="cursor:', cursor, '" onclick="', onclick, '">'
   );
    if (!title.isEmpty || !tagLine.isEmpty) {
        ms_outHtml.push(''
        , '            <div class="homePageHeroItem-LeftCaption" style="background-color: ', '#' + leftCaptionBGColor, '; opacity: ', leftCaptionBGOpacity, '">'
        , '                <p class="homePageHeroItem-LeftCaptionTitle">'
        , '                    ', title, ''
        , '                </p>'
        , '                <p class="homePageHeroItem-LeftCaptionTagLine">'
        , '                    ', tagLine, ''
        , '                </p>'
        , '            </div>'
        );
    }
    ms_outHtml.push(''
    , '            <div class="homePageHeroItem-RightCaption">'
    , '                <p class="homePageHeroItem-RightCaptionTitle">'
    , '                    ', rightCaptionTitle, ''
    , '                </p>'
    , '                <p class="homePageHeroItem-RightCaptionDescription">'
    , '                    ', rightCaptionDescription, ''
    , '                </p>'
    , '            </div>'
    , '        </div>'
    , '    '
    );

    ctx['ItemValues'] = cachePreviousItemValuesFunction;
    ctx['DisplayTemplateData'] = cachePreviousTemplateData;
    return ms_outHtml.join('');
}
function RegisterTemplate_0af02e06fc1642f4835552a92b566a9f() {

    if ("undefined" != typeof (Srch) && "undefined" != typeof (Srch.U) && typeof (Srch.U.registerRenderTemplateByName) == "function") {
        Srch.U.registerRenderTemplateByName("Item_Home_Page_Hero", DisplayTemplate_0af02e06fc1642f4835552a92b566a9f);
    }

    if ("undefined" != typeof (Srch) && "undefined" != typeof (Srch.U) && typeof (Srch.U.registerRenderTemplateByName) == "function") {
        Srch.U.registerRenderTemplateByName("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroItemTemplate.js", DisplayTemplate_0af02e06fc1642f4835552a92b566a9f);
    }
    $includeLanguageScript("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroItemTemplate.js", "~sitecollection/_catalogs/masterpage/Display Templates/Language Files/{Locale}/CustomStrings.js");
}
RegisterTemplate_0af02e06fc1642f4835552a92b566a9f();
if (typeof (RegisterModuleInit) == "function" && typeof (Srch.U.replaceUrlTokens) == "function") {
    RegisterModuleInit(Srch.U.replaceUrlTokens("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroItemTemplate.js"), RegisterTemplate_0af02e06fc1642f4835552a92b566a9f);
}