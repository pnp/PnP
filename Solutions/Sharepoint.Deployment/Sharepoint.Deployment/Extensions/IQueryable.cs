using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Deployment {
    public static class IQueryable {
        public static IQueryable<TSource> Include<TSource>(this IQueryable<TSource> clientObjects, params Expression<Func<TSource, object>>[] retrievals) where TSource : ClientObject {
            return clientObjects.Include(retrievals);
        }
        public static IQueryable<TSource> IncludeWithDefaultProperties<TSource>(this IQueryable<TSource> clientObjects, params Expression<Func<TSource, object>>[] retrievals) where TSource : ClientObject {
            return clientObjects.IncludeWithDefaultProperties(retrievals);
        }
    }
}
