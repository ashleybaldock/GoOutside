using GoOutside.Events;
using GoOutside.Scheduling;
using Microsoft.Win32;

namespace GoOutside
{
    public class SessionTimer
    {
        private readonly IScheduler _Scheduler;

        public event PeriodSinceBreakElapsedEventHandler PeriodSinceBreakElapsed = delegate { };

        public SessionTimer(ISystemEvents systemEvents, IScheduler scheduler)
        {
            systemEvents.SessionSwitch += OnSessionSwitch;
            _Scheduler = scheduler;
            _Scheduler.Alarm += OnTimerElapsed;
        }

        private void OnTimerElapsed(object source, SchedulerEventArgs e)
        {
            PeriodSinceBreakElapsed.Invoke(this, new PeriodSinceBreakElapsedEventArgs());
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                case SessionSwitchReason.SessionLogoff:
                    // End session

                    _Scheduler.Stop();
                    break;
                case SessionSwitchReason.SessionUnlock:
                case SessionSwitchReason.SessionLogon:
                    // Start session

                    _Scheduler.Start();
                    break;
            }
        }
    }
}