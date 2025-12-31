using System.Windows;
using System.Windows.Controls;

namespace BlackFlag.Views
{
    public partial class PerformancePage : Page
    {
        public PerformancePage()
        {
            InitializeComponent();
        }
        
        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                ContentPanel.Children.Clear();
                
                switch (rb.Name)
                {
                    case "TabFuel":
                        ShowFuelContent();
                        break;
                    case "TabTiming":
                        ShowTimingContent();
                        break;
                    case "TabBoost":
                        ShowBoostContent();
                        break;
                    case "TabLimiters":
                        ShowLimitersContent();
                        break;
                }
            }
        }
        
        private void ShowFuelContent()
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "FUEL MAP EDITOR",
                FontWeight = FontWeights.Bold,
                Foreground = (System.Windows.Media.Brush)FindResource("AccentBrush"),
                Margin = new Thickness(0, 0, 0, 15)
            });
            
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Adjust fuel delivery parameters:\n\n• Base AFR: 14.7:1 (Stoichiometric)\n• WOT AFR: 12.5:1 (Rich for power)\n• Cruise AFR: 15.5:1 (Lean for economy)\n• Cold Start: +15% enrichment",
                Foreground = (System.Windows.Media.Brush)FindResource("ForegroundBrush")
            });
        }
        
        private void ShowTimingContent()
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "IGNITION TIMING",
                FontWeight = FontWeights.Bold,
                Foreground = (System.Windows.Media.Brush)FindResource("AccentBrush"),
                Margin = new Thickness(0, 0, 0, 15)
            });
            
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Adjust ignition timing parameters:\n\n• Base Timing: 10° BTDC\n• Max Advance: 35° @ 3000 RPM\n• Retard on Knock: -3° per event\n• High Load Timing: 28°",
                Foreground = (System.Windows.Media.Brush)FindResource("ForegroundBrush")
            });
        }
        
        private void ShowBoostContent()
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "BOOST CONTROL",
                FontWeight = FontWeights.Bold,
                Foreground = (System.Windows.Media.Brush)FindResource("AccentBrush"),
                Margin = new Thickness(0, 0, 0, 15)
            });
            
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Adjust turbo boost parameters:\n\n• Target Boost: 18 PSI\n• Wastegate Duty: 45%\n• Boost by Gear enabled\n• Overboost Protection: 22 PSI",
                Foreground = (System.Windows.Media.Brush)FindResource("ForegroundBrush")
            });
        }
        
        private void ShowLimitersContent()
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "LIMITERS",
                FontWeight = FontWeights.Bold,
                Foreground = (System.Windows.Media.Brush)FindResource("AccentBrush"),
                Margin = new Thickness(0, 0, 0, 15)
            });
            
            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Adjust engine limiters:\n\n• Rev Limiter: 7000 RPM\n• Speed Limiter: 155 MPH\n• Launch Control: 4500 RPM\n• Flat Foot Shifting: Enabled",
                Foreground = (System.Windows.Media.Brush)FindResource("ForegroundBrush")
            });
        }
    }
}
