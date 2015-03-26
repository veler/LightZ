namespace LightZApp.ViewModel
{
    using System;
    using System.Collections.Generic;

    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using LightZApp.Utils;

    using LightZPortableLibrary.Enums;
    using LightZPortableLibrary.Model;
    using LightZPortableLibrary.Utils.Threading;

    public class SettingsViewModel : ViewModelBase
    {
        #region Fields

        private string[] _devices;
        private Settings _settings;
        private Timer _timer;
        private Windows.UI.Color _selectedColor;

        #endregion

        #region Properties

        /// <summary>
        /// Obtient ou définit le modèle principale de la fenêtre Settings
        /// </summary>
        public Settings Settings
        {
            get { return this._settings; }
            set
            {
                this._settings = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Obtient la liste des ports disponibles
        /// </summary>
        public string[] AvailableComPorts
        {
            get
            {
                return this._devices;
            }
        }

        public Windows.UI.Color SelectedColor
        {
            get
            {
                return this._selectedColor;
            }
            set
            {
                this._selectedColor = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SettingsViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            this.Settings = new SettingsDesktop();

            if (this.IsInDesignMode)
            {
                this.Settings.ComPort = "COM5";
                this.Settings.CurrentColor = new Color(255, 0, 0);
                this.Settings.CurrentBrightness = (byte)200;
            }
            else
            {
                this.RefreshDeviceListCommand = new RelayCommand(this.RefreshDeviceListCommandExecute);
                this.ConnectCommand = new RelayCommand(this.ConnectCommandExecute);
                this.ModeCommand = new RelayCommand(this.ModeCommandExecute);

                this._timer = new Timer(TimeSpan.FromMilliseconds(100));
                this._timer.Tick += this.TimerTick;

                this.Settings.Connected = App.Service.Connected;
                this.Settings.ComPort = ApplicationSettings.GetSetting<string>("ComPort");
                this.Settings.Mode = ApplicationSettings.GetSetting<Mode>("Mode");
                this.Settings.CurrentColor = ApplicationSettings.GetSetting<Color>("CurrentColor");
                this.Settings.CurrentBrightness = ApplicationSettings.GetSetting<byte>("CurrentBrightness");
            }
        }

        #endregion

        #region Commands

        public RelayCommand RefreshDeviceListCommand { get; set; }

        private async void RefreshDeviceListCommandExecute()
        {
            var result = new List<string>();
            foreach (var device in await App.Service.AvailableDevices())
                result.Add(device.Name);
            this._devices = result.ToArray();
            this.RaisePropertyChanged("AvailableComPorts");
        }

        public RelayCommand ConnectCommand { get; set; }

        private void ConnectCommandExecute()
        {
            this.Settings.Mode = Mode.Manual;
            ApplicationSettings.SetSetting("Mode", this.Settings.Mode);

            ApplicationSettings.SetSetting("ComPort", this.Settings.ComPort);

            var service = App.Service;
            if (this.Settings.Connected)
                service.Disconnect();
            else
                service.Connect();
        }

        public RelayCommand ModeCommand { get; set; }

        private void ModeCommandExecute()
        {
            ApplicationSettings.SetSetting("Mode", this.Settings.Mode);
        }

        #endregion

        #region Methods

        private async void TimerTick(object sender, EventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var service = App.Service;
                this.Settings.Connected = service.Connected;
                if (this.SelectedColor != null)
                    this.Settings.CurrentColor = new Color(this.SelectedColor.R, this.SelectedColor.G, this.SelectedColor.B);
                ApplicationSettings.SetSetting("CurrentColor", this.Settings.CurrentColor);
                ApplicationSettings.SetSetting("CurrentBrightness", this.Settings.CurrentBrightness);
            });
        }

        public override void Cleanup()
        {
            App.Service.Dispose();
            this._timer.Tick -= this.TimerTick;
            this._timer.Dispose();
        }

        #endregion
    }
}