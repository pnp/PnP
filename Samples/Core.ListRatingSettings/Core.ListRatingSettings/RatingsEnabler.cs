using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.SharePoint.Client;

namespace Core.ListRatingSettings
{
    public enum VotingExperience
    {
        Ratings,
        Likes
    }

    public class RatingsEnabler 
    {

        private readonly ILogger _logger;
        private readonly ClientContext _context;
        private List _library;

        public RatingsEnabler(ClientContext context) : this(context, new ConsoleLogger())
        {
        }

        public RatingsEnabler(ClientContext context, ILogger logger)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (logger == null) throw new ArgumentNullException("logger");
            _context = context;
            _logger = logger;
        }


        #region Rating Field

        private readonly Guid RatingsFieldGuid_AverageRating = new Guid("5a14d1ab-1513-48c7-97b3-657a5ba6c742");
        private readonly Guid RatingsFieldGuid_RatingCount = new Guid("b1996002-9167-45e5-a4df-b2c41c6723c7");
        private readonly Guid RatingsFieldGuid_RatedBy = new Guid("4D64B067-08C3-43DC-A87B-8B8E01673313");
        private readonly Guid RatingsFieldGuid_Ratings = new Guid("434F51FB-FFD2-4A0E-A03B-CA3131AC67BA");
        private readonly Guid LikeFieldGuid_LikedBy = new Guid("2cdcd5eb-846d-4f4d-9aaf-73e8e73c7312");
        private readonly Guid LikeFieldGuid_LikeCount = new Guid("6e4d832b-f610-41a8-b3e0-239608efda41");

        #endregion

        /// <summary>
        /// Enable Social Settings Likes/Ratings on given list
        /// </summary>
        /// <param name="listName">List Name</param>
        /// <param name="experience">Likes/Ratings</param>
        public void Enable(string listName,VotingExperience experience)
        {
            /*  Get root Web
             *  Validate if current web is publishing web
             *  Find List/Library library 
             *  Add property to RootFolder of List/Library : key: Ratings_VotingExperience value:Likes
             *  Add rating fields
             *  Add fields to default view
             * */

            var web = _context.Site.RootWeb;
            _context.Load(_context.Site.RootWeb, p => p.Url);
            _context.ExecuteQuery();

            try
            {
                _logger.WriteInfo("Processing: " + web.Url);
                EnsureReputation(web, listName,experience);
            }
            catch (Exception e)
            {
                _logger.WriteException(string.Format("Error: {0} \nMessage: {1} \nStack: {2}", web.Url, e.Message, e.StackTrace));
            }
        }

        private void EnsureReputation(Web web,string listName,VotingExperience experience)
        {
            //  only process publishing web
            if (!IsPublishingWeb(web))
            {
                _logger.WriteWarning("Is publishing site : No");
                return;
            }

            _logger.WriteInfo("Is publishing site : Yes");

            SetCulture(GetWebCulture(web));

            //  Fetch Library
            try
            {
                _library = web.Lists.GetByTitle(listName);
                _context.Load(_library);
                _context.ExecuteQuery();

                _logger.WriteSuccess("Found list/library : " + _library.Title);
            }
            catch (ServerException e)
            {
                _logger.WriteException(string.Format("Error: {0} \nMessage: {1} \nStack: {2}", web.Url, e.Message, e.StackTrace));
                return;
            }

            //  Add to property Root Folder of Pages Library
            AddProperty(experience);

            AddListFields();

            AddViewFields(experience);

            _logger.WriteSuccess(string.Format("Enabled {0} on list/library {1}", experience,_library.Title));
        }

        /// <summary>
        /// Checks if the web is Publishing Web
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        private bool IsPublishingWeb(Web web)
        {
            _context.Load(web, p => p.AllProperties);
            _context.ExecuteQuery();

            return web.AllProperties.FieldValues.ContainsKey("__PublishingFeatureActivated");
        }

        /// <summary>
        /// Add Ratings/Likes related fields to List
        /// </summary>
        private void AddListFields()
        {
            //NOTE: The method returns the field, which can be used if required, for the sake of simplicity i removed it
            EnsureField(_library, RatingsFieldGuid_RatingCount);
            EnsureField(_library, RatingsFieldGuid_RatedBy);
            EnsureField(_library, RatingsFieldGuid_Ratings);
            EnsureField(_library, RatingsFieldGuid_AverageRating);
            EnsureField(_library, LikeFieldGuid_LikedBy);
            EnsureField(_library, LikeFieldGuid_LikeCount);

            _library.Update();
            _context.ExecuteQuery();

            _logger.WriteSuccess("Ensured fields in library.");

        }

        /// <summary>
        /// Add/Remove Ratings/Likes field in default view depending on exerpeince selected
        /// </summary>
        /// <param name="experience"></param>
        private void AddViewFields(VotingExperience experience)
        {
            //  Add LikesCount and LikeBy (Explicit) to view fields
            _context.Load(_library.DefaultView, p => p.ViewFields);
            _context.ExecuteQuery();

            var defaultView = _library.DefaultView;

            switch (experience)
            {
                case VotingExperience.Ratings:
                    //  Remove Likes Fields
                    if(defaultView.ViewFields.Contains("LikesCount"))
                        defaultView.ViewFields.Remove("LikesCount");
                    
                    defaultView.ViewFields.Add("AverageRating");
                    //  Add Ratings related field
                    break;
                case VotingExperience.Likes:
                    //  Remove Ratings Fields
                    //  Add Likes related field
                    if (defaultView.ViewFields.Contains("AverageRating"))
                        defaultView.ViewFields.Remove("AverageRating");
                    
                    defaultView.ViewFields.Add("LikesCount");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("experience");
            }

            defaultView.Update();
            _context.ExecuteQuery();
            _logger.WriteSuccess("Ensured view-field.");

        }

        /// <summary>
        /// Check for Ratings/Likes field and add to ListField if doesn't exists.
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="fieldId">Field Id</param>
        /// <returns></returns>
        private Field EnsureField(List list, Guid fieldId)
        {
            FieldCollection fields = list.Fields;

            FieldCollection availableFields = list.ParentWeb.AvailableFields;
            Field field = availableFields.GetById(fieldId);

            _context.Load(fields);
            _context.Load(field, p => p.SchemaXmlWithResourceTokens, p => p.Id, p => p.InternalName, p => p.StaticName);
            _context.ExecuteQuery();

            if (!fields.Any(p => p.Id == fieldId))
            {

                var newField = fields.AddFieldAsXml(field.SchemaXmlWithResourceTokens, false, AddFieldOptions.AddFieldInternalNameHint | AddFieldOptions.AddToAllContentTypes);
                return newField;
            }
            return field;
        }

        /// <summary>
        /// Add required key/value settings on List Root-Folder
        /// </summary>
        /// <param name="experience"></param>
        private void AddProperty(VotingExperience experience)
        {
            _context.Load(_library.RootFolder, p => p.Properties);
            _context.ExecuteQuery();

            _library.RootFolder.Properties["Ratings_VotingExperience"] = experience.ToString();
            _library.RootFolder.Update();
            _context.ExecuteQuery();
            _logger.WriteSuccess(string.Format("Ensured {0} Property.",experience));
        }

        private uint GetWebCulture(Web web)
        {
            _context.Load(web, p => p.Language);
            _context.ExecuteQuery();
            return web.Language;
        }

        private void SetCulture(uint culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(CultureInfo.GetCultureInfo((int)culture).ToString());
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(CultureInfo.GetCultureInfo((int)culture).ToString());
        }


    }
}