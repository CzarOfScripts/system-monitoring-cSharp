using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace App
{
	public class Utilities
	{
		[DllImport("shell32.dll")]
		private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

		[DllImport("kernel32")]
		extern public static ulong GetTickCount64();

		public static Icon GetShell32Icon(int iconIndex)
		{
			IntPtr iconHandle = ExtractIcon(IntPtr.Zero, "shell32.dll", iconIndex);

			return iconHandle == IntPtr.Zero ? null : Icon.FromHandle(iconHandle);
		}

		public static Color HexToColor(string hex)
		{
			hex = hex.Replace("#", "");

			int length = hex.Length;
			int step = length == 3 ? 1 : 2;

			if (length != 3 && length != 6)
			{
				throw new ArgumentException($"Incorrect HEX color format {hex}", nameof(hex));
			}

			int r = Convert.ToInt32(hex.Substring(0, step), 16);
			int g = Convert.ToInt32(hex.Substring(step, step), 16);
			int b = Convert.ToInt32(hex.Substring(2 * step, step), 16);

			return Color.FromArgb(r, g, b);
		}
	}
}
