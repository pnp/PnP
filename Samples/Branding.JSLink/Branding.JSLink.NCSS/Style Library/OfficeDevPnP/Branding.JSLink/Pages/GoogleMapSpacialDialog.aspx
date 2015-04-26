<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>Select a point on the map</title>

    <!-- Google Maps API from google URL -->
    <script src="https://maps.google.com/maps/api/js?sensor=false"></script>

    <!-- Relative Reference to the core jQuery library -->
    <script type="text/javascript" src="../jquery-1.10.2.min.js"></script>

    <!-- SharePoint default branding -->
    <link rel="stylesheet" type="text/css" href="/_layouts/15/1033/styles/Themable/corev15.css?rev=BdxJNFd%2FTPOed3Z8IKEJ9A%3D%3D" />

    <script type="text/javascript">
        function closeAndSendValue() {
            var markerValues = "";

            for (var i = 0; i < markers.length; i++) {
                markerValues += markers[i].position.lat() + "," + markers[i].position.lng() + ";";
            }

            window.frameElement.commitPopup(markerValues);
        }
    </script>

    <style type="text/css">
        #map_canvas {
            width: 100%;
            height: 450px;
        }

        .coords {
            padding-bottom: 10px;
        }

            .coords div {
                display: inline;
                padding-right: 10px;
            }

                .coords div input {
                    margin-left: 10px;
                }

        .top {
            padding-bottom: 10px;
        }

        .instructions {
            padding-bottom: 10px;
        }

        .bottom {
            padding-top: 10px;
        }
    </style>
</head>
<body>
    <div class="buttons top">
        <input type="button" value="Clear Map" title="Remove all elements from the map" onclick="clearMap(); return false;" class="ms-ButtonHeightWidth" />
    </div>

    <div class="instructions">Click on the map to place markers and create your shape. Finish by clicking on the first marker.
    You can drag each of the markers around, or click on them for more options. You can use the Clear Map button above to remove all markers.</div>
    <div id="map_canvas"></div>

    <div class="buttons bottom">
        <input type="button" value="Save" onclick="closeAndSendValue(); return false;" class="ms-ButtonHeightWidth" />
        <input type="button" value="Cancel" onclick="window.frameElement.cancelPopUp(); return false;" class="ms-ButtonHeightWidth" />
    </div>

    <!-- call script to draw the map -->
    <script type="text/javascript" src="DrawMapSpacial.js"></script>
</body>
</html>
