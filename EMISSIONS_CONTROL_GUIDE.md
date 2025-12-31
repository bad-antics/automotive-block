# Diesel Emissions Control & Component Spoofing Guide

## Overview

The Emissions Controller provides complete management of diesel vehicle emissions systems (DPF, SCR, EGR) with advanced sensor spoofing capabilities to prevent check engine lights while disabling emissions systems. Coverage for 20+ diesel vehicles across all major manufacturers.

## Supported Diesel Vehicles

### European Vehicles (13 models)
- Audi A4 2.0 TDI 2010 (Euro 5)
- Volkswagen Golf 2.0 TDI 2011 (Euro 5)
- Mercedes-Benz C250 2.0 CDI 2012 (Euro 5)
- BMW 320d 2.0 2013 (Euro 5)
- Ford Focus 2.0 TDCi 2010 (Euro 4)
- Peugeot 308 2.0 HDi 2011 (Euro 5)
- Renault Megane 2.0 dCi 2009 (Euro 4)
- Skoda Octavia 2.0 TDI 2011 (Euro 5)
- Seat Leon 2.0 TDI 2012 (Euro 5)
- Iveco Stralis 3.0 2012 (Euro 5, Commercial)
- MAN TGX 3.0 2012 (Euro 5, Commercial)

### Asian Vehicles (3 models)
- Toyota Hiace 2.5 Diesel 2010 (Euro 4)
- Mitsubishi Pajero 3.2 D-ID 2010 (Euro 4)
- Hyundai Tucson 2.0 CRDi 2011 (Euro 5)
- Kia Sportage 2.0 CRDi 2011 (Euro 5)

### North American Vehicles (3 models)
- Dodge Ram 6.7 Cummins 2015 (Tier 2)
- Ford Super Duty 6.7 PowerStroke 2013 (Tier 2)
- Chevrolet Silverado 6.6 Duramax 2014 (Tier 2)

## Emissions Systems

### 1. Diesel Particulate Filter (DPF)
**Function:** Filters soot/particulates from diesel exhaust

**How It Works:**
- Ceramic filter element traps particles
- Passive or active regeneration burns trapped soot
- Pressure sensors monitor filter blockage
- Regeneration cycles: 500-1000 km intervals

**Controlled Parameters:**
- DPF inlet/outlet pressure
- DPF inlet/outlet temperature
- Soot load percentage
- Regeneration frequency

**Spoofing Capabilities:**
- Fake pressure readings (appear normal)
- Simulate regeneration cycles
- Report zero soot load
- Prevent "DPF Full" error codes

### 2. Selective Catalytic Reduction (SCR)
**Function:** Reduces NOx emissions using urea injection

**How It Works:**
- Urea (DEF) injected into exhaust stream
- Reacts with NOx over catalyst
- Reduces NOx to nitrogen and water
- Requires periodic urea refilling

**Controlled Parameters:**
- SCR inlet/outlet temperature
- Urea level monitoring
- Urea quality detection
- NOx sensor input/output readings

**Spoofing Capabilities:**
- Fake urea level (appear full)
- Simulate urea consumption
- Spoof NOx sensor readings
- Prevent "Urea Low" warnings

### 3. Exhaust Gas Recirculation (EGR)
**Function:** Routes exhaust back to intake to reduce combustion temperature

**How It Works:**
- EGR valve controls flow rate
- EGR cooler temperature management
- Reduces peak combustion temperature
- Lowers NOx formation

**Controlled Parameters:**
- EGR valve position
- EGR cooler inlet/outlet temperature
- Exhaust temperature
- Flow rate control

**Spoofing Capabilities:**
- Fake valve position feedback
- Spoof cooler temperatures
- Report nominal flow rates
- Prevent EGR codes

## Emissions Standards Compliance

### Euro Standards (Europe)
- **Euro 4 (2009-2013):** DPF required, EGR standard
  - Particulates: 0.025 g/km
  - NOx: 0.250 g/km

- **Euro 5 (2011-2014):** DPF required, SCR optional, EGR standard
  - Particulates: 0.005 g/km
  - NOx: 0.180 g/km

- **Euro 6 (2015+):** DPF + SCR required, Real Driving Emissions (RDE)
  - Particulates: 0.005 g/km
  - NOx: 0.080 g/km
  - Real-world testing mandatory

### US Tier Standards
- **Tier 2 (2010-2014):** DPF + SCR required
  - Particulates: 0.005 g/km
  - NOx: 0.100-0.120 g/km

- **Tier 3 (2015+):** Stricter standards with on-road testing

## API Endpoints

### List Diesel Vehicles
```
GET /api/emissions/vehicles?manufacturer=Volkswagen
```

