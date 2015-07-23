using System.Timers;

namespace GoOutside.Scheduling
{
    [System.ComponentModel.DesignerCategory(@"Code")]
    public class Period : Timer, IPeriod
    {
        public new event PeriodElapsedEventHandler Elapsed = delegate { };

        public Period()
        {
            base.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Elapsed(sender, new PeriodElapsedEventArgs(elapsedEventArgs.SignalTime));
        }
    }
}