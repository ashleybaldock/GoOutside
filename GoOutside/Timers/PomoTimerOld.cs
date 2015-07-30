using System;
using GoOutside.Events;
using GoOutside.Scheduling;

namespace GoOutside.Timers
{
    public interface ITimeProvider
    {
        DateTime Now();
        IDispatcherTimer CreateDispatcherTimer();
        ICountdownTimer CreateCountdownTimer(TimeSpan duration, TimeSpan interval);
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

        public ICountdownTimer CreateCountdownTimer(TimeSpan duration, TimeSpan interval)
        {
            return null;
        }
    }

    public interface IConfiguration
    {
        TimeSpan PomoDuration { get; }
        TimeSpan PomoBreakDuration { get; }
    }

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

    public delegate void CountdownTimerDoneEventHandler(object sender, EventArgs args);

    public delegate void CountdownTimerTickEventHandler(object sender, CountdownTimerTickEventArgs args);

    public class CountdownTimerTickEventArgs
    {
        public TimeSpan RemainingTime { get; private set; }

        public CountdownTimerTickEventArgs(TimeSpan remainingTime)
        {
            RemainingTime = remainingTime;
        }
    }

    public class PomoTimer : IPomoTimer
    {
        private TimeSpan _WorkDuration = TimeSpan.FromMinutes(25);
        private TimeSpan _RestDuration = TimeSpan.FromMinutes(5);
        private TimeSpan _Interval = TimeSpan.FromMilliseconds(100);

        private PomoTimerState _State;
        private readonly ICountdownTimer _WorkTimer;
        private readonly ICountdownTimer _RestTimer;

        public PomoTimer(ITimeProvider timeProvider)
        {
            State = PomoTimerState.Disabled;
            _WorkTimer = timeProvider.CreateCountdownTimer(_WorkDuration, _Interval);
            _WorkTimer.Done += OnWorkTimerDone;
            _WorkTimer.Tick += OnTick;
            _RestTimer = timeProvider.CreateCountdownTimer(_RestDuration, _Interval);
            _RestTimer.Done += OnRestTimerDone;
            _RestTimer.Tick += OnTick;
        }

        private void OnTick(object sender, CountdownTimerTickEventArgs args)
        {
            Tick(this, new PomoTimerTickEventArgs(args.RemainingTime));
        }

        private void OnRestTimerDone(object sender, EventArgs args)
        {
            State = PomoTimerState.Disabled;
        }

        private void OnWorkTimerDone(object sender, EventArgs args)
        {
            State = PomoTimerState.Rest;
            _RestTimer.Start();
        }

        public void Start()
        {
            State = PomoTimerState.Work;
            _WorkTimer.Start();
        }

        public void Stop()
        {
            _WorkTimer.Stop();
            _RestTimer.Stop();
            State = PomoTimerState.Disabled;
        }

        public bool Running { get; private set; }

        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };

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
    }

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
            _DispatcherTimer = _TimeProvider.CreateDispatcherTimer();
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
            Tick(this, new PomoTimerTickEventArgs(remaining));
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