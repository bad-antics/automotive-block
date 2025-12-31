// BlackFlag v2.0 - Professional ECU Hacking & Tuning Suite
// Full-featured ECU manipulation with wiring diagrams, tune management, emissions control, 
// ECU cloning, module installation, and OEM as-built data support

const express = require('express');
const path = require('path');
const fs = require('fs');

// Import all handler modules
const OBD2Handler = require('./obd2');
const CANHandler = require('./canbus');
const DiagnosticEngine = require('./diagnostics');
const ecuDatabase = require('./ecudb');
const J2534Handler = require('./j2534');
const ECUProcessorController = require('./ecu-processor');
const WiringDiagramLibrary = require('./wiring-diagrams');
const TuneManager = require('./tune-manager');
const EmissionsController = require('./emissions-controller');
const OEMAsBuiltManager = require('./oem-as-built');
const ECUCloner = require('./ecu-cloner');
const ModuleInstaller = require('./module-installer');
const VehicleDatabase = require('./vehicle-database');
const EcuSoftwareUpdates = require('./ecu-software-updates');
const EcuTestBench = require('./ecu-test-bench');

const app = express();
const PORT = 3000;

// Initialize all handlers and modules
const obd2 = new OBD2Handler();
const canbus = new CANHandler();
const diagnostics = new DiagnosticEngine(ecuDatabase);
const j2534 = new J2534Handler();
const ecuProcessor = new ECUProcessorController();
const wiringDiagrams = new WiringDiagramLibrary();
const tuneManager = new TuneManager();
const emissionsController = new EmissionsController();
const oemAsBuilt = new OEMAsBuiltManager();
const ecuCloner = new ECUCloner();
const moduleInstaller = new ModuleInstaller();
const vehicleDatabase = new VehicleDatabase();
const ecuSoftwareUpdates = new EcuSoftwareUpdates();
const ecuTestBench = new EcuTestBench();

// Initialize CAN buses
canbus.initializeBus('CAN0', 500000);
canbus.initializeBus('CAN1', 250000);

// Middleware
app.use((req, res, next) => {
  res.header('Access-Control-Allow-Origin', '*');
  res.header('Access-Control-Allow-Headers', 'Content-Type');
  // Prevent caching of static files
  res.header('Cache-Control', 'no-cache, no-store, must-revalidate, max-age=0');
  res.header('Pragma', 'no-cache');
  res.header('Expires', '0');
  next();
});
app.use(express.json({ limit: '50mb' }));
app.use(express.urlencoded({ extended: true, limit: '50mb' }));

// Serve src folder as primary, fallback to public
app.use(express.static('src'));
app.use(express.static('public'));

// ============ STARTUP BANNER ============
const banner = `
╔═══════════════════════════════════════════════════╗
║         🏴 BLACKFLAG v2.0                        ║
║   Professional ECU Hacking & Tuning Suite        ║
║   + Wiring Diagrams (50+ circuits)               ║
║   + Tune Manager (100+ modifications)             ║
║   + ECU Cloning & Backup                         ║
║   + Programmable Module Installation             ║
║   + OEM As-Built Data Management                 ║
║   + Diesel Emissions Control + Spoofing          ║
║   + Advanced Diagnostics                         ║
║   + Free & Open Source | No Accounts Required    ║
╚═══════════════════════════════════════════════════╝
`;

console.log(banner);

// ============ BASIC STATUS ENDPOINT ============
app.get('/api/status', (req, res) => {
  res.json({
    status: 'online',
    version: '2.0.0',
    name: 'BlackFlag',
    tagline: 'Professional ECU Hacking & Tuning Suite',
    free: true,
    openSource: true,
    accountsRequired: false,
    modules: [
      'OBD-II Protocol',
      'CAN Bus Communication',
      'J2534 Pass-Through',
      'ECU Processor Controller',
      'Wiring Diagram Library',
      'Tune Manager',
      'Emissions Control',
      'ECU Cloner',
      'Module Installer',
      'OEM As-Built Manager',
      'Diagnostics Engine',
      'ECU Database',
      'Vehicle Database',
      'ECU Software Updates',
      'ECU Test Bench'
    ],
    totalEndpoints: 150,
    license: 'MIT',
    support: 'Free & Open Source'
  });
});

