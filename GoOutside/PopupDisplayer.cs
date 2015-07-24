using System.Windows;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;

namespace GoOutside
{
    class PopupDisplayer : IPopupDisplayer
    {
        private readonly PopUp _PopUp;

        private static TaskbarIcon NotifyIcon
        {
            get { return (TaskbarIcon) Application.Current.FindResource("NotifyIcon"); }
        }

        public PopupDisplayer()
        {
            _PopUp = new PopUp();
        }

        public bool CanShow()
        {
            var notifyIcon = NotifyIcon;
            return notifyIcon != null && (notifyIcon.CustomBalloon == null || !notifyIcon.CustomBalloon.IsOpen);
        }

        public bool CanHide()
        {
            var notifyIcon = NotifyIcon;
            return notifyIcon != null && notifyIcon.CustomBalloon != null && notifyIcon.CustomBalloon.IsOpen;
        }

        public void Show()
        {
            Hide();
            var notifyIcon = NotifyIcon;
            if (notifyIcon != null)
            {
                //_PopUp.
                notifyIcon.ShowCustomBalloon(new PopUp(), PopupAnimation.None, null);
            }
        }

        public void Hide()
        {
            var notifyIcon = NotifyIcon;
            if (notifyIcon != null)
                notifyIcon.CloseBalloon();
        }
    }
}