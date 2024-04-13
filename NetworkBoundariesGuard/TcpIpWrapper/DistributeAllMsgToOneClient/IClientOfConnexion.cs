using TcpIpLibrary;

namespace NTcpIpWrapper.DistributeAllMsgToOneClient
{
    public interface IClientOfConnexion
    {
        string Name { get; }

        string AdressIP { get; }

        int PortConnexion { get; }

        void OnReceiveMessage(AMsg msg);

        void OnConnect();

        void OnDisconnect();
    }
}
