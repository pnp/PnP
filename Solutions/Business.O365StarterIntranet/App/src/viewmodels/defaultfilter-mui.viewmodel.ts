// ========================================
// Taxonomy Refinement Filter View Model (Display Template)
// ========================================

// Note: to get this display template work, you have to use a managed property mapped to a taxonomy crawl property like ows_taxid_xxx (not the ows_xxx in string format)
// By this way, we are able to get the term id and retrieve the correct label according to the language

/// <reference path="../../typings/globals/knockout/index.d.ts" />
/// <reference path="../../typings/globals/trunk8/index.d.ts" />

import { TaxonomyModule } from "../core/taxonomy";
import "../shared/bindinghandlers";
import * as pnp from "sp-pnp-js";
import "trunk8"; // Trunk8 typings are exposed through an interface, so we have just to import it globally

export class DefaultFilterViewModel {

    public taxonomyModule: TaxonomyModule;

    constructor() {

        this.taxonomyModule = new TaxonomyModule();

        ko.bindingHandlers.localizedTermLabel = {

            init: (element, valueAccessor) => {

                let value: string = ko.unwrap(valueAccessor());

                // Check if the value seems to be a taxonomy term
                let isTerm = /L0\|#/i.test(value);

                if (isTerm) {

                    // Extract the id
                    let termId: Array<string> = value.match(/[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/);

                    if (termId.length > 0) {

                        $(element).addClass("spinner");

                        this.taxonomyModule.init().then(() => {

                            this.taxonomyModule.getTermById(new SP.Guid(termId[0])).then((term) => {

                                $(element).text(term.get_name());

                                $(element).removeClass("spinner");
                            });

                        }).catch((errorMesssage) => {
                            pnp.log.write(errorMesssage, pnp.log.LogLevel.Error);
                        });
                    }

                } else {

                    // Return the original value
                    $(element).text(value);
                }
            },
        };

        // This binding handlers is used to avoid applying bindings twice (from the main script for components)
        // More info here http://www.knockmeout.net/2012/05/quick-tip-skip-binding.html
        ko.bindingHandlers.stopBinding = {
            init: () => {
                return { controlsDescendantBindings: true };
            },
        };
    }
}
