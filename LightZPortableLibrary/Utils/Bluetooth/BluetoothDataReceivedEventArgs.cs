namespace LightZPortableLibrary.Utils.Bluetooth
{
    using System;

    public class BluetoothDataReceivedEventArgs : EventArgs
    {
        public BluetoothDataReceivedEventArgs(string data)
        {
            this.Data = data;
        }

        public string Data { get; private set; }
    }
}
