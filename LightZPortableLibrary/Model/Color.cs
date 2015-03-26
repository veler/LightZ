namespace LightZPortableLibrary.Model
{
    using System;

    using GalaSoft.MvvmLight;

    /// <summary>
    /// Represents a color
    /// </summary>
    [Serializable]
    public class Color : ObservableObject
    {
        #region Field

        private byte _red;
        private byte _green;
        private byte _blue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value of the red color
        /// </summary>
        public byte Red
        {
            get { return this._red; }
            set
            {
                this._red = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value of the green color
        /// </summary>
        public byte Green
        {
            get { return this._green; }
            set
            {
                this._green = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value of the blue color
        /// </summary>
        public byte Blue
        {
            get { return this._blue; }
            set
            {
                this._blue = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="Color"/>
        /// </summary>
        public Color()
        {
        }

        /// <summary>
        /// Initializes an instance of the class <see cref="Color"/>
        /// </summary>
        /// <param name="red">amount of red</param>
        /// <param name="green">amount of green</param>
        /// <param name="blue">amount of blue</param>
        public Color(byte red, byte green, byte blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Color)) 
                return false;
            var color = (Color)obj;
            return color.Red == this.Red && color.Green == this.Green && color.Blue == this.Blue;
        }

        #endregion
    }
}
