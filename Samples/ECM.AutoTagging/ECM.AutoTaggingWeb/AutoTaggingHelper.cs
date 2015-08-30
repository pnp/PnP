using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECM.AutoTaggingWeb
{
    /// <summary>
    /// AutoTagging Helper Class
    /// </summary>
    public class AutoTaggingHelper
    {
        //Query to find the item in the Taxonomy List to get the WssID
        private const string TAXONOMY_CAML_QRY = "<View><Query><Where><Eq><FieldRef Name='Title'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>";
        private const string TAXONOMY_FORMATED_STRING = "{0};#{1}|{2}";
        private const string TAXONOMY_HIDDEN_LIST_NAME = "TaxonomyHiddenList";
        private const string TITLE_FIELD = "Title";
        private const string TAXONOMY_FIELDS_IDFORTERM = "IdForTerm";
        private const string EXCEPTION_MSG_INVALID_ARG = "{The arguement {0}, is invalid or not supplied.";

        /// <summary>
        /// Helper Method to set a Taxonomy Field on a list item
        /// </summary>
        /// <param name="ctx">The Authenticated ClientContext</param>
        /// <param name="listItem">The listitem to modify</param>
        /// <param name="model">Domain Object of key/value pairs of the taxonomy field & value</param>
        public static void SetTaxonomyField(ClientContext ctx, ListItem listItem, Hashtable model)
        {
          
            FieldCollection _fields = listItem.ParentList.Fields;
            ctx.Load(_fields);
            ctx.ExecuteQuery();

            foreach(var _key in model.Keys)
            {
               var _termName = model[_key].ToString();
               TaxonomyField _field = ctx.CastTo<TaxonomyField>(_fields.GetByInternalNameOrTitle(_key.ToString()));
               ctx.Load(_field);
               ctx.ExecuteQuery();
               Guid _id = _field.TermSetId;
               string _termID = AutoTaggingHelper.GetTermIdByName(ctx, _termName, _id );
               var _termValue = new TaxonomyFieldValue()
               {
                   Label = _termName,
                   TermGuid = _termID,
                   WssId = -1
               };

               _field.SetFieldValueByValue(listItem, _termValue);
               listItem.Update();
               ctx.ExecuteQuery();
            }
        }

        /// <summary>
        /// Helper Methods to get a TermId by a Name
        /// </summary>
        /// <param name="ctx">The Authenticated ClientContext</param>
        /// <param name="term">The Term Name do lookup.</param>
        /// <param name="termSetId">The TermSet Guid</param>
        /// <returns></returns>
        public static string GetTermIdByName(ClientContext ctx, string term, Guid termSetId)
        {
            string _resultTerm = string.Empty;

            var _taxSession = TaxonomySession.GetTaxonomySession(ctx);
            var _termStore = _taxSession.GetDefaultSiteCollectionTermStore();
            var _termSet = _termStore.GetTermSet(termSetId);

            var _termMatch = new LabelMatchInformation(ctx)
            {
               Lcid = 1033,
               TermLabel = term,     
               TrimUnavailable = true
            };

            var _termCollection = _termSet.GetTerms(_termMatch);
            ctx.Load(_taxSession);
            ctx.Load(_termStore);
            ctx.Load(_termSet);
            ctx.Load(_termCollection);
            ctx.ExecuteQuery();

            if (_termCollection.Count() > 0)
                _resultTerm = _termCollection.First().Id.ToString();

            return _resultTerm;

        }

        /// <summary>
        /// Gets the WssId of the Term
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="term"></param>
        /// <returns>Returns the WssId for the supplied term</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static int GetWssId(ClientContext ctx, string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                throw new ArgumentException(string.Format(EXCEPTION_MSG_INVALID_ARG, "term"));
            }
            int _wssId = -1;
            var _list = ctx.Web.Lists.GetByTitle(TAXONOMY_HIDDEN_LIST_NAME);
            CamlQuery _caml = new CamlQuery();
            _caml.ViewXml = string.Format(TAXONOMY_CAML_QRY, term);
            var _listItemCollection = _list.GetItems(_caml);

            ctx.Load(_listItemCollection,
                eachItem => eachItem.Include(
                    item => item[TITLE_FIELD],
                    item => item.Id,
                    item => item[TAXONOMY_FIELDS_IDFORTERM]));
            ctx.ExecuteQuery();

            if(_listItemCollection.Count > 0)
            {
                var _item = _listItemCollection.FirstOrDefault();
                _wssId = _item.Id;
            }

            return _wssId;         

        }

        /// <summary>
        /// This will return the format of a Taxonomy value. 
        /// If the Term is not found this method will return an <see cref="string.Empty"/>
        /// <example>"2;#MYTERNNAME|74972ac9-3183-4775-b232-cd6de3569c65"</example>
        /// </summary>
        /// <param name="ctx">The Authenticated ClientContext</param>
        /// <param name="term"></param>
        /// <returns>Taxonomy Formatted string</returns>
        public static string GetTaxonomyFormat(ClientContext ctx, string term)
        { 
            if(string.IsNullOrEmpty(term))
            {
                throw new ArgumentException(string.Format(EXCEPTION_MSG_INVALID_ARG, "term"));
            }
            string _result = string.Empty;
            var _list = ctx.Web.Lists.GetByTitle(TAXONOMY_HIDDEN_LIST_NAME);
            CamlQuery _caml = new CamlQuery();

            _caml.ViewXml = string.Format(TAXONOMY_CAML_QRY, term);
            var _listItemCollection = _list.GetItems(_caml);

            ctx.Load(_listItemCollection,
                eachItem => eachItem.Include(
                    item => item,
                    item => item.Id,
                    item => item[TAXONOMY_FIELDS_IDFORTERM]));
            ctx.ExecuteQuery();

            if (_listItemCollection.Count > 0)
            {
                var _item = _listItemCollection.FirstOrDefault();
                var _wssId = _item.Id;
                var _termId = _item[TAXONOMY_FIELDS_IDFORTERM].ToString(); ;
                _result = string.Format(TAXONOMY_FORMATED_STRING, _wssId, term, _termId);
            }

            return _result;
        }

        public TaxonomyFieldValue GetTaxonomyField(ClientContext ctx, ListItem listItem, string fieldName, string term)
        {
            FieldCollection _fields = listItem.ParentList.Fields;
            ctx.Load(_fields);
            ctx.ExecuteQuery();

            TaxonomyField _field = ctx.CastTo<TaxonomyField>(_fields.GetByInternalNameOrTitle(fieldName));
            ctx.Load(_field);
            ctx.ExecuteQuery();

            Guid _id = _field.TermSetId;
            string _termID = AutoTaggingHelper.GetTermIdByName(ctx, term, _id);

            var _termValue = new TaxonomyFieldValue()
            {
                Label = term,
                TermGuid = _termID,
                WssId = -1
            };

            return _termValue;


        }
    }
}