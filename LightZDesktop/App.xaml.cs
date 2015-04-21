namespace LightZDesktop
{
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    using LightZDesktop.Properties;
    using LightZDesktop.Utils;
    using LightZDesktop.ViewModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Handled Methods

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.SingleInstance();

            ((InteractionService)Current.FindResource("Service")).Connect();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (e.ApplicationExitCode == 33695)
                return;
            ((InteractionService)Current.FindResource("Service")).Dispose();
            ViewModelLocator.Cleanup();
            Settings.Default.Save();
        }

        #endregion

        #region Methods

        private void SingleInstance()
        {
            var currentProcess = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(currentProcess.ProcessName).Any(p => p.Id != currentProcess.Id))
                Current.Shutdown(33695);
        }

        #endregion
    }
}
