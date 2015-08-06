<script type="text/javascript" src="~sitecollection/Style%20Library/OfficeDevPnP/jquery-1.11.2.min.js"></script>
<script type="text/javascript" src="~sitecollection/Style%20Library/OfficeDevPnP/knockout-3.3.0.js"></script>
<script type="text/javascript" src="~sitecollection/Style%20Library/OfficeDevPnP/event-registration-form.js"></script>

<div data-bind="template: {name: 'EventRegistrationForm-Template'}" id="EventRegistrationForm">Loading...</div>

<script type="text/html" id="EventRegistrationForm-Template">
    <!-- ko ifnot:isLoaded -->
    <p>Loading event registration information...</p>
    <!-- /ko -->
    <!-- ko if:isLoaded -->
        <!-- ko ifnot:isError-->
            <p data-bind="ifnot: isRegistrationAllowed">
                <b>Registrations for this event are not accepted.</b>
            </p>
            <p>
                <b data-bind="if: userIsRegistered">You are already registered for this event.</b>
                <b data-bind="ifnot: userIsRegistered">You are not registered for this event.</b>
                <!-- ko if:isRegistrationAllowed -->
                    <input data-bind="attr: { value: userIsRegistered() ? 'Unregister' : 'Register' }, click: registerClicked" class='ms-ButtonHeightWidth' type='button' id='btnRegister' />
                <!-- /ko -->
            </p>
            <p>Total number of registered attendees: <b data-bind="text: registeredAttendeesCount"></b>.</p>
            <!-- ko if:registeredAttendeesCount() > 0 -->
                <p>Attendee list:</p>
    <table border="0" cellspacing="0" cellpadding="2" class="ms-listviewtable">
        <thead>
            <tr>
                <th class="ms-vh2">Name</th>
                <th class="ms-vh2">Email</th>
            </tr>
        </thead>
        <tbody data-bind="foreach: attendeesList.sort(function (l, r) { return l.FullName > r.FullName ? 1 : -1 })">
            <tr>
                <td class="ms-vb"><!--ko text: FullName--><!--/ko--></td>
                <td class="ms-vb"><a class="ms-link" data-bind="text: Email, attr: {href: 'mailto:' + Email}"></a></td>
            </tr>
        </tbody>
    </table>
            <!-- /ko -->
        <!-- /ko -->
        <!-- ko if:isError-->
            <p><b>Error</b></p>
            <p>Unable to load even registration information.</p>
            <p>Error message: <span data-bind="text: errorMessage"></span></p>
            <p>Please try reloading the page.</p>
        <!-- /ko -->
    <!-- /ko -->
</script>
