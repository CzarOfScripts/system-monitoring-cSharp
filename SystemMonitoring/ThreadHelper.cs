using System.Windows.Forms;

namespace App
{
	public static class ThreadHelper
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
