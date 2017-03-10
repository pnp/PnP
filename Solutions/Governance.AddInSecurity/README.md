# Securing Add-in Principals #

### Summary ###

For the remote components of a provider-hosted SharePoint Add-in to interact with SharePoint using OAuth, the add-in must first register with the Azure ACS cloud-based service and the SharePoint App Management Service of the tenancy.  The registration involves creating new Client ID/Secret and other add-in specific details.  After you register your add-in, it has an add-in identity and is a security principal, referred to as an add-in principal. When you install your add-in, SharePoint administrators can retrieve information about that particular add-in principal. The Client ID/Secret values need to be entered in configuration file of the application.  Anyone who has access to the file system of the config file location can read these values.  Using these values, they can build their own application and do anything in the SharePoint they want to (though its only restricted to app permissions defined for the app principal).  This can become a big security threat especially if the app is given app-only policy permission.

This is a governance problem.  Any organization need to have well defined governance on the deployment process, person responsible in making the changes in the configuration file and so on.  If the config file is not well protected enough, it can become a problem.  To address this scenario, we need to secure the app principle itself, so that even if someone gets access to these principles they shouldn't be able to do anything with it. The idea is to encrypt the key values using Triple-DES algorithm at Machine level. Using the encrypted value generated in one machine we can't decrypted the value in another machine, it has to be decrypted in the same machine. When an application calls the DPAPI encryption routine, it can specify an optional secondary entropy ("secret" bytes) that will have to be provided by an application attempting to decrypt data. 

### Features ###
- Encrypt/Decrypt Keys
- Machine/User level scope


### Applies to ###
-  Office 365 Multi-tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises 


### Solution ###
Solution | Author(s)
---------|----------
Governance.AddInSecurity | Chandrasekar Natarajan

### Version history ###
Version  | Date | Comments
---------| -----| --------
.1  | February 25, 2016 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

**NOTICE THIS SOLUTION IS UNDER ACTIVE DEVELOPMENT**

# Encryption #

- Enter the Client ID or the Secret value in Client ID/Secret text box.  
- Entropy key is optional field.  
- Choose the scope as Local Machine (this is the default value)
- Click Encrypt button to get the encrypted text value and hit copy text to copy the values. 

![UI for providing client ID and secret](http://i.imgur.com/dWBFuZP.png)


![Text is copied message shown when encryption is done](http://i.imgur.com/bhGB5Rg.png)
 

### Validation ###

To validate the encrypted text, without altering any values, click the Decrypt button and you should see the original value of the ID/Secret given. 

![Decryp option](http://i.imgur.com/S8WciiJ.png)

If the value is tampered or if you try to use the encrypted text value in a different machine, the decryption will fail. 

![Show the decrypted value](http://i.imgur.com/HHWKQ5d.png)

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.AddInSecurity" />