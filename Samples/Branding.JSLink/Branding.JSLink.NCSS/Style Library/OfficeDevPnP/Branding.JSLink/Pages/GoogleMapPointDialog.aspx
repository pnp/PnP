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
    <link rel="stylesheet" type="text/css" href="/_layouts/15/1033/styles/Themable/corev15.css?rev=BdxJNFd%2FTPOed3Z8IKEJ9A%3D%3D"/>

    <script type="text/javascript">
        function closeAndSendValue() {
            var longitude = $("#longitude")[0].value;
            var latitude = $("#latitude")[0].value;
            window.frameElement.commitPopup(latitude + "," + longitude);
        }

        $("document").ready(function () {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function (e) {
                        // if successful, show the button
                        $("#btnGetCurrent").css("display", "inline");
                    },
                    function (error) {
                        // on error, hide the button
                        $("#btnGetCurrent").css("display", "none");
                    });
            }
        });
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

        .bottom {
            padding-top: 10px;
        }

    </style>
</head>
<body>
    <div class="coords">
        <div>Latitude<input type="text" id="latitude" /></div>
        <div>Longitude<input type="text" id="longitude" /></div>
    </div>

    <div class="buttons top">
        <input type="button" value="Update Map" onclick="updateMap(); return false;" class="ms-ButtonHeightWidth" />
        <input type="button" id="btnGetCurrent" value="Use Current Location" onclick="getCurrentLocation(true); return false;" class="ms-ButtonHeightWidth" style="display: none;" />
    </div>

    <div id="map_canvas"></div>

    <div class="buttons bottom">
        <input type="button" value="Save" onclick="closeAndSendValue(); return false;" class="ms-ButtonHeightWidth" />
        <input type="button" value="Cancel" onclick="window.frameElement.cancelPopUp(); return false;" class="ms-ButtonHeightWidth" />
    </div>

    <!-- call script to draw the map -->
    <script type="text/javascript" src="DrawMap.js"></script>
</body>
</html>