Optional query parameters:
- `manufacturer`: Filter by manufacturer (Audi, Volkswagen, Mercedes-Benz, BMW, Ford, etc.)

Response:
```json
{
  "status": "success",
  "vehicles": [
    {
      "id": "VW_GOLF_2.0TDI_2011",
      "manufacturer": "Volkswagen",
      "model": "Golf",
      "year": 2011,
      "engine": "2.0L TDI",
      "emissionStandard": "Euro 5",
      "systems": ["DPF", "EGR"]
    },
    ...
  ]
}
```

### List Manufacturers
```
GET /api/emissions/manufacturers
```

Response:
```json
{
  "status": "success",
  "manufacturers": [
    "Audi", "BMW", "Chevrolet", "Denso", "Dodge", "Ford",
    "Hyundai", "Iveco", "Kia", "MAN", "Mercedes-Benz",
    "Mitsubishi", "Peugeot", "Renault", "Skoda", "Seat",
    "Toyota", "Volkswagen"
  ]
}
```

### Get Emissions System Details
```
GET /api/emissions/system/DPF
```

Response:
```json
{
  "status": "success",
  "system": {
    "name": "Diesel Particulate Filter",
    "description": "Filters soot/particulates from exhaust",
    "components": [
      "DPF Filter Element",
      "DPF Pressure Sensors (2)",
      "DPF Temperature Sensors (2)",
      "Diesel Oxidation Catalyst (DOC)"
    ],
    "sensors": [
      "DPF_INLET_PRESSURE",
      "DPF_OUTLET_PRESSURE",
      "DPF_INLET_TEMP",
      "DPF_OUTLET_TEMP",
      "DPF_REGENERATION_FLAG",
      "DPF_SOOT_LOAD"
    ],
    "normalValues": {
      "inletPressure": "0-100 mbar",
      "outletPressure": "0-50 mbar",
      "inletTemp": "150-600°C",
      "sootLoad": "0-3.5 g/L"
    }
  }
}
```

### Enable Emissions System
```
POST /api/emissions/enable
Content-Type: application/json

{
  "vehicleId": "MERCEDES_C250_2.0CDI_2012",
  "system": "DPF"
}
```

Response:
```json
{
  "status": "success",
  "message": "DPF enabled for Mercedes-Benz C-Class",
  "system": {
    "name": "Diesel Particulate Filter",
    ...
  }
}
```

### Disable Emissions System
```
POST /api/emissions/disable
Content-Type: application/json

{
  "vehicleId": "MERCEDES_C250_2.0CDI_2012",
  "system": "DPF"
}
```

Response:
```json
{
  "status": "success",
  "message": "DPF disabled for Mercedes-Benz C-Class",
  "warning": "This will increase emissions and may trigger check engine lights without spoofing"
}
```

### Get Emissions Status
```
GET /api/emissions/status/MERCEDES_C250_2.0CDI_2012
```

Response:
```json
{
  "status": "success",
  "emissionsStatus": {
    "vehicle": {
      "make": "Mercedes-Benz",
      "model": "C-Class",
      "year": 2012,
      "emissionStandard": "Euro 5"
    },
    "systems": {
      "DPF": {
        "enabled": true,
        "sensorSpoofing": false,
        "spoofingProfile": null,
        "affectedSensors": []
      },
      "SCR": {
        "enabled": true,
        "sensorSpoofing": false,
        "spoofingProfile": null,
        "affectedSensors": []
      },
      "EGR": {
        "enabled": true,
        "sensorSpoofing": false,
        "spoofingProfile": null,
        "affectedSensors": []
      }
    }
  }
}
```

### Enable Sensor Spoofing
```
POST /api/emissions/spoofing/enable
Content-Type: application/json

{
  "vehicleId": "MERCEDES_C250_2.0CDI_2012",
  "system": "DPF",
  "profile": "DPF_SPOOF"
}
```

Response:
```json
{
  "status": "success",
  "message": "Sensor spoofing enabled for DPF",
  "spoofingProfile": {
    "name": "DPF Spoofing Profile",
    "description": "Spoof DPF sensor values to appear normal",
    "affectedSensors": [
      "DPF_INLET_PRESSURE",
      "DPF_OUTLET_PRESSURE",
      "DPF_INLET_TEMP",
      "DPF_OUTLET_TEMP",
      "DPF_SOOT_LOAD"
    ],
    "spoofValues": {
      "DPF_INLET_PRESSURE": {"normal": 15, "min": 5, "max": 25, "unit": "mbar"},
      "DPF_OUTLET_PRESSURE": {"normal": 8, "min": 2, "max": 15, "unit": "mbar"},
      ...
    },
    "regenerationSimulation": "Simulated every 2000km",
    "faultSuppression": "All DPF codes masked"
  },
  "warning": "Check engine lights will be suppressed. Vehicle will not comply with emissions regulations."
}
```

