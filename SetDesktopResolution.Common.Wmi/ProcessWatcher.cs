namespace SetDesktopResolution.Common.Wmi
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;

	public sealed class ProcessWatcher : IDisposable
	{
		public class ProcessWatcherEventArgs : EventArgs
		{
			[Flags]
			public enum ProcessWatcherEventFlags
			{
				Started = 1,
				Stopped = 2,
				Child = 4,
			}

			public ProcessWatcherEventFlags EventFlags { get; }
            
			public int Pid { get; }
            
			internal ProcessWatcherEventArgs(ProcessWatcherEventFlags flags, int pid)
			{
				EventFlags = flags;
				Pid = pid;
			}
		}
        
		private readonly string _path;

		private readonly string _args;

		private Process _process;
        
		public ProcessWatcher(string executablePath, string args)
		{
			_path = executablePath;
			_args = args;
		}

		public void Start()
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(ProcessWatcher));
			
			_process = Process.Start(_path, _args);
			
			_pidsToWatch.Add(Pid);

			Wmi.ProcessEvent += WmiEventHandler;
		}

		private readonly List<int> _pidsToWatch = new List<int>();
		
		private void WmiEventHandler(object sender, Win32ProcessEventArgs e)
		{
			if (_pidsToWatch.Contains(e.Process.ProcessId))
			{
				switch (e.EventType)
				{
					case Win32ProcessEventArgs.InstanceEventType.Create:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Started,
							               e.Process.ProcessId));
						break;
					case Win32ProcessEventArgs.InstanceEventType.Delete:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Stopped, 
							               e.Process.ProcessId));
						break;
				}
			}
			else if (_pidsToWatch.Contains(e.Process.ParentProcessId))
			{
				_pidsToWatch.Add(e.Process.ProcessId);

				switch (e.EventType)
				{
					case Win32ProcessEventArgs.InstanceEventType.Create:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Started | 
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Child, 
							               e.Process.ProcessId));
						break;
					case Win32ProcessEventArgs.InstanceEventType.Delete:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Stopped |
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Child,
							               e.Process.ProcessId));
						break;
				}
			}
		}
		
		public int Pid => _process?.Id ?? -1;

		public event EventHandler<ProcessWatcherEventArgs> ProcessEvent;

		private void OnProcessEvent(object sender, ProcessWatcherEventArgs e) => ProcessEvent?.Invoke(sender, e);

		private bool _disposed;
		
		public void Dispose()
		{
			_process?.Dispose();

			Wmi.ProcessEvent -= WmiEventHandler;
		
			_disposed = true;
		}
	}
}
