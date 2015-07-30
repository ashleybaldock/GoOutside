using System;

namespace GoOutside.Timers.Events
{
    public class CountdownTickEventArgs
    {
        public TimeSpan TimeRemaining;

        public CountdownTickEventArgs(TimeSpan timeRemaining)
        {
            TimeRemaining = timeRemaining;
        }
    }
}