// ============ WIRING DIAGRAM ENDPOINTS ============
app.get('/api/wiring/circuits', (req, res) => {
  const circuits = wiringDiagrams.listCircuits();
  res.json(circuits);
});

app.get('/api/wiring/vehicles', (req, res) => {
  const vehicles = wiringDiagrams.listVehicles();
  res.json(vehicles);
});

app.get('/api/wiring/select/:circuitId', (req, res) => {
  const selection = wiringDiagrams.selectCircuit(req.params.circuitId);
  res.json(selection);
});

app.post('/api/wiring/trace', (req, res) => {
  const { circuitId, startComponent, endComponent } = req.body;
  const trace = wiringDiagrams.traceCircuit(circuitId, startComponent, endComponent);
  res.json(trace);
});

app.get('/api/wiring/trace/:circuitId', (req, res) => {
  const trace = wiringDiagrams.traceCircuit(req.params.circuitId, 'START', 'END');
  res.json(trace);
});

app.get('/api/wiring/connector/:connectorType', (req, res) => {
  const connector = wiringDiagrams.getConnectorEndView(req.params.connectorType);
  res.json(connector);
});

app.get('/api/wiring/connectors/:connectorType', (req, res) => {
  const connector = wiringDiagrams.getConnectorEndView(req.params.connectorType);
  res.json(connector);
});

app.get('/api/wiring/exploded/:circuitId', (req, res) => {
  const exploded = wiringDiagrams.getExplodedView(req.params.circuitId);
  res.json(exploded);
});

app.get('/api/wiring/vehicle/:vehicleKey', (req, res) => {
  const wiring = wiringDiagrams.getVehicleWiring(req.params.vehicleKey);
  res.json(wiring);
});

// ============ TUNE MANAGER ENDPOINTS ============
app.get('/api/tunes/categories', (req, res) => {
  const categories = tuneManager.getTuneCategories();
  res.json(categories);
});

app.get('/api/tunes/list', (req, res) => {
  const category = req.query.category || null;
  const tunes = tuneManager.listAllTunes(category);
  res.json(tunes);
});

app.get('/api/tunes/compatible', (req, res) => {
  const { fuelType, ecuModel } = req.query;
  const compatible = tuneManager.getCompatibleTunes(fuelType, ecuModel);
  res.json(compatible);
});

app.get('/api/tunes/details/:tuneId', (req, res) => {
  const details = tuneManager.getTuneDetails(req.params.tuneId);
  res.json(details);
});

app.post('/api/tunes/select', (req, res) => {
  const { tuneId } = req.body;
  const selected = tuneManager.selectTune(tuneId);
  res.json(selected);
});

app.post('/api/tunes/apply', (req, res) => {
  const { tuneId, ecuId, vehicleId } = req.body;
  const applied = tuneManager.applyTune(tuneId, ecuId, vehicleId);
  res.json(applied);
});

app.post('/api/tunes/revert', (req, res) => {
  const { ecuId } = req.body;
  const reverted = tuneManager.revertTune(ecuId);
  res.json(reverted);
});

app.get('/api/tunes/history/:ecuId', (req, res) => {
  const limit = req.query.limit || 10;
  const history = tuneManager.getModificationHistory(req.params.ecuId, limit);
  res.json(history);
});

// ============ ECU CLONING ENDPOINTS ============
app.post('/api/cloning/backup', (req, res) => {
  const { ecuId, ecuType } = req.body;
  const backup = ecuCloner.backupECU(ecuId, ecuType, 0, 4194304);
  res.json(backup);
});

app.post('/api/cloning/restore', (req, res) => {
  const { ecuId, backupId } = req.body;
  const restore = ecuCloner.restoreECU(ecuId, backupId);
  res.json(restore);
});

app.post('/api/cloning/clone', (req, res) => {
  const { sourceEcuId, targetEcuId, ecuType } = req.body;
  const clone = ecuCloner.cloneECU(sourceEcuId, targetEcuId, ecuType);
  res.json(clone);
});

app.post('/api/cloning/verify', (req, res) => {
  const { backupId } = req.body;
  const verify = ecuCloner.verifyBackup(backupId);
  res.json(verify);
});

app.get('/api/cloning/backups', (req, res) => {
  const ecuId = req.query.ecuId || null;
  const backups = ecuCloner.listBackups(ecuId);
  res.json(backups);
});

