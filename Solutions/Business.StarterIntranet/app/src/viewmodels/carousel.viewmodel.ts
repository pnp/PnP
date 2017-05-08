// ========================================
// Carousel Component View Model
// ========================================
declare function require(name: string);

import { Web } from "sp-pnp-js";
import "trunk8";
import * as i18n from "i18next";
let Flickity = require('flickity');
require('flickity-imagesloaded');

export class CarouselViewModel {

    public items: KnockoutObservableArray<any> = ko.observableArray([]);
    public siteLogoUrl: KnockoutObservable<string> = ko.observable("");
    private readMoreLabel: KnockoutObservable<string> = ko.observable("");
    private carousel: any;

    constructor(params: any) {

        let listTitle = "Carousel Items";
        
        let languageFieldName = "IntranetContentLanguage";

        this.siteLogoUrl(_spPageContextInfo.webLogoUrl);
        this.readMoreLabel(i18n.t("readMore"));

        let trunk8OptionsNavLabel: Trunk8Options = {
            lines: 2,
            tooltip: false,
        };

        let trunk8OptionsSlideTitle: Trunk8Options = {
            lines: 4,
            tooltip: false,
        };

        let web = new Web(_spPageContextInfo.webAbsoluteUrl);   

        // Get the current page language
        web.lists.getByTitle("Pages").items.getById(_spPageContextInfo.pageItemId).select("ID", languageFieldName).get().then((item) => {

            let currentPageLanguage = item[languageFieldName];

            if (currentPageLanguage) {

                let now = new Date();
                let filterQuery = "CarouselItemEndDate ge datetime'" + now.toISOString() + "' and CarouselItemStartDate le datetime'" + now.toISOString() + "' and IntranetContentLanguage eq '" + currentPageLanguage + "'";

                web.lists.getByTitle(listTitle).items.orderBy("CarouselItemOrder", true).filter(filterQuery).get().then((elements) => {

                    // Fill the observable array
                    this.items(elements);

                    // Setup the carousels
                    // See http://flickity.metafizzy.co/ for more customizations
                    let carousel = new Flickity(".carousel", {
                        prevNextButtons: false,
                        pageDots: false,
                        setGallerySize: true,
                        imageLoaded: true,
                        lazyLoad: 1,
                        adaptiveHeight: true,
                    });

                    carousel.select(0);

                    // See https://codepen.io/desandro/pen/dMjbjR for Flickity vertical nav
                    carousel.on('select', (data) => {

                        let carouselNav = $(".carousel-nav");
                        let carouselNavCells = carouselNav.find('.carousel-nav-row');

                        let navTop  = carouselNav.position().top;
                        let navCellHeight = carouselNavCells.height();
                        let navHeight = carouselNav.height();

                        // Highlight the nav cell
                        let index = carousel.selectedIndex ;

                        carouselNav.find('.is-nav-selected').removeClass('is-nav-selected');
                        let selected = carouselNavCells.eq(index).addClass('is-nav-selected');

                        // scroll nav
                        let scrollY = selected.position().top + carouselNav.scrollTop() - ( navHeight + navCellHeight ) / 2;

                        carouselNav.animate({
                            scrollTop: scrollY
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

        var index = $(event.currentTarget).index();

        let carousel = new Flickity(".carousel");
        carousel.select(index);
    }
}
