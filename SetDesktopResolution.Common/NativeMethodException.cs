namespace SetDesktopResolution.Common
{
	using System;

	public class NativeMethodException : Exception
	{
		public string NativeMethodName { get; }
		
		internal NativeMethodException(string message, string nativeMethodName, Exception innerException)
			: base(message, innerException)
		{
			NativeMethodName = nativeMethodName;
		}

		internal NativeMethodException(string message, string nativeMethodName)
			: base(message)
		{
			NativeMethodName = nativeMethodName;
		}
	}

}
