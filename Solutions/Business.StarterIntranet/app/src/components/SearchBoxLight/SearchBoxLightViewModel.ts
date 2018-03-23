// ========================================
// Search box Light View Model (Desktop)
// ========================================
import SearchBoxViewModel from "../SearchBox/SearchBoxViewModel";

class SearchBoxLightViewModel extends SearchBoxViewModel {

    private showSearchHelp: KnockoutObservable<boolean>;

    public constructor(params: any) {
        super();

        this.showSearchHelp = ko.observable(params.showHelp ? params.showHelp : false);
    }

    public doSearch = () => {

        // Check if the input text is empty
        let queryUrl: string = "";
        let urlHash = window.location.hash;
        const queryStringValue = this.isSearchEmpty() ? "*" : this.inputQuery();

        // Replace the keyword in the refinement string if present
        // http://www.techmikael.com/2013/06/add-clear-filters-link-to-your-search.html
        if (urlHash.indexOf("Default") === 1) {
            urlHash = decodeURIComponent(urlHash);
            const kIdx = urlHash.indexOf('"k":');
            const rIdx = urlHash.indexOf('","');

            queryUrl = urlHash.substring(0, kIdx + 5) + queryStringValue + urlHash.substring(rIdx);

        } else {
            queryUrl = this.utilityModule.replaceQueryStringParam(window.location.href, "#k", queryStringValue);
        }

        // Redirect to the correct page according to selected category
        window.location.href = queryUrl;
    }
}

export default SearchBoxLightViewModel;
