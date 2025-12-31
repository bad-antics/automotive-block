using System;
using System.Linq;
using System.IO.Ports;

namespace PineFlipManager.Services
{
    public class SerialPortService
    {
        private SerialPort? _serialPort;
        private const int BaudRate = 230400;
        private const int ReadTimeout = 1000;
        private const int WriteTimeout = 1000;

        public bool FindAndOpenFirstPort(out string portName)
        {
            portName = string.Empty;

            // Scan COM1 through COM40
            for (int i = 1; i <= 40; i++)
            {
                string testPort = $"COM{i}";
                try
                {
                    // Check if port exists
                    if (SerialPort.GetPortNames().Contains(testPort))
                    {
                        var testSerial = new SerialPort(testPort, BaudRate)
                        {
                            DataBits = 8,
                            Parity = Parity.None,
                            StopBits = StopBits.One,
                            DtrEnable = false,
                            RtsEnable = false,
                            ReadTimeout = ReadTimeout,
                            WriteTimeout = WriteTimeout
                        };

                        testSerial.Open();
                        
                        _serialPort?.Close();
                        _serialPort = testSerial;
                        portName = testPort;
                        
                        // Give device time to initialize
                        System.Threading.Thread.Sleep(100);
                        return true;
                    }
                }
                catch
                {
                    // Port unavailable or in use, continue scanning
                    continue;
                }
            }

            return false;
        }

        public bool SendLine(string command)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
                return false;

            try
            {
                _serialPort.WriteLine(command);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ReadAvailable(int timeoutMs = 500)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
                return string.Empty;

            try
            {
                var buffer = new System.Text.StringBuilder();
                var startTime = DateTime.Now;

                while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        buffer.Append(_serialPort.ReadExisting());
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(20);
                    }
                }

                return buffer.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        public bool IsOpen => _serialPort?.IsOpen ?? false;
    }
}
