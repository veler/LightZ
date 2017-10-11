using LightZ.ComponentModel;
using LightZ.ComponentModel.Services;
using LightZ.ComponentModel.Services.Base;
using LightZ.Properties;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace LightZ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Handled Methods

        /// <summary>
        /// Occurs when the Run method of the Application object is called.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 10)
            {
                MessageBox.Show("This application is only compatible with Windows 10.", "LightZ", MessageBoxButton.OK, MessageBoxImage.Stop);
                Current.Shutdown(Consts.SingleInstanceProcessExitCode);
                return;
            }

            if (SingleInstance(e))
            {
                return;
            }
        }

        /// <summary>
        /// Occurs just before an application shuts down, and cannot be canceled.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (e.ApplicationExitCode == Consts.SingleInstanceProcessExitCode)
            {
                return;
            }

            ServiceLocator.GetService<MouseHookService>().Dispose();
            ServiceLocator.GetService<LedStripService>().Dispose();

            Settings.Default.Save();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check is there is more than one instance of the application, and if yes, it's killing it.
        /// </summary>
        /// <returns>Returns True if another instance has been founded and that the current process should be killed.</returns>
        /// <param name="e">The arguments of the <see cref="Application.Startup"/> application event.</param>
        private bool SingleInstance(StartupEventArgs startupInfo)
        {
            if (Debugger.IsAttached)
            {
                return false;
            }

            if (startupInfo.Args.Contains("-skipsingleinstance"))
            {
                return false;
            }

            var currentProcess = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(currentProcess.ProcessName).Count() > 1)
            {
                Current.Shutdown(Consts.SingleInstanceProcessExitCode);
                return true;
            }

            return false;
        }

        #endregion
    }
}
