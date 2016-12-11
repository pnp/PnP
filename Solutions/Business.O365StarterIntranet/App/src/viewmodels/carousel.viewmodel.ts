// ========================================
// Carousel Component View Model
// ========================================

import * as pnp from "sp-pnp-js";
import "trunk8";

declare function require(name: string);

export class CarouselViewModel {

    public items: KnockoutObservableArray<any> = ko.observableArray([]);
    public siteLogoUrl: KnockoutObservable<string> = ko.observable("");

    constructor(params: any) {

        let listTitle = "Carousel Items";
        let Flickity = require("flickity");
        let languageFieldName = "IntranetContentLanguage";

        this.siteLogoUrl(_spPageContextInfo.webLogoUrl);

        let trunk8Options: Trunk8Options = {
            lines: 1,
            tooltip: false,
        };

        // Get the current page language
        pnp.sp.web.lists.getByTitle("Pages").items.getById(_spPageContextInfo.pageItemId).select("ID", languageFieldName).get().then((item) => {

            let currentPageLanguage = item[languageFieldName];

            if (currentPageLanguage) {

                let now = new Date();
                let filterQuery = "CarouselItemEndDate ge datetime'" + now.toISOString() + "' and CarouselItemStartDate le datetime'" + now.toISOString() + "' and IntranetContentLanguage eq '" + currentPageLanguage + "'";

                pnp.sp.web.lists.getByTitle(listTitle).items.orderBy("CarouselItemOrder", true).filter(filterQuery).get().then((elements) => {

                    // Fill the observable array
                    this.items(elements);

                    // Setup the carousels
                    // See http://flickity.metafizzy.co/ for more customizations
                    let flkty = new Flickity(".carousel", {
                        prevNextButtons: false,
                        setGallerySize: true,
                        imageLoaded: true,
                        lazyLoad: 1,
                        adaptiveHeight: true
                    });

                    // Truncate the label
                    $(".carousel-label").trunk8(trunk8Options);

                    // Adjust automatically slide label on resize
                    $(window).resize((event) => {
                        $(".carousel-label").trunk8(trunk8Options);                        
                    });
                });
            }
        });
    }
}
