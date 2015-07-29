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
        public bool Running { get; private set; }
        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged;

        public PomoTimer(IConfiguration configuration, ITimeProvider timeProvider)
        {
            _Configuration = configuration;
            _TimeProvider = timeProvider;
            _DispatcherTimer = _TimeProvider.CreateDispatcherTimer();
            _DispatcherTimer.Interval = TimeSpan.FromMilliseconds(250);
            _DispatcherTimer.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            var remaining = TimeRemaining();
            if (remaining > TimeSpan.Zero)
            {
                Tick(this, new PomoTimerEventArgs(remaining));
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
        }

        public void Stop()
        {
            _DispatcherTimer.Stop();
        }
    }

    public class PomoTimer2 : IPomoTimer
    {
        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };

        private DateTime _StartTime;

        private readonly DispatcherTimer _Timer;

        public bool Running { get; private set; }

        public PomoTimer2()
        {
//            _Timer = new DispatcherTimer(DispatcherPriority.DataBind);
            _Timer.Tick += OnTick;
        }

        public void Start()
        {
            _StartTime = DateTime.Now;
            _Timer.Interval = TimeSpan.FromMilliseconds(250);
            _Timer.Start();
            Running = true;
            StateChanged(this, new PomoTimerStateChangeEventArgs(PomoTimerState.Work));
        }

        public void Stop()
        {
            Running = false;
            _Timer.Stop();
            StateChanged(this, new PomoTimerStateChangeEventArgs(PomoTimerState.Disabled));
        }

        private void OnTick(object sender, EventArgs args)
        {
            var remaining = _StartTime + TimeSpan.FromMinutes(25) - DateTime.Now;

            if (remaining <= TimeSpan.Zero)
            {
                Stop();
            }
            else
            {
                Tick(this, new PomoTimerEventArgs(remaining));
            }
        }
    }
}