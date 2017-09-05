namespace SetDesktopResolution.Common
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Threading.Tasks;
	using Annotations;
	using static NativeMethods;

	public class DisplayDevice : INotifyPropertyChanged
	{
		[NotNull]
		public static IEnumerable<DisplayDevice> GetDisplayDevices() => GetDevices().Select(d => new DisplayDevice(d));
		
		private readonly DISPLAY_DEVICE _nativeDevice;

		internal DisplayDevice(DISPLAY_DEVICE device)
		{
			_nativeDevice = device;

			UpdateModes();
		}

		public string DisplayString => _nativeDevice.DeviceString;
		public string DisplayName => _nativeDevice.DeviceName;

		public void UpdateModes()
		{
			Modes = new ObservableCollection<DisplayMode>(GetModes(_nativeDevice).Select(m => new DisplayMode(m)));

			try
			{
				CurrentMode = new DisplayMode(GetCurrentMode(_nativeDevice));
			}
			catch(InvalidDataException)
			{
				CurrentMode = null;
			}
		}

		public bool Attached => _nativeDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop);
		
		public bool DisconnectedOrDisabled => _nativeDevice.StateFlags == 0 || _nativeDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.Disconnect);

		private ObservableCollection<DisplayMode> _modes;
		public ObservableCollection<DisplayMode> Modes
		{
			get => _modes;
			set
			{
				if (_modes == value) return;
						
				_modes = value;
				OnPropertyChanged();
			}
		}

		private DisplayMode _currentMode;
		public DisplayMode CurrentMode
		{
			get => _currentMode;
			private set
			{
				if(ReferenceEquals(_currentMode, value)) return;
				
				_currentMode = value;
				OnPropertyChanged();
			}
		}

		/// <inheritdoc />
		public override string ToString() => $"{DisplayString}{DisplayName}";
		
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override bool Equals(object obj)
		{
			var device = obj as DisplayDevice;
			return device != null &&
				   EqualityComparer<DISPLAY_DEVICE>.Default.Equals(_nativeDevice, device._nativeDevice);
		}

		public override int GetHashCode() => 416999359 + EqualityComparer<DISPLAY_DEVICE>.Default.GetHashCode(_nativeDevice);

		public static bool operator ==(DisplayDevice device1, DisplayDevice device2) => EqualityComparer<DisplayDevice>.Default.Equals(device1, device2);
		public static bool operator !=(DisplayDevice device1, DisplayDevice device2) => !(device1 == device2);
	}
}
