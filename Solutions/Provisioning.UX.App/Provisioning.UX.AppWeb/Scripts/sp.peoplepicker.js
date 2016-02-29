(function ($) {
    "use strict";

    // This class allows for options to be set for each people picker used such as max users, etc. 
    // This peoplepicker implementation allows for repopulation of the picker from previously selected users if the user has navigated away from the users view and back.

    // Create a peoplke picker objcet for each peoplepickert used in the UI. 
    $.fn.spSecondaryOwnersPicker = function (options) {

        var methods = {

            init: function () {

                var container = $(this);

                container.append('<div class="dropdown sp-peoplepicker-wrapper"><input type="hidden" /><div class="sp-peoplepicker-selectedValues"></div><input type="text" class="form-control sp-peoplepicker-editInput" data-toggle="dropdown" /><ul class="dropdown-menu sp-peoplepicker-dropdown" role="menu" sp-peoplepicker-lastticks="0"></ul></div>');

                // delegate events to the container to maintain scope
                container.on('keypress', 'input.sp-peoplepicker-editInput', methods.searchUser);

                // we also want to grab any clicks on a's in the drop down so we can select the user
                container.on('click', 'ul.sp-peoplepicker-dropdown > li', methods.selectUser);

                // and we need to be able to remove people we have aleady chosen
                container.on('click', 'span.sp-peoplepicker-removeChosen', methods.removeChosenUser);

                return container;
            },

            removeChosenUser: function () {
                var source = $(this);
                var controlContext = methods.getControlContext(source);
                var chosen = source.closest('span.sp-peoplepicker-chosen');

                var chosenToRemoveLogin = chosen.attr('sp-login');

                var arr = methods.parseValue(controlContext.hiddenInput);

                var newArr = [];

                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].login !== chosenToRemoveLogin) {
                        newArr.push(arr[i]);
                    }
                }

                // update our hidden field value
                controlContext.hiddenInput.val(JSON.stringify(newArr));

                // now remove the DOM element
                chosen.remove();
            },

            selectUser: function (e) {
                e.preventDefault();

                // this represents the item they just clicked on in the dropdown
                var selectedItem = $(this);

                if (!selectedItem.hasClass('sp-peoplepicker-foundItem')) {
                    // they have clicked on a divider or information row
                    return;
                }

                // get our control context
                var controlContext = methods.getControlContext(selectedItem);

                // get a data object from the choice
                var chosen = { 
                    login: selectedItem.attr('sp-login'), 
                    title: selectedItem.attr('sp-title'), 
                    email: selectedItem.attr('sp-email'), 
                    displayName: selectedItem.attr('sp-displayName') 
                };

                // update our hidden field value
                var arr = methods.parseValue(controlContext.hiddenInput);

                if (arr.length >= settings.maxSelectedUsers) {
                    alert('This people picker is configured to allow ' + settings.maxSelectedUsers + ' users, please remove a selected user.');
                    return;
                }

                arr.push(chosen);

                methods.setSelectedUsers(controlContext, arr);

                // clear our previous entries and results                
                controlContext.dropdown.empty();
                controlContext.userEditInput.val('');
            },

            setSelectedUsers: function (/*controlContext*/ controlContext, /*userInfo[]*/ arr) {

                if (arr === null || !$.isArray(arr)) {
                    // count this as an attempt to clear the control
                    methods.emptyControl(controlContext);
                    return;
                }

                // set the hidden input's value with our JSON array
                controlContext.hiddenInput.val(JSON.stringify(arr));

                // update our display of found users     
                controlContext.selectUsersDiv.empty();

                for (var i = 0; i < arr.length; i++) {
                    var chosen = arr[i];
                    controlContext.selectUsersDiv.append('<span class="sp-peoplepicker-chosen" sp-login="' + chosen.login + '">' + chosen.displayName + '<span class="sp-peoplepicker-removeChosen"><span class="glyphicon glyphicon-remove"></span></span></span>');
                }

                

                // Update siteconfiguration object for site request
                angular.element($("#divFieldOwners")).scope().AddSecondaryOwners(JSON.stringify(arr));
                
            },

            searchUser: function (e) {

                var source = $(this);

                // get our control context
                var controlContext = methods.getControlContext(source);
                // clear previous results and let the user know we are searching
                controlContext.dropdown.empty();
                controlContext.dropdown.append('<li role="presentation">Searching...</li>');

                // we add on the key that was just pressed
                var currentValue = controlContext.userEditInput.val() + String.fromCharCode(e.which);

                if (currentValue === null || currentValue.length < settings.minSearchTriggerLength) {
                    // do nothing. seriously, stop doing things if they haven't entered enough letters
                    return;
                }

                

                $app.withSPContext(function (spContext) {

                    var queryTerm = '' + settings.searchPrefix + currentValue + settings.searchSuffix;

                    var query = new SP.UI.ApplicationPages.ClientPeoplePickerQueryParameters();
                    query.set_allowMultipleEntities(false);
                    query.set_maximumEntitySuggestions(settings.maximumEntitySuggestions);
                    query.set_principalType(settings.principalType);
                    query.set_principalSource(settings.principalSource);
                    query.set_queryString(queryTerm);
                    var searchResult = SP.UI.ApplicationPages.ClientPeoplePickerWebServiceInterface.clientPeoplePickerSearchUser(spContext, query);

                    // give ourselves some async context
                    var searchCtx = {
                        queryTicks: (new Date()).getTime(),
                        result: searchResult,
                        queryTerm: queryTerm,
                        controlContext: controlContext,
                        spContext: spContext
                    };

                    // issue request
                    spContext.executeQueryAsync(
                        $app.getCtxCallback(searchCtx, methods.searchSuccess), 
                        $app.getCtxCallback(searchCtx, methods.searchFail));
                }).fail(function () {
                    controlContext.dropdown.empty();
                    alert('There was a problem connecting to SharePoint. Please refresh the page to try again.');
                });
            },

            searchSuccess: function () {

                // get our control context from the async context we created in searchUser
                var searchCtx = this;
                var controlContext = searchCtx.controlContext;
                var spContext = searchCtx.spContext;

                // track our queries in time, giving preference to the most recent
                var lastTicks = parseFloat(controlContext.dropdown.attr('sp-peoplepicker-lastticks'));
                if (this.queryTicks < lastTicks) {
                    return;
                }

                // set our new last ticks count
                controlContext.dropdown.attr('sp-peoplepicker-lastticks', this.queryTicks);

                // clear previous results
                controlContext.dropdown.empty();
                

                // parse out results
                var results = spContext.parseObjectFromJsonString(this.result.get_value());

                if (results.length < 1) {
                    controlContext.dropdown.append('<li role="presentation">No results found</li>');
                }
                else {
                    var displayCount = results.length;
                    if (displayCount > settings.displayResultCount) {
                        displayCount = settings.displayResultCount;
                    }

                    var foundItems = [];

                    for (var i = 0; i < displayCount; i++) {
                        var item = results[i];
                        var loginName = item.Key;
                        var displayName = item.DisplayText;
                        var title = item.EntityData.Title;
                        var email = item.EntityData.Email;

                        foundItems.push('<li role="presentation" class="sp-peoplepicker-foundItem" sp-login="' + loginName
                            + '" sp-title="' + title + '" sp-email="' + email + '" sp-displayName="' + displayName + '">' +
                            displayName + '<br />' +
                            title + '<br />' +
                            email + '<br />' +
                            '</li>');
                    }

                    controlContext.dropdown.append(foundItems.join(''));

                    var ofStr = results.length < settings.maximumEntitySuggestions ? '' + results.length : settings.maximumEntitySuggestions + '+';

                    controlContext.dropdown.append('<li role="presentation" class="divider"></li><li role="presentation">Showing ' + displayCount + ' items of ' + ofStr + '</li>');
                }
            },

            searchFail: function () {
                alert('There was a problem executing your search. Please try again, if the problem persists please refresh the page.');
            },

            getControlContext: function (/*jQuery*/ elementInControlContainer) {

                var container = elementInControlContainer.hasClass('sp-peoplepicker-wrapper') ? elementInControlContainer : elementInControlContainer.closest('div.sp-peoplepicker-wrapper');

                return {
                    container: container,
                    selectUsersDiv: container.find('div.sp-peoplepicker-selectedValues'),
                    userEditInput: container.find('input.sp-peoplepicker-editInput'),
                    dropdown: container.find('ul.sp-peoplepicker-dropdown'),
                    hiddenInput: container.find('input[type="hidden"]')
                };
            },

            parseValue: function (hiddenInput) {
                var rawValue = hiddenInput.val();
                return rawValue === '' || rawValue === null ? [] : eval(rawValue);
            },

            emptyControl: function (controlContext) {
                controlContext.hiddenInput.val('');
                controlContext.userEditInput.val('');
                controlContext.selectUsersDiv.empty();
                controlContext.dropdown.empty();
            }
        };


        //setup default settings
        var settings = $.extend({
            onLoaded: null,
            minSearchTriggerLength: 3,
            maximumEntitySuggestions: 10,
            principalType: 1,
            principalSource: 15,
            searchPrefix: '',
            searchSuffix: '',
            displayResultCount: 4,
            maxSelectedUsers: 5
        }, options);


        // now we handle our command options
        if (options === 'clear') {

            // clear any matching people picker controls
            return this.each(function () {

                // we expect the target to be a div with a people picker inside                
                var container = $(this).find('div.sp-peoplepicker-wrapper');
                methods.emptyControl(methods.getControlContext(container));
            });
        }
        else if (options === 'get') {

            // get the value of the specified people picker (expects selector to be a single instance)
            // we expect the target to be a div with a people picker inside
            var container = $(this).find('div.sp-peoplepicker-wrapper');
            var controlContext = methods.getControlContext(container);
            return methods.parseValue(controlContext.hiddenInput);
        }
        else if (options === 'set') {

            var value = null;

            if (arguments.length > 1) {

                value = arguments[1];

                // put a single object into an array
                if (!$.isArray(value)) {
                    value = [value];
                }
            }

            // set any matching people picker controls
            return this.each(function () {
                // we expect the target to be a div with a people picker inside                
                var container = $(this).find('div.sp-peoplepicker-wrapper');
                var controlContext = methods.getControlContext(container);

                methods.setSelectedUsers(controlContext, value);
            });
        }
        else {

            return this.each(function () {

                // we expect the target to be a div which we will put things inside
                var peoplePicker = methods.init.call(this);

                if ($.isFunction(settings.onLoaded)) {
                    settings.onLoaded.call(peoplePicker);
                }
            });
        }
    };
    
})(jQuery);