using System;
using System.Windows.Threading;
using GoOutside.Events;

namespace GoOutside.Timers
{
    public class PomoTimer : IPomoTimer
    {
        public event PomoTimerTickEventHandler Tick = delegate { };
        public event PomoTimerStateChangeEventHandler StateChanged = delegate { };

        private DateTime _StartTime;

        private readonly DispatcherTimer _Timer;

        public bool Running { get; private set; }

        public PomoTimer()
        {
            _Timer = new DispatcherTimer(DispatcherPriority.DataBind);
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