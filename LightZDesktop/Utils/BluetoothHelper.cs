namespace LightZDesktop.Utils
{
    using System.IO.Ports;
    using System.Threading;
    using System.Threading.Tasks;

    using LightZPortableLibrary.Utils.Bluetooth;

    internal class BluetoothHelper : LightZPortableLibrary.Utils.Bluetooth.BluetoothHelper
    {
        #region Fields

        private readonly SerialPort _serialPort;

        #endregion

        #region Properties

        /// <summary>
        /// Pour l'application Desktop en particulier, retourne le nom du port auquel on est connecté pour intéragir par Bluetooth
        /// </summary>
        public override string PortName
        {
            get
            {
                return this._serialPort.PortName;
            }
        }

        /// <summary>
        /// Retourne True si on est actuellement connecté à l'appareil
        /// </summary>
        public override bool Connected
        {
            get
            {
                return this._serialPort.IsOpen;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Evènement levé lorsqu'une donnée provenant de l'appareil en Bluetooth est reçu.
        /// </summary>
        public override event BluetoothDataReceivedEventArgsEventHandler DataReceived;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialise une instance de la classe BluetoothHelper
        /// </summary>
        public BluetoothHelper()
        {
            this._serialPort = new SerialPort();
        }

        #endregion

        #region Handled Methods

        /// <summary>
        /// Méthode appelée lorsque le port série reçoit une donnée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var data = this._serialPort.ReadLine().ToUpper();

                if (string.IsNullOrWhiteSpace(data))
                    return;

                Task.Run(
                    () =>
                    {
                        var handler = this.DataReceived;
                        if (handler != null)
                        {
                            handler(this, new BluetoothDataReceivedEventArgs(data));
                        }
                    });
            }
            catch { }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Se connecte à l'appareil
        /// </summary>
        /// <param name="portName">Nom du port</param>
        public override void Connect(string portName)
        {
            Task.Run(
                () =>
                {
                    try
                    {
                        this._serialPort.PortName = portName;
                        this._serialPort.BaudRate = 115200;
                        this._serialPort.WriteTimeout = 100;
                        this._serialPort.Open();
                        this._serialPort.DataReceived += this.SerialPort_DataReceived;
                    }
                    catch { }
                });
        }

        /// <summary>
        /// Se déconnecter de l'appareil
        /// </summary>
        public override void Disconnect()
        {
            this._serialPort.DataReceived -= this.SerialPort_DataReceived;
            this._serialPort.Close();
        }

        /// <summary>
        /// Envoyer une donnée par Bluetooth à l'appareil
        /// </summary>
        /// <param name="data">La donnée à envoyer</param>
        public override void Send(byte[] data)
        {
            if (!this.Connected)
                return;

            try
            {
                this._serialPort.Write(data, 0, data.Length);
            }
            catch { }
        }

        #endregion
    }
}
