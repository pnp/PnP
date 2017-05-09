using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Deployment {
    public static class IEnumerable {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) {
            if (collection != null) {
                foreach (T item in collection) {
                    if (action != null) action(item);
                }
            }
        }
    }
}
