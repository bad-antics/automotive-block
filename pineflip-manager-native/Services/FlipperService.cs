using System;
using System.Collections.Generic;
using PineFlipManager.Models;

namespace PineFlipManager.Services
{
    public class FlipperService
    {
        private readonly SerialPortService _serialService;
        private string _currentPort = string.Empty;

        public FlipperService()
        {
            _serialService = new SerialPortService();
        }

        public bool AutoConnect(out string portName)
        {
            if (_serialService.IsOpen)
            {
                portName = _currentPort;
                return true;
            }

            if (_serialService.FindAndOpenFirstPort(out portName))
            {
                _currentPort = portName;
                return true;
            }

            return false;
        }

        public string SendCommand(string command)
        {
            if (!_serialService.IsOpen)
                return "Not connected";

            if (_serialService.SendLine(command))
            {
                // Wait for command to process
                System.Threading.Thread.Sleep(600);
                return _serialService.ReadAvailable(500);
            }

            return "Command failed";
        }

        public FlipperDevice GetDeviceInfo()
        {
            var device = new FlipperDevice();

            if (AutoConnect(out string port))
            {
                device.PortName = port;
                device.IsConnected = true;
                device.DeviceInfo = SendCommand("info device");
                device.Uptime = SendCommand("uptime");
                device.MemoryInfo = SendCommand("free");
                
                // Extract firmware version from device info
                var lines = device.DeviceInfo.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("firmware", StringComparison.OrdinalIgnoreCase))
                    {
                        device.FirmwareVersion = line.Trim();
                        break;
                    }
                }
            }
            else
            {
                device.IsConnected = false;
                device.DeviceInfo = "Flipper Zero not found";
            }

            return device;
        }

        public List<FileEntry> ListDirectory(string path)
        {
            var files = new List<FileEntry>();
            var response = SendCommand($"storage list {path}");

            if (string.IsNullOrEmpty(response) || response.Contains("Error"))
                return files;

            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Parse directory entries (format: [D] or [F] name size)
                bool isDir = trimmed.StartsWith("[D]");
                bool isFile = trimmed.StartsWith("[F]");

                if (isDir || isFile)
                {
                    var parts = trimmed.Substring(3).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        files.Add(new FileEntry
                        {
                            Name = parts[0],
                            Path = System.IO.Path.Combine(path, parts[0]),
                            IsDirectory = isDir,
                            Size = parts.Length > 1 && long.TryParse(parts[1], out long size) ? size : 0
                        });
                    }
                }
            }

            return files;
        }

        public string ReadFile(string filePath)
        {
            return SendCommand($"storage read {filePath}");
        }

        public void Disconnect()
        {
            _serialService.Close();
            _currentPort = string.Empty;
        }
    }

    public class FileEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
    }
}
