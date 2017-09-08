import UtilityModule from "../modules/UtilityModule";
import ISearchNavigationNode from "./ISearchNavigationNode";

class SearchNavigationNode implements ISearchNavigationNode {

    private static iconParam: string = "icon";

    private title: string;
    private url: string;
    private icon: string;

    private utilityModule: UtilityModule;

    public get Title(): string { return this.title; }
    public set Title(value: string) { this.title = value; }

    public get Url(): string { return this.url ; }
    public set Url(value: string) { this.url = value; }

    public get Icon(): string { return this.icon ; }

    public constructor(title: string, url: string) {
        this.utilityModule = new UtilityModule();

        this.title = title;
        this.url = this.utilityModule.removeQueryStringParam(SearchNavigationNode.iconParam, url);

        const iconName =  this.utilityModule.getQueryStringParam(SearchNavigationNode.iconParam, url);
        this.icon = iconName ? iconName : "fa-search";
    }
}

export default SearchNavigationNode;
