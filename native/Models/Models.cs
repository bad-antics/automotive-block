using System.Collections.Generic;

namespace BlackFlag.Models
{
    public class Vehicle
    {
        public string Id { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Engine { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public string DriveType { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public string Power { get; set; } = string.Empty;
        public string Torque { get; set; } = string.Empty;
        public List<string> EcuTypes { get; set; } = new();
        public List<string> Systems { get; set; } = new();
        
        public string DisplayName => $"{Year} {Make} {Model} - {Engine} ({string.Join(", ", EcuTypes ?? new List<string>())})";
        
        public override string ToString() => DisplayName;
    }
    
    public class EcuProfile
    {
        public string Id { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string EcuType { get; set; } = string.Empty;
        public string SoftwareVersion { get; set; } = string.Empty;
        public string HardwareVersion { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
    }
    
    public class Tune
    {
        public string Id { get; set; } = string.Empty;
        public string VehicleId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Stage1, Stage2, Economy, Stock
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public System.DateTime Created { get; set; }
        public bool IsStock { get; set; }
    }
    
    public class DiagnosticCode
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string PossibleCauses { get; set; } = string.Empty;
    }
    
    public class WiringDiagram
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DiagramData { get; set; } = string.Empty; // ASCII art or SVG path
    }
    
    public class ScanResult
    {
        public string ModuleName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public List<DiagnosticCode> Codes { get; set; } = new();
    }
}
