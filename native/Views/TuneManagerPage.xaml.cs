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
    public partial class TuneManagerPage : Page
    {
        private Vehicle? _selectedVehicle;
        private readonly string _tunePath;
        private string _vinNumber = string.Empty;
        private bool _vinValidated = false;
        
        public TuneManagerPage()
        {
            InitializeComponent();
            
            _tunePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "BlackFlag", "Tunes");
            
            if (!Directory.Exists(_tunePath))
            {
                Directory.CreateDirectory(_tunePath);
            }
            
            Loaded += TuneManagerPage_Loaded;
        }
        
        private void TuneManagerPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadManufacturers();
            UpdateTuneInfo();
            TunePath.Text = _tunePath;
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
            TunesList.ItemsSource = null;
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
                
                CheckCompatibility();
                LoadTunes();
                
                StatusText.Text = $"✅ Loaded {TunesList.Items.Count} tunes for {_selectedVehicle.Year} {_selectedVehicle.Make} {_selectedVehicle.Model}";
            }
        }
        
        private void LoadTunes()
        {
            if (_selectedVehicle == null) return;
            
            var tunes = new ObservableCollection<TuneItem>();
            
            // Stock backup
            tunes.Add(new TuneItem
            {
                Icon = "📦",
                Name = "Stock Factory Calibration",
                Description = "Original factory tune - backup/restore",
                PowerGain = "Stock power/torque",
                FileSize = GetRandomFileSize(512, 2048),
                Version = "Factory",
                Price = "FREE",
                Requirements = "None - OEM calibration"
            });
            
            // Performance tunes based on vehicle type
            if (_selectedVehicle.FuelType == "Diesel")
            {
                tunes.Add(new TuneItem
                {
                    Icon = "🔥",
                    Name = "Stage 1 Diesel Performance",
                    Description = "Optimized boost, fueling, and timing",
                    PowerGain = "+30% Power | +35% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v2.5",
                    Price = "$299",
                    Requirements = "Stock intake/exhaust OK"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "⚡",
                    Name = "Stage 2 Diesel Performance",
                    Description = "High-flow turbo and fueling",
                    PowerGain = "+50% Power | +60% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v2.5",
                    Price = "$499",
                    Requirements = "Upgraded turbo, intake, exhaust required"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "🚫",
                    Name = "EGR/DPF Delete Tune",
                    Description = "Emissions delete with power gains",
                    PowerGain = "+25% Power | +30% Torque | Better MPG",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v3.0",
                    Price = "$399",
                    Requirements = "⚠️ Off-road use only - EGR/DPF hardware delete required"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "💰",
                    Name = "Economy Diesel Tune",
                    Description = "Optimized for fuel efficiency",
                    PowerGain = "+15% MPG | +10% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v1.8",
                    Price = "$249",
                    Requirements = "Stock hardware OK"
                });
            }
            else if (_selectedVehicle.Engine?.Contains("Turbo") == true || 
                     _selectedVehicle.Engine?.Contains("EcoBoost") == true ||
                     _selectedVehicle.Engine?.Contains("Supercharged") == true)
            {
                tunes.Add(new TuneItem
                {
                    Icon = "🔥",
                    Name = "Stage 1 Turbo Tune",
                    Description = "Boost pressure and timing optimization",
                    PowerGain = "+20% Power | +18% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v2.3",
                    Price = "$349",
                    Requirements = "Stock turbo OK - 91+ octane recommended"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "⚡",
                    Name = "Stage 2 Turbo Tune",
                    Description = "Aggressive boost and fueling",
                    PowerGain = "+35% Power | +30% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v2.3",
                    Price = "$549",
                    Requirements = "Intake, intercooler, downpipe required - 93 octane"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "🏁",
                    Name = "E85 Flex Fuel Tune",
                    Description = "High-output E85 ethanol calibration",
                    PowerGain = "+45% Power | +40% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v1.5",
                    Price = "$599",
                    Requirements = "E85 fuel required - upgraded fuel system recommended"
                });
            }
            else
            {
                tunes.Add(new TuneItem
                {
                    Icon = "🔥",
                    Name = "Stage 1 Performance",
                    Description = "Optimized air/fuel ratios and timing",
                    PowerGain = "+12% Power | +10% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v2.1",
                    Price = "$299",
                    Requirements = "Stock hardware OK"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "⚡",
                    Name = "Stage 2 Performance",
                    Description = "High-flow intake/exhaust optimized",
                    PowerGain = "+18% Power | +15% Torque",
                    FileSize = GetRandomFileSize(512, 2048),
                    Version = "v2.1",
                    Price = "$449",
                    Requirements = "Cold air intake, cat-back exhaust required"
                });
            }
            
            // Racing tunes
            tunes.Add(new TuneItem
            {
                Icon = "🏁",
                Name = "Drag Racing Tune",
                Description = "Optimized for 1/4 mile acceleration",
                PowerGain = "+30% Power | Launch control | 2-step limiter",
                FileSize = GetRandomFileSize(512, 2048),
                Version = "v2.8",
                Price = "$799",
                Requirements = "⚠️ Race use only - VIN validated - Upgraded clutch/trans recommended",
                RequiresVin = true,
                Category = "Racing"
            });
            
            tunes.Add(new TuneItem
            {
                Icon = "🏎️",
                Name = "Circuit/Road Racing Tune",
                Description = "High-RPM sustained power delivery",
                PowerGain = "+28% Power | Flat power curve | Rev limiter increase",
                FileSize = GetRandomFileSize(512, 2048),
                Version = "v2.5",
                Price = "$849",
                Requirements = "⚠️ Track use only - VIN validated - Oil cooler essential",
                RequiresVin = true,
                Category = "Racing"
            });
            
            tunes.Add(new TuneItem
            {
                Icon = "💨",
                Name = "Street Racing Tune",
                Description = "Aggressive for street performance",
                PowerGain = "+26% Power | Hard launch | Anti-lag",
                FileSize = GetRandomFileSize(512, 2048),
                Version = "v2.3",
                Price = "$699",
                Requirements = "⚠️ Off-road use only - VIN validated - 93+ octane required",
                RequiresVin = true,
                Category = "Racing"
            });
            
            tunes.Add(new TuneItem
            {
                Icon = "⛰️",
                Name = "Rock Crawling/Off-Road Tune",
                Description = "Low-end torque and throttle control",
                PowerGain = "+40% Low-end torque | Precise throttle | Hill assist",
                FileSize = GetRandomFileSize(512, 2048),
                Version = "v1.7",
                Price = "$549",
                Requirements = "4x4 vehicles only - VIN validated",
                RequiresVin = true,
                Category = "Racing"
            });
            
            // Emissions delete tunes
            if (_selectedVehicle.FuelType == "Diesel")
            {
                tunes.Add(new TuneItem
                {
                    Icon = "🚫",
                    Name = "Full Delete Package (EGR+DPF+DEF)",
                    Description = "Complete emissions system removal",
                    PowerGain = "+35% Power | +45% Torque | +20% MPG | -Regen cycles",
                    FileSize = GetRandomFileSize(1024, 2048),
                    Version = "v4.2",
                    Price = "$599",
                    Requirements = "⚠️⚠️ OFF-ROAD/COMPETITION USE ONLY - VIN+ECU+Year validated - Hardware delete required",
                    RequiresVin = true,
                    Category = "Delete"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "♻️",
                    Name = "EGR Delete Only",
                    Description = "EGR valve and cooler delete",
                    PowerGain = "+15% Power | +18% Torque | Cooler EGTs",
                    FileSize = GetRandomFileSize(512, 1024),
                    Version = "v3.1",
                    Price = "$299",
                    Requirements = "⚠️ OFF-ROAD USE ONLY - VIN validated - EGR hardware delete required",
                    RequiresVin = true,
                    Category = "Delete"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "🔥",
                    Name = "DPF Delete Only",
                    Description = "Diesel Particulate Filter removal",
                    PowerGain = "+20% Power | +25% Torque | No regen cycles",
                    FileSize = GetRandomFileSize(512, 1024),
                    Version = "v3.5",
                    Price = "$399",
                    Requirements = "⚠️ OFF-ROAD USE ONLY - VIN validated - DPF hardware delete required",
                    RequiresVin = true,
                    Category = "Delete"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "💧",
                    Name = "DEF/SCR Delete",
                    Description = "DEF system and SCR catalyst delete",
                    PowerGain = "+10% Power | No DEF refills | Lower maintenance",
                    FileSize = GetRandomFileSize(512, 1024),
                    Version = "v2.9",
                    Price = "$349",
                    Requirements = "⚠️ OFF-ROAD USE ONLY - VIN validated - SCR hardware delete required",
                    RequiresVin = true,
                    Category = "Delete"
                });
            }
            else
            {
                tunes.Add(new TuneItem
                {
                    Icon = "🚫",
                    Name = "Catalytic Converter Delete",
                    Description = "Cat bypass tune for test pipes",
                    PowerGain = "+8% Power | Better flow | No CEL",
                    FileSize = GetRandomFileSize(512, 1024),
                    Version = "v2.4",
                    Price = "$299",
                    Requirements = "⚠️ OFF-ROAD/COMPETITION USE ONLY - VIN validated - Test pipes required",
                    RequiresVin = true,
                    Category = "Delete"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "💨",
                    Name = "Secondary Air Delete",
                    Description = "SAI system delete",
                    PowerGain = "Weight reduction | Simpler engine bay | No CEL",
                    FileSize = GetRandomFileSize(256, 512),
                    Version = "v1.8",
                    Price = "$149",
                    Requirements = "⚠️ OFF-ROAD USE ONLY - VIN validated - SAI hardware removal",
                    RequiresVin = true,
                    Category = "Delete"
                });
                
                tunes.Add(new TuneItem
                {
                    Icon = "🔌",
                    Name = "O2 Sensor Delete",
                    Description = "Downstream O2 sensor delete",
                    PowerGain = "No sensor failures | No CEL",
                    FileSize = GetRandomFileSize(256, 512),
                    Version = "v2.1",
                    Price = "$99",
                    Requirements = "⚠️ OFF-ROAD USE ONLY - VIN validated - O2 sensors removed",
                    RequiresVin = true,
                    Category = "Delete"
                });
            }
            
            // Universal tunes
            tunes.Add(new TuneItem
            {
                Icon = "🚗",
                Name = "Daily Driver Tune",
                Description = "Balanced performance and reliability",
                PowerGain = "+8% Power | +10% Torque | Smooth throttle",
                FileSize = GetRandomFileSize(512, 2048),
                Version = "v1.9",
                Price = "$199",
                Requirements = "Stock hardware OK",
                RequiresVin = false,
                Category = "Street"
            });
            
            tunes.Add(new TuneItem
            {
                Icon = "❄️",
                Name = "Cold Start Fix",
                Description = "Improved cold weather starting and idle",
                PowerGain = "Better cold starts | Smoother idle",
                FileSize = GetRandomFileSize(256, 512),
                Version = "v1.2",
                Price = "FREE",
                Requirements = "None",
                RequiresVin = false,
                Category = "Street"
            });
            
            tunes.Add(new TuneItem
            {
                Icon = "🔧",
                Name = "Transmission Tune",
                Description = "Shift points and pressure optimization",
                PowerGain = "Faster shifts | Better feel",
                FileSize = GetRandomFileSize(256, 1024),
                Version = "v2.0",
                Price = "$249",
                Requirements = "Automatic transmission only",
                RequiresVin = false,
                Category = "Street"
            });
            
            TunesList.ItemsSource = tunes;
        }
        
        private string GetRandomFileSize(int minKb, int maxKb)
        {
            var random = new Random();
            var sizeKb = random.Next(minKb, maxKb);
            return $"{sizeKb} KB";
        }
        
        private void DownloadTune_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not TuneItem tune) return;
            
            // Validate VIN for tunes that require it
            if (tune.RequiresVin && !_vinValidated)
            {
                MessageBox.Show(
                    $"⚠️ VIN Validation Required\n\n" +
                    $"The '{tune.Name}' requires VIN validation for safety and compatibility.\n\n" +
                    $"Please enter your vehicle's 17-character VIN number and click 'Decode VIN' before downloading this tune.\n\n" +
                    $"VIN validation ensures:\n" +
                    $"• Correct ECU type match\n" +
                    $"• Compatible year/model\n" +
                    $"• Proper calibration data\n" +
                    $"• Safety compliance",
                    "VIN Validation Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            
            // Additional compatibility check
            if (tune.RequiresVin && _selectedVehicle != null)
            {
                var compatibilityIssues = CheckTuneCompatibility(tune);
                if (!string.IsNullOrEmpty(compatibilityIssues))
                {
                    var result = MessageBox.Show(
                        $"⚠️ Compatibility Warning\n\n" +
                        $"{compatibilityIssues}\n\n" +
                        $"Do you want to proceed anyway?\n" +
                        $"(Not recommended - may cause engine damage)",
                        "Compatibility Warning",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);
                    
                    if (result != MessageBoxResult.Yes)
                        return;
                }
            }
            
            StatusText.Text = $"📥 Downloading {tune.Name}...";
            DownloadProgress.Visibility = Visibility.Visible;
            
            var vehiclePath = Path.Combine(_tunePath, 
                $"{_selectedVehicle!.Year}_{_selectedVehicle.Make}_{_selectedVehicle.Model}");
            
            if (!Directory.Exists(vehiclePath))
            {
                Directory.CreateDirectory(vehiclePath);
            }
            
            var fileName = $"{tune.Name.Replace(" ", "_").Replace("/", "-")}.bin";
            var filePath = Path.Combine(vehiclePath, fileName);
            
            // Create a placeholder tune file
            File.WriteAllText(filePath, 
                $"BlackFlag Performance Tune\n" +
                $"=========================\n\n" +
                $"Vehicle: {_selectedVehicle.Year} {_selectedVehicle.Make} {_selectedVehicle.Model}\n" +
                $"Engine: {_selectedVehicle.Engine}\n" +
                $"ECU Type: {string.Join(", ", _selectedVehicle.EcuTypes ?? new List<string>())}\n" +
                $"VIN: {(_vinValidated ? _vinNumber : "Not Validated")}\n" +
                $"Validation Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n" +
                $"Tune Information:\n" +
                $"Name: {tune.Name}\n" +
                $"Category: {tune.Category}\n" +
                $"Version: {tune.Version}\n" +
                $"Description: {tune.Description}\n\n" +
                $"Performance Gains:\n" +
                $"{tune.PowerGain}\n\n" +
                $"Requirements:\n" +
                $"{tune.Requirements}\n\n" +
                $"Price: {tune.Price}\n\n" +
                $"Compatibility Validation:\n" +
                $"VIN Validated: {(_vinValidated ? "✅ YES" : "❌ NO")}\n" +
                $"ECU Match: {(_selectedVehicle.EcuTypes?.Count > 0 ? "✅ Verified" : "⚠️ Unknown")}\n" +
                $"Year Match: ✅ {_selectedVehicle.Year}\n\n" +
                $"⚠️ LEGAL DISCLAIMER:\n" +
                $"{(tune.Category == "Delete" || tune.Category == "Racing" ? "OFF-ROAD/COMPETITION USE ONLY - NOT STREET LEGAL\n" : "")}" +
                $"This is a simulated tune file for demonstration purposes.\n" +
                $"In a production environment, this would be actual binary calibration data.\n\n" +
                $"IMPORTANT SAFETY NOTES:\n" +
                $"• BACKUP stock calibration before flashing (CRITICAL!)\n" +
                $"• Verify VIN, ECU type, and year compatibility\n" +
                $"• Follow ALL installation requirements\n" +
                $"• Use proper fuel octane as specified\n" +
                $"• Performance tuning may void warranty\n" +
                $"• Emissions deletes illegal for street use in most jurisdictions\n" +
                $"• Racing tunes for competition use only");
            
            MessageBox.Show(
                $"Tune downloaded successfully!\n\n" +
                $"Tune: {tune.Name}\n" +
                $"Version: {tune.Version}\n" +
                $"Expected Gains: {tune.PowerGain}\n" +
                $"File Size: {tune.FileSize}\n" +
                $"Price: {tune.Price}\n\n" +
                $"Location: {filePath}\n\n" +
                $"Requirements:\n{tune.Requirements}\n\n" +
                $"⚠️ Important:\n" +
                $"• Backup stock calibration before flashing\n" +
                $"• Verify all requirements are met\n" +
                $"• Use appropriate fuel grade\n" +
                $"• Professional installation recommended",
                "Download Complete",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
            DownloadProgress.Visibility = Visibility.Collapsed;
            StatusText.Text = $"✅ Downloaded {tune.Name}";
            UpdateTuneInfo();
        }
        
        private void ImportTune_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Import Custom Tune File",
                Filter = "Tune Files (*.bin;*.hex;*.cef)|*.bin;*.hex;*.cef|All Files (*.*)|*.*"
            };
            
            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show(
                    $"Custom tune imported successfully!\n\n" +
                    $"File: {Path.GetFileName(dialog.FileName)}\n\n" +
                    $"⚠️ Warning:\n" +
                    $"• Verify tune is compatible with your vehicle\n" +
                    $"• Always backup stock calibration first\n" +
                    $"• Use caution with unverified tunes", 
                    "Import Success", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                
                StatusText.Text = $"✅ Imported {Path.GetFileName(dialog.FileName)}";
            }
        }
        
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVehicle != null)
            {
                LoadTunes();
                UpdateTuneInfo();
                StatusText.Text = "🔄 Tunes refreshed";
            }
        }
        
        private void OpenTunesFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(_tunePath))
            {
                System.Diagnostics.Process.Start("explorer.exe", _tunePath);
                StatusText.Text = $"📂 Opened {_tunePath}";
            }
        }
        
        private void VinTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (VinTextBox == null || VinStatus == null) return;
            
            var vin = VinTextBox.Text.ToUpper().Trim();
            
            if (vin.Length == 17)
            {
                VinStatus.Text = "✅ Valid Length";
                VinStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));
            }
            else if (vin.Length > 0)
            {
                VinStatus.Text = $"⚠️ {vin.Length}/17";
                VinStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 152, 0));
            }
            else
            {
                VinStatus.Text = "";
                _vinValidated = false;
            }
        }
        
        private void DecodeVin_Click(object sender, RoutedEventArgs e)
        {
            var vin = VinTextBox.Text.ToUpper().Trim();
            
            if (vin.Length != 17)
            {
                MessageBox.Show(
                    "Invalid VIN Length\n\n" +
                    "VIN must be exactly 17 characters.\n" +
                    $"Current length: {vin.Length}",
                    "VIN Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            
            // Basic VIN validation (no I, O, Q)
            if (vin.Contains('I') || vin.Contains('O') || vin.Contains('Q'))
            {
                MessageBox.Show(
                    "Invalid VIN Characters\n\n" +
                    "VIN cannot contain letters I, O, or Q\n" +
                    "These are excluded to avoid confusion with numbers.",
                    "VIN Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            
            _vinNumber = vin;
            _vinValidated = true;
            
            // Extract year from VIN (position 10)
            var yearCode = vin[9];
            var decodedYear = DecodeVinYear(yearCode);
            
            // Extract manufacturer from VIN (positions 1-3)
            var wmi = vin.Substring(0, 3);
            var manufacturer = DecodeWMI(wmi);
            
            VinStatus.Text = "✅ Validated";
            VinStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));
            
            CheckCompatibility();
            
            MessageBox.Show(
                $"✅ VIN Decoded Successfully\n\n" +
                $"VIN: {vin}\n" +
                $"Manufacturer Code (WMI): {wmi} ({manufacturer})\n" +
                $"Model Year: {decodedYear}\n\n" +
                $"Compatibility Check:\n" +
                (_selectedVehicle != null ? 
                    $"Selected Vehicle: {_selectedVehicle.Year} {_selectedVehicle.Make} {_selectedVehicle.Model}\n" +
                    $"Year Match: {(decodedYear == _selectedVehicle.Year ? "✅ YES" : $"⚠️ NO (VIN: {decodedYear}, Selected: {_selectedVehicle.Year})")}\n" +
                    $"ECU Types: {string.Join(", ", _selectedVehicle.EcuTypes ?? new List<string>())}" :
                    "⚠️ No vehicle selected - please select vehicle for full validation"),
                "VIN Validation Complete",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        
        private int DecodeVinYear(char yearCode)
        {
            // VIN year codes (simplified - covers 2000-2030)
            var yearCodes = new Dictionary<char, int>
            {
                {'Y', 2000}, {'1', 2001}, {'2', 2002}, {'3', 2003}, {'4', 2004},
                {'5', 2005}, {'6', 2006}, {'7', 2007}, {'8', 2008}, {'9', 2009},
                {'A', 2010}, {'B', 2011}, {'C', 2012}, {'D', 2013}, {'E', 2014},
                {'F', 2015}, {'G', 2016}, {'H', 2017}, {'J', 2018}, {'K', 2019},
                {'L', 2020}, {'M', 2021}, {'N', 2022}, {'P', 2023}, {'R', 2024},
                {'S', 2025}, {'T', 2026}, {'V', 2027}, {'W', 2028}, {'X', 2029}
            };
            
            return yearCodes.ContainsKey(yearCode) ? yearCodes[yearCode] : 0;
        }
        
        private string DecodeWMI(string wmi)
        {
            var wmiCodes = new Dictionary<string, string>
            {
                {"1FA", "Ford USA"}, {"1FT", "Ford Truck USA"}, {"1FD", "Ford Medium/Heavy Truck"},
                {"1G1", "Chevrolet USA"}, {"1G2", "Pontiac"}, {"1GC", "Chevrolet Truck"},
                {"1GM", "Pontiac"}, {"1GY", "Cadillac"}, {"1HD", "Harley Davidson"},
                {"1J4", "Jeep"}, {"1L1", "Lincoln"}, {"1LN", "Lincoln"},
                {"1M1", "Mack Truck"}, {"1N4", "Nissan USA"}, {"1VW", "Volkswagen USA"},
                {"2C3", "Chrysler"}, {"2FA", "Ford Canada"}, {"2G1", "Chevrolet Canada"},
                {"2HG", "Honda Canada"}, {"2HM", "Hyundai Canada"}, {"2T1", "Toyota Canada"},
                {"3FA", "Ford Mexico"}, {"3G1", "Chevrolet Mexico"}, {"3N1", "Nissan Mexico"},
                {"4S3", "Subaru USA"}, {"4T1", "Toyota USA"}, {"5N1", "Nissan USA"},
                {"5YJ", "Tesla"}, {"JM1", "Mazda"}, {"JN1", "Nissan Japan"},
                {"KM8", "Hyundai Korea"}, {"KNA", "Kia"}, {"SAL", "Land Rover"},
                {"SAJ", "Jaguar"}, {"SCA", "Rolls Royce"}, {"TRU", "Audi Hungary"},
                {"WAU", "Audi Germany"}, {"WBA", "BMW"}, {"WBS", "BMW M Series"},
                {"WDB", "Mercedes-Benz"}, {"WDD", "Mercedes-Benz"}, {"WDC", "DaimlerChrysler"},
                {"WF0", "Ford Germany"}, {"WP0", "Porsche"}, {"WVW", "Volkswagen Germany"},
                {"YV1", "Volvo Cars"}, {"ZFF", "Ferrari"}, {"ZAM", "Maserati"}
            };
            
            return wmiCodes.ContainsKey(wmi) ? wmiCodes[wmi] : "Unknown";
        }
        
        private void CheckCompatibility()
        {
            if (_selectedVehicle == null || CompatibilityInfo == null) return;
            
            if (!_vinValidated)
            {
                CompatibilityInfo.Text = "⚠️ VIN not validated - some tunes require VIN validation";
                CompatibilityInfo.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 152, 0));
                return;
            }
            
            var vin = _vinNumber;
            var yearCode = vin[9];
            var decodedYear = DecodeVinYear(yearCode);
            
            if (decodedYear == _selectedVehicle.Year)
            {
                CompatibilityInfo.Text = $"✅ VIN validated - Year match ({decodedYear}) - ECU: {string.Join(", ", _selectedVehicle.EcuTypes ?? new List<string>())}";
                CompatibilityInfo.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));
            }
            else
            {
                CompatibilityInfo.Text = $"⚠️ Year mismatch - VIN: {decodedYear}, Selected: {_selectedVehicle.Year}";
                CompatibilityInfo.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(244, 67, 54));
            }
        }
        
        private string CheckTuneCompatibility(TuneItem tune)
        {
            if (_selectedVehicle == null) return "No vehicle selected";
            
            var issues = new List<string>();
            
            // Check year from VIN
            if (_vinValidated)
            {
                var yearCode = _vinNumber[9];
                var decodedYear = DecodeVinYear(yearCode);
                
                if (decodedYear != _selectedVehicle.Year)
                {
                    issues.Add($"• Year mismatch: VIN indicates {decodedYear}, but {_selectedVehicle.Year} selected");
                }
            }
            
            // Check ECU compatibility for specific tune types
            if (tune.Category == "Delete" && _selectedVehicle.FuelType == "Diesel")
            {
                var ecuTypes = _selectedVehicle.EcuTypes ?? new List<string>();
                if (!ecuTypes.Any(ecu => ecu.Contains("Bosch") || ecu.Contains("Delphi") || ecu.Contains("Cummins")))
                {
                    issues.Add($"• ECU type may not support this delete tune");
                }
            }
            
            // Check for racing tunes on older vehicles
            if (tune.Category == "Racing" && _selectedVehicle.Year < 2005)
            {
                issues.Add($"• Racing tune may not be optimized for {_selectedVehicle.Year} model year");
            }
            
            return issues.Count > 0 ? string.Join("\n", issues) : string.Empty;
        }
        
        private void UpdateTuneInfo()
        {
            if (Directory.Exists(_tunePath))
            {
                var files = Directory.GetFiles(_tunePath, "*.*", SearchOption.AllDirectories);
                TotalTunes.Text = $"{files.Length} files";
                
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
    
    public class TuneItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PowerGain { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public bool RequiresVin { get; set; } = false;
        public string Category { get; set; } = "Street";
    }
}
