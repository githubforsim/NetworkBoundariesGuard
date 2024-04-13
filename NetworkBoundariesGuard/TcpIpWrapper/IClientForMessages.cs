using TcpIpLibrary;

namespace NTcpIpWrapper
{
    public interface IMsgReceiver
    {
        void OnReceivedMessage(AMsg msg);
    }

    public interface IClientForMessages : IMsgReceiver
    {
        string NameAsDestinataire { get; }

        void OnConnect();

        void OnDisconnect();
    }
}
