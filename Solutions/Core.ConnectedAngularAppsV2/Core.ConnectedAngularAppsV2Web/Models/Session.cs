using CorporateEvents;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Core.ConnectedAngularAppsV2Web.Models
{
    public class Session : BaseListItem {
        internal const string FIELD_SESSIONDESCRIPTION = "SessionDescription";
        internal const string FIELD_SESSIONDATE = "SessionDate";
        internal const string FIELD_SESSIONID = "SessionID";
        internal const string FIELD_SESSIONIMAGEURL = "SessionImageUrl";
        internal const string FIELD_SESSIONSTATUS = "SessionStatus";
        internal const string FIELD_EVENTID = "RegisteredEventID";
        internal const string FIELD_SESSIONSPEAKERID = "SpeakerID";

        public Session() : base() { }
        public Session(ListItem item) : base(item) {
            Initialize(item);
        }
        [Display(Name = "Session Id")]
        public string SessionId { get; set; }

        [DataType(DataType.MultilineText)]
        public string SessionDescription { get; set; }

        [Display(Name = "Session Date")]
        public DateTime? SessionDate { get; set; }
              
        public string Status { get; set; }

        [Display(Name = "Session Image Url")]
        public string SessionImageUrl { get; set; }

        public string ShortDescription { get { return SessionDescription; } }

        [Display(Name = "Event Id")]
        public string RegisteredEventId { get; set; }

        [Display(Name = "Speaker Id")]
        public string SpeakerId { get; set; }

        protected override void SetProperties(ListItem item)
        {
            BaseSet(item, FIELD_SESSIONID, SessionId);
            BaseSet(item, FIELD_SESSIONDESCRIPTION, SessionDescription);
            BaseSet(item, FIELD_SESSIONDATE, SessionDate);            
            BaseSet(item, FIELD_SESSIONSTATUS, Status);
            BaseSet(item, FIELD_SESSIONIMAGEURL, SessionImageUrl);
            BaseSet(item, FIELD_EVENTID, RegisteredEventId);
            BaseSet(item, FIELD_SESSIONSPEAKERID, SpeakerId);
        }

        protected override void ReadProperties(ListItem item)
        {
            SessionId = BaseGet<string>(item, FIELD_EVENTID);
            SessionDescription = BaseGet<string>(item, FIELD_SESSIONDESCRIPTION);           
            SessionDate = BaseGet<DateTime?>(item, FIELD_SESSIONDATE);            
            Status = BaseGet<string>(item, FIELD_SESSIONSTATUS);
            var sessionImageUrl = BaseGet<FieldUrlValue>(item, FIELD_SESSIONIMAGEURL);

            if (sessionImageUrl != null)
                SessionImageUrl = sessionImageUrl.Url;
            RegisteredEventId = BaseGet<string>(item, FIELD_EVENTID);
            SpeakerId = BaseGet<string>(item, FIELD_SESSIONSPEAKERID);      
        }

        protected override string ListTitle
        {
            get { return ListDetails.SessionsListName; }
        }

        protected override string ContentTypeName
        {
            get { return ContentTypes.EventSession; }
        }

        protected override string[] FieldInternalNames
        {
            get
            {
                return new string[]{
                    FIELD_SESSIONID,
                    FIELD_SESSIONDESCRIPTION,
                    FIELD_SESSIONDATE,         
                    FIELD_SESSIONSTATUS,
                    FIELD_SESSIONIMAGEURL,
                    FIELD_EVENTID,
                    FIELD_SESSIONSPEAKERID
                };
            }
        }
    }
}