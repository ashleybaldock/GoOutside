using GoOutside.Events;

namespace GoOutside.Timers
{
    public interface ISessionTimer
    {
        event BreakNeededEventHandler BreakNeeded;

        void PostponeBreak();
    }
}