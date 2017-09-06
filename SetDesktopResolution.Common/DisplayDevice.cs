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
	using JetBrains.Annotations;

	using Serilog;

	using static NativeMethods;

	public class DisplayDevice : INotifyPropertyChanged
	{
		public static IEnumerable<DisplayDevice> GetDisplayDevices() => GetDevices().Select(d => new DisplayDevice(d));
		
		internal readonly DISPLAY_DEVICE NativeDevice;

		internal DisplayDevice(DISPLAY_DEVICE device)
		{
			NativeDevice = device;

			UpdateModes();
		}

		public string DisplayString => NativeDevice.DeviceString;
		public string DisplayName => NativeDevice.DeviceName;

		public void UpdateModes()
		{
			Modes = new ObservableCollection<DisplayMode>(GetModes(NativeDevice).Select(m => new DisplayMode(m)));
		}

		public bool Attached => NativeDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop);
		
		public bool DisconnectedOrDisabled => NativeDevice.StateFlags == 0 || NativeDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.Disconnect);

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
		
		public DisplayMode CurrentMode
		{
			get
			{
				try
				{
					return new DisplayMode(GetCurrentMode(NativeDevice));
				}
				catch (NativeMethodException nme)
				{
					Log.Logger.Error(nme.ToString());

					throw;
				}
			}
		}

		public void SetMode(DisplayMode mode)
		{
			if (!Modes.Contains(mode))
				throw new ArgumentException("Unsupported mode requested", nameof(mode));

			NativeMethods.SetMode(NativeDevice, mode.NativeMode);
			
			OnPropertyChanged(nameof(CurrentMode));
		}

		/// <inheritdoc />
		public override string ToString() => $"{DisplayString}{DisplayName}";
		
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public override bool Equals(object obj)
		{
			var device = obj as DisplayDevice;
			return device != null &&
				   EqualityComparer<DISPLAY_DEVICE>.Default.Equals(NativeDevice, device.NativeDevice);
		}

		public override int GetHashCode() => 416999359 + EqualityComparer<DISPLAY_DEVICE>.Default.GetHashCode(NativeDevice);

		public static bool operator ==(DisplayDevice device1, DisplayDevice device2) => EqualityComparer<DisplayDevice>.Default.Equals(device1, device2);
		public static bool operator !=(DisplayDevice device1, DisplayDevice device2) => !(device1 == device2);
	}
}
