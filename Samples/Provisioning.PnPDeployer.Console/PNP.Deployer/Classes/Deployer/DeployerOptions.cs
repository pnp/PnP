using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    public class DeployerOptions
    {
        #region Constants

        private const string HELP_ENVIRONMENT           = "Whether the deployment occurs on a 'OnPrem' on 'Online' envrionment.";
        private const string HELP_WORKING_DIRECTORY     = "The working directory on which the deployer should be mapped to in order to deploy the artifacts.";
        private const string HELP_PROMPT_CREDENTIALS    = "Wheter there should be a prompt for credentials or not.";

        #endregion


        #region Public Members

        // --------------------------------------------------
        // The environment on which the deployer is executed
        // --------------------------------------------------
        [Option('e', "Environment", DefaultValue = EnvironmentType.OnPrem, HelpText = HELP_ENVIRONMENT, Required = false)]
        public EnvironmentType Environment { get; set; }

        // --------------------------------------------------
        // The working directory that contains the artifacts
        // --------------------------------------------------
        [Option('w', "WorkingDirectory", HelpText = HELP_WORKING_DIRECTORY, Required = true)]
        public string WorkingDirectory { get; set; }

        // --------------------------------------------------
        // Whether to prompt for specific credentials or not
        // --------------------------------------------------
        [Option('p', "PromptCredentials", DefaultValue = false, HelpText = HELP_PROMPT_CREDENTIALS, Required = false)]
        public Boolean PromptCredentials { get; set; }

        // --------------------------------------------------
        // Stores the parser state for errors handling
        // --------------------------------------------------
        [ParserState]
        public IParserState LastParserState { get; set; }

        #endregion


        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Default 'help' screen implementation for the console
        /// </summary>
        /// <returns>The default help screen</returns>
        // ===========================================================================================================
        [HelpOption(HelpText = "Display this deployer help screen")]
        public string GetUsage()
        {
            
            // --------------------------------------------------
            // Initializes the HelpText object
            // --------------------------------------------------
            var help = new HelpText
            {
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            
            help.AddPreOptionsLine(string.Format("  PNP.Deployer (v.{0}) - By Simon-Pierre Plante", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("");

            // --------------------------------------------------
            // Adds the error section if any
            // --------------------------------------------------
            if (this.LastParserState != null && this.LastParserState.Errors.Count > 0 )
            {
                
                var errors = help.RenderParsingErrorsText(this, 4);

                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine("  Error(s):");
                    help.AddPreOptionsLine("");
                    help.AddPreOptionsLine(errors);
                    help.AddPreOptionsLine("");
                }
            }

            // --------------------------------------------------
            // Adds the Usage section
            // --------------------------------------------------
            help.AddPreOptionsLine("  Usage:");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("    PNP.Deployer.exe --WorkingDirectory \"C:\\DeploymentFolder\"");
            help.AddPreOptionsLine("                     [--PromptCredentials] [--Environment \"OnPrem|Online\"]");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("");

            // --------------------------------------------------
            // Adds the Options section
            // --------------------------------------------------
            help.AddPreOptionsLine("  Options: ");
            help.AddOptions(this);

            return help;
        }

        #endregion
    }
}
