﻿namespace SetDesktopResolution
{
	using System;
	using System.Reactive.Subjects;
	using System.Windows;

	using Serilog.Events;

	public class App : Application
	{
		private readonly Subject<LogEvent> _logSubject = new Subject<LogEvent>();

		internal IObservable<LogEvent> LogEvents => _logSubject;

		protected Window StartupWindow;
		
		public App(IObservable<LogEvent> logObservable)
		{
			logObservable.Subscribe(e => _logSubject.OnNext(e),
			                        ex => _logSubject.OnError(ex),
			                        () => _logSubject.OnCompleted());

			StartupWindow = new MainWindow(this);
		}

		public new int Run() => Run(StartupWindow);
	}
}
