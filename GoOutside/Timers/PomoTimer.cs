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
            if (remaining > TimeSpan.Zero)
            {
                Tick(this, new PomoTimerEventArgs(remaining));
            }
            else
            {
                if (State == PomoTimerState.Work)
                {
                    State = PomoTimerState.Rest;
                }
                else if (State == PomoTimerState.Rest)
                {
                    State = PomoTimerState.Disabled;
                }
            }
        }

        private TimeSpan TimeRemaining()
        {
            return _StartTime + _Configuration.PomoDuration - _TimeProvider.Now();
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

    public class PomoTimer2
    {
        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };

        private DateTime _StartTime;

        private readonly DispatcherTimer _Timer;

        public bool Running { get; private set; }

        public void Start()
        {
            _StartTime = DateTime.Now;
            _Timer.Interval = TimeSpan.FromMilliseconds(250);
            _Timer.Start();
            Running = true;
            StateChanged(this, new PomoTimerStateChangeEventArgs(PomoTimerState.Work));
        }

        private void OnTick(object sender, EventArgs args)
        {
            var remaining = _StartTime + TimeSpan.FromMinutes(25) - DateTime.Now;

            if (remaining <= TimeSpan.Zero)
            {
//                Stop();
            }
            else
            {
                Tick(this, new PomoTimerEventArgs(remaining));
            }
        }
    }
}