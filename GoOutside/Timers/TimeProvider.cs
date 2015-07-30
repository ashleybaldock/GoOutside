using System;
using GoOutside.Scheduling;

namespace GoOutside.Timers
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public IDispatcherTimer CreateDispatcherTimer(TimeSpan interval)
        {
            return new DispatcherTimer {Interval = interval};
        }

        public ICountdownTimer CreateCountdownTimer(TimeSpan duration, TimeSpan interval)
        {
            return new CountdownTimer(this, duration, interval);
        }
    }
}