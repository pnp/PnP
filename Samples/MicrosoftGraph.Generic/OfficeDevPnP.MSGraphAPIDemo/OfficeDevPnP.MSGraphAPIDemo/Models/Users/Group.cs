using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a Group
    /// </summary>
    public class Group
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
        public Boolean AllowExternalSenders;
        public Boolean AutoSubscribeNewMembers;
        public Boolean IsSubscribedByMail;
        public Int32 UnseenCount;
    }
}