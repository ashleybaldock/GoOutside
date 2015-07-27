using GoOutside.Events;

namespace GoOutside.Timers
{
    public interface ISessionTimer
    {
        event BreakNeededEventHandler BreakNeeded;
        event BreakTakenEventHandler BreakTaken;
        void PostponeBreak();
    }
}