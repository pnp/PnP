using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Contoso.Provisioning.Cloud.SyncWeb.Modules
{
    public partial class YammerProvisioningModule : BaseProvisioningModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //add listitems to dropdownlist
            if (cboNewsfeedType.Items.Count == 0)
            {
                cboNewsfeedType.Items.Add(new System.Web.UI.WebControls.ListItem("None", "None"));
                cboNewsfeedType.Items.Add(new System.Web.UI.WebControls.ListItem("Private Group", "Private"));
                cboNewsfeedType.Items.Add(new System.Web.UI.WebControls.ListItem("Public Group", "Public"));
            }

            //register the script for this control
            string clientID = "hyB2pTvrL36Y50py8EWj6A";
            string yamScript = @"
                var yamAuth = false;
                var auth = false;
                var groupValid = false;
                $(document).ready(function () {
                    //wire up change event on parent textbox
                    titleChangeCallbacks.push(function () { $('#" + txtGroupName.ClientID + @"').val($('#txtTitle').val()); });
                    validationChecks.push(validateYamModule);

                    //validate group
                    $('#" + txtGroupName.ClientID + @"').blur(function () {
                        var txt = $('#" + txtGroupName.ClientID + @"').val();
                        if (txt.length > 1) {
                            groupSearch(txt, function(results) {
                                var available = true;
                                for (i = 0; i < results.group.length; i++) {
                                    if (results.group[i].full_name.toLowerCase() == txt.toLowerCase()) {
                                        available = false;
                                        break;
                                    }
                                }
                                groupValid = available
                                if (groupValid) {
                                    $('#yamGroupUnavailable').hide();
                                    $('#yamGroupAvailable').show();
                                }
                                else {
                                    $('#yamGroupAvailable').hide();
                                    $('#yamGroupUnavailable').show();
                                }
                            });
                        }
                        else
                            groupValid = false;
                    });

                    yam.config({appId: '" + clientID + @"'});
                    yam.getLoginStatus(function (response) {
                        if (response.authResponse) {
                            yamAuth = true;
                        }

                        //if not authenticated to Yammer, then we will set the yam button to do oauth redirect, else go directly into the app
                        if (!yamAuth) {
                            $('.yamLogin').show();
                            yam.connect.loginButton('.yamLogin', function (resp) {
                                if (!auth) //this is a hack for double return from the login button
                                {
                                    auth = true;
                                    if (resp.authResponse) {
                                        $('.yamLogin').hide();
                                        $('#" + this.hdnYammerAccessToken.ClientID + @"').val(response.access_token.token);
                                        $('#divGroupType').show();
                                    }
                                }
                            });
                        }
                        else {
                            $('.yamLogin').hide();
                            $('#" + this.hdnYammerAccessToken.ClientID + @"').val(response.access_token.token);
                            $('#divGroupType').show();
                        }
                    });
                });

                function cboNewsfeedTypeChanged(ctrl) {
                    if ($(ctrl).val() != 'None') {
                        $('#divGroupName').show();
                        $('#" + txtGroupName.ClientID + @"').focus();
                    }
                    else
                        $('#divGroupName').hide();
                }

                function validateYamModule() {
                    if ($('#" + cboNewsfeedType.ClientID + @"').val() != 'None' && $('#" + txtGroupName.ClientID + @"').val().length > 0 && groupValid)
                        return true;
                    else {
                        $('#" + txtGroupName.ClientID + @"').addClass('invalid');
                        return false;
                    }
                }

                function groupSearch(txt, callback) {
                    yam.request({
                        url: 'https://www.yammer.com/api/v1/autocomplete/ranked?prefix=' + txt + '&models=group:5',
                        method: 'GET',
                        headers: { 'Accept': 'application/json' },
                        success: function (results) {
                            callback(results);
                        },
                        error: function (err) {
                            callback([]);
                        }
                    });
                }";
            ScriptManager.RegisterClientScriptBlock(this, typeof(YammerProvisioningModule), "yamChanged", yamScript, true);
            ScriptManager.RegisterClientScriptBlock(this, typeof(YammerProvisioningModule), "yamScript", "<script id='yamScript' type='text/javascript' data-app-id='" + clientID + "' src='https://assets.yammer.com/platform/yam.js'></script>", false);
        }

        public override void Provision(ClientContext context, Web web)
        {
            //create the group
            createYammerGroup(txtGroupName.Text, (cboNewsfeedType.SelectedValue == "Private") ? true : false);

            //get user details
            var response = getYammerJson(String.Format("https://www.yammer.com/api/v1/users/current.json?access_token={0}", hdnYammerAccessToken.Value));
            JObject oResponse = JObject.Parse(response);
            var network = oResponse.SelectToken("network_domains[0]").ToString();
            var userId = oResponse.SelectToken("id").ToString();

            //get the users groups to check for the group
            response = getYammerJson(String.Format("https://www.yammer.com/api/v1/groups/for_user/{0}.json", userId));
            var groups = JsonConvert.DeserializeObject<List<YamGroup>>(response);
            string groupId = null;
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].full_name.Equals(txtGroupName.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    groupId = groups[i].Id;
                    break;
                }
            }

            if (groupId != null)
            {
                string wpXML = @"
<?xml version='1.0' encoding='utf-8'?>
<webParts>
<webPart xmlns='http://schemas.microsoft.com/WebPart/v3'>
<metaData>
<type name='Microsoft.SharePoint.WebPartPages.ScriptEditorWebPart, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c' />
<importErrorMessage>$Resources:core,ImportErrorMessage;</importErrorMessage>
</metaData>
<data>
<properties>
<property name='Title' type='string'>$Resources:core,ScriptEditorWebPartTitle;</property>
<property name='Description' type='string'>$Resources:core,ScriptEditorWebPartDescription;</property>
<property name='ChromeType' type='chrometype'>None</property>
<property name='Content' type='string'>
<![CDATA[
<div id='embedded-feed' style='height: 500px;'></div>
<script type='text/javascript' src='https://assets.yammer.com/assets/platform_embed.js'></script>
<script type='text/javascript'>  yam.connect.embedFeed({ container: '#embedded-feed', network: '" + network + @"', feedType: 'group', feedId: '" + groupId + @"'}); </script>

]]>
</property>
</properties>
</data>
</webPart>
</webParts>";
                wpXML = wpXML.Replace("\r\n", "");

                //get the web part page
                var list = web.Lists.GetByTitle("Site Pages");
                CamlQuery camlQuery = new CamlQuery();
                var items = list.GetItems(camlQuery);
                context.Load(items, i =>
                    i.Include(item => item.DisplayName, item => item["WikiField"]).Where(item => item.DisplayName == "Home"));
                context.ExecuteQuery();

                //remove the sitefeed
                var wikiPage = items[0].File;
                LimitedWebPartManager limitedWebPartManager = wikiPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
                var wps = limitedWebPartManager.WebParts;
                context.Load(wps);
                context.ExecuteQuery();
                for (int i = 0; i < wps.Count; i++)
                {
                    var wp = wps[i].WebPart;
                    context.Load(wp);
                    context.ExecuteQuery();

                    if (wp.ZoneIndex == 1)
                    {
                        wps[i].DeleteWebPart();
                        context.ExecuteQuery();
                        break;
                    }
                }

                //add the yammer embed
                WebPartDefinition wpd = limitedWebPartManager.ImportWebPart(wpXML);
                var newWP = limitedWebPartManager.AddWebPart(wpd.WebPart, "wpz", 0);
                context.Load(newWP);
                context.ExecuteQuery();

                // Create reference to WebPart in HTML
                string wikiField = items[0]["WikiField"] as string;
                XmlDocument xd = new XmlDocument();
                xd.PreserveWhitespace = true;
                xd.LoadXml(wikiField);
                XmlElement layoutsZoneInner = xd.SelectSingleNode("div/table/tbody/tr[2]/td/div/div") as XmlElement;

                //create wrapper
                XmlElement wpWrapper = xd.CreateElement("div");
                layoutsZoneInner.AppendChild(wpWrapper);
                XmlAttribute attribute = xd.CreateAttribute("class");
                wpWrapper.Attributes.Append(attribute);
                attribute.Value = "ms-rtestate-read ms-rte-wpbox";

                //create inner elements
                XmlElement div1 = xd.CreateElement("div");
                wpWrapper.AppendChild(div1);
                div1.IsEmpty = false;
                attribute = xd.CreateAttribute("class");
                div1.Attributes.Append(attribute);
                attribute.Value = "ms-rtestate-notify ms-rtestate-read " + newWP.Id.ToString("D");
                attribute = xd.CreateAttribute("id");
                div1.Attributes.Append(attribute);
                attribute.Value = "div_" + newWP.Id.ToString("D");

                XmlElement div2 = xd.CreateElement("div");
                wpWrapper.AppendChild(div2);
                div2.IsEmpty = false;
                attribute = xd.CreateAttribute("class");
                div2.Attributes.Append(attribute);
                attribute.Value = "ms-rtestate-read";
                attribute = xd.CreateAttribute("style");
                div2.Attributes.Append(attribute);
                attribute.Value = "display:none";
                attribute = xd.CreateAttribute("id");
                div2.Attributes.Append(attribute);
                attribute.Value = "vid_" + newWP.Id.ToString("D");

                // Update
                items[0]["WikiField"] = xd.OuterXml;
                items[0].Update();
                context.ExecuteQuery();
            }
        }

        private void createYammerGroup(string name, bool isPrivate) 
        {
            var url = String.Format("https://www.yammer.com/api/v1/groups.json?name={0}&private={1}", name, isPrivate.ToString().ToLower());
            postYammerJson(url); 
        }

        private string getYammerJson(string url) 
        {
            //make the request
            string json = null;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Headers.Add("Authorization", "Bearer" + " " + hdnYammerAccessToken.Value);
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                json = reader.ReadToEnd();
            }
            return json;
        }

        private string postYammerJson(string url)
        {
            //make the request
            string json = null;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.Headers.Add("Authorization", "Bearer" + " " + hdnYammerAccessToken.Value);
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                json = reader.ReadToEnd();
            }
            return json;
        }
    }

    public class YamGroup
    {
        public string Id { get; set; }
        public string full_name { get; set; }
    }
}