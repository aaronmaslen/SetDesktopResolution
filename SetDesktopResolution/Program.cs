using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetDesktopResolution
{
	using Serilog;
	using Serilog.Events;

	using SetDesktopResolution.Common.Wmi;

	internal static class Program
	{
		[STAThread]
		private static void Main()
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
				new App(logObservable).Run();
			}
		}
	}
}
