using GoOutside.Events;

namespace GoOutside.Timers
{
    public interface IPomoTimer
    {
        void Start();
        void Stop();
        bool Running { get; }
        event PomoTimerTickEventHandler Tick;
        event PomoTimerStateChangeEventHandler StateChanged;
    }
}