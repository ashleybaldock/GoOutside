using System.Windows;
using System.Windows.Input;

namespace GoOutside.ViewModels
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
