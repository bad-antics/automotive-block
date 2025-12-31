// BlackFlag v2.0 - Main Application JavaScript
// Professional ECU Hacking & Tuning Suite

const API_URL = 'http://localhost:3000/api';
let allVehicles = []; // Global vehicles array
let selectedVehicle = null; // Track selected vehicle
let allManufacturers = []; // Global manufacturers array
let currentTheme = 'cyberpunk'; // Current theme
let vehicleHistory = []; // Vehicle history array
let ecuProfiles = {}; // ECU profiles by vehicle
let backupTunes = {}; // Backup tunes by vehicle
let stockMaps = {}; // Stock maps by vehicle

// ============ THEME MANAGEMENT ============
function switchTheme(themeName) {
    const body = document.body;
    // Remove all theme classes
    body.classList.remove('dark-pirate-theme', 'commodore64-theme', 'ford-fjds-theme', 'witech-theme');
    
    // Apply new theme with complete style transformation
    switch(themeName) {
        case 'cyberpunk':
            body.classList.add('dark-pirate-theme');
            updateBrandText('☠️ BLACKFLAG v2.0 ☠️');
            updateStatusBanner('System Online', 'green');
            break;
        case 'commodore64':
            body.classList.add('commodore64-theme');
            updateBrandText('💾 BLACKFLAG COMMODORE EDITION');
            updateStatusBanner('READY.', 'blue');
            break;
        case 'ford-fjds':
            body.classList.add('ford-fjds-theme');
            updateBrandText('🔧 BLACKFLAG DIAGNOSTIC SYSTEM');
            updateStatusBanner('DIAGNOSTICS READY', 'blue');
            break;
        case 'witech':
            body.classList.add('witech-theme');
            updateBrandText('🚗 BLACKFLAG WITECH PRO');
            updateStatusBanner('CONNECTED', 'orange');
            break;
    }
    
    currentTheme = themeName;
    localStorage.setItem('blackflag-theme', themeName);
    console.log('Theme switched to:', themeName);
}

function updateBrandText(text) {
    const brandElement = document.querySelector('.brand-name');
    if (brandElement) brandElement.textContent = text;
}

function updateStatusBanner(text, color) {
    const statusText = document.querySelector('.status-text');
    const statusIndicator = document.querySelector('.status-indicator');
    if (statusText) statusText.textContent = text;
    if (statusIndicator) {
        statusIndicator.className = `status-indicator ${color}`;
    }
}

// Load saved theme on startup
function loadSavedTheme() {
    const savedTheme = localStorage.getItem('blackflag-theme') || 'cyberpunk';
    const themeSelector = document.getElementById('themeSelector');
    if (themeSelector) {
        themeSelector.value = savedTheme;
        switchTheme(savedTheme);
    }
}

// ============ MANUFACTURER FILTERING ============
function populateManufacturers(vehicles) {
    const manufacturers = [...new Set(vehicles.map(v => v.make || v.manufacturer))].sort();
    allManufacturers = manufacturers;
    
    const manufacturerFilter = document.getElementById('manufacturerFilter');
    if (manufacturerFilter) {
        manufacturerFilter.innerHTML = '<option value="">⚔️ All Manufacturers</option>';
        manufacturers.forEach(mfg => {
            const option = document.createElement('option');
            option.value = mfg;
            option.textContent = mfg;
            manufacturerFilter.appendChild(option);
        });
    }
}

