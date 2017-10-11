using CoreAudioApi;
using LightZ.ComponentModel.Core;
using LightZ.ComponentModel.Services.Base;
using LightZ.Models;
using LightZ.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace LightZ.ComponentModel.Services
{
    /// <summary>
    /// Provides a set of functions designed to listen to an sound card an retrieves the list of audio devices.
    /// </summary>
    internal sealed class AudioService : IService, IPausable
    {
        #region Fields

        private float[] _fftDataBuffer;
        private int _lastOutputLevel;
        private int _lastOutputLevelCounter;
        private WASAPIPROC _wasapiProcessCallback;
        private AudioDevice _currentAudioDevice;
        private MMDevice _mmAudioDevice;

        private bool _inPause;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current audio device to use
        /// </summary>
        internal AudioDevice CurrentAudioDevice
        {
            get
            {
                return _currentAudioDevice;
            }
            set
            {
                if (value != null && _currentAudioDevice != null && _currentAudioDevice.DeviceId == value.DeviceId)
                {
                    return;
                }
                _currentAudioDevice = value;

                if (_currentAudioDevice != null)
                {
                    Reset();
                }
            }
        }

        /// <summary>
        /// Gets a value that defines whether the service is in pause or not.
        /// </summary>
        internal bool IsPaused => _inPause;

        #endregion

        #region Handled Methods

        /// <summary>
        /// User defined WASAPI output/input processing callback function (to be used with BASS_WASAPI_Init(Int32, Int32, Int32, BASSWASAPIInit, Single, Single, WASAPIPROC, IntPtr)). 
        /// </summary>
        /// <param name="buffer">Pointer to the buffer to put the sample data for an output device, or to get the data from an input device. The sample data is always 32-bit floating-point.</param>
        /// <param name="length">The number of bytes to process.</param>
        /// <param name="user">The user instance data given when BASS_WASAPI_Init(Int32, Int32, Int32, BASSWASAPIInit, Single, Single, WASAPIPROC, IntPtr) was called.</param>
        /// <returns>In the case of an output device, the number of bytes written to the buffer. In the case of an input device, 0 = stop the device, else continue.</returns>
        private int WasapiProcessCallBack(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Initialize()
        {
            _inPause = true;

            _fftDataBuffer = new float[1024];
            _wasapiProcessCallback = new WASAPIPROC(WasapiProcessCallBack);

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            var devEnum = new MMDeviceEnumerator();
            _mmAudioDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            if (!string.IsNullOrWhiteSpace(Settings.Default.AudioDevice))
            {
                CurrentAudioDevice = GetAudioDevices().FirstOrDefault(device => device.DeviceName == Settings.Default.AudioDevice);
            }

            Resume();
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Pause();
            Resume();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            CurrentAudioDevice = null;
            Pause();
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            if (!_inPause)
            {
                _inPause = true;

                if (BassWasapi.BASS_WASAPI_IsStarted())
                {
                    BassWasapi.BASS_WASAPI_Stop(true);
                }
            }
        }

        /// <inheritdoc/>
        public void Resume()
        {
            if (_inPause)
            {
                BassWasapi.BASS_WASAPI_Free();
                Bass.BASS_Free();
                Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

                if (CurrentAudioDevice != null)
                {
                    BassWasapi.BASS_WASAPI_Init(CurrentAudioDevice.DeviceId, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _wasapiProcessCallback, IntPtr.Zero);
                    if (!BassWasapi.BASS_WASAPI_IsStarted())
                    {
                        BassWasapi.BASS_WASAPI_Start();
                        _inPause = false;
                    }
                }
            }
        }

        /// <summary>
        /// Retireves the list of sound card connected to the computer.
        /// </summary>
        /// <returns>The list of audio devices</returns>
        internal List<AudioDevice> GetAudioDevices()
        {
            var result = new List<AudioDevice>();
            for (var i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    result.Add(new AudioDevice(i, device.name));
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves audio data from the device, makes an average of the bass, rounded to the Left and Right levels and returns the result.
        /// </summary>
        /// <returns>Data dictionary containing the rounded left and right data</returns>
        internal Dictionary<string, byte> GetBassAverage()
        {
            if (_inPause)
            {
                return null;
            }

            float volume = 0;

            if (!CoreHelper.IsUnitTesting())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    volume = -(_mmAudioDevice.AudioEndpointVolume.MasterVolumeLevelScalar - 1.1f); // The higher the volume of the PC is, the lower the values returned by BASS are. A calculation based on the volume of the PC can counter this.
                }, DispatcherPriority.ApplicationIdle);
            }
            else
            {
                volume = -(_mmAudioDevice.AudioEndpointVolume.MasterVolumeLevelScalar - 1.1f); // The higher the volume of the PC is, the lower the values returned by BASS are. A calculation based on the volume of the PC can counter this.
            }

            var dataCount = BassWasapi.BASS_WASAPI_GetData(_fftDataBuffer, (int)BASSData.BASS_DATA_FFT2048);
            if (dataCount < -1)
            {
                return null;
            }

            int j;
            int b1;
            float peak;
            var b0 = 0;
            var average = 0;

            for (var i = Consts.AudioSpectrumBassLines[0]; i < Consts.AudioSpectrumBassLines[1]; i++)
            {
                peak = 0;
                b1 = (int)Math.Pow(2, i * 10.0 / (Consts.MaxAudioSpectrumLine - 1));
                if (b1 > 1023)
                {
                    b1 = 1023;
                }

                if (b1 <= b0)
                {
                    b1 = b0 + 1;
                }

                for (; b0 < b1; b0++)
                {
                    if (peak < _fftDataBuffer[1 + b0])
                    {
                        peak = _fftDataBuffer[1 + b0] / volume;
                    }
                }

                j = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (j > 255)
                {
                    j = 255;
                }

                if (j < 0)
                {
                    j = 0;
                }

                average += (byte)j;
            }

            average /= Consts.AudioSpectrumBassLines[1];

            var level = BassWasapi.BASS_WASAPI_GetLevel();
            var left = ((double)Utils.LowWord32(level) - 0) / ((double)ushort.MaxValue - 0) * (255 - 0) + 0;
            var right = ((double)Utils.HighWord32(level) - 0) / ((double)ushort.MaxValue - 0) * (255 - 0) + 0;
            left = average * (1 - (1 / (1 + left)));
            right = average * (1 - (1 / (1 + right)));

            var result = new Dictionary<string, byte>
            {
                { "left", (byte)left },
                { "right", (byte)right }
            };

            if (level == _lastOutputLevel && level != 0)
            {
                _lastOutputLevelCounter++;
            }

            _lastOutputLevel = level;

            if (_lastOutputLevelCounter > 3) // regularly, the API can no longer recover the spectrum and should be reset ... as the official documentation says
            {
                _lastOutputLevelCounter = 0;
                Reset();
            }

            return result;
        }

        #endregion
    }
}
