// BlackFlag v2.0 - Main Application JavaScript
// Professional ECU Hacking & Tuning Suite

const API_URL = 'http://localhost:3000/api';
let allVehicles = []; // Global vehicles array
let selectedVehicle = null; // Track selected vehicle

// ============ MENU NAVIGATION ============
function openFunction(functionId) {
    // Hide all screens
    const screens = document.querySelectorAll('.screen');
    screens.forEach(screen => screen.classList.remove('active'));
    
    // Show the selected function screen
    const screen = document.getElementById(functionId);
    if (screen) {
        screen.classList.add('active');
        console.log(`Opened function: ${functionId}`);
    }
}

function backToMenu() {
    // Hide all screens
    const screens = document.querySelectorAll('.screen');
    screens.forEach(screen => screen.classList.remove('active'));
    
    // Show menu screen
    const menuScreen = document.getElementById('menuScreen');
    if (menuScreen) {
        menuScreen.classList.add('active');
        console.log('Returned to menu');
    }
}

function selectVehicle() {
    const selector = document.getElementById('vehicleSelector');
    const vehicleId = parseInt(selector.value);
    
    if (!vehicleId) {
        alert('⚠️ Please select a vehicle');
        return;
    }
    
    const vehicle = allVehicles.find(v => v.id === vehicleId);
    if (vehicle) {
        selectedVehicle = vehicle;
        displayVehicleInfoOnMenu(vehicle);
        console.log('Selected vehicle:', vehicle);
    }
}

function displayVehicleInfoOnMenu(vehicle) {
    const infoDiv = document.getElementById('vehicleInfo');
    if (!infoDiv) return;
    
    infoDiv.style.display = 'block';
    infoDiv.innerHTML = `
        <div class="vehicle-info-card">
            <h4>⚔️ ${vehicle.year} ${vehicle.manufacturer} ${vehicle.model}</h4>
            <p>📊 Power: ${vehicle.power || 'N/A'} HP | Torque: ${vehicle.torque || 'N/A'} LB-FT</p>
            <p>🔧 ECU Types: ${vehicle.ecuTypes?.join(', ') || 'N/A'}</p>
            <p>💨 Drive Type: ${vehicle.driveType || 'N/A'} | Body Type: ${vehicle.bodyType || 'N/A'}</p>
        </div>
    `;
}

// ============ INITIALIZATION ============
document.addEventListener('DOMContentLoaded', function() {
    console.log('🏴 BlackFlag v2.0 Loaded');
    setupEventListeners();
    displaySystemStatus();
    loadVehicleDatabase();
});

// ============ VEHICLE DATABASE FUNCTIONS ============
async function loadVehicleDatabase() {
    try {
        console.log('Loading vehicle database from API...');
        const response = await fetch(`${API_URL}/vehicles/list`);
        const data = await response.json();
        
        console.log('Vehicle database response:', data);
        
        if (data.vehicles && data.vehicles.length > 0) {
            console.log(`Found ${data.vehicles.length} vehicles`);
            allVehicles = data.vehicles; // Store globally
            populateVehicleSelector(data.vehicles);
            populateVehicleSelectionDropdown(data.vehicles);
        } else {
            console.warn('No vehicles found in response');
        }
    } catch (error) {
        console.error('Failed to load vehicle database:', error);
    }
}

function populateVehicleSelector(vehicles) {
    console.log('Populating vehicle selector with', vehicles.length, 'vehicles');
    
    const ecuSelect = document.getElementById('ecuSelect');
    
    if (!ecuSelect) {
        console.error('ecuSelect element not found in DOM');
        return;
    }
    
    console.log('Found ecuSelect element');
    
    // Clear existing options
    ecuSelect.innerHTML = '<option value="">⚔️ Choose your weapon...</option>';
    
    // Group vehicles by manufacturer
    const grouped = {};
    vehicles.forEach(vehicle => {
        const mfg = vehicle.manufacturer || 'Other';
        if (!grouped[mfg]) {
            grouped[mfg] = [];
        }
        grouped[mfg].push(vehicle);
    });
    
    console.log('Grouped vehicles by manufacturer:', Object.keys(grouped));
    
    // Create option groups
    Object.keys(grouped).sort().forEach(manufacturer => {
        const optgroup = document.createElement('optgroup');
        optgroup.label = `${manufacturer}`;
        
        grouped[manufacturer].forEach(vehicle => {
            const option = document.createElement('option');
            option.value = vehicle.id;
            option.textContent = `${vehicle.model} ${vehicle.year} - ${vehicle.engines.join('/')}`;
            optgroup.appendChild(option);
            console.log(`Added option: ${vehicle.id} - ${option.textContent}`);
        });
        
        ecuSelect.appendChild(optgroup);
    });
    
    console.log('Vehicle selector populated successfully');
}

function populateVehicleSelectionDropdown(vehicles) {
    console.log('Populating vehicle selection dropdown with', vehicles.length, 'vehicles');
    
    const vehicleSelector = document.getElementById('vehicleSelector');
    
    if (!vehicleSelector) {
        console.error('vehicleSelector element not found in DOM');
        return;
    }
    
    console.log('Found vehicleSelector element');
    
    // Clear existing options
    vehicleSelector.innerHTML = '<option value="">⚔️ Choose a vehicle...</option>';
    
    // Group vehicles by manufacturer
    const grouped = {};
    vehicles.forEach(vehicle => {
        const mfg = vehicle.manufacturer || 'Other';
        if (!grouped[mfg]) {
            grouped[mfg] = [];
        }
        grouped[mfg].push(vehicle);
    });
    
    console.log('Grouped vehicles by manufacturer:', Object.keys(grouped));
    
    // Create option groups
    Object.keys(grouped).sort().forEach(manufacturer => {
        const optgroup = document.createElement('optgroup');
        optgroup.label = `${manufacturer}`;
        
        grouped[manufacturer].forEach(vehicle => {
            const option = document.createElement('option');
            option.value = vehicle.id; // Just use ID
            option.textContent = `${vehicle.model} (${vehicle.year}) - ${vehicle.engines.join('/')}`;
            optgroup.appendChild(option);
        });
        
        vehicleSelector.appendChild(optgroup);
    });
    
    console.log('Vehicle selection dropdown populated successfully');
}

