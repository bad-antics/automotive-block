using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using BlackFlag.Models;

namespace BlackFlag.Services
{
    public class Database
    {
        private static Database? _instance;
        public static Database Instance => _instance ??= new Database();
        
        private readonly string _dataPath;
        private List<Vehicle> _vehicles = new();
        private List<Vehicle> _vehicleHistory = new();
        private Dictionary<string, List<EcuProfile>> _ecuProfiles = new();
        private Dictionary<string, List<Tune>> _tunes = new();
        private Dictionary<string, object> _settings = new();
        
        private Database()
        {
            _dataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlackFlag");
        }
        
        public void Initialize()
        {
            // Create data directory
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
            
            // Load or create default data
            LoadVehicles();
            LoadHistory();
            LoadSettings();
            
            Console.WriteLine($"✅ BlackFlag Database initialized at: {_dataPath}");
        }
        
        private void LoadVehicles()
        {
            var vehiclesFile = Path.Combine(_dataPath, "vehicles.json");
            
            if (File.Exists(vehiclesFile))
            {
                var json = File.ReadAllText(vehiclesFile);
                _vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(json) ?? new();
                Console.WriteLine($"Loaded {_vehicles.Count} vehicles from file");
            }
            else
            {
                _vehicles = GetDefaultVehicles();
                Console.WriteLine($"Created {_vehicles.Count} default vehicles");
                SaveVehicles();
            }
        }
        
        private void SaveVehicles()
        {
            var vehiclesFile = Path.Combine(_dataPath, "vehicles.json");
            var json = JsonConvert.SerializeObject(_vehicles, Formatting.Indented);
            File.WriteAllText(vehiclesFile, json);
        }
        
        private void LoadHistory()
        {
            var historyFile = Path.Combine(_dataPath, "history.json");
            
            if (File.Exists(historyFile))
            {
                var json = File.ReadAllText(historyFile);
                _vehicleHistory = JsonConvert.DeserializeObject<List<Vehicle>>(json) ?? new();
            }
        }
        
        private void SaveHistory()
        {
            var historyFile = Path.Combine(_dataPath, "history.json");
            var json = JsonConvert.SerializeObject(_vehicleHistory, Formatting.Indented);
            File.WriteAllText(historyFile, json);
        }
        
        private void LoadSettings()
        {
            var settingsFile = Path.Combine(_dataPath, "settings.json");
            
            if (File.Exists(settingsFile))
            {
                var json = File.ReadAllText(settingsFile);
                _settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? new();
            }
        }
        
        public void SaveSettings()
        {
            var settingsFile = Path.Combine(_dataPath, "settings.json");
            var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(settingsFile, json);
        }
        
        // Vehicle Operations
        public List<Vehicle> GetVehicles() => _vehicles;
        
        public List<string> GetManufacturers() => 
            _vehicles.Select(v => v.Make).Distinct().OrderBy(m => m).ToList();
        
        public List<Vehicle> GetVehiclesByManufacturer(string manufacturer) =>
            _vehicles.Where(v => v.Make == manufacturer).ToList();
        
        public Vehicle? GetVehicleById(string id) =>
            _vehicles.FirstOrDefault(v => v.Id == id);
        
        // History Operations
        public List<Vehicle> GetVehicleHistory() => _vehicleHistory;
        
        public void AddToHistory(Vehicle vehicle)
        {
            // Remove if already exists
            _vehicleHistory.RemoveAll(v => v.Id == vehicle.Id);
            
            // Add to front
            _vehicleHistory.Insert(0, vehicle);
            
            // Keep only last 10
            if (_vehicleHistory.Count > 10)
            {
                _vehicleHistory = _vehicleHistory.Take(10).ToList();
            }
            
            SaveHistory();
        }
        
        // Settings Operations
        public T? GetSetting<T>(string key, T? defaultValue = default)
        {
            if (_settings.TryGetValue(key, out var value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return defaultValue;
        }
        
        public void SetSetting(string key, object value)
        {
            _settings[key] = value;
            SaveSettings();
        }
        
        // Default Data - Comprehensive Vehicle Database
        private List<Vehicle> GetDefaultVehicles() => new()
        {
            // === FORD ===
            new Vehicle
            {
                Id = "ford_mustang_2015",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Mustang GT",
                Year = 2015,
                Engine = "5.0L V8 Coyote",
                Transmission = "6-Speed Manual",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "435 HP",
                Torque = "400 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Ford PCM" },
                Systems = new() { "Adaptive Steering", "Launch Control", "SYNC 3" }
            },
            new Vehicle
            {
                Id = "ford_f150_2017",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-150",
                Year = 2017,
                Engine = "3.5L EcoBoost V6",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "375 HP",
                Torque = "470 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Ford Powertrain" },
                Systems = new() { "4WD", "Adaptive Cruise", "SYNC 3" }
            },
            new Vehicle
            {
                Id = "ford_f250_2019",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-250 Super Duty",
                Year = 2019,
                Engine = "6.7L Power Stroke V8 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "450 HP",
                Torque = "935 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Ford PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Exhaust Brake" }
            },
            new Vehicle
            {
                Id = "ford_focus_rs_2017",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Focus RS",
                Year = 2017,
                Engine = "2.3L EcoBoost I4",
                Transmission = "6-Speed Manual",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "350 HP",
                Torque = "350 lb-ft",
                EcuTypes = new() { "Bosch MEDC17", "Ford PCM" },
                Systems = new() { "Drift Mode", "Launch Control", "AWD Torque Vectoring" }
            },
            new Vehicle
            {
                Id = "ford_ranger_2020",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Ranger",
                Year = 2020,
                Engine = "2.3L EcoBoost I4",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "270 HP",
                Torque = "310 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Ford PCM" },
                Systems = new() { "Trail Control", "4WD", "SYNC 3" }
            },
            
