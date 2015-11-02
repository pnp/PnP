namespace Embedder
{
    using System;
    using System.Linq;
    using Microsoft.SharePoint.Client;

    class Program
    {
        static void Main(string[] args)
        {
            ContextManager.WithContext((context) =>
            {
                // this is for testing, and is the start url of the CDN project
                var loaderFilePath = "https://localhost:44323/Loader.js";

                // this is the script block that will be embedded into the page
                // in practice this can be done during provisioning of the site/web
                // make sure to include ';' at end to play nice with page embedding
                // using the script on demand feature built into SharePoint we load jQuery, then our remote loader file using a dependency
                var script =  @"(function (loaderFile, nocache) {
                                        var url = loaderFile + ((nocache) ? '?' + encodeURIComponent((new Date()).getTime()) : '');
                                        SP.SOD.registerSod('testcdn-jquery.js', 'https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.4.js');
                                        SP.SOD.registerSod('testcdn-loader.js', url);
                                        SP.SOD.registerSodDep('testcdn-loader.js', 'testcdn-jquery.js');
                                        SP.SOD.executeFunc('testcdn-loader.js', null, function() {});
                                })('" + loaderFilePath + "', true);";


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
                newLink.ScriptBlock = script;
                newLink.Location = "ScriptLink";
                newLink.Update();
                context.ExecuteQueryRetry();
            });
        }
    }
}
