using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace BlackFlag.Views
{
    public partial class LiveDataPage : Page
    {
        private SerialPort? _obdPort;
        private bool _isConnected = false;
        private bool _isStreaming = false;
        private CancellationTokenSource? _streamCts;
        
        // Chart data
        private readonly ObservableCollection<double> _rpmData = new();
        private readonly ObservableCollection<double> _speedData = new();
        private readonly ObservableCollection<double> _coolantData = new();
        private readonly ObservableCollection<double> _iatData = new();
        private readonly ObservableCollection<double> _stftData = new();
        private readonly ObservableCollection<double> _ltftData = new();
        private readonly ObservableCollection<double> _loadData = new();
        private readonly ObservableCollection<double> _throttleData = new();
        
        private int _sampleCount = 0;
        private readonly int _maxSamples = 100;
        
        public LiveDataPage()
        {
            InitializeComponent();
            Loaded += LiveDataPage_Loaded;
        }

        private void LiveDataPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            // RPM/Speed Chart
            RpmSpeedChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _rpmData,
                    Name = "RPM",
                    Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                },
                new LineSeries<double>
                {
                    Values = _speedData,
                    Name = "Speed (MPH)",
                    Stroke = new SolidColorPaint(SKColors.LimeGreen) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                }
            };
            
            // Temperature Chart
            TempChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _coolantData,
                    Name = "Coolant Temp (°F)",
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                },
                new LineSeries<double>
                {
                    Values = _iatData,
                    Name = "Intake Air Temp (°F)",
                    Stroke = new SolidColorPaint(SKColors.SkyBlue) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                }
            };
            
            // Fuel Trim Chart
            FuelTrimChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _stftData,
                    Name = "Short Term FT (%)",
                    Stroke = new SolidColorPaint(SKColors.Yellow) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                },
                new LineSeries<double>
                {
                    Values = _ltftData,
                    Name = "Long Term FT (%)",
                    Stroke = new SolidColorPaint(SKColors.Magenta) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                }
            };
            
            // Load/Throttle Chart
            LoadThrottleChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _loadData,
                    Name = "Engine Load (%)",
                    Stroke = new SolidColorPaint(SKColors.Cyan) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                },
                new LineSeries<double>
                {
                    Values = _throttleData,
                    Name = "Throttle Position (%)",
                    Stroke = new SolidColorPaint(SKColors.GreenYellow) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0
                }
            };
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConnected)
            {
                await ConnectToOBD();
            }
            else
            {
                DisconnectFromOBD();
            }
        }

        private async Task ConnectToOBD()
        {
            try
            {
                StatusText.Text = "Connecting...";
                
                // Simulate OBD connection (in real app, use actual serial port)
                await Task.Delay(500);
                
                // In production, use:
                // _obdPort = new SerialPort("COM3", 38400, Parity.None, 8, StopBits.One);
                // _obdPort.Open();
                // await SendCommand("ATZ"); // Reset
                // await SendCommand("ATE0"); // Echo off
                // await SendCommand("ATL0"); // Linefeeds off
                // await SendCommand("ATSP0"); // Auto protocol
                
                _isConnected = true;
                ConnectBtn.Content = "🔌 DISCONNECT";
                StatusText.Text = "Connected - Streaming Data";
                StatusIndicator.Foreground = Brushes.LimeGreen;
                
                // Start data streaming
                await StartDataStream();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "OBD2 Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Connection Failed";
            }
        }

        private void DisconnectFromOBD()
        {
            _streamCts?.Cancel();
            _obdPort?.Close();
            _obdPort?.Dispose();
            
            _isConnected = false;
            _isStreaming = false;
            ConnectBtn.Content = "🔌 CONNECT";
            StatusText.Text = "Disconnected";
            StatusIndicator.Foreground = Brushes.Red;
        }

        private async Task StartDataStream()
        {
            _isStreaming = true;
            _streamCts = new CancellationTokenSource();
            var token = _streamCts.Token;
            
            await Task.Run(async () =>
            {
                var random = new Random();
                int samplesPerSecond = 0;
                var lastSecond = DateTime.Now;
                
                while (_isStreaming && !token.IsCancellationRequested)
                {
                    try
                    {
                        // Simulate OBD2 PID queries (in real app, send actual PID commands)
                        var rpm = await ReadPID("010C", random); // Engine RPM
                        var speed = await ReadPID("010D", random); // Vehicle Speed
                        var coolant = await ReadPID("0105", random); // Coolant Temp
                        var throttle = await ReadPID("0111", random); // Throttle Position
                        var maf = await ReadPID("0110", random); // MAF
                        var iat = await ReadPID("010F", random); // Intake Air Temp
                        var o2 = await ReadPID("0114", random); // O2 Sensor
                        var stft = await ReadPID("0106", random); // Short Term Fuel Trim
                        var ltft = await ReadPID("0107", random); // Long Term Fuel Trim
                        var timing = await ReadPID("010E", random); // Timing Advance
                        var load = await ReadPID("0104", random); // Engine Load
                        var fuelLevel = await ReadPID("012F", random); // Fuel Level
                        var fuelPressure = await ReadPID("010A", random); // Fuel Pressure
                        
                        // Update UI on dispatcher thread
                        await Dispatcher.InvokeAsync(() =>
                        {
                            UpdateGauges(rpm, speed, coolant, throttle);
                            UpdateCharts(rpm, speed, coolant, iat, stft, ltft, load, throttle);
                            UpdateDataGrid(maf, iat, fuelPressure, o2, stft, ltft, timing, load, fuelLevel);
                            
                            samplesPerSecond++;
                            if ((DateTime.Now - lastSecond).TotalSeconds >= 1)
                            {
                                DataRateText.Text = $"{samplesPerSecond} samples/sec";
                                samplesPerSecond = 0;
                                lastSecond = DateTime.Now;
                            }
                        });
                        
                        await Task.Delay(100, token); // 10 Hz update rate
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Streaming error: {ex.Message}");
                    }
                }
            }, token);
        }

        private Task<double> ReadPID(string pid, Random random)
        {
            // Simulate reading PID (in production, send actual OBD command and parse response)
            // Example real implementation:
            // await SendCommand(pid);
            // var response = await ReadResponse();
            // return ParsePIDResponse(pid, response);
            
            return Task.FromResult(pid switch
            {
                "010C" => 750 + random.Next(-200, 2000), // RPM (750-2750)
                "010D" => random.Next(0, 80), // Speed (0-80 mph)
                "0105" => 180 + random.Next(-10, 30), // Coolant (180-210°F)
                "0111" => random.Next(0, 100), // Throttle (0-100%)
                "0110" => 2.5 + (random.NextDouble() * 3), // MAF (2.5-5.5 g/s)
                "010F" => 70 + random.Next(-10, 30), // IAT (70-100°F)
                "0114" => 0.1 + (random.NextDouble() * 0.8), // O2 (0.1-0.9V)
                "0106" => -5 + (random.NextDouble() * 10), // STFT (-5 to +5%)
                "0107" => -3 + (random.NextDouble() * 6), // LTFT (-3 to +3%)
                "010E" => 10 + (random.NextDouble() * 20), // Timing (10-30°)
                "0104" => random.Next(10, 90), // Load (10-90%)
                "012F" => 50 + random.Next(-20, 30), // Fuel Level (50-80%)
                "010A" => 40 + random.Next(-5, 10), // Fuel Pressure (40-50 psi)
                _ => 0
            });
        }

        private void UpdateGauges(double rpm, double speed, double coolant, double throttle)
        {
            RpmValue.Text = ((int)rpm).ToString();
            RpmBar.Value = rpm;
            
            SpeedValue.Text = ((int)speed).ToString();
            SpeedBar.Value = speed;
            
            CoolantValue.Text = ((int)coolant).ToString();
            CoolantBar.Value = coolant;
            
            ThrottleValue.Text = ((int)throttle).ToString();
            ThrottleBar.Value = throttle;
        }

        private void UpdateCharts(double rpm, double speed, double coolant, double iat, 
                                  double stft, double ltft, double load, double throttle)
        {
            _rpmData.Add(rpm / 10); // Scale RPM for chart
            _speedData.Add(speed);
            _coolantData.Add(coolant);
            _iatData.Add(iat);
            _stftData.Add(stft);
            _ltftData.Add(ltft);
            _loadData.Add(load);
            _throttleData.Add(throttle);
            
            // Keep only last N samples
            if (_rpmData.Count > _maxSamples)
            {
                _rpmData.RemoveAt(0);
                _speedData.RemoveAt(0);
                _coolantData.RemoveAt(0);
                _iatData.RemoveAt(0);
                _stftData.RemoveAt(0);
                _ltftData.RemoveAt(0);
                _loadData.RemoveAt(0);
                _throttleData.RemoveAt(0);
            }
        }

        private void UpdateDataGrid(double maf, double iat, double fuelPressure, double o2,
                                    double stft, double ltft, double timing, double load, double fuelLevel)
        {
            MafValue.Text = $"{maf:F2} g/s";
            IatValue.Text = $"{iat:F0}°F";
            FuelPressValue.Text = $"{fuelPressure:F0} psi";
            O2Value.Text = $"{o2:F2} V";
            StftValue.Text = $"{stft:F1}%";
            LtftValue.Text = $"{ltft:F1}%";
            TimingValue.Text = $"{timing:F1}°";
            LoadValue.Text = $"{load:F0}%";
            FuelLevelValue.Text = $"{fuelLevel:F0}%";
        }
    }
}
