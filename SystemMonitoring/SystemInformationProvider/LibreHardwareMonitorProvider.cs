using System;
using System.IO;
using System.Linq;
using System.Text;

using LibreHardwareMonitor.Hardware;

namespace App
{
	public class LibreHardwareMonitorProvider : SystemInformationProvider
	{
		private readonly Computer _computer;

		public LibreHardwareMonitorProvider()
		{
			_computer = new Computer() { IsCpuEnabled = true, IsGpuEnabled = true, IsMemoryEnabled = true };
			_computer.Open();
			_computer.Accept(new UpdateVisitor());
		}

		public override void SaveReport(string path)
		{
			SystemDataInformation systemData = GetSystemInformation();

			StringBuilder report = new StringBuilder();
			report.AppendLine("|-----------------------");
			report.AppendLine("| System Information Report");
			report.AppendLine("|");
			report.AppendLine("| Used library: LibreHardwareMonitor");
			report.AppendLine("|-----------------------");
			report.AppendLine();

			report.AppendLine($"{"CPU Temperature:",16} {systemData.cpuTemperature}°C");
			report.AppendLine($"{"CPU Load:",16} {systemData.cpuLoad}%");
			report.AppendLine();

			report.AppendLine($"{"GPU Temperature:",16} {systemData.gpuTemperature}°C");
			report.AppendLine($"{"GPU Load:",16} {systemData.gpuLoad}%");
			report.AppendLine();

			report.AppendLine($"{"RAM Load:",16} {systemData.ramLoad}%");
			report.AppendLine($"{"RAM Available:",16} {systemData.ramAvailable} GB");
			report.AppendLine($"{"RAM Used:",16} {systemData.ramUsed} GB");
			report.AppendLine();

			foreach (IHardware hardware in _computer.Hardware)
			{
				hardware.Update();

				report.AppendLine();
				report.AppendLine();
				report.AppendLine("/----------");
				report.AppendLine($"| {hardware.HardwareType,9}");

				var sensorGroups = hardware.Sensors.GroupBy(s => s.SensorType);
				foreach (var group in sensorGroups)
				{
					report.AppendLine($"|---------- {group.Key}");
					foreach (ISensor sensor in group)
					{
						report.AppendLine($"| {sensor.Value,9:F2} - {sensor.Name}");
					}
					report.AppendLine("|");
				}
			}

			File.WriteAllText(path, report.ToString());
			System.Diagnostics.Process.Start(path);
		}

		public override SystemDataInformation GetSystemInformation()
		{
			float cpuTemperature = 0;
			float cpuLoad = 0;

			byte gpuTemperature = 0;
			byte gpuLoad = 0;

			float ramLoad = 0;
			float ramAvailable = 0;
			float ramUsed = 0;

			foreach (IHardware hardware in _computer.Hardware)
			{
				hardware.Update();

				if (hardware.HardwareType == HardwareType.Cpu)
				{
					cpuTemperature = GetSensorValue(hardware, SensorType.Temperature, "Core (Tctl/Tdie)");
					cpuLoad = GetSensorValue(hardware, SensorType.Load, "CPU Total");
				}
				else if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuIntel)
				{
					gpuTemperature = Convert.ToByte(GetSensorValue(hardware, SensorType.Temperature, "GPU Core"));
					gpuLoad = Convert.ToByte(GetSensorValue(hardware, SensorType.Load, "GPU Core"));
				}
				else if (hardware.HardwareType == HardwareType.Memory)
				{
					ramUsed = GetSensorValue(hardware, SensorType.Data, "Memory Used");
					ramAvailable = GetSensorValue(hardware, SensorType.Data, "Memory Available");
					ramLoad = GetSensorValue(hardware, SensorType.Load);
				}
			}

			return new SystemDataInformation(cpuTemperature, cpuLoad, gpuTemperature, gpuLoad, ramLoad, ramAvailable, ramUsed);
		}

		private float GetSensorValue(IHardware hardware, SensorType sensorType, string sensorName = null)
		{
			ISensor sensor = hardware.Sensors.FirstOrDefault(s => s.SensorType == sensorType && (sensorName == null || s.Name.Contains(sensorName)));
			return sensor != null ? (float) sensor.Value : 0;
		}

		public void Close()
		{
			_computer.Close();
		}
	}
}
