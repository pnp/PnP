using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class UserPermissionsCheckRequest
    {
        [DataMember]
        public bool DoesUserHavePermissions { get; set; }

       
    }
}