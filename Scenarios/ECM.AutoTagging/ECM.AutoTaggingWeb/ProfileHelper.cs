using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECM.AutoTaggingWeb
{
    /// <summary>
    /// Helper Class to Retrieve User Profile Properties
    /// </summary>
    public static class ProfileHelper
    {
        /// <summary>
        /// Gets a user profile property Value for the specified user.
        /// </summary>
        /// <param name="ctx">An Authenticated ClientContext</param>
        /// <param name="userName">The name of the target user.</param>
        /// <param name="propertyName">The value of the property to get.</param>
        /// <returns><see cref="System.String"/>The specified profile property for the specified user. Will return an Empty String if the property is not available.</returns>
        public static string GetProfilePropertyFor(ClientContext ctx, string userName, string propertyName)
        {
            string _result = string.Empty;
            if (ctx != null)
            {
                try
                {
                    //// PeopleManager class provides the methods for operations related to people
                    PeopleManager peopleManager = new PeopleManager(ctx);
                    //// GetUserProfilePropertyFor method is used to get a specific user profile property for a user
                    var _profileProperty = peopleManager.GetUserProfilePropertyFor(userName, propertyName);
                    ctx.ExecuteQuery();
                    _result = _profileProperty.Value;
                }
                catch
                {
                    throw;
                }
            }
            return _result;
        }
    }
}