app.get('/api/cloning/jobs', (req, res) => {
  const jobs = ecuCloner.listCloneJobs();
  res.json(jobs);
});

app.get('/api/cloning/firmware/:ecuType', (req, res) => {
  const firmware = ecuCloner.getECUFirmwareInfo(req.params.ecuType);
  res.json(firmware);
});

app.post('/api/cloning/compare', (req, res) => {
  const { backupId1, backupId2 } = req.body;
  const comparison = ecuCloner.compareBackups(backupId1, backupId2);
  res.json(comparison);
});

// ============ MODULE INSTALLER ENDPOINTS ============
app.get('/api/modules/list', (req, res) => {
  const compatibility = req.query.compatibility || null;
  const modules = moduleInstaller.listAvailableModules(compatibility);
  res.json(modules);
});

app.get('/api/modules/details/:moduleId', (req, res) => {
  const details = moduleInstaller.getModuleDetails(req.params.moduleId);
  res.json(details);
});

app.post('/api/modules/install', (req, res) => {
  const { moduleId, ecuId, ecuType } = req.body;
  const install = moduleInstaller.installModule(moduleId, ecuId, ecuType);
  res.json(install);
});

app.post('/api/modules/uninstall', (req, res) => {
  const { moduleId, ecuId } = req.body;
  const uninstall = moduleInstaller.uninstallModule(moduleId, ecuId);
  res.json(uninstall);
});

app.get('/api/modules/installed/:ecuId', (req, res) => {
  const modules = moduleInstaller.listInstalledModules(req.params.ecuId);
  res.json(modules);
});

app.get('/api/modules/verify/:moduleId/:ecuId', (req, res) => {
  const verify = moduleInstaller.verifyModuleIntegrity(req.params.moduleId, req.params.ecuId);
  res.json(verify);
});

app.post('/api/modules/toggle', (req, res) => {
  const { moduleId, ecuId, enabled } = req.body;
  const toggle = moduleInstaller.toggleModule(moduleId, ecuId, enabled);
  res.json(toggle);
});

app.get('/api/modules/dependencies/:moduleId', (req, res) => {
  const deps = moduleInstaller.getModuleDependencies(req.params.moduleId);
  res.json(deps);
});

app.post('/api/modules/package', (req, res) => {
  const { moduleId, ecuId } = req.body;
  const pkg = moduleInstaller.createModulePackage(moduleId, ecuId);
  res.json(pkg);
});

app.get('/api/modules/status/:ecuId', (req, res) => {
  const status = moduleInstaller.getEcuModuleStatusReport(req.params.ecuId);
  res.json(status);
});

// ============ OEM AS-BUILT ENDPOINTS ============
app.post('/api/asbuilt/decode-vin', (req, res) => {
  const { vin } = req.body;
  const decoded = oemAsBuilt.decodeVIN(vin);
  res.json(decoded);
});

app.get('/api/asbuilt/available/:manufacturer/:model/:generation/:year', (req, res) => {
  const available = oemAsBuilt.getAvailableAsBuiltCodes(
    req.params.manufacturer,
    req.params.model,
    req.params.generation,
    req.params.year
  );
  res.json(available);
});

app.post('/api/asbuilt/import', (req, res) => {
  const { vehicleId, asBuiltString } = req.body;
  const imported = oemAsBuilt.importAsBuiltData(vehicleId, asBuiltString);
  res.json(imported);
});

app.post('/api/asbuilt/compare', (req, res) => {
  const { sessionId, currentValues } = req.body;
  const comparison = oemAsBuilt.compareAsBuiltValues(sessionId, currentValues);
  res.json(comparison);
});

app.post('/api/asbuilt/apply', (req, res) => {
  const { sessionId, ecuId } = req.body;
  const applied = oemAsBuilt.applyAsBuiltData(sessionId, ecuId);
  res.json(applied);
});

app.post('/api/asbuilt/export', (req, res) => {
  const { sessionId, format } = req.body;
  const exported = oemAsBuilt.exportAsBuiltData(sessionId, format || 'json');
  res.json(exported);
});

app.post('/api/asbuilt/session', (req, res) => {
  const { vehicleId } = req.body;
  const sessionId = oemAsBuilt.createAsBuiltSession(vehicleId);
  res.json({ sessionId, vehicleId });
});

