using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LightZ.ComponentModel.Core;
using LightZ.ComponentModel.Services;
using LightZ.ComponentModel.Services.Base;
using LightZ.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace LightZ.ViewModels
{
    /// <summary>
    /// Interaction logic for <see cref="MainWindow"/>
    /// </summary>
    internal sealed class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private Visibility _notifyIconVisibility;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the visibility of the <see cref="NotifyIcon"/> of the <see cref="MainWindow"/>.
        /// </summary>
        public Visibility NotifyIconVisibility
        {
            get { return _notifyIconVisibility; }
            private set
            {
                _notifyIconVisibility = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        internal MainWindowViewModel()
        {
            InitializeCommands();

            if (IsInDesignMode)
            {
                return;
            }

            ShowNotifyIcon();

            ServiceLocator.GetService<AudioService>();
            ServiceLocator.GetService<DirectXScreenService>();
            ServiceLocator.GetService<SerialService>();
            ServiceLocator.GetService<LedStripService>();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Initialize the commands of the View Model
        /// </summary>
        private void InitializeCommands()
        {
            SettingsCommand = new RelayCommand(ExecuteSettingsCommand);
            ExitCommand = new RelayCommand(ExecuteExitCommand);
            ContextMenuOpeningCommand = new RelayCommand(ExecuteContextMenuOpeningCommand);
        }

        #region Settings

        /// <summary>
        /// Gets or sets a <see cref="RelayCommand"/> executed when we click on the Settings button
        /// </summary>
        public RelayCommand SettingsCommand { get; private set; }

        private void ExecuteSettingsCommand()
        {
            HideNotifyIcon();

            var window = new SettingsWindow();
            window.ShowDialog();

            ShowNotifyIcon();
        }

        #endregion

        #region Exit

        /// <summary>
        /// Gets or sets a <see cref="RelayCommand"/> executed when we click on the Quit button
        /// </summary>
        public RelayCommand ExitCommand { get; private set; }

        private void ExecuteExitCommand()
        {
            HideNotifyIcon();

            var delayer = new Delayer<object>(TimeSpan.FromMilliseconds(300));
            delayer.Action += (o, args) =>
            {
                if (CoreHelper.IsUnitTesting())
                {
                    throw new OperationCanceledException("Unable to quit a unit test");
                }

                Application.Current.Shutdown(0);
            };
            delayer.ResetAndTick();
        }

        #endregion

        #region ContextMenuOpening

        /// <summary>
        /// Gets or sets a <see cref="RelayCommand"/> executed when the context menu is opening
        /// </summary>
        public RelayCommand ContextMenuOpeningCommand { get; private set; }

        private void ExecuteContextMenuOpeningCommand()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Hide the <see cref="ComponentModel.UI.Controls.NotifyIcon"/> and disable the mouse and keyboard interaction.
        /// </summary>
        private void HideNotifyIcon()
        {
            CoreHelper.ThrowIfNotStaThread();

            NotifyIconVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Show the <see cref="ComponentModel.UI.Controls.NotifyIcon"/> and enable the mouse and keyboard interaction.
        /// </summary>
        private void ShowNotifyIcon()
        {
            CoreHelper.ThrowIfNotStaThread();

            NotifyIconVisibility = Visibility.Visible;
        }

        #endregion
    }
}
