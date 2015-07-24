using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoOutside.Events;

namespace GoOutside
{
    public class PopUpViewModel : INotifyPropertyChanged
    {
        private bool _Visible;

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                _Visible = value;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                {
                    NotifyPropertyChanged("Visible");
                }));
            }
        }

        public double Top { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public PopUpViewModel(ISessionTimer sessionTimer)
        {
            Top = 400;
            Visible = false;
            sessionTimer.PeriodSinceBreakElapsed += OnPeriodSinceBreakElapsed;
        }

        private void OnPeriodSinceBreakElapsed(object sender, PeriodSinceBreakElapsedEventArgs args)
        {
            ShowPopUpCommand.Execute(null);
        }

        public ICommand ShowPopUpCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => !Visible,
                    CommandAction = () => Visible = true
                };
            }
        }

        public ICommand HidePopUpCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Visible,
                    CommandAction = () => Visible = false
                };
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
