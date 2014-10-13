using System;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Services
{
    /// <summary>
    /// Simple cache implementation based on the singleton pattern. Caches the SharePoint access token, refresh token and the information passed 
    /// during service "registration". All of this information is wrapped in a <see cref="SharePointServiceContexCacheItem"/> object.
    /// </summary>
    public class SharePointServiceContextCache
    {
        private Dictionary<string, SharePointServiceContexCacheItem> clientContextCache = new Dictionary<string, SharePointServiceContexCacheItem>();

        #region Singleton implementation
        // Singleton variables
        private static volatile SharePointServiceContextCache instance;
        private static object syncRoot = new Object();
        
        // Singleton private constructor
        private SharePointServiceContextCache() { }

        /// <summary>
        /// Singleton instance to access this class
        /// </summary>
        public static SharePointServiceContextCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SharePointServiceContextCache();
                    }
                }

                return instance;
            }
        }
        #endregion

        /// <summary>
        /// Adds an item to the cache. Updates if the item already existed
        /// </summary>
        /// <param name="cacheKey">Key to cache the item</param>
        /// <param name="sharePointServiceContextCacheItem">A <see cref="SharePointServiceContexCacheItem"/> object</param>
        public void Put(string cacheKey, SharePointServiceContexCacheItem sharePointServiceContextCacheItem)
        {
            if (!this.clientContextCache.ContainsKey(cacheKey))
            {
                this.clientContextCache.Add(cacheKey, sharePointServiceContextCacheItem);
            }
            else
            {
                this.clientContextCache[cacheKey] = sharePointServiceContextCacheItem;
            }
        }

        /// <summary>
        /// Gets an item from the cache
        /// </summary>
        /// <param name="cacheKey">Key to retrieve an item from cache</param>
        /// <returns>A <see cref="SharePointServiceContexCacheItem"/> object</returns>
        public SharePointServiceContexCacheItem Get(string cacheKey)
        {
            return this.clientContextCache[cacheKey];
        }

    }
}
