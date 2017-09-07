namespace SetDesktopResolution
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Linq;
	using System.Reactive;
	using System.Reactive.Linq;
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
		public App()
		{
			var log = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.Debug()
				.CreateLogger();
			
			Log.Logger = log;
		}
	}
}
