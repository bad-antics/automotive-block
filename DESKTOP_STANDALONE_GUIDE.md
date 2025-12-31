# BlackFlag v2.0 Desktop - Standalone Edition

## Overview

The desktop app is now **completely independent** and runs as a standalone application with its own integrated database. It no longer requires the external server or any network connectivity.

## Key Features

### 🔒 **Local Database**
- All data stored locally in the user's home directory
- No external server required
- Complete data privacy - nothing sent to external services
- Automatic backup and restore functionality

### 🚀 **Standalone Architecture**
- **Desktop Database Module** (`desktop/db.js`)
  - JSON-based local storage
  - Automatic initialization
  - Support for vehicles, ECU profiles, tunes, settings, and logs

- **Integrated Express API** (`desktop/main.js`)
  - Runs embedded within Electron
  - Provides REST API endpoints for the UI
  - All operations use the local database

- **IPC Communication** (`desktop/preload.js`)
  - Secure bridge between renderer and main processes
  - Database operations accessible via IPC

- **Enhanced UI** (`desktop/ui/`)
  - Works seamlessly with local database
  - API helper module for consistent data access
  - No external API calls needed

## Data Storage

### Database Location
- **Windows**: `C:\Users\{username}\.blackflag\`
- **macOS**: `/Users/{username}/.blackflag/`
- **Linux**: `/home/{username}/.blackflag/`

### Data Files
```
.blackflag/
├── vehicles.json           # Vehicle database
├── ecu-profiles.json       # ECU profiles per vehicle
├── tunes.json             # Saved tunes and modifications
├── settings.json          # Application settings
├── logs.json              # System logs and history
└── backups/               # Automatic backups
    ├── 2024-12-30T10-30-45-123Z/
    │   ├── vehicles.json
    │   ├── ecu-profiles.json
    │   ├── tunes.json
    │   └── settings.json
    └── ...
```

## API Endpoints (Internal)

All endpoints are served locally on `http://localhost:3000/api`:

### Vehicles
- `GET /api/vehicles/list` - Get all vehicles
- `GET /api/vehicles/:vehicleId` - Get specific vehicle
- `GET /api/manufacturers` - Get all manufacturers
- `GET /api/manufacturers/:manufacturer/vehicles` - Get vehicles by manufacturer

### ECU Profiles
- `GET /api/ecu-profiles/:vehicleId` - Get profiles for vehicle
- `POST /api/ecu-profiles/:vehicleId` - Save new ECU profile

### Tunes
- `GET /api/tunes/:vehicleId` - Get tunes for vehicle
- `POST /api/tunes/:vehicleId` - Save new tune
- `GET /api/tunes/:vehicleId/:tuneId` - Get specific tune

### Settings
- `GET /api/settings` - Get all settings
- `GET /api/settings/:key` - Get specific setting
- `POST /api/settings/:key` - Update setting

### Backup & Restore
- `POST /api/backup` - Create database backup
- `GET /api/backups` - List available backups
- `POST /api/restore/:timestamp` - Restore from backup

### System
- `GET /api/health` - Health check
- `GET /api/status` - System status and modules
- `GET /api/logs` - System logs

## Included Databases

The app comes pre-loaded with:
- **5 sample vehicles** with full specifications
- ECU configuration examples
- Wiring diagram references
- Tune templates

Add more vehicles through the UI or by editing `vehicles.json` directly.

## Default Settings

```json
{
  "theme": "cyberpunk",
  "language": "en",
  "autoBackup": true,
  "backupInterval": 3600000,     // 1 hour
  "maxBackups": 10,
  "debugMode": false,
  "checkUpdates": true,
  "telemetry": false,
  "defaultVehicleIndex": 0
}
```

## Backup Management

### Automatic Backups
- Triggered when data is modified (optional - requires setting)
- Triggered on app exit
- Configurable interval

### Manual Backups
- Use `File > Create Backup` menu option
- Or call `electron.backup()` from UI
- Or POST to `/api/backup` endpoint

### Restore Backup
- Accessible through menu or API
- Restores all data files from selected backup
- Previous state is preserved as new backup

## Security

