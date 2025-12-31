using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Threading.Tasks;
using PineFlipManager.Models;

namespace PineFlipManager.Services
{
    public class PineappleService
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl = "http://172.16.42.1:1471";

        public PineappleService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        private async Task<bool> ProbeEndpoint(string host, int port, string path)
        {
            try
            {
                var url = $"http://{host}:{port}{path}";
                var response = await _httpClient.GetAsync(url);
                
                // Accept 200, 401 (auth required), or 403 (forbidden) as "device found"
                return response.StatusCode == HttpStatusCode.OK ||
                       response.StatusCode == HttpStatusCode.Unauthorized ||
                       response.StatusCode == HttpStatusCode.Forbidden;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DiscoverDevice()
        {
            try
            {
                // Get all network interfaces
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var iface in interfaces)
                {
                    if (iface.OperationalStatus != OperationalStatus.Up)
                        continue;

                    var ipProps = iface.GetIPProperties();
                    foreach (var unicast in ipProps.UnicastAddresses)
                    {
                        if (unicast.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                            continue;

                        var bytes = unicast.Address.GetAddressBytes();
                        
                        // Look for 172.16.x.x network
                        if (bytes[0] == 172 && bytes[1] == 16)
                        {
                            var host = $"172.16.{bytes[2]}.1";

                            // Try port 1471 first
                            if (await ProbeEndpoint(host, 1471, "/api/status"))
                            {
                                _baseUrl = $"http://{host}:1471";
                                return true;
                            }

                            // Try port 80
                            if (await ProbeEndpoint(host, 80, "/api/status"))
                            {
                                _baseUrl = $"http://{host}";
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Discovery failed
            }

            return false;
        }

        public async Task<PineappleDevice> GetDeviceStatus()
        {
            var device = new PineappleDevice();

            try
            {
                bool discovered = await DiscoverDevice();

                if (!discovered)
                {
                    device.StatusJson = "WiFi Pineapple not found on network";
                    device.IsAuthenticated = false;
                    return device;
                }

                // Parse base URL
                var uri = new Uri(_baseUrl);
                device.IpAddress = uri.Host;
                device.Port = uri.Port;

                // Fetch status
                var statusUrl = $"{_baseUrl}/api/status";
                var response = await _httpClient.GetAsync(statusUrl);

                if (response.IsSuccessStatusCode)
                {
                    device.StatusJson = await response.Content.ReadAsStringAsync();
                    device.IsAuthenticated = true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    device.StatusJson = "Device found - Authentication required";
                    device.IsAuthenticated = false;
                }
                else
                {
                    device.StatusJson = $"Status: {response.StatusCode}";
                    device.IsAuthenticated = false;
                }
            }
            catch (Exception ex)
            {
                device.StatusJson = $"Error: {ex.Message}";
                device.IsAuthenticated = false;
            }

            return device;
        }

        public string BaseUrl => _baseUrl;
    }
}
