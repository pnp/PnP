using System;

namespace OfficeDevPnP.Core.Entities
{
    /// <summary>
    /// External user entity
    /// </summary>
    public class ExternalUserEntity
    {
        public string AcceptedAs { get; set; }
        public string DisplayName { get; set; }
        public string InvitedAs { get; set; }
        public string InvitedBy { get; set; }
        public string UniqueId { get; set; }
        public DateTime WhenCreated { get; set; }

    }
}
