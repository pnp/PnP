<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="error.aspx.cs" Inherits="ECM.DocumentLibrariesWeb.Pages.Opps" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error</title>
</head>
<body>
    <form id="form1" runat="server">
           <div id="error-head">
               <div id="error-title-panel">Sorry, something went wrong</div>
                <div id="error-message">
                    <br />
                    <br />
                    <asp:Label ID="errorMessage" runat="server"></asp:Label>
                </div>
           </div>
    </form>
</body>
</html>
