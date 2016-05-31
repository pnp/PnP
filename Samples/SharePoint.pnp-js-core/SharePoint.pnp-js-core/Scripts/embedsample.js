var sample = {

    getUrlParamByName: function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
        var results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    },

    init: function (f) {
        $(function () {
            f();
        });
    },

    show: function (data) {
        $("#pnp-sample-result").empty().append(JSON.stringify(data));
    },

    append: function (data) {
        $("#pnp-sample-result").append("<hr />").append(JSON.stringify(data));
    }
};