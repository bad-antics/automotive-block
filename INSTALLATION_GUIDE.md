# BlackFlag Desktop - Installation Guide

## 📦 Installation Methods

### Method 1: Standalone Executable (Easiest - Recommended)

**Perfect for end users who just want to run the application.**

#### Steps:
1. Download `BlackFlag.exe` from the [Releases](../../releases) page
2. Save to any folder (e.g., `C:\Program Files\BlackFlag\` or `Desktop`)
3. Double-click `BlackFlag.exe` to launch
4. ✅ That's it! No installation required.

#### Features:
- ✅ No .NET Runtime installation needed
- ✅ All dependencies bundled (~155 MB)
- ✅ Single file - easy to manage
- ✅ Portable - run from USB drive
- ✅ No admin rights required

---

### Method 2: Install .NET Runtime + Application

**For users who prefer smaller download sizes.**

#### Step 1: Install .NET 8.0 Runtime
1. Download [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Run installer: `windowsdesktop-runtime-8.0.x-win-x64.exe`
3. Follow installation wizard

#### Step 2: Download BlackFlag
1. Download `BlackFlag-Runtime-Required.zip` from [Releases](../../releases)
2. Extract to desired folder
3. Run `BlackFlag.exe`

#### Features:
- ✅ Smaller download (~5 MB application)
- ✅ Shared .NET runtime (used by other apps)
- ✅ Faster updates

---

### Method 3: Build from Source (For Developers)

**For developers who want to modify or contribute to BlackFlag.**

#### Prerequisites:
- **Git** - [Download](https://git-scm.com/)
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (optional) or **VS Code**

#### Step 1: Clone Repository
```powershell
git clone https://github.com/yourusername/blackflag.git
cd blackflag
```

#### Step 2: Navigate to Native Project
```powershell
cd native
```

#### Step 3: Restore Dependencies
```powershell
dotnet restore
```

#### Step 4: Build Application
```powershell
# Debug build
dotnet build

# Release build
dotnet build -c Release
```

#### Step 5: Run Application
```powershell
# Debug
.\bin\Debug\net8.0-windows\BlackFlag.exe

# Release
.\bin\Release\net8.0-windows\BlackFlag.exe
```

#### Features:
- ✅ Full source code access
- ✅ Modify and customize
- ✅ Debug capabilities
- ✅ Contribute improvements

---

### Method 4: Publish Single-File Executable (For Distribution)

**Create your own standalone build.**

#### Steps:
```powershell
cd native
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

#### Output Location:
```
native\bin\Release\net8.0-windows\win-x64\publish\BlackFlag.exe
```

#### Publish Options:
```powershell
# Windows x64 (most common)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Windows x86 (32-bit)
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true

# Windows ARM64
dotnet publish -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true
```

---

## 🖥️ System Requirements

### Minimum:
- **OS:** Windows 10 (64-bit) version 1809 or later
- **CPU:** Dual-core 1.5 GHz
- **RAM:** 4 GB
- **Storage:** 200 MB free space
- **Display:** 1366x768

### Recommended:
- **OS:** Windows 11 (64-bit)
- **CPU:** Quad-core 2.5 GHz or better
- **RAM:** 8 GB or more
- **Storage:** 500 MB free space
- **Display:** 1920x1080 or higher
- **Optional:** OBD2 ELM327 adapter for live data

---

## 🔧 First Launch Setup

### 1. Initial Configuration
When you first launch BlackFlag:

1. **Database Initialization**
   - BlackFlag creates: `%LOCALAPPDATA%\BlackFlag\`
   - Loads 159 vehicles from embedded database
   - Creates settings file

2. **Theme Selection**
   - Default: Dark Theme
   - Change via hamburger menu (top-left)
   - 4 themes available: Dark, Retro Green, Ford Blue, Orange Tech

3. **File Locations**
   - Tunes: `%USERPROFILE%\Documents\BlackFlag\Tunes\`
   - OE Files: `%USERPROFILE%\Documents\BlackFlag\OE_Files\`
   - Settings: `%LOCALAPPDATA%\BlackFlag\settings.json`
   - Database: `%LOCALAPPDATA%\BlackFlag\vehicles.json`

### 2. Optional: OBD2 Setup (for Live Data)

#### Hardware:
- **Recommended:** ELM327 Bluetooth or USB adapter
- **Supported:** Any OBD2-compliant adapter
- **Connection:** USB (COM port) or Bluetooth (virtual COM port)

#### Configuration:
1. Plug in OBD2 adapter
2. Navigate to **Live Data** page
3. Select **COM Port** from dropdown
4. Set **Baud Rate** (usually 38400 or 115200)
5. Click **Connect**

---

## 📂 Folder Structure After Installation

```
BlackFlag.exe                    # Main executable

%LOCALAPPDATA%\BlackFlag\
├── vehicles.json               # Vehicle database cache
└── settings.json               # User settings

%USERPROFILE%\Documents\BlackFlag\
├── Tunes\                      # Downloaded tune files
│   └── {Year}_{Make}_{Model}\
│       └── *.bin
└── OE_Files\                   # OE flash files & as-built
    └── {Year}_{Make}_{Model}\
        ├── *.bin
        └── AsBuilt\
            └── *.txt
```

---

## 🛠️ Troubleshooting

### Issue: "This app can't run on your PC"
**Solution:** Download correct architecture:
- Most users need **win-x64**
- 32-bit Windows needs **win-x86**
- ARM laptops need **win-arm64**

### Issue: Application won't start (Method 2)
**Solution:** Install .NET 8.0 Desktop Runtime:
```powershell
# Check if .NET 8.0 is installed
dotnet --list-runtimes

# Should show: Microsoft.WindowsDesktop.App 8.0.x
```

### Issue: "Missing DLL" error
**Solution:** Use standalone executable (Method 1) or reinstall .NET Runtime

### Issue: Slow startup on first launch
**Cause:** Database initialization (loads 159 vehicles)
**Normal:** First launch takes 3-5 seconds
**Subsequent:** Launches in 1-2 seconds

### Issue: OBD2 device not found
**Solution:**
1. Check Device Manager → Ports (COM & LPT)
2. Note COM port number (e.g., COM3)
3. Try different baud rates: 9600, 38400, 115200
4. Bluetooth: Ensure adapter is paired

### Issue: Can't download tunes (VIN validation)
**Solution:**
1. Enter full 17-character VIN
2. Click "🔍 Decode VIN" button
3. Verify year matches selected vehicle
4. Racing/delete tunes REQUIRE VIN validation

---

## 🔄 Updating BlackFlag

### Standalone Version:
1. Download new `BlackFlag.exe` from releases
2. Replace old executable
3. Settings and data preserved

### Runtime Version:
1. Download new `BlackFlag-Runtime-Required.zip`
2. Extract and replace files
3. Settings preserved

### From Source:
```powershell
git pull origin main
cd native
dotnet build -c Release
```

---

## ❓ FAQ

**Q: Do I need admin rights?**
A: No, BlackFlag runs without admin privileges.

**Q: Can I run from USB drive?**
A: Yes! Use standalone executable (Method 1).

**Q: Does it work offline?**
A: Yes, all features work offline. Vehicle database is embedded.

**Q: Is antivirus blocking it?**
A: Uncommon, but possible. Add BlackFlag.exe to exclusions if needed.

**Q: Can I install on multiple PCs?**
A: Yes, copy standalone executable to any Windows PC.

**Q: Where are my tunes stored?**
A: `%USERPROFILE%\Documents\BlackFlag\Tunes\`

**Q: How do I backup my data?**
A: Copy entire `%USERPROFILE%\Documents\BlackFlag\` folder

**Q: Can I customize themes?**
A: Yes! Edit XAML files in `native\Themes\` directory (requires rebuild)

---

## 📞 Support

- **Issues:** [GitHub Issues](../../issues)
- **Documentation:** See main [README.md](README.md)
- **Build Errors:** Check [Developer Guide](DEVELOPER_GUIDE.md)

---

## ✅ Installation Checklist

- [ ] Downloaded BlackFlag.exe
- [ ] Placed in desired folder
- [ ] Launched successfully
- [ ] Database initialized (159 vehicles loaded)
- [ ] Selected theme preference
- [ ] (Optional) Connected OBD2 adapter
- [ ] (Optional) Tested VIN decoder
- [ ] (Optional) Downloaded sample tune
- [ ] Ready to use! 🎉

---

<div align="center">

**Installation Complete! Enjoy BlackFlag Desktop! 🚀**

⭐ Star the repo | 🐛 Report issues | 💡 Request features

</div>
