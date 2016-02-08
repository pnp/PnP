using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class MailHelper
    {
        /// <summary>
        /// This method retrieves the email folders of the current user
        /// </summary>
        /// <param name="startIndex">The startIndex (0 based) of the folders to retrieve, optional</param>
        /// <returns>A page of up to 10 email folders</returns>
        public static List<MailFolder> ListFolders(Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/mailFolders?$skip={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    startIndex));

            var folders = JsonConvert.DeserializeObject<MailFolderList>(jsonResponse);
            return (folders.Folders);
        }

        /// <summary>
        /// This method retrieves the email messages from a folder in the current user's mailbox
        /// </summary>
        /// <param name="folderId">The ID of the target folder, optional</param>
        /// <param name="startIndex">The startIndex (0 based) of the email messages to retrieve, optional</param>
        /// <param name="includeAttachments">Defines whether to include attachments or not, optional</param>
        /// <returns>A page of up to 10 email messages in the folder</returns>
        public static List<MailMessage> ListMessages(String folderId = null, Int32 startIndex = 0, Boolean includeAttachments = false)
        {
            String targetUrl = null;

            if (!String.IsNullOrEmpty(folderId))
            {
                targetUrl = String.Format("{0}me/mailFolders/{1}/messages?$skip={2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    folderId, startIndex);
            }
            else
            {
                targetUrl = String.Format("{0}me/messages?$skip={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    startIndex);
            }

            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(targetUrl);

            var messages = JsonConvert.DeserializeObject<MailMessageList>(jsonResponse);

            if (includeAttachments)
            {
                foreach (var message in messages.Messages.Where(m => m.HasAttachments))
                {
                    message.LoadAttachments();
                }
            }

            return (messages.Messages);
        }

        /// <summary>
        /// This method retrieves an email message from the current user's mailbox
        /// </summary>
        /// <param name="id">The ID of the email message</param>
        /// <param name="includeAttachments">Defines whether to include attachments or not, optional</param>
        /// <returns>The email message</returns>
        public static MailMessage GetMessage(String id, Boolean includeAttachments = false)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/messages/{1}",
                MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                id));

            var message = JsonConvert.DeserializeObject<MailMessage>(jsonResponse);

            if (includeAttachments)
            {
                // message.LoadAttachments();
                message.LoadAttachmentsSmartly();
            }

            return (message);
        }

        /// <summary>
        /// Extension method to load the attachments of an email message
        /// </summary>
        /// <param name="message">The target email message</param>
        public static void LoadAttachments(this MailMessage message)
        {
            if (message.HasAttachments)
            {
                String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                    String.Format("{0}me/messages/{1}/attachments",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    message.Id));

                var attachments = JsonConvert.DeserializeObject<MailAttachmentList>(jsonResponse);
                message.Attachments.AddRange(attachments.Attachments);

                foreach (var attachment in message.Attachments)
                {
                    attachment.ParentMessageId = message.Id;
                }
            }
        }

        /// <summary>
        /// Extension method to load the attachments of an email message in a smart way
        /// </summary>
        /// <param name="message">The target email message</param>
        public static void LoadAttachmentsSmartly(this MailMessage message)
        {
            if (message.HasAttachments)
            {
                String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                    String.Format("{0}me/messages/{1}/attachments?$select=contentType,id,name,size",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    message.Id));

                var attachments = JsonConvert.DeserializeObject<MailAttachmentList>(jsonResponse);
                message.Attachments.AddRange(attachments.Attachments);

                foreach (var attachment in message.Attachments)
                {
                    attachment.ParentMessageId = message.Id;
                }
            }
        }

        /// <summary>
        /// Extension method to load the content of a specific attachment
        /// </summary>
        /// <param name="message">The target email message</param>
        public static void EnsureContent(this MailAttachment attachment)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/messages/{1}/attachments/{2}",
                MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                attachment.ParentMessageId,
                attachment.Id));

            var result = JsonConvert.DeserializeObject<MailAttachment>(jsonResponse);
            attachment.Content = result.Content;
        }

        /// <summary>
        /// This method sends an email message
        /// </summary>
        /// <param name="message"></param>
        public static void SendMessage(MailMessageToSend message)
        {
            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}me/microsoft.graph.sendMail",
                MicrosoftGraphHelper.MicrosoftGraphV1BaseUri),
                message, "application/json");
        }

        /// <summary>
        /// This method sends a reply to an email message
        /// </summary>
        /// <param name="messageId">The ID of the message to reply to</param>
        /// <param name="comment">Any comment to include in the reply, optional</param>
        public static void Reply(String messageId,
            String comment = null)
        {
            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}me/messages/{1}/reply",
                MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, messageId),
                content: !String.IsNullOrEmpty(comment) ? new { Comment = comment } : null,
                contentType: "application/json");
        }

        /// <summary>
        /// This method sends a reply all to an email message
        /// </summary>
        /// <param name="messageId">The ID of the message to reply all to</param>
        /// <param name="comment">Any comment to include in the reply all, optional</param>
        public static void ReplyAll(String messageId,
            String comment = null)
        {
            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}me/messages/{1}/replyAll",
                MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, messageId),
                content: !String.IsNullOrEmpty(comment) ? new { Comment = comment } : null,
                contentType: "application/json");
        }

        /// <summary>
        /// This method forwards an email message to someone else
        /// </summary>
        /// <param name="messageId">The ID of the message to forward</param>
        /// <param name="recipients">The recipients of the forward</param>
        /// <param name="comment">Any comment to include in the reply all, optional</param>
        public static void Forward(String messageId,
            List<UserInfoContainer> recipients,
            String comment = null)
        {
            MicrosoftGraphHelper.MakePostRequest(
                String.Format("{0}me/messages/{1}/forward",
                MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, messageId),
                content: new {
                    Comment = !String.IsNullOrEmpty(comment) ? comment : null,
                    ToRecipients = recipients,
                }, contentType: "application/json");
        }
    }
}