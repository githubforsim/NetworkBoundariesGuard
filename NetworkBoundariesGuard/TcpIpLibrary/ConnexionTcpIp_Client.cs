using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace TcpIpLibrary
{
    public class ConnexionTcpIp_Client : ConnexionTcpIp
    {
        // ********* PUBLIC *************************************************

        public ConnexionTcpIp_Client(int port, string ipAddress) : base($"Client port {port}")
        {
            m_port = port;
            m_ipAddress = IPAddress.Parse(ipAddress);
            IsActive = true;
        }

        public override void OnTick()
        {
            if (false == IsActive)
                return;

            base.OnTick();
        }

        public override void CloseConnexion()
        {
            m_connexion?.Close();
        }

        // ********* PROTECTED **********************************************

        private readonly IPAddress m_ipAddress;

        protected override void TryToConnect()
        {
            // Cette partie a été ajoutée dans le cadre du projet Kr-PAN et elle n'existe pas
            // dans le projet PCE.
            // Son rôle est d'empêcher le message log récurrent "Exception levée : 'System.Net.Sockets.SocketException' dans System.dll" ?
            if (false == IsListenerProbablyExist())
            {
                return;
            }

            TcpClient client = new TcpClient();

            try
            {
                client.Connect(m_ipAddress, m_port);
            }
            catch
            {
            }

            if (true == client.Connected)
            {
                m_connexion = client;
                ConfigureSocket(m_connexion);
                m_stream = m_connexion.GetStream();
                OnCreateConnection();
            }
            else
            {
                OnFailToConnectAsClient();
            }
        }

        private bool IsListenerProbablyExist()
        {
            var hostName = Dns.GetHostName();
            var addressList = Dns.GetHostEntry(hostName).AddressList;
            var myIP = addressList[1].ToString();

            if (myIP == m_ipAddress.ToString() || "127.0.0.1" == m_ipAddress.ToString())
            {
                return IsLocalListenerExist();
            }
            else
            {
                return IsRemoteIpAddressActive();
            }

        }

        private bool IsLocalListenerExist()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var endPoints = properties.GetActiveTcpListeners();
            var isPortListened = false;
            foreach (var endPoint in endPoints)
            {
                if (endPoint.Port == m_port)
                {
                    isPortListened = true;
                    break;
                }
            }

            return isPortListened;
        }

        private bool IsRemoteIpAddressActive()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            try
            {
                PingReply reply = pingSender.Send(m_ipAddress, timeout, buffer, options);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        protected virtual void OnFailToConnectAsClient()
        {
        }
    }
}
