// OBD-II Protocol Implementation
// Standard diagnostic data for OBD-II vehicles

class OBD2Handler {
  constructor() {
    this.pids = this.initializePIDs();
    this.dtc = new Map(); // Diagnostic Trouble Codes
  }

  initializePIDs() {
    return {
      '0101': { name: 'Calculated Load', unit: '%', min: 0, max: 100 },
      '0105': { name: 'Engine Coolant Temp', unit: '°C', min: -40, max: 215 },
      '010C': { name: 'Engine RPM', unit: 'rpm', min: 0, max: 16383 },
      '010D': { name: 'Vehicle Speed', unit: 'km/h', min: 0, max: 255 },
      '0114': { name: 'O2 Sensor Voltage', unit: 'V', min: 0, max: 1.275 },
      '0142': { name: 'Fuel Level', unit: '%', min: 0, max: 100 },
      '015C': { name: 'Oil Temp', unit: '°C', min: -40, max: 210 },
    };
  }

  getParameter(pid) {
    return this.pids[pid] || null;
  }

  readAllPIDs() {
    return this.pids;
  }

  readDTC() {
    return Array.from(this.dtc.entries()).map(([code, desc]) => ({
      code,
      description: desc
    }));
  }

  clearDTC() {
    this.dtc.clear();
    return { status: 'DTC cleared', count: 0 };
  }

  addDTC(code, description) {
    this.dtc.set(code, description);
  }

  simulateData() {
    const simulated = {};
    Object.entries(this.pids).forEach(([pid, params]) => {
      const range = params.max - params.min;
      simulated[pid] = {
        name: params.name,
        value: Math.round((Math.random() * range + params.min) * 100) / 100,
        unit: params.unit
      };
    });
    return simulated;
  }
}

module.exports = OBD2Handler;
