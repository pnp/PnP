using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Core.ListViewThreshold
{
    public enum QueryThrottleMode
    {
        Default = 0,
        Override = 1,
        Strict = 2
    }

    public enum QueryScope
    {
        FilesOnly = 0,
        Recursive = 1,
        RecursiveAll = 2
    }

    public static partial class CamlQueryExtensions
    {
        private const string VIEW_QUERY = "<View><RowLimit>{0}</RowLimit></View>";
        private const string VIEW_XPATH = "/View";
        private const string QUERY_XPATH = "/View/Query";
        private const string ROWLIMIT_XPATH = "/View/RowLimit";
        private const string VIEWFIELDS_XPATH = "/View/ViewFields";
        private const string QUERY_THROTTLE_MODE = "<QueryThrottleMode>{0}</QueryThrottleMode>";
        private const string ORDER_BY_NVPFIELD = "<OrderBy UseIndexForOrderBy='TRUE' Override='TRUE' />";
        private const string ORDER_BY_ID = "<OrderBy Override='TRUE'><FieldRef Name='ID' /></OrderBy>";
        private const string ORDER_BY_ID_DESC = "<OrderBy Override='TRUE' ><FieldRef Name='ID' Ascending='FALSE' /></OrderBy>";

        private const uint DefaultMaxItemsPerThrottledOperation = 5000u;

        /// <summary>
        /// This method will set scope attribute in query
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        /// <param name="scope">Scope option</param>
        public static void SetViewAttribute(this CamlQuery camlQuery, QueryScope scope)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            //Add scope attribute to root element
            CreateScopeAttributeWithValue(xmlDoc, GetXmlNodeByXPath(VIEW_XPATH, xmlDoc), scope.ToString());

            //Update ViewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }

        /// <summary>
        /// This method will update the query condition in ViewXml
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        /// <param name="queryXml">query condition</param>
        public static void SetQuery(this CamlQuery camlQuery, string queryXml)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            if (string.IsNullOrEmpty(queryXml)) return;

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            XmlNode viewNode = GetXmlNodeByXPath(VIEW_XPATH, xmlDoc);
            XmlNode queryNode = GetXmlNodeByXPath(QUERY_XPATH, xmlDoc);

            XmlDocument queryDocument = LoadViewXml(queryXml);
            XmlNode qNode = GetXmlNodeByXPath(QUERY_XPATH.Replace(VIEW_XPATH, ""), queryDocument);

            XmlNode newQueryNode = queryNode == null ? xmlDoc.CreateElement("Query") : queryNode;

            if (qNode == null)
                newQueryNode.InnerXml = string.Format("{0}{1}", queryXml, newQueryNode.InnerXml);
            else
                newQueryNode.InnerXml = string.Format("{0}{1}", qNode.InnerXml, newQueryNode.InnerXml);

            XmlNode firstChildNode = viewNode.FirstChild;
            viewNode.InsertBefore(newQueryNode, firstChildNode);

            //Update ViewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }

        /// <summary>
        /// This Method will append the ViewFields xml to the Query
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        /// <param name="viewFieldsXml">ViewFields as xml string</param>
        public static void SetViewFields(this CamlQuery camlQuery, string viewFieldsXml)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            if (string.IsNullOrEmpty(viewFieldsXml)) return;

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            //Set ViewFields
            XmlNode node = GetOrCreateXmlNodeByXPath(VIEWFIELDS_XPATH, "ViewFields", viewFieldsXml, xmlDoc);

            xmlDoc.DocumentElement.InsertAfter(node, GetXmlNodeByXPath(QUERY_XPATH, xmlDoc));

            //Update viewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }


        /// <summary>
        /// This Method will append the ViewFields to the Query
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        /// <param name="viewFields">ViewFields As Array of string</param>
        public static void SetViewFields(this CamlQuery camlQuery, string[] viewFields)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            //Set ViewFields
            XmlNode node = GetOrCreateXmlNodeByXPath(VIEWFIELDS_XPATH, "ViewFields", CreateViewFieldsXml(viewFields), xmlDoc);

            xmlDoc.DocumentElement.InsertAfter(node, GetXmlNodeByXPath(QUERY_XPATH, xmlDoc));

            //Update viewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }

        /// <summary>
        /// This method will set the OrderBy index field
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        /// <param name="orderByIndex">true or false</param>
        public static void SetOrderByIndexField(this CamlQuery camlQuery)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            //get or create query node
            XmlNode QueryNode = GetXmlNodeByXPath(QUERY_XPATH, xmlDoc);

            if (QueryNode == null)
            {
                QueryNode = xmlDoc.CreateElement("Query");
                XmlNode viewNode = GetXmlNodeByXPath(VIEW_XPATH, xmlDoc);
                viewNode.InsertBefore(QueryNode, viewNode.FirstChild);
            }

            //Set orderByIndex field
            OverrideOrderByField(QueryNode, ORDER_BY_NVPFIELD);

            //Update ViewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }

        /// <summary>
        /// This method will set the OrderBy ID field which is indexed by default 
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        public static void SetOrderByIDField(this CamlQuery camlQuery)
        {
            SetOrderByIDField(camlQuery, true);
        }

        /// <summary>
        /// This method will set the OrderBy index field or ID field which is indexed by default 
        /// </summary>
        /// <param name="camlQuery"></param>
        /// <param name="orderByIndex"></param>
        /// <param name="sortAsc"></param>
        public static void SetOrderByIDField(this CamlQuery camlQuery, bool sortAsc)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            XmlNode QueryNode = GetXmlNodeByXPath(QUERY_XPATH, xmlDoc);

            if (QueryNode == null)
            {
                QueryNode = xmlDoc.CreateElement("Query");
                XmlNode viewNode = GetXmlNodeByXPath(VIEW_XPATH, xmlDoc);
                viewNode.InsertBefore(QueryNode, viewNode.FirstChild);
            }

            //Set orderBy ID field
            if (sortAsc)
                OverrideOrderByField(QueryNode, ORDER_BY_ID);
            else
                OverrideOrderByField(QueryNode, ORDER_BY_ID_DESC);


            //Update ViewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }

        /// <summary>
        /// This method will set the Query row limit
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        /// <param name="rowLimit">rowlimit</param>
        public static void SetQueryRowlimit(this CamlQuery camlQuery, uint rowLimit)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            //Set Rowlimit
            GetOrCreateXmlNodeByXPath(ROWLIMIT_XPATH, "RowLimit", rowLimit.ToString(), xmlDoc);

            //Update viewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }

        /// <summary>
        /// This method will set the QueryThrottle Mode for ListViewThreshold exception
        /// </summary>
        /// <param name="camlQuery">CamlQuery</param>
        /// <param name="queryThrottleMode">ThrottleMode option</param>
        public static void SetQueryThrottleMode(this CamlQuery camlQuery, QueryThrottleMode queryThrottleMode)
        {
            //if Query ViewXml is Empty then create View root Node
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
                camlQuery.ViewXml = string.Format(VIEW_QUERY, DefaultMaxItemsPerThrottledOperation.ToString());

            //Load ViewXml
            XmlDocument xmlDoc = LoadViewXml(camlQuery.ViewXml);

            //Get or Create RowLimit node from ViewXml
            OverrideQueryThrottleMode(xmlDoc, GetXmlNodeByXPath(VIEW_XPATH, xmlDoc), queryThrottleMode);

            //Update viewXml
            UpdateCamlQuery(camlQuery, xmlDoc);
        }


        /// <summary>
        /// This method will create ml document from string
        /// </summary>
        /// <param name="viewXml">viewXml string</param>
        /// <returns></returns>
        private static XmlDocument LoadViewXml(string viewXml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            using (XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(viewXml)))
            {
                xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
                xmlDoc.Load(xmlTextReader);
            }

            return xmlDoc;
        }

        /// <summary>
        /// this method will return Xml node by Specified xpath
        /// </summary>
        /// <param name="path">XPath to find the Xmlnode</param>
        /// <param name="xmlDoc">Xml Document</param>
        /// <returns>Xml Node</returns>
        private static XmlNode GetXmlNodeByXPath(string path, XmlDocument xmlDoc)
        {
            return xmlDoc.DocumentElement.SelectSingleNode(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private static XmlNode GetOrCreateRowLimitNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(ROWLIMIT_XPATH);
            if (node == null)
            {
                //Create node
                node = xmlDoc.CreateNode("element", "RowLimit", "");
                node.InnerText = DefaultMaxItemsPerThrottledOperation.ToString();

                XmlNode viewNode = GetXmlNodeByXPath(VIEW_XPATH, xmlDoc);
                viewNode.InsertBefore(node, viewNode.LastChild);
            }

            return node;
        }

        /// <summary>
        /// This Method will create specified node if it doesn't exists, else return XmlNode
        /// </summary>
        /// <param name="path">XPath to find the Node in Xml Document</param>
        /// <param name="name">Name of the new node to create.</param>
        /// <param name="value">Value for newly create node</param>
        /// <param name="xmlDoc">Xml document</param>
        /// <returns>Xml Node</returns>
        private static XmlNode GetOrCreateXmlNodeByXPath(string path, string name, string value, XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(path);
            if (node == null)
            {
                //Create node
                node = xmlDoc.CreateNode("element", name, "");
                node.InnerXml = value;

            }
            else
                node.InnerXml = value;

            return node;
        }

        /// <summary>
        /// This mehtod will create Scope attribute for Query
        /// </summary>
        /// <param name="xmlDoc">View Xml as Xml document</param>
        /// <param name="node">View root Node</param>
        /// <param name="value">Scope value</param>
        private static void CreateScopeAttributeWithValue(XmlDocument xmlDoc, XmlNode node, string value)
        {
            if (node.Attributes.GetNamedItem("Scope") == null)
            {
                //Create scope Attribute
                XmlAttribute attribute = xmlDoc.CreateAttribute("Scope");
                attribute.Value = value;
                node.Attributes.Append(attribute);
            }
            else
            {
                node.Attributes.GetNamedItem("Scope").Value = value;
            }

        }

        /// <summary>
        /// This method will insert the fieldml into query
        /// </summary>
        /// <param name="node">xml node</param>
        /// <param name="fieldXml">order field as xml</param>
        private static void OverrideOrderByField(XmlNode node, string fieldXml)
        {
            node.InnerXml = node.InnerXml + fieldXml;
        }

        /// <summary>
        /// This method will override the QueryThrottle mode in QueryOptions of CamlQuery
        /// </summary>
        /// <param name="xmlDoc">Xml Document</param>
        /// <param name="node">Xml Node</param>
        /// <param name="queryThrottleMode">QueryThrottle Mode option</param>
        private static void OverrideQueryThrottleMode(XmlDocument xmlDoc, XmlNode node, QueryThrottleMode queryThrottleMode)
        {
            //Create QueryOptions node
            XmlNode queryThrottleNode = xmlDoc.CreateNode("element", "QueryOptions", "");
            queryThrottleNode.InnerXml = string.Format(QUERY_THROTTLE_MODE, queryThrottleMode);
            xmlDoc.DocumentElement.InsertAfter(queryThrottleNode, node.LastChild);

        }

        /// <summary>
        /// This Method will create the ViewFields xml from sting array
        /// </summary>
        /// <param name="fields">fields string array</param>
        /// <returns>ViewFields as xml string</returns>
        private static string CreateViewFieldsXml(string[] fields)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string field in fields)
                builder.Append(string.Format("<FieldRef Name='{0}'/>", field));

            return builder.ToString();
        }

        /// <summary>
        /// Update the VewXml with content from CAMLQuery extension methods
        /// </summary>
        /// <param name="camlQuery">CAMLQuery object</param>
        /// <param name="xmlDoc">Converted ViewXml document</param>
        private static void UpdateCamlQuery(CamlQuery camlQuery, XmlDocument xmlDoc)
        {
            camlQuery.ViewXml = xmlDoc.DocumentElement.OuterXml;
        }
    }
}
