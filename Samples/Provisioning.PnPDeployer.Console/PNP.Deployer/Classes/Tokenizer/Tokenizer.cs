using NLog;
using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using PNP.Deployer.Common;


namespace PNP.Deployer
{
    // =======================================================
    /// <author>
    /// Simon-Pierre Plante (sp.plante@gmail.com)
    /// </author>
    // =======================================================
    public class Tokenizer
    {
        #region Constants

        private const string ERROR_FOLDER_NOT_FOUND         = "Unable to tokenize the specified folder '{0}' : Folder not found.";
        private const string ERROR_FILE_NOT_FOUND           = "Unable to tokenize the specified file '{0}' : File not found.";
        private const string ERROR_TOKENS_NOT_FOUND         = "Unable to  tokenize the specified folder with the tokens file '{0}' : File not found.";
        private const string TOKENS_SCHEMA_FILE_NAME        = "Classes/XSD/TokensConfiguration.xsd";
        private const string TOKENIZED_FOLDER_EXTENSION     = "_Tokenized";
        private const string DEFAULT_TOKENS_PREFIX          = "token-";
        private const string APP_SETTING_IGNORED_FOLDERS    = "clientIgnoredFolders";

        #endregion


        #region Private Members

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private List<string> ignoredFolders = new List<string>();

        #endregion


        #region Public Members

        public List<Token> Tokens = new List<Token>();

        #endregion


        #region Constructors

        public Tokenizer(string tokensFilePath)
        {
            // --------------------------------------------------
            // Loads the 'default' tokens from app.config
            // --------------------------------------------------
            List<Token> appSettingsTokens = LoadTokensFromAppSettings();
            this.Tokens.AddRange(appSettingsTokens);
            logger.Info("Loaded '{0}' tokens from app settings", appSettingsTokens.Count);

            // --------------------------------------------------
            // Loads tokens from the client tokens file
            // --------------------------------------------------
            List<Token> fileTokens = LoadTokensFromFile(tokensFilePath);
            this.Tokens.AddRange(fileTokens);
            logger.Info("Loaded '{0}' tokens from file '{1}'", fileTokens.Count, tokensFilePath);
        }

        #endregion


        #region Private Methods

        // ===========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // ===========================================================================================================
        private List<Token> LoadTokensFromAppSettings()
        {
            List<Token> tokens = new List<Token>();

            if(ConfigurationManager.AppSettings != null)
            {
                foreach (string key in ConfigurationManager.AppSettings.AllKeys.Where(x => x.StartsWith(DEFAULT_TOKENS_PREFIX)))
                {
                    tokens.Add(new Token() { Key = key.Substring(DEFAULT_TOKENS_PREFIX.Length), Value = ConfigurationManager.AppSettings[key] });
                }
            }

            return tokens;
        }

        // ===========================================================================================================
        /// <summary>
        /// Deserializes the specified tokens file if available and returns a list of <b>Token</b> objects
        /// </summary>
        /// <param name="tokensFilePath"></param>
        /// <returns>A list of <b>Token</b> objects</returns>
        // ===========================================================================================================
        private List<Token> LoadTokensFromFile(string tokensFilePath)
        {
            List<Token> tokens = new List<Token>();

            if (File.Exists(tokensFilePath))
            {
                // --------------------------------------------------
                // Validates the tokens file schema by XSD
                // --------------------------------------------------
                ValidateTokensFile(tokensFilePath);

                // --------------------------------------------------
                // Deserializes the tokens file
                // --------------------------------------------------
                TokensConfiguration tokensConfig = XmlUtility.DeserializeXmlFile<TokensConfiguration>(tokensFilePath);

                if (tokensConfig != null && tokensConfig.Tokens != null && tokensConfig.Tokens.Count > 0)
                {
                    tokens = tokensConfig.Tokens;
                }
            }

            return tokens;
        }


        // ===========================================================================================================
        /// <summary>
        /// Throws an exception if the tokens file is invalid
        /// </summary>
        /// <param name="tokensFilePath"> The tokens file path</param>
        // ===========================================================================================================
        private void ValidateTokensFile(string tokensFilePath)
        {
            logger.Info("Validating the '{0}' file", Path.GetFileName(tokensFilePath));

            // --------------------------------------------------
            // Throws an exception if tokens file is invalid
            // --------------------------------------------------
            XmlUtility.ValidateSchema(tokensFilePath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TOKENS_SCHEMA_FILE_NAME));

