using GoOutside.Timers.Events;

namespace GoOutside.Timers
{
    public interface ICountdownTimer
    {
        void Start();
        void Stop();
        bool Running { get; }
        event CountdownTimerTickEventHandler Tick;
        event CountdownDoneEventHandler Done;
    }
}