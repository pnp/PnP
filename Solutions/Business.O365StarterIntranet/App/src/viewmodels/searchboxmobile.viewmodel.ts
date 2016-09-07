// ========================================
// Search box View Model (Mobile)
// ========================================

/// <reference path="../../typings/globals/knockout/index.d.ts" />

import "../shared/bindinghandlers";
import { SearchBoxViewModel } from "./searchbox.viewmodel";

export class SearchBoxMobileViewModel extends SearchBoxViewModel {

    public displaySearchBox: KnockoutObservable<boolean>;
    public selectedIndex: KnockoutObservable<number>;
    public toggleUIElements: KnockoutComputed<void>;

    constructor() {

        super();

        this.displaySearchBox = ko.observable(false);
        this.selectedIndex = ko.observable(0);
        this.toggleUIElements = ko.computed(() => {

            // Give more space to the searchbox by hidding the burger menu and the language switcher
            if (this.displaySearchBox()) {
                $(".navbar-header .navbar-toggle").hide();
                $("#languageswitcher-mobile").hide();
            } else {
                // Reset to the default behavior (managed by Bootstrap, not by our code)
                $(".navbar-header .navbar-toggle").css("display", "");

                // Manually show the element (controlled by our code)
                $("#languageswitcher-mobile").show();
            }
        });

        if (this.inputQuery().length > 0) {

            // If a search query was already performed
            this.displaySearchBox(true);
        }

        ko.bindingHandlers.inputFocus = {

            init: (element, valueAccessor) => {

                let value = valueAccessor();
                ko.unwrap(value) ? $(element).focus() : $(element).blur();
            },
        };

    }

    public toggleSearchBox = () => {

        this.displaySearchBox(!this.displaySearchBox());
    }

    public toggleCategory = () => {

        if (this.selectedIndex() === (this.searchCategories().length - 1)) {
            // Reset the index
            this.selectedIndex(0);

        } else {
            this.selectedIndex(this.selectedIndex() + 1);
        }

        this.selectedCategory(this.searchCategories()[this.selectedIndex()]);
    }
}
