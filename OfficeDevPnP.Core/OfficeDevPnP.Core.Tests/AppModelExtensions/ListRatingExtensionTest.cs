using System;
using System.Linq;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Tests;

namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class ListRatingExtensionTest
    {
        //SharePoint Server Publishing Infrastructure - Site
        private const string PublishingSiteFeature = "f6924d36-2fa8-4f0b-b16d-06b7250180fa";
        //SharePoint Server Publishing - Web
        private const string PublishingWebFeature = "94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb";
        
        private const string Averagerating = "AverageRating";
        private const string Ratedby = "RatedBy";
        private const string Ratingcount = "RatingCount";
        private const string Likescount = "LikesCount";
        private const string Ratings = "Ratings";
        private const string Likedby = "LikedBy";
        private const string RatingsVotingexperience = "Ratings_VotingExperience";

        private ClientContext _clientContext;
        private List _list;

        #region Test initialize and cleanup
        
        [TestInitialize()]
        public void Initialize()
        {
            /*** Make sure that the user defined in the App.config has permissions to Manage Terms ***/
            _clientContext = TestCommon.CreateClientContext();

            // Create Simple List
            _list = _clientContext.Web.CreateList(ListTemplateType.Contacts, "Test_list_" + DateTime.Now.ToFileTime(), false);
            _clientContext.Load(_list);
            _clientContext.ExecuteQueryRetry();

        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up list
            _list.DeleteObject();
            _clientContext.ExecuteQueryRetry();
        }
        #endregion

        #region Rating's Test Scenarios
        
        [TestMethod()]
        [ExpectedException(typeof(NotPublishingWebException))]
        public void Enable_Rating_On_Non_Publishing_Web_Expect_Exception()
        {
             _list.SetRating(VotingExperience.Ratings);
        }

        [TestMethod()]
        public void Enable_Rating_Experience()
        {
            // Enable Publishing Feature on Site and Web 

            if(!_clientContext.Site.IsFeatureActive(new Guid(PublishingSiteFeature)))
                _clientContext.Site.ActivateFeature(new Guid(PublishingSiteFeature));

            if (!_clientContext.Web.IsFeatureActive(new Guid(PublishingWebFeature)))
                _clientContext.Web.ActivateFeature(new Guid(PublishingWebFeature));

            _list.SetRating(VotingExperience.Ratings);

            //  Check if the Rating Fields are added to List, Views and Root Folder Property 

            Assert.IsTrue(IsRootFolderPropertySet(), "Root Folder property not set");
            Assert.IsTrue(HasRatingFields(), "Missing Rating Fields in List.");
            Assert.IsTrue(RatingFieldSetOnDefaultView(), "Required rating fields not added to default view.");

        }

        [TestMethod()]
        public void Enable_Likes_Experience()
        {
            // Enable Publishing Feature on Site and Web 

            if (!_clientContext.Site.IsFeatureActive(new Guid(PublishingSiteFeature)))
                _clientContext.Site.ActivateFeature(new Guid(PublishingSiteFeature));

            if (!_clientContext.Web.IsFeatureActive(new Guid(PublishingWebFeature)))
                _clientContext.Web.ActivateFeature(new Guid(PublishingWebFeature));

            _list.SetRating(VotingExperience.Likes);

            //  Check if the Rating Fields are added to List, Views and Root Folder Property 

            Assert.IsTrue(IsRootFolderPropertySet(VotingExperience.Likes), "Required Root Folder property not set.");
            Assert.IsTrue(HasRatingFields(), "Missing Rating Fields in List.");
            Assert.IsTrue(RatingFieldSetOnDefaultView(VotingExperience.Likes), "Required rating fields not added to default view.");

        }

        #endregion


        /// <summary>
        /// Validate if required experience selected fields are added to default view
        /// </summary>
        /// <param name="experience"></param>
        /// <returns></returns>
        private bool RatingFieldSetOnDefaultView(VotingExperience experience = VotingExperience.Ratings)
        {
            _clientContext.Load(_list.DefaultView.ViewFields);
            _clientContext.ExecuteQueryRetry();

            switch (experience)
            {
                case VotingExperience.Ratings:
                    return _list.DefaultView.ViewFields.Contains(Averagerating);
                case VotingExperience.Likes:
                    return _list.DefaultView.ViewFields.Contains(Likescount);
                default:
                    throw new ArgumentOutOfRangeException("experience");
            }
        }

        /// <summary>
        /// Validates if required rating fields are present in the list.
        /// </summary>
        /// <returns></returns>
        private bool HasRatingFields()
        {
            _clientContext.Load(_list.Fields, p => p.Include(prop => prop.InternalName));
            _clientContext.ExecuteQueryRetry();

            var avgRating = _list.Fields.FirstOrDefault(p => p.InternalName == Averagerating);
            var ratedBy = _list.Fields.FirstOrDefault(p => p.InternalName == Ratedby);
            var ratingCount = _list.Fields.FirstOrDefault(p => p.InternalName == Ratingcount);
            var likeCount = _list.Fields.FirstOrDefault(p => p.InternalName == Likescount);
            var ratings = _list.Fields.FirstOrDefault(p => p.InternalName == Ratings);
            var likedBy = _list.Fields.FirstOrDefault(p => p.InternalName == Likedby);

            var fieldsExist = avgRating != null && ratedBy != null && ratingCount != null && ratings != null &&
                              likeCount != null && likedBy != null;

            return fieldsExist;
        }


        /// <summary>
        /// Validate if the RootFolder property is set appropriately
        /// </summary>
        /// <returns></returns>
        private bool IsRootFolderPropertySet(VotingExperience experience = VotingExperience.Ratings)
        {
            _clientContext.Load(_list.RootFolder.Properties);
            _clientContext.ExecuteQueryRetry();

            if (_list.RootFolder.Properties.FieldValues.ContainsKey(RatingsVotingexperience))
            {
                object exp;
                if (_list.RootFolder.Properties.FieldValues.TryGetValue(RatingsVotingexperience, out exp))
                {
                    return string.Compare(exp.ToString(),experience.ToString(),StringComparison.InvariantCultureIgnoreCase) == 0;    
                }
            }

            return false;
        }

        

    }
}