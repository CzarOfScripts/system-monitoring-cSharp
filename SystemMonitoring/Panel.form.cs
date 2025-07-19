using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using NLog;

namespace App
{
	public partial class PanelForm : Form
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private bool isDragging = false;
		private Point startPoint = Point.Empty;
		private readonly Config config = new Config();
		private readonly InactiveAnalyzer analyzer = new InactiveAnalyzer();

		private ContextMenuStrip contextMenuStrip;
		private ToolStripMenuItem isHideFromAltTabItem;
		private NotifyIcon trayIcon;

		private System.Threading.Timer updateKeyboardLayoutTimer;
		private System.Threading.Timer updateSystemInformation;

		public PanelForm()
		{
			try
			{
				InitializeComponent();

				try
				{
									SystemInformation.SetProvider(config.data.UseLibrary);
			}
			catch (Exception ex)
			{
				logger.Warn(ex, "Error setting provider {Provider}: {Message}", config.data.UseLibrary, ex.Message);
				config.data.UseLibrary = SystemInformationProviderType.LibreHardwareMonitor;
				config.Save();

				try
				{
					SystemInformation.SetProvider(config.data.UseLibrary);
				}
				catch (Exception ex2)
				{
					logger.Error(ex2, "Critical error setting fallback provider: {Message}", ex2.Message);
					MessageBox.Show("Failed to initialize system monitoring. Application will be closed.",
						"Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					throw;
				}
			}
		}
		catch (Exception ex)
		{
			logger.Fatal(ex, "Critical error in PanelForm constructor: {Message}", ex.Message);
			throw;
		}

			analyzer.ShutdownMethod = config.data.ShutdownMethod;
			analyzer.IsForceShutdown = config.data.IsForceShutdown;
			analyzer.IdleMinutes = config.data.IdleMinutes;
			analyzer.InactiveDetected += () =>
			{
				trayIcon.ShowBalloonTip(
					5000,
					"System Monitoring",
					"[Inactive analyzer] has noticed inactivity and will shut down your computer in 5 minutes",
					ToolTipIcon.Info
				);
			};

			ShowInTaskbar = config.data.IsShowInTaskbar;
			TopMost = config.data.AlwaysOnTop;
			Opacity = config.data.Opacity / 100.0;

			if (config.data.IsMoved)
			{
				StartPosition = FormStartPosition.Manual;
				Location = new Point(config.data.PositionX, config.data.PositionY);
			}
			else
			{
				StartPosition = FormStartPosition.CenterScreen;
			}

			if (config.data.IsShowInTaskbar == true)
			{
				isHideFromAltTabItem.Enabled = false;
			}

			CreateContextMenu();
			CreateTrayMenu();
		}

		private void PanelForm_Load(object sender, EventArgs e)
		{
			try
			{
				updateKeyboardLayoutTimer = new System.Threading.Timer(new TimerCallback(UpdateKeyboardLayout), null, 0, 50);
				updateSystemInformation = new System.Threading.Timer(new TimerCallback(UpdateSystemInformation), null, 0, config.data.Interval);
				UpdateSystemInformation(null);

				if (config.data.IsHideAltTab && config.data.IsShowInTaskbar == false)
				{
					Utilities.SetAltTabVisibility(Handle, true);
				}
			}
						catch (Exception ex)
			{
				logger.Error(ex, "Error in PanelForm_Load: {Message}", ex.Message);
				MessageBox.Show($"Error loading form: {ex.Message}",
					"Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void PanelForm_ControlAdded(object sender, ControlEventArgs e)
		{
			try
			{
				e.Control.MouseDown += PanelForm_MouseDown;
				e.Control.MouseMove += PanelForm_MouseMove;
				e.Control.MouseUp += PanelForm_MouseUp;
				e.Control.MouseClick += PanelForm_MouseClick;
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error adding event handlers: {Message}", ex.Message);
			}
		}

		private void PanelForm_ControlRemoved(object sender, ControlEventArgs e)
		{
			try
			{
				e.Control.MouseDown -= PanelForm_MouseDown;
				e.Control.MouseMove -= PanelForm_MouseMove;
				e.Control.MouseUp -= PanelForm_MouseUp;
				e.Control.MouseClick -= PanelForm_MouseClick;
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error removing event handlers: {Message}", ex.Message);
			}
		}

		private void UpdateKeyboardLayout(object obj)
		{
			try
			{
				string currentLang = Keyboard.GetLayoutNameFromIds(Keyboard.GetKeyboardLayoutId());

				if (currentLang != ThreadHelper.GetLabelText(this, KeyboardLayout))
				{
					ThreadHelper.SetLabelText(this, KeyboardLayout, currentLang);
				}
			}
			catch (Exception ex)
			{
				// Log error but don't let application crash
				logger.Error(ex, "Error in UpdateKeyboardLayout: {Message}", ex.Message);
			}
		}

		private void UpdateSystemInformation(object obj)
		{
			try
			{
				SystemDataInformation data = SystemInformation.GetSystemInformation();

				//GPU
				ThreadHelper.SetLabelText(this, GpuLoad, Convert.ToString(data.gpuLoad) + (data.gpuLoad < 100 ? " " : "") + "%");
				ThreadHelper.SetControlForeColor(this, GpuLoad, Utilities.HexToColor(config.GetColor(GetColorType.LOAD, GetColorDevice.GPU, data.gpuLoad)));

				ThreadHelper.SetLabelText(this, GpuTemp, Convert.ToString(data.gpuTemperature) + "°C");
				ThreadHelper.SetControlForeColor(this, GpuTemp, Utilities.HexToColor(config.GetColor(GetColorType.TEMPERATURES, GetColorDevice.GPU, data.gpuTemperature)));

				//CPU
				ThreadHelper.SetLabelText(this, CpuLoad, Convert.ToString((int) data.cpuLoad) + (data.cpuLoad < 100 ? " " : "") + "%");
				ThreadHelper.SetControlForeColor(this, CpuLoad, Utilities.HexToColor(config.GetColor(GetColorType.LOAD, GetColorDevice.CPU, (int) data.cpuLoad)));

				ThreadHelper.SetLabelText(this, CpuTemp, Convert.ToString((int) data.cpuTemperature) + "°C");
				ThreadHelper.SetControlForeColor(this, CpuTemp, Utilities.HexToColor(config.GetColor(GetColorType.TEMPERATURES, GetColorDevice.CPU, (int) data.cpuTemperature)));

				//RAM
				ThreadHelper.SetLabelText(this, RamLoad, string.Format("{0:0.00}", data.ramLoad) + " % ");
				ThreadHelper.SetControlForeColor(this, RamLoad, Utilities.HexToColor(config.GetColor(GetColorType.LOAD, GetColorDevice.RAM, (int) data.ramLoad)));

				ThreadHelper.SetLabelText(this, RamFree, string.Format("{0:0.00}", data.ramAvailable) + " GB");

				//Uptime
				TimeSpan uptimeTime = TimeSpan.FromMilliseconds(Utilities.GetTickCount64());

				if (uptimeTime.Days > 0)
				{
					ThreadHelper.SetLabelText(this, Uptime, $"{uptimeTime.Days}d {uptimeTime.Hours:D2}h");
				}
				else if (uptimeTime.Hours > 0)
				{
					ThreadHelper.SetLabelText(this, Uptime, $"{uptimeTime.Hours:D2}h {uptimeTime.Minutes:D2}m");
				}
				else
				{
					ThreadHelper.SetLabelText(this, Uptime, $"{uptimeTime.Minutes:D2}m {uptimeTime.Seconds:D2}s");
				}

				// Check inactive
				analyzer.Add(data, Utilities.GetLastInputTime());
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in UpdateSystemInformation: {Message}", ex.Message);
			}
		}

		private void PanelForm_MouseClick(object sender, MouseEventArgs e)
		{
			try
			{
				if (e.Button != MouseButtons.Right)
				{
					return;
				}

				contextMenuStrip.Show(this, PointToClient(Cursor.Position));
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in PanelForm_MouseClick: {Message}", ex.Message);
			}
		}

		private void PanelForm_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if (config.data.AllowMove == false)
				{
					return;
				}

				if (e.Button == MouseButtons.Left)
				{
					isDragging = true;
					startPoint = new Point(e.X, e.Y);
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in PanelForm_MouseDown: {Message}", ex.Message);
			}
		}

		private void PanelForm_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if (config.data.AllowMove == false)
				{
					return;
				}

				if (!isDragging)
				{
					return;
				}

				int deltaX = e.X - startPoint.X;
				int deltaY = e.Y - startPoint.Y;
				Location = new Point(Location.X + deltaX, Location.Y + deltaY);

				config.data.PositionX = Location.X;
				config.data.PositionY = Location.Y;
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in PanelForm_MouseMove: {Message}", ex.Message);
			}
		}

		private void PanelForm_MouseUp(object sender, MouseEventArgs e)
		{
			try
			{
				if (config.data.AllowMove == false)
				{
					return;
				}

				if (isDragging && e.Button == MouseButtons.Left)
				{
					isDragging = false;
					startPoint = Point.Empty;

					config.data.IsMoved = true;
					config.Save();
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error in PanelForm_MouseUp: {Message}", ex.Message);
			}
		}

		private void PanelForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				updateKeyboardLayoutTimer?.Dispose();
				updateSystemInformation?.Dispose();
				SystemInformation.Close();
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Error closing form: {Message}", ex.Message);
			}
		}

