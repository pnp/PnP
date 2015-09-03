<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.Services.Authenticate.SharePointWeb.Default" Async="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>

        <script type="text/javascript">

            function callWebAPIService() {
                var uri = '/api/demo';

                $.get(uri)
                    .done(function (data) {
                        var resultingHTML = "";
                        // On success, 'data' contains a list of items.
                        $.each(data, function (key, item) {
                            resultingHTML = resultingHTML + item.Title + " | ";
                        });

                        $('#items').html(resultingHTML);

                    })
                    .fail(function (jqXHR, textStatus, err) {
                        alert(err);
                    });
            }

            function callWebAPIServiceCORS() {
                var uri = 'https://bjansencorswebapi.azurewebsites.net/api/demo?servicestoken=' + getCookie("servicesToken");

                $.get(uri)
                    .done(function (data) {
                        var resultingHTML = "";
                        // On success, 'data' contains a list of items.
                        $.each(data, function (key, item) {
                            resultingHTML = resultingHTML + item.Title + " | ";
                        });
                        $('#items2').html(resultingHTML);
                    })
                    .fail(function (jqXHR, textStatus, err) {
                        alert(err);
                    });
            }

            function getCookie(cname) {
                var name = cname + "=";
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];
                    while (c.charAt(0) == ' ') c = c.substring(1);
                    if (c.indexOf(name) != -1) return c.substring(name.length, c.length);
                }
                return "";
            }

        </script>
        <div style="left: 40px; position: absolute;">
            <h1>Demo preparation</h1>
            Before you can use this demo you'll need to create a test list with data. Click on below button to create this test list.
            <br />
            <asp:Button ID="btnCreateTestData" runat="server" Text="Create test data" OnClick="btnCreateTestData_Click" />
            <br />
            <br />
            <h1>Call a WebAPI service running in the same host</h1>
            The call to this service is done using jQuery and since the service is running on the same host this will just work. In order to allow the WebAPI service to use the CSOM the service first has to be registered using the RegisterWebAPIService method. The call will return the items from the list WebAPIDemo.
            <br />
            <input type="button" value="jQuery call to WebAPI service" onclick="callWebAPIService()" />
            <div id="items"></div>
            <br />
            <br />
            <h1>Call a WebAPI service running in another host (=cross domain)</h1>
            The WebAPI service is implementing CORS to allow a cross domain call from the browser. In order to allow the WebAPI service to use the CSOM the service first has to be registered using the RegisterWebAPIService method. The call will return the items from the list WebAPIDemo.
            <br />
            <input type="button" value="cross domain jQuery call to WebAPI service" onclick="callWebAPIServiceCORS()" />
            <div id="items2"></div>
            <br />
            <br />
            <h1>Demo cleanup</h1>
            You can optionally cleanup the created list. Click on below button to do so.
            <br />
            <asp:Button ID="btnCleanupTestData" runat="server" Text="Cleanup test data" OnClick="btnCleanupTestData_Click" />
            <br />
            <br />
        </div>
    </form>
</body>
</html>
