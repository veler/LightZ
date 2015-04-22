namespace LightZDesktop.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using CoreAudioApi;

    using LightZPortableLibrary.Model;

    using Un4seen.Bass;
    using Un4seen.BassWasapi;

    internal class AudioAnalyzer : IDisposable
    {
        #region Consts

        private const int MaxLines = 16;

        /// <summary>
        /// The samples corresponding to the bass are the first 4.
        /// </summary>
        private static readonly int[] BassLines = { 0, 4 };

        #endregion

        #region Fields

        private readonly float[] _fftDataBuffer;
        private int _lastOutputLevel;
        private int _lastOutputLevelCounter;
        private bool _listening;
        private WASAPIPROC _wasapiProcessCallback;
        private AudioDevice _currentAudioDevice;
        private MMDevice _mmAudioDevice;

        #endregion

        #region Properties

        /// <summary>
        /// Get the list of audio devices
        /// </summary>
        public List<AudioDevice> AudioDevices
        {
            get
            {
                var result = new List<AudioDevice>();
                for (var i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
                {
                    var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                    if (device.IsEnabled && device.IsLoopback)
                        result.Add(new AudioDevice(i, device.name));
                }
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the current audio device to use
        /// </summary>
        public AudioDevice CurrentAudioDevice
        {
            get
            {
                return this._currentAudioDevice;
            }
            set
            {
                if (value != null && this._currentAudioDevice != null && this._currentAudioDevice.DeviceId == value.DeviceId)
                    return;
                this._currentAudioDevice = value;

                if (value != null)
                    BassWasapi.BASS_WASAPI_Init(this._currentAudioDevice.DeviceId, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, this._wasapiProcessCallback, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Gets or sets if the current audio device is listened
        /// </summary>
        public bool Listening
        {
            get
            {
                return this._listening;
            }
            set
            {
                if (this.CurrentAudioDevice == null)
                {
                    this._listening = false;
                    if (BassWasapi.BASS_WASAPI_IsStarted())
                        BassWasapi.BASS_WASAPI_Stop(true);
                    return;
                }
                if (this._listening != value)
                {
                    this._listening = value;
                    if (value)
                        BassWasapi.BASS_WASAPI_Start();
                    else
                        BassWasapi.BASS_WASAPI_Stop(true);
                    Thread.Sleep(500);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="AudioAnalyzer"/>
        /// </summary>
        public AudioAnalyzer()
        {
            this._fftDataBuffer = new float[1024];
            this._wasapiProcessCallback = new WASAPIPROC(this.WasapiProcessCallBack);

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            var devEnum = new MMDeviceEnumerator();
            this._mmAudioDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            this._mmAudioDevice.AudioEndpointVolume.OnVolumeNotification += this.AudioEndpointVolume_OnVolumeNotification;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves audio data from the device, makes an average of the bass, rounded to the Left and Right levels and returns the result.
        /// </summary>
        /// <returns>Data dictionary containing the rounded left and right data</returns>
        public Dictionary<string, byte> AnalyzeBassAverage()
        {
            var result = new Dictionary<string, byte>();

            if (!this.Listening)
                return null;

            var volume = -(this._mmAudioDevice.AudioEndpointVolume.MasterVolumeLevelScalar - 1.1f); // The higher the volume of the PC is, the higher the values returned by BASS are low. A calculation based on the volume of the PC can counter this.
            var dataCount = BassWasapi.BASS_WASAPI_GetData(this._fftDataBuffer, (int)BASSData.BASS_DATA_FFT2048);
            if (dataCount < -1)
                return null;

            int j;
            int b1;
            float peak;
            var b0 = 0;
            var average = 0;

            for (var i = BassLines[0]; i < BassLines[1]; i++)
            {
                peak = 0;
                b1 = (int)Math.Pow(2, i * 10.0 / (MaxLines - 1));
                if (b1 > 1023)
                    b1 = 1023;
                if (b1 <= b0)
                    b1 = b0 + 1;
                for (; b0 < b1; b0++)
                    if (peak < this._fftDataBuffer[1 + b0])
                        peak = this._fftDataBuffer[1 + b0] / volume;

                j = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (j > 255)
                    j = 255;
                if (j < 0)
                    j = 0;

                average += (byte)j;
            }
            average /= BassLines[1];

            var level = BassWasapi.BASS_WASAPI_GetLevel();
            var left = ((double)Utils.LowWord32(level) - 0) / ((double)ushort.MaxValue - 0) * (255 - 0) + 0;
            var right = ((double)Utils.HighWord32(level) - 0) / ((double)ushort.MaxValue - 0) * (255 - 0) + 0;
            left = average * (1 - (1 / (1 + left)));
            right = average * (1 - (1 / (1 + right)));

            result.Add("left", (byte)left);
            result.Add("right", (byte)right);

            if (level == this._lastOutputLevel && level != 0)
                this._lastOutputLevelCounter++;
            this._lastOutputLevel = level;

            if (this._lastOutputLevelCounter > 3) // regularly, the API can no longer recover the spectrum and should be reset ... as the official documentation says
            {
                this._lastOutputLevelCounter = 0;
                BassWasapi.BASS_WASAPI_Free();
                Bass.BASS_Free();
                Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                BassWasapi.BASS_WASAPI_Init(this._currentAudioDevice.DeviceId, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, this._wasapiProcessCallback, IntPtr.Zero);
                BassWasapi.BASS_WASAPI_Start();
            }

            return result;
        }

        private int WasapiProcessCallBack(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
        }

        public void Dispose()
        {
            this.CurrentAudioDevice = null;
            this.Listening = false;
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }

        #endregion
    }
}
