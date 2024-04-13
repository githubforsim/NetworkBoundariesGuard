using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpIpLibrary;

namespace NTcpIpWrapper.NDistributeToClients
{
    internal class MultiThreadWrapper : MultiThreadWrapperBase, IConnexionForOwner
    {
        // *** PUBLIC ***********************************

        public MultiThreadWrapper(IChief chief, SNfoConnexion nfo)
            : base(nfo)
        {
            _chief = chief;
        }

        public void DeclareClient(IClientForMessages client)
        {
            _listOfClientsForMessages.Add(client);
        }

        // *** RESTRICTED *******************************

        private readonly IChief _chief;

        private IList<IClientForMessages> _listOfClientsForMessages = new List<IClientForMessages>();

        protected override void OnConnect()
        {
            base.OnConnect();

            _chief.OnConnect();

            foreach (var client in _listOfClientsForMessages)
            {
                client.OnConnect();
            }
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();

            _chief.OnDisconnect();

            foreach (var client in _listOfClientsForMessages)
            {
                client.OnDisconnect();
            }
        }

        protected override void WhatToDoWithAReceivedMessage(AMsg msg)
        {
            var isClientFound = false;
            foreach (var client in _listOfClientsForMessages)
            {
                if (client.NameAsDestinataire == "All" 
                    || client.NameAsDestinataire == msg.Destinataire
                    || msg.Destinataire.StartsWith(client.NameAsDestinataire + "/")
                    || msg.Destinataire.StartsWith(client.NameAsDestinataire + ":")
                    )
                {
                    client.OnReceivedMessage(msg);

                    isClientFound = true;
                    break;
                }
            }

            if (false == isClientFound)
            {
                _chief.Log($"WARNING - Destinataire [{msg.Destinataire}] not found");
                _chief.OnReceivedMessageWithNoClientIdentified(msg);
            }
        }

        protected override void WhatToDoWithALog(string log)
        {
            _chief.Log(log);
        }
    }
}
