using System;
using System.Linq;

namespace TcpIpLibrary
{
    public abstract class AMsg
    {
        // *** PUBLIC *******************************

        public int GetSizeInBytes()
        {
            unsafe
            {
                return _head._sizeOfContentInBytes;
            }
        }

        public new abstract string ToString();

        public unsafe void ChangeId(ulong id)
        {
            _head._id = id;
        }

        public ulong Id => _head._id;

        public abstract byte[] AsBytesArray();

        public bool IsRoutine => _head._isRoutine;

        public string MotClef => _head._motClef;

        public string Destinataire => _head._destinataire;

        public long GetDeliveryTime_ms()
        {
            if (0 == _head._time_ms)
            {
                return 0;
            }

            return (GetCurrentTime_ms() - _head._time_ms);
        }

        public void UpdateTime()
        {
            _head._time_ms = GetCurrentTime_ms();
        }

        public bool IsContentSimpleText()
        {
            return Type.GetType(_head._typeOfContent) == typeof(SimpleTxt);
            //return (_head._typeOfContent == typeof(SimpleTxt).AssemblyQualifiedName);
        }

        public string GetContentAsSimpleText()
        {
            return ((Msg<SimpleTxt>)this)._datas._txt;
        }

        // *** RESTRICTED ***************************

        public HeadOfMsg _head;

        protected AMsg(string destinataire, string motClef, bool isRoutine)
        {
            _head = new HeadOfMsg(destinataire, motClef, isRoutine);
        }

        protected AMsg(HeadOfMsg head)
        {
            _head = head;
        }

        public static long GetCurrentTime_ms()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}