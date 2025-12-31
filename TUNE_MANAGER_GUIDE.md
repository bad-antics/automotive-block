# Tune Manager Guide - ECU Modification System

## Overview

The Tune Manager provides access to 100+ ECU tuning modifications across all vehicle types, fuel types, and engine configurations. Supports performance optimization, efficiency tuning, emissions control, and custom configurations.

## Tune Categories

### 1. Performance Tunes
Enhanced power and torque output for improved acceleration and overall performance.

**Available Performance Tunes:**
- **Power Increase +10%**: Modest gains (10% power, 8% torque) - Very High Reliability
- **Power Increase +25%**: Significant gains (25% power, 20% torque) - High Reliability
- **Power Increase +50%**: Aggressive gains (50% power, 45% torque) - Medium Reliability
- **Turbo Boost Increase**: Increased boost pressure (30-40% power) - Medium-High Reliability
- **Sport Mode Tune**: Enhanced performance (20-30% power) for track driving - Medium Reliability
- **Race Tune**: Maximum performance (60-80% power) for racing only - Low-Medium Reliability

**Fuel Consumption Impact:**
- +10% tune: -5% consumption
- +25% tune: +5% consumption
- +50% tune: +15% consumption
- Sport mode: +20% consumption
- Race tune: +40% consumption

### 2. Efficiency Tunes
Optimized for fuel economy and eco-conscious driving.

**Available Efficiency Tunes:**
- **Fuel Economy Mode**: Maximum fuel savings (20% less consumption, 15% power loss)
- **Highway Cruise Optimization**: Steady-state efficiency (12% fuel savings, 5% power loss)

### 3. Emissions Control Tunes
**For Diesel Vehicles Only**

#### Emissions On (Stock Compliance)
- All emissions systems active
- Full sensor monitoring
- OBD-II compliance
- Check engine lights possible if maintenance needed

#### Emissions Off (Performance)
- DPF/EGR/SCR disabled
- 15-20% power increase
- 10-15% torque increase
- Increased NOx/particulate emissions
- Check engine lights will appear

#### Emissions Off + Sensor Spoofing (Advanced)
- All emissions systems disabled
- Sensor values spoofed to appear normal
- Check engine lights prevented
- 15-20% power increase without warning lights
- **WARNING: Illegal for road use in most jurisdictions**

#### System-Specific Disables
- **DPF Only Off**: Disable particulate filter only
- **SCR Only Off**: Disable urea injection system
- **EGR Only Off**: Disable exhaust recirculation

### 4. Comfort & Control Tunes
Optimized driving experience and transmission behavior.

- **Idle Stability Enhancement**: Smoother idle, better low-RPM response
- **Shift Point Optimization**: Customizable transmission shift behavior
- **Anti-Knock Protection**: Enhanced knock detection and safety margins

### 5. Diagnostics Tunes
Extended logging and monitoring capabilities.

- **Extended Diagnostics**: All sensors logged, fault memory extended

## API Endpoints

### List All Tunes
```
GET /api/tunes/list?category=Performance
```

Optional query parameters:
- `category`: Filter by category (Performance, Efficiency, Emissions Control, Custom, Diagnostics, Comfort)

Response:
```json
{
  "status": "success",
  "tunes": [
    {
      "id": "POWER_INCREASE_10",
      "name": "Power Increase +10%",
      "category": "Performance",
      "description": "Modest power increase with stock parameters",
      "powerGain": "10%",
      "torqueGain": "8%",
      "fuelConsumption": "-5%",
      "checkEngineRisk": "Very Low"
    },
    ...
  ]
}
```

### Get Tune Categories
```
GET /api/tunes/categories
```

Response:
```json
{
  "status": "success",
  "categories": [
    "Comfort",
    "Custom",
    "Diagnostics",
    "Efficiency",
    "Emissions Control",
    "Performance"
  ]
}
```

### Get Compatible Tunes
```
POST /api/tunes/compatible
Content-Type: application/json

{
  "fuelType": "Diesel",
  "ecuModel": "Bosch EDC17"
}
```

Response:
```json
{
  "status": "success",
  "tunes": [
    {
      "id": "EMISSIONS_OFF_WITH_SPOOFING",
      "name": "Emissions Off + Component Spoofing",
      "category": "Emissions Control",
      "powerGain": "15-20%",
      "checkEngineRisk": "None (sensors spoofed)"
    },
    ...
  ]
}
```

