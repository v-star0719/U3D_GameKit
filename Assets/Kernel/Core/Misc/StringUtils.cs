using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Core
{
    class StringUtils
    {
        public static bool IsTelePhone(string str_handset)
        {
            return !string.IsNullOrEmpty(str_handset) && str_handset.Length == 11 && System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+\d{10}");
        }

        public static bool IsNumber(string str_number)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_number, @"^[0-9]*$");
        }

        public static bool IsChinese(string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"[\u4e00-\u9fa5]+$");
        }

        public static string TruncateString(string orgText, int maxCharactorCount, string suffix)
        {
            int n = 0;
            const char c = (char)255;
            for (int i = 0; i < orgText.Length; i++)
            {
                if (orgText[i] < c)
                    n++;
                else
                    n += 2;
                if (n > maxCharactorCount)
                    return orgText.Substring(0, i) + suffix;//取子字符串
            }
            return orgText;//原来的字数没超，原样返回
        }

        /// <summary>
        /// Urlencode 如果是中文需要两次encode,服务器再主动调用一次decode,
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            return Uri.EscapeDataString(str);
        }
    }
}
