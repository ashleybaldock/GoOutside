using System;
using GoOutside.Scheduling;
using GoOutside.Timers.Events;

namespace GoOutside.Timers
{
    internal class CountdownTimer : ICountdownTimer
    {
        private readonly ITimeProvider _TimeProvider;
        private readonly TimeSpan _Duration;
        private readonly IDispatcherTimer _IntervalTimer;
        private DateTime _StartTime;

        public bool Running
        {
            get { return _IntervalTimer.IsEnabled; }
        }
        public event CountdownTimerTickEventHandler Tick = delegate { };
        public event CountdownDoneEventHandler Done = delegate { };

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

        public void Start()
        {
            _StartTime = _TimeProvider.Now();
            _IntervalTimer.Start();
        }

        public void Stop()
        {
            _IntervalTimer.Stop();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var remaining = TimeRemaining();
            if (remaining <= TimeSpan.Zero)
            {
                Tick(this, new CountdownTickEventArgs(TimeSpan.Zero));
                Done(this, new EventArgs());
                _IntervalTimer.Stop();
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
    }
}