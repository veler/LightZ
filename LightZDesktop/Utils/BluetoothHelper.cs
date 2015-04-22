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
        /// For the Desktop application in particular : returns the name of the port it is connected by Bluetooth to interact
        /// </summary>
        public override string PortName
        {
            get
            {
                return this._serialPort.PortName;
            }
        }

        /// <summary>
        /// Returns True if it is currently connected to the device
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
        /// Event raised when data from the Bluetooth device is received.
        /// </summary>
        public override event BluetoothDataReceivedEventArgsEventHandler DataReceived;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="BluetoothHelper"/>
        /// </summary>
        public BluetoothHelper()
        {
            this._serialPort = new SerialPort();
        }

        #endregion

        #region Handled Methods

        /// <summary>
        /// Method called when the serial port receives data
        /// </summary>
        /// <param name="sender">the serial port</param>
        /// <param name="e">the data</param>
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
        /// Connect to the device
        /// </summary>
        /// <param name="portName">port name</param>
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
        /// Disconnect from the device
        /// </summary>
        public override void Disconnect()
        {
            this._serialPort.DataReceived -= this.SerialPort_DataReceived;
            this._serialPort.Close();
        }

        /// <summary>
        /// Send a data to the device by Bluetooth
        /// </summary>
        /// <param name="data">The data to send</param>
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
