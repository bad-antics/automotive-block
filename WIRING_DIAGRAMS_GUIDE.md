# Wiring Diagram Library Guide

## Overview

The Wiring Diagram Library provides comprehensive electrical schematic data, circuit tracing capabilities, connector end views, and exploded diagrams for 50+ automotive circuits.

## Features

### 1. Circuit Database
- 25+ common automotive circuits
- Categories: Engine Control, Fuel System, Sensors, Lighting, Comfort, Emissions Control
- Detailed component lists
- Wiring colors and gauges
- Voltage and amperage specifications

### 2. Circuit Tracing
Trace electrical paths between components with color-coded wire routing.

**Supported Circuits:**
- ECU Power Supply (12V/24V)
- Fuel Pump Circuit (12V, 20-40A)
- Fuel Injector Control (1-4A per injector)
- Ignition Coil Circuit (12V primary, 20KV secondary)
- Starter Motor (100-300A)
- Charging System (13.5-14.5V)
- Oxygen Sensors (Lambda feedback)
- Manifold Pressure Sensors
- Throttle Position Sensors
- Coolant Temperature Sensors
- Mass Air Flow Sensors
- Crankshaft Position Sensors
- Camshaft Position Sensors
- Idle Air Control
- Transmission Solenoids
- ABS Control Modules
- EGR System Control
- Evaporative Emission Control
- Headlight Control
- Brake Light Circuit
- Turn Signal Circuit
- Interior Lighting
- Windshield Wiper Motor
- HVAC Blower Motor
- Power Window Control
- Fuel Door Release

### 3. Connector End Views
Interactive connector diagrams with pin configurations.

**Supported Connectors:**
- Deutsch DT (circular, 2-16 pins)
- Bosch Standard (linear, 2-6 pins)
- TE Connectivity (rectangular matrix, 2-8 pins)
- Denso EV6 Injector (6-pin straight)

Each connector includes:
- Pin layout diagrams
- Wire color coding
- Voltage specifications
- Amperage ratings
- Locking mechanisms
- Sealing specifications

### 4. Exploded Views
Component-by-component assembly diagrams with spacing and connection info.

### 5. Vehicle-Specific Wiring
Pre-configured wiring specifications for popular vehicles:
- Audi A4 2.0 TFSI 2008
- BMW 335i 2010
- Mercedes-Benz C250 2012
- Volkswagen Golf 2011
- Ford Focus 2010

## API Endpoints

### List All Circuits
```
GET /api/wiring/circuits
```

Response:
```json
{
  "status": "success",
  "circuits": [
    {
      "id": "ECU_POWER",
      "name": "ECU Power Supply",
      "category": "Engine Control",
      "components": 6
    },
    ...
  ]
}
```

### Select Circuit for Tracing
```
POST /api/wiring/circuit/select
Content-Type: application/json

{
  "circuitId": "FUEL_PUMP"
}
```

Response:
```json
{
  "status": "success",
  "circuit": {
    "id": "FUEL_PUMP",
    "name": "Fuel Pump Circuit",
    "category": "Fuel System",
    "voltage": "12V",
    "amperage": "20-40A",
    "components": [
      "Fuel Pump Relay",
      "Fuel Pump Motor",
      "Fuel Pressure Sensor",
      ...
    ],
    "wiringColor": ["YELLOW/RED", "BLACK", "BLUE"],
    "gaugeAWG": "10-14"
  }
}
```

### Trace Circuit Path
```
POST /api/wiring/circuit/trace
Content-Type: application/json

{
  "circuitId": "FUEL_PUMP",
  "startComponent": "Fuel Pump Relay",
  "endComponent": "Fuel Pump Motor"
}
```

Response:
```json
{
  "status": "success",
  "trace": {
    "circuitId": "FUEL_PUMP",
    "path": [
      {
        "component": "Fuel Pump Relay",
        "order": 1,
        "color": "YELLOW/RED",
        "gauge": "10-14"
      },
      {
        "component": "Fuel Pump Motor",
        "order": 2,
        "color": "BLACK",
        "gauge": "10-14"
      }
    ],
    "wiringColors": ["YELLOW/RED", "BLACK", "BLUE"],
    "gaugeAWG": "10-14"
  }
}
```

### Get Connector End View
```
GET /api/wiring/connector/DEUTSCH_DT
```

