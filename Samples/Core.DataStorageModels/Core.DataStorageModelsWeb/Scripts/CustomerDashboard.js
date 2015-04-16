
var customerID = getQueryStringParameter("CustomerID");
var $customerDropdownList;

$(function () {

    var getCustomerIDsUrl = "https://odatasampleservices.azurewebsites.net/V3/Northwind/Northwind.svc/Customers?$format=json&$select=CustomerID";
    $.get(getCustomerIDsUrl).done(getCustomerIDsDone)
        .error(function (jqXHR, textStatus, errorThrown) {
            $('#topErrorMessage').text('Can\'t get customers. An error occurred: ' + jqXHR.statusText);
        }); 

    $customerDropdownList = $('select#CustomerID');
    $customerDropdownList.siblings('button:submit').hide();
    $customerDropdownList.on('change', customerDropdownListChanged);

    if (customerID == '' || customerID == undefined || customerID == null) {
        $('.cdsm_common_display').hide();
        return;
    }

    if ($(".cdsm_customerLanding").length > 0) {
        var url = "https://odatasampleservices.azurewebsites.net/V3/Northwind/Northwind.svc/Customers?$format=json" +
                        "&$select=CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Country,Phone,Fax" +
                        "&$filter=CustomerID eq '" + customerID + "'";
        $.get(url).done(getCustomersDone)
            .error(function (jqXHR, textStatus, errorThrown) {
                $('#customerLandingErrorMessage').text('Can\'t get customer ' + customerID + '. An error occurred: ' + jqXHR.statusText);
            });
    }
    else if ($('.cdsm_notes').length > 0) {
        var scriptbase = hostUrl + "/_layouts/15/";
        $.getScript(scriptbase + "SP.RequestExecutor.js", getNotesAndShow);
        $('#addNote').on('click', function () {
            var note = $('#note').val();
            addNoteToList(note, customerID);
        });
    } 
});

function getCustomersDone(data) {
    if (data.value.length > 0) {
        var customer = data.value[0];
        $('.cdsm_Name').text(customer.CompanyName);
        $('.cdsm_Other_Infor')
            .append($('<div>').text(customer.ContactName + ", " + customer.ContactTitle))
            .append($('<div>').text(customer.Address + ", " + customer.City + ", " + customer.Country))
            .append($('<div>').text(customer.Phone + ", " + customer.Phone))
    }
    else {
        $('#customerLandingErrorMessage').text("No Customer found!");
    }
}

function getCustomerIDsDone(data) {
    $.each(data.value, function (index, value) {
        var itemId = value.CustomerID;
        var option = $('<option>')
            .attr("value", itemId)
            .text(itemId);
        if (itemId == customerID) {
            option.prop("selected", true);
        }
        $customerDropdownList.append(option);
    });
}

function customerDropdownListChanged() {
    $(this).closest('form').submit();
}

function getNotesAndShow() {
    var executor = new SP.RequestExecutor(appWebUrl);
    executor.executeAsync(
       {
           url: appWebUrl + "/_api/web/lists/getByTitle('Notes')/items/" +
                "?$select=FTCAM_Description,Modified,Title,Author/ID,Author/Title" +
                "&$expand=Author/ID,Author/Title" +
                "&$filter=(Title eq '" + customerID + "')",
           type: "GET",
           dataType: 'json',
           headers: { "accept": "application/json;odata=verbose" },
           success: function (data) {
               var value = JSON.parse(data.body);
               showNotes(value.d.results);
           },
           error: getNoteFailed
       }
    );
}

function showNotes(notes) {
    var $notesList = $('ul#notes').html('');
    $.each(notes, function (index, note) {
        var modified = new Date(note.Modified);
        var line1 = $('<p>').text(note.FTCAM_Description);
        var line2 = $('<p>')
            .append($('<span>').addClass('author').text(note.Author.Title))
            .append($('<span>').addClass('modified').text(modified.toLocaleString()));
        $("<li>")
            .addClass('note')
            .append(line1)
            .append(line2)
            .appendTo($notesList);
    });
    $notesList.animate({ scrollTop: $notesList.prop("scrollHeight") }, 1000);
    $('#note').focus().select();
}

function addNoteToList(note, customerID) {
    var executor = new SP.RequestExecutor(appWebUrl);
    var bodyProps = {
        '__metadata': { 'type': 'SP.Data.NotesListItem' },
        'Title': customerID,
        'FTCAM_Description': note
    };
    executor.executeAsync({
        url: appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('Notes')/items?@target='" + appWebUrl + "'",
        contentType: "application/json;odata=verbose",
        method: "POST",
        headers: {
            "accept": "application/json;odata=verbose",
            "content-type": "application/json;odata=verbose",
            "X-RequestDigest": $("#__REQUESTDIGEST").val()
        },
        body: JSON.stringify(bodyProps),
        success: getNotesAndShow,
        error: addNoteFailed
    });
}

function addNoteFailed(data) {
    var value = JSON.parse(data.body);
    var message = 'An error was encountered adding the note:\n' + value.error.message.value;
    $('#addNoteErrorMessage').text(message);
}

function getNoteFailed(data) {
    var value = JSON.parse(data.body);
    var message = 'An error was encountered getting the note:\n' + value.error.message.value;
    $('#getNotesErrorMessage').text(message);
    $('#note').focus().select();
}