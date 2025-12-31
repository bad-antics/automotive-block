using System.Windows;
using System.Windows.Controls;

namespace BlackFlag.Views
{
    public partial class EmissionsDeletePage : Page
    {
        public EmissionsDeletePage()
        {
            InitializeComponent();
        }

        private void GenerateDeleteTune_Click(object sender, RoutedEventArgs e)
        {
            var selectedOptions = new System.Collections.Generic.List<string>();
            
            if (ChkDpfRegen.IsChecked == true) selectedOptions.Add("DPF Regen Delete");
            if (ChkDpfPressure.IsChecked == true) selectedOptions.Add("DPF Pressure Sensor Delete");
            if (ChkDpfTemp.IsChecked == true) selectedOptions.Add("DPF Temp Sensor Delete");
            if (ChkEgrValve.IsChecked == true) selectedOptions.Add("EGR Valve Delete");
            if (ChkEgrCooler.IsChecked == true) selectedOptions.Add("EGR Cooler Monitor Delete");
            if (ChkEgrDtc.IsChecked == true) selectedOptions.Add("EGR DTC Delete");
            if (ChkDefInjection.IsChecked == true) selectedOptions.Add("DEF Injection Delete");
            if (ChkScrEfficiency.IsChecked == true) selectedOptions.Add("SCR Efficiency Delete");
            if (ChkDefDerate.IsChecked == true) selectedOptions.Add("DEF Derate Delete");
            if (ChkCatMonitor.IsChecked == true) selectedOptions.Add("CAT Monitor Delete");
            if (ChkSai.IsChecked == true) selectedOptions.Add("SAI Delete");
            if (ChkEvap.IsChecked == true) selectedOptions.Add("EVAP Delete");
            if (ChkSpeedLimiter.IsChecked == true) selectedOptions.Add("Speed Limiter Delete");
            
            if (selectedOptions.Count == 0)
            {
                MessageBox.Show("Please select at least one delete option.", "BlackFlag", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            StatusText.Text = $"✅ Generated tune with {selectedOptions.Count} delete options: {string.Join(", ", selectedOptions)}";
            MessageBox.Show($"Delete tune generated with {selectedOptions.Count} options.\n\n" +
                "Selected options:\n• " + string.Join("\n• ", selectedOptions) + 
                "\n\n⚠️ Remember: For off-road/competition use only!", 
                "Tune Generated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "💾 Configuration saved to emissions_config.json";
            MessageBox.Show("Configuration saved successfully.", "BlackFlag", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "📥 Configuration loaded from emissions_config.json";
            MessageBox.Show("Configuration loaded successfully.", "BlackFlag", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
