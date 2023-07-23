using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Kernel.Unity
{
    class PlatformUtils
    {
        public static string GetPlatformName()
        {
#if UNITY_IPHONE
        return GetIOSPlatformName(SystemInfo.deviceModel);
#else
            return SystemInfo.deviceModel;
#endif
        }
        static string GetIOSPlatformName(string platform)
        {
            if (platform.Equals(@"iPhone1,1")) return @"iPhone 2G";
            if (platform.Equals(@"iPhone1,2")) return @"iPhone 3G";
            if (platform.Equals(@"iPhone2,1")) return @"iPhone 3GS";

            if (platform.Equals(@"iPhone3,1")) return @"iPhone 4";
            if (platform.Equals(@"iPhone3,2")) return @"iPhone 4";
            if (platform.Equals(@"iPhone3,3")) return @"iPhone 4";
            if (platform.Equals(@"iPhone4,1")) return @"iPhone 4S";

            if (platform.Equals(@"iPhone5,1")) return @"iPhone 5";
            if (platform.Equals(@"iPhone5,2")) return @"iPhone 5";

            if (platform.Equals(@"iPhone5,3")) return @"iPhone 5c";
            if (platform.Equals(@"iPhone5,4")) return @"iPhone 5c";

            if (platform.Equals(@"iPhone6,1")) return @"iPhone 5s";
            if (platform.Equals(@"iPhone6,2")) return @"iPhone 5s";

            if (platform.Equals(@"iPhone7,1")) return @"iPhone 6 Plus";
            if (platform.Equals(@"iPhone7,2")) return @"iPhone 6";

            if (platform.Equals(@"iPhone8,1")) return @"iPhone 6S Plus";
            if (platform.Equals(@"iPhone8,2")) return @"iPhone 6S";

            if (platform.Equals(@"iPod1,1")) return @"iPod Touch 1G";
            if (platform.Equals(@"iPod2,1")) return @"iPod Touch 2G";
            if (platform.Equals(@"iPod3,1")) return @"iPod Touch 3G";
            if (platform.Equals(@"iPod4,1")) return @"iPod Touch 4G";
            if (platform.Equals(@"iPod5,1")) return @"iPod Touch 5G";

            if (platform.Equals(@"iPad1,1")) return @"iPad 1G";

            if (platform.Equals(@"iPad2,1")) return @"iPad 2";
            if (platform.Equals(@"iPad2,2")) return @"iPad 2";
            if (platform.Equals(@"iPad2,3")) return @"iPad 2";
            if (platform.Equals(@"iPad2,4")) return @"iPad 2";

            if (platform.Equals(@"iPad2,5")) return @"iPad Mini 1G";
            if (platform.Equals(@"iPad2,6")) return @"iPad Mini 1G";
            if (platform.Equals(@"iPad2,7")) return @"iPad Mini 1G";

            if (platform.Equals(@"iPad3,1")) return @"iPad 3";
            if (platform.Equals(@"iPad3,2")) return @"iPad 3";
            if (platform.Equals(@"iPad3,3")) return @"iPad 3";
            if (platform.Equals(@"iPad3,4")) return @"iPad 4";
            if (platform.Equals(@"iPad3,5")) return @"iPad 4";
            if (platform.Equals(@"iPad3,6")) return @"iPad 4";

            if (platform.Equals(@"iPad4,1")) return @"iPad Air";
            if (platform.Equals(@"iPad4,2")) return @"iPad Air";
            if (platform.Equals(@"iPad4,3")) return @"iPad Air";

            if (platform.Equals(@"iPad4,4")) return @"iPad Mini 2G ";
            if (platform.Equals(@"iPad4,5")) return @"iPad Mini 2G ";
            if (platform.Equals(@"iPad4,6")) return @"iPad Mini 2G ";

            if (platform.Equals(@"iPad4,7")) return @"iPad Mini 3 ";
            if (platform.Equals(@"iPad4,8")) return @"iPad Mini 3 ";
            if (platform.Equals(@"iPad4,9")) return @"iPad Mini 3 ";

            if (platform.Equals(@"iPad5,1")) return @"iPad Mini 4 WiFi  ";
            if (platform.Equals(@"iPad5,2")) return @"iPad Mini 4 WiFi+Cellular ";

            if (platform.Equals(@"iPad5,3")) return @"iPad Air2 ";
            if (platform.Equals(@"iPad5,4")) return @"iPad Air2 ";

            if (platform.Equals(@"iPad6,7")) return @"iPad Pro WiFi ";
            if (platform.Equals(@"iPad6,8")) return @"iPad Pro WiFi+Cellular";

            if (platform.Equals(@"i386")) return @"iPhone Simulator";
            if (platform.Equals(@"x86_64")) return @"iPhone Simulator";

            return platform;
        }
    }
}
