using TcpIpLibrary;

namespace NTcpIpWrapper.NDistributeToClients
{
    public interface IConnexionForUSer
    {
        void SendMsg(AMsg msg);
        void SendMsg<T>(string destinataire, string motClef, T content, bool isRoutine = false) where T : new();       
        void SendMsg(string destinataire, string motClef, string txt = "", bool isRoutine = false);
        void DeclareClient(IClientForMessages client);
        EStatutConnexion StatutConnexion();
        string Name();
    }
}