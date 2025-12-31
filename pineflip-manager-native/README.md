# PineFlip Manager Native

**Built by antx | Bad Antics**

A unified native Windows application for managing both Flipper Zero and WiFi Pineapple devices from a single interface.

## Features

### Flipper Zero Support
- **Auto-Connect**: Automatically finds and connects to Flipper Zero via serial (COM1-COM40 scan)
- **Device Info**: View firmware version, uptime, and memory statistics
- **Command Console**: Send direct commands to Flipper Zero CLI
- **Real-time Output**: Live serial output display
- **File Operations**: Built-in support for storage commands

### WiFi Pineapple Support
- **Network Discovery**: Auto-discovers Pineapple on 172.16.x.x networks
- **Status Monitoring**: Fetch device status via API
- **Web UI Integration**: Quick launch to Pineapple web interface
- **Multi-port Detection**: Scans both port 1471 and 80

### Unified Interface
- **Tab-based UI**: Switch between Flipper and Pineapple seamlessly
- **Hacker Aesthetic**: Terminal-style green-on-black theme
- **Standalone Executable**: Single .exe file, no installation required
- **Background Services**: Async device communication

## System Requirements

- **OS**: Windows 10/11 (x64)
- **Flipper Zero**: USB serial connection (230400 baud)
- **WiFi Pineapple**: Network connection to 172.16.x.x subnet
- **Runtime**: Self-contained (.NET 10.0 embedded)

## Quick Start

1. **Connect Devices**:
   - Plug in Flipper Zero via USB
   - Connect to WiFi Pineapple's wireless network

2. **Launch**:
   - Run `PineFlipManager.exe`
   - Devices will auto-connect on startup

3. **Flipper Zero Tab**:
   - Click **CONNECT** to manually connect
   - Use **REFRESH INFO** to get device details
   - Enter commands in the input box and click **SEND**

4. **WiFi Pineapple Tab**:
   - Click **DISCOVER** to find Pineapple on network
   - Use **GET STATUS** to fetch API status
   - Click **OPEN WEB UI** to launch browser

## Technical Details

- **Framework**: .NET 10.0 WPF
- **Serial**: System.IO.Ports (230400 baud, 8N1)
- **HTTP**: System.Net.Http (async API calls)
- **Discovery**: Network interface scanning for 172.16.x.x
- **UI**: XAML with custom hacker theme

## Serial Commands (Flipper)

Common Flipper Zero CLI commands:
```
info device    - Device information
uptime         - System uptime
free           - Memory stats
storage list / - List root directory
help           - Show all commands
```

## API Endpoints (Pineapple)

Automatically probes:
- `http://172.16.x.1:1471/api/status`
- `http://172.16.x.1/api/status`

## File Structure

```
PineFlipManager.exe          # Standalone executable (140MB)
```

## Version

**v1.0.0** - Initial Release
- Dual device management
- Auto-connect functionality
- Hacker-themed UI
- Self-contained deployment

## Credits

**Bad Antics | Built by antx**

Combines Flipper Zero and WiFi Pineapple management into a unified native Windows application.

## License

Built by antx for Bad Antics
Copyright © 2025
