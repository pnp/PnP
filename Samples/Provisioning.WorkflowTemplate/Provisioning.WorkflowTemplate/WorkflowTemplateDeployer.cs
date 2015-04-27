using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.WorkflowTemplate
{
    /// <summary>
    /// Object encapsulates logic for automating workflow template provisioning
    /// </summary>
    public class WorkflowTemplateDeployer : IDisposable
    {
        /// <summary>
        /// Sharepoint client context
        /// </summary>
        private ClientContext clientContext = null;
        /// <summary>
        /// Library for storing workflow template file (wsp)
        /// </summary>
        private List templateLibrary = null;
        /// <summary>
        /// Logger/tracer
        /// </summary>
        private ILogger logger = null;

        public WorkflowTemplateDeployer(ClientContext context)
        {
            this.clientContext = context;
            logger = new ConsoleLogger();
            templateLibrary = this.clientContext.Web.Lists.GetByTitle("Site Assets");
            this.clientContext.Load(templateLibrary);
            this.clientContext.Load(templateLibrary.RootFolder);
            this.clientContext.ExecuteQuery();
        }
        /// <summary>
        /// Deploy workflow template to the template library
        /// </summary>
        /// <param name="solutionPath">Path to the workflow template (wsp)</param>
        public void DeployWorkflowSolution(string solutionPath)
        {
            // get the file from the server path in the provider site
            using (var file = new FileStream(solutionPath, FileMode.Open))
            {
                // create the FileCreationInformation object and prepare
                // to upload it to the solution gallery
                var fileCI = new FileCreationInformation()
                {
                    ContentStream = file,
                    Url = Path.GetFileName(solutionPath),
                    Overwrite = true,
                };

                // upload the solution to the gallery
                var uploadedFile = templateLibrary.RootFolder.Files.Add(fileCI);
                clientContext.Load(uploadedFile);
                clientContext.ExecuteQuery();
            }
            logger.WriteMessage("Workflow solution " + Path.GetFileName(solutionPath) + " is deployed.");
        }
        /// <summary>
        /// Activate user solution based on workflow template information
        /// </summary>
        /// <param name="wfSolution">Encapsulates workflow solution information - package name is mandatory</param>
        public void ActivateWorkflowSolution(WorkflowTemplateInfo wfSolution)
        {

            // install the solution from the file url
            var filerelativeurl = templateLibrary.RootFolder.ServerRelativeUrl + "/" + Path.GetFileName(wfSolution.PackageFilePath);

            DesignPackageInfo packageInfo = new DesignPackageInfo()
            {
                PackageGuid = wfSolution.PackageGuid,
                PackageName = wfSolution.PackageName
            };
            DesignPackage.Install(clientContext, clientContext.Site, packageInfo, filerelativeurl);
            clientContext.ExecuteQuery();
            logger.WriteMessage("Workflow solution " + Path.GetFileName(wfSolution.PackageFilePath) + " is activated.");

            ActivateWorkflowFeature(wfSolution.FeatureId);

        }

        /// <summary>
        /// Activate web feature based on Id
        /// </summary>
        /// <param name="featureId"></param>
        private void ActivateWorkflowFeature(Guid featureId)
        {
            var features = clientContext.Web.Features;
            features.Add(featureId, true, FeatureDefinitionScope.Site);
            clientContext.ExecuteQuery();
        }
        /// <summary>
        /// Deactivate user solution based on workflow template information
        /// </summary>
        /// <param name="wfSolution">Encapsulates workflow solution information - package guid is mandatory</param>
        public void DeactivateWorkflowSolution(WorkflowTemplateInfo wfSolution)
        {
            if (wfSolution.PackageGuid.Equals(Guid.Empty))
            {
                throw new Exception("PackageGuid is not specified, please fill in.");
            }
            // uninstall the solution

            DesignPackageInfo packageInfo = new DesignPackageInfo()
            {
                PackageGuid = wfSolution.PackageGuid,
                PackageName = wfSolution.PackageName
            };

            DesignPackage.UnInstall(clientContext, clientContext.Site, packageInfo);

            clientContext.ExecuteQuery();

            logger.WriteMessage("Workflow solution " + wfSolution.PackageName + " is deactivated.");
        }
        /// <summary>
        /// Remove workflow template from template library
        /// </summary>
        /// <param name="templateFileName">SharePoint template filename</param>
        public void RemoveWorkflowSolution(string templateFileName)
        {
            // find the solution in the gallery and delete it

            var files = templateLibrary.RootFolder.Files;
            clientContext.Load(files,
                fs => fs.Where(f => f.Name == templateFileName));
            clientContext.ExecuteQuery();
                var file = files.FirstOrDefault();

                if (file == null)
                {
                    throw new InvalidOperationException("Solution does not exist");
                }

                file.DeleteObject();
                clientContext.ExecuteQuery();

            logger.WriteMessage("Workflow solution " + templateFileName + " is removed.");
        }

        public void Dispose()
        {
            //
            if (clientContext != null)
            {
                clientContext.Dispose();
            }
        }
    }
}
