// ====================
// Search box View Model (Desktop)
// ====================

/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/sharepoint/index.d.ts" />

import { UtilityModule } from "../core/utility";
import i18n = require("i18next");

export class SearchBoxViewModel {

    public searchPlaceHolderLabel: KnockoutObservable<string>;
    public searchCategories: KnockoutObservableArray<any>;
    public selectedCategory: KnockoutObservable<SearchCategory>;
    public inputQuery: KnockoutObservable<string>;
    public utilityModule: UtilityModule;
    public isError: KnockoutObservable<boolean>;
    public isSearchEmpty: KnockoutComputed<boolean>;

    constructor() {

        this.searchPlaceHolderLabel = ko.observable(i18n.t("searchPlaceholderLabel"));
        this.utilityModule = new UtilityModule();

        // Check if there is already a query performed
        let k = this.utilityModule.getQueryString("k", window.location.href);
        let keywords: string;

        if (k) {
            keywords = decodeURIComponent(k);
        } else {
            keywords = "";
        }

        this.inputQuery = ko.observable(keywords);

        this.isError = ko.observable(false);
        this.isSearchEmpty = ko.computed(() => {

            if (this.inputQuery().length > 0) {

                this.isError(false);
                return false;

            } else {
                return true;
            }
        });

        this.searchCategories = ko.observableArray([
            new SearchCategory(i18n.t("intranetSearchCategory"), "ms-Icon--globe", i18n.t("intranetSearchPageUrl")),
            new SearchCategory(i18n.t("documentsSearchCategory"), "ms-Icon--documents", i18n.t("documentsSearchPageUrl")),
            new SearchCategory(i18n.t("peopleSearchCategory"), "ms-Icon--people", null, true),
        ]);

        let currentPageName = window.location.pathname;
        let currentCategory = ko.utils.arrayFirst(this.searchCategories(), (item) => {
            return item.searchPageUrl === currentPageName.substring(currentPageName.lastIndexOf("/") + 1);
        });

        if (currentCategory) {
            this.selectedCategory = ko.observable(currentCategory);
        } else {
             this.selectedCategory = ko.observable(this.searchCategories()[0]);
        }
    }

    public selectCategory = (data: any) => {
        this.selectedCategory(data);
    }

    public doSearch = () => {

        // Check if the input text is empty
        if (this.isSearchEmpty()) {

            this.isError(true);

        } else {

            let queryUrl: string = "";

            // Check if people search. In this case, we use the Delve portal instead of SharePoint
            if (this.selectedCategory().isPeople) {

                let profileUrl = _spPageContextInfo["ProfileUrl"];
                profileUrl = this.utilityModule.getLocation(profileUrl);

                // Build the search query for Delve
                queryUrl = profileUrl.protocol + "//" + profileUrl.hostname + "/_layouts/15/me.aspx?q=" + this.inputQuery();

                // Open the page in a new tab
                window.open(queryUrl);

            } else {

                queryUrl = _spPageContextInfo.siteAbsoluteUrl + "/Pages/" + this.selectedCategory().searchPageUrl + "?k=" + this.inputQuery();

                // Redirect to the correct page according to selected category
                window.location.href = queryUrl;
            }
        }
    }
}

class SearchCategory {

    public name: string;
    public iconClass: string;
    public searchPageUrl: string;
    public isPeople: boolean = false;

    constructor(name: string, iconClass: string, searchPageUrl: string, isPeople?: boolean) {
        this.name = name;
        this.iconClass = iconClass;
        this.searchPageUrl = searchPageUrl;
        this.isPeople = isPeople;
    }
}