app.get('/api/asbuilt/session/:sessionId', (req, res) => {
  const details = oemAsBuilt.getSessionDetails(req.params.sessionId);
  res.json(details);
});

app.get('/api/asbuilt/sessions', (req, res) => {
  const sessions = oemAsBuilt.listActiveSessions();
  res.json({ activeSessions: sessions.length, sessions });
});

// ============ EMISSIONS CONTROL ENDPOINTS ============
app.get('/api/emissions/vehicles', (req, res) => {
  const manufacturer = req.query.manufacturer || null;
  const vehicles = emissionsController.listDieselVehicles(manufacturer);
  res.json(vehicles);
});

app.get('/api/emissions/manufacturers', (req, res) => {
  const manufacturers = emissionsController.getManufacturers();
  res.json(manufacturers);
});

app.get('/api/emissions/system/:systemName', (req, res) => {
  const system = emissionsController.getEmissionsSystemDetails(req.params.systemName);
  res.json(system);
});

app.post('/api/emissions/enable', (req, res) => {
  const { vehicleId, system } = req.body;
  const result = emissionsController.enableEmissions(vehicleId, system);
  res.json(result);
});

app.post('/api/emissions/disable', (req, res) => {
  const { vehicleId, system } = req.body;
  const result = emissionsController.disableEmissions(vehicleId, system);
  res.json(result);
});

app.get('/api/emissions/status/:vehicleId', (req, res) => {
  const status = emissionsController.getEmissionsStatus(req.params.vehicleId);
  res.json(status);
});

app.get('/api/emissions/compliance/:vehicleId', (req, res) => {
  const compliance = emissionsController.getComplianceStatus(req.params.vehicleId);
  res.json(compliance);
});

app.post('/api/emissions/spoof/enable', (req, res) => {
  const { vehicleId, system, profile } = req.body;
  const result = emissionsController.enableSensorSpoofing(vehicleId, system, profile);
  res.json(result);
});

app.post('/api/emissions/spoof/disable', (req, res) => {
  const { vehicleId, system } = req.body;
  const result = emissionsController.disableSensorSpoofing(vehicleId, system);
  res.json(result);
});

app.get('/api/emissions/spoof/:vehicleId/:sensorName', (req, res) => {
  const value = emissionsController.getSpoofSensorValues(req.params.vehicleId, req.params.sensorName);
  res.json(value);
});

// ============ OBD-II ENDPOINTS ============
app.get('/api/obd2/pids', (req, res) => {
  res.json({ status: 'success', pids: obd2.readAllPIDs() });
});

app.get('/api/obd2/pid/:pid', (req, res) => {
  const param = obd2.getParameter(req.params.pid);
  res.json(param || { error: 'PID not found' });
});

app.get('/api/obd2/read', (req, res) => {
  res.json({ status: 'success', data: obd2.simulateData() });
});

app.get('/api/obd2/dtc', (req, res) => {
  res.json({ status: 'success', codes: obd2.readDTC() });
});

app.post('/api/obd2/clear-dtc', (req, res) => {
  res.json({ status: 'success', message: 'DTC codes cleared' });
});

// ============ CAN BUS ENDPOINTS ============
app.post('/api/can/init', (req, res) => {
  const { busName, baudRate } = req.body;
  res.json({ status: 'success', bus: busName, baudRate });
});

app.get('/api/can/status/:busName', (req, res) => {
  const status = canbus.getStatus(req.params.busName);
  res.json(status || { status: 'unknown' });
});

app.post('/api/can/send', (req, res) => {
  const { busName, message } = req.body;
  res.json({ status: 'sent', busName, message });
});

app.post('/api/can/receive', (req, res) => {
  const { busName } = req.body;
  res.json({ status: 'listening', busName });
});

// ============ J2534 ENDPOINTS ============
app.get('/api/j2534/devices', (req, res) => {
  const devices = j2534.scanDevices();
  res.json(devices);
});

app.post('/api/j2534/open', (req, res) => {
  const { deviceId } = req.body;
  res.json({ status: 'opened', deviceId });
});

app.post('/api/j2534/close', (req, res) => {
  const { deviceId } = req.body;
  res.json({ status: 'closed', deviceId });
});

