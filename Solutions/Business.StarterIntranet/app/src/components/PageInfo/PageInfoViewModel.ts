// ========================================
// Page Info Menu Component
// ========================================
import * as i18n from "i18next";
import * as moment from "moment";
import { Logger, LogLevel, Web } from "sp-pnp-js";
import LocalizationModule from "../../modules/LocalizationModule";
import TaxonomyModule from "../../modules/TaxonomyModule";
import "../IKnockoutBindingHandlers";

class PageInfoViewModel {

    public pageItem: KnockoutObservable<any>;
    public localization: LocalizationModule;
    private selectedFields: string;
    private expandedFields: string;
    private taxonomyModule: TaxonomyModule;

    constructor(params: any) {

        this.taxonomyModule = new TaxonomyModule();
        this.localization = new LocalizationModule();

        // Internal name of fields to retrieve in the page
        this.selectedFields = params.selectedFields;
        this.expandedFields = params.expandedFields;

        this.pageItem = ko.observable();

        ko.bindingHandlers.formatDateField = {

            init: (element, valueAccessor, allBindings) => {

                // Needed to get the correct locale for the date
                this.localization.ensureResourcesLoaded(() => {

                    const value = ko.unwrap(valueAccessor());
                    const dateFormat = allBindings.get("dateFormat") || "LL";
                    const date = moment(value).format(dateFormat);
                    $(element).text(date);
                });
            },
        };

        ko.bindingHandlers.getResource = {

            init: (element, valueAccessor) => {

                this.localization.ensureResourcesLoaded(() => {
                    const value = ko.unwrap(valueAccessor());
                    $(element).text(i18n.t(value));
                });
            },
        };

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        // Note 1: Be careful, there is a bug with GET REST API for taxonomy fields when they have only a single value (i.e the Label property is not correct)
        // In our case, we don"t use directly the label because we have to get it according the current language so it does not matter. Remember, by default, the returned label follows the current web language
        // Note 2: If no fields are specified, the pnp call return all fields from the item (without expand)
        web.lists.getById(_spPageContextInfo.pageListId.replace(/{|}/g, "")).items.getById(_spPageContextInfo.pageItemId).select(this.selectedFields).expand(this.expandedFields).get().then((item) => {

            const allItemProperties: Array<Promise<any>> = [];

            // Loop through each returned properties for the item and build an array of promises
            for (const itemKey in item) {

                if (item.hasOwnProperty(itemKey)) {

                    const itemValue = item[itemKey];

                    const p = new Promise<any>((resolve) => {

                        if (itemValue) {

                            // Mutiple values taxonomy (returned as an array of objects)
                            // Be careful, with SharePoint 2013, the response is not the same as SharePoint Online
                            // With SharePoint 2013, values must be retrieved via the "results" property of itemValue (because of odata=verbose)
                            if (Array.isArray(itemValue.results)) {

                                const arrayValues: KnockoutObservableArray<Promise<string>> = ko.observableArray([]);
                                itemValue.results.forEach((element) => {

                                    if (element.hasOwnProperty("TermGuid")) {

                                        // tslint:disable-next-line:no-shadowed-variable
                                        const p2 = new Promise<any>((resolve) => {
                                            this.taxonomyModule.init().then(() => {

                                                this.taxonomyModule.getTermById(new SP.Guid(element.TermGuid)).then((term) => {

                                                    resolve(term.get_name());
                                                });
                                            });
                                        });

                                        arrayValues.push(p2);
                                    }
                                });

                                Promise.all(arrayValues()).then((multiValues) => {

                                    resolve({key: itemKey, value: multiValues });
                                });

                            } else {

                                // If we want to process values singularly to apply styles, we have to use an observale array instead of a regualr array
                                const arrayValues: KnockoutObservableArray<string> = ko.observableArray([]);

                                // Single value taxonomy (returned as a single object)
                                if (itemValue.hasOwnProperty("TermGuid")) {

                                    const termId = itemValue.TermGuid;

                                    this.taxonomyModule.init().then(() => {

                                        this.taxonomyModule.getTermById(new SP.Guid(termId)).then((term) => {

                                            resolve({key: itemKey, value: arrayValues.push(term.get_name())});
                                        });
                                    });

                                } else {

                                    resolve({key: itemKey, value: itemValue});
                                }
                            }

                         } else {

                            resolve({key: itemKey, value: null});
                        }
                    });

                    allItemProperties.push(p);
                }
            }

            // Resolve all nested async calls
            Promise.all(allItemProperties).then((properties) => {

                const listItem = {};

                // Build a single object from the array of resolved properties
                // tslint:disable-next-line:prefer-for-of
                for (let i = 0; i < properties.length; i++) {
                    listItem[properties[i].key] = properties[i].value;
                }

                // Build dynamically the view model via knockout mapping plugin
                this.pageItem(ko.mapping.fromJS(listItem));
            });

        }).catch((errorMesssage) => {

            Logger.write("[PageInfo.getPageProperties]: " + errorMesssage, LogLevel.Error);
        });
    }
}

export default PageInfoViewModel;
