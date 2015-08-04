using AzureAD.RedisCacheUserProfile.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace AzureAD.RedisCacheUserProfile.Data
{
    public class TokenCacheDataContext : DbContext
    {
        public TokenCacheDataContext()
            : base("TokenCacheDataContext")
        { }

        public DbSet<PerUserWebCache> PerUserCacheList { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}