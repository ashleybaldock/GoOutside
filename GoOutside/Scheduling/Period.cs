using System.ComponentModel;
using System.Timers;

namespace GoOutside.Scheduling
{
    [DesignerCategory(@"Code")]
    public class Period : Timer, IPeriod
    {
        public new event PeriodElapsedEventHandler Elapsed = delegate { };

        public Period(double interval)
        {
            Interval = interval;
            AutoReset = false;
            base.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Elapsed(sender, new PeriodElapsedEventArgs(elapsedEventArgs.SignalTime));
        }
    }
}