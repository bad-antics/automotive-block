using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using BlackFlag.Models;

namespace BlackFlag.Views
{
    public partial class WiringDiagramsPage : Page
    {
        private Dictionary<string, List<PinInfo>> pinDatabase = new();
        private double currentZoom = 1.0;
        
        public WiringDiagramsPage()
        {
            InitializeComponent();
            InitializePinDatabase();
        }

        // PART 1: PIN DATABASE INITIALIZATION
        private void InitializePinDatabase()
        {
            pinDatabase = new Dictionary<string, List<PinInfo>>();

            // OBD-II 16-Pin Complete Database
            pinDatabase["OBD-II"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "Manufacturer Discretion", WireColor = "Brown", WireColorCode = Colors.Brown, Voltage = "Varies", SignalType = "Varies", Notes = "Manufacturer specific", Description = "Used for manufacturer-specific diagnostic features" },
                new PinInfo { Pin = "2", Function = "J1850 Bus+", WireColor = "Red/Black", WireColorCode = Colors.Red, Voltage = "7V", SignalType = "PWM", Notes = "GM vehicles primarily", Description = "SAE J1850 VPW (Variable Pulse Width) bus positive" },
                new PinInfo { Pin = "3", Function = "Manufacturer Discretion", WireColor = "Gray", WireColorCode = Colors.Gray, Voltage = "Varies", SignalType = "Varies", Notes = "Manufacturer specific", Description = "Optional manufacturer protocol or feature" },
                new PinInfo { Pin = "4", Function = "Chassis Ground", WireColor = "Black", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "Vehicle frame ground", Description = "Main chassis ground connection" },
                new PinInfo { Pin = "5", Function = "Signal Ground", WireColor = "Black/White", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "Isolated signal ground", Description = "Dedicated ground for communication signals" },
                new PinInfo { Pin = "6", Function = "CAN High (J2284)", WireColor = "Yellow", WireColorCode = Colors.Yellow, Voltage = "2.5-3.5V", SignalType = "CAN", Notes = "ISO 15765-4", Description = "CAN Bus High line - Modern vehicles" },
                new PinInfo { Pin = "7", Function = "K-Line (ISO 9141)", WireColor = "Orange", WireColorCode = Colors.Orange, Voltage = "12V", SignalType = "Serial", Notes = "European vehicles", Description = "ISO 9141-2 / ISO 14230 K-Line diagnostic" },
                new PinInfo { Pin = "8", Function = "Manufacturer Discretion", WireColor = "Blue", WireColorCode = Colors.Blue, Voltage = "Varies", SignalType = "Varies", Notes = "Manufacturer specific", Description = "Optional network or feature" },
                new PinInfo { Pin = "9", Function = "Manufacturer Discretion", WireColor = "Purple", WireColorCode = Colors.Purple, Voltage = "Varies", SignalType = "Varies", Notes = "Manufacturer specific", Description = "Optional manufacturer use" },
                new PinInfo { Pin = "10", Function = "J1850 Bus-", WireColor = "Black/Yellow", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "PWM", Notes = "GM/Ford vehicles", Description = "SAE J1850 bus negative/ground" },
                new PinInfo { Pin = "11", Function = "Manufacturer Discretion", WireColor = "White", WireColorCode = Colors.White, Voltage = "Varies", SignalType = "Varies", Notes = "Manufacturer specific", Description = "Optional feature or protocol" },
                new PinInfo { Pin = "12", Function = "Manufacturer Discretion", WireColor = "Green", WireColorCode = Colors.Green, Voltage = "Varies", SignalType = "Varies", Notes = "Manufacturer specific", Description = "Optional use by manufacturer" },
                new PinInfo { Pin = "13", Function = "Manufacturer Discretion", WireColor = "Pink", WireColorCode = Colors.Pink, Voltage = "Varies", SignalType = "Varies", Notes = "Manufacturer specific", Description = "Optional manufacturer feature" },
                new PinInfo { Pin = "14", Function = "CAN Low (J2284)", WireColor = "Green/White", WireColorCode = Colors.Green, Voltage = "1.5-2.5V", SignalType = "CAN", Notes = "ISO 15765-4", Description = "CAN Bus Low line - Modern vehicles" },
                new PinInfo { Pin = "15", Function = "L-Line (ISO 9141)", WireColor = "Brown/White", WireColorCode = Colors.Brown, Voltage = "12V", SignalType = "Serial", Notes = "Some European vehicles", Description = "ISO 9141-2 L-Line (optional)" },
                new PinInfo { Pin = "16", Function = "Battery Power", WireColor = "Red", WireColorCode = Colors.Red, Voltage = "12-14V", SignalType = "Power", Notes = "Always hot", Description = "Constant battery voltage (unswitched)" }
            };

            // O2 Sensor (4-Wire Heated Narrowband)
            pinDatabase["O2 Sensor"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "Signal +", WireColor = "White", WireColorCode = Colors.White, Voltage = "0-1V", SignalType = "Analog", Notes = "To ECM", Description = "Oxygen sensor signal voltage (lean=0.1V, rich=0.9V)" },
                new PinInfo { Pin = "2", Function = "Signal Ground", WireColor = "Black", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "Signal return", Description = "Dedicated ground for sensor signal" },
                new PinInfo { Pin = "3", Function = "Heater Power +", WireColor = "Red/Black", WireColorCode = Colors.Red, Voltage = "12V", SignalType = "Power", Notes = "From relay", Description = "Heater element power supply" },
                new PinInfo { Pin = "4", Function = "Heater Ground", WireColor = "Black/White", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "PWM Ground", Notes = "ECM controlled", Description = "ECM-switched ground for heater circuit" }
            };

