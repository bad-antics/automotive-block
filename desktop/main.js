// BlackFlag v2.0 Desktop - Electron Main Process (Standalone)
const { app, BrowserWindow, Menu, ipcMain, dialog } = require('electron');
const path = require('path');
const isDev = process.argv.includes('--dev') || process.env.NODE_ENV === 'development';
const express = require('express');
const fs = require('fs');
const DesktopDatabase = require('./db');

let mainWindow;
let server;
let db;
const PORT = 3000;

// Disable GPU acceleration to avoid issues in some environments
app.disableHardwareAcceleration();

// Initialize database
function initDatabase() {
    try {
        db = new DesktopDatabase();
        console.log('💾 Database initialized successfully');
        return true;
    } catch (error) {
        console.error('Failed to initialize database:', error);
        return false;
    }
}

// Create Express app for local backend (database API layer)
const expressApp = express();
expressApp.use(express.json({ limit: '50mb' }));
expressApp.use(express.urlencoded({ extended: true, limit: '50mb' }));

// Serve UI files
expressApp.use(express.static(path.join(__dirname, 'ui')));
expressApp.use(express.static(path.join(__dirname, '../public')));

// CORS headers
expressApp.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Content-Type');
    res.header('Cache-Control', 'no-cache, no-store, must-revalidate');
    next();
});

// ========== API ENDPOINTS - Using Local Database ==========

// Health check endpoint
expressApp.get('/api/health', (req, res) => {
    res.json({ 
        status: 'online',
        version: '2.0.0',
        platform: 'desktop-standalone',
        database: 'local',
        timestamp: new Date().toISOString()
    });
});

// Status endpoint
expressApp.get('/api/status', (req, res) => {
    const dbInfo = db.getDataDirectoryInfo();
    res.json({
        status: 'online',
        version: '2.0.0',
        name: 'BlackFlag',
        tagline: 'Professional ECU Hacking & Tuning Suite',
        free: true,
        openSource: true,
        accountsRequired: false,
        platform: 'desktop-standalone',
        database: {
            path: dbInfo.path,
            size: dbInfo.size,
            backups: dbInfo.backups.length
        },
        modules: [
            'OBD-II Protocol',
            'CAN Bus Communication',
            'J2534 Pass-Through',
            'ECU Processor Controller',
            'Wiring Diagram Library',
            'Tune Manager',
            'ECU Cloning & Backup',
            'Module Installer',
            'OEM As-Built Data',
            'Emissions Controller'
        ]
    });
});

// Vehicle database endpoint - uses local database
expressApp.get('/api/vehicles/list', (req, res) => {
    try {
        const vehicles = db.getVehicles();
        res.json({
            totalVehicles: vehicles.length,
            vehicles: vehicles
        });
    } catch (error) {
        console.error('Error loading vehicles:', error);
        res.json({ totalVehicles: 0, vehicles: [], error: error.message });
    }
});