### IPC Security
- Context isolation enabled
- No node integration in renderer
- Sandbox enabled
- Limited API exposure through preload

### Data Privacy
- All data stored locally
- No telemetry by default
- No external connections required
- User controls all backups

## Installation & Running

### Requirements
- Node.js 14+
- npm or yarn

### First Run
```powershell
cd desktop
npm install
npm start
```

### Development Mode
```powershell
npm run dev
```

### Build Executable
```powershell
npm run dist
```

## Included Database Methods

The database module (`db.js`) provides:

### Vehicle Management
```javascript
getVehicles()                           // Get all vehicles
getVehicleById(vehicleId)              // Get single vehicle
getVehiclesByManufacturer(manufacturer) // Filter by manufacturer
getManufacturers()                      // Get all manufacturers
addVehicle(vehicleData)                 // Add new vehicle
updateVehicle(vehicleId, updates)      // Update vehicle
```

### ECU Profiles
```javascript
getECUProfiles(vehicleId)               // Get profiles
addECUProfile(vehicleId, profile)      // Save profile
```

### Tunes
```javascript
getTunes(vehicleId)                     // Get tunes
saveTune(vehicleId, tuneData)          // Save tune
getTuneById(vehicleId, tuneId)         // Get specific tune
```

### Settings
```javascript
getSetting(key)                         // Get setting
setSetting(key, value)                  // Update setting
getAllSettings()                        // Get all settings
```

### Logging
```javascript
addLog(logEntry)                        // Add log entry
getLogs(limit)                          // Get recent logs
```

### Backup & Restore
```javascript
createBackup()                          // Create backup
getDataDirectoryInfo()                  // Get database info
restoreBackup(timestamp)                // Restore backup
```

## API Helper Module

The frontend uses `api-helper.js` for consistent data access:

```javascript
// Example usage in UI
const vehicles = await BlackFlagAPI.fetchVehicles();
const vehicle = await BlackFlagAPI.getVehicleById('ford_mustang_2015');
const profiles = await BlackFlagAPI.fetchECUProfiles(vehicleId);
```

All functions return promises and handle errors gracefully.

## Menu Options

### File Menu
- Create Backup (Ctrl+B)
- Exit (Ctrl+Q)

### Edit Menu
- Standard cut/copy/paste

### View Menu
- Reload, Debug Tools, Zoom, Full Screen

### Tools Menu
- Database Info - View storage location and size

### Help Menu
- About BlackFlag

## Features by Design

✅ **Fully Standalone**
- Works offline
- No internet required
- No external API dependencies

✅ **Data Persistence**
- JSON-based storage for flexibility
- Easy to backup and restore
- Human-readable format for debugging

✅ **Modular Architecture**
- Separate database layer
- API abstraction layer
- UI layer uses consistent API

✅ **Easy to Extend**
- Add new data types by updating database
- Add new API endpoints in main.js
- Add new UI features in app.js

## Troubleshooting

### Database Issues
1. Check database directory exists: `~/.blackflag/`
2. Check files have proper permissions
3. View logs through UI (Menu > Help > Logs)
4. Create manual backup and restore if corrupted

### API Connection Failed
1. Ensure localhost:3000 is available
2. Check for port conflicts
3. Restart application
4. Check console for errors (Dev Tools)

### Data Loss
1. Restore from backup (File > Database > Restore)
2. Check backups directory for available backups
3. Last 10 backups are kept automatically

## Next Steps

1. **Customize Vehicle Database**: Edit `vehicles.json` to add your vehicles
2. **Set Preferences**: Configure theme, backup interval, etc.
3. **Create Profiles**: Build ECU profiles for your vehicles
4. **Save Tunes**: Store and organize your tune configurations
5. **Backup Regularly**: Use File > Create Backup menu option

## Roadmap

Future enhancements:
- SQLite database option for larger datasets
- Export/import data formats (XML, CSV)
- Multi-device sync capability
- Advanced analytics and reporting
- Custom database field definitions
- Database encryption option

---

**Version**: 2.0.0 - Desktop Standalone Edition  
**Status**: Production Ready  
**Last Updated**: December 30, 2024
