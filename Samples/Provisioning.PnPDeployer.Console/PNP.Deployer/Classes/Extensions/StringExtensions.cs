using System.Security;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    public static class StringExtensions
    {
        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Converts the current <b>String</b> into a <b>SecureString</b> object
        /// </summary>
        /// <param name="str">The current <b>String</b></param>
        /// <returns>The current string converted as a <b>SecureString</b> object</returns>
        // ===========================================================================================================
        public static SecureString ToSecureString(this string str)
        {
            SecureString ss = new SecureString();

            foreach(char c in str.ToCharArray())
            {
                ss.AppendChar(c);
            }

            return ss;
        }

        #endregion
    }
}
