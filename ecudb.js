// ECU Database - Definitions and Specifications
// Vehicle ECU parameters, thresholds, and characteristics

const ecuDatabase = {
  vehicles: {
    'toyota_corolla_2020': {
      make: 'Toyota',
      model: 'Corolla',
      year: 2020,
      engine: '1.6L 4-cyl',
      ecu: 'Denso',
      protocols: ['OBD2', 'CAN'],
      tuningParams: {
        ignition_timing: { min: -10, max: 35, unit: 'degrees', default: 15 },
        air_fuel_ratio: { min: 10, max: 20, unit: 'ratio', default: 14.7 },
        boost_pressure: { min: 0, max: 20, unit: 'psi', default: 0 },
        fuel_injector_pulse: { min: 1, max: 50, unit: 'ms', default: 5 },
        idle_rpm: { min: 400, max: 1200, unit: 'rpm', default: 750 }
      },
      limits: {
        max_rpm: 7000,
        max_boost: 0,
        max_temp: 120,
        min_fuel_pressure: 35
      }
    },
    'ford_mustang_2022': {
      make: 'Ford',
      model: 'Mustang',
      year: 2022,
      engine: '5.0L V8',
      ecu: 'Ford EEC-V',
      protocols: ['OBD2', 'CAN'],
      tuningParams: {
        ignition_timing: { min: -5, max: 40, unit: 'degrees', default: 25 },
        air_fuel_ratio: { min: 11, max: 20, unit: 'ratio', default: 14.7 },
        boost_pressure: { min: 0, max: 25, unit: 'psi', default: 0 },
        fuel_injector_pulse: { min: 2, max: 80, unit: 'ms', default: 8 },
        idle_rpm: { min: 500, max: 1500, unit: 'rpm', default: 800 }
      },
      limits: {
        max_rpm: 7500,
        max_boost: 0,
        max_temp: 125,
        min_fuel_pressure: 40
      }
    },
    'bmw_m340i_2021': {
      make: 'BMW',
      model: 'M340i',
      year: 2021,
      engine: '3.0L Turbo I6',
      ecu: 'BMW Bosch MSV90',
      protocols: ['OBD2', 'CAN', 'Kunbus'],
      tuningParams: {
        ignition_timing: { min: 0, max: 45, unit: 'degrees', default: 30 },
        air_fuel_ratio: { min: 9, max: 18, unit: 'ratio', default: 14.7 },
        boost_pressure: { min: 8, max: 30, unit: 'psi', default: 16 },
        fuel_injector_pulse: { min: 3, max: 120, unit: 'ms', default: 10 },
        idle_rpm: { min: 400, max: 1000, unit: 'rpm', default: 650 }
      },
      limits: {
        max_rpm: 8000,
        max_boost: 30,
        max_temp: 110,
        min_fuel_pressure: 50
      }
    }
  },

  ecuModels: {
    'denso_v08': {
      brand: 'Denso',
      model: 'V08',
      type: 'Gasoline Engine Control Unit',
      supports: ['OBD2', 'CAN'],
      memory: '256KB',
      cpu: 'H8/3694'
    },
    'bosch_mevd17': {
      brand: 'Bosch',
      model: 'ME-Motronic D17',
      type: 'Gasoline Engine Control Unit',
      supports: ['OBD2', 'CAN', 'KWP2000'],
      memory: '512KB',
      cpu: 'C166'
    },
    'ford_eec5': {
      brand: 'Ford',
      model: 'EEC-V',
      type: 'Gasoline Engine Control Unit',
      supports: ['OBD2', 'CAN', 'J1850'],
      memory: '1MB',
      cpu: 'Intel 8051'
    }
  },

  diagnosticThresholds: {
    engine_temp_warning: 105,
    engine_temp_critical: 120,
    oil_pressure_min: 20,
    fuel_pressure_min: 35,
    lambda_lean_threshold: 0.8,
    lambda_rich_threshold: 1.2
  },

  commonDTC: {
    'P0101': 'Mass or Volume Air Flow Circuit Range/Performance',
    'P0102': 'Mass or Volume Air Flow Circuit Low Input',
    'P0103': 'Mass or Volume Air Flow Circuit High Input',
    'P0105': 'Manifold Absolute Pressure/Barometric Pressure Circuit Malfunction',
    'P0110': 'Intake Air Temperature Circuit Malfunction',
    'P0111': 'Intake Air Temperature Circuit Range/Performance',
    'P0112': 'Intake Air Temperature Circuit Low Input',
    'P0113': 'Intake Air Temperature Circuit High Input',
    'P0115': 'Engine Coolant Temperature Circuit Malfunction',
    'P0116': 'Engine Coolant Temperature Circuit Range/Performance',
    'P0117': 'Engine Coolant Temperature Circuit Low Input',
    'P0118': 'Engine Coolant Temperature Circuit High Input'
  }
};

module.exports = ecuDatabase;
