// ========================================
// Search box View Model (Desktop)
// ========================================
import * as i18n from "i18next";
import { Util } from "sp-pnp-js";
import SearchNavigationNode from "../../models/SearchNavigationNode";
import SearchModule from "../../modules/SearchModule";
import UtilityModule from "../../modules/UtilityModule";

class SearchBoxViewModel {

    public searchPlaceHolderLabel: KnockoutObservable<string>;
    public searchNavigationNodes: KnockoutObservableArray<SearchNavigationNode>;
    public selectedCategory: KnockoutObservable<SearchNavigationNode>;
    public currentCategory: KnockoutComputed<SearchNavigationNode>;
    public inputQuery: KnockoutObservable<string>;
    public utilityModule: UtilityModule;
    public searchModule: SearchModule;
    public isError: KnockoutObservable<boolean>;
    public isSearchEmpty: KnockoutComputed<boolean>;

    constructor() {

        this.searchPlaceHolderLabel = ko.observable(i18n.t("searchPlaceholderLabel"));
        this.utilityModule = new UtilityModule();
        this.searchModule = new SearchModule();

        // Check if there is already a query performed
        const k = this.utilityModule.getQueryStringParam("k", window.location.href);
        let keywords: string;

        // tslint:disable-next-line:prefer-conditional-expression
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

        this.searchNavigationNodes = ko.observableArray([]);
        this.selectedCategory = ko.observable(null);

        // Init search navigation settings
        this.searchModule.getSearchNavigationSettings().then((nodes) => {
            ko.utils.arrayPushAll(this.searchNavigationNodes(), nodes);
            this.searchNavigationNodes.valueHasMutated();

            // Set initial selected category
            const currentPageName = window.location.pathname;
            const current =  ko.utils.arrayFirst(this.searchNavigationNodes(), (item) => {
                return item.Url.indexOf(currentPageName) !== -1;
            });

            if (current) {
                this.selectedCategory(current);
            } else {
                this.selectedCategory(this.searchNavigationNodes()[0]);
            }
        });
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

            queryUrl = this.selectedCategory().Url + "?#k=" + this.inputQuery();

            // Redirect to the correct page according to selected category
            window.location.href = queryUrl;
        }
    }
}

export default SearchBoxViewModel;
