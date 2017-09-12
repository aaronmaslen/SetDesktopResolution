namespace SetDesktopResolution.Common.Wmi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Management;
	using System.Text;
	using System.Threading.Tasks;

	using JetBrains.Annotations;

	internal static class ProcessEventHandler
	{
		public static
			Func<EventHandler<Win32ProcessEventArgs>, Win32ProcessEventArgs.InstanceEventType, 
				EventArrivedEventHandler> HandlerMethod =>
					(onEvent, t) => (sender, e) =>
					{
						if (!(e.NewEvent["TargetInstance"] is ManagementBaseObject targetInstance))
							throw new ArgumentException(
								"Missing TargetInstance or TargetInstance is not a ManagementBaseObject");
					
						onEvent(sender, new Win32ProcessEventArgs(new Win32Process(targetInstance), t));
					};
	}
}
