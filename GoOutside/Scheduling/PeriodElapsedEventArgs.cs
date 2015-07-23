using System;

namespace GoOutside.Scheduling
{
    public class PeriodElapsedEventArgs
    {
        public DateTime SignalTime { get; set; }

        public PeriodElapsedEventArgs() : this(DateTime.Now) {}

        public PeriodElapsedEventArgs(DateTime signalTime)
        {
            SignalTime = signalTime;
        }
    }
}