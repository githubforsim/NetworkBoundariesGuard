using System;
using System.IO;
using System.Text;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReadFirewallLogs_Click(object sender, RoutedEventArgs e)
        {
            LogTextBox.IsReadOnly = true;
            string firewallLogFilePath = @"C:\Windows\System32\LogFiles\Firewall\pfirewall.log";

            try
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), "pfirewall_copie.log");

                // Copier le fichier pfirewall.log vers un emplacement temporaire
                File.Copy(firewallLogFilePath, tempFilePath, true);

                // Lire le fichier temporaire
                string[] logLines = File.ReadAllLines(tempFilePath);

                LogTextBox.Clear();

                foreach (string line in logLines)
                {
                    LogTextBox.AppendText(line + Environment.NewLine);
                }

                // delete  fichier temporaire après lecture
                File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                LogTextBox.AppendText($"Une erreur s'est produite : {ex.Message}");
            }
        }
    }
}
