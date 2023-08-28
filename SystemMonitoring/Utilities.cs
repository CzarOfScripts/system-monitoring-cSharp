using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace App
{
	public class Utilities
	{
		[DllImport("shell32.dll")]
		private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

		[DllImport("kernel32")]
		extern public static ulong GetTickCount64();

		private const int GWL_EXSTYLE = -20;
		private const uint WS_EX_TOOLWINDOW = 0x00000080;

		public static void SetAltTabVisibility(IntPtr hWnd, bool hide)
		{
			try
			{
				IntPtr oldStyle = IntPtr.Zero;

				if (hide)
				{
					oldStyle = SetWindowLongPtr(hWnd, GWL_EXSTYLE, (IntPtr) ((uint) GetWindowLongPtr(hWnd, GWL_EXSTYLE) | WS_EX_TOOLWINDOW));
				}
				else
				{
					oldStyle = SetWindowLongPtr(hWnd, GWL_EXSTYLE, (IntPtr) ((uint) GetWindowLongPtr(hWnd, GWL_EXSTYLE) & ~WS_EX_TOOLWINDOW));
				}

				if (oldStyle != IntPtr.Zero)
				{
					return;
				}

				int error = Marshal.GetLastWin32Error();
				string errorMessage = $"Error code: {error}";

				errorMessage = error switch
				{
					0 => "The operation completed successfully.",
					5 => "Access is denied. You may not have sufficient permissions to perform this operation.",
					87 => "The parameter is incorrect. There might be an issue with the provided parameter values.",
					1400 => "Invalid window handle. Make sure the window handle (HWND) is valid.",
					1401 => "Invalid menu handle. Make sure the menu handle is valid.",
					1402 => "Invalid cursor handle. Make sure the cursor handle is valid.",
					1403 => "Invalid accelerator table handle. Make sure the accelerator table handle is valid.",
					1404 => "Invalid hook handle. Make sure the hook handle is valid.",
					1405 => "Invalid DWP handle. Make sure the DWP handle is valid.",
					1406 => "Invalid thread ID. Make sure the thread ID is valid.",
					1407 => "Clipboard format is not valid.",
					_ => $"An unknown error occurred with code: {error}.",
				};

				MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

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
