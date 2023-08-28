namespace App
{
	partial class PanelForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Panel Splitter1;
			System.Windows.Forms.Label GpuLabel;
			System.Windows.Forms.Label GpuLoadLabel;
			System.Windows.Forms.Label GpuTempLabel;
			System.Windows.Forms.Panel Splitter2;
			System.Windows.Forms.Label CpuLabel;
			System.Windows.Forms.Label CpuLoadLabel;
			System.Windows.Forms.Label CpuTempLabel;
			System.Windows.Forms.Panel Splitter3;
			System.Windows.Forms.Label RamLabel;
			System.Windows.Forms.Label RamLoadLabel;
			System.Windows.Forms.Label RamFreeLabel;
			System.Windows.Forms.Panel Splitter4;
			System.Windows.Forms.Label UptimeLabel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelForm));
			this.KeyboardLayout = new System.Windows.Forms.Label();
			this.GpuLoad = new System.Windows.Forms.Label();
			this.GpuTemp = new System.Windows.Forms.Label();
			this.CpuLoad = new System.Windows.Forms.Label();
			this.CpuTemp = new System.Windows.Forms.Label();
			this.RamLoad = new System.Windows.Forms.Label();
			this.RamFree = new System.Windows.Forms.Label();
			this.Uptime = new System.Windows.Forms.Label();
			Splitter1 = new System.Windows.Forms.Panel();
			GpuLabel = new System.Windows.Forms.Label();
			GpuLoadLabel = new System.Windows.Forms.Label();
			GpuTempLabel = new System.Windows.Forms.Label();
			Splitter2 = new System.Windows.Forms.Panel();
			CpuLabel = new System.Windows.Forms.Label();
			CpuLoadLabel = new System.Windows.Forms.Label();
			CpuTempLabel = new System.Windows.Forms.Label();
			Splitter3 = new System.Windows.Forms.Panel();
			RamLabel = new System.Windows.Forms.Label();
			RamLoadLabel = new System.Windows.Forms.Label();
			RamFreeLabel = new System.Windows.Forms.Label();
			Splitter4 = new System.Windows.Forms.Panel();
			UptimeLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Splitter1
			// 
			Splitter1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
			Splitter1.Location = new System.Drawing.Point(37, 8);
			Splitter1.Name = "Splitter1";
			Splitter1.Size = new System.Drawing.Size(2, 24);
			Splitter1.TabIndex = 1;
			// 
			// GpuLabel
			// 
			GpuLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			GpuLabel.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			GpuLabel.ForeColor = System.Drawing.Color.White;
			GpuLabel.Location = new System.Drawing.Point(42, 8);
			GpuLabel.Margin = new System.Windows.Forms.Padding(0);
			GpuLabel.Name = "GpuLabel";
			GpuLabel.Size = new System.Drawing.Size(46, 24);
			GpuLabel.TabIndex = 2;
			GpuLabel.Text = "GPU";
			// 
			// GpuLoadLabel
			// 
			GpuLoadLabel.AutoSize = true;
			GpuLoadLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			GpuLoadLabel.ForeColor = System.Drawing.Color.White;
			GpuLoadLabel.Location = new System.Drawing.Point(87, 5);
			GpuLoadLabel.Name = "GpuLoadLabel";
			GpuLoadLabel.Size = new System.Drawing.Size(42, 14);
			GpuLoadLabel.TabIndex = 3;
			GpuLoadLabel.Text = "Load:";
			// 
			// GpuTempLabel
			// 
			GpuTempLabel.AutoSize = true;
			GpuTempLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			GpuTempLabel.ForeColor = System.Drawing.Color.White;
			GpuTempLabel.Location = new System.Drawing.Point(87, 21);
			GpuTempLabel.Name = "GpuTempLabel";
			GpuTempLabel.Size = new System.Drawing.Size(42, 14);
			GpuTempLabel.TabIndex = 4;
			GpuTempLabel.Text = "Temp:";
			// 
			// Splitter2
			// 
			Splitter2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
			Splitter2.Location = new System.Drawing.Point(164, 8);
			Splitter2.Name = "Splitter2";
			Splitter2.Size = new System.Drawing.Size(2, 24);
			Splitter2.TabIndex = 7;
			// 
			// CpuLabel
			// 
			CpuLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			CpuLabel.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			CpuLabel.ForeColor = System.Drawing.Color.White;
			CpuLabel.Location = new System.Drawing.Point(169, 8);
			CpuLabel.Margin = new System.Windows.Forms.Padding(0);
			CpuLabel.Name = "CpuLabel";
			CpuLabel.Size = new System.Drawing.Size(46, 24);
			CpuLabel.TabIndex = 8;
			CpuLabel.Text = "CPU";
			// 
			// CpuLoadLabel
			// 
			CpuLoadLabel.AutoSize = true;
			CpuLoadLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			CpuLoadLabel.ForeColor = System.Drawing.Color.White;
			CpuLoadLabel.Location = new System.Drawing.Point(214, 5);
			CpuLoadLabel.Name = "CpuLoadLabel";
			CpuLoadLabel.Size = new System.Drawing.Size(42, 14);
			CpuLoadLabel.TabIndex = 9;
			CpuLoadLabel.Text = "Load:";
			// 
			// CpuTempLabel
			// 
			CpuTempLabel.AutoSize = true;
			CpuTempLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			CpuTempLabel.ForeColor = System.Drawing.Color.White;
			CpuTempLabel.Location = new System.Drawing.Point(214, 21);
			CpuTempLabel.Name = "CpuTempLabel";
			CpuTempLabel.Size = new System.Drawing.Size(42, 14);
			CpuTempLabel.TabIndex = 10;
			CpuTempLabel.Text = "Temp:";
			// 
			// Splitter3
			// 
			Splitter3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
			Splitter3.Location = new System.Drawing.Point(292, 8);
			Splitter3.Name = "Splitter3";
			Splitter3.Size = new System.Drawing.Size(2, 24);
			Splitter3.TabIndex = 13;
			// 
			// RamLabel
			// 
			RamLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			RamLabel.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			RamLabel.ForeColor = System.Drawing.Color.White;
			RamLabel.Location = new System.Drawing.Point(297, 8);
			RamLabel.Margin = new System.Windows.Forms.Padding(0);
			RamLabel.Name = "RamLabel";
			RamLabel.Size = new System.Drawing.Size(46, 24);
			RamLabel.TabIndex = 14;
			RamLabel.Text = "RAM";
			// 
			// RamLoadLabel
			// 
			RamLoadLabel.AutoSize = true;
			RamLoadLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			RamLoadLabel.ForeColor = System.Drawing.Color.White;
			RamLoadLabel.Location = new System.Drawing.Point(342, 5);
			RamLoadLabel.Name = "RamLoadLabel";
			RamLoadLabel.Size = new System.Drawing.Size(42, 14);
			RamLoadLabel.TabIndex = 15;
			RamLoadLabel.Text = "Load:";
			// 
			// RamFreeLabel
			// 
			RamFreeLabel.AutoSize = true;
			RamFreeLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			RamFreeLabel.ForeColor = System.Drawing.Color.White;
			RamFreeLabel.Location = new System.Drawing.Point(342, 21);
			RamFreeLabel.Name = "RamFreeLabel";
			RamFreeLabel.Size = new System.Drawing.Size(42, 14);
			RamFreeLabel.TabIndex = 16;
			RamFreeLabel.Text = "Free:";
			// 
			// Splitter4
			// 
			Splitter4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
			Splitter4.Location = new System.Drawing.Point(455, 8);
			Splitter4.Name = "Splitter4";
			Splitter4.Size = new System.Drawing.Size(2, 24);
			Splitter4.TabIndex = 14;
			// 
			// UptimeLabel
			// 
			UptimeLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			UptimeLabel.ForeColor = System.Drawing.Color.White;
			UptimeLabel.Location = new System.Drawing.Point(465, 5);
			UptimeLabel.Name = "UptimeLabel";
			UptimeLabel.Size = new System.Drawing.Size(65, 14);
			UptimeLabel.TabIndex = 19;
			UptimeLabel.Text = "Up Time:";
			UptimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// KeyboardLayout
			// 
			this.KeyboardLayout.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.KeyboardLayout.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.KeyboardLayout.ForeColor = System.Drawing.Color.White;
			this.KeyboardLayout.Location = new System.Drawing.Point(2, 8);
			this.KeyboardLayout.Margin = new System.Windows.Forms.Padding(0);
			this.KeyboardLayout.Name = "KeyboardLayout";
			this.KeyboardLayout.Size = new System.Drawing.Size(34, 24);
			this.KeyboardLayout.TabIndex = 0;
			this.KeyboardLayout.Text = "EN";
			// 
			// GpuLoad
			// 
			this.GpuLoad.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.GpuLoad.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.GpuLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.GpuLoad.Location = new System.Drawing.Point(127, 5);
			this.GpuLoad.Margin = new System.Windows.Forms.Padding(0);
			this.GpuLoad.Name = "GpuLoad";
			this.GpuLoad.Size = new System.Drawing.Size(35, 14);
			this.GpuLoad.TabIndex = 5;
			this.GpuLoad.Text = "0 %";
			this.GpuLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// GpuTemp
			// 
			this.GpuTemp.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.GpuTemp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.GpuTemp.Location = new System.Drawing.Point(127, 21);
			this.GpuTemp.Margin = new System.Windows.Forms.Padding(0);
			this.GpuTemp.Name = "GpuTemp";
			this.GpuTemp.Size = new System.Drawing.Size(35, 14);
			this.GpuTemp.TabIndex = 6;
			this.GpuTemp.Text = "0°C";
			this.GpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// CpuLoad
			// 
			this.CpuLoad.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.CpuLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.CpuLoad.Location = new System.Drawing.Point(254, 5);
			this.CpuLoad.Margin = new System.Windows.Forms.Padding(0);
			this.CpuLoad.Name = "CpuLoad";
			this.CpuLoad.Size = new System.Drawing.Size(35, 14);
			this.CpuLoad.TabIndex = 11;
			this.CpuLoad.Text = "0 %";
			this.CpuLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// CpuTemp
			// 
			this.CpuTemp.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.CpuTemp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.CpuTemp.Location = new System.Drawing.Point(254, 21);
			this.CpuTemp.Margin = new System.Windows.Forms.Padding(0);
			this.CpuTemp.Name = "CpuTemp";
			this.CpuTemp.Size = new System.Drawing.Size(35, 14);
			this.CpuTemp.TabIndex = 12;
			this.CpuTemp.Text = "0°C";
			this.CpuTemp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// RamLoad
			// 
			this.RamLoad.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.RamLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.RamLoad.Location = new System.Drawing.Point(387, 5);
			this.RamLoad.Margin = new System.Windows.Forms.Padding(0);
			this.RamLoad.Name = "RamLoad";
			this.RamLoad.Size = new System.Drawing.Size(63, 14);
			this.RamLoad.TabIndex = 17;
			this.RamLoad.Text = "0 % ";
			this.RamLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// RamFree
			// 
			this.RamFree.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.RamFree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.RamFree.Location = new System.Drawing.Point(387, 21);
			this.RamFree.Margin = new System.Windows.Forms.Padding(0);
			this.RamFree.Name = "RamFree";
			this.RamFree.Size = new System.Drawing.Size(63, 14);
			this.RamFree.TabIndex = 18;
			this.RamFree.Text = "0 GB";
			this.RamFree.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Uptime
			// 
			this.Uptime.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Uptime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.Uptime.Location = new System.Drawing.Point(465, 21);
			this.Uptime.Margin = new System.Windows.Forms.Padding(0);
			this.Uptime.Name = "Uptime";
			this.Uptime.Size = new System.Drawing.Size(65, 14);
			this.Uptime.TabIndex = 20;
			this.Uptime.Text = "0m 0s";
			this.Uptime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// PanelForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.ControlText;
			this.ClientSize = new System.Drawing.Size(536, 40);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PanelForm_FormClosed);
			this.Load += new System.EventHandler(this.PanelForm_Load);
			this.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.PanelForm_ControlAdded);
			this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.PanelForm_ControlRemoved);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PanelForm_MouseClick);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelForm_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PanelForm_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PanelForm_MouseUp);
			this.Controls.Add(this.Uptime);
			this.Controls.Add(UptimeLabel);
			this.Controls.Add(Splitter4);
			this.Controls.Add(Splitter3);
			this.Controls.Add(RamLabel);
			this.Controls.Add(this.KeyboardLayout);
			this.Controls.Add(RamLoadLabel);
			this.Controls.Add(Splitter1);
			this.Controls.Add(RamFreeLabel);
			this.Controls.Add(GpuLabel);
			this.Controls.Add(this.RamLoad);
			this.Controls.Add(GpuLoadLabel);
			this.Controls.Add(this.RamFree);
			this.Controls.Add(GpuTempLabel);
			this.Controls.Add(this.GpuLoad);
			this.Controls.Add(this.GpuTemp);
			this.Controls.Add(Splitter2);
			this.Controls.Add(CpuLabel);
			this.Controls.Add(CpuLoadLabel);
			this.Controls.Add(CpuTempLabel);
			this.Controls.Add(this.CpuLoad);
			this.Controls.Add(this.CpuTemp);
			this.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(-542, 600);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "PanelForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "System Monitoring | Panel";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label KeyboardLayout;
		private System.Windows.Forms.Label GpuTemp;
		private System.Windows.Forms.Label CpuLoad;
		private System.Windows.Forms.Label CpuTemp;
		private System.Windows.Forms.Label GpuLoad;
		private System.Windows.Forms.Label RamLoad;
		private System.Windows.Forms.Label RamFree;
		private System.Windows.Forms.Label Uptime;
	}
}