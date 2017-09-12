namespace SetDesktopResolution.Common.Wmi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Management;
	using System.Text;
	using System.Threading.Tasks;

	using JetBrains.Annotations;

	public class Win32Process
	{
		internal Win32Process([NotNull] ManagementBaseObject targetInstance)
		{
			TargetInstance = targetInstance;
		}

		[NotNull]
		protected ManagementBaseObject TargetInstance { get; }

		protected internal Dictionary<string, PropertyData> Properties => TargetInstance
			.SystemProperties.Cast<PropertyData>().Concat(TargetInstance.Properties.Cast<PropertyData>())
			.ToDictionary(p => p.Name, p => p);
		
		public string ExecutablePath => Properties["ExecutablePath"].Value as string;

		public string Name => Properties["Name"].Value as string;

		public int ProcessId => Properties["ProcessId"].Value as int? ?? -1;

		public int ParentProcessId => Properties["ParentProcessId"].Value as int? ?? -1;

		public string Status => Properties["Status"].Value as string;
	}
}
