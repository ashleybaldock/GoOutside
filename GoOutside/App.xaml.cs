using System.Windows;
using GoOutside.Events;
using GoOutside.Scheduling;
using Hardcodet.Wpf.TaskbarNotification;

namespace GoOutside
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _NotifyIcon;

        private SessionTimer _SessionTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _NotifyIcon = (TaskbarIcon) FindResource("NotifyIcon");

            _SessionTimer = new SessionTimer(new SystemEventsWrapper(), new Period());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _NotifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}