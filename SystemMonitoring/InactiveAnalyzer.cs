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

	public class InactiveAnalyzer
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private Timer shutdownTimer = null;
		private readonly List<ActivityData> activityData = new List<ActivityData>();

		public bool IsInactive { get; private set; } = false;
		public int IdleMinutes { get; set; } = 45;
		public float LowGpuLoad { get; set; } = 2.0f;
		public float LowCpuLoad { get; set; } = 5.0f;

		public event Action InactiveDetected;

		public void Add(SystemDataInformation systemDataInformation, DateTime lastInputTime)
		{
			activityData.Add(new ActivityData(systemDataInformation, lastInputTime));

			while (activityData.Count > 60)
			{
				activityData.RemoveAt(0);
			}

			if (activityData.Count == 1)
			{
				return;
			}

			if (IdleMinutes > 0)
			{
				Check();
			}
		}

		public void StopTimer()
		{
			shutdownTimer?.Dispose();
			shutdownTimer = null;
		}

		private void Check()
		{
			int lastInputInMinutes = (int) (DateTime.Now - activityData[activityData.Count - 1].lastInputTime).TotalMinutes;
			float averageCpuLoad = activityData.Average((data) => data.systemDataInformation.cpuLoad);
			float averageGpuLoad = (float) activityData.Average((data) => data.systemDataInformation.gpuLoad);

			if (lastInputInMinutes > IdleMinutes && averageCpuLoad <= LowCpuLoad && averageGpuLoad <= LowGpuLoad)
			{
				if (IsInactive == true)
				{
					return;
				}

				IsInactive = true;

				InactiveDetected?.Invoke();
				StopTimer();

				shutdownTimer = new Timer(
					new TimerCallback((object obj) =>
					{
						try
						{
							Process.Start("shutdown", "/s /t 0");
						}
						catch (Exception ex)
						{
							logger.Error(ex, "Process.Start error");
						}
					}),
					null,
					5 * 60 * 1000,
					Timeout.Infinite
				);
			}
			else if (IsInactive == true)
			{
				IsInactive = false;
				StopTimer();
			}
		}
	}
}
