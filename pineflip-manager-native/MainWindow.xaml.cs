using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using PineFlipManager.Services;

namespace PineFlipManager
{
    public partial class MainWindow : Window
    {
        private readonly FlipperService _flipperService;
        private readonly PineappleService _pineappleService;

        public MainWindow()
        {
            InitializeComponent();
            
            _flipperService = new FlipperService();
            _pineappleService = new PineappleService();
            
            // Auto-connect on startup
            AutoConnectDevices();
        }

        private async void AutoConnectDevices()
        {
            // Try to auto-connect Flipper
            if (_flipperService.AutoConnect(out string port))
            {
                FlipperStatus.Text = $"Status: Connected";
                FlipperPort.Text = $"Port: {port}";
                FlipperOutput.Text = "Flipper Zero connected!\n\n";
                RefreshFlipperInfo();
            }

            // Try to discover Pineapple
            bool discovered = await _pineappleService.DiscoverDevice();
            if (discovered)
            {
                PineappleStatus.Text = "Status: Discovered";
                PineappleIP.Text = $"IP: {_pineappleService.BaseUrl}";
                PineappleOutput.Text = "WiFi Pineapple discovered on network!\n\n";
                await RefreshPineappleStatus();
            }
        }

        // ===== FLIPPER ZERO HANDLERS =====

        private void FlipperConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_flipperService.AutoConnect(out string port))
            {
                FlipperStatus.Text = "Status: Connected";
                FlipperPort.Text = $"Port: {port}";
                FlipperOutput.Text += $"\n[CONNECTED] Flipper Zero on {port}\n";
                RefreshFlipperInfo();
            }
            else
            {
                FlipperStatus.Text = "Status: Not Found";
                FlipperPort.Text = "Port: N/A";
                FlipperOutput.Text += "\n[ERROR] Flipper Zero not found. Check USB connection.\n";
            }
        }

        private void FlipperRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshFlipperInfo();
        }

        private void RefreshFlipperInfo()
        {
            var device = _flipperService.GetDeviceInfo();
            
            if (device.IsConnected)
            {
                FlipperStatus.Text = "Status: Connected";
                FlipperPort.Text = $"Port: {device.PortName}";
                FlipperFirmware.Text = $"Firmware: {device.FirmwareVersion}";
                
                FlipperOutput.Text += $"\n=== DEVICE INFO ===\n{device.DeviceInfo}\n";
                FlipperOutput.Text += $"\n=== UPTIME ===\n{device.Uptime}\n";
                FlipperOutput.Text += $"\n=== MEMORY ===\n{device.MemoryInfo}\n";
                FlipperOutput.ScrollToEnd();
            }
            else
            {
                FlipperOutput.Text += "\n[ERROR] Not connected to Flipper Zero\n";
            }
        }

        private void FlipperSendCommand_Click(object sender, RoutedEventArgs e)
        {
            string command = FlipperCommandInput.Text.Trim();
            if (string.IsNullOrEmpty(command))
                return;

            FlipperOutput.Text += $"\n> {command}\n";
            
            string response = _flipperService.SendCommand(command);
            FlipperOutput.Text += response + "\n";
            FlipperOutput.ScrollToEnd();
            
            FlipperCommandInput.Clear();
        }

        private void FlipperCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FlipperSendCommand_Click(sender, e);
            }
        }

        private void FlipperDisconnect_Click(object sender, RoutedEventArgs e)
        {
            _flipperService.Disconnect();
            FlipperStatus.Text = "Status: Disconnected";
            FlipperPort.Text = "Port: N/A";
            FlipperFirmware.Text = "Firmware: N/A";
            FlipperOutput.Text += "\n[DISCONNECTED] Flipper Zero\n";
        }

        // ===== WIFI PINEAPPLE HANDLERS =====

        private async void PineappleDiscover_Click(object sender, RoutedEventArgs e)
        {
            PineappleOutput.Text += "\n[SEARCHING] Scanning for WiFi Pineapple on network...\n";
            
            bool discovered = await _pineappleService.DiscoverDevice();
            
            if (discovered)
            {
                PineappleStatus.Text = "Status: Discovered";
                PineappleIP.Text = $"IP: {_pineappleService.BaseUrl}";
                PineappleOutput.Text += $"[FOUND] WiFi Pineapple at {_pineappleService.BaseUrl}\n";
            }
            else
            {
                PineappleStatus.Text = "Status: Not Found";
                PineappleIP.Text = "IP: N/A";
                PineappleOutput.Text += "[ERROR] WiFi Pineapple not found on network\n";
                PineappleOutput.Text += "Make sure you're connected to the Pineapple's WiFi (172.16.x.x)\n";
            }
            
            PineappleOutput.ScrollToEnd();
        }

        private async void PineappleStatus_Click(object sender, RoutedEventArgs e)
        {
            await RefreshPineappleStatus();
        }

        private async System.Threading.Tasks.Task RefreshPineappleStatus()
        {
            PineappleOutput.Text += "\n[FETCHING] Getting device status...\n";
            
            var device = await _pineappleService.GetDeviceStatus();
            
            PineappleStatus.Text = device.IsAuthenticated ? "Status: Connected" : "Status: Discovered (Auth Required)";
            PineappleIP.Text = $"IP: {device.IpAddress}";
            PineapplePort.Text = $"Port: {device.Port}";
            
            PineappleOutput.Text += "\n=== STATUS ===\n";
            PineappleOutput.Text += device.StatusJson + "\n";
            PineappleOutput.ScrollToEnd();
        }

        private void PineappleOpenWeb_Click(object sender, RoutedEventArgs e)
        {
            string url = _pineappleService.BaseUrl;
            
            if (string.IsNullOrEmpty(url) || url == "http://172.16.42.1:1471")
            {
                PineappleOutput.Text += "\n[ERROR] Pineapple not discovered yet. Click DISCOVER first.\n";
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                
                PineappleOutput.Text += $"\n[OPENED] Web UI in browser: {url}\n";
            }
            catch (Exception ex)
            {
                PineappleOutput.Text += $"\n[ERROR] Failed to open browser: {ex.Message}\n";
            }
            
            PineappleOutput.ScrollToEnd();
        }

        protected override void OnClosed(EventArgs e)
        {
            _flipperService.Disconnect();
            base.OnClosed(e);
        }
    }
}
