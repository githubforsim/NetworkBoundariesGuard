using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TcpIpLibrary;

namespace NTcpIpWrapper
{
    internal class ManagerForThreadLoop
    {
        // *** PUBLIC ***********************************

        public ManagerForThreadLoop(SNfoConnexion nfo)
        {
            switch ((EModeConnexion)nfo._mode)
            {
                case EModeConnexion.CLIENT:
                    _connexion = new ConnexionTcpIp_Client((int)nfo._port, nfo._adrIP);
                    break;
                case EModeConnexion.SERVER:
                    _connexion = new ConnexionTcpIp_Server((int)nfo._port);
                    break;
            }
        }

        ~ManagerForThreadLoop()
        {
        }

        public void Core()
        {
            _connexion.OnTick();
        }

        public void Close()
        {
            _connexion.CloseConnexion();
        }

        public bool IsActive
        {
            get
            {
                return _connexion.IsActive;
            }
            set
            {
                _connexion.IsActive = value;
            }
        }

        public void SendMsg(AMsg msg)
        {
            _connexion.SendMessage(msg);
        }

        public EStatutConnexion StatutConnexion()
        {
            return _connexion.GetStatut();
        }

        public IList<AMsg> ExtractReceivedMsg()
        {
            return _connexion.ExtractReceivedMsg();
        }

        public IList<string> ExtractLogs()
        {
            return _connexion.ExtractLogs();
        }

        public ELogSendReceivedMsgStatut ModeOfLogForSentMsg
        {
            get 
            {
                return _connexion.ModeOfLogForSentMsg;
            }
            set 
            {
                _connexion.ModeOfLogForSentMsg = value;
            }
        }

        public ELogSendReceivedMsgStatut ModeOfLogForReceivedMsg
        {
            get
            {
                return _connexion.ModeOfLogForReceivedMsg;
            }
            set
            {
                _connexion.ModeOfLogForReceivedMsg = value;
            }
        }


        // *** RESTRICTED *******************************

        private ConnexionTcpIp _connexion;
    }
}
