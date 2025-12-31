// BlackFlag Desktop - Integrated Local Database
// Provides standalone database without external dependencies
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');

class DesktopDatabase {
    constructor(dataDir) {
        this.dataDir = dataDir || path.join(require('os').homedir(), '.blackflag');
        this.vehiclesFile = path.join(this.dataDir, 'vehicles.json');
        this.ecuProfilesFile = path.join(this.dataDir, 'ecu-profiles.json');
        this.tunesFile = path.join(this.dataDir, 'tunes.json');
        this.settingsFile = path.join(this.dataDir, 'settings.json');
        this.logsFile = path.join(this.dataDir, 'logs.json');
        
        this.initializeDatabase();
    }

    initializeDatabase() {
        // Create data directory if it doesn't exist
        if (!fs.existsSync(this.dataDir)) {
            fs.mkdirSync(this.dataDir, { recursive: true });
            console.log(`📁 Created BlackFlag data directory: ${this.dataDir}`);
        }

        // Initialize default data files if they don't exist
        if (!fs.existsSync(this.vehiclesFile)) {
            this.saveData(this.vehiclesFile, { vehicles: this.getDefaultVehicles() });
        }
        if (!fs.existsSync(this.ecuProfilesFile)) {
            this.saveData(this.ecuProfilesFile, { profiles: {} });
        }
        if (!fs.existsSync(this.tunesFile)) {
            this.saveData(this.tunesFile, { tunes: {} });
        }
        if (!fs.existsSync(this.settingsFile)) {
            this.saveData(this.settingsFile, this.getDefaultSettings());
        }
        if (!fs.existsSync(this.logsFile)) {
            this.saveData(this.logsFile, { logs: [] });
        }

        console.log('✅ Desktop database initialized');
    }

    getDefaultSettings() {
        return {
            theme: 'cyberpunk',
            language: 'en',
            autoBackup: true,
            backupInterval: 3600000, // 1 hour
            maxBackups: 10,
            debugMode: false,
            checkUpdates: true,
            telemetry: false,
            defaultVehicleIndex: 0
        };
    }

    getDefaultVehicles() {
        return [
            {
                id: 'ford_mustang_2015',
                manufacturer: 'Ford',
                make: 'Ford',
                model: 'Mustang',
                generation: '6th Gen',
                year: 2015,
                engines: ['3.7L V6', '5.0L V8', '2.3L EcoBoost'],
                engine: '5.0L V8',
                transmissions: ['6-speed Manual', '6-speed Automatic'],
                ecuTypes: ['Bosch ECU', 'Delphi DCM'],
                driveType: 'RWD',
                fuelType: 'Gasoline',
                displacement: '2300-5000cc',
                power: '223-435 kW',
                torque: '347-542 Nm',
                wiring: 'CAN-FD',
                systems: ['Adaptive Steering', 'Launch Control', 'Select Shift', 'SYNC 3'],
                vin: '1FA6P8*'
            },
            {
                id: 'ford_f150_2017',
                manufacturer: 'Ford',
                make: 'Ford',
                model: 'F-150',
                generation: '13th Gen',
                year: 2017,
                engines: ['3.5L EcoBoost', '5.0L V8', '3.3L V6', '2.7L EcoBoost'],
                engine: '5.0L V8',
                transmissions: ['10-speed Automatic'],
                ecuTypes: ['Bosch ECU', 'Ford Powertrain'],
                driveType: 'RWD/4WD',
                fuelType: 'Gasoline',
                displacement: '3300-5000cc',
                power: '283-400 kW',
                torque: '542-678 Nm',
                wiring: 'CAN-FD',
                systems: ['4WD', 'Lane Keep Assist', 'Adaptive Cruise', 'SYNC 3', 'Trailer Backup Guide'],
                vin: '1FTFW1*'
            },
            {
                id: 'chevrolet_corvette_2014',
                manufacturer: 'Chevrolet',
                make: 'Chevrolet',
                model: 'Corvette',
                generation: 'C7',
                year: 2014,
                engines: ['6.2L V8 LS7'],
                engine: '6.2L V8 LS7',
                transmissions: ['7-speed Manual', '8-speed Automatic'],
                ecuTypes: ['GM ECU', 'Bosch DDM6.2'],
                driveType: 'RWD',
                fuelType: 'Gasoline',
                displacement: '6200cc',
                power: '460 kW',
                torque: '637 Nm',
                wiring: 'CAN-FD',
                systems: ['Magnetic Ride Control', 'Active Rev Match', 'Launch Control', 'Performance Traction Management'],
                vin: '1G1YY*'
            },
            {
                id: 'dodge_challenger_2015',
                manufacturer: 'Dodge',
                make: 'Dodge',
                model: 'Challenger',
                generation: '3rd Gen',
                year: 2015,
                engines: ['3.6L Pentastar V6', '5.7L V8 HEMI', '6.4L V8 SRT'],
                engine: '5.7L V8 HEMI',
                transmissions: ['8-speed Automatic', '6-speed Manual'],
                ecuTypes: ['Bosch ECU', 'Chrysler TCM'],
                driveType: 'RWD',
                fuelType: 'Gasoline',
                displacement: '3600-6400cc',
                power: '257-485 kW',
                torque: '470-645 Nm',
                wiring: 'CAN-FD',
                systems: ['Line Lock', 'Launch Control', 'Drag Launch', 'Performance Pages'],
                vin: '2B3CJ5DV*'
            },
            {
                id: 'bmw_m3_2014',
                manufacturer: 'BMW',
                make: 'BMW',
                model: 'M3',
                generation: 'F80',
                year: 2014,
                engines: ['3.0L Twin-Turbo S55'],
                engine: '3.0L Twin-Turbo S55',
                transmissions: ['7-speed Dual-Clutch'],
                ecuTypes: ['Bosch Motorsport ECU', 'ZF Getrag Transmission'],
                driveType: 'RWD/AWD',
                fuelType: 'Gasoline',
                displacement: '2979cc',
                power: '431 kW',
                torque: '550 Nm',
                wiring: 'CAN-FD / K-Line',
                systems: ['M-Adaptive Suspension', 'M DCT Transmission', 'Active M Differential', 'Launch Control'],
                vin: 'WBSFR9*'
            }
        ];
    }

