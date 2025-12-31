# ECU Processor Unlock & Rewrite Guide

## Overview

Blackflag now includes advanced ECU processor unlock and rewrite capabilities with full bi-directional communication. This guide covers the complete workflow for reading, unlocking, and rewriting ECU processor firmware.

## Supported ECU Models

### 1. Bosch ME7.x Series
- **Vendor**: Bosch
- **Flash Type**: NOR (512KB)
- **Protocol**: UDS (ISO 14229)
- **Access Method**: K-Line (ISO 9141)
- **Applications**: Audi, VW, Skoda, Seat
- **Processor**: MPC5xx Family

### 2. Siemens SIMOS 8-18
- **Vendor**: Siemens
- **Flash Type**: NAND (1MB)
- **Protocol**: UDS (ISO 14229)
- **Access Method**: CAN
- **Applications**: Audi, Porsche, Lamborghini
- **Processor**: TriCore

### 3. Denso Common Rail Diesel
- **Vendor**: Denso
- **Flash Type**: NOR (2MB)
- **Protocol**: UDS (ISO 14229)
- **Access Method**: K-Line (ISO 9141)
- **Applications**: Toyota, Lexus, Daihatsu
- **Processor**: Renesas

### 4. Continental MED17 Series
- **Vendor**: Continental
- **Flash Type**: NAND (4MB)
- **Protocol**: UDS (ISO 14229)
- **Access Method**: CAN
- **Applications**: Volkswagen, Audi, Skoda
- **Processor**: MPC5xx Family

## Complete Workflow

### Step 1: Get Available ECU Models
```bash
curl http://localhost:3000/api/ecu/models/available
```

**Response:**
```json
{
  "status": "success",
  "count": 4,
  "ecus": [
    {
      "id": "ECU_BOSCH_ME7",
      "vendor": "Bosch",
      "model": "ME7.x Series",
      "description": "Common engine control unit for petrol engines",
      "protocol": "ISO14229",
      "flashSize": "512KB",
      "features": ["read_ecu", "unlock_processor", "rewrite_flash", "verify_checksum"]
    }
  ]
}
```

### Step 2: Connect J2534 Device and Open Channel
```bash
# Open J2534 device
curl -X POST http://localhost:3000/api/j2534/device/open \
  -H "Content-Type: application/json" \
  -d '{"deviceId": "device_001"}'

# Connect to appropriate channel
curl -X POST http://localhost:3000/api/j2534/channel/connect \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "device_001",
    "protocol": "ISO9141",
    "baudrate": 10400
  }'
```

### Step 3: Connect to ECU Session
```bash
curl -X POST http://localhost:3000/api/ecu/session/connect \
  -H "Content-Type: application/json" \
  -d '{
    "channelId": "device_001_ch_0",
    "ecuId": "ECU_BOSCH_ME7"
  }'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "ecuModel": "ME7.x Series",
  "vendor": "Bosch",
  "flashSize": 524288,
  "processorType": "MPC5xx Family",
  "protocol": "ISO14229",
  "message": "ECU session established"
}
```

### Step 4: Keep Session Alive with Tester Present
```bash
curl -X POST http://localhost:3000/api/ecu/session/tester-present \
  -H "Content-Type: application/json" \
  -d '{"sessionId": "session_1735653600000_abc123def"}'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "message": "Tester present - session alive",
  "keepAliveInterval": 3000
}
```

### Step 5: Security Access - Get Seed
```bash
curl -X POST http://localhost:3000/api/ecu/security/get-seed \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": "session_1735653600000_abc123def",
    "level": 1
  }'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "accessLevel": 1,
  "seed": "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6",
  "seedLength": 16,
  "algorithm": "XOR_SIMPLE",
  "expectedKeyFormat": "hex_string",
  "message": "Seed generated - compute key using algorithm and send key",
  "instructions": "Use XOR_SIMPLE algorithm with seed to compute key"
}
```

### Step 6: Calculate and Submit Security Key
Using the seed and the appropriate algorithm, calculate the key:

```bash
curl -X POST http://localhost:3000/api/ecu/security/submit-key \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": "session_1735653600000_abc123def",
    "key": "calculated_key_here"
  }'
```

