using System;
using GoOutside.Scheduling;
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
        event CountdownDoneEventHandler Done;
    }

    internal class CountdownTimer : ICountdownTimer
    {
        private readonly ITimeProvider _TimeProvider;
        private readonly TimeSpan _Duration;
        private readonly IDispatcherTimer _IntervalTimer;
        private DateTime _StartTime;

        internal CountdownTimer(
            ITimeProvider timeProvider,
            TimeSpan duration,
            TimeSpan interval)
        {
            _TimeProvider = timeProvider;
            _Duration = duration;
            _IntervalTimer = timeProvider.CreateDispatcherTimer(interval);
            _IntervalTimer.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            var remaining = TimeRemaining();
            if (remaining <= TimeSpan.Zero)
            {
                Tick(this, new CountdownTickEventArgs(TimeSpan.Zero));
                Done(this, new EventArgs());
            }
            else
            {
                Tick(this, new CountdownTickEventArgs(remaining));
            }
        }

        private TimeSpan TimeRemaining()
        {
            return _StartTime + _Duration - _TimeProvider.Now();
        }

        public void Start()
        {
            _StartTime = _TimeProvider.Now();
            _IntervalTimer.Start();
        }

        public void Stop()
        {
            _IntervalTimer.Stop();
        }

        public TimeSpan Interval { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsEnabled { get; private set; }
        public event CountdownTimerTickEventHandler Tick = delegate { };
        public event CountdownDoneEventHandler Done = delegate { };
    }
}