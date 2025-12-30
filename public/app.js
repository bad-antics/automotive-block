// Automotive Block - Complete Frontend Application
// Real-time diagnostics, OBD-II, CAN Bus, and tuning

let engineChart, canChart;
const API_BASE = '/api';

// ============ INITIALIZATION ============

document.addEventListener('DOMContentLoaded', async () => {
  console.log('🚗 Automotive Block Frontend Initializing...');
  
  initializeUI();
  await fetchSystemStatus();
  await loadVehicles();
  setupEventListeners();
  initializeCharts();
  
  console.log('✅ Frontend Ready');
});

// ============ UI INITIALIZATION ============

function initializeUI() {
  // Tab switching
  document.querySelectorAll('.nav-item').forEach(item => {
    item.addEventListener('click', (e) => {
      document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
      document.querySelectorAll('.tab-content').forEach(t => t.classList.remove('active'));
      
      e.target.classList.add('active');
      const tabId = e.target.dataset.tab;
      document.getElementById(tabId).classList.add('active');
    });
  });
}

async function fetchSystemStatus() {
  try {
    const response = await fetch(`${API_BASE}/status`);
    const data = await response.json();
    
    document.getElementById('systemStatus').textContent = `✓ ${data.status}`;
    document.getElementById('moduleCount').textContent = data.modules.length;
    document.querySelector('.nav-status').classList.add('connected');
    document.querySelector('.nav-status').textContent = '● Connected';
  } catch (error) {
    console.error('❌ Connection error:', error);
    document.querySelector('.nav-status').classList.add('disconnected');
  }
}

// ============ VEHICLE MANAGEMENT ============

async function loadVehicles() {
  try {
    const response = await fetch(`${API_BASE}/ecu/vehicles`);
    const data = await response.json();
    
    const vehiclesGrid = document.getElementById('vehiclesGrid');
    vehiclesGrid.innerHTML = '';
    
    data.vehicles.forEach(vehicle => {
      const card = document.createElement('div');
      card.className = 'vehicle-card';
      card.innerHTML = `
        <h4>${vehicle.make} ${vehicle.model}</h4>
        <div class="vehicle-specs">
          <p><strong>Year:</strong> ${vehicle.year}</p>
          <p><strong>Engine:</strong> ${vehicle.engine}</p>
        </div>
        <div class="protocols">
          ${vehicle.protocols.map(p => `<span class="protocol-badge">${p}</span>`).join('')}
        </div>
      `;
      vehiclesGrid.appendChild(card);
    });

    // Populate select dropdowns
    const selects = ['diagnosticsVehicleSelect', 'tuningVehicleSelect'];
    selects.forEach(selectId => {
      const select = document.getElementById(selectId);
      select.innerHTML = '<option value="">Select Vehicle...</option>';
      data.vehicles.forEach(vehicle => {
        const option = document.createElement('option');
        option.value = vehicle.id;
        option.textContent = `${vehicle.make} ${vehicle.model} ${vehicle.year}`;
        select.appendChild(option);
      });
    });
  } catch (error) {
    console.error('Error loading vehicles:', error);
  }
}

// ============ EVENT LISTENERS ============

function setupEventListeners() {
  // OBD-II Events
  document.getElementById('readOBD2')?.addEventListener('click', readOBD2Data);
  document.getElementById('clearDTC')?.addEventListener('click', clearDTC);

  // CAN Bus Events
  document.getElementById('startCANMonitor')?.addEventListener('click', startCANMonitor);
  document.getElementById('simulateCANData')?.addEventListener('click', simulateCANData);

  // Diagnostics Events
  document.getElementById('runFullDiagnostic')?.addEventListener('click', runFullDiagnostic);

  // Tuning Events
  document.getElementById('loadTuningParams')?.addEventListener('click', loadTuningParams);
}

// ============ OBD-II FUNCTIONS ============

async function readOBD2Data() {
  try {
    const response = await fetch(`${API_BASE}/obd2/read`);
    const data = await response.json();
    
    const pidsList = document.getElementById('pidsList');
    pidsList.innerHTML = '';
    
    Object.entries(data.data).forEach(([pid, param]) => {
      const item = document.createElement('div');
      item.className = 'param-item';
      item.innerHTML = `
        <strong>${param.name}</strong>
        <div>${param.value} ${param.unit}</div>
        <small>PID: ${pid}</small>
      `;
      pidsList.appendChild(item);
    });
  } catch (error) {
    console.error('Error reading OBD2:', error);
  }
}

async function clearDTC() {
  try {
    const response = await fetch(`${API_BASE}/obd2/dtc/clear`, { method: 'POST' });
    const data = await response.json();
    alert('Diagnostic Trouble Codes cleared');
    readOBD2Data();
  } catch (error) {
    console.error('Error clearing DTC:', error);
  }
}

// ============ CAN BUS FUNCTIONS ============

async function startCANMonitor() {
  try {
    const response = await fetch(`${API_BASE}/can/simulate`);
    const data = await response.json();
    
    console.log('CAN Data:', data);
    alert('CAN Bus monitoring started (simulated data)');
  } catch (error) {
    console.error('Error starting CAN monitor:', error);
  }
}

async function simulateCANData() {
  try {
    const response = await fetch(`${API_BASE}/can/messages`);
    const data = await response.json();
    
    alert(`Received ${data.messages.length} CAN messages`);
  } catch (error) {
    console.error('Error simulating CAN data:', error);
  }
}

// ============ DIAGNOSTICS FUNCTIONS ============

