<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.TaxonomyPickerWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TaxonomyPicker Samples</title>
    <link rel="Stylesheet" type="text/css" href="../Styles/taxonomypickercontrol.css" />
    <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/app.js?rev=2404" type="text/javascript"></script>
    <script src="../Scripts/taxonomypickercontrol.js?rev=2404" type="text/javascript"></script>
</head>
<body style="display: none;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>
        <div style="left: 50%; width: 600px; margin-left: -300px; position: absolute;">
            <table>
                <tr>
                    <td class="ms-formlabel" valign="top"><h3 class="ms-standardheader">Keywords Termset:</h3></td>
                    <td class="ms-formbody" valign="top">
                        <div class="ms-core-form-line" style="margin-bottom: 0px;">
                            <asp:HiddenField runat="server" id="taxPickerKeywords" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <b>Important:</b>
                        <br />
                        To make the Continent, Country and Region work using a hierarchical termset you first need to create the termset as described in Appendix A. Then obtain the termset ID and update app.js
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" valign="top"><h3 class="ms-standardheader">Continent:</h3></td>
                    <td class="ms-formbody" valign="top">
                        <div class="ms-core-form-line" style="margin-bottom: 0px;">
                            <asp:HiddenField runat="server" ID="taxPickerContinent" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" valign="top"><h3 class="ms-standardheader">Country:</h3></td>
                    <td class="ms-formbody" valign="top">
                        <div class="ms-core-form-line" style="margin-bottom: 0px;">
                            <asp:HiddenField runat="server" ID="taxPickerCountry" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" valign="top"><h3 class="ms-standardheader">Region:</h3></td>
                    <td class="ms-formbody" valign="top">
                        <div class="ms-core-form-line" style="margin-bottom: 0px;">
                            <asp:HiddenField runat="server" ID="taxPickerRegion" />
                        </div>
                    </td>
                </tr>
            </table>

            <asp:Button runat="server" OnClick="SubmitButton_Click" Text="Submit" />

            <asp:BulletedList runat="server" ID="SelectedValues" DataTextField="Label" />
        </div>
    </form>
</body>
</html>
