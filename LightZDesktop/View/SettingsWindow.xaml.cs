namespace LightZDesktop.View
{
    using System.Windows;
    using System.Windows.Controls;

    using LightZDesktop.ViewModel;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            this.InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((SettingsViewModel)this.DataContext).AudioDeviceCommand.Execute(null);
        }
    }
}
