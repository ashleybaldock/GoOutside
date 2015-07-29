using System;
using System.Timers;

namespace GoOutside.Scheduling
{
    [System.ComponentModel.DesignerCategory(@"Code")]
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

    public class DispatcherTimer : System.Windows.Threading.DispatcherTimer, IDispatcherTimer { }

    public interface IDispatcherTimer
    {
        void Start();
        void Stop();
        TimeSpan Interval { get; set; }
        bool IsEnabled { get; set; }
        event EventHandler Tick;
    }
}