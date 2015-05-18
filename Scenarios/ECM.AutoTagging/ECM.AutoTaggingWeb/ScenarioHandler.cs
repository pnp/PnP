using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using Microsoft.SharePoint.Client.Taxonomy;

namespace ECM.AutoTaggingWeb
{
    /// <summary>
    /// Helper Class to create the necessary fields, Content Types, and libraries for the scenario
    /// </summary>
    public class ScenarioHandler
    {
        #region Profile Properties
        public const string UPA_CLASSIFICATION_PROPERTY = "Classification";
        #endregion

        #region Remote Event Receiver Names
        public const string AUTOTAGGING_ITEM_ADDED_RERNAME = "PnPAutoTaggingItemAdded";
        public const string AUTOTAGGING_ITEM_ADDING_RERNAME = "PnPAutoTaggingItemAddIng";
        #endregion

        /// COMMON 
        private const string CT_GROUP = "Contoso Content Types";
        private const string CT_DESC = "Create a new Document";
        private const string FIELDS_GROUP_NAME = "Contoso Columns";

        /// CONTENT TYPE IT DOCUMENT
        private const string ITDOCUMENT_CT_ID = "0x01005D4F34E4BE7F4B6892AEBE088EDD215E";
        private const string ITDOCUMENT_CT_NAME = "IT Document";

        /// CONTENT TYPE CONTOSO DOCUMENT
        private const string CONTOSODOCUMENT_CT_ID = "0x0100A112247905884D0DA49735433433A93C";
        private const string CONTOSODOCUMENT_CT_NAME = "Contoso Document";

        //FIELD BUSINESS UNIT
        private readonly Guid FLD_BUSINESS_UNIT_ID = new Guid("91AE1803-2F95-427F-97DB-5CE1652C07B0");
        public const string FLD_BUSINESS_UNIT_INTERNAL_NAME = "_BusinessUnit";
        private const string FLD_BUSINESS_UNIT_DISPLAY_NAME = "Business Unit";

        //FIELD CLASSIFICATION
        private readonly Guid FLD_CLASSIFICATION_ID = new Guid("D7A785FC-7974-4CBD-864C-AE0012E97A22");
        public const string FLD_CLASSIFICATION_INTERNAL_NAME = "_classification";
        private const string FLD_CLASSIFICATION_DISPLAY_NAME = "Classification";
        private const string TAXONOMY_GROUP = "Enterprise";
        private const string TAXONOMY_TERMSET_CLASSIFICATION_NAME = "Classification";

        /// <summary>
        /// Used to create a custom document library and Contoso Content type
        /// </summary>
        /// <param name="ctx">The authenticated ClientContext</param>
        /// <param name="library">The Library to create</param>
        public void CreateContosoDocumentLibrary(ClientContext ctx, Library library)
        {
            //Check the fields
            if (!ctx.Web.FieldExistsById(FLD_CLASSIFICATION_ID))
            {
                TermStore termStore = GetDefaultTermStore(ctx.Web);

                if (termStore == null)
                {
                    throw new NullReferenceException("The default term store is not available.");
                }

                // get the term group and term set
                TermGroup termGroup = termStore.Groups.GetByName(TAXONOMY_GROUP);
                TermSet termSet = termGroup.TermSets.GetByName(TAXONOMY_TERMSET_CLASSIFICATION_NAME);
                ctx.Load(termStore);
                ctx.Load(termSet);
                ctx.ExecuteQuery();

                TaxonomyFieldCreationInformation fldCreate = new TaxonomyFieldCreationInformation()
                {
                    Id = FLD_CLASSIFICATION_ID,
                    InternalName = FLD_CLASSIFICATION_INTERNAL_NAME,
                    DisplayName = FLD_CLASSIFICATION_DISPLAY_NAME,
                    Group = FIELDS_GROUP_NAME,
                    TaxonomyItem = termSet,                    
                };
                ctx.Web.CreateTaxonomyField(fldCreate);

            }

            //check the content type
            if (!ctx.Web.ContentTypeExistsById(CONTOSODOCUMENT_CT_ID))
            {
                ctx.Web.CreateContentType(CONTOSODOCUMENT_CT_NAME,
                                          CT_DESC, CONTOSODOCUMENT_CT_ID,
                                          CT_GROUP);
            }

            //associate fields to content types
            if (!ctx.Web.FieldExistsByNameInContentType(CONTOSODOCUMENT_CT_NAME, FLD_CLASSIFICATION_INTERNAL_NAME))
            {
                ctx.Web.AddFieldToContentTypeById(CONTOSODOCUMENT_CT_ID,
                                                  FLD_CLASSIFICATION_ID.ToString(),
                                                  false);
            }

            
            CreateLibrary(ctx, library, CONTOSODOCUMENT_CT_ID);
        }

