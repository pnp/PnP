
var infoWindow, map, poly, markers = null;
var markers = Array();
var isClosed = false;
var markerImage = {
    url: 'dot.png',
    scaledSize: new google.maps.Size(16, 16),
    anchor: new google.maps.Point(7.5, 7.5)
};

$(document).ready(function () {

    var startPoint = new google.maps.LatLng(35.02999636902565, 31.640625);

    map = new google.maps.Map(document.getElementById('map_canvas'),
        {
            center: startPoint,
            zoom: 3,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });

    poly = new google.maps.Polyline({ map: map, path: [], strokeColor: "#FF0000", strokeOpacity: 1.0, strokeWeight: 2, setEditable: true });

    LoadDialogArguments();

    google.maps.event.addListener(map, 'click', function (clickEvent) {
        if (infoWindow) {
            infoWindow.close();
        }

        if (isClosed) {
            return;
        }

        var markerIndex = poly.getPath().length;
        var isFirstMarker = markerIndex === 0;

        var marker = addMarker(clickEvent.latLng);

        if (isFirstMarker) {
            google.maps.event.addListener(marker, 'click', function () {
                if (isClosed) {
                    return;
                }

                var path = poly.getPath();
                poly.setMap(null);
                poly = new google.maps.Polygon({ map: map, path: path, strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35, setEditable: true });
                isClosed = true;
            });
        }


    });
});

function LoadDialogArguments() {
    // handle the starting value from the query string
    var polylinePath = window.frameElement.dialogArgs;
    if (polylinePath.length > 0) {

        var bounds = new google.maps.LatLngBounds();

        var polylinePathArray = polylinePath.split(";");

        for (var i = 0; i < polylinePathArray.length; i++) {
            if (polylinePathArray[i].length > 0) {
                var coords = polylinePathArray[i].split(",");
                var latLng = new google.maps.LatLng(coords[0], coords[1]);

                // add it to the map (and build the polygon)
                addMarker(latLng);
                bounds.extend(latLng);
            }
        }

        // set the polyline
        var path = poly.getPath();
        poly.setMap(null);
        poly = new google.maps.Polygon({ map: map, path: path, strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35, setEditable: true });
        isClosed = true;

        map.fitBounds(bounds);
    }
}

function addMarker(latLng, index) {
    var marker = new google.maps.Marker({
        map: map,
        position: latLng,
        draggable: true,
        icon: markerImage,
        title: 'Drag to move or click for options'
    });

    google.maps.event.addListener(marker, 'drag', function (dragEvent) {
        onMarkerDrag(marker, dragEvent);
    });

    google.maps.event.addListener(marker, 'click', function () {
        onMarkerClick(marker);
    });

    if (index) {
        // add the marker at the specified location
        markers.splice(index, 0, marker);

        // add the location to the polygon
        poly.getPath().insertAt(index, latLng);
    }
    else {
        // undefined .. just put it at the end
        markers.push(marker);
        poly.getPath().push(latLng);
    }
    return marker;
}
function addMarkerAfter(marker) {
    var currentIndex = getMarkerIndex(marker);
    var isLast = currentIndex == markers.length - 1;

    var nextMarker = null;

    if (isLast) {
        // add at the beginning
        nextMarker = markers[0];
    }
    else {
        // pop it in after the current marker
        nextMarker = markers[currentIndex + 1];
    }

    var newLatLng = getMiddlePoint(marker, nextMarker);
    var newIndex = currentIndex + 1;
    if (isLast) { newIndex = 0; }

    // add the marker in the specified location
    addMarker(newLatLng, newIndex);
}
function addMarkerBefore(marker) {
    var currentIndex = getMarkerIndex(marker);
    var isFirst = currentIndex == 0;

    var previousMarker = null;

    if (isFirst) {
        // add at the beginning
        // so the "previous" marker is actually the last one
        previousMarker = markers[markers.length - 1];
    }
    else {
        // pop it in before the current marker
        previousMarker = markers[currentIndex - 1];
    }

    var newLatLng = getMiddlePoint(previousMarker, marker);
    var newIndex = currentIndex;
    if (isFirst) { newIndex = markers.length; }

    addMarker(newLatLng, newIndex);
}
function removeMarker(marker) {

    var index = getMarkerIndex(marker);

    if (index != -1) {
        // remove from the marker array
        markers.splice(index, 1);

        // remove the point from the polygon
        poly.getPath().removeAt(index);

        // remove the marker from the map
        marker.setMap(null);
        marker = null;
    }
}

function onMarkerDrag(marker, dragEvent) {

    var markerIndex = getMarkerIndex(marker);

    poly.getPath().setAt(markerIndex, dragEvent.latLng);
}
var infoWindowMarker = null;
function onMarkerClick(marker, content) {
    if (!isClosed && getMarkerIndex(marker) == 0) {
        // make sure we don't interfere with "building the polygon"
        return;
    }


    infoWindowMarker = marker;

    // if it already exists, close the window
    if (infoWindow) {
        infoWindow.close();
    }

    // if we didn't receive a content variable
    // then go to the default message
    if (content == undefined) {

        if (isClosed) {
            content = "<h3>Shape Marker</h3>";

            if (markers.length < 4) {
                content += "<p>You cannot remove this marker as you would not have enough markers remaining to maintain a shape.</p>";
            }
            else {
                content += "<p><a href='JavaScript:removeMarker(infoWindowMarker)'>Remove</a></p>";
            }

            content += "<a href='JavaScript:addMarkerBefore(infoWindowMarker)'>Add Before</a>";
            content += " | ";
            content += "<a href='JavaScript:addMarkerAfter(infoWindowMarker)'>Add After</a>";
        }
        else {
            content = "Continue clicking on the map to complete your shape. When you have finished, click on the first marker to complete it.";
        }
    }

    // define our new info window
    infoWindow = new google.maps.InfoWindow({
        content: content,
        maxWidth: 250
    });

    // pop the window open
    infoWindow.open(map, marker);
}

function getMarkerIndex(marker) {

    for (var i = 0; i < markers.length; i++) {
        if (markers[i] == marker) {
            return i;
        }
    }

    return -1;
}
function getMiddlePoint(marker1, marker2) {

    if (marker1.position.lng() < marker2.position.lng()) {
        // marker 1 is the South-West point
        var boundary = new google.maps.LatLngBounds(marker1.position, marker2.position);
        return boundary.getCenter();
    }
    else {
        // marker 2 is the South-West point
        var boundary = new google.maps.LatLngBounds(marker2.position, marker1.position);
        return boundary.getCenter();
    }
}

function clearMap() {
    // remove all of the markers from the map
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }

    // remove the existing polyline
    poly.setMap(null);
    // and reset it 
    poly = new google.maps.Polyline({ map: map, path: [], strokeColor: "#FF0000", strokeOpacity: 1.0, strokeWeight: 2, setEditable: true });
    isClosed = false;

    // clear out the marker array
    markers = Array();
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