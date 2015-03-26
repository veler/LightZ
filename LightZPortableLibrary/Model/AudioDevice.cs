namespace LightZPortableLibrary.Model
{
    using System;

    using GalaSoft.MvvmLight;

    /// <summary>
    /// Represents an audio device
    /// </summary>
    [Serializable]
    public class AudioDevice : ObservableObject
    {
        #region Field

        private int _deviceId;
        private string _deviceName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the audio device
        /// </summary>
        public int DeviceId
        {
            get { return this._deviceId; }
            set
            {
                this._deviceId = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the audio device
        /// </summary>
        public string DeviceName
        {
            get { return this._deviceName; }
            set
            {
                this._deviceName = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="AudioDevice"/>
        /// </summary>
        public AudioDevice()
        {
        }

        /// <summary>
        /// Initializes an instance of the class <see cref="AudioDevice"/>
        /// </summary>
        /// <param name="deviceId">Audio device's Id</param>
        /// <param name="deviceName">Audio device's name</param>
        public AudioDevice(int deviceId, string deviceName)
        {
            this._deviceId = deviceId;
            this._deviceName = deviceName;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return this.DeviceName;
        }

        public override int GetHashCode()
        {
            return this._deviceId.GetHashCode() + this._deviceName.GetHashCode();
        }

        #endregion
    }
}
