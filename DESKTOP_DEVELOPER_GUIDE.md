# BlackFlag Desktop Standalone - Developer Guide

## Architecture Overview

```
┌─────────────────────────────────────────────────┐
│              Electron Main Process              │
├─────────────────────────────────────────────────┤
│  • main.js - Electron app lifecycle            │
│  • db.js - Local database management           │
│  • Express API - REST endpoints for UI         │
│  • IPC Handlers - Secure communication         │
└─────────────────────────────────────────────────┘
           ▲                              ▲
           │ IPC                         │ HTTP
           │                             │ localhost:3000
┌──────────┴─────────────────┐  ┌────────┴──────────────┐
│  Preload Script            │  │  Browser Window      │
│  (preload.js)              │  │  • HTML UI           │
│  Secure API Bridge         │  │  • CSS Styling       │
└────────────────────────────┘  │  • JavaScript App    │
                                 └─────────┬───────────┘
                                           │
                                    ┌──────▼──────────┐
                                    │  API Helper     │
                                    │ (api-helper.js) │
                                    └─────────────────┘
```

## File Structure

```
desktop/
├── main.js                    # Electron main process
├── db.js                      # Database layer (NEW)
├── preload.js                 # Security preload script
├── package.json               # Dependencies
├── ui/
│   ├── index.html            # Main UI
│   ├── blackflag-app.js      # UI functions
│   ├── desktop-app.js        # Desktop-specific features
│   ├── api-helper.js         # API abstraction (NEW)
│   ├── styles-blackflag.css
│   └── themes.css
└── README.md

Database Storage:
~/.blackflag/
├── vehicles.json
├── ecu-profiles.json
├── tunes.json
├── settings.json
├── logs.json
└── backups/
```

## Key Classes & Methods

### DesktopDatabase Class

Initialize database in main.js:
```javascript
const DesktopDatabase = require('./db');
const db = new DesktopDatabase();
```

Core methods:
```javascript
// Vehicles
db.getVehicles()                          // Array of all vehicles
db.getVehicleById(vehicleId)             // Single vehicle object
db.getVehiclesByManufacturer(manufacturer) // Filtered vehicles
db.getManufacturers()                     // Array of manufacturer names
db.addVehicle(vehicleData)               // Returns vehicle ID
db.updateVehicle(vehicleId, updates)     // Boolean success

// ECU Profiles
db.getECUProfiles(vehicleId)             // Array of profiles
db.addECUProfile(vehicleId, profile)     // Returns profile ID

// Tunes
db.getTunes(vehicleId)                   // Array of tunes
db.saveTune(vehicleId, tuneData)         // Returns tune ID
db.getTuneById(vehicleId, tuneId)        // Single tune object

// Settings
db.getSetting(key)                       // Any type value
db.setSetting(key, value)                // Boolean success
db.getAllSettings()                      // Object of all settings

// Logging
db.addLog(logEntry)                      // Returns log ID
db.getLogs(limit = 100)                  // Array of recent logs

// Backup
db.createBackup()                        // Returns timestamp or null
db.getDataDirectoryInfo()                // Object with path, size, backups
db.restoreBackup(timestamp)              // Boolean success

// Utility
db.saveData(filePath, data)              // Direct file save
db.loadData(filePath)                    // Direct file load
db.getDirectorySize(dir)                 // Size in bytes
db.getBackupsList()                      // Array of backup timestamps
```

## Adding New Features

### 1. Adding a New Data Type

In `db.js`, add methods:
```javascript
// Get all items
getMyItems(vehicleId) {
    const data = this.loadData(this.myItemsFile);
    if (!data || !data.items) return [];
    return data.items[vehicleId] || [];
}

// Save new item
saveMyItem(vehicleId, item) {
    const data = this.loadData(this.myItemsFile);
    if (!data.items[vehicleId]) {
        data.items[vehicleId] = [];
    }
    item.id = item.id || crypto.randomBytes(8).toString('hex');
    item.createdAt = new Date().toISOString();
    data.items[vehicleId].push(item);
    this.saveData(this.myItemsFile, data);
    return item.id;
}
```

In constructor:
```javascript
this.myItemsFile = path.join(this.dataDir, 'my-items.json');
// In initializeDatabase:
if (!fs.existsSync(this.myItemsFile)) {
    this.saveData(this.myItemsFile, { items: {} });
}
```

### 2. Adding API Endpoints

In `main.js`:
```javascript
// Get items for vehicle
expressApp.get('/api/my-items/:vehicleId', (req, res) => {
    try {
        const items = db.getMyItems(req.params.vehicleId);
        res.json({ items });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Save new item
expressApp.post('/api/my-items/:vehicleId', (req, res) => {
    try {
        const itemId = db.saveMyItem(req.params.vehicleId, req.body);
        res.json({ success: true, itemId });
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});
```

### 3. Using in Frontend

In `api-helper.js`:
```javascript
async function fetchMyItems(vehicleId) {
    try {
        const response = await fetch(`${API_URL}/my-items/${vehicleId}`);
        if (!response.ok) throw new Error('Failed to fetch items');
        const data = await response.json();
        return data.items || [];
    } catch (error) {
        console.error('Error fetching items:', error);
        return [];
    }
}

async function saveMyItem(vehicleId, item) {
    try {
        const response = await fetch(`${API_URL}/my-items/${vehicleId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(item)
        });
        if (!response.ok) throw new Error('Failed to save item');
        const data = await response.json();
        return data.itemId;
    } catch (error) {
        console.error('Error saving item:', error);
        return null;
    }
}

