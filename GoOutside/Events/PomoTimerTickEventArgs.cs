using System;

namespace GoOutside.Events
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