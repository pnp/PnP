/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.SharePoint.Client;
//using Microsoft.SharePoint.Samples;
using Microsoft.IdentityModel.S2S.Tokens;
using System.Net;
using System.IO;
using System.Xml;

using System.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Data.OData;

using Core.ODataBatchWeb.ODataHelpers;

namespace Core.ODataBatchWeb
{
    public partial class Default : System.Web.UI.Page
    {
        SharePointContextToken contextToken;
        string accessToken;
        Uri sharepointUrl;
      
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
            string contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);

            if (contextTokenString != null)
            {
                contextToken =
                    TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
                accessToken =
                    TokenHelper.GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;
               
                // Cache the access token in the command argument of the buttons. Don't do this in a production app.
                Button2.CommandArgument = accessToken;
                Button3.CommandArgument = accessToken;
                Button4.CommandArgument = accessToken;
            }
            else if (!IsPostBack)
            {
                Response.Write("Could not find a context token.");
            } 
        }


            protected void Button2_Click(object sender, EventArgs e)
        {
            string accessToken = ((Button)sender).CommandArgument;
            Int16 listRetrievalCount = 0;

            if (IsPostBack)
            {
                // Get the host web's URL.
                sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
            }
            
            // Create the parent request
            var batchRequest = new BatchODataRequest(sharepointUrl + "_api/"); // ctor adds "$batch"
            batchRequest.SetHeader("Authorization", "Bearer " + accessToken);
            
            using (var oDataMessageWriter = new ODataMessageWriter(batchRequest))
            {
                var oDataBatchWriter = oDataMessageWriter.CreateODataBatchWriter();
                oDataBatchWriter.WriteStartBatch();

               // Create the two child query operations.
               oDataBatchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(sharepointUrl.ToString() + "_api/Web/lists/getbytitle('Composed Looks')/items?$select=Title"));
                listRetrievalCount++;

                oDataBatchWriter.CreateOperationRequestMessage(
                   "GET", new Uri(sharepointUrl.ToString() + "_api/Web/lists/getbytitle('User Information List')/items?$select=Title"));
                listRetrievalCount++;
                
                oDataBatchWriter.WriteEndBatch();
                oDataBatchWriter.Flush();
            }

            // Parse the response and bind the data to the UI controls
            var oDataResponse = batchRequest.GetResponse();

            using (var oDataReader = new ODataMessageReader(oDataResponse))
            {
                var oDataBatchReader = oDataReader.CreateODataBatchReader();

                while (oDataBatchReader.Read())
                {
                    switch (oDataBatchReader.State)
                    {
                        case ODataBatchReaderState.Initial:

                            // Optionally, handle the start of a batch payload.
                            break;
                        case ODataBatchReaderState.Operation:

                            // Start of an operation (either top-level or in a changeset)
                            var operationResponse = oDataBatchReader.CreateOperationResponseMessage();

                            // Response's ATOM markup parsing and presentation section
                            using (var stream = operationResponse.GetStream())
                            {
                                List<XElement> entries = SharePointDataHelpers.ListDataHelper.ExtractListItemsFromATOMResponse(stream);

                                var itemTitles = SharePointDataHelpers.ListDataHelper.GetItemTitles(entries);

                                // Bind data to the grid on the page.
                                   // In a production app, check operationResponse.StatusCode and handle non-200 statuses.
                                   // For simplicity, this sample assumes status 200 (the list items are returned).
                                switch (listRetrievalCount)
                                {
                                    case 2:
                                        GridView2.DataSource = itemTitles;
                                        GridView2.DataBind();
                                        listRetrievalCount--;
                                        break;

                                    case 1:
                                        GridView1.DataSource = itemTitles;
                                        GridView1.DataBind();
                                        listRetrievalCount--;
                                        break;
                                }
                            };
                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            // Optionally, handle the start of a change set.
                            break;

                        case ODataBatchReaderState.ChangesetEnd:
                            // When this sample was created, SharePoint did not support "all or nothing" transactions. 
                            // If that changes in the future this is where you would commit the transaction.
                            break;

                        case ODataBatchReaderState.Exception:
                            // In a producition app handle exeception. Omitted for simplicity in this sample app.
                            break;
                    }
                }                
            }
            TwoLists.Visible = true;
        }


        protected void Button3_Click(object sender, EventArgs e)
        {
            string accessToken = ((Button)sender).CommandArgument;
            Int16 operationCount = 0;

            if (IsPostBack)
            {
                // Get the host web's URL.
                sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
            }

            // Create the parent request
            var batchRequest = new BatchODataRequest(sharepointUrl + "_api/"); // ctor adds "$batch"
            batchRequest.SetHeader("Authorization", "Bearer " + accessToken);

            using (var oDataMessageWriter = new ODataMessageWriter(batchRequest))
            {
                var oDataBatchWriter = oDataMessageWriter.CreateODataBatchWriter();
                oDataBatchWriter.WriteStartBatch();

                oDataBatchWriter.WriteStartChangeset();

                // Create the list adding operation
                var addListOperation = oDataBatchWriter.CreateOperationRequestMessage(
                    "POST", new Uri(sharepointUrl.ToString() + "_api/lists"));
                addListOperation.SetHeader("Content-Type", "application/json;odata=verbose");

                // Write the body of the operation
                using (var oDataInsertWriter = new ODataMessageWriter(addListOperation))
                {
                    var entryWriter = oDataInsertWriter.CreateODataEntryWriter();

                    var insertionBody = new ODataEntry()
                    {
                        Properties = new[]
                            {
                                new ODataProperty() {Name = "Title", Value = NewList.Text},
                                new ODataProperty() {Name = "BaseTemplate", Value = "100"}
                            }
                    };

                    // Set the "__metadata" type property
                    insertionBody.TypeName = "SP.List";

                    entryWriter.WriteStart(insertionBody);
                    entryWriter.WriteEnd();
                }
                oDataBatchWriter.WriteEndChangeset();
                operationCount++;

                // Create the query operation
                var queryOperationMessage3 = oDataBatchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(sharepointUrl.ToString() + "_api/Web/lists"));
                operationCount++;

                oDataBatchWriter.WriteEndBatch();
                oDataBatchWriter.Flush();
            }

            // Parse the response and bind the data to the UI controls
            var oDataResponse = batchRequest.GetResponse();

            using (var oDataReader = new ODataMessageReader(oDataResponse))
            {
                var oDataBatchReader = oDataReader.CreateODataBatchReader();

                while (oDataBatchReader.Read())
                {
                    switch (oDataBatchReader.State)
                    {
                        case ODataBatchReaderState.Initial:
                            // Optionally, handle the start of a batch payload.
                            break;

                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a changeset)
                            var operationResponse = oDataBatchReader.CreateOperationResponseMessage();

                            // Response ATOM markup parsing and presentation
                            using (var stream = operationResponse.GetStream())
                            {

                                switch (operationCount)
                                {
                                    case 2: // The "add new list" operation
                                        
                                        if (operationResponse.StatusCode == 201)
                                        {
                                            AddListResponse.Text = "Your list was created!";
                                        }
                                        else
                                        {
                                            AddListResponse.Text = "Your list was not created. Status returned: " + operationResponse.StatusCode.ToString();
                                        }

                                        operationCount--;
                                        break;

                                    case 1: // The "List of Lists" operation

                                        // Bind data to the grid on the page.
                                           // In a production app, check operationResponse.StatusCode and handle non-200 statuses.
                                           // For simplicity, this sample assumes status 200 (the list items are returned).
                                        List<XElement> entries = SharePointDataHelpers.ListDataHelper.ExtractListItemsFromATOMResponse(stream);
                                        var itemTitles = SharePointDataHelpers.ListDataHelper.GetItemTitles(entries);
                                        GridView3.DataSource = itemTitles;
                                        GridView3.DataBind();
                                        operationCount--;
                                        break;
                                }
                            };
                            break;

                        case ODataBatchReaderState.ChangesetStart:
                            // Optionally, handle the start of a change set.
                            break;

                        case ODataBatchReaderState.ChangesetEnd:
                            // When this sample was created, SharePoint did not support "all or nothing" transactions. 
                            // If that changes in the future this is where you would commit the transaction.
                            break;

                        case ODataBatchReaderState.Exception:
                            // In a producition app handle exeception. Omitted for simplicity in this sample app.
                            break;
                    }
                }
            }
            GridView3.Visible = true;
            TwoLists.Visible = false;
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            string accessToken = ((Button)sender).CommandArgument;
            Int16 operationCount = 0;

            if (IsPostBack)
            {
                // Get the host web's URL.
                sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);
            }

            // Create the parent request
            var batchRequest = new BatchODataRequest(sharepointUrl + "_api/"); // ctor adds "$batch"
            batchRequest.SetHeader("Authorization", "Bearer " + accessToken);

            using (var oDataMessageWriter = new ODataMessageWriter(batchRequest))
            {
                var oDataBatchWriter = oDataMessageWriter.CreateODataBatchWriter();
                oDataBatchWriter.WriteStartBatch();

                oDataBatchWriter.WriteStartChangeset();

                // Create the list deleting operation
                var deleteOperation = oDataBatchWriter.CreateOperationRequestMessage(
                    "DELETE", new Uri(sharepointUrl.ToString() + "_api/Web/lists/getbytitle(\'" +OldList.Text+ "\')"));
                deleteOperation.SetHeader("If-Match", "\"1\"");

                oDataBatchWriter.WriteEndChangeset();
                operationCount++;

                // Create the query operation
                var queryOperationMessage3 = oDataBatchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(sharepointUrl.ToString() + "_api/Web/lists"));
                operationCount++;

                oDataBatchWriter.WriteEndBatch();
                oDataBatchWriter.Flush();
            }

            // Parse the response and bind the data to the UI controls
            var oDataResponse = batchRequest.GetResponse();

            using (var oDataReader = new ODataMessageReader(oDataResponse))
            {
                var oDataBatchReader = oDataReader.CreateODataBatchReader();

                while (oDataBatchReader.Read())
                {
                    switch (oDataBatchReader.State)
                    {
                        case ODataBatchReaderState.Initial:
                            // Optionally, handle the start of a batch payload.
                            break;

                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a changeset)
                            var operationResponse = oDataBatchReader.CreateOperationResponseMessage();

                            // Response ATOM markup parsing and presentation
                            using (var stream = operationResponse.GetStream())
                            {

                                switch (operationCount)
                                {
                                    case 2: // The "delete list" operation

                                        if (operationResponse.StatusCode == 200)
                                        {
                                            DeleteListResponse.Text = "Your list was deleted!";
                                        }
                                        else
                                        {
                                            DeleteListResponse.Text = "Your list was not deleted. Status returned: " + operationResponse.StatusCode.ToString();
                                        }

                                        operationCount--;
                                        break;

                                    case 1: // The "List of Lists" operation

                                        // Bind data to the grid on the page.
                                           // In a production app, check operationResponse.StatusCode and handle non-200 statuses.
                                           // For simplicity, this sample assumes status 200 (the list items are returned).
                                        List<XElement> entries = SharePointDataHelpers.ListDataHelper.ExtractListItemsFromATOMResponse(stream);
                                        var itemTitles = SharePointDataHelpers.ListDataHelper.GetItemTitles(entries);
                                        GridView4.DataSource = itemTitles;
                                        GridView4.DataBind();
                                        operationCount--;
                                        break;
                                }
                            };
                            break;

                        case ODataBatchReaderState.ChangesetStart:
                            // Optionally, handle the start of a change set.
                            break;

                        case ODataBatchReaderState.ChangesetEnd:
                            // When this sample was created, SharePoint did not support "all or nothing" transactions. 
                            // If that changes in the future this is where you would commit the transaction.
                            break;

                        case ODataBatchReaderState.Exception:
                            // In a producition app handle exeception. Omitted for simplicity in this sample app.
                            break;
                    }
                }
            }

            GridView3.Visible = false;
        }
    }
}

