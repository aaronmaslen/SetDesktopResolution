namespace SetDesktopResolution.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using JetBrains.Annotations;

	using Serilog;

	[SuppressMessage("ReSharper", "EnumUnderlyingTypeIsInt")]
	internal static class NativeMethods
	{
		[Flags]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public enum DisplayDeviceStateFlags : int
		{
			/// <summary>The device is part of the desktop.</summary>
			AttachedToDesktop = 0x1,
			MultiDriver = 0x2,
			/// <summary>The device is part of the desktop.</summary>
			PrimaryDevice = 0x4,
			/// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
			MirroringDriver = 0x8,
			/// <summary>The device is VGA compatible.</summary>
			VGACompatible = 0x10,
			/// <summary>The device is removable; it cannot be the primary display.</summary>
			Removable = 0x20,
			/// <summary>The device has more display modes than its output devices support.</summary>
			ModesPruned = 0x8000000,
			Remote = 0x4000000,
			Disconnect = 0x2000000
		}

		[Flags]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public enum DisplayFlags : int
		{
			DM_GRAYSCALE = 0x1,
			DM_INTERLACED = 0x2
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public enum FixedOutputMode : int
		{
			DMDFO_DEFAULT = 0x0,
			DMDFO_STRETCH = 0x1,
			DMDFO_CENTER = 0x2
			
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public enum DisplayOrientation : int
		{
			DMDO_DEFAULT = 0,
			DMDO_90 = 1,
			DMDO_180 = 2,
			DMDO_270 = 3
		}
		

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public struct DISPLAY_DEVICE
		{
			[MarshalAs(UnmanagedType.U4)]
			public int cb;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DeviceName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceString;
			[MarshalAs(UnmanagedType.U4)]
			public DisplayDeviceStateFlags StateFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceID;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceKey;

			public override bool Equals(object obj)
			{
				if (!(obj is DISPLAY_DEVICE))
				{
					return false;
				}

				var dEVICE = (DISPLAY_DEVICE)obj;
				return cb == dEVICE.cb &&
					   DeviceName == dEVICE.DeviceName &&
					   DeviceString == dEVICE.DeviceString &&
					   StateFlags == dEVICE.StateFlags &&
					   DeviceID == dEVICE.DeviceID &&
					   DeviceKey == dEVICE.DeviceKey;
			}

			[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
			public override int GetHashCode()
			{
				var hashCode = 2140488231;
				hashCode = hashCode * -1521134295 + base.GetHashCode();
				hashCode = hashCode * -1521134295 + cb.GetHashCode();
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeviceName);
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeviceString);
				hashCode = hashCode * -1521134295 + StateFlags.GetHashCode();
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeviceID);
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeviceKey);
				return hashCode;
			}

			public static bool operator ==(DISPLAY_DEVICE dEVICE1, DISPLAY_DEVICE dEVICE2) => dEVICE1.Equals(dEVICE2);
			public static bool operator !=(DISPLAY_DEVICE dEVICE1, DISPLAY_DEVICE dEVICE2) => !(dEVICE1 == dEVICE2);
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public struct DEVMODE
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmDeviceName;

			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;
			public int dmPositionX;
			public int dmPositionY;
			public DisplayOrientation dmDisplayOrientation;
			public FixedOutputMode dmDisplayFixedOutput;
			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmFormName;

			public short dmLogPixels;
			public short dmBitsPerPel;
			public int dmPelsWidth;
			public int dmPelsHeight;
			public DisplayFlags dmDisplayFlags;
			public int dmDisplayFrequency;
			public int dmICMMethod;
			public int dmICMIntent;
			public int dmMediaType;
			public int dmDitherType;
			public int dmReserved1;
			public int dmReserved2;
			public int dmPanningWidth;
			public int dmPanningHeight;

			public override bool Equals(object obj)
			{
				if (!(obj is DEVMODE devmode))
				{
					return false;
				}

				return dmDeviceName == devmode.dmDeviceName &&
					   dmSpecVersion == devmode.dmSpecVersion &&
					   dmDriverVersion == devmode.dmDriverVersion &&
					   dmSize == devmode.dmSize &&
					   dmDriverExtra == devmode.dmDriverExtra &&
					   dmFields == devmode.dmFields &&
					   dmPositionX == devmode.dmPositionX &&
					   dmPositionY == devmode.dmPositionY &&
					   dmDisplayOrientation == devmode.dmDisplayOrientation &&
					   dmDisplayFixedOutput == devmode.dmDisplayFixedOutput &&
					   dmColor == devmode.dmColor &&
					   dmDuplex == devmode.dmDuplex &&
					   dmYResolution == devmode.dmYResolution &&
					   dmTTOption == devmode.dmTTOption &&
					   dmCollate == devmode.dmCollate &&
					   dmFormName == devmode.dmFormName &&
					   dmLogPixels == devmode.dmLogPixels &&
					   dmBitsPerPel == devmode.dmBitsPerPel &&
					   dmPelsWidth == devmode.dmPelsWidth &&
					   dmPelsHeight == devmode.dmPelsHeight &&
					   dmDisplayFlags == devmode.dmDisplayFlags &&
					   dmDisplayFrequency == devmode.dmDisplayFrequency &&
					   dmICMMethod == devmode.dmICMMethod &&
					   dmICMIntent == devmode.dmICMIntent &&
					   dmMediaType == devmode.dmMediaType &&
					   dmDitherType == devmode.dmDitherType &&
					   dmReserved1 == devmode.dmReserved1 &&
					   dmReserved2 == devmode.dmReserved2 &&
					   dmPanningWidth == devmode.dmPanningWidth &&
					   dmPanningHeight == devmode.dmPanningHeight;
			}

			[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
			public override int GetHashCode()
			{
				var hashCode = -586899440;
				hashCode = hashCode * -1521134295 + base.GetHashCode();
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(dmDeviceName);
				hashCode = hashCode * -1521134295 + dmSpecVersion.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDriverVersion.GetHashCode();
				hashCode = hashCode * -1521134295 + dmSize.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDriverExtra.GetHashCode();
				hashCode = hashCode * -1521134295 + dmFields.GetHashCode();
				hashCode = hashCode * -1521134295 + dmPositionX.GetHashCode();
				hashCode = hashCode * -1521134295 + dmPositionY.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDisplayOrientation.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDisplayFixedOutput.GetHashCode();
				hashCode = hashCode * -1521134295 + dmColor.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDuplex.GetHashCode();
				hashCode = hashCode * -1521134295 + dmYResolution.GetHashCode();
				hashCode = hashCode * -1521134295 + dmTTOption.GetHashCode();
				hashCode = hashCode * -1521134295 + dmCollate.GetHashCode();
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(dmFormName);
				hashCode = hashCode * -1521134295 + dmLogPixels.GetHashCode();
				hashCode = hashCode * -1521134295 + dmBitsPerPel.GetHashCode();
				hashCode = hashCode * -1521134295 + dmPelsWidth.GetHashCode();
				hashCode = hashCode * -1521134295 + dmPelsHeight.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDisplayFlags.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDisplayFrequency.GetHashCode();
				hashCode = hashCode * -1521134295 + dmICMMethod.GetHashCode();
				hashCode = hashCode * -1521134295 + dmICMIntent.GetHashCode();
				hashCode = hashCode * -1521134295 + dmMediaType.GetHashCode();
				hashCode = hashCode * -1521134295 + dmDitherType.GetHashCode();
				hashCode = hashCode * -1521134295 + dmReserved1.GetHashCode();
				hashCode = hashCode * -1521134295 + dmReserved2.GetHashCode();
				hashCode = hashCode * -1521134295 + dmPanningWidth.GetHashCode();
				hashCode = hashCode * -1521134295 + dmPanningHeight.GetHashCode();
				return hashCode;
			}

			public static bool operator ==(DEVMODE mode1, DEVMODE mode2) => mode1.Equals(mode2);
			public static bool operator !=(DEVMODE mode1, DEVMODE mode2) => !(mode1 == mode2);
		}

		[Flags]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public enum ChangeDisplaySettingsFlags : uint
		{
			CDS_NONE = 0,
			CDS_UPDATEREGISTRY = 0x00000001,
			CDS_TEST = 0x00000002,
			CDS_FULLSCREEN = 0x00000004,
			CDS_GLOBAL = 0x00000008,
			CDS_SET_PRIMARY = 0x00000010,
			CDS_VIDEOPARAMETERS = 0x00000020,
			CDS_ENABLE_UNSAFE_MODES = 0x00000100,
			CDS_DISABLE_UNSAFE_MODES = 0x00000200,
			CDS_RESET = 0x40000000,
			CDS_RESET_EX = 0x20000000,
			CDS_NORESET = 0x10000000
		}

		// ReSharper disable once InconsistentNaming
		public enum DISP_CHANGE : int
		{
			Successful = 0,
			Restart = 1,
			Failed = -1,
			BadMode = -2,
			NotUpdated = -3,
			BadFlags = -4,
			BadParam = -5,
			BadDualView = -6
		}

		// ReSharper disable once InconsistentNaming
		public const int ENUM_CURRENT_SETTINGS = -1;

		// ReSharper disable once InconsistentNaming
		public const uint EDS_RAWMODE = 2;

		[DllImport("user32.dll")]
		public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

		[DllImport("user32.dll")]
		public static extern bool EnumDisplaySettingsEx(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode, uint dwFlags);

		[DllImport("user32.dll")]
		public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

		public static void SetMode(DISPLAY_DEVICE dev, DEVMODE mode)
		{
			var result = ChangeDisplaySettingsEx(dev.DeviceName, ref mode, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_FULLSCREEN, IntPtr.Zero);
			if(result != DISP_CHANGE.Successful)
				throw new NativeMethodException($"Setting mode failed: {result} returned", nameof(ChangeDisplaySettingsEx));
		}

		internal static DEVMODE NewDevMode()
		{
			var devmode = new DEVMODE
				              {
					              dmDeviceName = new string(new char[32]),
				                  dmFormName = new string(new char[32])
				              };

			devmode.dmSize = (short)Marshal.SizeOf(devmode);

			return devmode;
		}

		internal static DISPLAY_DEVICE NewDisplayDevice()
		{
			var dev = new DISPLAY_DEVICE
				          {
					          DeviceName = new string(new char[32]),
							  DeviceString = new string(new char[128]),
							  DeviceID = new string(new char[128]),
							  DeviceKey = new string(new char[128])
				          };

			dev.cb = Marshal.SizeOf(dev);

			return dev;
		}

		[NotNull]
		public static IEnumerable<DISPLAY_DEVICE> GetDevices()
		{
			for(uint i = 0; ; i++)
			{
				var dev = NewDisplayDevice();

				if(!EnumDisplayDevices(null, i, ref dev, 1))
					yield break;

				yield return dev;
			}
		}

		[NotNull]
		public static IEnumerable<DEVMODE> GetModes(DISPLAY_DEVICE device)
		{
			Log.Logger.Debug("DeviceString: \"{DeviceString}\", DeviceName: \"{DeviceName}\"", device.DeviceString, device.DeviceName);
			
			for(int i = 0;; i++)
			{
				var devMode = NewDevMode();

				if (!EnumDisplaySettingsEx(device.DeviceName, i, ref devMode, EDS_RAWMODE))
					yield break;
				
				Log.Logger.Debug("DEVMODE.DeviceName: \"{DeviceName}\", Mode: {Width}x{Height} {Frequency}Hz", devMode.dmDeviceName, devMode.dmPelsWidth, devMode.dmPelsHeight, devMode.dmDisplayFrequency);

				yield return devMode;
			}
		}

		public static DEVMODE GetCurrentMode(DISPLAY_DEVICE device)
		{
			var devMode = NewDevMode();

			if(!EnumDisplaySettingsEx(device.DeviceName, ENUM_CURRENT_SETTINGS, ref devMode, EDS_RAWMODE))
				throw new NativeMethodException("Unknown error occurred. Display disconnected?", nameof(EnumDisplaySettingsEx));

			return devMode;
		}

		public static Dictionary<DISPLAY_DEVICE, ICollection<DEVMODE>> GetAllModes()
		{
			return GetDevices().ToDictionary<DISPLAY_DEVICE, DISPLAY_DEVICE, ICollection<DEVMODE>>(
				device => device,
				device => GetModes(device).ToList());
		}
	}
}
