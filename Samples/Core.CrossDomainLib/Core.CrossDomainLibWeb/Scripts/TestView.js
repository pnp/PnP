//hook click event on form button
$("#TestPostButton").click(function (e) {
    e.preventDefault();

    var testName = $("#TestName").val();
    var testStreet = $("#TestStreet").val();

    if (cdlUtil.Initialized) {
        cdlUtil.ajax({
            method: "POST",
            url: sampleServerUrl + "/home/TestPost?SPHostUrl=" + sampleHostUrl,
            data: { name: testName, street: testStreet },
            dataType: "json",
            success: function (data) {
                alert("Got response from form post: " + data);
            },
            error: function(error){
                alert("domething went wrong while sending post: " + error);
            }
        });
    }
    else {
        alert("lib not ready yet. Try again later");
    }
});