using System.Timers;

namespace GoOutside.Scheduling
{
    [System.ComponentModel.DesignerCategory("Code")]
    class Period : Timer, IPeriod
    {
        public new event PeriodElapsedEventHandler Elapsed = delegate { };

        public Period()
        {
            base.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Elapsed(this, new PeriodElapsedEventArgs(elapsedEventArgs.SignalTime));
        }
    }
}