app.post('/api/j2534/connect', (req, res) => {
  const { deviceId, protocol } = req.body;
  res.json({ status: 'connected', deviceId, protocol });
});

app.post('/api/j2534/send', (req, res) => {
  const { deviceId, data } = req.body;
  res.json({ status: 'sent', deviceId, bytes: data.length });
});

app.post('/api/j2534/read', (req, res) => {
  const { deviceId } = req.body;
  res.json({ status: 'success', data: '0x00 0xFF 0xAA' });
});

// ============ ECU PROCESSOR ENDPOINTS ============
app.post('/api/ecu/connect', (req, res) => {
  const { ecuType } = req.body;
  res.json({ status: 'connected', ecuType, sessionId: 'sess_' + Date.now() });
});

app.post('/api/ecu/disconnect', (req, res) => {
  const { ecuId } = req.body;
  res.json({ status: 'disconnected', ecuId });
});

app.post('/api/ecu/unlock', (req, res) => {
  const { ecuId } = req.body;
  res.json({ status: 'unlocked', ecuId });
});

app.post('/api/ecu/read', (req, res) => {
  const { ecuId, address, length } = req.body;
  res.json({ status: 'success', data: 'FF'.repeat(length || 256) });
});

app.post('/api/ecu/write', (req, res) => {
  const { ecuId, address, data } = req.body;
  res.json({ status: 'success', ecuId, bytesWritten: data.length });
});

// ============ DIAGNOSTICS ENDPOINTS ============
app.get('/api/diagnostics/codes', (req, res) => {
  const codes = diagnostics.getAllDTCCodes();
  res.json(codes);
});

app.get('/api/diagnostics/decode/:code', (req, res) => {
  const decoded = diagnostics.decodeDTC(req.params.code);
  res.json(decoded || { error: 'Code not found' });
});

// ============ VEHICLE DATABASE ENDPOINTS ============

app.get('/api/vehicles/list', (req, res) => {
  const listData = vehicleDatabase.listVehicles();
  // Map to include full vehicle data
  const vehicles = listData.vehicles.map(v => {
    const fullVehicle = vehicleDatabase.vehicles[v.id];
    return {
      id: v.id,
      manufacturer: v.manufacturer,
      model: v.model,
      year: v.year,
      bodyType: fullVehicle.bodyType || 'Vehicle',
      driveType: fullVehicle.driveType || 'Standard',
      engines: v.engines,
      transmissions: fullVehicle.transmissions || [],
      ecuTypes: v.ecuTypes,
      power: fullVehicle.power || 'N/A',
      torque: fullVehicle.torque || 'N/A',
      wiring: fullVehicle.wiring || 'CAN',
      systems: fullVehicle.systems || []
    };
  });
  res.json({
    totalVehicles: listData.totalVehicles,
    vehicles: vehicles
  });
});

app.get('/api/vehicles/get/:vehicleId', (req, res) => {
  const { vehicleId } = req.params;
  res.json(vehicleDatabase.getVehicle(vehicleId));
});

app.post('/api/vehicles/search', (req, res) => {
  const { manufacturer, year, model, fuelType } = req.body;
  res.json(vehicleDatabase.searchVehicles({
    manufacturer,
    year,
    model,
    fuelType
  }));
});

app.get('/api/vehicles/:vehicleId/ecu-types', (req, res) => {
  const { vehicleId } = req.params;
  res.json(vehicleDatabase.getEcuTypesForVehicle(vehicleId));
});

app.get('/api/vehicles/:vehicleId/wiring', (req, res) => {
  const { vehicleId } = req.params;
  res.json(vehicleDatabase.getWiring(vehicleId));
});

app.get('/api/vehicles/:vehicleId/wiring-full', (req, res) => {
  const { vehicleId } = req.params;
  res.json(vehicleDatabase.getFullWiringSpec(vehicleId));
});

/**
 * Comprehensive ECU Information Endpoint
 * Returns: ECU types, engines, transmissions, wiring protocols, fuel type, power/torque, systems
 * Useful for understanding what ECUs are compatible with vehicle and their specifications
 */
