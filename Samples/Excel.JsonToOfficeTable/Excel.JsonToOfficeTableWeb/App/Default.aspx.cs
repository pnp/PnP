using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Excel.JsonToOfficeTableWeb.App
{
    public partial class Default : System.Web.UI.Page
    {
        protected void btnSubmit2_Click(object sender, EventArgs e)
        {
            //use the stock service to get the history
            //although this samples a local service...
            //ANY data access .NET supports could be used
            Services.Stocks s = new Services.Stocks();
            var history = s.GetHistory(txtSymbol2.Text, Convert.ToInt32(cboFromYear2.SelectedValue));
            using (MemoryStream stream = new MemoryStream())
            {
                //serialize the List<StockStats> to a JSON string
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Services.StockStat>));
                ser.WriteObject(stream, history);
                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                var json = sr.ReadToEnd();

                //output the json string of stock history as javascript on the page so script can read and process it
                Page.ClientScript.RegisterStartupScript(typeof(Default), "JSONData", String.Format("var jsonData = {0};", json), true);
            }
        }
    }
}