            // Wideband AFR Sensor (6-Wire)
            pinDatabase["Wideband AFR"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "Heater Power +", WireColor = "Red", WireColorCode = Colors.Red, Voltage = "12V", SignalType = "Power", Notes = "Constant", Description = "Heater power supply" },
                new PinInfo { Pin = "2", Function = "Heater Ground", WireColor = "Black", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "ECM controlled", Description = "Heater circuit ground" },
                new PinInfo { Pin = "3", Function = "Pump Cell +", WireColor = "White", WireColorCode = Colors.White, Voltage = "2.5-3.3V", SignalType = "Analog", Notes = "Reference", Description = "Oxygen pump cell positive" },
                new PinInfo { Pin = "4", Function = "Pump Cell -", WireColor = "Gray", WireColorCode = Colors.Gray, Voltage = "Variable", SignalType = "Analog", Notes = "Controlled by ECM", Description = "Oxygen pump cell negative" },
                new PinInfo { Pin = "5", Function = "Nernst Cell +", WireColor = "Yellow", WireColorCode = Colors.Yellow, Voltage = "~450mV", SignalType = "Analog", Notes = "Reference", Description = "Nernst sensing cell positive" },
                new PinInfo { Pin = "6", Function = "Nernst Cell -", WireColor = "Green", WireColorCode = Colors.Green, Voltage = "Variable", SignalType = "Analog", Notes = "Signal", Description = "Nernst sensing cell negative" }
            };

            // MAP/MAF Sensor (3-Wire)
            pinDatabase["MAP Sensor"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "5V Reference", WireColor = "Red/White", WireColorCode = Colors.Red, Voltage = "5V", SignalType = "Power", Notes = "From ECM", Description = "Regulated 5V supply from ECM" },
                new PinInfo { Pin = "2", Function = "MAP Signal", WireColor = "Green/White", WireColorCode = Colors.Green, Voltage = "0.5-4.5V", SignalType = "Analog", Notes = "Varies with pressure", Description = "Manifold pressure signal output" },
                new PinInfo { Pin = "3", Function = "Sensor Ground", WireColor = "Black/Green", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "Signal return", Description = "Dedicated sensor ground" }
            };

            // TPS Sensor (3-Wire)
            pinDatabase["TPS"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "5V Reference", WireColor = "Gray/Red", WireColorCode = Colors.Gray, Voltage = "5V", SignalType = "Power", Notes = "From ECM", Description = "Regulated 5V supply from ECM" },
                new PinInfo { Pin = "2", Function = "TPS Signal", WireColor = "Dark Blue", WireColorCode = Colors.DarkBlue, Voltage = "0.5-4.5V", SignalType = "Analog", Notes = "Varies with position", Description = "Throttle position signal output (potentiometer)" },
                new PinInfo { Pin = "3", Function = "Sensor Ground", WireColor = "Black/Blue", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "Signal return", Description = "Dedicated sensor ground" }
            };

            // Crankshaft Position Sensor (Hall Effect)
            pinDatabase["Crank Sensor"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "5V Reference", WireColor = "Pink", WireColorCode = Colors.Pink, Voltage = "5V", SignalType = "Power", Notes = "From ECM", Description = "Hall effect sensor power supply" },
                new PinInfo { Pin = "2", Function = "CKP Signal", WireColor = "Light Blue", WireColorCode = Colors.LightBlue, Voltage = "0-5V", SignalType = "Digital Square Wave", Notes = "RPM signal", Description = "Crankshaft position signal output" },
                new PinInfo { Pin = "3", Function = "Sensor Ground", WireColor = "Black", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "Signal return", Description = "Sensor ground" }
            };

            // Camshaft Position Sensor (Hall Effect)
            pinDatabase["Cam Sensor"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "5V Reference", WireColor = "Orange", WireColorCode = Colors.Orange, Voltage = "5V", SignalType = "Power", Notes = "From ECM", Description = "Hall effect sensor power supply" },
                new PinInfo { Pin = "2", Function = "CMP Signal", WireColor = "Purple", WireColorCode = Colors.Purple, Voltage = "0-5V", SignalType = "Digital Square Wave", Notes = "Cylinder ID", Description = "Camshaft position signal output" },
                new PinInfo { Pin = "3", Function = "Sensor Ground", WireColor = "Black", WireColorCode = Colors.Black, Voltage = "0V", SignalType = "Ground", Notes = "Signal return", Description = "Sensor ground" }
            };

            // Fuel Injector (2-Wire)
            pinDatabase["Injector"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "12V Power", WireColor = "Red/Yellow", WireColorCode = Colors.Red, Voltage = "12V", SignalType = "Power", Notes = "From fuel pump relay", Description = "Constant 12V power supply" },
                new PinInfo { Pin = "2", Function = "ECM Control", WireColor = "Dark Green", WireColorCode = Colors.DarkGreen, Voltage = "PWM", SignalType = "PWM Ground", Notes = "Duty cycle varies", Description = "ECM-switched ground (pulse width modulated)" }
            };

            // Ignition Coil (3-Wire COP)
            pinDatabase["Ignition Coil"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "12V Switched", WireColor = "Pink", WireColorCode = Colors.Pink, Voltage = "12V", SignalType = "Power", Notes = "Ignition on", Description = "Switched 12V power supply" },
                new PinInfo { Pin = "2", Function = "IGT Trigger", WireColor = "Black/Yellow", WireColorCode = Colors.Black, Voltage = "0-5V", SignalType = "Digital", Notes = "From ECM", Description = "Ignition trigger signal from ECM" },
                new PinInfo { Pin = "3", Function = "IGF Feedback", WireColor = "Green/Red", WireColorCode = Colors.Green, Voltage = "0-5V", SignalType = "Digital", Notes = "To ECM", Description = "Ignition confirmation feedback to ECM" }
            };

            // VVT Solenoid (2-Wire)
            pinDatabase["VVT Solenoid"] = new List<PinInfo>
            {
                new PinInfo { Pin = "1", Function = "12V Power", WireColor = "Orange/White", WireColorCode = Colors.Orange, Voltage = "12V", SignalType = "Power", Notes = "From main relay", Description = "Constant 12V power supply" },
                new PinInfo { Pin = "2", Function = "ECM Control", WireColor = "Purple", WireColorCode = Colors.Purple, Voltage = "PWM", SignalType = "PWM Ground", Notes = "Duty cycle varies", Description = "ECM-switched PWM ground for oil control valve" }
            };
        }

        // PART 2: EVENT HANDLERS AND INTERACTIVITY
        private void CategorySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategorySelector?.SelectedItem is ComboBoxItem categoryItem)
            {
                var category = categoryItem.Content?.ToString() ?? "";
                FilterDiagramsByCategory(category);
            }
        }

        private void FilterDiagramsByCategory(string category)
        {
            if (DiagramSelector == null) return;
            
            DiagramSelector.Items.Clear();
            
            var diagrams = new List<(string Display, string Category)>
            {
                ("OBD-II Connector Pinout", "OBD-II / J1962"),
                ("J1962 Connector", "OBD-II / J1962"),
                ("CAN Bus Network", "CAN Bus"),
                ("CAN Bus Troubleshooting Guide", "CAN Bus"),
                ("K-Line (ISO 9141) Wiring", "K-Line / ISO"),
                ("LIN Bus Network", "LIN / FlexRay / Ethernet"),
                ("FlexRay Network", "LIN / FlexRay / Ethernet"),
                ("Ethernet (DoIP) Network", "LIN / FlexRay / Ethernet"),
                ("Ford PATS Wiring", "Manufacturer Specific"),
                ("GM Class 2 Serial Data", "Manufacturer Specific"),
                ("Chrysler CCD/PCI Bus", "Manufacturer Specific"),
                ("ECU Pinout", "ECU / Bench Setup"),
                ("ECU Bench Wiring Setup", "ECU / Bench Setup"),
                ("Power Supply Requirements", "Power / Grounding"),
                ("O2 Sensor Circuit (Narrowband)", "Sensor Circuits"),
                ("Wideband AFR Sensor Circuit", "Sensor Circuits"),
                ("MAP/MAF Sensor Circuit", "Sensor Circuits"),
                ("TPS Circuit", "Sensor Circuits"),
                ("Crank/Cam Position Sensors", "Sensor Circuits"),
                ("Fuel Injector Circuit", "Actuator Circuits"),
                ("Ignition Coil Circuit (COP)", "Actuator Circuits"),
                ("VVT Solenoid Circuit", "Actuator Circuits"),
                ("Turbo/Supercharger Control", "Actuator Circuits"),
                ("EGR Valve Circuit", "Actuator Circuits"),
                ("DPF System Circuits", "Actuator Circuits"),
                ("DEF/SCR System Circuits", "Actuator Circuits"),
                ("EVAP System Circuits", "Actuator Circuits")
            };

            var filtered = category == "All Diagrams" 
                ? diagrams 
                : diagrams.Where(d => d.Category == category);

            foreach (var diagram in filtered)
            {
                DiagramSelector.Items.Add(new ComboBoxItem { Content = diagram.Display });
            }

            if (DiagramSelector.Items.Count > 0)
            {
                DiagramSelector.SelectedIndex = 0;
            }
        }
        
        private void DiagramSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DiagramSelector?.SelectedItem is ComboBoxItem item)
            {
                var selection = item.Content?.ToString() ?? "";
                
                // Hide interactive elements by default
                ShowInteractivePins(false);
                
                switch (selection)
                {
                    case "OBD-II Connector Pinout":
                        ShowOBD2Pinout();
                        ShowInteractivePins(true, "OBD-II");
                        break;
                    case "O2 Sensor Circuit (Narrowband)":
                        ShowO2Sensor();
                        ShowInteractivePins(true, "O2 Sensor");
                        break;
                    case "Wideband AFR Sensor Circuit":
                        ShowWidebandAFR();
                        ShowInteractivePins(true, "Wideband AFR");
                        break;
                    case "MAP/MAF Sensor Circuit":
                        ShowMAPSensor();
                        ShowInteractivePins(true, "MAP Sensor");
                        break;
                    case "TPS Circuit":
                        ShowTPSSensor();
                        ShowInteractivePins(true, "TPS");
                        break;
                    case "Crank/Cam Position Sensors":
                        ShowCrankCamSensors();
                        break;
                    case "Fuel Injector Circuit":
                        ShowInjectorCircuits();
                        ShowInteractivePins(true, "Injector");
                        break;
                    case "Ignition Coil Circuit (COP)":
                        ShowIgnitionCoil();
                        ShowInteractivePins(true, "Ignition Coil");
                        break;
                    case "VVT Solenoid Circuit":
                        ShowVVTSolenoid();
                        ShowInteractivePins(true, "VVT Solenoid");
                        break;
                    case "Turbo/Supercharger Control":
                        ShowTurboControl();
                        break;
                    case "EGR Valve Circuit":
                        ShowEGRValve();
                        break;
                    case "DPF System Circuits":
                        ShowDPFSystem();
                        break;
                    case "DEF/SCR System Circuits":
                        ShowDEFSCRSystem();
                        break;
                    case "EVAP System Circuits":
                        ShowEVAPSystem();
                        break;
                    case "CAN Bus Network":
                        ShowCANBus();
                        break;
                    case "ECU Pinout":
                        ShowECUPinout();
                        break;
                    case "J1962 Connector":
                        ShowJ1962();
                        break;
                    case "CAN Bus Troubleshooting Guide":
                        ShowCANTroubleshooting();
                        break;
                    case "K-Line (ISO 9141) Wiring":
                        ShowKLine();
                        break;
                    case "LIN Bus Network":
                        ShowLINBus();
                        break;
                    case "FlexRay Network":
                        ShowFlexRay();
                        break;
                    case "Ethernet (DoIP) Network":
                        ShowDoIP();
                        break;
                    case "Ford PATS Wiring":
                        ShowFordPATS();
                        break;
                    case "GM Class 2 Serial Data":
                        ShowGMClass2();
                        break;
                    case "Chrysler CCD/PCI Bus":
                        ShowChryslerBus();
                        break;
                    case "ECU Bench Wiring Setup":
                        ShowBenchSetup();
                        break;
                    case "Power Supply Requirements":
                        ShowPowerRequirements();
                        break;
                    default:
                        if (DiagramTitle != null) DiagramTitle.Text = "Select a diagram type above";
                        if (DiagramContent != null) DiagramContent.Text = "";
                        if (DiagnosticTips != null) DiagnosticTips.Text = "";
                        break;
                }
            }
        }

        private void ShowInteractivePins(bool show, string diagramType = "")
        {
            if (InteractiveCanvas != null)
                InteractiveCanvas.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (PinDetailsGrid != null)
                PinDetailsGrid.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            if (QuickRefList != null)
            {
                if (show && !string.IsNullOrEmpty(diagramType) && pinDatabase.ContainsKey(diagramType))
                {
                    QuickRefList.ItemsSource = pinDatabase[diagramType];
                }
                else
                {
                    QuickRefList.ItemsSource = null;
                }
            }
        }

        private void QuickRef_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is PinInfo pin)
            {
                ShowPinDetails(pin);
            }
        }

        private void ShowPinDetails(PinInfo pin)
        {
            PinNumber.Text = $"Pin {pin.Pin}";
            PinFunction.Text = pin.Function;
            WireColorText.Text = pin.WireColor;
            WireColorSwatch.Background = new SolidColorBrush(pin.WireColorCode);
            PinVoltage.Text = pin.Voltage;
            PinSignalType.Text = pin.SignalType;
            PinNotes.Text = pin.Notes;
        }

        private void CopyDiagram_Click(object sender, RoutedEventArgs e)
        {
            var text = $"{DiagramTitle.Text}\n\n{DiagramContent.Text}\n\nDiagnostic Tips:\n{DiagnosticTips.Text}";
            Clipboard.SetText(text);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            currentZoom = Math.Min(currentZoom + 0.2, 3.0);
            ApplyZoom();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            currentZoom = Math.Max(currentZoom - 0.2, 0.5);
            ApplyZoom();
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            currentZoom = 1.0;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            DiagramContent.LayoutTransform = new ScaleTransform(currentZoom, currentZoom);
        }

        // PART 3: NEW DIAGRAM METHODS
        private void ShowWidebandAFR()
        {
            DiagramTitle.Text = "Wideband AFR Sensor (6-Wire)";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │         WIDEBAND AIR/FUEL RATIO SENSOR                   │
    │              (6-Wire Configuration)                      │
    └─────────────────────────────────────────────────────────┘

                      ┌────────┐
                      │Wideband│
                      │  AFR   │
                      │ Sensor │
                      └───┬────┘
                          │
              ┌───────────┼───────────┐
              │           │           │
           ┌──┴──┐     ┌──┴──┐     ┌──┴──┐
           │1-2  │     │3-4  │     │5-6  │
           │Heat │     │Pump │     │Sense│
           └──┬──┘     └──┬──┘     └──┬──┘
              │           │           │
       ┌──────┴───┐  ┌────┴────┐  ┌──┴────┐
       │ Heater   │  │  Pump   │  │Nernst │
       │ Circuit  │  │  Cell   │  │ Cell  │
       └──────────┘  └─────────┘  └───────┘

    ADVANTAGE OVER NARROWBAND:
    ─────────────────────────────────────────
    • Reads AFR from 10:1 to 20:1
    • Linear output voltage
    • Faster response time
    • More precise fuel control
";
            DiagnosticTips.Text = "• Requires dedicated AFR controller\n• Do not connect to standard O2 sensor input\n• Check all 6 wire connections\n• Common in performance/tuning applications";
        }

        private void ShowOBD2Pinout()
        {
            DiagramTitle.Text = "OBD-II Connector Pinout (J1962)";
            DiagramContent.Text = @"
    ┌─────────────────────────────────┐
    │    OBD-II 16-PIN CONNECTOR      │
    │         (Female View)            │
    └─────────────────────────────────┘

              1   2   3   4   5   6   7   8
            ┌───┬───┬───┬───┬───┬───┬───┬───┐
            │ ○ │ ○ │ ○ │ ○ │ ○ │ ○ │ ○ │ ○ │
            └───┴───┴───┴───┴───┴───┴───┴───┘
            ┌───┬───┬───┬───┬───┬───┬───┬───┐
            │ ○ │ ○ │ ○ │ ○ │ ○ │ ○ │ ○ │ ○ │
            └───┴───┴───┴───┴───┴───┴───┴───┘
              9  10  11  12  13  14  15  16

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    Pin 1:  Manufacturer Discretionary
    Pin 2:  J1850 Bus+ (PWM/VPW)
    Pin 3:  Manufacturer Discretionary
    Pin 4:  CHASSIS GROUND ◄── Essential
    Pin 5:  SIGNAL GROUND ◄── Essential
    Pin 6:  CAN HIGH (ISO 15765) ◄── Modern
    Pin 7:  K-Line (ISO 9141-2)
    Pin 8:  Manufacturer Discretionary
    Pin 9:  Manufacturer Discretionary
    Pin 10: J1850 Bus- (PWM only)
    Pin 11: Manufacturer Discretionary
    Pin 12: Manufacturer Discretionary
    Pin 13: Manufacturer Discretionary
    Pin 14: CAN LOW (ISO 15765) ◄── Modern
    Pin 15: L-Line (ISO 9141-2)
    Pin 16: BATTERY POSITIVE ◄── Essential

    COMMON PROTOCOLS:
    ─────────────────────────────────────────
    • CAN Bus: Pins 6 & 14 (Most 2008+ vehicles)
    • K-Line: Pin 7 (Older European vehicles)
    • J1850 PWM: Pins 2 & 10 (Ford pre-2008)
    • J1850 VPW: Pin 2 (GM pre-2008)
";
            DiagnosticTips.Text = "• Pin 6/14 must have proper voltage\n• Check for 120Ω termination resistance\n• Scan tool shows CAN errors if present\n• Twisted pair wiring critical for reliability";
        }

        private void ShowCANBus()
        {
            DiagramTitle.Text = "CAN Bus Network Topology";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │              CAN BUS NETWORK TOPOLOGY                    │
    └─────────────────────────────────────────────────────────┘

                          120Ω                   120Ω
                         ┌────┐                 ┌────┐
    ┌───────┐    ┌───────┴────┴───────┬─────────┴────┴───────┐
    │  ECM  │────┤     CAN HIGH       │                      │
    │       │    │                    │                      │
    │ Engine│    │   ════════════════════════════════════    │
    │Control│    │                    │                      │
    │       │────┤     CAN LOW        │                      │
    └───────┘    └────────────────────┴──────────────────────┘
                       │         │         │         │
                    ┌──┴──┐   ┌──┴──┐   ┌──┴──┐   ┌──┴──┐
                    │ TCM │   │ ABS │   │ BCM │   │ IC  │
                    │     │   │     │   │     │   │     │
                    │Trans│   │Brake│   │Body │   │Instr│
                    └─────┘   └─────┘   └─────┘   └─────┘

    SPECIFICATIONS:
    ─────────────────────────────────────────
    • High Speed CAN: 500 kbps (Powertrain)
    • Medium Speed CAN: 125-250 kbps (Body)
    • Termination: 120Ω at each end of bus
    • Differential Voltage: 
      - CAN-H: 2.5V to 3.5V (dominant)
      - CAN-L: 1.5V to 2.5V (dominant)
      - Both at 2.5V (recessive)
    • Wire: Twisted pair, shielded

    COMMON CAN IDs (OBD-II):
    ─────────────────────────────────────────
    0x7DF - Broadcast Request (All ECUs)
    0x7E0 - ECM Request
    0x7E8 - ECM Response
    0x7E1 - TCM Request
    0x7E9 - TCM Response
";
            DiagnosticTips.Text = "• Use multimeter to check CAN-H and CAN-L voltages\n• Both lines should be ~2.5V with key on, engine off\n• Check for 60Ω resistance between CAN-H and CAN-L\n• Shorts to ground or power will take down entire bus";
        }

        private void ShowECUPinout()
        {
            DiagramTitle.Text = "Generic ECU Connector Pinout";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │              GENERIC ECU CONNECTOR                       │
    │                 (104-Pin Example)                        │
    └─────────────────────────────────────────────────────────┘

    CONNECTOR A (Power & Ground):
    ─────────────────────────────────────────
    A1:  Battery + (Main)      A2:  Ignition +
    A3:  Ground (Main)         A4:  Ground (Sensor)
    A5:  Ground (Power)        A6:  5V Reference
    A7:  CAN High              A8:  CAN Low
    
    CONNECTOR B (Sensors):
    ─────────────────────────────────────────
    B1:  MAF Signal            B2:  IAT Signal
    B3:  TPS Signal            B4:  MAP Signal
    B5:  O2 Sensor (B1S1)      B6:  O2 Sensor (B1S2)
    B7:  Coolant Temp          B8:  Oil Pressure
    B9:  Knock Sensor 1        B10: Knock Sensor 2
    B11: Crank Position +      B12: Crank Position -
    B13: Cam Position          B14: VSS Signal
    
    CONNECTOR C (Outputs):
    ─────────────────────────────────────────
    C1:  Injector 1            C2:  Injector 2
    C3:  Injector 3            C4:  Injector 4
    C5:  Coil 1                C6:  Coil 2
    C7:  Coil 3                C8:  Coil 4
    C9:  IAC Motor A           C10: IAC Motor B
    C11: Fuel Pump Relay       C12: MIL Lamp
    C13: Tach Output           C14: A/C Clutch
";
            DiagnosticTips.Text = "• Never apply power without load resistors on outputs\n• Use current-limited power supply (fuse protected)\n• Ground must be solid - use thick wire\n• Label all connections before disconnecting\n• Monitor ECU temperature during extended bench testing";
        }

        private void ShowJ1962()
        {
            DiagramTitle.Text = "J1962 DLC Connector Details";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │              J1962 CONNECTOR STANDARD                    │
    │           (Data Link Connector - DLC)                    │
    └─────────────────────────────────────────────────────────┘

    PHYSICAL SPECIFICATIONS:
    ─────────────────────────────────────────
    • Standard: SAE J1962
    • Type: 16-pin female trapezoid
    • Location: Under dashboard, driver side
    • Within 2 feet of steering column

    CONNECTOR DIMENSIONS:
    ─────────────────────────────────────────
    
              ←─── 30mm ───→
         ┌────────────────────┐  ↑
         │  ○ ○ ○ ○ ○ ○ ○ ○   │  │
         │                    │  15mm
         │  ○ ○ ○ ○ ○ ○ ○ ○   │  │
         └──┐              ┌──┘  ↓
            └──────────────┘

    REQUIRED PINS (All Vehicles):
    ─────────────────────────────────────────
    Pin 4:  Chassis Ground
    Pin 5:  Signal Ground  
    Pin 16: Battery Positive (+12V)

    PROTOCOL-SPECIFIC PINS:
    ─────────────────────────────────────────
    ISO 15765 (CAN): Pins 6 (CAN-H), 14 (CAN-L)
    ISO 9141-2:      Pin 7 (K-Line), Pin 15 (L-Line)
    SAE J1850 PWM:   Pin 2 (Bus+), Pin 10 (Bus-)
    SAE J1850 VPW:   Pin 2 (Bus+)
";
            DiagnosticTips.Text = "• Pins 4, 5, and 16 must be present on all OBD-II vehicles\n• Most 2008+ vehicles use CAN (pins 6 & 14)\n• Connector must be within reach of driver's seat\n• Use DLC breakout box for voltage measurements\n• Never probe connector with ignition on - risk of shorts";
        }

        private void ShowCANTroubleshooting()
        {
            DiagramTitle.Text = "CAN Bus Troubleshooting Guide";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │           CAN BUS TROUBLESHOOTING GUIDE                  │
    └─────────────────────────────────────────────────────────┘

    VOLTAGE MEASUREMENTS (Key On, Engine Off):
    ─────────────────────────────────────────
    CAN-H to Ground:  ~2.5V (should be stable)
    CAN-L to Ground:  ~2.5V (should be stable)
    CAN-H to CAN-L:   ~0V (when idle/recessive)
    
    During Communication:
    CAN-H to CAN-L:   ~2.0V differential (dominant)

    RESISTANCE MEASUREMENTS (Key Off):
    ─────────────────────────────────────────
    CAN-H to CAN-L:   60Ω (two 120Ω in parallel)
    CAN-H to Ground:  >10kΩ (no short)
    CAN-L to Ground:  >10kΩ (no short)

    COMMON PROBLEMS:
    ─────────────────────────────────────────
    ┌─────────────────┬────────────────────────────────────┐
    │ Symptom         │ Likely Cause                       │
    ├─────────────────┼────────────────────────────────────┤
    │ No communication│ Open in CAN-H or CAN-L             │
    │ 120Ω measured   │ One termination resistor missing   │
    │ <50Ω measured   │ Extra termination or short         │
    │ Erratic data    │ EMI interference, bad ground       │
    │ Intermittent    │ Loose connection, chafed wire      │
    │ One ECU offline │ Check that specific node           │
    └─────────────────┴────────────────────────────────────┘

    OSCILLOSCOPE PATTERN (Good CAN Signal):
    ─────────────────────────────────────────
    Edge rise time: <40ns (500kbps)
    Check for clean square waves with proper levels
";
            DiagnosticTips.Text = "• Always measure CAN voltages with engine/key off\n• 60Ω between CAN-H and CAN-L indicates proper termination\n• One disconnected module can take down entire bus\n• Look for chafed wires near moving parts\n• Water intrusion causes intermittent CAN faults";
        }

        private void ShowKLine()
        {
            DiagramTitle.Text = "K-Line (ISO 9141) Wiring";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │              K-LINE (ISO 9141-2) WIRING                  │
    └─────────────────────────────────────────────────────────┘

                    +12V (Pin 16)
                        │
                       ┌┴┐
                       │ │ 510Ω Pull-up
                       └┬┘
                        │
    ┌──────┐            │            ┌──────┐
    │ Scan │ ───────────┼─────────── │ ECU  │
    │ Tool │  K-Line    │  Pin 7     │      │
    └──────┘ (Pin 7)    │            └──────┘
                        │
                       GND (Pin 5)

    SPECIFICATIONS:
    ─────────────────────────────────────────
    • Baud Rate: 10.4 kbps
    • Voltage Levels:
      - Logic 0: 0V - 3.6V
      - Logic 1: 6.5V - 14V
    • Pull-up: 510Ω to +12V
    
    L-LINE (Optional):
    ─────────────────────────────────────────
    • Pin 15 on OBD-II
    • Used for initialization only
    • Uni-directional (Tool → ECU)
    • Not present on all vehicles

    INITIALIZATION SEQUENCE:
    ─────────────────────────────────────────
    1. Pull K-Line low for 200ms
    2. Release K-Line (goes high)
    3. Wait 200ms
    4. Send address byte at 5 baud
    5. Wait for ECU response
    6. Begin 10.4 kbps communication
";
            DiagnosticTips.Text = "• K-Line used primarily on pre-2008 European vehicles\n• Requires slow initialization sequence (5 baud)\n• Check for 510Ω pull-up resistor in ECU\n• Pin 7 voltage should idle at ~12V\n• Some Asian vehicles use K-Line on non-standard pins";
        }

        private void ShowLINBus()
        {
            DiagramTitle.Text = "LIN Bus Network";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │                 LIN BUS NETWORK                          │
    │        (Local Interconnect Network)                      │
    └─────────────────────────────────────────────────────────┘

                          +12V
                           │
                          ┌┴┐
                          │ │ 1kΩ Master Pull-up
                          └┬┘
                           │
    ┌────────┐      ┌──────┴──────┐      ┌────────┐
    │ Master │──────│   LIN Bus   │──────│ Slave  │
    │  (BCM) │      │  Single Wire │      │ Window │
    └────────┘      └─────────────┘      └────────┘
                           │
                    ┌──────┼──────┐
                    │      │      │
                 ┌──┴──┐┌──┴──┐┌──┴──┐
                 │Slave││Slave││Slave│
                 │Seat ││Light││Wiper│
                 └─────┘└─────┘└─────┘

    SPECIFICATIONS:
    ─────────────────────────────────────────
    • Baud Rate: 1-20 kbps (typically 19.2 kbps)
    • Topology: Single master, multiple slaves
    • Wire: Single wire + ground
    • Max nodes: 16 per network
    • Max length: 40m

    COMMON APPLICATIONS:
    ─────────────────────────────────────────
    • Window lifters    • Mirror controls
    • Seat controls     • Rain sensors
    • Interior lights   • HVAC actuators
";
            DiagnosticTips.Text = "• LIN is low-speed, cost-effective for non-critical systems\n• Single master controls all slave nodes\n• Much slower than CAN - not suitable for safety systems\n• Diagnose using LIN-capable scan tool\n• Check for 1kΩ pull-up at master node";
        }

        private void ShowFlexRay()
        {
            DiagramTitle.Text = "FlexRay Network";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │                 FLEXRAY NETWORK                          │
    │       (High-Speed Safety-Critical Network)               │
    └─────────────────────────────────────────────────────────┘

    DUAL CHANNEL TOPOLOGY:
    ─────────────────────────────────────────
    
         ┌─────┐    ┌─────┐    ┌─────┐
         │ ECU │    │ ECU │    │ ECU │
         │  1  │    │  2  │    │  3  │
         └──┬──┘    └──┬──┘    └──┬──┘
            │          │          │
    ════════╪══════════╪══════════╪════════  Channel A
            │          │          │
    ════════╪══════════╪══════════╪════════  Channel B
            │          │          │

    SPECIFICATIONS:
    ─────────────────────────────────────────
    • Baud Rate: 10 Mbps per channel
    • Topology: Bus, Star, or Hybrid
    • Redundancy: Dual channel capable
    • Deterministic: Time-triggered
    • Max nodes: 64 per channel

    APPLICATIONS:
    ─────────────────────────────────────────
    • Chassis control      • Suspension
    • Brake-by-wire        • Steer-by-wire
    • Adaptive damping     • Stability control
";
            DiagnosticTips.Text = "• FlexRay used in high-end vehicles (BMW, Mercedes, Audi)\n• Dual-channel provides redundancy for safety systems\n• 100x faster than CAN - critical for real-time control\n• Requires manufacturer-specific diagnostic tools\n• Time-triggered protocol - very deterministic";
        }

        private void ShowDoIP()
        {
            DiagramTitle.Text = "Ethernet (DoIP) Network";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │           ETHERNET / DoIP NETWORK                        │
    │    (Diagnostics over Internet Protocol)                  │
    └─────────────────────────────────────────────────────────┘

    NETWORK TOPOLOGY:
    ─────────────────────────────────────────

    ┌────────────┐   Ethernet   ┌────────────┐
    │   Scan     │══════════════│  Gateway   │
    │   Tool     │  100Mbps+    │    ECU     │
    │            │              │            │
    └────────────┘              └─────┬──────┘
                                      │
                           ┌──────────┼──────────┐
                           │          │          │
                      ┌────┴────┐┌────┴────┐┌────┴────┐
                      │  ECU 1  ││  ECU 2  ││  ECU 3  │
                      │ (CAN)   ││ (CAN)   ││ (ETH)   │
                      └─────────┘└─────────┘└─────────┘

    DoIP SPECIFICATIONS:
    ─────────────────────────────────────────
    • Standard: ISO 13400
    • Speed: 100 Mbps / 1 Gbps
    • Protocol: TCP/IP, UDP
    • Port: 13400 (default)
    • Addressing: IPv4 or IPv6

    VEHICLE MANUFACTURERS USING DoIP:
    ─────────────────────────────────────────
    • BMW (ISTA+)        • Mercedes (Xentry)
    • Volkswagen Group   • Jaguar Land Rover
    • Volvo              • Porsche
";
            DiagnosticTips.Text = "• DoIP replaces traditional OBD-II connectors on some vehicles\n• Requires Ethernet connection (RJ45 or vehicle-specific)\n• Much faster programming/flashing than CAN\n• Gateway ECU routes to other vehicle networks\n• Check for proper IP address configuration";
        }

        private void ShowFordPATS()
        {
            DiagramTitle.Text = "Ford PATS Wiring";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │           FORD PATS (PASSIVE ANTI-THEFT)                 │
    │              SYSTEM WIRING DIAGRAM                       │
    └─────────────────────────────────────────────────────────┘

         ┌──────────────┐
         │  Transponder │
         │     Key      │
         └──────┬───────┘
                │ (Inductive coupling)
         ┌──────┴───────┐
         │ Transceiver  │
         │   Module     │
         └──────┬───────┘
                │
            ┌───┴───┐
            │ PATS  │◄───── CAN or K-Line
            │ Data  │
            └───┬───┘
                │
    ┌───────────┴───────────┐
    │         PCM           │
    │  (Powertrain Control) │
    │                       │
    │ • Validates key code  │
    │ • Enables fuel system │
    │ • Enables starter     │
    └───────────────────────┘

    KEY PROGRAMMING REQUIREMENTS:
    ─────────────────────────────────────────
    • 2 working keys OR
    • PCM parameter reset OR
    • Security access tool (FDRS, Forscan)

    THEFT INDICATOR BEHAVIOR:
    ─────────────────────────────────────────
    • Solid on 3 sec → Key accepted
    • Flashing rapidly → Key not recognized
    • Flashing slow → System disabled
";
            DiagnosticTips.Text = "• PATS programming requires 2 original keys OR dealer tool\n• Theft light behavior indicates system status\n• PCM and transceiver must match/communicate\n• Aftermarket key must be properly chipped\n• Use Forscan for DIY key programming (some models)";
        }

        private void ShowGMClass2()
        {
            DiagramTitle.Text = "GM Class 2 Serial Data";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │           GM CLASS 2 SERIAL DATA BUS                     │
    │              (Single Wire CAN Predecessor)               │
    └─────────────────────────────────────────────────────────┘

                        +12V
                         │
                        ┌┴┐
                        │ │ Pull-up (in each module)
                        └┬┘
                         │
    ═══════════╦═════════╪═════════╦═════════════
               │         │         │
           ┌───┴───┐ ┌───┴───┐ ┌───┴───┐
           │  PCM  │ │  BCM  │ │  IPC  │
           │       │ │       │ │       │
           └───────┘ └───────┘ └───────┘

    SPECIFICATIONS:
    ─────────────────────────────────────────
    • Baud Rate: 10.4 kbps
    • Voltage: 0V (dominant), 7V (recessive)
    • OBD-II Pin: 2 (same as J1850 VPW)
    • Years: 1996-2006 GM vehicles

    COMPARED TO J1850 VPW:
    ─────────────────────────────────────────
    • Similar voltage levels
    • Same physical layer
    • Different message format
    • Class 2 is GM-proprietary
    • J1850 VPW is OBD-II standardized
";
            DiagnosticTips.Text = "• Class 2 found on 1996-2006 GM vehicles\n• Check for 7V on pin 2 with key on, no communication\n• One shorted module takes down entire bus\n• Tech 2 scan tool works best for Class 2\n• Voltage drops to near 0V during active communication";
        }

        private void ShowChryslerBus()
        {
            DiagramTitle.Text = "Chrysler CCD/PCI Bus";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │         CHRYSLER COMMUNICATION BUSES                     │
    │            CCD / PCI / CAN Evolution                     │
    └─────────────────────────────────────────────────────────┘

    CCD BUS (1988-2003):
    ─────────────────────────────────────────
    • Chrysler Collision Detection
    • Two-wire differential bus
    • 7812.5 bps
    • OBD-II Pins: 3 (+) and 11 (-)

    PCI BUS (1998-2007):
    ─────────────────────────────────────────
    • Programmable Communication Interface
    • Single wire with variable pulse width
    • 10.4 kbps
    • OBD-II Pin: 2

    SCI BUS (2003+):
    ─────────────────────────────────────────
    • Chrysler serial, not standard OBD
    • 7812.5 bps or 62.5 kbps
    • OBD-II Pins: 3 (Tx) and 12 (Rx)

    CAN BUS (2008+):
    ─────────────────────────────────────────
    • Standard ISO 15765 CAN
    • 500 kbps High Speed
    • 125 kbps Medium Speed
    • OBD-II Pins: 6 (CAN-H), 14 (CAN-L)
";
            DiagnosticTips.Text = "• Chrysler used multiple protocols over the years\n• CCD is two-wire differential (like early CAN)\n• PCI is single-wire (similar to GM Class 2)\n• SCI is bidirectional serial (non-OBD)\n• 2008+ transitioned to standard CAN bus";
        }

        private void ShowBenchSetup()
        {
            DiagramTitle.Text = "ECU Bench Wiring Setup";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │              ECU BENCH FLASH SETUP                       │
    │          (Out-of-Vehicle Programming)                    │
    └─────────────────────────────────────────────────────────┘

                     ┌─────────────────┐
                     │   POWER SUPPLY  │
                     │   12V - 14.4V   │
                     │     10A min     │
                     └────────┬────────┘
                              │
              ┌───────────────┼───────────────┐
              │               │               │
              ▼               ▼               ▼
         +12V MAIN      +12V IGN        GROUND
              │               │               │
    ┌─────────┴───────────────┴───────────────┴─────────┐
    │                        ECU                         │
    │                                                    │
    │  CAN-H ●───────────┐                              │
    │                    │    ┌──────────────────┐      │
    │  CAN-L ●───────────┼────│  J2534 PASSTHRU  │      │
    │                    │    │      DEVICE       │      │
    │  K-LINE ●──────────┼────│                  │──►PC │
    │                    │    └──────────────────┘      │
    └───────────────────────────────────────────────────┘

    REQUIRED CONNECTIONS:
    ─────────────────────────────────────────
    1. Main Battery + (constant 12V)
    2. Ignition + (switched 12V)
    3. Ground (solid chassis ground)
    4. CAN-H and CAN-L (for CAN ECUs)
    5. K-Line (for older ECUs)

    ⚠️ CRITICAL NOTES:
    ─────────────────────────────────────────
    • Never interrupt power during flash
    • Verify all connections before power on
    • Ensure adequate cooling for ECU
";
            DiagnosticTips.Text = "• Use stable 12-14V power supply (battery maintainer recommended)\n• Double-check all pin connections before applying power\n• Keep ECU cool - use fan during extended operations\n• Never disconnect during flash - can brick ECU\n• Document all connections with photos before starting";
        }

        private void ShowPowerRequirements()
        {
            DiagramTitle.Text = "Power Supply Requirements";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │           POWER SUPPLY REQUIREMENTS                      │
    │         FOR ECU PROGRAMMING & DIAGNOSTICS                │
    └─────────────────────────────────────────────────────────┘

    MINIMUM VOLTAGE REQUIREMENTS:
    ─────────────────────────────────────────
    ┌──────────────────┬─────────────────────┐
    │ Operation        │ Minimum Voltage     │
    ├──────────────────┼─────────────────────┤
    │ Diagnostics      │ 11.5V               │
    │ Module Coding    │ 12.0V               │
    │ ECU Programming  │ 12.5V (stable)      │
    │ Key Programming  │ 12.5V               │
    └──────────────────┴─────────────────────┘

    CURRENT REQUIREMENTS:
    ─────────────────────────────────────────
    • OBD Diagnostics: 1-2A
    • Module Programming: 5-10A
    • ECU Flash: 10-20A (spike)
    • Full system: 30A+ peak

    ⚠️ DO NOT USE:
    ─────────────────────────────────────────
    • Trickle chargers (< 10A)
    • Jump packs alone
    • Vehicle alternator only
    • Switched/unstable supplies

    RECOMMENDED EQUIPMENT:
    ─────────────────────────────────────────
    • Midtronics GR8 or similar
    • NOCO Genius Pro 25A+
    • Bench: Meanwell RSP-500-12
";
            DiagnosticTips.Text = "• Insufficient power causes ECM resets\n• Use battery maintainer during long programming sessions\n• Monitor voltage during flash operations\n• Keep battery above 12.5V for reliable programming";
        }

        private void ShowO2Sensor()
        {
            DiagramTitle.Text = "Oxygen Sensor Wiring (4-Wire Heated)";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │           4-WIRE HEATED OXYGEN SENSOR                    │
    │        (Narrowband - Standard O2 Sensor)                 │
    └─────────────────────────────────────────────────────────┘

                    ┌──────────┐
                    │   O2     │
                    │  Sensor  │
                    │  (B1S1)  │
                    └────┬─────┘
                         │
            ┌────────────┼────────────┐
            │            │            │
         ┌──┴──┐      ┌──┴──┐     ┌──┴──┐
         │  1  │      │  2  │     │  3  │
         │White│      │Black│     │Red/ │
         │     │      │     │     │Black│
         └──┬──┘      └──┬──┘     └──┬──┘
            │            │            │
    ┌───────┴───┐  ┌─────┴─────┐  ┌──┴─────┐
    │  Signal + │  │   Signal  │  │Heater+ │
    │  to ECM   │  │   Ground  │  │  12V   │
    │ 0-1V      │  │           │  │        │
    └───────────┘  └───────────┘  └────────┘

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    1 (WHITE):     Signal + to ECM (0-1V)
    2 (BLACK):     Signal Ground
    3 (RED/BLACK): Heater Power (+12V)
    4 (BLACK/WHT): Heater Ground (ECM)

    VOLTAGE READINGS (Key On, Engine Running):
    ─────────────────────────────────────────
    Lean Mixture:  0.1 - 0.3V
    Stoich (14.7): ~0.45V
    Rich Mixture:  0.7 - 0.9V
    
    Heater Circuit: 12V (PWM modulated)

    DIAGNOSTIC TIPS:
    ─────────────────────────────────────────
    • Sensor should switch 0.1-0.9V at idle
    • Switching should be 1-2 times per second
    • No switching = dead sensor
    • Stuck high/low = contaminated sensor
    • Heater resistance: 4-12Ω cold
";
            DiagnosticTips.Text = "• Check signal voltage at idle (should oscillate)\n• Verify heater circuit with ohmmeter\n• Lean codes = sensor stuck low\n• Rich codes = sensor stuck high\n• Slow response = aged sensor";
        }

        private void ShowMAPSensor()
        {
            DiagramTitle.Text = "MAP Sensor Circuit (3-Wire)";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │        MANIFOLD ABSOLUTE PRESSURE SENSOR                 │
    │             (3-Wire Configuration)                       │
    └─────────────────────────────────────────────────────────┘

                      ┌────────┐
                      │  MAP   │
                      │ Sensor │
                      └───┬────┘
                          │
              ┌───────────┼───────────┐
              │           │           │
           ┌──┴──┐     ┌──┴──┐     ┌──┴──┐
           │  1  │     │  2  │     │  3  │
           │Red/ │     │Grn/ │     │Blk/ │
           │White│     │White│     │Grn  │
           └──┬──┘     └──┬──┘     └──┬──┘
              │           │           │
       ┌──────┴───┐  ┌────┴────┐  ┌──┴────┐
       │ 5V REF   │  │  Signal │  │ Sensor│
       │ from ECM │  │ 0.5-4.5V│  │Ground │
       └──────────┘  └─────────┘  └───────┘

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    1 (RED/WHITE):   5V Reference from ECM
    2 (GRN/WHITE):   MAP Signal Output
    3 (BLACK/GREEN): Sensor Ground

    VOLTAGE READINGS:
    ─────────────────────────────────────────
    Key On, Engine Off:  ~4.5V (atmospheric)
    Idle (18-22 in Hg):  ~1.0-1.5V
    Cruise:              ~1.5-2.5V
    WOT:                 ~4.0-4.5V
    Boost (Turbo):       >4.5V (3-bar sensor)

    TESTING PROCEDURE:
    ─────────────────────────────────────────
    1. Check 5V ref: Should be 4.9-5.1V
    2. Check ground: <0.05V resistance
    3. Apply vacuum: Signal should decrease
    4. Scan tool: Compare to BARO sensor
";
            DiagnosticTips.Text = "• No 5V reference = ECM power issue\n• Signal stuck at 4.5V = vacuum leak\n• Signal stuck low = sensor failure\n• Erratic signal = loose connector\n• Check vacuum hose for cracks";
        }

        private void ShowTPSSensor()
        {
            DiagramTitle.Text = "Throttle Position Sensor (TPS)";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │         THROTTLE POSITION SENSOR (TPS)                   │
    │           Potentiometer Type (3-Wire)                    │
    └─────────────────────────────────────────────────────────┘

                 ┌────────────────┐
                 │  Throttle Body │
                 │   ┌─────┐      │
                 │   │ TPS │◄─────┼─── Throttle Shaft
                 │   └──┬──┘      │
                 └──────┼─────────┘
                        │
             ┌──────────┼──────────┐
             │          │          │
          ┌──┴──┐    ┌──┴──┐    ┌──┴──┐
          │  1  │    │  2  │    │  3  │
          │Gray/│    │Dark │    │Blk/ │
          │ Red │    │Blue │    │Blue │
          └──┬──┘    └──┬──┘    └──┬──┘
             │          │          │
      ┌──────┴───┐ ┌────┴────┐ ┌──┴────┐
      │  5V REF  │ │TPS SIG  │ │Sensor │
      │          │ │0.5-4.5V │ │Ground │
      └──────────┘ └─────────┘ └───────┘

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    1 (GRAY/RED):   5V Reference from ECM
    2 (DARK BLUE):  TPS Signal Output
    3 (BLK/BLUE):   Sensor Ground

    VOLTAGE READINGS:
    ─────────────────────────────────────────
    Closed Throttle (Idle):  0.45-0.90V
    Part Throttle:           1.0-3.5V
    Wide Open Throttle:      4.0-4.8V

    TESTING PROCEDURE:
    ─────────────────────────────────────────
    1. Key On, check idle voltage
    2. Slowly open throttle
    3. Voltage should increase smoothly
    4. No dead spots or jumps
    5. Check for proper return to idle

    COMMON PROBLEMS:
    ─────────────────────────────────────────
    • Worn potentiometer track (dead spots)
    • Incorrect adjustment
    • Corroded connector
    • Binding throttle shaft
";
            DiagnosticTips.Text = "• Voltage jumps = worn track\n• Won't reach 4.5V at WOT = misadjusted\n• Stuck at one voltage = open circuit\n• Fluctuating at idle = bad ground\n• Some vehicles use 2 TPS for redundancy";
        }

        private void ShowCrankCamSensors()
        {
            DiagramTitle.Text = "Crankshaft & Camshaft Position Sensors";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │     CRANKSHAFT & CAMSHAFT POSITION SENSORS               │
    │    (Hall Effect & Variable Reluctance Types)             │
    └─────────────────────────────────────────────────────────┘

    HALL EFFECT SENSORS (3-Wire):
    ═════════════════════════════════════════

          ┌──────────────┐
          │  Hall Effect │
          │CKP/CMP Sensor│
          └──────┬───────┘
                 │
        ┌────────┼────────┐
        │        │        │
     ┌──┴──┐  ┌──┴──┐  ┌──┴──┐
     │  1  │  │  2  │  │  3  │
     │Pink │  │Light│  │Black│
     │5V   │  │Blue │  │GND  │
     └──┬──┘  └──┬──┘  └──┬──┘
        │        │        │
    ┌───┴──┐  ┌──┴───┐  ┌┴────┐
    │ 5V   │  │Signal│  │ GND │
    │ REF  │  │0-5V  │  │     │
    └──────┘  └──────┘  └─────┘

    VARIABLE RELUCTANCE (2-Wire):
    ═════════════════════════════════════════

          ┌──────────────┐
          │ VR Magnetic  │
          │CKP/CMP Sensor│
          └──────┬───────┘
                 │
            ┌────┴────┐
            │    │    │
         ┌──┴──┐ ┌──┴──┐
         │  +  │ │  -  │
         │Org/ │ │Tan/ │
         │Blk  │ │Yel  │
         └──┬──┘ └──┬──┘
            │      │
      ┌─────┴──┐ ┌─┴─────┐
      │Signal +│ │Signal-│
      │AC Sine │ │AC Sine│
      └────────┘ └───────┘

    CRANKSHAFT POSITION:
    ─────────────────────────────────────────
    • 24-60 teeth on reluctor wheel
    • Missing tooth for TDC reference
    • RPM calculated from frequency
    • Critical for ignition timing

    CAMSHAFT POSITION:
    ─────────────────────────────────────────
    • Determines cylinder ID (cyl #1)
    • Usually one pulse per revolution
    • Used for sequential fuel injection
    • VVT systems may have multiple cams

    TESTING:
    ─────────────────────────────────────────
    Hall Effect:
    • Pin 1: 5.0V
    • Pin 2: 0-5V square wave (cranking)
    • Pin 3: 0V (ground)

    Variable Reluctance:
    • AC voltage while cranking
    • Amplitude increases with RPM
    • Check resistance: 200-3000Ω
";
            DiagnosticTips.Text = "• No crank signal = no start\n• VR sensors: check air gap (0.020-0.050\")\n• Hall sensors: check 5V supply\n• Erratic signal = damaged reluctor ring\n• Cam sensor = cylinder misfire codes";
        }

        private void ShowInjectorCircuits()
        {
            DiagramTitle.Text = "Fuel Injector Circuit Wiring";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │             FUEL INJECTOR CIRCUIT                        │
    │         (Port & Direct Injection Types)                  │
    └─────────────────────────────────────────────────────────┘

    PORT FUEL INJECTION:
    ═════════════════════════════════════════

                     +12V (Fuel Pump Relay)
                         │
                     ┌───┴────┐
                     │Injector│
                     │  Coil  │
                     │ 12-16Ω │
                     └───┬────┘
                         │
                      ┌──┴──┐
                      │  2  │
                      │Dark │
                      │Green│
                      └──┬──┘
                         │
                    ┌────┴────┐
                    │   ECM   │
                    │  Driver │
                    │  (GND)  │
                    └─────────┘

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    1 (RED/YELLOW):  +12V from Fuel Pump Relay
    2 (VARIES):      ECM Ground (PWM)

    DIRECT INJECTION (High Pressure):
    ═════════════════════════════════════════
    • Higher voltage operation (48-90V)
    • ECM boost circuit required
    • Resistance: 0.5-2.0Ω
    • Current limited by ECM

    TESTING PROCEDURE:
    ─────────────────────────────────────────
    1. Resistance Test:
       • Port: 12-16Ω
       • Direct: 0.5-2.0Ω
    
    2. Power Test:
       • Pin 1: 12V key on
    
    3. Pulse Test:
       • Noid light or scope
       • 2-4ms at idle (port)
       • Varies with load

    INJECTOR PULSE WIDTH:
    ─────────────────────────────────────────
    Idle:      2-4ms
    Cruise:    4-8ms
    WOT:       12-20ms
    Cold Start: 8-15ms

    COMMON ISSUES:
    ─────────────────────────────────────────
    • Open coil = no pulse
    • Shorted coil = blown ECM driver
    • Clogged injector = lean cylinder
    • Leaking injector = rich cylinder
";
            DiagnosticTips.Text = "• Use noid light to check pulse\n• Scope shows voltage spike when injector closes\n• Compare pulse width between cylinders\n• Dead injector = misfire + lean code\n• Check wiring harness for chafing";
        }

        private void ShowIgnitionCoil()
        {
            DiagramTitle.Text = "Ignition Coil Wiring (COP & CNP)";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │              IGNITION COIL CIRCUITS                      │
    │    Coil-On-Plug (COP) & Coil-Near-Plug (CNP)            │
    └─────────────────────────────────────────────────────────┘

    COIL-ON-PLUG (3-Wire):
    ═════════════════════════════════════════

              Spark Plug
                  ↓
            ┌──────────┐
            │ Ignition │
            │   Coil   │
            └─────┬────┘
                  │
         ┌────────┼────────┐
         │        │        │
      ┌──┴──┐  ┌──┴──┐  ┌──┴──┐
      │  1  │  │  2  │  │  3  │
      │Pink │  │Blk/ │  │Grn/ │
      │     │  │Yel  │  │Red  │
      └──┬──┘  └──┬──┘  └──┬──┘
         │        │        │
    ┌────┴───┐ ┌──┴───┐ ┌──┴──┐
    │  12V   │ │ IGT  │ │ IGF │
    │Switched│ │Signal│ │Fbk  │
    └────────┘ └──────┘ └─────┘

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    1 (PINK):       12V Switched (Ign On)
    2 (BLK/YEL):    IGT Trigger from ECM
    3 (GRN/RED):    IGF Feedback to ECM

    COIL OPERATION:
    ─────────────────────────────────────────
    • ECM pulls IGT low to charge coil
    • Dwell time: 2-4ms
    • ECM releases IGT → spark fires
    • IGF confirms to ECM

    DWELL CONTROL:
    ─────────────────────────────────────────
    Idle:     2.5-3.5ms
    Cruise:   3.0-4.0ms
    WOT:      4.0-5.0ms

    PRIMARY RESISTANCE:
    ─────────────────────────────────────────
    COP Coils: 0.5-2.0Ω
    CNP Coils: 0.8-3.0Ω

    SECONDARY RESISTANCE:
    ─────────────────────────────────────────
    COP Coils: 5-15kΩ
    CNP Coils: 8-25kΩ

    TESTING:
    ─────────────────────────────────────────
    1. Check 12V on Pin 1 (key on)
    2. Scope Pin 2 for IGT pulses
    3. Check primary resistance
    4. Check secondary resistance
    5. Spark test with coil-on-plug tester

    COMMON FAILURES:
    ─────────────────────────────────────────
    • Open primary = no spark
    • Open secondary = weak/no spark
    • Internal arc = intermittent misfire
    • Cracked tower = carbon track
";
            DiagnosticTips.Text = "• Misfire codes point to bad coil\n• Swap coils to confirm failure\n• Check spark plug gap (too wide = coil stress)\n• Carbon tracking visible under UV light\n• Modern coils rarely fail - check plugs first";
        }

        private void ShowVVTSolenoid()
        {
            DiagramTitle.Text = "Variable Valve Timing Solenoid";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │         VARIABLE VALVE TIMING (VVT) SOLENOID             │
    │        (Oil Control Valve - OCV)                         │
    └─────────────────────────────────────────────────────────┘

                  Engine Oil Supply
                         ↓
                  ┌──────────┐
                  │   VVT    │
                  │ Solenoid │
                  │   (OCV)  │
                  └─────┬────┘
                        │
                   ┌────┴────┐
                   │    │    │
                ┌──┴──┐ ┌──┴──┐
                │  1  │ │  2  │
                │Org/ │ │Purp │
                │White│ │ le  │
                └──┬──┘ └──┬──┘
                   │      │
              ┌────┴──┐ ┌─┴─────┐
              │  12V  │ │  PWM  │
              │Supply │ │Control│
              └───────┘ └───────┘

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    1 (ORANGE/WHITE): 12V from Main Relay
    2 (PURPLE):       PWM Control from ECM

    OPERATION:
    ─────────────────────────────────────────
    • ECM varies duty cycle (0-100%)
    • Controls engine oil flow to cam phaser
    • Advances/retards cam timing
    • Improves power, economy, emissions

    DUTY CYCLE:
    ─────────────────────────────────────────
    Idle:       ~30% (retarded)
    Cruise:     ~50% (mid-range)
    WOT:        ~80% (advanced)

    RESISTANCE:
    ─────────────────────────────────────────
    6-12Ω (typical)

    OIL FILTER SCREEN:
    ─────────────────────────────────────────
    • Small screen inside solenoid
    • Can clog with debris
    • Causes VVT performance codes
    • Clean during oil changes

    TESTING:
    ─────────────────────────────────────────
    1. Check 12V on Pin 1
    2. Scope Pin 2 for PWM signal
    3. Check resistance (6-12Ω)
    4. Remove and inspect filter screen
    5. Listen for solenoid clicking (commanded on/off)

    COMMON ISSUES:
    ─────────────────────────────────────────
    • Clogged filter screen
    • Sludge in oil passages
    • Stuck solenoid
    • Cam phaser failure
    • Low oil pressure
";
            DiagnosticTips.Text = "• P0010-P0014 = VVT system codes\n• Use fresh full-synthetic oil only\n• Check oil level - critical for VVT\n• Rattling at startup = worn phaser\n• Scan tool can command % for testing";
        }

        private void ShowTurboControl()
        {
            DiagramTitle.Text = "Turbo/Supercharger Control Circuits";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │      TURBOCHARGER/SUPERCHARGER CONTROL SYSTEM            │
    │         (Wastegate & Bypass Valve Control)               │
    └─────────────────────────────────────────────────────────┘

    ELECTRONIC WASTEGATE ACTUATOR:
    ═════════════════════════════════════════

              Turbo Wastegate Flap
                      ↓
            ┌──────────────────┐
            │  Electronic WG   │
            │    Actuator      │
            │   (Stepper/PWM)  │
            └────────┬─────────┘
                     │
         ┌───────────┼───────────┐
         │           │           │
      ┌──┴──┐     ┌──┴──┐     ┌──┴──┐
      │  1  │     │  2  │     │  3  │
      │Red  │     │Brown│     │Black│
      └──┬──┘     └──┬──┘     └──┬──┘
         │           │           │
    ┌────┴────┐ ┌────┴────┐ ┌────┴────┐
    │  12V    │ │  PWM    │ │  GND    │
    │         │ │ Control │ │         │
    └─────────┘ └─────────┘ └─────────┘

    BOOST PRESSURE SENSOR:
    ─────────────────────────────────────────
    3-Wire MAP-style sensor
    • Measures intake manifold pressure
    • 0-3 bar typical (stock turbo)
    • 0-4 bar (performance applications)

    BYPASS/BLOW-OFF VALVE:
    ─────────────────────────────────────────
    Vacuum-Operated:
    • Vacuum line to intake manifold
    • Opens on deceleration
    • Prevents compressor surge

    Electronic:
    • PWM solenoid control
    • 12V supply + ECM ground
    • More precise control

    CONTROL STRATEGY:
    ─────────────────────────────────────────
    • ECM targets boost based on:
      - Engine RPM
      - Throttle position
      - Air temp
      - Coolant temp
      - Knock detection
    
    • Wastegate opens to limit boost
    • Closed loop control via boost sensor

    COMMON ISSUES:
    ─────────────────────────────────────────
    • Stuck wastegate (overboost/underboost)
    • Boost leaks (intercooler, piping)
    • Failed actuator
    • BOV leaking (idle issues)
";
            DiagnosticTips.Text = "• P0234 = Engine Overboost\n• P0299 = Turbo Underboost\n• Check for boost leaks with smoke test\n• Wastegate should move freely\n• Monitor actual vs target boost on scan tool";
        }

        private void ShowEGRValve()
        {
            DiagramTitle.Text = "EGR Valve Wiring & Control";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │       EXHAUST GAS RECIRCULATION (EGR) SYSTEM             │
    │         Electronic EGR Valve with Position Sensor        │
    └─────────────────────────────────────────────────────────┘

    EGR VALVE (5-Wire):
    ═════════════════════════════════════════

                 ┌────────────┐
                 │  EGR Valve │
                 │  + Sensor  │
                 └──────┬─────┘
                        │
      ┌─────────────────┼─────────────────┐
      │         │       │       │         │
   ┌──┴──┐   ┌──┴──┐ ┌──┴──┐ ┌──┴──┐   ┌──┴──┐
   │  1  │   │  2  │ │  3  │ │  4  │   │  5  │
   │ 12V │   │ PWM │ │ 5V  │ │ POS │   │ GND │
   └──┬──┘   └──┬──┘ └──┬──┘ └──┬──┘   └──┬──┘
      │         │       │       │         │
   Power    Control   Ref    Signal    Ground

    PIN ASSIGNMENTS:
    ─────────────────────────────────────────
    1: 12V Supply (from main relay)
    2: PWM Control (ECM)
    3: 5V Reference (for position sensor)
    4: Position Signal (0.5-4.5V)
    5: Ground

    POSITION SENSOR VOLTAGES:
    ─────────────────────────────────────────
    Closed: 0.5-0.9V
    25% Open: 1.5-2.0V
    50% Open: 2.5-3.0V
    100% Open: 4.0-4.5V

    EGR COOLER BYPASS (Some Systems):
    ─────────────────────────────────────────
    • Additional 2-wire solenoid
    • Routes exhaust around cooler when cold
    • Prevents condensation/corrosion

    OPERATION:
    ─────────────────────────────────────────
    • Closed at idle
    • Opens during cruise (10-30%)
    • Closed at WOT
    • Reduces NOx emissions
    • Improves fuel economy

    TESTING:
    ─────────────────────────────────────────
    1. Check 12V supply
    2. Scope PWM control signal
    3. Check 5V reference
    4. Monitor position voltage
    5. Command EGR open with scan tool

    COMMON FAILURES:
    ─────────────────────────────────────────
    • Carbon buildup (valve stuck)
    • Failed position sensor
    • Leaking EGR cooler
    • Clogged EGR passages
    • Stuck open (rough idle)
    • Stuck closed (NOx codes)
";
            DiagnosticTips.Text = "• P0400-P0409 = EGR system codes\n• Stuck open causes rough idle\n• Stuck closed causes pinging under load\n• Clean EGR passages during service\n• Position sensor can fail separately from valve";
        }

        private void ShowDPFSystem()
        {
            DiagramTitle.Text = "Diesel Particulate Filter System";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │         DIESEL PARTICULATE FILTER (DPF) SYSTEM           │
    │        Sensors, Regen Control, & Monitoring              │
    └─────────────────────────────────────────────────────────┘

    DPF PRESSURE SENSOR (3-Wire):
    ═════════════════════════════════════════

              DPF Filter
                  │
           ┌──────┴──────┐
           │  Pressure   │
           │   Sensor    │
           └──────┬──────┘
                  │
         ┌────────┼────────┐
         │        │        │
      ┌──┴──┐  ┌──┴──┐  ┌──┴──┐
      │ 5V  │  │ SIG │  │ GND │
      └──┬──┘  └──┬──┘  └──┬──┘
         │        │        │
      Ref(5V) Pressure  Ground
              (0-5V)

    DPF TEMP SENSORS (2-Wire Each):
    ─────────────────────────────────────────
    Inlet Temp:  Before DPF
    Outlet Temp: After DPF

    • NTC thermistor type
    • Resistance decreases with temp
    • Typical: 2.5kΩ @ 68°F
    • High temp: 100-200Ω @ 1200°F

    FUEL DOSING INJECTOR (2-Wire):
    ─────────────────────────────────────────
    • Injects fuel into exhaust for regen
    • 12V supply + ECM ground
    • PWM controlled
    • Creates heat to burn soot

    REGEN PROCESS:
    ─────────────────────────────────────────
    Passive Regen:
    • Occurs during highway driving
    • Exhaust temp >600°F naturally
    • No driver intervention

    Active Regen:
    • ECM commanded
    • Post-injection + dosing injector
    • Raises DPF temp to 1100-1200°F
    • Burns accumulated soot
    • Takes 20-40 minutes

    PRESSURE READINGS:
    ─────────────────────────────────────────
    Clean Filter:     0.5-1.5 inHg
    50% Full:         2.0-4.0 inHg
    Regen Needed:     5.0-8.0 inHg
    Plugged:          >10 inHg

    SENSORS & WIRING:
    ─────────────────────────────────────────
    • DPF Pressure Sensor: 3-wire
    • Inlet Temp: 2-wire NTC
    • Outlet Temp: 2-wire NTC
    • Fuel Doser: 2-wire solenoid

    COMMON ISSUES:
    ─────────────────────────────────────────
    • Failed pressure sensor
    • Plugged DPF (short trips)
    • Fuel doser clogged
    • Temp sensor failure
    • Interrupted regen cycles
";
            DiagnosticTips.Text = "• P2002/P2463 = DPF efficiency codes\n• P242F = DPF restriction\n• Monitor soot load % on scan tool\n• Force regen if >80% full\n• Check for oil contamination in DPF\n• Short trips prevent passive regen";
        }

        private void ShowDEFSCRSystem()
        {
            DiagramTitle.Text = "DEF/SCR System (Diesel Exhaust Fluid)";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │      DIESEL EXHAUST FLUID / SCR SYSTEM                   │
    │   (Selective Catalytic Reduction for NOx Control)        │
    └─────────────────────────────────────────────────────────┘

    DEF INJECTOR (4-Wire):
    ═════════════════════════════════════════

           DEF Tank → Pump → Injector → SCR Catalyst
                                 │
                      ┌──────────┴──────────┐
                      │    DEF Injector     │
                      └─────────┬───────────┘
                                │
                ┌───────────────┼───────────────┐
                │       │       │       │       │
             ┌──┴──┐ ┌──┴──┐ ┌──┴──┐ ┌──┴──┐
             │ 12V │ │ GND │ │Heater│ │Heater│
             │     │ │     │ │  +   │ │  -  │
             └──┬──┘ └──┬──┘ └──┬──┘ └──┬──┘
                │       │       │       │
              Power   Control  Heat    Heat

    DEF QUALITY SENSOR (5-Wire):
    ─────────────────────────────────────────
    • Measures DEF concentration
    • Temperature sensor included
    • Heater for cold weather
    • 32.5% urea concentration required

    DEF LEVEL SENSOR:
    ─────────────────────────────────────────
    • Ultrasonic or float type
    • Warns at <25% level
    • Derate at <5% level

    NOx SENSORS (Before/After SCR):
    ─────────────────────────────────────────
    Upstream:  Measures NOx from engine
    Downstream: Measures NOx after SCR
    • 4-5 wire sensors
    • Heated for accuracy
    • Calculate SCR efficiency

    SCR TEMPERATURE SENSORS:
    ─────────────────────────────────────────
    Inlet Temp:  Before SCR catalyst
    Outlet Temp: After SCR catalyst
    • NTC thermistor type
    • Monitor catalyst temp
    • Ensure proper DEF dosing

    DEF DOSING STRATEGY:
    ─────────────────────────────────────────
    • ECM monitors NOx levels
    • Injects DEF into exhaust
    • DEF decomposes to ammonia (NH3)
    • NH3 reacts with NOx → N2 + H2O
    • Reduces NOx by 90%+

    DEF CONSUMPTION:
    ─────────────────────────────────────────
    • Typically 2-3% of fuel consumption
    • 100 miles = ~1 gallon DEF
    • Quality critical (ISO 22241)

    DERATE STRATEGY:
    ─────────────────────────────────────────
    Low DEF Level:
    • Warning at 1000 miles to empty
    • 5 mph derate when empty

    Poor DEF Quality:
    • Immediate warning
    • Derate after 100-400 miles

    COMMON ISSUES:
    ─────────────────────────────────────────
    • Contaminated DEF (diesel, water)
    • Frozen DEF system
    • Clogged DEF injector
    • Failed NOx sensors
    • DEF pump failure
    • Crystallized DEF in lines
";
            DiagnosticTips.Text = "• P20EE = SCR NOx catalyst efficiency\n• P204F = DEF quality poor\n• P207F = DEF level sensor circuit\n• Never use contaminated DEF\n• System has multiple heaters for cold weather\n• Clean DEF injector during service";
        }

        private void ShowEVAPSystem()
        {
            DiagramTitle.Text = "EVAP System Circuits & Testing";
            DiagramContent.Text = @"
    ┌─────────────────────────────────────────────────────────┐
    │       EVAPORATIVE EMISSION CONTROL SYSTEM                │
    │         (Canister Purge & Vent Solenoids)                │
    └─────────────────────────────────────────────────────────┘

    PURGE SOLENOID (2-Wire):
    ═════════════════════════════════════════

         Fuel Tank → Canister → Purge Valve → Intake
                                     │
                          ┌──────────┴──────────┐
                          │   Purge Solenoid    │
                          └─────────┬───────────┘
                                    │
                              ┌─────┴─────┐
                              │     │     │
                           ┌──┴──┐ ┌──┴──┐
                           │ 12V │ │ PWM │
                           │     │ │ GND │
                           └──┬──┘ └──┬──┘
                              │       │
                            Power  Control

    VENT SOLENOID (2-Wire):
    ═════════════════════════════════════════
    • Seals canister during leak tests
    • Normally open
    • Closes for EVAP monitor

    FUEL TANK PRESSURE SENSOR (3-Wire):
    ─────────────────────────────────────────
    • 5V reference
    • Signal output
    • Ground
    • Measures tank vacuum/pressure

    OPERATION:
    ─────────────────────────────────────────
    Engine Off:
    • Fuel vapors → Canister
    • Purge valve closed
    • Vent valve open

    Engine Running (Cold):
    • Purge valve closed
    • Vent valve open
    • No purge until warm

    Engine Running (Warm):
    • Purge valve PWM (0-100%)
    • Vent valve open
    • Vapors drawn into engine

    EVAP Monitor (Leak Test):
    • Purge valve closed
    • Vent valve closed
    • ECM monitors pressure
    • Detects leaks >0.020""

    PURGE DUTY CYCLE:
    ─────────────────────────────────────────
    Idle:      0-10%
    Cruise:    20-80%
    WOT:       0% (closed)

    TESTING:
    ─────────────────────────────────────────
    1. Check 12V to purge solenoid
    2. Scope PWM control signal
    3. Check for vacuum at idle
    4. Smoke test for leaks
    5. Monitor FTP sensor voltage

    COMMON ISSUES:
    ─────────────────────────────────────────
    • Loose gas cap (most common)
    • Cracked EVAP lines
    • Failed purge valve
    • Canister saturated with fuel
    • Failed FTP sensor
    • Vent valve stuck
";
            DiagnosticTips.Text = "• P0440-P0457 = EVAP system codes\n• P0442 = Small leak detected\n• P0455 = Large leak detected\n• Always check gas cap first\n• Smoke test finds leaks quickly\n• Canister can become fuel-saturated (replace)";
        }
    }
}
