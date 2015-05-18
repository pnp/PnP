<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="../_layouts/15/sp.js"></script>
    <script type="text/javascript" src="../_layouts/15/sp.workflowservices.js"></script>

    <!-- Scripts added to support client-side PeoplePicker -->
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <SharePoint:ScriptLink ID="ScriptLink1" Name="clienttemplates.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink2" Name="clientforms.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink3" Name="clientpeoplepicker.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink4" Name="autofill.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <SharePoint:ScriptLink ID="ScriptLink6" Name="sp.core.js" runat="server" LoadAfterUI="true" Localizable="false" />
    <script type="text/javascript" src="../Scripts/ClientSidePeoplePicker.js"></script>

</asp:Content>

<%--    
        IMPORTANT NOTE: 
        Be sure to update the InitiationUrl property value to the URL of the custom initiation form.
        InitiationUrl property can be updated from the workflow's property grid.
--%>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Start Sample Approval Workflow 
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <table>
        <tr>
            <td>Target Approver(s):<br />
                <div id="peoplePicker"></div>
                <br />
            </td>
        </tr>
        <tr>
            <td>Due Days:<br />
                <input type="text" id="dueDays" />
                <br />
            </td>
        </tr>
        <tr>
            <td><br />&nbsp;</td>
        </tr>
        <tr>
            <td>
                <input type="button" name="startWorkflowButton" value="Start" onclick="StartWorkflow()" />
                <input type="button" name="cancelButton" value="Cancel" onclick="RedirFromInitForm()" />
                <br />
            </td>
        </tr>
    </table>

    <!--
    This is the "standard" Workflow Initiation Form created by VS2013
    except that we use the Client-side PeoplePicker control and we also
    start the workflow instance providing some custom startup arguments
    -->

    <script type="text/javascript">
        // ---------- Start workflow ----------
        function StartWorkflow() {
            var errorMessage = "An error occured when starting the workflow.";
            var subscriptionId = "", itemId = "", redirectUrl = "";

            var urlParams = GetUrlParams();
            if (urlParams) {
                //itemGuid = urlParams["ItemGuid"];
                itemId = urlParams["ID"];
                redirectUrl = urlParams["Source"];
                subscriptionId = urlParams["TemplateID"];
            }

            if (subscriptionId == null || subscriptionId == "") {
                // Cannot load the workflow subscription without a subscriptionId, so workflow cannot be started.
                alert(errorMessage + "  Could not find the workflow subscription id.");
                RedirFromInitForm(redirectUrl);
            }
            else {
                // Set workflow in-arguments/initiation parameters
                var wfParams = new Object();

                var targetApproverValue = getUserKeys("peoplePicker");
                if (targetApproverValue) {
                    wfParams['targetApprover'] = targetApproverValue;
                }

                var dueDaysValue = document.getElementById("dueDays").value;
                if (dueDaysValue) {
                    var intDueDaysValue = parseInt(dueDaysValue);
                    if (intDueDaysValue)
                        wfParams['dueDays'] = intDueDaysValue;
                }

                // Get workflow subscription and then start the workflow
                var context = SP.ClientContext.get_current();
                var wfManager = SP.WorkflowServices.WorkflowServicesManager.newObject(context, context.get_web());
                var wfDeployService = wfManager.getWorkflowDeploymentService();
                var subscriptionService = wfManager.getWorkflowSubscriptionService();

                context.load(subscriptionService);
                context.executeQueryAsync(

                    function (sender, args) { // Success
                        var subscription = null;
                        // Load the workflow subscription
                        if (subscriptionId)
                            subscription = subscriptionService.getSubscription(subscriptionId);
                        if (subscription) {
                            if (itemId != null && itemId != "") {
                                // Start list workflow
                                wfManager.getWorkflowInstanceService().startWorkflowOnListItem(subscription, itemId, wfParams);
                            }
                            else {
                                // Start site workflow
                                wfManager.getWorkflowInstanceService().startWorkflow(subscription, wfParams);
                            }
                            context.executeQueryAsync(
                                function (sender, args) {
                                    // Success
                                    RedirFromInitForm(redirectUrl);
                                },
                                function (sender, args) {
                                    // Error
                                    alert(errorMessage + "  " + args.get_message());
                                    RedirFromInitForm(redirectUrl);
                                }
                            )
                        }
                        else {
                            // Failed to load the workflow subscription, so workflow cannot be started.
                            alert(errorMessage + "  Could not load the workflow subscription.");
                            RedirFromInitForm(redirectUrl);
                        }
                    },
                    function (sender, args) { // Error
                        alert(errorMessage + "  " + args.get_message());
                        RedirFromInitForm(redirectUrl);
                    }
                )
            }
        }

        // ---------- Redirect from page ----------
        function RedirFromInitForm(redirectUrl) {
            window.location = redirectUrl;
        }

        // ---------- Returns an associative array (object) of URL params ----------
        function GetUrlParams() {
            var urlParams = null;
            if (urlParams == null) {
                urlParams = {};
                var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
                    urlParams[key] = decodeURIComponent(value);
                });
            }
            return urlParams;
        }
    </script>

</asp:Content>
