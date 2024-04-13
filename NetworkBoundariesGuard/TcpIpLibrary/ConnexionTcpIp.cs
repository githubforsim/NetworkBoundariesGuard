using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace TcpIpLibrary
{
    public enum EStatutConnexion
    {
        NOT_CONNECTED,
        TRYING_TO_CONNECT,
        CONNECTED
    }

    public enum ELogSendReceivedMsgStatut
    {
        NONE,
        ALL,
        NON_ROUTINE_ONLY
    }

    public abstract class ConnexionTcpIp
    {
        // ********* PUBLIC *************************************************

        public virtual void OnTick()
        {
            if (false == IsActive)
                return;

            if (true == IsConnexionOpened())
                TryToReadIncomingData();
            else
                TryToConnect();

            WatchStatutConnexion();
            ProcessPings();
        }

        public bool IsActive = false;

        public virtual void SendMessage(AMsg msg)
        {
            try
            {
                var typeContent = Type.GetType(msg._head._typeOfContent);
                if (typeContent == typeof(SimpleTxt))
                {
                    if (msg.MotClef == "MSG_NOT_RECEIVED")
                    {
                        Test();
                    }
                }

                unsafe
                {
                    if (true == msg.IsRoutine)
                    {
                        msg.ChangeId(0);
                    }
                    else
                    {
                        msg.ChangeId(GetNextSendMsgId());
                    }

                    var sizeOfHead = Marshal.SizeOf(typeof(HeadOfMsg));
                    var sizeInBytes = msg.GetSizeInBytes();
                    Log("Envoi du message = " + msg.ToString());
                }

                msg.UpdateTime();

                SendMessage(msg.AsBytesArray());
            }
            catch
            {
                try
                {
                    Log("Erreur lors de l'envoi de ce message : " + msg);
                }
                catch
                {
                    Log("Erreur lors de l'envoi d'un message et impossible de l'afficher");
                }
            }
        }

        public void ReSendNonReceivedMessage(AMsg msg)
        {
            unsafe
            {
                // Comme on renvoi un message non reçu par le destinataire, on ne modifie pas son id

                var sizeOfHead = Marshal.SizeOf(typeof(HeadOfMsg));
                var sizeInBytes = msg.GetSizeInBytes();
                Log("Envoi du message = " + msg.ToString());
            }

            SendMessage(msg.AsBytesArray());
        }

        public void FalseSendMessageForContinuitéTest(AMsg msg)
        {
            unsafe
            {
                if (true == msg.IsRoutine)
                {
                    msg.ChangeId(0);
                }
                else
                {
                    msg.ChangeId(GetNextSendMsgId());
                }

                var sizeOfHead = Marshal.SizeOf(typeof(HeadOfMsg));
                var sizeInBytes = msg.GetSizeInBytes();
                Log("Faux envoi du message = " + msg.ToString() + " avec une taille totale de " + sizeInBytes + " dont " + sizeOfHead.ToString() + " bytes pour la head");
            }
        }

        public void Disconnect(string reason)
        {
            OnDisconnect(reason);
        }

        public EStatutConnexion GetStatut()
        {
            if (false == IsActive)
            {
                return EStatutConnexion.NOT_CONNECTED;
            }

            return true == IsConnected(out string comments) ? EStatutConnexion.CONNECTED : EStatutConnexion.TRYING_TO_CONNECT;
        }

        public bool IsConnected(out string comments)
        {
            if (m_connexion?.Client == null)
            {
                comments = "m_connexion?.Client == null";
                return false;
            }

            var part1 = m_connexion.Client.Poll(0, SelectMode.SelectRead);
            var part2 = (m_connexion.Client.Available == 0);

            if (true == part1 && true == part2)
            {
                comments = "m_connexion.Client se voit déconnecté";
                return false;
            }

            var timeElapsedSinceLastReceivedMsg_s = (DateTime.Now.TimeOfDay - m_timeLastMsgReceived).TotalSeconds;
            if (timeElapsedSinceLastReceivedMsg_s > TIME_OF_SILENCE_BEFORE_DISCONNECT_s)
            {
                comments = $"timeElapsedSinceLastReceivedMsg_s > {TIME_OF_SILENCE_BEFORE_DISCONNECT_s:0.0} sec";
                return false;
            }

            comments = "";
            return true;
        }

        public static string EConnexionStatutToString(EStatutConnexion statut)
        {
            switch (statut)
            {
                case EStatutConnexion.NOT_CONNECTED:
                    return "Sleeping";
                case EStatutConnexion.TRYING_TO_CONNECT:
                    return "Trying to connect";
                case EStatutConnexion.CONNECTED:
                    return "Connected";
            }

            throw new System.NotImplementedException();
        }

        public ulong SendMsgCounter { get; private set; }

        public IList<AMsg> ExtractReceivedMsg()
        {
            var output = _listOfReceivedMsg;

            _listOfReceivedMsg = new List<AMsg>();
            return output;
        }

        public IList<string> ExtractLogs()
        {
            var output = _listOfLogs;

            _listOfLogs = new List<string>();
            return output;
        }

        public virtual void ProcessPings()
        {
            if (false == IsConnexionOpened())
                return;

            var timeElapsedSinceLastPing_s = (DateTime.Now.TimeOfDay - m_timeLastPing).TotalSeconds;
            if (timeElapsedSinceLastPing_s > PING_PERIOD_s)
            {
                SendMessage(new Msg<SimpleTxt>(PING, "ZID", new SimpleTxt($"{SendMsgCounter};{_lastIdReceivedMsgProcessed}"), /*IsRoutine*/true));
                m_timeLastPing = DateTime.Now.TimeOfDay;
            }
        }

        public abstract void CloseConnexion();

        public ELogSendReceivedMsgStatut ModeOfLogForSentMsg = ELogSendReceivedMsgStatut.ALL;

        public ELogSendReceivedMsgStatut ModeOfLogForReceivedMsg = ELogSendReceivedMsgStatut.ALL;

        // ********* PROTECTED **********************************************

        protected readonly Sablier _askForMsgNotReceived = new Sablier();
        protected ulong _lastIdReceivedMsgProcessed = 0;
        protected readonly string _name;
        protected const float TIME_OF_SILENCE_BEFORE_DISCONNECT_s = 2.0f;
        protected const float PING_PERIOD_s = 0.5f;
        protected const string PING = "PING";
        protected TimeSpan m_timeLastMsgReceived;
        protected TimeSpan m_timeLastMsgSent;
        protected TimeSpan m_timeLastPing;
        protected int m_port;
        protected bool m_isFirstMsgReceived = false;
        protected TcpClient m_connexion;
        protected NetworkStream m_stream;
        protected EStatutConnexion m_statut;
        protected byte[] m_mainDatasbuffer;
        protected int m_datasSizeInBuffer;
        protected IList<AMsg> _listOfReceivedMsg = new List<AMsg>();
        protected IList<string> _listOfLogs = new List<string>();

        protected IPAddress GetInternetIpAdress()
        {
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName()); // get my own IP
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    return address;
            }

            throw new ApplicationException("Function GetInternetIpAdress() failed");
        }
        private ulong GetNextSendMsgId()
        {
            return ++SendMsgCounter;
        }
        protected virtual void Test()
        {

        }
        protected ConnexionTcpIp(string name)
        {
            _name = name;
            m_statut = EStatutConnexion.NOT_CONNECTED;
        }
        private void WatchStatutConnexion()
        {
            if (EStatutConnexion.CONNECTED != m_statut && true == IsConnected(out string comments_1))
            {
                OnConnect();
            }
            else if (EStatutConnexion.CONNECTED == m_statut && false == IsConnected(out string comments_2))
            {
                OnDisconnect(comments_2);
            }
        }
        private void TryToReadIncomingData()
        {
            if (false == m_stream.DataAvailable)
                return;

            Debug.Assert(null != m_mainDatasbuffer);
            
            var sizeReaden = UpdateTheMainDatasBufferWithReceivedDatas();
            if (0 == sizeReaden) // Exit if nothing has been received
                return;

            // Try to understand the messages stored in the main buffer
            ReadMessageLoop:
                if (true == TryToReadTheNextMsgStoredInTheMainBuffer())
                    goto ReadMessageLoop;
        }
        private int UpdateTheMainDatasBufferWithReceivedDatas()
        {
            if (false == m_stream.DataAvailable)
            {
                return 0;
            }

            // Put received Datas in a particular array[]
            var maxBufferSize = m_connexion.ReceiveBufferSize;
            var thisReceivedBuffer = new byte[MainDatasBufferSize];
            var sizeReaden = m_stream.Read(thisReceivedBuffer, 0, maxBufferSize);

            // Add the received datas to the main buffer
            if (0 == m_datasSizeInBuffer)
            {
                m_mainDatasbuffer = thisReceivedBuffer;
            }
            else
            {
                if (m_datasSizeInBuffer + sizeReaden < MainDatasBufferSize)
                {
                    for (int i = 0; i < sizeReaden; i++)
                    {
                        int indexInPrincipal = m_datasSizeInBuffer + i;
                        if (indexInPrincipal >= MainDatasBufferSize)
                        {
                            throw new Exception("Souci avec l'index principal");
                        }

                        m_mainDatasbuffer[indexInPrincipal] = thisReceivedBuffer[i];
                    }
                }
                else
                {
                    Debug.WriteLine("La mémoire tampon est saturée et le dernier message n'a pas été lu");
                }
            }

            m_datasSizeInBuffer += sizeReaden;
            //Log(sizeReaden.ToString() + " bytes reçus. Nouvelle taille du buffer = " + m_datasSizeInBuffer.ToString() + " bytes");

            return sizeReaden;
        }
        private HeadOfMsg ReadHeadNextMsg()
        {
            var size = Marshal.SizeOf(typeof(HeadOfMsg));
            IntPtr arrPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(m_mainDatasbuffer, 0, arrPtr, size);

            var head = Marshal.PtrToStructure<HeadOfMsg>(arrPtr);

            Marshal.FreeHGlobal(arrPtr);

            return head;
        }
        private bool TryToReadTheNextMsgStoredInTheMainBuffer()
        {
            if (0 == m_datasSizeInBuffer)
                return false;

            HeadOfMsg head = ReadHeadNextMsg();
            byte[] contentOfMsg = new byte[head._sizeOfContentInBytes];

            unsafe
            {
                int fullSizeOfNextMsg = Marshal.SizeOf(typeof(HeadOfMsg)) + head._sizeOfContentInBytes;

                if (fullSizeOfNextMsg > m_datasSizeInBuffer)
                {
                    //Log("Tentative de lecture du message dont la head = " + head.ToString() + " (total size = " + fullSizeOfNextMsg.ToString() + " bytes) MAIS il n'y a pas encore toutes les données");
                    return false;
                }

                if (head._sizeOfContentInBytes + Marshal.SizeOf(typeof(HeadOfMsg)) >= MainDatasBufferSize)
                    throw new ApplicationException("Head incorrect : " + head.ToString());

                int sizeOfHead = Marshal.SizeOf(typeof(HeadOfMsg));
                for (int i = 0; i < head._sizeOfContentInBytes; i++)
                {
                    contentOfMsg[i] = m_mainDatasbuffer[sizeOfHead + i];
                }

                AMsg msg = null;
                try
                {
                    var typeContent = Type.GetType(head._typeOfContent);
                    Type type = typeof(Msg<>).MakeGenericType(typeContent);
                    msg = (AMsg)Activator.CreateInstance(type, head, contentOfMsg);
                }
                catch(System.Exception)
                {
                    Log("Echec lecture du message dont la head = " + head.ToString() + " (total size = " + fullSizeOfNextMsg.ToString() + " bytes) MAIS il n'est pas compréhensible ... Le buffer a été réinitialisé !");
                    EmptyOutTheMainBuffer();
                    return false;
                }

                OnReceiveMessage(msg);

                ExtractReadenDatasFromMainBuffer(fullSizeOfNextMsg);
            }

            return true;
        }
        private void ExtractReadenDatasFromMainBuffer(int sizeToExtract)
        {
            int finalSizeOfMainBuffer = m_datasSizeInBuffer - sizeToExtract;

            Debug.Assert(finalSizeOfMainBuffer >= 0);

            for (int i = 0; i < finalSizeOfMainBuffer; i++)
            {
                m_mainDatasbuffer[i] = m_mainDatasbuffer[i + sizeToExtract];
            }

            m_datasSizeInBuffer = finalSizeOfMainBuffer;

            //Log("Extraction de " + sizeToExtract + " bytes ... Nouvelle taille du buffer = " + m_datasSizeInBuffer.ToString() + " bytes");
        }
        private void CreateReceivedDatasBuffer()
        {
            Debug.Assert(null != m_connexion);

            m_mainDatasbuffer = new byte[MainDatasBufferSize];
            m_datasSizeInBuffer = 0;
        }
        private void DestroyReceivedDatasBuffer()
        {
            m_mainDatasbuffer = null;
            m_datasSizeInBuffer = 0;
        }
        private void EmptyOutTheMainBuffer()
        {
            m_datasSizeInBuffer = 0;
        }
        private int MainDatasBufferSize => (1000 * m_connexion.ReceiveBufferSize);
        private void Log(string msg)
        {
            _listOfLogs.Add(msg);

            if (_listOfLogs.Count > 1000)
                _listOfLogs.RemoveAt(0);
        }
        protected void SendMessage(Byte[] sendBytes)
        {
            if (false == IsConnexionOpened())
            {
                return;
            }

            try
            {
                m_stream.Write(sendBytes, 0, sendBytes.Length);
                m_timeLastMsgSent = DateTime.Now.TimeOfDay;
                //Debug.WriteLine("SendMessage(Byte[] sendBytes)");
            }
            catch (System.IO.IOException)
            {
                Disconnect("Déconnection sur System.IO.IOException dans ConnexionTcpIp::SendMessage(byte[])");
                Log("Déconnection sur System.IO.IOException dans ConnexionTcpIp::SendMessage(byte[])");
            }
        }
        protected void OnCreateConnection()
        {
            CreateReceivedDatasBuffer();
        }
        protected virtual void OnConnect()
        {
            m_statut = EStatutConnexion.CONNECTED;
            m_timeLastMsgReceived = DateTime.Now.TimeOfDay;
            m_timeLastMsgSent = DateTime.Now.TimeOfDay;
            m_timeLastPing = DateTime.Now.TimeOfDay;
            Log($"{_name} - OnConnect()");
        }
        protected bool IsConnexionOpened()
        {
            return (null != m_connexion);
        }
        protected virtual void OnDisconnect(string reason)
        {
            m_statut = EStatutConnexion.TRYING_TO_CONNECT;
            m_connexion = null;
            m_isFirstMsgReceived = false;
            Log($"{_name} - OnDisconnect() - {reason}");

            DestroyReceivedDatasBuffer();
        }
        protected virtual void OnReceiveMessage(AMsg msg)
        {
            bool isLogMsg = false;
            switch (ModeOfLogForReceivedMsg)
            {
                case ELogSendReceivedMsgStatut.NONE:
                    break;
                case ELogSendReceivedMsgStatut.ALL:
                    isLogMsg = true;
                    break;
                case ELogSendReceivedMsgStatut.NON_ROUTINE_ONLY:
                    if (false == msg.IsRoutine)
                    {
                        isLogMsg = true;
                    }
                    break;
            }

            if (isLogMsg)
            {
                Log($"{_name} - OnReceiveMessage - id = {msg.Id} - {msg.ToString()}");
            }

            if (false == m_isFirstMsgReceived)
            {
                Log($"{_name} - Connexion confirmée suite réception 1er message");
                m_isFirstMsgReceived = true;
                OnConnect();
            }

            m_timeLastMsgReceived = DateTime.Now.TimeOfDay;

            if (false == IsPingMsg(msg))
            {
                _listOfReceivedMsg.Add(msg);
            }
        }
        protected abstract void TryToConnect();
        protected static bool IsPingMsg(AMsg msg)
        {
            return (msg.Destinataire == PING);
        }
        protected static void ConfigureSocket(TcpClient connexion)
        {
            connexion.NoDelay = true;
        }
    }
}
