# Office 365 Modern Provisioning #

## Summary ##

This sample demonstrates how to integrate a typical enterprise scenario where the user can submit a creation's request
for a new SharePoint team site, communication site or a Microsoft team through a node.js Bot (App Only) which is available on Teams, Skype, Direct line and so on.
The request is stored in a SharePoint list accessible only by an admin which can approve it, triggering a Microsoft Flow
that contains the logic necessary to send an email to the end user and the admin in order to notify that the process is started.
After that, if a request has the status equal to "Requested" the latter is processed by calling an Azure c# function that
creates a SharePoint team site, communication site or a Microsoft Team.
Microsoft Flow receives a response from the Azure function with HTTP status 200, at the end the user receives an email that notifies the end of the process.
Furthermore, there is also a SharePoint Framework Application Customizer which allows the user to interact with the Bot by leveraging the capabilities of the direct line from a SharePoint site.

### When to use this pattern? ###
This sample is suitable when you want to implement a typical enterprise scenario in order to request and approving the creation of a new SharePoint site or Microsoft team. 

<p align="center">
  <img src="./images/o365-modern-provisioning.gif"/>
</p>

## Used SharePoint Framework Version 
![drop](https://img.shields.io/badge/drop-1.4.1-green.svg)

## Applies to

* [SharePoint Framework](https:/dev.office.com/sharepoint)
* [Office 365 tenant](https://dev.office.com/sharepoint/docs/spfx/set-up-your-development-environment)

## Solution

Solution|Author(s)
--------|---------
O365-Modern-Provisioning | Giuliano De Luca (MVP Office Development at HUGO BOSS) - Twitter @giuleon

## Version history

Version|Date|Comments
-------|----|--------
1.0 | February 19, 2018 | Initial release

## Disclaimer
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

---

## Minimal Path to Awesome

- Clone this repository
- Setup the environment as described below
- In the command line run:
  - `npm install`
  - `gulp bundle`
  - `gulp package-solution`
- Upload the generated package to the SharePoint app catalog
- Install the *PnP - Graph Bot* app in your site
- In the command line run:
  - `gulp serve --nobrowser`
- Play with the bot!

## Prerequisites ##
 
