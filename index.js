// Automotive Block - ECU Diagnostic Tool
// Main entry point with OBD-II, CAN Bus, and comprehensive diagnostics

const express = require('express');
const path = require('path');
const OBD2Handler = require('./obd2');
const CANHandler = require('./canbus');
const DiagnosticEngine = require('./diagnostics');
const ecuDatabase = require('./ecudb');

const app = express();
const PORT = 3000;

// Initialize handlers
const obd2 = new OBD2Handler();
const canbus = new CANHandler();
const diagnostics = new DiagnosticEngine(ecuDatabase);

// Initialize CAN buses
canbus.initializeBus('CAN0', 500000);
canbus.initializeBus('CAN1', 250000);

// Middleware
app.use(express.json());
app.use(express.static('public'));

// ============ API ROUTES ============

// Basic status
app.get('/api/status', (req, res) => {
  res.json({
    status: 'running',
    version: '1.0.0',
    name: 'Automotive Block',
    modules: ['OBD2', 'CAN Bus', 'Diagnostics', 'ECU Database']
  });
});

// ============ OBD-II ROUTES ============

app.get('/api/obd2/pids', (req, res) => {
  res.json({
    status: 'success',
    pids: obd2.readAllPIDs()
  });
});

app.get('/api/obd2/pid/:pid', (req, res) => {
  const param = obd2.getParameter(req.params.pid);
  if (!param) {
    return res.status(404).json({ error: 'PID not found' });
  }
  res.json({ pid: req.params.pid, ...param });
});

app.get('/api/obd2/read', (req, res) => {
  res.json({
    status: 'success',
    data: obd2.simulateData()
  });
});

app.get('/api/obd2/dtc', (req, res) => {
  res.json({
    status: 'success',
    codes: obd2.readDTC()
  });
});

app.post('/api/obd2/dtc/clear', (req, res) => {
  res.json(obd2.clearDTC());
});

// ============ CAN BUS ROUTES ============

app.post('/api/can/bus/init', (req, res) => {
  const { busId, baudrate } = req.body;
  res.json(canbus.initializeBus(busId, baudrate));
});

app.get('/api/can/bus/:busId/status', (req, res) => {
  const status = canbus.getBusStatus(req.params.busId);
  if (!status) {
    return res.status(404).json({ error: 'Bus not found' });
  }
  res.json(status);
});

app.post('/api/can/send', (req, res) => {
  const { busId, canId, data } = req.body;
  res.json(canbus.sendMessage(busId, canId, data));
});

app.get('/api/can/receive/:busId/:canId', (req, res) => {
  const messages = canbus.receiveMessage(req.params.busId, parseInt(req.params.canId));
  res.json({ count: messages.length, messages });
});

app.get('/api/can/messages', (req, res) => {
  res.json({ messages: canbus.getAllMessages() });
});

app.get('/api/can/simulate', (req, res) => {
  res.json(canbus.simulateCANData());
});

// ============ ECU DATABASE ROUTES ============

app.get('/api/ecu/vehicles', (req, res) => {
  res.json({
    count: Object.keys(ecuDatabase.vehicles).length,
    vehicles: Object.entries(ecuDatabase.vehicles).map(([id, v]) => ({
      id,
      make: v.make,
      model: v.model,
      year: v.year,
      engine: v.engine,
      protocols: v.protocols
    }))
  });
});

app.get('/api/ecu/vehicle/:vehicleId', (req, res) => {
  const vehicle = ecuDatabase.vehicles[req.params.vehicleId];
  if (!vehicle) {
    return res.status(404).json({ error: 'Vehicle not found' });
  }
  res.json(vehicle);
});

app.get('/api/ecu/models', (req, res) => {
  res.json(ecuDatabase.ecuModels);
});

app.get('/api/ecu/dtc', (req, res) => {
  res.json(ecuDatabase.commonDTC);
});

// ============ DIAGNOSTICS ROUTES ============

app.post('/api/diagnostics/full', (req, res) => {
  const { vehicleId, liveData } = req.body;
  
  const defaultData = {
    rpm: 2500,
    temp: 90,
    fuel_pressure: 45,
    lambda: 1.0,
    gear: 'D'
  };

  const data = { ...defaultData, ...liveData };
  const results = diagnostics.runFullDiagnostic(vehicleId, data);
  
  res.json(results);
});

app.get('/api/diagnostics/alerts', (req, res) => {
  res.json({
    alerts: diagnostics.getActiveAlerts()
  });
});

app.get('/api/diagnostics/history', (req, res) => {
  const limit = req.query.limit || 10;
  res.json({
    history: diagnostics.getDiagnosticHistory(limit)
  });
});

app.post('/api/diagnostics/clear', (req, res) => {
  res.json(diagnostics.clearDiagnosticHistory());
});

// ============ TUNING ROUTES ============

app.get('/api/tuning/params/:vehicleId', (req, res) => {
  const vehicle = ecuDatabase.vehicles[req.params.vehicleId];
  if (!vehicle) {
    return res.status(404).json({ error: 'Vehicle not found' });
  }
  res.json({
    vehicle: `${vehicle.make} ${vehicle.model} ${vehicle.year}`,
    tuningParams: vehicle.tuningParams,
    limits: vehicle.limits
  });
});

app.post('/api/tuning/apply', (req, res) => {
  const { vehicleId, params } = req.body;
  const vehicle = ecuDatabase.vehicles[vehicleId];
  
  if (!vehicle) {
    return res.status(404).json({ error: 'Vehicle not found' });
  }

  // Validate parameters
  const validation = {};
  Object.entries(params).forEach(([key, value]) => {
    const param = vehicle.tuningParams[key];
    if (param) {
      validation[key] = {
        requested: value,
        valid: value >= param.min && value <= param.max,
        range: `${param.min} - ${param.max} ${param.unit}`
      };
    }
  });

  res.json({
    status: 'parameters_validated',
    vehicle: vehicleId,
    validation
  });
});

// ============ START SERVER ============

app.listen(PORT, () => {
  console.log(`
╔═══════════════════════════════════════╗
║   🚗 AUTOMOTIVE BLOCK v1.0.0         ║
║   ECU Diagnostic & Tuning Tool       ║
╚═══════════════════════════════════════╝

Server running on http://localhost:${PORT}

Available APIs:
  - OBD-II Protocol Support
  - CAN Bus Communication
  - ECU Database (20+ vehicles)
  - Real-time Diagnostics
  - Tuning Parameter Control
  
Start exploring: http://localhost:${PORT}
  `);
});
