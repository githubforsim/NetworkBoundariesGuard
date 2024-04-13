using System.Collections.Generic;

namespace NTcpIpWrapper.NDistributeToClients
{
    public interface IConnexionForOwner : IConnexionForUSer
    {
        void Start();

        void Core();

        void Close();

        void SetFiltersForLogs(IList<string> filters);
    }
}