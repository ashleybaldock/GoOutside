using System.Windows;
using GoOutside.Views;
using Hardcodet.Wpf.TaskbarNotification;

namespace GoOutside
{
    public partial class App
    {
        private TaskbarIcon _NotifyIcon;
        private PopUp _PopUp;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _PopUp = (PopUp)FindResource("PopUp");

            _NotifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            var pomo = (Pomo)FindResource("Pomo");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _NotifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}