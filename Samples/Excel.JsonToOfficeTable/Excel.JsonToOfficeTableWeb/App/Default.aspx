<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Excel.JsonToOfficeTableWeb.App.Default" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.9.1.js" type="text/javascript"></script>
    <link href="../Content/Office.css" rel="stylesheet" type="text/css" />
    <script src="https://appsforoffice.microsoft.com/lib/1.1/hosted/office.js" type="text/javascript"></script>
    <!-- To enable offline debugging using a local reference to Office.js, use:                        -->
    <!-- <script src="../../Scripts/Office/MicrosoftAjax.js" type="text/javascript"></script>  -->
    <!-- <script src="../../Scripts/Office/1.1/office.js" type="text/javascript"></script>  -->
    <link href="App.css" rel="stylesheet" type="text/css" />
    <script src="App.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%;">
        <asp:HiddenField ID="hdnActiveTab" runat="server" Value="1" />
        <div id="nav">
            <div id="nav1" class="navItem">Client-Side</div>
            <div id="nav2" class="navItem">Server-Side</div>
        </div>
        <div id="content">
            <div id="content1" class="content">
                <h2>Client-side Reporting</h2>
                <p>This sample makes a client-side REST call to get stock data and populate a table in the Excel workbook.</p>
                <h3>Stock Symbol:</h3>
                <input type="text" id="txtSymbol1" />
                <h3>Start Date:</h3>
                <select id="cboFromYear1">
                    <option value="2014">2014</option>
                    <option value="2013">2013</option>
                    <option value="2012">2012</option>
                    <option value="2011">2011</option>
                    <option value="2010">2010</option>
                </select>
                <div class="buttonRow">
                    <button id="btnSubmit1">Get History</button>
                </div>
            </div>
            <div id="content2" class="content">
                <h2>Server-side Reporting</h2>
                <p>This sample performs a post-back to get stock data server-side, which is returned as script on the page that is used to populate a table in the Excel workbook.</p>
                <h3>Stock Symbol:</h3>
                <asp:TextBox ID="txtSymbol2" runat="server"></asp:TextBox>
                <h3>Start Date:</h3>
                <asp:DropDownList ID="cboFromYear2" runat="server">
                    <asp:ListItem Text="2014" Value="2014"></asp:ListItem>
                    <asp:ListItem Text="2013" Value="2013"></asp:ListItem>
                    <asp:ListItem Text="2012" Value="2012"></asp:ListItem>
                    <asp:ListItem Text="2011" Value="2011"></asp:ListItem>
                    <asp:ListItem Text="2010" Value="2010"></asp:ListItem>
                </asp:DropDownList>
                <div class="buttonRow">
                    <asp:Button ID="btnSubmit2" runat="server" Text="Get History" OnClick="btnSubmit2_Click" />
                </div>
            </div>
        </div>
    </div>
    <div id="message"></div>
    </form>
</body>
</html>