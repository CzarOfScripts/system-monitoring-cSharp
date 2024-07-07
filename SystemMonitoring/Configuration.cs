using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Newtonsoft.Json;

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
		public ConfigData data = new ConfigData();
		public Action OnDataChanged;

		public Config()
		{
			Load();
		}

		public void Save()
		{
			File.WriteAllText("settings.json", JsonConvert.SerializeObject(data, Formatting.Indented));
			OnDataChanged?.Invoke();
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

		private void Load()
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
		}
	}

	public class ConfigData
	{
		public bool IsShowInTaskbar { get; set; } = true;
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