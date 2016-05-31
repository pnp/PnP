using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    public class ConversationThreadPost : BaseModel
    {
        public ItemBody Body { get; set; }

        public Nullable<DateTimeOffset> ReceivedDateTime { get; set; }

        public Boolean HasAttachments { get; set; }

        public UserInfoContainer From { get; set; }

        public UserInfoContainer Sender { get; set; }

        public String ConversationThreadId { get; set; }

        public List<UserInfoContainer> NewParticipants { get; set; }

        public String ConversationId { get; set; }
    }
}