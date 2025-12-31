using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BlackFlag.Models;
using BlackFlag.Services;

namespace BlackFlag.Views
{
    public partial class OeFlashPage : Page
    {
        private Vehicle? _selectedVehicle;
        private readonly string _downloadPath;
        
        public OeFlashPage()
        {
            InitializeComponent();
            
            _downloadPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "BlackFlag", "OE_Files");
            
            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
            }
            
            Loaded += OeFlashPage_Loaded;
        }
        
        private void OeFlashPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadManufacturers();
            UpdateDownloadInfo();
            DownloadPath.Text = _downloadPath;
        }
        
        private void LoadManufacturers()
        {
            var manufacturers = Database.Instance.GetManufacturers();
            ManufacturerCombo.Items.Clear();
            ManufacturerCombo.Items.Add("Select manufacturer...");
            
            foreach (var manufacturer in manufacturers.OrderBy(m => m))
            {
                ManufacturerCombo.Items.Add(manufacturer);
            }
            
            ManufacturerCombo.SelectedIndex = 0;
        }
        
        private void ManufacturerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManufacturerCombo == null || VehicleCombo == null) return;
            if (ManufacturerCombo.SelectedIndex <= 0) return;
            
            var manufacturer = ManufacturerCombo.SelectedItem.ToString();
            var vehicles = Database.Instance.GetVehiclesByManufacturer(manufacturer!);
            
            VehicleCombo.Items.Clear();
            VehicleCombo.Items.Add("Select vehicle...");
            
            foreach (var vehicle in vehicles.OrderByDescending(v => v.Year))
            {
                VehicleCombo.Items.Add(vehicle.DisplayName);
            }
            
            VehicleCombo.SelectedIndex = 0;
            VehicleInfoPanel.Visibility = Visibility.Collapsed;
            FlashFilesList.ItemsSource = null;
            AsBuiltDataList.ItemsSource = null;
        }
        
        private void VehicleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VehicleCombo == null || VehicleCombo.SelectedIndex <= 0) return;
            
            var displayName = VehicleCombo.SelectedItem.ToString();
            var manufacturer = ManufacturerCombo.SelectedItem.ToString();
            var vehicles = Database.Instance.GetVehiclesByManufacturer(manufacturer!);
            _selectedVehicle = vehicles.FirstOrDefault(v => v.DisplayName == displayName);
            
            if (_selectedVehicle != null)
            {
                VehicleInfoPanel.Visibility = Visibility.Visible;
                VehicleInfo.Text = $"{_selectedVehicle.Year} {_selectedVehicle.Make} {_selectedVehicle.Model}";
                EcuInfo.Text = $"Engine: {_selectedVehicle.Engine} | ECU: {string.Join(", ", _selectedVehicle.EcuTypes ?? new List<string>())}";
                
                LoadFlashFiles();
                LoadAsBuiltData();
                
                StatusText.Text = $"✅ Loaded {FlashFilesList.Items.Count} flash files and {AsBuiltDataList.Items.Count} as-built modules";
            }
        }
        
        private void LoadFlashFiles()
        {
            if (_selectedVehicle == null) return;
            
            var flashFiles = new ObservableCollection<FlashFileInfo>();
            
            // Generate flash files based on ECU types
            var ecuTypes = _selectedVehicle.EcuTypes ?? new List<string>();
            
            foreach (var ecuType in ecuTypes)
            {
                // Stock calibration
                flashFiles.Add(new FlashFileInfo
                {
                    FileName = $"{_selectedVehicle.Make}_{_selectedVehicle.Model}_{_selectedVehicle.Year}_Stock.bin",
                    EcuType = ecuType,
                    Version = "Stock",
                    FileSize = GetRandomFileSize(512, 4096),
                    Description = "Original factory calibration - unmodified",
                    ReleaseDate = $"{_selectedVehicle.Year}-01-15"
                });
                
                // Latest update if newer vehicle
                if (_selectedVehicle.Year >= 2015)
                {
                    flashFiles.Add(new FlashFileInfo
                    {
                        FileName = $"{_selectedVehicle.Make}_{_selectedVehicle.Model}_{_selectedVehicle.Year}_Update.bin",
                        EcuType = ecuType,
                        Version = "TSB Update",
                        FileSize = GetRandomFileSize(512, 4096),
                        Description = "Latest TSB/recall update calibration",
                        ReleaseDate = $"{_selectedVehicle.Year + 1}-06-20"
                    });
                }
            }
            
            // Add transmission calibration for automatic transmissions
            if (_selectedVehicle.Transmission?.Contains("Automatic") == true || 
                _selectedVehicle.Transmission?.Contains("DCT") == true)
            {
                flashFiles.Add(new FlashFileInfo
                {
                    FileName = $"{_selectedVehicle.Make}_{_selectedVehicle.Model}_{_selectedVehicle.Year}_TCM.bin",
                    EcuType = "TCM (Transmission Control Module)",
                    Version = "Stock",
                    FileSize = GetRandomFileSize(256, 1024),
                    Description = "Transmission control module calibration",
                    ReleaseDate = $"{_selectedVehicle.Year}-01-15"
                });
            }
            
            FlashFilesList.ItemsSource = flashFiles;
        }
        
        private void LoadAsBuiltData()
        {
            if (_selectedVehicle == null) return;
            
            var asBuiltData = new ObservableCollection<AsBuiltDataInfo>();
            
            // Common modules
            asBuiltData.Add(new AsBuiltDataInfo
            {
                ModuleName = "Powertrain Control Module (PCM)",
                ModuleAddress = "0x7E0",
                DataBlocks = 15,
                FileSize = "8 KB",
                Description = "Engine control and fuel management configuration"
            });
            
            asBuiltData.Add(new AsBuiltDataInfo
            {
                ModuleName = "Body Control Module (BCM)",
                ModuleAddress = "0x726",
                DataBlocks = 12,
                FileSize = "6 KB",
                Description = "Lighting, locks, comfort features"
            });
            
            asBuiltData.Add(new AsBuiltDataInfo
            {
                ModuleName = "Instrument Cluster (IPC)",
                ModuleAddress = "0x720",
                DataBlocks = 8,
                FileSize = "4 KB",
                Description = "Gauge cluster configuration and options"
            });
            
            asBuiltData.Add(new AsBuiltDataInfo
            {
                ModuleName = "Anti-Lock Brake System (ABS)",
                ModuleAddress = "0x760",
                DataBlocks = 10,
                FileSize = "5 KB",
                Description = "ABS, traction control, stability control"
            });
            
            // Add 4WD module if applicable
            if (_selectedVehicle.DriveType == "4WD" || _selectedVehicle.DriveType == "AWD")
            {
                asBuiltData.Add(new AsBuiltDataInfo
                {
                    ModuleName = "4WD/Transfer Case Module",
                    ModuleAddress = "0x733",
                    DataBlocks = 6,
                    FileSize = "3 KB",
                    Description = "4WD system configuration and shift modes"
                });
            }
            
            // Add infotainment for newer vehicles
            if (_selectedVehicle.Year >= 2015)
            {
                asBuiltData.Add(new AsBuiltDataInfo
                {
                    ModuleName = "Audio Control Module (ACM)",
                    ModuleAddress = "0x7D0",
                    DataBlocks = 20,
                    FileSize = "12 KB",
                    Description = "SYNC/Infotainment system configuration"
                });
            }
            
            AsBuiltDataList.ItemsSource = asBuiltData;
        }
        
        private string GetRandomFileSize(int minKb, int maxKb)
        {
            var random = new Random();
            var sizeKb = random.Next(minKb, maxKb);
            return $"{sizeKb} KB";
        }
        
        private void DownloadFlash_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not FlashFileInfo flashFile) return;
            
            StatusText.Text = $"📥 Downloading {flashFile.FileName}...";
            DownloadProgress.Visibility = Visibility.Visible;
            
            // Simulate download
            var vehiclePath = Path.Combine(_downloadPath, 
                $"{_selectedVehicle!.Year}_{_selectedVehicle.Make}_{_selectedVehicle.Model}");
            
            if (!Directory.Exists(vehiclePath))
            {
                Directory.CreateDirectory(vehiclePath);
            }
            
            var filePath = Path.Combine(vehiclePath, flashFile.FileName);
            
            // Create a placeholder file
            File.WriteAllText(filePath, 
                $"BlackFlag OE Flash File\n" +
                $"Vehicle: {_selectedVehicle.Year} {_selectedVehicle.Make} {_selectedVehicle.Model}\n" +
                $"ECU Type: {flashFile.EcuType}\n" +
                $"Version: {flashFile.Version}\n" +
                $"Release Date: {flashFile.ReleaseDate}\n" +
                $"Description: {flashFile.Description}\n\n" +
                $"This is a simulated flash file for demonstration purposes.\n" +
                $"In a production environment, this would be the actual binary calibration data.");
            
            MessageBox.Show(
                $"Flash file downloaded successfully!\n\n" +
                $"File: {flashFile.FileName}\n" +
                $"ECU: {flashFile.EcuType}\n" +
                $"Version: {flashFile.Version}\n" +
                $"Size: {flashFile.FileSize}\n\n" +
                $"Location: {filePath}\n\n" +
                $"⚠️ Important:\n" +
                $"• Always verify file integrity before flashing\n" +
                $"• Ensure battery is fully charged\n" +
                $"• Keep original calibration backed up",
                "Download Complete",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
            DownloadProgress.Visibility = Visibility.Collapsed;
            StatusText.Text = $"✅ Downloaded {flashFile.FileName}";
            UpdateDownloadInfo();
        }
        
        private void DownloadAsBuilt_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not AsBuiltDataInfo asBuiltData) return;
            
            StatusText.Text = $"📥 Downloading {asBuiltData.ModuleName} as-built data...";
            
            var vehiclePath = Path.Combine(_downloadPath, 
                $"{_selectedVehicle!.Year}_{_selectedVehicle.Make}_{_selectedVehicle.Model}", "AsBuilt");
            
            if (!Directory.Exists(vehiclePath))
            {
                Directory.CreateDirectory(vehiclePath);
            }
            
            var fileName = $"{asBuiltData.ModuleName.Replace(" ", "_")}_AsBuilt.txt";
            var filePath = Path.Combine(vehiclePath, fileName);
            
            // Create sample as-built data
            var asBuiltContent = GenerateAsBuiltData(asBuiltData);
            File.WriteAllText(filePath, asBuiltContent);
            
            MessageBox.Show(
                $"As-Built data downloaded successfully!\n\n" +
                $"Module: {asBuiltData.ModuleName}\n" +
                $"Address: {asBuiltData.ModuleAddress}\n" +
                $"Data Blocks: {asBuiltData.DataBlocks}\n\n" +
                $"Location: {filePath}",
                "Download Complete",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
            StatusText.Text = $"✅ Downloaded {asBuiltData.ModuleName} as-built data";
            UpdateDownloadInfo();
        }
        
        private string GenerateAsBuiltData(AsBuiltDataInfo module)
        {
            var content = $"BlackFlag As-Built Configuration Data\n";
            content += $"==========================================\n\n";
            content += $"Vehicle: {_selectedVehicle!.Year} {_selectedVehicle.Make} {_selectedVehicle.Model}\n";
            content += $"VIN: 1FAHP3F29CL123456\n";
            content += $"Module: {module.ModuleName}\n";
            content += $"Module Address: {module.ModuleAddress}\n";
            content += $"Description: {module.Description}\n\n";
            content += $"Configuration Blocks:\n";
            content += $"---------------------\n\n";
            
            var random = new Random();
            for (int i = 1; i <= module.DataBlocks; i++)
            {
                var blockId = $"7D0-{i:D2}-01";
                var hexData = GenerateRandomHex(8);
                content += $"Block {blockId}: {hexData}\n";
            }
            
            content += $"\n\nNOTE: This is simulated as-built data for demonstration.\n";
            content += $"In production, this would contain actual factory configuration codes.";
            
            return content;
        }
        
        private string GenerateRandomHex(int bytes)
        {
            var random = new Random();
            var hex = "";
            for (int i = 0; i < bytes; i++)
            {
                hex += random.Next(0, 256).ToString("X2") + " ";
            }
            return hex.Trim();
        }
        
        private void RefreshFlashFiles_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVehicle != null)
            {
                LoadFlashFiles();
                LoadAsBuiltData();
                StatusText.Text = "🔄 Files refreshed";
            }
        }
        
        private void OpenDownloadsFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(_downloadPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", _downloadPath);
                StatusText.Text = $"📂 Opened {_downloadPath}";
            }
        }
        
        private void ExportAllAsBuilt_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVehicle == null)
            {
                MessageBox.Show("Please select a vehicle first.", "BlackFlag", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var vehiclePath = Path.Combine(_downloadPath, 
                $"{_selectedVehicle.Year}_{_selectedVehicle.Make}_{_selectedVehicle.Model}", "AsBuilt");
            
            if (!Directory.Exists(vehiclePath))
            {
                Directory.CreateDirectory(vehiclePath);
            }
            
            var asBuiltList = AsBuiltDataList.ItemsSource as ObservableCollection<AsBuiltDataInfo>;
            if (asBuiltList != null)
            {
                foreach (var module in asBuiltList)
                {
                    var fileName = $"{module.ModuleName.Replace(" ", "_")}_AsBuilt.txt";
                    var filePath = Path.Combine(vehiclePath, fileName);
                    var content = GenerateAsBuiltData(module);
                    File.WriteAllText(filePath, content);
                }
                
                MessageBox.Show(
                    $"All as-built data exported successfully!\n\n" +
                    $"Modules exported: {asBuiltList.Count}\n" +
                    $"Location: {vehiclePath}",
                    "Export Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                StatusText.Text = $"✅ Exported {asBuiltList.Count} as-built modules";
                UpdateDownloadInfo();
            }
        }
        
        private void ViewAsBuiltText_Click(object sender, RoutedEventArgs e)
        {
            if (AsBuiltDataList.SelectedItem is AsBuiltDataInfo selectedModule)
            {
                var content = GenerateAsBuiltData(selectedModule);
                
                var viewer = new Window
                {
                    Title = $"{selectedModule.ModuleName} - As-Built Data",
                    Width = 600,
                    Height = 500,
                    Content = new ScrollViewer
                    {
                        Content = new TextBox
                        {
                            Text = content,
                            IsReadOnly = true,
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            Padding = new Thickness(10),
                            TextWrapping = TextWrapping.NoWrap,
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                        }
                    }
                };
                viewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an as-built module first.", "BlackFlag",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void UpdateDownloadInfo()
        {
            if (Directory.Exists(_downloadPath))
            {
                var files = Directory.GetFiles(_downloadPath, "*.*", SearchOption.AllDirectories);
                TotalDownloads.Text = $"{files.Length} files";
                
                long totalSize = files.Sum(f => new FileInfo(f).Length);
                StorageUsed.Text = $"{totalSize / 1024.0 / 1024.0:F2} MB";
                
                if (files.Length > 0)
                {
                    var lastFile = files.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                    LastDownload.Text = Path.GetFileName(lastFile);
                }
            }
        }
    }
    
    public class FlashFileInfo
    {
        public string FileName { get; set; } = "";
        public string EcuType { get; set; } = "";
        public string Version { get; set; } = "";
        public string FileSize { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseDate { get; set; } = "";
    }
    
    public class AsBuiltDataInfo
    {
        public string ModuleName { get; set; } = "";
        public string ModuleAddress { get; set; } = "";
        public int DataBlocks { get; set; }
        public string FileSize { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
