using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SetDesktopResolution
{
	using System.IO;
	using Common;
	using Common.Annotations;
	using System.Diagnostics;
	using System.Runtime.Remoting.Channels;

	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel()
		{
			PropertyChanged += DeviceSelectedHandler;
			
			Update();
		}

		public void Update()
		{
			var device = SelectedDevice;
			var mode = SelectedMode;
			
			Devices = new ObservableCollection<DisplayDevice>(DisplayDevice.GetDisplayDevices().Where(d => d.Attached));

			if(device != null && Devices.Contains(device))
				SelectedDevice = device;

			if(mode!= null && (SelectedDevice?.Modes.Contains(mode) ?? false))
				SelectedMode = mode;
		}

		private DisplayDevice _selectedDevice;
		public DisplayDevice SelectedDevice
		{
			get => _selectedDevice;
			set
			{
				if(_selectedDevice == value) return;
				
				_selectedDevice = value;
				OnPropertyChanged();
			}
		}

		private PropertyChangedEventHandler DeviceSelectedHandler =>
			(sender, eventArgs) =>
			{
				if(eventArgs.PropertyName != nameof(SelectedDevice)) return;

				SelectedDeviceModes = new ObservableCollection<DisplayMode>(
					SelectedDevice.Modes.Where(m => m.ScalingMode == DisplayMode.ScalingType.Default)
					                    .OrderBy(m => m.Resolution.Width)
					                    .Reverse());

				SelectedMode = SelectedDevice.CurrentMode;
			};

		private DisplayMode _selectedMode;
		public DisplayMode SelectedMode
		{
			get => _selectedMode;
			set
			{
				if(ReferenceEquals(_selectedMode, value)) return;
				
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
				if(_devices == value) return;
				
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
				if(_selectedDeviceModes == value) return;

				_selectedDeviceModes = value;
				OnPropertyChanged();
			}
		}

		private string _executablePath;
		public string ExecutablePath
		{
			get => _executablePath;
			set
			{
				if(_executablePath == value) return;
				
				_executablePath = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
