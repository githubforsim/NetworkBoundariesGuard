using NLogger;
using System;
using System.Windows;
using System.Windows.Threading;
using TcpIpLibrary;
using NTcpIpWrapper;
using System.Windows.Interop;

namespace NetworkTester
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, NLogger.ILogger, NTcpIpWrapper.NDistributeToClients.IChief, IClientForMessages
    {
        // *** PUBLIC *********************************************

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SendMsg<T>(string destinataire, string motClef, T content) where T : new()
        {
            var msg = new Msg<T>(destinataire, motClef, content);

            _connexion.SendMsg(msg);
        }

        public void LogNewLine(string msg)
        {
            if (msg.Contains("PING"))
            {
                return;
            }
            Logger.Items.Add(msg);
        }

        public void Log(string msg)
        {
            Logger.Items.Add(msg);
        }

        public void NetworkLog(string log)
        {
            Logger.Items.Add(log);
        }

        public bool ArePingFiltered()
        {
            return true;
        }

        public void OnReceivedMessageWithNoClientIdentified(AMsg msg)
        {
            Logger.Items.Add("Le client [" + msg.Destinataire + "] est inconnu");
        }

        public void OnReceivedMessage(AMsg msg)
        {
            Received.Items.Add(msg.ToString());
        }

        public void Log(string msg, LogLevel level = LogLevel.INFO)
        {
            Logger.Items.Add(level.ToString() + " - " + msg);
        }

        public void OnConnect()
        {
            Logger.Items.Add("On connect");
        }

        public void OnDisconnect()
        {
            Logger.Items.Add("On disconnect");
        }

        // *** RESTRICTED *****************************************

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer_250ms.Tick += OnTimer_250ms;
            _timer_250ms.Interval = new TimeSpan(0, 0, 0, 0, 250/*ms*/);
            _timer_250ms.Start();
            _timer_250ms.IsEnabled = true;

            var t = typeof(Datas);
            var s = t.ToString();
            var t2 = Type.GetType(s);
        }

        private void OnTimer_250ms(object sender, EventArgs e)
        {
            if (null != _connexion)
            {
                _connexion.Core();

                //var listMsg = _connexion.Declare(.ExtractReceivedMsg();
                //foreach (var msg in listMsg)
                //{
                //    Received.Items.Add("FROM REAL : " + msg.ToString());
                //}

                //var listLogs = _connexion.ExtractLogs();
                //foreach (var log in listLogs)
                //{
                //    Logger.Items.Add(log.ToString());
                //}
            }
        }

        private void OnClick_StartConnexion(object sender, RoutedEventArgs e)
        {
            if (null == _connexion)
            {
                if (true == ClientRadioButton.IsChecked)
                {
                    _connexion = Factory.CreateMultiThreaded(this, new SNfoConnexion("To UNITY", EModeConnexion.CLIENT, "127.0.0.1", 1001));
                }
                else
                {
                    _connexion = Factory.CreateMultiThreaded(this, new SNfoConnexion("To UNITY", EModeConnexion.SERVER, 1001));
                }

                _connexion.DeclareClient(this);
                _connexion.Start();
                StopAndStart.Content = "Stop";
            }
            else
            {
                _connexion = null;

                StopAndStart.Content = "Start";
            }

        }

        private void OnClick_SendTxtMsg(object sender, RoutedEventArgs e)
        {
            _connexion.SendMsg("Frédéric", "C'est trop cool", "");
        }

        private void OnClick_SendStructMsg(object sender, RoutedEventArgs e)
        {
            SendMsg<Datas>("Frédéric", "Coucou", new Datas(3.14f, 6.01f));
        }

        private AMsg MessageReader(HeadOfMsg head, byte[] buffer, int port)
        {
            var test = typeof(SimpleTxt).ToString();
            var tezz = typeof(SimpleTxt).AssemblyQualifiedName;
            var typeContent2 = Type.GetType(tezz);
            var typeContent = Type.GetType(head._typeOfContent);
            Type type = typeof(Msg<>).MakeGenericType(typeContent);
            var msg = (AMsg)Activator.CreateInstance(type, head, buffer);
            return msg;
        }

        protected static bool IsPingMsg(Msg<SimpleTxt> msg)
        {
            return (msg._head._destinataire == "PING");
        }

        private readonly DispatcherTimer _timer_250ms = new DispatcherTimer();

        NTcpIpWrapper.NDistributeToClients.IConnexionForOwner _connexion;

        public string NameAsDestinataire => "Frédéric";
    }
}
