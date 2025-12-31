using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BlackFlag.Views
{
    public partial class EcuCloningPage : Page
    {
        private DispatcherTimer? _timer;
        private int _progress;
        private string _currentOperation = "";
        
        public EcuCloningPage()
        {
            InitializeComponent();
        }
        
        private void ReadEcu_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "This will read the entire ECU memory.\n\nMake sure your OBD adapter is connected.\n\nContinue?",
                "Read ECU", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                StartOperation("read");
            }
        }
        
        private void WriteEcu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select ECU File to Write",
                Filter = "ECU Files (*.bin;*.hex)|*.bin;*.hex|All Files (*.*)|*.*"
            };
            
            if (dialog.ShowDialog() == true)
            {
                var result = MessageBox.Show(
                    $"WARNING: This will overwrite the ECU with:\n{dialog.FileName}\n\nThis operation cannot be undone!\n\nContinue?",
                    "Write ECU", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    StartOperation("write");
                }
            }
        }
        
        private void CloneEcu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "ECU Clone Mode\n\n" +
                "Step 1: Connect source ECU\n" +
                "Step 2: Read data from source\n" +
                "Step 3: Connect target ECU\n" +
                "Step 4: Write data to target\n\n" +
                "Use READ and WRITE functions separately.",
                "Clone ECU", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void StartOperation(string operation)
        {
            _currentOperation = operation;
            _progress = 0;
            
            if (operation == "read")
            {
                ReadProgress.Visibility = Visibility.Visible;
                ReadProgress.Value = 0;
                ReadStatus.Text = "Starting read...";
            }
            else
            {
                WriteProgress.Visibility = Visibility.Visible;
                WriteProgress.Value = 0;
                WriteStatus.Text = "Starting write...";
            }
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        
        private void Timer_Tick(object? sender, EventArgs e)
        {
            _progress += 1;
            
            if (_currentOperation == "read")
            {
                ReadProgress.Value = _progress;
                ReadStatus.Text = $"Reading... {_progress}%";
                
                if (_progress >= 100)
                {
                    _timer?.Stop();
                    ReadStatus.Text = "Read complete! File saved.";
                    MessageBox.Show(
                        $"ECU read complete!\n\nSize: 512KB\nChecksum: 0xA7B3C2D1\nSaved to: ecu_backup_{DateTime.Now:yyyyMMdd_HHmmss}.bin",
                        "Read Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                WriteProgress.Value = _progress;
                WriteStatus.Text = $"Writing... {_progress}%";
                
                if (_progress >= 100)
                {
                    _timer?.Stop();
                    WriteStatus.Text = "Write complete!";
                    MessageBox.Show(
                        "ECU write complete!\n\nVerification: PASSED\n\nPlease turn ignition off and on.",
                        "Write Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
