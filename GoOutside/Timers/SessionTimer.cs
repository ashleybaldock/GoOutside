using GoOutside.Events;
using GoOutside.Scheduling;
using Microsoft.Win32;

namespace GoOutside.Timers
{
    public class SessionTimer : ISessionTimer
    {
        private readonly IPeriod _PeriodBetweenBreaks;

        public event PeriodSinceBreakElapsedEventHandler PeriodSinceBreakElapsed = delegate { };

        public SessionTimer(ISystemEvents systemEvents, IPeriodFactory periodFactory)
        {
            systemEvents.SessionSwitch += OnSessionSwitch;
            _PeriodBetweenBreaks = periodFactory.PeriodBetweenBreaks();
            _PeriodBetweenBreaks.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, PeriodElapsedEventArgs periodElapsedEventArgs)
        {
            PeriodSinceBreakElapsed(this, new PeriodSinceBreakElapsedEventArgs());
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                case SessionSwitchReason.SessionLogoff:
                    // End session

                    _PeriodBetweenBreaks.Stop();
                    break;
                case SessionSwitchReason.SessionUnlock:
                case SessionSwitchReason.SessionLogon:
                    // Start session

                    _PeriodBetweenBreaks.Start();
                    break;
            }
        }
    }
}