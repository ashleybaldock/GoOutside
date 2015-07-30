using System;
using GoOutside.Timers.Events;

namespace GoOutside.Timers
{
    public interface ICountdownTimer
    {
        void Start();
        void Stop();
        TimeSpan Interval { get; set; }
        TimeSpan Duration { get; set; }
        bool IsEnabled { get; }
        event CountdownTimerTickEventHandler Tick;
        event CountdownTimerDoneEventHandler Done;
    }

    class CountdownTimer : ICountdownTimer
    {
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public TimeSpan Interval { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsEnabled { get; private set; }
        public event CountdownTimerTickEventHandler Tick;
        public event CountdownTimerDoneEventHandler Done;
    }
}