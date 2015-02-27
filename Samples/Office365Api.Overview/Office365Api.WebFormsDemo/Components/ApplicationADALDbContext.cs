using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Office365Api.WebFormsDemo
{
    public class ApplicationADALDbContext : DbContext
    {
        public ApplicationADALDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<ApplicationADALDbContext>(
                new DropCreateDatabaseAlways<ApplicationADALDbContext>());
        }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<User> Users { get; set; }
    }

    public class UserTokenCache
    {
        [Key]
        public int UserTokenCacheId { get; set; }
        public string webUserUniqueId { get; set; }
        public byte[] cacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }

    // Entity for keeping track of organizations onboarded as customers of the app
    public class Tenant
    {
        public int ID { get; set; }
        public string IssValue { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        [DisplayName("Check this if you are an administrator and you want to enable the app for all your users")]
        public bool AdminConsented { get; set; }


    }

    // Entity for keeping track of individual users onboarded as customers of the app
    public class User
    {
        [Key]
        public string UPN { get; set; }
        public string TenantID { get; set; }
    }
}