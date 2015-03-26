namespace LightZApp.Utils
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Windows.Devices.Enumeration;

    using LightZPortableLibrary.Enums;
    using LightZPortableLibrary.Model;
    using LightZPortableLibrary.Utils.Bluetooth;
    using LightZPortableLibrary.Utils.Services;

    using BluetoothHelper = LightZApp.BluetoothHelper;

    /// <summary>
    /// Classe permettant d'implémenter le code devant envoyer des requêtes à interval régulier à l'Arduino
    /// </summary>
    public sealed class InteractionService : LightZPortableLibrary.Utils.Services.InteractionService
    {
        #region Properties

        /// <summary>
        /// La classe permettant d'intéragir par Bluetooth avec l'Arduino. Cette classe change selon la plateforme (Desktop, Tablette, Smartphone).
        /// </summary>
        protected override LightZPortableLibrary.Utils.Bluetooth.BluetoothHelper Bluetooth { get; set; }

        /// <summary>
        /// Retourne True si l'on est connecté à l'appareil en Bluetooth
        /// </summary>
        public bool Connected
        {
            get
            {
                return this.Bluetooth.Connected;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialise une instance de la classe InteractionService
        /// </summary>
        public InteractionService()
        {
            this.Bluetooth = new BluetoothHelper();
            this.Bluetooth.DataReceived += this.BluetoothDataReceived;
        }

        #endregion

        #region Handled Methods

        /// <summary>
        /// Methode appelée lorsqu'une donnée provenant de l'appareil en Bluetooth est reçu.
        /// </summary>
        private void BluetoothDataReceived(object sender, BluetoothDataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Exécuté en bouclé. Cette méthode est destiné à être exploité pour envoyer les données à l'Arduino.
        /// </summary>
        protected override async void Loop()
        {
            if (this.Bluetooth == null || !this.Bluetooth.Connected)
                return;

            var mode = ApplicationSettings.GetSetting<Mode>("Mode");
            if (this.CurrentArduinoMode == Mode.Unknow || this.CurrentArduinoMode != mode)
            {
                this.CurrentArduinoMode = mode;
                this.Bluetooth.Send(QueryManager.GenerateModeQuery(mode));

                if (this.CurrentArduinoMode == Mode.Manual)
                    this.ShowColor();
            }

            switch (this.CurrentArduinoMode)
            {
                case Mode.Off:
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    break;

                case Mode.Manual:
                    if (ApplicationSettings.GetSetting<Color>("CurrentColor").Equals(this.CurrentArduinoColor) && ApplicationSettings.GetSetting<byte>("CurrentBrightness") == this.CurrentArduinoBrightness)
                        break;
                    this.ShowColor();
                    break;
            }
        }

        public void Disconnect()
        {
            this.Bluetooth.Disconnect();
        }

        public void Connect()
        {
            this.Bluetooth.Connect(ApplicationSettings.GetSetting<string>("ComPort"));
        }

        public void ShowColor()
        {
            this.CurrentArduinoColor = ApplicationSettings.GetSetting<Color>("CurrentColor");
            this.CurrentArduinoBrightness = ApplicationSettings.GetSetting<byte>("CurrentBrightness");

            var led = new Led();
            led.LedIndex = Target.AllLeds;
            led.Color = this.CurrentArduinoColor;

            this.Bluetooth.Send(QueryManager.GenerateBrightnessQuery(this.CurrentArduinoBrightness));
            this.Bluetooth.Send(QueryManager.GenerateLedQuery(led));
        }

        /// <summary>
        /// Retourne la liste des périphériques Bluetooth
        /// </summary>
        public async Task<DeviceInformationCollection> AvailableDevices()
        {
            return await ((BluetoothHelper)this.Bluetooth).AvailableDevicesAsync();
        }

        public new void Dispose()
        {
            this.Disconnect();
        }

        #endregion
    }
}
