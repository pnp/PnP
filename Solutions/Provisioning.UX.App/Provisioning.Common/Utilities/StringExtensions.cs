using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    }
}
