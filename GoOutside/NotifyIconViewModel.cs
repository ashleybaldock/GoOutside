using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoOutside.Events;

namespace GoOutside
{
    public class NotifyIconViewModel
    {
        private readonly IPopupDisplayer _PopupDisplayer;

        public NotifyIconViewModel(ISessionTimer sessionTimer, IPopupDisplayer popupDisplayer)
        {
            _PopupDisplayer = popupDisplayer;
//            sessionTimer.PeriodSinceBreakElapsed += OnPeriodSinceBreakElapsed;
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
                    CommandAction = _PopupDisplayer.Hide
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
}
