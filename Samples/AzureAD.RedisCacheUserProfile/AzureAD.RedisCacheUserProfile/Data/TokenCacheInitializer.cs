using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureAD.RedisCacheUserProfile.Data
{
    public class TokenCacheInitializer : System.Data.Entity.CreateDatabaseIfNotExists<TokenCacheDataContext>
    {
    }
}