using System;
using System.Windows.Threading;
using GoOutside.Events;
using GoOutside.ViewModels;

namespace GoOutside.Timers
{
    public class PomoTimer : IPomoTimer
    {
        public bool Running()
        {
            return false;
        }

        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };

        private DateTime _startTime;

        private DispatcherTimer _timer;

        public PomoTimer()
        {
            _timer = new DispatcherTimer(DispatcherPriority.DataBind);
            _timer.Tick += OnTick;
        }

        public void Start()
        {
            _startTime = DateTime.Now;
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void OnTick(object sender, EventArgs args)
        {
            var remaining = _startTime + TimeSpan.FromMinutes(25) - DateTime.Now;

            if (remaining <= TimeSpan.Zero)
            {
                _timer.Stop();
            }
            else
            {
                Tick(this, new PomoTimerEventArgs(remaining));
            }
        }
    }
}