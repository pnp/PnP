var OfficeDevPnP = OfficeDevPnP || {};
OfficeDevPnP.Core = OfficeDevPnP.Core || {};

OfficeDevPnP.Core.EventRegistration = (function () {
    //private members
    function AppViewModel() {
        var self = this;
        self.isEventInformationLoaded = ko.observable(false);
        self.isAttendeeInformationLoaded = ko.observable(false);
        self.isLoaded = ko.computed(function () {
            return this.isEventInformationLoaded() && this.isAttendeeInformationLoaded();
        }, this);
        self.isError = ko.observable(false);
        self.eventEndDate = ko.observable(null);
        self.eventRegistrationAllowed = ko.observable(false);
        self.errorMessage = ko.observable("");
        self.registeredAttendeesCount = ko.observable(-1);
        self.isRegistrationAllowed = ko.computed(function () {
            var endDate = self.eventEndDate();
            var now = new Date();
            return self.eventRegistrationAllowed() && now < endDate;
        }, this);
        self.userIsRegistered = ko.observable(false);
        self.isProcessingEventRegistrationRequest = ko.observable(false);
        self.attendeesList = ko.observableArray([]);

        self.registerClicked = function () {
            if (self.isProcessingEventRegistrationRequest())
                return;
            self.isProcessingEventRegistrationRequest(true);

            // check if the user is already registered
            var url = eventRegistrationListItemsRESTUrl + "?$filter=OfficeDevPnPEventLookupId eq " + itemId + " and AuthorId eq " + currentUserId + "&$select=ID&$top=10";
            $.ajax({
                url: url,
                type: "GET",
                headers: {
                    "accept": "application/json;odata=verbose",
                },
                success: function (data) {
                    var oList = clientContext.get_web().get_lists().getByTitle(eventRegistrationListTitle);
                    if (!self.userIsRegistered() && data.d.results.length == 0) {
                        // user is not registered -> register
                        var itemCreateInfo = new SP.ListItemCreationInformation();
                        var oListItem = oList.addItem(itemCreateInfo);
                        oListItem.set_item('OfficeDevPnPEventLookup', itemId);
                        oListItem.update();
                        clientContext.load(oListItem);
                        executeClientContextQuery();
                    } else if (self.userIsRegistered() && data.d.results.length > 0) {
                        // user is already registered -> unregister
                        $.each(data.d.results, function (index, item) {
                            oListItem = oList.getItemById(item.ID);
                            oListItem.deleteObject();
                            executeClientContextQuery();
                        });
                    } else {
                        self.reloadData();
                    }
                },
                error: function (error) {
                    alert("Unable to complete registration request: " + error);
                    self.reloadData();
                }
            });
        }
        function executeClientContextQuery() {
            clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
                // success
                self.reloadData();
            }), Function.createDelegate(this, function (sender, args) {
                // error
                alert("Unable to complete registration request: " + args.get_message());
                self.reloadData();
            }));
        }

        self.resetData = function () {
            self.isError(false);
            self.errorMessage("");
            self.registeredAttendeesCount(-1);
            self.isEventInformationLoaded(false);
            self.isAttendeeInformationLoaded(false);
            self.eventEndDate(null);
            self.eventRegistrationAllowed(false);
            self.userIsRegistered(false);
            self.isProcessingEventRegistrationRequest(false);
            self.attendeesList = ko.observableArray([]);
        }

        self.reloadData = function () {
            self.resetData();

            clientContext = SP.ClientContext.get_current();
            JSRequest.EnsureSetup();
            itemId = JSRequest.QueryString["ID"];
            listId = _spPageContextInfo.pageListId;
            currentUserId = _spPageContextInfo.userId;
            isProcessingEventRegistrationRequest = false;
            eventRegistrationListItemsRESTUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/Web/Lists/GetByTitle('" + eventRegistrationListTitle + "')/items";
            currentEventItemRESTUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/Web/Lists('" + listId + "')/items(" + itemId + ")";

            // load event information
            $.ajax({
                url: currentEventItemRESTUrl,
                type: "GET",
                headers: {"accept": "application/json;odata=verbose"},
                success: function (data) {
                    var endDate = new Date(data.d.EndDate);
                    self.eventEndDate(endDate);
                    self.eventRegistrationAllowed(data.d.OfficeDevPnPRegistrationAllowed);
                    self.isEventInformationLoaded(true);
                },
                error: function (error) {
                    self.handleErrorOnLoad(error);
                }
            });

            // load attendee information
            var url = eventRegistrationListItemsRESTUrl + "?$filter=OfficeDevPnPEventLookupId eq " + itemId + "&$select=ID,Title,Author/Id,Author/Title,Author/EMail&$expand=Author&$orderby=Author/Title";
            $.ajax({
                url: url,
                type: "GET",
                headers: {"accept": "application/json;odata=verbose"},
                success: function (data) {
                    self.registeredAttendeesCount(data.d.results.length);
                    self.attendeesList.removeAll();

                    $.each(data.d.results, function (index, item) {
                        if (item.Author.Id == currentUserId) {
                            self.userIsRegistered(true);
                        }

                        self.attendeesList.push(
                            {
                                FullName: item.Author.Title,
                                Email: item.Author.EMail
                            });
                    });
                    self.isAttendeeInformationLoaded(true);
                },
                error: function (error) {
                    self.handleErrorOnLoad(error);
                }
            });
        }

        self.handleErrorOnLoad = function(error) {
            self.isError(true);
            self.errorMessage(error);
        }

        var clientContext;
        var itemId;
        var listId;
        var currentUserId;
        var isProcessingEventRegistrationRequest;
        var eventRegistrationListTitle = "Event Registration";
        var eventRegistrationListItemsRESTUrl;
        var currentEventItemRESTUrl;
    }
    return {
        // public interface
        initializeForm: function () {
            var viewModel = new AppViewModel();
            ko.applyBindings(viewModel, document.getElementById('EventRegistrationForm'));
            viewModel.reloadData();
        },
    };
})();

$(document).ready(function () {
    SP.SOD.executeFunc('sp.js', 'SP.ClientContext', OfficeDevPnP.Core.EventRegistration.initializeForm);
});