**Response (Success):**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "message": "Security key accepted",
  "securityLevel": "UNLOCKED_L1",
  "nextSteps": ["extend_session", "unlock_processor", "read_ecu"]
}
```

### Step 7: Extend Session for Long Operations
```bash
curl -X POST http://localhost:3000/api/ecu/session/extend \
  -H "Content-Type: application/json" \
  -d '{"sessionId": "session_1735653600000_abc123def"}'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "message": "Session extended for programming mode",
  "sessionTimeout": 120000,
  "note": "Keep tester present messages active"
}
```

### Step 8: Read ECU Data
```bash
curl -X POST http://localhost:3000/api/ecu/read \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": "session_1735653600000_abc123def",
    "readType": "FULL"
  }'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "ecuId": "ECU_BOSCH_ME7",
  "readType": "FULL",
  "dataSize": 524288,
  "dataFormat": "hex",
  "flashContent": "48206d6f62696c6573...",
  "checksumOriginal": "0xABCD",
  "timestamp": "2025-12-30T12:00:00.000Z",
  "message": "ECU data read successfully (524288 bytes)",
  "canBeUnlocked": true
}
```

### Step 9: Unlock ECU Processor
```bash
curl -X POST http://localhost:3000/api/ecu/unlock-processor \
  -H "Content-Type: application/json" \
  -d '{"sessionId": "session_1735653600000_abc123def"}'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "processorUnlocked": true,
  "securityLevel": "UNLOCKED_FULL",
  "flashType": "NOR",
  "unlockMethod": "DISABLE_WRITE_PROTECTION",
  "unlockSequence": [
    "Disable JTAG interface",
    "Disable flash read-out protection",
    "Disable flash write protection",
    "Switch to programming mode",
    "Unlock all flash sectors"
  ],
  "readyForRewrite": true,
  "message": "ECU processor successfully unlocked - ready for rewriting",
  "nextSteps": ["write_ecu_data", "verify_checksum"]
}
```

### Step 10: Write New ECU Data (Rewrite Processor)
```bash
curl -X POST http://localhost:3000/api/ecu/write \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": "session_1735653600000_abc123def",
    "data": "new_hex_data_content_here"
  }'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "bytesWritten": 524288,
  "targetFlash": "NOR",
  "flashSize": 524288,
  "usedSpace": "100%",
  "checksumNew": "0xDEF0",
  "checksumOld": "0xABCD",
  "message": "ECU data written successfully",
  "requiresVerification": true,
  "nextSteps": ["verify_checksum", "reset_ecu"]
}
```

### Step 11: Verify Checksum
```bash
curl -X POST http://localhost:3000/api/ecu/verify-checksum \
  -H "Content-Type: application/json" \
  -d '{"sessionId": "session_1735653600000_abc123def"}'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "checksumValid": true,
  "checksumValue": "0xDEF0",
  "verification": "PASSED",
  "message": "Checksum verification passed",
  "nextSteps": ["reset_ecu", "disconnect"]
}
```

### Step 12: Reset ECU
```bash
curl -X POST http://localhost:3000/api/ecu/reset \
  -H "Content-Type: application/json" \
  -d '{"sessionId": "session_1735653600000_abc123def"}'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "message": "ECU reset initiated",
  "resetType": "HARD_RESET",
  "estimatedBootTime": 5000,
  "note": "Keep power supplied during reset"
}
```

### Step 13: Disconnect ECU Session
```bash
curl -X POST http://localhost:3000/api/ecu/session/disconnect \
  -H "Content-Type: application/json" \
  -d '{"sessionId": "session_1735653600000_abc123def"}'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "message": "ECU session closed",
  "sessionDuration": "120s",
  "processorUnlocked": true,
  "dataRead": true
}
```

## Bi-Directional Memory Operations

### Read Memory at Specific Address
```bash
curl -X POST http://localhost:3000/api/ecu/memory/read \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": "session_1735653600000_abc123def",
    "address": "0x1000",
    "length": 256
  }'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "address": "0x1000",
  "length": 256,
  "data": "48206d6f62696c6573...",
  "dataPreview": "48206d6f62696c6573...",
  "message": "Read 256 bytes from memory"
}
```

### Write Memory at Specific Address
```bash
curl -X POST http://localhost:3000/api/ecu/memory/write \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": "session_1735653600000_abc123def",
    "address": "0x2000",
    "data": "new_data_hex_string"
  }'
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "address": "0x2000",
  "bytesWritten": 128,
  "message": "Wrote 128 bytes to memory",
  "requiresVerification": true
}
```

## Get ECU Diagnostic Data
```bash
curl http://localhost:3000/api/ecu/session/session_1735653600000_abc123def/diagnostics
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "ecuModel": "ME7.x Series",
  "diagnosticData": {
    "hardwareVersion": "v5.2",
    "softwareVersion": "v45.67",
    "supplierVersion": "Bosch",
    "systemSerialNumber": "BOSCH-ABC12345",
    "calibrationIdentificationNumber": "CALID-987654",
    "calibrationVerificationNumber": "F1A2B3C4",
    "systemSupplierCode": "Bosch",
    "ecuHardwareNumber": "MPC5xx Family",
    "ecuSoftwareNumber": "SW-ME7.x Series",
    "systemType": "Common engine control unit for petrol engines"
  }
}
```

## Get Session Status
```bash
curl http://localhost:3000/api/ecu/session/session_1735653600000_abc123def/status
```

**Response:**
```json
{
  "status": "success",
  "sessionId": "session_1735653600000_abc123def",
  "ecuModel": "ME7.x Series",
  "sessionStatus": "CONNECTED",
  "uptime": 125000,
  "securityLevel": "UNLOCKED_FULL",
  "processorUnlocked": true,
  "readProgress": 100,
  "unlockProgress": 100,
  "steps": [
    {
      "step": "connect",
      "status": "completed",
      "completedAt": "2025-12-30T12:00:05.000Z"
    },
    {
      "step": "tester_present",
      "status": "completed",
      "completedAt": "2025-12-30T12:00:10.000Z"
    },
    {
      "step": "security_access",
      "status": "completed",
      "completedAt": "2025-12-30T12:00:20.000Z"
    },
    {
      "step": "extend_session",
      "status": "completed",
      "completedAt": "2025-12-30T12:00:25.000Z"
    },
    {
      "step": "unlock_processor",
      "status": "completed",
      "completedAt": "2025-12-30T12:00:45.000Z"
    }
  ],
  "ecuDataRead": true
}
```

## Security Algorithms

### Supported Seed-Key Algorithms
1. **XOR_SIMPLE** - Simple XOR transformation
2. **CRC32_SEED** - CRC32-based algorithm
3. **AES128** - AES-128 encryption
4. **DES_DOUBLE** - Double DES encryption

### Example: XOR_SIMPLE Implementation
```python
def calculate_key(seed_hex):
    seed = bytes.fromhex(seed_hex)
    key = bytearray()
    for byte in seed:
        key.append(byte ^ 0xAA)  # XOR with constant
    return key.hex()
