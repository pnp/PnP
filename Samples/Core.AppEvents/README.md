# SharePoint Add-In Event Handler with Rollback Logic #

### Summary ###
This sample shows how to implement handlers for the **AppInstalled** and **AppUninstalling** events that:
- Incorporate rollback logic if the handler encounters an error.
- Incorporate "already done" logic to accommodate the fact that SharePoint retries the handler up to three more times if it fails or takes more than 30 seconds to complete.


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
Core.AppEvents | Ricky Kirkham (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 17th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
This sample is one of two that show different ways to implement a handler for the **AppInstalled** event that adds a list to the host web. It includes rollback-on-error logic that deletes the list if the installation has to cancelled. The samples also show how to implement an **AppUninstalling** event that recycles the list when the add-in is uninstalled. It includes rollback-on-error logic that restores the list from the recycle bin if the uninstallation has to be canceled.

In this sample, the error handling and "already done" logic is executed on the web service server. This may involve many calls across the internet from the web service to the SharePoint farm or tenancy. The other sample, **Core.AppEvents.HandlerDelegation**, uses an alternate design that can reduce the number of calls across the internet. However, the technique used in that other sample cannot be used in every scenario, so there is a need for this sample too.

For more information about the two different techniques and when they can be used, see the MSDN article [Handling events in apps for SharePoint](https://msdn.microsoft.com/en-us/library/office/jj220048.aspx) and it's child articles. 

# To use this sample #
1. Open the .sln file for the sample in **Visual Studio**.
2. In Solution Explorer, highlight the SharePoint add-in project and replace the **Site URL** property with the URL of your SharePoint developer site.
3. Configure the project for debugging as instructed in the MSDN article [How to: Debug a remote event receiver in an add-in for SharePoint](https://msdn.microsoft.com/EN-US/library/office/dn275975.aspx).
3. Open the AppEventReceiver.svc.cs file and in the second line of the ProcessEvent method, change the string "TestList" to a string that is *not* already a list on the test SharePoint host web.
4. You can now run the sample with F5. The first time you do, you are prompted to trust the add-in. The default page of the web application then opens and displays some text that reminds you not to close the page while you are debugging.
5. Minimize, but *do not close*, the page, and then open (or refresh) the **Site Contents** page of your test SharePoint site. You will see both the **Core.AppEvents** add-in installed and a new list with the name you used in step 3 above. The presence of the list verifies that your handler was executed.
6. To test the **AppUninstalling** handler, begin by removing the **Core.AppEvents** add-in from the **Site Contents** page. You do this by clicking the "**...**" callout button on the add-in tile and then clicking **Remove**. (This does *not* trigger the **AppUninstalling** event!)
7. Open the recycle bin. (There's a link to it in the left navigation bar of the **Site Contents** page.) The **Core.AppEvents** add-in will be the top item listed. 
8. Click the checkbox for **Core.AppEvents** add-in, and then click **Delete Selection**.
9. Open the second-stage recycle bin. (There's a link to it at the bottom of the **Recycle Bin** page.) The **Core.AppEvents** add-in will be the top item listed. 
10. Click the checkbox for **Core.AppEvents** add-in, and then click **Delete Selection**. It is *this* action that triggers the **AppUninstalling** event.
11. Open (or refresh) the **Site Contents** page, the new list is no longer there. This verifies that your **AppUninstalling** handler was executed. Go back to the (first-stage) recycle bin. The list is now the top item. 
12. You can now close the default page of the web application or stop debugging in **Visual Studio**.

###To test the exception handling

There is a comment in the **try** blocks of the both the **AppInstalled** and **AppUninstalling** handlers that explains how to test the exception handling. 

An exception in the **AppInstalled** handler should leave neither the **Core.AppEvents** add-in nor the new list on the **Site Contents** page. An exception in the **AppUninstalling** should leave the **Core.AppEvents** add-in in the second-stage recycle bin and it should leave the new list on the **Site Contents** page.

<a name="resources"/>
# Additional resources

* [Handling events in apps for SharePoint](https://msdn.microsoft.com/en-us/library/office/jj220048.aspx) and it's child articles.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.AppEvents" />