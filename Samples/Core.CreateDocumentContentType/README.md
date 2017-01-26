# Creating Document Content Types #

### Summary ###
This sample shows how you can create document content types and add then associate a document template to the content type. Sample adds new content type called "Contoso Sample Document" as new content type to the site collection, which can be used in the document libraries. This content type has custom template associated to it when new document is created.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
Core.CreateDocumentContentType | Ashish Trivedi (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 28th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Creation of Content Types and Site Columns #
The following code shows associating a document template with a document content type

            ContentType ct = web.ContentTypes.GetById("0x0101009189AB5D3D2647B580F011DA2F356FB2");
            cc.Load(ct); cc.ExecuteQuery();
            string ctFolderServerRelativeURL = "_cts/" + ct.Name;
            Folder ctFolder = web.GetFolderByServerRelativeUrl(ctFolderServerRelativeURL);
            cc.Load(ctFolder);
            cc.ExecuteQuery();

            string path = @"C:\Data\Test Documents\Doc CT File.docx";
            string fileName = System.IO.Path.GetFileName(path);
            byte[] filecontent = System.IO.File.ReadAllBytes(path);

            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = filecontent;
                newFile.Url = ctFolderServerRelativeURL + "/" + fileName;

                Microsoft.SharePoint.Client.File uploadedFile = ctFolder.Files.Add(newFile);
                cc.Load(uploadedFile);
                cc.ExecuteQuery();
                //Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, ctFolderServerRelativeURL + "/" + fileName, fs, true);

                
                cc.Load(ct); cc.ExecuteQuery();
                ct.DocumentTemplate = fileName;
                ct.Update(false);
                cc.Load(ct); cc.ExecuteQuery();
                Console.WriteLine("Content type updates");
            }

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.CreateDocumentContentType" />