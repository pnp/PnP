<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Debug.TracingWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Debug.Tracing</title>
    <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/app.js" type="text/javascript"></script>
</head>
<body style="overflow:auto !important; display: none;">
    

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>

    <div style="padding-left:10px;padding-bottom:40px;">
        Normally it is not possible to debug in production. If an error occurs you are often faced with a cryptic error message. <br />
        It would help a lot if you had the detailed error message, and the method where the error occured. Even better would be to have the parameters used to call the method.<br />
        Tracing can be used to help you with this. Tracing can be turned on and off easy. It provides you with detailed information to fix a problem.<br />
        Even when facing performance issues you can use tracing to see what method is working slow.<br />
    </div>
    <div>
        <asp:Button ID="TraceMessage" runat="server" Text="Trace message" OnClick="TraceMessage_Click" ToolTip="Traces a message to the trace output. This is a good way to troubleshoot in production"/>
        <asp:Button ID="TraceMethods" runat="server" Text="Trace methods" OnClick="TraceMethods_Click" />
        <asp:Button ID="LogError" runat="server" Text="Log error to Sharepoint and tracing" OnClick="LogError_Click" />
        <asp:Button ID="ManageTracing" runat="server" Text="Manage trace settings" OnClick="ManageTracing_Click" />
        <br />
        <br />
        <span style="padding-left:10px">Click here to view tracing -></span><a href="~/Pages/Trace.axd" target="_blank">View Tracing</a>
    </div>



    <%--This is info--%>
    <asp:Panel runat="server" ID="TraceMessagePanel" Visible="false">
        <div style="padding-left:10px;color:red">Trace messages written to trace. Below you can find the information about how to retrieve them.</div>
        <div style="padding-left:10px">
            <table >
                <tr>
                    <td>
                        <img src="../Images/tracemessage1.png" width="500" />
                    </td>
                    <td>If you click the 'View tracing' link (or go to 'pages/trace.axd') you can see all the requests to the webserver, with their HTTP status. If you click the request we can see the details</td>
                </tr>
                <tr>
                    <td>
                        <img src="../Images/tracemessage2.png" width="500" />
                    </td>
                    <td>In the details of the request you can see the logging of all the default.aspx method. In this page you can also see the 3 messages we logged. If something would go wrong you can see in what phase it happened. The 'from first' column shows the timing of the call in the total request. The 'from last' column shows the time spent from the last trace entry until the current one. With this information you can even troubleshoot performance issues. </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
        <asp:Panel runat="server" ID="TraceMethodPanel" Visible="false">
        <div style="padding-left:10px;color:red">Trace METHODS written to trace. Below you can find the information about how to retrieve them.</div>
        <div style="padding-left:10px">
            <table >
                <tr>
                    <td>
                        <img src="../Images/tracemessage1.png" width="500" />
                    </td>
                    <td>If you click the 'View tracing' link (or go to 'pages/trace.axd') you can see all the requests to the webserver, with their HTTP status. If you click the request we can see the details</td>
                </tr>
                <tr>
                    <td>
                        <img src="../Images/tracemethod.png" width="500" />
                    </td>
                    <td>The method logger logs the beginning and ending of the method. In the first column it automaticly logs the class, containing the code. The second column shows the method name and parameters.</td>
                </tr>
            </table>
        </div>
    </asp:Panel>
        <asp:Panel runat="server" ID="TraceErrorPanel" Visible="false">
        <div style="padding-left:10px;color:red">ERROR written to trace and Sharepoint. Below you can find the information about how to retrieve them.</div>
        <div style="padding-left:10px">
            <table >
                <tr>
                    <td>
                        <img src="../Images/tracemessage1.png" width="500" />
                    </td>
                    <td>If you click the 'View tracing' link (or go to 'pages/trace.axd') you can see all the requests to the webserver, with their HTTP status. If you click the request we can see the details</td>
                </tr>
                <tr>
                    <td>
                        <img src="../Images/errortrace.png" width="500" />
                    </td>
                    <td>The errorlogger logs the exception to the tracing, including stacktrace and inner exceptions</td>
                </tr>
                <tr>
                    <td>
                        <img src="../Images/errorsp1.png" width="500" />
                    </td>
                    <td>The exception is also logged to Sharepoint. Go to site contents of your site. Click the '...' and choose details.</td>
                </tr>
                <tr>
                    <td>
                        <img src="../Images/errorsp2.png" width="500" />
                    </td>
                    <td>This screen shows that the app has 1 runtime error. Click the link to see the error</td>
                </tr>
                <tr>
                    <td>
                        <img src="../Images/errorsp3.png" width="500" />
                    </td>
                    <td>The error and stacktrace is logged. It can take a couple of minutes for the error to show up in this screen.</td>
                </tr>
            </table>
        </div>
    </asp:Panel>




    </form>
</body>
</html>
