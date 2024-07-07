namespace App
{
	public class UpdateVisitor : LibreHardwareMonitor.Hardware.IVisitor
	{
		public void VisitComputer(LibreHardwareMonitor.Hardware.IComputer computer)
		{
			computer.Traverse(this);
		}

		public void VisitHardware(LibreHardwareMonitor.Hardware.IHardware hardware)
		{
			hardware.Update();
			foreach (LibreHardwareMonitor.Hardware.IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
		}

		public void VisitSensor(LibreHardwareMonitor.Hardware.ISensor sensor) { }
		public void VisitParameter(LibreHardwareMonitor.Hardware.IParameter parameter) { }
	}
}
