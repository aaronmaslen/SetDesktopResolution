namespace SetDesktopResolution
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Runtime.CompilerServices;
	using System.Windows;
	using System.Windows.Input;

	using Common;

	using JetBrains.Annotations;

	using Serilog;
	using Serilog.Events;

	using SetDesktopResolution.Common.Windows;
	using SetDesktopResolution.Common.Wmi;

	using static SetDesktopResolution.Common.Wmi.ProcessWatcher.ProcessWatcherEventArgs.ProcessWatcherEventFlags;

	public class MainWindowViewModel : INotifyPropertyChanged
	{
		internal MainWindowViewModel(IObservable<LogEvent> logObservable)
		{
			PropertyChanged += DeviceSelectedHandler;

			// Save log events
			logObservable
				.Do(e =>
				{
					// Copy on write - this runs on a separate thread to the UI
					_logEntries = _logEntries.Concat(new[] { e }).ToList();
					OnPropertyChanged(nameof(LogText));
				})
				.Subscribe();
			Update();
		}

		private IReadOnlyList<LogEvent> _logEntries = new List<LogEvent>();

		public void Update()
		{
			// Save current device and mode
			var device = SelectedDevice;
			var mode = SelectedMode;
			
			Devices = new ObservableCollection<DisplayDevice>(
				DisplayDevice.GetDisplayDevices().Where(d => d.Attached));

			// Restore device and mode if they're still available
			if (device != null && Devices.Contains(device))
			{
				SelectedDevice = device;

				if (mode != null && device.Modes.Contains(mode))
					SelectedMode = mode;
			}
			else
			{
				SelectedDevice = Devices[0];
			}
		}

		public LogEventLevel MinimumLogDisplayLevel { get; set; } = LogEventLevel.Information;

		private DisplayDevice _selectedDevice;
		
		public DisplayDevice SelectedDevice
		{
			get => _selectedDevice;
			set
			{
				if (_selectedDevice == value) return;
				
				_selectedDevice = value;
				OnPropertyChanged();
			}
		}

		private PropertyChangedEventHandler DeviceSelectedHandler =>
			(sender, eventArgs) =>
			{
				if (eventArgs.PropertyName != nameof(SelectedDevice)) return;
				
				SelectedDeviceModes = new ObservableCollection<DisplayMode>(
					SelectedDevice.Modes.Where(m => m.ScalingMode == DisplayMode.ScalingType.Default)
					                    .Where(m => Properties.Settings.Default.IncludeInterlacedModes || !m.Interlaced)
					                    .Where(m => !Properties.Settings.Default.Only32BitColor || m.Bpp == 32)
					                    .Where(m => m.RefreshRate >= Properties.Settings.Default.MinimumRefreshRate)
					                    .Where(m => m.RefreshRate <= (Properties.Settings.Default.MaximumRefreshRate > 0 ?
						                                                  Properties.Settings.Default.MaximumRefreshRate :
						                                                  int.MaxValue))
					                    .OrderBy(m => m.Resolution.Width)
					                    .Reverse());

				SelectedDeviceCurrentMode = SelectedMode = SelectedDevice.CurrentMode;
			};

		private DisplayMode _selectedMode;
		
		public DisplayMode SelectedMode
		{
			get => _selectedMode;
			set
			{
				if (ReferenceEquals(_selectedMode, value)) return;
				
				_selectedMode = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<DisplayDevice> _devices;
		
		public ObservableCollection<DisplayDevice> Devices
		{
			get => _devices;
			private set
			{
				if (_devices == value) return;
				
				_devices = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<DisplayMode> _selectedDeviceModes;
		
		public ObservableCollection<DisplayMode> SelectedDeviceModes
		{
			get => _selectedDeviceModes;
			private set
			{
				if (_selectedDeviceModes == value) return;

				_selectedDeviceModes = value;
				OnPropertyChanged();
			}
		}

		private DisplayMode _selectedDeviceCurrentMode;

		public DisplayMode SelectedDeviceCurrentMode
		{
			get => _selectedDeviceCurrentMode;
			private set
			{
				if (ReferenceEquals(_selectedDeviceCurrentMode, value)) return;

				_selectedDeviceCurrentMode = value;
				OnPropertyChanged();
			}
		}

		private string _executablePath;
		
		public string ExecutablePath
		{
			get => _executablePath;
			set
			{
				if (_executablePath == value) return;
				
				_executablePath = value;
				OnPropertyChanged();
			}
		}

		private DisplayMode _savedMode;
		private DisplayDevice _savedDevice;
		
		public ICommand RunCommand => new CustomCommand<DisplayMode>(
			m =>
				{
					_watcher = new ProcessWatcher(ExecutablePath, string.Empty, Properties.Settings.Default.ProcessDetectionMode);
					_watcher.ProcessEvent += (s, e) =>
						{
							if (e.EventFlags.HasFlag(Started))
							{
								Log.Information("{Process} ({PID}) started", e.Process.Name, e.Process.ProcessId);
								Activate(m);
							}
							else if (e.EventFlags.HasFlag(Stopped))
							{
								Log.Information("{Process} ({PID}) stopped", e.Process.Name, e.Process.ProcessId);
							}

							Log.Debug("{PID} {flags}", e.Process.ProcessId, e.EventFlags);
						};
					_watcher.StopEvent += (s, e) =>
						{
							Log.Information("All processes stopped");
							
							Deactivate();

							_watcher.Dispose();
						};
					
					_watcher.Start();

					EnableControls = false;
				});
		
		private ProcessWatcher _watcher;

		private void Activate(DisplayMode m)
		{
			_savedDevice = SelectedDevice;
			_savedMode = SelectedDevice.CurrentMode;
			Log.Logger.Information("Saving current mode {mode}", _savedMode);

			Log.Logger.Information("Setting mode {mode} on device {device}", m, SelectedDevice);
			try
			{
				SelectedDevice.SetMode(m);
				SelectedDeviceCurrentMode = SelectedDevice.CurrentMode;
			}
			catch (ArgumentException ae)
			{
				Log.Logger.Error(ae, "Invalid mode specified");
			}
			catch (NativeMethodException nme)
			{
				Log.Logger.Error(nme, "Failed to set mode");
			}
		}

		private void Deactivate()
		{
			Log.Logger.Information("Restoring saved mode {mode} on device {device}", _savedMode, _savedDevice);
			try
			{
				_savedDevice.SetMode(_savedMode);
				SelectedDeviceCurrentMode = SelectedDevice.CurrentMode;
			}
			catch (ArgumentException ae)
			{
				Log.Logger.Error(ae, "Invalid mode specified");
			}
			catch (NativeMethodException nme)
			{
				Log.Logger.Error(nme, "Failed to set mode");
			}

			EnableControls = true;
		}

		public ICommand RestoreCommand => new CustomCommand(Deactivate);

		private bool _enableControls = true;

		public bool EnableControls
		{
			get => _enableControls;
			private set
			{
				if (_enableControls == value) return;

				_enableControls = value;
				OnPropertyChanged();
			}
		}

		public string LogText
		{
			get
			{
				var tempList = _logEntries.ToList();
				return tempList.Where(e => e.Level >= Properties.Settings.Default.MinimumLogDisplayLevel)
				               .OrderBy(e => e.Timestamp)
				               .Select(e => $"[{e.Timestamp:HH:mm:ss} {e.Level.ToString().ToUpper()}] {e.RenderMessage()}")
				               .Aggregate(string.Empty, (acc, curr) => acc + Environment.NewLine + curr);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
