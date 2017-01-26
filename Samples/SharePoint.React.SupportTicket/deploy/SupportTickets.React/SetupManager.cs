using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;

namespace SupportTickets.React
{
    public class SetupManager
    {
        public static void ProvisionLists(ClientContext ctx)
        {            
            List supportPlansList = null;
            if (!ctx.Web.ListExists("SupportPlans"))
            {
                Console.WriteLine("Support plans list...");
                supportPlansList = ctx.Web.CreateList(ListTemplateType.GenericList, "SupportPlans", false, false, "Lists/SupportPlans", false);
                ctx.Load(supportPlansList);
                ctx.ExecuteQueryRetry();

                //Provision support plan items
                ListItemCreationInformation newSupportPlanCreationInfomation;
                newSupportPlanCreationInfomation = new ListItemCreationInformation();
                ListItem newSupportPlan = supportPlansList.AddItem(newSupportPlanCreationInfomation);
                newSupportPlan["Title"] = "Basic Free Plan";
                newSupportPlan.Update();
                newSupportPlan = supportPlansList.AddItem(newSupportPlanCreationInfomation);
                newSupportPlan["Title"] = "Standard Plan";
                newSupportPlan.Update();
                newSupportPlan = supportPlansList.AddItem(newSupportPlanCreationInfomation);
                newSupportPlan["Title"] = "Premium Plan";
                newSupportPlan.Update();
                newSupportPlan = supportPlansList.AddItem(newSupportPlanCreationInfomation);
                newSupportPlan["Title"] = "Super Duper Plan";
                newSupportPlan.Update();

                ctx.ExecuteQueryRetry();
            }
            else
            {
                supportPlansList = ctx.Web.GetListByUrl("Lists/SupportPlans");
                Console.WriteLine("Support Plans list was already available");
            }

            List businessImpactsList = null;
            if (!ctx.Web.ListExists("BusinessImpacts"))
            {
                Console.WriteLine("Business Impacts list...");
                businessImpactsList = ctx.Web.CreateList(ListTemplateType.GenericList, "BusinessImpacts", false, false, "Lists/BusinessImpacts", false);
                ctx.Load(businessImpactsList);
                ctx.ExecuteQueryRetry();

                //Provision business impact items
                ListItemCreationInformation newBusinessImpactCreationInfomation;
                newBusinessImpactCreationInfomation = new ListItemCreationInformation();
                ListItem newBusinessImpact = businessImpactsList.AddItem(newBusinessImpactCreationInfomation);
                newBusinessImpact["Title"] = "Low Business Impact";
                newBusinessImpact.Update();
                newBusinessImpact = businessImpactsList.AddItem(newBusinessImpactCreationInfomation);
                newBusinessImpact["Title"] = "Medium Business Impact";
                newBusinessImpact.Update();
                newBusinessImpact = businessImpactsList.AddItem(newBusinessImpactCreationInfomation);
                newBusinessImpact["Title"] = "High Business Impact";
                newBusinessImpact.Update();
               
                ctx.ExecuteQueryRetry();
            }
            else
            {
                businessImpactsList = ctx.Web.GetListByUrl("Lists/BusinessImpacts");
                Console.WriteLine("Business Impacts list was already available");
            }


            List ticketsQueue = null;
            if (!ctx.Web.ListExists("TicketsQueue"))
            {
                Console.WriteLine("Tickets Queue...");
                ticketsQueue = ctx.Web.CreateList(ListTemplateType.GenericList, "TicketsQueue", false, false, "Lists/TicketsQueue", false);
                ctx.Load(ticketsQueue);
                ctx.ExecuteQueryRetry();

                ticketsQueue.CreateField(@"<Field Type=""Number"" DisplayName=""Ticket Number"" ID=""{69608652-463B-4A4E-AB6D-E436011C06EC}"" Name=""TicketNumber""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Issue Description"" ID=""{B8E6D974-92A9-4826-A932-7863C202239C}"" Name=""IssueDescription""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Ticket History"" ID=""{2EA0D957-DF93-42A9-AA74-9E03E55BB7D6}"" Name=""TicketHistory""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Business Impact"" ID=""{D4105758-AA5B-41EF-B9AC-CD61DE496AEC}"" Name=""BusinessImpact""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Contact Full Name"" ID=""{E01C089F-D34E-46FB-8BE3-1AD7EA63E2A4}"" Name=""ContactFullName""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Contact Email"" ID=""{6A2EC179-74EA-43A7-8DEB-C01E4572B33D}"" Name=""ContactEmail""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Contact Phone"" ID=""{3B103F93-937A-419D-A42E-C41ED7AB457A}"" Name=""ContactPhone""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Support Plan"" ID=""{75F4CF90-F771-4047-8F54-1114D2C45271}"" Name=""SupportPlan""></Field>", false);
                ticketsQueue.CreateField(@"<Field Type=""Text"" DisplayName=""Current Status"" ID=""{A37D84BC-D31C-4A6F-9BF7-E1159069F457}"" Name=""CurrentStatus""></Field>", false);
                ticketsQueue.Update();
                ctx.Load(ticketsQueue.DefaultView, p => p.ViewFields);
                ctx.ExecuteQueryRetry();

                // Add fields to view
                ticketsQueue.DefaultView.ViewFields.Add("Title");
                ticketsQueue.DefaultView.ViewFields.Add("TicketNumber");
                ticketsQueue.DefaultView.ViewFields.Add("IssueDescription");
                ticketsQueue.DefaultView.ViewFields.Add("BusinessImpact");
                ticketsQueue.DefaultView.ViewFields.Add("TicketHistory");
                ticketsQueue.DefaultView.ViewFields.Add("ContactFullName");
                ticketsQueue.DefaultView.ViewFields.Add("ContactEmail");
                ticketsQueue.DefaultView.ViewFields.Add("ContactPhone");
                ticketsQueue.DefaultView.ViewFields.Add("SupportPlan");
                ticketsQueue.DefaultView.ViewFields.Add("CurrentStatus");                
                ticketsQueue.DefaultView.Update();
                ctx.ExecuteQueryRetry();

            }
            else
            {
                ticketsQueue = ctx.Web.GetListByUrl("Lists/TicketsQueue");
                Console.WriteLine("TicketsQueue list was already available");
            }
        }

