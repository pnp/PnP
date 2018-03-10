// ========================================
// Carousel Component View Model
// ========================================
declare function require(name: string);

import * as i18n from "i18next";
import { Site, Web } from "sp-pnp-js";
import "trunk8";
// tslint:disable-next-line:no-var-requires
const Flickity = require("flickity");
require("flickity-imagesloaded");

class CarouselViewModel {

    public items: KnockoutObservableArray<any> = ko.observableArray([]);
    public siteLogoUrl: KnockoutObservable<string> = ko.observable("");
    private readMoreLabel: KnockoutObservable<string> = ko.observable("");
    private carousel: any;

    constructor(params: any) {

        const listTitle = "Carousel Items";

        const languageFieldName = "IntranetContentLanguage";

        this.siteLogoUrl(_spPageContextInfo.webLogoUrl);
        this.readMoreLabel(i18n.t("readMore"));

        const trunk8OptionsNavLabel: Trunk8Options = {
            lines: 2,
            tooltip: false,
        };

        const trunk8OptionsSlideTitle: Trunk8Options = {
            lines: 4,
            tooltip: false,
        };

        const site = new Site(_spPageContextInfo.siteAbsoluteUrl);
        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        // Get the current page language
        web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(_spPageContextInfo.pageItemId).select("ID", languageFieldName).get().then((item) => {

            const currentPageLanguage = item[languageFieldName];

            if (currentPageLanguage) {

                const now = new Date();
                const filterQuery = "CarouselItemEndDate ge datetime'" + now.toISOString() + "' and CarouselItemStartDate le datetime'" + now.toISOString() + "' and IntranetContentLanguage eq '" + currentPageLanguage + "'";

                site.rootWeb.lists.getByTitle(listTitle).items.orderBy("CarouselItemOrder", true).filter(filterQuery).get().then((elements) => {

                    // Fill the observable array
                    this.items(elements);

                    // Setup the carousels
                    // See http://flickity.metafizzy.co/ for more customizations
                    const carousel = new Flickity(".carousel", {
                        adaptiveHeight: true,
                        imageLoaded: true,
                        lazyLoad: 1,
                        pageDots: false,
                        prevNextButtons: false,
                        setGallerySize: true,
                    });

                    carousel.select(0);

                    // See https://codepen.io/desandro/pen/dMjbjR for Flickity vertical nav
                    carousel.on("select", (data) => {

                        const carouselNav = $(".carousel-nav");
                        const carouselNavCells = carouselNav.find(".carousel-nav-row");

                        const navTop  = carouselNav.position().top;
                        const navCellHeight = carouselNavCells.height();
                        const navHeight = carouselNav.height();

                        // Highlight the nav cell
                        const index = carousel.selectedIndex ;

                        carouselNav.find(".is-nav-selected").removeClass("is-nav-selected");
                        const selected = carouselNavCells.eq(index).addClass("is-nav-selected");

                        // scroll nav
                        const scrollY = selected.position().top + carouselNav.scrollTop() - (navHeight + navCellHeight) / 2;

                        carouselNav.animate({
                            scrollTop: scrollY,
                        });
                    });

                    // Truncate the label
                    $(".nav-label").trunk8(trunk8OptionsNavLabel);
                    $("#slide-title").trunk8(trunk8OptionsSlideTitle);

                    // Adjust automatically slide label on resize
                    $(window).resize((event) => {
                        $(".nav-label").trunk8(trunk8OptionsNavLabel);
                        $("#slide-title").trunk8(trunk8OptionsSlideTitle);
                    });
                });
            }
        });
    }

    public selectSlide = (data, event) => {
        const index = $(event.currentTarget).index();
        const carousel = new Flickity(".carousel");

        carousel.select(index);
    }
}

export default CarouselViewModel;
