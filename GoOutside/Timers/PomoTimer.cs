using System;
using GoOutside.Events;
using GoOutside.Scheduling;

namespace GoOutside.Timers
{
    public interface ITimeProvider
    {
        DateTime Now();

        IDispatcherTimer CreateDispatcherTimer();
    }

    class TimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public IDispatcherTimer CreateDispatcherTimer()
        {
            return new DispatcherTimer();
        }
    }

    public interface IConfiguration
    {
        TimeSpan PomoDuration { get; }
        TimeSpan PomoBreakDuration { get; }
    }

    public class PomoTimer : IPomoTimer
    {
        private readonly IDispatcherTimer _DispatcherTimer;
        private readonly IConfiguration _Configuration;
        private readonly ITimeProvider _TimeProvider;
        private DateTime _StartTime;
        private PomoTimerState _State;

        public bool Running { get { return _DispatcherTimer.IsEnabled; }}
        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };

        public PomoTimer(IConfiguration configuration, ITimeProvider timeProvider)
        {
            _State = PomoTimerState.Disabled;
            _Configuration = configuration;
            _TimeProvider = timeProvider;
            _DispatcherTimer = _TimeProvider.CreateDispatcherTimer();
            _DispatcherTimer.Interval = TimeSpan.FromMilliseconds(250);
            _DispatcherTimer.Tick += OnTick;
        }

        private PomoTimerState State
        {
            get { return _State; }
            set
            {
                if (_State == value) return;
                _State = value;
                StateChanged(this, new PomoTimerStateChangeEventArgs(value));
            }
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            var remaining = TimeRemaining();
            if (remaining <= TimeSpan.Zero)
            {
                if (State == PomoTimerState.Work)
                {
                    StartRest();
                    remaining = _Configuration.PomoBreakDuration;
                }
                else
                {
                    Stop();
                    return;
                }
            }
            Tick(this, new PomoTimerEventArgs(remaining));
        }

        private TimeSpan TimeRemaining()
        {
            var duration = State == PomoTimerState.Work
                ? _Configuration.PomoDuration
                : _Configuration.PomoDuration + _Configuration.PomoBreakDuration;
            return _StartTime + duration - _TimeProvider.Now();
        }

        private void StartRest()
        {
            State = PomoTimerState.Rest;
        }

        public void Start()
        {
            _StartTime = _TimeProvider.Now();
            _DispatcherTimer.Start();
            State = PomoTimerState.Work;
        }

        public void Stop()
        {
            _DispatcherTimer.Stop();
            State = PomoTimerState.Disabled;
        }
    }
}