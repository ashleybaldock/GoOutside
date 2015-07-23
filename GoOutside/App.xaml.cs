using System.Windows;
using GoOutside.Events;
using GoOutside.Properties;
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

            var interval = Settings.Default.PeriodBetweenBreaks.TotalMilliseconds;
            _SessionTimer = new SessionTimer(new SystemEventsWrapper(), new Period(2000));

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _NotifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            var notifyIconViewModel = new NotifyIconViewModel(_SessionTimer, _NotifyIcon);

            _NotifyIcon.DataContext = notifyIconViewModel;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _NotifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}