            // === CHEVROLET ===
            new Vehicle
            {
                Id = "chevrolet_corvette_2020",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Corvette C8",
                Year = 2020,
                Engine = "6.2L V8 LT2",
                Transmission = "8-Speed DCT",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "495 HP",
                Torque = "470 lb-ft",
                EcuTypes = new() { "GM ECU", "Bosch DDM" },
                Systems = new() { "Magnetic Ride", "Launch Control", "PDR" }
            },
            new Vehicle
            {
                Id = "chevrolet_camaro_zl1_2019",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Camaro ZL1",
                Year = 2019,
                Engine = "6.2L Supercharged LT4 V8",
                Transmission = "10-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "650 HP",
                Torque = "650 lb-ft",
                EcuTypes = new() { "GM E38 ECU", "Bosch" },
                Systems = new() { "Magnetic Ride", "Launch Control", "Line Lock" }
            },
            new Vehicle
            {
                Id = "chevrolet_silverado_duramax_2021",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Silverado 2500HD",
                Year = 2021,
                Engine = "6.6L Duramax V8 Diesel",
                Transmission = "10-Speed Allison",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "445 HP",
                Torque = "910 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Exhaust Brake" }
            },
            new Vehicle
            {
                Id = "chevrolet_colorado_2019",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Colorado ZR2",
                Year = 2019,
                Engine = "2.8L Duramax I4 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "181 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECM" },
                Systems = new() { "DPF", "EGR", "Multimatic DSSV", "4WD" }
            },
            
            // === DODGE / RAM ===
            new Vehicle
            {
                Id = "dodge_challenger_2019",
                Make = "Dodge",
                Manufacturer = "Stellantis",
                Model = "Challenger SRT Hellcat",
                Year = 2019,
                Engine = "6.2L Supercharged HEMI V8",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "717 HP",
                Torque = "656 lb-ft",
                EcuTypes = new() { "Continental ECU", "Chrysler PCM" },
                Systems = new() { "Line Lock", "Launch Control", "SRT Drive Modes" }
            },
            new Vehicle
            {
                Id = "ram_2500_cummins_2020",
                Make = "RAM",
                Manufacturer = "Stellantis",
                Model = "2500 Laramie",
                Year = 2020,
                Engine = "6.7L Cummins I6 Turbo Diesel",
                Transmission = "6-Speed Aisin",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "370 HP",
                Torque = "850 lb-ft",
                EcuTypes = new() { "Cummins CM2350", "Chrysler PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Exhaust Brake", "4WD" }
            },
            new Vehicle
            {
                Id = "ram_3500_cummins_2021",
                Make = "RAM",
                Manufacturer = "Stellantis",
                Model = "3500 Limited",
                Year = 2021,
                Engine = "6.7L Cummins High-Output I6 Diesel",
                Transmission = "6-Speed Aisin",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "420 HP",
                Torque = "1075 lb-ft",
                EcuTypes = new() { "Cummins CM2350", "Chrysler PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Exhaust Brake", "4WD" }
            },
            new Vehicle
            {
                Id = "dodge_durango_srt_2021",
                Make = "Dodge",
                Manufacturer = "Stellantis",
                Model = "Durango SRT Hellcat",
                Year = 2021,
                Engine = "6.2L Supercharged HEMI V8",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "710 HP",
                Torque = "645 lb-ft",
                EcuTypes = new() { "Continental ECU", "Chrysler PCM" },
                Systems = new() { "Launch Control", "SRT Drive Modes", "AWD" }
            },
            
            // === BMW ===
            new Vehicle
            {
                Id = "bmw_m3_2021",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "M3 Competition",
                Year = 2021,
                Engine = "3.0L Twin-Turbo I6 S58",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "503 HP",
                Torque = "479 lb-ft",
                EcuTypes = new() { "Bosch MG1", "BMW DME" },
                Systems = new() { "M Drive", "Active M Differential", "iDrive 7" }
            },
            new Vehicle
            {
                Id = "bmw_x5_m50d_2019",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "X5 M50d",
                Year = 2019,
                Engine = "3.0L Quad-Turbo I6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "xDrive",
                FuelType = "Diesel",
                Power = "400 HP",
                Torque = "560 lb-ft",
                EcuTypes = new() { "Bosch MD1", "BMW DME" },
                Systems = new() { "DPF", "SCR", "EGR", "xDrive", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "bmw_335d_2018",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "335d xDrive",
                Year = 2018,
                Engine = "3.0L Twin-Turbo I6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "xDrive",
                FuelType = "Diesel",
                Power = "313 HP",
                Torque = "465 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "BMW DME" },
                Systems = new() { "DPF", "SCR", "EGR", "xDrive" }
            },
            new Vehicle
            {
                Id = "bmw_x3_30d_2020",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "X3 30d xDrive",
                Year = 2020,
                Engine = "3.0L Twin-Turbo I6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "xDrive",
                FuelType = "Diesel",
                Power = "286 HP",
                Torque = "457 lb-ft",
                EcuTypes = new() { "Bosch MD1", "BMW DME" },
                Systems = new() { "DPF", "SCR", "EGR", "xDrive", "Adaptive M Suspension" }
            },
            new Vehicle
            {
                Id = "bmw_530d_2019",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "530d xDrive",
                Year = 2019,
                Engine = "3.0L Twin-Turbo I6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "xDrive",
                FuelType = "Diesel",
                Power = "286 HP",
                Torque = "457 lb-ft",
                EcuTypes = new() { "Bosch MD1", "BMW DME" },
                Systems = new() { "DPF", "SCR", "EGR", "xDrive", "Adaptive Drive" }
            },
            new Vehicle
            {
                Id = "bmw_x5_40d_2022",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "X5 40d xDrive",
                Year = 2022,
                Engine = "3.0L Twin-Turbo I6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "xDrive",
                FuelType = "Diesel",
                Power = "340 HP",
                Torque = "516 lb-ft",
                EcuTypes = new() { "Bosch MD1", "BMW DME" },
                Systems = new() { "DPF", "SCR", "EGR", "xDrive", "Mild Hybrid 48V" }
            },
            new Vehicle
            {
                Id = "bmw_740d_2018",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "740d xDrive",
                Year = 2018,
                Engine = "3.0L Quad-Turbo I6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "xDrive",
                FuelType = "Diesel",
                Power = "320 HP",
                Torque = "501 lb-ft",
                EcuTypes = new() { "Bosch MD1", "BMW DME" },
                Systems = new() { "DPF", "SCR", "EGR", "xDrive", "Integral Active Steering" }
            },
            
            // === MERCEDES-BENZ ===
            new Vehicle
            {
                Id = "mercedes_c63_amg_2020",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "C63 AMG S",
                Year = 2020,
                Engine = "4.0L Twin-Turbo V8",
                Transmission = "9-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "503 HP",
                Torque = "516 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Mercedes ECU" },
                Systems = new() { "AMG Drive", "Race Mode", "AMG Dynamics" }
            },
            new Vehicle
            {
                Id = "mercedes_gle_350d_2021",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "GLE 350d 4MATIC",
                Year = 2021,
                Engine = "2.9L I6 Turbo Diesel",
                Transmission = "9-Speed Automatic",
                DriveType = "4MATIC",
                FuelType = "Diesel",
                Power = "272 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Mercedes ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "Air Suspension", "4MATIC" }
            },
            new Vehicle
            {
                Id = "mercedes_e350d_2019",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "E 350d 4MATIC",
                Year = 2019,
                Engine = "3.0L Twin-Turbo I6 Diesel",
                Transmission = "9-Speed Automatic",
                DriveType = "4MATIC",
                FuelType = "Diesel",
                Power = "286 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Mercedes ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "4MATIC", "Air Body Control" }
            },
            new Vehicle
            {
                Id = "mercedes_gls350d_2021",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "GLS 350d 4MATIC",
                Year = 2021,
                Engine = "3.0L Twin-Turbo I6 Diesel",
                Transmission = "9-Speed Automatic",
                DriveType = "4MATIC",
                FuelType = "Diesel",
                Power = "286 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Mercedes ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "4MATIC", "E-Active Body Control", "Mild Hybrid 48V" }
            },
            new Vehicle
            {
                Id = "mercedes_s350d_2018",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "S 350d 4MATIC",
                Year = 2018,
                Engine = "3.0L Twin-Turbo V6 Diesel",
                Transmission = "9-Speed Automatic",
                DriveType = "4MATIC",
                FuelType = "Diesel",
                Power = "286 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Mercedes ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "4MATIC", "Magic Body Control" }
            },
            new Vehicle
            {
                Id = "mercedes_gle400d_2020",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "GLE 400d 4MATIC",
                Year = 2020,
                Engine = "3.0L Twin-Turbo I6 Diesel",
                Transmission = "9-Speed Automatic",
                DriveType = "4MATIC",
                FuelType = "Diesel",
                Power = "330 HP",
                Torque = "516 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Mercedes ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "4MATIC", "Air Suspension", "Mild Hybrid 48V" }
            },
            new Vehicle
            {
                Id = "mercedes_sprinter_2500_2020",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "Sprinter 2500 Diesel",
                Year = 2020,
                Engine = "3.0L V6 Diesel",
                Transmission = "7-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Diesel",
                Power = "188 HP",
                Torque = "325 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Mercedes ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "Crosswind Assist" }
            },
            
            // === AUDI ===
            new Vehicle
            {
                Id = "audi_rs6_2021",
                Make = "Audi",
                Manufacturer = "Volkswagen Group",
                Model = "RS6 Avant",
                Year = 2021,
                Engine = "4.0L Twin-Turbo V8 TFSI",
                Transmission = "8-Speed Tiptronic",
                DriveType = "quattro",
                FuelType = "Gasoline",
                Power = "591 HP",
                Torque = "590 lb-ft",
                EcuTypes = new() { "Bosch MED17", "Audi ECU" },
                Systems = new() { "quattro", "Air Suspension", "Sport Differential" }
            },
            new Vehicle
            {
                Id = "audi_q7_3.0tdi_2020",
                Make = "Audi",
                Manufacturer = "Volkswagen Group",
                Model = "Q7 50 TDI quattro",
                Year = 2020,
                Engine = "3.0L V6 TDI",
                Transmission = "8-Speed Tiptronic",
                DriveType = "quattro",
                FuelType = "Diesel",
                Power = "286 HP",
                Torque = "442 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Audi ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "quattro", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "audi_a6_55tdi_2020",
                Make = "Audi",
                Manufacturer = "Volkswagen Group",
                Model = "A6 55 TDI quattro",
                Year = 2020,
                Engine = "3.0L V6 TDI",
                Transmission = "8-Speed Tiptronic",
                DriveType = "quattro",
                FuelType = "Diesel",
                Power = "349 HP",
                Torque = "516 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Audi ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "quattro", "Mild Hybrid 48V" }
            },
            new Vehicle
            {
                Id = "audi_q5_40tdi_2021",
                Make = "Audi",
                Manufacturer = "Volkswagen Group",
                Model = "Q5 40 TDI quattro",
                Year = 2021,
                Engine = "2.0L TDI I4",
                Transmission = "7-Speed S tronic",
                DriveType = "quattro",
                FuelType = "Diesel",
                Power = "204 HP",
                Torque = "295 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Audi ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "quattro", "Mild Hybrid 12V" }
            },
            new Vehicle
            {
                Id = "audi_sq7_tdi_2020",
                Make = "Audi",
                Manufacturer = "Volkswagen Group",
                Model = "SQ7 TDI",
                Year = 2020,
                Engine = "4.0L V8 TDI",
                Transmission = "8-Speed Tiptronic",
                DriveType = "quattro",
                FuelType = "Diesel",
                Power = "435 HP",
                Torque = "664 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Audi ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "quattro", "Electric Compressor", "Mild Hybrid 48V" }
            },
            new Vehicle
            {
                Id = "audi_a4_40tdi_2019",
                Make = "Audi",
                Manufacturer = "Volkswagen Group",
                Model = "A4 40 TDI quattro",
                Year = 2019,
                Engine = "2.0L TDI I4",
                Transmission = "7-Speed S tronic",
                DriveType = "quattro",
                FuelType = "Diesel",
                Power = "190 HP",
                Torque = "295 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Audi ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "quattro" }
            },
            
            // === VOLKSWAGEN ===
            new Vehicle
            {
                Id = "vw_golf_r_2021",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Golf R",
                Year = 2021,
                Engine = "2.0L TSI I4",
                Transmission = "7-Speed DSG",
                DriveType = "4MOTION",
                FuelType = "Gasoline",
                Power = "315 HP",
                Torque = "310 lb-ft",
                EcuTypes = new() { "Bosch MED17", "VW ECU" },
                Systems = new() { "4MOTION", "Drift Mode", "Launch Control" }
            },
            new Vehicle
            {
                Id = "vw_tiguan_tdi_2019",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Tiguan 2.0 TDI",
                Year = 2019,
                Engine = "2.0L TDI I4",
                Transmission = "7-Speed DSG",
                DriveType = "4MOTION",
                FuelType = "Diesel",
                Power = "150 HP",
                Torque = "251 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "VW ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "4MOTION" }
            },
            new Vehicle
            {
                Id = "vw_touareg_tdi_2015",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Touareg V6 TDI",
                Year = 2015,
                Engine = "3.0L V6 TDI",
                Transmission = "8-Speed Automatic",
                DriveType = "4MOTION",
                FuelType = "Diesel",
                Power = "240 HP",
                Torque = "406 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "VW ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "4MOTION", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "vw_passat_tdi_2015",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Passat TDI SEL",
                Year = 2015,
                Engine = "2.0L TDI I4",
                Transmission = "6-Speed DSG",
                DriveType = "FWD",
                FuelType = "Diesel",
                Power = "150 HP",
                Torque = "236 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "VW ECU" },
                Systems = new() { "DPF", "SCR", "EGR" }
            },
            new Vehicle
            {
                Id = "vw_jetta_tdi_2014",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Jetta TDI",
                Year = 2014,
                Engine = "2.0L TDI I4",
                Transmission = "6-Speed DSG",
                DriveType = "FWD",
                FuelType = "Diesel",
                Power = "140 HP",
                Torque = "236 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "VW ECU" },
                Systems = new() { "DPF", "EGR" }
            },
            new Vehicle
            {
                Id = "vw_golf_tdi_2015",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Golf TDI",
                Year = 2015,
                Engine = "2.0L TDI I4",
                Transmission = "6-Speed DSG",
                DriveType = "FWD",
                FuelType = "Diesel",
                Power = "150 HP",
                Torque = "236 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "VW ECU" },
                Systems = new() { "DPF", "SCR", "EGR" }
            },
            new Vehicle
            {
                Id = "vw_amarok_v6_tdi_2020",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Amarok V6 TDI",
                Year = 2020,
                Engine = "3.0L V6 TDI",
                Transmission = "8-Speed Automatic",
                DriveType = "4MOTION",
                FuelType = "Diesel",
                Power = "258 HP",
                Torque = "428 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "VW ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "4MOTION" }
            },
            
            // === TOYOTA ===
            new Vehicle
            {
                Id = "toyota_supra_2020",
                Make = "Toyota",
                Manufacturer = "Toyota",
                Model = "GR Supra",
                Year = 2020,
                Engine = "3.0L Twin-Turbo I6 B58",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "382 HP",
                Torque = "368 lb-ft",
                EcuTypes = new() { "Bosch MG1", "Toyota ECM" },
                Systems = new() { "Active Differential", "Launch Control", "Sport Mode" }
            },
            new Vehicle
            {
                Id = "toyota_tacoma_2021",
                Make = "Toyota",
                Manufacturer = "Toyota",
                Model = "Tacoma TRD Pro",
                Year = 2021,
                Engine = "3.5L V6",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "278 HP",
                Torque = "265 lb-ft",
                EcuTypes = new() { "Denso ECU", "Toyota ECM" },
                Systems = new() { "Crawl Control", "4WD", "Multi-Terrain Select" }
            },
            new Vehicle
            {
                Id = "toyota_land_cruiser_2020",
                Make = "Toyota",
                Manufacturer = "Toyota",
                Model = "Land Cruiser",
                Year = 2020,
                Engine = "5.7L V8",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "381 HP",
                Torque = "401 lb-ft",
                EcuTypes = new() { "Denso ECU", "Toyota ECM" },
                Systems = new() { "KDSS", "Crawl Control", "Multi-Terrain Select", "4WD" }
            },
            
            // === HONDA ===
            new Vehicle
            {
                Id = "honda_civic_type_r_2021",
                Make = "Honda",
                Manufacturer = "Honda",
                Model = "Civic Type R",
                Year = 2021,
                Engine = "2.0L Turbo VTEC I4",
                Transmission = "6-Speed Manual",
                DriveType = "FWD",
                FuelType = "Gasoline",
                Power = "306 HP",
                Torque = "295 lb-ft",
                EcuTypes = new() { "Keihin ECU", "Honda PCM" },
                Systems = new() { "Adaptive Dampers", "+R Mode", "Rev Match" }
            },
            
            // === SUBARU ===
            new Vehicle
            {
                Id = "subaru_wrx_sti_2020",
                Make = "Subaru",
                Manufacturer = "Subaru",
                Model = "WRX STI",
                Year = 2020,
                Engine = "2.5L Turbo Boxer H4",
                Transmission = "6-Speed Manual",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "310 HP",
                Torque = "290 lb-ft",
                EcuTypes = new() { "Denso ECU", "Subaru ECM" },
                Systems = new() { "DCCD AWD", "SI-Drive", "Multi-Mode VDC" }
            },
            
            // === NISSAN ===
            new Vehicle
            {
                Id = "nissan_gtr_2020",
                Make = "Nissan",
                Manufacturer = "Nissan",
                Model = "GT-R NISMO",
                Year = 2020,
                Engine = "3.8L Twin-Turbo V6 VR38DETT",
                Transmission = "6-Speed DCT",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "600 HP",
                Torque = "481 lb-ft",
                EcuTypes = new() { "Hitachi ECU", "Nissan ECM" },
                Systems = new() { "ATTESA E-TS AWD", "Launch Control", "R-Mode" }
            },
            new Vehicle
            {
                Id = "nissan_titan_xd_2020",
                Make = "Nissan",
                Manufacturer = "Nissan",
                Model = "Titan XD",
                Year = 2020,
                Engine = "5.0L Cummins V8 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "310 HP",
                Torque = "555 lb-ft",
                EcuTypes = new() { "Cummins ECU", "Nissan ECM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD" }
            },
            
            // === JEEP ===
            new Vehicle
            {
                Id = "jeep_wrangler_2021",
                Make = "Jeep",
                Manufacturer = "Stellantis",
                Model = "Wrangler Rubicon 392",
                Year = 2021,
                Engine = "6.4L HEMI V8",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "470 HP",
                Torque = "470 lb-ft",
                EcuTypes = new() { "Continental ECU", "Chrysler PCM" },
                Systems = new() { "Rock-Trac 4WD", "Selec-Trac", "Sway Bar Disconnect" }
            },
            new Vehicle
            {
                Id = "jeep_grand_cherokee_ecodiesel_2020",
                Make = "Jeep",
                Manufacturer = "Stellantis",
                Model = "Grand Cherokee EcoDiesel",
                Year = 2020,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "240 HP",
                Torque = "420 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Quadra-Drive II", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "jeep_wrangler_ecodiesel_2021",
                Make = "Jeep",
                Manufacturer = "Stellantis",
                Model = "Wrangler EcoDiesel",
                Year = 2021,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "260 HP",
                Torque = "442 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Rock-Trac 4WD", "Selec-Trac" }
            },
            new Vehicle
            {
                Id = "jeep_gladiator_ecodiesel_2021",
                Make = "Jeep",
                Manufacturer = "Stellantis",
                Model = "Gladiator EcoDiesel",
                Year = 2021,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "260 HP",
                Torque = "442 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Rock-Trac 4WD", "Trailer Tow" }
            },
            new Vehicle
            {
                Id = "jeep_grand_cherokee_ecodiesel_2016",
                Make = "Jeep",
                Manufacturer = "Chrysler",
                Model = "Grand Cherokee EcoDiesel",
                Year = 2016,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "240 HP",
                Torque = "420 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DPF", "EGR", "Quadra-Lift", "Selec-Terrain" }
            },
            new Vehicle
            {
                Id = "jeep_cherokee_diesel_2014",
                Make = "Jeep",
                Manufacturer = "Chrysler",
                Model = "Cherokee Diesel (EU)",
                Year = 2014,
                Engine = "2.0L MultiJet II Diesel",
                Transmission = "9-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "170 HP",
                Torque = "258 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DPF", "EGR", "Active Drive II" }
            },
            
            // === GMC ===
            new Vehicle
            {
                Id = "gmc_sierra_denali_2021",
                Make = "GMC",
                Manufacturer = "General Motors",
                Model = "Sierra 2500HD Denali",
                Year = 2021,
                Engine = "6.6L Duramax V8 Diesel",
                Transmission = "10-Speed Allison",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "445 HP",
                Torque = "910 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "ProGrade Trailering", "MultiPro Tailgate" }
            },
            
            // === PORSCHE ===
            new Vehicle
            {
                Id = "porsche_911_turbo_2021",
                Make = "Porsche",
                Manufacturer = "Volkswagen Group",
                Model = "911 Turbo S (992)",
                Year = 2021,
                Engine = "3.8L Twin-Turbo H6",
                Transmission = "8-Speed PDK",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "640 HP",
                Torque = "590 lb-ft",
                EcuTypes = new() { "Bosch MED17", "Porsche DME" },
                Systems = new() { "PASM", "Sport Chrono", "Launch Control", "Rear-Axle Steering" }
            },
            new Vehicle
            {
                Id = "porsche_cayenne_diesel_2018",
                Make = "Porsche",
                Manufacturer = "Volkswagen Group",
                Model = "Cayenne S Diesel",
                Year = 2018,
                Engine = "4.2L V8 TDI",
                Transmission = "8-Speed Tiptronic",
                DriveType = "AWD",
                FuelType = "Diesel",
                Power = "385 HP",
                Torque = "627 lb-ft",
                EcuTypes = new() { "Bosch MD1", "Porsche DME" },
                Systems = new() { "DPF", "SCR", "EGR", "PASM", "Air Suspension" }
            },
            
            // === FORD (Additional) ===
            new Vehicle
            {
                Id = "ford_bronco_2021",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Bronco Wildtrak",
                Year = 2021,
                Engine = "2.7L EcoBoost V6",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "330 HP",
                Torque = "415 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Ford PCM" },
                Systems = new() { "G.O.A.T. Modes", "Trail Turn Assist", "4WD" }
            },
            new Vehicle
            {
                Id = "ford_raptor_2021",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-150 Raptor",
                Year = 2021,
                Engine = "3.5L EcoBoost V6 High-Output",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "450 HP",
                Torque = "510 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Ford PCM" },
                Systems = new() { "FOX Live Valve", "Trail Control", "Terrain Modes" }
            },
            new Vehicle
            {
                Id = "ford_expedition_2022",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Expedition Limited",
                Year = 2022,
                Engine = "3.5L EcoBoost V6",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "400 HP",
                Torque = "480 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Ford PCM" },
                Systems = new() { "SYNC 4", "4WD", "Adaptive Cruise" }
            },
            new Vehicle
            {
                Id = "ford_gt_2020",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "GT",
                Year = 2020,
                Engine = "3.5L EcoBoost V6",
                Transmission = "7-Speed DCT",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "660 HP",
                Torque = "550 lb-ft",
                EcuTypes = new() { "Bosch MEDC17", "Ford PCM" },
                Systems = new() { "Active Suspension", "Track Mode", "Carbon Ceramic Brakes" }
            },
            new Vehicle
            {
                Id = "ford_maverick_2022",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Maverick Hybrid",
                Year = 2022,
                Engine = "2.5L Hybrid I4",
                Transmission = "CVT",
                DriveType = "FWD",
                FuelType = "Hybrid",
                Power = "191 HP",
                Torque = "155 lb-ft",
                EcuTypes = new() { "Ford PCM", "Hybrid Control Module" },
                Systems = new() { "Hybrid System", "SYNC 3", "FordPass Connect" }
            },
            
            // === CHEVROLET (Additional) ===
            new Vehicle
            {
                Id = "chevrolet_tahoe_2022",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Tahoe Z71",
                Year = 2022,
                Engine = "5.3L V8 L84",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "355 HP",
                Torque = "383 lb-ft",
                EcuTypes = new() { "GM E80 ECU", "Bosch" },
                Systems = new() { "Magnetic Ride", "Air Suspension", "4WD" }
            },
            new Vehicle
            {
                Id = "chevrolet_suburban_duramax_2021",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Suburban Diesel",
                Year = 2021,
                Engine = "3.0L Duramax I6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "277 HP",
                Torque = "460 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECM" },
                Systems = new() { "DPF", "SCR", "EGR", "Magnetic Ride", "4WD" }
            },
            new Vehicle
            {
                Id = "chevrolet_bolt_ev_2022",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Bolt EV",
                Year = 2022,
                Engine = "Electric Motor",
                Transmission = "Single-Speed",
                DriveType = "FWD",
                FuelType = "Electric",
                Power = "200 HP",
                Torque = "266 lb-ft",
                EcuTypes = new() { "GM EV Control Module", "LG Battery Management" },
                Systems = new() { "Regen Braking", "One Pedal Driving", "Battery Thermal Mgmt" }
            },
            
            // === TESLA ===
            new Vehicle
            {
                Id = "tesla_model_s_2022",
                Make = "Tesla",
                Manufacturer = "Tesla",
                Model = "Model S Plaid",
                Year = 2022,
                Engine = "Tri Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "1020 HP",
                Torque = "1050 lb-ft",
                EcuTypes = new() { "Tesla MCU3", "Tesla BMS" },
                Systems = new() { "Autopilot", "Ludicrous Mode", "Track Mode" }
            },
            new Vehicle
            {
                Id = "tesla_model_3_2022",
                Make = "Tesla",
                Manufacturer = "Tesla",
                Model = "Model 3 Performance",
                Year = 2022,
                Engine = "Dual Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "450 HP",
                Torque = "471 lb-ft",
                EcuTypes = new() { "Tesla MCU3", "Tesla BMS" },
                Systems = new() { "Autopilot", "Track Mode", "Regen Braking" }
            },
            new Vehicle
            {
                Id = "tesla_model_x_2022",
                Make = "Tesla",
                Manufacturer = "Tesla",
                Model = "Model X Long Range",
                Year = 2022,
                Engine = "Dual Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "670 HP",
                Torque = "700 lb-ft",
                EcuTypes = new() { "Tesla MCU3", "Tesla BMS" },
                Systems = new() { "Autopilot", "Falcon Wing Doors", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "tesla_cybertruck_2024",
                Make = "Tesla",
                Manufacturer = "Tesla",
                Model = "Cybertruck Tri Motor",
                Year = 2024,
                Engine = "Tri Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "845 HP",
                Torque = "Over 10000 lb-ft",
                EcuTypes = new() { "Tesla MCU4", "Tesla BMS" },
                Systems = new() { "Steer-by-Wire", "Adaptive Air Suspension", "Vault Tonneau" }
            },
            
            // === RIVIAN ===
            new Vehicle
            {
                Id = "rivian_r1t_2022",
                Make = "Rivian",
                Manufacturer = "Rivian",
                Model = "R1T Adventure",
                Year = 2022,
                Engine = "Quad Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "835 HP",
                Torque = "908 lb-ft",
                EcuTypes = new() { "Rivian VCU", "Rivian BMS" },
                Systems = new() { "Tank Turn", "Adaptive Air Suspension", "Driver+" }
            },
            new Vehicle
            {
                Id = "rivian_r1s_2022",
                Make = "Rivian",
                Manufacturer = "Rivian",
                Model = "R1S",
                Year = 2022,
                Engine = "Quad Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "835 HP",
                Torque = "908 lb-ft",
                EcuTypes = new() { "Rivian VCU", "Rivian BMS" },
                Systems = new() { "Tank Turn", "Adaptive Air Suspension", "Driver+" }
            },
            
            // === LUCID ===
            new Vehicle
            {
                Id = "lucid_air_2022",
                Make = "Lucid",
                Manufacturer = "Lucid Motors",
                Model = "Air Dream Edition",
                Year = 2022,
                Engine = "Dual Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "1111 HP",
                Torque = "1025 lb-ft",
                EcuTypes = new() { "Lucid ADAS", "Lucid BMS" },
                Systems = new() { "DreamDrive Pro", "Glass Canopy", "Air Suspension" }
            },
            
            // === LAMBORGHINI ===
            new Vehicle
            {
                Id = "lamborghini_huracan_2021",
                Make = "Lamborghini",
                Manufacturer = "Volkswagen Group",
                Model = "Huracán EVO",
                Year = 2021,
                Engine = "5.2L V10",
                Transmission = "7-Speed DCT",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "631 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch MED17", "Lamborghini ECU" },
                Systems = new() { "LDVI", "ALA 2.0", "Magneto-Rheological Suspension" }
            },
            new Vehicle
            {
                Id = "lamborghini_urus_2022",
                Make = "Lamborghini",
                Manufacturer = "Volkswagen Group",
                Model = "Urus",
                Year = 2022,
                Engine = "4.0L Twin-Turbo V8",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "641 HP",
                Torque = "627 lb-ft",
                EcuTypes = new() { "Bosch MED17", "Lamborghini ECU" },
                Systems = new() { "ANIMA Modes", "Torque Vectoring", "Air Suspension" }
            },
            
            // === FERRARI ===
            new Vehicle
            {
                Id = "ferrari_488_gtb_2019",
                Make = "Ferrari",
                Manufacturer = "Ferrari",
                Model = "488 GTB",
                Year = 2019,
                Engine = "3.9L Twin-Turbo V8",
                Transmission = "7-Speed DCT",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "661 HP",
                Torque = "561 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Ferrari ECU" },
                Systems = new() { "SSC2", "E-Diff3", "Manettino" }
            },
            new Vehicle
            {
                Id = "ferrari_sf90_2021",
                Make = "Ferrari",
                Manufacturer = "Ferrari",
                Model = "SF90 Stradale",
                Year = 2021,
                Engine = "4.0L Twin-Turbo V8 + 3 Electric Motors",
                Transmission = "8-Speed DCT",
                DriveType = "AWD",
                FuelType = "PHEV",
                Power = "986 HP",
                Torque = "590 lb-ft",
                EcuTypes = new() { "Ferrari Hybrid ECU", "Ferrari BMS" },
                Systems = new() { "eManettino", "Assetto Fiorano", "RAC-e" }
            },
            
            // === MCLAREN ===
            new Vehicle
            {
                Id = "mclaren_720s_2020",
                Make = "McLaren",
                Manufacturer = "McLaren",
                Model = "720S",
                Year = 2020,
                Engine = "4.0L Twin-Turbo V8",
                Transmission = "7-Speed DCT",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "710 HP",
                Torque = "568 lb-ft",
                EcuTypes = new() { "Bosch ME17", "McLaren ECU" },
                Systems = new() { "Proactive Chassis Control II", "Variable Drift Control" }
            },
            new Vehicle
            {
                Id = "mclaren_765lt_2021",
                Make = "McLaren",
                Manufacturer = "McLaren",
                Model = "765LT",
                Year = 2021,
                Engine = "4.0L Twin-Turbo V8",
                Transmission = "7-Speed DCT",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "755 HP",
                Torque = "590 lb-ft",
                EcuTypes = new() { "Bosch ME17", "McLaren ECU" },
                Systems = new() { "PCC II", "Active Rear Wing", "Carbon Ceramic Brakes" }
            },
            
            // === JAGUAR ===
            new Vehicle
            {
                Id = "jaguar_f_type_r_2021",
                Make = "Jaguar",
                Manufacturer = "Jaguar Land Rover",
                Model = "F-TYPE R",
                Year = 2021,
                Engine = "5.0L Supercharged V8",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "575 HP",
                Torque = "516 lb-ft",
                EcuTypes = new() { "Bosch ME17", "JLR ECU" },
                Systems = new() { "Active Sports Exhaust", "Switchable Dynamics" }
            },
            new Vehicle
            {
                Id = "jaguar_i_pace_2022",
                Make = "Jaguar",
                Manufacturer = "Jaguar Land Rover",
                Model = "I-PACE EV400",
                Year = 2022,
                Engine = "Dual Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "394 HP",
                Torque = "512 lb-ft",
                EcuTypes = new() { "JLR EV Control", "JLR BMS" },
                Systems = new() { "Adaptive Dynamics", "Regen Braking", "Pivi Pro" }
            },
            
            // === LAND ROVER ===
            new Vehicle
            {
                Id = "land_rover_defender_2022",
                Make = "Land Rover",
                Manufacturer = "Jaguar Land Rover",
                Model = "Defender 110 V8",
                Year = 2022,
                Engine = "5.0L Supercharged V8",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "518 HP",
                Torque = "461 lb-ft",
                EcuTypes = new() { "Bosch ME17", "JLR ECU" },
                Systems = new() { "Terrain Response 2", "ClearSight Mirror", "Wade Sensing" }
            },
            new Vehicle
            {
                Id = "land_rover_range_rover_2022",
                Make = "Land Rover",
                Manufacturer = "Jaguar Land Rover",
                Model = "Range Rover P530",
                Year = 2022,
                Engine = "4.4L Twin-Turbo V8",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "523 HP",
                Torque = "553 lb-ft",
                EcuTypes = new() { "Bosch MG1", "JLR ECU" },
                Systems = new() { "Terrain Response 2", "Air Suspension", "Rear-Wheel Steering" }
            },
            new Vehicle
            {
                Id = "land_rover_discovery_td6_2017",
                Make = "Land Rover",
                Manufacturer = "Jaguar Land Rover",
                Model = "Discovery TD6",
                Year = 2017,
                Engine = "3.0L V6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "254 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "JLR ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "Terrain Response", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "land_rover_range_rover_sport_sdv6_2019",
                Make = "Land Rover",
                Manufacturer = "Jaguar Land Rover",
                Model = "Range Rover Sport SDV6",
                Year = 2019,
                Engine = "3.0L V6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "306 HP",
                Torque = "516 lb-ft",
                EcuTypes = new() { "Bosch MD1", "JLR ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "Terrain Response 2", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "land_rover_defender_d300_2022",
                Make = "Land Rover",
                Manufacturer = "Jaguar Land Rover",
                Model = "Defender 110 D300",
                Year = 2022,
                Engine = "3.0L I6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "296 HP",
                Torque = "479 lb-ft",
                EcuTypes = new() { "Bosch MD1", "JLR ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "Terrain Response 2", "Mild Hybrid 48V" }
            },
            new Vehicle
            {
                Id = "land_rover_range_rover_tdv6_2016",
                Make = "Land Rover",
                Manufacturer = "Jaguar Land Rover",
                Model = "Range Rover TDV6",
                Year = 2016,
                Engine = "3.0L V6 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "258 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "JLR ECU" },
                Systems = new() { "DPF", "SCR", "EGR", "Terrain Response", "Air Suspension" }
            },
            
            // === VOLVO ===
            new Vehicle
            {
                Id = "volvo_xc90_t8_2022",
                Make = "Volvo",
                Manufacturer = "Volvo",
                Model = "XC90 T8 Recharge",
                Year = 2022,
                Engine = "2.0L Turbo + Supercharged + Electric",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "PHEV",
                Power = "455 HP",
                Torque = "523 lb-ft",
                EcuTypes = new() { "Denso ECU", "Volvo PHEV Module" },
                Systems = new() { "Pilot Assist", "Air Suspension", "PHEV System" }
            },
            new Vehicle
            {
                Id = "volvo_s60_polestar_2020",
                Make = "Volvo",
                Manufacturer = "Volvo",
                Model = "S60 Polestar Engineered",
                Year = 2020,
                Engine = "2.0L Turbo + Supercharged + Electric",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "PHEV",
                Power = "415 HP",
                Torque = "494 lb-ft",
                EcuTypes = new() { "Denso ECU", "Volvo PHEV Module" },
                Systems = new() { "Öhlins DFV", "Brembo Brakes", "PHEV System" }
            },
            
            // === MAZDA ===
            new Vehicle
            {
                Id = "mazda_mx5_2022",
                Make = "Mazda",
                Manufacturer = "Mazda",
                Model = "MX-5 Miata",
                Year = 2022,
                Engine = "2.0L I4 SkyActiv-G",
                Transmission = "6-Speed Manual",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "181 HP",
                Torque = "151 lb-ft",
                EcuTypes = new() { "Denso ECU", "Mazda ECM" },
                Systems = new() { "i-ELOOP", "Bilstein Dampers", "LSD" }
            },
            new Vehicle
            {
                Id = "mazda_3_turbo_2022",
                Make = "Mazda",
                Manufacturer = "Mazda",
                Model = "Mazda3 Turbo",
                Year = 2022,
                Engine = "2.5L Turbo I4 SkyActiv-G",
                Transmission = "6-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "250 HP",
                Torque = "320 lb-ft",
                EcuTypes = new() { "Denso ECU", "Mazda ECM" },
                Systems = new() { "i-Activ AWD", "G-Vectoring Control Plus" }
            },
            new Vehicle
            {
                Id = "mazda_cx50_2023",
                Make = "Mazda",
                Manufacturer = "Mazda",
                Model = "CX-50 Turbo",
                Year = 2023,
                Engine = "2.5L Turbo I4 SkyActiv-G",
                Transmission = "6-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "256 HP",
                Torque = "320 lb-ft",
                EcuTypes = new() { "Denso ECU", "Mazda ECM" },
                Systems = new() { "i-Activ AWD", "Mi-Drive", "Off-Road Mode" }
            },
            new Vehicle
            {
                Id = "mazda_cx5_diesel_2019",
                Make = "Mazda",
                Manufacturer = "Mazda",
                Model = "CX-5 SkyActiv-D",
                Year = 2019,
                Engine = "2.2L SkyActiv-D I4 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Diesel",
                Power = "173 HP",
                Torque = "310 lb-ft",
                EcuTypes = new() { "Denso ECU", "Mazda ECM" },
                Systems = new() { "DPF", "SCR", "EGR", "i-Activ AWD" }
            },
            new Vehicle
            {
                Id = "mazda_6_diesel_2018",
                Make = "Mazda",
                Manufacturer = "Mazda",
                Model = "Mazda6 SkyActiv-D",
                Year = 2018,
                Engine = "2.2L SkyActiv-D I4 Diesel",
                Transmission = "6-Speed Manual",
                DriveType = "FWD",
                FuelType = "Diesel",
                Power = "173 HP",
                Torque = "310 lb-ft",
                EcuTypes = new() { "Denso ECU", "Mazda ECM" },
                Systems = new() { "DPF", "SCR", "EGR", "i-ELOOP" }
            },
            
            // === HYUNDAI ===
            new Vehicle
            {
                Id = "hyundai_elantra_n_2022",
                Make = "Hyundai",
                Manufacturer = "Hyundai",
                Model = "Elantra N",
                Year = 2022,
                Engine = "2.0L Turbo I4",
                Transmission = "8-Speed DCT",
                DriveType = "FWD",
                FuelType = "Gasoline",
                Power = "276 HP",
                Torque = "289 lb-ft",
                EcuTypes = new() { "Kefico ECU", "Hyundai ECM" },
                Systems = new() { "N Grin Shift", "Rev Matching", "eLSD" }
            },
            new Vehicle
            {
                Id = "hyundai_ioniq5_2022",
                Make = "Hyundai",
                Manufacturer = "Hyundai",
                Model = "IONIQ 5 AWD",
                Year = 2022,
                Engine = "Dual Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "320 HP",
                Torque = "446 lb-ft",
                EcuTypes = new() { "Hyundai E-GMP Module", "Hyundai BMS" },
                Systems = new() { "Vehicle-to-Load", "800V Ultra-Fast Charging", "Regen Braking" }
            },
            new Vehicle
            {
                Id = "hyundai_kona_n_2022",
                Make = "Hyundai",
                Manufacturer = "Hyundai",
                Model = "Kona N",
                Year = 2022,
                Engine = "2.0L Turbo I4",
                Transmission = "8-Speed DCT",
                DriveType = "FWD",
                FuelType = "Gasoline",
                Power = "276 HP",
                Torque = "289 lb-ft",
                EcuTypes = new() { "Kefico ECU", "Hyundai ECM" },
                Systems = new() { "N Grin Shift", "Launch Control", "eLSD" }
            },
            
            // === KIA ===
            new Vehicle
            {
                Id = "kia_stinger_gt_2022",
                Make = "Kia",
                Manufacturer = "Hyundai",
                Model = "Stinger GT",
                Year = 2022,
                Engine = "3.3L Twin-Turbo V6",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "368 HP",
                Torque = "376 lb-ft",
                EcuTypes = new() { "Kefico ECU", "Kia ECM" },
                Systems = new() { "Dynamic AWD", "Variable Exhaust", "Drive Mode Select" }
            },
            new Vehicle
            {
                Id = "kia_ev6_gt_2023",
                Make = "Kia",
                Manufacturer = "Hyundai",
                Model = "EV6 GT",
                Year = 2023,
                Engine = "Dual Motor Electric",
                Transmission = "Single-Speed",
                DriveType = "AWD",
                FuelType = "Electric",
                Power = "576 HP",
                Torque = "546 lb-ft",
                EcuTypes = new() { "Hyundai E-GMP Module", "Kia BMS" },
                Systems = new() { "GT Mode", "Drift Mode", "800V Charging" }
            },
            new Vehicle
            {
                Id = "kia_telluride_2022",
                Make = "Kia",
                Manufacturer = "Hyundai",
                Model = "Telluride SX",
                Year = 2022,
                Engine = "3.8L V6",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "291 HP",
                Torque = "262 lb-ft",
                EcuTypes = new() { "Kefico ECU", "Kia ECM" },
                Systems = new() { "Snow Mode", "Terrain Modes", "Self-Leveling Suspension" }
            },
            
            // === GENESIS ===
            new Vehicle
            {
                Id = "genesis_g70_2022",
                Make = "Genesis",
                Manufacturer = "Hyundai",
                Model = "G70 3.3T",
                Year = 2022,
                Engine = "3.3L Twin-Turbo V6",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "365 HP",
                Torque = "376 lb-ft",
                EcuTypes = new() { "Kefico ECU", "Genesis ECM" },
                Systems = new() { "Launch Control", "Adaptive Suspension", "Drive Mode" }
            },
            new Vehicle
            {
                Id = "genesis_gv80_2022",
                Make = "Genesis",
                Manufacturer = "Hyundai",
                Model = "GV80 3.5T",
                Year = 2022,
                Engine = "3.5L Twin-Turbo V6",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "375 HP",
                Torque = "391 lb-ft",
                EcuTypes = new() { "Kefico ECU", "Genesis ECM" },
                Systems = new() { "Road Preview Suspension", "AWD", "Air Suspension" }
            },
            
            // === LEXUS ===
            new Vehicle
            {
                Id = "lexus_lc500_2022",
                Make = "Lexus",
                Manufacturer = "Toyota",
                Model = "LC 500",
                Year = 2022,
                Engine = "5.0L V8",
                Transmission = "10-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "471 HP",
                Torque = "398 lb-ft",
                EcuTypes = new() { "Denso ECU", "Lexus ECM" },
                Systems = new() { "Active Sport Exhaust", "LDH", "Adaptive Suspension" }
            },
            new Vehicle
            {
                Id = "lexus_is500_2022",
                Make = "Lexus",
                Manufacturer = "Toyota",
                Model = "IS 500 F Sport",
                Year = 2022,
                Engine = "5.0L V8",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "472 HP",
                Torque = "395 lb-ft",
                EcuTypes = new() { "Denso ECU", "Lexus ECM" },
                Systems = new() { "Dynamic Handling", "Yamaha Sound", "LSD" }
            },
            new Vehicle
            {
                Id = "lexus_lx600_2022",
                Make = "Lexus",
                Manufacturer = "Toyota",
                Model = "LX 600",
                Year = 2022,
                Engine = "3.4L Twin-Turbo V6",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "409 HP",
                Torque = "479 lb-ft",
                EcuTypes = new() { "Denso ECU", "Lexus ECM" },
                Systems = new() { "Multi-Terrain Select", "Crawl Control", "E-KDSS" }
            },
            
            // === INFINITI ===
            new Vehicle
            {
                Id = "infiniti_q60_red_sport_2022",
                Make = "Infiniti",
                Manufacturer = "Nissan",
                Model = "Q60 Red Sport 400",
                Year = 2022,
                Engine = "3.0L Twin-Turbo V6",
                Transmission = "7-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "400 HP",
                Torque = "350 lb-ft",
                EcuTypes = new() { "Hitachi ECU", "Infiniti ECM" },
                Systems = new() { "Dynamic Digital Suspension", "AWD", "Drive Mode" }
            },
            
            // === ACURA ===
            new Vehicle
            {
                Id = "acura_type_s_2022",
                Make = "Acura",
                Manufacturer = "Honda",
                Model = "TLX Type S",
                Year = 2022,
                Engine = "3.0L Turbo V6",
                Transmission = "10-Speed Automatic",
                DriveType = "SH-AWD",
                FuelType = "Gasoline",
                Power = "355 HP",
                Torque = "354 lb-ft",
                EcuTypes = new() { "Keihin ECU", "Acura ECM" },
                Systems = new() { "SH-AWD", "Adaptive Dampers", "Brembo Brakes" }
            },
            new Vehicle
            {
                Id = "acura_nsx_2022",
                Make = "Acura",
                Manufacturer = "Honda",
                Model = "NSX Type S",
                Year = 2022,
                Engine = "3.5L Twin-Turbo V6 + 3 Electric Motors",
                Transmission = "9-Speed DCT",
                DriveType = "AWD",
                FuelType = "Hybrid",
                Power = "600 HP",
                Torque = "492 lb-ft",
                EcuTypes = new() { "Honda Hybrid ECU", "Acura Sport Hybrid" },
                Systems = new() { "Sport Hybrid SH-AWD", "Track Mode", "Carbon Ceramic Brakes" }
            },
            
            // === MITSUBISHI ===
            new Vehicle
            {
                Id = "mitsubishi_outlander_phev_2022",
                Make = "Mitsubishi",
                Manufacturer = "Mitsubishi",
                Model = "Outlander PHEV",
                Year = 2022,
                Engine = "2.4L I4 + Dual Motors",
                Transmission = "Single-Speed",
                DriveType = "S-AWC",
                FuelType = "PHEV",
                Power = "248 HP",
                Torque = "332 lb-ft",
                EcuTypes = new() { "Mitsubishi PHEV Module", "Denso ECU" },
                Systems = new() { "S-AWC", "EV Mode", "Regenerative Braking" }
            },
            
            // === ALFA ROMEO ===
            new Vehicle
            {
                Id = "alfa_romeo_giulia_qv_2022",
                Make = "Alfa Romeo",
                Manufacturer = "Stellantis",
                Model = "Giulia Quadrifoglio",
                Year = 2022,
                Engine = "2.9L Twin-Turbo V6",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "505 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Alfa Romeo ECU" },
                Systems = new() { "Race Mode", "Torque Vectoring Differential", "Cylinder Deactivation" }
            },
            new Vehicle
            {
                Id = "alfa_romeo_stelvio_qv_2022",
                Make = "Alfa Romeo",
                Manufacturer = "Stellantis",
                Model = "Stelvio Quadrifoglio",
                Year = 2022,
                Engine = "2.9L Twin-Turbo V6",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "505 HP",
                Torque = "443 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Alfa Romeo ECU" },
                Systems = new() { "Q4 AWD", "Torque Vectoring", "Adaptive Suspension" }
            },
            
            // === MASERATI ===
            new Vehicle
            {
                Id = "maserati_ghibli_trofeo_2022",
                Make = "Maserati",
                Manufacturer = "Stellantis",
                Model = "Ghibli Trofeo",
                Year = 2022,
                Engine = "3.8L Twin-Turbo V8",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "580 HP",
                Torque = "538 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Maserati ECU" },
                Systems = new() { "Launch Control", "Corsa Mode", "Integrated Vehicle Control" }
            },
            new Vehicle
            {
                Id = "maserati_mc20_2022",
                Make = "Maserati",
                Manufacturer = "Stellantis",
                Model = "MC20",
                Year = 2022,
                Engine = "3.0L Twin-Turbo V6 Nettuno",
                Transmission = "8-Speed DCT",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "621 HP",
                Torque = "538 lb-ft",
                EcuTypes = new() { "Maserati ECU", "Bosch MG1" },
                Systems = new() { "Pre-Chamber Combustion", "Launch Control", "Virtual Cockpit" }
            },
            
            // === ASTON MARTIN ===
            new Vehicle
            {
                Id = "aston_martin_dbx_2022",
                Make = "Aston Martin",
                Manufacturer = "Aston Martin",
                Model = "DBX707",
                Year = 2022,
                Engine = "4.0L Twin-Turbo V8",
                Transmission = "9-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "697 HP",
                Torque = "664 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Aston Martin ECU" },
                Systems = new() { "48V Anti-Roll", "Air Suspension", "Sport+ Mode" }
            },
            new Vehicle
            {
                Id = "aston_martin_vantage_2022",
                Make = "Aston Martin",
                Manufacturer = "Aston Martin",
                Model = "Vantage F1 Edition",
                Year = 2022,
                Engine = "4.0L Twin-Turbo V8",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "528 HP",
                Torque = "505 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Aston Martin ECU" },
                Systems = new() { "Track Mode", "Electronic Differential", "Adaptive Damping" }
            },
            
            // === BENTLEY ===
            new Vehicle
            {
                Id = "bentley_continental_gt_2022",
                Make = "Bentley",
                Manufacturer = "Volkswagen Group",
                Model = "Continental GT Speed",
                Year = 2022,
                Engine = "6.0L W12 Twin-Turbo",
                Transmission = "8-Speed DCT",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "650 HP",
                Torque = "664 lb-ft",
                EcuTypes = new() { "Bosch MED17", "Bentley ECU" },
                Systems = new() { "48V Anti-Roll", "Rear-Wheel Steering", "Active Chassis" }
            },
            
            // === ROLLS-ROYCE ===
            new Vehicle
            {
                Id = "rolls_royce_cullinan_2022",
                Make = "Rolls-Royce",
                Manufacturer = "BMW",
                Model = "Cullinan Black Badge",
                Year = 2022,
                Engine = "6.75L Twin-Turbo V12",
                Transmission = "8-Speed Automatic",
                DriveType = "AWD",
                FuelType = "Gasoline",
                Power = "600 HP",
                Torque = "664 lb-ft",
                EcuTypes = new() { "Bosch ME17", "Rolls-Royce ECU" },
                Systems = new() { "Magic Carpet Ride", "Self-Leveling Air", "Night Vision" }
            },
            
            // === CADILLAC ===
            new Vehicle
            {
                Id = "cadillac_ct5_v_blackwing_2022",
                Make = "Cadillac",
                Manufacturer = "General Motors",
                Model = "CT5-V Blackwing",
                Year = 2022,
                Engine = "6.2L Supercharged V8 LT4",
                Transmission = "6-Speed Manual",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "668 HP",
                Torque = "659 lb-ft",
                EcuTypes = new() { "GM E95 ECU", "Bosch" },
                Systems = new() { "Magnetic Ride 4.0", "eLSD", "V-Mode Performance" }
            },
            new Vehicle
            {
                Id = "cadillac_escalade_v_2023",
                Make = "Cadillac",
                Manufacturer = "General Motors",
                Model = "Escalade-V",
                Year = 2023,
                Engine = "6.2L Supercharged V8 LT4",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "682 HP",
                Torque = "653 lb-ft",
                EcuTypes = new() { "GM E95 ECU", "Bosch" },
                Systems = new() { "Magnetic Ride", "Air Suspension", "Super Cruise" }
            },
            
            // === LINCOLN ===
            new Vehicle
            {
                Id = "lincoln_navigator_2022",
                Make = "Lincoln",
                Manufacturer = "Ford",
                Model = "Navigator Reserve",
                Year = 2022,
                Engine = "3.5L EcoBoost V6",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Gasoline",
                Power = "440 HP",
                Torque = "510 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Ford/Lincoln PCM" },
                Systems = new() { "Adaptive Suspension", "Active Glide", "SYNC 4" }
            },
            
            // === DODGE (Additional) ===
            new Vehicle
            {
                Id = "dodge_charger_hellcat_2023",
                Make = "Dodge",
                Manufacturer = "Stellantis",
                Model = "Charger SRT Hellcat Redeye",
                Year = 2023,
                Engine = "6.2L Supercharged HEMI V8",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Gasoline",
                Power = "797 HP",
                Torque = "707 lb-ft",
                EcuTypes = new() { "Continental ECU", "Chrysler PCM" },
                Systems = new() { "Widebody", "Power Chiller", "After-Run Chiller" }
            },
            new Vehicle
            {
                Id = "dodge_demon_170_2023",
                Make = "Dodge",
                Manufacturer = "Stellantis",
                Model = "Challenger SRT Demon 170",
                Year = 2023,
                Engine = "6.2L Supercharged HEMI V8",
                Transmission = "8-Speed Automatic",
                DriveType = "RWD",
                FuelType = "E85",
                Power = "1025 HP",
                Torque = "945 lb-ft",
                EcuTypes = new() { "Continental ECU", "Chrysler PCM" },
                Systems = new() { "TransBrake", "Drag Mode", "Power Chiller" }
            },
            
            // === LIGHT DUTY DIESEL TRUCKS (1997+) ===
            
            // Ford F-150 Diesel
            new Vehicle
            {
                Id = "ford_f150_diesel_2018",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-150 Power Stroke",
                Year = 2018,
                Engine = "3.0L Power Stroke V6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "250 HP",
                Torque = "440 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Ford PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD" }
            },
            new Vehicle
            {
                Id = "ford_f150_diesel_2020",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-150 Power Stroke",
                Year = 2020,
                Engine = "3.0L Power Stroke V6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "250 HP",
                Torque = "440 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Ford PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Pro Trailer Backup", "4WD" }
            },
            new Vehicle
            {
                Id = "ford_f150_diesel_2023",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-150 Power Stroke",
                Year = 2023,
                Engine = "3.0L Power Stroke V6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "250 HP",
                Torque = "440 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Ford PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "Pro Power Onboard", "4WD" }
            },
            
            // Chevrolet Silverado 1500 Duramax
            new Vehicle
            {
                Id = "chevrolet_silverado_1500_diesel_2019",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Silverado 1500 Duramax",
                Year = 2019,
                Engine = "3.0L Duramax I6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "277 HP",
                Torque = "460 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM E98 ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Auto Stop/Start" }
            },
            new Vehicle
            {
                Id = "chevrolet_silverado_1500_diesel_2021",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Silverado 1500 Duramax",
                Year = 2021,
                Engine = "3.0L Duramax I6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "277 HP",
                Torque = "460 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM E98 ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Trailering Package" }
            },
            new Vehicle
            {
                Id = "chevrolet_silverado_1500_diesel_2024",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Silverado 1500 Duramax",
                Year = 2024,
                Engine = "3.0L Duramax I6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "305 HP",
                Torque = "495 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM E98 ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Super Cruise" }
            },
            
            // GMC Sierra 1500 Duramax
            new Vehicle
            {
                Id = "gmc_sierra_1500_diesel_2019",
                Make = "GMC",
                Manufacturer = "General Motors",
                Model = "Sierra 1500 Duramax",
                Year = 2019,
                Engine = "3.0L Duramax I6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "277 HP",
                Torque = "460 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM E98 ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "MultiPro Tailgate" }
            },
            new Vehicle
            {
                Id = "gmc_sierra_1500_diesel_2022",
                Make = "GMC",
                Manufacturer = "General Motors",
                Model = "Sierra 1500 AT4 Duramax",
                Year = 2022,
                Engine = "3.0L Duramax I6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "277 HP",
                Torque = "460 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM E98 ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Off-Road Suspension" }
            },
            new Vehicle
            {
                Id = "gmc_sierra_1500_diesel_2024",
                Make = "GMC",
                Manufacturer = "General Motors",
                Model = "Sierra 1500 Denali Duramax",
                Year = 2024,
                Engine = "3.0L Duramax I6 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "305 HP",
                Torque = "495 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM E98 ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Adaptive Ride Control" }
            },
            
            // RAM 1500 EcoDiesel
            new Vehicle
            {
                Id = "ram_1500_ecodiesel_2014",
                Make = "RAM",
                Manufacturer = "Stellantis",
                Model = "1500 EcoDiesel",
                Year = 2014,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "240 HP",
                Torque = "420 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DPF", "EGR", "4WD", "Air Suspension" }
            },
            new Vehicle
            {
                Id = "ram_1500_ecodiesel_2016",
                Make = "RAM",
                Manufacturer = "Stellantis",
                Model = "1500 EcoDiesel",
                Year = 2016,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "240 HP",
                Torque = "420 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DPF", "EGR", "4WD", "Uconnect" }
            },
            new Vehicle
            {
                Id = "ram_1500_ecodiesel_2020",
                Make = "RAM",
                Manufacturer = "Stellantis",
                Model = "1500 EcoDiesel",
                Year = 2020,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "260 HP",
                Torque = "480 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "eTorque Hybrid" }
            },
            new Vehicle
            {
                Id = "ram_1500_ecodiesel_2023",
                Make = "RAM",
                Manufacturer = "Stellantis",
                Model = "1500 EcoDiesel Rebel",
                Year = 2023,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "260 HP",
                Torque = "480 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Off-Road Package" }
            },
            
            // Chevrolet Colorado Duramax
            new Vehicle
            {
                Id = "chevrolet_colorado_diesel_2016",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Colorado Duramax",
                Year = 2016,
                Engine = "2.8L Duramax I4 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "181 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECU" },
                Systems = new() { "DPF", "EGR", "4WD" }
            },
            new Vehicle
            {
                Id = "chevrolet_colorado_diesel_2019",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Colorado ZR2 Duramax",
                Year = 2019,
                Engine = "2.8L Duramax I4 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "186 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECU" },
                Systems = new() { "DPF", "EGR", "4WD", "Multimatic DSSV Dampers" }
            },
            new Vehicle
            {
                Id = "chevrolet_colorado_diesel_2022",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Colorado Duramax",
                Year = 2022,
                Engine = "2.8L Duramax I4 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "186 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECU" },
                Systems = new() { "DPF", "EGR", "4WD", "Trailering Package" }
            },
            
            // GMC Canyon Duramax
            new Vehicle
            {
                Id = "gmc_canyon_diesel_2016",
                Make = "GMC",
                Manufacturer = "General Motors",
                Model = "Canyon Duramax",
                Year = 2016,
                Engine = "2.8L Duramax I4 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "181 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECU" },
                Systems = new() { "DPF", "EGR", "4WD" }
            },
            new Vehicle
            {
                Id = "gmc_canyon_diesel_2020",
                Make = "GMC",
                Manufacturer = "General Motors",
                Model = "Canyon AT4 Duramax",
                Year = 2020,
                Engine = "2.8L Duramax I4 Diesel",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "186 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "GM ECU" },
                Systems = new() { "DPF", "EGR", "4WD", "Off-Road Suspension" }
            },
            
            // Nissan Titan XD Cummins
            new Vehicle
            {
                Id = "nissan_titan_xd_diesel_2016",
                Make = "Nissan",
                Manufacturer = "Nissan",
                Model = "Titan XD Cummins",
                Year = 2016,
                Engine = "5.0L Cummins V8 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "310 HP",
                Torque = "555 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Nissan ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD" }
            },
            new Vehicle
            {
                Id = "nissan_titan_xd_diesel_2018",
                Make = "Nissan",
                Manufacturer = "Nissan",
                Model = "Titan XD PRO-4X Cummins",
                Year = 2018,
                Engine = "5.0L Cummins V8 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "310 HP",
                Torque = "555 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Nissan ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Off-Road Package" }
            },
            new Vehicle
            {
                Id = "nissan_titan_xd_diesel_2019",
                Make = "Nissan",
                Manufacturer = "Nissan",
                Model = "Titan XD Platinum Reserve Cummins",
                Year = 2019,
                Engine = "5.0L Cummins V8 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "310 HP",
                Torque = "555 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Nissan ECU" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Trailer Brake" }
            },
            
            // Dodge RAM 1500 (Older EcoDiesel)
            new Vehicle
            {
                Id = "dodge_ram_1500_ecodiesel_2015",
                Make = "Dodge",
                Manufacturer = "Chrysler",
                Model = "RAM 1500 EcoDiesel",
                Year = 2015,
                Engine = "3.0L EcoDiesel V6",
                Transmission = "8-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "240 HP",
                Torque = "420 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Chrysler PCM" },
                Systems = new() { "DPF", "EGR", "4WD", "Rambox" }
            },
            
            // International Markets - Ford Ranger Diesel
            new Vehicle
            {
                Id = "ford_ranger_diesel_2019",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Ranger Wildtrak Diesel",
                Year = 2019,
                Engine = "3.2L Power Stroke I5 Diesel",
                Transmission = "6-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "200 HP",
                Torque = "347 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Ford PCM" },
                Systems = new() { "DPF", "EGR", "4WD", "Terrain Management" }
            },
            new Vehicle
            {
                Id = "ford_ranger_diesel_2023",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "Ranger Raptor Diesel",
                Year = 2023,
                Engine = "2.0L Bi-Turbo I4 Diesel",
                Transmission = "10-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "210 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC17", "Ford PCM" },
                Systems = new() { "DEF/SCR", "DPF", "EGR", "4WD", "Fox Shocks" }
            },
            
            // Older Dodge Dakota (Rare Diesel)
            new Vehicle
            {
                Id = "dodge_dakota_diesel_1997",
                Make = "Dodge",
                Manufacturer = "Chrysler",
                Model = "Dakota Diesel",
                Year = 1997,
                Engine = "2.5L VM Motori I4 Diesel",
                Transmission = "5-Speed Manual",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "115 HP",
                Torque = "195 lb-ft",
                EcuTypes = new() { "Bosch EDC" },
                Systems = new() { "4WD" }
            },
            
            // === OLDER DIESEL VEHICLES (1997-2010) ===
            
            // Ford F-250/F-350 Power Stroke (7.3L Era)
            new Vehicle
            {
                Id = "ford_f250_7.3_powerstroke_1999",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-250 Super Duty 7.3L Power Stroke",
                Year = 1999,
                Engine = "7.3L Power Stroke V8 Diesel",
                Transmission = "5-Speed Manual",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "235 HP",
                Torque = "500 lb-ft",
                EcuTypes = new() { "International ECU" },
                Systems = new() { "4WD" }
            },
            new Vehicle
            {
                Id = "ford_f350_7.3_powerstroke_2003",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-350 Super Duty 7.3L Power Stroke",
                Year = 2003,
                Engine = "7.3L Power Stroke V8 Diesel",
                Transmission = "6-Speed Manual",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "250 HP",
                Torque = "525 lb-ft",
                EcuTypes = new() { "International ECU" },
                Systems = new() { "4WD", "Exhaust Brake" }
            },
            
            // Ford 6.0L Power Stroke
            new Vehicle
            {
                Id = "ford_f250_6.0_powerstroke_2005",
                Make = "Ford",
                Manufacturer = "Ford",
                Model = "F-250 Super Duty 6.0L Power Stroke",
                Year = 2005,
                Engine = "6.0L Power Stroke V8 Diesel",
                Transmission = "5-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "325 HP",
                Torque = "560 lb-ft",
                EcuTypes = new() { "International ECU", "Bosch" },
                Systems = new() { "EGR", "4WD", "Exhaust Brake" }
            },
            
            // Dodge RAM Cummins (Early Models)
            new Vehicle
            {
                Id = "dodge_ram_2500_cummins_1998",
                Make = "Dodge",
                Manufacturer = "Chrysler",
                Model = "RAM 2500 5.9L Cummins",
                Year = 1998,
                Engine = "5.9L Cummins I6 Diesel",
                Transmission = "5-Speed Manual",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "235 HP",
                Torque = "460 lb-ft",
                EcuTypes = new() { "Cummins P-Pump" },
                Systems = new() { "4WD" }
            },
            new Vehicle
            {
                Id = "dodge_ram_2500_cummins_2002",
                Make = "Dodge",
                Manufacturer = "Chrysler",
                Model = "RAM 2500 5.9L Cummins",
                Year = 2002,
                Engine = "5.9L Cummins I6 Diesel",
                Transmission = "6-Speed Manual",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "245 HP",
                Torque = "505 lb-ft",
                EcuTypes = new() { "Bosch VP44", "Chrysler PCM" },
                Systems = new() { "4WD", "Quad Cab" }
            },
            new Vehicle
            {
                Id = "dodge_ram_3500_cummins_2007",
                Make = "Dodge",
                Manufacturer = "Chrysler",
                Model = "RAM 3500 5.9L Cummins",
                Year = 2007,
                Engine = "5.9L Cummins I6 Diesel",
                Transmission = "6-Speed Manual",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "325 HP",
                Torque = "610 lb-ft",
                EcuTypes = new() { "Bosch ECU", "Cummins ECM" },
                Systems = new() { "4WD", "Exhaust Brake", "Mega Cab" }
            },
            
            // GM Duramax (LB7/LLY Era)
            new Vehicle
            {
                Id = "chevrolet_silverado_2500hd_duramax_2002",
                Make = "Chevrolet",
                Manufacturer = "General Motors",
                Model = "Silverado 2500HD Duramax LB7",
                Year = 2002,
                Engine = "6.6L Duramax V8 Diesel",
                Transmission = "Allison 1000 5-Speed",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "300 HP",
                Torque = "520 lb-ft",
                EcuTypes = new() { "Bosch ECU", "GM ECM" },
                Systems = new() { "4WD" }
            },
            new Vehicle
            {
                Id = "gmc_sierra_2500hd_duramax_2005",
                Make = "GMC",
                Manufacturer = "General Motors",
                Model = "Sierra 2500HD Duramax LLY",
                Year = 2005,
                Engine = "6.6L Duramax V8 Diesel",
                Transmission = "Allison 1000 5-Speed",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "310 HP",
                Torque = "605 lb-ft",
                EcuTypes = new() { "Bosch ECU", "GM ECM" },
                Systems = new() { "EGR", "4WD" }
            },
            
            // Mercedes E-Class Diesel (W210/W211)
            new Vehicle
            {
                Id = "mercedes_e300_diesel_1997",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "E 300 Turbodiesel",
                Year = 1997,
                Engine = "3.0L OM606 I6 Diesel",
                Transmission = "5-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Diesel",
                Power = "177 HP",
                Torque = "243 lb-ft",
                EcuTypes = new() { "Bosch EDC" },
                Systems = new() { "EGR" }
            },
            new Vehicle
            {
                Id = "mercedes_e320_cdi_2003",
                Make = "Mercedes-Benz",
                Manufacturer = "Daimler",
                Model = "E 320 CDI",
                Year = 2003,
                Engine = "3.2L OM648 V6 Diesel",
                Transmission = "5-Speed Automatic",
                DriveType = "RWD",
                FuelType = "Diesel",
                Power = "204 HP",
                Torque = "369 lb-ft",
                EcuTypes = new() { "Bosch EDC16" },
                Systems = new() { "DPF", "EGR" }
            },
            
            // VW Jetta TDI (MK4)
            new Vehicle
            {
                Id = "vw_jetta_tdi_2003",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Jetta TDI",
                Year = 2003,
                Engine = "1.9L TDI I4 Diesel",
                Transmission = "5-Speed Manual",
                DriveType = "FWD",
                FuelType = "Diesel",
                Power = "100 HP",
                Torque = "177 lb-ft",
                EcuTypes = new() { "Bosch EDC15" },
                Systems = new() { "Pumpe Düse (PD)" }
            },
            new Vehicle
            {
                Id = "vw_golf_tdi_2005",
                Make = "Volkswagen",
                Manufacturer = "Volkswagen Group",
                Model = "Golf TDI",
                Year = 2005,
                Engine = "1.9L TDI I4 Diesel",
                Transmission = "5-Speed Manual",
                DriveType = "FWD",
                FuelType = "Diesel",
                Power = "100 HP",
                Torque = "177 lb-ft",
                EcuTypes = new() { "Bosch EDC15" },
                Systems = new() { "Pumpe Düse (PD)" }
            },
            
            // BMW E46 Diesel
            new Vehicle
            {
                Id = "bmw_330d_e46_2003",
                Make = "BMW",
                Manufacturer = "BMW",
                Model = "330d (E46)",
                Year = 2003,
                Engine = "3.0L M57 I6 Diesel",
                Transmission = "6-Speed Manual",
                DriveType = "RWD",
                FuelType = "Diesel",
                Power = "204 HP",
                Torque = "304 lb-ft",
                EcuTypes = new() { "Bosch EDC15", "BMW DME" },
                Systems = new() { "EGR" }
            },
            
            // Jeep Liberty CRD
            new Vehicle
            {
                Id = "jeep_liberty_crd_2005",
                Make = "Jeep",
                Manufacturer = "Chrysler",
                Model = "Liberty CRD (Diesel)",
                Year = 2005,
                Engine = "2.8L VM Motori CRD I4 Diesel",
                Transmission = "5-Speed Automatic",
                DriveType = "4WD",
                FuelType = "Diesel",
                Power = "160 HP",
                Torque = "295 lb-ft",
                EcuTypes = new() { "Bosch EDC16", "Chrysler PCM" },
                Systems = new() { "EGR", "4WD", "Selec-Trac" }
            }
        };
    }
}

