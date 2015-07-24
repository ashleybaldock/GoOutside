using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GoOutside.Events;

namespace GoOutside
{
    public class NotifyIconViewModel
    {
        public NotifyIconViewModel(ISessionTimer sessionTimer)
        {
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
