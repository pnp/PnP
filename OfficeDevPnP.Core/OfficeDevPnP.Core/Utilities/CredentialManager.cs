using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.SharePoint.Client;
using Microsoft.Win32.SafeHandles;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace OfficeDevPnP.Core.Utilities
{
    public static class CredentialManager
    {

        /// <summary>
        /// Returns a SharePoint Online Credential given a certain name. Add the credential in the Windows Credential Manager and create a new Windows Credential. Then add a new GENERIC Credential. The name parameter in the method maps to the Internet or network address field.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Microsoft.SharePoint.Client.SharePointOnlineCredentials</returns>
        public static SharePointOnlineCredentials GetSharePointOnlineCredential(string name)
        {
            var networkCredential = GetCredential(name);

            var credential = new SharePointOnlineCredentials(networkCredential.UserName,networkCredential.SecurePassword);

            return credential;
        }

        /// <summary>
        /// Returns a NetworkCredential given a certain name. Add the credential in the Windows Credential Manager and create a new Windows Credential. Then add a new GENERIC Credential. The name parameter in the method maps to the Internet or network address field.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>System.Net.NetworkCredential</returns>
        public static NetworkCredential GetCredential(string name)
        {
            NetworkCredential credential = null;
            IntPtr credPtr;

            var success = NativeMethods.CredRead(name, CRED_TYPE.GENERIC, 0, out credPtr);
            if (success)
            {
                var critCred = new CriticalCredentialHandle(credPtr);
                var cred = critCred.GetCredential();
                var username = cred.UserName;
                var securePassword = new SecureString();
                var credentialBlob = cred.CredentialBlob;
                var passwordChars = credentialBlob.ToCharArray();
                foreach (var c in passwordChars)
                {
                    securePassword.AppendChar(c);
                }
                credential = new NetworkCredential(username, securePassword);
            }
            return credential;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NativeCredential
        {
            public UInt32 Flags;
            public CRED_TYPE Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public IntPtr CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;

            internal static NativeCredential GetNativeCredential(Credential cred)
            {
                var ncred = new NativeCredential();
                ncred.AttributeCount = 0;
                ncred.Attributes = IntPtr.Zero;
                ncred.Comment = IntPtr.Zero;
                ncred.TargetAlias = IntPtr.Zero;
                ncred.Type = CRED_TYPE.GENERIC;
                ncred.Persist = (UInt32)1;
                ncred.CredentialBlobSize = (UInt32)cred.CredentialBlobSize;
                ncred.TargetName = Marshal.StringToCoTaskMemUni(cred.TargetName);
                ncred.CredentialBlob = Marshal.StringToCoTaskMemUni(cred.CredentialBlob);
                ncred.UserName = Marshal.StringToCoTaskMemUni(Environment.UserName);
                return ncred;
            }

           }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct Credential
        {
            public UInt32 Flags;
            public CRED_TYPE Type;
            public string TargetName;
            public string Comment;
            public FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public string CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public string TargetAlias;
            public string UserName;
        }

        private enum CRED_TYPE : uint
        {
            GENERIC = 1,
            DOMAIN_PASSWORD = 2,
            DOMAIN_CERTIFICATE = 3,
            DOMAIN_VISIBLE_PASSWORD = 4,
            GENERIC_CERTIFICATE = 5,
            DOMAIN_EXTENDED = 6,
            MAXIMUM = 7,      // Maximum supported cred type
            MAXIMUM_EX = (MAXIMUM + 1000),  // Allow new applications to run on old OSes
        }

        private class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
        {
            public CriticalCredentialHandle(IntPtr preexistingHandle)
            {
                SetHandle(preexistingHandle);
            }

            public Credential GetCredential()
            {
                if (!IsInvalid)
                {
                    var ncred = (NativeCredential)Marshal.PtrToStructure(handle,
                          typeof(NativeCredential));
                    var cred = new Credential();
                    cred.CredentialBlobSize = ncred.CredentialBlobSize;
                    cred.CredentialBlob = Marshal.PtrToStringUni(ncred.CredentialBlob,
                          (int)ncred.CredentialBlobSize / 2);
                    cred.UserName = Marshal.PtrToStringUni(ncred.UserName);
                    cred.TargetName = Marshal.PtrToStringUni(ncred.TargetName);
                    cred.TargetAlias = Marshal.PtrToStringUni(ncred.TargetAlias);
                    cred.Type = ncred.Type;
                    cred.Flags = ncred.Flags;
                    cred.Persist = ncred.Persist;
                    return cred;
                }
                else
                {
                    throw new InvalidOperationException("Invalid CriticalHandle!");
                }
            }

            override protected bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    NativeMethods.CredFree(handle);
                    SetHandleAsInvalid();
                    return true;
                }
                return false;
            }
        }

        private static class NativeMethods
        {
            [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool CredRead(string target, CRED_TYPE type, int reservedFlag, out IntPtr CredentialPtr);

            [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
            public static extern bool CredFree([In] IntPtr cred);
        }
    }
}