async function runFullDiagnostic() {
  const vehicleId = document.getElementById('diagnosticsVehicleSelect').value;
  
  if (!vehicleId) {
    alert('Please select a vehicle');
    return;
  }

  try {
    const response = await fetch(`${API_BASE}/diagnostics/full`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        vehicleId,
        liveData: {
          rpm: 2500,
          temp: 90,
          fuel_pressure: 45,
          lambda: 1.0
        }
      })
    });

    const results = await response.json();
    displayDiagnosticResults(results);
  } catch (error) {
    console.error('Error running diagnostic:', error);
  }
}

function displayDiagnosticResults(results) {
  const resultsDiv = document.getElementById('diagnosticResults');
  
  const healthClass = results.score >= 85 ? '' : 
                      results.score >= 70 ? 'warning' : 'critical';
  
  resultsDiv.innerHTML = `
    <div class="diagnostic-card">
      <h4>Vehicle Health Score</h4>
      <div class="health-score ${healthClass}">${results.score}/100</div>
      <p>${results.vehicle.make} ${results.vehicle.model} ${results.vehicle.year}</p>
    </div>

    <div class="diagnostic-card">
      <h4>Engine Status</h4>
      <p><strong>RPM:</strong> ${results.systems.engine.rpm}</p>
      <p><strong>Temperature:</strong> ${results.systems.engine.temperature}°C</p>
      <p><strong>Status:</strong> ${results.systems.engine.temp_status}</p>
    </div>

    <div class="diagnostic-card">
      <h4>Emissions</h4>
      <p><strong>Lambda:</strong> ${results.systems.emissions.lambda_value.toFixed(2)}</p>
      <p><strong>Status:</strong> ${results.systems.emissions.status}</p>
    </div>

    ${results.alerts.length > 0 ? `
      <div class="diagnostic-card" style="background: #fee;">
        <h4>Active Alerts</h4>
        ${results.alerts.map(a => `
          <p><strong>${a.level}:</strong> ${a.message}</p>
        `).join('')}
      </div>
    ` : '<p>✓ No alerts</p>'}
  `;
}

// ============ TUNING FUNCTIONS ============

async function loadTuningParams() {
  const vehicleId = document.getElementById('tuningVehicleSelect').value;
  
  if (!vehicleId) {
    alert('Please select a vehicle');
    return;
  }

  try {
    const response = await fetch(`${API_BASE}/tuning/params/${vehicleId}`);
    const data = await response.json();
    
    const panel = document.getElementById('tuningPanel');
    panel.innerHTML = `<h3>${data.vehicle} - Tuning Parameters</h3>`;
    
    const group = document.createElement('div');
    group.className = 'tuning-group';
    
    Object.entries(data.tuningParams).forEach(([key, param]) => {
      const paramDiv = document.createElement('div');
      paramDiv.className = 'tuning-param';
      paramDiv.innerHTML = `
        <label for="${key}">${key.replace(/_/g, ' ').toUpperCase()}</label>
        <div style="display: flex; align-items: center; gap: 15px;">
          <input type="range" id="${key}" min="${param.min}" max="${param.max}" value="${param.default}" step="0.1">
          <span class="tuning-param-value">${param.default} ${param.unit}</span>
        </div>
      `;
      
      const input = paramDiv.querySelector('input');
      const valueSpan = paramDiv.querySelector('.tuning-param-value');
      
      input.addEventListener('input', (e) => {
        valueSpan.textContent = `${e.target.value} ${param.unit}`;
      });
      
      group.appendChild(paramDiv);
    });
    
    panel.appendChild(group);
  } catch (error) {
    console.error('Error loading tuning params:', error);
  }
}

// ============ CHART INITIALIZATION ============

function initializeCharts() {
  const engineCtx = document.getElementById('engineChart');
  if (engineCtx) {
    engineChart = new Chart(engineCtx, {
      type: 'line',
      data: {
        labels: ['00:00', '01:00', '02:00', '03:00', '04:00', '05:00'],
        datasets: [
          {
            label: 'RPM',
            data: [1200, 2500, 3000, 2800, 2200, 1500],
            borderColor: '#ff6b6b',
            backgroundColor: 'rgba(255, 107, 107, 0.1)',
            tension: 0.4,
            yAxisID: 'y'
          },
          {
            label: 'Temp (°C)',
            data: [75, 85, 92, 95, 88, 80],
            borderColor: '#51cf66',
            backgroundColor: 'rgba(81, 207, 102, 0.1)',
            tension: 0.4,
            yAxisID: 'y1'
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: true,
        interaction: { mode: 'index', intersect: false },
        scales: {
          y: { type: 'linear', display: true, position: 'left', title: { display: true, text: 'RPM' } },
          y1: { type: 'linear', display: true, position: 'right', title: { display: true, text: 'Temp' } }
        }
      }
    });
  }

  const canCtx = document.getElementById('canChart');
  if (canCtx) {
    canChart = new Chart(canCtx, {
      type: 'bar',
      data: {
        labels: ['CAN0', 'CAN1'],
        datasets: [
          {
            label: 'Messages Sent',
            data: [245, 128],
            backgroundColor: '#2a5298'
          },
          {
            label: 'Messages Received',
            data: [238, 126],
            backgroundColor: '#51cf66'
          }
        ]
      },
      options: {
        responsive: true,
        scales: { y: { beginAtZero: true } }
      }
    });
  }
}

console.log('🚗 Automotive Block v1.0.0 - Ready');