Response:
```json
{
  "status": "success",
  "connector": {
    "name": "Deutsch DT Connector",
    "pins": [1, 2, 3, 4, 6, 8, 12, 16],
    "voltage": "12-24V",
    "amperage": "10-30A per pin"
  },
  "endView": {
    "type": "Deutsch DT Connector",
    "pinCount": 8,
    "layout": "Circular arrangement",
    "colors": {
      "1": "Red (Power)",
      "2": "Black (Ground)",
      ...
    }
  }
}
```

### Get Exploded View
```
GET /api/wiring/exploded/IGNITION_COIL
```

Response:
```json
{
  "status": "success",
  "explodedView": {
    "circuitId": "IGNITION_COIL",
    "circuitName": "Ignition Coil Circuit",
    "components": [
      {
        "order": 1,
        "component": "Ignition Module",
        "position": "Layer 1",
        "spacing": "2-3 inches"
      },
      ...
    ]
  }
}
```

### Get Vehicle Wiring Specification
```
GET /api/wiring/vehicle/AUDI_A4_2008
```

Response:
```json
{
  "status": "success",
  "vehicle": {
    "make": "Audi",
    "model": "A4",
    "year": 2008,
    "engine": "2.0 TFSI",
    "ecuModel": "Bosch ME7.x"
  },
  "circuits": [
    {
      "id": "ECU_POWER",
      "name": "ECU Power Supply",
      ...
    }
  ]
}
```

### List All Vehicles
```
GET /api/wiring/vehicles
```

## Usage Examples

### Example 1: Trace a Complete Fuel System Circuit
```bash
# Select the fuel pump circuit
curl -X POST http://localhost:3000/api/wiring/circuit/select \
  -H "Content-Type: application/json" \
  -d '{"circuitId": "FUEL_PUMP"}'

# Trace from relay to pump
curl -X POST http://localhost:3000/api/wiring/circuit/trace \
  -H "Content-Type: application/json" \
  -d '{
    "circuitId": "FUEL_PUMP",
    "startComponent": "Fuel Pump Relay",
    "endComponent": "Fuel Pump Motor"
  }'

# Get connector types used
curl http://localhost:3000/api/wiring/connector/DEUTSCH_DT

# Get exploded view
curl http://localhost:3000/api/wiring/exploded/FUEL_PUMP
```

### Example 2: Analyze Vehicle-Specific Wiring
```bash
# Get all circuits for Audi A4
curl http://localhost:3000/api/wiring/vehicle/AUDI_A4_2008

# List available vehicles
curl http://localhost:3000/api/wiring/vehicles
```

### Example 3: Check Ignition System Specifications
```bash
# Get ignition coil circuit details
curl http://localhost:3000/api/wiring/circuits | grep -i "ignition"

# Select and trace ignition circuit
curl -X POST http://localhost:3000/api/wiring/circuit/select \
  -H "Content-Type: application/json" \
  -d '{"circuitId": "IGNITION_COIL"}'

# Get exploded view for installation
curl http://localhost:3000/api/wiring/exploded/IGNITION_COIL
```

## Technical Specifications

### Wire Gauges (AWG)
- 2-4: High-amperage circuits (300A+)
- 8-12: Charging and starter circuits (60-200A)
- 10-14: Fuel pump and main power (20-40A)
- 14-18: Lighting and control circuits (5-20A)
- 18-22: Sensor circuits (0.5-3A)
- 20-28: Low-signal sensor circuits (<1A)

### Voltage Standards
- 12V: Standard passenger vehicles
- 24V: Heavy trucks and commercial vehicles
- 5V: Sensor reference voltage
- 0-5V: Analog sensor signals
- 20KV+: Secondary ignition coil

### Color Coding Standards
- Red: Positive power
- Black: Ground/Negative
- Blue: Main signal
- Yellow/Orange: Auxiliary/Control
- Green: Low signal
- Gray: Return/Feedback
- White: Sensor output
- Brown: Secondary power

## Integration with Other Modules

The Wiring Diagram Library integrates with:
- **ECU Database**: Vehicle-specific electrical configurations
- **Tune Manager**: Wire specifications for modification implementations
- **Diagnostics Engine**: Sensor circuit validation

## Notes

- All wiring specifications are based on OEM standards
- Voltage and amperage ratings are maximum safe values
- Actual colors may vary by manufacturer
- Always verify against factory service manuals before modifications
- Sensor spoofing may require understanding of wiring to properly implement