        #region web part manipulation

        public static bool IsWebPartOnPage(ClientContext ctx, string relativePageUrl, string title)
        {
            var webPartPage = ctx.Web.GetFileByServerRelativeUrl(relativePageUrl);
            ctx.Load(webPartPage);
            ctx.ExecuteQuery();

            if (webPartPage == null)
            {
                return false;
            }

            LimitedWebPartManager limitedWebPartManager = webPartPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            ctx.Load(limitedWebPartManager.WebParts, wps => wps.Include(wp => wp.WebPart.Title));
            ctx.ExecuteQueryRetry();

            if (limitedWebPartManager.WebParts.Count >= 0)
            {
                for (int i = 0; i < limitedWebPartManager.WebParts.Count; i++)
                {
                    WebPart oWebPart = limitedWebPartManager.WebParts[i].WebPart;
                    if (oWebPart.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void CloseAllWebParts(ClientContext ctx, string relativePageUrl)
        {
            var webPartPage = ctx.Web.GetFileByServerRelativeUrl(relativePageUrl);
            ctx.Load(webPartPage);
            ctx.ExecuteQuery();

            if (webPartPage == null)
            {
                return;
            }

            LimitedWebPartManager limitedWebPartManager = webPartPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            ctx.Load(limitedWebPartManager.WebParts, wps => wps.Include(wp => wp.WebPart.Title));
            ctx.ExecuteQueryRetry();

            if (limitedWebPartManager.WebParts.Count >= 0)
            {
                for (int i = 0; i < limitedWebPartManager.WebParts.Count; i++)
                {
                    limitedWebPartManager.WebParts[i].CloseWebPart();
                    limitedWebPartManager.WebParts[i].SaveWebPartChanges();
                }
                ctx.ExecuteQuery();
            }
        }
        #endregion

    }
}
