using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    internal static class NativeMethods
    {
        private const string BCRYPT = "bcrypt.dll";

        [System.Runtime.InteropServices.DllImport("bcrypt.dll", SetLastError = true)]
        public static extern int BCryptGetFipsAlgorithmMode([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)] out bool pfEnabled);
    }
}