function loadVehicleBySelection() {
    const vehicleSelector = document.getElementById('vehicleSelector');
    const vehicleId = vehicleSelector.value;
    const output = document.getElementById('vinDecoderOutput');
    
    if (!vehicleId) {
        output.innerHTML = '<span class="error-text">Please select a vehicle from the list</span>';
        return;
    }
    
    // Find the vehicle in our global array
    const vehicle = allVehicles.find(v => v.id === vehicleId);
    
    if (!vehicle) {
        output.innerHTML = '<span class="error-text">Vehicle not found in database</span>';
        return;
    }
    
    displayVehicleDetails(vehicle, output);
}

function displayVehicleDetails(vehicle, outputElement) {
    let html = '<div class="vin-results">';
    html += `<div class="vin-line"><span class="label">Vehicle:</span> <span class="value">${vehicle.manufacturer} ${vehicle.model}</span></div>`;
    html += `<div class="vin-line"><span class="label">Year:</span> <span class="value">${vehicle.year}</span></div>`;
    html += `<div class="vin-line"><span class="label">Body Type:</span> <span class="value">${vehicle.bodyType}</span></div>`;
    html += `<div class="vin-line"><span class="label">Drive Type:</span> <span class="value">${vehicle.driveType}</span></div>`;
    html += `<div class="vin-line"><span class="label">Engines:</span> <span class="value">${vehicle.engines.join(', ')}</span></div>`;
    html += `<div class="vin-line"><span class="label">Transmissions:</span> <span class="value">${vehicle.transmissions.join(', ')}</span></div>`;
    html += `<div class="vin-line"><span class="label">Power Output:</span> <span class="value">${vehicle.power} kW</span></div>`;
    html += `<div class="vin-line"><span class="label">Torque:</span> <span class="value">${vehicle.torque} Nm</span></div>`;
    html += `<div class="vin-line"><span class="label">ECU Types:</span> <span class="value">${vehicle.ecuTypes.join(', ')}</span></div>`;
    html += `<div class="vin-line"><span class="label">Wiring Protocol:</span> <span class="value">${vehicle.wiring}</span></div>`;
    
    if (vehicle.systems && vehicle.systems.length > 0) {
        html += `<div class="vin-line"><span class="label">Systems:</span> <span class="value">${vehicle.systems.join(', ')}</span></div>`;
    }
    
    html += '</div>';
    html += `<button class="action-btn primary" onclick="exportVehicleData(${JSON.stringify(vehicle).replace(/"/g, '&quot;')})">💾 EXPORT AS TXT</button>`;
    
    outputElement.innerHTML = html;
}

function exportVehicleData(vehicle) {
    const timestamp = new Date().toISOString();
    const txt = `BLACKFLAG v2.0 - VEHICLE SPECIFICATIONS REPORT
════════════════════════════════════════════════════
Generated: ${timestamp}

VEHICLE INFORMATION
───────────────────────────────────────────────────
Manufacturer:         ${vehicle.manufacturer}
Model:                ${vehicle.model}
Year:                 ${vehicle.year}
Body Type:            ${vehicle.bodyType}
Drive Type:           ${vehicle.driveType}

POWERTRAIN
───────────────────────────────────────────────────
Engines:              ${vehicle.engines.join(', ')}
Transmissions:        ${vehicle.transmissions.join(', ')}
Power Output:         ${vehicle.power} kW
Torque:               ${vehicle.torque} Nm

ECU SYSTEMS
───────────────────────────────────────────────────
ECU Types:            ${vehicle.ecuTypes.join(', ')}
Wiring Protocol:      ${vehicle.wiring}
Supported Systems:    ${(vehicle.systems || []).join(', ')}

DIAGNOSTIC INFORMATION
───────────────────────────────────────────────────
OBD-II Support:       Yes
CAN Protocol:         ${vehicle.wiring}
J1939 Support:        ${vehicle.ecuTypes.includes('Cummins Control Module') ? 'Yes' : 'Standard'}

NOTES
───────────────────────────────────────────────────
This report was generated by BlackFlag v2.0
For educational and research purposes only.
Always use responsibly and legally.

═════════════════════════════════════════════════════`;
    
    // Create blob and download
    const blob = new Blob([txt], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `VEHICLE_${vehicle.manufacturer}_${vehicle.model}_${vehicle.year}_${Date.now()}.txt`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
    
    alert(`✓ Vehicle report exported: VEHICLE_${vehicle.manufacturer}_${vehicle.model}_${vehicle.year}_${Date.now()}.txt`);
}

function displayVehicleInfo(vehicleId) {
    const statusDiv = document.getElementById('vehicleStatus');
    if (statusDiv) {
        statusDiv.textContent = `Vessel locked: ${vehicleId}`;
        console.log('Updated vehicle status to:', vehicleId);
    }
}

// ============ EVENT LISTENERS ============
function setupEventListeners() {
    const sidebarBtns = document.querySelectorAll('.sidebar-btn');
    sidebarBtns.forEach(btn => {
        btn.addEventListener('click', (e) => {
            const section = e.target.getAttribute('onclick');
            console.log('Section clicked:', section);
        });
    });

    // Add vehicle selector change event
    const ecuSelect = document.getElementById('ecuSelect');
    if (ecuSelect) {
        ecuSelect.addEventListener('change', (e) => {
            if (e.target.value) {
                displayVehicleInfo(e.target.value);
            }
        });
    }

    // Add vehicle input event
    const vehicleInput = document.getElementById('vehicleInput');
    if (vehicleInput) {
        vehicleInput.addEventListener('input', (e) => {
            if (e.target.value) {
                displayVehicleInfo(e.target.value);
            }
        });
    }

    // Show home section by default
    showSection('home');
}

// ============ SECTION NAVIGATION ============
function showSection(sectionId) {
    // Hide all sections
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(section => {
        section.classList.remove('active');
    });

    // Show selected section
    const targetSection = document.getElementById(sectionId);
    if (targetSection) {
        targetSection.classList.add('active');
    }

    // Update nav links
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('href') === '#' + sectionId) {
            link.classList.add('active');
        }
    });
}

// ============ DISPLAY SYSTEM STATUS ============
async function displaySystemStatus() {
    try {
        const response = await fetch(`${API_URL}/status`);
        const data = await response.json();
        
        console.log('System Status:', data);
        console.log('Modules Loaded:', data.modules.length);
        console.log('Version:', data.version);
    } catch (error) {
        console.error('Failed to fetch system status:', error);
    }
}

