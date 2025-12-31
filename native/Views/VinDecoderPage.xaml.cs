using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BlackFlag.Views
{
    public partial class VinDecoderPage : Page
    {
        private readonly Dictionary<string, string> _wmiCodes = new()
        {
            { "1FA", "Ford USA" }, { "1FT", "Ford Truck USA" }, { "1G1", "Chevrolet USA" },
            { "1GC", "Chevrolet Truck" }, { "2B3", "Dodge" }, { "WBA", "BMW Germany" },
            { "WBS", "BMW M" }, { "WVW", "Volkswagen" }, { "JHM", "Honda Japan" },
            { "5YJ", "Tesla" }, { "WAU", "Audi" }, { "WDB", "Mercedes-Benz" },
            { "ZFF", "Ferrari" }, { "1J4", "Jeep" }, { "3FA", "Ford Mexico" },
            { "JTD", "Toyota" }, { "WF0", "Ford Europe" }
        };
        
        private readonly Dictionary<char, int> _yearCodes = new()
        {
            { 'A', 2010 }, { 'B', 2011 }, { 'C', 2012 }, { 'D', 2013 },
            { 'E', 2014 }, { 'F', 2015 }, { 'G', 2016 }, { 'H', 2017 },
            { 'J', 2018 }, { 'K', 2019 }, { 'L', 2020 }, { 'M', 2021 },
            { 'N', 2022 }, { 'P', 2023 }, { 'R', 2024 }, { 'S', 2025 }
        };
        
        public VinDecoderPage()
        {
            InitializeComponent();
            VinInput.TextChanged += (s, e) => 
            {
                VinLength.Text = $"{VinInput.Text.Length}/17 characters";
            };
        }
        
        private void DecodeVin_Click(object sender, RoutedEventArgs e)
        {
            var vin = VinInput.Text.Trim().ToUpper();
            
            if (vin.Length != 17)
            {
                MessageBox.Show("VIN must be exactly 17 characters.", "Invalid VIN", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Extract VIN sections
            var wmi = vin.Substring(0, 3);
            var vds = vin.Substring(3, 6);
            var yearChar = vin[9];
            var plantCode = vin[10];
            var serial = vin.Substring(11, 6);
            
            // Get manufacturer
            var manufacturer = _wmiCodes.TryGetValue(wmi, out var mfg) ? mfg : "Unknown";
            
            // Get year
            var year = _yearCodes.TryGetValue(yearChar, out var y) ? y.ToString() : "Unknown";
            
            // Display results
            WmiResult.Text = wmi;
            ManufacturerResult.Text = manufacturer;
            VdsResult.Text = vds;
            YearResult.Text = year;
            PlantResult.Text = plantCode.ToString();
            SerialResult.Text = serial;
            
            ResultsPanel.Visibility = Visibility.Visible;
        }
    }
}
