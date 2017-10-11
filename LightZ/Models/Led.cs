namespace LightZ.Models
{
    /// <summary>
    /// Defines a LED
    /// </summary>
    internal sealed class Led
    {
        #region Fields

        private byte _ledIndex;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color of the LED
        /// </summary>
        internal Color Color { get; set; }

        /// <summary>
        /// Gets or sets the number of LED light. Between -1 and IntMax. From 0 to 19 : the 20 LEDs of the led strip. -1 : All LEDs.
        /// </summary>
        internal byte ActionTarget
        {
            get
            {
                return _ledIndex;
            }
            set
            {
                _ledIndex = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="Led"/>
        /// </summary>
        public Led()
        {
            Color = new Color();
        }

        #endregion
    }
}
