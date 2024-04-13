using System;
using System.Runtime.InteropServices;

namespace NTcpIpWrapper
{
    public struct SNfoConnexion
    {
        public string _name;

        public EModeConnexion _mode;

        public string _adrIP;

        public int _port;

        public SNfoConnexion(string name, EModeConnexion mode, string adrIP, int port)
        {
            _name = name;
            _mode = mode;
            _adrIP = adrIP ?? throw new ArgumentNullException(nameof(adrIP));
            _port = port;
        }

        public SNfoConnexion(string name, EModeConnexion mode, int port)
        {
            _name = name;
            _mode = mode;
            _adrIP = string.Empty;
            _port = port;
        }
    }
}
