using System.Windows.Forms;

namespace App
{
	public partial class Program
	{
		public static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PanelForm());
		}
	}
}