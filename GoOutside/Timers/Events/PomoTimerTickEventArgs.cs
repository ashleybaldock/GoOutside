using System;

namespace GoOutside.Timers.Events
{
    public class PomoTimerTickEventArgs
    {
        public TimeSpan TimeRemaining;

        public PomoTimerTickEventArgs(TimeSpan timeRemaining)
        {
            TimeRemaining = timeRemaining;
        }
    }
}