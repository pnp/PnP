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

    <script type="text/javascript" src="../scripts/pnp.js"></script>
    <script type="text/javascript">

        $(function () {

            sample.ensureContextQueryString();

            $("#executeTest").on("click", function (e) {

                e.preventDefault();

                $("#sample-show").empty().append("<img src=\"/_layouts/images/gears_an.gif\" />");

                var codeText = "$pnp.sp.crossDomainWeb('" + sample.appWebUrl() + "', '" + sample.hostWebUrl() + "')." + $("#testCode").val() + ".then(sample.show).catch(sample.show);";

                try {
                    eval(codeText);
                }
                catch (e) {
                    sample.show(e.message);
                }
            });
        });

    </script>


</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    PnP Sample Viewer Add-In : API Playground
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="container-fluid" style="margin-top: 30px;">
        <div class="row">

            <div class="col-md-2">
                <ul id="sample-nav" class="list-group">
                    <!-- This is where additional samples will be added, the href should point to the sample content -->
                    <li class="list-group-item"><a href="default.aspx" class="directLink">Home</a></li>
                    <li class="list-group-item active">API Playground</li>
                </ul>
            </div>

            <div id="sampleContainer" class="col-md-10">
                <div id="sample-content">

                    <p>You can use this sample to explore the API by typing commands and seeing the results live from your host web. You do not need to include the "then", just the final "get()".</p>

                    <form>
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon">$pnp.sp.crossDomainWeb(appWebUrl, hostWebUrl).</span>
                                <input id="testCode" type="text" class="form-control" placeholder="get()">
                            </div>
                        </div>
                        <button id="executeTest" type="submit" class="btn btn-default">Submit</button>
                    </form>

                </div>

                <fieldset>
                    <legend>Live Result
                    </legend>
                    <p>This is the live result of executing the above code on your host web.</p>
                    <pre id="sample-show"></pre>
                </fieldset>

            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <hr />
                <p>
                    Examples:
                </p>
                <ul>
                    <li>
                        <pre>select("Title").get()</pre>
                    </li>
                    <li>
                        <pre>lists.select("Title", "Url").get()</pre>
                    </li>
                </ul>
            </div>
        </div>
    </div>

</asp:Content>
