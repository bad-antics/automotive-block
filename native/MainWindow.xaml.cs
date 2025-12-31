using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BlackFlag.Models;
using BlackFlag.Services;
using BlackFlag.Views;

namespace BlackFlag
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Vehicle> VehicleHistory { get; } = new();
        private Vehicle? _selectedVehicle;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainWindow_Loaded;
            ContentFrame.Navigate(new DashboardPage());
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (ManufacturerCombo == null || VehicleCombo == null) return;
            
            // Load manufacturers
            var manufacturers = Database.Instance.GetManufacturers();
            Console.WriteLine($"Found {manufacturers.Count} manufacturers");
            
            ManufacturerCombo.Items.Clear();
            ManufacturerCombo.Items.Add("All Manufacturers");
            foreach (var mfg in manufacturers)
            {
                ManufacturerCombo.Items.Add(mfg);
                Console.WriteLine($"Added manufacturer: {mfg}");
            }
            ManufacturerCombo.SelectedIndex = 0;

            // Load vehicle history
            var history = Database.Instance.GetVehicleHistory();
            VehicleHistory.Clear();
            foreach (var v in history)
            {
                VehicleHistory.Add(v);
            }
        }

        private void ManufacturerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManufacturerCombo == null || VehicleCombo == null) return;
            if (ManufacturerCombo.SelectedItem == null) return;

            var selected = ManufacturerCombo.SelectedItem.ToString();
            var vehicles = selected == "All Manufacturers"
                ? Database.Instance.GetVehicles()
                : Database.Instance.GetVehiclesByManufacturer(selected!);

            // Sort by year descending (newest first)
            var sortedVehicles = vehicles.OrderByDescending(v => v.Year).ToList();
            
            Console.WriteLine($"Loading {sortedVehicles.Count} vehicles for {selected}");

            VehicleCombo.Items.Clear();
            VehicleCombo.Items.Add("Select a vehicle...");
            foreach (var v in sortedVehicles)
            {
                VehicleCombo.Items.Add(v);
            }
            VehicleCombo.SelectedIndex = 0;
        }

        private void VehicleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VehicleCombo.SelectedItem is Vehicle vehicle)
            {
                _selectedVehicle = vehicle;
                
                // Build detailed info string with ECU and specs
                var ecuList = vehicle.EcuTypes != null && vehicle.EcuTypes.Count > 0 
                    ? string.Join(", ", vehicle.EcuTypes) 
                    : "N/A";
                    
                SelectedVehicleInfo.Text = $"⚔️ {vehicle.Year} {vehicle.Make} {vehicle.Model} | {vehicle.Engine} | {vehicle.Power} | ECU: {ecuList}";
            }
        }

        private void AddToHistory_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVehicle == null)
            {
                MessageBox.Show("Please select a vehicle first.", "BlackFlag", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!VehicleHistory.Any(v => v.Id == _selectedVehicle.Id))
            {
                VehicleHistory.Insert(0, _selectedVehicle);
                Database.Instance.AddToHistory(_selectedVehicle);
            }
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                Page? page = rb.Name switch
                {
                    "NavHome" => new DashboardPage(),
                    "NavVinDecoder" => new VinDecoderPage(),
                    "NavEcuScanner" => new EcuScannerPage(),
                    "NavVoltageMeter" => new VoltageMeterPage(),
                    "NavWiringDiagrams" => new WiringDiagramsPage(),
                    "NavTuneManager" => new TuneManagerPage(),
                    "NavEcuCloning" => new EcuCloningPage(),
                    "NavPerformance" => new PerformancePage(),
                    "NavEmissionsDelete" => new EmissionsDeletePage(),
                    "NavJ2534" => new J2534Page(),
                    "NavEcuUnlock" => new EcuUnlockPage(),
                    "NavLiveData" => new LiveDataPage(),
                    "NavOeFlash" => new OeFlashPage(),
                    _ => null
                };

                if (page != null)
                {
                    ContentFrame.Navigate(page);
                }
            }
        }

        private void HistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HistoryList.SelectedItem is Vehicle vehicle)
            {
                _selectedVehicle = vehicle;
                SelectedVehicleInfo.Text = $"⚔️ {vehicle.Year} {vehicle.Make} {vehicle.Model} | {vehicle.Engine}";
            }
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedIndex < 0) return;

            string themePath = ThemeSelector.SelectedIndex switch
            {
                0 => "Themes/DarkTheme.xaml",
                1 => "Themes/RetroGreenTheme.xaml",
                2 => "Themes/FordBlueTheme.xaml",
                3 => "Themes/OrangeTechTheme.xaml",
                _ => "Themes/DarkTheme.xaml"
            };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) });
        }
    }
}
