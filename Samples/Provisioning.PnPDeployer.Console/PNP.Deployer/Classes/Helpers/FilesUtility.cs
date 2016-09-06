using System.IO;


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
        // ===========================================================================================================
        private static void CopyDirectoryRecursive(DirectoryInfo source, DirectoryInfo target)
        {
            // --------------------------------------------------
            // Creates the destination folder if needed
            // --------------------------------------------------
            if (!target.Exists)
                Directory.CreateDirectory(target.FullName);

            // --------------------------------------------------
            // Copies each file in the destination folder
            // --------------------------------------------------
            foreach(FileInfo fileInfo in source.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
            }

            // --------------------------------------------------
            // Copies each folder in the destination folder
            // --------------------------------------------------
            foreach (DirectoryInfo sourceSubDirInfo in source.GetDirectories())
            {
                DirectoryInfo targetSubDirInfo = target.CreateSubdirectory(sourceSubDirInfo.Name);
                CopyDirectoryRecursive(sourceSubDirInfo, targetSubDirInfo);
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
        // ===========================================================================================================
        public static void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            DirectoryInfo infoSourceDirectory = new DirectoryInfo(sourceDirectory);
            DirectoryInfo infoDestinationDirectory = new DirectoryInfo(destinationDirectory);
            CopyDirectoryRecursive(infoSourceDirectory, infoDestinationDirectory);
        }

        #endregion
    }
}
