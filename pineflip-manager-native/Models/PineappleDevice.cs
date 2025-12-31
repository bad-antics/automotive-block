using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PineFlipManager.Models
{
    public class PineappleDevice : INotifyPropertyChanged
    {
        private string _ipAddress = "172.16.42.1";
        private int _port = 1471;
        private bool _isAuthenticated;
        private string _statusJson = string.Empty;
        private string _logs = string.Empty;
        private string _notifications = string.Empty;
        private string _username = "root";
        private string _authToken = string.Empty;
        private DateTime _lastUpdate = DateTime.Now;

        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); }
        }

        public int Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set { _isAuthenticated = value; OnPropertyChanged(); }
        }

        public string StatusJson
        {
            get => _statusJson;
            set { _statusJson = value; OnPropertyChanged(); }
        }

        public string Logs
        {
            get => _logs;
            set { _logs = value; OnPropertyChanged(); }
        }

        public string Notifications
        {
            get => _notifications;
            set { _notifications = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string AuthToken
        {
            get => _authToken;
            set { _authToken = value; OnPropertyChanged(); }
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
