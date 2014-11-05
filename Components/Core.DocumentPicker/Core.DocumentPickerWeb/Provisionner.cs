using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Core.DocumentPickerWeb
{
    public class Provisionner
    {
        public const string DocLibrary1Name = "DocumentPickerDocLib";
        public const string DocLibrary2Name = "DocumentPickerDocLibExtra";

        public Guid ProvisionnedList1Id { get; set; }
        public Guid ProvisionnedList2Id { get; set; }

        public  void ProvisionData(HttpContext context)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(context);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (!IsContentProvisionned(clientContext))
                {
                    //get document path
                    string assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    assemblyPath = assemblyPath.Substring(6);

                    //create library 1
                    var docLibrary1 = CreateDocumentLibrary(clientContext, DocLibrary1Name);
                    ProvisionnedList1Id = docLibrary1.Id;

                    //create folders for library 1
                    var folderImportantDocuments = CreateFolder(clientContext, docLibrary1.RootFolder, "Important documents");
                    var folderFlagged = CreateFolder(clientContext, folderImportantDocuments, "Flagged");
                    var folderOther = CreateFolder(clientContext, docLibrary1.RootFolder, "Other");

                    //add files for library 1
                    UploadFile(clientContext, docLibrary1.RootFolder, assemblyPath, "sample txt.txt");
                    UploadFile(clientContext, docLibrary1.RootFolder, assemblyPath, "Sample word doc 1.docx");
                    UploadFile(clientContext, folderImportantDocuments, assemblyPath, "Sample word doc 2.docx");
                    UploadFile(clientContext, folderImportantDocuments, assemblyPath, "Sample ppt1.pptx");
                    UploadFile(clientContext, folderFlagged, assemblyPath, "sample excel 1.xlsx");
                    UploadFile(clientContext, folderOther, assemblyPath, "Sample ppt2.pptx");

                    //create library 2
                    var docLibrary2 = CreateDocumentLibrary(clientContext, DocLibrary2Name);
                    ProvisionnedList2Id = docLibrary2.Id;

                    //create folders for library 2
                    var folderMixedFiles = CreateFolder(clientContext, docLibrary2.RootFolder, "Mixed documents");
                    var folderOldDocuments = CreateFolder(clientContext, docLibrary2.RootFolder, "Old documents");

                    //add files for library 2
                    UploadFile(clientContext, folderMixedFiles, assemblyPath, "sample excel 1.xlsx");
                    UploadFile(clientContext, folderMixedFiles, assemblyPath, "Sample word doc 1.docx");
                    UploadFile(clientContext, folderOldDocuments, assemblyPath, "sample excel 2.xlsx");
                }
            }
        }

        private bool IsContentProvisionned(ClientContext clientContext)
        {
            bool listFound = true;
            try
            {
                var foundList = clientContext.Web.Lists.GetByTitle(DocLibrary1Name);
                clientContext.Load(foundList);
                clientContext.ExecuteQuery();

                ProvisionnedList1Id = foundList.Id;
            }
            catch (Exception)
            {
                listFound = false;
            }

            try
            {
                var foundList = clientContext.Web.Lists.GetByTitle(DocLibrary2Name);
                clientContext.Load(foundList);
                clientContext.ExecuteQuery();

                ProvisionnedList2Id = foundList.Id;
            }
            catch (Exception)
            {
                listFound = false;
            }

            return listFound;
        }

        private List CreateDocumentLibrary(ClientContext clientContext, string name)
        {
            //create list
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = name;
            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            List list = clientContext.Web.Lists.Add(creationInfo);
            list.Description = name;
            list.Update();
            clientContext.Load(list);
            clientContext.ExecuteQuery();
           
            return list;
        }

        private Folder CreateFolder(ClientContext clientContext, Folder parent,  string name)
        {
            var newFolder = parent.Folders.Add(name);
            clientContext.ExecuteQuery();
            return newFolder;
        }

        private void UploadFile(ClientContext clientContext,Folder parent,string assemblyPath, string fileName)
        {
            FileCreationInformation fci = new FileCreationInformation();
            fci.Content = System.IO.File.ReadAllBytes(Path.Combine(Path.Combine(assemblyPath, "TestDocuments"), fileName));
            fci.Url = fileName;
            fci.Overwrite = true;
            Microsoft.SharePoint.Client.File fileToUpload = parent.Files.Add(fci);
            clientContext.Load(fileToUpload);
            clientContext.ExecuteQuery();
        }

        public string GetSampleDocumentUrl1(SharePointContext spContext)
        {
            return string.Format("{0}{1}/sample%20txt.txt", spContext.SPHostUrl, Provisionner.DocLibrary1Name);
        }

        public string GetSampleDocumentPath1()
        {
            return string.Format("{0}/sample txt.txt", Provisionner.DocLibrary1Name);
        }

        public string GetSampleDocumentUrl2(SharePointContext spContext)
        {
            return string.Format("{0}{1}/Sample%20word%20doc%201.docx", spContext.SPHostUrl, Provisionner.DocLibrary1Name);
        }

        public string GetSampleDocumentPath2()
        {
            return string.Format("{0}/Sample word doc 1.docx", Provisionner.DocLibrary1Name);
        }
    }
}