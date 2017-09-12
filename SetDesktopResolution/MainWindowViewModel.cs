﻿namespace SetDesktopResolution
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
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

	public class MainWindowViewModel : INotifyPropertyChanged
	{
		internal MainWindowViewModel(IObservable<LogEvent> logObservable)
		{
			PropertyChanged += DeviceSelectedHandler;

			logObservable
				.Do(e =>
				{
					_logEntries.Add(e);
					OnPropertyChanged(nameof(LogText));
				})
				.Subscribe();
			Update();
		}

		private readonly List<LogEvent> _logEntries = new List<LogEvent>();

		public void Update()
		{
			var device = SelectedDevice;
			var mode = SelectedMode;
			
			Devices = new ObservableCollection<DisplayDevice>(
				DisplayDevice.GetDisplayDevices().Where(d => d.Attached));

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
					SelectedDevice.Modes.Where(m => m.ScalingMode == DisplayMode.ScalingType.Default &&
					                                !m.Interlaced &&
					                                m.Bpp == 32)
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
				});
		
		public ICommand RestoreCommand => new CustomCommand(
			() =>
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
				});
		
		public string LogText => 
			_logEntries.ToList()
			           .Where(e => e.Level >= MinimumLogDisplayLevel)
			           .Select(e => $"[{e.Timestamp:HH:mm:ss} {e.Level.ToString().ToUpper()}] {e.RenderMessage()}")
			           .Aggregate(string.Empty, (acc, curr) => acc + Environment.NewLine + curr);
		
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
