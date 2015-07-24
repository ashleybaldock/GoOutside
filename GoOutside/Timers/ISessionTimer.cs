using GoOutside.Events;

namespace GoOutside.Timers
{
    public interface ISessionTimer
    {
        event PeriodSinceBreakElapsedEventHandler PeriodSinceBreakElapsed;
    }
}