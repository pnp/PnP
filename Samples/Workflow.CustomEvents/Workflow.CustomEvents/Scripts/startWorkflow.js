function startWorkflow() {

    // Retrieve a reference to the target ListItem
    var urlParams = GetUrlParams();
    var targetItemID = urlParams["SPListItemId"];
    var targetListID = urlParams["SPListId"];

    // Set workflow in-arguments/initiation parameters
    var wfParams = new Object();

    // Retrieve a reference to the Workflow Services Manager library
    var wfManager = SP.WorkflowServices.WorkflowServicesManager.newObject(context, context.get_web());

    // Retrieve a reference to the Workflow Services Manager Instance Service
    var wfInstanceService = wfManager.getWorkflowInstanceService();
    context.load(wfInstanceService);

    // Retrieve a reference to the Workflow Services Manager Subscription Service
    var wfSubscriptionService = wfManager.getWorkflowSubscriptionService();
    context.load(wfSubscriptionService);

    context.executeQueryAsync(
        function (sender, args) { // Success

            // Enumerate all the subscriptions for the target list
            var subscriptions = wfSubscriptionService.enumerateSubscriptionsByList(targetListID);
            context.load(subscriptions);


            context.executeQueryAsync(
                function (sender, args) { // Success

                    subscriptions.getEnumerator();
                    // Browse the workflow subscription for the current target list
                    var subscriptionsEnumerator = subscriptions.getEnumerator();
                    while (subscriptionsEnumerator.moveNext()) {

                        var subscription = subscriptionsEnumerator.get_current();

                        // If the Workflow Subscription is the one that we are looking for ...
                        if (subscription.get_name() == "WorkflowWithCustomEvents") {

                            // Start a new workflow instance against the current item
                            wfInstanceService.startWorkflowOnListItem(subscription, targetItemID, wfParams);

                            context.executeQueryAsync(
                                function (sender, args) { // Success
                                    redirFromCurrentPage("Default.aspx");
                                },
                                function (sender, args) {
                                    // Error
                                    alert(errorMessage + "  " + args.get_message());
                                    redirFromCurrentPage(redirectUrl);
                                }
                            );
                        }
                    }
                },
                function (sender, args) { // Error
                    handleException(
                        errorMessage + " Error while retrieving the subscription for the current workflow definition.");
                }
            );
        },
        function (sender, args) { // Error
            handleException(
                errorMessage + " Error while starting the workflow for the target item.");
        }
    );
}