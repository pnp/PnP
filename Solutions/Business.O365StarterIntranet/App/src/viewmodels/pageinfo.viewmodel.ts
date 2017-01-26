// ====================
// Page Info Menu Component
// ====================

/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/knockout.mapping/index.d.ts" />
/// <reference path="../../typings/globals/sharepoint/index.d.ts" />

import { TaxonomyModule } from "../core/taxonomy";
import "../shared/bindinghandlers";
import * as moment from "moment";
import * as pnp from "sp-pnp-js";

export class PageInfoViewModel {

    public pageItem: KnockoutObservable<any>;
    private selectedFields: string;
    private expandedFields: string;
    private taxonomyModule: TaxonomyModule;

    constructor(params: any) {

        this.taxonomyModule = new TaxonomyModule();

        // Internal name of fields to retrieve in the page
        this.selectedFields = params.selectedFields;
        this.expandedFields = params.expandedFields;

        this.pageItem = ko.observable();

        ko.bindingHandlers.formatDateField = {

            init: (element, valueAccessor) => {

                // Get the current value of the current property we"re bound to
                let value = ko.unwrap(valueAccessor());

                let date = moment(value).format("LL");

                $(element).text(date);
            },
        };

        // Note 1: Be careful, there is a bug with GET REST API for taxonomy fields when they have only a single value (i.e the Label property is not correct)
        // In our case, we don"t use directly the label because we have to get it according the current language so it does not matter. Remember, by default, the returned label follows the current web language
        // Note 2: If no fields are specified, the pnp call return all fields from the item (without expand)
        pnp.sp.web.lists.getByTitle("Pages").items.getById(_spPageContextInfo.pageItemId).select(this.selectedFields).expand(this.expandedFields).get().then((item) => {

            let allItemProperties: Array<Promise<any>> = [];

            // Loop through each returned properties for the item and build an array of promises
            for (let key in item) {

                if (item.hasOwnProperty(key)) {

                    let value = item[key];

                    let p = new Promise<any>((resolve) => {

                        if (value) {

                            // Mutiple values taxonomy (returned as an array of objects)
                            if (Array.isArray(value)) {

                                let arrayValues: Array<Promise<string>> = [];
                                value.forEach(element => {

                                    if (element.hasOwnProperty("TermGuid")) {

                                        let p2 = new Promise<any>((resolve) => {
                                            this.taxonomyModule.init().then(() => {

                                                this.taxonomyModule.getTermById(new SP.Guid(element.TermGuid)).then((term) => {

                                                    resolve(term.get_name());
                                                });
                                            });
                                        });

                                        arrayValues.push(p2);
                                    }
                                });

                                Promise.all(arrayValues).then((multiValues) => {

                                    resolve({key: key, value: multiValues.join(" - ")});
                                });

                            } else {

                                // Single value taxonomy (returned as a single object)
                                if (value.hasOwnProperty("TermGuid")) {

                                    let termId = value.TermGuid;

                                    this.taxonomyModule.init().then(() => {

                                        this.taxonomyModule.getTermById(new SP.Guid(termId)).then((term) => {

                                            resolve({key: key, value: term.get_name()});
                                        });
                                    });

                                } else {

                                    resolve({key: key, value: value});
                                }
                            }

                         } else {

                            resolve({key: key, value: null});
                        }
                    });

                    allItemProperties.push(p);
                }
            }

            // Resolve all nested async calls
            Promise.all(allItemProperties).then((properties) => {

                let listItem = {};

                // Build a single object from the array of resolved properties
                for (let i = 0; i < properties.length; i++) {
                    listItem[properties[i].key] = properties[i].value;
                }

                // Build dynamically the view model via knockout mapping plugin
                this.pageItem(ko.mapping.fromJS(listItem));
            });

        }).catch((errorMesssage) => {

            pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
        });
    }
}
