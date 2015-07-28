using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using GoOutside.Timers;

namespace GoOutside.ViewModels
{
    public class PomoViewModel : INotifyPropertyChanged, IPomoViewModel
    {
        private const string _Start = "Start";
        private const string _Cancel = "Cancel";

        private readonly IPomoTimer _PomoTimer;
        private bool _Visible;
        private string _TimerText;
        private bool _ShowTimer = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Height { get { return 200; } }
        public double Width { get { return 200; } }

        public Color LightColour { get; set; }
        public Color DarkColour { get; set; }
        public Color BackgroundColour { get; set; }

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

        public PomoViewModel(IPomoTimer pomoTimer)
        {
            _PomoTimer = pomoTimer;
            _PomoTimer.Tick += OnTick;
            Visible = true;
            TimerText = "25:00";
            LightColour = Color.FromArgb(255, 212, 0, 0);
            DarkColour = Color.FromArgb(255, 128, 0, 0);
            BackgroundColour = Color.FromArgb(255, 85, 0, 0);
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
                        _ShowTimer = false;
                        TimerText = _PomoTimer.Running() ? _Cancel : _Start;
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
                        if (!_PomoTimer.Running())
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
                        if (_PomoTimer.Running())
                        {
                            _PomoTimer.Stop();
                        }
                        else
                        {
                            _PomoTimer.Start();
                        }
                    }
                };
            }
        }

        private void OnTick(object sender, PomoTimerEventArgs args)
        {
            if (_ShowTimer) TimerText = args.TimeRemaining.ToString(@"mm\:ss");
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