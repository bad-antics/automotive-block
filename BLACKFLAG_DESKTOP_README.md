# BlackFlag Desktop - Professional Automotive Diagnostic & Tuning Suite

<div align="center">

![BlackFlag Logo](https://via.placeholder.com/200x200/1e1e1e/4CAF50?text=BlackFlag)

**Advanced ECU Diagnostics | Performance Tuning | Real-Time Data Analysis**

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-blue)](https://github.com/dotnet/wpf)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)](https://www.microsoft.com/windows)

</div>

## 🚀 Overview

BlackFlag Desktop is a comprehensive, native Windows application for automotive diagnostics, ECU tuning, and performance optimization. Built with C# and WPF on .NET 8.0, it provides professional-grade tools for mechanics, tuners, and automotive enthusiasts.

## ✨ Key Features

### 🔍 **Diagnostic Tools**
- **VIN Decoder** - Decode 17-character VINs with manufacturer/year validation
- **ECU Scanner** - Real-time ECU monitoring and diagnostics
- **Voltage Meter** - Electrical system analysis and battery health
- **Live Data** - OBD2 real-time data streaming with 4 charts & 4 gauges
- **Diagnostics Dashboard** - Comprehensive vehicle health overview

### ⚡ **Performance Tuning**
- **Tune Manager** - Browse, download, and manage performance tunes
  - VIN-validated racing tunes (drag, circuit, street, off-road)
  - Emissions delete tunes (EGR, DPF, DEF, catalytic converter)
  - ECU/Year/VIN compatibility validation
  - Stage 1/2/3 performance calibrations
  - Economy and daily driver tunes
- **ECU Unlock** - Processor security bypass for 7 vendors, 30+ ECU models
- **J2534 Integration** - Professional flash/reprogram interface
- **OE Flash Files** - Download stock calibrations and as-built data

### 🛠️ **Technical Features**
- **Wiring Diagrams** - Interactive diagrams for 29 sensor/connector types
- **ECU Cloning** - Backup and clone ECU calibrations
- **Emissions Control** - EGR/DPF/DEF system management
- **Vehicle Database** - 159 vehicles across 39 manufacturers

## 📋 System Requirements

### Minimum Requirements
- **OS:** Windows 10/11 (64-bit)
- **RAM:** 4 GB
- **Storage:** 200 MB free space
- **Framework:** .NET 8.0 Runtime (included in standalone build)

### Recommended
- **OS:** Windows 11 (64-bit)
- **RAM:** 8 GB or more
- **Storage:** 500 MB free space
- **Display:** 1920x1080 or higher
- **Hardware:** OBD2 ELM327 adapter (for live data)

## 🔧 Installation

### Option 1: Standalone Executable (Recommended)
1. Download `BlackFlag.exe` from [Releases](../../releases)
2. Run `BlackFlag.exe` - No installation required!
3. All dependencies are bundled (~155 MB)

### Option 2: Build from Source
```powershell
# Clone repository
git clone https://github.com/yourusername/blackflag.git
cd blackflag/native

# Restore dependencies
dotnet restore

# Build Release
dotnet build -c Release

# Run application
.\bin\Release\net8.0-windows\BlackFlag.exe
```

### Option 3: Publish Single-File Executable
```powershell
cd native
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Executable created at:
# .\bin\Release\net8.0-windows\win-x64\publish\BlackFlag.exe
```

## 🎯 Quick Start Guide

### First Launch
1. **Launch BlackFlag.exe**
2. **Select Theme** - Choose from 4 professional themes:
   - Dark Theme (default)
   - Retro Green
   - Ford Blue
   - Orange Tech
3. **Select Manufacturer** - Choose from 39 manufacturers
4. **Select Vehicle** - Pick from 159 vehicles (sorted by year)

### Using Tune Manager
1. Navigate to **Tune Manager** from sidebar
2. Select **Manufacturer** and **Vehicle**
3. Enter **VIN Number** (required for racing/delete tunes)
4. Click **🔍 Decode VIN** to validate
5. Browse available tunes and click **📥 Download**
6. Tunes saved to: `Documents\BlackFlag\Tunes\{Vehicle}\`

### Using OE Flash Files
1. Navigate to **OE Flash Files** from sidebar
2. Select your vehicle
3. Download stock flash files (PCM, TCM, etc.)
4. Download as-built data for all modules
5. Files saved to: `Documents\BlackFlag\OE_Files\{Vehicle}\`

### Live Data Monitoring
1. Navigate to **Live Data** from sidebar
2. Connect OBD2 adapter (ELM327 compatible)
3. Select COM port
4. Click **Connect**
5. View real-time gauges and charts

## 📊 Features Overview

| Feature | Description | Status |
|---------|-------------|--------|
| VIN Decoder | 17-char VIN validation with year/manufacturer | ✅ Complete |
| ECU Scanner | Real-time ECU monitoring | ✅ Complete |
| Voltage Meter | Electrical system analysis | ✅ Complete |
| Live Data | OBD2 streaming with charts | ✅ Complete |
| Wiring Diagrams | 29 interactive diagrams | ✅ Complete |
| Tune Manager | VIN-validated tune downloads | ✅ Complete |
| ECU Unlock | 7 vendors, 30+ models | ✅ Complete |
| ECU Cloning | Backup/restore calibrations | ✅ Complete |
| J2534 Integration | Professional flash interface | ✅ Complete |
| OE Flash Files | Stock calibration downloads | ✅ Complete |
| Emissions Control | EGR/DPF/DEF management | ✅ Complete |

## 🎨 Themes

BlackFlag includes 4 professionally designed themes:

- **🌑 Dark Theme** - Modern dark UI (default)
- **💚 Retro Green** - Classic terminal green
- **💙 Ford Blue** - Ford-inspired blue accent
- **🧡 Orange Tech** - High-contrast orange

## 🗂️ Supported Vehicles

### 39 Manufacturers
Acura • Alfa Romeo • Aston Martin • Audi • Bentley • BMW • Cadillac • Chevrolet • Dodge • Ferrari • Ford • Genesis • GMC • Honda • Hyundai • Infiniti • Jaguar • Jeep • Kia • Lamborghini • Land Rover • Lexus • Lincoln • Lucid • Maserati • Mazda • McLaren • Mercedes-Benz • Mitsubishi • Nissan • Porsche • RAM • Rivian • Rolls-Royce • Subaru • Tesla • Toyota • Volkswagen • Volvo

### 159 Total Vehicles
- **89 Diesel Vehicles** - Comprehensive diesel truck/SUV coverage
- **Light Duty Trucks** - Ford F-150, Silverado, RAM, Colorado, Titan XD
- **Performance Cars** - Mustang, Camaro, Challenger, Corvette, GT-R
- **Luxury Vehicles** - BMW, Mercedes, Audi, Porsche, Land Rover
- **Electric Vehicles** - Tesla Model S/3/X/Y, Rivian R1T/R1S, Lucid Air

## 📁 File Locations

| Type | Path |
|------|------|
| Tunes | `%USERPROFILE%\Documents\BlackFlag\Tunes\` |
| OE Flash Files | `%USERPROFILE%\Documents\BlackFlag\OE_Files\` |
| Database | `%LOCALAPPDATA%\BlackFlag\vehicles.json` |
| Settings | `%LOCALAPPDATA%\BlackFlag\settings.json` |

## 🔒 VIN Validation System

BlackFlag uses a comprehensive VIN validation system for racing and emissions delete tunes:

1. **17-Character Validation** - Ensures proper VIN length
2. **Character Check** - Excludes invalid characters (I, O, Q)
3. **Year Decode** - Extracts model year from position 10
4. **Manufacturer Code** - Decodes WMI (positions 1-3)
5. **Compatibility Check** - Validates VIN year vs selected vehicle
6. **ECU Match** - Ensures tune compatibility with ECU type

## ⚠️ Legal Disclaimer

**IMPORTANT:** Racing and emissions delete tunes are for:
- ✅ **OFF-ROAD USE ONLY**
- ✅ **COMPETITION VEHICLES ONLY**
- ❌ **NOT STREET LEGAL** in most jurisdictions

BlackFlag requires VIN validation for all racing and delete tunes to ensure:
- Proper ECU compatibility
- Correct calibration data
- User awareness of legal restrictions

## 🛡️ Safety Features

- **VIN Validation** - Required for sensitive tunes
- **Compatibility Warnings** - Alerts for ECU/year mismatches
- **Backup Reminders** - Prompts to backup stock calibration
- **Legal Disclaimers** - Clear warnings in all tune files
- **Requirement Checks** - Validates hardware modifications

## 📝 Development

### Technology Stack
- **Framework:** .NET 8.0 Windows Desktop
- **UI:** WPF (Windows Presentation Foundation)
- **Charts:** LiveChartsCore v2.0.0-rc2
- **Serial:** System.IO.Ports
- **JSON:** Newtonsoft.Json

### Project Structure
```
native/
├── Models/              # Data models (Vehicle, Settings)
├── Services/            # Core services (Database)
├── Views/              # XAML pages (14 pages)
├── Themes/             # Theme resource dictionaries (4 themes)
└── MainWindow.xaml     # Main application window
```

### Building
```powershell
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Publish standalone
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 🤝 Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Credits

- **Developer:** Built with passion for automotive excellence
- **Framework:** Microsoft .NET Team
- **Charts:** LiveCharts Team
- **Community:** Automotive tuning and diagnostic community

## 📞 Support

- **Issues:** [GitHub Issues](../../issues)
- **Discussions:** [GitHub Discussions](../../discussions)
- **Documentation:** See `/docs` folder

## 🗺️ Roadmap

- [ ] Real OBD2 hardware integration
- [ ] CAN bus sniffing and logging
- [ ] Custom tune editor
- [ ] Multi-language support
- [ ] Cloud tune repository
- [ ] Mobile companion app

---

<div align="center">

**Built with ❤️ for the automotive community**

⭐ Star this repo if you find it useful!

</div>
