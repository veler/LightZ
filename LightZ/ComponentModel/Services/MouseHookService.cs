using LightZ.ComponentModel.Core;
using LightZ.ComponentModel.Events;
using LightZ.ComponentModel.Services.Base;
using System;

namespace LightZ.ComponentModel.Services
{
    /// <summary>
    /// Provides a set of functions designed to listen to the mouse.
    /// </summary>
    internal sealed class MouseHookService : IService, IPausable
    {
        #region Fields

        private MouseHook _mouseHook;

        private bool _inPause;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that defines wether the service is in pause of not.
        /// </summary>
        internal bool IsPaused => _inPause;

        /// <summary>
        /// Determine whether the library is to intercept mouse button up
        /// </summary>
        internal bool HandleMouseButtonUp
        {
            get
            {
                return _mouseHook.HandleMouseButtonUp;
            }
            set
            {
                _mouseHook.HandleMouseButtonUp = value;
            }
        }

        /// <summary>
        /// Determine whether the library is to intercept mouse button down
        /// </summary>
        internal bool HandleMouseButtonDown
        {
            get
            {
                return _mouseHook.HandleMouseButtonDown;
            }
            set
            {
                _mouseHook.HandleMouseButtonDown = value;
            }
        }

        /// <summary>
        /// Determine whether the library is to intercept mouse button move
        /// </summary>
        internal bool HandleMouseMove
        {
            get
            {
                return _mouseHook.HandleMouseMove;
            }
            set
            {
                _mouseHook.HandleMouseMove = value;
            }
        }

        /// <summary>
        /// Determine whether the library is to intercept mouse wheel
        /// </summary>
        internal bool HandleMouseWheel
        {
            get
            {
                return _mouseHook.HandleMouseWheel;
            }
            set
            {
                _mouseHook.HandleMouseWheel = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when a mouse movement or click is detected.
        /// </summary>
        internal event EventHandler<MouseHookEventArgs> MouseAction;

        #endregion

        #region Methods 

        /// <inheritdoc/>
        public void Initialize()
        {
            CoreHelper.ThrowIfNotStaThread();

            _mouseHook = new MouseHook();
            _inPause = true;

            HandleMouseButtonDown = true;
            HandleMouseButtonUp = true;
            HandleMouseMove = true;
            HandleMouseWheel = true;

            Resume();
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Pause();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Pause();
            _mouseHook.Dispose();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            if (!_inPause)
            {
                _inPause = true;

                if (_mouseHook != null)
                {
                    _mouseHook.MouseAction -= OnMouseAction;
                    _mouseHook.Pause();
                }
            }
        }

        /// <inheritdoc/>
        public void Resume()
        {
            if (_inPause)
            {
                _inPause = false;

                if (_mouseHook != null)
                {
                    _mouseHook.Resume();
                    _mouseHook.MouseAction += OnMouseAction;
                }
            }
        }

        /// <summary>
        /// Resume the hooking with the specified delay.
        /// </summary>
        /// <param name="delay">The delay before resuming.</param>
        internal void DelayedResume(TimeSpan delay)
        {
            var delayedHooking = new Delayer<object>(delay);
            delayedHooking.Action += (sender, args) => Resume();
            delayedHooking.ResetAndTick();
        }

        #endregion

        #region Handled Methods

        private void OnMouseAction(object sender, MouseHookEventArgs e)
        {
            MouseAction?.Invoke(this, e);

            if (e.Handled)
            {
                return;
            }
        }

        #endregion
    }
}
