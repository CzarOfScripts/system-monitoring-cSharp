using System;

namespace App
{
	public static class SystemInformation
	{
		private static SystemInformationProvider provider;

		public static void SetProvider(SystemInformationProviderType type)
		{
			provider = SystemInformationProviderFactory.CreateProvider(type);
		}

		public static void SaveReport(string path)
		{
			if (provider == null)
			{
				throw new InvalidOperationException("Provider has not been set. Call SetProvider() first.");
			}

			provider.SaveReport(path);
		}

		public static SystemDataInformation GetSystemInformation()
		{
			if (provider == null)
			{
				throw new InvalidOperationException("Provider has not been set. Call SetProvider() first.");
			}

			return provider.GetSystemInformation();
		}

		public static void Close()
		{
			if (provider != null)
			{
				if (provider is OpenHardwareMonitorProvider)
				{
					((OpenHardwareMonitorProvider) provider).Close();
				}
				else if (provider is LibreHardwareMonitorProvider)
				{
					((LibreHardwareMonitorProvider) provider).Close();
				}
				provider = null;
			}
		}
	}
}
