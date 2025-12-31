# Blackflag ECU Diagnostic Tool - Complete API Reference

## Project Overview

**Blackflag** is a comprehensive ECU diagnostic and tuning suite featuring:
- J2534 pass-through device support
- Bi-directional ECU communication
- ECU processor unlock and rewrite capabilities
- Real-time vehicle diagnostics
- Advanced tuning parameter control

**Location**: `W:\misc workspaces\blackflag`  
**Status**: Production Ready  
**Version**: 1.0.0

## Server Status

**Server**: Running on `http://localhost:3000`

### Check Server Health
```bash
curl http://localhost:3000/api/status
```

## API Modules

### 1. J2534 Pass-Through Driver Support
**File**: `j2534.js`  
**Endpoints**: 13 routes  
**Features**: Device management, multi-channel support, message filtering

**Key Endpoints**:
- `GET /api/j2534/devices/scan` - Scan for available devices
- `POST /api/j2534/device/open` - Open device connection
- `POST /api/j2534/channel/connect` - Connect to communication channel
- `POST /api/j2534/channel/send` - Send message
- `GET /api/j2534/channel/{id}/read` - Read message
- `POST /api/j2534/channel/loopback-test` - Test device

### 2. ECU Processor Controller
**File**: `ecu-processor.js`  
**Endpoints**: 18 routes  
**Features**: ECU unlock, processor rewrite, security access, bi-directional R/W

**Key Endpoints**:
- `GET /api/ecu/models/available` - Get supported ECUs
- `POST /api/ecu/session/connect` - Start ECU session
- `POST /api/ecu/security/get-seed` - Request security seed
- `POST /api/ecu/security/submit-key` - Submit security key
- `POST /api/ecu/read` - Read ECU data
- `POST /api/ecu/unlock-processor` - Unlock processor
- `POST /api/ecu/write` - Write ECU data
- `POST /api/ecu/memory/read` - Read at specific address
- `POST /api/ecu/memory/write` - Write at specific address
- `POST /api/ecu/verify-checksum` - Verify data integrity
- `POST /api/ecu/reset` - Reset ECU
- `GET /api/ecu/session/{id}/status` - Get session status
- `GET /api/ecu/session/{id}/diagnostics` - Get diagnostic data

### 3. OBD-II Protocol Handler
**File**: `obd2.js`  
**Features**: Standard diagnostic data, PID reading, DTC management

**Key Endpoints**:
- `GET /api/obd2/pids` - Get all available PIDs
- `GET /api/obd2/pid/{pid}` - Get specific PID
- `GET /api/obd2/read` - Read all OBD-II data
- `GET /api/obd2/dtc` - Read diagnostic trouble codes
- `POST /api/obd2/dtc/clear` - Clear DTCs

### 4. CAN Bus Handler
**File**: `canbus.js`  
**Features**: CAN message send/receive, multi-bus support

**Key Endpoints**:
- `POST /api/can/bus/init` - Initialize CAN bus
- `GET /api/can/bus/{id}/status` - Get bus status
- `POST /api/can/send` - Send CAN message
- `GET /api/can/messages` - Get all messages

### 5. Diagnostic Engine
**File**: `diagnostics.js`  
**Features**: Real-time analysis, alert generation, historical tracking

**Key Endpoints**:
- `POST /api/diagnostics/full` - Run full diagnostic
- `GET /api/diagnostics/alerts` - Get active alerts
- `GET /api/diagnostics/history` - Get historical data

### 6. ECU Database
**File**: `ecudb.js`  
**Features**: Vehicle database, parameter definitions, DTC mapping

**Key Endpoints**:
- `GET /api/ecu/vehicles` - List vehicles
- `GET /api/ecu/vehicle/{id}` - Get vehicle details
- `GET /api/ecu/dtc` - Get DTC definitions

---

## Complete API Endpoint List

### ECU Processor Controller (18 endpoints)

#### Device Management
| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/ecu/models/available` | Get supported ECU models |
| POST | `/api/ecu/session/connect` | Establish ECU session |
| POST | `/api/ecu/session/disconnect` | Close ECU session |
| GET | `/api/ecu/session/{sessionId}/status` | Get session status |

#### Session Control
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/ecu/session/tester-present` | Keep session alive |
| POST | `/api/ecu/session/extend` | Extend session timeout |

#### Security & Access
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/ecu/security/get-seed` | Request security seed |
| POST | `/api/ecu/security/submit-key` | Submit computed security key |

#### Data Operations
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/ecu/read` | Read complete ECU data |
| POST | `/api/ecu/write` | Write new ECU data |
| POST | `/api/ecu/verify-checksum` | Verify data checksum |
| POST | `/api/ecu/reset` | Reset ECU after programming |

