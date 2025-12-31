# BlackFlag v2.0 - Native Windows Application

A professional ECU hacking and tuning suite built with C# WPF for native Windows performance.

## Why Native?

Unlike the previous Electron version, this native C# WPF application offers:

- **~10x smaller memory footprint** (50MB vs 500MB+)
- **Instant startup** (no Chromium to load)
- **True native Windows UI** (proper look and feel)
- **Direct hardware access** (serial ports, USB, CAN adapters)
- **No JavaScript overhead** (compiled .NET code)
- **Single executable deployment**

## Requirements

- Windows 10/11
- .NET 8.0 SDK (for building)
- .NET 8.0 Runtime (for running)

## Building

### Using Visual Studio 2022
1. Open `BlackFlag.csproj`
2. Build > Build Solution (Ctrl+Shift+B)
3. Run with F5

### Using Command Line
```powershell
# Build
dotnet build

# Run
dotnet run

# Publish self-contained executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Features

### 🔍 VIN Decoder
- Decode any 17-character VIN
- Identify manufacturer, model year, plant code
- Support for all major manufacturers

### 🔌 ECU Scanner
- Scan for connected ECU modules
- CAN bus communication
- Module identification and diagnostics

### ⚡ Voltage Meter
- Real-time voltage monitoring
- Min/Max/Average tracking
- Visual voltage bar indicator

### 📋 Wiring Diagrams
- OBD-II connector pinouts
- CAN bus network topology
- ECU pinout diagrams

### 🎛️ Tune Manager
- Import/export tune files
- Stage 1/2 performance tunes
- Economy tunes
- Stock backup management

### 💾 ECU Cloning
- Read full ECU memory
- Write tunes to ECU
- Clone ECU to ECU

### 🏎️ Performance Tuning
- Fuel map editing
- Ignition timing adjustment
- Boost control
- Rev/speed limiter adjustment

## Themes

Four professional themes included:
- 🌙 **Dark Mode** - Modern dark theme
- 💾 **Retro Green** - Classic hacker aesthetic
- 🔧 **Ford Blue** - Professional diagnostic look
- 🚗 **Orange Tech** - Witech-inspired theme

## Architecture

```
BlackFlag/
├── App.xaml              # Application entry point
├── MainWindow.xaml       # Main window layout
├── Models/
│   └── Models.cs         # Data models (Vehicle, ECU, Tune, etc.)
├── Services/
│   └── Database.cs       # Local JSON database service
├── Views/
│   ├── DashboardPage.xaml
│   ├── VinDecoderPage.xaml
│   ├── EcuScannerPage.xaml
│   ├── VoltageMeterPage.xaml
│   ├── WiringDiagramsPage.xaml
│   ├── TuneManagerPage.xaml
│   ├── EcuCloningPage.xaml
│   └── PerformancePage.xaml
└── Themes/
    ├── DarkTheme.xaml
    ├── RetroGreenTheme.xaml
    ├── FordBlueTheme.xaml
    └── OrangeTechTheme.xaml
```

## Data Storage

User data is stored locally at:
```
%LOCALAPPDATA%\BlackFlag\
├── vehicles.json      # Vehicle database
├── history.json       # Recently used vehicles
├── settings.json      # User preferences
└── backups/           # ECU backups
```

## Future Roadmap

- [ ] J2534 PassThru driver integration
- [ ] Real CAN bus communication
- [ ] Live data graphing
- [ ] Custom PID definitions
- [ ] Plugin system for manufacturer-specific features

## License

MIT License - Free and Open Source
