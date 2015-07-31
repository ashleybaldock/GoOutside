using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using GoOutside.Timers;
using GoOutside.Timers.Events;

namespace GoOutside.ViewModels
{
    public static class TomatoColours
    {
        public static readonly TomatoColourSet Work = new TomatoColourSet
        {
            Background = Color.FromArgb(255, 85, 0, 0),
            Dark = Color.FromArgb(255, 128, 0, 0),
            Light = Color.FromArgb(255, 212, 0, 0),
            StemHighlight = Color.FromArgb(255, 204, 255, 0),
            StemOutline = Color.FromArgb(255, 17, 43, 0),
            StemDark = Color.FromArgb(255, 34, 85, 0),
            StemMid = Color.FromArgb(255, 68, 85, 0),
            StemLight = Color.FromArgb(255, 102, 128, 0)
        };

        public static readonly TomatoColourSet Rest = new TomatoColourSet
        {
            Background = Color.FromArgb(255, 0, 85, 0),
            Dark = Color.FromArgb(255, 0, 128, 0),
            Light = Color.FromArgb(255, 0, 212, 0),
            StemHighlight = Color.FromArgb(255, 204, 255, 0),
            StemOutline = Color.FromArgb(255, 17, 43, 0),
            StemDark = Color.FromArgb(255, 34, 85, 0),
            StemMid = Color.FromArgb(255, 68, 85, 0),
            StemLight = Color.FromArgb(255, 102, 128, 0)
        };

        public static readonly TomatoColourSet Disabled = new TomatoColourSet
        {
            Background = Color.FromArgb(255, 85, 85, 85),
            Dark = Color.FromArgb(255, 128, 128, 128),
            Light = Color.FromArgb(255, 212, 212, 212),
            StemHighlight = Color.FromArgb(255, 255, 255, 255),
            StemOutline = Color.FromArgb(255, 43, 43, 43),
            StemDark = Color.FromArgb(255, 85, 85, 85),
            StemMid = Color.FromArgb(255, 85, 85, 85),
            StemLight = Color.FromArgb(255, 128, 128, 128)
        };

        public class TomatoColourSet
        {
            public Color Light { get; set; }
            public Color Dark { get; set; }
            public Color Background { get; set; }
            public Color StemHighlight { get; set; }
            public Color StemOutline { get; set; }
            public Color StemDark { get; set; }
            public Color StemMid { get; set; }
            public Color StemLight { get; set; }

            // Stem outline #FF112B00
            // Stem dark #FF225500
            // Stem mid #FF445500
            // Stem light #FF668000
        }
    }


    public class PomoViewModel : INotifyPropertyChanged, IPomoViewModel
    {
        private const string _Start = "Start";
        private const string _Cancel = "Cancel";

        private readonly IPomoTimer _PomoTimer;

        private bool _Visible;
        private string _TimerText;
        private bool _ShowTimer = true;
        private TomatoColours.TomatoColourSet _ActiveColourSet;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Height { get { return 200; } }
        public double Width { get { return 200; } }

        public TomatoColours.TomatoColourSet ColourSet
        {
            get { return _ActiveColourSet; }
            set
            {
                if (_ActiveColourSet == value) return;
                _ActiveColourSet = value;
                OnPropertyChanged();
            }
        }

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
            _PomoTimer.StateChanged += OnStateChanged;
            Visible = true;
            TimerText = _Start;
            ColourSet = TomatoColours.Disabled;
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
                        TimerText = _PomoTimer.Running ? _Cancel : _Start;
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
                        if (!_PomoTimer.Running)
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
                        if (_PomoTimer.Running)
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

        private void OnTick(object sender, CountdownTickEventArgs args)
        {
            if (_ShowTimer) TimerText = args.TimeRemaining.ToString(@"mm\:ss");
        }

        private void OnStateChanged(object sender, PomoTimerStateChangeEventArgs eventargs)
        {
            switch (eventargs.State)
            {
                case PomoTimerState.Disabled:
                    Disabled();
                    break;
                case PomoTimerState.Work:
                    Work();
                    break;
                case PomoTimerState.Rest:
                    Rest();
                    break;
            }
        }

        private void Work()
        {
            ColourSet = TomatoColours.Work;
        }

        private void Rest()
        {
            ColourSet = TomatoColours.Rest;
        }

        private void Disabled()
        {
            ColourSet = TomatoColours.Disabled;
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