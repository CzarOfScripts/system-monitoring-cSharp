using System;
using System.Linq;

using OpenHardwareMonitor.Hardware;

namespace App
{
	public class OpenHardwareMonitorProvider : SystemInformationProvider
	{
		private readonly Computer _computer;

		public OpenHardwareMonitorProvider()
		{
			_computer = new Computer() { CPUEnabled = true, GPUEnabled = true, RAMEnabled = true };
			_computer.Open();
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

				if (hardware.HardwareType == HardwareType.CPU)
				{
					cpuTemperature = GetSensorValue(hardware, SensorType.Temperature, "CPU Package");
					cpuLoad = GetSensorValue(hardware, SensorType.Load, "CPU Total");
				}
				else if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti)
				{
					gpuTemperature = Convert.ToByte(GetSensorValue(hardware, SensorType.Temperature, "GPU Core"));
					gpuLoad = Convert.ToByte(GetSensorValue(hardware, SensorType.Load, "GPU Core"));
				}
				else if (hardware.HardwareType == HardwareType.RAM)
				{
					ramUsed = GetSensorValue(hardware, SensorType.Data, "Used Memory");
					ramAvailable = GetSensorValue(hardware, SensorType.Data, "Available Memory");
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
