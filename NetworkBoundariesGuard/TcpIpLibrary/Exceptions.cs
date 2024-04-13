using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TcpIpLibrary
{
    public class UnknownPortException : SystemException
    {
        public int m_port;

        public UnknownPortException(int port) : base()
        {
            m_port = port;
        }
    }
}
