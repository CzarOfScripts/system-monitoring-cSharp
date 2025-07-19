using System;
using System.Threading;
using System.Windows.Forms;

using NLog;
using NLog.Config;

namespace App
{
	public partial class Program
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public static void Main()
		{
			try
			{
				LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.ThreadException += ThreadExceptionHandler;
				AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
				AppDomain.CurrentDomain.FirstChanceException += FirstChanceExceptionHandler;

				Application.Run(new PanelForm());
			}
			catch (Exception ex)
			{
				logger.Fatal(ex, "Critical error in Main: {Message}", ex.Message);
				MessageBox.Show($"Critical error: {ex.Message}\n\nDetails logged.",
					"System Monitoring - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
		{
			logger.Error(e.Exception, "Unhandled exception in UI thread: {Message}", e.Exception.Message);

			try
			{
				MessageBox.Show(
					$"Interface error occurred: {e.Exception.Message}\n\nDetails logged.",
					"System Monitoring - Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
				);
			}
			catch
			{
				logger.Error("Failed to show error dialog");
			}
		}

		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = e.ExceptionObject as Exception;
			logger.Fatal(exception, "Unhandled exception: {Message}. Application will be closed.",
				exception?.Message ?? "Unknown error");

			try
			{
				MessageBox.Show(
					$"Critical error: {exception?.Message ?? "Unknown error"}\n\nApplication will be closed.",
					"System Monitoring - Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error
				);
			}
			catch {}
		}

		private static void FirstChanceExceptionHandler(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
		{
			logger.Debug(e.Exception, "FirstChanceException: {Message}", e.Exception.Message);
		}
	}
}
