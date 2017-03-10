using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace PNP.Deployer
{
    // =======================================================
    /// <author>
    /// Simon-Pierre Plante (sp.plante@gmail.com)
    /// </author>
    // =======================================================
    public static class FilesUtility
    {
        #region Private Methods

        // ===========================================================================================================
        /// <summary>
        /// Copies recursively all files/folders from the specified source folder to the specified target folder
        /// </summary>
        /// <param name="source">The DirectoryInfo of the source folder</param>
        /// <param name="target">The DirectoryInfo of the target folder</param>
        /// <param name="ignoredFolders">An optional list of folder names which needs to be ignored</param>
        // ===========================================================================================================
        private static void CopyDirectoryRecursive(DirectoryInfo source, DirectoryInfo target, List<string> ignoredFolders = null)
        {
            bool ignoreFolder = (ignoredFolders != null && ignoredFolders.FirstOrDefault(x => source.FullName.ToLower() == x.ToLower()) != null);

            // --------------------------------------------------
            // If the current folder name isn't ignored
            // --------------------------------------------------
            if(!ignoreFolder)
            {
                // --------------------------------------------------
                // Creates the destination folder if needed
                // --------------------------------------------------
                if (!target.Exists)
                    Directory.CreateDirectory(target.FullName);

                // --------------------------------------------------
                // Copies each file in the destination folder
                // --------------------------------------------------
                foreach (FileInfo fileInfo in source.GetFiles())
                {
                    fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
                }

                // --------------------------------------------------
                // Copies each folder in the destination folder
                // --------------------------------------------------
                foreach (DirectoryInfo sourceSubDirInfo in source.GetDirectories())
                {
                    DirectoryInfo targetSubDirInfo = new DirectoryInfo(Path.Combine(target.FullName, sourceSubDirInfo.Name));
                    CopyDirectoryRecursive(sourceSubDirInfo, targetSubDirInfo, ignoredFolders);
                }
            }
        }

        #endregion


        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Copies recursively all files/folders from the specified source folder to the specified target folder
        /// </summary>
        /// <param name="sourceDirectory">The source directory that needs to be copied</param>
        /// <param name="destinationDirectory">The target directory </param>
        /// <param name="ignoredFolders">An optional list of folder names which needs to be ignored</param>
        // ===========================================================================================================
        public static void CopyDirectory(string sourceDirectory, string destinationDirectory, List<string> ignoredFolders = null)
        {
            DirectoryInfo infoSourceDirectory = new DirectoryInfo(sourceDirectory);
            DirectoryInfo infoDestinationDirectory = new DirectoryInfo(destinationDirectory);
            CopyDirectoryRecursive(infoSourceDirectory, infoDestinationDirectory, ignoredFolders);
        }

        #endregion
    }
}
