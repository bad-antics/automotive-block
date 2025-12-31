# 🏴 BlackFlag v2.0 Quick Start Guide

## What is BlackFlag?

BlackFlag is a **FREE**, **OPEN SOURCE**, **NO ACCOUNTS REQUIRED** professional ECU hacking and tuning suite. Zero authentication. Zero restrictions. Complete automotive control.

---

## 🚀 Get Started in 30 Seconds

### 1. Start the Server
```bash
cd "W:\misc workspaces\blackflag"
npm start
```

### 2. Open Your Browser
```
http://localhost:3000
```

### 3. You're Ready!
No login. No signup. No accounts. Just pure ECU power.

---

## 🎮 Web Interface

The dark pirate/hacker themed dashboard includes:
- **Navigation bar** - Switch between features (Wiring, Tuning, Cloning, Modules, OEM Data, Emissions)
- **Sidebar** - Select vehicle, ECU type, quick tools
- **Main panel** - Feature-specific controls
- **Output area** - Real-time results and data

### Color Scheme:
- 🔴 **Neon Red** - Action buttons, warnings
- 🟢 **Hacker Green** - Text, success status
- 🔵 **Neon Cyan** - Secondary information
- ⚫ **Black** - Background (pirate theme)

---

## 🔌 API Access (120+ Endpoints)

### System Status
```bash
curl http://localhost:3000/api/status
```

### Wiring Diagrams
```bash
# List all circuits
curl http://localhost:3000/api/wiring/circuits

# Trace a circuit
curl -X POST http://localhost:3000/api/wiring/trace \
  -H "Content-Type: application/json" \
  -d '{"circuitId":"FUEL_PUMP","startComponent":"Relay","endComponent":"Motor"}'
```

### Tune Manager
```bash
# List tunes
curl http://localhost:3000/api/tunes/list

# Apply tune
curl -X POST http://localhost:3000/api/tunes/apply \
  -H "Content-Type: application/json" \
  -d '{"tuneId":"PERFORMANCE_50","ecuId":"ECU_001","vehicleId":"VIN_123"}'
```

### ECU Cloning
```bash
# Backup ECU
curl -X POST http://localhost:3000/api/cloning/backup \
  -H "Content-Type: application/json" \
  -d '{"ecuId":"ECU_001","ecuType":"Bosch ME7"}'

# Clone ECU
curl -X POST http://localhost:3000/api/cloning/clone \
  -H "Content-Type: application/json" \
  -d '{"sourceEcuId":"ECU_001","targetEcuId":"ECU_002","ecuType":"Bosch ME7"}'
```

### Module Installer
```bash
# List modules
curl http://localhost:3000/api/modules/list

# Install module
curl -X POST http://localhost:3000/api/modules/install \
  -H "Content-Type: application/json" \
  -d '{"moduleId":"DPF_DISABLE","ecuId":"ECU_001","ecuType":"Bosch ME7"}'
```

### OEM As-Built Data
```bash
# Decode VIN
curl -X POST http://localhost:3000/api/asbuilt/decode-vin \
  -H "Content-Type: application/json" \
  -d '{"vin":"WBADT43452G915991"}'

# Import codes
curl -X POST http://localhost:3000/api/asbuilt/import \
  -H "Content-Type: application/json" \
  -d '{"vehicleId":"VIN_123","asBuiltString":"3004:01\n3010:02\n3050:00"}'
```

### Emissions Control
```bash
# List diesel vehicles
curl http://localhost:3000/api/emissions/vehicles

# Enable DPF spoofing
curl -X POST http://localhost:3000/api/emissions/spoof/enable \
  -H "Content-Type: application/json" \
  -d '{"vehicleId":"AUDI_A4_TDI_2010","system":"DPF","profile":"DPF_SPOOF"}'
```

---

## 📊 Feature Breakdown

### 🔌 Wiring Diagram Library
- **50+ automotive circuits**
- Complete color-coded tracing
- Connector pin diagrams
- Exploded assembly views
- Vehicle-specific specs

**Example Circuits:**
- Fuel Pump (12V, 20-40A)
- Ignition System (12V/20KV)
- Starter Motor (100-300A)
- Oxygen Sensors (Lambda feedback)
- Transmission Control
- Lighting systems
- HVAC control

---

### ⚡ Tune Manager
- **100+ ECU modifications**
- 6 tune categories
- Compatibility checking
- Modification history tracking
- Power/torque/fuel impact data

**Tune Categories:**
1. **Performance** - +10% to +80% power
2. **Efficiency** - Up to 20% fuel savings
3. **Emissions** - On/off with spoofing
4. **Custom** - User-defined parameters
5. **Comfort** - Idle, transmission, anti-knock
6. **Diagnostics** - Extended logging

---

### 💾 ECU Cloning
- **Complete backup system** (4MB standard)
- **Full restoration** from backup
- **Clone ECU** from source to target
- **Checksum verification** (CRC32/CRC16)
- **Backup comparison**

**Supported ECUs:**
- Bosch ME7 (2MB)
- EDC17 (4MB)
- Siemens SIMOS (16MB)
- Delphi DCM (8MB)
- Denso (4MB)

---

### 📦 Module Installer
- **10+ programmable modules**
- **Installation/uninstallation**
- **Dependency management**
- **Integrity verification**
- **Enable/disable control**

