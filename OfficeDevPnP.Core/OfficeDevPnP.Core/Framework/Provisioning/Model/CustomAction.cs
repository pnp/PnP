using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for custom actions  associated with a SharePoint list, Web site, or subsite.
    /// </summary>
    public partial class CustomAction
    {
        #region Private Members
        private int _rightsValue = 0;
        #endregion

        #region Properties
        
        public string CommandUIExtension { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom action.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the custom action.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies an implementation-specific value that determines the position of the custom action in the page.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the location of the custom action.
        /// A string that contains the location; for example, Microsoft.SharePoint.SiteSettings.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the display title of the custom action.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value that specifies an implementation-specific value that determines the order of the custom action that appears on the page.
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Gets or sets the value that specifies the permissions needed for the custom action.
        /// </summary>
        public BasePermissions Rights { get; set; }

        /// <summary>
        /// Gets or sets the value that specifies the permissions needed for the custom action.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.permissionkind.aspx"/>
        /// </summary>
        public int RightsValue {
            get
            {
                return this._rightsValue;
            }
            set 
            {
                this._rightsValue = value;
                BasePermissions _bp = new BasePermissions();
                if(Enum.IsDefined(typeof(PermissionKind), value))
                {
                    var _pk = (PermissionKind)value;
                    _bp.Set(_pk);
                    this.Rights = _bp;
                }
            }
        }

        public string RegistrationId { get; set; }

        public UserCustomActionRegistrationType RegistrationType { get; set; }

        public bool Remove { get; set; }

        /// <summary>
        /// Gets or sets the URL, URI, or ECMAScript (JScript, JavaScript) function associated with the action.
        /// </summary>
        public string Url { get; set; }

        public bool Enabled { get; set; }
        
        /// <summary>
        /// Gets or sets the value that specifies the ECMAScript to be executed when the custom action is performed.
        /// </summary>
        public string ScriptBlock { get; set; }
        
        /// <summary>
        /// Gets or sets the URL of the image associated with the custom action.
        /// </summary>
        public string ImageUrl { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies the URI of a file which contains the ECMAScript to execute on the page
        /// </summary>
        public string ScriptSrc { get; set; }
        #endregion
    }
}
