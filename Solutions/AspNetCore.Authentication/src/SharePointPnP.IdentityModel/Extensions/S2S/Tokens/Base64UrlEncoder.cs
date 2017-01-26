using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    internal static class Base64UrlEncoder
    {
        public static System.Text.Encoding TextEncoding = System.Text.Encoding.UTF8;

        private static char Base64PadCharacter = '=';

        private static string DoubleBase64PadCharacter = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{0}", new object[]
        {
            Base64UrlEncoder.Base64PadCharacter
        });

        private static char Base64Character62 = '+';

        private static char Base64Character63 = '/';

        private static char Base64UrlCharacter62 = '-';

        private static char Base64UrlCharacter63 = '_';

        public static string Encode(string arg)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("arg", arg);
            return Base64UrlEncoder.Encode(Base64UrlEncoder.TextEncoding.GetBytes(arg));
        }

        public static string Encode(byte[] arg)
        {
            Utility.VerifyNonNullArgument("arg", arg);
            string text = System.Convert.ToBase64String(arg);
            text = text.Split(new char[]
            {
                Base64UrlEncoder.Base64PadCharacter
            })[0];
            text = text.Replace(Base64UrlEncoder.Base64Character62, Base64UrlEncoder.Base64UrlCharacter62);
            return text.Replace(Base64UrlEncoder.Base64Character63, Base64UrlEncoder.Base64UrlCharacter63);
        }

        public static byte[] DecodeBytes(string arg)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("arg", arg);
            string text = arg.Replace(Base64UrlEncoder.Base64UrlCharacter62, Base64UrlEncoder.Base64Character62);
            text = text.Replace(Base64UrlEncoder.Base64UrlCharacter63, Base64UrlEncoder.Base64Character63);
            switch (text.Length % 4)
            {
                case 0:
                    goto IL_7D;
                case 2:
                    text += Base64UrlEncoder.DoubleBase64PadCharacter;
                    goto IL_7D;
                case 3:
                    text += Base64UrlEncoder.Base64PadCharacter;
                    goto IL_7D;
            }
            throw new System.ArgumentException("Illegal base64url string!", arg);
            IL_7D:
            return System.Convert.FromBase64String(text);
        }

        public static string Decode(string arg)
        {
            return Base64UrlEncoder.TextEncoding.GetString(Base64UrlEncoder.DecodeBytes(arg));
        }
    }
}
