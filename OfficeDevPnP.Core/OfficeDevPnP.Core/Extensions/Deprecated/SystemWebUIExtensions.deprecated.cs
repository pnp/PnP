namespace System.Web.UI {
    public static class SystemWebUIExtensions {

        /// <summary>
        /// Renders the base SharePoint chrome script to the page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageType"></param>
        [Obsolete("Method deprecated")]
        public static void RegisterSPChromePageScript(this Page page, Type pageType) {
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }

            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };

                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            //register script in page
            page.ClientScript.RegisterClientScriptBlock(pageType, "BasePageScript", script, true);
        }
    }
}
