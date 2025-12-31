using System;
using System.Windows;
using System.Windows.Threading;

namespace BlackFlag
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Console.WriteLine("OnStartup called");
            base.OnStartup(e);
            
            // Global exception handlers
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            try
            {
                Console.WriteLine("Initializing database...");
                // Initialize database
                Services.Database.Instance.Initialize();
                Console.WriteLine("Database initialized");
                
                Console.WriteLine("Creating MainWindow...");
                var mainWindow = new MainWindow();
                Console.WriteLine("Showing MainWindow...");
                mainWindow.Show();
                Console.WriteLine("MainWindow shown");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Startup error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show($"Startup error:\n{ex.Message}\n\n{ex.StackTrace}", 
                    "BlackFlag Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Dispatcher error: {e.Exception.Message}");
            MessageBox.Show($"Application error:\n{e.Exception.Message}\n\n{e.Exception.StackTrace}", 
                "BlackFlag Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Console.WriteLine($"Domain error: {ex?.Message}");
            MessageBox.Show($"Fatal error:\n{ex?.Message}\n\n{ex?.StackTrace}", 
                "BlackFlag Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
