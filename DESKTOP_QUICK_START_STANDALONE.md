# BlackFlag Desktop - Standalone Edition - Quick Start

## What's New?

Your BlackFlag desktop app is now **completely independent** with its own local database. No external server needed!

## ⚡ Quick Start (30 seconds)

### 1. Launch the App
```powershell
cd desktop
npm start
```

The app will:
- Create a local database in `~/.blackflag/`
- Start an embedded Express server on port 3000
- Load the Electron window
- Display 5 sample vehicles

### 2. That's It!
The app is fully functional and offline-ready.

## 📂 Where's My Data?

**Windows**: `C:\Users\YourUsername\.blackflag\`

**Files created automatically:**
- `vehicles.json` - Your vehicle database
- `ecu-profiles.json` - ECU profiles per vehicle
- `tunes.json` - Saved tune configurations
- `settings.json` - Application preferences
- `logs.json` - System activity logs
- `backups/` - Automatic backups with timestamps

## 🎮 Main Features

✅ **Add Vehicles** - Use the dropdown to add vehicles to your history  
✅ **Create Profiles** - Build ECU profiles for your vehicles  
✅ **Save Tunes** - Store tune configurations locally  
✅ **Backup Data** - File > Create Backup (automatic backups available too)  
✅ **Restore** - Recover from any previous backup  
✅ **Themes** - Switch between 4 themes  
✅ **Offline** - Works without internet  

## 🔧 Key Menu Options

**File Menu:**
- Create Backup (Ctrl+B) - Backup all your data
- Exit (Ctrl+Q) - Close the application

**Tools Menu:**
- Database Info - See where your data is stored

**View Menu:**
- Toggle DevTools (F12) - For debugging
- Zoom controls - Adjust window size

## 📊 Sample Vehicles Included

1. **Ford Mustang 2015** - 5.0L V8 RWD
2. **Ford F-150 2017** - Truck/Diesel options
3. **Chevrolet Corvette 2014** - High-performance V8
4. **Dodge Challenger 2015** - Muscle car HEMI
5. **BMW M3 2014** - Turbo performance

Edit `~/.blackflag/vehicles.json` to add your own vehicles.

## 🛠️ For Developers

See **DESKTOP_DEVELOPER_GUIDE.md** for:
- Architecture overview
- How to add features
- API endpoint documentation
- Code examples
- Testing procedures

## 🔐 Data Privacy

✅ All data stored locally on your machine  
✅ No internet connections by default  
✅ No telemetry (disabled by default)  
✅ No accounts required  
✅ Complete user control  

## 📱 Build an Installer

Create a standalone executable for distribution:

```powershell
cd desktop
npm run dist
```

Creates:
- **Windows**: `.exe` and portable `.exe` installers
- **macOS**: `.dmg` installer
- **Linux**: `.AppImage` file

## 🆘 Troubleshooting

### App Won't Start?
1. Check Node.js is installed: `node --version`
2. Install dependencies: `npm install`
3. Check port 3000 is available
4. Check `npm start` output for errors

### Can't Find My Data?
1. Check directory: `~/.blackflag/`
2. Windows users: Use `C:\Users\YourName\.blackflag\`
3. If missing, app will recreate on next launch

### Need to Reset?
1. Stop the app
2. Delete `~/.blackflag/` directory
3. Restart app - fresh database created

### Want to Restore Old Data?
1. Backups stored in: `~/.blackflag/backups/`
2. Each backup is a timestamp folder
3. Manual restore: Copy backup files over current data files
4. Or use File > Database > Restore menu (when implemented)

## 📚 Documentation

**For Users**: Read `DESKTOP_STANDALONE_GUIDE.md`
- Features, installation, API reference, troubleshooting

**For Developers**: Read `DESKTOP_DEVELOPER_GUIDE.md`
- Architecture, code examples, extension guide, deployment

**Implementation Details**: Read `DESKTOP_STANDALONE_IMPLEMENTATION.md`
- Complete technical overview of changes made

## 🔗 API Endpoints (All Local)

```
Health Check:     GET  http://localhost:3000/api/health
Status:           GET  http://localhost:3000/api/status
Vehicles:         GET  http://localhost:3000/api/vehicles/list
Tunes:            GET  http://localhost:3000/api/tunes/:vehicleId
Profiles:         GET  http://localhost:3000/api/ecu-profiles/:vehicleId
Settings:         GET  http://localhost:3000/api/settings
Backup:           POST http://localhost:3000/api/backup
Restore:          POST http://localhost:3000/api/restore/:timestamp
Logs:             GET  http://localhost:3000/api/logs
```

## 🎯 Common Tasks

### Add a New Vehicle
1. Open app
2. Select manufacturer from dropdown
3. Choose vehicle
4. Click "ADD TO HISTORY"
5. Data automatically saved locally

### Create a Backup
1. File > Create Backup
2. Backup created with timestamp
3. Stored in `~/.blackflag/backups/`
4. Automatic backups created hourly (configurable)

### View System Logs
1. Tools > Database Info (shows storage location)
2. Or open `~/.blackflag/logs.json` in text editor
3. Or check console output (F12 > Console tab)

### Change Theme
1. Click theme selector (top right)
2. Choose from: Cyberpunk, Commodore 64, Ford FJDS, Witech
3. Changes saved automatically

### Configure Settings
1. Open `~/.blackflag/settings.json`
2. Edit settings (theme, backup interval, etc.)
3. Changes take effect on next app restart

## 🚀 What's Next?

1. **Customize Your Vehicle List**
   - Edit `vehicles.json` to add your vehicles
   - Include specifications you care about

2. **Create ECU Profiles**
   - Use the UI to create custom profiles
   - Save your tuning strategies

3. **Organize Tunes**
   - Build a library of modifications
   - Tag and categorize your tunes

4. **Regular Backups**
   - Use File > Create Backup weekly
   - Test restore functionality occasionally

5. **Share Configurations**
   - Export `vehicles.json` to share vehicle specs
   - Share `tunes.json` for collaboration

## 💡 Pro Tips

**Tip 1**: Backups are stored as separate folders with timestamps. You can have multiple backups.

**Tip 2**: JSON files are human-readable. You can edit them directly if needed.

**Tip 3**: All API endpoints respond on `localhost:3000`. You can test them with curl or Postman.

**Tip 4**: Enable DevTools (F12) to monitor all API calls and debug issues.

**Tip 5**: Database auto-initializes on startup. No manual setup needed!

## 📞 Support

### Common Issues
- **Port 3000 in use**: Restart app or kill process using port 3000
- **Permissions error**: Check folder permissions in `~/.blackflag/`
- **Data not saving**: Check filesystem space and folder write access
- **UI not loading**: Check console (F12) for errors

### Debug Mode
Set in `~/.blackflag/settings.json`:
```json
{
  "debugMode": true,
  "checkUpdates": false,
  "telemetry": false
}
```

Then check console output and logs for detailed messages.

---

**Version**: BlackFlag v2.0 - Desktop Standalone  
**Status**: Ready to Use  
**Last Updated**: December 30, 2024

Start now: `npm start`
