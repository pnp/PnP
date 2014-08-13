<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Contoso.Provisioning.OneDriveWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OneDrive Customizer</title>
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
     <link rel="Stylesheet" type="text/css" href="../Styling/app.css" />
</head>
<body style="display: none; overflow: auto;">
    <form id="form1" runat="server">
        <div id="divSPChrome"></div>
        <div id="ContentArea">
            <p>App should be added to public OneDrive host or other web site associated to user profiles. After this a App part should be added to host web, which actually modifyes the site when it exists.</p>
            <p>App works from any site located in the same tenant as the OneDrive for Business sites. </p>
            <p>This example has been tested with Microsoft Azure and with Office365. </p>
            <p>
                This demo is used only for modifying already created personal sites when they are available and actual creation 
                process is still using out of the box timer based approach for site creation and branding is changed after the site is created. 
                You could however also override this model, if needed.
            </p>
        </div>
    </form>
</body>
</html>
