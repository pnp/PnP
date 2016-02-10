//----------------------------------------------------------------------------------------------
//    Copyright 2014 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Data.Entity;
using System.Linq;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
    public class ADALTokenCache : TokenCache
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        string User;
        UserTokenCache Cache;

        // constructor
        public ADALTokenCache(string user)
        {
            // associate the cache to the current user of the web app
            User = user;
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            this.BeforeWrite = BeforeWriteNotification;

            // look up the entry in the DB
            Cache = db.UserTokenCacheList.FirstOrDefault(c => c.webUserUniqueId == User);
            // place the entry in memory
            this.Deserialize((Cache == null) ? null : Cache.cacheBits);
        }

        // clean up the DB
        public override void Clear()
        {
            base.Clear();
            foreach (var cacheEntry in db.UserTokenCacheList)
                db.UserTokenCacheList.Remove(cacheEntry);
            db.SaveChanges();
        }

        // Notification raised before ADAL accesses the cache.
        // This is your chance to update the in-memory copy from the DB, if the in-memory version is stale
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            if (Cache == null)
            {
                // first time access
                Cache = db.UserTokenCacheList.FirstOrDefault(c => c.webUserUniqueId == User);
            }
            else
            {   // retrieve last write from the DB
                var status = from e in db.UserTokenCacheList
                             where (e.webUserUniqueId == User)
                             select new
                             {
                                 LastWrite = e.LastWrite
                             };
                // if the in-memory copy is older than the persistent copy
                if (status.First().LastWrite > Cache.LastWrite)
                //// read from from storage, update in-memory copy
                {
                    Cache = db.UserTokenCacheList.FirstOrDefault(c => c.webUserUniqueId == User);
                }
            }
            this.Deserialize((Cache == null) ? null : Cache.cacheBits);
        }

        // Notification raised after ADAL accessed the cache.
        // If the HasStateChanged flag is set, ADAL changed the content of the cache
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if state changed
            if (this.HasStateChanged)
            {
                Cache = new UserTokenCache
                {
                    webUserUniqueId = User,
                    cacheBits = this.Serialize(),
                    LastWrite = DateTime.Now
                };
                //// update the DB and the lastwrite                
                db.Entry(Cache).State = Cache.UserTokenCacheId == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
                this.HasStateChanged = false;
            }
        }

        void BeforeWriteNotification(TokenCacheNotificationArgs args)
        {
            // if you want to ensure that no concurrent write take place, use this notification to place a lock on the entry
        }
    }
}
