using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        public static string HandleEnvironmentToken(this string input)
        {
            var _returnValue = input;

            Regex r = new Regex(@"(?:(?<=\().+?(?=\))|(?<=\[).+?(?=\]))");
            Regex r1 = new Regex(@"\[(.*?)\]");

            Match _outPut = r.Match(_returnValue);
            if (_outPut.Success)
            {
                var _envPath = Environment.GetEnvironmentVariable(_outPut.Value);
                if(_envPath == null)
                {
                   //This means that the environment variable doesnt exist throw exception
                    var _message = string.Format("Environment Variable {0} does not exist in input value {1}. Please check your configuration files.", _outPut.Value, input);
                    throw new ConfigurationErrorsException(_message);
                }
                _returnValue = r1.Replace(_returnValue, _envPath);
            }
            return _returnValue;
        }
        public static string UrlNameFromString(this string title, int maxlength = 255)
        {
            Regex nonstd = new Regex(@"[^a-zA-Z0-9\s]");
            Regex whites = new Regex(@"\s+");
            Regex dashes = new Regex(@"^[-]|[-]+$");
            string s = RemoveDiacritics(title);
            s = nonstd.Replace(s, "");
            s = whites.Replace(s, "-");
            if (s.Length > maxlength)
            {
                s = s.Substring(0, maxlength);
            }
            s = dashes.Replace(s, "");
            return s.ToLower();
        }

        public static string RemoveDiacritics(this string s)
        {
            string d = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < d.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(d[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(d[i]);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
