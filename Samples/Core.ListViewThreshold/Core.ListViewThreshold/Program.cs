using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ListViewThreshold
{
    class Program
    {
        static void Main(string[] args)
        {
            CamlQuery camlQuery = new CamlQuery();

            //CamlQuery extension for LisThreshold limit

            //Set View Scope for the Query
            camlQuery.SetViewAttribute(QueryScope.RecursiveAll);

            //Set Viewfields as String array
            //camlQuery.SetViewFields(new string[] { "ID", "Title"});

            //Or Set the ViewFields xml
            camlQuery.SetViewFields(@"<FieldRef Name='ID'/><FieldRef Name='Title'/>");

            //Override the QueryThrottle Mode for avoiding ListViewThreshold exception
            camlQuery.SetQueryThrottleMode(QueryThrottleMode.Override);

            //If Query has filter, column which is Indexed can be used Override in Orderby
            camlQuery.SetOrderByIndexField();

            //Use OrderBy ID field if Query doesn't have filter with indexed column
            //camlQuery.SetOrderByIDField();

            //Set Query condition
            camlQuery.SetQuery("<Eq><FieldRef Name='IndexedField' /><Value Type='Text'>value</Value></Eq>");

            //Set RowLimit
            camlQuery.SetQueryRowlimit(5000);

            using (ClientContext context = new ClientContext("SiteUrl"))
            {
                ContentIterator contentIterator = new ContentIterator(context);

                //Ensure Field indexed
                contentIterator.EnsureFieldIndexed("ListName", "FieldNameUsedInQuery");

                try
                {
                   contentIterator.ProcessListItems("ListName", camlQuery,
                   ProcessItems,
                   delegate(ListItemCollection items, System.Exception ex)
                   {
                       return true;
                   });
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
               
            }
        }

        private static void ProcessItems(ListItemCollection items)
        {
            foreach (var item in items)
                Console.WriteLine(item.Id);
        }
    }
}
