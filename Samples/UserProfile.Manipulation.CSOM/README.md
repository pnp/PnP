# User profile manipulation using CSOM #

### Summary ###
This sample shows simple operations on how to manipulate user profile using Client Side Object Model (CSOM). It is using the latest SharePoint Online CSOM, which is exposing APIs also to update user profile properties. You can download the latest version of the SharePoint online client SDK from following link - http://aka.ms/spocsom

### Applies to ###
-  Office 365 Multi Tenant (MT)


### Prerequisites ###
Capability will have to be enabled in the used tenant. This will happen gradually for all public tenants.

### Solution ###
Solution | Author(s)
---------|----------
UserProfile.Manipulation.CSOM | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 10th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Reading user profile properties #
Reading of the user profile properties using CSOM has been supported since the RTM version of SharePoint 2013 or SharePoint Online. This model has remained the same since. 

In this sample we are calling into the user profile CSOM to list all the user profile properties from current user. If user has specific access rights, you can also request or read properties from other profiles.

![Add-in UI](http://i.imgur.com/RnIBWv5.png)


Code for reading the user profile properties is pretty straight forward. You will need to have reference to Microsoft.SharePoint.Client.UserProfiles assembly which is providing the needed objects for accessing user profile capabilities in the SharePoint.

```C#
var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

using (var clientContext = spContext.CreateUserClientContextForSPHost())
{
    // Get the people manager instance and load current properties
    PeopleManager peopleManager = new PeopleManager(clientContext);
    PersonProperties personProperties = peopleManager.GetMyProperties();
    clientContext.Load(personProperties);
    clientContext.ExecuteQuery();

    // just to output what we have now
    txtProperties.Text = "";
    foreach (var item in personProperties.UserProfileProperties)
    {
        txtProperties.Text += string.Format("{0} - {1}{2}", item.Key, item.Value, Environment.NewLine);
    }
}
```

# Updating user profile property using CSOM #
Updating of the user profile properties using CSOM is really straight forward with the updates to CSOM. You can simply update property by calling new methods. There is actually two different methods, one for single value properties and one for multi-value properties. 

Here's the UI for the single value property update. In this sample we are updating the *About me* property, but code is identical with any other property as well.

![Add-in UI for scenario 2](http://i.imgur.com/We6lHkM.png)

Actual code is pretty simple. We will just need to use the *PeopleManager* object, which is exposing needed method called *SetSingleValueProfileProperty* for property update.

```C#
var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

using (var clientContext = spContext.CreateUserClientContextForSPHost())
{
    // Get the people manager instance for current context to get account name
    PeopleManager peopleManager = new PeopleManager(clientContext);
    PersonProperties personProperties = peopleManager.GetMyProperties();
    clientContext.Load(personProperties, p => p.AccountName);
    clientContext.ExecuteQuery();

    // Update the AboutMe property for the user using account name from profile
    peopleManager.SetSingleValueProfileProperty(personProperties.AccountName, "AboutMe", txtAboutMe.Text);
    clientContext.ExecuteQuery();

}
```

**Notice.** Model is identical if you are using custom properties as well, so there's really no difference on the actual code. You will need to though remember to configure the user profile property to be editable by the end users, so that code can update that.

# Updating multi-value user profile property #
This code is pretty much identical as for the single value update, we are just bypassing list of values, which CSOM API will update for the property in the SharePoint side. Here's the sample UI for this scenario.

![Add-in UI for scenario 3](http://i.imgur.com/5rRLUAw.png)


Below code example shows how to update multi-value property called skills. 

```C#
var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

using (var clientContext = spContext.CreateUserClientContextForSPHost())
{
    // Get the people manager instance for current context to get account name
    PeopleManager peopleManager = new PeopleManager(clientContext);
    PersonProperties personProperties = peopleManager.GetMyProperties();
    clientContext.Load(personProperties, p => p.AccountName);
    clientContext.ExecuteQuery();

    // Collect values for profile update
    List<string> skills = new List<string>();
    for (int i = 0; i < lstSkills.Items.Count; i++)
    {
        skills.Add(lstSkills.Items[i].Value);
    }

    // Update the SPS-Skills property for the user using account name from profile.
    peopleManager.SetMultiValuedProfileProperty(personProperties.AccountName, "SPS-Skills", skills);
    clientContext.ExecuteQuery();

    //Refresh the values 
    RefreshUIValues();
}
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/UserProfile.Manipulation.CSOM" />