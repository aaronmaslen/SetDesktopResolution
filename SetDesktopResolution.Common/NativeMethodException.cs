namespace SetDesktopResolution.Common
{
	using System;

	public class NativeMethodException : Exception
	{
		public string NativeMethodName { get; }

		internal object ExceptionData { get; }
		
		internal Type ExceptionDataType { get; }
		
		internal NativeMethodException(string message, Exception innerException, string nativeMethodName)
			: base(message, innerException)
		{
			NativeMethodName = nativeMethodName;
		}

		internal NativeMethodException(string message, string nativeMethodName)
			: base(message)
		{
			NativeMethodName = nativeMethodName;
		}

		internal NativeMethodException(string message, string nativeMethodName, object o, Type t)
			: this(message, nativeMethodName)
		{
			ExceptionData = o;
			ExceptionDataType = t;
		}

		internal NativeMethodException(
			string message, Exception innerException, string nativeMethodName, object o, Type t)
			: this(message, innerException, nativeMethodName)
		{
			ExceptionData = o;
			ExceptionDataType = t;
		}
	}
}
