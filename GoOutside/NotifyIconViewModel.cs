using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using GoOutside.Events;
using Hardcodet.Wpf.TaskbarNotification;

namespace GoOutside
{
    public interface IPopupDisplayer
    {
        bool CanShow();
        bool CanHide();
        void Show();
        void Hide();
    }

    class PopupDisplayer : IPopupDisplayer
    {
        public bool CanShow()
        {
            var notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");
            return notifyIcon != null && (notifyIcon.CustomBalloon == null || !notifyIcon.CustomBalloon.IsOpen);
        }

        public bool CanHide()
        {
            var notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");
            return notifyIcon != null && notifyIcon.CustomBalloon != null && notifyIcon.CustomBalloon.IsOpen;
        }

        public void Show()
        {
            var popup = new PopUp();
            var notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");
            if (notifyIcon != null)
                notifyIcon.ShowCustomBalloon(popup, PopupAnimation.None, null);
        }

        public void Hide()
        {
            var notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");
            if (notifyIcon != null)
                notifyIcon.CloseBalloon();
        }
    }

    public class NotifyIconViewModel
    {
        private readonly IPopupDisplayer _PopupDisplayer;

        public NotifyIconViewModel(ISessionTimer sessionTimer, IPopupDisplayer popupDisplayer)
        {
            _PopupDisplayer = popupDisplayer;
            sessionTimer.PeriodSinceBreakElapsed += OnPeriodSinceBreakElapsed;
        }

        private void OnPeriodSinceBreakElapsed(object sender, PeriodSinceBreakElapsedEventArgs args)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
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
                    CanExecuteFunc = _PopupDisplayer.CanShow,
                    CommandAction = _PopupDisplayer.Show
                };
            }
        }

        public ICommand HidePopUpCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = _PopupDisplayer.CanHide,
                    CommandAction = () =>
                    {
                        var notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");
                        if (notifyIcon != null)
                            notifyIcon.CloseBalloon();
                    }
                };
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand {CommandAction = () => Application.Current.Shutdown()};
            }
        }
    }

    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null  || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