		private void CreateContextMenu()
		{
			ContextMenuStrip menuStrip = new ContextMenuStrip();
			menuStrip.Font = new Font("Consolas", 10, FontStyle.Regular);

			AddMenuItem(menuStrip, "Add to Startup", StartUp.IsStartUp(), OnShowAddToStartUpClick);
			AddMenuItem(menuStrip, "Show in taskbar", config.data.IsShowInTaskbar, OnShowInTaskbarClick);
			isHideFromAltTabItem = AddMenuItem(menuStrip, "Hide from Alt+Tab", config.data.IsHideAltTab, OnIsHideAltTabClick);
			AddMenuItem(menuStrip, "Allow move", config.data.AllowMove, OnAllowMoveClick);
			AddMenuItem(menuStrip, "Always On Top", config.data.AlwaysOnTop, OnAlwaysOnTopClick);

			menuStrip.Items.Add(new ToolStripSeparator());

			menuStrip.Items.Add(CreateUseLibraryMenu());
			AddMenuItem(menuStrip, "Save report", false, (object sender, EventArgs e) => SystemInformation.SaveReport("report.txt"));

			menuStrip.Items.Add(new ToolStripSeparator());

			menuStrip.Items.Add(CreateOpacityMenu());
			menuStrip.Items.Add(CreateIntervalMenu());
			menuStrip.Items.Add(CreateIdleMinutesMenu());
			menuStrip.Items.Add(CreateShutdownMethodMenu());

			menuStrip.Items.Add(new ToolStripSeparator());

			menuStrip.Items.Add(
				new ToolStripMenuItem(
					"Close",
					Utilities.GetShell32Icon(131)?.ToBitmap(),
					OnCloseClick
				)
			);

			contextMenuStrip = menuStrip;
		}

