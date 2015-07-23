using GoOutside.Events;
using Microsoft.Win32;

namespace GoOutside
{
    public class SessionTimer
    {
        public SessionTimer(ISystemEvents systemEvents)
        {
            systemEvents.SessionSwitch += SessionSwitchDelegate;
        }

        public void SessionSwitchDelegate(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                case SessionSwitchReason.SessionLogoff:
                    // End session

                    // Stop the timer
                    break;
                case SessionSwitchReason.SessionUnlock:
                case SessionSwitchReason.SessionLogon:
                    // Start session

                    // Start the timer
                    break;
            }
        }
    }
}