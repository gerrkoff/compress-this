using System;

namespace CompressThis.Models
{
    public class ProgressUpdatedEventArgs : EventArgs
    {
        public ProgressUpdatedEventArgs(double completed)
        {
            Completed = completed;
        }

        public double Completed { get; }
    }
}