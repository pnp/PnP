using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Office365Api.MVCDemo
{
    public class ApplicationADALDbContext : DbContext
    {
        public ApplicationADALDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }
    }

    public class UserTokenCache
    {
        [Key]
        public int UserTokenCacheId { get; set; }
        public string webUserUniqueId { get; set; }
        public byte[] cacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }
}