using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    public static class Utility
    {
        private const int WindowsVistaMajorNumber = 6;

        internal const int S_OK = 0;

        private const string fipsPolicyRegistryKey = "System\\CurrentControlSet\\Control\\Lsa";

        private static int fipsAlgorithmPolicy = -1;

        internal static bool RequiresFipsCompliance
        {
            [System.Security.SecuritySafeCritical]
            get
            {
                if (Utility.fipsAlgorithmPolicy == -1)
                {
                    if (System.Environment.OSVersion.Version.Major >= 6)
                    {
                        bool flag2;
                        bool flag = 0 == NativeMethods.BCryptGetFipsAlgorithmMode(out flag2);
                        if (flag && flag2)
                        {
                            Utility.fipsAlgorithmPolicy = 1;
                        }
                        else
                        {
                            Utility.fipsAlgorithmPolicy = 0;
                        }
                    }
                    else
                    {
                        Utility.fipsAlgorithmPolicy = Utility.GetFipsAlgorithmPolicyKeyFromRegistry();
                        if (Utility.fipsAlgorithmPolicy != 1)
                        {
                            Utility.fipsAlgorithmPolicy = 0;
                        }
                    }
                }
                return Utility.fipsAlgorithmPolicy == 1;
            }
        }

        public static void VerifyNonNullArgument(string name, object value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException(name);
            }
        }

        public static void VerifyNonNullOrEmptyStringArgument(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new System.ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "The parameter '{0}' cannot be a null or empty string", new object[]
                {
                    name
                }));
            }
        }

        [System.Security.SecurityCritical]
        [System.Security.Permissions.RegistryPermission(System.Security.Permissions.SecurityAction.Assert, Read = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Control\\Lsa")]
        private static int GetFipsAlgorithmPolicyKeyFromRegistry()
        {
            int result = -1;
            using (Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa", false))
            {
                if (registryKey != null)
                {
                    object value = registryKey.GetValue("FIPSAlgorithmPolicy");
                    if (value != null)
                    {
                        try
                        {
                            result = (int)value;
                        }
                        catch (System.InvalidCastException)
                        {
                            return -1;
                        }
                    }
                }
            }
            return result;
        }
    }
}
