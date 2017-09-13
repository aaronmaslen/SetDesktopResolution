namespace SetDesktopResolution.Common.Wmi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Management;
	using System.Text;
	using System.Threading.Tasks;

	using JetBrains.Annotations;

	using Serilog;

	public class Win32Process
	{
		internal Win32Process([NotNull] ManagementBaseObject targetInstance)
		{
			TargetInstance = targetInstance;
			
			foreach (var p in Properties)
				Log.Logger.Verbose("[{TargetInstance}] {Property}: {Value}", TargetInstance, p.Key, p.Value.Value);
		}

		[NotNull]
		protected ManagementBaseObject TargetInstance { get; }

		protected internal Dictionary<string, PropertyData> Properties => TargetInstance
			.SystemProperties.Cast<PropertyData>().Concat(TargetInstance.Properties.Cast<PropertyData>())
			.ToDictionary(p => p.Name, p => p);
		
		public string ExecutablePath => Properties["ExecutablePath"].Value as string;

		public string Name => Properties["Name"].Value as string;

		public long ProcessId => (long?)(Properties["ProcessId"].Value as uint?) ?? -1;

		public long ParentProcessId => (long?)(Properties["ParentProcessId"].Value as uint?) ?? -1;

		public string Status => Properties["Status"].Value as string;
	}
}
