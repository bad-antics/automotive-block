// BlackFlag Desktop - Preload Script (Security Bridge)
const { contextBridge, ipcRenderer } = require('electron');

// Expose limited API to renderer process
contextBridge.exposeInMainWorld('electron', {
    // App version and info
    getVersion: () => ipcRenderer.invoke('get-app-version'),
    getAppInfo: () => ipcRenderer.invoke('get-app-info'),
    checkBackend: () => ipcRenderer.invoke('check-backend'),
    
    // Database operations
    backup: () => ipcRenderer.invoke('db:backup'),
    getDbInfo: () => ipcRenderer.invoke('db:get-info'),
    getLogs: (limit) => ipcRenderer.invoke('db:get-logs', limit),
    
    // Window control operations
    minimizeWindow: () => ipcRenderer.invoke('window:minimize'),
    toggleMaximize: () => ipcRenderer.invoke('window:maximize'),
    closeWindow: () => ipcRenderer.invoke('window:close'),
    
    // File operations
    readFile: (filePath) => ipcRenderer.invoke('read-file', filePath),
    writeFile: (filePath, data) => ipcRenderer.invoke('write-file', filePath, data),
    
    // Settings
    getSetting: (key) => ipcRenderer.invoke('get-setting', key),
    setSetting: (key, value) => ipcRenderer.invoke('set-setting', key, value),
    
    // Hardware (when available)
    scanHardware: () => ipcRenderer.invoke('scan-hardware'),
    
    // Logging
    log: (message) => ipcRenderer.send('log', message),
    error: (message) => ipcRenderer.send('error', message)
});

// Prevent navigation to external URLs
window.addEventListener('will-navigate', (event) => {
    const url = new URL(event.url);
    if (url.hostname !== 'localhost' && url.hostname !== '127.0.0.1') {
        event.preventDefault();
    }
});

console.log('✅ BlackFlag Desktop Preload Script Loaded');
