namespace SetDesktopResolution
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Serilog;
	using Serilog.Events;

	using SetDesktopResolution.Common.Wmi;

	public static class Program
	{
		[STAThread]
		internal static int Main()
		{
			IObservable<LogEvent> logObservable = null;
			
			var log = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.Debug()
				.WriteTo.Observers(o => logObservable = o)
				.CreateLogger();

			Log.Logger = log;

			using (Wmi.RegisterProcessEventWatcher())
			{
				Wmi.ProcessEvent += (s, e) =>
					{
						Log.Logger.Verbose("{Name} ({PID}) - {Event}", e.Process.Name, e.Process.ProcessId, e.EventType);
					};
				
				return new App(logObservable).Run();
			}
		}
	}
}
