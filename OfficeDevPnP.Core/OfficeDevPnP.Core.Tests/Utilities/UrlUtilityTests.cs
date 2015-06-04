using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Utilities;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class UrlUtilityTests
    {
        [TestMethod]
        public void ContainsInvalidCharsReturnsFalseForValidString()
        {
            string validString = "abd-123";
            Assert.IsFalse(validString.ContainsInvalidUrlChars());
        }
	
        [TestMethod]
        public void ContainsInvalidUrlCharsReturnsTrueForInvalidString()
        {
#if !CLIENTSDKV15
            var targetVals = new List<char> { '#', '%', '*', '\\', ':', '<', '>', '?', '/', '+', '|', '"' };
#else
            var targetVals = new List<char> { '#', '~', '%', '&', '*', '{', '}', '\\', ':', '<', '>', '?', '/', '+', '|', '"' };
#endif

            targetVals.ForEach(v => Assert.IsTrue((string.Format("abc{0}abc", v).ContainsInvalidUrlChars())));
        }

        [TestMethod]
        public void StripInvalidUrlCharsReturnsStrippedString()
        {
#if !CLIENTSDKV15
            var invalidString = "a#%*\\:<>?/+|b";
#else
            var invalidString = "a#~%&*{}\\:<>?/+|b";
#endif

            Assert.AreEqual("ab", invalidString.StripInvalidUrlChars());
        }

        [TestMethod]
        public void ReplaceInvalidUrlCharsReturnsStrippedString()
        {
#if !CLIENTSDKV15
            var invalidString = "a#%*\\:<>?/+|b";
#else
            var invalidString = "a#~%&*{}\\:<>?/+|b";
#endif

            Assert.AreEqual("a------------------------------------------b", invalidString.ReplaceInvalidUrlChars("---"));
        }
    }
}
