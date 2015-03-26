namespace LightZApp
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.Devices.Bluetooth.Rfcomm;
    using Windows.Devices.Enumeration;
    using Windows.Foundation;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    using LightZPortableLibrary.Utils.Bluetooth;

    internal class BluetoothHelper : LightZPortableLibrary.Utils.Bluetooth.BluetoothHelper
    {
        #region Fields

        private bool _connected;
        private DeviceInformation _currentDevice;
        private RfcommDeviceService _rfcommService;
        private StreamSocket _socket;
        private DataReader _reader;
        private DataWriter _writer;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the name of the device/com port we are connected
        /// </summary>
        public override string PortName
        {
            get
            {
                return this._currentDevice == null ? string.Empty : this._currentDevice.Name;
            }
        }

        /// <summary>
        /// Returns True whether the PC/Smartphone/Tablet is connected to the device
        /// </summary>
        public override bool Connected
        {
            get
            {
                return this._connected;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when a data is received
        /// </summary>
        public override event BluetoothDataReceivedEventArgsEventHandler DataReceived;

        #endregion

        #region Methods

        /// <summary>
        /// Connect to the device
        /// </summary>
        /// <param name="portName">the name of the device to connect.</param>
        public override async void Connect(string portName = null)
        {
            if (this.Connected)
                return;

            try
            {
                this._currentDevice = (await this.AvailableDevicesAsync()).SingleOrDefault(d => d.Name == portName);
                this._socket = new StreamSocket();
                this._rfcommService = await RfcommDeviceService.FromIdAsync(this._currentDevice.Id);
                await this._socket.ConnectAsync(this._rfcommService.ConnectionHostName, this._rfcommService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                this._writer = new DataWriter(this._socket.OutputStream);
                this._reader = new DataReader(this._socket.InputStream);
                this._connected = true;

                this.DataReceivedAsync();
            }
            catch
            {
                this._connected = false;
            }
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public override void Disconnect()
        {
            if (this._reader != null)
                this._reader = null;
            if (this._writer != null)
            {
                this._writer.DetachStream();
                this._writer = null;
            }
            if (this._socket != null)
            {
                this._socket.Dispose();
                this._socket = null;
            }
            if (this._rfcommService != null)
                this._rfcommService = null;
            this._connected = false;
        }

        /// <summary>
        /// Send an array of byte to the device
        /// </summary>
        /// <param name="data">the array of byte to send</param>
        public override async void Send(byte[] data)
        {
            if (!this.Connected || this._writer == null)
                return;

            this._writer.WriteBytes(data);
            await this._writer.StoreAsync();
        }

        /// <summary>
        /// Retrieve a collection that contains informations about the available devices
        /// </summary>
        /// <returns>a DeviceInformationCollection</returns>
        public async Task<DeviceInformationCollection> AvailableDevicesAsync()
        {
            return await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
        }

        /// <summary>
        /// Listen to the device to read each data we receive.
        /// </summary>
        private async void DataReceivedAsync()
        {
            while (this.Connected && this._reader != null)
            {
                var sizeFieldCount = await this._reader.LoadAsync(1);
                if (sizeFieldCount != 1)
                    return;

                var messageLength = this._reader.ReadByte();
                var actualMessageLength = await this._reader.LoadAsync(messageLength);
                if (messageLength != actualMessageLength)
                    return;

                var data = this._reader.ReadString(actualMessageLength);

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
        }

        #endregion
    }
}
