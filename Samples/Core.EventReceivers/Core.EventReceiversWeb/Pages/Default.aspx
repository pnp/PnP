<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Core.EventReceiversWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script 
        src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" 
        type="text/javascript">
    </script>
    <script 
        type="text/javascript" 
        src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.2.min.js">
    </script>      
    <script 
        type="text/javascript"
        src="../scripts/ChromeLoader.js">
    </script>

</head>
<body style="display: none">
    <form id="form1" runat="server">
    <!-- Chrome control placeholder -->
    <div id="chrome_ctrl_placeholder"></div>

    <!-- The chrome control also makes the SharePoint
          Website stylesheet available to your page -->
    <h1 class="ms-accentText">Attaching Events to a List in the Host Web</h1>    
    <div id="MainContent">
        <p>This sample demonstrates how to attach remote 
        event receivers to a list in the host web.  </p>
        <p>
        Attaching a remote event receiver to 
        a list only requires Manage permissions for the list.
        The app requests
        Manage permissions for the Web it is being installed to because
        the app will create a list "Remote Event Receiver Jobs" if one
        does not already exist.</p>
    </div>
    </form>
</body>

</html>