    // File operations
    saveData(filePath, data) {
        try {
            fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf8');
            return true;
        } catch (error) {
            console.error(`Error saving data to ${filePath}:`, error);
            return false;
        }
    }

    loadData(filePath) {
        try {
            if (!fs.existsSync(filePath)) {
                return null;
            }
            const data = fs.readFileSync(filePath, 'utf8');
            return JSON.parse(data);
        } catch (error) {
            console.error(`Error loading data from ${filePath}:`, error);
            return null;
        }
    }

    // Vehicle operations
    getVehicles() {
        const data = this.loadData(this.vehiclesFile);
        return data ? data.vehicles || [] : [];
    }

    getVehicleById(vehicleId) {
        const vehicles = this.getVehicles();
        return vehicles.find(v => v.id === vehicleId);
    }

    getVehiclesByManufacturer(manufacturer) {
        const vehicles = this.getVehicles();
        return vehicles.filter(v => (v.manufacturer || v.make) === manufacturer);
    }

    getManufacturers() {
        const vehicles = this.getVehicles();
        const makers = [...new Set(vehicles.map(v => v.manufacturer || v.make))];
        return makers.sort();
    }

    addVehicle(vehicleData) {
        const vehicles = this.getVehicles();
        vehicleData.id = vehicleData.id || crypto.randomBytes(8).toString('hex');
        vehicles.push(vehicleData);
        this.saveData(this.vehiclesFile, { vehicles });
        return vehicleData.id;
    }

    updateVehicle(vehicleId, updates) {
        const vehicles = this.getVehicles();
        const index = vehicles.findIndex(v => v.id === vehicleId);
        if (index !== -1) {
            vehicles[index] = { ...vehicles[index], ...updates };
            this.saveData(this.vehiclesFile, { vehicles });
            return true;
        }
        return false;
    }

    // ECU Profile operations
    getECUProfiles(vehicleId) {
        const data = this.loadData(this.ecuProfilesFile);
        if (!data || !data.profiles) return [];
        return data.profiles[vehicleId] || [];
    }

    addECUProfile(vehicleId, profile) {
        const data = this.loadData(this.ecuProfilesFile);
        if (!data.profiles[vehicleId]) {
            data.profiles[vehicleId] = [];
        }
        profile.id = profile.id || crypto.randomBytes(8).toString('hex');
        profile.createdAt = new Date().toISOString();
        data.profiles[vehicleId].push(profile);
        this.saveData(this.ecuProfilesFile, data);
        return profile.id;
    }

    // Tune operations
    getTunes(vehicleId) {
        const data = this.loadData(this.tunesFile);
        if (!data || !data.tunes) return [];
        return data.tunes[vehicleId] || [];
    }

