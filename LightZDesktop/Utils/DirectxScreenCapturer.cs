namespace LightZDesktop.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using LightZPortableLibrary.Enums;
    using LightZPortableLibrary.Model;

    using SlimDX;
    using SlimDX.Direct3D9;
    using System.Diagnostics;

    internal class DirectxScreenCapturer
    {
        #region Fields

        private readonly Device _device;
        private readonly List<long> _led0Pos = new List<long>();
        private readonly List<long> _led1Pos = new List<long>();
        private readonly List<long> _led2Pos = new List<long>();
        private readonly List<long> _led3Pos = new List<long>();
        private readonly List<long> _led4Pos = new List<long>();
        private readonly List<long> _led5Pos = new List<long>();
        private readonly List<long> _led6Pos = new List<long>();
        private readonly List<long> _led7Pos = new List<long>();
        private readonly List<long> _led8Pos = new List<long>();
        private readonly List<long> _led9Pos = new List<long>();
        private readonly List<long> _led10Pos = new List<long>();
        private readonly List<long> _led11Pos = new List<long>();
        private readonly List<long> _led12Pos = new List<long>();
        private readonly List<long> _led13Pos = new List<long>();
        private readonly List<long> _led14Pos = new List<long>();
        private readonly List<long> _led15Pos = new List<long>();
        private readonly List<long> _led16Pos = new List<long>();
        private readonly List<long> _led17Pos = new List<long>();
        private readonly List<long> _led18Pos = new List<long>();
        private readonly List<long> _led19Pos = new List<long>();

        #endregion

        #region Consts

        private const int BytePerPixel = 4;
        private const int HorizontalLedCount = 6;
        private const int VerticalLedCount = 4;
        private const int Margin = 40;
        private const int Thickness = 20;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="DirectxScreenCapturer"/>
        /// </summary>
        public DirectxScreenCapturer()
        {
            this.CalculatePositions();

            var parameters = new PresentParameters();
            parameters.Windowed = true;
            parameters.SwapEffect = SwapEffect.Discard;
            this._device = new Device(new Direct3D(), 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing, parameters);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculate the positions to analyze on the screen
        /// </summary>
        private void CalculatePositions()
        {
            long x;
            long y;
            long pos;
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;
            var ledWidth = screenWidth / HorizontalLedCount;
            var ledHeight = screenHeight / VerticalLedCount;

            for (y = Margin; y < screenHeight - Margin; y++)
            {
                //led 0 to 3 (bottom right to top right)
                x = screenWidth - Margin - Thickness;
                for (x = x; x < screenWidth - Margin; x++)
                {
                    pos = (y * Screen.PrimaryScreen.Bounds.Width + x) * BytePerPixel;
                    if (y >= Margin && y <= ledHeight)
                        this._led3Pos.Add(pos);
                    else if (y >= ledHeight && y <= ledHeight * 2)
                        this._led2Pos.Add(pos);
                    else if (y >= ledHeight * 2 && y <= ledHeight * 3)
                        this._led1Pos.Add(pos);
                    else if (y >= ledHeight * 3 && y <= ledHeight * 4)
                        this._led0Pos.Add(pos);
                }
                //led 10 to 13 (top left to bottom left)
                for (x = Margin; x < Margin + Thickness; x++)
                {
                    pos = (y * Screen.PrimaryScreen.Bounds.Width + x) * BytePerPixel;
                    if (y >= Margin && y <= ledHeight)
                        this._led10Pos.Add(pos);
                    else if (y >= ledHeight && y <= ledHeight * 2)
                        this._led11Pos.Add(pos);
                    else if (y >= ledHeight * 2 && y <= ledHeight * 3)
                        this._led12Pos.Add(pos);
                    else if (y >= ledHeight * 3 && y <= ledHeight * 4)
                        this._led13Pos.Add(pos);
                }
            }

            for (x = Margin; x < screenWidth - Margin; x++)
            {
                //led 4 to 9 (top right to top left)
                for (y = Margin; y < Margin + Thickness; y++)
                {
                    pos = (y * Screen.PrimaryScreen.Bounds.Width + x) * BytePerPixel;
                    if (x >= Margin && x <= ledWidth)
                        this._led9Pos.Add(pos);
                    else if (x >= ledWidth && x <= ledWidth * 2)
                        this._led8Pos.Add(pos);
                    else if (x >= ledWidth * 2 && x <= ledWidth * 3)
                        this._led7Pos.Add(pos);
                    else if (x >= ledWidth * 3 && x <= ledWidth * 4)
                        this._led6Pos.Add(pos);
                    else if (x >= ledWidth * 4 && x <= ledWidth * 5)
                        this._led5Pos.Add(pos);
                    else if (x >= ledWidth * 5 && x <= ledWidth * 6)
                        this._led4Pos.Add(pos);
                }
                //led 14 to 19 (bottom right to bottom left)
                y = screenHeight - Margin - Thickness;
                for (y = y; y < screenHeight - Margin; y++)
                {
                    pos = (y * Screen.PrimaryScreen.Bounds.Width + x) * BytePerPixel;
                    if (x >= Margin && x <= ledWidth)
                        this._led14Pos.Add(pos);
                    else if (x >= ledWidth && x <= ledWidth * 2)
                        this._led15Pos.Add(pos);
                    else if (x >= ledWidth * 2 && x <= ledWidth * 3)
                        this._led16Pos.Add(pos);
                    else if (x >= ledWidth * 3 && x <= ledWidth * 4)
                        this._led17Pos.Add(pos);
                    else if (x >= ledWidth * 4 && x <= ledWidth * 5)
                        this._led18Pos.Add(pos);
                    else if (x >= ledWidth * 5 && x <= ledWidth * 6)
                        this._led19Pos.Add(pos);
                }
            }
        }

        /// <summary>
        /// Generates a value corresponding to a LED by analyzing the screen
        /// </summary>
        /// <param name="ledIndex">index of the LED</param>
        /// <param name="data">The entire byte array corresponding to each pixel of the screen</param>
        /// <param name="positions">The position on the screen to analyze</param>
        /// <returns>a LED</returns>
        private Led GenerateLed(Target ledIndex, DataStream data, List<long> positions)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var result = new Led();
            result.LedIndex = ledIndex;

            if (positions.Count == 0)
            {
                result.Color = new Color(0, 0, 0);
                return result;
            }

            var buffer = new byte[4];
            var red = 0;
            var green = 0;
            var blue = 0;
            var total = 0;

            // colors average
            foreach (var position in positions)
            {
                data.Position = position;
                data.Read(buffer, 0, 4);
                red += buffer[2];
                green += buffer[1];
                blue += buffer[0];
                total++;
            }

            red /= total;
            green /= total;
            blue /= total;

            // gamma correction
            red = (byte)(Math.Pow(red / 255.0, 2) * 127.0 + 0.5);
            green = (byte)(Math.Pow(green / 255.0, 2) * 127.0 + 0.5);
            blue = (byte)(Math.Pow(blue / 255.0, 2) * 127.0 + 0.5);

            result.Color = new Color((byte)red, (byte)green, (byte)blue);
            return result;
        }

        /// <summary>
        /// Generate all LEDs to display
        /// </summary>
        /// <returns>all the LEDs</returns>
        public List<List<Led>> GetLedsFromScreenCapture()
        {
            var result = new List<List<Led>>();

            var surface = Surface.CreateOffscreenPlain(this._device, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);
            try
            {
                this._device.GetFrontBufferData(0, surface);
                // throw an exception after a while (between 1 and 5min) without error code.
                var dataRectangle = surface.LockRectangle(LockFlags.None);
                var data = dataRectangle.Data;

                var led0 = this.GenerateLed(Target.Led1, data, this._led0Pos);
                var led1 = this.GenerateLed(Target.Led2, data, this._led1Pos);
                var led2 = this.GenerateLed(Target.Led3, data, this._led2Pos);
                var led3 = this.GenerateLed(Target.Led4, data, this._led3Pos);
                var led4 = this.GenerateLed(Target.Led5, data, this._led4Pos);
                var led5 = this.GenerateLed(Target.Led6, data, this._led5Pos);
                var led6 = this.GenerateLed(Target.Led7, data, this._led6Pos);
                var led7 = this.GenerateLed(Target.Led8, data, this._led7Pos);
                var led8 = this.GenerateLed(Target.Led9, data, this._led8Pos);
                var led9 = this.GenerateLed(Target.Led10, data, this._led9Pos);
                var led10 = this.GenerateLed(Target.Led11, data, this._led10Pos);
                var led11 = this.GenerateLed(Target.Led12, data, this._led11Pos);
                var led12 = this.GenerateLed(Target.Led13, data, this._led12Pos);
                var led13 = this.GenerateLed(Target.Led14, data, this._led13Pos);
                var led14 = this.GenerateLed(Target.Led15, data, this._led14Pos);
                var led15 = this.GenerateLed(Target.Led16, data, this._led15Pos);
                var led16 = this.GenerateLed(Target.Led17, data, this._led16Pos);
                var led17 = this.GenerateLed(Target.Led18, data, this._led17Pos);
                var led18 = this.GenerateLed(Target.Led19, data, this._led18Pos);
                var led19 = this.GenerateLed(Target.Led20, data, this._led19Pos);

                result.Add(new List<Led> { led0, led1, led2, led3, led4 });
                result.Add(new List<Led> { led5, led6, led7, led8, led9 });
                result.Add(new List<Led> { led10, led11, led12, led13, led14 });
                result.Add(new List<Led> { led15, led16, led17, led18, led19 });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetLedsFromScreenCapture ({0}) : {1}", DateTime.Now.ToShortTimeString(), ex.Message);
            }
            finally
            {
                surface.UnlockRectangle();
                surface.Dispose();
            }

            return result;
        }

        #endregion
    }
}
