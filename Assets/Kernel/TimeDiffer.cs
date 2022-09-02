using System;

namespace GameKit.Kernel
{
    public class TimeDiffer
    {
        public long starTime;
        public TimeDiffer()
        {
            starTime = System.DateTime.Now.Ticks;
        }

        ///时间差
        public long GetDiffTicks()
        {
            return 0;
        }

        ///毫秒级的时间差字符串：100ms
        public string GetDiffMsStr()
        {
            return GetDiffTicks() / 10000000 + "ms";
        }
    }
}