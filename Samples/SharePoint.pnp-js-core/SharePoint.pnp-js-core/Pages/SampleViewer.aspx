<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <!-- utility js -->
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/bootstrap.min.js"></script>

    <!-- Polyfills to ensure browser functionality -->
    <script type="text/javascript" src="../Scripts/es6-promise.min.js"></script>
    <script type="text/javascript" src="../Scripts/fetch.js"></script>

    <script type="text/javascript" src="../Scripts/sample.js"></script>
    <meta name="WebPartPageExpansion" content="full" />

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/bootstrap.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript">

        function resetView() {
            $("#sample-content").append("<img src=\"/_layouts/images/gears_an.gif\" />");
            $("#sample-code").empty();
            $("#embed-code").empty();
            $("#sample-show").empty();
        }

        $(function () {

            sample.ensureContextQueryString();

            // setup and load the navigation
            $("#sample-nav").on("click", "a:not('.directLink')", function (e) {
                e.preventDefault();

                resetView();

                var link = $(e.target);
                link.closest(".list-group").find(".list-group-item").removeClass("active");
                link.closest(".list-group-item").addClass("active");
                $.get(sample.appWebUrl() + "\\samples\\" + link.attr("href")).done(function (content) { $("#sample-content").empty().append(content); });
            });

            // bind the button click to run the sample

            $("#sample-run").on("click", function (e) { sample.run(e); });

            // set the link to the host web for embed section
            $(".hostWebAddress").attr("href", sample.hostWebUrl());

            if (window.location.hash.length > 0) {
                var page = window.location.hash.replace("#", "");
                $.get(sample.appWebUrl() + "\\samples\\" + page).done(function (content) { $("#sample-content").empty().append(content); });
            }
        });

    </script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    PnP Sample Viewer Add-In : View Samples
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="container-fluid" style="margin-top: 30px;">
        <div class="row">

            <div class="col-md-2">
                <ul id="sample-nav" class="list-group">
                    <!-- This is where additional samples will be added, the href should point to the sample content -->
                    <li class="list-group-item"><a href="default.aspx" class="directLink">Home</a></li>
                    <li class="list-group-item"><a href="helloworld.html">Hello World</a></li>
                    <li class="list-group-item"><a href="RequireBasic.html">Hello World (requireJS)</a></li>
                    <li class="list-group-item"><a href="ReadListItems.html">Read List Items</a></li>
                    <li class="list-group-item"><a href="ReadListItemsPaged.html">List Item Paging</a></li>
                    <li class="list-group-item"><a href="CachingBasic.html">Caching Basics</a></li>
                    <li class="list-group-item"><a href="Logging.html">Logging</a></li>
                    <li class="list-group-item"><a href="LoggingAdvanced.html">Logging Advanced</a></li>                    
                    <li class="list-group-item"><a href="Search.html">Search</a></li>      
                    <li class="list-group-item"><a href="AddListAndItem.html">Add List & Item</a></li>    
                    <li class="list-group-item"><a href="Configuration.html">Configuration</a></li>
                    <li class="list-group-item"><a href="RequestCaching.html">Request Caching</a></li>                       
                    <li class="list-group-item"><a href="RequestBatching.html">Request Batching</a></li>       
                    <li class="list-group-item"><a href="RequestBatchingAndCaching.html">Request Batching with Caching</a></li>                          
                    <li class="list-group-item"><a href="Playground.aspx" class="directLink">API Playground</a></li>
                </ul>
            </div>

            <div id="sampleContainer" class="col-md-10" style="display: none;">
                <div id="sample-content"></div>

                <div>
                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="active"><a href="#sample" aria-controls="sample" role="tab" data-toggle="tab">Sample Code</a></li>
                        <li role="presentation"><a href="#embed" aria-controls="embed" role="tab" data-toggle="tab">Script Editor Embed Code</a></li>
                    </ul>

                    <!-- Tab panes -->
                    <div class="tab-content">
                        <div role="tabpanel" class="tab-pane active" id="sample">
                            <fieldset>
                                <legend>Sample Code
                                </legend>
                                <p>This the code actually executing on this page.</p>
                                <pre id="sample-code"></pre>
                                <button id="sample-run" class="btn btn-primary">Run Sample</button>
                            </fieldset>

                            <fieldset style="margin-top: 20px;">
                                <legend>Live Result
                                </legend>
                                <p>This is the live result of executing the above code on your host web.</p>
                                <div id="sample-show"></div>
                            </fieldset>
                        </div>
                        <div role="tabpanel" class="tab-pane" id="embed">
                            <fieldset>
                                <legend>Embed Code
                                </legend>
                                <p>This code can be pasted into a script editor web part in your <a href="#" class="hostWebAddress" target="_blank">host web</a>.</p>
                                <pre id="embed-code"></pre>
                            </fieldset>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