		private void CreateTrayMenu()
		{
			trayIcon = new NotifyIcon()
			{
				Icon = Icon,
				Visible = true,
				ContextMenuStrip = contextMenuStrip,
				Text = "System Monitoring"
			};
		}

		private ToolStripMenuItem CreateUseLibraryMenu()
		{
			ToolStripMenuItem intervalItems = new ToolStripMenuItem("Use Library");

			SystemInformationProviderType[] libraries = { SystemInformationProviderType.LibreHardwareMonitor, SystemInformationProviderType.OpenHardwareMonitor };

			foreach (SystemInformationProviderType library in libraries)
			{
				ToolStripMenuItem item = new ToolStripMenuItem()
				{
					Text = Enum.GetName(typeof(SystemInformationProviderType), library),
					Checked = config.data.UseLibrary == library
				};
				item.Click += OnUseLibraryItemClick;
				intervalItems.DropDownItems.Add(item);
			}

			return intervalItems;
		}

		private ToolStripMenuItem CreateShutdownMethodMenu()
		{
			ToolStripMenuItem items = new ToolStripMenuItem("Shutdown method");

			ShutdownMethod[] methods = { ShutdownMethod.Shutdown, ShutdownMethod.Hibernation, ShutdownMethod.Sleep, ShutdownMethod.Restart, ShutdownMethod.Logout };

			foreach (ShutdownMethod method in methods)
			{
				ToolStripMenuItem newItem = new ToolStripMenuItem()
				{
					Text = method.ToString(),
					Checked = config.data.ShutdownMethod == method
				};
				newItem.Click += OnShutdownMethodItemClick;
				items.DropDownItems.Add(newItem);
			}

			items.DropDownItems.Add(new ToolStripSeparator());

			ToolStripMenuItem forceItem = new ToolStripMenuItem()
			{
				Text = "Force",
				Checked = config.data.IsForceShutdown
			};
			forceItem.Click += OnForceShutdownItemClick;
			items.DropDownItems.Add(forceItem);

			return items;
		}

