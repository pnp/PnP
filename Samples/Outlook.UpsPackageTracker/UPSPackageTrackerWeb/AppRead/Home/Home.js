/// <reference path="../App.js" />

(function () {
    "use strict";

    // The Office initialize function must be run each time a new page is loaded
    Office.initialize = function (reason) {
        $(document).ready(function () {
            app.initialize();

            displayItemDetails();
            $("#trackingNumberSelection").bind('change', function () {
                getTrackingData($("#trackingNumberSelection").val());
            });
        });
    };

    // Displays the "Subject" and "From" fields, based on the current mail item
    function displayItemDetails() {
        var item = Office.cast.item.toItemRead(Office.context.mailbox.item);
        var matches = item.getRegExMatchesByName("UPSTrackingNumberInBody");

        if (matches && matches.length > 0) {
            if (matches.length > 1) {
                $("#trackingNumberSelection").empty();
                $.each(matches, function (idx, match) {
                    $("#trackingNumberSelection").append($("<option>" + match + "</option>"));
                })
                $("#multipleTrackingNumbers").show();
            } else {
                $("#multipleTrackingNumbers").hide();
            }

            var firstMatch = matches[0];
            getTrackingData(firstMatch);
        } else {
            app.showNotification('error', 'Could not find a valid tracking number');
        }

    }

    String.prototype.splice = function (idx, rem, s) {
        return (this.slice(0, idx) + s + this.slice(idx + Math.abs(rem)));
    };

    function getTrackingData(trackingNumber) {
        $("#trackingResult").hide('fast');
        $("#loadingNotification").show('fast');
        $("#trackingNumberLink").attr({ href: "http://wwwapps.ups.com/WebTracking/track?track=yes&trackNums=" + trackingNumber });
        $("#trackingNumberLink").text(trackingNumber);
        $('#trackingTable > tbody').empty();
        $.getJSON("../../api/UPSTracking/" + trackingNumber, {}, function (data) {
            if (data) {
                var packageInfo = data.shipmentField[0].packageField[0];
                $("#trackingNumber").text(packageInfo.trackingNumberField);
                //var trackingTable = $("#trackingTable");
                $.each(packageInfo.activityField, function (i, activity) {
                    var addressStr = ""
                    if (activity.activityLocationField) {
                        if (activity.activityLocationField.addressField.cityField) { addressStr += activity.activityLocationField.addressField.cityField + ", "; }
                        if (activity.activityLocationField.addressField.stateProvinceCodeField) { addressStr += activity.activityLocationField.addressField.stateProvinceCodeField + ", "; }
                        if (activity.activityLocationField.addressField.countryCodeField) { addressStr += activity.activityLocationField.addressField.countryCodeField }
                    }

                    $('#trackingTable > tbody:last').append($("<tr>" +
                        "<td>" + addressStr +
                        "</td><td>" + activity.dateField.splice(4, 0, "-").splice(7, 0, "-") + " " + activity.timeField.splice(2, 0, ":").splice(5, 0, ":") +
                        "</td><td>" + activity.statusField.descriptionField + "</td><tr>"));
                })
            }
        })
        .success(function () {
            $("#trackingResult").show('fast');
            $("#loadingNotification").hide('fast');
        })
        .fail(function (event, jqxhr, settings, thrownError) {
            app.showNotification('failure', event.responseText.ExceptionMessage)
        })
        .always(function () {
            $(".glyphicon.spinning").hide('fast');
        });

    }
})();