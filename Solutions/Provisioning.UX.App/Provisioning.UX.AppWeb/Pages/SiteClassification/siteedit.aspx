<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="siteedit.aspx.cs" Inherits="Provisioning.UX.AppWeb.Pages.SiteClassification.SiteEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Site Classification</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="../../Styles/site.css" rel="stylesheet" type="text/css" />
    <!-- Optionally include jQuery to use Fabric's Component jQuery plugins -->
    <script src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js" type="text/javascript"></script>

    <!-- Fabric core -->
    <link href="https://appsforoffice.microsoft.com/fabric/1.0/fabric.min.css" rel="stylesheet" />
    <link href="https://appsforoffice.microsoft.com/fabric/1.0/fabric.components.min.css" rel="stylesheet" />

    <!-- Application-specific CSS -->
    <link rel="stylesheet" href="../../styles/app.css" />
</head>
<body>
    <!-- Application content goes here -->
    <div class="navBar">
        <div class="ms-fontWeight-semilight" style="padding: 7px 0px 0px 12px; color: white; font-size: 24px;">Office 365</div>
        <div class="NavLine"></div>
        <div class="appTitle ms-fontWeight-semilight">Site Classification</div>
    </div>

    <script type="text/javascript">
        $(function () {
            $('#edit_button').click(function () {
                $.ajax({
                    success: function () {
                    }
                });
            });
            $('#cancel_button').click(function () {
                window.location = $('#Url').val();
            });
            $('#home_button').click(function () {
                window.location = $('#Url').val();
            });
        });
    </script>
    <%--<div class="leftNav">
     
