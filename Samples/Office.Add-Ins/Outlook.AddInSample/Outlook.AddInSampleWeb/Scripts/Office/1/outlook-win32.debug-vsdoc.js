/* Version: 16.0.3825.1000 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/


Office._appMode = {
    Read: 1,
    Compose: 2,
    ReadCompose: 3
}

Office._cast_item = function () {
    this.toAppointmentCompose = function (item) {
        return new Office._$MailboxAppointment(Office._appMode.Compose);
    }
    this.toAppointmentRead = function (item) {
        return new Office._$MailboxAppointment(Office._appMode.Read);
    }
    this.toAppointment = function (item) {
        return new Office._$MailboxAppointment(Office._appMode.ReadCompose);
    }
    this.toMessageCompose = function (item) {
        return new Office._$MailboxMessage(Office._appMode.Compose);
    }
    this.toMessageRead = function (item) {
        return new Office._$MailboxMessage(Office._appMode.Read);
    }
    this.toMessage = function (item) {
        return new Office._$MailboxMessage(Office._appMode.ReadCompose);
    }
    this.toItemCompose = function (item) {
        return new Office._$MailboxItem(Office._appMode.Compose);
    }
    this.toItemRead = function (item) {
        return new Office._$MailboxItem(Office._appMode.Read);
    }
};

Office._context_mailbox_item = function () {
    Office._$MailboxItem_helper(this, Office._appMode.ReadCompose);
    Office._$MailboxAppointment_helper(this, Office._appMode.ReadCompose);
    Office._$MailboxMessage_helper(this, Office._appMode.ReadCompose);
};

Office._$MailboxItem = function (appMode) {
    Office._$MailboxItem_helper(this, appMode);
    Office._$MailboxAppointment_helper(this, appMode);
    Office._$MailboxMessage_helper(this, appMode);
}

Office._$MailboxAppointment = function (appMode) {
    Office._$MailboxItem_helper(this, appMode);
    Office._$MailboxAppointment_helper(this, appMode);
}

Office._$MailboxMessage = function (appMode) {
    Office._$MailboxItem_helper(this, appMode);
    Office._$MailboxMessage_helper(this, appMode);
}

Office._$MailboxItem_helper = function (obj, appMode) {
    // Field documentation ------------------------------------------

    // Attachments property.
    attachmentsDoc = {
        attachments_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "attachments",
            annotate: {
                ///<field name="attachments" type='AttachmentDetails[]'>Gets a list of attachments to the item.</field>
                attachments: undefined
            },
            contents: function () {
                return new Array(new Office._context_mailbox_item_attachmentDetails())
            }
        },
        attachments_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "attachments",
            annotate: {
                ///<field name="attachments">Gets a list of attachments to the item. In compose mode the attachments property is undefined. In read mode it returns a list of attachments to the item.</field>
                attachments: undefined
            },
            contents: function () {
                return new Array(new Office._context_mailbox_item_attachmentDetails())
            }
        }
    }

    bodyDoc = {
        body_compose: {
            conditions: {
                hosts: ["not outlook, outlookcompose"]
            },
            name: "body",
            annotate: {
                /// <field name="body" type='Body'>Provides methods to get and set the body of the item.</field>
                body: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_body()
            }
        },
        body_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "body",
            annotate: {
                /// <field name="body"> Gets the content of an item. In read mode, the body property is undefined. In compose mode it returns a Body object.</field>
                body: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_body()
            }
        }
    }

    // dateTimeCreated property.
    dateTimeCreatedDoc = {
        dateTimeCreated_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "dateTimeCreated",
            annotate: {
                ///<field name="dateTimeCreated" type='Date'>Gets the date and time that the item was created.</field>
                dateTimeCreated: undefined
            },
            contents: function () {
                return new Date()
            }
        },
        dateTimeCreated_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "dateTimeCreated",
            annotate: {
                ///<field name="dateTimeCreated" type='Date'>Gets the date and time that the item was created. In compose mode the dateTimeCreated property is undefined.</field>
                dateTimeCreated: undefined
            },
            contents: function () {
                return new Date()
            }
        }
    }

    // dateTimeModified property.
    dateTimeModifiedDoc = {
        dateTimeModified_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "dateTimeModified",
            annotate: {
                ///<field name="dateTimeModified" type='Date'>Gets the date and time that the item was last modified.</field>
                dateTimeModified: undefined
            },
            contents: function () {
                return new Date()
            }
        },
        dateTimeModified_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "dateTimeModified",
            annotate: {
                ///<field name="dateTimeModified" type='Date'>Gets the date and time that the item was last modified. In compose mode the dateTimeModified property is undefined.</field></field>
                dateTimeModified: undefined
            },
            contents: function () {
                return new Date()
            }
        }
    }

    // itemClass property.
    itemClassDoc = {
        itemClass_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "itemClass",
            annotate: {
                ///<field name="itemClass" type='String'>Gets the item class of the item.</field>
                itemClass: undefined
            }
        },
        itemClass_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "itemClass",
            annotate: {
                ///<field name="itemClass" type='String'>Gets the item class of the item. In compose mode the itemClass property is undefined.</field>
                itemClass: undefined
            }
        }
    }

    // itemId property
    // This property is in all modes, so it gets processed in place rather than
    // after the extra documentation is removed.
    Office._processContents(obj, {
        itemIdDoc: {
            conditions: {
                hosts: ["outlook", "outlookcompose"]
            },
            name: "itemId",
            annotate: {
                ///<field name="itemId" type='String'>Gets the Exchange Web Services (EWS) item identifier of an item.</field>
                itemId: undefined
            }
        }
    })

    // itemType property.
    Office._processContents(obj, {
        itemTypeDoc: {
            conditions: {
                hosts: ["outlook", "outlookcompose"]
            },
            name: "itemType",
            annotate: {
                ///<field name="itemType" type='Office.MailboxEnums.ItemType'>Gets the type of an item that an instance represents.</field>
                itemType: undefined
            }
        }
    })

    //obj.normalizedSubject = {};
    normalizedSubjectDoc = {
        normalizedSubject_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "normalizedSubject",
            annotate: {
                ///<field name="normalizedSubject" type='String'>Gets the subject of the item, with standard prefixes removed.</field>
                normalizedSubject: undefined
            }
        },
        normalizedSubject_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "normalizedSubject",
            annotate: {
                ///<field name="normalizedSubject" type='String'>Gets the subject of the item, with standard prefixes removed. In compose mode, the normalizedSubject property is undefined.</field>
                normalizedSubject: undefined
            }
        }
    }

    subjectDoc = {
        subject_compose: {
            conditions: {
                hosts: ["not outlook, outlookcompose"]
            },
            name: "subject",
            annotate: {
                /// <field name="subject" type='Subject'>Provides methods to get and set the item subject.</field>
                subject: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_subject()
            }
        },
        subject_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "subject",
            annotate: {
                /// <field name="subject" type='String'>Gets the subject of the item.</field>
                subject: undefined
            }
        },
        subject_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "subject",
            annotate: {
                /// <field name="subject"> Gets the subject of an item. In compose mode the Subject property returns a Subject object. In read mode, it returns a string.</field>
                subject: undefined
            }
        }
    }

    if (appMode == Office._appMode.Compose) {
        delete attachmentsDoc["attachments_read"];
        delete attachmentsDoc["attachments_read_compose"];
        delete bodyDoc.body_compose["conditions"];
        delete bodyDoc["body_read_compose"];
        delete dateTimeCreatedDoc["dateTimeCreated_read"];
        delete dateTimeCreatedDoc["dateTimeCreated_read_compose"];
        delete dateTimeModifiedDoc["dateTimeModified_read"];
        delete dateTimeModifiedDoc["dateTimeModified_read_compose"];
        delete itemClassDoc["itemClass_read"];
        delete itemClassDoc["itemClass_read_compose"];
        delete normalizedSubjectDoc["normalizedSubject_read"];
        delete normalizedSubjectDoc["normalizedSubject_read_compose"];
        delete subjectDoc.subject_compose["conditions"];
        delete subjectDoc["subject_read"];
        delete subjectDoc["subject_read_compose"];
    }
    else if (appMode == Office._appMode.Read) {
        delete attachmentsDoc.attachments_read["conditions"];
        delete attachmentsDoc["attachments_read_compose"];
        delete bodyDoc["body_compose"];
        delete bodyDoc["body_read_compose"];
        delete dateTimeCreatedDoc.dateTimeCreated_read["conditions"];
        delete dateTimeCreatedDoc["dateTimeCreated_read_compose"];
        delete dateTimeModifiedDoc.dateTimeModified_read["conditions"];
        delete dateTimeModifiedDoc["dateTimeModified_read_compose"];
        delete itemClassDoc.itemClass_read["conditions"];
        delete itemClassDoc["itemClass_read_compose"];
        delete normalizedSubjectDoc.normalizedSubject_read["conditions"];
        delete normalizedSubjectDoc["normalizedSubject_read_compose"];
        delete subjectDoc.subject_read["conditions"];
        delete subjectDoc["subject_compose"];
        delete subjectDoc["subject_read_compose"];
    }
    else if (appMode == Office._appMode.ReadCompose) {
        delete attachmentsDoc["attachments_read"];
        delete attachmentsDoc.attachments_read_compose["conditions"];
        delete bodyDoc["body_compose"];
        delete bodyDoc.body_read_compose["conditions"];
        delete dateTimeCreatedDoc["dateTimeCreated_read"];
        delete dateTimeCreatedDoc.dateTimeCreated_read_compose["conditions"];
        delete dateTimeModifiedDoc["dateTimeModified_read"];
        delete dateTimeModifiedDoc.dateTimeModified_read_compose["conditions"];
        delete itemClassDoc["itemClass_read"];
        delete itemClassDoc.itemClass_read_compose["conditions"];
        delete normalizedSubjectDoc["normalizedSubject_read"];
        delete normalizedSubjectDoc.normalizedSubject_read_compose["conditions"];
        delete subjectDoc["subject_compose"];
        delete subjectDoc["subject_read"];
        delete subjectDoc.subject_read_compose["conditions"];
    }

    Office._processContents(obj, attachmentsDoc);
    Office._processContents(obj, bodyDoc);
    Office._processContents(obj, dateTimeCreatedDoc);
    Office._processContents(obj, dateTimeModifiedDoc);
    Office._processContents(obj, itemClassDoc);
    Office._processContents(obj, normalizedSubjectDoc);
    Office._processContents(obj, subjectDoc);

    if (appMode == Office._appMode.Compose || appMode == Office._appMode.ReadCompose) {
        obj.addFileAttachmentAsync = function (uri, attachmentName, options, callback) {
            ///<summary>Attach a file to an item.</summary>
            ///<param name="uri" type="String">A URI that provides the location of the file. Required.</param>
            ///<param name="attachmentName" type="String">The name to display while the attachment is loading. The name is limited to 256 characters. Required.</param>
            ///<param name="options" type="Object" optional="true">An optional parameters or state data passed to the callback method. Optional.</param>
            ///<param name="callback" type="function" optional="true">The method to invoke when the attachment finishes uploading. Optional.</param>

            var result = new Office._Mailbox_AsyncResult("attachmentAsync");
            if (arguments.length == 3) { callback = options; }
            callback(result);
        };

        obj.addItemAttachmentAsync = function (itemId, attachmentName, options, callback) {
            ///<summary>Attach an email item to an item.</summary>
            ///<param name="itemId" type="string">The Exchange identifier of the item to attach. The maximum length is 100 characters.</param>
            ///<param name="attachmentName" type="string">The name to display while the attachment is loading. The name is limited to 256 characters. </param>
            ///<param name="options" type="Object" optional="true">An optional parameters or state data passed to the callback method. </param>
            ///<param name="callback" type="function" optional="true">The method to invoke when the attachment finishes uploading. </param>

            var result = new Office._Mailbox_AsyncResult("attachmentAsync");
            if (arguments.length == 3) { callback = options; }
            callback(result);
        };

        obj.removeAttachmentAsync = function (attachmentIndex, options, callback) {
            ///<summary>Removes a file or item that was previously attached to an item.</summary>
            ///<param name="attachmentIndex" type="String">The index of the attachment to remove.</param>
            ///<param name="options" type="Object" optional="true">An optional parameters or state data passed to the callback method. </param>
            ///<param name="callback" type="function" optional="true">The method to invoke when the attachment is removed. </param>

            var result = new Office._Mailbox_AsyncResult("removeAttachmentAsync");
            if (arguments.length == 2) { callback = options; }
            callback(result);
        };

        Office._processItem(obj, {
            conditions: {
                reqs: ["set Mailbox GE 1.2"]
            },
            metaOnly: true,
            contents: {
                getSelectedDataAsync: {
                    value: function (coercionType, options, callback) {
                        ///<summary>Gets the selected data in the subject or body of the current item; or null if not data is selected.</summary>
                        ///<param name="coercionType" type="Office.CoercionType">One of the CoercionType enumeration values indicating whether the selected content is HTML or plain text.</param>
                        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method.</param>
                        ///<param name="callback" type="function">The method to call when the asynchronous method is complete.</param>
                        var result = new Office._Mailbox_AsyncResult("getSelectedDataAsync");
                        if (arguments.length == 2) { callback = options; }
                        callback(result);
                    }
                },
                setSelectedDataAsync: {
                    value: function (data, options, callback) {
                        ///<summary>Sets the specified data in the subject or body of the current item.</summary>
                        ///<param name="data" type='String'>The text to insert into the subject or body of the item.</param>
                        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method.</param>
                        ///<param name="callback" type="function">The method to call when the asynchronous method is complete.</param>
                        var result = new Office._Mailbox_AsyncResult();
                        if (arguments.length == 2) { callback = options; }
                        callback(result);
                    }
                }
            }
        });
    }

    if (appMode == Office._appMode.Read || appMode == Office._appMode.ReadCompose) {
        obj.displayReplyAllForm = function (htmlBody) {
            ///<summary>Display a form for creating an email reply to all recipients.</summary>
            ///<param name="htmlBody" type="String">The HTML contents of the email reply. 32 KB limit.</param>
        };
        obj.displayReplyForm = function (htmlBody) {
            ///<summary>Display a form for creating an email reply to the sender.</summary>
            ///<param name="htmlBody" type="String">The HTML contents of the email reply. 32 KB limit</param>
        };
        Office._processItem(obj, {
            conditions: {
                reqs: ["set Mailbox GE 1.2"]
            },
            metaOnly: true,
            contents: {
                displayReplyAllForm: {
                    value: function (replyForm) {
                        ///<summary>Display a form for creating an email reply to all recipients.</summary>
                        ///<param name="replyForm" type='Object'>Syntax example: {
                        ///&#10;    &#34;body&#34;: &#34;...&#34; , 
                        ///&#10;    &#34;attachments&#34;: [
                        ///&#10;        {
                        ///&#10;            &#34;type&#34;:&#34;item&#34;,
                        ///&#10;            &#34;id&#34;:&#34;...&#34;,
                        ///&#10;        },
                        ///&#10;        {
                        ///&#10;            &#34;type&#34;:&#34;file&#34;,
                        ///&#10;            &#34;name&#34;:&#34;...&#34;,
                        ///&#10;            &#34;url&#34;:&#34;...&#34;,
                        ///&#10;        }
                        ///&#10; ]}
                        ///&#10;     body: HTML contents of the email reply. Optional, 32 KB limit.
                        ///&#10;     attachments: An array of JSON objects that are either file or item attachments. Optional.
                        ///&#10;          attachments:type: &#34;item&#34; to indicate that the attachment is an Exchange item, or &#34;file&#34; to indicate that the attachment is a file.
                        ///&#10;          attachments:id: The EWS identifier of the item to attach. 100 character limit.
                        ///&#10;          attachments:file: The file name of the file to attach. 255 character limit.
                        ///&#10;          attachments:url: The URL of the location where the file bytes are located. 2048 character limit.
                        ///&#10; -or-
                        ///&#10; The HTML contents of the email reply. 32 KB limit.</param>
                    }
                },
                displayReplyForm: {
                    value: function (replyForm) {
                        ///<summary>Display a form for creating an email reply to the sender.</summary>
                        ///<param name="replyForm" type='Object'>Syntax example: {
                        ///&#10;    &#34;body&#34;: &#34;...&#34; , 
                        ///&#10;    &#34;attachments&#34;: [
                        ///&#10;        {
                        ///&#10;            &#34;type&#34;:&#34;item&#34;,
                        ///&#10;            &#34;id&#34;:&#34;...&#34;,
                        ///&#10;        },
                        ///&#10;        {
                        ///&#10;            &#34;type&#34;:&#34;file&#34;,
                        ///&#10;            &#34;name&#34;:&#34;...&#34;,
                        ///&#10;            &#34;url&#34;:&#34;...&#34;,
                        ///&#10;        }
                        ///&#10; ]}
                        ///&#10;     body: HTML contents of the email reply. Optional, 32 KB limit.
                        ///&#10;     attachments: An array of JSON objects that are either file or item attachments. Optional.
                        ///&#10;          attachments:type: &#34;item&#34; to indicate that the attachment is an Exchange item, or &#34;file&#34; to indicate that the attachment is a file.
                        ///&#10;          attachments:id: The EWS identifier of the item to attach. 100 character limit.
                        ///&#10;          attachments:file: The file name of the file to attach. 255 character limit.
                        ///&#10;          attachments:url: The URL of the location where the file bytes are located. 2048 character limit.
                        ///&#10; -or-
                        ///&#10; The HTML contents of the email reply. 32 KB limit.</param>
                    }
                }
            }
        });
        obj.getEntities = function () {
            ///<summary>Gets an array of entities found in an item.</summary>
            return (new Office._context_mailbox_item_entities());
        };
        obj.getEntitiesByType = function (entityType) {
            ///<summary>Gets an array of entities of the specified entity type found in an item.</summary>
            ///<param name="entityType" type="Office.MailboxEnums.EntityType">One of the EntityType enumeration values.</param>
            if (entityType == Office.MailboxEnums.EntityType.Address) {
                return (new Array(""));
            }

            if (entityType == Office.MailboxEnums.EntityType.Contact) {
                return (new Array(new Office._context_mailbox_item_contact()));
            }

            if (entityType == Office.MailboxEnums.EntityType.EmailAddress) {
                return (new Array(""));
            }

            if (entityType == Office.MailboxEnums.EntityType.MeetingSuggestion) {
                return (new Array(new Office._context_mailbox_item_meetingSuggestion()));
            }

            if (entityType == Office.MailboxEnums.EntityType.PhoneNumber) {
                return (new Array(new Office._context_mailbox_item_phoneNumber()));
            }

            if (entityType == Office.MailboxEnums.EntityType.TaskSuggestion) {
                return (new Array(new Office._context_mailbox_item_taskSuggestion()));
            }

            if (entityType == Office.MailboxEnums.EntityType.Url) {
                return (new Array(""));
            }
        };
        obj.getFilteredEntitiesByName = function (name) {
            ///<summary>Returns well-known enitities that pass the named filter defined in the manifest XML file.</summary>
            ///<param name="name" type="String">The name of the filter defined in the manifest XML file.</param>
            return (new Array(Office._context_mailbox_item_entities()));
        };
        obj.getRegExMatches = function () {
            ///<summary>Returns string values that match the regular expressions defined in the manifest XML file.</summary>
            return (new Array(""));
        };
        obj.getRegExMatchesByName = function (name) {
            ///<summary>Returns string values that match the named regular expression defined in the manifest XML file.</summary>
            ///<param name="name" type="String">The name of the regular expression defined in the manifest XML file.</param>
            return (new Array(""));
        };
    }

    obj.loadCustomPropertiesAsync = function (callback, userContext) {
        ///<summary>Asynchronously loads custom properties that are specific to the item and a mail app for Outlook.</summary>
        ///<param name="callback" type="Function">The method to call when the asynchronous load is complete.</param>
        ///<param name="userContext" type="Object" optional="true">Any state data that is passed to the asynchronous method.</param>

        var result = new Office._Mailbox_AsyncResult("loadCustomPropertiesAsync");
        callback(result);
    };
};

Office._$MailboxAppointment_helper = function (obj, appMode) {
    // End property.
    endDoc = {
        end_compose: {
            conditions: {
                hosts: ["not outlook; outlookcompose"]
            },
            name: "end",
            annotate: {
                /// <field name="end" type='Time'>Provides methods to get and set the end time of an appointment.</field>
                end: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_time();
            }
        },
        end_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "end",
            annotate: {
                ///<field name="end" type='Date'>Gets the date and time that the appointment is to end.</field>
                end: undefined
            },
            contents: function () {
                return new Date();
            }
        },
        end_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "end",
            annotate: {
                /// <field name="end">Gets the date and time that the appointment is to end. In read mode, returns Date object. In compose mode, returns a Time object.</field>
                end: undefined
            }
        }
    }

    // Location property.
    locationDoc = {
        location_compose: {
            conditions: {
                hosts: ["not outlook, outlookcompose"]
            },
            name: "location",
            annotate: {
                /// <field name="location" type='Location'>Provides methods to get and set the location of an appointment.</field>
                location: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_location()
            }
        },
        location_read: {
            conditions: {
                hosts: ["not outlook, outlookcompose"]
            },
            name: "location",
            annotate: {
                ///<field name="location" type='String'>Gets the location of an appointment.</field>
                location: undefined
            }
        },
        location_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "location",
            annotate: {
                /// <field name="location"> Gets the location of an appointment. In read mode, returns a string. In compose mode returns a Location object.</field>
                location: undefined
            }
        }
    }

    // Optional attendees property.
    optionalAttendeesDoc = {
        optionalAttendees_compose: {
            conditions: {
                hosts: ["not outlook; outlookcompose"]
            },
            name: "optionalAttendees",
            annotate: {
                /// <field name="optionalAttendees" type='Recipients'>Provides methods to get and set the optional attendees of an appointment.</field>
                optionalAttendees: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_recipients();
            }
        },
        optionalAttendees_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "optionalAttendees",
            annotate: {
                ///<field name="optionalAttendees" type='EmailAddressDetails[]'>Gets a list of email addresses for optional attendees of an appointment.</field>
                optionalAttendees: undefined
            },
            contents: function () {
                return new Array(new Office._context_mailbox_item_emailAddressDetails())
            }
        },
        optionalAttendees_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "optionalAttendees",
            annotate: {
                /// <field name="optionalAttendees">Gets a list of email addresses for optional attendees of an appointment. In read mode, returns an array of EmailAddressDetails objects. In compose mode, returns a Recipients object.</field>
                optionalAttendees: undefined
            }
        }
    }

    // Organizer property.
    organizerDoc = {
        organizer_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "organizer",
            annotate: {
                ///<field name="organizer" type="EmailAddressDetails">Gets the email address of the meeting organizer for the specified meeting.</field>
                organizer: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_emailAddressDetails()
            }
        },
        organizer_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "organizer",
            annotate: {
                ///<field name="organizer" type="EmailAddressDetails">Gets the email address of the meeting organizer for the specified meeting. In compose mode, the organizer property is undefined.</field>
                organizer: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_emailAddressDetails()
            }
        }
    }

    // Required attendees
    requiredAttendeesDoc = {
        requiredAttendees_compose: {
            conditions: {
                hosts: ["not outlook, outlookcompose"]
            },
            name: "requiredAttendees",
            annotate: {
                /// <field name="requiredAttendees" type='Recipients'>Provides methods to get and set the required attendees of an appointment.</field>
                requiredAttendees: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_recipients();
            }
        },
        requiredAttendees_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "requiredAttendees",
            annotate: {
                /// <field name="requiredAttendees" type='EmailAddressDetails[]'>Gets a list of email addresses for required attendees of an appointment.</field>
                requiredAttendees: undefined
            },
            contents: function () {
                return new Array(new Office._context_mailbox_item_emailAddressDetails())
            }
        },
        requiredAttendees_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "requiredAttendees",
            annotate: {
                /// <field name="optionalAttendees">Gets a list of email addresses for required attendees of an appointment. In read mode, returns an array of EmailAddressDetails objects. In compose mode, returns a Recipients object.</field>
                requiredAttendees: undefined
            }
        }
    }

    // Resources property.
    resourcesDoc = {
        resources_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "resources",
            annotate: {
                /// <field name="resources" type='EmailAddressDetails[]'>Gets the resources required for an appointment.</field>
                resources: undefined
            },
            contents: function () {
                return new Array(new Office._context_mailbox_item_emailAddressDetails())
            }
        },
        resources_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "resources",
            annotate: {
                /// <field name="resources">Gets the resources required for an appointment. In read mode, returns an array of EmailAddressDetails objects. In compose mode, the resources property is undefined.</field>
                resources: undefined
            }
        }
    }

    // Start property
    startDoc = {
        start_compose: {
            conditions: {
                hosts: ["not outlook; outlookcompose"]
            },
            name: "start",
            annotate: {
                /// <field name="start" type='Time'>Provides methods to get and set the start time for the appointment.</field>
                start: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_time();
            }
        },
        start_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "start",
            annotate: {
                ///<field name="start" type='Date'>Gets the date and time that the appointment is to begin.</field>
                start: undefined
            },
            contents: function () {
                return new Date();
            }
        },
        start_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "start",
            annotate: {
                /// <field name="start">Gets the date and time that the appointment is to begin. In read mode, returns Date object. In compose mode, returns a Time object.</field>
                start: undefined
            }
        }
    }

    if (appMode == Office._appMode.Compose) {
        delete endDoc.end_compose["conditions"];
        delete endDoc["end_read"];
        delete endDoc["end_read_compose"];
        delete locationDoc.location_compose["conditions"];
        delete locationDoc["location_read"];
        delete locationDoc["location_read_compose"];
        delete optionalAttendeesDoc.optionalAttendees_compose["conditions"];
        delete optionalAttendeesDoc["optionalAttendees_read"];
        delete optionalAttendeesDoc["optionalAttendees_read_compose"];
        delete organizerDoc["organizer_read"];
        delete organizerDoc["organizer_read_compose"];
        delete requiredAttendeesDoc.requiredAttendees_compose["conditions"];
        delete requiredAttendeesDoc["requiredAttendees_read"];
        delete requiredAttendeesDoc["requiredAttendees_read_compose"];
        delete resourcesDoc["resources_read"];
        delete resourcesDoc["resources_read_compose"];
        delete startDoc.start_compose["conditions"];
        delete startDoc["start_read"];
        delete startDoc["start_read_compose"];
    }
    else if (appMode == Office._appMode.Read) {
        delete endDoc["end_compose"];
        delete endDoc.end_read["conditions"];
        delete endDoc["end_read_compose"];
        delete locationDoc["location_compose"];
        delete locationDoc.location_read["conditions"];
        delete locationDoc["location_read_compose"];
        delete optionalAttendeesDoc["optionalAttendees_compose"];
        delete optionalAttendeesDoc.optionalAttendees_read["conditions"];
        delete optionalAttendeesDoc["optionalAttendees_read_compose"];
        delete organizerDoc.organizer_read["conditions"];
        delete organizerDoc["organizer_read_compose"];
        delete requiredAttendeesDoc["requiredAttendees_compose"];
        delete requiredAttendeesDoc.requiredAttendees_read["conditions"];
        delete requiredAttendeesDoc["requiredAttendees_read_compose"];
        delete resourcesDoc.resources_read["conditions"];
        delete resourcesDoc["resources_read_compose"];
        delete startDoc["start_compose"];
        delete startDoc.start_read["conditions"];
        delete startDoc["start_read_compose"];
    }
    else if (appMode == Office._appMode.ReadCompose) {
        delete endDoc["end_compose"];
        delete endDoc["end_read"];
        delete endDoc.end_read_compose["condtions"];
        delete locationDoc["location_compose"];
        delete locationDoc["location_read"];
        delete optionalAttendeesDoc["optionalAttendees_compose"];
        delete optionalAttendeesDoc["optionalAttendees_read"];
        delete optionalAttendeesDoc.optionalAttendees_read_compose["conditions"];
        delete locationDoc.location_read_compose["conditions"];
        delete organizerDoc["organizer_read"];
        delete organizerDoc.organizer_read_compose["conditions"];
        delete requiredAttendeesDoc["requiredAttendees_compose"];
        delete requiredAttendeesDoc["requiredAttendees_read"];
        delete requiredAttendeesDoc.requiredAttendees_read_compose["conditions"];
        delete resourcesDoc["resources_read"];
        delete resourcesDoc.resources_read_compose["conditions"];
        delete startDoc["start_compose"];
        delete startDoc["start_read"];
        delete startDoc.start_read_compose["conditions"];
    }

    Office._processContents(obj, endDoc);
    Office._processContents(obj, locationDoc);
    Office._processContents(obj, optionalAttendeesDoc);
    Office._processContents(obj, organizerDoc);
    Office._processContents(obj, requiredAttendeesDoc);
    Office._processContents(obj, resourcesDoc);
    Office._processContents(obj, startDoc);
};

Office._$MailboxMessage_helper = function (obj, appMode) {
    // BCC property.
    bccDoc = {
        bcc_compose: {
            conditions: {
                hosts: ["not outlook; outlookcompose"]
            },
            name: "bcc",
            annotate: {
                /// <field name="bcc" type='Recipients'>Provides methods to get and set the Bcc recipients of a message.</field>
                bcc: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_recipients();
            }
        },
        bcc_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "bcc",
            annotate: {
                /// <field name="bcc">Gets the Bcc recipients of a message. In read mode, the bcc property is undefined. In compose mode it returns a Recipients object.</field>
                bcc: undefined
            }
        }
    }

    // CC property.
    ccDoc = {
        cc_compose: {
            conditions: {
                hosts: ["not outlook; outlookcompose"]
            },
            name: "cc",
            annotate: {
                /// <field name="cc" type='Recipients'>Provides methods to get and set the carbon-copy (Cc) recipients of a message.</field>
                cc: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_recipients();
            }
        },
        cc_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "cc",
            annotate: {
                /// <field name="cc" type='String'>Gets the carbon-copy (Cc) recipients of a message.</field>
                cc: undefined
            },
            contents: function () {
                return new Array(new Office._context_mailbox_item_emailAddressDetails())
            }
        },
        cc_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "cc",
            annotate: {
                /// <field name="cc"> Gets the carbon-copy (Cc) recipients of a message. In read mode, the cc property returns an array of EmailAddressDetails objects. In compose mode it returns a Recipients object.</field>
                cc: undefined
            }
        }
    }

    // conversationId property
    Office._processContents(obj, {
        conversationIdDoc: {
            conditions: {
                hosts: ["outlook", "outlookcompose"]
            },
            name: "conversationId",
            annotate: {
                ///<field name="conversationId" type='String'>Gets an identifier for the email conversation that contains a particular message.</field>
                conversationId: undefined
            }
        }
    });

    // From property.
    fromDoc = {
        from_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "from",
            annotate: {
                /// <field name="from" type='EmailAddressDetails'>Gets the email address of the message's sender.</field>
                from: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_emailAddressDetails();
            }
        },
        from_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "from",
            annotate: {
                /// <field name="from" type='EmailAddressDetails'>Gets the email address of the message's sender. In compose mode, the from property is undefined.</field>
                from: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_emailAddressDetails();
            }
        }
    }

    // InternetMessageId property.
    internetMessageIdDoc = {
        internetMessageId_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "internetMessageId",
            annotate: {
                /// <field name="internetMessageId" type='String'>Gets the internet message identifier of the message.</field>
                internetMessageId: undefined
            }
        },
        internetMessageId_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "internetMessageId",
            annotate: {
                /// <field name="internetMessageId" type='String'>Gets the internet message identifier of the message. In compose mode, the internetMessageId property is undefined.</field>
                internetMessageId: undefined
            }
        }
    }

    // Sender property.
    senderDoc = {
        sender_read: {
            conditions: {
                hosts: ["outlook, not outlookcompose"]
            },
            name: "sender",
            annotate: {
                /// <field name="sender" type='EmailAddressDetails'>Gets the email address of the message sender.</field>
                sender: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_emailAddressDetails();
            }
        },
        sender_read_compose: {
            conditions: {
                hosts: ["outlook, outlookcompose"]
            },
            name: "sender",
            annotate: {
                /// <field name="sender" type='EmailAddressDetails'>Gets the email address of the message sender. In compose mode, the sender property is undefined.</field>
                sender: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_emailAddressDetails();
            }
        }
    }

    // To property
    toDoc = {
        to_compose: {
            conditions: {
                hosts: ["not outlook; outlookcompose"]
            },
            name: "to",
            annotate: {
                /// <field name="to" type='Recipients'>Provides methods to get and set the list of recipients of a message.</field>
                to: undefined
            },
            contents: function () {
                return new Office._context_mailbox_item_recipients();
            }
        },
        to_read: {
            conditions: {
                hosts: ["outlook; not outlookcompose"]
            },
            name: "to",
            annotate: {
                /// <field name="to" type='EmailAddressDetails[]'>Gets the list of recipients of a message.</field>
                to: undefined
            },
            contents: function () {
                return new Array(new Office._context_mailbox_item_emailAddressDetails())
            }
        },
        to_read_compose: {
            conditions: {
                hosts: ["outlook; outlookcompose"]
            },
            name: "to",
            annotate: {
                /// <field name="to">Gets the list of recipients of a message. In read mode, the to property returns an array of EmailAddressDetails objects. In compose mode, it returns a Recipients object.</field>
                to: undefined
            }
        }
    }

    if (appMode == Office._appMode.Compose) {
        delete bccDoc.bcc_compose["conditions"];
        delete bccDoc["bcc_read_compose"];
        delete ccDoc.cc_compose["conditions"];
        delete ccDoc["cc_read"];
        delete ccDoc["cc_read_compose"];
        delete fromDoc["from_read"];
        delete fromDoc["from_read_compose"];
        delete internetMessageIdDoc["internetMessageId_read"];
        delete internetMessageIdDoc["internetMessageId_read_compose"];
        delete senderDoc["sender_read"];
        delete senderDoc["sender_read_compose"];
        delete toDoc.to_compose["conditions"];
        delete toDoc["to_read"];
        delete toDoc["to_read_compose"];
    }
    else if (appMode == Office._appMode.Read) {
        delete bccDoc["bcc_compose"];
        delete bccDoc["bcc_read_compose"];
        delete ccDoc["cc_compose"];
        delete ccDoc.cc_read["conditions"];
        delete ccDoc["cc_read_compose"];
        delete fromDoc.from_read["conditions"];
        delete fromDoc["from_read_compose"];
        delete internetMessageIdDoc.internetMessageId_read["conditions"];
        delete internetMessageIdDoc["internetMessageId_read_compose"];
        delete senderDoc.sender_read["conditions"];
        delete senderDoc["sender_read_compose"];
        delete toDoc["to_compose"];
        delete toDoc.to_read["conditions"];
        delete toDoc["to_read_compose"];
    }
    else if (appMode == Office._appMode.ReadCompose) {
        delete bccDoc["bcc_compose"];
        delete bccDoc.bcc_read_compose["conditions"];
        delete ccDoc["cc_compose"];
        delete ccDoc["cc_read"];
        delete ccDoc.cc_read_compose["conditions"];
        delete fromDoc["from_read"];
        delete fromDoc.from_read_compose["conditions"];
        delete internetMessageIdDoc["internetMessageId_read"];
        delete internetMessageIdDoc.internetMessageId_read_compose["conditions"];
        delete senderDoc["sender_read"];
        delete senderDoc.sender_read_compose["conditions"];
        delete toDoc["to_compose"];
        delete toDoc["to_read"];
        delete toDoc.to_read_compose["conditions"];
    }

    Office._processContents(obj, bccDoc);
    Office._processContents(obj, ccDoc);
    Office._processContents(obj, fromDoc);
    Office._processContents(obj, internetMessageIdDoc);
    Office._processContents(obj, senderDoc);
    Office._processContents(obj, toDoc);
}

Office._context_mailbox = function () {
    ///<field name="ewsUrl" type='String'>Gets the URL of the Exchange Web Services (EWS) endpoint for this email account.</field>
    ///<field name="item" type="Item">Represents the current item (message or appointment).</field>
    ///<field name="userProfile" type="UserProfile">Represents the host application's user profile information.</field>
    ///<field name="diagnostics" type="Diagnostics">Provides troubleshooting capabilities for a mail app.</field>
    var instance = new Office._context_mailbox_item();

    this.ewsUrl = {};
    this.item = intellisense.nullWithCompletionsOf(instance);
    this.userProfile = new Office._context_mailbox_userProfile();
    this.diagnostics = new Office._context_mailbox_diagnostics();

    this.displayAppointmentForm = function (itemId) {
        ///<summary>Displays an existing calendar appointment.</summary>
        ///<param name="itemId" type="String">The Exchange Web Services (EWS) identifier for an existing calendar appointment.</param>
    };
    this.displayMessageForm = function (itemId) {
        ///<summary>Displays an existing message.</summary>
        ///<param name="itemId" type="String">The Exchange Web Services (EWS) identifier for an existing message.</param>
    };
    this.displayNewAppointmentForm = function (meetingRequest) {
        ///<summary>Display a form for creating a new calendar appointment.</summary>
        ///<param name="meetingRequest" type="Object">
        ///    Syntax example: {requiredAttendees:, optionalAttendees:, start:, end:, location:, resources:, subject:, body:}
        /// &#10;      requiredAttendees: An array of strings containing the SMTP email addresses of the required attendees for the meeting. The array is limited to 100 entries.
        /// &#10;      optionalAttendees: An array of strings containing the SMTP email addresses of the optional attendees for the meeting. The array is limited to 100 entries.
        /// &#10;      start: The start date and time of the appointment.
        /// &#10;      end: The end date and time of the appointment.
        /// &#10;      location: A string containing the location of the appointment. Limited to 255 characters.
        /// &#10;      resources: An array of strings containing the resources required for the appointment. The array is limited to 100 entries.
        /// &#10;      subject: A string containing the subject of the appointment. Limited to 255 characters.
        /// &#10;      body: The body of the appointment message. Limited to 32 Kb.
        /// </param>
    };

    this.convertToLocalClientTime = function (timeValue) {
        ///<summary>Gets a dictionary containing time information in local client time.</summary>
        ///<param name="timeValue" type="Date">The date and time to convert.</param>
    }

    this.convertToUtcClientTime = function (input) {
        ///<summary>Get s Date object from a dictionary containing time information.</summary>
        ///<param name="input" type="dictionary">A dictionary containing a date. The dictionary should contain the following fields: year, month, date, hours, minutes, seconds, time zone, time zone offset.</param>
    }
    
    this.getCallbackTokenAsync = function (callback, userContext) {
        ///<summary>Gets a token that can be used to retrieve attachments for the current item. This method is only available in read mode.</summary>
        ///<param name="callback" type="function">The method to call when the asynchronous method is complete.</param>
        ///<param name="userContext" type="Object" optional="true">Any state data that is passed to the asynchronous method.</param>
        var result = new Office._Mailbox_AsyncResult("getCallbackTokenAsync");
        callback(result);
    }
    this.getUserIdentityTokenAsync = function (callback, userContext) {
        ///<summary>Gets a token identifying the user and the mail app for Outlook.</summary>
        ///<param name="callback" type="function">The method to call when the asynchronous load method is complete.</param>
        ///<param name="userContext" type="Object" optional="true">Any state data that is passed to the asynchronous method.</param>

        var result = new Office._Mailbox_AsyncResult("getUserIdentityTokenAsync");
        callback(result);
    };
    this.makeEwsRequestAsync = function (data, callback, userContext) {
        ///<summary>Gets a token identifying the user and the mail app for Outlook.</summary>
        ///<param name="data" type="String">Makes an asynchronous request to an Exchange Web Services (EWS) service on the Microsoft Exchange Server 2013 Preview that hosts the mail app for Outlook.</param>
        ///<param name="callback" type="function">The method to call when the asynchronous load method is complete.</param>
        ///<param name="userContext" type="Object" optional="true">Any state data that is passed to the asynchronous method.</param>

        var result = new Office._Mailbox_AsyncResult("makeEwsRequestAsync");
        callback(result);
    };
}

Office._context_mailbox_diagnostics = function () {
    ///<field name="hostName" type='String'>Gets a string containing the name of the host application for the mail app.</field>
    ///<field name="hostVersion" type='String'>Gets a string containing the version of either the host application or the Exchange server.</field>
    ///<field name="OWAView" type='String'>Gets a string containing the current view of the Outlook Web App.</field>
    this.hostName = {};
    this.hostVersion = {};
    this.OWAView = {};
}

Office._context_mailbox_item_attachmentDetails = function () {
    /// <field name="attachmentType" type='Office.MailboxEnums.AttachmentType'>Indicates whether the attachment is an Exchange item or file.</field>
    /// <field name="contentType" type='String'>The MIME content type of the attachment.</field>
    /// <field name="id" type='String'>The Exchange Web Services (EWS) attachment identifer for the attachment.</field>
    /// <field name="isInline" type='Boolean'>true if the attachment is inline, otherwise, false.</field>
    /// <field name="name" type='String'>The name of the attachment.</field>
    /// <field name="size' type='number'>The size of the attachment in bytes.</field>
    this.attachmentType = {};
    this.contentType = {};
    this.id = {};
    this.isInline = {};
    this.name = {};
    this.size = {};
}

Office._context_mailbox_item_body = function () {
    this.getTypeAsync = function (options, callback) {
        ///<summary>Gets a value that indicates whether the body is in HTML or text format.</summary>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method. 
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult("bodyGetTypeAsync");
        if (arguments.length == 1) { callback = options; }
        callback(result);
    };
    this.prependAsync = function (data, options, callback) {
        ///<summary>Sets the body of a message or meeting.</summary>
        ///<param name="data" type="String">The text to insert at the beginning of the item body. The string is limited to 1,000,000 characters.</param>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method. </param>
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult();
        if (arguments.length == 2) { callback = options; }
        callback(result);
    };
    this.setSelectedDataAsync = function (data, options, callback) {
        ///<summary>Replaces the selection in the body with the specified text.</summary>
        ///<param name="data" type="String">The text to insert in the item body. The string is limited to 1,000,000 characters.</param>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method. </param>
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult();
        if (arguments.length == 2) { callback = options; }
        callback(result);
    };
}
Office._context_mailbox_item_contact = function () {
    ///<field name="addresses" type='String[]'>Gets the mailing and street addresses associated with a contact.</field>
    ///<field name="businessName" type='String'>Gets the name of the business associated with a contact.</field>
    ///<field name="emailAddresses" type='String[]'>Gets the SMTP email addresses associated with a contact.</field>
    ///<field name="personName" type='String'>Gets the name of the person associated with a contact.</field>
    ///<field name="phoneNumbers" type='PhoneNumber[]'>Gets the phone numbers associated with a contact.</field>
    ///<field name="urls" type='String[]'>Get the list of Internet URLs associated with a contact.</field>
    this.addresses = new Array("");
    this.businessName = {};
    this.emailAddresses = new Array("");
    this.personName = {};
    this.phoneNumbers = new Array(new Office._context_mailbox_item_phoneNumber());
    this.urls = new Array("");
}
Office._context_mailbox_item_customProperties = function () {
    this.get = function (name) {
        ///<summary>Gets the value of the specicifed custom property.</summary>
        ///<param name="name" type="String">The name of the custom property to be returned.</param>
    }

    this.remove = function (name) {
        ///<summary>Removes the specicifed custom property.</summary>
        ///<param name="name" type="String">The name of the custom property to be removed.</param>
    }

    this.saveAsync = function (callback, userContext) {
        ///<summary>Saves item-specific custom properties to the Exchange server.</summary>
        ///<param name="callback" type="String" optional="true">The method to call when an asynchronous call is complete.</param>
        ///<param name="userContext" type="Object" optional="true">Any state data that is passed to the callback method.</param>
    }

    this.set = function (name, value) {
        ///<summary>Sets the value of the specicifed custom property.</summary>
        ///<param name="name" type="String">The name of the custom property to be set.</param>
        ///<param name="value" type="Object">The value of the custom property to be set.
    }
}
Office._context_mailbox_item_emailAddressDetails = function () {
    ///<field name="appointmentResponse" type="Office.MailboxEnums.ResponseType">One of the ResponseType enumeration values.</field>
    ///<field name="displayName" type="String">Gets the display name associated with the email address.</field>
    ///<field name="emailAddress" type="String">Gets the SMTP email address.</field>
    ///<field name="recipientType" type="Office.MailboxEnums.RecipientType">One of the RecipientType enumeration values.</field>
    this.appointmentResponse = {};
    this.displayName = {};
    this.emailAddress = {};
    this.recipientType = {};
}
Office._context_mailbox_item_emailUser = function () {
    ///<field name="name" type="String">Gets the name associated with an email account.</field>
    this.name = {};
    ///<field name="userId" type="String">Gets the SMTP email address of the email account.</field>
    this.userId = {};
}
Office._context_mailbox_item_entities = function () {
    ///<field name="addresses" type='Array'>Gets the physical addresses (street or mailing address) found in an email message or appointment.</field>
    ///<field name="contacts" type='Array'>Gets the contacts found in an email message or appointment.</field>
    ///<field name="emailAddresses" type='Array'>Gets the email addresses found in an email message or appointment.</field>
    ///<field name="meetingSuggestions" type='Array'>Gets the meeting suggestions found in an email message or appointment.</field>
    ///<field name="phoneNumbers" type='Array'>Gets the phone numbers found in an email message or appointment.</field>
    ///<field name="taskSuggestions" type='Array'>Gets the task suggestions found in an email message or appointment.</field>
    ///<field name="urls" type='Array'>Gets the Internet URLs found in an email message or appointment.</field>
    this.addresses = new Array("");
    this.contacts = new Array(new Office._context_mailbox_item_contact());
    this.emailAddresses = new Array("");
    this.meetingSuggestions = new Array(new Office._context_mailbox_item_meetingSuggestion());
    this.phoneNumbers = new Array(new Office._context_mailbox_item_phoneNumber());
    this.taskSuggestions = new Array(new Office._context_mailbox_item_taskSuggestion());
    this.urls = new Array("");
}
Office._context_mailbox_item_location = function () {
    this.getAsync = function (options, callback) {
        ///<summary>Gets the location of an appointment.</summary>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method. </param>
        ///<param name="callback" type="function">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult("locationGetAsync");
        if (arguments.length == 1) { callback = options; }
        callback(result);
    };

    this.setAsync = function (location, options, callback) {
        ///<summary>Sets the subject of an item.</summary>
        ///<param name="location" type="String">The location of the appointment. The string is limited to 255 characters.</param>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method. Optional.</param>
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult();
        if (arguments.length == 2) { callback = options; }
        callback(result);
    }
}
Office._context_mailbox_item_meetingRequest = function () {
    ///<field name="end" type='Date'>Gets the date and time that a meeting is to end.</field>
    ///<field name="location" type='String'>Gets the location of the proposed meeting.</field>
    ///<field name="optionalAttendees" type='EmailAddressDetails[]'>Gets a list of the optional attendees for the meeting.</field>
    ///<field name="requiredAttendees" type='EmailAddressDetails[]'>Gets the required attendees for the meeting.</field>
    ///<field name="resources" type='String'>Gets a list of the resources required for the meeting.</field>
    ///<field name="start" type='Date'>Gets the date and time that the meeting is to begin.</field>
    this.end = new Date;
    this.location = {};
    this.optionalAttendees = new Array(new Office._context_mailbox_item_emailAddressDetails());
    this.requiredAttendees = new Array(new Office._context_mailbox_item_emailAddressDetails());
    this.resources = new Array("");
    this.start = new Date();
}
Office._context_mailbox_item_meetingSuggestion = function () {
    ///<field name="attendees" type='EmailAddressDetails[]'>Gets the attendees for a suggested meeting.</field>
    ///<field name="end" type='Date'>Gets the date and time that a suggested meeting is to end.</field>
    ///<field name="location" type='String'>Gets the location of a suggested meeting.</field>
    ///<field name="meetingString" type='String'>Gets a string that was identified as a meeting suggestion.</field>
    ///<field name="start" type='Date'>Gets the date and time that a suggested meeting is to begin.</field>
    ///<field name="subject" type='String'>Gets the subject of a suggested meeting.</field>
    this.attendees = new Array(new Office._Context_mailbox_item_emailAddressDetails());
    this.end = new Date();
    this.location = {};
    this.meetingString = {};
    this.start = new Date();
    this.subject = {};
}
Office._context_mailbox_item_recipients = function () {
    this.addAsync = function (recipients, options, callback) {
        ///<summary>Adds recipients to an item.</summary>
        ///<param name="recipients" type="Array">
        /// An array containing the recipients of the item. It can be:
        /// &#10;     An array of strings containing the SMTP email addresses of the recipients.
        /// &#10;     An array of {"diplayName":, "emailAddress":} dictionaries.
        /// &#10;     An array of EmailAddressDetail objects.
        /// </param>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method. </param>
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult();
        if (arguments.length == 2) { callback = options; }
        callback(result);
    };
    this.getAsync = function (options, callback) {
        ///<summary>Gets the list of recipients for the item.</summary>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method.</param>
        ///<param name="callback" type="function">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult("recipientGetAsync");
        if (arguments.length == 1) { callback = options; }
        callback(result);
    };
    this.setAsync = function (recipients, options, callback) {
        ///<summary>Sets the recipients of an item.</summary>
        ///<param name="recipients" type="Array">
        /// An array containing the recipients of the item. The array is limited to 100 entries. It can be:
        /// &#10;     An array of strings containing the SMTP email addresses of the recipients.
        /// &#10;     An array of {"diplayName":, "emailAddress":} dictionaries.
        /// &#10;     An array of EmailAddressDetail objects.
        /// </param>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method. </param>
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult();
        if (arguments.length == 2) { callback = options; }
        callback(result);
    }
}
Office._context_mailbox_item_phoneNumber = function () {
    ///<field name="originalPhoneString" type='String'>Gets the text that was identified in an item as a phone number.</field>
    ///<field name="phoneString" type='String'>Gets a text string identified as a phone number.</field>
    ///<field name="type" type='String'>Gets the type of a phone number.</field>
    this.originalPhoneString = {};
    this.phoneString = {};
    this.type = {};
}
Office._context_mailbox_item_subject = function () {
    this.getAsync = function (options, callback) {
        ///<summary>Gets the subject of an item.</summary>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method.</param>
        ///<param name="callback" type="function">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult("subjectGetAsync");
        if (arguments.length = 1) { callback = options; }
        callback(result);
    };

    this.setAsync = function (data, options, callback) {
        ///<summary>Sets the subject of an item.</summary>
        ///<param name="data" type="String">The subject of the item. The string is limited to 255 characters.</param>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method.</param>
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult();
        if (arguments.length == 2) { callback = options; }
        callback(result);
    }
}
Office._context_mailbox_item_taskSuggestion = function () {
    ///<field name="assignees" type='EmailAddressDetails[]'>Gets the users that should be assigned a suggested task.</field>
    ///<field name="taskString" type='String'>Gets the text of an item that was identified as a task suggestion.</field>
    this.assignees = new Array(new Office._context_mailbox_item_emailAddressDetails());
    this.taskString = {};
}
Office._context_mailbox_item_time = function () {
    this.getAsync = function (options, callback) {
        ///<summary>Gets the UTC value of a time.</summary>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method.</param>
        ///<param name="callback" type="function">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult("subjectGetAsync");
        if (arguments.length == 1) { callback = options; }
        callback(result);
    };

    this.setAsync = function (dateTime, options, callback) {
        ///<summary>Sets the subject of an item.</summary>
        ///<param name="dateTime" type="String">The date and time to set in UTC.</param>
        ///<param name="options" type="Object" optional="true">Any optional parameters or state data passed to the method.</param>
        ///<param name="callback" type="function" optional="true">The method to call when the asynchronous request is complete.</param>

        var result = new Office._Mailbox_AsyncResult();
        if (arguments.length == 2) { callback = options; }
        callback(result);
    }
}
Office._context_mailbox_userProfile = function () {
    ///<field name="displayName" type='String'>Gets the user's display name.</field>
    ///<field name="emailAddress" type='String'>Gets the user's SMTP email address.</field>
    ///<field name="timeZone" type='String'>Gets the user's local time zone.</field>
    this.displayName = {};
    this.emailAddress = {};
    this.timeZone = {};
}

Office._context_mailbox_selectedDataResult = function () {
    ///<field name="data" type='String'>The selected text.</field>
    ///<field name="sourceProperty" type='String'>Indicates whether the data is from the item's subject line or body.</field>
    this.data = {};
    this.sourceProperty = {};
}

Office._Mailbox_AsyncResult = function (method) {
    ///<field name="asyncContext" type='Object'>The userContext parameter passed to the callback function.</field>
    ///<field name="error" type='Object'>Any error that occured while processing the asynchronous request.</field>
    ///<field name="status" type='Object'>The status of the asynchronous request.</field>
    this.asyncContext = {};
    this.error = {};
    this.status = {};

    if (method == "attachmentAsync") {
        this.value = {}
        intellisense.annotate(this, {
            ///<field name="value" type='String'>The identifier of the attachment.</field>
            value: null
        });
    }

    if (method == "getCallbackTokenAsync") {
        this.value = {};
        intellisense.annotate(this, {
            ///<field name="value" type='String'>The EWS callback token.</field>
            value: null
        });
    }

    if (method == "getUserIdentityTokenAsync") {
        this.value = {}
        intellisense.annotate(this, {
            ///<field name="value" type='String'>A JSON Web token that identifies the current user.</field>
            value: null
        });
    }

    if (method == "makeEwsRequestAsync") {
        this.value = {};
        intellisense.annotate(this, {
            ///<field name="value" type='String'>The XML response from the EWS operation.</field>
            value: null
        });
    }

    if (method == "loadCustomPropertiesAsync") {
        this.value = new Office._context_mailbox_item_customProperties();
        intellisense.annotate(this, {
            ///<field name="value" type='CustomProperties'>The custom properties</field>
            value: null
        });
    }

    if (method == "bodyGetTypeAsync") {
        this.value = {};
        annotate(this, {
            ///<field name="value" type='String'>A value that indicates whether the body is text or HTML.</field>
            value: undefined
        });
    }

    if (method == "locationGetAsync") {
        this.value = {};
        intellisense.annotate(this, {
            ///<field name="value" type='String'>The location of the appointment.</field>
            value: undefined
        });
    }

    if (method == "recipientGetAsync") {
        this.value = {};
        intellisense.annotate(this, {
            ///<field name="value" type='String'>An array of EmailAddressDetails objects containt the recipients of the item.</field>
            value: undefined
        });
    }

    if (method == "subjectGetAsync") {
        this.value = {};
        intellisense.annotate(this, {
            ///<field name="value" type='String'>The subject of the item.</field>
            value: undefined
        });
    }

    if (method == "getSelectedDataAsync") {
        this.value = new Office._context_mailbox_selectedDataResult();
        intellisense.annotate(this, {
            ///<field name="value" type='String'>The selected data.</field>
            value:undefined
        });
    }
}

Office._MailboxEnums = function () {
    this.AttachmentType = {
        ///<field type="String">Specifies that the attachment is a file.</field>
        File: "file",
        ///<field type="String">Specifies that the attachment is an email message, appointment, or task.</field>
        Item: "item"
    };
    this.BodyType = {
        ///<field type="String">Specifies that the item body is in HTML format.</field>
        HTML: "HTML",
        ///<field type="String">Specifies that the item body is text format.</field>
        Text: "text"
    };
    this.EntityType = {
        ///<field type="String">Specifies that the entity is a meeting suggestion.</field>
        MeetingSuggestion: "meetingSuggestion",
        ///<field type="String">Specifies that the entity is a task suggestion.</field>
        TaskSuggestion: "taskSuggestion",
        ///<field type="String">Specifies that the entity is a postal address.</field>
        Address: "address",
        ///<field type="String">Specifies that the entity is SMTP email address.</field>
        EmailAddress: "emailAddress",
        ///<field type="String">Specifies that the entity is an Internet URL.</field>
        Url: "url",
        ///<field type="String">Specifies that the entity is US phone number.</field>
        PhoneNumber: "phoneNumber",
        ///<field type="String">Specifies that the entity is a contact.</field>
        Contact: "contact"
    };
    this.ItemType = {
        ///<field type="String">Specifies a message item. This is an IPM.Note type.</field>
        Message: "message",
        ///<field type="String">Specifies an appointment item. This is an IPM.Appointment type.</field>
        Appointment: "appointment"
    };
    this.SourceProperty = {
        ///<field type="String">Specifies that the source of the text is the body of an appointment or message.</field>
        MailBody: "mailBody",
        ///<field type="string">Specifies that the source of the text is the subject of an appointment or message.</field>
        MailSubject: "mailSubject"
    };
    this.RecipientType = {
        ///<field type="String">Specifies that the recipient is not one of the other recipient types.</field>
        Other: "other",
        ///<field type="String">Specifies that the recipient is a distribution list containing a list of email addresses.</field>
        DistributionList: "distributionList",
        ///<field type="String">Specifies that the recipient is an SMTP email address that is on the Exchange server.</field>
        User: "user",
        ///<field type="String">Specifies that the recipient is an SMTP email address that is not on the Exchange server.</field>
        ExternalUser: "externalUser"
    };
    this.ResponseType = {
        ///<field type="String">Specifies that no response has been received.</field>
        None: "none",
        ///<field type="String">Specifies that you are the meeting organizer.</field>
        Organizer: "organizer",
        ///<field type="String">Specifies that the attendee is tentatively attending.</field>
        Tentative: "tentative",
        ///<field type="String">Specifies that the attendee is attending.</field>
        Accepted: "accepted",
        ///<field type="String">Specifies that the attendee is not attending.</field>
        Declined: "declined"
    };
};