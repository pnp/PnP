# PnP Unit Test report for OnlineAppOnly on Monday, August 10, 2015 #
This page is showing the results of the PnP unit test run.

## Test configuration ##
This report contains the unit test results from the following run:

Parameter | Value
----------|------
PnP Unit Test configuration | OnlineAppOnly
Test run date | Monday, August 10, 2015
Test run time | 10:14 PM
PnP branch | dev
Visual Studio build configuration | debug

## Test summary ##
During this test run 249 tests have been executed with following outcome:

Parameter | Value
----------|------
Executed tests | 249
Elapsed time | 0h 28m 16s
Passed tests | 213
Failed tests | **3**
Skipped tests | 33
Was canceled | False
Was aborted | False
Error | 

## Test run details ##

### Failed tests ###
<table>
<tr>
<td><b>Test name</b></td>
<td><b>Test outcome</b></td>
<td><b>Duration</b></td>
<td><b>Message</b></td>
</tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.GetOneDriveSiteCollectionsTest</td><td>Failed</td><td>0h 0m 4s</td><td>Initialization method OfficeDevPnP.Core.Tests.AppModelExtensions.TenantExtensionsTests.Initialize threw exception. Microsoft.SharePoint.Client.ServerException: Microsoft.SharePoint.Client.ServerException: Access to this Web site has been blocked.

Please contact the administrator to resolve this problem..</td></tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.SubSiteExistsTest</td><td>Failed</td><td>0h 0m 4s</td><td>Initialization method OfficeDevPnP.Core.Tests.AppModelExtensions.TenantExtensionsTests.Initialize threw exception. Microsoft.SharePoint.Client.ServerException: Microsoft.SharePoint.Client.ServerException: Access to this Web site has been blocked.

Please contact the administrator to resolve this problem..</td></tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.SetSiteLockStateTest</td><td>Failed</td><td>0h 0m 11s</td><td>Test method OfficeDevPnP.Core.Tests.AppModelExtensions.TenantExtensionsTests.SetSiteLockStateTest threw exception: 
Microsoft.SharePoint.Client.ServerException: A site already exists at url https://bertonline.sharepoint.com:443/sites/TestPnPSC_123456789_2.</td></tr>

</table>


### Skipped tests ###
<table>
<tr>
<td><b>Test name</b></td>
<td><b>Test outcome</b></td>
<td><b>Duration</b></td>
<td><b>Message</b></td>
</tr>
<tr><td>Microsoft.SharePoint.Client.Tests.ListExtensionsTests.SetDefaultColumnValuesTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Tests.AppModelExtensions.SearchExtensionsTests.SetSiteCollectionSearchCenterUrlTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Search tests are not supported when testing using app-only</td></tr>
<tr><td>Tests.AppModelExtensions.SearchExtensionsTests.GetSearchConfigurationFromWebTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Search tests are not supported when testing using app-only</td></tr>
<tr><td>Tests.AppModelExtensions.SearchExtensionsTests.GetSearchConfigurationFromSiteTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Search tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.CreateTaxonomyFieldTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.CreateTaxonomyFieldMultiValueTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.SetTaxonomyFieldValueTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.CreateTaxonomyFieldLinkedToTermSetTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.CreateTaxonomyFieldLinkedToTermTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetTaxonomySessionTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetDefaultKeywordsTermStoreTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetDefaultSiteCollectionTermStoreTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetTermSetsByNameTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetTermGroupByNameTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetTermGroupByIdTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetTermByNameTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.GetTaxonomyItemByPathTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.AddTermToTermsetTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.AddTermToTermsetWithTermIdTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ImportTermsTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ImportTermsToTermStoreTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ImportTermSetSampleShouldCreateSetTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ImportTermSetShouldUpdateSetTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ImportTermSetShouldUpdateByGuidTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ExportTermSetTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ExportTermSetFromTermstoreTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.TaxonomyExtensionsTests.ExportAllTermsTest</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.UploadFileWebDavTest</td><td>Skipped</td><td>0h 0m 1s</td><td>Assert.Inconclusive failed. Tests involving webdav are not supported when testing using app-only</td></tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.GetUserProfileServiceClientTest</td><td>Skipped</td><td>0h 0m 5s</td><td>Assert.Inconclusive failed. Web service tests are not supported when testing using app-only</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectTermGroupsTests.CanProvisionObjects</td><td>Skipped</td><td>0h 0m 0s</td><td>Assert.Inconclusive failed. Taxonomy tests are not supported when testing using app-only</td></tr>
<tr><td>Tests.Framework.Providers.BaseTemplateTests.DumpBaseTemplates</td><td>Skipped</td><td>0h 0m 0s</td><td></td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.InstallSolutionTest</td><td>Skipped</td><td>0h 0m 0s</td><td></td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.UninstallSolutionTest</td><td>Skipped</td><td>0h 0m 0s</td><td></td></tr>

