using Microsoft.Win32;

namespace GoOutside.Events
{
    class SystemEventsWrapper : ISystemEvents
    {
        public event SessionSwitchEventHandler SessionSwitch
        {
            add { SystemEvents.SessionSwitch += value; }
            remove { SystemEvents.SessionSwitch -= value; }
        }
    }
}