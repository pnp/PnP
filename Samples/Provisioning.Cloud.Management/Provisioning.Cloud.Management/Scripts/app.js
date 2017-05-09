function loadLanguages(languageElement, templateElement) {
    $.ajax({
        type: "GET",
        url: "/api/language",
        dataType: "json",
        success: function (data) {
            $.each(data, function () {
                languageElement.append($("<option />").val(this.languageId).text(this.displayName));
            });

            loadTemplates(templateElement, languageElement.val())
        },
        failure: function () {
            alert("Failed!");
        }
    });

    languageElement.change(
        function () {
            loadTemplates(templateElement, $(this).val());
        });
};

function loadTemplates(templateElement, lcid) {
    templateElement.empty();

    $.ajax({
        type: "GET",
        url: "/api/webtemplate?lcid=" + lcid,
        dataType: "json",
        success: function (data) {
            $.each(data, function () {
                templateElement.append($("<option />").val(this.name).text(this.title));
            });
        },
        failure: function () {
            alert("Failed!");
        }
    });
};