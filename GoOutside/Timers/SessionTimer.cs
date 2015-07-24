using GoOutside.Events;
using GoOutside.Scheduling;
using Microsoft.Win32;

namespace GoOutside.Timers
{
    public class SessionTimer : ISessionTimer
    {
        private readonly IPeriod _Period;

        public event PeriodSinceBreakElapsedEventHandler PeriodSinceBreakElapsed = delegate { };

        public SessionTimer(ISystemEvents systemEvents, IPeriod period)
        {
            systemEvents.SessionSwitch += OnSessionSwitch;
            _Period = period;
            _Period.Elapsed += OnTimerElapsed;
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

                    _Period.Stop();
                    break;
                case SessionSwitchReason.SessionUnlock:
                case SessionSwitchReason.SessionLogon:
                    // Start session

                    _Period.Start();
                    break;
            }
        }
    }
}