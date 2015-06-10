var map, geocoder, marker = null;

$(document).ready(function () {
    // authentication 
    $.support.cors = true;
    var user = false;
    var userid = 0;

    var startPoint = new google.maps.LatLng(35.02999636902565, 31.640625);

    // create map and set the center location
    var mapOptions = {
        center: startPoint,
        zoom: 3,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    // render map over our "map_canvas" div
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

    // set map boundary
    bounds = map.getBounds();

    // set the onclick event
    google.maps.event.addListener(map, "click", function (event) {
        dropPin(event.latLng);
    })

    // handle the passed query string value (if applicable)
    var latLongQueryString = getQuerystring('latlong');

    if (latLongQueryString) {
        dropPin(latLongQueryString);
        map.setZoom(5);
    }
    else {
        // try to get the current coordinates
        getCurrentLocation();
    }
    
});

function dropPin(latLng) {
    // remove any existing markers
    if (marker) {
        marker.setMap(null);
        marker = null;
    }

    if (typeof (latLng) == "string") {

        var values = latLng.split(",");

        var latitude = values[0];
        var longitude = values[1];

        latLng = new google.maps.LatLng(latitude, longitude);
    }
    
    marker = new google.maps.Marker({
        position: latLng,
        map: map
    });

    // store lat-long in our textbox
    $('#latitude').val(latLng.lat());
    $('#longitude').val(latLng.lng());
    map.panTo(latLng);
}

function getCurrentLocation(showErrors) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            // on success
            function (e) {
                // save the value and update the map
                $("#longitude").val(e.coords.longitude);
                $("#latitude").val(e.coords.latitude);
                updateMap();
                map.setZoom(5);
            },
            // on error
            function (error) {
                if (showErrors) {
                    var errors = {
                        1: 'Permission denied',
                        2: 'Position unavailable',
                        3: 'Request timeout'
                    };

                    alert("Geolocation is either not enabled or not supported by your browser (" + errors[error.code] + ")");
                }
            });
    }
}

function updateMap() {
    var val = $("#latitude").val() + "," + $("#longitude").val();
    dropPin(val);
}

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