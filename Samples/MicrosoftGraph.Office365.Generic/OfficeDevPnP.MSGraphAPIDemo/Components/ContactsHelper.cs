using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class ContactsHelper
    {
        /// <summary>
        /// This method retrieves the contacts of the current user
        /// </summary>
        /// <param name="startIndex">The startIndex (0 based) of the contacts to retrieve, optional</param>
        /// <returns>A page of up to 10 contacts</returns>
        public static List<Contact> ListContacts(Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/contacts?$skip={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    startIndex));

            var contactList = JsonConvert.DeserializeObject<ContactList>(jsonResponse);
            return (contactList.Contacts);
        }

        /// <summary>
        /// This method retrieves the contacts of a contacts folder for the current user
        /// </summary>
        /// <param name="contactFolderId">The ID of the contacts folder</param>
        /// <param name="startIndex">The startIndex (0 based) of the contacts to retrieve, optional</param>
        /// <returns>A page of up to 10 contacts</returns>
        public static List<Contact> ListContacts(String contactFolderId, Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/contactFolders/{1}/contacts?$skip={2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    contactFolderId,
                    startIndex));

            var contactList = JsonConvert.DeserializeObject<ContactList>(jsonResponse);
            return (contactList.Contacts);
        }

        /// <summary>
        /// Retrieves the picture of a contact, if any
        /// </summary>
        /// <param name="contactId">The ID of the contact</param>
        /// <returns>The picture as a binary Stream</returns>
        public static Stream GetContactPhoto(String contactId)
        {
            Stream result = null;
            String contentType = "image/png";

            try
            {
                result = MicrosoftGraphHelper.MakeGetRequestForStream(
                    String.Format("{0}me/contacts/{1}/photo/$value",
                        MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, contactId),
                    contentType);
            }
            catch (ApplicationException ex)
            {
                HttpException httpException = ex.InnerException as HttpException;
                if (httpException != null && httpException.GetHttpCode() == 404)
                {
                    // If 404 -> The contact does not have a picture
                    // Keep NULL value for result
                    result = null;
                }
            }

            return (result);
        }

        /// <summary>
        /// This method retrieves a single contact
        /// </summary>
        /// <param name="contactId">The ID of the contact to retrieve</param>
        /// <returns>The retrieved contact</returns>
        public static Contact GetContact(String contactId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/contacts/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    contactId));

            var contact = JsonConvert.DeserializeObject<Contact>(jsonResponse);
            return (contact);
        }

        /// <summary>
        /// This method updates a contact
        /// </summary>
        /// <param name="contact">The contact to update</param>
        /// <returns>The updated contact</returns>
        public static Contact UpdateContact(Contact contact)
        {
            String jsonResponse = MicrosoftGraphHelper.MakePatchRequestForString(
                String.Format("{0}me/contacts/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    contact.Id),
                    contact,
                    "application/json");

            var updatedContact = JsonConvert.DeserializeObject<Contact>(jsonResponse);
            return (updatedContact);
        }

        /// <summary>
        /// This method adds a contact
        /// </summary>
        /// <param name="contact">The contact to add</param>
        /// <returns>The added contact</returns>
        public static Contact AddContact(Contact contact)
        {
            String jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}me/contacts",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri),
                    contact,
                    "application/json");

            var addedContact = JsonConvert.DeserializeObject<Contact>(jsonResponse);
            return (addedContact);
        }

        /// <summary>
        /// This method deletes a contact
        /// </summary>
        /// <param name="contactId">The ID of the contact to delete</param>
        public static void DeleteContact(String contactId)
        {
            MicrosoftGraphHelper.MakeDeleteRequest(
                String.Format("{0}me/contacts/{1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    contactId));
        }
    }
}