function filterByManufacturer(manufacturer) {
    const vehicleSelector = document.getElementById('vehicleSelector');
    if (!vehicleSelector) return;
    
    vehicleSelector.innerHTML = '<option value="">⚔️ Choose a vehicle...</option>';
    
    const filteredVehicles = manufacturer 
        ? allVehicles.filter(v => (v.make || v.manufacturer) === manufacturer)
        : allVehicles;
    
    // Group by manufacturer even in filtered view
    const grouped = {};
    filteredVehicles.forEach(vehicle => {
        const make = vehicle.make || vehicle.manufacturer || 'Other';
        if (!grouped[make]) {
            grouped[make] = [];
        }
        grouped[make].push(vehicle);
    });
    
    Object.keys(grouped).sort().forEach(make => {
        const optgroup = document.createElement('optgroup');
        optgroup.label = make;
        
        grouped[make].forEach(vehicle => {
            const option = document.createElement('option');
            option.value = vehicle.id;
            option.textContent = `${vehicle.year} ${vehicle.model} (${vehicle.engine || 'N/A'})`;
            optgroup.appendChild(option);
        });
        
        vehicleSelector.appendChild(optgroup);
    });
    
    console.log(`Filtered to ${filteredVehicles.length} vehicles for manufacturer: ${manufacturer || 'All'}`);
}

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
            <h4>⚔️ ${vehicle.year} ${vehicle.make} ${vehicle.model}</h4>
            <p>📊 Power: ${vehicle.horsepower || 'N/A'} HP | Torque: ${vehicle.torque || 'N/A'} Nm</p>
            <p>🔧 Engine: ${vehicle.engine || 'N/A'}</p>
            <p>💨 Drive Type: ${vehicle.driveType || 'N/A'} | Body Type: ${vehicle.bodyType || 'N/A'}</p>
            <p style="color: var(--neon-cyan);">📡 Available ECUs: ${vehicle.ecus?.length || 0}</p>
        </div>
    `;
}

// ============ INITIALIZATION ============
document.addEventListener('DOMContentLoaded', function() {
    console.log('🏴 BlackFlag v2.0 Desktop Edition Loaded');
    loadSavedTheme();
    setupEventListeners();
    displaySystemStatus();
    loadVehicleDatabase();
    loadVehicleHistory();
});

// ============ VEHICLE DATABASE FUNCTIONS ============
async function loadVehicleDatabase() {
    try {
        console.log('Loading vehicle database from API...');
        const response = await fetch(`${API_URL}/vehicles/list`);
        const data = await response.json();
        
        console.log('Vehicle database response:', data);
        
        if (Array.isArray(data)) {
            console.log(`Found ${data.length} vehicles`);
            allVehicles = data;
            populateManufacturers(data);
            populateVehicleSelector(data);
        } else if (data.vehicles && Array.isArray(data.vehicles)) {
            console.log(`Found ${data.vehicles.length} vehicles`);
            allVehicles = data.vehicles;
            populateManufacturers(data.vehicles);
            populateVehicleSelector(data.vehicles);
        }
    } catch (error) {
        console.error('Failed to load vehicle database:', error);
    }
}

function populateVehicleSelector(vehicles) {
    console.log('Populating vehicle selector with', vehicles.length, 'vehicles');
    
    const selector = document.getElementById('vehicleSelector');
    if (!selector) {
        console.error('vehicleSelector element not found in DOM');
        return;
    }
    
    // Clear existing options
    selector.innerHTML = '<option value="">⚔️ Choose a vehicle...</option>';
    
    // Group vehicles by manufacturer
    const grouped = {};
    vehicles.forEach(vehicle => {
        const make = vehicle.make || vehicle.manufacturer || 'Other';
        if (!grouped[make]) {
            grouped[make] = [];
        }
        grouped[make].push(vehicle);
    });
    
    console.log('Grouped vehicles by manufacturer:', Object.keys(grouped));
    
    // Create option groups
    Object.keys(grouped).sort().forEach(make => {
        const optgroup = document.createElement('optgroup');
        optgroup.label = make;
        
        grouped[make].forEach(vehicle => {
            const option = document.createElement('option');
            option.value = vehicle.id;
            option.textContent = `${vehicle.year} ${vehicle.model} (${vehicle.engine || 'N/A'})`;
            optgroup.appendChild(option);
        });
        
        selector.appendChild(optgroup);
    });
    
    console.log('Vehicle selector populated successfully');
}

// ============ EVENT LISTENERS ============
function setupEventListeners() {
    // Add vehicle selector change event if needed
    const vehicleSelector = document.getElementById('vehicleSelector');
    if (vehicleSelector) {
        vehicleSelector.addEventListener('change', () => {
            console.log('Vehicle selected');
        });
    }
}

// ============ DISPLAY SYSTEM STATUS ============
async function displaySystemStatus() {
    try {
        const response = await fetch(`${API_URL}/status`);
        const data = await response.json();
        
        console.log('System Status:', data);
    } catch (error) {
        console.error('Failed to fetch system status:', error);
    }
}

// ============ LIBRARY BROWSER FUNCTIONS ============
function switchLibraryTab(tabName) {
    // Hide all tabs
    const tabs = document.querySelectorAll('.library-content');
    tabs.forEach(tab => tab.classList.remove('active'));
    
    // Remove active class from buttons
    const buttons = document.querySelectorAll('.library-tab');
    buttons.forEach(btn => btn.classList.remove('active'));
    
    // Show selected tab
    const selectedTab = document.getElementById(`lib-${tabName}`);
    if (selectedTab) {
        selectedTab.classList.add('active');
    }
    
    // Mark button as active
    if (event && event.target) {
        event.target.classList.add('active');
    }
}

function searchVehicleDatabase(query) {
    if (!query) {
        alert('Please enter a vehicle name to search');
        return;
    }
    
    const results = allVehicles.filter(v => 
        (v.make || '').toLowerCase().includes(query.toLowerCase()) ||
        (v.model || '').toLowerCase().includes(query.toLowerCase()) ||
        (v.year || '').toString().includes(query)
    );
    
    if (results.length === 0) {
        alert('No vehicles found matching: ' + query);
        return;
    }
    
    displaySearchResults(results);
}

function displaySearchResults(results) {
    const grid = document.querySelector('.vehicle-grid');
    if (!grid) return;
    
    grid.innerHTML = '';
    results.forEach(vehicle => {
        const card = document.createElement('div');
        card.className = 'vehicle-card';
        card.innerHTML = `
            <h4>${vehicle.year} ${vehicle.make} ${vehicle.model}</h4>
            <p><strong>Engine:</strong> ${vehicle.engine || 'N/A'}</p>
            <p><strong>Horsepower:</strong> ${vehicle.horsepower || 'N/A'} hp</p>
            <p><strong>Torque:</strong> ${vehicle.torque || 'N/A'} Nm</p>
            <p><strong>ECUs:</strong> ${vehicle.ecus?.length || 0}</p>
        `;
        card.addEventListener('click', () => selectVehicleFromSearch(vehicle));
        grid.appendChild(card);
    });
}

function selectVehicleFromSearch(vehicle) {
    window.selectedVehicle = vehicle;
    alert(`✅ Selected: ${vehicle.year} ${vehicle.make} ${vehicle.model}\n\nECUs Available: ${vehicle.ecus?.join(', ') || 'N/A'}`);
}

function viewGuide(guideName) {
    const guides = {
        'obd2-codes': {
            title: 'OBD-II Diagnostic Trouble Codes Reference',
            content: `
                <h2>OBD-II DTC Categories</h2>
                
                <h3>POWERTRAIN (P-Codes):</h3>
                <ul>
                    <li><strong>P0000-P0099:</strong> Fuel and Air Metering</li>
                    <li><strong>P0100-P0199:</strong> Fuel and Air Metering</li>
                    <li><strong>P0200-P0299:</strong> Fuel and Air Metering (Injector Circuit)</li>
                    <li><strong>P0300-P0399:</strong> Ignition System or Misfire</li>
                    <li><strong>P0400-P0499:</strong> Auxiliary Emission Controls</li>
                    <li><strong>P0500-P0599:</strong> Vehicle Speed Controls and Idle Control</li>
                    <li><strong>P0600-P0699:</strong> Computer Output Circuit</li>
                    <li><strong>P0700-P0799:</strong> Transmission</li>
                </ul>

                <h3>CHASSIS (C-Codes):</h3>
                <p><strong>C0000-C3000:</strong> Chassis systems including ABS, airbags, suspension</p>

                <h3>BODY (B-Codes):</h3>
                <p><strong>B0000-B3000:</strong> Body systems including HVAC, lighting, security</p>

                <h3>NETWORK (U-Codes):</h3>
                <p><strong>U0000-U3000:</strong> Network communication errors</p>

                <h3>COMMON CODES:</h3>
                <ul>
                    <li><code>P0300</code> - Random/Multiple Cylinder Misfire Detected</li>
                    <li><code>P0301-P0312</code> - Cylinder 1-12 Misfire Detected</li>
                    <li><code>P0420</code> - Catalyst System Efficiency Below Threshold (Bank 1)</li>
                    <li><code>P0430</code> - Catalyst System Efficiency Below Threshold (Bank 2)</li>
                    <li><code>P0171</code> - System Too Lean (Bank 1)</li>
                    <li><code>P0174</code> - System Too Lean (Bank 2)</li>
                    <li><code>P0172</code> - System Too Rich (Bank 1)</li>
                    <li><code>P0175</code> - System Too Rich (Bank 2)</li>
                    <li><code>P0401</code> - Exhaust Gas Recirculation Flow Insufficient</li>
                    <li><code>P0442</code> - Evaporative Emission System Leak Detected (small leak)</li>
                    <li><code>P0455</code> - Evaporative Emission System Leak Detected (large leak)</li>
                </ul>
            `
        },
        'obd2-pids': {
            title: 'OBD-II Parameter IDs (PIDs)',
            content: `
                <h2>OBD-II Standard PIDs</h2>

                <h3>MODE 01 - CURRENT DATA:</h3>
                <ul>
                    <li><code>0x00</code> - PIDs supported [01-20]</li>
                    <li><code>0x01</code> - Monitor status since DTCs cleared</li>
                    <li><code>0x04</code> - Calculated engine load (%)</li>
                    <li><code>0x05</code> - Engine coolant temperature (°C)</li>
                    <li><code>0x0C</code> - Engine RPM</li>
                    <li><code>0x0D</code> - Vehicle speed (km/h)</li>
                    <li><code>0x0F</code> - Intake air temperature (°C)</li>
                    <li><code>0x10</code> - Mass air flow rate (g/s)</li>
                    <li><code>0x11</code> - Throttle position (%)</li>
                    <li><code>0x21</code> - Distance with MIL on (km)</li>
                    <li><code>0x2F</code> - Fuel tank level (%)</li>
                    <li><code>0x33</code> - Absolute barometric pressure (kPa)</li>
                    <li><code>0x5C</code> - Engine oil temperature (°C)</li>
                </ul>

                <h3>MODE 02 - FREEZE FRAME DATA:</h3>
                <p>Same PIDs as Mode 01, but frozen at DTC occurrence</p>

                <h3>MODE 03 - SHOW STORED DTCs:</h3>
                <p>Returns list of confirmed diagnostic trouble codes</p>

                <h3>MODE 04 - CLEAR DTCs:</h3>
                <p>Clears trouble codes and turns off MIL</p>

                <h3>MODE 09 - VEHICLE INFO:</h3>
                <ul>
                    <li><code>0x02</code> - Vehicle Identification Number (VIN)</li>
                    <li><code>0x04</code> - Calibration ID</li>
                    <li><code>0x06</code> - Calibration Verification Numbers (CVN)</li>
                    <li><code>0x0A</code> - ECU name</li>
                </ul>
            `
        },
        'dpf-removal': {
            title: 'DPF Delete & Disable Procedures',
            content: `
                <h2>DPF (Diesel Particulate Filter) Removal</h2>
                <p><strong>WARNING:</strong> DPF removal may be illegal in your jurisdiction</p>

                <h3>PROCEDURE:</h3>
                <ol>
                    <li>Physical removal of DPF from exhaust system</li>
                    <li>ECU reprogramming to disable DPF monitoring</li>
                    <li>Disable <code>P2002</code>, <code>P2463</code>, <code>P244A</code> DTC codes</li>
                    <li>Remove regeneration cycle programming</li>
                    <li>Adjust EGT (Exhaust Gas Temperature) monitoring</li>
                </ol>

                <h3>ECU MODIFICATIONS:</h3>
                <ul>
                    <li>Disable DPF pressure sensor monitoring</li>
                    <li>Remove active regeneration triggers</li>
                    <li>Adjust fuel injection timing</li>
                    <li>Modify boost pressure maps</li>
                </ul>

                <h3>COMMON PLATFORMS:</h3>
                <ul>
                    <li>Cummins 6.7L (RAM 2500/3500)</li>
                    <li>Duramax LML/L5P (GM 2500/3500)</li>
                    <li>Power Stroke 6.7L (Ford F-250/350)</li>
                    <li>EcoDiesel 3.0L (RAM 1500)</li>
                </ul>
            `
        },
        'scr-tuning': {
            title: 'SCR System Modification & Tuning',
            content: `
                <h2>SCR (Selective Catalytic Reduction) System Tuning</h2>
                <p>SCR uses Diesel Exhaust Fluid (DEF) to reduce NOx emissions</p>

                <h3>DEF DELETE PROCEDURE:</h3>
                <ol>
                    <li>Remove DEF dosing module</li>
                    <li>Install DEF bypass hardware</li>
                    <li>ECU programming to disable DEF monitoring</li>
                    <li>Disable <code>P20E8</code>, <code>P20BA</code>, <code>P20EE</code> DTC codes</li>
                </ol>

                <h3>ECU SOFTWARE MODIFICATIONS:</h3>
                <ul>
                    <li>Disable DEF quality sensor</li>
                    <li>Remove DEF level monitoring</li>
                    <li>Bypass DEF heater control</li>
                    <li>Eliminate engine derate functions</li>
                    <li>Adjust NOx sensor scaling</li>
                </ul>

                <h3>AFFECTED SYSTEMS:</h3>
                <ul>
                    <li>Dosing Control Unit (DCU)</li>
                    <li>NOx sensors (upstream & downstream)</li>
                    <li>DEF tank heater</li>
                    <li>DEF pump and injector</li>
                </ul>

                <h3>POPULAR APPLICATIONS:</h3>
                <ul>
                    <li>Cummins ISX/X15 (Commercial trucks)</li>
                    <li>Detroit DD13/DD15/DD16</li>
                    <li>Paccar MX-11/MX-13</li>
                    <li>International MaxxForce engines</li>
                </ul>
            `
        },
        'egr-disable': {
            title: 'EGR Disable Procedures',
            content: `
                <h2>EGR (Exhaust Gas Recirculation) Disabling</h2>

                <h3>BENEFITS:</h3>
                <ul>
                    <li>Improved throttle response</li>
                    <li>Reduced intake manifold carbon buildup</li>
                    <li>Lower EGT (Exhaust Gas Temperatures)</li>
                    <li>Better fuel economy</li>
                    <li>Increased turbo lifespan</li>
                </ul>

                <h3>METHODS:</h3>
                <ol>
                    <li>EGR Block-Off Plate Installation</li>
                    <li>ECU Software Disable</li>
                    <li>EGR Valve Disconnection</li>
                </ol>

                <h3>ECU PROGRAMMING:</h3>
                <ul>
                    <li>Disable EGR valve position monitoring</li>
                    <li>Zero EGR flow rate tables</li>
                    <li>Disable <code>P0401</code>, <code>P0403</code>, <code>P0404</code> codes</li>
                    <li>Adjust MAF sensor compensation</li>
                    <li>Modify boost pressure maps</li>
                </ul>

                <h3>HARDWARE OPTIONS:</h3>
                <ul>
                    <li>EGR cooler delete</li>
                    <li>EGR valve block-off plates</li>
                    <li>Up-pipe modifications (Subaru)</li>
                    <li>Intake manifold cleaning</li>
                </ul>

                <h3>COMMON VEHICLES:</h3>
                <ul>
                    <li>Volkswagen TDI (2.0L/3.0L)</li>
                    <li>BMW N47/N57 diesel engines</li>
                    <li>GM Duramax LB7/LLY/LBZ/LMM</li>
                    <li>Ford Power Stroke 6.0L/6.4L/6.7L</li>
                </ul>
            `
        },
        'performance-tuning': {
            title: 'Performance ECU Tuning Guide',
            content: `
                <h2>ECU Performance Tuning Fundamentals</h2>

                <h3>TUNING PARAMETERS:</h3>

                <h4>1. Fuel Delivery</h4>
                <ul>
                    <li>Injection timing (advance/retard)</li>
                    <li>Injection duration</li>
                    <li>Rail pressure</li>
                    <li>Pulse width</li>
                </ul>

                <h4>2. Air Management</h4>
                <ul>
                    <li>Boost pressure limits</li>
                    <li>Wastegate duty cycle</li>
                    <li>VGT (Variable Geometry Turbo) position</li>
                    <li>MAF sensor scaling</li>
                </ul>

                <h4>3. Ignition Control (Gasoline)</h4>
                <ul>
                    <li>Timing advance maps</li>
                    <li>Knock sensor thresholds</li>
                    <li>Rev limiter</li>
                    <li>Cylinder cutoff</li>
                </ul>

                <h4>4. Torque Management</h4>
                <ul>
                    <li>Torque limiting tables</li>
                    <li>Gear-based power delivery</li>
                    <li>Traction control thresholds</li>
                </ul>

                <h3>STAGE DEFINITIONS:</h3>
                <ul>
                    <li><strong>STAGE 1:</strong> ECU software only (10-30% gains)</li>
                    <li><strong>STAGE 2:</strong> ECU + exhaust/intake (20-40% gains)</li>
                    <li><strong>STAGE 3:</strong> ECU + turbo upgrade (40-60% gains)</li>
                </ul>

                <h3>SAFETY LIMITS:</h3>
                <ul>
                    <li>Monitor EGT < 1400°F continuous</li>
                    <li>Keep boost pressure within turbo specs</li>
                    <li>Maintain proper air/fuel ratios</li>
                    <li>Watch transmission slip thresholds</li>
                </ul>
            `
        }
    };
    
    const guide = guides[guideName];
    if (guide) {
        const modal = document.getElementById('guideModal');
        const modalContent = document.getElementById('guideModalContent');
        
        modalContent.innerHTML = `
            <h2>${guide.title}</h2>
            ${guide.content}
            <button class="guide-close-btn" onclick="closeGuideModal()">✕ Close</button>
        `;
        
        modal.classList.add('active');
    }
}

function closeGuideModal() {
    const modal = document.getElementById('guideModal');
    modal.classList.remove('active');
}

// Close modal when clicking outside
document.addEventListener('click', function(e) {
    const modal = document.getElementById('guideModal');
    if (e.target === modal) {
        closeGuideModal();
    }
});


function loadWiringDiagram(diagramName) {
    const diagrams = {
        'obd2-pinout': {
            title: 'OBD2 Connector Pinout',
            image: 'data:image/svg+xml;base64,' + btoa(`
