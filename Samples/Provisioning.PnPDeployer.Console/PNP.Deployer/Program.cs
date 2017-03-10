using NLog;
using System;
using System.IO;
using System.Net;
using System.Security;
using System.Configuration;
using Microsoft.SharePoint.Client;


namespace PNP.Deployer
{
    class Program
    {
        #region Constants

        private const string APP_SETTING_TOKENS_FILE    = "clientTokensFile";
        private const string LABEL_USERNAME             = "Username";
        private const string LABEL_PASSWORD             = "Password";

        #endregion


        #region Private Members

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Private Methods

        // ===========================================================================================================
        /// <summary>
        /// Returns the correct credential set based on the environment and PromptCredentials switch
        /// </summary>
        /// <param name="environment">The environment on which the deployer is executed</param>
        /// <param name="prompt">Whether the user should be prompted for credentials or not (always true for online)</param>
        /// <returns>A <b>NetworkCredential</b> object</returns>
        // ===========================================================================================================
        private static ICredentials GetAuthenticationCredentials(EnvironmentType environment, bool prompt)
        {
            ICredentials credentials = CredentialCache.DefaultNetworkCredentials;

            if(prompt || environment == EnvironmentType.Online)
            {
                string username = ConsoleUtility.GetInputAsText(LABEL_USERNAME);
                SecureString password = ConsoleUtility.GetInputAsSecureString(LABEL_PASSWORD);
                Console.WriteLine(string.Empty);

                if(environment == EnvironmentType.Online)
                {
                    credentials = new SharePointOnlineCredentials(username, password);
                }
                else
                {
                    credentials = new NetworkCredential(username, password);
                }
            }

            return credentials;
        }

        #endregion


        #region Public Methods
           
        public static void Main(string[] args)
        {
            try
            {
                // ============================================================
                // Parses the arguments using the CommandLine Parsing Library
                // ============================================================
                DeployerOptions options = new DeployerOptions();

                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    // -------------------------------------------------
                    // Gets the credentials based on the arguments
                    // -------------------------------------------------
                    ICredentials credentials = GetAuthenticationCredentials(options.Environment, options.PromptCredentials);


                    // -------------------------------------------------
                    // Tokenizes the deployer's working directory
                    // -------------------------------------------------
                    logger.Section("Tokenizing working directory", LogLevel.Info);
                    Tokenizer tokenizer = new Tokenizer(Path.Combine(options.WorkingDirectory, ConfigurationManager.AppSettings[APP_SETTING_TOKENS_FILE]));
                    string tokenizedWorkingDirectory = tokenizer.TokenizeFolder(options.WorkingDirectory);
                    

                    // -------------------------------------------------
                    // Initializes the deployer
                    // -------------------------------------------------
                    logger.Section("Initializing deployer", LogLevel.Info);
                    Deployer deployer = new Deployer(tokenizedWorkingDirectory, options.Environment, credentials);


                    // -------------------------------------------------
                    // Starts the deployment
                    // -------------------------------------------------
                    logger.Section("Launching deployer", LogLevel.Info);
                    deployer.Launch();

                    Environment.Exit((int)ExitCode.Success);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
                Environment.Exit((int)ExitCode.Failure);
            }
        }

        #endregion
    }
}
