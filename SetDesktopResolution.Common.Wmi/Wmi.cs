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
		private static bool registered;

		public static IDisposable RegisterProcessEventWatcher()
		{
			if (registered) throw new InvalidOperationException("Already registered globally for WMI events");

			registered = true;

			return new DisposableAggregate(new[]
				                               {
					                               RegisterProcessEventWatcher(OnProcessEvent),
					                               new DisposedTrigger(() => registered = false)
				                               });
		}

		public static IDisposable RegisterProcessEventWatcher(EventHandler<Win32ProcessEventArgs> handler)
		{
			var createWatcher = new ManagementEventWatcher
				                    {
					                    Query = new WqlEventQuery("__InstanceCreationEvent", 
					                                              TimeSpan.FromSeconds(1), 
					                                              "TargetInstance isa \"Win32_Process\"")
				                    };

			createWatcher.EventArrived += 
				ProcessEventHandler.HandlerMethod(handler, Win32ProcessEventArgs.InstanceEventType.Create);

			var deleteWatcher = new ManagementEventWatcher
				                    {
					                    Query = new WqlEventQuery("__InstanceDeletionEvent", 
					                                              TimeSpan.FromSeconds(1), 
					                                              "TargetInstance isa \"Win32_Process\"")
				                    };
			
			deleteWatcher.EventArrived += 
				ProcessEventHandler.HandlerMethod(handler, Win32ProcessEventArgs.InstanceEventType.Delete);

			var modifyWatcher = new ManagementEventWatcher
				                    {
					                    Query = new WqlEventQuery("__InstanceModificationEvent",
					                                              TimeSpan.FromSeconds(1),
					                                              "TargetInstance isa \"Win32_Process\"")
				                    };
			
			modifyWatcher.EventArrived += 
				ProcessEventHandler.HandlerMethod(handler, Win32ProcessEventArgs.InstanceEventType.Modify);

			createWatcher.Start();
			deleteWatcher.Start();
			modifyWatcher.Start();

			return new DisposableAggregate(new[] { createWatcher, deleteWatcher, modifyWatcher });
		}

		private static void OnProcessEvent(object sender, Win32ProcessEventArgs e) => ProcessEvent?.Invoke(sender, e);

		public static event EventHandler<Win32ProcessEventArgs> ProcessEvent;
	}
}
