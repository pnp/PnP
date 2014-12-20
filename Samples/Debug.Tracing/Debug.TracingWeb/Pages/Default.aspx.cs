using Debug.Tracing;
using Debug.TracingWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Debug.TracingWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RenderChromeControl();
        }

        private void RenderChromeControl()
        {
            // define initial script, needed to render the chrome control
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }

            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };

                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            //register script in page
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);
        }

        protected void TraceMessage_Click(object sender, EventArgs e)
        {
            TraceUtil.TraceMessage("Starting some work");  //log message to trace.axd
            
            //Do some work here (step 1)

            TraceUtil.TraceMessage("work step 1 complete");  //log your work progress

            //Do some work here (step 2)

            TraceUtil.TraceMessage("work complete");  //log that your work completed

            TraceMessagePanel.Visible = true;
            TraceMethodPanel.Visible = false;
            TraceErrorPanel.Visible = false;
        }

        protected void ManageTracing_Click(object sender, EventArgs e)
        {
            Server.Transfer("Trace.aspx", true);
        }

        protected void TraceMethods_Click(object sender, EventArgs e)
        {
            using (new TraceUtil().TraceMethod(sender, e))  //this logs the beginning and end of the method to trace.axd
            {
                //do some logic here

                //use class to execute logic
                new SomeClass().SomeMethod1("parameter1", "param2");

                TraceMessagePanel.Visible = false;
                TraceMethodPanel.Visible = true;
                TraceErrorPanel.Visible = false;
            }
        }

        protected void LogError_Click(object sender, EventArgs e)
        {
            try
            {
               //do some logic here
                TraceMessagePanel.Visible = false;
                TraceMethodPanel.Visible = false;
                TraceErrorPanel.Visible = true;

                throw new Exception("Something bad happened here");
            }
            catch (Exception eX)  //place the try catch block at the root method, to catch exceptions. You can also log all unhandled exceptions in a application by adding some code to the GLOBAL.ASAX file (see global.asax in this solution)
            {
                ErrorLogger.LogException(eX); //this method wil log the exception + stacktrace to the trace, and to Sharepoint (site content screen)
                //for this to work you have to add "<add key="ErrorLoggerAppProductId" value="{b4351824-86ea-41f0-b29c-1605b159e4f0}" />" to your web.config file
                //change the {b4351824-86ea-41f0-b29c-1605b159e4f0} with the ProductID of your application (you can find this by opening the appmanifest.xml with a xml viewer)
                //It can take a couple of minutes for the error to show up in the sharepoint screen

                //show some error message to the user...
            }
        }
    }
}