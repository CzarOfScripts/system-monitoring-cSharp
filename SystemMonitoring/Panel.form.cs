using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace App
{
	public partial class PanelForm : Form
	{
		private bool isDragging = false;
		private Point startPoint = Point.Empty;
		private ContextMenuStrip contextMenuStrip;
		private readonly Config config = new Config();

		private System.Threading.Timer updateKeyboardLayoutTimer;
		private System.Threading.Timer updateSystemInformation;

		[DllImport("shell32.dll")]
		private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

		[DllImport("kernel32")]
		extern static ulong GetTickCount64();

		public PanelForm()
		{
			InitializeComponent();
			CreateContextMenu();

			ShowInTaskbar = config.data.IsShowInTaskbar;
			TopMost = config.data.AlwaysOnTop;

			if (config.data.IsMoved)
			{
				StartPosition = FormStartPosition.Manual;
				Location = new Point(config.data.PositionX, config.data.PositionY);
			}
			else
			{
				StartPosition = FormStartPosition.CenterScreen;
			}
		}

		private void CreateContextMenu()
		{
			ContextMenuStrip menuStrip = new ContextMenuStrip();
			menuStrip.Font = new Font("Consolas", 10, FontStyle.Regular);

			// Show in taskbar
			ToolStripMenuItem showInTaskbarITem = new ToolStripMenuItem()
			{
				Text = "Show in taskbar",
				Checked = config.data.IsShowInTaskbar,
				CheckOnClick = true
			};
			showInTaskbarITem.CheckedChanged += (object sender, EventArgs e) =>
			{
				config.data.IsShowInTaskbar = ((ToolStripMenuItem) sender).Checked;
				config.Save();

				ShowInTaskbar = config.data.IsShowInTaskbar;
			};

			// Allow Move
			ToolStripMenuItem allowMoveItem = new ToolStripMenuItem()
			{
				Text = "Allow move",
				Checked = config.data.AllowMove,
				CheckOnClick = true
			};
			allowMoveItem.CheckedChanged += (object sender, EventArgs e) =>
			{
				config.data.AllowMove = ((ToolStripMenuItem) sender).Checked;
				config.Save();
			};

			// Always On Top
			ToolStripMenuItem alwaysOnTopItem = new ToolStripMenuItem()
			{
				Text = "Always On Top",
				Checked = config.data.AlwaysOnTop,
				CheckOnClick = true
			};
			alwaysOnTopItem.CheckedChanged += (object sender, EventArgs e) =>
			{
				config.data.AlwaysOnTop = ((ToolStripMenuItem) sender).Checked;
				config.Save();

				TopMost = config.data.AlwaysOnTop;
			};

			// Interval
			ToolStripMenuItem intervalItems = new ToolStripMenuItem("Interval");

			int[] intervals = { 50, 150, 250, 500, 750, 1000 };

			foreach (int interval in intervals)
			{
				ToolStripMenuItem item = new ToolStripMenuItem()
				{
					Text = interval + "ms",
					Checked = config.data.Interval == interval
				};
				item.Click += (object sender, EventArgs e) =>
				{
					if (((ToolStripMenuItem) sender).Checked == true)
					{
						return;
					}

					((ToolStripMenuItem) sender).Checked = true;

					foreach (ToolStripMenuItem intervalItem in intervalItems.DropDown.Items)
					{
						if (intervalItem.Checked && intervalItem.Text != ((ToolStripMenuItem) sender).Text)
						{
							intervalItem.Checked = false;
						}
					}

					config.data.Interval = interval;
					config.Save();
					updateSystemInformation.Change(0, config.data.Interval);
				};

				intervalItems.DropDownItems.Add(item);
			}

			// Added to context menu
			menuStrip.Items.Add(showInTaskbarITem);
			menuStrip.Items.Add(allowMoveItem);
			menuStrip.Items.Add(alwaysOnTopItem);

			menuStrip.Items.Add(new ToolStripSeparator());

			menuStrip.Items.Add(intervalItems);

			menuStrip.Items.Add(new ToolStripSeparator());

			menuStrip.Items.Add(
				new ToolStripMenuItem(
				"Close",
				GetShell32Icon(131)?.ToBitmap(),
				(object sender, EventArgs e) => Close())
			);

			contextMenuStrip = menuStrip;
		}

		private void PanelForm_Load(object sender, EventArgs e)
		{
			updateKeyboardLayoutTimer = new System.Threading.Timer(new TimerCallback(UpdateKeyboardLayout), null, 0, 50);
			updateSystemInformation = new System.Threading.Timer(new TimerCallback(UpdateSystemInformation), null, 0, config.data.Interval);
			UpdateSystemInformation(null);
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

			if (currentLang != ThreadHelperClass.GetLabelText(this, KeyboardLayout))
			{
				ThreadHelperClass.SetLabelText(this, KeyboardLayout, currentLang);
			}
		}

		private void UpdateSystemInformation(object obj)
		{
			SystemDataInformation data = SystemInformation.GetSystemInformation();

			//GPU
			ThreadHelperClass.SetLabelText(this, GpuLoad, Convert.ToString(data.gpuLoad) + (data.gpuLoad < 100 ? " " : "") + "%");
			ThreadHelperClass.SetControlForeColor(this, GpuLoad, HexToColor(config.GetColor(GetColorType.LOAD, GetColorDevice.GPU, data.gpuLoad)));

			ThreadHelperClass.SetLabelText(this, GpuTemp, Convert.ToString(data.gpuTemperature) + "°C");
			ThreadHelperClass.SetControlForeColor(this, GpuTemp, HexToColor(config.GetColor(GetColorType.TEMPERATURES, GetColorDevice.GPU, data.gpuTemperature)));

			//CPU
			ThreadHelperClass.SetLabelText(this, CpuLoad, Convert.ToString((int) data.cpuLoad) + (data.cpuLoad < 100 ? " " : "") + "%");
			ThreadHelperClass.SetControlForeColor(this, CpuLoad, HexToColor(config.GetColor(GetColorType.LOAD, GetColorDevice.CPU, (int) data.cpuLoad)));

			ThreadHelperClass.SetLabelText(this, CpuTemp, Convert.ToString((int) data.cpuTemperature) + "°C");
			ThreadHelperClass.SetControlForeColor(this, CpuTemp, HexToColor(config.GetColor(GetColorType.TEMPERATURES, GetColorDevice.CPU, (int) data.cpuTemperature)));

			//RAM
			ThreadHelperClass.SetLabelText(this, RamLoad, string.Format("{0:0.00}", data.ramLoad) + " % ");
			ThreadHelperClass.SetControlForeColor(this, RamLoad, HexToColor(config.GetColor(GetColorType.LOAD, GetColorDevice.RAM, (int) data.ramLoad)));

			ThreadHelperClass.SetLabelText(this, RamFree, string.Format("{0:0.00}", data.ramAvailable) + " GB");

			//Uptime
			TimeSpan uptimeTime = TimeSpan.FromMilliseconds(GetTickCount64());

			if (uptimeTime.Days > 0)
			{
				ThreadHelperClass.SetLabelText(this, Uptime, $"{uptimeTime.Days}d {uptimeTime.Hours:D2}h");
			}
			else if (uptimeTime.Hours > 0)
			{
				ThreadHelperClass.SetLabelText(this, Uptime, $"{uptimeTime.Hours:D2}h {uptimeTime.Minutes:D2}m");
			}
			else
			{
				ThreadHelperClass.SetLabelText(this, Uptime, $"{uptimeTime.Minutes:D2}m {uptimeTime.Seconds:D2}s");
			}
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

		private Icon GetShell32Icon(int iconIndex)
		{
			IntPtr iconHandle = ExtractIcon(IntPtr.Zero, "shell32.dll", iconIndex);

			return iconHandle == IntPtr.Zero ? null : Icon.FromHandle(iconHandle);
		}

		private Color HexToColor(string hex)
		{
			hex = hex.Replace("#", "");

			int length = hex.Length;
			int step = length == 3 ? 1 : 2;

			if (length != 3 && length != 6)
			{
				throw new ArgumentException($"Incorrect HEX color format {hex}", nameof(hex));
			}

			int r = Convert.ToInt32(hex.Substring(0, step), 16);
			int g = Convert.ToInt32(hex.Substring(step, step), 16);
			int b = Convert.ToInt32(hex.Substring(2 * step, step), 16);

			return Color.FromArgb(r, g, b);
		}
	}

	public static class ThreadHelperClass
	{
		delegate void SetTextCallback(Form f, Control ctrl, string text);
		delegate string GetTextCallback(Form f, Control ctrl);
		delegate void SetControlForeColorCallback(Form f, Control ctrl, System.Drawing.Color color);

		public static void SetLabelText(Form form, Control ctrl, string text)
		{
			if (ctrl.InvokeRequired)
			{
				form.Invoke(new SetTextCallback(SetLabelText), new object[] { form, ctrl, text });
			}
			else
			{
				ctrl.Text = text;
			}
		}

		public static string GetLabelText(Form form, Control ctrl)
		{
			if (ctrl.InvokeRequired)
			{
				return (string) form.Invoke(new GetTextCallback(GetLabelText), new object[] { form, ctrl });
			}
			else
			{
				return ctrl.Text;
			}
		}

		public static void SetControlForeColor(Form form, Control ctrl, System.Drawing.Color color)
		{
			if (ctrl.InvokeRequired)
			{
				form.Invoke(new SetControlForeColorCallback(SetControlForeColor), new object[] { form, ctrl, color });
			}
			else
			{
				ctrl.ForeColor = color;
			}
		}
	}
}