            logger.Info("File '{0}' has completed XSD validation with success", Path.GetFileName(tokensFilePath));
        }


        // ===========================================================================================================
        /// <summary>
        /// Tokenizes the specified folder and it's files/subfolders recursively
        /// </summary>
        /// <param name="folderPath">The path of the folder that needs to be tokenized</param>
        // ===========================================================================================================
        private void TokenizeFolderRecursive(string folderPath)
        {
            DirectoryInfo infoFolder = new DirectoryInfo(folderPath);

            // --------------------------------------------------
            // Tokenizes each file in the folder
            // --------------------------------------------------
            foreach (FileInfo fileInfo in infoFolder.GetFiles())
            {
                TokenizeFile(fileInfo.FullName);
            }

            // --------------------------------------------------
            // Tokenizes each subfolder in the folder
            // --------------------------------------------------
            foreach (DirectoryInfo dirInfo in infoFolder.GetDirectories())
            {
                TokenizeFolderRecursive(dirInfo.FullName);
            }
        }

        #endregion


        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Creates a tokenized version of the specified folder based on the available tokens 
        /// </summary>
        /// <param name="folderPath">The path of the folder that needs to be tokenized</param>
        /// <returns>The path of the tokenized folder</returns>
        // ===========================================================================================================
        public string TokenizeFolder(string folderPath)
        {
            string tokenizedFolderPath = null;

            if (this.Tokens.Count > 0)
            {
                logger.Info("Tokenizing folder '{0}' to destination '{0}{1}'", Path.GetFileName(folderPath), TOKENIZED_FOLDER_EXTENSION);

                // --------------------------------------------------
                // Validates the source folder
                // --------------------------------------------------
                if (!Directory.Exists(folderPath))
                    throw new DirectoryNotFoundException(string.Format(ERROR_FOLDER_NOT_FOUND, folderPath));

                // --------------------------------------------------
                // Loads the ignored folders
                // --------------------------------------------------
                ignoredFolders = ConfigurationManager.AppSettings[APP_SETTING_IGNORED_FOLDERS].Split('|').Where(x => !string.IsNullOrEmpty(x)).Select(x => Path.Combine(folderPath.ToLower(), x.ToLower())).ToList<string>();

                // --------------------------------------------------
                // Copies the source folder to it's tokenized destination
                // --------------------------------------------------
                tokenizedFolderPath = folderPath + TOKENIZED_FOLDER_EXTENSION;
                FilesUtility.CopyDirectory(folderPath, tokenizedFolderPath, ignoredFolders);

                // --------------------------------------------------
                // Tokenizes the destination folder recursively
                // --------------------------------------------------
                TokenizeFolderRecursive(tokenizedFolderPath);

                logger.Info("Folder '{0}' tokenized with success to destination '{0}{1}'", Path.GetFileName(folderPath), TOKENIZED_FOLDER_EXTENSION);
            }

            return tokenizedFolderPath;
        }


        // ===========================================================================================================
        /// <summary>
        /// Tokenizes the specified file based on the available tokens
        /// </summary>
        /// <param name="filePath">The path of the file that needs to be tokenized</param>
        // ===========================================================================================================
        public void TokenizeFile(string filePath)
        {
            // --------------------------------------------------
            // Validates the specified file
            // --------------------------------------------------
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format(ERROR_FILE_NOT_FOUND, filePath));
            
            if (!filePath.ToLower().Trim().EndsWith(".pnp"))
			{
                // --------------------------------------------------
                // Replaces the tokens by their value
                // --------------------------------------------------
                string fileContent = File.ReadAllText(filePath);
                bool needSave = false;

                foreach (Token token in this.Tokens)
                {
                    if (fileContent.Contains("{{" + token.Key + "}}"))
					{
                        fileContent = fileContent.Replace("{{" + token.Key + "}}", token.Value);
                        needSave = true;
                    }
                }
                if(needSave)
                    File.WriteAllText(filePath, fileContent);
            }
        }

        #endregion
    }
}
