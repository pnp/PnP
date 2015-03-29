(function ($) {
    
    var hostUrl = getParameterByName("SPHostUrl");

    $(document).ready(function () {

        /*Get terms from the 'Keywords' termset for autocomplete suggestions.
        It might be a good idea to cache these values.
        */
        $.ajax({
            url: "/Home/Keywords?SPHostUrl=" + hostUrl,
            success: function (data) {

                var bannedCharacters = ['"', ';', '<', '>', '|'];
                var bannedCharactersLength = bannedCharacters.length;
                var bannedCharactersMessage = $("#bannedCharacters");

                $('#taxonomyPickerID').tagit({
                    fieldName: "taxonomyPickerName",
                    availableTags: data,
                    allowSpaces: true,
                    beforeTagAdded: function (event, ui) {
                        
                        if (!ui.duringInitialization) {

                            var _tagLabel = ui.tagLabel;

                            for (var i = 0; i < bannedCharactersLength; i++) {
                                if (_tagLabel.indexOf(bannedCharacters[i]) != -1) {
                                    bannedCharactersMessage.show();
                                    return false;
                                }
                                else {
                                    bannedCharactersMessage.hide();
                                }
                            }
                        }
                    }
                });

                /******Usage of the Tag-It plugin.************/
                /* Options
                fieldName: "skills",
                availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"],
                autocomplete: {delay: 0, minLength: 2},
                showAutocompleteOnFocus: false,
                removeConfirmation: false,
                caseSensitive: true,
                allowDuplicates: false,
                allowSpaces: false,
                readOnly: false,
                tagLimit: null,
                singleField: false,
                singleFieldDelimiter: ',',
                singleFieldNode: null,
                tabIndex: null,
                placeholderText: null,

                // Events
                beforeTagAdded: function(event, ui) {
                    console.log(ui.tag);
                },
                afterTagAdded: function(event, ui) {
                    console.log(ui.tag);
                },
                beforeTagRemoved: function(event, ui) {
                    console.log(ui.tag);
                },
                onTagExists: function(event, ui) {
                    console.log(ui.tag);
                },
                onTagClicked: function(event, ui) {
                    console.log(ui.tag);
                },
                onTagLimitExceeded: function(event, ui) {
                    console.log(ui.tag);
                }*/

            },
            error: function (jqxr, errorCode, errorThrown) {
                console.log(jqxr.responseText);
            }
        });

        //When 'Update' button is clicked
        $("#SubmitValues").on("click", function () {

            //Convert values from the input into an array.
            var taxPickerValue = $("#taxonomyPickerID").val();
            var valuesArray = taxPickerValue.split(",");

            //Convert the array to JSON.
            var skillsData = { skills: valuesArray };

            //Post the JSON to the Skills controller 
            $.ajax({
                url: "/Home/Skills?SPHostUrl=" + hostUrl,
                type: "POST",
                data: skillsData,
                success: function (data) {
                    showSuccess();
                },
                error: function (jqxr, errorCode, errorThrown) {
                    showError();
                    console.log(jqxr.responseText);
                    
                }
            });

        });
        
    });
    
    function showSuccess() {
        $("#updateError").hide();
        $("#updateSuccess").show();
    }

    function showError() {
        $("#updateSuccess").hide();
        $("#updateError").show();
    }

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

}(jQuery));