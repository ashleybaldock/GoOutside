using GoOutside.ViewModels;

namespace GoOutside.Timers
{
    public interface IPomoTimer
    {
        void Start();
        void Stop();
        bool Running();
        event PomoTimerTickEventHandler Tick;
        event PomoTimerStateChangeEventHandler StateChanged;
    }
}