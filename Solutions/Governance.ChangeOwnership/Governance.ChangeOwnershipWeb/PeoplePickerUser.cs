using System.Runtime.Serialization;

namespace Contoso.Office365.common
{
    [DataContract]
    public class PeoplePickerUser
    {
        [DataMember]
        internal int LookupId;
        [DataMember]
        internal string Login;
        [DataMember]
        internal string Name;
        [DataMember]
        internal string Email;
    }
}