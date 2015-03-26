namespace LightZPortableLibrary.Model
{
    using GalaSoft.MvvmLight;

    using LightZPortableLibrary.Enums;

    /// <summary>
    /// Defines the parameters shared between the Desktop application, Tablet and Smartphone
    /// </summary>
    public class Settings : ObservableObject
    {
        #region Field

        private Mode _mode;
        private Color _currentColor;
        private byte _currentBrightness;
        private bool _connected;
        private string _comPort;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current mode of the Arduino
        /// </summary>
        public Mode Mode
        {
            get { return this._mode; }
            set
            {
                this._mode = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current color of the led strip (if possible)
        /// </summary>
        public Color CurrentColor
        {
            get { return this._currentColor; }
            set
            {
                this._currentColor = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current contrast of the led strip
        /// </summary>
        public byte CurrentBrightness
        {
            get { return this._currentBrightness; }
            set
            {
                this._currentBrightness = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if we are connected to the Arduino
        /// </summary>
        public bool Connected
        {
            get { return this._connected; }
            set
            {
                this._connected = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the COM port name to use the Bluetooth connection
        /// </summary>
        public string ComPort
        {
            get { return this._comPort; }
            set
            {
                this._comPort = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="Settings"/>
        /// </summary>
        public Settings()
        {
            this.CurrentColor = new Color();
        }

        #endregion
    }
}
