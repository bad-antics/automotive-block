using System.Collections.Generic;
using System.Windows.Media;

namespace BlackFlag.Models
{
    public class PinInfo
    {
        public string Pin { get; set; } = "";
        public string Function { get; set; } = "";
        public string WireColor { get; set; } = "";
        public Color WireColorCode { get; set; } = Colors.Gray;
        public string Voltage { get; set; } = "";
        public string SignalType { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class DiagramCategory
    {
        public string Name { get; set; } = "";
        public List<string> Diagrams { get; set; } = new();
    }
}
