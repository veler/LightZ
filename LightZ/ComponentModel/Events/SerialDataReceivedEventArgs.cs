using System;

namespace LightZ.ComponentModel.Events
{
    /// <summary>
    /// Represents the arguments of an event <see cref="SerialService.DataReceived"/>
    /// </summary>
    internal sealed class SerialDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the data received from the Bluetooth or USB device.
        /// </summary>
        internal string Data { get; }

        /// <summary>
        /// Initialize a new instance of the <see cref="SerialDataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="data">The data received from the Bluetooth or USB device.</param>
        internal SerialDataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }
}
