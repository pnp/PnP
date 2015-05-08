using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
using SP = Microsoft.SharePoint.Client;

namespace SharePoint.Deployment {
    public interface ISharePointContext : IDisposable {
        SP.ClientContext Context { get; set; }
        void Dispose();
        void ExecuteAsync(Action action, Action onSuccess, Action<Exception> onError);
        void ExecuteAsync<T>(T clientObject, Action onSuccess, Action<Exception> onError, params Expression<Func<T, object>>[] retrievals) where T : ClientObject;
        void ExecuteAsync(Action action);
        void ExecuteAsync<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject;
        void ExecuteSync(Action action);
        void ExecuteSync<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject;
        void Flush();
        SP.Web OpenWeb(string url);
        SP.Site SiteCollection { get; }
    }
}
