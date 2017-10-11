namespace LightZ.Models
{
    /// <summary>
    /// Represents a Bluetooth or USB device.
    /// </summary>
    internal sealed class SerialDevice
    {
        /// <summary>
        /// Gets the COM port number of the device.
        /// </summary>
        internal string ComPortNumber { get; }

        /// <summary>
        /// Gets the name of the Bluetooth or USB device.
        /// </summary>
        public string DeviceName { get; }

        #region Contructors

        /// <summary>
        /// Initializes an instance of the <see cref="SerialDevice"/> class.
        /// </summary>
        /// <param name="comPortNumber">The COM port number of the device.</param>
        /// <param name="deviceName">The name of the Bluetooth or USB device.</param>
        internal SerialDevice(string comPortNumber, string deviceName)
        {
            DeviceName = deviceName;
            ComPortNumber = comPortNumber;
        }

        #endregion
    }
}
