using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace projet_test
{
    public partial class MainWindow : Window
    {
        private bool isCapturing = false;
        private bool isPaused = false;
        private Process process = new Process();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void StartCapture_bouton(object sender, RoutedEventArgs e)
        {
            if (!isCapturing)
            {
                StartCapture();// la capture demarre
                captureButton.Content = "PAUSE";
                isCapturing = true;
            }
            else
            {
                if (isPaused)
                {
                    ResumeCapture();//la capture reprend 
                    captureButton.Content = "PAUSE";
                }
                else
                {
                    PauseCapture();//capture en pause 
                    captureButton.Content = "CAPTURE";
                }
            }
        }

        private void StartCapture()
        {
            listBox.Items.Clear();
            MessageBox.Show("Capture démarrée.");
       
            ProcessStartInfo startInfo = new ProcessStartInfo
            {     //Paramètres de processus 
                FileName = @"C:\Program Files\Wireshark\tshark.exe",
                Arguments = "-i Wi-Fi -V", //argument de la capture (filtre)
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            process = new Process();
            process.StartInfo = startInfo;
            //l'événement OutputDataReceived est appelé qui detecte les nouvelles données qui sont disponibles
            //sur la sortie standard 
            process.OutputDataReceived += new DataReceivedEventHandler(Process_OutputDataReceived);
            // l’événement OutputDataReceived signale chaque fois que
            // le processus écrit une ligne dans le flux redirigé StandardOutput , jusqu’à ce que le processus se termine.
            //la méthode Process_OutputDataReceived doit être appelée pour gérer les données reçus sur la SS.
            process.Start();
            process.BeginOutputReadLine();
        }

        private void PauseCapture()
        {
            MessageBox.Show("Capture en pause.");
            isPaused = true;

            if (process != null && !process.HasExited)
            {
                process.CancelOutputRead();
            }
        }

        private void ResumeCapture()
        {
            MessageBox.Show("Capture reprise.");
            isPaused = false; // la capture n'est pas en pause 

            if (process != null && !process.HasExited)
            {
                process.BeginOutputReadLine(); //la lecture reprend de la sortie standard 
            }
        }

        // Méthode appelée lors de la réception de données de sortie du processus de capture
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data) && !string.IsNullOrWhiteSpace(e.Data)) // si les données ne sont ! nulles ni vides
            {
                Dispatcher.Invoke(() =>
                {
                    listBox.Items.Add(e.Data); 
                });
            }
        }

        private void StopCapture()  // Méthode pour arrêter la capture
        {
            MessageBox.Show("Capture arrêtée.");

            if (process != null && !process.HasExited)
            {
                process.Kill();
                process.Dispose();
            }
        }

        protected override void OnClosed(EventArgs e)
        {// Arrête la capture lors de la fermeture de la fenêtre
            base.OnClosed(e);
            StopCapture();
        }

    }
}
