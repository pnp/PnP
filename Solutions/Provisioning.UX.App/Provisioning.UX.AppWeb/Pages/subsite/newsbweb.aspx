<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newsbweb.aspx.cs" Inherits="Provisioning.UX.AppWeb.Pages.SubSite.newsbweb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../../Styles/site.css" rel="stylesheet" type="text/css" />
     <script src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" type="text/javascript"></script>
     <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js" type="text/javascript" ></script>      
     <script src="../../Scripts/chromeloader.js?rev=1" type="text/javascript"></script>
</head>
<body>
   <div id="divSPChrome"></div>    
        <div class="page">
        <section id ="main">
            <script type="text/javascript">
                $(function () {
                    $('#edit_button').click(function () {
                        $.ajax({
                            success: function () {
                            }
                        });
                    });
                    $('#cancel_button').click(function () {
                        window.location = $('#Url').val();
                    });
                });
            </script>
            <div id="loading_dialog" title="Saving..." style="display:none;">
                <p>
                    <img src="../../images/spinningwheel.gif" width='20' height='20' alt="" style="position:relative; top:5px; right:5px; " />
                    <span style="color: #696969;">Please wait while your changes are processed. </span>
                </p>
             </div>
            <form id="form" runat="server">
                <fieldset>
                    <table id="newWebTable">
                        <tbody>
                            <tr>
                              <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>
                                        <p>Title and Description</p>
                                    </div>
                                </div>
                              </td>
                              <td valign="top" class="right-column">
                                <div class="editor-field O15_editor_field_head">
                                    <p>Title:</p>
                                    <asp:TextBox ID="txtTitle" runat="server" Visible="true"></asp:TextBox>
                                    <p>
                                        <span>Description:</span>
                                    </p>
                                    <asp:TextBox ID="txtDescription" runat="server" Visible="true" TextMode="MultiLine" Rows="3" Columns="40"></asp:TextBox>
                                </div>
                               </td>
                            </tr>
                            <tr>
                              <td valign="top">
                                <div class="editor-label">
                                    <div class='O15_editor_label_head'>   
                                        <p>Web Site Address</p>
                                    </div>
                                </div>
                              </td>
                              <td valign="top" class="ms-authoringcontrols ms-inputformcontrols">
                                    <p>URL name:</p>
                                    <asp:Label ID="lblHostSite" runat="server" Visible="true"></asp:Label>
                                    <asp:TextBox ID="txtCreateSubWebName" runat="server" Visible="true" MaxLength="260"></asp:TextBox>
                               </td>
                            </tr>
                       </tbody>
                    </table>
                    <input id="Url" name="Url" type="hidden" value="" runat="server"/>
                    <p style="float: right">
                        <asp:Button runat="server" ID="submit_button" Text="Create" OnClick="Submit_Click" />
                        <input type="button" id="cancel_button" value="Cancel" />
                    </p>
                    <div class="clear"></div>
                </fieldset>
            </form>
        </section>
    </div>  
    <div id="MicrosoftOnlineRequired">
        <div style="float:left">
            <img style="position:relative;top:4px;"  src="../../images/MicrosoftLogo.png" alt="©2015 Microsoft Corporation"/>
            <span id="copyright">©2015 Contoso Corporation</span>&nbsp;&nbsp;&nbsp;
            <a id="legalUrl" href="https://officeams.codeplex.com/license" target="_blank">Legal</a> |
            <a id="privacyUrl" href="https://www.codeplex.com/site/legal/privacy" target="_blank">Privacy</a>
        </div>
        <div style="float:right">
            <a id="supportUrl" href="https://officeams.codeplex.com/" target="_blank">Community</a> |
            <a id="feedbackUrl" href="https://officeams.codeplex.com/discussions" target="_blank">Feedback</a>
        </div>
        <div class="clear"></div>
    </div>
</body>
</html>
