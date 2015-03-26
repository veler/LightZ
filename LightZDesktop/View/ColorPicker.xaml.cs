namespace LightZDesktop.View
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Color = LightZPortableLibrary.Model.Color;

    /// <summary>
    /// A simple WPF color picker.  The basic idea is to use a Color swatch image and then pick out a single
    /// pixel and use that pixel's RGB values along with the Alpha slider to form a SelectedColor.
    /// 
    /// This class is from Sacha Barber at http://sachabarber.net/?p=424 and http://www.codeproject.com/KB/WPF/WPFColorPicker.aspx.
    /// 
    /// This class borrows an idea or two from the following sources:
    ///  - AlphaSlider and Preview box; Based on an article by ShawnVN's Blog; 
    ///    http://weblogs.asp.net/savanness/archive/2006/12/05/colorcomb-yet-another-color-picker-dialog-for-wpf.aspx.
    ///  - 1*1 pixel copy; Based on an article by Lee Brimelow; http://thewpfblog.com/?p=62.
    /// 
    /// Enhanced by Mark Treadwell (1/2/10):
    ///  - Left click to select the color with no mouse move
    ///  - Set tab behavior
    ///  - Set an initial color (note that the search to set the cursor ellipse delays the initial display)
    ///  - Fix single digit hex displays
    ///  - Add Mouse Wheel support to change the Alpha value
    ///  - Modify color select dragging behavior
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        #region Data

        private DrawingAttributes drawingAttributes = new DrawingAttributes();
        // private Color selectedColor = Colors.Transparent;
        private Boolean IsMouseDown = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor that initializes the ColorPicker to Black.
        /// </summary>
        public ColorPicker()
            : this(new Color(0, 0, 0))
        { }

        /// <summary>
        /// Constructor that initializes to ColorPicker to the specified color.
        /// </summary>
        /// <param name="initialColor"></param>
        public ColorPicker(Color initialColor)
        {
            this.InitializeComponent();
            this.SelectedColor = initialColor;
            this.SelectedBrightness = 255;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The SelectedColor property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new UIPropertyMetadata(SelectedColorPropertyChangedCallback));

        /// <summary>
        /// Gets or privately sets the Selected Color.
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return (Color)this.GetValue(SelectedColorProperty);
            }
            set
            {
                if (this.SelectedColor != value)
                {
                    this.SetValue(SelectedColorProperty, value);
                    this.CreateAlphaLinearBrush();
                }
            }
        }

        /// <summary>
        /// The SelectedBrightness property.
        /// </summary>
        public static readonly DependencyProperty SelectedBrightnessProperty = DependencyProperty.Register("SelectedBrightness", typeof(byte), typeof(ColorPicker), new UIPropertyMetadata(SelectedBrightnessPropertyChangedCallback));
        /// <summary>
        /// Gets or privately sets the Selected Color.
        /// </summary>
        public byte SelectedBrightness
        {
            get
            {
                return (byte)this.GetValue(SelectedBrightnessProperty);
            }
            set
            {
                if (this.SelectedBrightness != value)
                    this.SetValue(SelectedBrightnessProperty, value);
            }
        }

        #endregion

        #region Control Events

        /// <summary>
        /// 
        /// </summary>
        private void AlphaSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int change = e.Delta / Math.Abs(e.Delta);
            this.AlphaSlider.Value = this.AlphaSlider.Value + (double)change;
        }

        /// <summary>
        /// Update SelectedColor Alpha based on Slider value.
        /// </summary>
        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SelectedBrightness = (byte)this.AlphaSlider.Value;
            this.SelectedColor = new Color(this.SelectedColor.Red, this.SelectedColor.Green, this.SelectedColor.Blue);
        }

        /// <summary>
        /// Update the SelectedColor if moving the mouse with the left button down.
        /// </summary>
        private void CanvasImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseDown) this.UpdateColor();
        }

        /// <summary>
        /// Handle MouseDown event.
        /// </summary>
        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
            this.UpdateColor();
        }

        /// <summary>
        /// Handle MouseUp event.
        /// </summary>
        private void CanvasImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = false;
            //UpdateColor();
        }


        #endregion // Control Events

        #region Private Methods

        private static void SelectedColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var appbar = (ColorPicker)d;
            if ((e.NewValue != null && e.OldValue != null && ((Color)e.NewValue).Equals((Color)e.OldValue)) || appbar.IsMouseDown)
                return;

            appbar.CreateAlphaLinearBrush();
            appbar.UpdateCursorEllipse((Color)e.NewValue);
        }

        private static void SelectedBrightnessPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((byte)e.NewValue == (byte)e.OldValue)
                return;

            var appbar = (ColorPicker)d;
            if (appbar.SelectedColor == null)
            {
                appbar.SelectedColor = new Color(0, 0, 0);
                appbar.CreateAlphaLinearBrush();
            }
            //   appbar.AlphaSlider.Value = (byte)e.NewValue;
        }

        /// <summary>
        /// Creates a new LinearGradientBrush background for the Alpha area slider.  This is based on the current color.
        /// </summary>
        private void CreateAlphaLinearBrush()
        {
            if (this.SelectedColor == null) 
                return;
            var startColor = System.Windows.Media.Color.FromArgb((byte)0, this.SelectedColor.Red, this.SelectedColor.Green, this.SelectedColor.Blue);
            var endColor = System.Windows.Media.Color.FromArgb((byte)255, this.SelectedColor.Red, this.SelectedColor.Green, this.SelectedColor.Blue);
            var alphaBrush = new LinearGradientBrush(startColor, endColor, new Point(0, 0), new Point(1, 0));
            this.AlphaBorder.Background = alphaBrush;
        }

        /// <summary>
        /// Sets a new Selected Color based on the color of the pixel under the mouse pointer.
        /// </summary>
        private void UpdateColor()
        {
            // Test to ensure we do not get bad mouse positions along the edges
            int imageX = (int)Mouse.GetPosition(this.canvasImage).X;
            int imageY = (int)Mouse.GetPosition(this.canvasImage).Y;
            if ((imageX < 0) || (imageY < 0) || (imageX > this.ColorImage.Width - 1) || (imageY > this.ColorImage.Height - 1)) return;
            // Get the single pixel under the mouse into a bitmap and copy it to a byte array
            CroppedBitmap cb = new CroppedBitmap(this.ColorImage.Source as BitmapSource, new Int32Rect(imageX, imageY, 1, 1));
            byte[] pixels = new byte[4];
            cb.CopyPixels(pixels, 4, 0);
            // Update the mouse cursor position and the Selected Color
            this.ellipsePixel.SetValue(Canvas.LeftProperty, (double)(Mouse.GetPosition(this.canvasImage).X - (this.ellipsePixel.Width / 2.0)));
            this.ellipsePixel.SetValue(Canvas.TopProperty, (double)(Mouse.GetPosition(this.canvasImage).Y - (this.ellipsePixel.Width / 2.0)));
            this.canvasImage.InvalidateVisual();
            // Set the Selected Color based on the cursor pixel and Alpha Slider value
            this.SelectedColor = new Color(pixels[2], pixels[1], pixels[0]);
            this.SelectedBrightness = (byte)this.AlphaSlider.Value;
        }

        /// <summary>
        /// Update the mouse cursor ellipse position.
        /// </summary>
        private void UpdateCursorEllipse(Color searchColor)
        {
            // Scan the canvas image for a color which matches the search color
            CroppedBitmap cb;
            Color tempColor = new Color();
            byte[] pixels = new byte[4];
            int searchY = 0;
            int searchX = 0;
            for (searchY = 0; searchY <= this.canvasImage.Width - 1; searchY++)
            {
                for (searchX = 0; searchX <= this.canvasImage.Height - 1; searchX++)
                {
                    cb = new CroppedBitmap(this.ColorImage.Source as BitmapSource, new Int32Rect(searchX, searchY, 1, 1));
                    cb.CopyPixels(pixels, 4, 0);
                    tempColor = new Color(pixels[2], pixels[1], pixels[0]);
                    if (tempColor == searchColor) break;
                }
                if (tempColor == searchColor) break;
            }
            // Default to the top left if no match is found
            if (tempColor != searchColor)
            {
                searchX = 0;
                searchY = 0;
            }
            // Update the mouse cursor ellipse position
            this.ellipsePixel.SetValue(Canvas.LeftProperty, ((double)searchX - (this.ellipsePixel.Width / 2.0)));
            this.ellipsePixel.SetValue(Canvas.TopProperty, ((double)searchY - (this.ellipsePixel.Width / 2.0)));
        }

        #endregion // Update Methods

    }
}
