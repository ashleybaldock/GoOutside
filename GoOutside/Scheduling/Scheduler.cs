using System;
using GoOutside.Events;

namespace GoOutside.Scheduling
{
    public class Scheduler : IScheduler
    {
        public event EventHandler<SchedulerEventArgs> Alarm;
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}