<div class="navlink selected"><div class="ms-Icon ms-Icon--home" style="font-size: 24px;"></div>
<div class="ms-font-m ms-fontWeight-semilight" style="margin-top: 3px; margin-right: 145px; float: right; display: table-cell;"><input class="ms-button" id="home_button" type="image" value="Home"/></div>
          </div></div>--%>



    <form id="form" runat="server">
        <div class="TileContainer">
            <div>
                <div class="TileContent ms-u-fadeIn500">
                    <div class="TextPostion">
                        <!-- Site info tile -->
                        <h2 class="ms-Icon ms-Icon--infoCircle ms-font-xxl" style="margin: 13px;">  Site Information</h2>
                        <div class="ms-Toggle-description ms-font-s-plus" style="margin: 0px 0px 0px 30px;">
                            <p class="ms-Label is-required">Site Owner: <b>
                                <asp:Label ID="siteOwner" runat="server" Visible="true"></asp:Label></b></p>
                            <br />
                            <!-- current division goes here -->
                            <p class="ms-Label is-required">Current Division: <b>
                                <asp:Label ID="lblDivision" runat="server" Visible="true"></asp:Label></b></p>

                            <script type="text/javascript" charset="utf-8">
                                $(document).ready(function () {
                                    $.ajax({
                                        url: "../../Scripts/data/json/divisions.json",
                                        data: {},
                                        dataType: "json",
                                        contentType: "application/json; charset=utf-8",
                                        success: function (data) {
                                            var jsdata = data.divisions;
                                            $('#selectDivision').append($("<option></option>").val("Select...").html("Select..."));
                                            $.each(jsdata, function (key, value) {
                                                $('#selectDivision').append($("<option></option>").val(value.key).html(value.value));
                                            });

                                            var divisions = document.getElementById('selectDivision').options;

                                            if ($('#lblDivision').text() == '') {
                                                $('#selectDivision option:first').attr('selected', 'selected');
                                            }
                                            else {
                                                $("#selectDivision option:contains(" + $('#lblDivision').text() + ")").attr('selected', 'selected');
                                            }
                                        },
                                        error: function (data) {
                                            alert("../../Scripts/data/json/divisions.json not found!");
                                        }
                                    });
                                });
                            </script>

                            <p>
                                <span>Select Division:</span>
                            </p>
                            <!-- select division goes here -->
                            <select name="selectDivision" id="selectDivision" style="width: 230px; margin-left: 13px;">
                            </select>
                            <br />
                            <div>
                                <!-- current segment goes here -->
                                <p class="ms-Label is-required" style="padding-top: 17px;">Current Function: <b>
                                    <asp:Label ID="lblFunction" runat="server" Visible="true"></asp:Label></b></p>
                                <p>
                                    <!-- select segment goes here -->
                                    <span>Select Function:</span>
                                </p>
                                <script type="text/javascript" charset="utf-8">
                                    $(document).ready(function () {
                                        $.ajax({
                                            url: "../../Scripts/data/json/functions.json",
                                            data: {},
                                            dataType: "json",
                                            contentType: "application/json; charset=utf-8",
                                            success: function (data) {
                                                var jsdata = data.functions;
                                                $('#selectFunction').append($("<option></option>").val("Select...").html("Select..."));
                                                $.each(jsdata, function (key, value) {
                                                    $('#selectFunction').append($("<option></option>").val(value.key).html(value.value));
                                                });

                                                var segments = document.getElementById('selectFunction').options;

                                                if ($('#lblFunction').text() == '') {
                                                    $('#selectFunction option:first').attr('selected', 'selected');
                                                }
                                                else {
                                                    //alert("Label text: " + $('#lblRegion').text());
                                                    var selectedFunctions = $('#lblFunction').text().split(',');
                                                    for (var i = 0; i < selectedFunctions.length; i++) {
                                                        $("#selectFunction option:contains(" + selectedFunctions[i].toString() + ")").attr('selected', 'selected');
                                                    }
                                                }
                                            },
                                            error: function (data) {
                                                alert("../../Scripts/data/json/functions.json not found!");
                                            }
                                        });
                                    });
                                </script>
                                <select name="selectFunction" id="selectFunction" style="width: 230px; margin-left: 13px;" multiple="multiple"></select>
                            </div>
                            <br />
                            <div>
                                <!-- current region goes here -->
                                <p class="ms-Label is-required" style="padding-top: 17px;">Current Region: <b>
                                    <asp:Label ID="lblRegion" runat="server" Visible="true"></asp:Label></b></p>
                                <!-- select region goes here -->
                                <p><span>Select Region:</span></p>
                                <script type="text/javascript" charset="utf-8">
                                    $(document).ready(function () {
                                        $.ajax({
                                            url: "../../Scripts/data/json/regions.json",
                                            data: {},
                                            dataType: "json",
                                            contentType: "application/json; charset=utf-8",
                                            success: function (data) {
                                                var jsdata = data.regions;
                                                $('#selectRegions').append($("<option></option>").val("Select...").html("Select..."));
                                                $.each(jsdata, function (key, value) {
                                                    $('#selectRegions').append($("<option></option>").val(value.key).html(value.value));
                                                });

                                                var regions = document.getElementById('selectRegions').options;

                                                if ($('#lblRegion').text() == '') {
                                                    $('#selectRegions option:first').attr('selected', 'selected');
                                                }
                                                else {
                                                    //alert("Label text: " + $('#lblRegion').text());
                                                    $("#selectRegions option:contains(" + $('#lblRegion').text() + ")").attr('selected', 'selected');
                                                }
                                            },
                                            error: function (data) {
                                                alert("../../Scripts/data/json/regions.json not found!");
                                            }
                                        });
                                    });
                                </script>
                                <select name="selectRegions" id="selectRegions" style="width: 230px; margin-left: 13px;"></select>
                                <br />
                                <br />
                                <p>
                                <span>Site Expiration:</span>
                                </p>
                                <!-- select division goes here -->
                                <p>  <asp:Label runat="server" ID="lblExpirationDate"></asp:Label></p>                               
                                <div>
                                    <p style="border-color: rgb(215, 216, 137) !important; padding: 10px; width: 267px; text-align: center; margin-top: 30px; display: block; background-color: rgb(255, 241, 157) !important;">All Sites must have Site Information applied.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="clear: both;"></div>
            </div>
            <div>
                <div id="divSecurityClassification" runat="server">
                    <div>
                        <div class="ms-u-fadeIn500" style="left: 450px; top: 97px; width: 371px; position: absolute; background-color: white;">
                            <div class="TextPostion">
                                <!-- Security Classification Tile -->
                                <h2 class="ms-Icon ms-Icon--shield ms-font-xxl" style="margin: 13px;">  Security Classification</h2>
                                <div style="margin: 10px; padding-left: 16px">
                                    <!-- current site classification goes here -->
                                    <p class="ms-Label is-required">Current Site Data Classification: <b>
                                        <asp:Label ID="lblSitePolicy" runat="server" Visible="true"></asp:Label></b></p>
                                    <p>
                                        <span class="ms-Label">How sensitive is your site?</span>
                                    </p>
                                    <script type="text/javascript" charset="utf-8">
                                        $(document).ready(function () {
                                            $('#BusinessImpact').prepend($("<option></option>").val("Select...").html("Select..."));
                                            setTimeout(function () { $('#BusinessImpact option:first').attr('selected', 'selected'); }, 100);
                                        });
                                    </script>
                                    <!-- select site classification goes here -->
                                    <select name="BusinessImpact" id="BusinessImpact" runat="server" style="width: 230px; margin-left: 13px;" data-val-required="The BusinessImpact field is required." data-val-length-max="128" data-val-length="Security Classification must be a string with a maximum length of 128." data-val="true">
                                    </select>



                                    <div class="ms-Label">
                                        <p>Learn more about  <a href="https://www.contoso.com/sites/information-security/" target="_blank">information classification</a>.</p>
                                        <p>
                                            <div class="ms-status-yellow" aria-live="polite" aria-relevant="all" style="border-color: rgb(215, 216, 137) !important; padding: 10px; width: 267px; text-align: center; margin-top: 18px; display: block; background-color: rgb(255, 241, 157) !important;">All sites must have a security classification</div>
                                    </div>
                                </div>                               

                            </div>
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                </div>
                <div id="divExternalSharing" runat="server">
                    <div class="ms-u-fadeIn500" style="left: 450px; top: 400px; height: 300px; width: 371px; position: absolute; background-color: white;">
                        <div class="TextPostion">
                            <!-- Partner Sharing Tiles-->
                            <h2 class="ms-Icon ms-Icon--share ms-font-xxl" style="margin: 13px;">  External Sharing</h2>
                            <div class="ms-Toggle" style="padding-left: 23px">
                                <!-- Toggle Partner sharing goes here -->
                                <span class="ms-Toggle-description">Turn On/Off External Sharing for this site.</span>
                                <input class="ms-Toggle-input" id="toggleSharing" onchange="" name="toggleSharing" type="checkbox" runat="server" />
                                <label class="ms-Toggle-field" id="lblSharingToggle" for="toggleSharing" runat="server">
                                    <span runat="server" class="ms-Label ms-Label--off">Off</span>
                                    <span runat="server" class="ms-Label ms-Label--on">On</span>                                    
                                </label>
                            </div>
                            <div class="ms-Label" style="padding-top: 0px; padding-bottom: 32px; margin-left: 26px;">
                                <p>Learn more about  <a href="https://team.contoso.com/sites/information-security/" target="_blank">Information Handling Guidelines</a>.</p>
                                <p>
                                    <div class="ms-status-yellow" aria-live="polite" aria-relevant="all" style="border-color: rgb(215, 216, 137) !important; padding: 10px; width: 267px; text-align: center; margin-top: 30px; display: block; background-color: rgb(255, 241, 157) !important;">Site Owners accept the responsibility of enabling this option. Please share responsibly!</div>
                            </div>
                            <input id="Url" name="Url" type="hidden" value="" runat="server" />
                                <p style="margin-top: 16%; margin-left: 49%">
                                    <asp:Button CssClass="ms-Button" runat="server" ID="submit_button" Text="OK" OnClick="Submit_Click" />
                                    <input class="ms-Button" id="cancel_button" type="button" value="Back" />
                                </p>
                        </div>
                    </div>
                    <div style="clear: both;"></div>
                </div>                
            </div>

        </div>
    </form>
</body>
</html>
