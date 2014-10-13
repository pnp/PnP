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
            var uri = '/api/demo';

            $(document).ready(function () {
                //Send an AJAX request
                $.get(uri)
                    .done(function (data) {
                        // On success, 'data' contains a list of products.
                        $.each(data, function (key, item) {
                            // Add a list item for the product.
                            $('<li>', { text: formatItem(item) }).appendTo($('#products'));
                        });
                    })
                    .fail(function (jqXHR, textStatus, err) {
                        alert(err);
                    });
            });

            function formatItem(item) {
                return item.Name + ': $' + item.Price;
            }

            function find() {
                var id = $('#prodId').val();
                $.get(uri + '/' + id)
                    .done(function (data) {
                        $('#product').text(formatItem(data));
                    })
                    .fail(function (jqXHR, textStatus, err) {
                        $('#product').text('Error: ' + err);
                    });
            }
        </script>
        <div style="left: 40px; position: absolute;">
            <div>
                <h2>All Products</h2>
                <ul id="products" />
            </div>

            <div>
                <h2>Search by ID</h2>
                <input type="text" id="prodId" size="5" />
                <input type="button" value="Search" onclick="find();" />
                <p id="product" />
            </div>
        </div>
    </form>
</body>
</html>
