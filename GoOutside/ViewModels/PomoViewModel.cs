using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace GoOutside.ViewModels
{
    public class PomoViewModel : INotifyPropertyChanged, IPomoViewModel
    {
        private bool _Visible;
        private string _TimerText;
        private const string _Start = "Start";
        private const string _Cancel = "Stop";

        private bool _ShowTimer = true;


        private DateTime _startTime;

        private DispatcherTimer _timer;

        private void SetupTimer()
        {
            _timer = new DispatcherTimer(DispatcherPriority.DataBind);
            _timer.Tick += Tick;
        }
        private void Start()
        {
            _startTime = DateTime.Now;
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Start();
        }

        private void Stop()
        {
            _timer.Stop();
        }

        private void Tick(object sender, EventArgs args)
        {
            var remaining = _startTime + TimeSpan.FromMinutes(25) - DateTime.Now;

            if (remaining <= TimeSpan.Zero)
            {
                _timer.Stop();
            }
            else
            {
                if (_ShowTimer)
                {
                    TimerText = remaining.ToString(@"mm\:ss");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public PomoViewModel()
        {
            Visible = true;
            TimerText = "25:00";
            SetupTimer();
        }

        public double Height { get { return 200; } }
        public double Width { get { return 200; } }

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible == value) return;
                _Visible = value;
                OnPropertyChanged();
            }
        }

        public string TimerText
        {
            get { return _TimerText; }
            set
            {
                if (_TimerText == value) return;
                _TimerText = value;
                OnPropertyChanged();
            }
        }

        public ICommand Show
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => !Visible,
                    CommandAction = () =>
                    {
                        Visible = true;
                    }
                };
            }
        }

        public ICommand Hide
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Visible,
                    CommandAction = () =>
                    {
                        Visible = false;
                    }
                };
            }
        }

        public ICommand OnMouseEnter
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        if (_timer.IsEnabled)
                        {
                            _ShowTimer = false;
                            TimerText = "Stop";
                        }
                        else
                        {
                            TimerText = "Start";
                        }
                    }
                };
            }
        }

        public ICommand OnMouseLeave
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        _ShowTimer = true;
                        if (!_timer.IsEnabled)
                        {
                            TimerText = "Start";
                        }
                    }
                };
            }
        }

        public ICommand OnMouseClick
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        if (_timer.IsEnabled)
                        {
                            Stop();
                        }
                        else
                        {
                            Start();
                        }
                    }
                };
            }
        }

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}