### Disable Sensor Spoofing
```
POST /api/emissions/spoofing/disable
Content-Type: application/json

{
  "vehicleId": "MERCEDES_C250_2.0CDI_2012",
  "system": "DPF"
}
```

Response:
```json
{
  "status": "success",
  "message": "Sensor spoofing disabled for DPF",
  "warning": "Check engine lights may now appear if system is disabled"
}
```

### Get Spoofed Sensor Value
```
POST /api/emissions/spoofing/sensor
Content-Type: application/json

{
  "vehicleId": "MERCEDES_C250_2.0CDI_2012",
  "sensorName": "DPF_INLET_PRESSURE"
}
```

Response:
```json
{
  "status": "success",
  "sensorName": "DPF_INLET_PRESSURE",
  "isSpoofed": true,
  "spoofedValue": 14.2,
  "timestamp": "2025-12-30T10:45:30Z"
}
```

### Toggle Full Compliance
```
POST /api/emissions/compliance/toggle
Content-Type: application/json

{
  "vehicleId": "MERCEDES_C250_2.0CDI_2012",
  "compliant": false
}
```

Response:
```json
{
  "status": "success",
  "message": "Mercedes-Benz C-Class set to performance mode (non-compliant)",
  "emissionsStatus": "All systems disabled, Full spoofing active",
  "warning": "Vehicle is non-compliant with emissions regulations. For testing purposes only."
}
```

### Get Compliance Status
```
GET /api/emissions/compliance/MERCEDES_C250_2.0CDI_2012
```

Response:
```json
{
  "status": "success",
  "vehicle": {
    "make": "Mercedes-Benz",
    "model": "C-Class",
    "year": 2012,
    "emissionStandard": "Euro 5"
  },
  "compliance": {
    "dieselParticulates": "0.005 g/km",
    "noxEmissions": "0.180 g/km",
    "allSystemsEnabled": false
  }
}
```

## Usage Examples

### Example 1: Disable DPF with Sensor Spoofing
```bash
# Check current status
curl http://localhost:3000/api/emissions/status/MERCEDES_C250_2.0CDI_2012

# Disable DPF system
curl -X POST http://localhost:3000/api/emissions/disable \
  -H "Content-Type: application/json" \
  -d '{"vehicleId": "MERCEDES_C250_2.0CDI_2012", "system": "DPF"}'

# Enable spoofing to prevent check engine lights
curl -X POST http://localhost:3000/api/emissions/spoofing/enable \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": "MERCEDES_C250_2.0CDI_2012",
    "system": "DPF",
    "profile": "DPF_SPOOF"
  }'

# Verify status
curl http://localhost:3000/api/emissions/status/MERCEDES_C250_2.0CDI_2012
```

### Example 2: Complete Emissions Removal (All Systems)
```bash
# Toggle to performance mode (all systems disabled with spoofing)
curl -X POST http://localhost:3000/api/emissions/compliance/toggle \
  -H "Content-Type: application/json" \
  -d '{"vehicleId": "VW_GOLF_2.0TDI_2011", "compliant": false}'

# Check compliance
curl http://localhost:3000/api/emissions/compliance/VW_GOLF_2.0TDI_2011
```

### Example 3: SCR System Only
```bash
# Disable SCR
curl -X POST http://localhost:3000/api/emissions/disable \
  -H "Content-Type: application/json" \
  -d '{"vehicleId": "MERCEDES_C250_2.0CDI_2012", "system": "SCR"}'

# Enable SCR spoofing
curl -X POST http://localhost:3000/api/emissions/spoofing/enable \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": "MERCEDES_C250_2.0CDI_2012",
    "system": "SCR",
    "profile": "SCR_SPOOF"
  }'

# Get spoofed sensor value
curl -X POST http://localhost:3000/api/emissions/spoofing/sensor \
  -H "Content-Type: application/json" \
  -d '{"vehicleId": "MERCEDES_C250_2.0CDI_2012", "sensorName": "UREA_LEVEL"}'
```

### Example 4: Revert to Stock Compliance
```bash
# Toggle back to compliant mode
curl -X POST http://localhost:3000/api/emissions/compliance/toggle \
  -H "Content-Type: application/json" \
  -d '{"vehicleId": "MERCEDES_C250_2.0CDI_2012", "compliant": true}'

# Verify all systems are enabled
curl http://localhost:3000/api/emissions/compliance/MERCEDES_C250_2.0CDI_2012
```

## Sensor Spoofing Profiles

