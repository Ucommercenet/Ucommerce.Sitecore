using System;
using System.Text;

namespace Ucommerce.Sitecore.Installer.Steps
{
	/// <summary>
	/// Extensions for Guids.
	/// </summary>
	public static class GuidExtensions
	{
		/// <summary>
		/// Derives a new different Guid from an existing one.
		/// </summary>
		/// <param name="this">The Guid to derive a new Guid from.</param>
		/// <param name="key">A key used for deriving the Guid.</param>
		/// <returns>The derived Guid.</returns>
		public static Guid Derived(this Guid @this, string key)
		{
			var bytes = @this.ToByteArray();

			RotateBytes(bytes);
			MixInKeyBytes(key, bytes);

			var derived = new Guid(bytes);
			return derived;
		}

		private static void MixInKeyBytes(string key, byte[] bytes)
		{
			var keyBytes = Encoding.UTF8.GetBytes(key);

			int byteIndex = 0;
			for (int i = 0; i < keyBytes.Length; i++)
			{
				bytes[byteIndex] ^= keyBytes[i];
				byteIndex++;
				if (byteIndex >= bytes.Length)
				{
					byteIndex = 0;
				}
			}
		}

		private static void RotateBytes(byte[] bytes)
		{
			// Simply rotate the bytes. Index 0 => 1, 1 => 2, etc. 15 => 0.
			var last = bytes[15];
			for (int i = 14; i >= 0; i--)
			{
				bytes[i + 1] = bytes[i];
			}
			bytes[0] = last; // Last to be first.
		}
	}
}