**Available Modules:**
1. Speed Limiter Removal
2. DPF Disable Module
3. EGR System Disable
4. SCR System Disable
5. Turbo Boost Optimizer
6. Fuel Injector Tuner
7. Transmission Controller
8. Idle Speed Controller
9. Immobilizer Bypass
10. Extended Diagnostics Logger

---

### 🏭 OEM As-Built Data
- **VIN decoding** (18+ manufacturers)
- **As-built import/export**
- **Code comparison** (OEM vs Current)
- **Session management**
- **Factory code application**

**Supported Formats:**
- BMW FA Coding
- Mercedes ASRA
- Audi/VW/Skoda
- Ford/Peugeot/Renault
- Other manufacturers

---

### 🌍 Emissions Control
- **20+ diesel vehicles**
- **DPF/SCR/EGR control**
- **Sensor spoofing** (4 profiles)
- **Regeneration simulation**
- **Euro 4/5/6 compliance**

**Emissions Systems:**
- **DPF** - Pressure (0-100 mbar), Temp (150-600°C), Soot tracking
- **SCR** - Urea injection, NOx reduction (70-90%)
- **EGR** - Valve position (0-100%), Temperature monitoring

---

## 🎯 Common Tasks

### Task 1: Backup Your ECU
```bash
1. Navigate to "Cloning" tab
2. Enter Source ECU ID
3. Click "BACKUP ECU"
4. Save Backup ID for later restoration
```

### Task 2: Apply a Performance Tune
```bash
1. Go to "Tuning" tab
2. Select "Performance" category
3. Click "LIST TUNES"
4. Choose tune (+10%, +25%, +50%, etc.)
5. Click "APPLY TUNE"
6. Verify success message
```

### Task 3: Clone ECU Memory
```bash
1. Navigate to "Cloning" tab
2. Enter Source ECU ID
3. Enter Target ECU ID
4. Click "CLONE ECU"
5. Verify checksum match
```

### Task 4: Install Custom Module
```bash
1. Go to "Modules" tab
2. Click "LIST MODULES"
3. Select module from dropdown
4. Click "INSTALL MODULE"
5. Verify installation status
```

### Task 5: Disable DPF Emissions
```bash
1. Navigate to "Emissions" tab
2. Select diesel vehicle
3. Click "TOGGLE DPF"
4. Confirm sensor spoofing enabled
5. Monitor status
```

---

## 🔑 Key Advantages

✅ **100% FREE** - No subscription, no trial, no premium features  
✅ **NO ACCOUNTS** - Direct access, zero registration  
✅ **NO SIGN-IN** - Open immediately on startup  
✅ **OPEN SOURCE** - MIT licensed, fully transparent  
✅ **PROFESSIONAL** - Enterprise-grade functionality  
✅ **COMPREHENSIVE** - 120+ endpoints, 12 modules  
✅ **DOCUMENTED** - 10,000+ lines of documentation  
✅ **EXAMPLES** - 7+ working demonstrations included  

---

## 📋 Requirements

- **Operating System:** Windows, Mac, Linux
- **Node.js:** v12 or higher
- **NPM:** v6 or higher
- **Browser:** Chrome, Firefox, Edge (modern)
- **Port:** 3000 (default)
- **Network:** Local machine or local network

---

## ⚙️ Configuration

### Change Port
```javascript
// In index.js, line 21:
const PORT = 3000; // Change this number
```

### Enable HTTPS
- Currently running on HTTP
- SSL support can be added via Express configuration

### Database Connection
- Currently using in-memory storage
- Can be extended with persistent database

---

## 🆘 Troubleshooting

### "Cannot find module" Error
```bash
npm install
```

### Port 3000 Already in Use
```bash
# Use different port or kill process on 3000
# Windows: netstat -ano | findstr :3000
# Then: taskkill /PID [PID] /F
```

### CORS Errors
- Already configured for all origins
- Can be modified in middleware section

### Module Not Found
- Ensure all .js files are in root directory:
  - oem-as-built.js
  - ecu-cloner.js
  - module-installer.js
  - wiring-diagrams.js
  - tune-manager.js
  - emissions-controller.js

---

## 📚 Additional Resources

- **Complete Guide:** BLACKFLAG_v2.0_COMPLETE.md
- **API Reference:** Included in status endpoint
- **Examples:** integrated-examples.js
- **Wiring Guide:** WIRING_DIAGRAMS_GUIDE.md
- **Tune Guide:** TUNE_MANAGER_GUIDE.md
- **Emissions Guide:** EMISSIONS_CONTROL_GUIDE.md

---

## 🏴 About BlackFlag

**Name Origin:** Black flag = ultimate freedom, no rules  
**Philosophy:** Open, transparent, unrestricted access  
**Goal:** Professional ECU hacking for everyone  
**License:** MIT (Do whatever you want)  

---

## ⚖️ Legal Notice

BlackFlag is provided for **educational and research purposes**. Users are responsible for ensuring compliance with local regulations regarding vehicle modifications.

- For research only in most jurisdictions
- Not for street use in many regions
- Check local vehicle modification laws
- Professional use: dyno testing, tuning shops, development

---

## 🚀 Start Now!

```bash
cd "W:\misc workspaces\blackflag"
npm start
```

Then open your browser to **http://localhost:3000**

---

**🏴 BlackFlag v2.0 - Zero Restrictions, Maximum Power**  
*Professional ECU Hacking Suite | Free & Open | No Accounts*

