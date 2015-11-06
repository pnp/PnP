using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Perficient.Provisioning.VSTools.Helpers
{
    public static class ProjectHelpers
    {
        public static string GetFullPath(ProjectItem projectItem)
        {
            return Convert.ToString(projectItem.Properties.Item("FullPath").Value);
        }

        public static bool IsItemInsideFolder(string itemPath, string folderPath)
        {
            return itemPath.StartsWith(folderPath, true, CultureInfo.InvariantCulture);
        }

        public static string MakeRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }


        public static ProjectItem GetProjectItem(this IVsHierarchy hierarchy, uint ItemID)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            Object prjItemObject = null;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(
                ItemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjItemObject));

            return prjItemObject as ProjectItem;
        }

        public static EnvDTE.Project GetProject(this IVsHierarchy hierarchy, uint ItemID)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            Object prjItemObject = null;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(
                ItemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjItemObject));

            return prjItemObject as EnvDTE.Project;
        }

        public static IVsHierarchy GetCurrentHierarchy(out uint projectItemId)
        {
            IntPtr hierarchyPtr, selectionContainerPtr;

            IVsMultiItemSelect mis;

            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

            return Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
        }


        public static EnvDTE80.DTE2 GetDTE2()
        {
            return Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
        }
        public static string GetSourceFilePath()
        {
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            UIHierarchy uih = _applicationObject.ToolWindows.SolutionExplorer;
            Array selectedItems = (Array)uih.SelectedItems;
            if (null != selectedItems)
            {
                foreach (UIHierarchyItem selItem in selectedItems)
                {
                    ProjectItem prjItem = selItem.Object as ProjectItem;
                    string filePath = prjItem.Properties.Item("FullPath").Value.ToString();
                    //System.Windows.Forms.MessageBox.Show(selItem.Name + filePath);
                    return filePath;
                }
            }
            return string.Empty;
        }
    }
}
