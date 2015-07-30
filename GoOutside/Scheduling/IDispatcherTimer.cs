using System;

namespace GoOutside.Scheduling
{
    public interface IDispatcherTimer
    {
        void Start();
        void Stop();
        TimeSpan Interval { get; set; }
        bool IsEnabled { get; set; }
        event EventHandler Tick;
    }
}