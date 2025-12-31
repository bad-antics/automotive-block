using System;
using System.Windows;

namespace BlackFlag
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting BlackFlag...");
            try
            {
                var app = new App();
                app.InitializeComponent();
                Console.WriteLine("App initialized, running...");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\nPress Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