<svg width="400" height="300" xmlns="http://www.w3.org/2000/svg">
  <rect width="400" height="300" fill="#0a0a0a"/>
  <rect x="50" y="50" width="300" height="200" fill="#1a1a1a" stroke="#00ff00" stroke-width="2"/>
  <text x="200" y="30" fill="#00ffff" font-size="18" text-anchor="middle" font-family="monospace">OBD2 16-Pin Connector</text>
  <circle cx="100" cy="100" r="8" fill="#00ff00"/><text x="120" y="105" fill="#00ff00" font-size="12">Pin 1: Mfr Defined</text>
  <circle cx="100" cy="130" r="8" fill="#00ff00"/><text x="120" y="135" fill="#00ff00" font-size="12">Pin 2: CAN High</text>
  <circle cx="100" cy="160" r="8" fill="#00ff00"/><text x="120" y="165" fill="#00ff00" font-size="12">Pin 4: GND</text>
  <circle cx="100" cy="190" r="8" fill="#00ff00"/><text x="120" y="195" fill="#00ff00" font-size="12">Pin 5: Signal GND</text>
  <circle cx="100" cy="220" r="8" fill="#00ff00"/><text x="120" y="225" fill="#00ff00" font-size="12">Pin 6: CAN Low</text>
  <circle cx="280" cy="100" r="8" fill="#00ffff"/><text x="170" y="105" fill="#00ffff" font-size="12" text-anchor="end">Pin 16: +12V</text>
  <circle cx="280" cy="130" r="8" fill="#00ffff"/><text x="170" y="135" fill="#00ffff" font-size="12" text-anchor="end">Pin 14: CAN Low</text>
  <circle cx="280" cy="160" r="8" fill="#00ffff"/><text x="170" y="165" fill="#00ffff" font-size="12" text-anchor="end">Pin 7: K-Line</text>
