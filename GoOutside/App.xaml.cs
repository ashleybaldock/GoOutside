using System.Windows;
using GoOutside.Events;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;

namespace GoOutside
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon NotifyIcon;

        private SessionTimer SessionTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            NotifyIcon = (TaskbarIcon) FindResource("NotifyIcon");

            SessionTimer = new SessionTimer(new SystemEventsWrapper());
            SystemEvents.SessionSwitch += SessionTimer.SessionSwitchDelegate;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            NotifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}