using System;
using GoOutside.Scheduling;

namespace GoOutside.Timers
{
    class TimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public IDispatcherTimer CreateDispatcherTimer()
        {
            return new DispatcherTimer();
        }

        public ICountdownTimer CreateCountdownTimer(TimeSpan duration, TimeSpan interval)
        {
            return null;
        }
    }
}