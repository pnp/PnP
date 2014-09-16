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
            var targetVals = new List<char> { '#', '%', '&', '*', '{', '}', '\\', ':', '<', '>', '?', '/', '+', '|', '"' };

            targetVals.ForEach(v => Assert.IsTrue((string.Format("abc{0}abc", v).ContainsInvalidUrlChars())));
        }

        [TestMethod]
        public void StripInvalidUrlCharsReturnsStrippedString()
        {
            var invalidString = "a#%&*{}\\:<>?/+|b";

            Assert.AreEqual("ab", invalidString.StripInvalidUrlChars());
        }

        [TestMethod]
        public void ReplaceInvalidUrlCharsReturnsStrippedString()
        {
            var invalidString = "a#%&*{}\\:<>?/+|b";

            Assert.AreEqual("a------------------------------------------b", invalidString.ReplaceInvalidUrlChars("---"));
        }
    }
}
