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
        public bool Visible { get; set; }

        public double Top { get; set; }

        public PopUpViewModel(ISessionTimer sessionTimer)
        {
            Top = 400;
            Visible = false;
            sessionTimer.PeriodSinceBreakElapsed += OnPeriodSinceBreakElapsed;
        }

        private void OnPeriodSinceBreakElapsed(object sender, PeriodSinceBreakElapsedEventArgs args)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                ShowPopUpCommand.Execute(null);
            }));
        }

        public ICommand ShowPopUpCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => !Visible,
                    CommandAction = () =>
                    {
                        Visible = true;
                        NotifyPropertyChanged("Visible");
                    }
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
                    CommandAction = () =>
                    {
                        Visible = false;
                        NotifyPropertyChanged("Visible");
                    }
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