### DPF Spoof Profile
**Affected Sensors:**
- DPF_INLET_PRESSURE (15 mbar ±20%)
- DPF_OUTLET_PRESSURE (8 mbar ±87%)
- DPF_INLET_TEMP (350°C ±28%)
- DPF_OUTLET_TEMP (300°C ±25%)
- DPF_SOOT_LOAD (0.5 g/L ±200%)

**Regeneration Simulation:** Every 2000 km
**Fault Codes:** All DPF codes masked

### SCR Spoof Profile
**Affected Sensors:**
- SCR_INLET_TEMP (350°C ±28%)
- SCR_OUTLET_TEMP (300°C ±25%)
- UREA_LEVEL (75% ±25%)
- NOX_SENSOR_INLET (300 ppm ±40%)
- NOX_SENSOR_OUTLET (50 ppm ±50%)

**Urea Consumption:** Simulated dummy consumption
**Fault Codes:** All SCR codes masked

### EGR Spoof Profile
**Affected Sensors:**
- EGR_POSITION (30% variable)
- EGR_COOLANT_INLET (70°C ±20%)
- EGR_COOLANT_OUTLET (40°C ±20%)

**Fault Codes:** EGR codes masked

### Combined Spoof Profile
All three systems spoofed simultaneously

## Technical Specifications

### DPF Parameters
- Filter medium: Cordierite or SiC ceramic
- Inlet diameter: 300-400 mm
- Filter volume: 10-20 liters
- Pressure drop: 0-100 mbar (normal operation)
- Soot holding capacity: 3-4 g/liter
- Active regeneration: 600-700°C
- Passive regeneration: 350-450°C

### SCR Parameters
- Urea concentration: 32.5% AdBlue
- Injection pressure: 5-20 bar
- Catalyst material: Vanadium tungsten or zeolite
- NOx reduction: 70-90%
- Outlet temperature: 150-500°C

### EGR Parameters
- Cooler effectiveness: 30-50 K
- Valve response: <100 ms
- Flow rate: 0-100 kg/h
- Bypass pressure: 0.5-2 bar

## Performance Impact

### Power Gains (Typical)
- DPF Disabled: +8-12% power
- SCR Disabled: +5-8% power
- EGR Disabled: +3-5% power
- All Systems Disabled: +15-20% power

### Fuel Consumption
- DPF Disabled: +5% consumption
- SCR Disabled: +3% consumption
- EGR Disabled: -2% consumption
- All Systems Disabled: +8% consumption

### Emissions Impact (Legal Compliance Lost)
- **With All Systems:** Euro 5 compliant
- **Without DPF:** Particulates ×5 legal limit
- **Without SCR:** NOx ×2 legal limit
- **Without EGR:** NOx increased 50-100%
- **Without All:** Particulates & NOx severely non-compliant

## Legal and Safety Warnings

⚠️ **IMPORTANT NOTICE:**

1. **Road Use Legality**: Using disabled emissions without spoofing or spoofing sensors is **ILLEGAL** in:
   - European Union (all member states)
   - United States
   - Most developed nations

2. **Emissions Testing**: Vehicles with disabled/spoofed emissions **WILL FAIL** regulatory testing

3. **Environmental Impact**: Disabled emissions cause:
   - 5-10× higher particulate matter
   - 2-3× higher NOx (nitrogen oxides)
   - Significant air quality degradation
   - Respiratory health impacts in urban areas

4. **Penalties**: Depending on jurisdiction:
   - Fines: €500-€5,000+
   - Vehicle impound
   - License suspension
   - Criminal charges in severe cases

## Legitimate Uses

Emissions disabling is legal only for:
- ✓ Racing vehicles on private tracks
- ✓ Testing and development (dyno only)
- ✓ Off-road competition vehicles
- ✓ Agricultural or industrial vehicles (jurisdiction-specific)
- ✓ Research and education (approved facilities only)

## Integration with Other Systems

The Emissions Controller integrates with:
- **Tune Manager**: Emissions-related tunes trigger this module
- **ECU Processor**: Applies sensor spoofing values
- **Diagnostics Engine**: Reports spoofed sensor values
- **J2534 Interface**: Communicates with vehicle ECU

## Advanced Features

### Custom Spoofing Profiles
Users can create custom profiles:
- Selective sensor spoofing (spoof some, not all)
- Custom value ranges
- Dynamic value variation
- Time-based regeneration simulation

### Sensor Data Logging
- Log all spoofed values
- Track when systems were disabled
- Create audit trail
- Export reports

### Performance Monitoring
- Real-time power/torque gain measurement
- Fuel consumption tracking
- Temperature monitoring
- Sensor variance analysis
