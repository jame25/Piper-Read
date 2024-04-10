using System;
using System.Threading;
using System.Windows.Forms;

namespace piper_read
{
    static class Program
    {
        private static Mutex mutex = null;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string appName = "PiperRead";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                MessageBox.Show("Another instance of Piper Read is already running.", "Piper Read", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
            finally
            {
                // Release the mutex when the application exits
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }
    }
}
