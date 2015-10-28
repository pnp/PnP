using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Diagnostics;
using OfficeDevPnP.Core.Entities;
using CorporateEvents;

namespace Core.ConnectedAngularAppsV2Web.Models
{
    class DataInitializer {
        // used to ensure that the initializer doesn't run while already in process
        static bool _isInitializing = false;
        SharePointContext _spContext { get; set; }

        public DataInitializer(SharePointContext context) {
            _spContext = context;
        }

        public void Initialize(bool createDefaultData) {
            if (_spContext == null)
                throw new InvalidOperationException("HttpContext is not initialized.");

            if (_isInitializing)
                return;

            try {
                _isInitializing = true;

                // ** Events setup process **
                using (var clientContext = _spContext.CreateUserClientContextForSPHost()) {
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    // Create events list
                    var listId = CreateList(web, ListDetails.EventsListName, ListDetails.EventsListDesc, ListDetails.EventsUrl);
                    // Create content type for events list
                    var contentTypeId = CreateContentType(web, ContentTypes.CorporateEvent, ContentTypes.CorporateEventContentTypeDesc, ContentTypes.CorporateEventContentTypeGroup, ContentTypes.CorporateEventContentTypeId);
                    // Create fields for events list
                    ApplyListSchema(web, contentTypeId, ListDetails.EventsListName);
                }

                // ** Registration setup process **
                using (var clientContext = _spContext.CreateUserClientContextForSPHost()) {
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    // Create registration list
                    var listId = CreateList(web, ListDetails.RegistrationListName, ListDetails.RegistrationListDesc, ListDetails.RegistrationUrl);
                    // Create content type for registration list
                    var contentTypeId = CreateContentType(web, ContentTypes.EventRegistration, ContentTypes.EventRegistrationContentTypeDesc, ContentTypes.EventRegistrationContentTypeGroup, ContentTypes.EventRegistrationContentTypeId);
                    // Create fields for registration list
                    ApplyListSchema(web, contentTypeId, ListDetails.RegistrationListName);
                }

                // ** Speakers setup process **
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    // Create speakers list
                    var listId = CreateList(web, ListDetails.SpeakersListName, ListDetails.SpeakersListDesc, ListDetails.SpeakersUrl);
                    // Create content type for speakers list
                    var contentTypeId = CreateContentType(web, ContentTypes.EventSpeaker, ContentTypes.EventSpeakerContentTypeDesc, ContentTypes.EventSpeakerContentTypeGroup, ContentTypes.EventSpeakerContentTypeId);
                    // Create fields for speakers list
                    ApplyListSchema(web, contentTypeId, ListDetails.SpeakersListName);
                }

                // ** Sessions setup process **
                using (var clientContext = _spContext.CreateUserClientContextForSPHost()) {
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    // Create sessions list
                    var listId = CreateList(web, ListDetails.SessionsListName, ListDetails.SessionsListDesc, ListDetails.SessionsUrl);
                    // Create content type for sessions list
                    var contentTypeId = CreateContentType(web, ContentTypes.EventSession, ContentTypes.EventSessionContentTypeDesc, ContentTypes.EventSessionContentTypeGroup, ContentTypes.EventSessionContentTypeId);
                    // Create fields for sessions list
                    ApplyListSchema(web, contentTypeId, ListDetails.SessionsListName);
                }
                
                if (createDefaultData) {
                    using (var clientContext = _spContext.CreateUserClientContextForSPHost()) {
                        var web = clientContext.Web;
                        clientContext.Load(web);
                        clientContext.ExecuteQuery();
                        CreateSampleData(web);
                    }
                }

                _isInitializing = false;
            }
            catch(Exception ex) {
                // enable the configuration to run again if the initializer fails
                _isInitializing = false;
                throw;
            }
        }

