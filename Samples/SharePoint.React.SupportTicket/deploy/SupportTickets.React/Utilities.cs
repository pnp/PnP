using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportTickets.React
{
    public class Utilities
    {
        public static string serverRelativeUrl = null;
        public static void InitServerRelativeUrl(ClientContext ctx)
        {
            if (serverRelativeUrl == null)
            {
                ctx.Load(ctx.Web);
                ctx.ExecuteQuery();
                serverRelativeUrl = ctx.Web.ServerRelativeUrl;
            }
        }
        public static string ReplaceTokens(ClientContext ctx, string input)
        {
            InitServerRelativeUrl(ctx);
            string output = input.Replace("~sitecollection", serverRelativeUrl);
            return output;
        }
        public static string ReplaceTokensInAssetFile(ClientContext ctx, string filePath)
        {
            string fileContent = System.IO.File.ReadAllText(filePath);
            fileContent = ReplaceTokens(ctx, fileContent);
            string newFilePath = filePath + Guid.NewGuid().ToString("D");
            System.IO.File.WriteAllText(newFilePath, fileContent);
            return newFilePath;
        }
    }
}