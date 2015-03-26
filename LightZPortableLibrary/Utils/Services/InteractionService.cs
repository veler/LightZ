namespace LightZPortableLibrary.Utils.Services
{
    using System;
    using System.Threading.Tasks;

    using LightZPortableLibrary.Enums;
    using LightZPortableLibrary.Model;
    using LightZPortableLibrary.Utils.Bluetooth;
    using LightZPortableLibrary.Utils.Threading;

    /// <summary>
    /// Class used to implement the code to send requests at regular interval to the Arduino
    /// </summary>
    public abstract class InteractionService : IDisposable
    {
        #region Fields

        private Timer _timer;

        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// The class allows to interact with Arduino by Bluetooth. This class changes with the platform (Desktop, tablet, smartphone).
        /// </summary>
        protected abstract BluetoothHelper Bluetooth { get; set; }

        /// <summary>
        /// Gets the current mode.
        /// </summary>
        public Mode CurrentArduinoMode { get; protected set; }

        /// <summary>
        /// Gets the overall color.
        /// </summary>
        public Color CurrentArduinoColor { get; protected set; }

        /// <summary>
        /// Gets the overall contrast.
        /// </summary>
        public byte CurrentArduinoBrightness { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the class <see cref="InteractionService"/>
        /// </summary>
        protected InteractionService()
        {
            this.CurrentArduinoMode = Mode.Unknow;

            this._timer = new Timer(TimeSpan.FromDays(10));
            this._timer.Tick += this.Tick;
        }

        #endregion

        #region Handled Methods

        private void Tick(object sender, EventArgs e)
        {
            this._timer.Dispose();
            Task.Run(
                () =>
                {
                    while (!this._disposed)
                    {
                        this.Loop();
                    }
                });
        }

        protected abstract void Loop();

        #endregion

        #region Implements

        public void Dispose()
        {
            this._disposed = true;
            this._timer.Dispose();
        }

        #endregion
    }
}
