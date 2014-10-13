using System;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Services
{
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

        public void Add(string cacheKey, SharePointServiceContexCacheItem sharePointServiceContextCacheItem)
        {
            this.clientContextCache.Add(cacheKey, sharePointServiceContextCacheItem);
        }

        public SharePointServiceContexCacheItem Get(string cacheKey)
        {
            return this.clientContextCache[cacheKey];
        }

    }
}
