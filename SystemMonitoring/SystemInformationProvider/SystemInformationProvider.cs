namespace App
{
	public abstract class SystemInformationProvider
	{
		public abstract SystemDataInformation GetSystemInformation();
		public abstract void SaveReport(string path);
	}
}
