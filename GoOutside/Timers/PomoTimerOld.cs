using System;
using GoOutside.Configuration;
using GoOutside.Events;
using GoOutside.Scheduling;
using GoOutside.Timers.Events;

namespace GoOutside.Timers
{
    public class PomoTimerOld : IPomoTimer
    {
        private readonly IDispatcherTimer _DispatcherTimer;
        private readonly IConfiguration _Configuration;
        private readonly ITimeProvider _TimeProvider;
        private DateTime _StartTime;
        private PomoTimerState _State;

        public bool Running { get { return _DispatcherTimer.IsEnabled; }}
        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };

        public PomoTimerOld(IConfiguration configuration, ITimeProvider timeProvider)
        {
            _State = PomoTimerState.Disabled;
            _Configuration = configuration;
            _TimeProvider = timeProvider;
//            _DispatcherTimer = _TimeProvider.CreateDispatcherTimer();
            _DispatcherTimer.Interval = TimeSpan.FromMilliseconds(250);
            _DispatcherTimer.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            var remaining = TimeRemaining();
            if (remaining <= TimeSpan.Zero)
            {
                Stop();
            }
            Tick(this, new CountdownTickEventArgs(remaining));
        }

        private TimeSpan TimeRemaining()
        {
            var duration =  _Configuration.PomoDuration;
            return _StartTime + duration - _TimeProvider.Now();
        }

        public void Start()
        {
            _StartTime = _TimeProvider.Now();
            _DispatcherTimer.Start();
        }

        public void Stop()
        {
            _DispatcherTimer.Stop();
        }
    }
}