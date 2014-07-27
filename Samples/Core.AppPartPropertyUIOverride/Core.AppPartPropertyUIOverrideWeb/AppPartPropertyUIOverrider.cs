namespace Contoso.Core.AppPartPropertyUIOverrideWeb
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceModel;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.SharePoint.Client.EventReceivers;

    /// <summary>
    /// A helper class that makes easy to override the default properties UI for AppParts via JavaScript.
    /// </summary>
    public class AppPartPropertyUIOverrider
    {
        #region (private instance fields)
        /// <summary>
        /// Private instance field that contains the host web manager.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private HostWebManager hostWebManagerField;

        /// <summary>
        /// Private instance field that contains the jQuery filename in the ASP.NET application (Remote Web)'s "Scripts" directory to deploy to the host web's "_apps/_globals/" folder.  
        /// If the file and path already exists, it will overwrite it. .
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string jQueryFileNameField;

        /// <summary>
        /// Private instance field that contains the properties of the app event receiver event.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SPRemoteEventProperties propertiesField;

        /// <summary>
        /// Private instance field that contains the remote web full url for override.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string remoteWebFullUrlOverrideField;

        /// <summary>
        /// Private instance field that contains a template of the JavaScript source code that will be used to load dependencies and then invoke the specified
        /// App Part property UI override JavaScript code.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string templateField;
        #endregion

        #region (constructor)
        /// <summary>
        /// Initializes a new instance of the <see cref="AppPartPropertyUIOverrider" /> class.
        /// </summary>
        /// <param name="hostWebManager">The host web manager.</param>
        /// <param name="properties">The properties of the app event receiver event.</param>
        /// <param name="jQueryFileName">Filename of jquery-x.x.x.min.js sitting in the ASP.NET (Remote Web) Scripts directory.  Example: "jquery-2.1.0.min.js"</param>
        public AppPartPropertyUIOverrider(HostWebManager hostWebManager, SPRemoteEventProperties properties, string jQueryFileName)
            : this(hostWebManager, properties, jQueryFileName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppPartPropertyUIOverrider" /> class.
        /// </summary>
        /// <param name="hostWebManager">The host web manager.</param>
        /// <param name="properties">The properties of the app event receiver event.</param>
        /// <param name="jQueryFileName">Filename of jquery-x.x.x.min.js sitting in the ASP.NET (Remote Web) Scripts directory.  Example: "jquery-2.1.0.min.js"</param>
        /// <param name="remoteWebFullUrlOverride">Optional override to the remote web full url detection logic.  Example: "https://acutalurl.contoso.com"</param>
        public AppPartPropertyUIOverrider(HostWebManager hostWebManager, SPRemoteEventProperties properties, string jQueryFileName, string remoteWebFullUrlOverride)
        {
            this.hostWebManagerField = hostWebManager;
            this.propertiesField = properties;
            this.jQueryFileNameField = jQueryFileName;
            this.remoteWebFullUrlOverrideField = remoteWebFullUrlOverride;
        }
        #endregion

        #region PUBLIC METHOD: OverrideAppPartPropertyUI()
        /// <summary>
        /// Allows overriding of the default properties UI for AppParts via an app for SharePoint specific JavaScript file that is already deployed on host web via that deployed via <see cref="HostWebManager"/>'s DeployAppSpecificFile() method.
        /// </summary>
        /// <param name="categoryDisplayTextToFind">The App Part property category display text to find.</param>
        /// <param name="jsFileNameToInvoke">The host web deployed JavaScript filename to load and invoke. This file was deployed via <see cref="HostWebManager"/>'s DeployAppSpecificFile() method.</param>
        public void OverrideAppPartPropertyUI(string categoryDisplayTextToFind, string jsFileNameToInvoke)
        {
            // deploy static files to host web
            this.hostWebManagerField.DeployGlobalFile("Scripts/" + this.jQueryFileNameField);
            this.hostWebManagerField.DeployGlobalFile("Scripts/Contoso.AppPartPropertyUIOverride.js");
            this.hostWebManagerField.DeployAppSpecificFile("Scripts/" + jsFileNameToInvoke);

            // generate and deploy custom JavaScript file
            Regex regex = new Regex("[^a-zA-Z0-9-]");
            string safeFunctionName = "ContosoAppPartPropertyUIOverrideInit_" + regex.Replace(categoryDisplayTextToFind, "");
            string safeFileName = safeFunctionName + ".js";
            string contents = GetAppPartPropertyOverrideUILoaderJSFileTemplate();
            contents = contents.Replace("_SAFE_FILE_NAME_", safeFileName);
            contents = contents.Replace("_SAFE_FUNCTION_NAME_", safeFunctionName);
            contents = contents.Replace("_CATEGORY_DISPLAY_TEXT_TO_FIND_", categoryDisplayTextToFind);
            contents = contents.Replace("_JS_FILE_NAME_TO_INVOKE_", jsFileNameToInvoke);
            this.hostWebManagerField.CreateAppSpecificFile("Scripts/" + safeFileName, contents);

            // wire up custom JavaScript file on all pages of the host web
            this.hostWebManagerField.WireUpAppSpecificJSFileOnAllPagesInWeb(safeFileName, 1000);
        }
        #endregion

        #region (private helper methods)
        private string GetAppPartPropertyOverrideUILoaderJSFileTemplate()
        {
            if (string.IsNullOrEmpty(this.templateField))
            {
                // template not generated yet
                #region get the language, hostWebFullUrl, hostWebServerRelativeUrl, appWebFullUrl, and remoteWebFullurl
                SPRemoteAppEventProperties appEventProperties = this.propertiesField.AppEventProperties;
                Uri uri = appEventProperties.HostWebFullUrl;
                string language = this.propertiesField.UICultureLCID.ToString(CultureInfo.InvariantCulture);
                string hostWebFullUrl = uri.ToString();
                string hostWebServerRelativeUrl = uri.AbsolutePath;
                string appWebFullUrl = string.Empty;
                try
                {
                    appWebFullUrl = appEventProperties.AppWebFullUrl.ToString();
                }
                catch
                {
                    appWebFullUrl = string.Empty;
                }

                string remoteWebFullUrl = string.Empty;
                if (string.IsNullOrWhiteSpace(this.remoteWebFullUrlOverrideField))
                {
                    ReadOnlyCollection<Uri> baseAddresses = OperationContext.Current.Host.BaseAddresses;
                    foreach (Uri baseAddress in baseAddresses)
                    {
                        if (baseAddress.Scheme == "https")
                        {
                            remoteWebFullUrl = baseAddress.ToString();
                            int index = remoteWebFullUrl.IndexOf("/obj/");
                            if (index > -1)
                            {
                                remoteWebFullUrl = remoteWebFullUrl.Substring(0, index);
                            }

                            break;
                        }
                    }
                }
                else
                {
                    remoteWebFullUrl = this.remoteWebFullUrlOverrideField;
                }
                #endregion

                // generate the JavaScript code template now
                StringBuilder sb = new StringBuilder();

                #region function _SAFE_FUNCTION_NAME_DOMLoaded()
                sb.Append("function _SAFE_FUNCTION_NAME_DOMLoaded() { ");
                sb.Append("window.AppPartPropertyUIDOMLoaded = true;");

                // see if App Part property pane already found... (to prevent double execution)
                sb.Append("if (window.AppPartPropertyUIFound !== true) {");

                // not found, see if it's on this page
                sb.Append("var appPartPropertyPaneTd = document.getElementById(\"MSOTlPn_Parts\");");
                sb.Append("if (appPartPropertyPaneTd !== null) {");

                // found it (the user is editing an App Part)
                sb.Append("window.AppPartPropertyUIFound = true;");

                // now see if the App Part is the one we care about
                // by looking for the unique custom category text
                sb.Append("if (appPartPropertyPaneTd.innerHTML.indexOf(\"_CATEGORY_DISPLAY_TEXT_TO_FIND_\") > -1) {");

                // category found!
                // this is the App Part property UI we are looking for
                // immediately hide the property pane so that we can manipulate it
                sb.Append("appPartPropertyPaneTd.style.display=\"none\";");

                // now, dynamically load and execute Contoso.AppPartPropertyUIOverride.js
                sb.Append("var head = document.getElementsByTagName(\"head\")[0];");
                sb.Append("var script = document.createElement(\"script\");");
                sb.Append("script.src = \"" + hostWebServerRelativeUrl + "/_apps/_globals/Scripts/Contoso.AppPartPropertyUIOverride.js\";");
                sb.Append("var done = false;");

                sb.Append("script.onload = script.onreadystatechange = function () {");

                sb.Append("if (!done && (!this.readyState || this.readyState == \"loaded\" || this.readyState == \"complete\")) {");
                sb.Append("done = true;");

                // Contoso.AppPartPropertyUIOverride.js loaded!

                // set public fields on Contoso.AppPartPropertyUIOverride.js
                sb.Append("Contoso.AppPartPropertyUIOverride.appWebFullUrl=\"" + appWebFullUrl + "\";");
                sb.Append("Contoso.AppPartPropertyUIOverride.hostWebFullUrl=\"" + hostWebFullUrl + "\",");
                sb.Append("Contoso.AppPartPropertyUIOverride.hostWebServerRelativeUrl=\"" + hostWebServerRelativeUrl + "\";");
                sb.Append("Contoso.AppPartPropertyUIOverride.language=" + language + ";");
                sb.Append("Contoso.AppPartPropertyUIOverride.remoteWebFullUrl=\"" + remoteWebFullUrl + "\";");

                // now call init
                sb.Append("Contoso.AppPartPropertyUIOverride.zinternal.init({");
                sb.Append("appPartPropertyPaneTdElement:appPartPropertyPaneTd,");
                sb.Append("jqueryPath:\"" + hostWebServerRelativeUrl + "/_apps/_globals/Scripts/" + this.jQueryFileNameField + "\",");
                sb.Append("jsFileToInvoke:\"" + hostWebServerRelativeUrl + "/_apps/" + this.hostWebManagerField.AppForSharePointInternalName + "/Scripts/_JS_FILE_NAME_TO_INVOKE_\"");
                sb.Append("});");

                // end if (!done && (!this.readyState || this.readyState == \"loaded\" || this.readyState == \"complete\"))
                sb.Append("}");

                // end script.onload = script.onreadystatechange = function () {
                sb.Append("};");


                sb.Append("head.appendChild(script);");

                // now that it is loaded, set the fields
                // now that the fields are set, call Contoso.AppPartPropertyUIOverride.zinternal.init()
                // and pass the appropriate parameters


                // end if (appPartPropertyPaneTd.innerHTML.indexOf(\"_CATEGORY_DISPLAY_TEXT_TO_FIND_\") > -1)
                sb.Append("}");

                // end if (appPartPropertyPaneTd !== null)
                sb.Append("}");

                // end if (window.AppPartPropertyUIFound !== true)
                sb.Append("}");

                // end function _SAFE_FUNCTION_NAME_DOMLoaded()
                sb.Append("}");
                #endregion

                #region function _SAFE_FUNCTION_NAME_()
                // the JavaScript code below gets called when this generated script is done loading
                sb.Append("function _SAFE_FUNCTION_NAME_() { ");

                // wait until dom loaded
                sb.Append("if (window.AppPartPropertyUIDOMLoaded === true) {");
                sb.Append("_SAFE_FUNCTION_NAME_DOMLoaded();");
                sb.Append("} ");
                sb.Append("else {");
                sb.Append("if (document.readyState === \"complete\") {");
                sb.Append("_SAFE_FUNCTION_NAME_DOMLoaded();");
                sb.Append("}");
                sb.Append("else {");
                sb.Append("document.addEventListener(\"DOMContentLoaded\", _SAFE_FUNCTION_NAME_DOMLoaded, false);");
                sb.Append("window.addEventListener(\"load\", _SAFE_FUNCTION_NAME_DOMLoaded, false );");
                sb.Append("}");
                sb.Append("}");

                sb.Append("}");
                #endregion

                #region (SharePoint 2013 Minimal Download Strategy)
                // the JavaScript code below implements the SharePoint 2013 Minimal Download Strategy if it's turned on
                // if it's turned off, there's a fallback as well
                // both routes automatically call the _SAFE_FUNCTION_NAME_ above
                sb.Append("RegisterModuleInit(\"_SAFE_FILE_NAME_\", _SAFE_FUNCTION_NAME_); _SAFE_FUNCTION_NAME_();");
                sb.Append("if (typeof(Sys) != \"undefined\" && Boolean(Sys) && Boolean(Sys.Application)) { Sys.Application.notifyScriptLoaded(); }");
                sb.Append("if (typeof(NotifyScriptLoadedAndExecuteWaitingJobs) == \"function\") { NotifyScriptLoadedAndExecuteWaitingJobs(\"_SAFE_FILE_NAME_\"); }");
                #endregion

                this.templateField = sb.ToString();
            }

            return this.templateField;
        }
        #endregion
    }
}
