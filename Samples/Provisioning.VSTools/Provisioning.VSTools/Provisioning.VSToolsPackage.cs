using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Perficient.Provisioning.VSTools.Helpers;
using Perficient.Provisioning.VSTools.Models;

namespace Perficient.Provisioning.VSTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidProvisioning_VSToolsPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class Provisioning_VSToolsPackage : Package
    {
        private const string PnPTemplateToolsConfig = "ProvisioningTemplateTools.config";

        private const string ProjectCommandEnablePnPToolsText = "Enable PnP Provisioning Tools";
        private const string ProjectCommandDisablePnPToolsText = "Disable PnP Provisioning Tools";

        private OleMenuCommand _projectItemDeployCommand;
        private OleMenuCommand _projectFolderDeployCommand;

        public Provisioning_VSToolsPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        #region Package Members

        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            DTE2 dte = (DTE2)GetService(typeof(DTE));

            OutputWindow outputWindow = (OutputWindow)dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Object;
            outputWindowPane = outputWindow.OutputWindowPanes.Add("PnP Deployment Tools");

            try
            {

                OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
                if (null != mcs)
                {
                    // project-level commands
                    CommandID toolsToggleCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningProjectCmdSet, (int)PkgCmdIDList.cmdidPnPToolsToggle);
                    var toolsToggleMenuItem = new OleMenuCommand(MenuItemCallback_ToggleTools, toolsToggleCommandID);
                    toolsToggleMenuItem.BeforeQueryStatus += BeforeQueryStatus_ToggleToolsMenu;

                    mcs.AddCommand(toolsToggleMenuItem);
                }

                AttachFileEventListeners();
                AddProjectItemCommand();
                AddProjectFolderCommand();

            }
            catch (Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error in Initialize : {0} , {1}\n", ex.Message, ex.StackTrace));
            }

        }
        #endregion

        private void AddProjectItemCommand()
        {
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID menuCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningItemCmdSet, (int)PkgCmdIDList.cmdidDeployItemWithPNP);
                _projectItemDeployCommand = new OleMenuCommand(MenuItemCallback_DeploySingleitem, menuCommandID);
                _projectItemDeployCommand.BeforeQueryStatus += BeforeQueryStatus_DeploySingleItem;
                mcs.AddCommand(_projectItemDeployCommand);
            }
        }

        private void AddProjectFolderCommand()
        {
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID menuCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningFolderCmdSet, (int)PkgCmdIDList.cmdidDeployFolderWithPNP);
                _projectFolderDeployCommand = new OleMenuCommand(MenuItemCallback_DeployFolder, menuCommandID);
                _projectFolderDeployCommand.BeforeQueryStatus += BeforeQueryStatus_DeployFolder;
                mcs.AddCommand(_projectFolderDeployCommand);
            }
        }

        private void AttachFileEventListeners()
        {
            try
            {
                DTE2 dte = (DTE2)GetService(typeof(DTE));

                //IVsHierarchyEvents
                // ((Events2)dte.Events).SolutionEvents.

                projItemsEvents = (EnvDTE.ProjectItemsEvents)
                dte.Events.GetObject("CSharpProjectItemsEvents");
                projItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(ProjItemAdded);
                projItemsEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(ProjItemRemoved);
                projItemsEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(ProjItemRenamed);

                docEvents = (EnvDTE.DocumentEvents)
                dte.Events.DocumentEvents;
                docEvents.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(DocEventsDocSaved);
            }
            catch (System.Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error registering file event handlers : {0} , {1}\n", ex.Message, ex.StackTrace));
            }
        }


        private ProvisioningTemplateLocationInfo GetParentProvisioningTemplateInformation(string projectItemFullPath, string projectFolderPath, ProvisioningTemplateToolsConfiguration config)
        {
            if (config == null || config.Templates == null)
            {
                return null;
            }

            foreach (var template in config.Templates)
            {
                var pnpResourcesFolderPath = Path.Combine(projectFolderPath, template.ResourcesFolder);
                var templateFilePath = Path.Combine(projectFolderPath, template.Path);

                if (ProjectHelpers.IsItemInsideFolder(projectItemFullPath, pnpResourcesFolderPath))
                {
                    return new ProvisioningTemplateLocationInfo()
                    {
                        ResourcesPath = pnpResourcesFolderPath,
                        TemplateFolderPath = Path.GetDirectoryName(templateFilePath),
                        TemplateFileName = Path.GetFileName(templateFilePath)
                    };
                }
            }
            return null;
        }

        private ProvisioningTemplateLocationInfo GetParentProvisioningTemplateInformation(ProjectItem projectItem, ProvisioningTemplateToolsConfiguration config)
        {
            if (config == null || config.Templates == null)
            {
                return null;
            }
            var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
            var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);


            foreach (var template in config.Templates)
            {
                var pnpResourcesFolderPath = Path.Combine(projectFolderPath, template.ResourcesFolder);
                var templateFilePath = Path.Combine(projectFolderPath, template.Path);

                if (ProjectHelpers.IsItemInsideFolder(projectItemFullPath, pnpResourcesFolderPath))
                {
                    return new ProvisioningTemplateLocationInfo()
                    {
                        ResourcesPath = pnpResourcesFolderPath,
                        TemplateFolderPath = Path.GetDirectoryName(templateFilePath),
                        TemplateFileName = Path.GetFileName(templateFilePath)
                    };
                }
            }
            return null;
        }

        private ProvisioningTemplateLocationInfo GetCurrentProvisioningTemplateInformation(ProjectItem projectItem, ProvisioningTemplateToolsConfiguration config)
        {
            if (config == null || config.Templates == null)
            {
                return null;
            }
            var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
            var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);


            foreach (var template in config.Templates)
            {
                var pnpResourcesFolderPath = Path.Combine(projectFolderPath, template.ResourcesFolder);
                var templateFilePath = Path.Combine(projectFolderPath, template.Path);

                if (projectItemFullPath.Equals(templateFilePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new ProvisioningTemplateLocationInfo()
                    {
                        ResourcesPath = pnpResourcesFolderPath,
                        TemplateFolderPath = Path.GetDirectoryName(templateFilePath),
                        TemplateFileName = Path.GetFileName(templateFilePath)
                    };
                }
            }
            return null;
        }


        private XMLFileSystemTemplateProvider InitializeProvisioningTemplateProvider(ProvisioningTemplateLocationInfo templateInfo)
        {
            XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(templateInfo.TemplateFolderPath, "");
            return provider;
        }

        private ProvisioningTemplate InitializeProvisioningTemplate(XMLFileSystemTemplateProvider provider,
            ProvisioningTemplateLocationInfo templateInfo)
        {
            ProvisioningTemplate template = null;

            try
            {
                template = provider.GetTemplate(templateInfo.TemplateFileName);
                template.Connector = new FileSystemConnector(templateInfo.ResourcesPath, "");
            }
            catch (Exception ex)
            {
                ShowMessage("Error parsing Provisioning Template",
                    string.Format("Could not load template: {0}, {1}", ex.Message, ex.StackTrace));
            }

            return template;
        }

        private void ShowMessage(string title, string message)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       title,
                       message,
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }


        private void ProjItemAdded(EnvDTE.ProjectItem projectItem)
        {
            try
            {
                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);

                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                if (config == null || !config.ToolsEnabled)
                {
                    return;
                }

                if (projectItem.Kind != EnvDTE.Constants.vsProjectItemKindPhysicalFile)
                {
                    // we handle only files
                    // when folder with files is added, event is raised separately for all files as well
                    return;
                }
                var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
                outputWindowPane.OutputString(string.Format("Item added : {0} \n", projectItemFullPath));

                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItem, config);
                if (pnpTemplateInfo != null)
                {
                    // Item is PnP resource. 
                    var src = ProjectHelpers.MakeRelativePath(projectItemFullPath, pnpTemplateInfo.ResourcesPath);
                    var targetFolder = String.Join("/", Path.GetDirectoryName(src).Split('\\'));

                    XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                    ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);

                    if (template != null)
                    {

                        template.Files.Add(new OfficeDevPnP.Core.Framework.Provisioning.Model.File()
                        {
                            Src = src,
                            Folder = targetFolder,
                            Overwrite = true,
                            Security = null
                        });

                        provider.Save(template);
                    }

                }
            }
            catch (Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error in item added events: {0}, {1} \n", ex.Message, ex.StackTrace));
            }

        }

        private void ProjItemRemoved(EnvDTE.ProjectItem projectItem)
        {
            try
            {
                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                if (config == null || !config.ToolsEnabled)
                {
                    return;
                }

                var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
                outputWindowPane.OutputString(string.Format("Item removed : {0} \n", projectItemFullPath));

                if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                {
                    var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItem, config);
                    if (pnpTemplateInfo != null)
                    {
                        var src = ProjectHelpers.MakeRelativePath(projectItemFullPath, pnpTemplateInfo.ResourcesPath);

                        XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                        ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);

                        if (template != null)
                        {
                            // Remove all files where src path starts with given folder path
                            template.Files.RemoveAll(f => f.Src.StartsWith(src, StringComparison.InvariantCultureIgnoreCase));

                            provider.Save(template);
                        }

                    }
                }
                else if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
                {
                    var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItem, config);
                    if (pnpTemplateInfo != null)
                    {
                        var src = ProjectHelpers.MakeRelativePath(projectItemFullPath, pnpTemplateInfo.ResourcesPath);

                        // PNP-powered code
                        XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                        ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);

                        if (template != null)
                        {

                            template.Files.RemoveAll(f => f.Src.Equals(src, StringComparison.InvariantCultureIgnoreCase));

                            provider.Save(template);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error in item removed event: {0}, {1} \n", ex.Message, ex.StackTrace));
            }
        }

        private void ProjItemRenamed(EnvDTE.ProjectItem projectItem, string oldName)
        {
            try
            {
                var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);

                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                if (config == null || !config.ToolsEnabled)
                {
                    return;
                }

                outputWindowPane.OutputString(string.Format("Item renamed : {0}, old name: {1} \n", projectItemFullPath, oldName));

                if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                {
                    var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItem, config);
                    if (pnpTemplateInfo != null)
                    {
                        var src = ProjectHelpers.MakeRelativePath(projectItemFullPath, pnpTemplateInfo.ResourcesPath);
                        var oldSrc = Path.Combine(src.Substring(0, src.TrimEnd('\\').LastIndexOf('\\')), oldName) + "\\";

                        XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                        ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);

                        if (template != null)
                        {
                            // Remove all files where src path starts with given folder path
                            var filesToRename = template.Files.Where(f => f.Src.StartsWith(oldSrc, StringComparison.InvariantCultureIgnoreCase));

                            foreach (var file in filesToRename)
                            {
                                file.Src = Regex.Replace(file.Src, Regex.Escape(oldSrc), src, RegexOptions.IgnoreCase);
                            }

                            provider.Save(template);
                        }

                    }
                }
                else if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
                {

                    var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItem, config);
                    if (pnpTemplateInfo != null)
                    {
                        // Item is PnP resource. 
                        var src = ProjectHelpers.MakeRelativePath(projectItemFullPath, pnpTemplateInfo.ResourcesPath);
                        var oldSrc = Path.Combine(Path.GetDirectoryName(src), oldName);

                        //PNP-powered code
                        XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                        ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);

                        if (template != null)
                        {
                            var file =
                                template.Files.Where(
                                    f => f.Src.Equals(oldSrc, StringComparison.InvariantCultureIgnoreCase))
                                    .FirstOrDefault();

                            if (file != null)
                            {
                                file.Src = src;
                                provider.Save(template);

                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error in item renamed event: {0}, {1} \n", ex.Message, ex.StackTrace));
            }


        }

        private void DocEventsDocSaved(EnvDTE.Document Doc)
        {
            outputWindowPane.OutputString(string.Format("Document Saved : {0} \n", Doc.Name));
        }

        private EnvDTE.ProjectItemsEvents projItemsEvents;
        private EnvDTE.DocumentEvents docEvents;
        private OutputWindowPane outputWindowPane;

        private void MenuItemCallback_DeployFolder(object sender, EventArgs e)
        {
            try
            {
                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;

                if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return;
                // Get the file path
                string itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);



                string projectFilePath = null;
                ((IVsProject)hierarchy).GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFilePath);

                var projectFolderPath = Path.GetDirectoryName(projectFilePath);

                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                if (config == null || !config.ToolsEnabled)
                {
                    return;
                }

                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(itemFullPath, projectFolderPath, config);
                if (pnpTemplateInfo != null)
                {
                    var src = ProjectHelpers.MakeRelativePath(itemFullPath, pnpTemplateInfo.ResourcesPath);

                    XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                    ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);

                    if (template != null)
                    {
                        var files =
                            template.Files.Where(
                                f => f.Src.StartsWith(src, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();

                        if (files.Count > 0)
                        {
                            var filesUnderFolderTemplate = new ProvisioningTemplate(template.Connector);
                            filesUnderFolderTemplate.Files.AddRange(files);
                            outputWindowPane.OutputString(string.Format("\nStarting deployment of files under folder {0} from template template {1} ....\n", src, pnpTemplateInfo.TemplateFileName));
                            DeployProvisioningTemplate(filesUnderFolderTemplate, config);
                            outputWindowPane.OutputString(string.Format("Finished deployment of files under folde {0} from template template {1} ....\n\n", src, pnpTemplateInfo.TemplateFileName));
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error in deploying files to SharePoint: {0}, {1} \n", ex.Message, ex.StackTrace));
            }

        }


        private void MenuItemCallback_DeploySingleitem(object sender, EventArgs e)
        {
            try
            {
                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;

                if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return;
                // Get the file path
                string itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);

                var transformFileInfo = new FileInfo(itemFullPath);

                // then check if the file is named 'web.config'
                //TODO: Addd as config value

                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                var projectItem = dte.Solution.FindProjectItem(itemFullPath);

                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);

                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                if (config == null || !config.ToolsEnabled)
                {
                    return;
                }


                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItem, config);
                if (pnpTemplateInfo != null)
                {
                    var src = ProjectHelpers.MakeRelativePath(itemFullPath, pnpTemplateInfo.ResourcesPath);

                    XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                    ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);

                    if (template != null)
                    {
                        var file =
                            template.Files.Where(
                                f => f.Src.Equals(src, StringComparison.InvariantCultureIgnoreCase))
                                .FirstOrDefault();

                        if (file != null)
                        {
                            var singleFileTemplate = new ProvisioningTemplate(template.Connector);
                            singleFileTemplate.Files.Add(file);
                            outputWindowPane.OutputString(string.Format("\nStarting deployment of file {0} from template template {1} ....\n", src, pnpTemplateInfo.TemplateFileName));
                            DeployProvisioningTemplate(singleFileTemplate, config);
                            outputWindowPane.OutputString(string.Format("Finished deployment of file {0} from template template {1} ....\n\n", src, pnpTemplateInfo.TemplateFileName));
                        }

                    }

                }
                else
                {
                    pnpTemplateInfo = GetCurrentProvisioningTemplateInformation(projectItem, config);
                    if (pnpTemplateInfo != null)
                    {
                        XMLFileSystemTemplateProvider provider = InitializeProvisioningTemplateProvider(pnpTemplateInfo);
                        ProvisioningTemplate template = InitializeProvisioningTemplate(provider, pnpTemplateInfo);
                        outputWindowPane.OutputString(string.Format("\nStarting deployment of template {0} ....\n", pnpTemplateInfo.TemplateFileName));
                        DeployProvisioningTemplate(template, config);
                        outputWindowPane.OutputString(string.Format("Finished deployment of template {0} ....\n\n", pnpTemplateInfo.TemplateFileName));

                    }
                }

            }
            catch (Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error in deploying file to SharePoint: {0}, {1} \n", ex.Message, ex.StackTrace));
            }

        }

        private void DeployProvisioningTemplate(ProvisioningTemplate template,
            ProvisioningTemplateToolsConfiguration config)
        {
            var siteUrl = config.Deployment.TargetSite;
            var login = config.Deployment.Authentication.Office365.Username;

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                SecureString passWord = new SecureString();

                #region password

                foreach (char c in config.Deployment.Authentication.Office365.Password.ToCharArray())
                    passWord.AppendChar(c);

                #endregion

                clientContext.Credentials = new SharePointOnlineCredentials(login, passWord);

                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();
                ProvisioningTemplateApplyingInformation ptai = new ProvisioningTemplateApplyingInformation();
                ptai.ProgressDelegate = delegate(string message, int step, int total)
                {
                    outputWindowPane.OutputString(string.Format("Deploying {0}, Step {1}/{2} \n", message, step, total));
                };

                clientContext.Web.ApplyProvisioningTemplate(template, ptai);
            }

        }


        //Context menu check for specific file name
        void BeforeQueryStatus_DeploySingleItem(object sender, EventArgs e)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {

                // start by assuming that the menu will not be shown
                menuCommand.Visible = false;
                menuCommand.Enabled = false;

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;

                if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return;
                // Get the file path
                string itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);

                var transformFileInfo = new FileInfo(itemFullPath);

                // then check if the file is named 'web.config'
                //TODO: Addd as config value

                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                var projectItem = dte.Solution.FindProjectItem(itemFullPath);

                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);

                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                if (config == null || !config.ToolsEnabled)
                {
                    return;
                }


                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItem, config);
                if (pnpTemplateInfo != null)
                {
                    menuCommand.Visible = true;
                    menuCommand.Enabled = true;
                }
                else
                {
                    pnpTemplateInfo = GetCurrentProvisioningTemplateInformation(projectItem, config);
                    if (pnpTemplateInfo != null)
                    {
                        menuCommand.Visible = true;
                        menuCommand.Enabled = true;
                    }
                }

            }
        }

        void BeforeQueryStatus_DeployFolder(object sender, EventArgs e)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {

                // start by assuming that the menu will not be shown
                menuCommand.Visible = false;
                menuCommand.Enabled = false;

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;

                if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return;
                // Get the file path
                string itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);

                string projectFilePath = null;
                ((IVsProject)hierarchy).GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFilePath);

                var projectFolderPath = Path.GetDirectoryName(projectFilePath);

                var transformFileInfo = new FileInfo(itemFullPath);

                // then check if the file is named 'web.config'
                //TODO: Addd as config value


                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                if (config == null || !config.ToolsEnabled)
                {
                    return;
                }

                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(itemFullPath, projectFolderPath, config);
                if (pnpTemplateInfo != null)
                {
                    menuCommand.Visible = true;
                    menuCommand.Enabled = true;
                }

            }
        }

        public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;
            int hr = VSConstants.S_OK;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            IVsMultiItemSelect multiItemSelect = null;
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null) return false;

                // there is a hierarchy root node selected, thus it is not a single item inside a project

                if (itemid == VSConstants.VSITEMID_ROOT) return false;

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null) return false;

                Guid guidProjectID = Guid.Empty;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID)))
                {
                    return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }


        private void BeforeQueryStatus_ToggleToolsMenu(object sender, EventArgs eventArgs)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {

                menuCommand.Text = ProjectCommandEnablePnPToolsText;
                try
                {
                    uint projectItemId;
                    var hierarchy = ProjectHelpers.GetCurrentHierarchy(out projectItemId);

                    EnvDTE.Project project = ProjectHelpers.GetProject(hierarchy, projectItemId);

                    var projectFolderPath = Path.GetDirectoryName(project.FullName);

                    var configFilePath = Path.Combine(projectFolderPath, PnPTemplateToolsConfig);

                    // project.ProjectItems[]
                    if (System.IO.File.Exists(configFilePath))
                    {
                        var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                        var configItem = dte.Solution.FindProjectItem(configFilePath);

                        if (configItem != null)
                        {
                            ProvisioningTemplateToolsConfiguration config = XmlHelpers.DeserializeObject(configFilePath);

                            if (config.ToolsEnabled)
                            {
                                menuCommand.Text = ProjectCommandDisablePnPToolsText;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    outputWindowPane.OutputString(string.Format("Error in determining if PnP tools are active: {0}, {1} \n", ex.Message, ex.StackTrace));
                }
            }
        }

        private ProvisioningTemplateToolsConfiguration GetProvisioningTemplateToolsConfiguration(string projectFolderPath)
        {
            ProvisioningTemplateToolsConfiguration config = null;

            try
            {
                var configFilePath = Path.Combine(projectFolderPath, PnPTemplateToolsConfig);
                if (System.IO.File.Exists(configFilePath))
                {
                    var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                    var configItem = dte.Solution.FindProjectItem(configFilePath);

                    if (configItem != null)
                    {
                        config = XmlHelpers.DeserializeObject(configFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                outputWindowPane.OutputString(string.Format("Error in GetProvisioningTemplateToolsConfiguration: {0}, {1} \n", ex.Message, ex.StackTrace));
            }
            return config;
        }

        private void MenuItemCallback_ToggleTools(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                uint projectItemId;
                var hierarchy = ProjectHelpers.GetCurrentHierarchy(out projectItemId);

                EnvDTE.Project project = ProjectHelpers.GetProject(hierarchy, projectItemId);
                var projectFolderPath = Path.GetDirectoryName(project.FullName);

                var configFilePath = Path.Combine(projectFolderPath, PnPTemplateToolsConfig);
                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);


                if (menuCommand.Text == ProjectCommandEnablePnPToolsText)
                {
                    if (config == null)
                    {
                        config = new ProvisioningTemplateToolsConfiguration();
                        config.Templates.Add(new Template()
                        {
                            Path = "Path to your template fix.xml",
                            ResourcesFolder = "Path to your Resources folder\\"
                        });
                        config.Deployment.TargetSite = " ";
                        config.Deployment.Authentication = new Authentication()
                        {
                            Type = "Office365",
                            Office365 = new Office365()
                            {
                                Username = "your.username@tenant.sharepoint.com",
                                Password = " "
                            }
                        };
                    }
                    config.ToolsEnabled = true;
                    XmlHelpers.SerializeObject(config, configFilePath);

                    var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                    var configItem = dte.Solution.FindProjectItem(configFilePath);

                    if (configItem == null)
                    {
                        project.ProjectItems.AddFromFile(configFilePath);
                    }
                }
                else
                {

                    if (config != null)
                    {
                        config.ToolsEnabled = false;
                        XmlHelpers.SerializeObject(config, configFilePath);
                    }
                }
            }

        }


    }
}
