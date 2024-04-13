using NLogger;
using TcpIpLibrary;

namespace NTcpIpWrapper.NDistributeToClients
{
    public interface IChief
    {
        void Log(string msg, LogLevel level = LogLevel.INFO);

        bool ArePingFiltered();

        void OnReceivedMessageWithNoClientIdentified(AMsg msg);

        void OnConnect();

        void OnDisconnect();
    }
}
