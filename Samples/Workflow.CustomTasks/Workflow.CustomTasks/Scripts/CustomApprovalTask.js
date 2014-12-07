var ctx;
var urlParams = null;

$(document).ready(function () {
    $('#chooseOutcomePanel').hide();
    $('#showOutcomePanel').hide();

    ExecuteOrDelayUntilScriptLoaded(checkTaskCompleted, "sp.js");
});

// This function retrieves arguments from the QueryString and comes from Microsoft samples
function getUrlParams() {
    if (urlParams == null) {
        urlParams = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi,
            function (m, key, value) { urlParams[key] = value; });
    }
    return urlParams;
}

// This function determines whether the current task has already been completed or not
function checkTaskCompleted() {
    ctx = SP.ClientContext.get_current();
    var web = ctx.get_web();
    var tasksList = web.get_lists().getById(decodeURIComponent(getUrlParams()["List"]));
    var task = tasksList.getItemById(getUrlParams()["ID"]);

    ctx.load(task);

    ctx.executeQueryAsync(
        function (sender, args) {
            var statusValue = task.get_item("Status");
            var outcomeValue = task.get_item("CustomApprovalOutcome");
            if (statusValue != "Completed") {
                $("#chooseOutcomePanel").show();
            }
            else {
                $("#outcome").append("Task outcome: " + outcomeValue);
                $("#showOutcomePanel").show();
            }
        },
        function (sender, args) {
            alert("Error while loading the task status: " + args.get_message());
        });
}

// This function defines the task outcome and completes the current task
function setTaskOutcome(outcome) {

    ctx = SP.ClientContext.get_current();
    var web = ctx.get_web();
    var tasksList = web.get_lists().getById(decodeURIComponent(getUrlParams()["List"]));
    var task = tasksList.getItemById(getUrlParams()["ID"]);

    task.set_item("CustomApprovalOutcome", outcome);
    task.set_item("Status", "Completed");
    task.update();

    ctx.executeQueryAsync(
        function (sender, args) {
            redirFromInitForm();
        },
        function (sender, args) {
            alert("Error while saving the task outcome: " + args.get_message());
        });
}

// Redirects the browser to the original Source
function redirFromInitForm() {
    window.location = decodeURIComponent(getUrlParams()["Source"]);
}

