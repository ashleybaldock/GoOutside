using Microsoft.Win32;

namespace GoOutside.Events
{
    public interface ISystemEvents
    {
        event SessionSwitchEventHandler SessionSwitch;
    }
}