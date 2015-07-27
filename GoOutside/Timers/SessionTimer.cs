using System;
using GoOutside.Events;
using GoOutside.Scheduling;
using Microsoft.Win32;

namespace GoOutside.Timers
{
    public class SessionTimer : ISessionTimer
    {
        private readonly IPeriod _PeriodBetweenBreaks;

        private readonly IPeriod _PostponeBreakPeriod;

        public event BreakNeededEventHandler BreakNeeded = delegate { };

        public event BreakTakenEventHandler BreakTaken = delegate { };

        public SessionTimer(ISystemEvents systemEvents, IPeriodFactory periodFactory)
        {
            systemEvents.SessionSwitch += OnSessionSwitch;
            _PeriodBetweenBreaks = periodFactory.PeriodBetweenBreaks();
            _PeriodBetweenBreaks.Elapsed += OnPeriodBetweenBreaksElapsed;

            _PostponeBreakPeriod = periodFactory.PostponeBreakPeriod();
        }

        private void OnPeriodBetweenBreaksElapsed(object sender, PeriodElapsedEventArgs periodElapsedEventArgs)
        {
            BreakNeeded(this, new PeriodSinceBreakElapsedEventArgs());
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                case SessionSwitchReason.SessionLogoff:
                    SessionEnd();
                    break;
                case SessionSwitchReason.SessionUnlock:
                case SessionSwitchReason.SessionLogon:
                    SessionStart();
                    break;
            }
        }

        public void PostponeBreak()
        {
            _PostponeBreakPeriod.Start();
        }

        private void SessionEnd()
        {
            _PeriodBetweenBreaks.Stop();
            _PostponeBreakPeriod.Stop();
            BreakTaken(this, new EventArgs());
        }

        private void SessionStart()
        {
            _PeriodBetweenBreaks.Start();
        }
    }
}