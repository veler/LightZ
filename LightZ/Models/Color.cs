using GalaSoft.MvvmLight;
using System;

namespace LightZ.Models
{
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
            get { return _red; }
            set
            {
                _red = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value of the green color
        /// </summary>
        public byte Green
        {
            get { return _green; }
            set
            {
                _green = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value of the blue color
        /// </summary>
        public byte Blue
        {
            get { return _blue; }
            set
            {
                _blue = value;
                RaisePropertyChanged();
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
            Red = red;
            Green = green;
            Blue = blue;
        }

        #endregion
    }
}
