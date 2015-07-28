namespace GoOutside.ViewModels
{
    public class PomoTimerStateEventArgs
    {
        public enum PomoTimerState
        {
            Work,
            Rest,
            Disabled
        }

        public PomoTimerState State;

        public PomoTimerStateEventArgs(PomoTimerState state)
        {
            State = state;
        }
    }
}