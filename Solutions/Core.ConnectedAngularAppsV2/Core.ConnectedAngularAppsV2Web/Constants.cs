using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents
{
    internal static class ListDetails {
        internal const string CURRENT_EVENTS_CONFIGURATION_VERSION = "0.1.0.0";

        public static readonly string RegistrationListName = "Event Registration";
        public static readonly string RegistrationUrl = "Lists/EventRegistration";
        public static readonly string RegistrationListDesc = "Corporate Event Registration";
        
        public static readonly string SessionsListName = "Event Sessions";
        public static readonly string SessionsUrl = "Lists/EventSessions";
        public static readonly string SessionsListDesc = "Lists all sessions for each Corporate Event";

        public static readonly string EventsListName = "Corporate Events";
        public static readonly string EventsUrl = "Lists/CorporateEvents";
        public static readonly string EventsListDesc = "Lists corporate events.";

        public static readonly string SpeakersListName = "Event Speakers";
        public static readonly string SpeakersUrl = "Lists/EventSpeakers";
        public static readonly string SpeakersListDesc = "Event and Session Speakers";

        public static readonly string CorporateEventsSiteColumnsGroup = "Corporate Events Columns";
    }
    internal static class ContentTypes {
        public static readonly string EventSession = "Event Session";
        public static readonly string EventSessionContentTypeDesc = "Event Session";
        public static readonly string EventSessionContentTypeGroup = "Corporate Events Group";
        public static readonly string EventSessionContentTypeId = "0x0100C3C2BFE250354897AEED6099688D6C25";

        public static readonly string EventSpeaker = "Event Speaker";
        public static readonly string EventSpeakerContentTypeDesc = "Event Speaker";
        public static readonly string EventSpeakerContentTypeGroup = "Corporate Events Group";
        public static readonly string EventSpeakerContentTypeId = "0x010099726077CDF64F4DAC5022766961D5C4";

        public static readonly string CorporateEvent = "Corporate Event";
        public static readonly string CorporateEventContentTypeDesc = "Corporate Event";
        public static readonly string CorporateEventContentTypeGroup = "Corporate Events Group";
        public static readonly string CorporateEventContentTypeId = "0x01002AF578033DB44942AE8CA0C43623C957";

        public static readonly string EventRegistration = "Event Registration";
        public static readonly string EventRegistrationContentTypeDesc = "Event Registration";
        public static readonly string EventRegistrationContentTypeGroup = "Corporate Events Group";
        public static readonly string EventRegistrationContentTypeId = "0x010062B2D150C3DB42C5B15BDACA5CC19F1E";
    }
}