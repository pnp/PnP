using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    public class LightGraphUser
    {
        /// <summary>
        /// The unique ID of the entity
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Display Name of the user
        /// </summary>
        public String DisplayName;

        /// <summary>
        /// Mail of the user
        /// </summary>
        public String Mail;

        /// <summary>
        /// UPN for the user
        /// </summary>
        public String UserPrincipalName;
    }
}