namespace LightZPortableLibrary.Model
{
    /// <summary>
    /// Defines the Desktop application settings
    /// </summary>
    public class SettingsDesktop : Settings
    {
        #region Field

        private AudioDevice _audioDevice;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the audio device to use
        /// </summary>
        public AudioDevice AudioDevice
        {
            get { return this._audioDevice; }
            set
            {
                this._audioDevice = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion
    }
}