```

## Error Codes

| Code | Error | Description |
|------|-------|-------------|
| 0x01 | ECU_NOT_FOUND | ECU model not found |
| 0x04 | SESSION_NOT_FOUND | Session ID invalid or expired |
| 0x13 | DATA_TOO_LARGE | Write data exceeds flash size |
| 0x31 | CONDITIONS_NOT_MET | Prerequisites not met for operation |
| 0x33 | SECURITY_ACCESS_DENIED | Security key invalid or access denied |

## Important Notes

⚠️ **WARNING**: ECU programming is a critical operation. Always:
1. Back up original ECU data before rewriting
2. Ensure stable power supply during programming
3. Use correct ECU model identifier
4. Verify checksums match expected values
5. Keep tester present messages active during session
6. Never interrupt during write operation

## Performance Characteristics

- **Read Speed**: Variable (depends on protocol)
- **Write Speed**: Variable (depends on flash type)
- **Max Data Size**: 4MB (NAND) / 512KB (NOR)
- **Session Timeout**: 120 seconds
- **Retry Attempts**: 3 (security key)

## Troubleshooting

### Session Timeout
- Keep sending tester present messages every 3 seconds
- Use `POST /api/ecu/session/extend` to extend timeout

### Security Key Invalid
- Verify algorithm selection
- Double-check seed value
- Ensure key calculation is correct
- Check remaining attempts

### Write Failed
- Verify processor is unlocked
- Check data size against flash capacity
- Ensure security level is UNLOCKED_FULL

---

**Version**: 1.0.0  
**Last Updated**: December 30, 2025  
**Status**: Production Ready
