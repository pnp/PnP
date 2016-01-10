using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public class ContactsHelper
    {
        /// <summary>
        /// This method retrieves the calendars of the current user
        /// </summary>
        /// <param name="startIndex">The startIndex (0 based) of the folders to retrieve, optional</param>
        /// <returns>A page of up to 10 calendars</returns>
        public static List<Contact> ListContacts(Int32 startIndex = 0)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/contacts?$skip={1}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    startIndex));

            var contactList = JsonConvert.DeserializeObject<ContactList>(jsonResponse);
            return (contactList.Contacts);
        }

        public static Stream GetContactPhoto(String contactId)
        {
            String contentType = "image/png";

            var result = MicrosoftGraphHelper.MakeGetRequestForStream(
                String.Format("{0}me/contacts/{1}/photo/$value",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, contactId),
                contentType);

            return (result);
        }

    }
}