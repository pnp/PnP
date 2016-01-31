using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class Permission: BaseModel
    {
        public IdentitySet GrantedTo;
        public SharingInvitation Invitation;
        public ItemReference InheritedFrom;
        public SharingLink Link;
        public List<String> Roles;
        public String ShareId;
    }
}