using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;
using System.Runtime.InteropServices;
namespace System.Tests
{
    [TestClass()]
    public class StringExtensionsTests
    {
        [TestMethod()]
        public void ToDelimitedStringTest()
        {
            string testString = "abc";
            Assert.AreEqual(testString.ToDelimitedString(), "a, b, c");
        }

        [TestMethod()]
        public void ToDelimitedStringTest1()
        {
            string testString = "abc";
            Assert.AreEqual(testString.ToDelimitedString(";#"), "a;#b;#c");
        }

        [TestMethod()]
        public void ToDelimitedStringTest2()
        {
            Func<char, string> putQuotesAround = delegate(char s)
            {
                return string.Format("'{0}'", s);
            };

            string testString = "abc";
            Assert.AreEqual(testString.ToDelimitedString(putQuotesAround), "'a', 'b', 'c'");
        }

        [TestMethod()]
        public void ToDelimitedStringTest3()
        {
            Func<char, string> putQuotesAround = delegate(char s)
            {
                return string.Format("'{0}'", s);
            };

            string testString = "abc";
            Assert.AreEqual(testString.ToDelimitedString(putQuotesAround, ";#"), "'a';#'b';#'c'");
        }

        [TestMethod()]
        public void SplitCsvTest()
        {
            string testString = "a,b,c";
            Assert.AreEqual(testString.SplitCsv().Count(), 3);
            Assert.AreEqual(testString.SplitCsv().ElementAt(1), "b");

        }

        [TestMethod()]
        public void StripSpecialCharactersTest()
        {
            string testString = "a&,b#,c%";
            Assert.AreEqual(testString.StripSpecialCharacters(), "abc");
        }

        [TestMethod()]
        public void StripSpecialCharactersTest1()
        {
            string testString = "a&,b#,c%";
            Assert.AreEqual(testString.StripSpecialCharacters("_"), "a_b_c_");
        }

        [TestMethod()]
        public void NormalizePageNameTest()
        {
            string pageName = "page!@with£%^&*characters.aspx";
            Assert.AreEqual(pageName.NormalizePageName(), "pagewithcharacters.aspx");
        }

        [TestMethod()]
        public void HtmlEncodeTest()
        {
            string htmlString = "?param1=åäö&param2=<testvalue>";

            Assert.AreEqual(htmlString.HtmlEncode(false), "?param1=åäö&amp;param2=&lt;testvalue&gt;");

            Assert.AreEqual(htmlString.HtmlEncode(true), "?param1=åäö&param2=&lt;testvalue&gt;");
        }

        [TestMethod()]
        public void ToSecureStringTest()
        {
            string testString = "thisisateststring";

            var secureString = new SecureString();
            foreach (char c in testString.ToCharArray())
                secureString.AppendChar(c);

            Assert.IsTrue(IsSecureStringEqualTo(secureString, testString.ToSecureString()));
        }
         

        /// <summary>
        /// http://stackoverflow.com/questions/4502676/c-sharp-compare-two-securestrings-for-equality
        /// </summary>
        /// <param name="ss1"></param>
        /// <param name="ss2"></param>
        /// <returns></returns>
        private bool IsSecureStringEqualTo(SecureString ss1, SecureString ss2)
        {
            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = Marshal.SecureStringToBSTR(ss1);
                bstr2 = Marshal.SecureStringToBSTR(ss2);
                int length1 = Marshal.ReadInt32(bstr1, -4);
                int length2 = Marshal.ReadInt32(bstr2, -4);
                if (length1 == length2)
                {
                    for (int x = 0; x < length1; ++x)
                    {
                        byte b1 = Marshal.ReadByte(bstr1, x);
                        byte b2 = Marshal.ReadByte(bstr2, x);
                        if (b1 != b2) return false;
                    }
                }
                else return false;
                return true;
            }
            finally
            {
                if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
                if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            }
        }
    }
}
