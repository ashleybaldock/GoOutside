namespace GoOutside.Timers.Events
{
    public class PomoTimerStateChangeEventArgs
    {
        public PomoTimerState State;

        public PomoTimerStateChangeEventArgs(PomoTimerState state)
        {
            State = state;
        }
    }
}