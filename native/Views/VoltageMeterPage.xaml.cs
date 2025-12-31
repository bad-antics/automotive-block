using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BlackFlag.Views
{
    public partial class VoltageMeterPage : Page
    {
        private DispatcherTimer? _timer;
        private readonly Random _random = new();
        private readonly List<double> _readings = new();
        private double _min = double.MaxValue;
        private double _max = double.MinValue;
        
        public VoltageMeterPage()
        {
            InitializeComponent();
        }
        
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _readings.Clear();
            _min = double.MaxValue;
            _max = double.MinValue;
            
            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            _timer = null;
            
            StartBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
        }
        
        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Simulate voltage reading around 12.6V nominal
            var voltage = 12.6 + (_random.NextDouble() * 1.4 - 0.7);
            voltage = Math.Round(voltage, 1);
            
            _readings.Add(voltage);
            if (_readings.Count > 100) _readings.RemoveAt(0);
            
            if (voltage < _min) _min = voltage;
            if (voltage > _max) _max = voltage;
            
            // Calculate average
            double sum = 0;
            foreach (var v in _readings) sum += v;
            var avg = sum / _readings.Count;
            
            // Update display
            VoltageDisplay.Text = voltage.ToString("F1");
            VoltageBar.Value = (voltage / 15.0) * 100;
            
            MinVoltage.Text = $"{_min:F1}V";
            MaxVoltage.Text = $"{_max:F1}V";
            AvgVoltage.Text = $"{avg:F1}V";
            
            // Color based on voltage level
            if (voltage < 11.5)
                VoltageDisplay.Foreground = System.Windows.Media.Brushes.Red;
            else if (voltage > 14.5)
                VoltageDisplay.Foreground = System.Windows.Media.Brushes.Orange;
            else
                VoltageDisplay.Foreground = (System.Windows.Media.Brush)FindResource("AccentBrush");
        }
    }
}
