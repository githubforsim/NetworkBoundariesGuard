using System;
using System.Runtime.InteropServices;

namespace TcpIpLibrary
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SimpleTxt
    {
        public const int TXT_MEDIUMSIZE = 100;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = TXT_MEDIUMSIZE)]
        public string _txt;

        public string GetTypeOfDatas()
        {
            return "SIMPLE_TXT";
        }

        public SimpleTxt(string txt)
        {
            if (txt.Length > TXT_MEDIUMSIZE)
            {
                _txt = txt.Substring(0, TXT_MEDIUMSIZE) ?? throw new ArgumentNullException(nameof(txt));
            }
            else
            {
                _txt = txt ?? throw new ArgumentNullException(nameof(txt));
            }
        }

        public override string ToString()
        {
            return _txt;
        }
    }
}