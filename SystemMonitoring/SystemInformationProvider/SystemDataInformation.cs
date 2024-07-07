namespace App
{
	public readonly struct SystemDataInformation(float cpuTemperature, float cpuLoad, byte gpuTemperature, byte gpuLoad, float ramLoad, float ramAvailable, float ramUsed)
	{
		///<summary>In Celsius</summary>
		public readonly float cpuTemperature = cpuTemperature;
		///<summary>In percent</summary>
		public readonly float cpuLoad = cpuLoad;

		///<summary>In Celsius</summary>
		public readonly byte gpuTemperature = gpuTemperature;
		///<summary>In percent</summary>
		public readonly byte gpuLoad = gpuLoad;

		///<summary>In percent</summary>
		public readonly float ramLoad = ramLoad;
		///<summary>In GB</summary>
		public readonly float ramAvailable = ramAvailable;
		///<summary>In GB</summary>
		public readonly float ramUsed = ramUsed;
	}
}
