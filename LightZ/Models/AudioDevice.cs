namespace LightZ.Models
{
    /// <summary>
    /// Represents information about an audio device.
    /// </summary>
    internal sealed class AudioDevice
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ID of the audio device.
        /// </summary>
        internal int DeviceId { get; }

        /// <summary>
        /// Gets or sets the name of the audio device.
        /// </summary>
        public string DeviceName { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the <see cref="AudioDevice"/> class.
        /// </summary>
        /// <param name="deviceId">Audio device's Id</param>
        /// <param name="deviceName">Audio device's name</param>
        internal AudioDevice(int deviceId, string deviceName)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
        }

        #endregion
    }
}
