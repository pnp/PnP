using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.SharePoint.Client;

namespace ECM.AutoTaggingWeb
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

            if (!Page.IsPostBack)
            {

            }
        }

        /// <summary>
        /// Used to Create a Library & ReR for ItemAdding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScenario1_Click(object sender, EventArgs e)
        {
            var _libraryToCreate = this.GetLibaryInformationItemAdding();
 
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                try 
                { 
                    if(!ctx.Web.ListExists(_libraryToCreate.Title))
                    {
                        ScenarioHandler _scenario = new ScenarioHandler();
                        _scenario.CreateContosoDocumentLibrary(ctx, _libraryToCreate);
                    }
                    List _list = ctx.Web.Lists.GetByTitle(_libraryToCreate.Title);
                    EventReceiverDefinitionCreationInformation _rec = ReceiverHelper.CreateEventReciever(ScenarioHandler.AUTOTAGGING_ITEM_ADDING_RERNAME, EventReceiverType.ItemAdding);
                    ReceiverHelper.AddEventReceiver(ctx, _list, _rec);
                }
                catch(Exception)
                {

                }
            }
        }         
        /// <summary>
        /// Used to create a library & Rer for ItemAdded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScenario2_Click(object sender, EventArgs e)
        {
            var _libraryToCreate = this.GetLibaryInformationItemAdded();

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                try
                {
                    if (!ctx.Web.ListExists(_libraryToCreate.Title))
                    {
                        ScenarioHandler _scenario = new ScenarioHandler();
                        _scenario.CreateContosoDocumentLibrary(ctx, _libraryToCreate);
                    }
                    List _list = ctx.Web.Lists.GetByTitle(_libraryToCreate.Title);
                    EventReceiverDefinitionCreationInformation _rec = ReceiverHelper.CreateEventReciever(ScenarioHandler.AUTOTAGGING_ITEM_ADDED_RERNAME, EventReceiverType.ItemAdded);
                    ReceiverHelper.AddEventReceiver(ctx, _list, _rec);
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Removes ItemAdding ReR from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScenario3_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            var _sampleLibrary = this.GetLibaryInformationItemAdding();
            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                try
                {
                    if (ctx.Web.ListExists(_sampleLibrary.Title))
                    {
                        List _list = ctx.Web.Lists.GetByTitle(_sampleLibrary.Title);
                        ReceiverHelper.RemoveEventReceiver(ctx, _list, ScenarioHandler.AUTOTAGGING_ITEM_ADDING_RERNAME);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        /// <summary>
        /// Removes the ItemAdded ReR from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScenario4_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            var _sampleLibrary = this.GetLibaryInformationItemAdded();
            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                try
                {
                    if (ctx.Web.ListExists(_sampleLibrary.Title))
                    {
                        List _list = ctx.Web.Lists.GetByTitle(_sampleLibrary.Title);
                        ReceiverHelper.RemoveEventReceiver(ctx, _list, ScenarioHandler.AUTOTAGGING_ITEM_ADDED_RERNAME);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Helper Member to get the library for the ItemAdded ReR
        /// </summary>
        /// <returns></returns>
        private Library GetLibaryInformationItemAdded()
        {
            Library _libraryToCreate = new Library()
            {
                Title = "AutoTaggingSampleItemAdded",
                Description = "This is a demo showing how to use ItemAdded ReR",
                VerisioningEnabled = false
            };
            return _libraryToCreate;
        }

        /// <summary>
        /// Helper Member to get the libary for the ItemAdding Rer
        /// </summary>
        /// <returns></returns>
        private Library GetLibaryInformationItemAdding()
        {
            Library _libraryToCreate = new Library()
            {
                Title = "AutoTaggingSampleItemAdding",
                Description = "This is a demo showing how to use ItemAdding ReR",
                VerisioningEnabled = false
            };
            return _libraryToCreate;
        }
    }
}