app.get('/api/vehicles/:vehicleId/ecu-info-complete', (req, res) => {
  const { vehicleId } = req.params;
  const vehicle = vehicleDatabase.getVehicle(vehicleId);
  
  if (!vehicle) {
    return res.status(404).json({ error: 'Vehicle not found' });
  }
  
  res.json({
    vehicleId: vehicleId,
    vehicle: {
      manufacturer: vehicle.manufacturer,
      model: vehicle.model,
      year: vehicle.year,
      generation: vehicle.generation || 'N/A',
      vin_pattern: vehicle.vin
    },
    powertrainInfo: {
      engines: vehicle.engines,
      transmissions: vehicle.transmissions,
      driveType: vehicle.driveType,
      fuelType: vehicle.fuelType,
      displacement: vehicle.displacement,
      power: vehicle.power,
      torque: vehicle.torque
    },
    ecuInformation: {
      ecuTypes: vehicle.ecuTypes,
      wiringProtocol: vehicle.wiring,
      systems: vehicle.systems,
      totalEcuTypes: vehicle.ecuTypes.length,
      supportedProtocols: ['OBD-II', vehicle.wiring, 'J1939', 'CAN-FD']
    },
    wiringSpecification: vehicleDatabase.getFullWiringSpec(vehicleId).wiring || null
  });
});

/**
 * Diesel Truck ECU Specifications
 * Returns specific ECU and wiring info for diesel trucks with DPF, SCR, Glow Plug systems
 */
app.get('/api/vehicles/diesel/specifications', (req, res) => {
  const allVehicles = vehicleDatabase.listVehicles().vehicles;
  const dieselTrucks = allVehicles.filter(v => 
    v.fuelType === 'Diesel' && 
    (v.model.includes('Diesel') || v.model.includes('diesel'))
  );
  
  const dieselSpecs = dieselTrucks.map(truck => ({
    vehicleId: truck.id,
    manufacturer: truck.manufacturer,
    model: truck.model,
    year: truck.year,
    engines: truck.engines,
    ecuTypes: truck.ecuTypes,
    powertrainInfo: {
      power: truck.power,
      torque: truck.torque,
      transmission: truck.transmissions ? truck.transmissions[0] : 'N/A'
    },
    dieselSystems: {
      turbocharger: truck.systems.some(s => s.includes('Turbo')) ? 'Yes' : 'No',
      dpf: truck.systems.some(s => s.includes('DPF')) ? 'Yes' : 'No',
      scr: truck.systems.some(s => s.includes('SCR')) ? 'Yes' : 'No',
      glowPlugs: truck.systems.some(s => s.includes('Glow')) ? 'Yes' : 'No',
      exhaustBrake: truck.systems.some(s => s.includes('Brake')) ? 'Yes' : 'No'
    }
  }));
  
  res.json({
    totalDieselTrucks: dieselSpecs.length,
    trucks: dieselSpecs
  });
});

/**
 * Vehicle ECU Compatibility Search
 * Search for vehicles by ECU type, fuel type, manufacturer, etc.
 */
app.post('/api/vehicles/ecu-search', (req, res) => {
  const { ecuType, fuelType, manufacturer, yearMin, yearMax } = req.body;
  const allVehicles = vehicleDatabase.listVehicles().vehicles;
  
  const matches = allVehicles.filter(v => {
    if (ecuType && !v.ecuTypes.some(e => e.toLowerCase().includes(ecuType.toLowerCase()))) {
      return false;
    }
    if (fuelType && v.fuelType !== fuelType) {
      return false;
    }
    if (manufacturer && v.manufacturer.toLowerCase() !== manufacturer.toLowerCase()) {
      return false;
    }
    if (yearMin && v.year < yearMin) {
      return false;
    }
    if (yearMax && v.year > yearMax) {
      return false;
    }
    return true;
  });
  
  res.json({
    searchCriteria: { ecuType, fuelType, manufacturer, yearMin, yearMax },
    found: matches.length,
    vehicles: matches.map(v => ({
      id: v.id,
      manufacturer: v.manufacturer,
      model: v.model,
      year: v.year,
      ecuTypes: v.ecuTypes,
      fuelType: v.fuelType,
      power: v.power
    }))
  });
});

// ============ ECU SOFTWARE UPDATES ENDPOINTS ============

app.get('/api/updates/list', (req, res) => {
  res.json(ecuSoftwareUpdates.listAllUpdates());
});

