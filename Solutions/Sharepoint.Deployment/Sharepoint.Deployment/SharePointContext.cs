using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client;

namespace SharePoint.Deployment {
    public class SharePointContext : ISharePointContext {
        private const int MAX_REQUESTS = 10;
        private string _url;
        private ICredentials _credentials;
        private int _pendingRequests = 0;
        private Action _asyncActions;
        private Action _successActions;
        private Action<Exception> _errorActions;

        public SP.ClientContext Context { get; set; }
        public SP.Site SiteCollection { get { return this.Context.Site; } }

        public SharePointContext(string url, ICredentials credentials) {
            this._credentials = credentials;
            this._url = url;
            this.InitializeContext();
        }

        public SharePointContext(string url, string userName, string password) {
            SecureString securePwd = new SecureString();
            password.ToList().ForEach(c => securePwd.AppendChar(c));
            this._credentials = new SP.SharePointOnlineCredentials(userName, securePwd);
            this._url = url;
            this.InitializeContext();
        }

        protected void InitializeContext() {
            this.Context = new SP.ClientContext(this._url) {
                Credentials = this._credentials,
                AuthenticationMode = SP.ClientAuthenticationMode.Default
                };
        }

        public SP.Web OpenWeb(string url) {
            SP.Web returnValue = this.Context.Site.OpenWeb(url);
            this.ExecuteAsync(() => { returnValue = this.Context.Site.OpenWeb(url); });
            return returnValue;
        }

        public void ExecuteSync(Action action) {
            //this.flush();
            //this._errorActions += (ex) => { throw ex; };
            //action();
            this.ExecuteAsync(action);
            this.Flush();
        }

        public void ExecuteSync<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : SP.ClientObject {
            //this.flush();
            //this._errorActions += (ex) => { throw ex; };
            //this.context.Load(clientObject, retrievals);
            this.ExecuteAsync(clientObject, retrievals);
            this.Flush();
        }

        public bool TryExecuteSync<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : SP.ClientObject {
            bool returnValue = false;

            try {
                this.ExecuteSync(clientObject, retrievals);
                returnValue = true;
            } catch (SP.ServerException) {
                returnValue = false;
            }

            return returnValue;
        }

        public void ExecuteAsync(Action action) {
            this.ExecuteAsync(action, (Action)null);
        }

        public void ExecuteAsync(Action action, Action onSuccess) {
            this.ExecuteAsync(action, onSuccess, (ex) => { throw ex; });
        }

        public void ExecuteAsync(Action action, Action onSuccess, Action<Exception> onError) {
            if (onSuccess != null) this._successActions += onSuccess;
            if (onError != null) this._errorActions += onError;
            this._asyncActions += action;

            this._pendingRequests++;
            if (this._pendingRequests >= MAX_REQUESTS) {
                this.Flush();
            }
        }

        public void ExecuteAsync<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : SP.ClientObject {
            this.ExecuteAsync(clientObject, null, retrievals);
        }

        public void ExecuteAsync<T>(T clientObject, Action onSuccess, params Expression<Func<T, object>>[] retrievals) where T : SP.ClientObject {
            this.ExecuteAsync(clientObject, null, (ex) => { throw ex; }, retrievals);
        }

        public void ExecuteAsync<T>(T clientObject, Action onSuccess, Action<Exception> onError, params Expression<Func<T, object>>[] retrievals) where T : SP.ClientObject {
            if (onSuccess != null) this._successActions += onSuccess;
            if (onError != null) this._errorActions += onError;
            this._asyncActions += () => this.Context.Load(clientObject, retrievals);

            this._pendingRequests++;
            if (this._pendingRequests >= MAX_REQUESTS) {
                this.Flush();
            }
        }

        public void Flush() {
            if (this._pendingRequests > 0 | this.Context.HasPendingRequest) {
                Exception remoteError = null;
                bool success = false;
                int retries = 3;
                while (retries-- > 0 && !success) {
                    try {
                        if (this._asyncActions != null) this._asyncActions();
                        this.Context.ExecuteQuery();
                        remoteError = null;
                        success = true;
                    } catch (SP.ServerException ex) {
                        remoteError = ex;
                    } catch (Exception ex) {
                        remoteError = ex;
                    }
                }

                var successActions = this._successActions;
                var errorActions = this._errorActions;
                this.ResetPendingOperations();

                if (remoteError == null) {
                    if (successActions != null) {
                        successActions();
                    }
                } else {
                    if (errorActions != null) {
                        errorActions(remoteError);
                    }
                }
            }
        }

        protected void ResetPendingOperations() {
            this._pendingRequests = 0;
            this._asyncActions = null;
            this._successActions = null;
            this._errorActions = null;
        }

        public void Invalidate() {
            this.ResetPendingOperations();
            this.Context.Dispose();
            this.InitializeContext();
        }

        public void Dispose() {
            this.Flush();
            try { this.Context.Dispose(); } catch { /* Ignore, we're outta here */ }
        }

    }
}
