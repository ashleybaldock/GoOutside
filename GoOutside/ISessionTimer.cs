using GoOutside.Events;

namespace GoOutside
{
    public interface ISessionTimer
    {
        event PeriodSinceBreakElapsedEventHandler PeriodSinceBreakElapsed;
    }
}