using System;

namespace App
{
	public static class SystemInformationProviderFactory
	{
		public static SystemInformationProvider CreateProvider(SystemInformationProviderType type)
		{
			return type switch
			{
				SystemInformationProviderType.OpenHardwareMonitor => new OpenHardwareMonitorProvider(),
				SystemInformationProviderType.LibreHardwareMonitor => new LibreHardwareMonitorProvider(),
				_ => throw new ArgumentException($"Invalid provider type: {type}"),
			};
		}
	}
}