		private ToolStripMenuItem CreateIntervalMenu()
		{
			ToolStripMenuItem intervalItems = new ToolStripMenuItem("Interval");

			int[] intervals = { 50, 150, 250, 500, 750, 1000 };

			foreach (int interval in intervals)
			{
				ToolStripMenuItem item = new ToolStripMenuItem()
				{
					Text = interval + "ms",
					Checked = config.data.Interval == interval
				};
				item.Click += OnIntervalItemClick;
				intervalItems.DropDownItems.Add(item);
			}

			return intervalItems;
		}

		private ToolStripMenuItem CreateOpacityMenu()
		{
			ToolStripMenuItem opacityItems = new ToolStripMenuItem("Opacity");

			for (byte i = 100; i > 0; i -= 10)
			{
				ToolStripMenuItem item = new ToolStripMenuItem()
				{
					Text = i + "%",
					Checked = config.data.Opacity == i
				};
				item.Click += OnOpacityItemClick;
				opacityItems.DropDownItems.Add(item);
			}

			return opacityItems;
		}

		private ToolStripMenuItem CreateIdleMinutesMenu()
		{
			ToolStripMenuItem idleMinutesItems = new ToolStripMenuItem("Idle Minutes");

			byte i = 0;

			while (i <= 120)
			{
				ToolStripMenuItem item = new ToolStripMenuItem()
				{
					Text = (i == 0 ? "Off" : i + " min"),
					Tag = i,
					Checked = config.data.IdleMinutes == i
				};
				item.Click += OnIdleMinutesItemClick;
				idleMinutesItems.DropDownItems.Add(item);

				if (i % 60 == 0)
				{
					idleMinutesItems.DropDownItems.Add(new ToolStripSeparator());
				}

				i += (byte) (i < 60 ? 5 : 10);
			}

			return idleMinutesItems;
		}

		private ToolStripMenuItem AddMenuItem(ContextMenuStrip menuStrip, string text, bool isChecked, EventHandler clickEvent)
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem()
			{
				Text = text,
				Checked = isChecked,
				CheckOnClick = true
			};
			menuItem.CheckedChanged += clickEvent;
			menuStrip.Items.Add(menuItem);

