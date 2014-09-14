using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace Excel.JsonToOfficeTableWeb.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Stocks
    {
        [OperationContract]
        [WebGet()]
        public List<StockStat> GetHistory(string stock, int fromyear)
        {
            List<StockStat> history = new List<StockStat>();
            string url = String.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&a=01&b=01&c={1}&d={2}&e={3}&f={4}", stock, fromyear.ToString(), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("yyyy"));
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                string csv = reader.ReadToEnd();

                //split by newline character and skip header row
                var rows = csv.Split('\n');
                for (int i = 1; i < rows.Length; i++)
                {
                    //make sure this isn't an empty row
                    if (rows[i].Trim().Length > 0)
                    {
                        //split row into columns
                        var cols = rows[i].Split(',');
                        history.Add(new StockStat()
                        {
                            Date = cols[0],
                            Open = Convert.ToDouble(cols[1]),
                            High = Convert.ToDouble(cols[2]),
                            Low = Convert.ToDouble(cols[3]),
                            Close = Convert.ToDouble(cols[4]),
                            Volume = Convert.ToInt32(cols[5]),
                            AdjustedClose = Convert.ToDouble(cols[6])
                        });
                    }
                }
            }

            return history;
        }
    }

    public class StockStat
    {
        public string Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public int Volume { get; set; }
        public double AdjustedClose { get; set; }
    }
}