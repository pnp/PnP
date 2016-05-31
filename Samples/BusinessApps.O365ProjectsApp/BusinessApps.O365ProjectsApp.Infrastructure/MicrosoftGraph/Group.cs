using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    /// <summary>
    /// Defines a Group
    /// </summary>
    public class Group : BaseModel
    {
        public String Description;
        public String DisplayName;
        public List<String> GroupTypes;
        public String Mail;
        public Boolean MailEnabled;
        public String MailNickname;
        public Nullable<DateTimeOffset> OnPremisesLastSyncDateTime;
        public String OnPremisesSecurityIdentifier;
        public Nullable<Boolean> OnPremisesSyncEnabled;
        public List<String> ProxyAddresses;
        public Boolean SecurityEnabled;
        public String Visibility;
        public Nullable<Boolean> AllowExternalSenders;
        public Nullable<Boolean> AutoSubscribeNewMembers;
        public Nullable<Boolean> IsSubscribedByMail;
        public Nullable<Int32> UnseenCount;
    }
}