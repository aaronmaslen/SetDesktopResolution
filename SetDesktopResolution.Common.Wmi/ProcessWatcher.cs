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
            
			public Win32Process Process { get; }
            
			internal ProcessWatcherEventArgs(ProcessWatcherEventFlags flags, Win32Process process)
			{
				EventFlags = flags;
				Process = process;
			}
		}
        
		private readonly string _path;

		private readonly string _args;

		private Process _process;
		
		public ProcessDetectionMode Mode { get; }
		
		public ProcessWatcher(string executablePath, string args, ProcessDetectionMode mode)
		{
			_path = executablePath;
			_args = args;
			Mode = mode;
		}

		public void Start()
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(ProcessWatcher));

			if (Mode != ProcessDetectionMode.RunningProcess)
			{
				_process = Process.Start(_path, _args);

				Pid = _process?.Id ?? -1;
				_pidsToWatch.Add(Pid);
			}

			Wmi.ProcessEvent += WmiEventHandler;
		}

		private readonly ICollection<long> _pidsToWatch = new HashSet<long>();
		
		private void WmiEventHandler(object sender, Win32ProcessEventArgs e)
		{
			if (Mode == ProcessDetectionMode.RunningProcess &&
			    e.Process.ExecutablePath == _path)
			{
				Pid = e.Process.ProcessId;
				_pidsToWatch.Add(Pid);
			}

			if (_pidsToWatch.Contains(e.Process.ProcessId))
			{
				switch (e.EventType)
				{
					case Win32ProcessEventArgs.InstanceEventType.Create:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Started,
							               e.Process));
						break;
					case Win32ProcessEventArgs.InstanceEventType.Delete:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Stopped, 
							               e.Process));
						
						_pidsToWatch.Remove(e.Process.ProcessId);
						break;
				}
			}
			else if (Mode == ProcessDetectionMode.ProcessPlusChildren && 
			         _pidsToWatch.Contains(e.Process.ParentProcessId))
			{
				_pidsToWatch.Add(e.Process.ProcessId);

				switch (e.EventType)
				{
					case Win32ProcessEventArgs.InstanceEventType.Create:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Started | 
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Child, 
							               e.Process));
						break;
					case Win32ProcessEventArgs.InstanceEventType.Delete:
						OnProcessEvent(this, new ProcessWatcherEventArgs(
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Stopped |
							               ProcessWatcherEventArgs.ProcessWatcherEventFlags.Child,
							               e.Process));
						
						_pidsToWatch.Remove(e.Process.ProcessId);
						break;
				}
			}

			if (_pidsToWatch.Count == 0 && Pid != -1)
			{
				Wmi.ProcessEvent -= WmiEventHandler;
				
				OnStopEvent(this);
			}
		}

		public long Pid { get; private set; } = -1;

		public event EventHandler<ProcessWatcherEventArgs> ProcessEvent;

		private void OnStopEvent(object sender)
		{
			StopEvent?.Invoke(sender, new EventArgs());
		}
		
		public event EventHandler StopEvent;

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
