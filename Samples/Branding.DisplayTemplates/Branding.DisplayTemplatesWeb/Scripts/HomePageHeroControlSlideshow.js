/* This file is currently associated to an HTML file of the same name and is drawing content from it.  Until the files are disassociated, you will not be able to move, delete, rename, or make any other changes to this file. */

function DisplayTemplate_8aea791f157a4c608c0d6b1605c58b41(ctx) {
    var ms_outHtml = [];
    var cachePreviousTemplateData = ctx['DisplayTemplateData'];
    ctx['DisplayTemplateData'] = new Object();
    DisplayTemplate_8aea791f157a4c608c0d6b1605c58b41.DisplayTemplateData = ctx['DisplayTemplateData'];

    ctx['DisplayTemplateData']['TemplateUrl'] = '~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroControlSlideshow.js';
    ctx['DisplayTemplateData']['TemplateType'] = 'Control';
    ctx['DisplayTemplateData']['TargetControlType'] = ['Content Web Parts'];
    this.DisplayTemplateData = ctx['DisplayTemplateData'];

    ms_outHtml.push('', ''
    );
    var $noResults = Srch.ContentBySearch.getControlTemplateEncodedNoResultsMessage(ctx.ClientControl);

    if (!$isNull(ctx.ClientControl) &&
        !$isNull(ctx.ClientControl.shouldRenderControl) &&
        !ctx.ClientControl.shouldRenderControl()) {
        return "";
    }
    ctx.ListDataJSONGroupsKey = "ResultTables";

    var imagesHtml = [];
    var captionsHtml = [];
    var numResults = 0;
    var SlideShowRenderWrapper = function (itemRenderResult, inCtx, tpl) {
        regexp = new RegExp('(<div.*?>.*?</div>)\\s*(<div.*?>.*</div>)\\s*')
        matches = itemRenderResult.match(regexp);

        imgStr = [];
        imgStr.push('<li>');
        imgStr.push(matches[1]);
        imgStr.push('</li>');
        imagesHtml.push(imgStr.join(''));

        cptnStr = [];
        cptnStr.push('<li>');
        cptnStr.push(matches[2]);
        cptnStr.push('</li>');
        captionsHtml.push(cptnStr.join(''));

        numResults++;
        return '';
    };
    ctx['ItemRenderWrapper'] = SlideShowRenderWrapper;

    var timeoutid;
    window.hph_Slideshow_init = function () {
        if (numResults < 2) {
            return;
        }
        // set first navigation control active.
        firstNavControl = $(".homePageHeroControl-SlideshowPagingBar a:first");
        firstNavControl.removeClass('homePageHeroControl-SlideshowPagingLink-Inactive').addClass('homePageHeroControl-SlideshowPagingLink-Active');

        timoutid = setTimeout("window.hph_startSlide();", 5000);
    };

    window.hph_addSlideMouseHandlers = function () {
        slideItems = $(".homePageHeroControl-SlideshowItems ul").mouseenter(function () { clearTimeout(timeoutid); }).mouseleave(function () { window.hph_startSlide(); });
    }

    window.hph_slideOnce = function () {
        // change image
        $(".homePageHeroControl-SlideshowImageItems ul").animate({ "marginLeft": "-1180px" }, 500, function () {
            $(".homePageHeroControl-SlideshowImageItems ul li").eq(-1).after($(".homePageHeroControl-SlideshowImageItems ul li").eq(0));
            $(".homePageHeroControl-SlideshowImageItems ul").css({ "marginLeft": "0px" });
        });

        // change caption
        $(".homePageHeroControl-SlideshowCaptionItems ul").css({ "marginLeft": "-1180px" });
        $(".homePageHeroControl-SlideshowCaptionItems ul li").eq(-1).after($(".homePageHeroControl-SlideshowCaptionItems ul li").eq(0));
        $(".homePageHeroControl-SlideshowCaptionItems ul").css({ "marginLeft": "0px" });

        // change navigation control
        currentNavControl = $(".homePageHeroControl-SlideshowPagingLink-Active");
        nextNavControl = currentNavControl.next();
        if (nextNavControl.length == 0) {
            nextNavControl = currentNavControl.siblings(":first");
        }
        currentNavControl.removeClass('homePageHeroControl-SlideshowPagingLink-Active').addClass('homePageHeroControl-SlideshowPagingLink-Inactive');
        nextNavControl.removeClass('homePageHeroControl-SlideshowPagingLink-Inactive').addClass('homePageHeroControl-SlideshowPagingLink-Active');
    }

    window.hph_startSlide = function () {
        window.hph_slideOnce();
        timeoutid = window.setTimeout("window.hph_startSlide();", 5000);
    }

    window.hph_Slideshow_onclick = function (index) {
        allNav = $(".homePageHeroControl-SlideshowPagingBar > a");
        if (allNav.eq(index).hasClass("homePageHeroControl-SlideshowPagingLink-Active")) {
            return;
        }
        clearTimeout(timeoutid);
        activeIndex = -1;
        $.each(allNav, function (index, nav) {
            if ($(nav).hasClass("homePageHeroControl-SlideshowPagingLink-Active")) { activeIndex = index; return false; }
        }
              );
        step = index - activeIndex;
        if (step < 0) {
            step += allNav.length;
        }
        for (i = 0; i < step; ++i) {
            window.hph_slideOnce();
        }
        timeoutid = window.setTimeout("window.hph_startSlide();", 5000);
    };

    window.hph_getStyle = function (element, propertyName) {
        var styleValue = null;
        if ($isNull(element)) { return styleValue; }

        if (element.currentStyle) {
            styleValue = element.currentStyle[propertyName];
        }
        else if (window.getComputedStyle) {
            styleValue = document.defaultView.getComputedStyle(element, null).getPropertyValue(propertyName);
        }
        return styleValue;
    }

    var encodedId = $htmlEncode(ctx.ClientControl.get_nextUniqueId() + "_slideShow_");
    var itemsContainerId = encodedId + "container";
    var pagingOverlayId = encodedId + "pagingOverlay";
    var pagingBarId = encodedId + "pagingBar";
    var pagingMoreId = encodedId + "pagingMore";

    ctx.OnPostRender = [];

    ctx.OnPostRender.push(function () {
        window.hph_Slideshow_init();
        adjustHomeHeroImage();
    });

    function adjustHomeHeroImage() {
    }

    ms_outHtml.push(''
    , '        <div class="homePageHeroControl-Slideshow" id="', encodedId, '" data-displaytemplate="ControlSlideshow">'
    , '           <div class="homePageHeroControl-SlideshowItems" id="', itemsContainerId, '">'
    , '                 ', ctx.RenderGroups(ctx), ''
    , '                <div class="homePageHeroControl-SlideshowImageItems">'
    , '                    <ul style="width:', 1180 * numResults + 'px', '">', imagesHtml.join(''), '</ul>'
    , '                </div>'
    , '                <div class="homePageHeroControl-SlideshowCaptionItems">'
    , '                    <ul style="width:', 1180 * numResults + 'px', '">', captionsHtml.join(''), '</ul>'
    , '                </div>'
    , '            </div>'
    );
    if (ctx.ClientControl.get_shouldShowNoResultMessage()) {
        ms_outHtml.push(''
        , '            <div class="homePageHeroControl-SlideShow-noResults">', $noResults, '</div>'
        );
    }
    ms_outHtml.push(''
    , '            <div class="homePageHeroControl-SlideshowPagingBarOverlay" id="', pagingOverlayId, '"></div>'
    , '            <div class="homePageHeroControl-SlideshowPagingBar" id="', pagingBarId, '">'
    );
    var MaxNumOfResults = 5;
    var numResultsToShowPaging = Math.min(numResults, MaxNumOfResults);
    if (numResultsToShowPaging == 1) {
        numResultsToShowPaging = 0;
    }
    for (var i = 0; i < numResultsToShowPaging; i++) {
        var anchorId = encodedId + "pagingControl" + i;
        ms_outHtml.push(''
        , '                <a class="homePageHeroControl-SlideshowPagingLink-Inactive" href="javascript:{}" onclick="hph_Slideshow_onclick(', i, ');" id="', anchorId, '">'
        , '                    <span>&#160;</span>'
        , '                </a>'
        );
    }
    if (numResults > numResultsToShowPaging) {
        ms_outHtml.push(''
        , '                <div class="homePageHeroControl-SlideshowPaging-More" id="', pagingMoreId, '">'
        , '                    <span>&#8230;</span>'
        , '                </div>'
        );
    }
    ms_outHtml.push(''
    , '            </div>'
    , '        </div>'
    , '    '
    );

    ctx['DisplayTemplateData'] = cachePreviousTemplateData;
    return ms_outHtml.join('');
}
function RegisterTemplate_8aea791f157a4c608c0d6b1605c58b41() {

    if ("undefined" != typeof (Srch) && "undefined" != typeof (Srch.U) && typeof (Srch.U.registerRenderTemplateByName) == "function") {
        Srch.U.registerRenderTemplateByName("Slideshow", DisplayTemplate_8aea791f157a4c608c0d6b1605c58b41);
    }

    if ("undefined" != typeof (Srch) && "undefined" != typeof (Srch.U) && typeof (Srch.U.registerRenderTemplateByName) == "function") {
        Srch.U.registerRenderTemplateByName("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroControlSlideshow.js", DisplayTemplate_8aea791f157a4c608c0d6b1605c58b41);
    }
    $includeLanguageScript("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroControlSlideshow.js", "~sitecollection/Style Library/hero/js/jquery-1.9.1.min.js");
}
RegisterTemplate_8aea791f157a4c608c0d6b1605c58b41();
if (typeof (RegisterModuleInit) == "function" && typeof (Srch.U.replaceUrlTokens) == "function") {
    RegisterModuleInit(Srch.U.replaceUrlTokens("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fContent Web Parts\u002fHomePageHeroControlSlideshow.js"), RegisterTemplate_8aea791f157a4c608c0d6b1605c58b41);
}