using System;
using System.Security;

namespace Provisioning.Extensibility.Providers.Extensions
{
	public static class StringExtensions
	{
		public static SecureString ToSecureString(this string Source)
		{
			if (string.IsNullOrWhiteSpace(Source)) return null;

			SecureString Result = new SecureString();
			foreach (char c in Source.ToCharArray())
			{
				Result.AppendChar(c);
			}
			return Result;
		}
	}
}