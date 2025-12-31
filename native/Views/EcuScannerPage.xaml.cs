using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlackFlag.Views
{
    public partial class EcuScannerPage : Page
    {
        public ObservableCollection<ModuleInfo> DetectedModules { get; } = new();
        private DispatcherTimer? _scanTimer;
        private int _scanProgress;
        
        public EcuScannerPage()
        {
            InitializeComponent();
            DataContext = this;
        }
        
        private void StartScan_Click(object sender, RoutedEventArgs e)
        {
            DetectedModules.Clear();
            EmptyMessage.Visibility = Visibility.Collapsed;
            StartScanBtn.IsEnabled = false;
            StopScanBtn.IsEnabled = true;
            _scanProgress = 0;
            ScanProgress.Value = 0;
            ScanStatus.Text = "Scanning for ECU modules...";
            
            _scanTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _scanTimer.Tick += ScanTimer_Tick;
            _scanTimer.Start();
        }
        
        private void StopScan_Click(object sender, RoutedEventArgs e)
        {
            StopScan();
            ScanStatus.Text = "Scan stopped by user";
        }
        
        private void StopScan()
        {
            _scanTimer?.Stop();
            _scanTimer = null;
            StartScanBtn.IsEnabled = true;
            StopScanBtn.IsEnabled = false;
        }
        
        private void ScanTimer_Tick(object? sender, EventArgs e)
        {
            _scanProgress += 5;
            ScanProgress.Value = _scanProgress;
            
            // Simulate finding modules at certain progress points
            if (_scanProgress == 20)
            {
                DetectedModules.Add(new ModuleInfo
                {
                    ModuleName = "Engine Control Module (ECM)",
                    Address = "0x7E0",
                    Protocol = "CAN 500kbps",
                    IsOnline = true
                });
            }
            else if (_scanProgress == 40)
            {
                DetectedModules.Add(new ModuleInfo
                {
                    ModuleName = "Transmission Control Module (TCM)",
                    Address = "0x7E1",
                    Protocol = "CAN 500kbps",
                    IsOnline = true
                });
            }
            else if (_scanProgress == 60)
            {
                DetectedModules.Add(new ModuleInfo
                {
                    ModuleName = "Anti-lock Braking System (ABS)",
                    Address = "0x7B0",
                    Protocol = "CAN 500kbps",
                    IsOnline = true
                });
            }
            else if (_scanProgress == 80)
            {
                DetectedModules.Add(new ModuleInfo
                {
                    ModuleName = "Airbag Control Module",
                    Address = "0x770",
                    Protocol = "CAN 500kbps",
                    IsOnline = true
                });
            }
            
            if (_scanProgress >= 100)
            {
                StopScan();
                ScanStatus.Text = $"Scan complete. Found {DetectedModules.Count} modules.";
            }
            else
            {
                ScanStatus.Text = $"Scanning... {_scanProgress}%";
            }
        }
    }
    
    public class ModuleInfo
    {
        public string ModuleName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        
        public Brush StatusColor => IsOnline 
            ? new SolidColorBrush(Color.FromRgb(0, 255, 136)) 
            : new SolidColorBrush(Color.FromRgb(255, 68, 68));
    }
}
