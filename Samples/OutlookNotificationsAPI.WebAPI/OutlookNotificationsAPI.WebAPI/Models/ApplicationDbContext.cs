using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OutlookNotificationsAPI.WebAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }
        public DbSet<Subscription> SubscriptionList { get; set; }
    }

    public class UserTokenCache
    {
        [Key]
        public int UserTokenCacheId { get; set; }
        public string webUserUniqueId { get; set; }
        public byte[] cacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }

    public class Subscription
    {
        [Key]
        public int SubscriptionUserTokenCacheId { get; set; }
        public string SubscriptionExpirationDateTime { get; set; }
        public string SubscriptionId { get; set; }
        public string SignedInUserID { get; set; }
        public string TenantID { get; set; }
        public string UserObjectID { get; set; }
    }
}
