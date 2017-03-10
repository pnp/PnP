# SHAREPOINT BULK DOCUMENT UPLOADER #

### Summary ###
This sample shows how to bulk upload documents into a target site collection within SPO using the C# REST API.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.BulkDocumentUploader | Luke Bailey (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 3th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# OVERVIEW #
This sample code and documentation presents an automated way of uploading documents to a targetted site collection through the REST C# API as is a typical request during O365 migrations, for example uploading company policies to each users OneDrive or an index of their migrated content. Below sample shows how to upload one particular file per OneDrive, but this can be easily extended to upload multiple files to a single OneDrive

# SCENARIO 1: BULK UPLOADING DOCUMENTS TO ONEDRIVE #
The single input file to the utility is **OneDriveUploader.xml** which defines the profile property mappings and connection details for the entire utility. The two primary root elements for the file are ```<loggers>``` and ```<actions>```:

```XML
<?xml version="1.0" encoding="utf-8" ?>
<PropertyMapper xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Loggers>
    <!-- Configure verbose text logger-->
    <Log xsi:type="TextFileLogger">
      <TraceTypes>
        <Trace>Verbose</Trace>
      </TraceTypes>     
      <LogFileLocation>C:\Contoso.OneDriveUploader.log</LogFileLocation>
    </Log>
    <!-- Configure CSV output logger-->
    <Log xsi:type="CsvLogger">
      <TraceTypes>
        <Trace>Verbose</Trace>
      </TraceTypes>
      <LogFileLocation>C:\Contoso.OneDriveUploader.csv</LogFileLocation>
    </Log>
  </Loggers>
  <Actions>
    <Action xsi:type="OneDriveMapper">
      <!-- CSV mapping location for OneDrive URLs and file uploads-->
      <UserMappingCSVFile>Input\SharePointSites.csv</UserMappingCSVFile>
      <!-- Directory location for upload files to OneDrive-->
      <DirectoryLocation>Input\OneDriveFiles</DirectoryLocation>
      <!-- SPO user account detials to perform operations-->
      <UserName>anadminuser@uksharepoint.onmicrosoft.com</UserName>
      <Password>*********</Password>
      <!-- Column index of sites and files in CSV mappings -->
      <SiteIndex>0</SiteIndex>
      <FileIndex>1</FileIndex>
      <!-- Select either Upload or Delete for this action-->
      <DocumentAction>Upload</DocumentAction>
      <!-- Filename to appear in user OneDrives-->
      <FileUploadName>_ COMPANY POLICY DOCUMENT _.xlsx</FileUploadName>
    </Action>
  </Actions>
</PropertyMapper>
```

## BEFORE YOU RUN THE SAMPLE FROM VS ##
If you want to test this sample from Visual Studio it's needed that:
* You update the above OneDriveUploader.xml file to contain:
    * Your username and password
    * UserMappingCSVFile pointing to the full locatoin of the SharePointSites.csv file (e.g. C:\Git\PnP\Samples\Core.BulkDocumentUploader\Input\SharePointSites.csv)
    * DirectoryLocation pointing to the fully qualified directory (e.g. C:\GitHub\BertPnP\Samples\Core.BulkDocumentUploader\Input\OneDriveFiles)
* Adjust the SharePointSites.CSV file to have the correct OneDrive urls for the users that will need to get the one file
* Adjust the properties of the Core.BulkDocumentUploader project by adding the fully qualified path the OneDriveUplaoder.xml file as command line argument in the Debug settings



## LOG ELEMENTS ##
Parent Element | Child Element | Value and Description
-------------- | ------------- | ---------------------
TraceTypes | Trace | Ability to define level of logging for the current logging object. **Values:** Error, Verbose
LogFileLocation | n/a | TextFileLogger and CsvLogger specific attribute that governs where the output file will be written to 

## ACTION ELEMENTS ##
Parent Element | Child Element | Value and Description
-------------- | ------------- | ---------------------
UserMappingCSVFile | n/a | Defines the location of the CSV containing the target sites and the filename to be uploaded
DirectoryLocation | n/a | The folder containing the actual files to be uploaded that map to the CSV files entries. 
UserName | n/a | A dedicated O365 service account created to run the utility that is a global administrator
Password | n/a | Corresponding O365 user account password
SiteIndex | n/a | The column index of the target sites in the CSV file
FileIndex | n/a | The column index of the target filenames in the CSV file
DocumentAction | n/a | Whether the files are to be uploaded or deleted from the target site collection
FileUploadName | n/a | Attribute to allow the files to be renamed to something approraite to the content such as company policy.

## Running the application ##
Navigate to the directory of the **Contoso.BulkDocumentUploader.exe**, the executable requires a single input parameter which is the ContosoOneDriveUploader.xml


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.BulkDocumentUploader" />