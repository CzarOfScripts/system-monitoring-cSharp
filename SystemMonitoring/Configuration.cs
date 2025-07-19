using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Newtonsoft.Json;
using NLog;

namespace App
{
	public enum GetColorType
	{
		LOAD,
		TEMPERATURES
	}

	public enum GetColorDevice
	{
		CPU,
		GPU,
		RAM
	}

	public class Config
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public ConfigData data = new ConfigData();
		public Action OnDataChanged;

		public Config()
		{
			Load();
		}

		public void Save()
		{
			try
			{
				File.WriteAllText("settings.json", JsonConvert.SerializeObject(data, Formatting.Indented));
				OnDataChanged?.Invoke();
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error saving configuration: {Message}", ex.Message);
			}
		}

		public string GetColor(GetColorType type, GetColorDevice device, int value)
		{
			string color = data.DefaultColor;

			Dictionary<string, Dictionary<string, string>> colors = type == GetColorType.LOAD ? data.LoadColor : data.TempColor;

			if (colors.TryGetValue(device.ToString(), out var deviceColors))
			{
				foreach (KeyValuePair<string, string> colorsInfo in deviceColors)
				{
					if (Convert.ToInt32(colorsInfo.Key) <= value)
					{
						color = colorsInfo.Value;
					}
				}
			}

			return color;
		}

		private void AskRestoreIncorrectConfig()
		{
			try
			{
				DialogResult result = MessageBox.Show(
						"The settings file (settings.json) is in an incorrect format. Would you like to replace the file with the default settings?\nChoosing \"No\" will result in closing the program.",
						"Error in settings file",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Error
					);

				if (result == DialogResult.Yes)
				{
					data = new ConfigData();
					Save();
				}
				else
				{
					Application.Exit();
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in AskRestoreIncorrectConfig: {Message}", ex.Message);
				data = new ConfigData();
				Save();
			}
		}

		private void Load()
		{
			try
			{
				if (File.Exists("settings.json") == false)
				{
					Save();
					return;
				}

				try
				{
					JsonConvert.PopulateObject(File.ReadAllText("settings.json"), data);
					Save();
				}
				catch (JsonSerializationException)
				{
					AskRestoreIncorrectConfig();
				}
				catch (Exception ex)
				{
					logger.Error(ex, "Error loading configuration: {Message}", ex.Message);
					AskRestoreIncorrectConfig();
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Critical error loading configuration: {Message}", ex.Message);
				data = new ConfigData();
			}
		}
	}

	public enum ShutdownMethod
	{
		Shutdown = 0,    // /s
		Hibernation = 1, // /h
		Sleep = 2,
		Restart = 3,     // /r
		Logout = 4       // /l
	}

	public class ConfigData
	{
		public bool IsShowInTaskbar { get; set; } = false;
		public bool IsHideAltTab { get; set; } = true;
		public bool IsMoved { get; set; } = false;
		public bool AlwaysOnTop { get; set; } = false;
		public bool AllowMove { get; set; } = true;
		public int PositionX { get; set; }
		public int PositionY { get; set; }
		public byte Opacity { get; set; } = 100;
		public int Interval { get; set; } = 1000;
		public byte IdleMinutes { get; set; } = 45;
		public string DefaultColor { get; set; } = "#9F9F9F";
		public SystemInformationProviderType UseLibrary { get; set; } = SystemInformationProviderType.LibreHardwareMonitor;
		public ShutdownMethod ShutdownMethod { get; set; } = ShutdownMethod.Shutdown;
		public bool IsForceShutdown { get; set; } = false;

		public Dictionary<string, Dictionary<string, string>> TempColor { get; set; } = new Dictionary<string, Dictionary<string, string>>
		{
			["CPU"] = new Dictionary<string, string>
			{
				["73"] = "#FFD700",
				["80"] = "#DC4242"
			},
			["GPU"] = new Dictionary<string, string>
			{
				["68"] = "#FFD700",
				["80"] = "#DC4242"
			}
		};

		public Dictionary<string, Dictionary<string, string>> LoadColor { get; set; } = new Dictionary<string, Dictionary<string, string>>
		{
			["CPU"] = new Dictionary<string, string>
			{
				["55"] = "#FFD700",
				["80"] = "#DC4242"
			},
			["GPU"] = new Dictionary<string, string>
			{
				["40"] = "#FFD700",
				["80"] = "#DC4242"
			},
			["RAM"] = new Dictionary<string, string>
			{
				["65"] = "#FFD700",
				["80"] = "#DC4242"
			}
		};
	}
}