        #region [ List schemas ]
        private void ApplyListSchema(Web web, string contentTypeId, string listTitle) {
            IEnumerable<Field> fieldsList;

           if (listTitle == ListDetails.EventsListName) {
                fieldsList = CreateEventsSiteColumns(web);
            }
            else if (listTitle == ListDetails.SpeakersListName) {
                fieldsList = CreateSpeakersSiteColumns(web);
            }
            else {
                fieldsList = CreateSessionsSiteColumns(web);
            }

            // Bind fields to content type
            BindFieldsToContentType(web, contentTypeId, fieldsList);

            // Bind content type to events list
            AddContentTypeToList(web, contentTypeId, listTitle, fieldsList);
        }
        #endregion

        #region [ Site Columns ]        

         private static IEnumerable<Field> CreateEventsSiteColumns(Web web) {
            var fields = new List<Field>();
            var fieldsXml = new Dictionary<string,string>();
            var context = web.Context;

            #region Create Events List Site Columns
            // Build site columns for events list
            fieldsXml.Add(Event.FIELD_DESCRIPTION,
                FormatField(
                new Guid("{A385CEFA-8C4D-49EF-A586-C35E9C539CC5}"),
                Event.FIELD_DESCRIPTION,
                FieldType.Note,
                "Event Description",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "NumLines='6' Customization=''"));

            fieldsXml.Add(Event.FIELD_REGISTERED_EVENT_ID,
                FormatField(
                Event.RegisteredEventFieldId,
                Event.FIELD_REGISTERED_EVENT_ID,
                FieldType.Text,
                "Event ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Event.FIELD_DATE,
                FormatField(
                new Guid("{E71EE5E4-FD31-4478-A8B0-1839607D5419}"),
                Event.FIELD_DATE,
                FieldType.DateTime,
                "Event Date",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Event.FIELD_LOCATION,
                FormatField(
                new Guid("{B74D15A6-A30A-4499-8085-23D8620BE7C2}"),
                Event.FIELD_LOCATION,
                FieldType.Text,
                "Event Location",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Event.FIELD_CONTACT_EMAIL,
                FormatField(
                new Guid("{6E6F0A1F-0BB2-4A95-AC62-EF8EF54FE137}"),
                Event.FIELD_CONTACT_EMAIL,
                FieldType.Text,
                "Event Contact",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "MaxLength='255' Customization=''"));

            fieldsXml.Add(Event.FIELD_IMAGE_URL,
                FormatField(
                new Guid("{4B2A0C31-CDB9-44C9-A0E1-B8F3D3A505B4}"),
                Event.FIELD_IMAGE_URL,
                FieldType.URL,
                "Event Image",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Event.FIELD_CATEGORY,
                FormatField(
                new Guid("{C94EF074-B098-4E6F-A945-C24E4C24DA0F}"),
                Event.FIELD_CATEGORY,
                FieldType.Choice,
                "Event Category",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Event.FIELD_STATUS,
                FormatField(
                new Guid("{A2B81C4E-6B17-4BD7-B7AF-F6CB2A0697CE}"),
                Event.FIELD_STATUS,
                FieldType.Choice,
                "Event Status",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            TryCreateFields(web, fields, fieldsXml);

            var categoryField = web.Fields.GetFieldByName<FieldChoice>(Event.FIELD_CATEGORY);
            categoryField.Choices = new string[] { "Featured", "Leadership", "General" };
            categoryField.DefaultValue = "General";
            categoryField.Update();

            var statusField = web.Fields.GetFieldByName<FieldChoice>(Event.FIELD_STATUS);
            statusField.Choices = new string[] { "Active", "Cancelled", "Expired" };
            categoryField.DefaultValue = "Active";
            statusField.Update();

            context.Load(web.Fields);
            context.ExecuteQuery();
            #endregion

            return fields;
        }

        private static IEnumerable<Field> CreateSessionsSiteColumns(Web web) {
            var fields = new List<Field>();
            var fieldsXml = new Dictionary<string,string>();

            #region Create Sessions List Site Columns

            // Build site columns for event sessions list    
            fieldsXml.Add(Session.FIELD_SESSIONDESCRIPTION,
                FormatField(
                new Guid("{850034CB-02F0-4051-BD3F-E34426BF319E}"),
                Session.FIELD_SESSIONDESCRIPTION,
                FieldType.Note,
                "Session Description",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "NumLines='6' Customization=''"));

            fieldsXml.Add(Session.FIELD_SESSIONDATE,
                FormatField(
                new Guid("{3C40CB12-D533-4AB1-9D69-43306D8A7D41}"),
                Session.FIELD_SESSIONDATE,
                FieldType.DateTime,
                "Session Date",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Session.FIELD_SESSIONID,
                FormatField(
                new Guid("{3779D32B-35DF-46AB-84E4-AA969D165AF1}"),
                Session.FIELD_SESSIONID,
                FieldType.Text,
                "Session ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Session.FIELD_SESSIONIMAGEURL,
                FormatField(
                new Guid("{918CAE08-D4B0-43BC-B5D7-118FEA8586E3}"),
                Session.FIELD_SESSIONIMAGEURL,
                FieldType.URL,
                "Session Image",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Session.FIELD_SESSIONSTATUS,
                FormatField(
                new Guid("{D6AF68B1-BEC2-4E98-9425-FC7E27ADA302}"),
                Session.FIELD_SESSIONSTATUS,
                FieldType.Choice,
                "Session Status",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Event.FIELD_REGISTERED_EVENT_ID,
                FormatField(
                Event.RegisteredEventFieldId,
                Event.FIELD_REGISTERED_EVENT_ID,
                FieldType.Text,
                "Event ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            fieldsXml.Add(Speaker.FIELD_ID, FormatField(
                new Guid("{46255703-8E66-4CC8-892E-CA6DBE7750C0}"),
                Speaker.FIELD_ID,
                FieldType.Text,
                "Speaker ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));
            
            TryCreateFields(web, fields, fieldsXml);

            var statusField = web.Fields.GetFieldByName<FieldChoice>(Session.FIELD_SESSIONSTATUS);
            statusField.Choices = new string[] { "Active", "Cancelled", "Expired" };
            statusField.Update();
            web.Context.ExecuteQuery();
            #endregion

            return fields;
        }

        private static IEnumerable<Field> CreateSpeakersSiteColumns(Web web) {
            var fields = new List<Field>();
            var fieldsXml = new Dictionary<string, string>();

            #region Create Speakers List Site Columns

            // Build site columns for event speakers list
            fieldsXml.Add(Speaker.FIELD_FIRSTNAME, FormatField(
                new Guid("{063EA18D-F9A1-482C-960F-34BACFD3F824}"),
                Speaker.FIELD_FIRSTNAME,
                FieldType.Text,
                "Speaker First Name",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "MaxLength='50' Customization=''"));

            fieldsXml.Add(Speaker.FIELD_LASTNAME,FormatField(
                new Guid("{EDA5E263-5CED-4D80-BF8B-1292FA968A4E}"),
                Speaker.FIELD_LASTNAME,
                FieldType.Text,
                "Speaker Last Name",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "MaxLength='50' Customization=''"));

            fieldsXml.Add(Speaker.FIELD_EMAIL,FormatField(
                new Guid("{4CCF3CAE-83B0-459B-9D35-29BF1014EDDA}"),
                Speaker.FIELD_EMAIL,
                FieldType.Text,
                "Speaker Email",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "MaxLength='100' Customization=''"));

            fieldsXml.Add(Speaker.FIELD_ID,FormatField(
                new Guid("{46255703-8E66-4CC8-892E-CA6DBE7750C0}"),
                Speaker.FIELD_ID,
                FieldType.Text,
                "Speaker ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                "Customization=''"));

            TryCreateFields(web, fields, fieldsXml);
            #endregion

            return fields;
        } 
        #endregion

        static string FormatField(Guid fieldId, string internalName, FieldType fieldType, string displayName, string groupName, string attributes) {
            var fieldCI = new FieldCreationInformation(fieldType)
            {
                Id = fieldId,
                InternalName = internalName,
                DisplayName = displayName,
                AddToDefaultView = true,
                Group = groupName
            };

            return FieldAndContentTypeExtensions.FormatFieldXml(fieldCI);
        }

        private static void TryCreateFields(Web web, List<Field> fields, Dictionary<string, string> fieldsXml) {
            web.Context.Load(web.Fields);
            web.Context.ExecuteQuery();

            foreach (var key in fieldsXml.Keys) {
                var field = web.Fields.GetFieldByName<Field>(key);
                if (field == null) {
                    field = web.Fields.AddFieldAsXml(fieldsXml[key], true, AddFieldOptions.DefaultValue);
                    field.StaticName = key;
                }
                fields.Add(field);
            }
            web.Context.ExecuteQuery();

            foreach (var field in fields) {
                web.Context.Load(field);
            }
            web.Context.ExecuteQuery();
        }
       
        protected string CreateList(Web web, string listName, string listDescription, string listUrl) {
            try {
                var testList = web.GetListByTitle(listName);

                if (testList != null)
                    // leave if the list is valid
                    return testList.Id.ToString();
            }
            catch { }

            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = listName;
            creationInfo.Description = listDescription;
            creationInfo.Url = listUrl;
            creationInfo.TemplateType = (int)ListTemplateType.GenericList;

            List list = web.Lists.Add(creationInfo);
            web.Context.Load(list);
            web.Context.ExecuteQuery();
            return list.Id.ToString();
        }

        static TField CreateField<TField>(Web web, Guid fieldId, string internalName, FieldType fieldType, string displayName, string groupName, string attributes, bool executeQuery = true) where TField : Field{
            if (!web.Fields.ServerObjectIsNull.HasValue ||
                web.Fields.ServerObjectIsNull.Value) {
                web.Context.Load(web.Fields);
                web.Context.ExecuteQuery();
            }

            var fieldCI = new FieldCreationInformation(fieldType)
            {
                Id = fieldId,
                InternalName = internalName,
                DisplayName = displayName,
                AddToDefaultView = true,
                Group = groupName                
            };

            var field = web.Fields.FirstOrDefault(f => f != null && f.StaticName == internalName);
            if (field == null)

                field = web.CreateField<TField>(fieldCI, executeQuery);

            return web.Context.CastTo<TField>(field);
        }
        
        private string CreateContentType(Web web, string name, string description, string group, string id) {
            if (web.ContentTypeExistsById(id))
                return id;

            // add the description
            var ctypeCreationInfo = new ContentTypeCreationInformation() {
                Id = id,
                Description = description,
                Group = group,
                Name = name
            };
            web.ContentTypes.Add(ctypeCreationInfo);
            web.Context.ExecuteQuery();
            return id;
        }

        private void AddContentTypeToList(Web web, string contentTypeId, string listName, IEnumerable<Field> fields) {
            Debug.WriteLine("CType: {0}, List: {1}", contentTypeId, listName);
            var list = web.GetListByTitle(listName);

            if (!list.ContentTypeExistsById(contentTypeId))
                web.AddContentTypeToListById(listName, contentTypeId);

            list.ContentTypesEnabled = true;
            list.Update();
            list.RefreshLoad();

            var contentType = web.ContentTypes.GetById(contentTypeId);
            var defaultView = list.DefaultView;
            web.Context.Load(contentType);
            web.Context.Load(defaultView);
            web.Context.Load(defaultView.ViewFields);
            web.Context.ExecuteQuery();

            foreach (var field in fields) {
                // add the fields to the default view
                if (field.FieldTypeKind != FieldType.Note &&
                    !defaultView.ViewFields.Contains(field.InternalName)) {
                    defaultView.ViewFields.Add(field.InternalName);
                    Debug.WriteLine("  added to default view");
                }

                defaultView.Update();
            }
            web.Context.ExecuteQuery();

            // Item content type
            DeleteContentTypeFromList(web, list, "Item");
        }

        private void BindFieldsToContentType(Web web, string contentTypeId, IEnumerable<Field> fields) {
            var contentType = web.GetContentTypeById(contentTypeId);

            web.Context.Load(web.Fields);
            web.Context.Load(contentType.FieldLinks);
            web.Context.ExecuteQuery();

            var missingFields = fields.Except(from f in fields
                                              join fl in contentType.FieldLinks on f.StaticName equals fl.Name
                                              select f);

            foreach (var field in missingFields) {
                var fieldLinkCreationInfo = new FieldLinkCreationInformation() {
                    Field = field
                };
                contentType.FieldLinks.Add(fieldLinkCreationInfo);
            }
            contentType.Update(false);
            contentType.Context.ExecuteQuery();
        }

        private static void DeleteContentTypeFromList(Web web, List list, string contentTypeName) {
            ContentType ct = list.GetContentTypeByName(contentTypeName);

            if (ct != null) {
                ct.DeleteObject();
                web.Context.ExecuteQuery();
            }
        }

        private static void CreateSampleData(Web web) {
            var context = web.Context;
            var host = "https://" + HttpContext.Current.Request.Url.Authority;

            #region [ Events Sample Data ]
            // Setup events
            var event1 = new Event()
            {
                Title = "Corporate Event 1",
                Category = "Featured",
                ContactEmail = "eventadmin@domain.com",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras sit amet augue in dolor dapibus feugiat in eu odio. Proin vel egestas purus. Integer sit amet orci rhoncus, elementum nibh sit amet, maximus dui. Vivamus rutrum neque et massa hendrerit, varius consequat quam efficitur. Quisque aliquam pellentesque quam, a bibendum nibh dignissim sit amet. Curabitur accumsan tincidunt lectus et tincidunt.",
                ImageUrl = host + "/Images/company-events1.jpg",
                Location = "Pittsburgh, PA",
                Status = "Active",
                RegisteredEventId = "EVT001",
                EventDate = DateTime.Today.AddDays(25)
            };

            var event2 = new Event()
            {
                Title = "Corporate Event 2",
                Category = "Featured",
                ContactEmail = "eventadmin@domain.com",
                Description = "Vestibulum ex mauris, feugiat in vehicula id, congue eleifend elit. Morbi orci quam, mattis sit amet nisl sed, dictum fermentum velit. Quisque rhoncus, arcu vitae dignissim tempus, nisl felis volutpat ipsum, non lobortis tellus lectus at mauris. Fusce porta, lectus feugiat egestas fringilla, dui velit tincidunt est, nec congue ligula urna a felis. Nam vitae ullamcorper lectus. Sed vitae justo felis.",
                ImageUrl = host + "/Images/company-events2.jpg",
                Location = "Helsinki, Finland",
                Status = "Active",
                RegisteredEventId = "EVT002",
                EventDate = DateTime.Today.AddDays(45)
            };

            var event3 = new Event()
            {
                Title = "Corporate Event 3",
                Category = "Featured",
                ContactEmail = "eventadmin@domain.com",
                Description = "Vivamus scelerisque lectus et sapien mollis, ut vestibulum nunc vulputate. Nullam sed quam felis. Praesent sit amet egestas nunc, nec aliquam eros. Maecenas et nisl dapibus, varius metus ac, luctus quam. Donec vitae justo vitae nisi placerat ultrices nec sed ante.",
                ImageUrl = host + "/Images/company-events3.jpg",
                Location = "Chicago, IL",
                Status = "Active",
                RegisteredEventId = "EVT003",
                EventDate = DateTime.Today.AddDays(60)
            };
            event1.Save(web);
            event2.Save(web);
            event3.Save(web);
            
            #endregion

            #region [ Sessions Sample Data ]
            var session1 = new Session()
            {
                Title = "JDP Overview",
                SessionDescription = "Overview of the Joint Development Program",
                SessionImageUrl = host + "/Images/company-events1.jpg",                
                Status = "Active",
                SessionId = "EVT001-SESS001",
                SessionDate = DateTime.Today.AddDays(25),
                RegisteredEventId = "EVT001",
                SpeakerId = "SPKR001"
            };
            var session2 = new Session()
            {
                Title = "FTC 2 CAM",
                SessionDescription = "Deep dive session on moving from full trust code to the cloud app model.",
                SessionImageUrl = host + "/Images/company-events1.jpg",
                Status = "Active",
                SessionId = "EVT001-SESS002",
                SessionDate = DateTime.Today.AddDays(25),
                RegisteredEventId = "EVT001",
                SpeakerId = "SPKR002"
            };
            var session3 = new Session()
            {
                Title = "AngularJS",
                SessionDescription = "Overview of AngularJS",
                SessionImageUrl = host + "/Images/company-events1.jpg",
                Status = "Active",
                SessionId = "EVT002-SESS001",
                SessionDate = DateTime.Today.AddDays(25),
                RegisteredEventId = "EVT002",
                SpeakerId = "SPKR003"
            };
            var session4 = new Session()
            {
                Title = "SignalR",
                SessionDescription = "ASP.NET SignalR is a new library for ASP.NET developers that makes it incredibly simple to add real-time web functionality to your applications. What is 'real-time web' functionality? It's the ability to have your server-side code push content to the connected clients as it happens, in real-time.",
                SessionImageUrl = host + "/Images/company-events1.jpg",
                Status = "Active",
                SessionId = "EVT002-SESS002",
                SessionDate = DateTime.Today.AddDays(25),
                RegisteredEventId = "EVT002",
                SpeakerId = "SPKR004"
            };
            var session5 = new Session()
            {
                Title = "Apps Overview",
                SessionDescription = "Learn what apps for SharePoint are, why you should build them, and the concepts that are fundamental to building them in SharePoint 2013. This session describes the basic concepts about how to build apps in SharePoint 2013.",
                SessionImageUrl = host + "/Images/company-events1.jpg",
                Status = "Active",
                SessionId = "EVT003-SESS001",
                SessionDate = DateTime.Today.AddDays(25),
                RegisteredEventId = "EVT003",
                SpeakerId = "SPKR001"
            };
            var session6 = new Session()
            {
                Title = "Provider-hosted Apps",
                SessionDescription = "Learn how to create a basic provider-hosted app for SharePoint with the Office Developer Tools for Visual Studio 2013, how to interact with SharePoint 2013 sites by using the SharePoint CSOM, and how to implement OAuth in an app for SharePoint.",
                SessionImageUrl = host + "/Images/company-events1.jpg",
                Status = "Active",
                SessionId = "EVT003-SESS002",
                SessionDate = DateTime.Today.AddDays(25),
                RegisteredEventId = "EVT003",
                SpeakerId = "SPKR002"
            };
            var session7 = new Session()
            {
                Title = "SharePoint-hosted Apps",
                SessionDescription = "Learn how to create a basic SharePoint-hosted app for SharePoint with the Office Developer Tools for Visual Studio.",
                SessionImageUrl = host + "/Images/company-events1.jpg",
                Status = "Active",
                SessionId = "EVT003-SESS003",
                SessionDate = DateTime.Today.AddDays(25),
                RegisteredEventId = "EVT003",
                SpeakerId = "SPKR003"
            };

            session1.Save(web);
            session2.Save(web);
            session3.Save(web);
            session4.Save(web);
            session5.Save(web);
            session6.Save(web);
            session7.Save(web);
            #endregion

            #region [ Speakers Sample Data ]
            var speaker1 = new Speaker()
            {
                Title = "Ricky Bobby",
                SpeakerFirstName = "Ricky",
                SpeakerLastName = "Bobby",
                SpeakerEmail = "ricky@rickybobbyconsulting.com",                
                SpeakerId = "SPKR001"
            };
            var speaker2 = new Speaker()
            {
                Title = "Brian Cox",
                SpeakerFirstName = "Brian",
                SpeakerLastName = "Cox",
                SpeakerEmail = "brian.cox@contosoconsulting.com",
                SpeakerId = "SPKR002"
            };
            var speaker3 = new Speaker()
            {
                Title = "Steve Stevens",
                SpeakerFirstName = "Steve",
                SpeakerLastName = "Stevens",
                SpeakerEmail = "steve.stevens@contosoconsulting.com",
                SpeakerId = "SPKR003"
            };
            var speaker4 = new Speaker()
            {
                Title = "Jane Smith",
                SpeakerFirstName = "Jane",
                SpeakerLastName = "Smith",
                SpeakerEmail = "jane.smith@contosoconsulting.com",
                SpeakerId = "SPKR004"
            };

            speaker1.Save(web);
            speaker2.Save(web);
            speaker3.Save(web);
            speaker4.Save(web);
            #endregion           
        }
    }
}