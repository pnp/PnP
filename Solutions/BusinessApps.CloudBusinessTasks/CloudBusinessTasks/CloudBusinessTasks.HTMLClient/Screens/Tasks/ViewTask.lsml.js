/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewTask.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Task.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Task." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}


myapp.ViewTask.Delete_execute = function (screen) {
    // Delete the Task
    screen.Task.deleteEntity();
    // Save changes
    myapp.commitChanges().then(null, function fail(e) {
        // There was an error - show it in a box
        msls.showMessageBox(e.message, {
            title: "Error",
            buttons: msls.MessageBoxButtons.ok
        }).then(function (result) {
            if (result === msls.MessageBoxResult.ok) {
                // Discard Changes
                screen.details.dataWorkspace.ApplicationData
                    .details.discardChanges();
            }
        });
    });
};