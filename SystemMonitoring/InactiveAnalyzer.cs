using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using NLog;

namespace App
{
	public readonly struct ActivityData(SystemDataInformation systemDataInformation, DateTime lastInputTime)
	{
		public readonly SystemDataInformation systemDataInformation = systemDataInformation;
		public readonly DateTime lastInputTime = lastInputTime;
	}

	public class InactiveAnalyzer : IDisposable
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private readonly object _lock = new object();

		private const int MAX_ACTIVITY_DATA_COUNT = 60;
		private const int SHUTDOWN_DELAY_MINUTES = 5;

		private Timer shutdownTimer = null;
		private readonly List<ActivityData> activityData = new List<ActivityData>();
		private bool _disposed = false;

		public bool IsInactive { get; private set; } = false;
		public byte IdleMinutes { get; set; } = 45;
		public float LowGpuLoad { get; set; } = 2.0f;
		public float LowCpuLoad { get; set; } = 5.0f;
		public ShutdownMethod ShutdownMethod { get; set; } = ShutdownMethod.Shutdown;
		public bool IsForceShutdown { get; set; } = false;

		public event Action InactiveDetected;

		public void Add(SystemDataInformation systemDataInformation, DateTime lastInputTime)
		{
			if (_disposed)
			{
				logger.Warn("Attempt to add data to already disposed InactiveAnalyzer");
				return;
			}

			try
			{
				bool shouldCheck = false;

				lock (_lock)
				{
					activityData.Add(new ActivityData(systemDataInformation, lastInputTime));

					while (activityData.Count > MAX_ACTIVITY_DATA_COUNT)
					{
						activityData.RemoveAt(0);
					}

					if (activityData.Count == 1)
					{
						return;
					}

					if (IdleMinutes > 0)
					{
						shouldCheck = true;
					}
				}

				if (shouldCheck)
				{
					Check();
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in InactiveAnalyzer.Add: {Message}", ex.Message);
			}
		}

		public void StopTimer()
		{
			try
			{
				shutdownTimer?.Dispose();
				shutdownTimer = null;
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error stopping timer: {Message}", ex.Message);
			}
		}

		private void Check()
		{
			try
			{
				bool shouldCreateTimer = false;
				bool shouldStopTimer = false;
				bool wasInactive = IsInactive;

				lock (_lock)
				{
					if (activityData.Count == 0)
					{
						return;
					}

					int lastInputInMinutes = (int)(DateTime.Now - activityData[activityData.Count - 1].lastInputTime).TotalMinutes;

					var (averageCpuLoad, averageGpuLoad) = CalculateAverages();

					if (lastInputInMinutes > IdleMinutes && averageCpuLoad <= LowCpuLoad && averageGpuLoad <= LowGpuLoad)
					{
						if (!IsInactive)
						{
							IsInactive = true;
							shouldCreateTimer = true;
						}
					}
					else if (IsInactive)
					{
						IsInactive = false;
						shouldStopTimer = true;
					}
				}

				if (shouldCreateTimer)
				{
					InactiveDetected?.Invoke();
					StopTimer();
					CreateShutdownTimer();
				}
				else if (shouldStopTimer)
				{
					StopTimer();
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in InactiveAnalyzer.Check: {Message}", ex.Message);
			}
		}

		private (float cpuLoad, float gpuLoad) CalculateAverages()
		{
			float totalCpuLoad = 0;
			float totalGpuLoad = 0;
			int count = activityData.Count;

			foreach (var data in activityData)
			{
				totalCpuLoad += data.systemDataInformation.cpuLoad;
				totalGpuLoad += data.systemDataInformation.gpuLoad;
			}

			return (totalCpuLoad / count, totalGpuLoad / count);
		}

		private void CreateShutdownTimer()
		{
			try
			{
				shutdownTimer = new Timer(
					new TimerCallback((object obj) =>
					{
						try
						{
							ExecuteShutdownAction();
						}
						catch (Exception ex)
						{
							logger.Error(ex, "Error executing shutdown action: {Message}", ex.Message);
						}
					}),
					null,
					SHUTDOWN_DELAY_MINUTES * 60 * 1000,
					Timeout.Infinite
				);
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error creating shutdown timer: {Message}", ex.Message);
			}
		}

		private void ExecuteShutdownAction()
		{
			try
			{
				switch (ShutdownMethod)
				{
					case ShutdownMethod.Shutdown:
						Process.Start("shutdown", "/s /t 0" + (IsForceShutdown ? " /f" : ""));
						break;
					case ShutdownMethod.Hibernation:
						Process.Start("shutdown", "/h" + (IsForceShutdown ? " /f" : ""));
						break;
					case ShutdownMethod.Sleep:
						Utilities.SetSuspendState(false, true, false);
						break;
					case ShutdownMethod.Restart:
						Process.Start("shutdown", "/r /t 0" + (IsForceShutdown ? " /f" : ""));
						break;
					case ShutdownMethod.Logout:
						Process.Start("shutdown", "/l");
						break;
					default:
						logger.Warn("Unknown shutdown method: {ShutdownMethod}", ShutdownMethod);
						break;
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error executing shutdown command: {Message}", ex.Message);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				StopTimer();
				_disposed = true;
			}
		}
	}
}
