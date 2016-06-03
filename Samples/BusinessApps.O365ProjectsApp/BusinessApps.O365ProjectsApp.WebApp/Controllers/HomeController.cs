using BusinessApps.O365ProjectsApp.Infrastructure;
using BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusinessApps.O365ProjectsApp.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var me =
                MicrosoftGraphHelper.MakeGetRequestForString(MicrosoftGraphHelper.MicrosoftGraphV1BaseUri + "me");

            try
            {
                SPORemoteActions.ProvisionArtifactsByCode();
                SPORemoteActions.BrowseFilesLibrary();

                // PlayWithGroupsViaGraphAPI();
            }
            catch
            {
                // Skip exception for training purposes only
            }

            ViewBag.Current = "Index";
            return View();
        }

        private void PlayWithGroupsViaGraphAPI()
        {
            try
            {
                var test = GraphRemoteActions.Office365GroupExists("Sample Group");

                MemoryStream memPhoto = new MemoryStream();
                using (FileStream fs = new FileStream(Server.MapPath("~/AppIcon.png"), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Byte[] newPhoto = new Byte[fs.Length];
                    fs.Read(newPhoto, 0, (Int32)(fs.Length - 1));
                    memPhoto.Write(newPhoto, 0, newPhoto.Length);
                    memPhoto.Position = 0;
                }

                var groupNameId = Guid.NewGuid().ToString().Replace("-", "");
                var group = GraphRemoteActions.CreateOffice365Group(
                    new Group
                    {
                        DisplayName = $"Project's Group - {groupNameId}",
                        MailEnabled = true,
                        SecurityEnabled = true,
                        GroupTypes = new List<String>(new String[] { "Unified" }),
                        MailNickname = groupNameId,
                    },
                    new String[] {
                        "paolo@piasysdev.onmicrosoft.com",
                        "paolo.pialorsi@sharepoint-camp.com"
                    },
                    memPhoto);

                GraphRemoteActions.SendMessageToGroupConversation(group.Id,
                    new Conversation
                    {
                        Topic = "Let's manage this Business Project!",
                        Threads = new List<ConversationThread>(
                            new ConversationThread[] {
                                new ConversationThread
                                {
                                    Topic = "I've just created this Business Project",
                                    Posts = new List<ConversationThreadPost>(
                                        new ConversationThreadPost[]
                                        {
                                            new ConversationThreadPost
                                            {
                                                Body = new ItemBody
                                                {
                                                    Content = "<h1>Welcome to this Business Project</h1>",
                                                    Type = BodyType.Html,
                                                },
                                            }
                                        })
                                }
                            })
                    });
            }
            catch
            {
                // Skip exception for training purposes only
            }
        }

        public ActionResult StartNewProcess()
        {
            ViewBag.Current = "StartNewProcess";
            return View("Index");
        }

        public ActionResult MyProcesses()
        {
            ViewBag.Current = "MyProcesses";
            return View("Index");
        }

        public ActionResult Settings()
        {
            ViewBag.Current = "Settings";
            return View("Index");
        }
    }
}