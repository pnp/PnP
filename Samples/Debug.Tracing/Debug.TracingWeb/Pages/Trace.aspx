<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Trace.aspx.cs" Inherits="Debug.TracingWeb.Pages.Trace" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Debug.Tracing trace management</title>
    <script src="../Scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/app.js" type="text/javascript"></script>
</head>
<body>
    <form id="tracing" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>

    <div>
        <table>
            <tr>
                <td>
                    <asp:Button ID="EnableTracing" runat="server" Text="Enable Tracing" OnClick="EnableTracing_Click" />
                </td>
                <td>
                    This enables tracing. Be carefull in production. Enabeling tracing has some impact on performance. Clicking this button restarts your web application.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="DisbleTracing" runat="server" Text="Disable Tracing" OnClick="DisbleTracing_Click" />
                </td>
                <td>
                    This disables tracing. Clicking this button restarts your web application.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="noRequests" runat="server" Text="100"></asp:TextBox><asp:Button ID="SetMaxRequest" runat="server" Text="Set number of requests" OnClick="SetMaxRequest_Click" />
                </td>
                <td>
                    By default, only the first 10 requests are shown on the trace.axd page. With this you can increase it. (you can alse clear messages on the trace.axd page)
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    All the above cofiguration can also be configured in the web.config file: <br />
                    trace enabled="true" localOnly="false" requestLimit="100"
                </td>
            </tr>
        </table>

        <br />
        <asp:Label style="color:red;padding-left:10px;" ID="OutputLabel" runat="server"></asp:Label>
    </div>
    </form>
</body>
</html>
