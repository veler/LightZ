using System;

namespace LightZ.ComponentModel.Events
{
    internal sealed class AsyncBackgroundWorkerEndedEventArgs : EventArgs
    {
        internal bool IsCanceled { get; }

        internal Exception Exception { get; }

        internal AsyncBackgroundWorkerEndedEventArgs(bool isCanceled, Exception exception)
        {
            IsCanceled = isCanceled;
            Exception = exception;
        }
    }
}
