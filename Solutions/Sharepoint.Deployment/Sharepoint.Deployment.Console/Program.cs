using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client;

namespace SharePoint.Deployment {
    class Program {
        static void Main(string[] args) {
            string siteUrl = "https://jornata.sharepoint.com";
            string userName = "abc";
            string password = "xyz";
            Web xyz = new Web() {
                Title = "xyz1",
                UrlSegment = "xyz1",
                Fields = {
                    new SiteField(new Guid("94c46e17-25da-4a5a-80ff-eefb4ba07a40"), FieldType.Number, "sample1")
                    },
                Lists = {
                    new List(SP.ListTemplateType.GenericList, "List1") {
                        UrlSegment = "Lists/List1",
                        Fields = {
                            ListField.FromSiteField(new Guid("94c46e17-25da-4a5a-80ff-eefb4ba07a40")),
                            new ListField(FieldType.Text, "who does numbah 2 work foah") { Required = true, InternalName = "number2", Options = { {"StaticName","number2"} } }
                            },
                        Views = {
                            new ListView("No", "Title", "number2")
                            }
                        }
                    }
                };
            using (Web web = new Web() {
                    Parent = new Site { Url = siteUrl },
                    UrlSegment = "",
                    Webs = new List<Web> { xyz }
                    }) {
                web.Init(userName, password);
                xyz.Delete();
                web.Deploy();

                var list1 = xyz.Lists.First();

                list1.AddItems("",
                    new ListItem() { { "Title", DateTime.Now.ToString() }, { "number2", "abc" }, { "sample1", 123 } },
                    new ListItem() { { "Title", DateTime.Now.ToString() }, { "number2", "xyz" }, { "sample1", 1 } }
                    );

                var items = list1.GetItems();

                items.ForEach(i => {
                    Console.WriteLine("Item:");
                    i.ForEach(j => {
                        Console.WriteLine("   {0}: {1}", j.Key, j.Value);
                    });
                    Console.ReadLine();
                });
            }
        }
    }
}