// Export to window
window.BlackFlagAPI.fetchMyItems = fetchMyItems;
window.BlackFlagAPI.saveMyItem = saveMyItem;
```

### 4. Using in UI

In HTML/JavaScript:
```javascript
async function loadMyItems(vehicleId) {
    const items = await BlackFlagAPI.fetchMyItems(vehicleId);
    // Process items...
}

async function addMyItem(vehicleId, itemData) {
    const itemId = await BlackFlagAPI.saveMyItem(vehicleId, itemData);
    if (itemId) {
        console.log('Item saved:', itemId);
        // Refresh display
        loadMyItems(vehicleId);
    }
}
```

## IPC Communication

### Exposing New IPC Handlers

In `main.js`:
```javascript
ipcMain.handle('custom-operation', async (event, data) => {
    try {
        const result = await performOperation(data);
        return { success: true, result };
    } catch (error) {
        return { success: false, error: error.message };
    }
});
```

In `preload.js`:
```javascript
contextBridge.exposeInMainWorld('electron', {
    // ... existing methods ...
    customOperation: (data) => ipcRenderer.invoke('custom-operation', data)
});
```

In UI code:
```javascript
const result = await electron.customOperation(data);
```

## Express API Patterns

### Standard Success Response
```javascript
res.json({ success: true, data: result })
```

### Standard Error Response
```javascript
res.status(400).json({ success: false, error: 'Description' })
```

### With Data
```javascript
res.json({ items: [], count: 10, timestamp: new Date().toISOString() })
```

## Testing

### Manual Testing
1. Start app: `npm start` in desktop directory
2. Open Dev Tools: View > Toggle Developer Tools
3. Console: Check for errors
4. Test data operations through UI

### Testing Database
```javascript
// In Dev Tools console:
fetch('http://localhost:3000/api/status')
    .then(r => r.json())
    .then(d => console.log(d))

fetch('http://localhost:3000/api/vehicles/list')
    .then(r => r.json())
    .then(d => console.log(d))
```

### Database File Inspection
```powershell
# Windows
cat $env:USERPROFILE\.blackflag\vehicles.json | ConvertFrom-Json | ConvertTo-Json

# macOS/Linux
cat ~/.blackflag/vehicles.json | jq
```

## Performance Tips

1. **Limit Database Reads**: Cache results when possible
2. **Batch Operations**: Use arrays instead of individual saves
3. **Async Operations**: Always await database calls
4. **Clean Old Logs**: Implement log rotation (default: keep 1000)
5. **Backup Management**: Limit backups to 10 (configurable)

## Debugging

### Enable Debug Mode
```javascript
// In settings.json
{
    "debugMode": true,
    // ... other settings
}
```

### View Logs
- Menu > Tools > Logs
- Or check: `~/.blackflag/logs.json`

### Dev Tools
- F12 or View > Toggle Developer Tools
- Console tab for errors
- Network tab to see API calls
- Application tab to inspect storage

## Security Best Practices

1. **Validate Input**: Check data before saving to database
2. **Sanitize Output**: Clean data when displaying in UI
3. **IPC Security**: Don't expose filesystem directly
4. **Error Messages**: Don't expose internal paths in errors
5. **Backups**: Encrypt sensitive backups if adding encryption

## Common Tasks

### Add Vehicle to Database
```javascript
db.addVehicle({
    id: 'custom_vehicle_id',
    manufacturer: 'Toyota',
    model: 'Camry',
    year: 2020,
    engine: '2.5L 4-Cylinder',
    // ... other properties
});
```

### Create Backup
```javascript
const timestamp = db.createBackup();
console.log('Backup created:', timestamp);
```

### Export All Data
```javascript
const vehicles = db.getVehicles();
const settings = db.getAllSettings();
const logs = db.getLogs(1000);

const allData = { vehicles, settings, logs };
fs.writeFileSync('export.json', JSON.stringify(allData, null, 2));
```

### Clean Old Data
```javascript
// Keep only last 100 logs
const logs = db.getLogs(100);
db.saveData(db.logsFile, { logs });
```

## Deployment

### Building Installer
```bash
cd desktop
npm run dist
```

Creates:
- Windows: `dist/BlackFlag ECU Suite Setup 2.0.0.exe`
- macOS: `dist/BlackFlag ECU Suite-2.0.0.dmg`
- Linux: `dist/blackflag-2.0.0.AppImage`

### Configuration for Distribution
Edit `desktop/package.json` build section:
```json
"build": {
    "appId": "com.blackflag.ecu",
    "productName": "BlackFlag ECU Suite",
    // ... distribution settings
}
```

## Troubleshooting Development

### Database Not Initializing
- Check directory permissions
- Verify Node.js can write to home directory
- Check logs: `~/.blackflag/logs.json`

### API Not Responding
- Confirm port 3000 is available
- Check main process didn't crash
- View Dev Tools console for errors

### Data Not Persisting
- Verify all methods use `.saveData()`
- Check file write permissions
- Confirm correct file paths in constructor

---

**Last Updated**: December 30, 2024  
**Version**: 2.0.0 Desktop Standalone
