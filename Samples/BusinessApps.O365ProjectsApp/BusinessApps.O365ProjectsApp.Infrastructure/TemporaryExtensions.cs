using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure
{
    public static class TemporaryExtensions
    {
        /// <summary>
        /// Returns all custom actions in a list
        /// </summary>
        /// <param name="list">The list to process</param>
        /// <returns></returns>
        public static IEnumerable<UserCustomAction> GetCustomActions(this List list)
        {
            var clientContext = (ClientContext)list.Context;

            List<UserCustomAction> actions = new List<UserCustomAction>();

            clientContext.Load(list.UserCustomActions);
            clientContext.ExecuteQueryRetry();

            foreach (UserCustomAction uca in list.UserCustomActions)
            {
                actions.Add(uca);
            }
            return actions;
        }

        /// <summary>
        /// Returns a custom actions in a web
        /// </summary>
        /// <param name="list">The list to process</param>
        /// <param name="name">The name of the custom action</param>
        /// <returns></returns>
        public static UserCustomAction GetCustomAction(this List list, String name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            list.Context.Load(list.UserCustomActions);
            list.Context.ExecuteQueryRetry();

            var customActions = list.UserCustomActions.AsEnumerable<UserCustomAction>();
            foreach (var customAction in customActions)
            {
                var customActionName = customAction.Name;
                if (!string.IsNullOrEmpty(customActionName) &&
                    customActionName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return(customAction);
                }
            }

            return (null);
        }


        /// <summary>
        /// Utility method to check particular custom action already exists on the list
        /// </summary>
        /// <param name="site">The target list</param>
        /// <param name="name">Name of the custom action</param>
        /// <returns></returns>        
        public static bool CustomActionExists(this List list, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            list.Context.Load(list.UserCustomActions);
            list.Context.ExecuteQueryRetry();

            var customActions = list.UserCustomActions.AsEnumerable<UserCustomAction>();
            foreach (var customAction in customActions)
            {
                var customActionName = customAction.Name;
                if (!string.IsNullOrEmpty(customActionName) &&
                    customActionName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a custom action
        /// </summary>
        /// <param name="site">The site to process</param>
        /// <param name="id">The id of the action to remove. <seealso>
        ///         <cref>GetCustomActions</cref>
        ///     </seealso>
        /// </param>
        public static void DeleteCustomAction(this List list, Guid id)
        {
            var clientContext = (ClientContext)list.Context;

            clientContext.Load(list.UserCustomActions);
            clientContext.ExecuteQueryRetry();

            foreach (UserCustomAction action in list.UserCustomActions)
            {
                if (action.Id == id)
                {
                    action.DeleteObject();
                    clientContext.ExecuteQueryRetry();
                    break;
                }
            }
        }
    }
}