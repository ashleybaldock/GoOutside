using System;

namespace GoOutside.Timers.Events
{
    public class CountdownTimerTickEventArgs
    {
        public TimeSpan RemainingTime { get; private set; }

        public CountdownTimerTickEventArgs(TimeSpan remainingTime)
        {
            RemainingTime = remainingTime;
        }
    }
}