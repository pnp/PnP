namespace Core.JavaScript.Embedder
{
    using System;
    using System.Linq;
    using Microsoft.SharePoint.Client;
    using System.Text.RegularExpressions;

    class Program
    {
        static void Main(string[] args)
        {
            ContextManager.WithContext((context) =>
            {
                // this is the script block that will be embedded into the page
                // in practice this can be done during provisioning of the site/web
                // make sure to include ';' at end to play nice with page embedding
                // using the script on demand feature built into SharePoint we load jQuery, then our remote loader(pnp-loader.js or pnp-loader-cached.js) file using a dependency
                var script = @"(function (loaderFile, nocache) {
                                        var url = loaderFile + ((nocache) ? '?' + encodeURIComponent((new Date()).getTime()) : '');
                                        SP.SOD.registerSod('pnp-jquery.js', 'https://localhost:44324/js/jquery.js');
                                        SP.SOD.registerSod('pnp-loader.js', url);
                                        SP.SOD.registerSodDep('pnp-loader.js', 'pnp-jquery.js');
                                        SP.SOD.executeFunc('pnp-loader.js', null, function() {});
                                })('https://localhost:44324/pnp-loader.js', true);";


                // this version of the script along with pnp-loaderMDS.js (or pnp-loaderMDS-cached.js) handles pages where the minimum download strategy is active
                var script2 = @"ExecuteOrDelayUntilBodyLoaded(function () {
                                    var url = 'https://localhost:44324/js/pnp-loaderMDS.js?' + encodeURIComponent((new Date()).getTime());
                                    SP.SOD.registerSod('pnp-jquery.js', 'https://localhost:44324/js/jquery.js');
                                    SP.SOD.registerSod('pnp-loader.js', url);
                                    SP.SOD.registerSodDep('pnp-loader.js', 'pnp-jquery.js');
                                    SP.SOD.executeFunc('pnp-loader.js', null, function () {
                                        if (typeof pnpLoadFiles === 'undefined') {
                                            RegisterModuleInit('https://localhost:44324/js/pnp-loaderMDS.js', pnpLoadFiles);
                                        } else {
                                            pnpLoadFiles();
                                        }
                                    });    
                                });";

                // load the collection of existing links
                var links = context.Site.RootWeb.UserCustomActions;
                context.Load(links, ls => ls.Include(l => l.Title));
                context.ExecuteQueryRetry();

                // this block handles deleting previous test custom actions
                var doDelete = false;
                foreach (var link in links.Where(l => l.Title.Equals("MyTestCustomAction", StringComparison.OrdinalIgnoreCase)))
                {
                    link.DeleteObject();
                    doDelete = true;
                }

                if (doDelete)
                {
                    context.ExecuteQueryRetry();
                }

                // now we embed our script into the user custom action
                var newLink = context.Site.RootWeb.UserCustomActions.Add();
                newLink.Title = "MyTestCustomAction";
                newLink.Description = "Doing some testing.";
                newLink.ScriptBlock = script2;
                newLink.Location = "ScriptLink";
                newLink.Update();
                context.ExecuteQueryRetry();
            });
        }
    }
}
