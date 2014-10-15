<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Branding.ClientSideRenderingWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Side Rendering – JSLink</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/MicrosoftAjax.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>
        <div style="padding-left: 20px; padding-right: 20px;">
            <h2>Introduction</h2>
            <br />
            <p>The Client Side Rendering JS files in this sample were taken from an <a href="http://code.msdn.microsoft.com/office/Client-side-rendering-JS-2ed3538a">MSDN code sample by Muawiyah Shannak</a> and from a <a href="http://www.codeproject.com/Articles/610259/SharePoint-Client-Side-Rendering-List-Forms">CodeProject article by Andrei Markeev</a>.  They have been included in this code sample to demonstrate how the remote provisioning pattern may be used to deploy Client Side Rendering components and associate them with views and forms in a SharePoint list.</p>
            <br />
            <h2>Instructions</h2>
            <br />
            <h3>Provision the samples</h3>
            <p>
                Click the Provision Samples button to create the list columns, lists, list views, initialize the lists with data, and upload the Client Side Rendering JavaScript and image files that support the samples.  Then, register the Client Side Rendering JavaScript files with the list forms and views via the JSLink property.<br />
            </p>
            <asp:Button ID="btnCreateSamples" runat="server" Text="Provision Samples" OnClick="btnCreateSamples_Click" />
            <br />
            <br />
            <asp:Label ID="lblInfo" runat="server"></asp:Label>
            <br />
            <br />
            <h3>View the samples</h3>
            <p>
                After the samples are successfully provisioned, click the links below to view them.  Reference the scenario documentation for more details about these samples and how to interact with them.<br />
            </p>
            <div>
                <asp:HyperLink ID="link1" runat="server"> Sample 1 (Priority color)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link2" runat="server"> Sample 2 (Substring long text)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link3" runat="server"> Sample 3 (Confidential Documents)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link4" runat="server"> Sample 4 (Percent Complete)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link5" runat="server"> Sample 5 (Accordion)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link6" runat="server"> Sample 6 (Email Regex Validator)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link7" runat="server"> Sample 7 (Read-Only SP Controls)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link8" runat="server"> Sample 8 (Hidden Field)</asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="link9" runat="server"> Sample 9 (Dependent Fields)</asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>
