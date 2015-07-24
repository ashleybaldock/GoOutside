using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoOutside.Events;
using Hardcodet.Wpf.TaskbarNotification.Interop;

namespace GoOutside.ViewModels
{
    public class PopUpViewModel : INotifyPropertyChanged, IPopUpViewModel
    {
        private readonly ISessionTimer _SessionTimer;
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

        public double Height { get { return 200; } }

        public double Width { get { return 500; } }

        public double Top {
            get
            {
                var position = TrayInfo.GetTrayLocation();
                return position.Y - 1 - Height;
            }
        }

        public double Left
        {
            get
            {
                var position = TrayInfo.GetTrayLocation();
                return position.X - 1 - Width;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PopUpViewModel(ISessionTimer sessionTimer)
        {
            _SessionTimer = sessionTimer;
            Visible = true;
            sessionTimer.PeriodSinceBreakElapsed += OnPeriodSinceBreakElapsed;
        }

        private void OnPeriodSinceBreakElapsed(object sender, PeriodSinceBreakElapsedEventArgs args)
        {
            ShowPopUpCommand.Execute(null);
        }

        public ICommand DelayCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        // TODO _SessionTimer.StartDelay();
                        Visible = false;
                    }
                };
            }
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
