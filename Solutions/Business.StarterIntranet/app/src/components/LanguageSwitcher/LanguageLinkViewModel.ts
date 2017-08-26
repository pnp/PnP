class LanguageLinkViewModel {

    public displayName: KnockoutObservable<string>;
    public url: KnockoutObservable<string>;
    public isCurrentLanguage: KnockoutObservable<boolean>;
    public isValidTranslation: KnockoutObservable<boolean>;
    public flagCssClass: KnockoutObservable<string>;
    public languageLabel: KnockoutObservable<string>;

    constructor() {

        this.displayName = ko.observable("");
        this.url = ko.observable("");
        this.isCurrentLanguage = ko.observable(false);
        this.isValidTranslation = ko.observable(false);
        this.flagCssClass = ko.observable("");
        this.languageLabel  = ko.observable("");
    }
}

export default LanguageLinkViewModel;