</table>


### Passed tests ###
<table>
<tr>
<td><b>Test name</b></td>
<td><b>Test outcome</b></td>
<td><b>Duration</b></td>
</tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.CanUploadHtmlPageLayoutAndConvertItToAspxVersionTest</td><td>Passed</td><td>0h 1m 15s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.CanUploadPageLayoutTest</td><td>Passed</td><td>0h 0m 59s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.CanUploadPageLayoutWithPathTest</td><td>Passed</td><td>0h 0m 52s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.AllowAllPageLayoutsTest</td><td>Passed</td><td>0h 0m 49s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.DeployThemeAndCreateComposedLookTest</td><td>Passed</td><td>0h 0m 54s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.ComposedLookExistsTest</td><td>Passed</td><td>0h 0m 50s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.GetCurrentComposedLookTest</td><td>Passed</td><td>0h 1m 14s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.CreateComposedLookShouldWorkTest</td><td>Passed</td><td>0h 0m 51s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.CreateComposedLookByNameShouldWorkTest</td><td>Passed</td><td>0h 0m 46s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.SetComposedLookInheritsTest</td><td>Passed</td><td>0h 1m 24s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.SetComposedLookResetInheritanceTest</td><td>Passed</td><td>0h 2m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.SeattleMasterPageIsUnchangedTest</td><td>Passed</td><td>0h 0m 50s</td></tr>
<tr><td>Tests.AppModelExtensions.BrandingExtensionsTests.IsSubsiteTest</td><td>Passed</td><td>0h 0m 46s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FeatureExtensionsTests.ActivateSiteFeatureTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FeatureExtensionsTests.ActivateWebFeatureTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FeatureExtensionsTests.DeactivateSiteFeatureTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FeatureExtensionsTests.DeactivateWebFeatureTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FeatureExtensionsTests.IsSiteFeatureActiveTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FeatureExtensionsTests.IsWebFeatureActiveTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.CreateFieldTest</td><td>Passed</td><td>0h 0m 5s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.CanAddContentTypeToListByName</td><td>Passed</td><td>0h 0m 6s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.CanRemoveContentTypeFromListByName</td><td>Passed</td><td>0h 0m 8s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.CanRemoveContentTypeFromListById</td><td>Passed</td><td>0h 0m 9s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.CreateExistingFieldTest</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.GetContentTypeByIdTest</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.RemoveFieldByInternalNameThrowsOnNoMatchTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.CreateFieldFromXmlTest</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.ContentTypeExistsByNameTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.ContentTypeExistsByIdTest</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.ContentTypeExistsByNameInSubWebTest</td><td>Passed</td><td>0h 0m 10s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.ContentTypeExistsByIdInSubWebTest</td><td>Passed</td><td>0h 0m 11s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.ContentTypeExistsByNameSearchInSiteHierarchyTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.ContentTypeExistsByIdSearchInSiteHierarchyTest</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.AddFieldToContentTypeTest</td><td>Passed</td><td>0h 0m 5s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.AddFieldToContentTypeMakeRequiredTest</td><td>Passed</td><td>0h 0m 6s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.SetDefaultContentTypeToListTest</td><td>Passed</td><td>0h 0m 6s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.ReorderContentTypesTest</td><td>Passed</td><td>0h 0m 10s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.FieldAndContentTypeExtensionsTests.CreateContentTypeByXmlTest</td><td>Passed</td><td>0h 0m 7s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.AddJsLinkToWebTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.AddJsLinkToSiteTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.AddJsLinkIEnumerableToWebTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.AddJsLinkIEnumerableToSiteTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.DeleteJsLinkFromWebTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.DeleteJsLinkFromSiteTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.AddJsBlockToWebTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.JavaScriptExtensionsTests.AddJsBlockToSiteTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.ListExtensionsTests.CreateListTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.ListRatingExtensionTest.EnableRatingExperienceTest</td><td>Passed</td><td>0h 0m 5s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.ListRatingExtensionTest.EnableLikesExperienceTest</td><td>Passed</td><td>0h 0m 5s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.GetAdministratorsTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddAdministratorsTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddGroupTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.GroupExistsTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddPermissionLevelToGroupTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddPermissionLevelByRoleDefToGroupTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddPermissionLevelToUserTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddPermissionLevelToUserTestByRoleDefTest</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddReaderAccessToEveryoneExceptExternalsTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.SecurityExtensionsTests.AddReaderAccessToEveryoneTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.StructuralNavigationExtensionsTests.GetNavigationSettingsTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.StructuralNavigationExtensionsTests.UpdateNavigationSettingsTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.AppModelExtensions.StructuralNavigationExtensionsTests.UpdateNavigationSettings2Test</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.CheckOutFileTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.CheckInFileTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.UploadFileTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.VerifyIfUploadRequiredTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.SetFilePropertiesTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.GetFileTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.EnsureSiteFolderTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.EnsureLibraryFolderTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.AppModelExtensions.FileFolderExtensionsTests.EnsureLibraryFolderRecursiveTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.GetSiteCollectionsTest</td><td>Passed</td><td>0h 0m 27s</td></tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.CheckIfSiteExistsTest</td><td>Passed</td><td>0h 0m 13s</td></tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.SiteExistsTest</td><td>Passed</td><td>0h 0m 14s</td></tr>
<tr><td>Tests.AppModelExtensions.TenantExtensionsTests.CreateDeleteSiteCollectionTest</td><td>Passed</td><td>0h 3m 23s</td></tr>
<tr><td>Tests.AppModelExtensions.NavigationExtensionsTests.AddTopNavigationNodeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.NavigationExtensionsTests.AddQuickLaunchNodeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.NavigationExtensionsTests.AddSearchNavigationNodeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.NavigationExtensionsTests.DeleteTopNavigationNodeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.NavigationExtensionsTests.DeleteQuickLaunchNodeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.NavigationExtensionsTests.DeleteSearchNavigationNodeTest</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.AppModelExtensions.NavigationExtensionsTests.DeleteAllQuickLaunchNodesTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorGetFile1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorGetFile2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorGetFiles1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorGetFiles2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorGetFileBytes1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorGetFileBytes2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorSaveStream1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorSaveStream2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorSaveStream3Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorDelete1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorAzureTests.AzureConnectorDelete2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorGetFile1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorGetFile2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorGetFile3Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorGetFiles1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorGetFiles2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorGetFileBytes1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorSaveStream1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorSaveStream2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorSaveStream3Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorDelete1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorFileSystemTests.FileConnectorDelete2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorGetFile1Test</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorGetFile2Test</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorGetFiles1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorGetFiles2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorGetFiles3Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorGetFileBytes1Test</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorGetFileBytes2Test</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorSaveStream1Test</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorSaveStream2Test</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorSaveStream3Test</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorDelete1Test</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.Connectors.ConnectorSharePointTests.SharePointConnectorDelete2Test</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.ExtensibilityCallOut.ExtensibilityTests.CanProviderCallOut</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ExtensibilityCallOut.ExtensibilityTests.ProviderCallOutThrowsException</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ExtensibilityCallOut.ExtensibilityTests.ProviderAssemblyMissingThrowsAgrumentException</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ExtensibilityCallOut.ExtensibilityTests.ProviderTypeNameMissingThrowsAgrumentException</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ExtensibilityCallOut.ExtensibilityTests.ProviderClientCtxIsNullThrowsAgrumentNullException</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectComposedLookTests.CanCreateComposedLooks</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectPagesTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectPagesTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectSiteSecurityTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectSiteSecurityTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectPropertyBagEntryTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 7s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectPropertyBagEntryTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectListInstanceTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectListInstanceTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 28s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectFilesTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectFilesTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectFeaturesTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectFeaturesTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectCustomActionsTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectCustomActionsTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 1s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectFieldTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectFieldTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectContentTypeTests.CanProvisionObjects</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.ObjectContentTypeTests.CanCreateEntities</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.ObjectHandlers.TokenParserTests.ParseTests</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.Framework.Providers.BaseTemplateTests.GetBaseTemplateForCurrentSiteTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLFileSystemGetTemplatesTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLFileSystemGetTemplate1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLFileSystemGetTemplate2Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLAzureStorageGetTemplatesTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLAzureStorageGetTemplate1Test</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLAzureStorageGetTemplate2SecureTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLFileSystemConvertTemplatesFromV201503toV201505</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.ResolveSchemaFormatV201503</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.ResolveSchemaFormatV201505</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLResolveValidXInclude</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.Providers.XMLProvidersTests.XMLResolveInvalidXInclude</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanDeserializeXMLToDomainObject1</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeDomainObjectToXML1</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeDomainObjectToXMLStream1</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanDeserializeXMLToDomainObject2</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeDomainObjectToXML2</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetTemplateNameandVersion</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetPropertyBagEntries</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetOwners</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetAdministrators</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetMembers</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetVistors</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetFeatures</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanGetCustomActions</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeToJSon</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeToXml</td><td>Passed</td><td>0h 0m 34s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.ValidateFullProvisioningSchema5</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.ValidateSharePointProvisioningSchema6</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanDeserializeXMLToDomainObject5</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanDeserializeXMLToDomainObject6</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeDomainObjectToXML6</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeDomainObjectToXML5ByIdentifier</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeDomainObjectToXML5ByFileLink</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanSerializeDomainObjectWithJsonFormatter</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.CanHandleDomainObjectWithJsonFormatter</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.AreTemplatesEqual</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Framework.ProvisioningTemplates.DomainModelTests.GetRemoteTemplateTest</td><td>Passed</td><td>0h 0m 33s</td></tr>
<tr><td>Utilities.Tests.JsonUtilityTests.SerializeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Utilities.Tests.JsonUtilityTests.DeserializeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Utilities.Tests.JsonUtilityTests.DeserializeListTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Utilities.Tests.JsonUtilityTests.DeserializeListIsNotFixedSizeTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Utilities.Tests.JsonUtilityTests.DeserializeListNoDataStillWorksTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Utilities.EncryptionUtilityTests.ToSecureStringTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Utilities.EncryptionUtilityTests.ToInSecureStringTest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Utilities.EncryptionUtilityTests.EncryptStringWithDPAPITest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.Utilities.EncryptionUtilityTests.DecryptStringWithDPAPITest</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.UrlUtilityTests.ContainsInvalidCharsReturnsFalseForValidString</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.UrlUtilityTests.ContainsInvalidUrlCharsReturnsTrueForInvalidString</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.UrlUtilityTests.StripInvalidUrlCharsReturnsStrippedString</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Tests.AppModelExtensions.UrlUtilityTests.ReplaceInvalidUrlCharsReturnsStrippedString</td><td>Passed</td><td>0h 0m 0s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.SetPropertyBagValueIntTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.SetPropertyBagValueStringTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.SetPropertyBagValueMultipleRunsTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.RemovePropertyBagValueTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.GetPropertyBagValueIntTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.GetPropertyBagValueStringTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.PropertyBagContainsKeyTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.GetIndexedPropertyBagKeysTest</td><td>Passed</td><td>0h 0m 4s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.AddIndexedPropertyBagKeyTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.RemoveIndexedPropertyBagKeyTest</td><td>Passed</td><td>0h 0m 3s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.GetProvisioningTemplateTest</td><td>Passed</td><td>0h 0m 37s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.GetAppInstancesTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Microsoft.SharePoint.Client.Tests.WebExtensionsTests.RemoveAppInstanceByTitleTest</td><td>Passed</td><td>0h 0m 2s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.CanAddLayoutToWikiPageTest</td><td>Passed</td><td>0h 0m 7s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.CanAddHtmlToWikiPageTest</td><td>Passed</td><td>0h 0m 5s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.ProveThatWeCanAddHtmlToPageAfterChangingLayoutTest</td><td>Passed</td><td>0h 0m 6s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.CanCreatePublishingPageTest</td><td>Passed</td><td>0h 0m 25s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.PublishingPageWithInvalidCharsIsCorrectlyCreatedTest</td><td>Passed</td><td>0h 0m 19s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.CanCreatePublishedPublishingPageWhenModerationIsEnabledTest</td><td>Passed</td><td>0h 0m 18s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.CanCreatePublishedPublishingPageWhenModerationIsDisabledTest</td><td>Passed</td><td>0h 0m 18s</td></tr>
<tr><td>Tests.AppModelExtensions.PageExtensionsTests.CreatedPublishingPagesSetsTitleCorrectlyTest</td><td>Passed</td><td>0h 0m 20s</td></tr>

</table>



