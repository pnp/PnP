using Microsoft.Office365.OutlookServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Helpers
{
    public class ContactsHelper : BaseOffice365Helper
    {
        public ContactsHelper(AuthenticationHelper authenticationHelper) : 
            base(authenticationHelper)
        {
        }

        public async Task<IEnumerable<IContact>> GetContacts()
        {
            var client = await this.AuthenticationHelper
                .EnsureOutlookServicesClientCreatedAsync(
                Office365Capabilities.Contacts.ToString());

            // Obtain first page of contacts
            var contactsResults = await (from i in client.Me.Contacts
                                         orderby i.DisplayName
                                         select i).ExecuteAsync();
            
            return contactsResults.CurrentPage;
        }
    }
}
