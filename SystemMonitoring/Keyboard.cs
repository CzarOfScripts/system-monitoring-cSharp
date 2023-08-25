using System;
using System.Runtime.InteropServices;
using System.Text;

namespace App
{
	public static class Keyboard
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll")]
		private static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

		public static string GetKeyboardLayoutId()
		{
			IntPtr hWnd = GetForegroundWindow();
			GetWindowThreadProcessId(hWnd, out _);

			IntPtr hKL = GetKeyboardLayout(GetWindowThreadProcessId(hWnd, out _));
			long layoutId = hKL.ToInt64() & 0xFFFF; // Extract the low 16 bits

			var layoutIdString = $"{layoutId:X8}";

			return layoutIdString.Substring(4, 4);
		}

		public static string GetLayoutNameFromIds(string layoutId)
		{
			// Here you would implement a lookup table or logic to map the language and layout IDs to layout names
			// For demonstration purposes, I'll provide a simple example

			if (layoutId == "0409")
			{
				return "EN"; // English layout
			}
			else if (layoutId == "0419")
			{
				return "RU"; // Russian layout
			}
			// Add more mappings as needed

			return "Unknown";
		}
	}
}
