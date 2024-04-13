using System.Collections.Generic;
using TcpIpLibrary;

namespace NTcpIpWrapper.DistributeAllMsgToOneClient
{
    public interface IConnexionForOwner : IConnexionForUSer
    {
        void Close();

        void Core();

        void Start();

        void SetFiltersForLogs(IList<string> filters);

        ELogSendReceivedMsgStatut ModeOfLogForSentMsg { get; set; }

        ELogSendReceivedMsgStatut ModeOfLogForReceivedMsg { get; set; }
    }
}
