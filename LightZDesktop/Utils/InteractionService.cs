namespace LightZDesktop.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using LightZPortableLibrary.Enums;
    using LightZPortableLibrary.Model;
    using LightZPortableLibrary.Utils.Bluetooth;
    using LightZPortableLibrary.Utils.Services;

    using Settings = LightZDesktop.Properties.Settings;

    /// <summary>
    /// Class used to implement the code to send queries at regular interval to the Arduino
    /// </summary>
    public sealed class InteractionService : LightZPortableLibrary.Utils.Services.InteractionService
    {
        #region Fields

        private DirectxScreenCapturer _directxScreenCapturer;
        private AudioAnalyzer _audioAnalyze;
        private DateTime _lastScreenWait;

        #endregion

        #region Properties

        /// <summary>
        /// The class allows to interact with Arduino by Bluetooth. This class changes with the platform (Desktop, tablet, smartphone).
        /// </summary>
        protected override LightZPortableLibrary.Utils.Bluetooth.BluetoothHelper Bluetooth { get; set; }

        /// <summary>
        /// Returns True whether the PC/Smartphone/Tablet is connected to the device
        /// </summary>
        public bool Connected
        {
            get
            {
                return this.Bluetooth.Connected;
            }
        }

        /// <summary>
        /// Returns a list of audio devices
        /// </summary>
        public List<AudioDevice> AudioDevices
        {
            get
            {
                return this._audioAnalyze.AudioDevices;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="InteractionService"/>
        /// </summary>
        public InteractionService()
        {
            this._directxScreenCapturer = new DirectxScreenCapturer();
            this._audioAnalyze = new AudioAnalyzer();
            this._lastScreenWait = DateTime.Now;

            this.UpdateAudioDevice();

            this.Bluetooth = new BluetoothHelper();
            this.Bluetooth.DataReceived += this.BluetoothDataReceived;
        }

        #endregion

        #region Handled Methods

        /// <summary>
        /// Method called when data from the Bluetooth device is received.
        /// </summary>
        private void BluetoothDataReceived(object sender, BluetoothDataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executed repeatedly. This method is intended to be used to send data to the Arduino.
        /// </summary>
        protected override void Loop()
        {
            if (this.Bluetooth == null || !this.Bluetooth.Connected)
                return;

            if (this.CurrentArduinoMode == Mode.Unknow || this.CurrentArduinoMode != Settings.Default.Mode)
            {
                this.CurrentArduinoMode = Settings.Default.Mode;
                this.Bluetooth.Send(QueryManager.GenerateModeQuery(Settings.Default.Mode));

                if (this.CurrentArduinoMode == Mode.Manual)
                    this.ShowColor();
                this._audioAnalyze.Listening = this.CurrentArduinoMode == Mode.Sound;
            }

            switch (this.CurrentArduinoMode)
            {
                case Mode.Off:
                    Thread.Sleep(500);
                    break;

                case Mode.Manual:
                    if (Settings.Default.CurrentColor.Equals(this.CurrentArduinoColor) && Settings.Default.CurrentBrightness == this.CurrentArduinoBrightness)
                        break;
                    this.ShowColor();
                    break;

                case Mode.Screen:
                    var leds = this._directxScreenCapturer.GetLedsFromScreenCapture();
                    foreach (var ledsPart in leds) // small packets are sent to avoid saturating the Bluetooth antenna
                    {
                        this.Bluetooth.Send(QueryManager.GenerateLedQuery(ledsPart));
                        Thread.Sleep(2);
                    }
                    break;

                case Mode.Sound:
                    Thread.Sleep(20); // to be "about" a frequency of 40Hz
                    if (this._audioAnalyze.Listening)
                    {
                        var levels = this._audioAnalyze.AnalyzeBassAverage();
                        this.Bluetooth.Send(QueryManager.GenerateSoundQuery(levels));
                    }
                    break;
            }
        }

        public void Disconnect()
        {
            this.Bluetooth.Disconnect();
        }

        public void Connect()
        {
            this.Bluetooth.Connect(Settings.Default.ComPort);
        }

        public void ShowColor()
        {
            this.CurrentArduinoColor = Settings.Default.CurrentColor;
            this.CurrentArduinoBrightness = Settings.Default.CurrentBrightness;

            var led = new Led();
            led.LedIndex = Target.AllLeds;
            led.Color = this.CurrentArduinoColor;

            this.Bluetooth.Send(QueryManager.GenerateBrightnessQuery(this.CurrentArduinoBrightness));
            this.Bluetooth.Send(QueryManager.GenerateLedQuery(led));
        }

        public void UpdateAudioDevice()
        {
            this._audioAnalyze.CurrentAudioDevice = Settings.Default.CurrentAudioDevice;
        }

        public new void Dispose()
        {
            if (this.CurrentArduinoMode != Mode.Off)
            {
                var led = new Led();
                led.LedIndex = Target.AllLeds;
                led.Color = new Color(0, 0, 0);

                this.Bluetooth.Send(QueryManager.GenerateLedQuery(led));
            }

            this.Disconnect();
            this._audioAnalyze.Dispose();
        }

        #endregion
    }
}
