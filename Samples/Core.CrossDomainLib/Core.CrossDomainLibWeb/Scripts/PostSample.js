//This javascript file is injected on the sharepoint page with a script editor webpart. 
//You can also use the below javascript in pagelayouts or masterpages (make sure to replace the sampleServerUrl and sampleHostUrl parameter)

$(function () {

    //create instance of library
    var cdlUtil = new CrossDomainUtil();

    //initialize library
    cdlUtil.Init(sampleServerUrl + "/home/proxy?SPHostUrl=" + sampleHostUrl);

    var dataobject = { id: 5, name: "some cool name", street: "samplelane" };

    //code on get button click
    $("#DoPostButton").click(function (e) {
        e.preventDefault();

        if (cdlUtil.Initialized) {
            cdlUtil.ajax({
                method: "POST", //GET or POST
                url: sampleServerUrl + "/home/TestPost?SPHostUrl=" + sampleHostUrl, //action on controller that is called. Always pass SPHostUrl!!!
                data: dataobject, //pass data to controller action
                dataType: "json", //datatype that you expect back - eg. json or html
                success: function (data) { //function that executes when the call succeeds
                    alert("Got response from post request: " + data);
                },
                error: function (error) { //function that executes when the call fails
                    alert(error);
                }
            });
        }
        else {
            alert("lib not ready yet. Try again later");
        }
    });
});