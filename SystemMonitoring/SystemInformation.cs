using System;
using System.Linq;

using OpenHardwareMonitor.Hardware;

namespace App
{
	public readonly struct SystemDataInformation(float cpuTemperature, float cpuLoad, byte gpuTemperature, byte gpuLoad, float ramLoad, float ramAvailable, float ramUsed)
	{
		///<summary>In Celsius</summary>
		public readonly float cpuTemperature = cpuTemperature;
		///<summary>In percent</summary>
		public readonly float cpuLoad = cpuLoad;

		///<summary>In Celsius</summary>
		public readonly byte gpuTemperature = gpuTemperature;
		///<summary>In percent</summary>
		public readonly byte gpuLoad = gpuLoad;

		///<summary>In percent</summary>
		public readonly float ramLoad = ramLoad;
		///<summary>In GB</summary>
		public readonly float ramAvailable = ramAvailable;
		///<summary>In GB</summary>
		public readonly float ramUsed = ramUsed;
	}

	public static class SystemInformation
	{
		private static readonly Computer computer;

		static SystemInformation()
		{
			computer = new Computer() { CPUEnabled = true, GPUEnabled = true, RAMEnabled = true };
			computer.Open();
		}

		public static void Close()
		{
			computer.Close();
		}

		public static string GetReport()
		{
			return computer.GetReport();
		}

		public static SystemDataInformation GetSystemInformation()
		{
			float cpuTemperature = 0;
			float cpuLoad = 0;

			byte gpuTemperature = 0;
			byte gpuLoad = 0;

			float ramLoad = 0;
			float ramAvailable = 0;
			float ramUsed = 0;

			foreach (IHardware hardware in computer.Hardware)
			{
				if (hardware.HardwareType == HardwareType.CPU)
				{
					hardware.Update();

					cpuTemperature = GetSensorValue(hardware, SensorType.Temperature, "CPU Package");
					cpuLoad = GetSensorValue(hardware, SensorType.Load, "CPU Total");
				}
				else if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti)
				{
					hardware.Update();

					gpuTemperature = Convert.ToByte(GetSensorValue(hardware, SensorType.Temperature));
					gpuLoad = Convert.ToByte(GetSensorValue(hardware, SensorType.Load, "GPU Core"));
				}
				else if (hardware.HardwareType == HardwareType.RAM)
				{
					hardware.Update();

					ramUsed = GetSensorValue(hardware, SensorType.Data, "Used Memory");
					ramAvailable = GetSensorValue(hardware, SensorType.Data, "Available Memory");
					ramLoad = GetSensorValue(hardware, SensorType.Load);
				}
			}

			return new SystemDataInformation(cpuTemperature, cpuLoad, gpuTemperature, gpuLoad, ramLoad, ramAvailable, ramUsed);
		}

		private static float GetSensorValue(IHardware hardware, SensorType sensorType, string sensorName = null)
		{
			ISensor sensor = null;

			if (sensorName != null)
			{
				sensor = hardware.Sensors.FirstOrDefault(s => s.SensorType == sensorType && s.Name == sensorName);
			}
			else
			{
				sensor = hardware.Sensors.FirstOrDefault(s => s.SensorType == sensorType);
			}

			return sensor != null ? (float) sensor.Value : 0;
		}
	}
}