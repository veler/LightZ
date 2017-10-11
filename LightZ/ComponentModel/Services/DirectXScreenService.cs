using LightZ.ComponentModel.Core;
using LightZ.ComponentModel.Enums;
using LightZ.ComponentModel.Services.Base;
using LightZ.Models;
using LightZ.Properties;
using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightZ.ComponentModel.Services
{
    /// <summary>
    /// Provides a set of functions designed to get the borders color of a monitor to send it to the Led strip.
    /// </summary>
    internal sealed class DirectXScreenService : IService
    {
        #region Fields

        private List<List<long>> _ledsPositions;
        private Direct3D _direct3D9;
        private Device _direct3D9Device;
        private AdapterInformation _adapter;
        private ScreenInfo _currentMonitor;
        private LedStripMonitorPosition _ledStripMonitorPosition;
        private int _horizontalLedCount;
        private int _verticalLedCount;
        private bool _initialized;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current monitor to apply the render.
        /// </summary>
        internal ScreenInfo CurrentMonitor
        {
            get
            {
                return _currentMonitor;
            }
            set
            {
                _currentMonitor = value;
                if (_initialized)
                {
                    CalculatePositions();
                    Reset();
                    InitializeDirectX();
                }
            }
        }

        /// <summary>
        /// Gets or sets the led strip position on the monitor
        /// </summary>
        internal LedStripMonitorPosition LedStripMonitorPosition
        {
            get
            {
                return _ledStripMonitorPosition;
            }
            set
            {
                _ledStripMonitorPosition = value;
                if (_initialized)
                {
                    CalculatePositions();
                    Reset();
                    InitializeDirectX();
                }
            }
        }

        /// <summary>
        /// Gets or sets the total number of horizontal Leds (top and bottom)
        /// </summary>
        internal int HorizontalLedCount
        {
            get
            {
                return _horizontalLedCount;
            }
            set
            {
                _horizontalLedCount = value;
                if (_initialized)
                {
                    CalculatePositions();
                    Reset();
                    InitializeDirectX();
                }
            }
        }

        /// <summary>
        /// Gets or sets the total number of vertical Leds (left and right)
        /// </summary>
        internal int VerticalLedCount
        {
            get
            {
                return _verticalLedCount;
            }
            set
            {
                _verticalLedCount = value;
                if (_initialized)
                {
                    CalculatePositions();
                }
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Initialize()
        {
            CurrentMonitor = SystemInfoHelper.GetAllScreenInfos().FirstOrDefault(device => device.DeviceId == Settings.Default.Monitor);
            HorizontalLedCount = Settings.Default.HorizontalLeds;
            VerticalLedCount = Settings.Default.VerticalLeds;
            LedStripMonitorPosition = (LedStripMonitorPosition)Settings.Default.LedStripMonitorPosition;

            InitializeDirectX();

            _initialized = true;

            CalculatePositions();
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (_direct3D9 != null)
            {
                _direct3D9.Dispose();
            }

            if (_direct3D9Device != null)
            {
                _direct3D9Device.Dispose();
            }
        }

        /// <summary>
        /// Generate all LEDs to display
        /// </summary>
        /// <returns>all the LEDs</returns>
        public List<List<Led>> GetLedsFromScreenCapture()
        {
            var result = new List<List<Led>>();

            if (CurrentMonitor == null)
            {
                return result;
            }

            var surface = Surface.CreateOffscreenPlain(_direct3D9Device, CurrentMonitor.Bounds.Right, CurrentMonitor.Bounds.Bottom, Format.A8R8G8B8, Pool.Scratch);
            _direct3D9Device.BeginScene();
            try
            {
                Requires.IsTrue((HorizontalLedCount + VerticalLedCount) % 2 == 0);
                Requires.IsTrue(HorizontalLedCount + VerticalLedCount < 255);

                _direct3D9Device.GetFrontBufferData(0, surface); // throw an exception after a while (between 1 and 5min) without error code.

                var dataRectangle = surface.LockRectangle(LockFlags.None);
                var data = dataRectangle.Data;
                var tempList = new List<Led>();

                // making group of 5 leds to send to the Arduino.
                for (var i = 0; i < _ledsPositions.Count; i++)
                {
                    if (tempList.Count == 5)
                    {
                        result.Add(tempList);
                        tempList = new List<Led>();
                    }

                    tempList.Add(GenerateLed((byte)(i + 4), data, _ledsPositions[i]));
                }
                result.Add(tempList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetLedsFromScreenCapture ({0}) : {1}", DateTime.Now.ToShortTimeString(), ex.Message);
            }
            finally
            {
                try
                {
                    surface.UnlockRectangle();
                }
                catch
                {
                    Reset();
                    InitializeDirectX();
                }
                surface.Dispose();
                _direct3D9Device.EndScene();
            }

            return result;
        }

        /// <summary>
        /// Generates a value corresponding to a <see cref="Led"/> by analyzing the screen
        /// </summary>
        /// <param name="ledIndex">index of the <see cref="Led"/></param>
        /// <param name="data">The entire byte array corresponding to each pixel of the screen</param>
        /// <param name="positions">The position on the screen to analyze</param>
        /// <returns>a <see cref="Led"/></returns>
        private Led GenerateLed(byte actionTarget, DataStream data, List<long> positions)
        {
            Requires.NotNull(data, nameof(data));

            var result = new Led();
            result.ActionTarget = actionTarget;

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
        /// Calculate the positions to analyze on the screen
        /// </summary>
        private void CalculatePositions()
        {
            if (CurrentMonitor == null)
            {
                return;
            }

            long x;
            long y;
            long pos;
            var screenWidth = CurrentMonitor.Bounds.Right;
            var screenHeight = CurrentMonitor.Bounds.Bottom;
            var horizontalLedBarCount = HorizontalLedCount / 2;
            var verticalLedBarCount = VerticalLedCount / 2;
            var ledWidth = screenWidth / horizontalLedBarCount;
            var ledHeight = screenHeight / verticalLedBarCount;

            // Creates an array of Leds that contains an array of Long for each led. Each number in this array of Long corresponds to a Pixel position in an array of pixels to analyze. One pixel has 4 bytes.
            Requires.IsTrue((HorizontalLedCount + VerticalLedCount) % 2 == 0);
            Requires.IsTrue(HorizontalLedCount + VerticalLedCount < 255);
            _ledsPositions = new List<List<long>>();
            for (var i = 0; i < HorizontalLedCount + VerticalLedCount; i++)
            {
                _ledsPositions.Add(new List<long>());
            }

            // The following algorithm retrieves the Pixel position of the pixels that are in the area that corresponds to a Led.

            // For all the height of the screen
            for (y = Consts.Margin; y < screenHeight - Consts.Margin; y++)
            {
                // Retrieves the leds from top right to bottom right
                for (x = screenWidth - Consts.Margin - Consts.Thickness; x < screenWidth - Consts.Margin; x++)
                {
                    pos = (y * screenWidth + x) * Consts.BytePerPixel;

                    for (var verticalLedIndex = VerticalLedCount / 2; verticalLedIndex > 0; verticalLedIndex--)
                    {
                        // If the current Y position is in the range of the N vertical Led, then we set the position to the Led.
                        if (y >= ledHeight * (verticalLedIndex - 1) && y <= ledHeight * verticalLedIndex)
                        {
                            var ledId = 0;

                            // We retrieve the Led index depending of how the led strip are positioned behind the monitor.
                            switch (LedStripMonitorPosition)
                            {
                                case LedStripMonitorPosition.BottomRight:
                                    ledId = verticalLedBarCount - verticalLedIndex;
                                    break;

                                case LedStripMonitorPosition.BottomLeft:
                                    ledId = verticalLedBarCount + horizontalLedBarCount + verticalLedIndex - 1;
                                    break;

                                case LedStripMonitorPosition.TopRight:
                                    ledId = verticalLedIndex - 1;
                                    break;

                                case LedStripMonitorPosition.TopLeft:
                                    ledId = verticalLedBarCount + horizontalLedBarCount + verticalLedBarCount - verticalLedIndex;
                                    break;
                            }

                            _ledsPositions[ledId].Add(pos);
                        }
                    }
                }

                // Retrieves the leds from top left to bottom left
                for (x = Consts.Margin; x < Consts.Margin + Consts.Thickness; x++)
                {
                    pos = (y * screenWidth + x) * Consts.BytePerPixel;

                    for (var verticalLedIndex = VerticalLedCount / 2; verticalLedIndex > 0; verticalLedIndex--)
                    {
                        // If the current Y position is in the range of the N vertical Led, then we set the position to the Led.
                        if (y >= ledHeight * (verticalLedIndex - 1) && y <= ledHeight * verticalLedIndex)
                        {
                            var ledId = 0;

                            // We retrieve the Led index depending of how the led strip are positioned behind the monitor.
                            switch (LedStripMonitorPosition)
                            {
                                case LedStripMonitorPosition.BottomRight:
                                    ledId = verticalLedBarCount + horizontalLedBarCount + verticalLedIndex - 1;
                                    break;

                                case LedStripMonitorPosition.BottomLeft:
                                    ledId = verticalLedBarCount - verticalLedIndex;
                                    break;

                                case LedStripMonitorPosition.TopRight:
                                    ledId = VerticalLedCount + horizontalLedBarCount - verticalLedIndex;
                                    break;

                                case LedStripMonitorPosition.TopLeft:
                                    ledId = verticalLedIndex - 1;
                                    break;
                            }

                            _ledsPositions[ledId].Add(pos);
                        }
                    }
                }
            }


            // For all the width of the screen
            for (x = Consts.Margin; x < screenWidth - Consts.Margin; x++)
            {
                // Retrieves the leds from top left to top right
                for (y = Consts.Margin; y < Consts.Margin + Consts.Thickness; y++)
                {
                    pos = (y * screenWidth + x) * Consts.BytePerPixel;

                    for (var horizontalLedIndex = HorizontalLedCount / 2; horizontalLedIndex > 0; horizontalLedIndex--)
                    {
                        // If the current X position is in the range of the N horizontal Led, then we set the position to the Led.
                        if (x >= ledWidth * (horizontalLedIndex - 1) && x <= ledWidth * horizontalLedIndex)
                        {
                            var ledId = 0;

                            // We retrieve the Led index depending of how the led strip are positioned behind the monitor.
                            switch (LedStripMonitorPosition)
                            {
                                case LedStripMonitorPosition.BottomRight:
                                    ledId = horizontalLedBarCount + verticalLedBarCount - horizontalLedIndex;
                                    break;

                                case LedStripMonitorPosition.BottomLeft:
                                    ledId = verticalLedBarCount + horizontalLedIndex - 1;
                                    break;

                                case LedStripMonitorPosition.TopRight:
                                    ledId = VerticalLedCount + horizontalLedBarCount + horizontalLedIndex - 1;
                                    break;

                                case LedStripMonitorPosition.TopLeft:
                                    ledId = VerticalLedCount + HorizontalLedCount - horizontalLedIndex;
                                    break;
                            }

                            _ledsPositions[ledId].Add(pos);
                        }
                    }
                }

                // Retrieves the leds from bottom left to bottom right
                for (y = screenHeight - Consts.Margin - Consts.Thickness; y < screenHeight - Consts.Margin; y++)
                {
                    pos = (y * screenWidth + x) * Consts.BytePerPixel;

                    for (var horizontalLedIndex = HorizontalLedCount / 2; horizontalLedIndex > 0; horizontalLedIndex--)
                    {
                        // If the current X position is in the range of the N horizontal Led, then we set the position to the Led.
                        if (x >= ledWidth * (horizontalLedIndex - 1) && x <= ledWidth * horizontalLedIndex)
                        {
                            var ledId = 0;

                            // We retrieve the Led index depending of how the led strip are positioned behind the monitor.
                            switch (LedStripMonitorPosition)
                            {
                                case LedStripMonitorPosition.BottomRight:
                                    ledId = VerticalLedCount + horizontalLedBarCount + horizontalLedIndex - 1;
                                    break;

                                case LedStripMonitorPosition.BottomLeft:
                                    ledId = VerticalLedCount + HorizontalLedCount - horizontalLedIndex;
                                    break;

                                case LedStripMonitorPosition.TopRight:
                                    ledId = horizontalLedBarCount + verticalLedBarCount - horizontalLedIndex;
                                    break;

                                case LedStripMonitorPosition.TopLeft:
                                    ledId = verticalLedBarCount + horizontalLedIndex - 1;
                                    break;
                            }

                            _ledsPositions[ledId].Add(pos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the DirectX instance via SlimDX.
        /// </summary>
        private void InitializeDirectX()
        {
            _direct3D9 = new Direct3D();

            _adapter = _direct3D9.Adapters.DefaultAdapter;
            if (CurrentMonitor != null)
            {
                _adapter = _direct3D9.Adapters.FirstOrDefault(adapter => adapter.Details.DeviceName == CurrentMonitor.DeviceId);
            }

            var parameters = new PresentParameters();
            parameters.Windowed = true;
            parameters.SwapEffect = SwapEffect.Discard;
            parameters.BackBufferCount = 1;
            parameters.DeviceWindowHandle = IntPtr.Zero;
            parameters.BackBufferFormat = _adapter.CurrentDisplayMode.Format;
            parameters.BackBufferWidth = _adapter.CurrentDisplayMode.Width;
            parameters.BackBufferHeight = _adapter.CurrentDisplayMode.Height;

            _direct3D9Device = new Device(_direct3D9, _adapter.Adapter, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, parameters);
        }

        #endregion
    }
}
