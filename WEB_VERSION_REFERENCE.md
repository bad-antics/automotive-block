# BlackFlag v2.0 - Web Version Reference

## Overview
BlackFlag is a professional ECU hacking and tuning suite with a menu-driven web interface built with Node.js + Express.

## Architecture

### Backend
- **Server**: Node.js + Express.js on port 3000
- **Modules**: 15+ specialized handlers (OBD2, CAN, ECU Processor, etc.)
- **Database**: 49 vehicles with complete specifications
- **API**: 150+ endpoints for all features

### Frontend
- **Framework**: Vanilla JavaScript, HTML5, CSS3
- **Theme**: Cyberpunk dark theme with neon green/cyan accents
- **Layout**: Menu-driven UI with 9 function screens

## Features

### Main Menu Cards (9 Total)
1. **🔍 VIN Decoder** - Decode VINs & export specs as .txt
2. **🔌 ECU Scanner** - Auto-detect ECUs & connections
3. **⚡ Voltage Meter** - Real-time system voltage monitoring
4. **📋 Wiring Diagrams** - View circuit diagrams
5. **⚡ Tune Manager** - Browse & apply ECU tunes
6. **💾 ECU Cloning** - Backup, clone & restore ECU data
7. **📦 Module Installer** - Install custom modules
8. **💨 Emissions Control** - DPF/SCR/EGR management
9. **📚 Library Browser** - Guides, specs & diagrams

### Library Browser (3 Tabs)
- **Vehicle Specs**: Search database of 49 vehicles with specs
- **Diagnostic Guides**: 6 comprehensive technical guides
  - OBD-II Basics
  - CAN Bus Diagnostics
  - ECU Communication
  - Emissions Codes
  - Voltage Testing
  - ECU Reprogramming
- **Wiring Diagrams**: 10 system diagrams
  - Charging, Starting, Ignition, Fuel Pump, Injectors
  - O2 Sensors, CAN Bus, ABS, Transmission, Emissions

## Database

### Vehicle Database (49 vehicles)
- 35 original light-duty diesel trucks
- 14 new diesel truck additions
- Includes: Ford, Chevy, GMC, Ram, Toyota, Nissan
- Each vehicle has:
  - Power, torque, engine specs
  - ECU types and protocols
  - Transmission options
  - Wiring diagrams
  - Emissions systems

## API Endpoints (Key Examples)

### Vehicle Data
- `GET /api/vehicles/list` - List all vehicles
- `GET /api/vehicles/:id` - Get vehicle details
- `GET /api/vehicles/search?q=ford` - Search vehicles

### Diagnostics
- `GET /api/obd2/codes` - List fault codes
- `POST /api/diagnostics/scan` - Scan for issues
- `GET /api/ecu/detect` - Detect connected ECUs

### File Operations
- `POST /api/ecu/backup` - Backup ECU data
- `POST /api/ecu/clone` - Clone ECU
- `POST /api/ecu/restore` - Restore from backup

## Files & Structure

### Web Root
```
public/
├── index.html          # Main HTML interface
├── styles-blackflag.css # Cyberpunk theme (1762 lines)
├── blackflag-app.js    # Frontend logic (1000+ lines)
├── app.js              # (Legacy, use index.js)
├── pinouts.html        # Vehicle pinouts
└── styles.css          # Legacy styles

src/
├── index.html          # Mirrored from public
├── styles-blackflag.css # Mirrored from public
├── blackflag-app.js    # Mirrored from public
├── index-blackflag.html # Legacy
└── styles-blackflag.css # Legacy
```

### Server
```
index.js                    # Main Express server (839 lines)
├── obd2.js               # OBD-II handler
├── canbus.js             # CAN bus controller
├── diagnostics.js        # Diagnostic engine
├── ecu-processor.js      # ECU manipulation
├── tune-manager.js       # Tune management
├── ecu-cloner.js         # ECU backup/restore
├── module-installer.js   # Custom module installer
├── emissions-controller.js # DPF/SCR/EGR control
├── wiring-diagrams.js    # Wiring documentation
├── ecu-software-updates.js
├── ecu-test-bench.js
├── j2534.js              # J2534 protocol handler
├── vehicle-database.js   # 49 vehicle specs
├── ecudb.js              # ECU database
├── oem-as-built.js       # OEM data manager
└── package.json          # Dependencies
```

## How to Run

### Start Server
```powershell
cd "w:\misc workspaces\blackflag"
node index.js
```

Server runs on `http://localhost:3000`

### Access Web Version
- Browser: `http://localhost:3000`
- Cache-busting headers prevent stale content
- Supports both src/ and public/ folders

## Frontend JavaScript Functions

### Navigation
- `openFunction(functionId)` - Open function screen
- `backToMenu()` - Return to menu
- `selectVehicle()` - Select vehicle context

### Vehicle Library
- `searchVehicleDatabase()` - Search vehicles
- `switchLibraryTab(tabName)` - Switch library tabs
- `viewGuide(guideId)` - View diagnostic guide
- `loadWiringDiagram()` - Load wiring diagram

### Core Features
- `decodeVIN()` - VIN decoder
- `initializeOBD2()` - Connect to vehicle
- `scanECUs()` - Detect ECUs
- `startVoltageMeterFull()` - Monitor voltage
- `backupECU()`, `cloneECU()`, `restoreECU()` - ECU operations
- `toggleDPF()`, `toggleSCR()`, `toggleEGR()` - Emissions

## CSS Theme

### Color Scheme
- Primary: `#00ff00` (Neon Green)
- Secondary: `#00ffff` (Neon Cyan)
- Accent: `#ff0080` (Neon Pink)
- Background: `#000000` (Pure Black)

### Key Classes
- `.menu-card` - Clickable function cards
- `.library-tab-content` - Tab panels
- `.function-screen` - Full-screen function views
- `.output-panel` - Results display areas
- `.voltage-display-large` - Large voltage readout

## Dependencies
```json
{
  "express": "^4.x",
  "body-parser": "^1.x",
  "cors": "^2.x"
}
```

## Network Hosting

### Current Setup
- Localhost: `http://localhost:3000`
- Network access: `http://<YOUR_IP>:3000`

### For Network Deployment
1. Update firewall rules to allow port 3000
2. Bind server to `0.0.0.0` in index.js
3. Access from other machines on network

## Performance Notes
- CSS: 1762 lines (optimized)
- JS: 1000+ lines (modular functions)
- HTML: 373 lines (semantic structure)
- Load time: <1s on local network
- API response: <100ms typical

## Browser Support
- Chrome/Chromium: Full support
- Firefox: Full support
- Edge: Full support
- Mobile: Limited (menu responsive but optimized for desktop)

## Future Enhancements
- Real-time CAN message visualization
- Hardware integration (J2534 adapters)
- Multi-vehicle simultaneous access
- Advanced security/authentication
- Cloud synchronization option
