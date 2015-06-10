using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.WorkflowTemplate
{
    class Program
    {
        private static string solutionPath = null;
        private static Guid workflowFeature = new Guid();
        private static Guid workflowUserSolutionId = new Guid();

        static void Main(string[] args)
        {
                solutionPath = ConfigurationManager.AppSettings["WorkflowTemplatePath"];
                workflowFeature = new Guid(ConfigurationManager.AppSettings["WorkflowFeatureId"]);
                workflowUserSolutionId = new Guid(ConfigurationManager.AppSettings["WorkflowSolutionId"]);
                Uri webUrl = new Uri(ConfigurationManager.AppSettings["SharePointUrl"]);
                SharePointAuthenticationInfo authenticationInfo = new SharePointAuthenticationInfo()
                {
                    userName = ConfigurationManager.AppSettings["UserName"],
                    password = ConfigurationManager.AppSettings["Password"],
                    mode = (SharePointMode)Enum.Parse(typeof(SharePointMode), ConfigurationManager.AppSettings["SharePointMode"])
                };
                using (ClientContext context = SharePointContextProvider.Current.CreateSharePointContext(webUrl, authenticationInfo))
                {
                    ProvisionWorkflow(context);
                    RemoveWorkflow(context);
                }
        }

        private static void RemoveWorkflow(ClientContext context)
        {
            //Construct object with workflow template info
            WorkflowTemplateInfo solutionInfo = new WorkflowTemplateInfo();
            //Package Guid is mandatory
            solutionInfo.PackageGuid = workflowUserSolutionId;
            solutionInfo.PackageName = Path.GetFileNameWithoutExtension(solutionPath);
            //Init workflow template deployer
            using (WorkflowTemplateDeployer workflowDeployer = new WorkflowTemplateDeployer(context))
            {
                //Deactivate workflow template
                workflowDeployer.DeactivateWorkflowSolution(solutionInfo);
                //Remove workflow template files
                workflowDeployer.RemoveWorkflowSolution(Path.GetFileName(solutionPath));
            }
        }

        private static void ProvisionWorkflow(ClientContext context)
        {

            //Construct object with workflow template info
            WorkflowTemplateInfo solutionInfo = new WorkflowTemplateInfo();
            solutionInfo.PackageFilePath = solutionPath;
            //PackageName is mandatory
            solutionInfo.PackageName = Path.GetFileNameWithoutExtension(solutionPath);
            //Guid is automatically predefined in template file (wsp)
            solutionInfo.PackageGuid = workflowUserSolutionId;
            //Workflow feature Id is need to activate workflow in the web
            solutionInfo.FeatureId = workflowFeature;
            //Init workflow template deployer
            using (WorkflowTemplateDeployer workflowDeployer = new WorkflowTemplateDeployer(context))
            {
                //Provisiong workflow resources
                workflowDeployer.DeployWorkflowSolution(solutionPath);
                //Activates workflow template
                workflowDeployer.ActivateWorkflowSolution(solutionInfo);
            }
        }
    }
}
