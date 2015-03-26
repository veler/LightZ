namespace LightZDesktop.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Windows;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    using LightZDesktop.Utils;

    using LightZPortableLibrary.Model;
    using LightZPortableLibrary.Utils.Threading;

    public class SettingsViewModel : ViewModelBase
    {
        #region Fields

        private SettingsDesktop _settings;
        private Timer _timer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the main model of the Settings window
        /// </summary>
        public SettingsDesktop Settings
        {
            get { return this._settings; }
            set
            {
                this._settings = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the list of available ports
        /// </summary>
        public string[] AvailableComPorts
        {
            get { return SerialPort.GetPortNames(); }
        }

        /// <summary>
        /// Gets the list of audio devices available
        /// </summary>
        public List<AudioDevice> AvailableAudioDevices
        {
            get { return this.GetService().AudioDevices; }
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
                this.ConnectCommand = new RelayCommand(this.ConnectCommandExecute);
                this.AudioDeviceCommand = new RelayCommand(this.AudioDeviceCommandExecute);
                this.ModeCommand = new RelayCommand(this.ModeCommandExecute);

                this._timer = new Timer(TimeSpan.FromMilliseconds(10));
                this._timer.Tick += this.TimerTick;

                this.Settings.Connected = this.GetService().Connected;
                this.Settings.AudioDevice = Properties.Settings.Default.CurrentAudioDevice;
                this.Settings.ComPort = Properties.Settings.Default.ComPort;
                this.Settings.Mode = Properties.Settings.Default.Mode;
                this.Settings.CurrentColor = Properties.Settings.Default.CurrentColor;
                this.Settings.CurrentBrightness = Properties.Settings.Default.CurrentBrightness;
            }
        }

        #endregion

        #region Commands

        public RelayCommand ConnectCommand { get; set; }

        private void ConnectCommandExecute()
        {
            Properties.Settings.Default.ComPort = this.Settings.ComPort;
            Properties.Settings.Default.Save();

            var service = this.GetService();
            if (this.Settings.Connected)
                service.Disconnect();
            else
                service.Connect();
        }

        public RelayCommand AudioDeviceCommand { get; set; }

        private void AudioDeviceCommandExecute()
        {
            Properties.Settings.Default.CurrentAudioDevice = this.Settings.AudioDevice;
            Properties.Settings.Default.Save();

            this.GetService().UpdateAudioDevice();
        }

        public RelayCommand ModeCommand { get; set; }

        private void ModeCommandExecute()
        {
            Properties.Settings.Default.Mode = this.Settings.Mode;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Methods

        private InteractionService GetService()
        {
            return (InteractionService)Application.Current.FindResource("Service");
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var service = this.GetService();
            this.Settings.Connected = service.Connected;
            Properties.Settings.Default.CurrentColor = this.Settings.CurrentColor;
            Properties.Settings.Default.CurrentBrightness = this.Settings.CurrentBrightness;
            Properties.Settings.Default.Save();
        }

        public override void Cleanup()
        {
            this.GetService().Dispose();
            this._timer.Tick -= this.TimerTick;
            this._timer.Dispose();
        }

        #endregion
    }
}