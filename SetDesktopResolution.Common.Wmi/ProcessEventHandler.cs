namespace SetDesktopResolution.Common.Wmi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Management;
	using System.Text;
	using System.Threading.Tasks;

	using JetBrains.Annotations;

	internal class ProcessEventHandler
	{
		protected ManagementBaseObject TargetInstance { get; }
		
		protected Win32ProcessEventArgs.InstanceEventType EventType { get; }
		
		public ProcessEventHandler([NotNull] EventArrivedEventArgs e, 
		                           Win32ProcessEventArgs.InstanceEventType eventType)
		{
			if (!(e.NewEvent["TargetInstance"] is ManagementBaseObject targetInstance))
				throw new ArgumentException("Missing TargetInstance or TargetInstance is not a ManagementBaseObject");

			TargetInstance = targetInstance;
			EventType = eventType;
		}

		public Func<Action<object, Win32ProcessEventArgs>, Action<object>> HandlerMethod =>
			onEvent => sender =>
				onEvent(sender, new Win32ProcessEventArgs(new Win32Process(TargetInstance), EventType));
	}
}
