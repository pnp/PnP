# Site provisioning with custom UI modules #

**Sample is removed, since it was not up to date on latest guidance and patterns around site collection creation**

Key challenge of this solution was that it was creating site collections in synchronious way, which is not suitable since site collection creation will take usually longer than 1 minute, which will result timeout in the Azure web sites. In on-premises you could increase this timeout, but it's not good practice to run this operation in synchronious way.

Solution also did not use PnP Provisionign engine, it rather had it's own xml based configuration, which was not well documented and was rather technical proof of concept than suitable for production usage.

If you are looking into site collection provisioning, you should be looking into following PnP samples

- [Provisioning.Framework.Cloud.Async](https://github.com/OfficeDev/PnP/tree/master/Solutions/Provisioning.Framework.Cloud.Async) - Simplified solution around PnP remote provisioning engine usage asynchroniously based on Azure WebJobs
- [PnP Partner Pack](http://aka.ms/officedevpnppartnerpack) - Start kit for solution providing UI and automated asynchronious creation of site collection.
- [Provisioning.UX.App](https://github.com/OfficeDev/PnP/tree/master/Solutions/Provisioning.UX.App) - Most comprehensive solution with multi-lingual and branding configuration support


----------
