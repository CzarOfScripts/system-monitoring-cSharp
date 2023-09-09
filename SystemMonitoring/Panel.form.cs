using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace App
{
	public partial class PanelForm : Form
	{
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
			InitializeComponent();
			CreateContextMenu();
			CreateTrayMenu();

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
		}

		private void PanelForm_Load(object sender, EventArgs e)
		{
			updateKeyboardLayoutTimer = new System.Threading.Timer(new TimerCallback(UpdateKeyboardLayout), null, 0, 50);
			updateSystemInformation = new System.Threading.Timer(new TimerCallback(UpdateSystemInformation), null, 0, config.data.Interval);
			UpdateSystemInformation(null);

			if (config.data.IsHideAltTab && config.data.IsShowInTaskbar == false)
			{
				Utilities.SetAltTabVisibility(Handle, true);
			}
		}

		private void PanelForm_ControlAdded(object sender, ControlEventArgs e)
		{
			e.Control.MouseDown += PanelForm_MouseDown;
			e.Control.MouseMove += PanelForm_MouseMove;
			e.Control.MouseUp += PanelForm_MouseUp;
			e.Control.MouseClick += PanelForm_MouseClick;
		}

		private void PanelForm_ControlRemoved(object sender, ControlEventArgs e)
		{
			e.Control.MouseDown -= PanelForm_MouseDown;
			e.Control.MouseMove -= PanelForm_MouseMove;
			e.Control.MouseUp -= PanelForm_MouseUp;
			e.Control.MouseClick -= PanelForm_MouseClick;
		}

		private void UpdateKeyboardLayout(object obj)
		{
			string currentLang = Keyboard.GetLayoutNameFromIds(Keyboard.GetKeyboardLayoutId());

			if (currentLang != ThreadHelper.GetLabelText(this, KeyboardLayout))
			{
				ThreadHelper.SetLabelText(this, KeyboardLayout, currentLang);
			}
		}

		private void UpdateSystemInformation(object obj)
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

		private void PanelForm_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}

			contextMenuStrip.Show(this, PointToClient(Cursor.Position));
		}

		private void PanelForm_MouseDown(object sender, MouseEventArgs e)
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

		private void PanelForm_MouseMove(object sender, MouseEventArgs e)
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

		private void PanelForm_MouseUp(object sender, MouseEventArgs e)
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

		private void PanelForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			updateKeyboardLayoutTimer.Dispose();
			updateSystemInformation.Dispose();
			SystemInformation.Close();
		}

		private void CreateContextMenu()
		{
			ContextMenuStrip menuStrip = new ContextMenuStrip();
			menuStrip.Font = new Font("Consolas", 10, FontStyle.Regular);

			AddMenuItem(menuStrip, "Show in taskbar", config.data.IsShowInTaskbar, OnShowInTaskbarClick);
			isHideFromAltTabItem = AddMenuItem(menuStrip, "Hide from Alt+Tab", config.data.IsHideAltTab, OnIsHideAltTabClick);
			AddMenuItem(menuStrip, "Allow move", config.data.AllowMove, OnAllowMoveClick);
			AddMenuItem(menuStrip, "Always On Top", config.data.AlwaysOnTop, OnAlwaysOnTopClick);

			menuStrip.Items.Add(new ToolStripSeparator());

			menuStrip.Items.Add(CreateOpacityMenu());
			menuStrip.Items.Add(CreateIntervalMenu());
			menuStrip.Items.Add(CreateIdleMinutesMenu());

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

		private void OnCloseClick(object sender, EventArgs e)
		{
			Close();
		}
	}
}
