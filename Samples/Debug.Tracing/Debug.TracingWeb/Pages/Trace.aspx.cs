using Debug.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Debug.TracingWeb.Pages
{
    public partial class Trace : System.Web.UI.Page
    {
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

        protected void EnableTracing_Click(object sender, EventArgs e)
        {
            TraceUtil.EnableTracing();
            OutputLabel.Text = "Tracing enabled";
        }

        protected void DisbleTracing_Click(object sender, EventArgs e)
        {
            TraceUtil.DisableTracing();
            OutputLabel.Text = "Tracing disabled";
        }

        protected void SetMaxRequest_Click(object sender, EventArgs e)
        {
            TraceUtil.SetMaxTraceRequests(Convert.ToInt32(noRequests.Text));
            OutputLabel.Text = "Max requests altered";
        }
    }
}