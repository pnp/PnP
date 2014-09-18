using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateEvents.SharePointWeb.Models {
    public class Registration : BaseListItem {
        internal const string FIELD_DATE = "RegistrationDate";
        internal const string FIELD_FIRST_NAME = "RegistrationFirstName";
        internal const string FIELD_LAST_NAME = "RegistrationLastName";
        internal const string FIELD_USER_ID = "RegistrationUserId";
        internal const string FIELD_USER_EMAIL = "RegistrationUserEmail";
        internal const string FIELD_EVENT_ID = "RegistrationEventId";

        public Registration() : base() { }
        public Registration(ListItem item) : base(item) {
            Initialize(item);
        }

        [Required]
        [Display(Name = "Event Id")]
        public string EventId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name="First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Alias")]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        protected override string ListTitle {
            get { return ListDetails.RegistrationListName; }
        }

        protected override string ContentTypeName {
            get { return ContentTypes.EventRegistration; }
        }

        protected override string[] FieldInternalNames {
            get {
                return new string[]{
                    FIELD_DATE,
                    FIELD_FIRST_NAME,
                    FIELD_LAST_NAME,
                    FIELD_USER_EMAIL,
                    FIELD_USER_ID
                };
            }
        }

        protected override void SetProperties(Microsoft.SharePoint.Client.ListItem item) {
            Title = string.Format("{0}: {1} {2}", EventId, FirstName, LastName);
            BaseSet(item, FIELD_EVENT_ID, EventId);
            BaseSet(item, FIELD_DATE, Date);
            BaseSet(item, FIELD_FIRST_NAME, FirstName);
            BaseSet(item, FIELD_LAST_NAME, LastName);
            BaseSet(item, FIELD_USER_ID, UserId);
            BaseSet(item, FIELD_USER_EMAIL, Email);
        }

        protected override void ReadProperties(Microsoft.SharePoint.Client.ListItem item) {
            EventId = BaseGet<string>(item, FIELD_EVENT_ID);
            Date = BaseGet<DateTime>(item, FIELD_DATE);
            FirstName = BaseGet<string>(item, FIELD_FIRST_NAME);
            LastName = BaseGet<string>(item, FIELD_LAST_NAME);
            UserId = BaseGet<string>(item, FIELD_USER_ID);
            Email = BaseGet<string>(item, FIELD_USER_EMAIL);
        }
    }
}