using System;
using System.Collections.Generic;
using System.Threading;
using TcpIpLibrary;

namespace NTcpIpWrapper.DistributeAllMsgToOneClient
{
    internal class MultiThreadWrapper : MultiThreadWrapperBase, IConnexionForOwner
    {
        // *** PUBLIC ***********************************

        public MultiThreadWrapper(IClientOfConnexion client, IChief chief, EModeConnexion mode)
            : base(new SNfoConnexion(client.Name, mode, client.AdressIP, client.PortConnexion))
        {
            _client = client;
            _chief = chief;
        }

        // *** RESTRICTED *******************************

        private readonly IChief _chief;

        private readonly IClientOfConnexion _client;

        protected override void OnConnect()
        {
            base.OnConnect();

            _chief.OnConnect();
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();

            _chief.OnDisconnect();
        }

        protected override void WhatToDoWithAReceivedMessage(AMsg msg)
        {
            _client.OnReceiveMessage(msg);
            if (_client != _chief)
            {
                _chief.OnReceiveMessage(msg);
            }
        }

        protected override void WhatToDoWithALog(string log)
        {
            _chief.Log(log);
        }
    }
}