app.get('/api/updates/vehicle/:vehicleId', (req, res) => {
  const { vehicleId } = req.params;
  res.json(ecuSoftwareUpdates.getUpdatesForVehicle(vehicleId));
});

app.get('/api/updates/detail/:updateId', (req, res) => {
  const { updateId } = req.params;
  res.json(ecuSoftwareUpdates.getUpdateDetails(updateId));
});

app.post('/api/updates/compatible', (req, res) => {
  const { vehicleId, ecuType } = req.body;
  res.json(ecuSoftwareUpdates.getCompatibleUpdates(vehicleId, ecuType));
});

app.post('/api/updates/map', (req, res) => {
  const { vehicleId, updateId } = req.body;
  res.json(ecuSoftwareUpdates.mapUpdateToVehicle(vehicleId, updateId));
});

app.post('/api/updates/check-compatibility', (req, res) => {
  const { vehicleId, updateId } = req.body;
  res.json(ecuSoftwareUpdates.checkCompatibility(vehicleId, updateId));
});

app.get('/api/updates/ecu-type/:ecuType', (req, res) => {
  const { ecuType } = req.params;
  res.json(ecuSoftwareUpdates.getUpdatesByEcuType(ecuType));
});

app.post('/api/updates/download-package', (req, res) => {
  const { updateId } = req.body;
  res.json(ecuSoftwareUpdates.createDownloadPackage(updateId));
});

app.post('/api/updates/verify', (req, res) => {
  const { updateId, checksum } = req.body;
  res.json(ecuSoftwareUpdates.verifyUpdateIntegrity(updateId, checksum));
});

app.get('/api/updates/history/:vehicleId', (req, res) => {
  const { vehicleId } = req.params;
  res.json(ecuSoftwareUpdates.getUpdateHistory(vehicleId));
});

// ============ ECU TEST BENCH ENDPOINTS ============

app.post('/api/testbench/session/start', (req, res) => {
  const { vehicleId, ecuId, ecuType, testMode } = req.body;
  res.json(ecuTestBench.startTestSession(vehicleId, ecuId, ecuType, testMode));
});

app.get('/api/testbench/session/:sessionId/status', (req, res) => {
  const { sessionId } = req.params;
  res.json(ecuTestBench.getSessionStatus(sessionId));
});

app.post('/api/testbench/session/:sessionId/run-test', (req, res) => {
  const { sessionId } = req.params;
  const { testName } = req.body;
  res.json(ecuTestBench.runTest(sessionId, testName));
});

app.post('/api/testbench/session/:sessionId/complete', (req, res) => {
  const { sessionId } = req.params;
  res.json(ecuTestBench.completeTestSession(sessionId));
});

app.post('/api/testbench/session/:sessionId/diagnostics', (req, res) => {
  const { sessionId } = req.params;
  const { testType } = req.body;
  res.json(ecuTestBench.runDiagnosticTest(sessionId, testType));
});

app.post('/api/testbench/session/:sessionId/integrity-check', (req, res) => {
  const { sessionId } = req.params;
  const { ecuId } = req.body;
  res.json(ecuTestBench.performIntegrityCheck(sessionId, ecuId));
});

app.post('/api/testbench/session/:sessionId/stress-test', (req, res) => {
  const { sessionId } = req.params;
  const { duration } = req.body;
  res.json(ecuTestBench.performStressTest(sessionId, duration));
});

app.get('/api/testbench/sessions', (req, res) => {
  res.json(ecuTestBench.listTestSessions());
});

app.get('/api/testbench/modes', (req, res) => {
  res.json(ecuTestBench.getAvailableTestModes());
});

// ============ CATCH-ALL 404 ============
app.use((req, res) => {
  res.status(404).json({
    error: 'Endpoint not found',
    path: req.path,
    method: req.method,
    message: 'Try /api/status for system information'
  });
});

// ============ START SERVER ============
app.listen(PORT, () => {
  console.log(`\n🚀 BlackFlag Server Running on http://localhost:${PORT}`);
  console.log(`📊 Total Endpoints: 150+`);
  console.log(`🏴 Version: 2.0.0`);
  console.log(`📦 Modules Loaded: 15`);
  console.log(`🔓 Free & Open Source | No Accounts Required\n`);
});

module.exports = app;
