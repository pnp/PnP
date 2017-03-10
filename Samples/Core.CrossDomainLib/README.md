# Cross domain lib - Javascript Calls from SharePoint page to MVC website #

### Summary ###
The cross domain library enables you to perform javascript calls from a SharePoint page to a provider hosted MVC app. It is also possible to get html (enabeling you to create app part behaviour, without Iframes) from the provider hosted app. The C# code in the provider hosted app is automatically authenticated in the same way a app part is (clientcontext is created in the same way, and also app-only calls are possible).

### Solution ###
Solution | Author(s)
---------|----------
Sample.CrossDomainLib | Stijn Neirinckx 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 22th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Why use the cross domain lib #
Javascript is a nice tool to customize SharePoint, but it has limitations that C# code does not have.

-  Execute code using app permission, or impersonating another user
-  Calling external resources, like webservices or databases
-  Use technologies that are not possible in javascript, like serverside cashing

With the cross site domain library you can combine javascript and C# code, giving you more development possibilities.<br />
        It is even possible to show views with javascript on your SharePoint page, rendered by the provider hosted app. (app part behaviour, without Iframes)

Example:
![Add-in part UI in SharePoint page](http://i.imgur.com/D63WcPd.png?1)

# How to use the cross domain lib #

## Perform a GET request ##

Using the cross domain library is easy. Create the object and Initialize it using the Init method. Pass the proxypage url as a parameter (don't forget to pass the SPHostUrl, since this is needed for SharePoint app authentication). For more info about the proxy page, see "How to setup the cross domain library" section later on this page.
After the library is initialized you can do calls like you would with the jquerry ajax method. You can use GET and POST, pass named parameters and recieve json and html responses.

```JavaScript

$(function () {
    //create instance of library
    var cdlUtil = new CrossDomainUtil();

    //initialize library
    cdlUtil.Init(sampleServerUrl + "/home/proxy?SPHostUrl=" + sampleHostUrl);

    //code on get button click
    $("#DoGetButton").click(function (e) {
        e.preventDefault();

        if (cdlUtil.Initialized) {
            cdlUtil.ajax({
                method: "GET", //GET or POST
                url: sampleServerUrl + "/home/TestGet?SPHostUrl=" + sampleHostUrl,  //action on controller that is called. Always pass SPHostUrl!!!
                data: { id: "5" },  //pass id to controller action
                dataType: "json", //datatype that you expect back - eg. json or html
                success: function (data) { //function that executes when the call succeeds
                    alert("Got response from get request: " + data);
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
``` 

## Perform a POST request ##

```JavaScript
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
``` 

## Render a MVC view on your SharePoint page, and do a POST ##

This code will show the view on your SharePoint page.

```JavaScript

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
``` 

This code will post data entered in the textboxes of the view back to the MVC webserver.

```JavaScript

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
``` 

# How to setup the cross domain lib #

## STEP1: add the following to your Global.asax file ##

The following code is needed to give the application access to your session cookie. Without this cookie, you will lose your session state. Since the SharepointContext lives there, that would be a bad thing.

```JavaScript

protected void Application_BeginRequest(object sender, EventArgs e)
{

    HttpContext.Current.Response.AddHeader("p3p", "CP=\"CAO PSA OUR\"");
}

``` 

## STEP2: add the proxy page ##

The cross domain lib uses hidden Iframes and postmessages to communicate across the different domains. To do this, it needs a page (proxy) that it can send messages to. This page will relay the messages to the server.

Create a new controller action. Make sure the SharePointContextFilter atribute is applied.

```JavaScript

[SharePointContextFilter]
public ActionResult Proxy()
{
    return View(); //this page is used as a proxy by the cross domain lib. The view should be a empty html page with references to jquerry and CrossDomainProxy.js
}

``` 

Add the view. Reference jquerry, and the CrossDomainProxy.js file. No additional code is needed.

```HTML
@{
    Layout = null;
}
<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>Proxy</title>
    <script src="~/Scripts/jquery-1.10.2.min.js"></script>
    <script src="~/Scripts/CrossDomainProxy.js"></script>
</head>
<body>
    <div> 
    </div>
</body>
</html>

```

## STEP3: add the controller action you want to invoke ##

```JavaScript

public ActionResult TestGet(string id)
{
    var userName = GetCurrentUsername();  //a call to SharePoint is made to get username
    var user = string.Format("Id passed to function is {0}. Current user fetched by C# CSOM on server: {1}.", id, userName);
    return Json(user, JsonRequestBehavior.AllowGet);
}

 ``` 

## STEP4: add the cross domain library on the SharePoint page ##

Make a instance of the library. Initialize the library by passing the URL of the proxy page that you created (in our case home/proxy)
After initializing you can call the .ajax method to communicate with the server.

```JavaScript

$(function () {
    //create instance of library
    var cdlUtil = new CrossDomainUtil();

    //initialize library
    cdlUtil.Init("https://somecoolhost.com/home/proxy?SPHostUrl=" + yourApplicationSpHostUrl);

    //code on get button click
    $("#DoGetButton").click(function (e) {
        e.preventDefault();

        if (cdlUtil.Initialized) {
            cdlUtil.ajax({
                method: "GET", //GET or POST
                url: sampleServerUrl + "/home/TestGet?SPHostUrl=" + sampleHostUrl,  //action on controller that is called. Always pass SPHostUrl!!!
                data: { id: "5" },  //pass id to controller action
                dataType: "json", //datatype that you expect back - eg. json or html
                success: function (data) { //function that executes when the call succeeds
                    alert("Got response from get request: " + data);
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

 ``` 
<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.CrossDomainLib" />