using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using GoOutside.Timers;
using Hardcodet.Wpf.TaskbarNotification.Interop;

namespace GoOutside.ViewModels
{
    public class PopUpViewModel : INotifyPropertyChanged, IPopUpViewModel
    {
        private readonly IDispatcher _Dispatcher;
        private readonly ISessionTimer _SessionTimer;
        private bool _Visible;

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                _Visible = value;
                ApplicationThreadNotifyPropertyChanged("Visible");
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

        public PopUpViewModel(IDispatcher dispatcher, ISessionTimer sessionTimer)
        {
            _Dispatcher = dispatcher;
            _SessionTimer = sessionTimer;
            Visible = false;
            sessionTimer.BreakNeeded += OnBreakNeeded;
            sessionTimer.BreakTaken += OnBreakTaken;
        }

        public ICommand DelayCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        _SessionTimer.PostponeBreak();
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

        private void OnBreakNeeded(object sender, EventArgs args)
        {
            ShowPopUpCommand.Execute(null);
        }

        private void OnBreakTaken(object sender, EventArgs args)
        {
            HidePopUpCommand.Execute(null);
        }

        private void ApplicationThreadNotifyPropertyChanged(string propertyName)
        {
            _Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                NotifyPropertyChanged(propertyName);
            }));
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