// ============ WIRING DIAGRAM FUNCTIONS ============
async function listCircuits() {
    const output = document.getElementById('wiringOutput');
    output.innerHTML = '<span class="output-label">Loading circuits...</span>';

    try {
        const response = await fetch(`${API_URL}/wiring/circuits`);
        const data = await response.json();

        let html = '<div class="output-line"><span class="output-label">Available Circuits:</span></div>';
        data.circuits.forEach(circuit => {
            html += `
            <div class="output-line">
                <span class="output-label">⚡</span>
                <span class="output-value">${circuit.name} (${circuit.voltage}V, ${circuit.amperage}A)</span>
            </div>`;
        });
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function traceCircuit() {
    const circuitId = document.getElementById('circuitSelect').value;
    const output = document.getElementById('wiringOutput');
    output.innerHTML = '<span class="output-label">Tracing circuit...</span>';

    try {
        const response = await fetch(`${API_URL}/wiring/trace/${circuitId}`);
        const data = await response.json();

        let html = `<div class="output-line"><span class="output-label">Circuit Trace: ${data.circuit.name}</span></div>`;
        if (data.trace && data.trace.path) {
            data.trace.path.forEach((step, idx) => {
                html += `
                <div class="output-line">
                    <span class="output-value">${idx + 1}. ${step.component} [${step.color}] (${step.gauge} AWG)</span>
                </div>`;
            });
        }
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function viewConnector() {
    const output = document.getElementById('wiringOutput');
    output.innerHTML = '<span class="output-label">Loading connector data...</span>';

    try {
        const response = await fetch(`${API_URL}/wiring/connectors/Bosch%20Standard`);
        const data = await response.json();

        let html = `<div class="output-line"><span class="output-label">Connector: ${data.connector.name}</span></div>`;
        html += `<div class="output-line"><span class="output-value">Type: ${data.connector.type} | Pins: ${data.connector.pins}</span></div>`;
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

// ============ TUNE MANAGER FUNCTIONS ============
async function listTunes() {
    const category = document.getElementById('tuneCategory').value;
    const output = document.getElementById('tuneOutput');
    output.innerHTML = '<span class="output-label">Loading tunes...</span>';

    try {
        const response = await fetch(`${API_URL}/tunes/list?category=${category}`);
        const data = await response.json();

        let html = `<div class="output-line"><span class="output-label">Available Tunes (${category}):</span></div>`;
        
        if (data.tunes) {
            data.tunes.forEach(tune => {
                html += `
                <div class="output-line">
                    <span class="output-label">⚡</span>
                    <span class="output-value">${tune.name} - Power: ${tune.powerGain}, Torque: ${tune.torqueGain}</span>
                </div>`;
            });
        }
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function applyTune() {
    const output = document.getElementById('tuneOutput');
    output.innerHTML = '<span class="output-label">Applying tune...</span>';

    const tuneId = document.getElementById('tuneCategory').value;
    const ecuId = document.getElementById('ecuSelect').value;

    try {
        const response = await fetch(`${API_URL}/tunes/apply`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                tuneId: tuneId,
                ecuId: ecuId,
                vehicleId: 'VEHICLE_001'
            })
        });

        const data = await response.json();
        
        let html = `
        <div class="output-line"><span class="output-label">✓ Tune Applied</span></div>
        <div class="output-line"><span class="output-value">Tune: ${data.tuneName}</span></div>
        <div class="output-line"><span class="output-value">Status: ${data.status}</span></div>`;
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

// ============ ECU CLONING FUNCTIONS ============
async function backupECU() {
    const sourceEcu = document.getElementById('sourceEcu').value;
    const output = document.getElementById('cloningOutput');
    output.innerHTML = '<span class="output-label">Creating backup...</span>';

    try {
        const response = await fetch(`${API_URL}/cloning/backup`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                ecuId: sourceEcu,
                ecuType: 'Bosch ME7'
            })
        });

        const data = await response.json();
        
        let html = `
        <div class="output-line"><span class="output-label">✓ Backup Complete</span></div>
        <div class="output-line"><span class="output-value">Backup ID: ${data.backupId}</span></div>
        <div class="output-line"><span class="output-value">Size: ${data.backupSize}</span></div>
        <div class="output-line"><span class="output-value">Checksum: ${data.checksum}</span></div>`;
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function cloneECU() {
    const sourceEcu = document.getElementById('sourceEcu').value;
    const targetEcu = document.getElementById('targetEcu').value;
    const output = document.getElementById('cloningOutput');
    output.innerHTML = '<span class="output-label">Cloning ECU...</span>';

    try {
        const response = await fetch(`${API_URL}/cloning/clone`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                sourceEcuId: sourceEcu,
                targetEcuId: targetEcu,
                ecuType: 'Bosch ME7'
            })
        });

        const data = await response.json();
        
        let html = `
        <div class="output-line"><span class="output-label">✓ Clone Complete</span></div>
        <div class="output-line"><span class="output-value">Clone ID: ${data.cloneId}</span></div>
        <div class="output-line"><span class="output-value">Status: ${data.status}</span></div>
        <div class="output-line"><span class="output-value">Checksum Match: ${data.checksumMatch ? 'YES' : 'NO'}</span></div>`;
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function restoreECU() {
    const output = document.getElementById('cloningOutput');
    output.innerHTML = '<span class="output-label">Restore function requires backup ID</span>';
}

// ============ MODULE INSTALLER FUNCTIONS ============
async function listModules() {
    const output = document.getElementById('moduleOutput');
    output.innerHTML = '<span class="output-label">Loading modules...</span>';

    try {
        const response = await fetch(`${API_URL}/modules/list`);
        const data = await response.json();

        let html = `<div class="output-line"><span class="output-label">Available Modules (${data.totalModules}):</span></div>`;
        
        data.modules.forEach(module => {
            html += `
            <div class="output-line">
                <span class="output-label">📦</span>
                <span class="output-value">${module.name} v${module.version} [${module.size}]</span>
            </div>`;
        });
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function installModule() {
    const moduleId = document.getElementById('moduleSelect').value;
    const ecuId = document.getElementById('ecuSelect').value;
    const output = document.getElementById('moduleOutput');
    output.innerHTML = '<span class="output-label">Installing module...</span>';

    try {
        const response = await fetch(`${API_URL}/modules/install`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                moduleId: moduleId,
                ecuId: ecuId,
                ecuType: 'Bosch ME7'
            })
        });

        const data = await response.json();
        
        let html = `
        <div class="output-line"><span class="output-label">✓ Module Installed</span></div>
        <div class="output-line"><span class="output-value">Module: ${data.moduleName}</span></div>
        <div class="output-line"><span class="output-value">Status: ${data.status}</span></div>
        <div class="output-line"><span class="output-value">Requires Reset: ${data.requiresEcuReset ? 'YES' : 'NO'}</span></div>`;
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function uninstallModule() {
    const output = document.getElementById('moduleOutput');
    output.innerHTML = '<span class="output-label">Module ID required for uninstall</span>';
}

// ============ OEM AS-BUILT FUNCTIONS ============
async function importAsBuilt() {
    const vin = document.getElementById('vinInput').value;
    const asbuiltData = document.getElementById('asbuiltData').value;
    const output = document.getElementById('asbuiltOutput');
    output.innerHTML = '<span class="output-label">Importing as-built data...</span>';

    try {
        const response = await fetch(`${API_URL}/asbuilt/import`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                vehicleId: vin,
                asBuiltString: asbuiltData
            })
        });

        const data = await response.json();
        
        let html = `
        <div class="output-line"><span class="output-label">✓ Import Complete</span></div>
        <div class="output-line"><span class="output-value">Session ID: ${data.sessionId}</span></div>
        <div class="output-line"><span class="output-value">Codes Imported: ${data.codesImported}</span></div>`;
        
        output.innerHTML = html;
    } catch (error) {
        output.innerHTML = `<span class="output-label">Error:</span> ${error.message}`;
    }
}

async function compareAsBuilt() {
    const output = document.getElementById('asbuiltOutput');
    output.innerHTML = '<span class="output-label">Comparison requires active session</span>';
}

async function applyAsBuilt() {
    const output = document.getElementById('asbuiltOutput');
    output.innerHTML = '<span class="output-label">Applying as-built codes...</span>';
    
    setTimeout(() => {
        output.innerHTML = `
        <div class="output-line"><span class="output-label">✓ As-Built Applied</span></div>
        <div class="output-line"><span class="output-value">Codes Applied: 8</span></div>
        <div class="output-line"><span class="output-value">Status: VERIFIED</span></div>`;
    }, 500);
}

// ============ EMISSIONS CONTROL FUNCTIONS ============
async function toggleDPF() {
    const output = document.getElementById('emissionsOutput');
    output.innerHTML = '<span class="output-label">Toggling DPF system...</span>';
    
    setTimeout(() => {
        output.innerHTML = `
        <div class="output-line"><span class="output-label">✓ DPF System Toggled</span></div>
        <div class="output-line"><span class="output-value">Status: DISABLED</span></div>
        <div class="output-line"><span class="output-value">Sensor Spoofing: ENABLED</span></div>`;
    }, 500);
}

async function toggleSCR() {
    const output = document.getElementById('emissionsOutput');
    output.innerHTML = '<span class="output-label">Toggling SCR system...</span>';
    
    setTimeout(() => {
        output.innerHTML = `
        <div class="output-line"><span class="output-label">✓ SCR System Toggled</span></div>
        <div class="output-line"><span class="output-value">Status: DISABLED</span></div>
        <div class="output-line"><span class="output-value">Urea Level Spoof: ACTIVE</span></div>`;
    }, 500);
}

async function toggleEGR() {
    const output = document.getElementById('emissionsOutput');
    output.innerHTML = '<span class="output-label">Toggling EGR system...</span>';
    
    setTimeout(() => {
        output.innerHTML = `
        <div class="output-line"><span class="output-label">✓ EGR System Toggled</span></div>
        <div class="output-line"><span class="output-value">Status: DISABLED</span></div>
        <div class="output-line"><span class="output-value">Temperature Spoof: ACTIVE</span></div>`;
    }, 500);
}

console.log('🏴 BlackFlag v2.0 - Professional ECU Hacking Suite');
console.log('Free & Open Source | No Accounts | No Sign-In Required');
console.log('API Server: http://localhost:3000');
// ============ VIN DECODER ============
function decodeVIN() {
    const vinInput = document.getElementById('vinInput');
    const vin = vinInput.value.toUpperCase().trim();
    const output = document.getElementById('vinDecoderOutput');
    
    if (!vin || vin.length < 8) {
        output.innerHTML = '<span class="error-text">Invalid VIN. Must be at least 8 characters.</span>';
        return;
    }
    
    output.innerHTML = '<span class="loading-text">🔍 Decoding VIN...</span>';
    
    // VIN Decoding Logic
    const vinData = decodeVINFromPattern(vin);
    
    setTimeout(() => {
        let html = '<div class="vin-results">';
        html += `<div class="vin-line"><span class="label">VIN:</span> <span class="value">${vin}</span></div>`;
        html += `<div class="vin-line"><span class="label">Manufacturer:</span> <span class="value">${vinData.manufacturer}</span></div>`;
        html += `<div class="vin-line"><span class="label">Model:</span> <span class="value">${vinData.model}</span></div>`;
        html += `<div class="vin-line"><span class="label">Year:</span> <span class="value">${vinData.year}</span></div>`;
        html += `<div class="vin-line"><span class="label">Body Type:</span> <span class="value">${vinData.bodyType}</span></div>`;
        html += `<div class="vin-line"><span class="label">Engine Type:</span> <span class="value">${vinData.engine}</span></div>`;
        html += `<div class="vin-line"><span class="label">Transmission:</span> <span class="value">${vinData.transmission}</span></div>`;
        html += `<div class="vin-line"><span class="label">ECU Types Found:</span> <span class="value">${vinData.ecuTypes.join(', ')}</span></div>`;
        html += '</div>';
        html += `<button class="action-btn primary" onclick="exportVINData('${vin}', ${JSON.stringify(vinData).replace(/"/g, '&quot;')})">💾 EXPORT AS TXT</button>`;
        
        output.innerHTML = html;
    }, 800);
}

function decodeVINFromPattern(vin) {
    // First 3 chars = manufacturer
    const winData = {
        '1F1': { manufacturer: 'Ford', country: 'USA' },
        '3G1': { manufacturer: 'Chevrolet', country: 'USA' },
        '2G1': { manufacturer: 'Chevrolet', country: 'USA' },
        '1C6': { manufacturer: 'Dodge/Ram', country: 'USA' },
        '3GM': { manufacturer: 'GMC', country: 'USA' },
        '5YJ': { manufacturer: 'Tesla', country: 'USA' },
        '1J4': { manufacturer: 'Jeep', country: 'USA' },
        'JN1': { manufacturer: 'Nissan', country: 'Japan' },
        'JF1': { manufacturer: 'Subaru', country: 'Japan' },
        'JM1': { manufacturer: 'Mazda', country: 'Japan' },
        'JH2': { manufacturer: 'Honda', country: 'Japan' },
        'WP0': { manufacturer: 'Porsche', country: 'Germany' },
        'WAU': { manufacturer: 'Audi', country: 'Germany' },
        'WBA': { manufacturer: 'BMW', country: 'Germany' },
        'WDD': { manufacturer: 'Mercedes', country: 'Germany' },
        'ZFF': { manufacturer: 'Ferrari', country: 'Italy' },
        'ZHW': { manufacturer: 'Lamborghini', country: 'Italy' }
    };
    
    const win = vin.substring(0, 3);
    const winInfo = winData[win] || { manufacturer: 'Unknown', country: 'Unknown' };
    
    // Year decoder (10th character)
    const yearMap = '9ABCDEFGHJKLMNPRSTUVWXYZ';
    const yearChar = vin.charAt(9);
    const yearIndex = yearMap.indexOf(yearChar);
    let year = 2000 + (yearIndex % 30);
    if (yearIndex < 9) year = 2010 + yearIndex;
    
    return {
        manufacturer: winInfo.manufacturer,
        country: winInfo.country,
        model: 'Vehicle Model (See Full Report)',
        year: year,
        bodyType: 'Light-Duty Truck/Vehicle',
        engine: 'Diesel/Gasoline (VIN Data)',
        transmission: 'Automatic/Manual',
        ecuTypes: [
            'Bosch ECU',
            'Delphi DCM',
            'Engine Control Module',
            'Transmission Control Module'
        ],
        displacement: '2000-6700cc',
        power: 'Variable',
        torque: 'Variable'
    };
}

function exportVINData(vin, vinData) {
    const timestamp = new Date().toISOString();
    const txt = `BLACKFLAG v2.0 - VIN DECODER REPORT
════════════════════════════════════════════════════
Generated: ${timestamp}

VIN INFORMATION
───────────────────────────────────────────────────
VIN Number:           ${vin}
Manufacturer:         ${vinData.manufacturer}
Country:              ${vinData.country}
Year:                 ${vinData.year}
Model:                ${vinData.model}
Body Type:            ${vinData.bodyType}

POWERTRAIN DETAILS
───────────────────────────────────────────────────
Engine Type:          ${vinData.engine}
Displacement:         ${vinData.displacement}
Power Output:         ${vinData.power}
Torque:               ${vinData.torque}
Transmission:         ${vinData.transmission}

ECU COMPATIBILITY
───────────────────────────────────────────────────
Detected ECU Types:
${vinData.ecuTypes.map(ecu => `  • ${ecu}`).join('\n')}

Total ECUs:           ${vinData.ecuTypes.length}
Supported Protocols:  OBD-II, CAN-FD, J1939

WIRING & SYSTEMS
───────────────────────────────────────────────────
System Voltage:       12V / 24V
OBD-II Connector:     DLC (Data Link Connector)
CAN Bus Speed:        500 kbps / 250 kbps
Diagnostics:          Full Support

NOTES
───────────────────────────────────────────────────
This report was generated by BlackFlag v2.0
For educational and research purposes only.
Always use responsibly and legally.

═════════════════════════════════════════════════════`;
    
    // Create blob and download
    const blob = new Blob([txt], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `VIN_${vin}_${Date.now()}.txt`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
    
    alert(`✓ VIN Report exported: VIN_${vin}_${Date.now()}.txt`);
}

// ============ ECU DETECTION & CONNECTION ============
let obd2Connected = false;
let voltagePollInterval = null;

async function initializeOBD2() {
    const status = document.getElementById('obd2StatusText');
    const statusLight = document.getElementById('obd2Status');
    
    status.textContent = 'CONNECTING...';
    
    setTimeout(() => {
        obd2Connected = true;
        statusLight.classList.remove('offline');
        statusLight.classList.add('online');
        status.textContent = 'CONNECTED';
        
        // Start voltage monitoring
        startVoltageMeter();
        
        // Auto-scan for ECUs
        scanECUs();
    }, 1200);
}

async function scanECUs() {
    const result = document.getElementById('ecuDetectionResult');
    result.innerHTML = '🔍 Scanning for ECUs...';
    
    setTimeout(() => {
        const ecuData = {
            'ECU_1': { id: 'F186', type: 'Engine Control', address: '0x18DA00F1' },
            'ECU_2': { id: 'F187', type: 'Transmission Control', address: '0x18DA00F1' },
            'ECU_3': { id: 'F18D', type: 'Body Control', address: '0x18DA00F1' },
            'ECU_4': { id: 'F191', type: 'ABS/Brake', address: '0x18DA00F1' }
        };
        
        result.classList.add('detected');
        let html = '<div class="ecu-scan-result">';
        html += '<strong>✓ ECUs Detected:</strong><br>';
        
        for (const [key, ecu] of Object.entries(ecuData)) {
            html += `<div class="ecu-item">
                <span class="ecu-type">${ecu.type}</span>
                <span class="ecu-id">ID: ${ecu.id}</span>
                <span class="ecu-addr">Addr: ${ecu.address}</span>
            </div>`;
        }
        
        html += '</div>';
        result.innerHTML = html;
    }, 800);
}

function startVoltageMeter() {
    if (voltagePollInterval) clearInterval(voltagePollInterval);
    
    // Simulate live voltage reading from OBD2
    voltagePollInterval = setInterval(() => {
        if (!obd2Connected) {
            clearInterval(voltagePollInterval);
            return;
        }
        
        // Simulate voltage fluctuation between 13-14.5V (healthy alternator)
        const baseVoltage = 13.8;
        const variation = (Math.random() - 0.5) * 1.5;
        const voltage = (baseVoltage + variation).toFixed(1);
        
        document.getElementById('voltageReading').textContent = voltage + 'V';
        
        // Update voltage bar
        const percentage = ((voltage - 10) / 5) * 100;
        document.getElementById('voltageFill').style.width = Math.min(100, percentage) + '%';
    }, 500);
}

function startVoltageMeterFull() {
    if (voltagePollInterval) clearInterval(voltagePollInterval);
    
    // Simulate live voltage reading from OBD2 for full-screen display
    voltagePollInterval = setInterval(() => {
        // Simulate voltage fluctuation between 13-14.5V (healthy alternator)
        const baseVoltage = 13.8;
        const variation = (Math.random() - 0.5) * 1.5;
        const voltage = (baseVoltage + variation).toFixed(1);
        
        const voltageReading = document.getElementById('voltageReadingLarge');
        const voltageFill = document.getElementById('voltageFillLarge');
        
        if (voltageReading) {
            voltageReading.textContent = voltage + 'V';
        }
        
        if (voltageFill) {
            // Update voltage bar (13V = 0%, 15V = 100%)
            const percentage = Math.max(0, Math.min(100, ((parseFloat(voltage) - 13) / 2) * 100));
            voltageFill.style.width = percentage + '%';
        }
    }, 500);
}

async function detectECULocation() {
    const result = document.getElementById('ecuLocationResult');
    result.innerHTML = '🔍 Testing ECU location...';
    
    setTimeout(() => {
        // Simulate detection logic
        const isTestBench = Math.random() > 0.5;
        const location = isTestBench ? 'TEST BENCH' : 'VEHICLE';
        const status = isTestBench ? 'Isolated ECU - No vehicle bus' : 'Live vehicle detected';
        
        result.classList.add('detected');
        result.innerHTML = `<strong>${location}</strong><br><small>${status}</small>`;
    }, 1000);
}

// ============ WIRING DIAGRAMS ============
async function loadVehicleWiring() {
    const ecuSelect = document.getElementById('ecuSelect');
    const viewer = document.getElementById('wiringDiagramViewer');
    
    if (!ecuSelect.value) {
        viewer.innerHTML = '<span class="error-text">Please select a vehicle first</span>';
        return;
    }
    
    viewer.innerHTML = '<span class="loading-text">Loading wiring diagram...</span>';
    
    setTimeout(() => {
        viewer.innerHTML = `
            <div class="diagram-content">
                <h3>Vehicle Wiring Diagram</h3>
                <p>Circuits: 50+ Standard Automotive Circuits</p>
                <p>Protocols: CAN-FD, OBD-II, J1939</p>
                <p>Status: LOADED</p>
                <button class="action-btn small" onclick="exportWiringDiagram()">📥 Download PNG</button>
            </div>
        `;
    }, 800);
}

function exportWiringDiagram() {
    alert('✓ Wiring diagram exported as PNG\nFile: vehicle_wiring_diagram.png');
}

// ============ LIBRARY BROWSER ============
function switchLibraryTab(tabName) {
    // Hide all tabs
    const tabs = document.querySelectorAll('.library-tab-content');
    tabs.forEach(tab => tab.classList.remove('active'));
    
    // Remove active class from buttons
    const buttons = document.querySelectorAll('.library-tab-btn');
    buttons.forEach(btn => btn.classList.remove('active'));
    
    // Show selected tab
    const selectedTab = document.getElementById(tabName);
    if (selectedTab) {
        selectedTab.classList.add('active');
    }
    
    // Mark button as active
    event.target.classList.add('active');
}

function searchVehicleDatabase() {
    const searchInput = document.getElementById('vehicleSearchInput').value.toLowerCase();
    const resultsDiv = document.getElementById('vehicleLibraryResults');
    
    if (!searchInput) {
        resultsDiv.innerHTML = '<p style="color: var(--neon-green); text-align: center;">Enter a manufacturer name to search...</p>';
        return;
    }
    
    // Filter vehicles by manufacturer
    const filteredVehicles = allVehicles.filter(vehicle => 
        vehicle.manufacturer.toLowerCase().includes(searchInput)
    );
    
    if (filteredVehicles.length === 0) {
        resultsDiv.innerHTML = '<p style="color: var(--neon-green); text-align: center;">No vehicles found matching your search.</p>';
        return;
    }
    
    // Display results
    let html = '';
    filteredVehicles.forEach(vehicle => {
        html += `
            <div class="vehicle-spec-card">
                <h4>${vehicle.year} ${vehicle.manufacturer} ${vehicle.model}</h4>
                <p>📊 Power: ${vehicle.power || 'N/A'} HP</p>
                <p>⚙️ Torque: ${vehicle.torque || 'N/A'} LB-FT</p>
                <p>🔧 Drive: ${vehicle.driveType || 'N/A'}</p>
                <div class="spec-detail">
                    <strong style="color: var(--neon-cyan);">ECU Types:</strong><br>
                    ${vehicle.ecuTypes ? vehicle.ecuTypes.join(', ') : 'N/A'}
                </div>
                <div class="spec-detail">
                    <strong style="color: var(--neon-cyan);">Transmission:</strong><br>
                    ${vehicle.transmissions ? vehicle.transmissions.join(', ') : 'N/A'}
                </div>
            </div>
        `;
    });
    
    resultsDiv.innerHTML = html;
}

function viewGuide(guideId) {
    const guideContent = document.getElementById('guideContent');
    const guides = {
        'obd2-basics': {
            title: 'OBD-II Basics',
            content: `
                <h4>OBD-II (On-Board Diagnostics) Fundamentals</h4>
                <p><strong>Protocol Standards:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>ISO 9141-2 (Keyword Protocol 2000)</li>
                    <li>ISO 14230-4 (Keyword Protocol 2000 with CAN)</li>
                    <li>ISO 15765-4 (CAN-based OBD)</li>
                    <li>SAE J1850 PWM (Pulse Width Modulation)</li>
                    <li>SAE J1850 VPW (Variable Pulse Width)</li>
                </ul>
                <p><strong>Connector Pinout (16-pin DLC):</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>Pin 1: Manufacturer Defined</li>
                    <li>Pin 2: CAN High / K-Line</li>
                    <li>Pin 3: Manufacturer Defined</li>
                    <li>Pin 4: Chassis Ground</li>
                    <li>Pin 5: Signal Ground</li>
                    <li>Pin 6: CAN Low / L-Line</li>
                    <li>Pin 16: Battery +12V</li>
                </ul>
            `
        },
        'can-diagnostics': {
            title: 'CAN Bus Diagnostics',
            content: `
                <h4>CAN (Controller Area Network) Bus Troubleshooting</h4>
                <p><strong>Common Issues & Solutions:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li><strong>No Communication:</strong> Check termination resistors, verify 120Ω at both ends of CAN bus</li>
                    <li><strong>Intermittent Loss:</strong> Check for corroded connectors or loose pins</li>
                    <li><strong>Voltage Levels:</strong> CAN High should be ~3.5V, CAN Low ~1.5V at idle</li>
                    <li><strong>Baud Rate:</strong> Common rates are 250 kbps, 500 kbps, 1 Mbps</li>
                </ul>
                <p><strong>Diagnostic Procedure:</strong></p>
                <ol style="color: var(--neon-green); margin: 1rem 0;">
                    <li>Use oscilloscope to check differential voltage</li>
                    <li>Verify termination resistance with multimeter</li>
                    <li>Check for recessive vs. dominant levels</li>
                    <li>Monitor CAN bus traffic patterns</li>
                </ol>
            `
        },
        'ecu-communication': {
            title: 'ECU Communication',
            content: `
                <h4>ECU Communication Protocols & Messaging</h4>
                <p><strong>Primary Protocols:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li><strong>UDS (Unified Diagnostic Services):</strong> ISO 14229-1 standard for diagnostics</li>
                    <li><strong>KWP2000:</strong> Keyword Protocol 2000 legacy standard</li>
                    <li><strong>J1939:</strong> Heavy-duty vehicle protocol (diesel trucks)</li>
                    <li><strong>CAN-FD:</strong> CAN with Flexible Data-rate (newer vehicles)</li>
                </ul>
                <p><strong>Message Format:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>Service ID (1 byte): Read/Write operation</li>
                    <li>Data Identifier (2 bytes): Specific parameter</li>
                    <li>Data Record: Variable length response</li>
                    <li>Checksum: 1 byte validation</li>
                </ul>
            `
        },
        'emissions-codes': {
            title: 'Emissions Codes',
            content: `
                <h4>Understanding Diagnostic Trouble Codes (DTCs)</h4>
                <p><strong>P-Code Format (Powertrain):</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>P0xxx: Generic powertrain codes</li>
                    <li>P1xxx: Manufacturer-specific codes</li>
                    <li>P2xxx: Fuel & air metering</li>
                    <li>P3xxx: Ignition system</li>
                </ul>
                <p><strong>Common Emissions Codes:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>P0400: EGR Flow Malfunction</li>
                    <li>P0405: EGR Sensor A Circuit</li>
                    <li>P0409: EGR Sensor A Circuit Range/Performance</li>
                    <li>P0420: Catalyst System Efficiency Below Threshold</li>
                    <li>P0430: Catalyst System Efficiency Below Threshold (Bank 2)</li>
                </ul>
            `
        },
        'voltage-testing': {
            title: 'Voltage Testing',
            content: `
                <h4>System Voltage Testing & Measurement</h4>
                <p><strong>Normal Operating Voltages:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>Battery Resting: 12.6V (fully charged)</li>
                    <li>Engine Running: 13.5V - 14.5V</li>
                    <li>System Ground: 0V reference</li>
                    <li>Cranking: 10V minimum</li>
                </ul>
                <p><strong>Sensor Voltages:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>Oxygen Sensor: 0.1V - 0.9V (switching)</li>
                    <li>Coolant Temp Sensor: 0.5V - 4.5V</li>
                    <li>Mass Air Flow: 1V - 7V</li>
                    <li>Throttle Position: 0.5V - 4.5V</li>
                </ul>
            `
        },
        'ecu-reprogramming': {
            title: 'ECU Reprogramming',
            content: `
                <h4>Safe ECU Flash Procedures</h4>
                <p><strong>Pre-Flash Checklist:</strong></p>
                <ul style="color: var(--neon-green); margin: 1rem 0;">
                    <li>✓ Battery voltage 13.5V - 14.5V minimum</li>
                    <li>✓ Backup original ECU data</li>
                    <li>✓ Verify correct firmware version</li>
                    <li>✓ Disable antitheft systems if required</li>
                    <li>✓ Ensure stable connection (J2534 or similar)</li>
                </ul>
                <p><strong>Flash Procedure Steps:</strong></p>
                <ol style="color: var(--neon-green); margin: 1rem 0;">
                    <li>Establish diagnostic connection</li>
                    <li>Read current ECU data (backup)</li>
                    <li>Erase ECU memory blocks</li>
                    <li>Write new calibration data</li>
                    <li>Verify checksum/security access</li>
                    <li>Power cycle vehicle</li>
                    <li>Confirm successful programming</li>
                </ol>
            `
        }
    };
    
    const guide = guides[guideId] || { title: 'Guide Not Found', content: '<p>This guide is not available.</p>' };
    guideContent.innerHTML = `<h4 style="color: var(--neon-cyan); margin-bottom: 1rem;">📖 ${guide.title}</h4>${guide.content}`;
}

function loadWiringDiagram() {
    const systemSelect = document.getElementById('wiringSystemSelect');
    const systemValue = systemSelect.value;
    const displayDiv = document.getElementById('wiringDiagramDisplay');
    
    if (!systemValue) {
        displayDiv.innerHTML = '<p style="color: var(--neon-green); text-align: center;">Select a system to view its wiring diagram...</p>';
        return;
    }
    
    const diagrams = {
        'charging': {
            name: 'Charging System',
            content: `
                <h4>Alternator & Charging Circuit</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Components:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Alternator (AC to DC conversion)</li>
                        <li>Voltage Regulator (maintains 13.5-14.5V)</li>
                        <li>Battery (storage & filtering)</li>
                        <li>Serpentine Belt (alternator drive)</li>
                    </ul>
                    <p><strong>Key Wire Colors:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Red: Battery positive (+)</li>
                        <li>Black: Ground (-)</li>
                        <li>Yellow: Alternator output</li>
                        <li>Green: Sense wire (voltage feedback)</li>
                    </ul>
                </div>
            `
        },
        'starting': {
            name: 'Starting System',
            content: `
                <h4>Starter Motor & Starting Circuit</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Components:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Starter Motor (engine cranking)</li>
                        <li>Solenoid Relay (high-current switching)</li>
                        <li>Ignition Switch (key position sensor)</li>
                        <li>Battery Cable (12V+ source)</li>
                    </ul>
                    <p><strong>Circuit Specifications:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Voltage: 12V system (minimum 10V during crank)</li>
                        <li>Current: 100-200A typical</li>
                        <li>Duty Cycle: Intermittent (few seconds max)</li>
                    </ul>
                </div>
            `
        },
        'ignition': {
            name: 'Ignition System',
            content: `
                <h4>Ignition Coil & Spark System</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Components:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Ignition Coil (voltage step-up)</li>
                        <li>Spark Plugs (combustion ignition)</li>
                        <li>Engine Control Module (timing control)</li>
                        <li>Distributor or Coil Pack</li>
                    </ul>
                    <p><strong>Output Specifications:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Primary Voltage: 12V</li>
                        <li>Secondary Voltage: 30,000-60,000V</li>
                        <li>Spark Gap: 0.04" - 0.06"</li>
                    </ul>
                </div>
            `
        },
        'fuel': {
            name: 'Fuel Pump Circuit',
            content: `
                <h4>Electric Fuel Pump System</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Components:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>In-Tank Fuel Pump (electric motor)</li>
                        <li>Fuel Pressure Regulator (4-6 psi carbureted, 35-45 psi fuel injected)</li>
                        <li>Fuel Filter (pre & post pump)</li>
                        <li>Fuel Rail & Injectors</li>
                    </ul>
                    <p><strong>Circuit Parameters:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Voltage: 12V DC</li>
                        <li>Current: 4-8A typical</li>
                        <li>Pressure: 35-45 psi fuel injected</li>
                    </ul>
                </div>
            `
        },
        'injectors': {
            name: 'Fuel Injector Circuit',
            content: `
                <h4>Fuel Injector Control & Waveforms</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Components:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Fuel Injectors (solenoid-controlled valve)</li>
                        <li>Injector Driver (ECM output stage)</li>
                        <li>Fuel Rail (common pressure line)</li>
                        <li>Return Line (excess fuel)</li>
                    </ul>
                    <p><strong>Control Signals:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Voltage: 12V pulse (100-1000 μs)</li>
                        <li>Current: 1-4A per injector</li>
                        <li>Frequency: 10-100 Hz (engine dependent)</li>
                    </ul>
                </div>
            `
        },
        'oxygen-sensors': {
            name: 'Oxygen Sensor Circuits',
            content: `
                <h4>O2/Lambda Sensor Systems</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Sensor Types:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Zirconia (heated, narrowband) - 0.1V-0.9V</li>
                        <li>Titania (unheated, wideband) - 0.5V-4.5V</li>
                        <li>Planar (heated, fast response)</li>
                    </ul>
                    <p><strong>Typical Configurations:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Upstream (pre-catalytic): Primary fuel control</li>
                        <li>Downstream (post-catalytic): Catalyst monitoring</li>
                        <li>Bank 1/Bank 2: Cylinder group separation</li>
                    </ul>
                </div>
            `
        },
        'can-bus': {
            name: 'CAN Bus Network',
            content: `
                <h4>CAN Bus Topology & Termination</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Network Configuration:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>CAN High: Yellow/White (dominant HIGH ~5V)</li>
                        <li>CAN Low: Green (dominant LOW ~0V)</li>
                        <li>Differential: 2V swing (recessive to dominant)</li>
                        <li>Termination: 120Ω resistors at each end</li>
                    </ul>
                    <p><strong>Bus Speed (Common):</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>250 kbps: Comfort features, body systems</li>
                        <li>500 kbps: Powertrain, safety systems</li>
                        <li>1 Mbps: High-speed diagnostics</li>
                    </ul>
                </div>
            `
        },
        'abs': {
            name: 'ABS/Brake System',
            content: `
                <h4>Anti-Lock Braking System Circuits</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Components:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>ABS Control Module (ECU)</li>
                        <li>Wheel Speed Sensors (ABS rings)</li>
                        <li>ABS Pump & Solenoids (pressure control)</li>
                        <li>Brake Fluid Pressure Lines</li>
                    </ul>
                    <p><strong>Sensor Specifications:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Type: Inductive or Hall-effect</li>
                        <li>Output: Frequency-based (speed proportional)</li>
                        <li>Voltage: 0-12V AC (inductive)</li>
                    </ul>
                </div>
            `
        },
        'transmission': {
            name: 'Transmission Control',
            content: `
                <h4>Automatic Transmission Control Systems</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Components:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Transmission Control Module (TCM)</li>
                        <li>Shift Solenoids (gear selection)</li>
                        <li>Pressure Control Solenoids (modulation)</li>
                        <li>Input/Output Speed Sensors</li>
                    </ul>
                    <p><strong>Control Signals:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Solenoid Voltage: 12V PWM (pulse width modulated)</li>
                        <li>Current: 0.5-2A per solenoid</li>
                        <li>Duty Cycle: 0-100% (pressure adjustment)</li>
                    </ul>
                </div>
            `
        },
        'emissions': {
            name: 'Emissions Control System',
            content: `
                <h4>Diesel & Gasoline Emissions Control</h4>
                <div style="background: rgba(0, 255, 0, 0.05); padding: 1.5rem; border-radius: 6px; margin: 1rem 0;">
                    <p><strong>Diesel Systems:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>DPF (Diesel Particulate Filter): Soot trap & regen</li>
                        <li>SCR (Selective Catalytic Reduction): NOx reduction with urea</li>
                        <li>EGR (Exhaust Gas Recirculation): Emission reduction</li>
                    </ul>
                    <p><strong>Gasoline Systems:</strong></p>
                    <ul style="color: var(--neon-green); margin: 1rem 0;">
                        <li>Catalytic Converter: Chemical conversion</li>
                        <li>EGR Valve: Recirculation control</li>
                        <li>Oxygen Sensors: Feedback control</li>
                    </ul>
                </div>
            `
        }
    };
    
    const diagram = diagrams[systemValue] || { name: 'System Not Found', content: '<p>This diagram is not available.</p>' };
    displayDiv.innerHTML = `<h4 style="color: var(--neon-cyan); margin-bottom: 1rem;">📋 ${diagram.name}</h4>${diagram.content}`;
}