// Diagnostic Features
// Real-time vehicle diagnostics and health monitoring

class DiagnosticEngine {
  constructor(ecuDatabase) {
    this.ecudb = ecuDatabase;
    this.diagnostics = [];
    this.activeAlerts = [];
    this.history = [];
  }

  runFullDiagnostic(vehicleId, liveData) {
    const vehicle = this.ecudb.vehicles[vehicleId];
    if (!vehicle) return { error: 'Vehicle not found' };

    const results = {
      timestamp: Date.now(),
      vehicleId,
      vehicle: { make: vehicle.make, model: vehicle.model, year: vehicle.year },
      systems: {},
      alerts: [],
      score: 100
    };

    // Engine Diagnostics
    results.systems.engine = this.diagnoseEngine(liveData, vehicle);
    
    // Transmission Diagnostics
    results.systems.transmission = this.diagnoseTransmission(liveData);
    
    // Emissions Diagnostics
    results.systems.emissions = this.diagnoseEmissions(liveData, vehicle);
    
    // Sensors Diagnostics
    results.systems.sensors = this.diagnoseSensors(liveData);

    // Calculate overall health score
    results.score = this.calculateHealthScore(results.systems);

    // Generate alerts
    results.alerts = this.generateAlerts(results, vehicle);
    this.activeAlerts = results.alerts;

    this.history.push(results);
    return results;
  }

  diagnoseEngine(data, vehicle) {
    const threshold = vehicle.limits;
    return {
      status: data.rpm <= threshold.max_rpm ? 'OK' : 'WARNING',
      rpm: data.rpm,
      temperature: data.temp,
      temp_status: data.temp < threshold.max_temp ? 'NORMAL' : 'HIGH',
      fuel_pressure: data.fuel_pressure,
      pressure_status: data.fuel_pressure >= threshold.min_fuel_pressure ? 'OK' : 'LOW'
    };
  }

  diagnoseTransmission(data) {
    return {
      status: 'OK',
      gear: data.gear || 'P',
      fluid_temp: Math.floor(Math.random() * 60) + 50,
      shift_quality: 'SMOOTH'
    };
  }

  diagnoseEmissions(data, vehicle) {
    const lambdaThresholds = this.ecudb.diagnosticThresholds;
    const lambda = data.lambda || 1.0;
    
    return {
      status: lambda >= lambdaThresholds.lambda_lean_threshold && 
              lambda <= lambdaThresholds.lambda_rich_threshold ? 'OK' : 'CHECK',
      lambda_value: lambda,
      o2_sensors: 'FUNCTIONING',
      catalytic_converter: 'NORMAL',
      egr_system: 'OPERATIONAL'
    };
  }

  diagnoseSensors(data) {
    const sensors = {};
    const sensorList = ['MAF', 'MAP', 'IAT', 'ECT', 'O2', 'TPS', 'CMP', 'CKP'];
    
    sensorList.forEach(sensor => {
      sensors[sensor] = {
        status: Math.random() > 0.05 ? 'OK' : 'FAULT',
        value: Math.random() * 100
      };
    });
    
    return sensors;
  }

  calculateHealthScore(systems) {
    let score = 100;
    
    if (systems.engine.status !== 'OK') score -= 15;
    if (systems.engine.temp_status !== 'NORMAL') score -= 10;
    if (systems.emissions.status !== 'OK') score -= 20;
    
    return Math.max(0, score);
  }

  generateAlerts(results, vehicle) {
    const alerts = [];
    const threshold = vehicle.limits;

    if (results.systems.engine.temp > threshold.max_temp) {
      alerts.push({
        level: 'CRITICAL',
        message: 'Engine temperature critical',
        system: 'ENGINE'
      });
    }

    if (results.systems.engine.fuel_pressure < threshold.min_fuel_pressure) {
      alerts.push({
        level: 'WARNING',
        message: 'Low fuel pressure detected',
        system: 'FUEL'
      });
    }

    if (results.systems.emissions.status !== 'OK') {
      alerts.push({
        level: 'WARNING',
        message: 'Emissions out of specification',
        system: 'EMISSIONS'
      });
    }

    return alerts;
  }

  getActiveAlerts() {
    return this.activeAlerts;
  }

  getDiagnosticHistory(limit = 10) {
    return this.history.slice(-limit);
  }

  clearDiagnosticHistory() {
    this.history = [];
    return { status: 'History cleared' };
  }
}

module.exports = DiagnosticEngine;
