using LightZ.ComponentModel.Events;
using System;

namespace LightZ.ComponentModel.Core
{
    /// <summary>
    /// Provides a sets of delegates
    /// </summary>
    internal static class Delegates
    {
        /// <summary>
        /// Called when a key is pressed or released.
        /// </summary>
        /// <param name="code">A code the hook procedure uses to determine how to process the message. If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.</param>
        /// <param name="wParam">The virtual-key code of the key that generated the keystroke message.</param>
        /// <param name="lParam">The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag. For more information about the lParam parameter, see Keystroke Message Flags.</param>
        /// <returns>If code is less than zero, the hook procedure must return the value returned by CallNextHookEx.</returns>
        internal delegate IntPtr HookProcCallback(int code, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Called when a <see cref="Delayer"/> execute its action.
        /// </summary>
        /// <typeparam name="T">The type of data in argument</typeparam>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event's argument</param>
        public delegate void DelayerActionEventHandler<T>(object sender, DelayerActionEventArgs<T> e) where T : class;

        /// <summary>
        /// Called when the <see cref="SerialService"/> receives a data from a connecte Bluetooth or USB device.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event's argument</param>
        public delegate void SerialDataReceivedEventArgsEventHandler(object sender, SerialDataReceivedEventArgs e);
    }
}
