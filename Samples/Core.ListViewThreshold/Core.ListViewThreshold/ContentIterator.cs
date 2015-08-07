using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ListViewThreshold
{
    public class ContentIterator
    {
        private readonly ClientContext _context;

        public ContentIterator(ClientContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
        }

        public delegate void ItemsProcessor(ListItemCollection items);

        public delegate bool ItemsProcessorErrorCallout(ListItemCollection items, System.Exception e);

        public delegate void ItemProcessor(ListItem item);

        public delegate bool ItemProcessorErrorCallout(ListItem item, System.Exception e);

        private const string itemEnumerationOrderByID = "<OrderBy Override='TRUE'><FieldRef Name='ID' /></OrderBy>";

        private const string itemEnumerationOrderByIDDesc = "<OrderBy Override='TRUE' ><FieldRef Name='ID' Ascending='FALSE' /></OrderBy>";

        private const string itemEnumerationOrderByPath = "<OrderBy Override='TRUE'><FieldRef Name='FileDirRef' /><FieldRef Name='FileLeafRef' /></OrderBy>";

        private const string itemEnumerationOrderByNVPField = "<OrderBy UseIndexForOrderBy='TRUE' Override='TRUE' />";

        private const string overrideQueryThrottleMode = "<QueryThrottleMode>Override</QueryThrottleMode>";

        public static string ItemEnumerationOrderByID
        {
            get
            {
                return "<OrderBy Override='TRUE'><FieldRef Name='ID' /></OrderBy>";
            }
        }

        public static string ItemEnumerationOrderByIDDesc
        {
            get
            {
                return "<OrderBy Override='TRUE' ><FieldRef Name='ID' Ascending='FALSE' /></OrderBy>";
            }
        }

        public static string ItemEnumerationOrderByPath
        {
            get
            {
                return "<OrderBy Override='TRUE'><FieldRef Name='FileDirRef' /><FieldRef Name='FileLeafRef' /></OrderBy>";
            }
        }

        public static string ItemEnumerationOrderByNVPField
        {
            get
            {
                return "<OrderBy UseIndexForOrderBy='TRUE' Override='TRUE' />";
            }
        }

        public static string OverrideQueryThrottleMode
        {
            get
            {
                return "<QueryOptions><QueryThrottleMode>Override</QueryThrottleMode></QueryOptions>";
            }
        }

        public void EnsureFieldIndexed(string listName, string fieldName)
        {
            List list = _context.Web.Lists.GetByTitle(listName);
            Field field = list.Fields.GetByInternalNameOrTitle(fieldName);
            field.Indexed = true;
            field.Update();
            _context.ExecuteQuery();
        }

        public void ProcessListItems(string listName, CamlQuery camlQuery, ItemsProcessor itemsProcessor, ItemsProcessorErrorCallout errorCallout)
        {
            List list = _context.Web.Lists.GetByTitle(listName);
            CamlQuery query = camlQuery;

            ListItemCollectionPosition position = null;
            query.ListItemCollectionPosition = position;

            while (true)
            {
                ListItemCollection listItems = list.GetItems(query);
                _context.Load(listItems, items => items.ListItemCollectionPosition);
                _context.ExecuteQuery();
                try
                {
                    itemsProcessor(listItems);
                }
                catch (System.Exception ex)
                {
                    if (errorCallout == null || errorCallout(listItems, ex))
                    {
                        throw;
                    }
                }

                if (listItems.ListItemCollectionPosition == null)
                {
                    return;
                }
                else
                {
                    /*if query contains lookup column filter last batch returns null 
                     by removing the lookup column in paginginfo query will return next records
                     */
                    string pagingInfo = listItems.ListItemCollectionPosition.PagingInfo;
                    string[] parameters = pagingInfo.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> requiredParameters = new List<string>();
                    foreach (string str in parameters)
                    {
                        if (str.Contains("Paged=") || str.Contains("p_ID="))
                            requiredParameters.Add(str);
                    }

                    pagingInfo = string.Join("&", requiredParameters.ToArray());
                    listItems.ListItemCollectionPosition.PagingInfo = pagingInfo;
                    query.ListItemCollectionPosition = listItems.ListItemCollectionPosition;
                }

            }

        }

        public void ProcessListItem(string listName, CamlQuery camlQuery, ItemProcessor itemProcessor, ItemProcessorErrorCallout errorCallout)
        {
            List list = _context.Web.Lists.GetByTitle(listName);
            CamlQuery query = camlQuery;

            ListItemCollectionPosition position = null;
            query.ListItemCollectionPosition = position;

            while (true)
            {
                ListItemCollection listItems = list.GetItems(query);
                _context.Load(listItems, items => items.ListItemCollectionPosition);
                _context.ExecuteQuery();

                for (int i = 0; i < listItems.Count; i++)
                {
                    try
                    {
                        itemProcessor(listItems[i]);

                    }
                    catch (System.Exception ex)
                    {
                        if (errorCallout == null || errorCallout(listItems[i], ex))
                        {
                            throw;
                        }
                    }

                }

                if (listItems.ListItemCollectionPosition == null)
                {
                    return;
                }
                else
                {
                    /*if query contains lookup column filter last batch returns null 
                     by removing the lookup column in paginginfo query will return next records
                     */
                    string pagingInfo = listItems.ListItemCollectionPosition.PagingInfo;
                    string[] parameters = pagingInfo.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> requiredParameters = new List<string>();
                    foreach (string str in parameters)
                    {
                        if (str.Contains("Paged=") || str.Contains("p_ID="))
                            requiredParameters.Add(str);
                    }

                    pagingInfo = string.Join("&", requiredParameters.ToArray());
                    listItems.ListItemCollectionPosition.PagingInfo = pagingInfo;
                    query.ListItemCollectionPosition = listItems.ListItemCollectionPosition;
                }

            }

        }
    }
}
