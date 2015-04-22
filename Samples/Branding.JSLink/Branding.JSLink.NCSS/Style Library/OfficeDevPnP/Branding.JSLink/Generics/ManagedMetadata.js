var jslinkTemplates = window.jslinkTemplates || {};
jslinkTemplates.Taxonomy = function() {


    var currentlySelectedTerms;

    function _editMode(ctx) {
        // register our "get value callback" method
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        formCtx.registerGetValueCallback(formCtx.fieldName, jslinkTemplates.Taxonomy.getValue.bind(null, formCtx.fieldName));

        _loadField(formCtx.fieldName, ctx.CurrentFieldSchema.SspId, ctx.CurrentFieldSchema.TermSetId, ctx.CurrentFieldValue);

        return "<select id='TAX_" + formCtx.fieldName + "' style='display:block; width:100%;'><option>Loading...</option></select>" + 
            "<input type='hidden' id='TAXVALUE_" + formCtx.fieldName + "' />";
    };

    function _loadField(fieldName, sspId, termSetId, currentValue) {
        

        // reset the current terms array
        currentlySelectedTerms = [];

        // wait for the page to render
        jQuery(document).ready(function () {

            var context = new SP.ClientContext.get_current();
            var taxonomySession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);

            var termStore = taxonomySession.get_termStores().getById(sspId);
            var termSet = termStore.getTermSet(termSetId);

            if (currentValue.length > 0) {
                // pull the Id from the current value
                var currentTermId = currentValue.split("|")[1];
                var currentTerm = termSet.getTerm(currentTermId);

                context.load(currentTerm);
                context.executeQueryAsync(
                    function () {

                        // retrieve the full path .. this will include all of the parent terms too 
                        var fullPath = currentTerm.get_pathOfTerm();

                        // they are semi-colon delimited, so if we perform a split we end up
                        // with an array containing all of the currently selected values
                        currentlySelectedTerms = fullPath.split(";");
                        
                        // now we can proceed with rendering...
                        _renderDropDowns(fieldName, sspId, termSetId);
                    },
                    function () {
                        alert("Call failed. Error: " + args.get_message());
                    });
            }
            else {
                // there is no current value, so just go
                _renderDropDowns(fieldName, sspId, termSetId);
            }
        });
    };

    function _renderDropDowns(fieldName, sspId, termSetId) {
        var elementId = "#TAX_" + fieldName;

        var context = new SP.ClientContext.get_current();
        var taxonomySession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);

        var termStore = taxonomySession.get_termStores().getById(sspId);
        var termSet = termStore.getTermSet(termSetId);

        var terms = termSet.get_terms();
        context.load(terms);
        context.executeQueryAsync(
            function () {
                jQuery(elementId).empty(); // clear out the existing options

                var html = "<option></option>";

                var termEnumerator = terms.getEnumerator();
                while (termEnumerator.moveNext()) {
                    var currentTerm = termEnumerator.get_current();

                    html += "<option value='" + currentTerm.get_id() + "'";
                    
                    // if the current option being rendered is in our array
                    // then make sure the option is selected
                    if (contains(currentlySelectedTerms, currentTerm.get_name())) {
                        html += " selected='selected'";
                    }

                    html += ">" + currentTerm.get_name() + "</option>";

                    
                }

                // add the options
                jQuery(elementId).append(html);

                // check if we selected one of them
                var selectedOption = jQuery(elementId + " option[selected='selected']");
                if (selectedOption.length > 0)
                {
                    // simulate the same "sender.target" property used in a click event
                    var newSender = {
                        'target': selectedOption[0]
                    };
                    // recursively render the child drop downs
                    _selectTerm(newSender);
                }

                jQuery(elementId + " option").click(jslinkTemplates.Taxonomy.selectTerm);


            },
            function (x, args) {
                alert("Call failed. Error: " + args.get_message());
            }
        );
    };

    function contains(a, obj) {
        for (var i = 0; i < a.length; i++) {
            if (a[i] === obj) {
                return true;
            }
        }
        return false;
    }

    function _selectTerm(sender) {
        var termId = jQuery(sender.target).val();
        var termLabel = jQuery(sender.target).text();

        if (termId == "") {
            return;
        }

        // update the hidden input field with the currently selected value
        // this should be in the value LABEL|GUID
        jQuery(sender.target).parent().siblings("input").val(termLabel + "|" + termId);

        // kill off any existing child-drop-downs (recursively)
        // before we re-render them below
        var selectElement = jQuery(sender.target).parent()[0];
        removeChildDropDowns(selectElement);

        var context = new SP.ClientContext.get_current();
        var taxonomySession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);

        var term = taxonomySession.getTerm(termId);
        var childTerms = term.get_terms();

        context.load(term);
        context.load(childTerms);
        context.executeQueryAsync(
            function () {
                // only execute if it has child terms
                if (term.get_termsCount() > 0) {
                    // start off with a select containing a blank option
                    var html = "<select style='display:block; margin-top: 5px; width:100%;' id='TERM_" + termId + "'><option></option>";

                    var termEnumerator = childTerms.getEnumerator();
                    while (termEnumerator.moveNext()) {
                        var currentTerm = termEnumerator.get_current();

                        html += "<option value='" + currentTerm.get_id() + "'";

                        // if the current option being rendered is in our array
                        // then make sure the option is selected
                        if (contains(currentlySelectedTerms, currentTerm.get_name())) {
                            html += " selected='selected'";
                        }

                        html += ">" + currentTerm.get_name() + "</option>";
                    }

                    // add the new drop-downs
                    jQuery(sender.target).parent().after(html);

                    // check if we selected one of them when we added them
                    var selectedOption = jQuery('#TERM_' + termId + " option[selected='selected']");
                    if (selectedOption.length > 0) {
                        // simulate the same "sender.target" property used in a click event
                        var newSender = {
                            'target': selectedOption[0]
                        };
                        // recursively render the child drop downs
                        _selectTerm(newSender);
                    }

                    // and add a click event
                    jQuery("#TERM_" + termId + " option").click(jslinkTemplates.Taxonomy.selectTerm);
                }
            },
            function (x, args) {
                alert("Call failed. Error: " + args.get_message());
            }
        );
    };
   
    function removeChildDropDowns(selectElement) {

        // the ~ means "any following siblings" and ignores any siblings
        // further up the chain
        jQuery("#" + selectElement.id + " ~ select").remove();
    };

    function _getValue(fieldName) {
        // retrieve value from the relevant hidden input
        var hiddenValue = jQuery("#TAXVALUE_" + fieldName).val();
        
        // return it
        return hiddenValue;
    };

    return {
        editMode: _editMode,
        selectTerm: _selectTerm,
        getValue: _getValue
    }
}();