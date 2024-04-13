using NLogger;
using TcpIpLibrary;

namespace NTcpIpWrapper.DistributeAllMsgToOneClient
{
    public interface IChief : ILogger
    {
        void OnReceiveMessage(AMsg msg);

        void OnConnect();

        void OnDisconnect();
    }
}
