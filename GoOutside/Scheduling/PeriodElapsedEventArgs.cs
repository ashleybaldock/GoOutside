using System;

namespace GoOutside.Scheduling
{
    public class PeriodElapsedEventArgs
    {
        public DateTime SignalTime { get; private set; }

        public PeriodElapsedEventArgs(DateTime signalTime)
        {
            SignalTime = signalTime;
        }
    }
}