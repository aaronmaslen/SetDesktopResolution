using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SetDesktopResolution
{
	using Serilog;
	using Serilog.Events;
	using System.Reactive;
	using System.Reactive.Linq;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		internal IObservable<LogEvent> LogEvents;
		
		public App()
		{
			var log = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.Debug()
				.WriteTo.Observers(e => LogEvents = e.AsObservable())
				.CreateLogger();
			
			Log.Logger = log;
		}
	}
}