        /// <summary>
        /// Creates a custom document library and IT Document Content Type
        /// </summary>
        /// <param name="ctx">The authenticated ClientContext</param>
        /// <param name="library">The Library  to create</param>
        public void CreateITDocumentLibrary(ClientContext ctx, Library library)
        {
            //Check the fields
            if (!ctx.Web.FieldExistsById(FLD_BUSINESS_UNIT_ID))
            {
                //ctx.Web.CreateField(FLD_BUSINESS_UNIT_ID,
                //                    FLD_BUSINESS_UNIT_INTERNAL_NAME,
                //                    FieldType.Text,
                //                    FLD_BUSINESS_UNIT_DISPLAY_NAME,
                //                    FIELDS_GROUP_NAME);
                FieldCreationInformation fldCreate = new FieldCreationInformation(FieldType.Text)
                {
                    Id = FLD_BUSINESS_UNIT_ID,
                    InternalName = FLD_BUSINESS_UNIT_INTERNAL_NAME,
                    DisplayName = FLD_BUSINESS_UNIT_DISPLAY_NAME,
                    Group = FIELDS_GROUP_NAME
                };
                ctx.Web.CreateField(fldCreate);

            }
            //check the content type
            if (!ctx.Web.ContentTypeExistsById(ITDOCUMENT_CT_ID))
            {
                ctx.Web.CreateContentType(ITDOCUMENT_CT_NAME, CT_DESC, ITDOCUMENT_CT_ID, CT_GROUP);
            }

            //associate fields to content types
            if (!ctx.Web.FieldExistsByNameInContentType(ITDOCUMENT_CT_NAME, FLD_BUSINESS_UNIT_INTERNAL_NAME))
            {
                ctx.Web.AddFieldToContentTypeById(ITDOCUMENT_CT_ID, FLD_BUSINESS_UNIT_ID.ToString(), false);
            }

            CreateLibrary(ctx, library, ITDOCUMENT_CT_ID);
        }

        /// <summary>
        /// Returns a collection Of Content Types names
        /// </summary>
        /// <returns></returns>
        public IList<String> GetContentTypesName()
        {
            IList<string> _contentTypes = new List<string>();
            _contentTypes.Add(ITDOCUMENT_CT_NAME);
            _contentTypes.Add(CONTOSODOCUMENT_CT_NAME);
            return _contentTypes;
        }

        /// <summary>
        /// Helper Class to create document libraries
        /// </summary>
        /// <param name="ctx">The ClientContext, that must be valide</param>
        /// <param name="library">Domain Object for the Library</param>
        /// <param name="associateContentTypeID">The Content Type ID to add to the list.</param>
        private void CreateLibrary(ClientContext ctx, Library library, string associateContentTypeID)
        {
            if (!ctx.Web.ListExists(library.Title))
            {
                ctx.Web.CreateList(ListTemplateType.DocumentLibrary, library.Title, false);
                List _list = ctx.Web.GetListByTitle(library.Title);
                if (!string.IsNullOrEmpty(library.Description))
                {
                    _list.Description = library.Description;
                }

                if (library.VerisioningEnabled)
                {
                    _list.EnableVersioning = true;
                }

                _list.ContentTypesEnabled = true;
                _list.RemoveContentTypeByName("Document");
                _list.Update();
                
     
                ctx.Web.AddContentTypeToListById(library.Title, associateContentTypeID, true);
                ctx.Web.Context.ExecuteQuery();
               
            }
            else
            {
                throw new Exception("A list, survey, discussion board, or document library with the specified title already exists in this Web site.  Please choose another title.");
            }
        }

        private TermStore GetDefaultTermStore(Web web)
        {
            TermStore termStore = null;
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(web.Context);
            web.Context.Load(taxonomySession,
                ts => ts.TermStores.Include(
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name
                        )
                    )
                );
            web.Context.ExecuteQueryRetry();
            if (taxonomySession != null)
            {
                termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            }

            return termStore;
        }
    }

    /// <summary>
    /// Domain Object for Creating a library
    /// </summary>
    public class Library
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool VerisioningEnabled { get; set; }

    }
}