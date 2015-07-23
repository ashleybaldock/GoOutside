using System;
using GoOutside.Events;

namespace GoOutside
{
    public interface IScheduler
    {
        event EventHandler<SchedulerEventArgs> Alarm;
        void Start();
        void Stop();
    }
}