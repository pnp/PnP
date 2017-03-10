# Office 365 Python Flask App Authentication #

### Summary ###
This scenario shows how to set up authentication between a Python app (using the Flask microframework) and Office 365 SharePoint Online site. The goal of this sample is to show how a user can authenticate and interact with data from the Office 365 SharePoint site.

### Applies to ###
- Office 365 Multi Tenant (MT)
- Office 365 Dedicated (D)

### Prerequisites ###
- Office 365 developer tenant
- Visual Studio 2015 installed
- Python Tools for Visual Studio installed
- Python 2.7 or 3.4 installed
- Flask, requests, PyJWT Python packages installed via pip

### Solution ###
Solution | Author(s)
---------|----------
Python.Office365.AppAuthentication | Velin Georgiev (**OneBit Software**), Radi Atanassov (**OneBit Software**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 9th 2016 | Initial release (Velin Georgiev)

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# The Office 365 Python Flask App Authentication Sample #
This section describes the Office 365 Python Flask App Authentication sample included in the current solution.

# Prepare the scenario for the Office 365 Python Flask app authentication sample #
The Office 365 Python Flask application will:

- Use Azure AD authorization endpoints to perform authentication
- Use Office 365 SharePoint API's to show the authenticated user's title

For these tasks to succeed you need to do additional setups explained below. 

- Create Azure trial account with the Office 365 account so the app can be registered, or you can register it with PowerShell. A good tutorial can be found on this link https://github.com/OfficeDev/PnP/blob/497b0af411a75b5b6edf55e59e48c60f8b87c7b9/Samples/AzureAD.GroupMembership/readme.md.
- Register the app in the Azure portal and assign http://localhost:5555 to the Sign-on URL and Reply URL
- Generate a client secret
- Grant the following permission to the Python Flask app: Office 365 SharePoint Online > Delegated Permissions > Read user profiles

![Azure portal permission setting](https://lh3.googleusercontent.com/-LxhYrbik6LQ/VrnZD-0Uf0I/AAAAAAAACaQ/jsUjHDQlmd4/s732-Ic42/office365-python-app2.PNG)

- Copy the client secret and the client id from the Azure portal and replace them into the Python Flask config file
- Assign URL to the SharePoint site you are going to access to the RESOURCE config variable.

![App details in config file](https://lh3.googleusercontent.com/-ETtW5MBuOcA/VrnZDQBAxQI/AAAAAAAACaY/ppp4My1JTlE/s616-Ic42/office365-python-app-config.PNG)

- Open the sample in Visual Studio 2015
- Go to Project > Properties > Debug and dedicate 5555 for Port Number

![Change port in debug option](https://lh3.googleusercontent.com/-M3upxeCKBN0/VrnZDSHnDoI/AAAAAAAACaA/BF4CTeKlUMs/s426-Ic42/office365-python-app-vs-config.PNG)

- Go to Python environments > your active python environment > execute "Install from requirements.txt". This will ensure that all the required Python packages are installed.

![Selection of menu option](https://lh3.googleusercontent.com/-At6Smrxg9DQ/VrnZD6KMvfI/AAAAAAAACaM/gcgJUATPigE/s479-Ic42/office365-python-packages.png)

## Run the Office 365 Python Flask app sample ##
When you run the sample you'll see the title and login url.

![Add-in UI](https://lh3.googleusercontent.com/-GDdAcmYylZE/VrnZD8sVGwI/AAAAAAAACaI/1gB0jvULLBo/s438-Ic42/office365-python-app.PNG)


Once you've clicked the sign-in link, the Office 365 API will go through the authentication handshake and the Python Flask home screen will reload with the logged in user title and access token displayed:

![Signing in UI](https://lh3.googleusercontent.com/-44rsAE2uGFQ/VrnZDdJAseI/AAAAAAAACaE/70N8UX8ErIk/s569-Ic42/office365-python-app-result.PNG)

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Office365.AppAuthentication" />