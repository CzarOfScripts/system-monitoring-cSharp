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
			LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += ThreadExceptionHandler;
			Application.Run(new PanelForm());
		}

		private static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
		{
			logger.Error(e.Exception, "ThreadExceptionHandler");
		}
	}
}