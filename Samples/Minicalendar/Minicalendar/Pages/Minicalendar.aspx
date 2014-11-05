<%@ Page language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<WebPartPages:AllowFraming ID="AllowFraming" runat="server" />

<html>
    <head>
        <title></title>

        <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
        <script type="text/javascript" src="/_layouts/15/MicrosoftAjax.js"></script>
        <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
        <script type="text/javascript" src="/_layouts/15/sp.js"></script>

        <!-- Grid CSS File (only needed for demo page) -->
	    <link rel="stylesheet" href="../Content/paragridma.css">

	    <!-- Core CSS File. The CSS code needed to make eventCalendar works -->
	    <link rel="stylesheet" href="../Content/eventCalendar.css">

	    <!-- Theme CSS file: it makes eventCalendar nicer -->
	    <link rel="stylesheet" href="../Content/eventCalendar_theme_responsive.css">

    </head>
    <body>
        <div id="eventCalendarInline"></div>
    </body>
    <script src="../Scripts/moment.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.eventCalendar.min.js" type="text/javascript"></script>
    <script src="../Scripts/App.js" type="text/javascript"></script>
</html>