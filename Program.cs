using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace piper_read
{
    static class Program
    {
        private static Mutex mutex = null;
        public static bool LoggingEnabled { get; private set; }

        public static void Log(string message)
        {
            if (LoggingEnabled)
            {
                File.AppendAllText("system.log", $"{DateTime.Now}: {message}\n");
            }
        }

        private static void LoadLoggingSettings()
        {
            string settingsPath = "settings.conf";
            if (File.Exists(settingsPath))
            {
                string[] lines = File.ReadAllLines(settingsPath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("Logging="))
                    {
                        LoggingEnabled = line.EndsWith("True", StringComparison.OrdinalIgnoreCase);
                        return;
                    }
                }
            }

            // Default to logging enabled if setting not found
            LoggingEnabled = true;
            File.AppendAllText(settingsPath, $"\nLogging=True");
        }

        [STAThread]
        static void Main()
        {
            LoadLoggingSettings();
            Log("Application starting");

            const string appName = "PiperRead";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                Log("Another instance detected - shutting down");
                MessageBox.Show("Another instance of Piper Read is already running.", "Piper Read", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Log("Initializing application configuration");
                ApplicationConfiguration.Initialize();
                Log("Starting main form");
                Application.Run(new Form1());
            }
            finally
            {
                Log("Application shutting down");
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }
    }
}
