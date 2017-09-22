namespace SetDesktopResolution.Common.Wmi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum ProcessDetectionMode
    {
        SingleProcess,
        ProcessPlusChildren,
        RunningProcess
    }
}
