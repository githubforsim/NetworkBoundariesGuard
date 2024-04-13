namespace TcpIpLibrary
{
    public class ConnexionToClientNfo
    {
        public ConnexionToClientNfo(int port, EStatutConnexion status)
        {
            m_port = port;
            m_status = status;
        }

        public int m_port;
        public EStatutConnexion m_status;
    }
}
