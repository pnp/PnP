function fireCustomEvent(eventName) {

    // Retrieve a reference to the target ListItem
    var urlParams = GetUrlParams();
    var targetItemID = urlParams["SPListItemId"];
    var targetListID = urlParams["SPListId"];

    // Retrieve a reference to the current user, for later usage
    var currentUser = context.get_web().get_currentUser();
    context.load(currentUser);

    context.executeQueryAsync(
        function (sender, args) { // Success

            // Retrieve a reference to the Workflow Services Manager library
            var wfManager = SP.WorkflowServices.WorkflowServicesManager.newObject(context, context.get_web());

            // Retrieve a reference to the Workflow Services Manager Instance Service
            var wfInstanceService = wfManager.getWorkflowInstanceService();
            context.load(wfInstanceService);

            // Retrieve a reference to the Workflow Services Manager Subscription Service
            var wfSubscriptionService = wfManager.getWorkflowSubscriptionService();
            context.load(wfSubscriptionService);

            // Retrieve a reference to the workflow instances running for the target ListItem
            var instances = wfInstanceService.enumerateInstancesForListItem(targetListID, targetItemID);
            context.load(instances);

            context.executeQueryAsync(
                function (sender, args) { // Success

                    // Browse the workflow instance for the current one
                    var instancesEnumerator = instances.getEnumerator();
                    while (instancesEnumerator.moveNext()) {
                        var instance = instancesEnumerator.get_current();

                        // If the current instance has a WorkflowStatus value of "Started"
                        // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.workflowservices.workflowstatus(v=office.15).aspx
                        // for further details
                        if (instance.get_status() == 1) {

                            var targetInstance = instance;

                            // Retrieve the Workflow Subscription corresponding to the current running instance
                            var subscription = wfSubscriptionService.getSubscription(instance.get_workflowSubscriptionId());
                            context.load(subscription);

                            context.executeQueryAsync(
                                function (sender, args) { // Success

                                    // If the Workflow Subscription is the one that we are looking for ...
                                    if (subscription.get_name() == "WorkflowWithCustomEvents") {

                                        // Publish the Custom Event
                                        wfInstanceService.publishCustomEvent(targetInstance, eventName, "Event from: " + currentUser.get_title() + " - Argument: " + $("#customEventArgument").val());

                                        context.executeQueryAsync(
                                            function (sender, args) { // Success
                                                alert('Event Fired!');
                                            },
                                            function (sender, args) { // Error
                                                handleException(
                                                    errorMessage + " Error while fireing the event.");
                                            }
                                        );
                                    }
                                },
                                function (sender, args) { // Error
                                    handleException(
                                        errorMessage + " Error while retrieving the Workflow Subscription.");
                                }
                            );
                        }
                    }
                },
                function (sender, args) { // Error
                    handleException(
                        errorMessage + " Error while loading the Workflow Instance service.");
                }
            );
        },
        function (sender, args) { // Error
            handleException(
                errorMessage + " Error while loading the list of target items.");
        }
    );
}