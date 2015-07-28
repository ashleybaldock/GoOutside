using System;

namespace GoOutside.ViewModels
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