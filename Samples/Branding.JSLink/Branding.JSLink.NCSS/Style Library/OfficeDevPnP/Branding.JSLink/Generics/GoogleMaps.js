// create unique namespace
var jslinkGoogleMaps = jslinkGoogleMaps || {};

// objects for point and spacial values
jslinkGoogleMaps.PointValue = {};
jslinkGoogleMaps.SpacialValue = {};

/* Generic Methods 
   =============== */

// checks if the input string is a valid set of longitude / latitude coordinates
jslinkGoogleMaps.IsCoordsValid = function (Coords) {
    // check the string is in the right format (number, number)
    if (Coords == undefined ||
        Coords.indexOf(",") == -1) {
        // not valid
        return false;
    }

    // split down the comma separated value into individual strings
    var values = Coords.split(",");

    // check that we have a valid longitude / latitude string 
    if (values.length != 2 ||
       (values[0] < -180 || values[0] > 180) ||
        (values[1] < -180 || values[1] > 180)) {
        // not valid
        return false;
    }

    return true;
}

// draw a map with a pin at the specified coords
jslinkGoogleMaps.CreateMap = function (MapDivID, Coords, isThumbnail) {
    // don't execute this until the Google Maps API has been loaded
    ExecuteOrDelayUntilScriptLoaded(function () {
        var LatLngPoint = null;

        if (jslinkGoogleMaps.IsCoordsValid(Coords)) {
            var values = Coords.split(",");
            var latitude = values[0];
            var longitude = values[1];

            // load up the points as the start point
            LatLngPoint = new google.maps.LatLng(latitude, longitude);

            var mapOptions = null;

            if (isThumbnail == undefined) {
                // create map and set the center location
                mapOptions = {
                    center: LatLngPoint,
                    zoom: 5,
                    streetViewControl: false,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
            }
            else if (isThumbnail) {
                // create map and set the center location
                // designed for preview thumbnail
                mapOptions = {
                    center: LatLngPoint,
                    zoom: 3,
                    mapTypeControl: false,
                    zoomControl: false,
                    streetViewControl: false,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
            }

            // render map over our "map_canvas" div
            var map = new google.maps.Map(document.getElementById(MapDivID), mapOptions);

            // set map boundary
            map.getBounds();

            // drop the "pin" on the target location
            var marker = new google.maps.Marker({
                position: LatLngPoint,
                map: map
            });
        }
        else {
            jQuery("#" + MapDivID).html("<span>Map Coordinates not valid</span>");
        }
    }, "jslink_GoogleMapsAPI");
}

/* Point Value Methods
   =================== */
jslinkGoogleMaps.PointValue.view = function (viewContext) {
    var itemID = viewContext.CurrentItem["ID"];
    var itemTitle = viewContext.CurrentItem["Title"];
    var Coords = viewContext.CurrentItem[viewContext.CurrentFieldSchema.Name];

    if (Coords.length == 0) {
        return "";
    }

    var returnHtml = "<div id='mapPreview" + viewContext.CurrentFieldSchema.Name + itemID + "' style='width: 200px; height: 150px;'>&nbsp;</div>";
    returnHtml += "<input id='mapCoords" + viewContext.CurrentFieldSchema.Name + itemID + "' type='hidden' value='" + Coords + "' />";

    var googleMapsUrl = "https://maps.google.co.uk/maps?ll=" + Coords + "&q=" + Coords + "+(" + itemTitle + ")&z=6";
    returnHtml += "<div><a href='" + googleMapsUrl + "' target='_blank'>View on Google Maps</a></div>";

    // load up the map
    jslinkGoogleMaps.PointValue.loadViewMap(itemID, viewContext.CurrentFieldSchema.Name);

    return returnHtml;
};
jslinkGoogleMaps.PointValue.displayForm = function (formContext) {
    var returnHtml = "<div id='mapPreview" + formContext.CurrentFieldSchema.Name + "' style='display: block; width: 450px; height: 300px;'>&nbsp</div>";
    returnHtml += "<input id='hiddenLatLong" + formContext.CurrentFieldSchema.Name + "' type='hidden' value='" + formContext.CurrentFieldValue + "'/>";

    var googleMapsUrl = "https://maps.google.co.uk/maps?ll=" + formContext.CurrentFieldValue + "&q=" + formContext.CurrentFieldValue + "+(" + formContext.CurrentItem['Title'] + ")&z=6";

    returnHtml += "<div><a href='" + googleMapsUrl + "' target='_blank'>View on Google Maps</a></div>";

    jslinkGoogleMaps.PointValue.loadMap(formContext.CurrentFieldSchema.Name);

    return returnHtml;
};
jslinkGoogleMaps.PointValue.editForm = function (formContext) {

    // register callbacks
    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(formContext);
    formCtx.registerGetValueCallback(formCtx.fieldName, jslinkGoogleMaps.PointValue.getFieldValue.bind(null, formCtx.fieldName));

    var returnHtml = "<table id=" + formContext.fieldName + " width='100%'>";
    returnHtml += "<tr>";
    returnHtml += "<td colspan='2'><a href='#' onclick='JavaScript:jslinkGoogleMaps.PointValue.showMapDialog(\"" + formContext.CurrentFieldSchema.Name + "\"); return false;'>Select Location</a></td>";
    returnHtml += "</tr>";
    returnHtml += "<tr><td colspan='2'>Location data will be sent to Google Maps. <a href='http://www.google.com/intl/en_ALL/help/terms_maps.html'>Learn more</a></td></tr>";
    returnHtml += "</table>";

    returnHtml += "<input id='hiddenLatLong" + formContext.CurrentFieldSchema.Name + "' type='hidden' value='" + formContext.CurrentFieldValue + "'/>";

    returnHtml += "<div id='mapPreview" + formContext.CurrentFieldSchema.Name + "' style='display: block; width: 450px; height: 300px;' />";

    jslinkGoogleMaps.PointValue.loadMap(formContext.CurrentFieldSchema.Name);

    return returnHtml;
};

jslinkGoogleMaps.PointValue.loadMap = function (fieldName) {
    // then wait until the document is ready
    jQuery("document").ready(function () {

        // retrieve current lat / long values
        var latLongString = jQuery("#hiddenLatLong" + fieldName).val();

        if (jslinkGoogleMaps.IsCoordsValid(latLongString)) {

            // make the map visible
            jQuery("#mapPreview").css("display", "block");

            // and build it
            jslinkGoogleMaps.CreateMap("mapPreview" + fieldName, latLongString);
        }
        else {
            // hide the map area
            jQuery("#mapPreview").css("display", "none");
        }
    });
}
jslinkGoogleMaps.PointValue.loadViewMap = function (ItemID, fieldName) {
    jQuery("document").ready(function () {
        // pull the coordinates
        var coords = jQuery("#mapCoords" + fieldName + ItemID).val();

        // create the map
        jslinkGoogleMaps.CreateMap("mapPreview" + fieldName + ItemID, coords, true);
    });
};

jslinkGoogleMaps.PointValue.showMapDialog = function (fieldName) {
    ExecuteOrDelayUntilScriptLoaded(function () {
        var latLong = jQuery("#hiddenLatLong" + fieldName).val();
        
        var dialogUrl = _spPageContextInfo.webAbsoluteUrl + "/Style Library/OfficeDevPnP/Branding.JSLink/Pages/GoogleMapPointDialog.aspx";
        
        if(latLong)
        {
            dialogUrl += "?latlong=" + latLong;
        }

        // need to get the country ID from the Query String
        var countryId = getQuerystring("CountryId", 1);
        if (countryId > 0) {
            if (latLong) {
                dialogUrl += "&CountryId=" + countryId;
            }
            else {
                dialogUrl += "?CountryId=" + countryId;
            }
        }

        var options = {
            url: dialogUrl,
            width: 800,
            height: 600,
            dialogReturnValueCallback: jslinkGoogleMaps.PointValue.dialogCallback.bind(null, fieldName)
        };

        SP.UI.ModalDialog.showModalDialog(options);

    }, 'SP.UI.Dialog.js');
};
jslinkGoogleMaps.PointValue.dialogCallback = function (fieldName, dialogResult, returnValue) {
    if (dialogResult == SP.UI.DialogResult.OK) {
        // update the lat / long value
        jQuery("#hiddenLatLong" + fieldName).val(returnValue);

        // show the map preview
        jslinkGoogleMaps.PointValue.loadMap(fieldName);
    }
};
jslinkGoogleMaps.PointValue.getFieldValue = function (fieldName) {
    return jQuery("#hiddenLatLong" + fieldName).val();
};


/* Spacial Value Methods
   ===================== */

jslinkGoogleMaps.SpacialValue.view = function (viewContext) {
    var itemID = viewContext.CurrentItem["ID"];
    var itemTitle = viewContext.CurrentItem["Title"];
    var spacialPath = viewContext.CurrentItem[viewContext.CurrentFieldSchema.Name];

    if (spacialPath.length == 0) {
        return "";
    }

    var returnHtml = "<div id='mapPreviewSpacial" + itemID + "' style='width: 200px; height: 150px;'>&nbsp;</div>";
    returnHtml += "<input id='mapSpacialPath" + itemID + "' type='hidden' value='" + spacialPath + "' />";

    // load up the map
    jslinkGoogleMaps.SpacialValue.loadViewMap(itemID);

    return returnHtml;
};
jslinkGoogleMaps.SpacialValue.displayForm = function (formContext) {
    var returnHtml = "<div id='mapPreviewSpacial' style='display: block; width: 450px; height: 300px;'>&nbsp</div>";
    returnHtml += "<input id='hiddenSpacialPath' type='hidden' value='" + formContext.CurrentFieldValue + "'/>";

    var googleMapsUrl = "https://maps.google.co.uk/maps?ll=" + formContext.CurrentFieldValue + "&q=" + formContext.CurrentFieldValue + "+(" + formContext.CurrentItem['Title'] + ")&z=6";

    returnHtml += "<div><a href='" + googleMapsUrl + "' target='_blank'>View on Google Maps</a></div>";

    jslinkGoogleMaps.SpacialValue.loadMap();

    return returnHtml;
};
jslinkGoogleMaps.SpacialValue.editForm = function (formContext) {
    // register callbacks
    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(formContext);
    formCtx.registerGetValueCallback(formCtx.fieldName, jslinkGoogleMaps.SpacialValue.getFieldValue.bind(null, formCtx.fieldName));

    var returnHtml = "<table id=" + formContext.fieldName + " width='100%'>";
    returnHtml += "<tr>";
    returnHtml += "<td colspan='2'><a href='JavaScript: {};' onclick='JavaScript:jslinkGoogleMaps.SpacialValue.showMapDialog(); return false;'>Edit Shape</a></td>";
    returnHtml += "</tr>";
    returnHtml += "<tr><td colspan='2'>Location data will be sent to Google Maps. <a href='http://www.google.com/intl/en_ALL/help/terms_maps.html'>Learn more</a></td></tr>";
    returnHtml += "</table>";

    returnHtml += "<input id='hiddenSpacialPath' type='hidden' value='" + formContext.CurrentFieldValue + "'/>";

    returnHtml += "<div id='mapPreviewSpacial' style='display: block; width: 450px; height: 300px;' />";

    jslinkGoogleMaps.SpacialValue.loadMap();

    return returnHtml;
};

jslinkGoogleMaps.SpacialValue.loadMap = function () {
    // then wait until the document is ready
    jQuery("document").ready(function () {

        // retrieve current lat / long values
        var polylinePath = jQuery("#hiddenSpacialPath").val();
        polylinePath = polylinePath.replace("<div dir=\"\">", "");
        polylinePath = polylinePath.replace("</div>", "");

        if (polylinePath.length == 0) {
            jQuery("#mapPreviewSpacial").css("display", "none");
        }
        else {
            jQuery("#mapPreviewSpacial").css("display", "block");

            // build generic map
            jslinkGoogleMaps.SpacialValue.CreateMap("mapPreviewSpacial", polylinePath);
        }

    });
}
jslinkGoogleMaps.SpacialValue.loadViewMap = function (ItemID) {
    jQuery("document").ready(function () {
        // pull the coordinates
        var spacialPath = jQuery("#mapSpacialPath" + ItemID).val();

        // create the map
        jslinkGoogleMaps.SpacialValue.CreateMap("mapPreviewSpacial" + ItemID, spacialPath, true);
    });
};

jslinkGoogleMaps.SpacialValue.CreateMap = function (MapDivID, SpacialPath, isThumbnail) {
    // don't execute this until the Google Maps API has been loaded
    ExecuteOrDelayUntilScriptLoaded(function () {

        var startPoint = new google.maps.LatLng(35.02999636902565, 31.640625);

        var mapOptions = null;

        if (isThumbnail == undefined) {
            // create map and set the center location
            mapOptions = {
                center: startPoint,
                zoom: 5,
                streetViewControl: false,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
        }
        else if (isThumbnail) {
            // create map and set the center location
            // designed for preview thumbnail
            mapOptions = {
                center: startPoint,
                zoom: 3,
                mapTypeControl: false,
                zoomControl: false,
                streetViewControl: false,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
        }

        // render map over our "map_canvas" div
        var map = new google.maps.Map(document.getElementById(MapDivID), mapOptions);
        poly = new google.maps.Polygon({ map: map, path: [], strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35, setEditable: true });
        var bounds = new google.maps.LatLngBounds();

        var spacialPathArray = SpacialPath.split(";");

        for (var i = 0; i < spacialPathArray.length; i++) {
            if (spacialPathArray[i].length > 0) {
                var coords = spacialPathArray[i].split(",");
                var latLng = new google.maps.LatLng(coords[0], coords[1]);

                // build the polygon
                poly.getPath().push(latLng)
                bounds.extend(latLng);
            }
        }

        //map.setCenter(bounds.getCenter());
        map.fitBounds(bounds);

    }, "jslink_GoogleMapsAPI");

}

jslinkGoogleMaps.SpacialValue.showMapDialog = function () {
    ExecuteOrDelayUntilScriptLoaded(function () {
        var polylinePath = jQuery("#hiddenSpacialPath").val();


        var dialogUrl = _spPageContextInfo.webAbsoluteUrl + "/Style Library/OfficeDevPnP/Branding.JSLink/Pages/GoogleMapSpacialDialog.aspx";

        // need to get the country ID from the Query String
        var countryId = getQuerystring("CountryId", 1);
        if (countryId > 0) {
           dialogUrl += "?CountryId=" + countryId;
        }

        var options = {
            url: dialogUrl,
            args: polylinePath, // must use argument not query string because it might be too long for the URL (and get truncated)
            width: 800,
            height: 600,
            dialogReturnValueCallback: jslinkGoogleMaps.SpacialValue.dialogCallback
        };

        SP.UI.ModalDialog.showModalDialog(options);

    }, 'SP.UI.Dialog.js');
};
jslinkGoogleMaps.SpacialValue.dialogCallback = function (dialogResult, returnValue) {
    if (dialogResult == SP.UI.DialogResult.OK) {
        // update the lat / long value
        jQuery("#hiddenSpacialPath").val(returnValue);

        // show the map preview
        jslinkGoogleMaps.SpacialValue.loadMap();
    }
};
jslinkGoogleMaps.SpacialValue.getFieldValue = function (fieldName) {
    return jQuery("#hiddenSpacialPath").val();
};

jslinkGoogleMaps.ScriptLoaded = function () {
    SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs("jslink_GoogleMapsAPI");
};

(function () {
    if (document.getElementById("GoogleMapsAPI") == undefined) {
        // load Google Maps API
        var script = document.createElement("script");
        script.id = "GoogleMapsAPI";
        script.src = "https://maps.google.com/maps/api/js?sensor=false&callback=jslinkGoogleMaps.ScriptLoaded";
        script.type = "text/javascript";
        document.getElementsByTagName("head")[0].appendChild(script);
    }
})();

function getQuerystring(key, default_) {
    if (default_ == null) default_ = "";
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
    var qs = regex.exec(window.location.href);
    if (qs == null)
        return default_;
    else
        return qs[1];
}