namespace LightZPortableLibrary.Utils.Bluetooth
{
    /// <summary>
    /// Manage a Bluetooth connection
    /// </summary>
    public abstract class BluetoothHelper
    {
        #region Properties

        /// <summary>
        /// Returns the name of the device/com port we are connected
        /// </summary>
        public abstract string PortName { get; }

        /// <summary>
        /// Returns True whether the PC/Smartphone/Tablet is connected to the device
        /// </summary>
        public abstract bool Connected { get; }

        #endregion

        #region Events

        public delegate void BluetoothDataReceivedEventArgsEventHandler(object sender, BluetoothDataReceivedEventArgs e);

        /// <summary>
        /// Raised when a data is received
        /// </summary>
        public abstract event BluetoothDataReceivedEventArgsEventHandler DataReceived;

        #endregion

        #region Methods

        /// <summary>
        /// Connect to the device
        /// </summary>
        /// <param name="portName">the name of the device to connect.</param>
        public abstract void Connect(string portName = null);

        /// <summary>
        /// Disconnect
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Send an array of byte to the device
        /// </summary>
        /// <param name="data">the array of byte to send</param>
        public abstract void Send(byte[] data);

        #endregion
    }
}
