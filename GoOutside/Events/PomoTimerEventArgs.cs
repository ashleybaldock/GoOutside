using System;

namespace GoOutside.Events
{
    public class PomoTimerEventArgs
    {
        public TimeSpan TimeRemaining;

        public PomoTimerEventArgs(TimeSpan timeRemaining)
        {
            TimeRemaining = timeRemaining;
        }
    }
}