using System.ComponentModel;
using System.Windows.Input;

namespace GoOutside.ViewModels
{
    public class PomoViewModel : INotifyPropertyChanged, IPomoViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PomoViewModel()
        {
            Visible = true;
            TimerText = "25:00";
        }

        public double Height { get { return 200; } }
        public double Width { get { return 200; } }
        public bool Visible { get; set; }
        public string TimerText { get; set; }

        public ICommand MouseEnterCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        TimerText = "MouseOver";
                        NotifyPropertyChanged("TimerText");
                    }
                };
            }
        }

        public ICommand MouseLeaveCommand
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

        public ICommand StopPomo
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
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