### Get Tune Details
```
GET /api/tunes/details/POWER_INCREASE_25
```

Response:
```json
{
  "status": "success",
  "tune": {
    "id": "POWER_INCREASE_25",
    "name": "Power Increase +25%",
    "category": "Performance",
    "description": "Significant power increase with optimized parameters",
    "powerGain": "25%",
    "torqueGain": "20%",
    "throttleResponse": "Improved",
    "fuelConsumption": "+5%",
    "compatibility": ["Gasoline", "CNG", "Diesel"],
    "ecuModels": ["Bosch ME7.x", "Siemens SIMOS", "Delphi", "Denso"],
    "modifiedParameters": {
      "injectorPulseWidth": "+8%",
      "ignitionAdvance": "+4°",
      "boostPressure": "+0.3 bar",
      "lambdaTarget": "0.95",
      "maxRPM": "+300"
    },
    "reliability": "High",
    "warranty": "Will void OEM warranty",
    "checkEngineRisk": "Low"
  }
}
```

### Select Tune
```
POST /api/tunes/select
Content-Type: application/json

{
  "tuneId": "POWER_INCREASE_25"
}
```

Response:
```json
{
  "status": "success",
  "tune": { ... },
  "message": "Tune POWER_INCREASE_25 selected for modification"
}
```

### Apply Tune to ECU
```
POST /api/tunes/apply
Content-Type: application/json

{
  "tuneId": "POWER_INCREASE_25",
  "ecuId": "ECU_0x12345",
  "vehicleId": "VIN_12345678"
}
```

Response:
```json
{
  "status": "success",
  "modification": {
    "tuneId": "POWER_INCREASE_25",
    "ecuId": "ECU_0x12345",
    "vehicleId": "VIN_12345678",
    "appliedAt": "2025-12-30T10:30:00Z",
    "status": "applied",
    "checkEngineRisk": "Low",
    "sensorSpoofingRequired": []
  }
}
```

### Revert to Stock Parameters
```
POST /api/tunes/revert
Content-Type: application/json

{
  "ecuId": "ECU_0x12345"
}
```

Response:
```json
{
  "status": "success",
  "message": "ECU ECU_0x12345 reverted to stock parameters"
}
```

### Get Modification History
```
GET /api/tunes/history/ECU_0x12345?limit=50
```

Response:
```json
{
  "status": "success",
  "history": [
    {
      "tuneId": "POWER_INCREASE_25",
      "ecuId": "ECU_0x12345",
      "vehicleId": "VIN_12345678",
      "appliedAt": "2025-12-30T10:30:00Z",
      "status": "applied"
    },
    ...
  ]
}
```

## Tune Compatibility

### ECU Model Support
Each tune specifies compatible ECU models:
- Bosch ME7.x (Audi, VW, Skoda, Seat)
- Bosch EDC17 (Diesel vehicles)
- Siemens SIMOS (Audi, Porsche, Lamborghini)
- Delphi DCM (Ford, some Chinese manufacturers)
- Denso (Toyota, Lexus, Daihatsu)
- Continental (VW, Audi, Skoda)

### Fuel Type Compatibility
- Gasoline (RON 95+)
- Gasoline Premium (RON 98+)
- Diesel
- CNG (Compressed Natural Gas)

### Conflict Resolution
The Tune Manager prevents incompatible tunes:
- Cannot apply both "Emissions Off" and "Emissions Off + Spoofing"
- Cannot apply "Economy Mode" with "Race Tune" simultaneously
- Automatic DPF settings when using "Emissions Off"

## Usage Examples

### Example 1: Performance Tuning a Diesel Vehicle
```bash
# Get all diesel-compatible tunes
curl -X POST http://localhost:3000/api/tunes/compatible \
  -H "Content-Type: application/json" \
  -d '{"fuelType": "Diesel", "ecuModel": "Bosch EDC17"}'

# Get details on turbo boost tune
curl http://localhost:3000/api/tunes/details/TURBO_BOOST_INCREASE

# Select and apply the tune
curl -X POST http://localhost:3000/api/tunes/select \
  -H "Content-Type: application/json" \
  -d '{"tuneId": "TURBO_BOOST_INCREASE"}'

curl -X POST http://localhost:3000/api/tunes/apply \
  -H "Content-Type: application/json" \
  -d '{
    "tuneId": "TURBO_BOOST_INCREASE",
    "ecuId": "EDC17_DIESEL_2012",
    "vehicleId": "VW_GOLF_2011"
  }'

# Check history
curl http://localhost:3000/api/tunes/history/EDC17_DIESEL_2012
```

