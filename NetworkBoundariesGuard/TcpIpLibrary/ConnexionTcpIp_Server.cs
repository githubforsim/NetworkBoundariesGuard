using System;
using System.Net;
using System.Net.Sockets;

namespace TcpIpLibrary
{
    public class ConnexionTcpIp_Server : ConnexionTcpIp
    {
        public ConnexionTcpIp_Server(int port) : base($"Server port {port}")
        {
            m_port = port;
            m_listener = new TcpListener(IPAddress.Any, port);
            if (null == m_listener)
            {
                throw new ApplicationException("Constructor of PortController is Called with an incorrect port");
            }

            m_listener.Start();
        }

        public override void CloseConnexion()
        {
            m_listener.Stop();
        }

        // ********* PROTECTED **********************************************

        protected readonly TcpListener m_listener;

        protected override void TryToConnect()
        {
            if (false == m_listener.Pending())
                return;

            m_connexion = m_listener.AcceptTcpClient();
            ConfigureSocket(m_connexion);
            m_stream = m_connexion.GetStream();
            OnCreateConnection();
        }
    }
}
