using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetDesktopResolution.Common.Wmi
{
	public class Win32ProcessEventArgs : EventArgs
	{
		public enum InstanceEventType
		{
			Other = 0,
			Create = 1,
			Modify = 2,
			Delete = 3
		}

		public InstanceEventType EventType { get; }
		
		public Win32Process Process { get; }
		
		internal Win32ProcessEventArgs(Win32Process process, InstanceEventType t)
		{
			Process = process;
			EventType = t;
		}
	}
}
