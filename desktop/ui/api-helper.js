// BlackFlag Desktop - API Helper for Local Database
// Provides abstraction layer for API calls to local database backend

const API_URL = 'http://localhost:3000/api';

// Vehicle Management
async function fetchVehicles() {
    try {
        const response = await fetch(`${API_URL}/vehicles/list`);
        if (!response.ok) throw new Error('Failed to fetch vehicles');
        const data = await response.json();
        return data.vehicles || [];
    } catch (error) {
        console.error('Error fetching vehicles:', error);
        return [];
    }
}

async function getVehicleById(vehicleId) {
    try {
        const response = await fetch(`${API_URL}/vehicles/${vehicleId}`);
        if (!response.ok) throw new Error('Vehicle not found');
        const data = await response.json();
        return data.vehicle;
    } catch (error) {
        console.error('Error fetching vehicle:', error);
        return null;
    }
}

async function fetchManufacturers() {
    try {
        const response = await fetch(`${API_URL}/manufacturers`);
        if (!response.ok) throw new Error('Failed to fetch manufacturers');
        const data = await response.json();
        return data.manufacturers || [];
    } catch (error) {
        console.error('Error fetching manufacturers:', error);
        return [];
    }
}

async function fetchVehiclesByManufacturer(manufacturer) {
    try {
        const response = await fetch(`${API_URL}/manufacturers/${manufacturer}/vehicles`);
        if (!response.ok) throw new Error('Failed to fetch vehicles');
        const data = await response.json();
        return data.vehicles || [];
    } catch (error) {
        console.error('Error fetching vehicles:', error);
        return [];
    }
}

// ECU Profiles
async function fetchECUProfiles(vehicleId) {
    try {
        const response = await fetch(`${API_URL}/ecu-profiles/${vehicleId}`);
        if (!response.ok) throw new Error('Failed to fetch profiles');
        const data = await response.json();
        return data.profiles || [];
    } catch (error) {
        console.error('Error fetching ECU profiles:', error);
        return [];
    }
}

async function saveECUProfile(vehicleId, profile) {
    try {
        const response = await fetch(`${API_URL}/ecu-profiles/${vehicleId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(profile)
        });
        if (!response.ok) throw new Error('Failed to save profile');
        const data = await response.json();
        return data.profileId;
    } catch (error) {
        console.error('Error saving ECU profile:', error);
        return null;
    }
}

// Tunes
async function fetchTunes(vehicleId) {
    try {
        const response = await fetch(`${API_URL}/tunes/${vehicleId}`);
        if (!response.ok) throw new Error('Failed to fetch tunes');
        const data = await response.json();
        return data.tunes || [];
    } catch (error) {
        console.error('Error fetching tunes:', error);
        return [];
    }
}

async function saveTune(vehicleId, tune) {
    try {
        const response = await fetch(`${API_URL}/tunes/${vehicleId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(tune)
        });
        if (!response.ok) throw new Error('Failed to save tune');
        const data = await response.json();
        return data.tuneId;
    } catch (error) {
        console.error('Error saving tune:', error);
        return null;
    }
}

async function getTuneById(vehicleId, tuneId) {
    try {
        const response = await fetch(`${API_URL}/tunes/${vehicleId}/${tuneId}`);
        if (!response.ok) throw new Error('Tune not found');
        const data = await response.json();
        return data.tune;
    } catch (error) {
        console.error('Error fetching tune:', error);
        return null;
    }
}

// Settings
async function fetchSettings() {
    try {
        const response = await fetch(`${API_URL}/settings`);
        if (!response.ok) throw new Error('Failed to fetch settings');
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching settings:', error);
        return {};
    }
}

async function getSetting(key) {
    try {
        const response = await fetch(`${API_URL}/settings/${key}`);
        if (!response.ok) throw new Error('Setting not found');
        const data = await response.json();
        return data.value;
    } catch (error) {
        console.error('Error fetching setting:', error);
        return null;
    }
}

async function saveSetting(key, value) {
    try {
        const response = await fetch(`${API_URL}/settings/${key}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ value })
        });
        if (!response.ok) throw new Error('Failed to save setting');
        return true;
    } catch (error) {
        console.error('Error saving setting:', error);
        return false;
    }
}

// Logs
async function fetchLogs(limit = 100) {
    try {
        const response = await fetch(`${API_URL}/logs?limit=${limit}`);
        if (!response.ok) throw new Error('Failed to fetch logs');
        const data = await response.json();
        return data.logs || [];
    } catch (error) {
        console.error('Error fetching logs:', error);
        return [];
    }
}

// Backup & Restore
async function createBackup() {
    try {
        const response = await fetch(`${API_URL}/backup`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
        if (!response.ok) throw new Error('Failed to create backup');
        const data = await response.json();
        return data.timestamp;
    } catch (error) {
        console.error('Error creating backup:', error);
        return null;
    }
}

async function fetchBackups() {
    try {
        const response = await fetch(`${API_URL}/backups`);
        if (!response.ok) throw new Error('Failed to fetch backups');
        const data = await response.json();
        return data.backups || [];
    } catch (error) {
        console.error('Error fetching backups:', error);
        return [];
    }
}

async function restoreBackup(timestamp) {
    try {
        const response = await fetch(`${API_URL}/restore/${timestamp}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
        if (!response.ok) throw new Error('Failed to restore backup');
        return true;
    } catch (error) {
        console.error('Error restoring backup:', error);
        return false;
    }
}

// System Status
async function fetchSystemStatus() {
    try {
        const response = await fetch(`${API_URL}/status`);
        if (!response.ok) throw new Error('Failed to fetch status');
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching system status:', error);
        return null;
    }
}

// Export all functions
if (typeof window !== 'undefined') {
    window.BlackFlagAPI = {
        // Vehicles
        fetchVehicles,
        getVehicleById,
        fetchManufacturers,
        fetchVehiclesByManufacturer,
        
        // ECU Profiles
        fetchECUProfiles,
        saveECUProfile,
        
        // Tunes
        fetchTunes,
        saveTune,
        getTuneById,
        
        // Settings
        fetchSettings,
        getSetting,
        saveSetting,
        
        // Logs
        fetchLogs,
        
        // Backup
        createBackup,
        fetchBackups,
        restoreBackup,
        
        // System
        fetchSystemStatus,
        API_URL
    };
    console.log('✅ BlackFlag API Helper Loaded');
}

// For Node.js environments (if needed)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        fetchVehicles,
        getVehicleById,
        fetchManufacturers,
        fetchVehiclesByManufacturer,
        fetchECUProfiles,
        saveECUProfile,
        fetchTunes,
        saveTune,
        getTuneById,
        fetchSettings,
        getSetting,
        saveSetting,
        fetchLogs,
        createBackup,
        fetchBackups,
        restoreBackup,
        fetchSystemStatus,
        API_URL
    };
}
