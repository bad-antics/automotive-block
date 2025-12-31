using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PineFlipManager.Models
{
    public class FlipperDevice : INotifyPropertyChanged
    {
        private string _portName = string.Empty;
        private bool _isConnected;
        private string _deviceInfo = string.Empty;
        private string _uptime = string.Empty;
        private string _memoryInfo = string.Empty;
        private string _firmwareVersion = string.Empty;
        private DateTime _lastUpdate = DateTime.Now;

        public string PortName
        {
            get => _portName;
            set { _portName = value; OnPropertyChanged(); }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(); }
        }

        public string DeviceInfo
        {
            get => _deviceInfo;
            set { _deviceInfo = value; OnPropertyChanged(); }
        }

        public string Uptime
        {
            get => _uptime;
            set { _uptime = value; OnPropertyChanged(); }
        }

        public string MemoryInfo
        {
            get => _memoryInfo;
            set { _memoryInfo = value; OnPropertyChanged(); }
        }

        public string FirmwareVersion
        {
            get => _firmwareVersion;
            set { _firmwareVersion = value; OnPropertyChanged(); }
        }

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set { _lastUpdate = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
