namespace SetDesktopResolution.Common.Wmi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Management;
	using System.Text;
	using System.Threading.Tasks;

	using Serilog;

	public static class Wmi
	{
		public static IDisposable RegisterProcessEventWatcher()
		{
			var createWatcher = new ManagementEventWatcher
				                    {
					                    Query = new WqlEventQuery("__InstanceCreationEvent", 
					                                              TimeSpan.FromSeconds(1), 
					                                              "TargetInstance isa \"Win32_Process\"")
				                    };

			createWatcher.EventArrived += (s, e) =>
				new ProcessEventHandler(e, Win32ProcessEventArgs.InstanceEventType.Create)
					.HandlerMethod(OnProcessEvent)(s);

			var deleteWatcher = new ManagementEventWatcher
				                    {
					                    Query = new WqlEventQuery("__InstanceDeletionEvent", 
					                                              TimeSpan.FromSeconds(1), 
					                                              "TargetInstance isa \"Win32_Process\"")
				                    };
			
			deleteWatcher.EventArrived += (s, e) =>
				new ProcessEventHandler(e, Win32ProcessEventArgs.InstanceEventType.Delete)
					.HandlerMethod(OnProcessEvent)(s);

			var modifyWatcher = new ManagementEventWatcher
				                    {
					                    Query = new WqlEventQuery("__InstanceModificationEvent",
					                                              TimeSpan.FromSeconds(1),
					                                              "TargetInstance isa \"Win32_Process\"")
				                    };
			
			modifyWatcher.EventArrived += (s, e) =>
				new ProcessEventHandler(e, Win32ProcessEventArgs.InstanceEventType.Modify)
					.HandlerMethod(OnProcessEvent)(s);
			
			createWatcher.Start();
			deleteWatcher.Start();
			modifyWatcher.Start();

			return new DisposableAggregate(new[] { createWatcher, deleteWatcher, modifyWatcher });
		}

		private static void OnProcessEvent(object sender, Win32ProcessEventArgs e) => ProcessEvent?.Invoke(sender, e);

		public static event Action<object, Win32ProcessEventArgs> ProcessEvent;
	}
}
