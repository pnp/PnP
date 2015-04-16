<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.ModifyPagesWeb.Default" ValidateRequest="false" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Modify Pages</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
            <div id="divSPChrome"></div>
        </div>
        <div style="left: 40px; position: absolute;">
            <h1>Wiki page manipulation</h1>
            <br />
            HTML text to add:
        <br />
            <asp:TextBox runat="server" ID="htmlEntry" Rows="6" TextMode="MultiLine" Width="400px" Text="Hello <strong>OfficeDev PnP</strong>!"></asp:TextBox>
            <br />
            <br />
            <asp:Button runat="server" ID="btnCreateNewPage" Text="Add page with html" OnClick="btnCreateNewPage_Click" />
            Click
            <asp:HyperLink ID="hplPage" runat="server" Text="here" Target="_blank"></asp:HyperLink>
            to go to the created page.
             <br />
            <br />
            <br />
            <br />
            <h1>Advance wiki page manipulation</h1>
            <br />
            Add a new page, with specific page layout and oob web part.
            <br />
            <br />
            <asp:Button runat="server" ID="btnCreatePageWithWebPart" Text="Create Page with WebPart" OnClick="btnCreatePageWithWebPart_Click" />
            Click
            <asp:HyperLink ID="hplPage2" runat="server" Text="here" Target="_blank"></asp:HyperLink>
            to go to the created page.
        </div>
    </form>
</body>
</html>
