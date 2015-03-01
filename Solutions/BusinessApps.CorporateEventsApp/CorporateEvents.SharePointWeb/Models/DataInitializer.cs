using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Diagnostics;
using OfficeDevPnP.Core.Entities;

namespace CorporateEvents.SharePointWeb.Models {
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

                // ** Speakers setup process **
                using (var clientContext = _spContext.CreateUserClientContextForSPHost()) {
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

            if (listTitle == ListDetails.RegistrationListName) {
                    fieldsList = CreateRegistrationSiteColumns(web);
            }
            else if (listTitle == ListDetails.EventsListName) {
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
        private static IEnumerable<Field> CreateRegistrationSiteColumns(Web web) {
            var fields = new List<Field>();
            var fieldsXml = new Dictionary<string, string>();

            #region Create Registration Site Columns

            // Build site columns for event registration list
            List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("ReadOnly", "TRUE"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Registration.FIELD_DATE,
                FormatField(
                new Guid("{E08894EF-5770-4DC4-936C-B9ED1E901F85}"),
                Registration.FIELD_DATE,
                FieldType.DateTime,
                "Registration Date",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("MaxLength", "50"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Registration.FIELD_FIRST_NAME,
                FormatField(
                new Guid("{16059CB2-353A-4FF3-A8CE-9E43C3D56C7D}"),
                Registration.FIELD_FIRST_NAME,
                FieldType.Text,
                "First Name",
                ListDetails.CorporateEventsSiteColumnsGroup,
                true,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("MaxLength", "50"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Registration.FIELD_LAST_NAME,
                FormatField(
                new Guid("{14C25003-ABC4-48CE-A4FB-3C7631CF4FBC}"),
                Registration.FIELD_LAST_NAME,
                FieldType.Text,
                "Last Name",
                ListDetails.CorporateEventsSiteColumnsGroup,
                true,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("MaxLength", "25"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Registration.FIELD_USER_ID,
                FormatField(
                new Guid("{695DE7E3-2BAD-4CA2-A10B-DCF4DED6626B}"),
                Registration.FIELD_USER_ID,
                FieldType.Text,
                "User Id",
                ListDetails.CorporateEventsSiteColumnsGroup,
                true,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Registration.FIELD_USER_EMAIL,
                FormatField(
                new Guid("{07042CA2-49D9-4C31-A932-47AF619EF8E5}"),
                Registration.FIELD_USER_EMAIL,
                FieldType.Text,
                "Email",
                ListDetails.CorporateEventsSiteColumnsGroup,
                true,
                additionalAttributes));

            TryCreateFields(web, fields, fieldsXml);

            var eventIdField = web.GetFieldById<Field>(Event.RegisteredEventFieldId);
            eventIdField.Required = true;
            eventIdField.Update();
            fields.Add(eventIdField);
            #endregion

            return fields;
        }   

         private static IEnumerable<Field> CreateEventsSiteColumns(Web web) {
            var fields = new List<Field>();
            var fieldsXml = new Dictionary<string,string>();
            var context = web.Context;

            #region Create Events List Site Columns
            // Build site columns for events list


            List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string,string>("NumLines", "6"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_DESCRIPTION,
                FormatField(
                new Guid("{A385CEFA-8C4D-49EF-A586-C35E9C539CC5}"),
                Event.FIELD_DESCRIPTION,
                FieldType.Note,
                "Event Description",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_REGISTERED_EVENT_ID,
                FormatField(
                Event.RegisteredEventFieldId,
                Event.FIELD_REGISTERED_EVENT_ID,
                FieldType.Text,
                "Event ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));


            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_DATE,
                FormatField(
                new Guid("{E71EE5E4-FD31-4478-A8B0-1839607D5419}"),
                Event.FIELD_DATE,
                FieldType.DateTime,
                "Event Date",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_LOCATION,
                FormatField(
                new Guid("{B74D15A6-A30A-4499-8085-23D8620BE7C2}"),
                Event.FIELD_LOCATION,
                FieldType.Text,
                "Event Location",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("MaxLength", "255"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_CONTACT_EMAIL,
                FormatField(
                new Guid("{6E6F0A1F-0BB2-4A95-AC62-EF8EF54FE137}"),
                Event.FIELD_CONTACT_EMAIL,
                FieldType.Text,
                "Event Contact",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_IMAGE_URL,
                FormatField(
                new Guid("{4B2A0C31-CDB9-44C9-A0E1-B8F3D3A505B4}"),
                Event.FIELD_IMAGE_URL,
                FieldType.URL,
                "Event Image",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_CATEGORY,
                FormatField(
                new Guid("{C94EF074-B098-4E6F-A945-C24E4C24DA0F}"),
                Event.FIELD_CATEGORY,
                FieldType.Choice,
                "Event Category",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Event.FIELD_STATUS,
                FormatField(
                new Guid("{A2B81C4E-6B17-4BD7-B7AF-F6CB2A0697CE}"),
                Event.FIELD_STATUS,
                FieldType.Choice,
                "Event Status",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

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
            List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("NumLines", "6"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));

            // Build site columns for event sessions list    
            fieldsXml.Add(Session.FIELD_DESCRIPTION,
                FormatField(
                new Guid("{850034CB-02F0-4051-BD3F-E34426BF319E}"),
                Session.FIELD_DESCRIPTION,
                FieldType.Note,
                "Session Description",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Session.FIELD_DATE,
                FormatField(
                new Guid("{3C40CB12-D533-4AB1-9D69-43306D8A7D41}"),
                Session.FIELD_DATE,
                FieldType.DateTime,
                "Session Date",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Session.FIELD_ID,
                FormatField(
                new Guid("{3779D32B-35DF-46AB-84E4-AA969D165AF1}"),
                Session.FIELD_ID,
                FieldType.Number,
                "Session ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Session.FIELD_IMAGEURL,
                FormatField(
                new Guid("{918CAE08-D4B0-43BC-B5D7-118FEA8586E3}"),
                Session.FIELD_IMAGEURL,
                FieldType.URL,
                "Session Image",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Session.FIELD_STATUS,
                FormatField(
                new Guid("{D6AF68B1-BEC2-4E98-9425-FC7E27ADA302}"),
                Session.FIELD_STATUS,
                FieldType.Choice,
                "Session Status",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));
            
            TryCreateFields(web, fields, fieldsXml);

            var statusField = web.Fields.GetFieldByName<FieldChoice>(Session.FIELD_STATUS);
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
            List<KeyValuePair<string, string>> additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("MaxLength", "50"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", "")); 
            fieldsXml.Add(Speaker.FIELD_FIRSTNAME, FormatField(
                new Guid("{063EA18D-F9A1-482C-960F-34BACFD3F824}"),
                Speaker.FIELD_FIRSTNAME,
                FieldType.Text,
                "Speaker First Name",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("MaxLength", "50"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Speaker.FIELD_LASTNAME,FormatField(
                new Guid("{EDA5E263-5CED-4D80-BF8B-1292FA968A4E}"),
                Speaker.FIELD_LASTNAME,
                FieldType.Text,
                "Speaker Last Name",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("MaxLength", "100"));
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Speaker.FIELD_EMAIL,FormatField(
                new Guid("{4CCF3CAE-83B0-459B-9D35-29BF1014EDDA}"),
                Speaker.FIELD_EMAIL,
                FieldType.Text,
                "Speaker Email",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            additionalAttributes = new List<KeyValuePair<string, string>>();
            additionalAttributes.Add(new KeyValuePair<string, string>("Customization", ""));
            fieldsXml.Add(Speaker.FIELD_ID,FormatField(
                new Guid("{46255703-8E66-4CC8-892E-CA6DBE7750C0}"),
                Speaker.FIELD_ID,
                FieldType.Text,
                "Speaker ID",
                ListDetails.CorporateEventsSiteColumnsGroup,
                false,
                additionalAttributes));

            TryCreateFields(web, fields, fieldsXml);
            #endregion

            return fields;
        } 
        #endregion

        static string FormatField(Guid fieldId, string internalName, FieldType fieldType, string displayName, string groupName, bool required, IEnumerable<KeyValuePair<string, string>> attributes)
        {

            FieldCreationInformation fldCreate = new FieldCreationInformation(fieldType)
            {
                Id = fieldId,
                InternalName = internalName,
                DisplayName = displayName,
                Group = groupName,    
                AdditionalAttributes = attributes,
                Required = required,
            };

            return FieldAndContentTypeExtensions.FormatFieldXml(fldCreate);
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

            var event1 = new Event() {
                Title = "Corporate Event 1",
                Category = "Featured",
                ContactEmail = "eventadmin@domain.com",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras sit amet augue in dolor dapibus feugiat in eu odio. Proin vel egestas purus. Integer sit amet orci rhoncus, elementum nibh sit amet, maximus dui. Vivamus rutrum neque et massa hendrerit, varius consequat quam efficitur. Quisque aliquam pellentesque quam, a bibendum nibh dignissim sit amet. Curabitur accumsan tincidunt lectus et tincidunt.",
                ImageUrl = host + "/Images/company-events1.jpg",
                Location = "Pittsburgh, PA",
                Status = EventStatus.Active,
                RegisteredEventId = "EVT001",
                EventDate = DateTime.Today.AddDays(25)
            };

            var event2 = new Event() {
                Title = "Corporate Event 2",
                Category = "Featured",
                ContactEmail = "eventadmin@domain.com",
                Description = "Vestibulum ex mauris, feugiat in vehicula id, congue eleifend elit. Morbi orci quam, mattis sit amet nisl sed, dictum fermentum velit. Quisque rhoncus, arcu vitae dignissim tempus, nisl felis volutpat ipsum, non lobortis tellus lectus at mauris. Fusce porta, lectus feugiat egestas fringilla, dui velit tincidunt est, nec congue ligula urna a felis. Nam vitae ullamcorper lectus. Sed vitae justo felis.",
                ImageUrl = host + "/Images/company-events2.jpg",
                Location = "Helsinki, Finland",
                Status = EventStatus.Active,
                RegisteredEventId = "EVT002",
                EventDate = DateTime.Today.AddDays(45)
            };

            var event3 = new Event() {
                Title = "Corporate Event 3",
                Category = "Featured",
                ContactEmail = "eventadmin@domain.com",
                Description = "Vivamus scelerisque lectus et sapien mollis, ut vestibulum nunc vulputate. Nullam sed quam felis. Praesent sit amet egestas nunc, nec aliquam eros. Maecenas et nisl dapibus, varius metus ac, luctus quam. Donec vitae justo vitae nisi placerat ultrices nec sed ante.",
                ImageUrl = host + "/Images/company-events3.jpg",
                Location = "Chicago, IL",
                Status = EventStatus.Active,
                RegisteredEventId = "EVT003",
                EventDate = DateTime.Today.AddDays(60)
            };

            var event4 = new Event() {
                Title = "Corporate Event 4",
                Category = "General",
                ContactEmail = "eventadmin@domain.com",
                Description = "Vivamus scelerisque lectus et sapien mollis, ut vestibulum nunc vulputate. Nullam sed quam felis. Praesent sit amet egestas nunc, nec aliquam eros. Maecenas et nisl dapibus, varius metus ac, luctus quam. Donec vitae justo vitae nisi placerat ultrices nec sed ante.",
                ImageUrl = host + "/Images/company-events1.jpg",
                Location = "Chicago, IL",
                Status = EventStatus.Active,
                RegisteredEventId = "EVT004",
                EventDate = DateTime.Today.AddDays(20)
            };

            var event5 = new Event() {
                Title = "Corporate Event 5",
                Category = "General",
                ContactEmail = "eventadmin@domain.com",
                Description = "Vestibulum ex mauris, feugiat in vehicula id, congue eleifend elit. Morbi orci quam, mattis sit amet nisl sed, dictum fermentum velit. Quisque rhoncus, arcu vitae dignissim tempus, nisl felis volutpat ipsum, non lobortis tellus lectus at mauris. Fusce porta, lectus feugiat egestas fringilla, dui velit tincidunt est, nec congue ligula urna a felis. Nam vitae ullamcorper lectus. Sed vitae justo felis.",
                ImageUrl = host + "/Images/company-events2.jpg",
                Location = "Helsinki, Finland",
                Status = EventStatus.Active,
                RegisteredEventId = "EVT005",
                EventDate = DateTime.Today.AddDays(45)
            };

            var event6 = new Event() {
                Title = "Corporate Event 6",
                Category = "General",
                ContactEmail = "eventadmin@domain.com",
                Description = "Vivamus scelerisque lectus et sapien mollis, ut vestibulum nunc vulputate. Nullam sed quam felis. Praesent sit amet egestas nunc, nec aliquam eros. Maecenas et nisl dapibus, varius metus ac, luctus quam. Donec vitae justo vitae nisi placerat ultrices nec sed ante.",
                ImageUrl = host + "/Images/company-events3.jpg",
                Location = "Chicago, IL",
                Status = EventStatus.Active,
                RegisteredEventId = "EVT006",
                EventDate = DateTime.Today.AddDays(60)
            };

            event1.Save(web);
            event2.Save(web);
            event3.Save(web);
            event4.Save(web);
            event5.Save(web);
            event6.Save(web);

            // create default wiki page
            web.AddWikiPage("Site Pages", "EventsHome.aspx");

            # region web parts
            var webPart1 = new WebPartEntity(){
                WebPartXml = @"<webParts>
  <webPart xmlns='http://schemas.microsoft.com/WebPart/v3'>
    <metaData>
      <type name='Microsoft.SharePoint.WebPartPages.ClientWebPart, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c' />
      <importErrorMessage>Cannot import this Web Part.</importErrorMessage>
    </metaData>
    <data>
      <properties>
        <property name='Description' type='string'>Displays featured events</property>
        <property name='FeatureId' type='System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'>3a6d7f41-2de8-4e69-b4b4-0325bd56b32c</property>
        <property name='Title' type='string'>Featured Events</property>
        <property name='ProductWebId' type='System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'>12ae648f-27db-4a97-9c63-37155d3ace1e</property>
        <property name='WebPartName' type='string'>FeaturedEvents</property>
        <property name='ProductId' type='System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'>3a6d7f41-2de8-4e69-b4b4-0325bd56b32b</property>
        <property name='ChromeState' type='chromestate'>Normal</property>
      </properties>
    </data>
  </webPart>
</webParts>",
                WebPartIndex = 0,
                WebPartTitle = "Featured Events",
                WebPartZone = "Rich Content"
            };

            var webPart2 = new WebPartEntity() {
                WebPartXml = @"<webParts>
  <webPart xmlns='http://schemas.microsoft.com/WebPart/v3'>
    <metaData>
      <type name='Microsoft.SharePoint.WebPartPages.ClientWebPart, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c' />
      <importErrorMessage>Cannot import this Web Part.</importErrorMessage>
    </metaData>
    <data>
      <properties>
        <property name='Description' type='string'>Events displayed by specific category</property>
        <property name='FeatureId' type='System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'>3a6d7f41-2de8-4e69-b4b4-0325bd56b32c</property>
        <property name='Title' type='string'>Events</property>
        <property name='ProductWebId' type='System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'>12ae648f-27db-4a97-9c63-37155d3ace1e</property>
        <property name='WebPartName' type='string'>Events</property>
        <property name='ProductId' type='System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'>3a6d7f41-2de8-4e69-b4b4-0325bd56b32b</property>
        <property name='ChromeState' type='chromestate'>Normal</property>
      </properties>
    </data>
  </webPart>
</webParts>",
                WebPartIndex = 1,
                WebPartTitle = "Events",
                WebPartZone = "Rich Content"
            };
            #endregion

            var welcomePage = "SitePages/EventsHome.aspx";
            var serverRelativeUrl = UrlUtility.Combine(web.ServerRelativeUrl, welcomePage);

            File webPartPage = web.GetFileByServerRelativeUrl(serverRelativeUrl);

            if (webPartPage == null) {
                return;
            }

            web.Context.Load(webPartPage);
            web.Context.Load(webPartPage.ListItemAllFields);
            web.Context.Load(web.RootFolder);
            web.Context.ExecuteQuery();

            web.RootFolder.WelcomePage = welcomePage;
            web.RootFolder.Update();
            web.Context.ExecuteQuery();

            var limitedWebPartManager = webPartPage.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);
            web.Context.Load(limitedWebPartManager.WebParts);
            web.Context.ExecuteQuery();

            for (var i = 0; i < limitedWebPartManager.WebParts.Count; i++) {
                limitedWebPartManager.WebParts[i].DeleteWebPart();
            }
            web.Context.ExecuteQuery();

            var oWebPartDefinition1 = limitedWebPartManager.ImportWebPart(webPart1.WebPartXml);
            var oWebPartDefinition2 = limitedWebPartManager.ImportWebPart(webPart2.WebPartXml);
            var wpdNew1 = limitedWebPartManager.AddWebPart(oWebPartDefinition1.WebPart, webPart1.WebPartZone, webPart1.WebPartIndex);
            var wpdNew2 = limitedWebPartManager.AddWebPart(oWebPartDefinition2.WebPart, webPart2.WebPartZone, webPart2.WebPartIndex);
            web.Context.Load(wpdNew1);
            web.Context.Load(wpdNew2);
            web.Context.ExecuteQuery();
        }
    }
}