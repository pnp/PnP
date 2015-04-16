using ECM.DocumentLibrariesWeb.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECM.DocumentLibrariesWeb
{
    public class ContentTypeManager
    {
        private const string DEFAULT_DOCUMENT_CT_NAME = "Document";
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
        private const string FLD_BUSINESS_UNIT_INTERNAL_NAME = "_BusinessUnit";
        private const string FLD_BUSINESS_UNIT_DISPLAY_NAME = "Business Unit";

        //FIELD CLASSIFICATION
        private readonly Guid FLD_CLASSIFICATION_ID = new Guid("D7A785FC-7974-4CBD-864C-AE0012E97A22");
        private const string FLD_CLASSIFICATION_INTERNAL_NAME = "_classification";
        private const string FLD_CLASSIFICATION_DISPLAY_NAME = "Classification";
        private const string TAXONOMY_GROUP = "Enterprise";
        private const string TAXONOMY_TERMSET_CLASSIFICATION_NAME = "Classification";

        /// <summary>
        /// Used to create a custom document library and Contoso Content type
        /// </summary>
        /// <param name="ctx">The client context that has be authenticated</param>
        /// <param name="library">The Library to create</param>
        public void CreateContosoDocumentLibrary(ClientContext ctx, Library library)
        {
            //Check the fields
            if (!ctx.Web.FieldExistsById(FLD_CLASSIFICATION_ID)){

                // Get access to the right term set
                TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(ctx.Web.Context);
                TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
                TermGroup termGroup = termStore.Groups.GetByName(TAXONOMY_GROUP);
                TermSet termSet = termGroup.TermSets.GetByName(TAXONOMY_TERMSET_CLASSIFICATION_NAME);
                ctx.Web.Context.Load(termStore);
                ctx.Web.Context.Load(termSet);
                ctx.Web.Context.ExecuteQueryRetry();

                TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
                {
                    Id = FLD_CLASSIFICATION_ID,
                    InternalName = FLD_CLASSIFICATION_INTERNAL_NAME,
                    DisplayName = FLD_CLASSIFICATION_DISPLAY_NAME,
                    Group = FIELDS_GROUP_NAME,
                    TaxonomyItem = termSet
                };
                ctx.Web.CreateTaxonomyField(fieldCI);
            }
            
            //check the content type
            if (!ctx.Web.ContentTypeExistsById(CONTOSODOCUMENT_CT_ID)){
                ctx.Web.CreateContentType(CONTOSODOCUMENT_CT_NAME, 
                                          CT_DESC, CONTOSODOCUMENT_CT_ID, 
                                          CT_GROUP);
            }

            //associate fields to content types
            if (!ctx.Web.FieldExistsByNameInContentType(CONTOSODOCUMENT_CT_NAME, FLD_CLASSIFICATION_INTERNAL_NAME)){
                ctx.Web.AddFieldToContentTypeByName(CONTOSODOCUMENT_CT_NAME, 
                                                    FLD_CLASSIFICATION_ID);
            }
            CreateLibrary(ctx, library, CONTOSODOCUMENT_CT_ID);
          
        }
       
        /// <summary>
        /// Creates a custom document library and IT Document Content Type
        /// </summary>
        /// <param name="ctx">The client context that has be authenticated</param>
        /// <param name="library">The Library  to create</param>
        public void CreateITDocumentLibrary(ClientContext ctx, Library library)
        {
            //Check the fields
            if (!ctx.Web.FieldExistsById(FLD_BUSINESS_UNIT_ID)){
                FieldCreationInformation field = new FieldCreationInformation(FieldType.Text)
                {
                    Id = FLD_BUSINESS_UNIT_ID,
                    InternalName = FLD_BUSINESS_UNIT_INTERNAL_NAME,
                    DisplayName = FLD_BUSINESS_UNIT_DISPLAY_NAME,
                    Group = FIELDS_GROUP_NAME
                };
                ctx.Web.CreateField(field);
            }
            //check the content type
            if (!ctx.Web.ContentTypeExistsById(ITDOCUMENT_CT_ID)) {
                ctx.Web.CreateContentType(ITDOCUMENT_CT_NAME, CT_DESC, ITDOCUMENT_CT_ID, CT_GROUP);
            }

            //associate fields to content types
            if (!ctx.Web.FieldExistsByNameInContentType(ITDOCUMENT_CT_NAME, FLD_BUSINESS_UNIT_INTERNAL_NAME)){
                ctx.Web.AddFieldToContentTypeByName(ITDOCUMENT_CT_NAME, FLD_BUSINESS_UNIT_ID);
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
                if(!string.IsNullOrEmpty(library.Description)) {
                    _list.Description = library.Description;
                }

                if(library.VerisioningEnabled) {
                    _list.EnableVersioning = true;
                }

                _list.ContentTypesEnabled = true;
                _list.Update();
                // Add content type tot eh list
                ctx.Web.AddContentTypeToListById(library.Title, associateContentTypeID);
                //Set default content type as with the one which we jus added
                ctx.Web.SetDefaultContentTypeToList(library.Title, associateContentTypeID);
                ctx.Web.Context.ExecuteQuery();
            }
            else
            {
                throw new Exception("A list, survey, discussion board, or document library with the specified title already exists in this Web site.  Please choose another title.");
            }
        }
    }
}