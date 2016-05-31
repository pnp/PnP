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

    <script type="text/javascript">
        $(function () {
            sample.ensureSPHostUrlInLinks($("a"));
        });
    </script>

</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    PnP Sample Viewer Add-In
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="container-fluid" style="margin-top: 30px;">
        <div class="row">
            <div class="col-md-4 col-sm-6">
                <div class="jumbotron">
                    <h1>Welcome!</h1>
                    <p>This SharePoint hosted add-in contains a set of samples to illustrate usage for the <a href="https://github.com/OfficeDev/PnP-JS-Core/" target="_blank">Patterns and Practices JavaScript core library</a>.</p>
                </div>
            </div>
            <div class="col-md-8 col-sm-6">
                <div class="container">
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="thumbnail">
                                <img src="../images/default_samples.png" alt="view samples" />
                                <div class="caption">
                                    <h3>View Samples</h3>
                                    <p>Browse the sample gallery and see examples of using the library live.</p>
                                    <p><a href="sampleviewer.aspx#helloworld.html" class="btn btn-primary" role="button">Samples</a></p>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="thumbnail">
                                <img src="../images/default_playground.png" alt="interact with the API playground" />
                                <div class="caption">
                                    <h3>API Playground</h3>
                                    <p>Interact directly with the API from your browser with real-time results.</p>
                                    <p><a href="playground.aspx" class="btn btn-primary" role="button">Playground</a></p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="thumbnail">
                                <img src="../images/default_behind.png" alt="take a peek at how the samples work" />
                                <div class="caption">
                                    <h3>Behind the scenes</h3>
                                    <p>Gain some insight into the plumbing to make the samples work.</p>
                                    <p><a href="bts.aspx" class="btn btn-primary" role="button">Behind the Scenes</a></p>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="thumbnail">
                                <img src="../images/default_help.png" alt="get help to frequently asked questions" />
                                <div class="caption">
                                    <h3>Help</h3>
                                    <p>Get answers to some common questions with the samples.</p>
                                    <p><a href="help.aspx" class="btn btn-primary" role="button">Help</a></p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
