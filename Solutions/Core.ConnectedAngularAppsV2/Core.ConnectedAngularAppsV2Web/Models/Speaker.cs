using CorporateEvents;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Core.ConnectedAngularAppsV2Web.Models
{
    public class Speaker : BaseListItem
    {
        internal const string FIELD_FIRSTNAME = "SpeakerFirstName";
        internal const string FIELD_LASTNAME = "SpeakerLastName";
        internal const string FIELD_EMAIL = "SpeakerEmail";
        internal const string FIELD_ID = "SpeakerID";       

        public Speaker() : base() { }
        public Speaker(ListItem item) : base(item) {
            Initialize(item);
        }
        [Display(Name = "First Name")]
        public string SpeakerFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string SpeakerLastName { get; set; }

        [Display(Name = "Speaker Email")]
        [DataType(DataType.EmailAddress)]
        public string SpeakerEmail { get; set; }      

        [Display(Name = "Speaker Id")]
        public string SpeakerId { get; set; }

        protected override void SetProperties(ListItem item)
        {
            BaseSet(item, FIELD_FIRSTNAME, SpeakerFirstName);
            BaseSet(item, FIELD_LASTNAME, SpeakerLastName);
            BaseSet(item, FIELD_EMAIL, SpeakerEmail);            
            BaseSet(item, FIELD_ID, SpeakerId);            
        }

        protected override void ReadProperties(ListItem item)
        {
            SpeakerId = BaseGet<string>(item, FIELD_ID);
            SpeakerFirstName = BaseGet<string>(item, FIELD_FIRSTNAME);
            SpeakerLastName = BaseGet<string>(item, FIELD_LASTNAME);
            SpeakerEmail = BaseGet<string>(item, FIELD_EMAIL);           
        }

        protected override string ListTitle
        {
            get { return ListDetails.SpeakersListName; }
        }

        protected override string ContentTypeName
        {
            get { return ContentTypes.EventSpeaker; }
        }

        protected override string[] FieldInternalNames
        {
            get
            {
                return new string[]{
                    FIELD_ID,
                    FIELD_FIRSTNAME,
                    FIELD_LASTNAME,         
                    FIELD_EMAIL                   
                };
            }
        }
    }
}