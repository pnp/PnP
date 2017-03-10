using NLog;
using System;
using System.IO;
using System.Net;
using System.Configuration;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using PNP.Deployer.Common;

// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    public class Deployer
    {
        #region Constants

        private const string APP_SETTING_SEQUENCES_FILE = "clientSequencesFile";
        private const string SEQUENCES_SCHEMA_FILE_NAME = "Classes/XSD/Sequences.xsd";
        private const string ERROR_FOLDER_INVALID       = "The specified working directory '{0}' does not exist";
        private const string ERROR_SEQUENCES_NOT_FOUND  = "Sequences configuration file was not found at path '{0}'";
        private const string ERROR_SEQUENCES_INVALID    = "The {0} file is invalid : {1}";

        #endregion


        #region Private Members

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Public Members

        public string WorkingDirectory { get; set; }
        public EnvironmentType Environment { get; set; }
        public ICredentials Credentials { get; set; }
        public SequencesConfiguration SequencesConfig { get; set; }

        #endregion


        #region Constructor

        // ===========================================================================================================
        /// <summary>
        /// Initializes the deployer based on the specified arguments
        /// </summary>
        /// <param name="WorkingDirectory">The working directory on which to map the deployer</param>
        /// <param name="Environment">The type of environment on which the deployer is executed</param>
        /// <param name="Credential">The credential that must be used the get the SharePoint context</param>
        // ===========================================================================================================
        public Deployer(string WorkingDirectory, EnvironmentType Environment, ICredentials Credentials)
        {
            // --------------------------------------------------
            // If the WorkingDirectory doesn't exist
            // --------------------------------------------------
            if (!Directory.Exists(WorkingDirectory))
                throw new DeployerArgumentsException(String.Format(ERROR_FOLDER_INVALID, WorkingDirectory));

            // --------------------------------------------------
            // Initializes the deployer's arguments
            // --------------------------------------------------
            this.WorkingDirectory = WorkingDirectory;
            logger.Info("Loaded 'WorkingDirectory' with value '{0}'", this.WorkingDirectory);

            this.Environment = Environment;
            logger.Info("Loaded 'EnvironmentType' with value '{0}'", this.Environment);

            this.Credentials = Credentials;
            logger.Info("Loaded 'Credentials' with success");
        }

        #endregion


        #region Private Methods

        // ===========================================================================================================
        /// <summary>
        /// Throws an exception if the sequences file is missing or invalid
        /// </summary>
        /// <param name="sequencesFilePath"> The Sequences.xml file path</param>
        // ===========================================================================================================
        private void ValidateSequencesFile(string sequencesFilePath)
        {
            logger.Info("Validating the '{0}' file", Path.GetFileName(sequencesFilePath));

            // --------------------------------------------------
            // Throws an exception if Sequences.xml is not found
            // --------------------------------------------------
            if (!File.Exists(sequencesFilePath))
                throw new FileNotFoundException(String.Format(ERROR_SEQUENCES_NOT_FOUND, sequencesFilePath));

            // --------------------------------------------------
            // Throws an exception if Sequences.xml is invalid
            // --------------------------------------------------
            XmlUtility.ValidateSchema(sequencesFilePath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SEQUENCES_SCHEMA_FILE_NAME));

            logger.Info("File '{0}' has completed XSD validation with success", Path.GetFileName(sequencesFilePath));
        }

        
        // ===========================================================================================================
        /// <summary>
        /// Deserializes the sequences.xml file and stores a <b>SequencesConfiguration</b> object
        /// </summary>
        /// <returns>A <b>SequencesConfiguration</b> object</returns>
        // ===========================================================================================================
        private void LoadSequencesConfiguration()
        {
            // --------------------------------------------------
            // Validates the deployer's sequences file
            // --------------------------------------------------
            string sequencesFilePath = Path.Combine(this.WorkingDirectory, ConfigurationManager.AppSettings[APP_SETTING_SEQUENCES_FILE]);
            ValidateSequencesFile(sequencesFilePath);

            // --------------------------------------------------
            // Deserializes the Sequences.xml file
            // --------------------------------------------------
            this.SequencesConfig = XmlUtility.DeserializeXmlFile<SequencesConfiguration>(sequencesFilePath);
        }

        #endregion


        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Launches the deployer
        /// </summary>
        // ===========================================================================================================
        public void Launch()
        {
            // --------------------------------------------------
            // Loads the sequences configuration
            // --------------------------------------------------
            LoadSequencesConfiguration();

            // --------------------------------------------------
            // Maps a template provider to the working directory
            // --------------------------------------------------
            XMLTemplateProvider provider = new XMLFileSystemTemplateProvider(this.WorkingDirectory, string.Empty);

            // --------------------------------------------------
            // Launches the sequences
            // --------------------------------------------------
            foreach (Sequence sequence in this.SequencesConfig.Sequences)
            {
                sequence.Launch(this.Credentials, provider);
            }
        }

        #endregion
    }
}
