using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpIpLibrary
{
    public interface IMessageSender
    {
        void SendMessage(string destinataire, string motClef, string content);

        void AskForImmediateUpdateOfUnity();
    }
}
