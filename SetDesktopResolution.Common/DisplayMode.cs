namespace SetDesktopResolution.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using static NativeMethods;

	public class DisplayMode
	{
		public class Size
		{
			internal Size() : this(0, 0)
			{
			}

			internal Size(int x, int y)
			{
				Width = x;
				Height = y;
			}
			
			public int Width { get; }
			public int Height { get; }

			public override bool Equals(object obj)
			{
				var size = obj as Size;
				return size != null &&
					   Width == size.Width &&
					   Height == size.Height;
			}

			public override int GetHashCode()
			{
				var hashCode = 859600377;
				hashCode = hashCode * -1521134295 + Width.GetHashCode();
				hashCode = hashCode * -1521134295 + Height.GetHashCode();
				return hashCode;
			}

			public static bool operator ==(Size size1, Size size2) => EqualityComparer<Size>.Default.Equals(size1, size2);
			public static bool operator !=(Size size1, Size size2) => !(size1 == size2);
		}
		
		public enum ScalingType
		{
			Default = FixedOutputMode.DMDFO_DEFAULT,
			Centered = FixedOutputMode.DMDFO_CENTER,
			Stretched = FixedOutputMode.DMDFO_STRETCH
		}

		public enum Rotation
		{
			Default = DisplayOrientation.DMDO_DEFAULT,
			Rotate90 = DisplayOrientation.DMDO_90,
			Rotate180 = DisplayOrientation.DMDO_180,
			Rotate270 = DisplayOrientation.DMDO_270
		}
		
		internal readonly DEVMODE NativeMode;
		
		internal DisplayMode(DEVMODE mode)
		{
			NativeMode = mode;
		}

		public int Bpp => NativeMode.dmBitsPerPel;
		public Size Resolution => new Size(NativeMode.dmPelsWidth, NativeMode.dmPelsHeight);
		public int RefreshRate => NativeMode.dmDisplayFrequency;
		public int Ppi => NativeMode.dmLogPixels;
		public ScalingType ScalingMode => (ScalingType)NativeMode.dmDisplayFixedOutput;
		public Rotation Orientation => (Rotation)NativeMode.dmDisplayOrientation;
		
		/// <summary>
		/// Mode is interlaced. Maybe unsupported?
		/// </summary>
		public bool Interlaced => NativeMode.dmDisplayFlags.HasFlag(DisplayFlags.DM_INTERLACED);

		/// <inheritdoc />
		public override string ToString() => $"{Resolution.Width}x{Resolution.Height}{(Interlaced ? "i" : "")}@{RefreshRate}Hz ({Bpp}bit)";

		public override bool Equals(object obj)
		{
			return obj is DisplayMode mode &&
				   Bpp == mode.Bpp &&
				   EqualityComparer<Size>.Default.Equals(Resolution, mode.Resolution) &&
				   RefreshRate == mode.RefreshRate &&
				   Ppi == mode.Ppi &&
				   Orientation == mode.Orientation &&
				   Interlaced == mode.Interlaced;
		}

		public override int GetHashCode()
		{
			var hashCode = 948579849;
			hashCode = hashCode * -1521134295 + Bpp.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Size>.Default.GetHashCode(Resolution);
			hashCode = hashCode * -1521134295 + RefreshRate.GetHashCode();
			hashCode = hashCode * -1521134295 + Ppi.GetHashCode();
			hashCode = hashCode * -1521134295 + Orientation.GetHashCode();
			hashCode = hashCode * -1521134295 + Interlaced.GetHashCode();
			return hashCode;
		}
	}
}
