using System.ComponentModel;
using System.Windows.Input;

namespace GoOutside.ViewModels
{
    public class PomoViewModel : INotifyPropertyChanged, IPomoViewModel
    {
        private bool _Visible;
        private const string _Start = "Start";
        private const string _Cancel = "Stop";

        public event PropertyChangedEventHandler PropertyChanged;

        public PomoViewModel()
        {
            Visible = true;
            TimerText = "25:00";
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
                NotifyPropertyChanged("Visible");
            }
        }

        public string TimerText { get; set; }

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
                        TimerText = "Start";
                        NotifyPropertyChanged("TimerText");
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
                        TimerText = "25:00";
                        NotifyPropertyChanged("TimerText");
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
                        TimerText = "Click";
                        NotifyPropertyChanged("TimerText");
                    }
                };
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}