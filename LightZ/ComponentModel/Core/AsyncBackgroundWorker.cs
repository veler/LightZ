using LightZ.ComponentModel.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightZ.ComponentModel.Core
{
    /// <summary>
    /// Provides a pausable background task that can report a progress of its state.
    /// </summary>
    internal sealed class AsyncBackgroundWorker : IPausable
    {
        #region Fields

        private CancellationTokenSource _cancellationTokenSource;
        private Exception _exception;
        private bool _inPause;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the worker should call <see cref="DoWork"/> one time or indefinitely until a pause or exception.
        /// </summary>
        internal bool WorkInLoop { get; set; }

        #endregion

        #region Event

        /// <summary>
        /// Raised when the worker started. The work to do must be placed in this event.
        /// </summary>
        internal event EventHandler DoWork;

        /// <summary>
        /// Raised when the worker paused/stopped (because of a cancellation, an exception or finished normally).
        /// </summary>
        internal event EventHandler<AsyncBackgroundWorkerEndedEventArgs> WorkerPaused;

        #endregion

        #region Contructors

        /// <summary>
        /// Initialize a new instance of the <see cref="AsyncBackgroundWorker"/> class.
        /// </summary>
        internal AsyncBackgroundWorker()
        {
            _inPause = true;
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Dispose()
        {
            Pause();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            if (!_inPause)
            {
                _inPause = true;

                _cancellationTokenSource?.Cancel(true);
            }
        }

        /// <inheritdoc/>
        public void Resume()
        {
            if (_inPause)
            {
                _inPause = false;

                _cancellationTokenSource = new CancellationTokenSource();
                CoreHelper.ThrowIfNotStaThread();
                var task = Task.Factory.StartNew(DoWorkInternal);
                task.ContinueWith(DoWorkEnded, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Throws a <see cref="OperationCanceledException"/> if this token has had cancellation requested.
        /// </summary>
        internal void ThrowIfCanceled()
        {
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Start the execution of a background operation and manage its potential exception.
        /// </summary>
        private void DoWorkInternal()
        {
            try
            {
                do
                {
                    DoWork?.Invoke(this, new EventArgs());
                    ThrowIfCanceled();
                } while (WorkInLoop);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
        }

        /// <summary>
        /// Raises the <see cref="WorkEnded"/> event.
        /// </summary>
        /// <param name="task"></param>
        private void DoWorkEnded(Task task)
        {
            _inPause = true;
            WorkerPaused?.Invoke(this, new AsyncBackgroundWorkerEndedEventArgs(_cancellationTokenSource.Token.IsCancellationRequested, _exception));
        }

        #endregion
    }
}
