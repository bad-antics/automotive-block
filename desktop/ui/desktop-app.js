// BlackFlag v2.0 - Desktop Edition Specific Code
// Handles Electron IPC and native desktop features

// Check if running in Electron
const isElectron = () => {
    return typeof window !== 'undefined' && 
           typeof window.electron !== 'undefined';
};

// Initialize desktop-specific features
async function initializeDesktopApp() {
    console.log('🖥️ Initializing BlackFlag Desktop Edition...');
    
    // Set up window controls
    setupWindowControls();
    
    // Check backend status
    checkBackendStatus();
    
    // Set up app status monitoring
    setupStatusMonitoring();
    
    // Get app version
    if (isElectron()) {
        const version = await window.electron.getVersion();
        document.getElementById('versionDisplay').textContent = `Version: ${version}`;
    }
    
    // Load vehicle database
    await loadVehicleDatabase();
}

// ============ WINDOW CONTROLS ============
function setupWindowControls() {
    if (!isElectron()) return;
    
    const minimizeBtn = document.getElementById('minimizeBtn');
    const maximizeBtn = document.getElementById('maximizeBtn');
    const closeBtn = document.getElementById('closeBtn');
    
    if (minimizeBtn) {
        minimizeBtn.addEventListener('click', () => {
            window.electron.minimizeWindow();
        });
    }
    
    if (maximizeBtn) {
        maximizeBtn.addEventListener('click', () => {
            window.electron.toggleMaximize();
        });
    }
    
    if (closeBtn) {
        closeBtn.addEventListener('click', () => {
            window.electron.closeWindow();
        });
    }
}

// ============ BACKEND STATUS ============
async function checkBackendStatus() {
    try {
        const response = await fetch('http://localhost:3000/api/health');
        const data = await response.json();
        
        if (data.status === 'ok') {
            updateStatus(true);
            console.log('✅ Backend is online');
        } else {
            updateStatus(false);
            console.warn('⚠️ Backend status unknown');
        }
    } catch (error) {
        updateStatus(false);
        console.error('❌ Backend connection failed:', error);
    }
}

function updateStatus(online) {
    const status = document.getElementById('appStatus');
    const statusText = document.getElementById('statusText');
    const backendStatus = document.getElementById('backendStatus');
    
    if (online) {
        status.classList.add('online');
        status.classList.remove('offline');
        statusText.textContent = 'Backend: Online';
        if (backendStatus) {
            backendStatus.innerHTML = '🔌 Backend: Connected';
        }
    } else {
        status.classList.add('offline');
        status.classList.remove('online');
        statusText.textContent = 'Backend: Offline';
        if (backendStatus) {
            backendStatus.innerHTML = '⚠️ Backend: Disconnected';
        }
    }
}

function setupStatusMonitoring() {
    // Check backend status every 5 seconds
    setInterval(checkBackendStatus, 5000);
}

// ============ VEHICLE DATABASE ============
async function loadVehicleDatabase() {
    try {
        console.log('🔄 Loading vehicle database...');
        const response = await fetch('http://localhost:3000/api/vehicles/list');
        const data = await response.json();
        
        console.log('📦 API Response:', data);
        
        // Handle both array and object response formats
        let vehicles = [];
        if (Array.isArray(data)) {
            vehicles = data;
        } else if (data.vehicles && Array.isArray(data.vehicles)) {
            vehicles = data.vehicles;
        }
        
        if (vehicles.length > 0) {
            window.allVehicles = vehicles;
            allVehicles = vehicles;
            populateManufacturers(vehicles);
            populateVehicleSelector(vehicles);
            console.log(`✅ Loaded ${vehicles.length} vehicles`);
        } else {
            console.warn('⚠️ No vehicles found in response');
        }
    } catch (error) {
        console.error('❌ Failed to load vehicles:', error);
        // Use fallback vehicles if available
        if (typeof allVehicles !== 'undefined' && allVehicles.length > 0) {
            populateVehicleSelector(allVehicles);
        }
    }
}

function populateVehicleSelector(vehicles) {
    const selector = document.getElementById('vehicleSelector');
    if (!selector) return;
    
    // Clear existing options
    selector.innerHTML = '<option value="">⚔️ Choose a vehicle...</option>';
    
    // Add vehicles
    vehicles.forEach(vehicle => {
        const option = document.createElement('option');
        option.value = vehicle.id;
        option.textContent = `${vehicle.year} ${vehicle.make} ${vehicle.model}`;
        selector.appendChild(option);
    });
}

