using System;
using System.Runtime.InteropServices;

namespace TcpIpLibrary
{
    public class Msg<T> : AMsg where T : new()
    {
        public Msg(string destinataire, string motClef, T datas, bool isRoutine = true)
            : base(destinataire, motClef, isRoutine)
        {
            _datas = datas;
            _head._typeOfContent = typeof(T).AssemblyQualifiedName;
            _head._sizeOfContentInBytes = Marshal.SizeOf(typeof(T));
        }

        public T _datas;

        public override string ToString()
        {
            if (typeof(T).ToString().Contains("SimpleTxt"))
            {
                return $"destinataire = {_head._destinataire}" +
                    $" - mot clef = {_head._motClef} - Contenu = {GetContentAsSimpleText()}" +
                    $" - routine = " + (_head._isRoutine ? "oui" : "non");
            }
            else
            {
                string txt = "destinataire = " + _head._destinataire
                    + ", mot clef = " + _head._motClef
                    + ", routine = " + (_head._isRoutine ? "oui" : "non")
                    + ", msg de type " + typeof(T).ToString() + " = ";
                var size = Marshal.SizeOf(typeof(T));
                if (size > 100)
                {
                    return txt + " ... too long (> 100 octets) ...";
                }
                else
                {
                    return txt + _datas.ToString();
                }
            }
        }

        public override byte[] AsBytesArray()
        {
            var sizeOfHead = Marshal.SizeOf(typeof(HeadOfMsg));
            var size = sizeOfHead + _head._sizeOfContentInBytes;
            IntPtr arrPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(_head, arrPtr, false);
            Marshal.StructureToPtr(_datas, arrPtr + sizeOfHead, false);

            var buffer = new byte[size];
            Marshal.Copy(arrPtr, buffer, 0, size);
            Marshal.FreeHGlobal(arrPtr);

            return buffer;
        }

        // *********** ICustomMsg **************

        public Msg(HeadOfMsg head) : base(head)
        {
        }

        public Msg(HeadOfMsg head, byte[] buffer) : base(head)
        {
            var size = Marshal.SizeOf(typeof(T));
            IntPtr arrPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, arrPtr, size);

            _datas = Marshal.PtrToStructure<T>(arrPtr);
        }
    }

}