// Get single vehicle
expressApp.get('/api/vehicles/:vehicleId', (req, res) => {
    try {
        const vehicle = db.getVehicleById(req.params.vehicleId);
        if (vehicle) {
            res.json({ success: true, vehicle });
        } else {
            res.status(404).json({ success: false, error: 'Vehicle not found' });
        }
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});

// Get manufacturers list
expressApp.get('/api/manufacturers', (req, res) => {
    try {
        const manufacturers = db.getManufacturers();
        res.json({ manufacturers });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Get vehicles by manufacturer
expressApp.get('/api/manufacturers/:manufacturer/vehicles', (req, res) => {
    try {
        const vehicles = db.getVehiclesByManufacturer(req.params.manufacturer);
        res.json({ vehicles });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// ECU Profiles endpoints
expressApp.get('/api/ecu-profiles/:vehicleId', (req, res) => {
    try {
        const profiles = db.getECUProfiles(req.params.vehicleId);
        res.json({ profiles });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

expressApp.post('/api/ecu-profiles/:vehicleId', (req, res) => {
    try {
        const profileId = db.addECUProfile(req.params.vehicleId, req.body);
        res.json({ success: true, profileId });
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});

// Tunes endpoints
expressApp.get('/api/tunes/:vehicleId', (req, res) => {
    try {
        const tunes = db.getTunes(req.params.vehicleId);
        res.json({ tunes });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

expressApp.post('/api/tunes/:vehicleId', (req, res) => {
    try {
        const tuneId = db.saveTune(req.params.vehicleId, req.body);
        res.json({ success: true, tuneId });
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});

expressApp.get('/api/tunes/:vehicleId/:tuneId', (req, res) => {
    try {
        const tune = db.getTuneById(req.params.vehicleId, req.params.tuneId);
        if (tune) {
            res.json({ success: true, tune });
        } else {
            res.status(404).json({ success: false, error: 'Tune not found' });
        }
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});

// Settings endpoints
expressApp.get('/api/settings', (req, res) => {
    try {
        const settings = db.getAllSettings();
        res.json(settings);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

expressApp.get('/api/settings/:key', (req, res) => {
    try {
        const value = db.getSetting(req.params.key);
        res.json({ key: req.params.key, value });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

expressApp.post('/api/settings/:key', (req, res) => {
    try {
        db.setSetting(req.params.key, req.body.value);
        res.json({ success: true });
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});

// Logs endpoints
expressApp.get('/api/logs', (req, res) => {
    try {
        const limit = parseInt(req.query.limit) || 100;
        const logs = db.getLogs(limit);
        res.json({ logs });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Backup endpoints
expressApp.post('/api/backup', (req, res) => {
    try {
        const timestamp = db.createBackup();
        if (timestamp) {
            res.json({ success: true, timestamp });
        } else {
            res.status(500).json({ success: false, error: 'Backup creation failed' });
        }
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});

expressApp.get('/api/backups', (req, res) => {
    try {
        const dbInfo = db.getDataDirectoryInfo();
        res.json({ backups: dbInfo.backups });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

expressApp.post('/api/restore/:backupTimestamp', (req, res) => {
    try {
        const success = db.restoreBackup(req.params.backupTimestamp);
        if (success) {
            res.json({ success: true });
        } else {
            res.status(500).json({ success: false, error: 'Restore failed' });
        }
    } catch (error) {
        res.status(500).json({ success: false, error: error.message });
    }
});

// Create Electron window
function createWindow() {
    mainWindow = new BrowserWindow({
        width: 1400,
        height: 900,
        minWidth: 1000,
        minHeight: 700,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            contextIsolation: true,
            enableRemoteModule: false,
            sandbox: true,
            nodeIntegration: false
        },
        icon: path.join(__dirname, '../assets/icon.png')
    });

    // Load URL
    const startUrl = `http://localhost:${PORT}`;
    mainWindow.loadURL(startUrl);

    // Open DevTools in development
    if (isDev) {
        mainWindow.webContents.openDevTools();
    }

    mainWindow.on('closed', () => {
        mainWindow = null;
    });

    // Handle any uncaught exceptions
    mainWindow.webContents.on('crashed', () => {
        console.error('Renderer process crashed');
        mainWindow.reload();
    });
}

// Start Express server
function startServer() {
    return new Promise((resolve, reject) => {
        server = expressApp.listen(PORT, 'localhost', () => {
            console.log(`🚀 BlackFlag Desktop Standalone Backend Running on http://localhost:${PORT}`);
            console.log(`💾 Database Location: ${db.dataDir}`);
            resolve();
        }).on('error', reject);
    });
}

// Display startup banner
function showStartupBanner() {
    const banner = `
╔═══════════════════════════════════════════════════╗
║  🏴 BLACKFLAG v2.0 - DESKTOP STANDALONE         ║
║  Professional ECU Hacking & Tuning Suite        ║
║                                                   ║
║  ✓ Local Database (No External Server)          ║
║  ✓ ECU Processor & Tune Management              ║
║  ✓ Wiring Diagrams & Schematics                 ║
║  ✓ OBD-II & CAN Bus Support                     ║
║  ✓ ECU Cloning & Backup                         ║
║  ✓ Free & Open Source                           ║
║                                                   ║
║  Database: ${db.dataDir.substring(0, 30).padEnd(30)} ║
╚═══════════════════════════════════════════════════╝
`;
    console.log(banner);
}

// App event handlers
app.on('ready', async () => {
    try {
        // Initialize database first
        if (!initDatabase()) {
            throw new Error('Database initialization failed');
        }
        
        showStartupBanner();
        
        // Start express server
        await startServer();
        
        // Create window
        createWindow();
        createMenu();
    } catch (error) {
        console.error('Failed to start application:', error);
        dialog.showErrorBox('BlackFlag Error', `Failed to start application: ${error.message}`);
        app.quit();
    }
});

app.on('window-all-closed', () => {
    // Cleanup
    if (server) server.close();
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('activate', () => {
    if (mainWindow === null) {
        createWindow();
    }
});

// Create application menu
function createMenu() {
    const template = [
        {
            label: 'File',
            submenu: [
                {
                    label: 'Create Backup',
                    accelerator: 'CmdOrCtrl+B',
                    click: () => {
                        const timestamp = db.createBackup();
                        if (timestamp) {
                            dialog.showMessageBox(mainWindow, {
                                type: 'info',
                                title: 'Backup Created',
                                message: `Database backup created successfully at ${timestamp}`
                            });
                        }
                    }
                },
                { type: 'separator' },
                {
                    label: 'Exit',
                    accelerator: 'CmdOrCtrl+Q',
                    click: () => {
                        app.quit();
                    }
                }
            ]
        },
        {
            label: 'Edit',
            submenu: [
                { role: 'undo' },
                { role: 'redo' },
                { type: 'separator' },
                { role: 'cut' },
                { role: 'copy' },
                { role: 'paste' }
            ]
        },
        {
            label: 'View',
            submenu: [
                { role: 'reload' },
                { role: 'forceReload' },
                { role: 'toggleDevTools' },
                { type: 'separator' },
                { role: 'resetZoom' },
                { role: 'zoomIn' },
                { role: 'zoomOut' },
                { type: 'separator' },
                { role: 'togglefullscreen' }
            ]
        },
        {
            label: 'Tools',
            submenu: [
                {
                    label: 'Database Info',
                    click: () => {
                        const dbInfo = db.getDataDirectoryInfo();
                        dialog.showMessageBox(mainWindow, {
                            type: 'info',
                            title: 'Database Information',
                            message: `BlackFlag Database`,
                            detail: `Location: ${dbInfo.path}\n\nSize: ${(dbInfo.size / 1024).toFixed(2)} KB\nBackups: ${dbInfo.backups.length}`
                        });
                    }
                }
            ]
        },
        {
            label: 'Help',
            submenu: [
                {
                    label: 'About',
                    click: () => {
                        dialog.showMessageBox(mainWindow, {
                            type: 'info',
                            title: 'About BlackFlag',
                            message: 'BlackFlag v2.0 - Desktop Standalone',
                            detail: 'Professional ECU Hacking & Tuning Suite\n\nDesktop Version with Local Database\nBuilt with Electron\n\nFree & Open Source'
                        });
                    }
                }
            ]
        }
    ];

    const menu = Menu.buildFromTemplate(template);
    Menu.setApplicationMenu(menu);
}

// ========== IPC HANDLERS ==========

// App info
ipcMain.handle('get-app-version', () => {
    return '2.0.0';
});

ipcMain.handle('get-app-info', () => {
    return {
        name: 'BlackFlag ECU Suite',
        version: '2.0.0',
        platform: process.platform,
        arch: process.arch,
        nodeVersion: process.version,
        mode: 'standalone-desktop',
        databasePath: db.dataDir
    };
});

ipcMain.handle('check-backend', async () => {
    return {
        status: 'online',
        url: `http://localhost:${PORT}`,
        ready: true,
        database: 'local'
    };
});

// Database operations
ipcMain.handle('db:backup', async () => {
    return db.createBackup();
});

ipcMain.handle('db:get-info', async () => {
    return db.getDataDirectoryInfo();
});

ipcMain.handle('db:get-logs', async (event, limit) => {
    return db.getLogs(limit || 100);
});

// Window control operations
ipcMain.handle('window:minimize', () => {
    if (mainWindow) {
        mainWindow.minimize();
    }
    return true;
});

ipcMain.handle('window:maximize', () => {
    if (mainWindow) {
        if (mainWindow.isMaximized()) {
            mainWindow.unmaximize();
        } else {
            mainWindow.maximize();
        }
    }
    return true;
});

ipcMain.handle('window:close', () => {
    if (mainWindow) {
        mainWindow.close();
    }
    return true;
});

// Handle any electron app errors
process.on('uncaughtException', (error) => {
    console.error('Uncaught Exception:', error);
    db.addLog({
        type: 'error',
        message: error.message,
        level: 'error'
    });
});
