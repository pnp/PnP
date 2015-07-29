//This javascript file is injected on the sharepoint page with a script editor webpart. 
//You can also use the below javascript in pagelayouts or masterpages (make sure to replace the sampleServerUrl and sampleHostUrl parameter)

var cdlUtil;

$(function () {
    //create instance of library
    cdlUtil = new CrossDomainUtil();

    //execute code when the library is initialized
    cdlUtil.OnInitialized(function () {
        //when initialiation is complete, do a get call, to get a html form
        cdlUtil.ajax({
            method: "GET",
            url: sampleServerUrl + "/home/TestView?SPHostUrl=" + sampleHostUrl,
            data: {},
            dataType: "html",
            success: function (data) {
                //when we get the html, add it to the page
                $("#testingcrossdiv").html(data);
            },
            error: function (error)
            {
                alert("something went wrong while getting the page: " + error);
            }
        });
    });

    //initialize library
    cdlUtil.Init(sampleServerUrl + "/home/proxy?SPHostUrl=" + sampleHostUrl);
});
