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

        private Guid _listId; // For easy reference

        private static readonly Guid RatingsFieldGuid_AverageRating = new Guid("5a14d1ab-1513-48c7-97b3-657a5ba6c741"); //2
        private static readonly Guid RatingsFieldGuid_RatingCount = new Guid("b1996002-9167-45e5-a4df-b2c41c6723c7");
        private static readonly Guid RatingsFieldGuid_RatedBy = new Guid("4D64B067-08C3-43DC-A87B-8B8E01673313");
        private static readonly Guid RatingsFieldGuid_Ratings = new Guid("434F51FB-FFD2-4A0E-A03B-CA3131AC67BA");
        private static readonly Guid LikeFieldGuid_LikedBy = new Guid("2cdcd5eb-846d-4f4d-9aaf-73e8e73c7312");
        private static readonly Guid LikeFieldGuid_LikeCount = new Guid("6e4d832b-f610-41a8-b3e0-239608efda41");
        private ClientContext _clientContext;
        private List _list;

        #region Test initialize and cleanup
        [TestInitialize()]
        public void Initialize()
        {
            /*** Make sure that the user defined in the App.config has permissions to Manage Terms ***/
            _clientContext = TestCommon.CreateClientContext();

            // Create Simple List
            var list = _clientContext.Web.CreateList(ListTemplateType.Contacts, "Test_list_" + DateTime.Now.ToFileTime(), false);
            _clientContext.Load(list);
            _clientContext.ExecuteQueryRetry();

            _listId = list.Id;

        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up list
            var list = _clientContext.Web.Lists.GetById(_listId);
            list.DeleteObject();
            _clientContext.ExecuteQueryRetry();
        }
        #endregion

        #region Enable Rating on List
        
        [TestMethod()]
        [ExpectedException(typeof(NotPublishingWebException))]
        public void EnableRating_On_Non_Publishing_Web_Expect_Exception()
        {
            _list = _clientContext.Web.Lists.GetById(_listId);
            
             _list.SetRating();

            //  Check if the Rating Fields are added to List, Views and Root Folder Property 

            Assert.IsTrue(IsRootFolderPropertySet());
            Assert.IsTrue(HasRatingFields());
            Assert.IsTrue(RatingFieldSetOnDefaultView());

            //Delete List
            _list.DeleteObject();
            _clientContext.ExecuteQueryRetry();

        }

        [TestMethod()]
        public void EnableRating()
        {
            // Enable Publishing Feature on Site and Web 

            if(!_clientContext.Site.IsFeatureActive(new Guid(PublishingSiteFeature)))
                _clientContext.Site.ActivateFeature(new Guid(PublishingSiteFeature));

            if (!_clientContext.Web.IsFeatureActive(new Guid(PublishingWebFeature)))
                _clientContext.Web.ActivateFeature(new Guid(PublishingWebFeature));

            _list = _clientContext.Web.Lists.GetById(_listId);

            _list.SetRating();

            //  Check if the Rating Fields are added to List, Views and Root Folder Property 

            Assert.IsTrue(IsRootFolderPropertySet());
            Assert.IsTrue(HasRatingFields());
            Assert.IsTrue(RatingFieldSetOnDefaultView());

            //Delete List
            _list.DeleteObject();
            _clientContext.ExecuteQueryRetry();

        }

        private bool RatingFieldSetOnDefaultView(VotingExperience experience = VotingExperience.Ratings)
        {
            _clientContext.Load(_list.DefaultView.ViewFields);
            _clientContext.ExecuteQueryRetry();

            switch (experience)
            {
                case VotingExperience.Ratings:
                    return _list.DefaultView.ViewFields.Contains("AverageRating");
                case VotingExperience.Likes:
                    return _list.DefaultView.ViewFields.Contains("LikesCount");
                default:
                    throw new ArgumentOutOfRangeException("experience");
            }
        }

        private bool HasRatingFields()
        {
            _clientContext.Load(_list.Fields,p=>p.Include(i=>i.Id));
            _clientContext.ExecuteQueryRetry();

            var avgRating = _list.Fields.GetById(RatingsFieldGuid_AverageRating);
            var ratedBy = _list.Fields.GetById(RatingsFieldGuid_RatedBy);
            var ratingCount = _list.Fields.GetById(RatingsFieldGuid_RatingCount);
            var ratings = _list.Fields.GetById(RatingsFieldGuid_Ratings);
            var likeCount = _list.Fields.GetById(LikeFieldGuid_LikeCount);
            var likedBy = _list.Fields.GetById(LikeFieldGuid_LikedBy);

            var fieldsExist = avgRating.Id.Equals(RatingsFieldGuid_AverageRating) &&
                             ratedBy.Id.Equals(RatingsFieldGuid_RatedBy) &&
                             ratingCount.Id.Equals(RatingsFieldGuid_RatingCount) &&
                             ratings.Id.Equals(RatingsFieldGuid_Ratings) &&
                             likeCount.Id.Equals(LikeFieldGuid_LikeCount) &&
                             likedBy.Id.Equals(LikeFieldGuid_LikedBy);

            return !fieldsExist;


        }

        private bool IsRootFolderPropertySet()
        {
            _clientContext.Load(_list.RootFolder.Properties);
            _clientContext.ExecuteQueryRetry();

            return _list.RootFolder.Properties.FieldValues.ContainsKey("Ratings_VotingExperience");
        }

        #endregion




    }
}