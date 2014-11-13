
if (typeof _spPageContextInfo != "undefined" && _spPageContextInfo != null) {
    RegisterInMDS();
}
else {
    RegisterDependentFieldsContext();
}

function RegisterInMDS() {
    // RegisterDependentFieldsContext-override for MDS enabled site
    RegisterModuleInit(_spPageContextInfo.siteServerRelativeUrl + "/Style%20Library/JSLink-Samples/DependentFields.js", RegisterDependentFieldsContext);
    //RegisterDependentFieldsContext-override for MDS disabled site (because we need to call the entry point function in this case whereas it is not needed for anonymous functions)
    RegisterDependentFieldsContext();
}

function RegisterDependentFieldsContext() {

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides({

        Templates: {
            OnPostRender: function (ctx) {
                var colorField = window[ctx.FormUniqueId + "FormCtx"].ListSchema["Color"];
                var colorFieldControlId = colorField.Name + "_" + colorField.Id + "_$RadioButton" + colorField.FieldType + "Field";

                var f = ctx.ListSchema.Field[0];
                if (f.Name == "Car") {
                    var fieldControl = $get(f.Name + "_" + f.Id + "_$" + f.FieldType + "Field");

                    $addHandler(fieldControl, "change", function (e) {
                        // first, let's hide all the colors - while the information is loading
                        for (var i = 0; i < 5; i++)
                            $get(colorFieldControlId + i).parentNode.style.display = "none";

                        var newValue = fieldControl.value;
                        var newText = fieldControl[fieldControl.selectedIndex].text;

                        var context = SP.ClientContext.get_current();
                        // here add logic for fetching information from an external list
                        // based on newText and newValue
                        context.executeQueryAsync(function () {
                            // fill this array according to the results of the async request
                            var showColors = [];
                            if (newText == "Kia Soul") showColors = [0, 2, 3];
                            if (newText == "Fiat 500L") showColors = [1, 4];
                            if (newText == "BMW X5") showColors = [0, 1, 2, 3, 4];

                            // now, display the relevant ones
                            for (var i = 0; i < showColors.length; i++)
                                $get(colorFieldControlId + showColors[i]).parentNode.style.display = "";
                        },
                        function (sender, args) {
                            alert("Error! " + args.get_message());
                        });

                    });
                } else if (f.Name == "Color") {
                    // initialization: hiding all the choices. first user must select a car
                    for (var i = 0; i < 5; i++)
                        $get(colorFieldControlId + i).parentNode.style.display = "none";

                }
            }
        }

    });
}