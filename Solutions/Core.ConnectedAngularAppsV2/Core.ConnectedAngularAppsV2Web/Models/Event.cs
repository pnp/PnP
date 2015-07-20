using CorporateEvents;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Core.ConnectedAngularAppsV2Web.Models
{
    public class Event : BaseListItem {
        internal static readonly Guid RegisteredEventFieldId = new Guid("{E10F8222-BCC3-4348-9463-4963D0AD4900}");
        internal const string FIELD_REGISTERED_EVENT_ID = "RegisteredEventID";
        internal const string FIELD_DESCRIPTION = "EventDescription";
        internal const string FIELD_CATEGORY = "EventCategory";
        internal const string FIELD_DATE = "EventDate";
        internal const string FIELD_CONTACT_EMAIL = "EventContactEmail";
        internal const string FIELD_LOCATION = "EventLocation";
        internal const string FIELD_IMAGE_URL = "EventImageUrl";
        internal const string FIELD_STATUS = "EventStatus";

        public Event() : base() { }
        public Event(ListItem item) : base(item) {
            Initialize(item);
        }

        [Display(Name="Event Id")]
        public string RegisteredEventId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string Category { get; set; }

        [Display(Name="Event Date")]
        public DateTime? EventDate { get; set; }

        public string Location { get; set; }

        [Display(Name="Contact Email")]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail { get; set; }

        public string Status { get; set; }

        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; }

        public string ShortDescription { get { return Description; } }

        protected override void SetProperties(ListItem item) {
            BaseSet(item, FIELD_REGISTERED_EVENT_ID, RegisteredEventId);
            BaseSet(item, FIELD_DESCRIPTION, Description);
            BaseSet(item, FIELD_CATEGORY, Category);
            BaseSet(item, FIELD_DATE, EventDate);
            BaseSet(item, FIELD_LOCATION, Location);
            BaseSet(item, FIELD_CONTACT_EMAIL, ContactEmail);
            BaseSet(item, FIELD_STATUS, Status);
            BaseSet(item, FIELD_IMAGE_URL, ImageUrl);
        }

        protected override void ReadProperties(ListItem item) {
            RegisteredEventId = BaseGet<string>(item, FIELD_REGISTERED_EVENT_ID);
            Description = BaseGet<string>(item, FIELD_DESCRIPTION);
            Category = BaseGet<string>(item, FIELD_CATEGORY);
            EventDate = BaseGet<DateTime?>(item, FIELD_DATE);
            Location = BaseGet<string>(item, FIELD_LOCATION);
            ContactEmail = BaseGet<string>(item, FIELD_CONTACT_EMAIL);
            Status = BaseGet<string>(item, FIELD_STATUS);
            var imageUrl = BaseGet<FieldUrlValue>(item, FIELD_IMAGE_URL);

            if (imageUrl != null)
                ImageUrl = imageUrl.Url;
        }

        protected override string ListTitle {
            get { return ListDetails.EventsListName; }
        }

        protected override string ContentTypeName {
            get { return ContentTypes.CorporateEvent; }
        }

        protected override string[] FieldInternalNames {
            get {
                return new string[]{
                    FIELD_CATEGORY,
                    FIELD_CONTACT_EMAIL,
                    FIELD_DATE,
                    FIELD_DESCRIPTION,
                    FIELD_IMAGE_URL,
                    FIELD_LOCATION,
                    FIELD_REGISTERED_EVENT_ID,
                    FIELD_STATUS
                };
            }
        }
    }
}