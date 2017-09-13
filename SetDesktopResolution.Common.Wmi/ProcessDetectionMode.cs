using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetDesktopResolution.Common.Wmi
{
    public enum ProcessDetectionMode
    {
        SingleProcess,
        ProcessPlusChildren,
        RunningProcess
    }
}