			return menuItem;
		}

		private void OnShowAddToStartUpClick(object sender, EventArgs e)
		{
			StartUp.SetIsStartUp(((ToolStripMenuItem) sender).Checked);
		}

		private void OnShutdownMethodItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem selectedItem = (ToolStripMenuItem) sender;

			try
			{
				ShutdownMethod method = (ShutdownMethod) Enum.Parse(typeof(ShutdownMethod), selectedItem.Text);

				foreach (var item in selectedItem.GetCurrentParent().Items)
				{
					if (item is ToolStripMenuItem menuItem)
					{
						if (menuItem.Text != "Force")
						{
							menuItem.Checked = (menuItem == selectedItem);
						}
					}
				}

				config.data.ShutdownMethod = method;
				config.Save();

				analyzer.ShutdownMethod = config.data.ShutdownMethod;
			}
			catch
			{
				return;
			}
		}

		private void OnForceShutdownItemClick(object sender, EventArgs e)
		{
			// Why do we have to do this here?
			ToolStripMenuItem item = ((ToolStripMenuItem) sender);
			item.Checked = !item.Checked;

			config.data.IsForceShutdown = item.Checked;
			config.Save();

			analyzer.IsForceShutdown = config.data.IsForceShutdown;
		}

		private void OnShowInTaskbarClick(object sender, EventArgs e)
		{
			bool isChecked = ((ToolStripMenuItem) sender).Checked;
			config.data.IsShowInTaskbar = isChecked;
			config.Save();
			ShowInTaskbar = isChecked;

			isHideFromAltTabItem.Enabled = !isChecked;

			if (isChecked == false && config.data.IsHideAltTab == true)
			{
				Utilities.SetAltTabVisibility(Handle, true);
			}
		}

		private void OnIsHideAltTabClick(object sender, EventArgs e)
		{
			bool isChecked = ((ToolStripMenuItem) sender).Checked;
			config.data.IsHideAltTab = isChecked;
			config.Save();
			Utilities.SetAltTabVisibility(Handle, isChecked);
		}

		private void OnAllowMoveClick(object sender, EventArgs e)
		{
			bool isChecked = ((ToolStripMenuItem) sender).Checked;
			config.data.AllowMove = isChecked;
			config.Save();
		}

		private void OnAlwaysOnTopClick(object sender, EventArgs e)
		{
			bool isChecked = ((ToolStripMenuItem) sender).Checked;
			config.data.AlwaysOnTop = isChecked;
			config.Save();
			TopMost = isChecked;
		}

		private void OnIntervalItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem selectedIntervalItem = (ToolStripMenuItem) sender;
			int selectedInterval = int.Parse(selectedIntervalItem.Text.Replace("ms", ""));

			foreach (ToolStripMenuItem intervalItem in selectedIntervalItem.GetCurrentParent().Items)
			{
				intervalItem.Checked = (intervalItem == selectedIntervalItem);
			}

			config.data.Interval = selectedInterval;
			config.Save();
			updateSystemInformation.Change(0, config.data.Interval);
		}

		private void OnOpacityItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem selectedOpacityItem = (ToolStripMenuItem) sender;
			byte selectedOpacity = byte.Parse(selectedOpacityItem.Text.Replace("%", ""));

			foreach (ToolStripMenuItem intervalItem in selectedOpacityItem.GetCurrentParent().Items)
			{
				intervalItem.Checked = (intervalItem == selectedOpacityItem);
			}

			config.data.Opacity = selectedOpacity;
			config.Save();
			Opacity = config.data.Opacity / 100.0;

			if (config.data.IsHideAltTab == true)
			{
				Utilities.SetAltTabVisibility(Handle, true);
			}
		}

		private void OnIdleMinutesItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem selectedItem = (ToolStripMenuItem) sender;

			foreach (var idleMinutesItem in selectedItem.GetCurrentParent().Items)
			{
				if (idleMinutesItem is ToolStripMenuItem menuItem)
				{
					menuItem.Checked = (menuItem == selectedItem);
				}
			}

			config.data.IdleMinutes = (byte) selectedItem.Tag;
			config.Save();

			analyzer.IdleMinutes = config.data.IdleMinutes;
		}

		private void OnUseLibraryItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem selectedItem = (ToolStripMenuItem) sender;

			foreach (ToolStripMenuItem item in selectedItem.GetCurrentParent().Items)
			{
				item.Checked = (item == selectedItem);
			}

			try
			{
				config.data.UseLibrary = (SystemInformationProviderType) Enum.Parse(typeof(SystemInformationProviderType), selectedItem.Text);
			}
			catch
			{
				config.data.UseLibrary = SystemInformationProviderType.LibreHardwareMonitor;
			}

			config.Save();

			SystemInformation.SetProvider(config.data.UseLibrary);
		}

		private void OnCloseClick(object sender, EventArgs e)
		{
			Close();
		}
	}
}