### Example 2: Emissions Control for Diesel Vehicle
```bash
# List all available tunes
curl 'http://localhost:3000/api/tunes/list?category=Emissions%20Control'

# Get details on emissions off with spoofing
curl http://localhost:3000/api/tunes/details/EMISSIONS_OFF_WITH_SPOOFING

# Apply the tune
curl -X POST http://localhost:3000/api/tunes/apply \
  -H "Content-Type: application/json" \
  -d '{
    "tuneId": "EMISSIONS_OFF_WITH_SPOOFING",
    "ecuId": "DIESEL_ECU_001",
    "vehicleId": "MERCEDES_C250_2012"
  }'
```

### Example 3: Fuel Economy Mode
```bash
# Get economy tunes
curl 'http://localhost:3000/api/tunes/list?category=Efficiency'

# Apply economy mode
curl -X POST http://localhost:3000/api/tunes/apply \
  -H "Content-Type: application/json" \
  -d '{
    "tuneId": "ECONOMY_MODE",
    "ecuId": "ECU_GASOLINE",
    "vehicleId": "VW_JETTA_2010"
  }'
```

### Example 4: Revert to Stock
```bash
# Revert ECU to factory settings
curl -X POST http://localhost:3000/api/tunes/revert \
  -H "Content-Type: application/json" \
  -d '{"ecuId": "ECU_GASOLINE"}'
```

## Parameter Adjustments

### Common Modified ECU Parameters

**Fuel System:**
- Injector Pulse Width: ±15%
- Injection Timing: ±5°
- Fuel Pressure: ±150 bar
- Fuel Rail Pressure: ±200 bar

**Ignition System:**
- Ignition Advance: ±8°
- Knock Threshold: Adjustable
- Safety Margins: Customizable

**Turbocharger:**
- Boost Pressure: ±1.5 bar
- Waste Gate Control: Variable
- Anti-Lag System: Optional

**Lambda Control:**
- Target Lambda: 0.90-1.10
- Adaptive Learning: Enabled/Disabled
- Closed-Loop Monitoring: Variable

**RPM Limits:**
- Maximum RPM: ±1500
- Speed Limiter: Configurable
- Shift Cut: Adjustable

## Warranty and Risk

### Warranty Impact
- **Very High Reliability Tunes**: Warranty may not be affected
- **High Reliability Tunes**: Warranty may be reduced or voided
- **Medium Reliability Tunes**: Warranty will likely be voided
- **Low Reliability Tunes**: Complete warranty loss

### Check Engine Light Risk
- **None**: No risk of fault codes
- **Very Low**: <1% risk (requires extreme conditions)
- **Low**: 1-5% risk
- **Medium**: 5-20% risk
- **High**: >20% risk

### Legal Status
⚠️ **WARNING**: Some tunes are illegal for road use:
- Emissions disabled tunes (road use illegal in EU, US, most countries)
- Sensor spoofing (illegal if used to bypass emissions testing)
- Speed limiter removal (illegal in many countries)

Legal uses include:
- Racing/track use on private property
- Testing and development
- Diagnostic purposes
- Research

## Integration with Other Systems

The Tune Manager integrates with:
- **Emissions Controller**: For emissions-related tunes
- **ECU Processor**: For applying tunes to ECU
- **Diagnostics Engine**: For verifying tune application
- **J2534 Interface**: For communicating with vehicle

## Advanced Topics

### Custom Tune Creation
Users can create custom tunes by:
1. Selecting a base tune
2. Modifying specific parameters
3. Testing reliability
4. Saving as custom profile

### Tune Stacking
Some tunes can be combined:
- Anti-Knock Protection + Performance tune
- Idle Stability + Performance tune
- But NOT: Performance + Efficiency simultaneously

### Parameter Optimization
Fine-tune individual parameters:
- Boost pressure increase only
- Timing advance only
- Fuel mixture only
- Or combination of all three
