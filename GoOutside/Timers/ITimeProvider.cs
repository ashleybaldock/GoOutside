using System;
using GoOutside.Scheduling;

namespace GoOutside.Timers
{
    public interface ITimeProvider
    {
        DateTime Now();
        IDispatcherTimer CreateDispatcherTimer();
        ICountdownTimer CreateCountdownTimer(TimeSpan duration, TimeSpan interval);
    }
}