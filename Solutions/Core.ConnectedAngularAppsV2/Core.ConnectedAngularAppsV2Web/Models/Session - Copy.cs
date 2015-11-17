using CorporateEvents;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.ConnectedSigRAngularJSApps.Models
{
    public class Session : BaseListItem {
        internal const string FIELD_DESCRIPTION = "SessionDescription";
        internal const string FIELD_DATE = "SessionDate";
        internal const string FIELD_ID = "SessionID";
        internal const string FIELD_IMAGEURL = "SessionImageUrl";
        internal const string FIELD_STATUS = "SessionStatus";
        internal const string FIELD_REGISTEREDEVENTID = "RegisteredEventID";
        internal const string FIELD_SPEAKERID = "SpeakerID";

        public Session() : base() { }
        public Session(ListItem item) : base(item) {
            Initialize(item);
        }

        protected override string ListTitle {
            get { return ListDetails.SessionsListName; }
        }

        protected override string ContentTypeName {
            get { return ContentTypes.EventSession; }
        }

        protected override string[] FieldInternalNames {
            get {
                return new string[]{
                };
            }
        }

        protected override void SetProperties(Microsoft.SharePoint.Client.ListItem item) {
            
        }

        protected override void ReadProperties(Microsoft.SharePoint.Client.ListItem item) {
            
        }
    }
}