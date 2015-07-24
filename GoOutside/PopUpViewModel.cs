using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoOutside.Annotations;
using GoOutside.Events;

namespace GoOutside
{
    public class PopUpViewModel : INotifyPropertyChanged
    {
        public bool Visible { get; set; }

        public PopUpViewModel(ISessionTimer sessionTimer)
        {
            Visible = true;
            sessionTimer.PeriodSinceBreakElapsed += OnPeriodSinceBreakElapsed;
        }

        private void OnPeriodSinceBreakElapsed(object sender, PeriodSinceBreakElapsedEventArgs args)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                HidePopUpCommand.Execute(null);
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
                        NotifyPropertyChanged("Visibility");
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
                        NotifyPropertyChanged("Visibility");
                    }
                };
            }
        }

        public double Top
        {
            get { return 400; }
            set { }
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
