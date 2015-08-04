using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Core.CrossDomainLib.Models
{
    public class Provisionner
    {
        private ClientContext _clientContext;
        private HttpServerUtilityBase _server;
        private HttpRequestBase _request;

        public Provisionner(ClientContext clientContext,HttpServerUtilityBase server, HttpRequestBase request){
            _clientContext = clientContext;
            _server = server;
            _request = request;
        }

        public string ProvisionGetSample()
        {
            //This method will create a SharePoint page, with a script editor, containing the javascript for the sample.

            string getSampleScriptEditorHtml = string.Format(@"<script type='text/javascript'>
                                                                    var sampleHostUrl='{0}';
                                                                    var sampleServerUrl='{1}';
                                                                   </script>
                                                                   <script src='{1}/Scripts/jquery-1.10.2.js'></script>
                                                                   <script src='{1}/Scripts/CrossDomainUtil.js'></script>
                                                                   <script type='text/javascript' src='{1}\Scripts\GetSample.js'></script>
                                                                   <div><button id='DoGetButton'>Perform get to server</button></div>"
                                                                    , _request.QueryString["SPHostUrl"]
                                                                    , "https://" + _request.Url.Authority);

            string wikiHtmlContent = @"<div>
                                            <h2>Get sample</h2>
                                            <span>When clicking the button, a GET action is executed to the MVC webserver.<br/>You can find the code that is executed in the <b>GetSample.js</b> file<br/>This javascript was injected using a script editor, but the same technique is possible from a pagelayout, or masterpage.</span>
                                            <br/><br/>
                                       </div>"; 

            return ProvisionSharepointPageWithScriptEditor("CDLGetSample.aspx", getSampleScriptEditorHtml,wikiHtmlContent);
        }

        public string ProvisionPostSample()
        {
            //This method will create a SharePoint page, with a script editor, containing the javascript for the sample.

            string getSampleScriptEditorHtml = string.Format(@"<script type='text/javascript'>
                                                                    var sampleHostUrl='{0}';
                                                                    var sampleServerUrl='{1}';
                                                                   </script>
                                                                   <script src='{1}/Scripts/jquery-1.10.2.js'></script>
                                                                   <script src='{1}/Scripts/CrossDomainUtil.js'></script>
                                                                   <script type='text/javascript' src='{1}\Scripts\PostSample.js'></script>
                                                                   <div><button id='DoPostButton'>Perform post to server</button></div>"
                                                                    , _request.QueryString["SPHostUrl"]
                                                                    , "https://" + _request.Url.Authority);

            string wikiHtmlContent = @"<div>
                                            <h2>Post sample</h2>
                                            <span>When clicking the button, a POST action is executed to the MVC webserver.<br/>You can find the code that is executed in the <b>PostSample.js</b> file<br/>This javascript was injected using a script editor, but the same technique is possible from a pagelayout, or masterpage.</span>
                                            <br/><br/>
                                       </div>";

            return ProvisionSharepointPageWithScriptEditor("CDLPostSample.aspx", getSampleScriptEditorHtml,wikiHtmlContent);
        }

        public string ProvisionViewWithPostSample() 
        {
            //This method will create a SharePoint page, with a script editor, containing the javascript for the sample.

            string getSampleScriptEditorHtml = string.Format(@"<script type='text/javascript'>
                                                                    var sampleHostUrl='{0}';
                                                                    var sampleServerUrl='{1}';
                                                                   </script>
                                                                   <script src='{1}/Scripts/jquery-1.10.2.js'></script>
                                                                   <script src='{1}/Scripts/CrossDomainUtil.js'></script>
                                                                   <script type='text/javascript' src='{1}\Scripts\ViewWithPostSample.js'></script>
                                                                   <div id='testingcrossdiv'></div>"
                                                        , _request.QueryString["SPHostUrl"]
                                                        , "https://" + _request.Url.Authority);

            string wikiHtmlContent = @"<div>
                                            <h2>View with post sample</h2>
                                            <span>When the cross domain library is initialized a GET request is made to the home/TestView action. The controller returns html and javascript, that is injected on the page.<br/>With this way you can create app parts, without having iframes.<br/>When the button is clicked, a POST is made with the values entered in the textboxes. <br/>You can find the code in the <b>ViewWithPostSample.js</b>, and the <b>TestView.js</b> file.<br/>This javascript was injected using a script editor, but the same technique is possible from a pagelayout, or masterpage.<br/><br/>The form below is generated by a MVC razor view:</span>
                                            <br/><br/>
                                       </div>";

            return ProvisionSharepointPageWithScriptEditor("CDLViewWithPostSample.aspx", getSampleScriptEditorHtml, wikiHtmlContent);
        }

        private string ProvisionSharepointPageWithScriptEditor(string pageName, string html, string wikiHtmlContent)
        {
            //get page library
            List wikiLibrary = _clientContext.Web.Lists.GetByTitle("Site Pages"); //make sure your developer site is in english, or change this title!!!
            _clientContext.Load(wikiLibrary);
            _clientContext.Load(wikiLibrary.RootFolder);
            _clientContext.ExecuteQuery();

            //get page
            string serverrelativeurl = string.Format("{0}/{1}", wikiLibrary.RootFolder.ServerRelativeUrl, pageName);

            Microsoft.SharePoint.Client.File page;
            try
            {
                page = _clientContext.Web.GetFileByServerRelativeUrl(serverrelativeurl);
                _clientContext.Load(page);
                _clientContext.ExecuteQuery();
            }
            catch (Exception)
            {
                //if page does not exist, create it
                var newWikiPage = new WikiPageCreationInformation();

                newWikiPage.WikiHtmlContent = wikiHtmlContent;
                newWikiPage.ServerRelativeUrl = serverrelativeurl;
                Utility.CreateWikiPageInContextWeb(_clientContext, newWikiPage);
                _clientContext.ExecuteQuery();

                page = _clientContext.Web.GetFileByServerRelativeUrl(newWikiPage.ServerRelativeUrl);

                //add script editor with the javascript that will do the magic
                string webPartXml = "";
                using (StreamReader sr = new StreamReader(_server.MapPath("~/ScriptEditorTemplate.webpart")))
                {
                    webPartXml = sr.ReadToEnd();
                }

                html = html.Replace("<", "&lt;").Replace(">", "&gt;"); //replace illegal xml tokens
                webPartXml = webPartXml.Replace("#content#", html); //put javascript in the content of script editor webpart

                var wpm = page.GetLimitedWebPartManager(PersonalizationScope.Shared);
                WebPartDefinition wpd = wpm.ImportWebPart(webPartXml);
                wpm.AddWebPart(wpd.WebPart, "Right", 1);
                _clientContext.ExecuteQuery();
            }

            return string.Format("{0}/{1}", wikiLibrary.RootFolder.ServerRelativeUrl, pageName);
        }
    }
}