    saveTune(vehicleId, tuneData) {
        const data = this.loadData(this.tunesFile);
        if (!data.tunes[vehicleId]) {
            data.tunes[vehicleId] = [];
        }
        tuneData.id = tuneData.id || crypto.randomBytes(8).toString('hex');
        tuneData.savedAt = new Date().toISOString();
        data.tunes[vehicleId].push(tuneData);
        this.saveData(this.tunesFile, data);
        return tuneData.id;
    }

    getTuneById(vehicleId, tuneId) {
        const tunes = this.getTunes(vehicleId);
        return tunes.find(t => t.id === tuneId);
    }

    // Settings operations
    getSetting(key) {
        const data = this.loadData(this.settingsFile);
        return data ? data[key] : null;
    }

    setSetting(key, value) {
        const data = this.loadData(this.settingsFile);
        data[key] = value;
        this.saveData(this.settingsFile, data);
        return true;
    }

    getAllSettings() {
        return this.loadData(this.settingsFile) || this.getDefaultSettings();
    }

    // Logging operations
    addLog(logEntry) {
        const data = this.loadData(this.logsFile);
        if (!data.logs) data.logs = [];
        logEntry.timestamp = new Date().toISOString();
        logEntry.id = crypto.randomBytes(8).toString('hex');
        data.logs.push(logEntry);
        
        // Keep only last 1000 logs to prevent file from getting too large
        if (data.logs.length > 1000) {
            data.logs = data.logs.slice(-1000);
        }
        
        this.saveData(this.logsFile, data);
        return logEntry.id;
    }

    getLogs(limit = 100) {
        const data = this.loadData(this.logsFile);
        if (!data || !data.logs) return [];
        return data.logs.slice(-limit).reverse();
    }

    // Backup operations
    createBackup() {
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
        const backupDir = path.join(this.dataDir, 'backups', timestamp);
        
        try {
            fs.mkdirSync(backupDir, { recursive: true });
            
            // Copy all data files
            fs.copyFileSync(this.vehiclesFile, path.join(backupDir, 'vehicles.json'));
            fs.copyFileSync(this.ecuProfilesFile, path.join(backupDir, 'ecu-profiles.json'));
            fs.copyFileSync(this.tunesFile, path.join(backupDir, 'tunes.json'));
            fs.copyFileSync(this.settingsFile, path.join(backupDir, 'settings.json'));
            
            this.addLog({
                type: 'backup',
                message: `Backup created at ${timestamp}`,
                level: 'info'
            });
            
            return timestamp;
        } catch (error) {
            console.error('Backup creation failed:', error);
            this.addLog({
                type: 'error',
                message: `Backup creation failed: ${error.message}`,
                level: 'error'
            });
            return null;
        }
    }

    // Utility
    getDataDirectoryInfo() {
        return {
            path: this.dataDir,
            exists: fs.existsSync(this.dataDir),
            size: this.getDirectorySize(this.dataDir),
            backups: this.getBackupsList()
        };
    }

    getDirectorySize(dir) {
        try {
            const files = fs.readdirSync(dir, { recursive: true });
            let size = 0;
            files.forEach(file => {
                const filePath = path.join(dir, file);
                if (fs.statSync(filePath).isFile()) {
                    size += fs.statSync(filePath).size;
                }
            });
            return size;
        } catch (error) {
            return 0;
        }
    }

    getBackupsList() {
        const backupsDir = path.join(this.dataDir, 'backups');
        try {
            if (!fs.existsSync(backupsDir)) return [];
            return fs.readdirSync(backupsDir).sort().reverse();
        } catch (error) {
            return [];
        }
    }

    restoreBackup(backupTimestamp) {
        const backupDir = path.join(this.dataDir, 'backups', backupTimestamp);
        
        try {
            if (!fs.existsSync(backupDir)) {
                throw new Error('Backup not found');
            }
            
            // Restore files
            fs.copyFileSync(
                path.join(backupDir, 'vehicles.json'),
                this.vehiclesFile
            );
            fs.copyFileSync(
                path.join(backupDir, 'ecu-profiles.json'),
                this.ecuProfilesFile
            );
            fs.copyFileSync(
                path.join(backupDir, 'tunes.json'),
                this.tunesFile
            );
            fs.copyFileSync(
                path.join(backupDir, 'settings.json'),
                this.settingsFile
            );
            
            this.addLog({
                type: 'restore',
                message: `Restored from backup ${backupTimestamp}`,
                level: 'info'
            });
            
            return true;
        } catch (error) {
            console.error('Backup restore failed:', error);
            this.addLog({
                type: 'error',
                message: `Backup restore failed: ${error.message}`,
                level: 'error'
            });
            return false;
        }
    }
}

module.exports = DesktopDatabase;
