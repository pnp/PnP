using System;
using System.Collections.Generic;
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
                _returnValue = r1.Replace(_returnValue, _envPath);
            }
            return _returnValue;
        }
    }
}
