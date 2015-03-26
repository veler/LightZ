namespace LightZPortableLibrary.Utils.Threading
{
    using System;

    /// <summary>
    /// Represents a Timer that raise an event at regular interval
    /// </summary>
    public class Timer : IDisposable
    {
        #region Fields

        private System.Threading.Timer _timer;
        private TimeSpan _timeDue = TimeSpan.FromMilliseconds(1);
        private TimeSpan _interval;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the time span between each tick
        /// </summary>
        public TimeSpan Interval
        {
            get
            {
                return this._interval;
            }
            set
            {
                this._interval = value;
                this._timer.Change(this._timeDue, this._interval);
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// Raised at each tick
        /// </summary>
        public event EventHandler Tick;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize an instance of <see cref="Timer"/>
        /// </summary>
        /// <param name="interval">Sets the time span between each tick</param>
        public Timer(TimeSpan interval)
        {
            if (interval == null)
                throw new ArgumentNullException("interval");
            this._timer = new System.Threading.Timer(this.TimerCallback, null, this._timeDue, interval);
        }

        #endregion

        #region Methods

        private void TimerCallback(object state)
        {
            var handler = this.Tick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Implements

        public void Dispose()
        {
            if (this._timer != null)
                this._timer.Dispose();
            this._timer = null;
        }

        #endregion
    }
}