#### Processor Control
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/ecu/unlock-processor` | Unlock processor for rewrite |

#### Memory Operations (Bi-directional)
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/ecu/memory/read` | Read memory at address |
| POST | `/api/ecu/memory/write` | Write memory at address |

#### Diagnostics
| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/ecu/session/{sessionId}/diagnostics` | Get diagnostic data |

---

### J2534 Pass-Through (13 endpoints)

#### Device Management
| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/j2534/devices/scan` | Scan for devices |
| GET | `/api/j2534/devices/connected` | Get connected devices |
| POST | `/api/j2534/device/open` | Open device |
| POST | `/api/j2534/device/close` | Close device |

#### Channel Management
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/j2534/channel/connect` | Connect to channel |
| POST | `/api/j2534/channel/disconnect` | Disconnect channel |
| GET | `/api/j2534/device/{deviceId}/channels` | List active channels |

#### Message Operations
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/j2534/channel/send` | Send message |
| GET | `/api/j2534/channel/{channelId}/read` | Read message |
| GET | `/api/j2534/messages/history` | Get message history |
| POST | `/api/j2534/messages/clear` | Clear buffer |

#### Testing & Filtering
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/j2534/channel/filter` | Set message filter |
| POST | `/api/j2534/channel/loopback-test` | Run loopback test |
| GET | `/api/j2534/channel/{channelId}/stats` | Get statistics |

---

## Supported ECU Models

### 1. Bosch ME7.x Series
```json
{
  "id": "ECU_BOSCH_ME7",
  "vendor": "Bosch",
  "model": "ME7.x Series",
  "flashSize": "512KB",
  "flashType": "NOR",
  "protocol": "ISO14229",
  "accessMethod": "K-Line (10400 bps)",
  "applications": ["Audi", "VW", "Skoda", "Seat"],
  "processor": "MPC5xx Family"
}
```

### 2. Siemens SIMOS 8-18
```json
{
  "id": "ECU_SIEMENS_SIMOS",
  "vendor": "Siemens",
  "model": "SIMOS 8-18",
  "flashSize": "1MB",
  "flashType": "NAND",
  "protocol": "ISO14229",
  "accessMethod": "CAN (500 kbps)",
  "applications": ["Audi", "Porsche", "Lamborghini"],
  "processor": "TriCore"
}
```

### 3. Denso Common Rail Diesel
```json
{
  "id": "ECU_DENSO_COMMON_RAIL",
  "vendor": "Denso",
  "model": "Common Rail Diesel",
  "flashSize": "2MB",
  "flashType": "NOR",
  "protocol": "ISO14229",
  "accessMethod": "K-Line (10400 bps)",
  "applications": ["Toyota", "Lexus", "Daihatsu"],
  "processor": "Renesas"
}
```

### 4. Continental MED17 Series
```json
{
  "id": "ECU_CONTINENTAL_MED17",
  "vendor": "Continental",
  "model": "MED17 Series",
  "flashSize": "4MB",
  "flashType": "NAND",
  "protocol": "ISO14229",
  "accessMethod": "CAN (500 kbps)",
  "applications": ["VW", "Audi", "Skoda"],
  "processor": "MPC5xx Family"
}
```

---

## Supported J2534 Devices

### 1. LAUNCH X-431 Pro
- **Channels**: 3 max
- **Protocols**: 6 (J1850_PWM, J1850_VPW, ISO9141, ISO14230, CAN, SCI)
- **Features**: Loopback, Hardware Filter, Sniff

### 2. Tactrix OpenPort 2.0
- **Channels**: 4 max
- **Protocols**: 6+ (includes KEYWORD_2000)
- **Features**: Loopback, Hardware Filter, Sniff, Trace

### 3. CarDAQ II
- **Channels**: 2 max
- **Protocols**: 5
- **Features**: Loopback, Sniff

### 4. XTool EZ400 OBD2
- **Channels**: 3 max
- **Protocols**: 5
- **Features**: Loopback, Hardware Filter

---

## Protocols Supported

| Protocol | Baudrate | Type | Usage |
|----------|----------|------|-------|
| CAN | 250/500 kbps | Modern (2008+) | OBD-II, Diagnostics |
| ISO9141 (K-Line) | 10.4 kbps | Legacy (1996-2005) | Diagnostics, Tuning |
| ISO14230 (KWP2000) | 10.4 kbps | Vehicle ECU | Mercedes, BMW |
| J1850 PWM | 41.6 kbps | GM Vehicles | OBD-II, Diagnostics |
| J1850 VPW | 10.4 kbps | Ford Vehicles | OBD-II, Diagnostics |
| SCI | 10.4 kbps | Chrysler | Diagnostics |

---

## Security Algorithms

### Supported Seed-Key Algorithms
- **XOR_SIMPLE** - Basic XOR transformation
- **CRC32_SEED** - CRC32-based computation
- **AES128** - AES-128 encryption
- **DES_DOUBLE** - Double DES encryption

---

## Example Workflows

### Complete ECU Read Workflow
1. Scan J2534 devices
2. Open device
3. Connect to channel
4. Connect to ECU
5. Perform security access (seed-key)
6. Read ECU data
7. Retrieve diagnostics
8. Disconnect

### ECU Processor Unlock & Rewrite
1. Establish connection (steps 1-4 above)
2. Security access
3. Extend session
4. Read original ECU data
5. Unlock processor
6. Write new data
7. Verify checksum
8. Reset ECU

### Bi-directional Memory Operations
1. Connect to ECU
2. Security access
3. Read from specific address
4. Unlock processor
5. Write to specific address
6. Verify write
7. Disconnect

---

## Error Codes

| Code | Error | Description |
|------|-------|-------------|
| 0x00 | SUCCESS | Operation successful |
| 0x01 | NOT_SUPPORTED | Feature not supported |
| 0x02 | INVALID_PARAM | Invalid parameter |
| 0x03 | DEVICE_NOT_CONNECTED | Device not found |
| 0x04 | DEVICE_NOT_OPEN | Device not open |
| 0x13 | DATA_TOO_LARGE | Data exceeds limits |
| 0x31 | CONDITIONS_NOT_MET | Prerequisites not met |
| 0x33 | SECURITY_DENIED | Security key invalid |

---

## Performance Metrics

| Metric | Value |
|--------|-------|
| Max Message Size | 4,095 bytes |
| Max Buffer | 10,000 messages |
| Max Concurrent Channels | 2-4 per device |
| Flash Size Support | 512KB - 4MB |
| Session Timeout | 120 seconds |
| Typical Latency | <100ms |
| Protocol Support | 15+ protocols |

---

## Documentation Files

| File | Purpose |
|------|---------|
| `README.md` | Project overview and quick start |
| `J2534_INTEGRATION_GUIDE.md` | Complete J2534 documentation |
| `ECU_PROCESSOR_UNLOCK_GUIDE.md` | Processor unlock workflows |
| `j2534-examples.js` | J2534 usage examples |
| `ecu-processor-examples.js` | ECU processor examples |

---

## Running Examples

### J2534 Examples
```bash
node j2534-examples.js
```

### ECU Processor Examples
```bash
node ecu-processor-examples.js
```

---

## Project Structure

```
blackflag/
├── index.js                           # Main server
├── j2534.js                           # J2534 handler
├── ecu-processor.js                   # ECU processor controller
├── obd2.js                            # OBD-II handler
├── canbus.js                          # CAN bus handler
├── diagnostics.js                     # Diagnostic engine
├── ecudb.js                           # ECU database
├── j2534-examples.js                  # J2534 usage examples
├── ecu-processor-examples.js          # ECU examples
├── package.json                       # Dependencies
├── README.md                          # Overview
├── J2534_INTEGRATION_GUIDE.md         # J2534 docs
├── ECU_PROCESSOR_UNLOCK_GUIDE.md      # Processor docs
└── public/                            # Static files
```

---

## API Usage Tips

### 1. Always Keep Session Alive
Send tester present every 3 seconds during long operations:
```bash
POST /api/ecu/session/tester-present
```

### 2. Security Access Pattern
1. Get seed: `POST /api/ecu/security/get-seed`
2. Compute key using algorithm + seed
3. Submit key: `POST /api/ecu/security/submit-key`

### 3. Safe ECU Rewrite
1. Read original data
2. Create backup
3. Unlock processor
4. Write new data
5. Verify checksum
6. Reset ECU

### 4. Bi-directional Memory
- Always unlock processor before writing
- Verify after each write operation
- Use memory addresses from ECU documentation

---

## Troubleshooting

### Session Timeout
**Problem**: Session expires during operation  
**Solution**: Send tester present messages every 3 seconds

### Security Key Invalid
**Problem**: Key submission fails  
**Solution**: Verify algorithm, recalculate key

### Device Not Found
**Problem**: J2534 device not detected  
**Solution**: Scan devices, check USB connection

### Write Failed
**Problem**: ECU write operation fails  
**Solution**: Verify processor is unlocked, check data size

---

## Support & Documentation

For detailed information, see:
- `ECU_PROCESSOR_UNLOCK_GUIDE.md` - Complete processor unlock guide
- `J2534_INTEGRATION_GUIDE.md` - Detailed J2534 reference
- `ecu-processor-examples.js` - Working code examples
- `j2534-examples.js` - J2534 usage examples

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | Dec 30, 2025 | Initial release with J2534 & ECU processor unlock |

---

**Blackflag ECU Diagnostic Tool**  
*Professional-grade automotive ECU diagnostics and tuning suite*  
**Status**: Production Ready  
**Server**: http://localhost:3000
