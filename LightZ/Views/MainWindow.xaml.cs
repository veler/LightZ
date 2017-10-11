using LightZ.ComponentModel.Core;
using LightZ.Properties;
using LightZ.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LightZ.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            foreach (var menuItem in NotifyIcon.ContextMenu.Items.OfType<MenuItem>())
            {
                menuItem.DataContext = DataContext;
            }

            var delayer = new Delayer<object>(TimeSpan.FromMilliseconds(1000));
            delayer.Action += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(Settings.Default.SerialDevice))
                {
                    var viewModel = (MainWindowViewModel)DataContext;
                    if (viewModel.SettingsCommand.CanExecute(null))
                    {
                        viewModel.SettingsCommand.Execute(null);
                    }
                }
            };

            delayer.ResetAndTick();

            Hide();
        }

        #endregion

        #region Handled Methods

        /// <summary>
        /// A <see cref="EventToCommand"/> would not work because the window is initialized but never loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            if (e.ChangedButton == MouseButton.Left && viewModel.SettingsCommand.CanExecute(null))
            {
                viewModel.SettingsCommand.Execute(null);
            }
        }

        #endregion
    }
}
