using System;
using System.Runtime.InteropServices;

namespace TcpIpLibrary
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct HeadOfMsg
    {
        public const int TXT_MEDIUMSIZE = 50;

        public ulong _id;

        public long _time_ms;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 150)]
        public string _typeOfContent; // AssemblyQualifiedName
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = TXT_MEDIUMSIZE)]
        public string _destinataire;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = TXT_MEDIUMSIZE)]
        public string _motClef;

        public int _sizeOfContentInBytes;

        public bool _isRoutine;
            
        public HeadOfMsg(string destinataire, string motClef, bool isRoutine)
        {
            _id = 0;
            _typeOfContent = string.Empty;
            _destinataire = destinataire;
            _motClef = motClef;
            _sizeOfContentInBytes = 0;
            _isRoutine = isRoutine;
            _time_ms = 0;
        }

        public override string ToString()
        {
            return $"Head : id = {_id} type = {_typeOfContent} FOR {_destinataire} MotClef = {_motClef} sizeInBytes = {_sizeOfContentInBytes}";
        }
    }
}