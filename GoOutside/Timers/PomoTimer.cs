using System;
using GoOutside.Timers.Events;

namespace GoOutside.Timers
{
    public class PomoTimer : IPomoTimer
    {
        private readonly TimeSpan _WorkDuration = TimeSpan.FromMinutes(25);
        private readonly TimeSpan _RestDuration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _Interval = TimeSpan.FromMilliseconds(100);

        private readonly ICountdownTimer _WorkTimer;
        private readonly ICountdownTimer _RestTimer;

        public PomoTimer(ITimeProvider timeProvider)
        {
            _WorkTimer = timeProvider.CreateCountdownTimer(_WorkDuration, _Interval);
            _WorkTimer.Done += OnWorkTimerDone;
            _WorkTimer.Tick += OnTick;
            _RestTimer = timeProvider.CreateCountdownTimer(_RestDuration, _Interval);
            _RestTimer.Done += OnRestTimerDone;
            _RestTimer.Tick += OnTick;
        }

        private void OnTick(object sender, CountdownTickEventArgs args)
        {
            Tick(this, args);
        }

        private void OnRestTimerDone(object sender, EventArgs args)
        {
            StateChanged(this, new PomoTimerStateChangeEventArgs(PomoTimerState.Disabled));
            _WorkTimer.Stop();
            _RestTimer.Stop();
        }

        private void OnWorkTimerDone(object sender, EventArgs args)
        {
            StateChanged(this, new PomoTimerStateChangeEventArgs(PomoTimerState.Rest));
            _RestTimer.Start();
            _WorkTimer.Stop();
        }

        public void Start()
        {
            StateChanged(this, new PomoTimerStateChangeEventArgs(PomoTimerState.Work));
            _WorkTimer.Start();
        }

        public void Stop()
        {
            if (_WorkTimer.Running || _RestTimer.Running)
            {
                StateChanged(this, new PomoTimerStateChangeEventArgs(PomoTimerState.Disabled));
            }
            _WorkTimer.Stop();
            _RestTimer.Stop();
        }

        public bool Running
        {
            get { return _WorkTimer.Running || _RestTimer.Running; }
        }

        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };
    }
}