</svg>`),
            description: 'Pin 1: Power (12V)\\nPin 2: J1962 Bus -\\nPin 3: TX (K-Line)\\nPin 4: Chassis Ground\\nPin 5: Signal Ground\\nPin 6: CAN High\\nPin 7: K-Line\\nPin 14: CAN Low\\nPin 16: Power (12V)'
        },
        'can-bus': {
            title: 'CAN Bus Wiring',
            image: 'data:image/svg+xml;base64,' + btoa(`
<svg width="400" height="300" xmlns="http://www.w3.org/2000/svg">
  <rect width="400" height="300" fill="#0a0a0a"/>
  <text x="200" y="30" fill="#00ffff" font-size="18" text-anchor="middle" font-family="monospace">CAN Bus Topology</text>
  <line x1="50" y1="150" x2="350" y2="150" stroke="#00ff00" stroke-width="3"/>
  <text x="200" y="140" fill="#00ff00" font-size="12" text-anchor="middle">CAN High</text>
  <line x1="50" y1="180" x2="350" y2="180" stroke="#00ffff" stroke-width="3"/>
  <text x="200" y="200" fill="#00ffff" font-size="12" text-anchor="middle">CAN Low</text>
  <rect x="40" y="130" width="20" height="70" fill="#ff0000"/>
  <text x="50" y="220" fill="#ff0000" font-size="10" text-anchor="middle">120Ω</text>
  <rect x="340" y="130" width="20" height="70" fill="#ff0000"/>
  <text x="350" y="220" fill="#ff0000" font-size="10" text-anchor="middle">120Ω</text>
  <circle cx="150" cy="165" r="15" fill="#00ff00"/>
  <text x="150" y="250" fill="#00ff00" font-size="10" text-anchor="middle">ECU 1</text>
  <circle cx="250" cy="165" r="15" fill="#00ff00"/>
  <text x="250" y="250" fill="#00ff00" font-size="10" text-anchor="middle">ECU 2</text>
