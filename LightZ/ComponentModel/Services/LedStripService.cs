using LightZ.ComponentModel.Core;
using LightZ.ComponentModel.Enums;
using LightZ.ComponentModel.Interop;
using LightZ.ComponentModel.Services.Base;
using LightZ.Models;
using LightZ.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightZ.ComponentModel.Services
{
    /// <summary>
    /// Provides a set of functions designed to manage the led strip behavior based on the other services and user settings.
    /// </summary>
    internal sealed class LedStripService : IService, IPausable
    {
        #region Fields

        private AsyncBackgroundWorker _worker;
        private EventWaitHandle _workerEventWaitHandle;
        private LedStripMode _mode;
        private Color _manualColor;
        private byte _manualBrightness;
        private bool _modeChanged;
        private bool _manualColorChanged;
        private bool _manualBrightnessChanged;
        private bool _pauseExpected;
        private bool _inPause;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Arduino mode.
        /// </summary>
        internal LedStripMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                _modeChanged = true;
                _workerEventWaitHandle.Set();
            }
        }

        /// <summary>
        /// Gets the overall color.
        /// </summary>
        internal Color ManualColor
        {
            get
            {
                return _manualColor;
            }
            set
            {
                _manualColor = value;
                _manualColorChanged = true;
                _workerEventWaitHandle.Set();
            }
        }

        /// <summary>
        /// Gets the overall contrast.
        /// </summary>
        public byte ManualBrightness
        {
            get
            {
                return _manualBrightness;
            }
            set
            {
                _manualBrightness = value;
                _manualBrightnessChanged = true;
                _workerEventWaitHandle.Set();
            }
        }

        #endregion

        #region Handled Methods

        private void SerialService_DataReceived(object sender, Events.SerialDataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        private void LedStripService_ConnectionStateChanged(object sender, EventArgs e)
        {
            var serialService = ServiceLocator.GetService<SerialService>();
            if (serialService.Connected)
            {
                _workerEventWaitHandle.Set();
                _worker.Resume();
            }
            else
            {
                _pauseExpected = true;
                _workerEventWaitHandle.Set();
                _worker.Pause();
            }
        }

        private void Worker_WorkerPaused(object sender, Events.AsyncBackgroundWorkerEndedEventArgs e)
        {
            if (e.Exception != null)
            {
                Debug.WriteLine(e.Exception.Message);
            }

            Reset();
        }

        private void Worker_DoWork(object sender, EventArgs e)
        {
            var serialService = ServiceLocator.GetService<SerialService>();

            if (!serialService.Connected)
            {
                _pauseExpected = true;
                _worker.Pause();
                return;
            }

            if (_modeChanged)
            {
                _modeChanged = false;

                if (Mode == LedStripMode.MonitorColors)
                {
                    NativeMethods.SetThreadExecutionState(ExecutionState.ES_CONTINUOUS | ExecutionState.ES_SYSTEM_REQUIRED | ExecutionState.ES_AWAYMODE_REQUIRED);
                }
                else
                {
                    NativeMethods.SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
                }

                serialService.Send(GenerateModeQuery(Mode));

                if (Mode == LedStripMode.Manual)
                {
                    _manualColorChanged = true; // force to update.
                }

                var audioService = ServiceLocator.GetService<AudioService>();
                if (Mode == LedStripMode.AudioSpectrum)
                {
                    audioService.Resume();
                }
                else
                {
                    audioService.Pause();
                }
            }

            _workerEventWaitHandle.Reset();

            switch (Mode)
            {
                case LedStripMode.Off:
                    _workerEventWaitHandle.WaitOne();
                    break;

                case LedStripMode.Manual:
                    if (_manualColorChanged || _manualBrightnessChanged)
                    {
                        _manualColorChanged = false;
                        _manualBrightnessChanged = false;
                        ShowColor();
                    }

                    _workerEventWaitHandle.WaitOne();
                    break;

                case LedStripMode.MonitorColors:
                    var leds = ServiceLocator.GetService<DirectXScreenService>().GetLedsFromScreenCapture();
                    foreach (var ledsPart in leds) // small packets are sent to avoid saturating the Bluetooth or USB serial port
                    {
                        serialService.Send(GenerateLedQuery(ledsPart));
                        _workerEventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(10));
                    }
                    break;

                case LedStripMode.AudioSpectrum:
                    _workerEventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(20)); // to be "about" a frequency of 40Hz
                    var audioService = ServiceLocator.GetService<AudioService>();
                    if (!audioService.IsPaused)
                    {
                        var levels = audioService.GetBassAverage();
                        serialService.Send(GenerateSoundQuery(levels));
                    }
                    break;
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Initialize()
        {
            _inPause = true;

            _workerEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, Guid.NewGuid().ToString());

            Mode = (LedStripMode)Settings.Default.LedStripMode;
            ManualColor = JsonConvert.DeserializeObject<Color>(Settings.Default.ManualColor);
            ManualBrightness = Settings.Default.ManualBrightness;

            _worker = new AsyncBackgroundWorker();
            _worker.WorkInLoop = true;
            _worker.DoWork += Worker_DoWork;
            _worker.WorkerPaused += Worker_WorkerPaused;

            var serialService = ServiceLocator.GetService<SerialService>();
            serialService.ConnectionStateChanged += LedStripService_ConnectionStateChanged;
            serialService.DataReceived += SerialService_DataReceived;

            Resume();
        }

        /// <inheritdoc/>
        public async void Reset()
        {
            Pause();
            await Task.Delay(2000);
            Resume();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Pause();
            ServiceLocator.GetService<DirectXScreenService>().Reset();
            ServiceLocator.GetService<AudioService>().Dispose();
            ServiceLocator.GetService<SerialService>().Reset();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            if (!_inPause)
            {
                _inPause = true;

                var serialService = ServiceLocator.GetService<SerialService>();
                if (serialService.Connected && Mode != LedStripMode.Off)
                {
                    var led = new Led();
                    led.ActionTarget = (byte)LedStripActionTarget.AllLeds;
                    led.Color = new Color(0, 0, 0);

                    serialService.Send(GenerateLedQuery(led));
                }

                serialService.Disconnect();

                var audioService = ServiceLocator.GetService<AudioService>();
                audioService.Pause();
            }
        }

        /// <inheritdoc/>
        public void Resume()
        {
            if (_inPause)
            {
                _inPause = false;

                var audioService = ServiceLocator.GetService<AudioService>();
                audioService.Resume();

                var serialService = ServiceLocator.GetService<SerialService>();
                if (!serialService.Connected)
                {
                    if (serialService.CurrentSerialDevice != null)
                    {
                        serialService.Connect(serialService.CurrentSerialDevice.ComPortNumber);
                    }
                }
            }
        }

        /// <summary>
        /// Display a color for all the leds.
        /// </summary>
        private void ShowColor()
        {
            var led = new Led();
            led.ActionTarget = (byte)LedStripActionTarget.AllLeds;
            led.Color = ManualColor;

            var serialService = ServiceLocator.GetService<SerialService>();
            serialService.Send(GenerateBrightnessQuery(ManualBrightness));
            serialService.Send(GenerateLedQuery(led));
        }

        /// <summary>
        /// Generates a query to ask the Arduino to switch modes
        /// </summary>
        /// <param name="mode">The desired mode</param>
        /// <returns>The generated query</returns>
        private byte[] GenerateModeQuery(LedStripMode mode)
        {
            return new[] { (byte)LedStripActionTarget.Mode, (byte)mode, (byte)1, (byte)1 };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the overall contrast of the led strip
        /// </summary>
        /// <param name="brightness">The desired contrast</param>
        /// <returns>The generated query</returns>
        private byte[] GenerateBrightnessQuery(byte brightness)
        {
            return new[] { (byte)LedStripActionTarget.Brightness, brightness, (byte)1, (byte)1 };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the led strip based on sound detected
        /// </summary>
        /// <param name="levels">The left and right sound levels</param>
        /// <returns>The generated query</returns>
        private byte[] GenerateSoundQuery(Dictionary<string, byte> levels)
        {
            return new[] { (byte)LedStripActionTarget.Audio, levels["left"], levels["right"], (byte)1 };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the color of an LED
        /// </summary>
        /// <param name="led">LED with desired color</param>
        /// <returns>The generated query</returns>
        private byte[] GenerateLedQuery(Led led)
        {
            if (led.Color == null)
            {
                led.Color = new Color(255, 255, 255);
            }

            return new[] { (byte)led.ActionTarget, led.Color.Red, led.Color.Green, led.Color.Blue };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the color of several LEDs
        /// </summary>
        /// <param name="leds">The list of desired colors with LED</param>
        /// <returns>The generated query</returns>
        private byte[] GenerateLedQuery(List<Led> leds)
        {
            IEnumerable<byte> result = new List<byte>();

            foreach (var led in leds)
            {
                result = result.Concat(GenerateLedQuery(led));
            }

            return result.ToArray();
        }

        #endregion
    }
}
