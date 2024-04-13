using System;
using System.Runtime.InteropServices;

namespace TcpIpLibrary
{
    [StructLayout(LayoutKind.Sequential)]
    public class ListOfStrings
    {
        public string GetTypeOfContent()
        {
            return "ListOfStrings";
        }

        public const int _sizeMaxOfArray = 50;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = _sizeMaxOfArray)]
        public SimpleTxt[] _array = new SimpleTxt[_sizeMaxOfArray];

        public int _nbOfElements;

        public static ListOfStrings Create(string[] array)
        {
            var output = new ListOfStrings();
            output._nbOfElements = Math.Min(_sizeMaxOfArray, array.Length);
            for (var i = 0; i < output._nbOfElements; i++)
            {
                output._array[i] = new SimpleTxt(array[i]);
            }

            return output;
        }

        public string[] GetArray()
        {
            var output = new string[_nbOfElements];
            for (var i = 0; i < _nbOfElements; i++)
            {
                output[i] = _array[i]._txt;
            }

            return output;
        }
    }
}