</svg>`),
            description: 'CAN High: Yellow/White wire\\nCAN Low: Green wire\\nTermination: 120Ω resistors at both ends\\nData Rate: 250-500 Kbps'
        }
    };
    
    const diagram = diagrams[diagramName];
    if (diagram) {
        const modal = document.createElement('div');
        modal.className = 'diagram-modal';
        modal.innerHTML = `
            <div class="diagram-modal-content">
                <div class="diagram-header">
                    <h3>${diagram.title}</h3>
                    <button onclick="this.closest('.diagram-modal').remove()" class="close-btn">✕</button>
                </div>
                <img src="${diagram.image}" alt="${diagram.title}" class="diagram-image"/>
                <div class="diagram-description">
                    <pre>${diagram.description}</pre>
                </div>
                <div class="diagram-nav">
                    <button class="btn" onclick="navigateDiagram('prev')">← Previous</button>
                    <button class="btn" onclick="navigateDiagram('next')">Next →</button>
                    <button class="btn btn-primary" onclick="downloadDiagram('${diagramName}')">💾 Download</button>
                </div>
            </div>
        `;
        document.body.appendChild(modal);
    }
}

function navigateDiagram(direction) {
    console.log('Navigate diagram:', direction);
    alert('Diagram navigation: ' + direction);
}

function downloadDiagram(diagramName) {
    alert('Download diagram: ' + diagramName);
    console.log('Downloading diagram:', diagramName);
}

// ============ VEHICLE HISTORY MANAGEMENT ============

function loadVehicleHistory() {
    const saved = localStorage.getItem('blackflag-vehicle-history');
    vehicleHistory = saved ? JSON.parse(saved) : [];
    displayHistoryItems();
}

function addCurrentToHistory() {
    const selector = document.getElementById('vehicleSelector');
    const vehicleId = selector.value;
    
    if (!vehicleId) {
        alert('Please select a vehicle first');
        return;
    }
    
    const vehicle = allVehicles.find(v => v.id === vehicleId);
    if (!vehicle) return;
    
    // Check if already in history
    const existingIndex = vehicleHistory.findIndex(v => v.id === vehicleId);
    if (existingIndex > -1) {
        vehicleHistory.splice(existingIndex, 1);
    }
    
    // Add to beginning of history
    vehicleHistory.unshift({
        ...vehicle,
        addedDate: new Date().toISOString(),
        lastAccessed: new Date().toISOString()
    });
    
    // Keep only last 10 vehicles
    if (vehicleHistory.length > 10) {
        vehicleHistory = vehicleHistory.slice(0, 10);
    }
    
    localStorage.setItem('blackflag-vehicle-history', JSON.stringify(vehicleHistory));
    displayHistoryItems();
    loadVehicleProfile(vehicleId);
}

function displayHistoryItems() {
    const container = document.getElementById('historyItems');
    if (!container) return;
    
    if (vehicleHistory.length === 0) {
        container.innerHTML = '<p class=\"empty-state\">No vehicles in history. Add one above.</p>';
        return;
    }
    
    container.innerHTML = vehicleHistory.map(vehicle => `
        <div class=\"history-item\" onclick=\"selectFromHistory('${vehicle.id}')\">
            <div class=\"history-icon\">🚗</div>
            <div class=\"history-info\">
                <strong>${vehicle.manufacturer || vehicle.make} ${vehicle.model}</strong>
                <span>${vehicle.year} | ${vehicle.engines ? vehicle.engines[0] : 'N/A'}</span>
                <small>Added: ${new Date(vehicle.addedDate).toLocaleDateString()}</small>
            </div>
            <button class=\"history-action\" onclick=\"removeFromHistory('${vehicle.id}'); event.stopPropagation();\">✕</button>
        </div>
    `).join('');
}

function selectFromHistory(vehicleId) {
    selectedVehicle = vehicleHistory.find(v => v.id === vehicleId);
    if (selectedVehicle) {
        // Update last accessed
        selectedVehicle.lastAccessed = new Date().toISOString();
        localStorage.setItem('blackflag-vehicle-history', JSON.stringify(vehicleHistory));
        loadVehicleProfile(vehicleId);
    }
}

function removeFromHistory(vehicleId) {
    vehicleHistory = vehicleHistory.filter(v => v.id !== vehicleId);
    localStorage.setItem('blackflag-vehicle-history', JSON.stringify(vehicleHistory));
    displayHistoryItems();
}

function loadVehicleProfile(vehicleId) {
    document.getElementById('ecuProfilesSection').style.display = 'block';
    document.getElementById('backupTunesSection').style.display = 'block';
    document.getElementById('stockMappingSection').style.display = 'block';
    
    loadECUProfiles(vehicleId);
    loadBackupTunes(vehicleId);
    loadStockMapping(vehicleId);
}

// ============ ECU PROFILES ============

function loadECUProfiles(vehicleId) {
    const container = document.getElementById('ecuProfiles');
    if (!container) return;
    
    // Load saved profiles or create defaults
    const savedProfiles = localStorage.getItem(`ecu-profiles-${vehicleId}`);
    const profiles = savedProfiles ? JSON.parse(savedProfiles) : [
        { name: 'Stock', modified: false, hp: 'Stock', torque: 'Stock' },
        { name: 'Stage 1', modified: true, hp: '+15%', torque: '+20%' },
        { name: 'Stage 2', modified: true, hp: '+30%', torque: '+35%' }
    ];
    
    ecuProfiles[vehicleId] = profiles;
    
    container.innerHTML = profiles.map((profile, index) => `
        <div class=\"profile-card ${profile.modified ? 'modified' : 'stock'}\">
            <h4>${profile.name}</h4>
            <div class=\"profile-stats\">
                <span>HP: ${profile.hp}</span>
                <span>Torque: ${profile.torque}</span>
            </div>
            <button class=\"btn-small\" onclick=\"selectProfile('${vehicleId}', ${index})\">SELECT</button>
        </div>
    `).join('');
}

function createNewProfile() {
    if (!selectedVehicle) {
        alert('Please select a vehicle first');
        return;
    }
    
    const name = prompt('Enter profile name:');
    if (!name) return;
    
    const profiles = ecuProfiles[selectedVehicle.id] || [];
    profiles.push({
        name: name,
        modified: true,
        hp: '+Custom',
        torque: '+Custom',
        created: new Date().toISOString()
    });
    
    ecuProfiles[selectedVehicle.id] = profiles;
    localStorage.setItem(`ecu-profiles-${selectedVehicle.id}`, JSON.stringify(profiles));
    loadECUProfiles(selectedVehicle.id);
}

function selectProfile(vehicleId, index) {
    const profiles = ecuProfiles[vehicleId];
    if (profiles && profiles[index]) {
        alert(`Profile "${profiles[index].name}" selected`);
    }
}

// ============ BACKUP TUNES ============

function loadBackupTunes(vehicleId) {
    const container = document.getElementById('backupTunes');
    if (!container) return;
    
    const savedBackups = localStorage.getItem(`backups-${vehicleId}`);
    const backups = savedBackups ? JSON.parse(savedBackups) : [];
    
    backupTunes[vehicleId] = backups;
    
    if (backups.length === 0) {
        container.innerHTML = '<p class=\"empty-state\">No backups created yet.</p>';
        return;
    }
    
    container.innerHTML = backups.map((backup, index) => `
        <div class=\"backup-item\">
            <div class=\"backup-icon\">💾</div>
            <div class=\"backup-info\">
                <strong>${backup.name}</strong>
                <span>${new Date(backup.created).toLocaleString()}</span>
                <small>Size: ${backup.size || '2.5 MB'}</small>
            </div>
            <button class=\"btn-small\" onclick=\"restoreBackup('${vehicleId}', ${index})\">RESTORE</button>
        </div>
    `).join('');
}

function createBackup() {
    if (!selectedVehicle) {
        alert('Please select a vehicle first');
        return;
    }
    
    const name = prompt('Enter backup name:', `Backup ${new Date().toLocaleDateString()}`);
    if (!name) return;
    
    const backups = backupTunes[selectedVehicle.id] || [];
    backups.push({
        name: name,
        created: new Date().toISOString(),
        size: '2.5 MB',
        vehicleId: selectedVehicle.id
    });
    
    backupTunes[selectedVehicle.id] = backups;
    localStorage.setItem(`backups-${selectedVehicle.id}`, JSON.stringify(backups));
    loadBackupTunes(selectedVehicle.id);
    alert('Backup created successfully!');
}

function restoreBackup(vehicleId, index) {
    const backups = backupTunes[vehicleId];
    if (backups && backups[index]) {
        if (confirm(`Restore backup "${backups[index].name}"?`)) {
            alert('Backup restored successfully!');
        }
    }
}

// ============ STOCK MAPPING ============

function loadStockMapping(vehicleId) {
    const container = document.getElementById('stockMapping');
    if (!container) return;
    
    const vehicle = allVehicles.find(v => v.id === vehicleId) || selectedVehicle;
    if (!vehicle) return;
    
    container.innerHTML = `
        <div class=\"mapping-grid\">
            <div class=\"map-section\">
                <h4>Fuel Map</h4>
                <div class=\"map-data\">
                    <span>RPM Range: 1000-7000</span>
                    <span>Load: 0-100%</span>
                    <span>Cells: 16x16</span>
                </div>
            </div>
            <div class=\"map-section\">
                <h4>Ignition Timing</h4>
                <div class=\"map-data\">
                    <span>Advance: 10-35°</span>
                    <span>Retard: -5-0°</span>
                    <span>Cells: 16x16</span>
                </div>
            </div>
            <div class=\"map-section\">
                <h4>Boost Control</h4>
                <div class=\"map-data\">
                    <span>Target: ${vehicle.engines && vehicle.engines[0].includes('Turbo') ? '15-25 PSI' : 'N/A'}</span>
                    <span>Wastegate: PWM</span>
                </div>
            </div>
        </div>
    `;
}

function exportStockMap() {
    if (!selectedVehicle) {
        alert('Please select a vehicle first');
        return;
    }
    alert(`Stock map for ${selectedVehicle.manufacturer} ${selectedVehicle.model} exported successfully!`);
}

// ============ VIN DECODER FUNCTIONS ============
function decodeVIN() {
    const vinInput = document.getElementById('vinInput');
    const resultsDiv = document.getElementById('vinResults');
    const vin = vinInput ? vinInput.value.trim().toUpperCase() : '';
    
    if (vin.length !== 17) {
        resultsDiv.innerHTML = '<p style="color: var(--neon-red);">⚠️ VIN must be exactly 17 characters</p>';
        return;
    }
    
    // Decode VIN (simplified decoder)
    const wmi = vin.substring(0, 3);
    const vds = vin.substring(3, 9);
    const vis = vin.substring(9, 17);
    const year = decodeModelYear(vin.charAt(9));
    
    resultsDiv.innerHTML = `
        <div class="vin-result">
            <h3>📋 VIN Decoded Successfully</h3>
            <div class="vin-sections">
                <div class="vin-section">
                    <h4>World Manufacturer ID (WMI)</h4>
                    <p><strong>${wmi}</strong> - ${getManufacturerFromWMI(wmi)}</p>
                </div>
                <div class="vin-section">
                    <h4>Vehicle Descriptor Section (VDS)</h4>
                    <p><strong>${vds}</strong></p>
                </div>
                <div class="vin-section">
                    <h4>Vehicle Identifier Section (VIS)</h4>
                    <p><strong>${vis}</strong></p>
                    <p>Model Year: <strong>${year}</strong></p>
                </div>
            </div>
        </div>
    `;
}

function decodeModelYear(char) {
    const yearCodes = {
        'A': 2010, 'B': 2011, 'C': 2012, 'D': 2013, 'E': 2014,
        'F': 2015, 'G': 2016, 'H': 2017, 'J': 2018, 'K': 2019,
        'L': 2020, 'M': 2021, 'N': 2022, 'P': 2023, 'R': 2024,
        'S': 2025
    };
    return yearCodes[char] || 'Unknown';
}

function getManufacturerFromWMI(wmi) {
    const manufacturers = {
        '1FA': 'Ford USA', '1FT': 'Ford Truck', '1G1': 'Chevrolet',
        '1GC': 'Chevrolet Truck', '2B3': 'Dodge', 'WBA': 'BMW',
        'WVW': 'Volkswagen', 'JHM': 'Honda', '5YJ': 'Tesla',
        'WAU': 'Audi', 'WDB': 'Mercedes-Benz', 'ZFF': 'Ferrari'
    };
    return manufacturers[wmi] || 'Unknown Manufacturer';
}

// ============ ECU SCANNER FUNCTIONS ============
let ecuScanInterval = null;

function startECUScan() {
    const resultsDiv = document.getElementById('ecuScanResults');
    resultsDiv.innerHTML = '<p>🔍 Scanning for ECUs...</p>';
    
    // Simulate ECU scan
    let progress = 0;
    ecuScanInterval = setInterval(() => {
        progress += 10;
        if (progress >= 100) {
            clearInterval(ecuScanInterval);
            resultsDiv.innerHTML = `
                <div class="scan-results">
                    <h3>✅ Scan Complete</h3>
                    <div class="ecu-found">
                        <p>🔧 Engine ECU - Detected at 0x7E0</p>
                        <p>🔧 Transmission ECU - Detected at 0x7E1</p>
                        <p>🔧 ABS Module - Detected at 0x7B0</p>
                        <p>🔧 Airbag Module - Detected at 0x770</p>
                    </div>
                </div>
            `;
        } else {
            resultsDiv.innerHTML = `<p>🔍 Scanning... ${progress}%</p>`;
        }
    }, 300);
}

function stopECUScan() {
    if (ecuScanInterval) {
        clearInterval(ecuScanInterval);
        ecuScanInterval = null;
    }
    document.getElementById('ecuScanResults').innerHTML = '<p class="empty-state">Scan stopped</p>';
}

// ============ VOLTAGE METER FUNCTIONS ============
let voltageInterval = null;

function startVoltageMonitor() {
    const voltageValue = document.getElementById('voltageValue');
    const voltageBar = document.getElementById('voltageBar');
    
    voltageInterval = setInterval(() => {
        // Simulate voltage reading (12V nominal + small variation)
        const voltage = (12 + Math.random() * 2 - 0.5).toFixed(1);
        voltageValue.textContent = `${voltage}V`;
        
        // Update bar color based on voltage
        const percent = Math.min(100, (voltage / 15) * 100);
        if (voltageBar) {
            voltageBar.style.width = `${percent}%`;
            voltageBar.style.background = voltage > 12 ? 'var(--neon-green)' : 'var(--neon-red)';
        }
    }, 500);
}

function stopVoltageMonitor() {
    if (voltageInterval) {
        clearInterval(voltageInterval);
        voltageInterval = null;
    }
    document.getElementById('voltageValue').textContent = '--.-V';
}

// ============ TUNE MANAGER FUNCTIONS ============
function loadTunesList() {
    const tunesList = document.getElementById('tunesList');
    tunesList.innerHTML = `
        <div class="tune-card">
            <h3>🔥 Stage 1 Performance</h3>
            <p>+15% Power | +10% Torque</p>
            <button class="menu-btn" onclick="applyTune('stage1')">APPLY</button>
        </div>
        <div class="tune-card">
            <h3>💰 Economy Tune</h3>
            <p>+8% MPG | Optimized throttle</p>
            <button class="menu-btn" onclick="applyTune('economy')">APPLY</button>
        </div>
        <div class="tune-card">
            <h3>📦 Stock Backup</h3>
            <p>Original factory calibration</p>
            <button class="menu-btn" onclick="applyTune('stock')">RESTORE</button>
        </div>
    `;
}

function importTune() {
    alert('📥 Tune import dialog would open here\n\nSupported formats: .bin, .hex, .cef');
}

function createNewTune() {
    alert('➕ New tune wizard would open here\n\n1. Select base tune\n2. Modify parameters\n3. Save & flash');
}

function applyTune(tuneId) {
    alert(`⚡ Applying tune: ${tuneId}\n\nThis would flash the tune to the ECU.`);
}

// ============ ECU CLONING FUNCTIONS ============
function readECU() {
    const statusDiv = document.getElementById('cloningStatus');
    statusDiv.innerHTML = '<p>📤 Reading ECU data...</p>';
    
    setTimeout(() => {
        statusDiv.innerHTML = `
            <div class="clone-result">
                <h3>✅ ECU Read Complete</h3>
                <p>Size: 512KB</p>
                <p>Checksum: 0xA7B3C2D1</p>
                <p>Saved to: ecu_backup_${Date.now()}.bin</p>
            </div>
        `;
    }, 2000);
}

function writeECU() {
    if (!confirm('⚠️ WARNING: This will overwrite the ECU data. Continue?')) return;
    
    const statusDiv = document.getElementById('cloningStatus');
    statusDiv.innerHTML = '<p>📥 Writing ECU data...</p>';
    
    setTimeout(() => {
        statusDiv.innerHTML = '<div class="clone-result"><h3>✅ ECU Write Complete</h3></div>';
    }, 3000);
}

function cloneECU() {
    alert('🔄 ECU Clone Mode\n\n1. Connect source ECU\n2. Read data\n3. Connect target ECU\n4. Write data');
}

// ============ MODULE INSTALLER FUNCTIONS ============
function installModule(moduleId) {
    alert(`📦 Installing module: ${moduleId}\n\nThis would install the selected module.`);
}

// ============ WIRING DIAGRAM FUNCTIONS ============
function loadDiagram(diagramType) {
    const display = document.getElementById('diagramDisplay');
    if (!diagramType) {
        display.innerHTML = '';
        return;
    }
    
    const diagrams = {
        'obd2': `
            <div class="diagram">
                <h3>OBD-II Connector Pinout (J1962)</h3>
                <pre style="font-family: monospace; background: #000; padding: 1rem; border-radius: 4px;">
    ___________
   /  1  2  3  \\
  |  4  5  6  7  |
  |  8  9 10 11  |
   \\ 12 13 14 15 16/
    ¯¯¯¯¯¯¯¯¯¯¯
 2: J1850 Bus+    7: K-Line
 4: Chassis GND  14: CAN-L
 5: Signal GND   15: ISO 9141-2
 6: CAN-H        16: Battery+
                </pre>
            </div>
        `,
        'can-bus': `
            <div class="diagram">
                <h3>CAN Bus Network Topology</h3>
                <pre style="font-family: monospace; background: #000; padding: 1rem; border-radius: 4px;">
 [ECU] ────┬──── [TCM]
           │
    CAN-H ─┼─ 120Ω ─┬─ CAN-H
    CAN-L ─┼────────┼─ CAN-L
           │        │
         [ABS]    [BCM]
                </pre>
            </div>
        `,
        'ecu-pinout': '<div class="diagram"><h3>ECU Pinout</h3><p>Select a vehicle to view specific ECU pinout</p></div>'
    };
    
    display.innerHTML = diagrams[diagramType] || '<p>Diagram not found</p>';
}

// ============ PERFORMANCE TUNING FUNCTIONS ============
function showTuneTab(tabName) {
    const content = document.getElementById('tuningContent');
    
    const tabContent = {
        'maps': '<p>Fuel map editor - Select vehicle first</p>',
        'timing': '<p>Ignition timing editor - Select vehicle first</p>',
        'boost': '<p>Boost control editor - Select vehicle first</p>'
    };
    
    content.innerHTML = tabContent[tabName] || '';
}