// ============ SETTINGS/PREFERENCES ============
function openSettings() {
    alert('⚙️ Settings panel coming soon!\n\n• Hardware Configuration\n• UI Preferences\n• Backend Settings\n• About & Version Info');
}

// ============ ENHANCED LIBRARY FUNCTIONS ============
function switchLibraryTab(tabName) {
    // Hide all library tabs
    const tabs = document.querySelectorAll('.library-content');
    tabs.forEach(tab => tab.classList.remove('active'));
    
    // Deactivate all tab buttons
    const tabBtns = document.querySelectorAll('.library-tab');
    tabBtns.forEach(btn => btn.classList.remove('active'));
    
    // Show selected tab
    const selectedTab = document.getElementById(`lib-${tabName}`);
    if (selectedTab) {
        selectedTab.classList.add('active');
    }
    
    // Activate selected button
    event.target.classList.add('active');
}

function searchVehicleDatabase(query) {
    if (!query) {
        alert('Please enter a vehicle name to search');
        return;
    }
    
    const results = window.allVehicles.filter(v => 
        v.make.toLowerCase().includes(query.toLowerCase()) ||
        v.model.toLowerCase().includes(query.toLowerCase()) ||
        v.year.toString().includes(query)
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
            <p><strong>Engine:</strong> ${vehicle.engine}</p>
            <p><strong>Horsepower:</strong> ${vehicle.horsepower}hp</p>
            <p><strong>Torque:</strong> ${vehicle.torque}Nm</p>
            <p><strong>ECUs:</strong> ${vehicle.ecus.length}</p>
        `;
        card.addEventListener('click', () => selectVehicleFromSearch(vehicle));
        grid.appendChild(card);
    });
}

function selectVehicleFromSearch(vehicle) {
    window.selectedVehicle = vehicle;
    alert(`✅ Selected: ${vehicle.year} ${vehicle.make} ${vehicle.model}\n\nECUs Available: ${vehicle.ecus.join(', ')}`);
}

function viewGuide(guideName) {
    const guides = {
        'dpf-removal': {
            title: 'DPF Delete Guide',
            content: `
            # DPF Delete & Disable Procedures
            
            ## Overview
            DPF (Diesel Particulate Filter) removal and disabling techniques for various platforms.
            
            ## Pre-Requirements
            • Full system backup
            • ECU file from target vehicle
            • Tuning software (Winols, HP Tuners, etc.)
            • Hardware: J2534 cable + laptop
            
            ## Common DPF Methods
            1. **Software Delete**: Disable via tune file
            2. **Hardware Delete**: Physical filter removal
            3. **Hybrid Approach**: Software + blank filter
            
            ⚠️ WARNING: DPF deletion may violate emissions regulations.
            `
        },
        'scr-tuning': {
            title: 'SCR System Tuning',
            content: `
            # SCR System Modification & Tuning
            
            ## What is SCR?
            Selective Catalytic Reduction - uses diesel exhaust fluid to reduce NOx emissions.
            
            ## Modification Options
            • Increase SCR effectiveness
            • Reduce AdBlue consumption
            • Disable NOx sensor
            • Remap SCR thresholds
            
            ## Safety Considerations
            • Monitor temperatures
            • Check sensor readings
            • Validate exhaust composition
            
            ⚠️ WARNING: SCR modifications affect emissions testing.
            `
        },
        'egr-disabled': {
            title: 'EGR Disable Procedures',
            content: `
            # EGR (Exhaust Gas Recirculation) Disabling
            
            ## What is EGR?
            Recirculates exhaust gases to reduce emissions and lower combustion temperature.
            
            ## Benefits of Disabling
            • Improved fuel economy
            • Better engine performance
            • Reduced carbon buildup
            • Lower operating temperatures
            
            ## Implementation
            1. Disable via ECU tune
            2. Block physical valve
            3. Install blanking plates
            4. Reprogram sensor inputs
            
            ⚠️ WARNING: EGR disable requires proper testing.
            `
        },
        'turbo-boost': {
            title: 'Turbo Boost Control',
            content: `
            # Turbocharger Boost Control & Tuning
            
            ## Standard Boost Control
            • Base boost level
            • Ramp-up rate
            • Max boost protection
            • Waste gate duty cycle
            
            ## Advanced Modifications
            • Increase peak boost pressure
            • Extended high-boost range
            • Custom boost curves
            • Temperature compensation
            
            ## Safety Limits
            • Engine block pressure limits
            • Intercooler capacity
            • Fuel octane rating
            • Oil cooling requirements
            
            ⚠️ WARNING: Excessive boost can cause engine damage.
            `
        },
        'fuel-injection': {
            title: 'Fuel Injection Mapping',
            content: `
            # Fuel Injection System Optimization
            
            ## Injection Parameters
            • Timing adjustments
            • Pressure maps
            • Duration control
            • Multiple injection events
            
            ## Performance Tuning
            • Increase fuel volume at high RPM
            • Advance injection timing
            • Custom rail pressure maps
            • Load-based fuel curves
            
            ## Quality Control
            • Emissions testing
            • Fuel consumption monitoring
            • Knock detection
            • Combustion analysis
            
            ⚠️ WARNING: Incorrect fuel mapping can damage injectors.
            `
        },
        'transmission-tune': {
            title: 'Transmission Tuning',
            content: `
            # Automatic Transmission Remapping
            
            ## TCU Programming
            • Shift points optimization
            • Pressure mapping
            • Torque converter lockup
            • Downshift behavior
            
            ## Performance Features
            • Quicker shift speeds
            • Eliminate unnecessary shifts
            • Custom launch control
            • Manual mode enhancements
            
            ## Testing Requirements
            • Transmission temperature monitoring
            • Pressure validation
            • Durability testing
            
            ⚠️ WARNING: Aggressive transmission tuning reduces longevity.
            `
        }
    };
    
    const guide = guides[guideName];
    if (guide) {
        const content = `${guide.title}\n\n${guide.content}`;
        showModal('📖 Diagnostic Guide', content);
    }
}

function loadWiringDiagram(diagramName) {
    const diagrams = {
        'obd2-pinout': '🔌 OBD2 Pinout Diagram\n\nPin 1: Power (12V)\nPin 2: J1962 Bus -\nPin 3: TX (K-Line)\nPin 4: Chassis Ground\nPin 5: Signal Ground\nPin 6: CAN High\nPin 7: K-Line\nPin 8: Reserved\nPin 9: Power (12V)\nPin 10: J1962 Bus +\nPin 11: Reserved\nPin 12: Reserved\nPin 13: Reserved\nPin 14: CAN Low\nPin 15: J1850 Bus -\nPin 16: Power (12V)',
        'can-bus': '🔗 CAN Bus Wiring\n\nCAN High: Pin 6 (White/Red)\nCAN Low: Pin 14 (Brown/White)\nShield: Pins 4, 5, 9, 16 (Ground)\nData Rate: 500Kbps (OBD2)\nTermination: 120Ω resistors on both ends',
        'j1939-truck': '🚛 J1939 Heavy Truck CAN\n\nPurple: CAN High\nGreen: CAN Low\nBlack: Ground\nYellow: 12V Power\nBaud Rate: 250Kbps\nCommon on: Cummins, Duramax, PowerStroke',
        'kwp-serial': '📡 KWP Serial Protocol (K-Line)\n\nGreen: K-Line TX/RX\nWhite: L-Line (optional)\nBlack: Ground\nRed: 12V Power\nBaud: 9600bps (can vary)\nUsed on: Older vehicles, some diagnostics'
    };
    
    const diagram = diagrams[diagramName];
    if (diagram) {
        showModal('⚡ Wiring Diagram', diagram);
    }
}

function showModal(title, content) {
    alert(`${title}\n\n${content}`);
}

// ============ INITIALIZE ON LOAD ============
document.addEventListener('DOMContentLoaded', () => {
    console.log('🖥️ Desktop Edition loaded');
    initializeDesktopApp();
});

// ============ DESKTOP WINDOW CONTROLS (mock if not in Electron) ============
if (!isElectron()) {
    window.electron = {
        getVersion: async () => '2.0.0 (Web Preview)',
        getAppInfo: async () => ({ name: 'BlackFlag', version: '2.0.0' }),
        checkBackend: async () => true,
        minimizeWindow: () => console.log('Would minimize'),
        toggleMaximize: () => console.log('Would maximize'),
        closeWindow: () => console.log('Would close')
    };
}
