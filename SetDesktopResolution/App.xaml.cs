namespace SetDesktopResolution
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Linq;
	using System.Reactive;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Threading.Tasks;
	using System.Windows;

	using Serilog;
	using Serilog.Events;

	/// <inheritdoc />
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static readonly Subject<LogEvent> LogSubject = new Subject<LogEvent>();
		
		internal static IObservable<LogEvent> LogEvents => LogSubject;
	
		public App()
		{
			var log = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.Debug()
				.WriteTo.Observers(o => o.Subscribe(e => LogSubject.OnNext(e), ex => LogSubject.OnError(ex), () => LogSubject.OnCompleted()))
				.CreateLogger();
			
			Log.Logger = log;
		}
	}
}
