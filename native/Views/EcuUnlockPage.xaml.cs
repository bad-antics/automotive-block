using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace BlackFlag.Views
{
    public partial class EcuUnlockPage : Page
    {
        private bool isConnected = false;
        private string currentEcuModel = "";
        
        public EcuUnlockPage()
        {
            InitializeComponent();
            Loaded += EcuUnlockPage_Loaded;
        }

        private void EcuUnlockPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeEcuModels();
            LoadJ2534Devices();
        }

        private void InitializeEcuModels()
        {
            // Pre-populate with first vendor's models
            UpdateModelsForVendor("Bosch");
        }

        private void VendorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VendorSelector.SelectedItem is ComboBoxItem item)
            {
                var vendor = item.Content.ToString() ?? "";
                var vendorName = vendor.Split('(')[0].Trim();
                UpdateModelsForVendor(vendorName);
            }
        }

        private void UpdateModelsForVendor(string vendor)
        {
            if (ModelSelector == null) return;
            
            ModelSelector.Items.Clear();
            
            switch (vendor)
            {
                case "Bosch":
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "ME7.1 (VW/Audi 1.8T)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "ME7.5 (VW/Audi 1.8T/2.7T)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "MED9.1 (VW 2.0 FSI)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "MED17.1 (VW/Audi TSI)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "MED17.5 (VW/Audi TSI)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "EDC15 (VW/Audi TDI)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "EDC16 (VW/Audi TDI)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "EDC17 (VW/Audi TDI)" });
                    break;
                    
                case "Siemens/Continental":
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "SIMOS 8.1 (VW/Skoda)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "SIMOS 18.1 (VW MQB)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "SIMOS 18.6 (Audi/Porsche)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "EMS3110 (BMW N52/N54)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "EMS3120 (BMW N55)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "MSD80/81 (BMW S65)" });
                    break;
                    
                case "Denso":
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "Common Rail Gen1 (Toyota)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "Common Rail Gen2 (Toyota/Lexus)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "Common Rail Gen3 (Toyota/Lexus)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "Gasoline D4 (Toyota)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "Gasoline D4S (Lexus)" });
                    break;
                    
                case "Delphi/Delco":
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "E38 (GM Gen V LT1/LT4)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "E78 (GM Truck/SUV)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "E92 (GM Camaro ZL1)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "MT80 (Ford Ecoboost)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "DDCR (GM Duramax Diesel)" });
                    break;
                    
                case "Magneti Marelli":
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "IAW 4AF (Alfa Romeo)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "IAW 5SF (Fiat/Alfa)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "IAW 6F (Fiat/Lancia)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "MJD 6F3 (Fiat Diesel)" });
                    break;
                    
                case "Hitachi":
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "SH7055 (Nissan VQ)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "SH7058 (Nissan GT-R)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "SH72543 (Subaru)" });
                    break;
                    
                case "Visteon":
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "LEVANTA (Ford Mustang)" });
                    ModelSelector.Items.Add(new ComboBoxItem { Content = "EEC-V (Ford Modular)" });
                    break;
            }
            
            if (ModelSelector.Items.Count > 0)
            {
                ModelSelector.SelectedIndex = 0;
            }
        }

        private void ModelSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelSelector.SelectedItem is ComboBoxItem item)
            {
                currentEcuModel = item.Content.ToString() ?? "";
                UpdateProcessorInfo(currentEcuModel);
            }
        }

        private void UpdateProcessorInfo(string model)
        {
            if (ProcessorInfo == null) return;
            
            if (model.Contains("ME7") || model.Contains("MED9"))
            {
                ProcessorInfo.Text = "Infineon C167 / Motorola MPC5xx";
            }
            else if (model.Contains("MED17") || model.Contains("EDC17"))
            {
                ProcessorInfo.Text = "Infineon TriCore TC1797 / TC1767";
            }
            else if (model.Contains("SIMOS"))
            {
                ProcessorInfo.Text = "Infineon TriCore TC1782 / TC1791";
            }
            else if (model.Contains("EMS") || model.Contains("MSD"))
            {
                ProcessorInfo.Text = "Infineon TriCore TC1766 / TC1796";
            }
            else if (model.Contains("Denso"))
            {
                ProcessorInfo.Text = "Renesas SH7058 / SH7059";
            }
            else if (model.Contains("E38") || model.Contains("E78") || model.Contains("E92"))
            {
                ProcessorInfo.Text = "Freescale MPC5674F";
            }
            else if (model.Contains("MT80"))
            {
                ProcessorInfo.Text = "Freescale MPC5668G";
            }
            else if (model.Contains("IAW") || model.Contains("MJD"))
            {
                ProcessorInfo.Text = "ST Microelectronics ST10";
            }
            else if (model.Contains("Hitachi") || model.Contains("SH7"))
            {
                ProcessorInfo.Text = "Renesas SuperH SH-2A / SH-4";
            }
            else if (model.Contains("Visteon"))
            {
                ProcessorInfo.Text = "Motorola MPC555 / MPC565";
            }
            else
            {
                ProcessorInfo.Text = "Unknown - Select ECU Model";
            }
        }

        private void LoadJ2534Devices()
        {
            if (DeviceSelector == null) return;
            
            DeviceSelector.Items.Clear();
            DeviceSelector.Items.Add(new ComboBoxItem { Content = "Drew Technologies Mongoose Pro GM II", IsSelected = true });
            DeviceSelector.Items.Add(new ComboBoxItem { Content = "Tactrix OpenPort 2.0" });
            DeviceSelector.Items.Add(new ComboBoxItem { Content = "OBDLink MX+" });
            DeviceSelector.Items.Add(new ComboBoxItem { Content = "VCX Nano (Clone)" });
            DeviceSelector.Items.Add(new ComboBoxItem { Content = "Bosch KTS 560/570" });
            DeviceSelector.Items.Add(new ComboBoxItem { Content = "Kvaser Leaf Light" });
        }

        private void RefreshDevices_Click(object sender, RoutedEventArgs e)
        {
            LogActivity("🔄 Scanning for J2534 devices...");
            LoadJ2534Devices();
            LogActivity("✅ Device list refreshed");
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                await ConnectToEcu();
            }
            else
            {
                DisconnectFromEcu();
            }
        }

        private async Task ConnectToEcu()
        {
            LogActivity("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            LogActivity("🔌 Initiating ECU connection...");
            
            ProgressBar.Value = 0;
            ProgressText.Text = "Status: Connecting...";
            
            // Simulate connection sequence
            await SimulateProgress("Opening J2534 device...", 20);
            await SimulateProgress("Establishing protocol...", 40);
            await SimulateProgress("Reading ECU identification...", 60);
            await SimulateProgress("Checking security status...", 80);
            await SimulateProgress("Connection established!", 100);
            
            isConnected = true;
            ConnectBtn.Content = "🔌 DISCONNECT FROM ECU";
            StatusText.Text = "Connected";
            StatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
            UnlockPanel.Visibility = Visibility.Visible;
            
            // Simulate ECU info
            FlashSizeText.Text = "512 KB";
            SecurityText.Text = "🔒 LOCKED";
            SecurityText.Foreground = System.Windows.Media.Brushes.Red;
            SwVersionText.Text = "1037360922";
            HwVersionText.Text = "8E0 906 018 AA";
            BootModeText.Text = "Application";
            
            LogActivity("✅ ECU Connected Successfully");
            LogActivity($"   Model: {currentEcuModel}");
            LogActivity($"   SW: {SwVersionText.Text}");
            LogActivity($"   HW: {HwVersionText.Text}");
        }

        private void DisconnectFromEcu()
        {
            isConnected = false;
            ConnectBtn.Content = "🔌 CONNECT TO ECU";
            StatusText.Text = "Disconnected";
            StatusText.Foreground = System.Windows.Media.Brushes.Red;
            UnlockPanel.Visibility = Visibility.Collapsed;
            ProgressBar.Value = 0;
            ProgressText.Text = "Status: Waiting...";
            
            LogActivity("🔌 Disconnected from ECU");
        }

        private async void ReadEcu_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Please connect to ECU first!", "Not Connected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            LogActivity("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            LogActivity("📥 Starting ECU backup read...");
            
            ProgressBar.Value = 0;
            
            await SimulateProgress("Entering diagnostic session...", 10);
            await SimulateProgress("Disabling timeouts...", 20);
            await SimulateProgress("Reading flash memory (0x00000-0x1FFFF)...", 40);
            await SimulateProgress("Reading flash memory (0x20000-0x3FFFF)...", 60);
            await SimulateProgress("Reading flash memory (0x40000-0x7FFFF)...", 80);
            await SimulateProgress("Verifying checksum...", 90);
            await SimulateProgress("Saving backup file...", 100);
            
            LogActivity("✅ ECU backup saved: ME71_BACKUP_20251231.bin (512 KB)");
            LogActivity("   Checksum: 0xA5B3C8D1");
            MessageBox.Show("ECU backup completed successfully!\n\nFile: ME71_BACKUP_20251231.bin\nSize: 512 KB\nChecksum: 0xA5B3C8D1", 
                          "Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void UnlockProcessor_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Please connect to ECU first!", "Not Connected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var result = MessageBox.Show(
                "⚠️ WARNING: Processor unlock is IRREVERSIBLE!\n\n" +
                "This will:\n" +
                "• Bypass ECU security permanently\n" +
                "• Allow unrestricted flash write access\n" +
                "• Potentially void warranties\n\n" +
                "Have you created a backup?\n\n" +
                "Proceed with unlock?",
                "Confirm Processor Unlock",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
                
            if (result != MessageBoxResult.Yes)
            {
                LogActivity("❌ Unlock cancelled by user");
                return;
            }
            
            LogActivity("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            LogActivity("🔓 INITIATING PROCESSOR UNLOCK SEQUENCE");
            LogActivity("⚠️  DO NOT DISCONNECT POWER OR INTERFACE!");
            
            ProgressBar.Value = 0;
            
            try
            {
                await SimulateProgress("Entering extended diagnostic session...", 5);
                await SimulateProgress("Requesting security seed...", 15);
                
                LogActivity("   Seed received: 0x3A 0xB7 0x2C 0x91");
                
                await SimulateProgress("Calculating key from seed...", 25);
                
                LogActivity("   Using algorithm: Bosch ME7 Seed/Key");
                LogActivity("   Key calculated: 0x5F 0x8E 0x4D 0x23");
                
                await SimulateProgress("Sending security key...", 35);
                await Task.Delay(500);
                
                LogActivity("   ✅ Security access granted!");
                
                await SimulateProgress("Disabling write protection...", 45);
                await SimulateProgress("Unlocking flash sectors...", 55);
                await SimulateProgress("Clearing immobilizer flags...", 65);
                await SimulateProgress("Patching bootloader...", 75);
                await SimulateProgress("Writing unlock signature...", 85);
                await SimulateProgress("Verifying unlock status...", 95);
                await SimulateProgress("UNLOCK COMPLETE!", 100);
                
                SecurityText.Text = "🔓 UNLOCKED";
                SecurityText.Foreground = System.Windows.Media.Brushes.LimeGreen;
                
                LogActivity("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                LogActivity("✅ PROCESSOR UNLOCK SUCCESSFUL!");
                LogActivity("   ECU is now fully accessible for tuning");
                LogActivity("   Write protection: DISABLED");
                LogActivity("   Flash access: UNRESTRICTED");
                
                MessageBox.Show(
                    "🔓 PROCESSOR UNLOCK SUCCESSFUL!\n\n" +
                    "Your ECU is now fully unlocked and ready for tuning.\n" +
                    "All security restrictions have been removed.\n\n" +
                    "You can now:\n" +
                    "• Read/Write flash without restrictions\n" +
                    "• Modify calibration data\n" +
                    "• Install custom tuning files\n" +
                    "• Clone to other ECUs",
                    "Unlock Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                LogActivity($"❌ ERROR: {ex.Message}");
                MessageBox.Show($"Unlock failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SimulateProgress(string message, int targetPercent)
        {
            ProgressText.Text = $"Status: {message}";
            LogActivity($"   {message}");
            
            int currentPercent = (int)ProgressBar.Value;
            int steps = Math.Abs(targetPercent - currentPercent);
            int delay = Math.Max(20, 500 / Math.Max(steps, 1));
            
            while (currentPercent != targetPercent)
            {
                if (currentPercent < targetPercent)
                    currentPercent++;
                else
                    currentPercent--;
                    
                ProgressBar.Value = currentPercent;
                await Task.Delay(delay);
            }
        }

        private void LogActivity(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            ActivityLog.Text += $"[{timestamp}] {message}\n";
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            ActivityLog.Text = "";
        }
    }
}
