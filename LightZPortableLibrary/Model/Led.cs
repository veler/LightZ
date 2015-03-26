namespace LightZPortableLibrary.Model
{
    using System;

    using GalaSoft.MvvmLight;

    using LightZPortableLibrary.Enums;

    /// <summary>
    /// Defines an LED
    /// </summary>
    public class Led : ObservableObject
    {
        #region Field

        private Color _color;
        private Target _ledIndex;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color of the LED
        /// </summary>
        public Color Color
        {
            get { return this._color; }
            set
            {
                this._color = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the number of LED light. Between -1 and 19. From 0 to 19 : the 20 LEDs of the led strip. -1 : All LEDs.
        /// </summary>
        public Target LedIndex
        {
            get { return this._ledIndex; }
            set
            {
                if (value < (Target)2 || value > (Target)26)
                    throw new IndexOutOfRangeException("LedIndex doit être compris entre 2 et 26");
                this._ledIndex = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="Led"/>
        /// </summary>
        public Led()
        {
            this.Color = new Color();
        }

        #endregion
    }
}
