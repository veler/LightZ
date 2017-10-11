using LightZ.ComponentModel.UI.Controls;
using System.Windows.Input;

namespace LightZ.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : BlurredWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void DragZoneGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
